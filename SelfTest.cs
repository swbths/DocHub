using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace DocHub
{
    /// <summary>
    /// 无界面的自检入口（--selftest）：验证皮肤对比度与图片转 PDF 的页面尺寸，
    /// 只在控制台/日志中输出结果，不创建任何窗口或对话框。
    /// </summary>
    internal static class SelfTest
    {
        public static int Run()
        {
            Loc.SetLanguage(Language.ZhCn);
            string root = Path.Combine(Path.GetTempPath(), "DocHub_selftest_" + Guid.NewGuid().ToString("N"));
            var report = new StringBuilder();
            int failures = 0;

            report.AppendLine("DocHub selftest " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            report.AppendLine();

            try
            {
                Directory.CreateDirectory(root);
                failures += CheckSkinContrast(report);
                failures += CheckSkinCatalog(report);
                failures += CheckMarqueeHitTest(report);
                failures += CheckSettingsDefaults(report);
                failures += CheckQuotes(report);
                failures += CheckImagePdf(report, root);
                failures += CheckImagePdfSeparate(report, root);
                failures += CheckPdfSourceIntegrity(report, root);
                failures += CheckPageSelection(report, root);
                failures += CheckWatermarkOutputs(report, root);
                failures += CheckIcons(report);
                failures += CheckSettingsLanguageReload(report);
                failures += CheckBackdropCacheRecovery(report);
            }
            catch (Exception ex)
            {
                failures++;
                report.AppendLine("[FAIL] 未处理的异常: " + ex);
            }
            finally
            {
                try { Directory.Delete(root, true); } catch { /* 临时文件留给系统清理 */ }
            }

            report.AppendLine();
            report.AppendLine(failures == 0 ? "RESULT: PASS" : $"RESULT: FAIL ({failures})");
            string logPath = Path.Combine(Path.GetTempPath(), "DocHub-selftest.log");
            try { File.WriteAllText(logPath, report.ToString(), Encoding.UTF8); } catch { /* 忽略 */ }
            try { Console.WriteLine(report.ToString()); } catch { /* WinExe 无控制台 */ }
            return failures == 0 ? 0 : 1;
        }

        // ---------------- 皮肤对比度 ----------------

        private static int CheckSkinContrast(StringBuilder report)
        {
            int failures = 0;
            report.AppendLine("[皮肤] 左侧功能栏标题/副标题对比度（WCAG）");

            foreach (SkinPalette skin in SkinPalette.All)
            {
                double titleStart = Contrast(skin.HeaderText, skin.HeaderStart);
                double titleEnd = Contrast(skin.HeaderText, skin.HeaderEnd);
                double subtitleStart = Contrast(skin.HeaderSubtitle, skin.HeaderStart);
                double subtitleEnd = Contrast(skin.HeaderSubtitle, skin.HeaderEnd);

                double title = Math.Min(titleStart, titleEnd);
                double subtitle = Math.Min(subtitleStart, subtitleEnd);
                bool ok = title >= 4.5 && subtitle >= 3.0;
                if (!ok) failures++;

                report.AppendLine(
                    $"  [{(ok ? "OK" : "FAIL")}] {skin.Id}: title {title:0.00}, subtitle {subtitle:0.00}");
            }

            report.AppendLine();
            report.AppendLine("[皮肤] 正文/次要/弱化文字与强调按钮对比度");
            foreach (SkinPalette skin in SkinPalette.All)
            {
                double primary = Contrast(skin.TextPrimary, skin.Card);
                double secondary = Contrast(skin.TextSecondary, skin.Card);
                double muted = Contrast(skin.TextMuted, skin.Card);
                double accentText = Contrast(skin.AccentText, skin.Accent);
                double selected = Contrast(skin.TextPrimary, skin.ListSelected);

                bool ok = primary >= 7.0 && secondary >= 4.5 && muted >= 2.5 &&
                          accentText >= 3.0 && selected >= 4.5;
                if (!ok) failures++;

                report.AppendLine(
                    $"  [{(ok ? "OK" : "FAIL")}] {skin.Id}: 正文 {primary:0.00}, 次要 {secondary:0.00}, " +
                    $"弱化 {muted:0.00}, 强调按钮 {accentText:0.00}, 选中行 {selected:0.00}");
            }

            return failures;
        }

        // ---------------- 皮肤目录 ----------------

        private static int CheckSkinCatalog(StringBuilder report)
        {
            int failures = 0;
            report.AppendLine();
            report.AppendLine("[皮肤] 目录与风格");

            string[] expected =
            {
                SkinPalette.ClassicBlueId, SkinPalette.NightId, SkinPalette.CloudWhiteId,
                SkinPalette.SkeuoId, SkinPalette.NeumorphId
            };
            var ids = SkinPalette.All.Select(s => s.Id).ToList();
            bool setOk = ids.Count == expected.Length &&
                         expected.All(id => ids.Contains(id)) &&
                         ids.Distinct().Count() == ids.Count;
            if (!setOk) failures++;
            report.AppendLine($"  [{(setOk ? "OK" : "FAIL")}] 皮肤数量 {ids.Count}：{string.Join(" / ", ids)}");

            bool legacyOk = ids.All(id => !id.Contains("netease", StringComparison.OrdinalIgnoreCase)) &&
                            !SkinPalette.Exists("netease") &&
                            SkinPalette.FromId("netease").Id == SkinPalette.ClassicBlueId;
            if (!legacyOk) failures++;
            report.AppendLine($"  [{(legacyOk ? "OK" : "FAIL")}] 网易云皮肤已移除，未知皮肤回退经典蓝");

            bool hudOk = !SkinPalette.Exists("hud") &&
                         SkinPalette.FromId("hud").Id == SkinPalette.ClassicBlueId;
            if (!hudOk) failures++;
            report.AppendLine($"  [{(hudOk ? "OK" : "FAIL")}] HUD 科幻皮肤已移除，旧配置回退经典蓝");

            bool terminalOk = !SkinPalette.Exists("terminal") &&
                              SkinPalette.FromId("terminal").Id == SkinPalette.ClassicBlueId;
            if (!terminalOk) failures++;
            report.AppendLine($"  [{(terminalOk ? "OK" : "FAIL")}] 终端风皮肤已移除，旧配置回退经典蓝");

            var surfaces = new Dictionary<string, SkinSurface>
            {
                [SkinPalette.SkeuoId] = SkinSurface.Skeuomorphic,
                [SkinPalette.NeumorphId] = SkinSurface.Neumorphic
            };
            foreach (var pair in surfaces)
            {
                SkinPalette skin = SkinPalette.FromId(pair.Key);
                bool ok = skin.Surface == pair.Value &&
                          !string.IsNullOrWhiteSpace(skin.Name) &&
                          !string.IsNullOrWhiteSpace(skin.Description);
                if (!ok) failures++;
                report.AppendLine(
                    $"  [{(ok ? "OK" : "FAIL")}] {pair.Key}: Surface={skin.Surface}, {skin.Name} / {skin.Description}");
            }

            return failures;
        }

        // ---------------- 框选命中 ----------------

        private static int CheckMarqueeHitTest(StringBuilder report)
        {
            report.AppendLine();
            report.AppendLine("[列表] 框选命中与反向拖动");

            var rows = new List<Rectangle>
            {
                new(10, 3, 500, 54),
                new(10, 65, 500, 54),
                new(10, 127, 500, 54),
                new(10, 189, 500, 54)
            };

            Rectangle forward = DocumentListBox.NormalizeMarqueeRect(new Point(50, 20), new Point(300, 120));
            List<int> hits = DocumentListBox.MarqueeHits(rows, forward);
            bool forwardOk = hits.SequenceEqual(new[] { 0, 1 });

            Rectangle backward = DocumentListBox.NormalizeMarqueeRect(new Point(300, 120), new Point(50, 20));
            bool backwardOk = backward == forward;

            // 从左侧 10px 留白起框，拖入行卡片后才命中
            Rectangle gutter = DocumentListBox.NormalizeMarqueeRect(new Point(3, 10), new Point(6, 40));
            bool gutterOk = DocumentListBox.MarqueeHits(rows, gutter).Count == 0;
            Rectangle enter = DocumentListBox.NormalizeMarqueeRect(new Point(3, 10), new Point(20, 40));
            bool enterOk = DocumentListBox.MarqueeHits(rows, enter).SequenceEqual(new[] { 0 });

            bool ok = forwardOk && backwardOk && gutterOk && enterOk;
            if (!ok) report.AppendLine($"  明细：forward={string.Join(",", hits)} backward={backwardOk} gutter={gutterOk} enter={enterOk}");
            if (!ok) return 1;

            report.AppendLine("  [OK] 正向/反向拖动、左右留白与行卡片命中一致");
            return 0;
        }

        // ---------------- 设置默认值 ----------------

        private static int CheckSettingsDefaults(StringBuilder report)
        {
            report.AppendLine();
            report.AppendLine("[设置] 默认值");

            var defaults = new AppSettings();
            bool convertOk = defaults.ConvertFormat == ConvertImageFormat.Jpeg &&
                             defaults.ConvertPpi == 300 &&
                             defaults.ConvertJpegQuality == 100 &&
                             defaults.ConvertPageMode == ConvertPageSelection.All;
            report.AppendLine(
                $"  [{(convertOk ? "OK" : "FAIL")}] PDF 转图片默认：{defaults.ConvertFormat} / {defaults.ConvertPpi}PPI / 质量 {defaults.ConvertJpegQuality}% / 页码 {defaults.ConvertPageMode}");

            bool soundOk = !defaults.PlaySound;
            report.AppendLine(
                $"  [{(soundOk ? "OK" : "FAIL")}] 完成提示音默认关闭：PlaySound={defaults.PlaySound}");

            bool watermarkOk = !defaults.WatermarkAnyEnabled;
            report.AppendLine(
                $"  [{(watermarkOk ? "OK" : "FAIL")}] 四个水印开关默认关闭：合并={defaults.WatermarkEnabled}, PDF转图片={defaults.WatermarkForPdfImage}, 图片转PDF={defaults.WatermarkForImagePdf}, Word转PDF={defaults.WatermarkForWordPdf}");

            AppSettings wordClone = defaults.Clone();
            bool wordPdfOk = !defaults.WordPdfMergeIntoOne &&
                             string.IsNullOrEmpty(defaults.LastWordPdfFolder) &&
                             !wordClone.WordPdfMergeIntoOne &&
                             string.IsNullOrEmpty(wordClone.LastWordPdfFolder);
            report.AppendLine(
                $"  [{(wordPdfOk ? "OK" : "FAIL")}] Word 转 PDF 默认逐份输出且可克隆：" +
                $"MergeIntoOne={defaults.WordPdfMergeIntoOne}, LastFolder=\"{defaults.LastWordPdfFolder}\"");

            string sanitized = WordPdfEngine.SanitizeStem("a/b:c.pdf");
            bool wordNameOk = sanitized == "a_b_c" &&
                              WordPdfEngine.SanitizeStem("  报告.pdf  ") == "报告";
            report.AppendLine(
                $"  [{(wordNameOk ? "OK" : "FAIL")}] Word 输出名清理：\"{sanitized}\"");

            bool wpsOk = MergeEngine.IsWordFile("示例.WPS") &&
                         MergeEngine.IsWordFile("模板.wpt") &&
                         MergeEngine.IsWpsFile("模板.wpt") &&
                         !MergeEngine.IsWpsFile("示例.docx") &&
                         MergeEngine.IsWordFile("示例.docx");
            WordConverter wpsConverter = MergeEngine.SelectConverter(new[] { "示例.wps" });
            bool wpsRouteOk = wpsConverter != WordConverter.MicrosoftWord &&
                              (!MergeEngine.HasWpsCom() || wpsConverter == WordConverter.WpsOffice);
            report.AppendLine(
                $"  [{(wpsOk ? "OK" : "FAIL")}] WPS 格式识别：.wps/.wpt 纳入 Word 支持且不误判 Word 文档");
            report.AppendLine(
                $"  [{(wpsRouteOk ? "OK" : "FAIL")}] WPS 文件转换引擎：{wpsConverter}（WPS COM={MergeEngine.HasWpsCom()}）");

            bool quotesOk = defaults.ShowQuotes && defaults.Clone().ShowQuotes;
            report.AppendLine(
                $"  [{(quotesOk ? "OK" : "FAIL")}] 名言卡片默认显示且可随设置克隆：ShowQuotes={defaults.ShowQuotes}");

            AppSettings quoteClone = defaults.Clone();
            bool quoteRefreshOk = defaults.QuoteAutoRefresh &&
                                   defaults.QuoteRefreshSeconds == 30 &&
                                   quoteClone.QuoteAutoRefresh &&
                                   quoteClone.QuoteRefreshSeconds == 30;
            report.AppendLine(
                $"  [{(quoteRefreshOk ? "OK" : "FAIL")}] 名言自动换句默认开启、间隔 30 秒且可克隆：" +
                $"AutoRefresh={defaults.QuoteAutoRefresh}, Seconds={defaults.QuoteRefreshSeconds}");

            return (convertOk ? 0 : 1) + (soundOk ? 0 : 1) + (watermarkOk ? 0 : 1) +
                   (wordPdfOk ? 0 : 1) + (wordNameOk ? 0 : 1) +
                   (quotesOk ? 0 : 1) + (quoteRefreshOk ? 0 : 1);
        }

        // ---------------- 名言库 ----------------

        private static int CheckQuotes(StringBuilder report)
        {
            int failures = 0;
            report.AppendLine();
            report.AppendLine("[名言] 六语言语录库");

            int total = 0;
            foreach (Language language in Loc.All)
            {
                IReadOnlyList<Quote> pool = Quotes.For(language);
                bool ok = pool.Count >= 50;
                var texts = new HashSet<string>(StringComparer.Ordinal);
                foreach (Quote quote in pool)
                {
                    if (string.IsNullOrWhiteSpace(quote.Text) ||
                        string.IsNullOrWhiteSpace(quote.Source) ||
                        !quote.Source.StartsWith("—", StringComparison.Ordinal))
                        ok = false;
                    if (!texts.Add(quote.Text.Trim())) ok = false;
                }

                total += pool.Count;
                if (!ok) failures++;
                report.AppendLine(
                    $"  [{(ok ? "OK" : "FAIL")}] {language}: {pool.Count} 句，示例「{Quotes.At(language, 0).Text}」{Quotes.At(language, 0).Source}");
            }

            bool totalOk = total >= 300;
            if (!totalOk) failures++;
            report.AppendLine(
                $"  [{(totalOk ? "OK" : "FAIL")}] 合计 {total} 句（要求 ≥ 300，同一语言内不重复）");

            // 随机播放：启动抽样应有多句不同，连续换句不与上一句重复
            bool randomOk = true;
            int index = -1;
            var covered = new HashSet<int>();
            for (int i = 0; i < 40; i++)
            {
                int next = Quotes.NextRandomIndex(Language.ZhCn, index);
                if (next < 0 || next == index) randomOk = false;
                index = next;
                covered.Add(next);
            }

            var startupTexts = new HashSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < 12; i++)
            {
                int startup = Quotes.NextRandomIndex(Language.ZhCn, -1);
                startupTexts.Add(Quotes.At(Language.ZhCn, startup).Text);
            }

            if (covered.Count < 20 || startupTexts.Count < 4) randomOk = false;
            if (!randomOk) failures++;
            report.AppendLine(
                $"  [{(randomOk ? "OK" : "FAIL")}] 启动与换句均为随机播放：" +
                $"启动抽样 {startupTexts.Count}/12 句不同，连续 40 次换句无重复且覆盖 {covered.Count} 句");

            return failures;
        }

        private static double Contrast(Color foreground, Color background)
        {
            double l1 = RelativeLuminance(foreground);
            double l2 = RelativeLuminance(background);
            double lighter = Math.Max(l1, l2);
            double darker = Math.Min(l1, l2);
            return (lighter + 0.05) / (darker + 0.05);
        }

        private static double RelativeLuminance(Color color)
        {
            static double Channel(byte value)
            {
                double c = value / 255.0;
                return c <= 0.04045 ? c / 12.92 : Math.Pow((c + 0.055) / 1.055, 2.4);
            }

            return 0.2126 * Channel(color.R) + 0.7152 * Channel(color.G) + 0.0722 * Channel(color.B);
        }

        // ---------------- 图片转 PDF ----------------

        private static int CheckImagePdf(StringBuilder report, string root)
        {
            int failures = 0;
            string landscape = Path.Combine(root, "landscape.png");
            string portrait = Path.Combine(root, "portrait.jpg");
            string square = Path.Combine(root, "square.bmp");

            CreateImage(landscape, 400, 300, ImageFormat.Png, Color.OrangeRed);
            CreateImage(portrait, 300, 450, ImageFormat.Jpeg, Color.SteelBlue);
            CreateImage(square, 256, 256, ImageFormat.Bmp, Color.SeaGreen);

            report.AppendLine();
            report.AppendLine("[图片转 PDF] 页面尺寸");

            failures += CheckPageSize(report, root, "A4 纵向", ImagePdfPageSize.A4,
                ImagePdfOrientation.Portrait, 10, 595.28, 841.89, landscape);
            failures += CheckPageSize(report, root, "A3 横向", ImagePdfPageSize.A3,
                ImagePdfOrientation.Landscape, 10, 1190.55, 841.89, landscape);
            failures += CheckPageSize(report, root, "A5 纵向", ImagePdfPageSize.A5,
                ImagePdfOrientation.Portrait, 10, 419.53, 595.28, portrait);
            failures += CheckPageSize(report, root, "B5 纵向", ImagePdfPageSize.B5,
                ImagePdfOrientation.Portrait, 10, 498.90, 708.66, portrait);
            failures += CheckPageSize(report, root, "Letter 纵向", ImagePdfPageSize.Letter,
                ImagePdfOrientation.Portrait, 10, 612.0, 792.0, portrait);
            failures += CheckPageSize(report, root, "A4 自动方向（横幅图）", ImagePdfPageSize.A4,
                ImagePdfOrientation.Auto, 0, 841.89, 595.28, landscape);

            // 自适应纸张：400×300 @96dpi = 300×225 pt
            string autoFile = Path.Combine(root, "auto.pdf");
            failures += CheckSingle(report, "自适应纸张", ImagePdfPageSize.Auto,
                ImagePdfOrientation.Auto, 0, new[] { landscape }, autoFile, 300, 225);

            report.AppendLine();
            report.AppendLine("[图片转 PDF] 批量与取消");
            string batchFile = Path.Combine(root, "batch.pdf");
            failures += CheckBatch(report, new[] { landscape, portrait, square }, batchFile, 3);

            string cancelFile = Path.Combine(root, "cancel.pdf");
            var cts = new CancellationTokenSource();
            cts.Cancel();
            try
            {
                ImagePdfResult result = ImagePdfEngine.Run(new[] { landscape }, cancelFile,
                    new ImagePdfOptions(), null, cts.Token);
                bool ok = result.Cancelled && !File.Exists(cancelFile);
                if (!ok) failures++;
                report.AppendLine($"  [{(ok ? "OK" : "FAIL")}] 取消任务: cancelled={result.Cancelled}, outputExists={File.Exists(cancelFile)}");
            }
            catch (Exception ex)
            {
                failures++;
                report.AppendLine("  [FAIL] 取消任务抛出异常: " + ex.Message);
            }
            finally
            {
                cts.Dispose();
            }

            string broken = Path.Combine(root, "broken.png");
            File.WriteAllText(broken, "not an image");
            try
            {
                ImagePdfEngine.Run(new[] { broken }, Path.Combine(root, "broken.pdf"),
                    new ImagePdfOptions(), null, CancellationToken.None);
                failures++;
                report.AppendLine("  [FAIL] 损坏图片未抛出 ImagePdfException");
            }
            catch (ImagePdfException)
            {
                report.AppendLine("  [OK] 损坏图片抛出 ImagePdfException");
            }
            catch (Exception ex)
            {
                failures++;
                report.AppendLine("  [FAIL] 损坏图片异常类型错误: " + ex.GetType().Name);
            }

            return failures;
        }

        private static int CheckImagePdfSeparate(StringBuilder report, string root)
        {
            int failures = 0;
            report.AppendLine();
            report.AppendLine("[图片转 PDF] 逐张输出");

            string first = Path.Combine(root, "sep_first.png");
            string second = Path.Combine(root, "sep_second.jpg");
            CreateImage(first, 320, 240, ImageFormat.Png, Color.MediumPurple);
            CreateImage(second, 240, 320, ImageFormat.Jpeg, Color.DarkCyan);

            string folder = Path.Combine(root, "separate");
            Directory.CreateDirectory(folder);
            string[] outputs =
            {
                Path.Combine(folder, "自定义_001.pdf"),
                Path.Combine(folder, "自定义_002.pdf")
            };

            try
            {
                ImagePdfBatchResult result = ImagePdfEngine.RunSeparate(
                    new[] { first, second }, outputs,
                    new ImagePdfOptions { PageSize = ImagePdfPageSize.A4, MarginMm = 10 },
                    null, CancellationToken.None);

                bool ok = !result.Cancelled && result.FailedCount == 0 &&
                          result.OutputPaths.Count == 2 &&
                          File.Exists(outputs[0]) && File.Exists(outputs[1]);
                if (ok)
                {
                    using PdfDocument document = PdfReader.Open(outputs[0], PdfDocumentOpenMode.Import);
                    ok = document.PageCount == 1;
                }

                if (!ok) failures++;
                report.AppendLine(
                    $"  [{(ok ? "OK" : "FAIL")}] 逐张生成独立 PDF：成功 {result.OutputPaths.Count}，失败 {result.FailedCount}");
            }
            catch (Exception ex)
            {
                failures++;
                report.AppendLine("  [FAIL] 逐张生成独立 PDF: " + ex.Message);
            }

            string broken = Path.Combine(root, "sep_broken.png");
            File.WriteAllText(broken, "not an image");
            string brokenOut = Path.Combine(folder, "broken.pdf");
            string goodOut = Path.Combine(folder, "good.pdf");

            try
            {
                ImagePdfBatchResult result = ImagePdfEngine.RunSeparate(
                    new[] { broken, first }, new[] { brokenOut, goodOut },
                    new ImagePdfOptions(), null, CancellationToken.None);

                bool ok = !result.Cancelled && result.FailedCount == 1 &&
                          result.OutputPaths.Count == 1 &&
                          File.Exists(goodOut) && !File.Exists(brokenOut);
                if (!ok) failures++;
                report.AppendLine(
                    $"  [{(ok ? "OK" : "FAIL")}] 单张失败继续处理：成功 {result.OutputPaths.Count}，失败 {result.FailedCount}");
            }
            catch (Exception ex)
            {
                failures++;
                report.AppendLine("  [FAIL] 单张失败继续处理: " + ex.Message);
            }

            return failures;
        }

        // ---------------- PDF 转图片：源文件完整性 ----------------

        private static int CheckPdfSourceIntegrity(StringBuilder report, string root)
        {
            report.AppendLine();
            report.AppendLine("[PDF 转图片] 源文件完整性");

            try
            {
                string image = Path.Combine(root, "integrity_source.png");
                CreateImage(image, 480, 360, ImageFormat.Png, Color.Goldenrod);

                string pdf = Path.Combine(root, "integrity_source.pdf");
                using (var document = new PdfDocument())
                {
                    PdfPage page = document.AddPage();
                    using (XGraphics graphics = XGraphics.FromPdfPage(page))
                    {
                        using XImage source = XImage.FromFile(image);
                        graphics.DrawImage(source, 40, 40, 400, 300);
                        graphics.DrawRectangle(XPens.DarkBlue, 40, 380, 400, 80);
                    }
                    document.Save(pdf);
                }

                string before = FileHash(pdf);
                string output = Path.Combine(root, "integrity_output");
                Directory.CreateDirectory(output);

                PdfConvertResult result = PdfImageEngine.Run(
                    new[] { pdf }, output,
                    new PdfConvertOptions
                    {
                        Format = ConvertImageFormat.Png,
                        Ppi = 96,
                        CreateSubfolder = false,
                        Naming = new AppSettings { NamingMode = NamingMode.FirstDocument }
                    },
                    null, CancellationToken.None, null);

                string after = FileHash(pdf);
                bool ok = before == after &&
                          result.FailedCount == 0 &&
                          result.ImageCount == 1 &&
                          result.Files.Count == 1 &&
                          result.Files[0].Images.Count == 1 &&
                          File.Exists(result.Files[0].Images[0]);

                report.AppendLine(
                    $"  [{(ok ? "OK" : "FAIL")}] 转换前后源 PDF 哈希一致：{before[..12]}… / {after[..12]}…");
                return ok ? 0 : 1;
            }
            catch (Exception ex)
            {
                report.AppendLine("  [FAIL] 源 PDF 完整性: " + ex.Message);
                return 1;
            }
        }

        // ---------------- PDF 转图片：页码模式 ----------------

        private static int CheckPageSelection(StringBuilder report, string root)
        {
            report.AppendLine();
            report.AppendLine("[PDF 转图片] 页码模式");
            int failures = 0;

            try
            {
                bool all = PdfImageEngine.TryResolvePages(ConvertPageSelection.All, null, 5,
                    out List<int> allPages, out _) && SamePages(allPages, 1, 2, 3, 4, 5);
                bool odd = PdfImageEngine.TryResolvePages(ConvertPageSelection.Odd, null, 5,
                    out List<int> oddPages, out _) && SamePages(oddPages, 1, 3, 5);
                bool even = PdfImageEngine.TryResolvePages(ConvertPageSelection.Even, null, 5,
                    out List<int> evenPages, out _) && SamePages(evenPages, 2, 4);
                bool custom = PdfImageEngine.TryResolvePages(ConvertPageSelection.Custom, "1-2,5", 5,
                    out List<int> customPages, out _) && SamePages(customPages, 1, 2, 5);
                bool emptyCustom = !PdfImageEngine.TryResolvePages(ConvertPageSelection.Custom, "", 5,
                    out _, out string? emptyError) && emptyError == Loc.T(Txt.ConvertRangeNeeded);
                bool evenSingle = !PdfImageEngine.TryResolvePages(ConvertPageSelection.Even, null, 1,
                    out _, out string? singleError) && singleError == Loc.T(Txt.ConvertNoMatchingPages);

                bool ok = all && odd && even && custom && emptyCustom && evenSingle;
                if (!ok) failures++;
                report.AppendLine(
                    $"  [{(ok ? "OK" : "FAIL")}] 解析：全部={DescribePages(allPages)}, " +
                    $"奇数={DescribePages(oddPages)}, 偶数={DescribePages(evenPages)}, " +
                    $"自定义={DescribePages(customPages)}, 空范围报错={emptyCustom}, 无匹配页报错={evenSingle}");
            }
            catch (Exception ex)
            {
                failures++;
                report.AppendLine("  [FAIL] 页码解析: " + ex.Message);
            }

            try
            {
                string pdf = CreatePageSelectionPdf(root, 5);
                var naming = new AppSettings { NamingMode = NamingMode.FirstDocument };
                string evenDir = Path.Combine(root, "page_even");
                string customDir = Path.Combine(root, "page_custom");

                PdfConvertResult evenResult = PdfImageEngine.Run(new[] { pdf }, evenDir,
                    new PdfConvertOptions
                    {
                        Format = ConvertImageFormat.Png,
                        Ppi = 48,
                        PageSelection = ConvertPageSelection.Even,
                        CreateSubfolder = false,
                        Naming = naming
                    }, null, CancellationToken.None, null);

                PdfConvertResult customResult = PdfImageEngine.Run(new[] { pdf }, customDir,
                    new PdfConvertOptions
                    {
                        Format = ConvertImageFormat.Png,
                        Ppi = 48,
                        PageSelection = ConvertPageSelection.Custom,
                        PageRange = "1-2,5",
                        CreateSubfolder = false,
                        Naming = naming
                    }, null, CancellationToken.None, null);

                string[] evenNames = evenResult.Files[0].Images
                    .Select(path => Path.GetFileName(path) ?? string.Empty).ToArray();
                string[] customNames = customResult.Files[0].Images
                    .Select(path => Path.GetFileName(path) ?? string.Empty).ToArray();

                PdfConvertResult folderResult = PdfImageEngine.Run(new[] { pdf },
                    Path.Combine(root, "page_folder"),
                    new PdfConvertOptions
                    {
                        Format = ConvertImageFormat.Png,
                        Ppi = 48,
                        PageSelection = ConvertPageSelection.Odd,
                        CreateSubfolder = true,
                        Naming = naming
                    }, null, CancellationToken.None, null);
                string folderName = folderResult.Files[0].Images.Count > 0
                    ? Path.GetFileName(Path.GetDirectoryName(folderResult.Files[0].Images[0]) ?? string.Empty)
                    : string.Empty;
                string expectedFolder = "page_modes_" + PdfImageEngine.PageSelectionLabel(ConvertPageSelection.Odd);

                bool evenOk = evenResult.ImageCount == 2 && evenResult.FailedCount == 0 &&
                              SameStrings(evenNames, "page_modes_p002.png", "page_modes_p004.png");
                bool customOk = customResult.ImageCount == 3 && customResult.FailedCount == 0 &&
                                SameStrings(customNames, "page_modes_p001.png",
                                    "page_modes_p002.png", "page_modes_p005.png");
                bool folderOk = folderResult.ImageCount == 3 && folderResult.FailedCount == 0 &&
                                folderName == expectedFolder;
                failures += (evenOk ? 0 : 1) + (customOk ? 0 : 1) + (folderOk ? 0 : 1);

                report.AppendLine(
                    $"  [{(evenOk ? "OK" : "FAIL")}] 偶数页实际导出：{string.Join(", ", evenNames)}");
                report.AppendLine(
                    $"  [{(customOk ? "OK" : "FAIL")}] 自定义实际导出：{string.Join(", ", customNames)}");
                report.AppendLine(
                    $"  [{(folderOk ? "OK" : "FAIL")}] 默认子文件夹：{folderName}（期望 {expectedFolder}）");
            }
            catch (Exception ex)
            {
                failures++;
                report.AppendLine("  [FAIL] 页码模式实际转换: " + ex.Message);
            }

            return failures;
        }

        private static string CreatePageSelectionPdf(string root, int pageCount)
        {
            string path = Path.Combine(root, "page_modes.pdf");
            using var document = new PdfDocument();
            for (int index = 1; index <= pageCount; index++)
            {
                PdfPage page = document.AddPage();
                page.Width = XUnit.FromPoint(595);
                page.Height = XUnit.FromPoint(842);
                using XGraphics graphics = XGraphics.FromPdfPage(page);
                graphics.DrawRectangle(XPens.DarkBlue, 40, 40, 400, 500);
                graphics.DrawLine(XPens.Firebrick, 40, 40 + index * 40, 440, 40 + index * 40);
            }
            document.Save(path);
            return path;
        }

        private static string DescribePages(IReadOnlyList<int> pages) =>
            pages.Count == 0 ? "-" : string.Join(",", pages);

        private static bool SamePages(IReadOnlyList<int> pages, params int[] expected)
        {
            if (pages.Count != expected.Length) return false;
            for (int i = 0; i < expected.Length; i++)
                if (pages[i] != expected[i]) return false;
            return true;
        }

        private static bool SameStrings(IReadOnlyList<string> actual, params string[] expected)
        {
            if (actual.Count != expected.Length) return false;
            for (int i = 0; i < expected.Length; i++)
                if (!string.Equals(actual[i], expected[i], StringComparison.OrdinalIgnoreCase))
                    return false;
            return true;
        }

        private static int CheckWatermarkOutputs(StringBuilder report, string root)
        {
            report.AppendLine();
            report.AppendLine("[水印] 输出目标");
            int failures = 0;
            var spec = new WatermarkSpec("TEST", WatermarkTint.Red, 100, 100, 30, 0, 0,
                WatermarkAnchor.Center);

            try
            {
                using var canvas = new Bitmap(480, 320, PixelFormat.Format24bppRgb);
                using (Graphics graphics = Graphics.FromImage(canvas))
                    graphics.Clear(Color.White);
                WatermarkPainter.ApplyToBitmap(canvas, spec);

                int changed = 0;
                for (int y = 0; y < canvas.Height; y += 4)
                {
                    for (int x = 0; x < canvas.Width; x += 4)
                    {
                        Color pixel = canvas.GetPixel(x, y);
                        if (pixel.R < 250 || pixel.G < 250 || pixel.B < 250) changed++;
                    }
                }

                bool ok = changed > 10;
                if (!ok) failures++;
                report.AppendLine(
                    $"  [{(ok ? "OK" : "FAIL")}] 图片输出叠加水印：变化采样点 {changed}");
            }
            catch (Exception ex)
            {
                failures++;
                report.AppendLine("  [FAIL] 图片输出叠加水印: " + ex.Message);
            }

            try
            {
                string image = Path.Combine(root, "wm_source.png");
                CreateImage(image, 400, 300, ImageFormat.Png, Color.SeaGreen);
                string pdf = Path.Combine(root, "wm_source.pdf");
                using (var document = new PdfDocument())
                {
                    PdfPage page = document.AddPage();
                    using XGraphics graphics = XGraphics.FromPdfPage(page);
                    using XImage source = XImage.FromFile(image);
                    graphics.DrawImage(source, 60, 60, 360, 270);
                    graphics.DrawRectangle(XPens.DarkBlue, 60, 360, 360, 60);
                    document.Save(pdf);
                }

                string before = FileHash(pdf);
                string plainDir = Path.Combine(root, "wm_pdf_plain");
                string markDir = Path.Combine(root, "wm_pdf_marked");
                Directory.CreateDirectory(plainDir);
                Directory.CreateDirectory(markDir);
                var naming = new AppSettings { NamingMode = NamingMode.FirstDocument };

                PdfConvertResult plain = PdfImageEngine.Run(new[] { pdf }, plainDir,
                    new PdfConvertOptions
                    {
                        Format = ConvertImageFormat.Png,
                        Ppi = 48,
                        CreateSubfolder = false,
                        Naming = naming
                    }, null, CancellationToken.None, null);

                PdfConvertResult marked = PdfImageEngine.Run(new[] { pdf }, markDir,
                    new PdfConvertOptions
                    {
                        Format = ConvertImageFormat.Png,
                        Ppi = 48,
                        CreateSubfolder = false,
                        Naming = naming,
                        Watermark = spec
                    }, null, CancellationToken.None, null);

                int diff = CountDifferentPixels(
                    plain.Files[0].Images[0], marked.Files[0].Images[0]);
                bool ok = diff > 100 && before == FileHash(pdf);
                if (!ok) failures++;
                report.AppendLine(
                    $"  [{(ok ? "OK" : "FAIL")}] PDF 转图片叠加水印且源文件不变：差异采样点 {diff}");
            }
            catch (Exception ex)
            {
                failures++;
                report.AppendLine("  [FAIL] PDF 转图片叠加水印: " + ex.Message);
            }

            try
            {
                string image = Path.Combine(root, "wm_image.png");
                CreateImage(image, 400, 300, ImageFormat.Png, Color.CornflowerBlue);
                string plainPdf = Path.Combine(root, "wm_plain.pdf");
                string markedPdf = Path.Combine(root, "wm_marked.pdf");

                ImagePdfEngine.Run(new[] { image }, plainPdf,
                    new ImagePdfOptions { PageSize = ImagePdfPageSize.A4 },
                    null, CancellationToken.None);
                ImagePdfEngine.Run(new[] { image }, markedPdf,
                    new ImagePdfOptions { PageSize = ImagePdfPageSize.A4, Watermark = spec },
                    null, CancellationToken.None);

                int plainObjects = CountXObjects(plainPdf);
                int markedObjects = CountXObjects(markedPdf);
                bool ok = markedObjects > plainObjects;
                if (!ok) failures++;
                report.AppendLine(
                    $"  [{(ok ? "OK" : "FAIL")}] 图片转 PDF 叠加水印：页面资源 {plainObjects} → {markedObjects}");
            }
            catch (Exception ex)
            {
                failures++;
                report.AppendLine("  [FAIL] 图片转 PDF 叠加水印: " + ex.Message);
            }

            return failures;
        }

        private static int CountDifferentPixels(string first, string second)
        {
            using var a = new Bitmap(first);
            using var b = new Bitmap(second);
            if (a.Width != b.Width || a.Height != b.Height) return int.MaxValue;

            int changed = 0;
            for (int y = 0; y < a.Height; y += 3)
            {
                for (int x = 0; x < a.Width; x += 3)
                {
                    if (a.GetPixel(x, y) != b.GetPixel(x, y)) changed++;
                }
            }
            return changed;
        }

        private static int CountXObjects(string path)
        {
            using PdfDocument document = PdfReader.Open(path, PdfDocumentOpenMode.Import);
            PdfPage page = document.Pages[0];
            PdfDictionary? resources = page.Elements.GetDictionary("/Resources");
            PdfDictionary? xobjects = resources?.Elements.GetDictionary("/XObject");
            return xobjects?.Elements.Count ?? 0;
        }

        private static string FileHash(string path)
        {
            using FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            return Convert.ToHexString(SHA256.HashData(stream));
        }

        private static int CheckPageSize(StringBuilder report, string root, string title,
            ImagePdfPageSize size, ImagePdfOrientation orientation, int margin,
            double expectedWidth, double expectedHeight, string image)
        {
            string path = Path.Combine(root, "size_" + Guid.NewGuid().ToString("N") + ".pdf");
            return CheckSingle(report, title, size, orientation, margin, new[] { image }, path,
                expectedWidth, expectedHeight);
        }

        private static int CheckSingle(StringBuilder report, string title,
            ImagePdfPageSize size, ImagePdfOrientation orientation, int margin,
            IReadOnlyList<string> images, string output,
            double expectedWidth, double expectedHeight)
        {
            try
            {
                ImagePdfResult result = ImagePdfEngine.Run(images, output,
                    new ImagePdfOptions
                    {
                        PageSize = size,
                        Orientation = orientation,
                        MarginMm = margin
                    }, null, CancellationToken.None);

                using PdfDocument document = PdfReader.Open(output, PdfDocumentOpenMode.Import);
                PdfPage page = document.Pages[0];
                double width = page.Width.Point;
                double height = page.Height.Point;
                bool ok = result.PageCount == 1 &&
                          Math.Abs(width - expectedWidth) <= 0.6 &&
                          Math.Abs(height - expectedHeight) <= 0.6;
                if (!ok)
                {
                    report.AppendLine(
                        $"  [FAIL] {title}: 期望 {expectedWidth:0.##}×{expectedHeight:0.##} pt，" +
                        $"实际 {width:0.##}×{height:0.##} pt，页数 {result.PageCount}");
                    return 1;
                }

                report.AppendLine($"  [OK] {title}: {width:0.##}×{height:0.##} pt");
                return 0;
            }
            catch (Exception ex)
            {
                report.AppendLine($"  [FAIL] {title}: {ex.Message}");
                return 1;
            }
        }

        private static int CheckBatch(StringBuilder report, IReadOnlyList<string> images,
            string output, int expectedPages)
        {
            try
            {
                ImagePdfResult result = ImagePdfEngine.Run(images, output,
                    new ImagePdfOptions { PageSize = ImagePdfPageSize.A4, MarginMm = 10 },
                    null, CancellationToken.None);

                using PdfDocument document = PdfReader.Open(output, PdfDocumentOpenMode.Import);
                bool ok = result.PageCount == expectedPages && document.PageCount == expectedPages;
                if (!ok)
                {
                    report.AppendLine(
                        $"  [FAIL] 批量合成: 期望 {expectedPages} 页，结果 {result.PageCount} 页，PDF {document.PageCount} 页");
                    return 1;
                }

                report.AppendLine($"  [OK] 批量合成: {document.PageCount} 页");
                return 0;
            }
            catch (Exception ex)
            {
                report.AppendLine("  [FAIL] 批量合成: " + ex.Message);
                return 1;
            }
        }

        // ---------------- 图标 ----------------

        private static int CheckSettingsLanguageReload(StringBuilder report)
        {
            report.AppendLine();
            report.AppendLine("[设置] 切换语言不跳动");
            try
            {
                var settings = new AppSettings();
                using var form = new SettingsForm(settings, 3)
                {
                    StartPosition = FormStartPosition.Manual,
                    Bounds = new Rectangle(140, 120, 860, 640)
                };
                Rectangle before = form.Bounds;
                int controlsBefore = form.Controls.Count;

                form.SwitchLanguage(Language.Ru);
                bool keepBounds = form.Bounds == before;
                bool keepPage = form.CurrentPage == 3;
                bool rebuilt = form.Controls.Count == controlsBefore && form.Controls.Count > 0;

                form.SwitchLanguage(Language.ZhCn);
                bool restored = form.CurrentPage == 3 && form.Bounds == before;

                bool ok = keepBounds && keepPage && rebuilt && restored;
                report.AppendLine(
                    $"  [{(ok ? "OK" : "FAIL")}] 原地重建: 位置不变={keepBounds}, " +
                    $"页={keepPage}, 控件重建={rebuilt}, 返回中文={restored}");
                return ok ? 0 : 1;
            }
            catch (Exception ex)
            {
                report.AppendLine("  [FAIL] 切换语言异常: " + ex.Message);
                return 1;
            }
            finally
            {
                Loc.SetLanguage(Language.ZhCn);
                Theme.ApplySkin(SkinPalette.ClassicBlue);
            }
        }

        private static int CheckBackdropCacheRecovery(StringBuilder report)
        {
            report.AppendLine();
            report.AppendLine("[玻璃] 背景缓存被误释放后自愈");
            try
            {
                var size = new Size(860, 640);
                SkinPalette skin = SkinPalette.ClassicBlue;
                GlassPaint.Backdrop first = GlassPaint.GetBackdrop(size, skin);
                Bitmap staleSharp = first.Sharp;
                Bitmap staleFrosted = first.Frosted;

                // 模拟旧版设置窗口在重建时误释放了缓存持有的位图
                staleSharp.Dispose();
                staleFrosted.Dispose();

                GlassPaint.Backdrop again = GlassPaint.GetBackdrop(size, skin);
                bool rebuilt = !ReferenceEquals(again.Sharp, staleSharp) &&
                               again.Sharp.Width == size.Width &&
                               again.Frosted.Width == size.Width;

                report.AppendLine($"  [{(rebuilt ? "OK" : "FAIL")}] 命中坏缓存后重建：{rebuilt}");
                return rebuilt ? 0 : 1;
            }
            catch (Exception ex)
            {
                report.AppendLine("  [FAIL] 缓存自愈异常: " + ex.Message);
                return 1;
            }
        }

        private static int CheckIcons(StringBuilder report)
        {
            int failures = 0;
            report.AppendLine();
            report.AppendLine("[图标] 渲染完整性");

            foreach (int size in new[] { 14, 24, 48 })
            {
                int empty = 0;
                foreach (IconKind kind in Enum.GetValues(typeof(IconKind)))
                {
                    if (kind == IconKind.None) continue;
                    using var bitmap = new Bitmap(size + 8, size + 8, PixelFormat.Format32bppArgb);
                    using (var graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.Clear(Color.White);
                        IconPainter.Draw(graphics, kind,
                            new RectangleF(4, 4, size, size), Color.FromArgb(31, 41, 55));
                    }
                    if (CountInk(bitmap) < Math.Max(4, size / 3)) empty++;
                }

                bool ok = empty == 0;
                if (!ok) failures++;
                report.AppendLine($"  [{(ok ? "OK" : "FAIL")}] {size}px 功能图标: 空白 {empty}");
            }

            using (var mark = new Bitmap(72, 72, PixelFormat.Format32bppArgb))
            {
                using (var graphics = Graphics.FromImage(mark))
                {
                    graphics.Clear(Color.White);
                    Art.DrawMark(graphics, new RectangleF(4, 4, 64, 64));
                }
                int ink = CountInk(mark);
                bool ok = ink > 600;
                if (!ok) failures++;
                report.AppendLine($"  [{(ok ? "OK" : "FAIL")}] 品牌标记: 着色像素 {ink}");
            }

            return failures;
        }

        private static int CountInk(Bitmap bitmap)
        {
            int count = 0;
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color pixel = bitmap.GetPixel(x, y);
                    if (pixel.A > 8 && (pixel.R < 248 || pixel.G < 248 || pixel.B < 248))
                        count++;
                }
            }
            return count;
        }

        private static void CreateImage(string path, int width, int height,
            ImageFormat format, Color color)
        {
            using var bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            bitmap.SetResolution(96f, 96f);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(color);
                using var brush = new SolidBrush(Color.White);
                graphics.FillEllipse(brush, width / 4f, height / 4f, width / 2f, height / 2f);
            }
            bitmap.Save(path, format);
        }
    }
}
