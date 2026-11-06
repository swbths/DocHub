using System;
using System.Collections.Generic;
using System.IO;
using System.Media;

namespace DocHub
{
    /// <summary>
    /// 内置提示音：全部在内存中即时合成，不依赖任何外部音频文件。
    /// </summary>
    public static class SoundLibrary
    {
        public const string DingId = "ding";
        public const string WaterId = "water";
        public const string QqId = "qq";
        public const string ChimeId = "chime";
        public const string SoftId = "soft";
        public const string OffId = "off";

        private const int SampleRate = 44100;
        private static readonly Dictionary<string, SoundPlayer> Players = new();

        public static readonly string[] All =
        {
            DingId, WaterId, QqId, ChimeId, SoftId, OffId
        };

        public static bool Exists(string? id)
        {
            if (string.IsNullOrWhiteSpace(id)) return false;
            foreach (string item in All)
                if (string.Equals(item, id, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        public static void Play(string? id)
        {
            if (string.IsNullOrWhiteSpace(id) || id == OffId) return;

            try
            {
                SoundPlayer player = GetPlayer(id);
                if (player != null) player.Play();
            }
            catch
            {
                // 无音频设备或驱动异常时静默忽略，不影响合并
            }
        }

        private static SoundPlayer? GetPlayer(string id)
        {
            lock (Players)
            {
                if (Players.TryGetValue(id, out SoundPlayer? cached) && cached != null) return cached;

                byte[]? wav = Build(id);
                if (wav == null) return null;

                var stream = new MemoryStream(wav);
                var player = new SoundPlayer(stream);
                Players[id] = player;
                return player;
            }
        }

        private static byte[]? Build(string id)
        {
            switch (id)
            {
                case WaterId: return BuildWater();
                case QqId: return BuildQq();
                case ChimeId: return BuildChime();
                case SoftId: return BuildSoft();
                case OffId: return null;
                default: return BuildDing();
            }
        }

        private static double[] NewBuffer(double durationSeconds)
        {
            int count = Math.Max(1, (int)(durationSeconds * SampleRate));
            return new double[count];
        }

        private static void AddTone(
            double[] buffer, double startSeconds, double frequency, double durationSeconds,
            double amplitude, double decaySeconds, double attackSeconds = 0.004, double harmonic = 0.0,
            double endFrequency = 0.0)
        {
            int start = (int)(startSeconds * SampleRate);
            int length = (int)(durationSeconds * SampleRate);
            double attack = Math.Max(0.0005, attackSeconds);

            for (int i = 0; i < length; i++)
            {
                int index = start + i;
                if (index < 0 || index >= buffer.Length) continue;

                double t = (double)i / SampleRate;
                double progress = (double)i / Math.Max(1, length - 1);
                double freq = endFrequency > 0
                    ? frequency + (endFrequency - frequency) * progress
                    : frequency;

                double attackFactor = 1.0 - Math.Exp(-t / attack);
                double envelope = attackFactor * Math.Exp(-t / Math.Max(0.0001, decaySeconds));
                double wave = Math.Sin(2.0 * Math.PI * freq * t);
                if (harmonic > 0)
                    wave += harmonic * Math.Sin(4.0 * Math.PI * freq * t);

                buffer[index] += wave * envelope * amplitude;
            }
        }

        private static byte[] BuildDing()
        {
            double[] buffer = NewBuffer(0.85);
            AddTone(buffer, 0.00, 880, 0.32, 0.55, 0.16, 0.006, 0.28);
            AddTone(buffer, 0.22, 659, 0.62, 0.52, 0.30, 0.008, 0.24);
            return ToWav(buffer);
        }

        private static byte[] BuildWater()
        {
            double[] buffer = NewBuffer(0.66);
            // 水滴：极短的宽带冲击 → 迅速上扬的共振峰（水滴声最标志性的“啾”）→ 水面余韵
            AddNoise(buffer, 0.000, 0.014, 0.30, 0.0045);
            AddTone(buffer, 0.002, 480, 0.14, 0.50, 0.045, 0.0018, 0.08, 1250);
            AddTone(buffer, 0.012, 1180, 0.07, 0.12, 0.020, 0.0015, 0.0, 1560);
            AddTone(buffer, 0.020, 300, 0.34, 0.11, 0.130, 0.010, 0.05, 240);

            // 第二滴（更轻、更高），形成干脆的“滴答”节奏
            AddNoise(buffer, 0.252, 0.010, 0.16, 0.0035);
            AddTone(buffer, 0.254, 620, 0.10, 0.36, 0.032, 0.0018, 0.06, 1420);
            AddTone(buffer, 0.250, 260, 0.22, 0.07, 0.090, 0.008, 0.04, 210);
            return ToWav(buffer, 0.60);
        }

        private static byte[] BuildQq()
        {
            double[] buffer = NewBuffer(0.80);
            // 经典 QQ 提示音：两声清脆的短音（第二声更高）＋ 一声低音收尾
            AddTone(buffer, 0.000, 1318.5, 0.11, 0.42, 0.030, 0.0015, 0.30, 1360);
            AddTone(buffer, 0.000, 2637.0, 0.06, 0.09, 0.018, 0.0015, 0.0);
            AddTone(buffer, 0.118, 1760.0, 0.11, 0.40, 0.032, 0.0015, 0.28, 1800);
            AddTone(buffer, 0.118, 3520.0, 0.05, 0.06, 0.016, 0.0015, 0.0);
            AddTone(buffer, 0.250, 880.0, 0.30, 0.34, 0.140, 0.0040, 0.20, 830);
            AddTone(buffer, 0.250, 1760.0, 0.14, 0.09, 0.050, 0.0030, 0.10);
            return ToWav(buffer, 0.58);
        }

        private static byte[] BuildChime()
        {
            double[] buffer = NewBuffer(1.15);
            double[] notes = { 523.25, 659.25, 783.99, 1046.50 };
            for (int i = 0; i < notes.Length; i++)
                AddTone(buffer, i * 0.09, notes[i], 0.55 - i * 0.06, 0.34, 0.22, 0.005, 0.16);
            return ToWav(buffer);
        }

        private static byte[] BuildSoft()
        {
            double[] buffer = NewBuffer(0.95);
            AddTone(buffer, 0.00, 415, 0.85, 0.20, 0.40, 0.12, 0.06);
            AddTone(buffer, 0.06, 523, 0.80, 0.16, 0.38, 0.14, 0.05);
            return ToWav(buffer);
        }

        /// <summary>极短的宽带噪声，用来做水滴的「空气冲击」起音；带一级低通，避免刺耳的嘶声。</summary>
        private static void AddNoise(
            double[] buffer, double startSeconds, double durationSeconds, double amplitude, double decaySeconds)
        {
            int start = (int)(startSeconds * SampleRate);
            int length = Math.Max(1, (int)(durationSeconds * SampleRate));
            double previous = 0;
            var random = new Random(20261104);

            for (int i = 0; i < length; i++)
            {
                int index = start + i;
                double t = (double)i / SampleRate;
                double envelope = Math.Exp(-t / Math.Max(0.0001, decaySeconds));
                double raw = random.NextDouble() * 2.0 - 1.0;

                // 一级低通：把白噪声压成更接近水面的「噗」
                previous += 0.35 * (raw - previous);

                if (index < 0 || index >= buffer.Length) continue;
                buffer[index] += previous * envelope * amplitude;
            }
        }

        private static byte[] ToWav(double[] samples) => ToWav(samples, 0.0);

        /// <summary>targetPeak 大于 0 时按目标峰值归一，让每个提示音的响度保持一致。</summary>
        private static byte[] ToWav(double[] samples, double targetPeak)
        {
            double peak = 0;
            for (int i = 0; i < samples.Length; i++)
                peak = Math.Max(peak, Math.Abs(samples[i]));

            double gain = 1.0;
            if (targetPeak > 0 && peak > 0.0001)
            {
                gain = targetPeak / peak;
            }
            else if (peak > 0.9)
            {
                gain = 0.9 / peak;
            }

            if (Math.Abs(gain - 1.0) > 0.0001)
            {
                for (int i = 0; i < samples.Length; i++) samples[i] *= gain;
            }

            int fade = Math.Min(samples.Length, SampleRate / 125);
            for (int i = 0; i < fade; i++)
                samples[samples.Length - 1 - i] *= (double)i / fade;

            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);

            writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
            writer.Write(36 + samples.Length * 2);
            writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));
            writer.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
            writer.Write(16);
            writer.Write((short)1);
            writer.Write((short)1);
            writer.Write(SampleRate);
            writer.Write(SampleRate * 2);
            writer.Write((short)2);
            writer.Write((short)16);
            writer.Write(System.Text.Encoding.ASCII.GetBytes("data"));
            writer.Write(samples.Length * 2);

            for (int i = 0; i < samples.Length; i++)
            {
                double value = Math.Clamp(samples[i], -1.0, 1.0);
                writer.Write((short)(value * short.MaxValue));
            }

            writer.Flush();
            return stream.ToArray();
        }
    }
}
