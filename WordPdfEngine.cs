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
    /// <summary>Word 转 PDF 进度：FileIndex 从 0 开始，-1 表示正在合并页面。</summary>
    public sealed record WordPdfProgress(int Percent, int FileIndex, int FileCount, string FileName);

    public sealed class WordPdfBatchResult
    {
        public WordPdfBatchResult(
            IReadOnlyList<string> outputPaths, int pageCount, TimeSpan elapsed,
            bool merged, bool cancelled)
        {
            OutputPaths = outputPaths;
            PageCount = pageCount;
            Elapsed = elapsed;
            Merged = merged;
            Cancelled = cancelled;
        }

        public IReadOnlyList<string> OutputPaths { get; }
        public int PageCount { get; }
        public TimeSpan Elapsed { get; }
        public bool Merged { get; }
        public bool Cancelled { get; }
    }

    public sealed class WordPdfException : Exception
    {
        public WordPdfException(string message) : base(message) { }
        public WordPdfException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// Word 转 PDF 引擎：复用合并页的 Word COM / LibreOffice 导出能力，
    /// 支持逐份输出（默认）和合并为单个 PDF，并可在输出 PDF 上叠加水印。
    /// </summary>
    public static class WordPdfEngine
    {
        public static bool IsAvailable => MergeEngine.IsWordAvailable;

        public static WordConverter Converter => MergeEngine.DetectedConverter;

        public static WordPdfBatchResult Run(
            IReadOnlyList<string> inputFiles,
            string outputFolder,
            bool mergeIntoOne,
            string? mergedStem,
            WatermarkSpec? watermark,
            IProgress<WordPdfProgress>? progress,
            IReadOnlyDictionary<string, string>? passwords = null,
            CancellationToken cancellationToken = default)
        {
            if (inputFiles.Count == 0)
                throw new WordPdfException(Loc.T(Txt.WordPdfNoFiles));

            foreach (string file in inputFiles)
            {
                if (!File.Exists(file))
                    throw new WordPdfException(Loc.T(Txt.MergeFileMissing, file));
                if (!MergeEngine.IsWordFile(file))
                    throw new WordPdfException(Loc.T(Txt.WordPdfNonWord, Path.GetFileName(file)));
            }

            WordConverter converter = MergeEngine.SelectConverter(inputFiles);
            if (converter == WordConverter.None)
                throw new WordPdfException(Loc.T(Txt.WordPdfNoEngine));

            string folder = Path.GetFullPath(outputFolder);
            Directory.CreateDirectory(folder);

            string tempDir = MergeEngine.CreateWorkDirectory();
            object? word = null;
            object? documents = null;
            var outputs = new List<string>();
            var stopwatch = Stopwatch.StartNew();
            bool cancelled = false;
            int pageCount = 0;

            try
            {
                if (mergeIntoOne)
                {
                    var tempPdfs = new string[inputFiles.Count];
                    for (int i = 0; i < inputFiles.Count; i++)
                    {
                        if (cancellationToken.IsCancellationRequested) { cancelled = true; break; }

                        string source = inputFiles[i];
                        string tempPdf = Path.Combine(tempDir, $"word_{i:D4}.pdf");
                        ExportWord(ref word, ref documents, converter, source, tempPdf,
                            ResolvePassword(passwords, source));

                        tempPdfs[i] = tempPdf;
                        Report(progress, (int)((i + 1) * 85.0 / inputFiles.Count),
                            i, inputFiles.Count, Path.GetFileName(source));
                    }

                    if (!cancelled)
                    {
                        Report(progress, 90, -1, inputFiles.Count, string.Empty);
                        string stem = SanitizeStem(mergedStem);
                        if (stem.Length == 0)
                            stem = SanitizeStem(Path.GetFileNameWithoutExtension(inputFiles[0]));
                        if (stem.Length == 0) stem = Loc.T(Txt.DefaultDocName);

                        string target = UniqueFilePath(Path.Combine(folder, stem + ".pdf"));
                        outputs.Add(target);
                        using (var output = new PdfDocument())
                        {
                            var sources = new List<PdfDocument>(tempPdfs.Length);
                            try
                            {
                                foreach (string pdfPath in tempPdfs)
                                {
                                    PdfDocument input = PdfReader.Open(
                                        pdfPath, PdfDocumentOpenMode.Import);
                                    sources.Add(input);
                                    foreach (PdfPage page in input.Pages)
                                    {
                                        output.AddPage(page);
                                        pageCount++;
                                    }
                                }

                                if (pageCount == 0)
                                    throw new WordPdfException(Loc.T(Txt.MergeNoPages));

                                if (watermark != null)
                                    WatermarkPainter.Apply(output, watermark, tempDir);

                                output.Save(target);
                            }
                            finally
                            {
                                foreach (PdfDocument source in sources) source.Dispose();
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < inputFiles.Count; i++)
                    {
                        if (cancellationToken.IsCancellationRequested) { cancelled = true; break; }

                        string source = inputFiles[i];
                        string stem = SanitizeStem(Path.GetFileNameWithoutExtension(source));
                        if (stem.Length == 0) stem = Loc.T(Txt.DefaultDocName);
                        string target = UniqueFilePath(Path.Combine(folder, stem + ".pdf"));
                        outputs.Add(target);

                        if (watermark == null)
                        {
                            ExportWord(ref word, ref documents, converter, source, target,
                                ResolvePassword(passwords, source));
                        }
                        else
                        {
                            string tempPdf = Path.Combine(tempDir, $"word_{i:D4}.pdf");
                            ExportWord(ref word, ref documents, converter, source, tempPdf,
                                ResolvePassword(passwords, source));
                            ApplyWatermark(tempPdf, target, watermark, tempDir);
                        }

                        if (!File.Exists(target) || new FileInfo(target).Length == 0)
                            throw new WordPdfException(Loc.T(Txt.WordPdfWriteError, target));

                        Report(progress, (int)((i + 1) * 100.0 / inputFiles.Count),
                            i, inputFiles.Count, Path.GetFileName(source));
                    }
                }

                stopwatch.Stop();
                if (cancelled)
                {
                    foreach (string path in outputs) TryDeleteFile(path);
                    outputs.Clear();
                }

                return new WordPdfBatchResult(outputs, pageCount, stopwatch.Elapsed,
                    mergeIntoOne, cancelled);
            }
            catch (OperationCanceledException)
            {
                foreach (string path in outputs) TryDeleteFile(path);
                stopwatch.Stop();
                return new WordPdfBatchResult(new List<string>(), pageCount,
                    stopwatch.Elapsed, mergeIntoOne, true);
            }
            catch (WordPdfException)
            {
                foreach (string path in outputs) TryDeleteFile(path);
                throw;
            }
            catch (IOException ex)
            {
                foreach (string path in outputs) TryDeleteFile(path);
                throw new WordPdfException(Loc.T(Txt.WordPdfWriteError, ex.Message), ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                foreach (string path in outputs) TryDeleteFile(path);
                throw new WordPdfException(Loc.T(Txt.WordPdfWriteError, ex.Message), ex);
            }
            catch (Exception ex)
            {
                foreach (string path in outputs) TryDeleteFile(path);
                throw new WordPdfException(Loc.T(Txt.WordPdfFailGeneric, ex.Message), ex);
            }
            finally
            {
                MergeEngine.QuitWord(ref word, ref documents);
                MergeEngine.TryDeleteDirectory(tempDir);
            }
        }

        private static void ExportWord(
            ref object? word, ref object? documents, WordConverter converter,
            string source, string target, string? password)
        {
            if (converter == WordConverter.MicrosoftWord || converter == WordConverter.WpsOffice)
            {
                MergeEngine.EnsureOffice(ref word, ref documents, converter);
                MergeEngine.ExportWordToPdf(word!, documents!, source, target, password);
            }
            else
            {
                MergeEngine.ExportWordWithLibreOffice(source, target);
            }
        }

        private static void ApplyWatermark(
            string sourcePdf, string targetPdf, WatermarkSpec watermark, string tempDir)
        {
            using var input = PdfReader.Open(sourcePdf, PdfDocumentOpenMode.Import);
            using var output = new PdfDocument();
            foreach (PdfPage page in input.Pages)
                output.AddPage(page);
            WatermarkPainter.Apply(output, watermark, tempDir);
            output.Save(targetPdf);
        }

        private static string? ResolvePassword(
            IReadOnlyDictionary<string, string>? passwords, string source)
        {
            return passwords != null && passwords.TryGetValue(source, out string? password)
                ? password
                : null;
        }

        private static void Report(
            IProgress<WordPdfProgress>? progress, int percent, int index, int count, string file)
        {
            progress?.Report(new WordPdfProgress(
                Math.Clamp(percent, 0, 100), index, count, file));
        }

        internal static string SanitizeStem(string? text)
        {
            string stem = text ?? string.Empty;
            foreach (char invalid in Path.GetInvalidFileNameChars())
                stem = stem.Replace(invalid, '_');
            stem = stem.Trim();
            if (stem.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                stem = stem[..^4].TrimEnd();
            return stem.TrimEnd('.');
        }

        internal static string UniqueFilePath(string desired)
        {
            if (!File.Exists(desired)) return desired;

            string folder = Path.GetDirectoryName(desired) ?? Environment.CurrentDirectory;
            string stem = Path.GetFileNameWithoutExtension(desired);
            string extension = Path.GetExtension(desired);

            for (int index = 1; index < 1000; index++)
            {
                string candidate = Path.Combine(folder, $"{stem}-{index}{extension}");
                if (!File.Exists(candidate)) return candidate;
            }

            return Path.Combine(folder, $"{stem}-{DateTime.Now:yyyyMMddHHmmss}{extension}");
        }

        private static void TryDeleteFile(string path)
        {
            try { if (File.Exists(path)) File.Delete(path); }
            catch { /* 清理失败不影响结果 */ }
        }
    }
}
