using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using NAudio.CoreAudioApi;

namespace CycloneKallApp
{
    public partial class MainWindow : Window
    {
        private readonly MMDeviceEnumerator _enumerator = new();
        private readonly AudioEngine _engine = new();

        public MainWindow()
        {
            InitializeComponent();
            LoadDevices();
            FightPreset_Click(this, new RoutedEventArgs());
            Closing += (_, _) => _engine.Dispose();
        }

        private void LoadDevices()
        {
            var micDevices = _enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).ToList();
            MicComboBox.ItemsSource = micDevices;
            MicComboBox.DisplayMemberPath = "FriendlyName";
            if (micDevices.Count > 0) MicComboBox.SelectedIndex = 0;

            var outputDevices = _enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active).ToList();
            OutputComboBox.ItemsSource = outputDevices;
            OutputComboBox.DisplayMemberPath = "FriendlyName";
            if (outputDevices.Count > 0)
            {
                var cable = outputDevices.FirstOrDefault(d => d.FriendlyName.Contains("CABLE Input", StringComparison.OrdinalIgnoreCase));
                OutputComboBox.SelectedItem = cable ?? outputDevices.First();
            }
            UpdateMicStatus();
        }

        private void UpdateMicStatus()
        {
            if (MicComboBox.SelectedItem is MMDevice mic)
                StatusText.Text = $"CYCLONE KALL APP • میکروفون انتخاب‌شده: {mic.FriendlyName}";
        }

        private void MicComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateMicStatus();

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            _engine.Stop();
            StartStopButton.Content = "▶ شروع پردازش Cyclone Kall App";
            LoadDevices();
        }

        private void BoostSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (BoostText == null) return;
            BoostText.Text = $"+{e.NewValue:0.0} dB";
            _engine.GainDb = (float)e.NewValue;
        }

        private void Processing_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (GateSlider == null) return;
            GateText.Text = $"{GateSlider.Value * 1000:0}";
            CompText.Text = $"{CompSlider.Value * 100:0}%";
            PresenceText.Text = $"+{PresenceSlider.Value:0.0} dB";
            _engine.Gate = (float)GateSlider.Value;
            _engine.Compression = (float)CompSlider.Value;
            _engine.PresenceDb = (float)PresenceSlider.Value;
        }

        private void StartStopButton_Click(object sender, RoutedEventArgs e)
        {
            if (_engine.IsRunning)
            {
                _engine.Stop();
                StartStopButton.Content = "▶ شروع پردازش Cyclone Kall App";
                StatusText.Text = "CYCLONE KALL APP • پردازش متوقف شد.";
                return;
            }

            if (MicComboBox.SelectedItem is not MMDevice mic || OutputComboBox.SelectedItem is not MMDevice output)
            {
                StatusText.Text = "CYCLONE KALL APP • لطفاً میکروفون و خروجی را انتخاب کن.";
                return;
            }

            try
            {
                _engine.Start(mic, output);
                _engine.GainDb = (float)BoostSlider.Value;
                _engine.Gate = (float)GateSlider.Value;
                _engine.Compression = (float)CompSlider.Value;
                _engine.PresenceDb = (float)PresenceSlider.Value;
                StartStopButton.Content = "■ توقف پردازش Cyclone Kall App";
                StatusText.Text = $"CYCLONE KALL APP • فعال\nMic: {mic.FriendlyName}\nOutput: {output.FriendlyName}";
            }
            catch (Exception ex)
            {
                StatusText.Text = "CYCLONE KALL APP • خطا: " + ex.Message;
            }
        }

        private void ApplyPreset(float gain, float gate, float comp, float presence)
        {
            BoostSlider.Value = gain;
            GateSlider.Value = gate;
            CompSlider.Value = comp;
            PresenceSlider.Value = presence;
        }

        private void FightPreset_Click(object sender, RoutedEventArgs e) => ApplyPreset(18, 0.008f, 0.88f, 5.0f);
        private void ClearPreset_Click(object sender, RoutedEventArgs e) => ApplyPreset(10, 0.012f, 0.65f, 4.0f);
        private void StudioPreset_Click(object sender, RoutedEventArgs e) => ApplyPreset(6, 0.004f, 0.45f, 2.0f);
        private void FlatPreset_Click(object sender, RoutedEventArgs e) => ApplyPreset(0, 0, 0, 0);
    }
}