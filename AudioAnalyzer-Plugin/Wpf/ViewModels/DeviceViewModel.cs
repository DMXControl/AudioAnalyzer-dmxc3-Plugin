using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AudioAnalyzer.Engine;
using LumosControlsWPF.Base;
using LumosLIB.Tools.I18n;

namespace AudioAnalyzer.Wpf.ViewModels
{
    /// <summary>
    /// Device selection, input channel and main gain. Purely over IAudioAnalyzer.
    /// </summary>
    public class DeviceViewModel : BasePropertyNotification
    {
        private readonly IAudioAnalyzer analyzer;

        private AudioDeviceInfo selectedDevice;
        private int selectedInputChannel;
        private int mainGain;
        private bool applying;

        public DeviceViewModel(IAudioAnalyzer analyzer)
        {
            this.analyzer = analyzer;

            Devices = new ObservableCollection<AudioDeviceInfo>();
            InputChannels = new ObservableCollection<string>();

            reload();

            analyzer.SettingsChanged += onSettingsChanged;
            analyzer.RunningChanged += onRunningChanged;
            analyzer.DevicesChanged += onSettingsChanged;   // Liste kam erst spaeter
        }

        public ObservableCollection<AudioDeviceInfo> Devices { get; }

        public ObservableCollection<string> InputChannels { get; }

        public AudioDeviceInfo SelectedDevice
        {
            get { return selectedDevice; }
            set
            {
                if (!SetProperty(ref selectedDevice, value))
                    return;

                if (applying || value == null)
                    return;

                analyzer.SelectedDeviceId = value.Id;
                reloadInputChannels();
            }
        }

        /// <summary>ASIO only; the list stays empty for WASAPI devices</summary>
        public int SelectedInputChannel
        {
            get { return selectedInputChannel; }
            set
            {
                if (!SetProperty(ref selectedInputChannel, value))
                    return;

                if (applying || value < 0)
                    return;

                analyzer.SelectedInputChannel = value;
            }
        }

        public bool HasInputChannels
        {
            get { return InputChannels.Count > 0; }
        }

        /// <summary>main gain, 0..100</summary>
        public int MainGain
        {
            get { return mainGain; }
            set
            {
                if (!SetProperty(ref mainGain, value))
                    return;

                if (applying)
                    return;

                analyzer.MainGain = value;
            }
        }

        public bool IsRunning
        {
            get { return analyzer.IsRunning; }
        }

        public string RunButtonText
        {
            get { return analyzer.IsRunning ? T._("Stop") : T._("Start"); }
        }

        public void ToggleRun()
        {
            analyzer.ToggleRun();
        }

        private void reload()
        {
            applying = true;
            try
            {
                Devices.Clear();
                foreach (var d in analyzer.Devices)
                {
                    Devices.Add(d);
                }

                string id = analyzer.SelectedDeviceId;
                AudioDeviceInfo match = null;
                foreach (var d in Devices)
                {
                    if (String.Equals(d.Id, id))
                    {
                        match = d;
                        break;
                    }
                }
                SetProperty(ref selectedDevice, match, nameof(SelectedDevice));

                SetProperty(ref mainGain, analyzer.MainGain, nameof(MainGain));

                reloadInputChannels();
            }
            finally
            {
                applying = false;
            }
        }

        private void reloadInputChannels()
        {
            bool outer = applying;
            applying = true;
            try
            {
                InputChannels.Clear();
                foreach (var c in analyzer.InputChannels)
                {
                    InputChannels.Add(c);
                }

                int channel = analyzer.SelectedInputChannel;
                if (channel >= InputChannels.Count)
                    channel = InputChannels.Count > 0 ? 0 : -1;

                SetProperty(ref selectedInputChannel, channel, nameof(SelectedInputChannel));
                InvokePropertyChanged(nameof(HasInputChannels));
            }
            finally
            {
                applying = outer;
            }
        }

        private void onSettingsChanged(object sender, EventArgs e)
        {
            reload();
        }

        private void onRunningChanged(object sender, EventArgs e)
        {
            InvokePropertyChanged(nameof(IsRunning));
            InvokePropertyChanged(nameof(RunButtonText));
        }

        protected override void OnDispose()
        {
            analyzer.SettingsChanged -= onSettingsChanged;
            analyzer.RunningChanged -= onRunningChanged;
            analyzer.DevicesChanged -= onSettingsChanged;
        }
    }
}
