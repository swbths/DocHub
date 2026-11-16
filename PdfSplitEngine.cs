using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace DocHub
{
    /// <summary>PDF 拆分方式：每页一个文件，或每 N 页一个文件。</summary>
    public enum PdfSplitMode
    {
        EveryPage = 0,
        EveryNPages = 1
    }

    public sealed record PdfSplitOptions
    {
        public PdfSplitMode Mode { get; init; } = PdfSplitMode.EveryPage;

        /// <summary>每份页数，仅在 EveryNPages 模式下生效。</summary>
        public int PagesPerFile { get; init; } = 1;

        public int ResolvePagesPerFile() =>
            Mode == PdfSplitMode.EveryPage ? 1 : Math.Clamp(PagesPerFile, 1, 500);
    }

    public sealed record PdfSplitFileResult(
        string Source, IReadOnlyList<string> Outputs, string? Error)
    {
        public bool Succeeded => Error == null && Outputs.Count > 0;
    }

    public sealed record PdfSplitProgress(
        int FileIndex, int FileCount, string FileName, int ChunkIndex, int ChunkCount, int Percent);

    public sealed record PdfSplitResult(
        IReadOnlyList<PdfSplitFileResult> Files,
        int OutputCount,
        int FailedCount,
        bool Cancelled,
        TimeSpan Elapsed)
    {
        public int SucceededCount => Files.Count(file => file.Error == null);
    }

    public sealed class PdfSplitException : Exception
    {
        public PdfSplitException(string message) : base(message) { }
    }

    /// <summary>PDF 拆分：逐页复制到新文档，保留原页面内容；源 PDF 始终只读。</summary>
    public static class PdfSplitEngine
    {
        public const int MinPagesPerFile = 1;
        public const int MaxPagesPerFile = 500;

        public static PdfSplitResult Run(
            IReadOnlyList<string> files,
            string outputFolder,
            PdfSplitOptions options,
            IProgress<PdfSplitProgress>? progress,
            CancellationToken token,
            IReadOnlyDictionary<string, string>? passwords)
        {
            if (files == null || files.Count == 0)
                throw new PdfSplitException(Loc.T(Txt.SplitNoFiles));
            if (string.IsNullOrWhiteSpace(outputFolder))
                throw new PdfSplitException(Loc.T(Txt.SplitNeedFolder));

            options ??= new PdfSplitOptions();
            int pagesPerFile = options.ResolvePagesPerFile();
            Directory.CreateDirectory(outputFolder);

            var results = new List<PdfSplitFileResult>(files.Count);
            var usedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var stopwatch = Stopwatch.StartNew();
            bool cancelled = false;
            int outputCount = 0;

            for (int fileIndex = 0; fileIndex < files.Count; fileIndex++)
            {
                if (token.IsCancellationRequested)
                {
                    cancelled = true;
                    break;
                }

                string file = files[fileIndex];
                string fileName = Path.GetFileName(file);
                var outputs = new List<string>();
                string? error = null;

                try
                {
                    if (!File.Exists(file))
                        throw new PdfSplitException(Loc.T(Txt.SplitUnreadable, fileName));

                    string? password = null;
                    passwords?.TryGetValue(file, out password);
                    using PdfDocument input = PdfReader.Open(
                        file, password ?? string.Empty, PdfDocumentOpenMode.Import);
                    int pageCount = input.PageCount;
                    if (pageCount <= 0)
                        throw new PdfSplitException(Loc.T(Txt.SplitUnreadable, fileName));

                    int chunkCount = (pageCount + pagesPerFile - 1) / pagesPerFile;
                    string stem = SafeStem(Path.GetFileNameWithoutExtension(file));
                    string fileFolder = ResolveFileFolder(outputFolder, stem);
                    Directory.CreateDirectory(fileFolder);

                    for (int chunk = 0; chunk < chunkCount; chunk++)
                    {
                        if (token.IsCancellationRequested)
                        {
                            cancelled = true;
                            break;
                        }

                        int start = chunk * pagesPerFile + 1;
                        int end = Math.Min(start + pagesPerFile - 1, pageCount);
                        string outputPath = BuildUniquePath(
                            fileFolder, BuildChunkName(stem, start, end, pageCount), usedNames);

                        using (var output = new PdfDocument())
                        {
                            output.Info.Title = Path.GetFileNameWithoutExtension(file);
                            for (int page = start; page <= end; page++)
                                output.AddPage(input.Pages[page - 1]);

                            using var stream = new FileStream(
                                outputPath, FileMode.Create, FileAccess.Write, FileShare.None);
                            output.Save(stream, false);
                        }

                        outputs.Add(outputPath);
                        outputCount++;
                        progress?.Report(new PdfSplitProgress(
                            fileIndex, files.Count, fileName, chunk + 1, chunkCount,
                            PercentFor(fileIndex, files.Count, chunk + 1, chunkCount)));
                    }
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                }

                results.Add(new PdfSplitFileResult(file, outputs, error));
                if (cancelled) break;
            }

            stopwatch.Stop();
            int failed = results.Count(result => result.Error != null);
            return new PdfSplitResult(results, outputCount, failed, cancelled, stopwatch.Elapsed);
        }

        /// <summary>按页数计算将生成的拆分文件数。</summary>
        public static int CountOutputs(int pageCount, PdfSplitOptions options)
        {
            if (pageCount <= 0) return 0;
            int pagesPerFile = (options ?? new PdfSplitOptions()).ResolvePagesPerFile();
            return (pageCount + pagesPerFile - 1) / pagesPerFile;
        }

        /// <summary>拆分后的页码标签，例如 p001 或 p001-005。</summary>
        public static string PageLabel(int start, int end, int totalPages)
        {
            int digits = Math.Max(3, Math.Max(1, totalPages).ToString().Length);
            string first = start.ToString("D" + digits);
            return end <= start ? "p" + first : $"p{first}-{end.ToString("D" + digits)}";
        }

        public static string SafeStem(string? name)
        {
            string stem = (name ?? string.Empty).Trim();
            foreach (char invalid in Path.GetInvalidFileNameChars())
                stem = stem.Replace(invalid, '_');
            return stem.Length == 0 ? Loc.T(Txt.DefaultDocName) : stem;
        }

        private static string BuildChunkName(string stem, int start, int end, int totalPages) =>
            $"{stem}_{PageLabel(start, end, totalPages)}";

        /// <summary>每个源 PDF 的输出放入同名子文件夹；已有同名文件夹时直接复用。</summary>
        private static string ResolveFileFolder(string root, string stem)
        {
            string candidate = Path.Combine(root, stem);
            int index = 2;
            while (File.Exists(candidate))
                candidate = Path.Combine(root, $"{stem} ({index++})");
            return candidate;
        }

        private static string BuildUniquePath(
            string folder, string baseName, HashSet<string> usedNames)
        {
            string candidate = Path.Combine(folder, baseName + ".pdf");
            int index = 2;
            while (File.Exists(candidate) || !usedNames.Add(candidate))
                candidate = Path.Combine(folder, $"{baseName} ({index++}).pdf");
            return candidate;
        }

        private static int PercentFor(int fileIndex, int fileCount, int chunk, int chunkCount)
        {
            if (fileCount <= 0) return 0;
            double value = (fileIndex + (double)chunk / Math.Max(1, chunkCount)) / fileCount * 100d;
            return Math.Clamp((int)Math.Round(value), 0, 100);
        }
    }
}
