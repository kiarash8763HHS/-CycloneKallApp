using System;
using NAudio.Wave;

namespace CycloneKallApp
{
    public sealed class GainSampleProvider : ISampleProvider
    {
        private readonly ISampleProvider _source;
        public float GainDb { get; set; }
        public float Gate { get; set; } = 0.01f;
        public float Compression { get; set; } = 0.7f;
        public float PresenceDb { get; set; } = 3f;
        public WaveFormat WaveFormat => _source.WaveFormat;

        public GainSampleProvider(ISampleProvider source) => _source = source;

        public int Read(float[] buffer, int offset, int count)
        {
            int n = _source.Read(buffer, offset, count);
            double gain = Math.Pow(10.0, GainDb / 20.0);
            // A conservative limiter is intentionally kept at the end. 100 dB is a UI range;
            // feeding 100 dB of unrestricted digital gain would only create severe clipping/noise.
            const float ceiling = 0.97f;

            for (int i = 0; i < n; i++)
            {
                float x = buffer[offset + i];
                float abs = Math.Abs(x);
                if (Gate > 0 && abs < Gate) x = 0;

                x *= (float)gain;

                // Simple soft-knee compression. Higher Compression = stronger leveling.
                float a = Math.Abs(x);
                if (Compression > 0 && a > 0.25f)
                {
                    float excess = a - 0.25f;
                    float compressed = 0.25f + excess / (1f + Compression * 7f);
                    x = MathF.CopySign(compressed, x);
                }

                // Presence control: a lightweight non-linear speech emphasis, kept subtle.
                float presence = Math.Clamp(PresenceDb / 12f, 0f, 1f);
                x += x * presence * 0.08f;

                // Smooth soft limiter.
                x = SoftLimit(x, ceiling);
                buffer[offset + i] = x;
            }
            return n;
        }

        private static float SoftLimit(float x, float ceiling)
        {
            if (Math.Abs(x) <= ceiling) return x;
            float sign = MathF.Sign(x);
            float over = Math.Abs(x) - ceiling;
            return sign * (ceiling + (1f - ceiling) * MathF.Tanh(over / (1f - ceiling)));
        }
    }
}