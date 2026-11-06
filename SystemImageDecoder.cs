using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace DocHub
{
    /// <summary>
    /// 系统解码器兜底：GDI+（System.Drawing）只能解 JPEG/PNG/BMP/TIFF/GIF，
    /// 手机照片常见的 HEIC 与网页/聊天工具里的 WebP、AVIF 需要走 Windows 自带的
    /// WIC 编解码器（WinRT BitmapDecoder）。解码成功后统一转成白底 PNG，
    /// 再交给 PDFsharp 嵌入，避免整批合并因为一个格式不认而失败。
    /// </summary>
    internal static class SystemImageDecoder
    {
        public static bool TryConvertToPng(string source, string targetPng,
            out int width, out int height, out double dpiX, out double dpiY, out string error)
        {
            width = 1;
            height = 1;
            dpiX = 96;
            dpiY = 96;
            error = string.Empty;

            try
            {
                DecodeAsync(source, targetPng).GetAwaiter().GetResult();

                using var probe = new Bitmap(targetPng);
                width = Math.Max(1, probe.Width);
                height = Math.Max(1, probe.Height);
                dpiX = probe.HorizontalResolution > 0 ? probe.HorizontalResolution : 96;
                dpiY = probe.VerticalResolution > 0 ? probe.VerticalResolution : 96;
                return true;
            }
            catch (Exception ex)
            {
                error = UnwrapMessage(ex);
                try { if (File.Exists(targetPng)) File.Delete(targetPng); }
                catch { /* 临时文件交给系统清理 */ }
                return false;
            }
        }

        /// <summary>聚合异常只取第一条，避免界面出现「One or more errors occurred」这类无信息量的前缀。</summary>
        private static string UnwrapMessage(Exception ex)
        {
            if (ex is AggregateException aggregate && aggregate.InnerExceptions.Count > 0)
                ex = aggregate.InnerExceptions[0];

            Exception? inner = ex.InnerException;
            while (inner != null)
            {
                ex = inner;
                inner = ex.InnerException;
            }

            return string.IsNullOrWhiteSpace(ex.Message) ? ex.GetType().Name : ex.Message;
        }

        private static async Task DecodeAsync(string source, string targetPng)
        {
            byte[] bytes = await File.ReadAllBytesAsync(source);

            using var stream = new InMemoryRandomAccessStream();
            using (var writer = new DataWriter(stream.GetOutputStreamAt(0)))
            {
                writer.WriteBytes(bytes);
                await writer.StoreAsync();
                await writer.FlushAsync();
                writer.DetachStream();
            }

            stream.Seek(0);
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);

            BitmapFrame frame = await decoder.GetFrameAsync(0);
            PixelDataProvider provider = await frame.GetPixelDataAsync(
                BitmapPixelFormat.Bgra8,
                BitmapAlphaMode.Premultiplied,
                new BitmapTransform(),
                ExifOrientationMode.RespectExifOrientation,
                ColorManagementMode.ColorManageToSRgb);

            byte[] pixels = provider.DetachPixelData();
            int width = Math.Max(1, (int)frame.PixelWidth);
            int height = Math.Max(1, (int)frame.PixelHeight);

            using var bitmap = new Bitmap(width, height, PixelFormat.Format32bppPArgb);
            BitmapData data = bitmap.LockBits(
                new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);
            try
            {
                // 逐行拷贝：WIC 的像素缓冲是紧密排列的，Bitmap 的 stride 可能带填充
                int rowBytes = width * 4;
                for (int y = 0; y < height; y++)
                    Marshal.Copy(pixels, y * rowBytes, data.Scan0 + y * data.Stride, rowBytes);
            }
            finally
            {
                bitmap.UnlockBits(data);
            }

            float dpiX = frame.DpiX > 0 ? (float)frame.DpiX : 96f;
            float dpiY = frame.DpiY > 0 ? (float)frame.DpiY : 96f;
            try { bitmap.SetResolution(dpiX, dpiY); }
            catch { /* 个别编码器不接受自定义 DPI */ }

            bitmap.Save(targetPng, ImageFormat.Png);
        }
    }
}
