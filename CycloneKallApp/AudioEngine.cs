using System;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace CycloneKallApp
{
    public sealed class AudioEngine : IDisposable
    {
        private WasapiCapture? _capture;
        private WasapiOut? _output;
        private BufferedWaveProvider? _buffer;
        private GainSampleProvider? _processor;

        public bool IsRunning { get; private set; }

        public float GainDb { get => _processor?.GainDb ?? 0; set { if (_processor != null) _processor.GainDb = Math.Clamp(value, 0, 100); } }
        public float Gate { get => _processor?.Gate ?? 0; set { if (_processor != null) _processor.Gate = Math.Clamp(value, 0, 0.12f); } }
        public float Compression { get => _processor?.Compression ?? 0; set { if (_processor != null) _processor.Compression = Math.Clamp(value, 0, 1); } }
        public float PresenceDb { get => _processor?.PresenceDb ?? 0; set { if (_processor != null) _processor.PresenceDb = Math.Clamp(value, 0, 12); } }

        public void Start(MMDevice inputDevice, MMDevice outputDevice)
        {
            Stop();

            _capture = new WasapiCapture(inputDevice);
            _buffer = new BufferedWaveProvider(_capture.WaveFormat)
            {
                DiscardOnBufferOverflow = true,
                BufferDuration = TimeSpan.FromMilliseconds(500)
            };

            _capture.DataAvailable += (_, e) =>
            {
                try { _buffer?.AddSamples(e.Buffer, 0, e.BytesRecorded); } catch { }
            };

            _processor = new GainSampleProvider(_buffer.ToSampleProvider())
            {
                GainDb = 0, Gate = 0.01f, Compression = 0.7f, PresenceDb = 3
            };

            _output = new WasapiOut(outputDevice, AudioClientShareMode.Shared, true, 40);
            _output.Init(_processor);
            _capture.StartRecording();
            _output.Play();
            IsRunning = true;
        }

        public void Stop()
        {
            try { _capture?.StopRecording(); } catch { }
            try { _output?.Stop(); } catch { }
            _capture?.Dispose();
            _output?.Dispose();
            _capture = null; _output = null; _buffer = null; _processor = null;
            IsRunning = false;
        }

        public void Dispose() => Stop();
    }
}