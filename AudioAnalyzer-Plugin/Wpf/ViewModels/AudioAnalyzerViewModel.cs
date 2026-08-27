using System;
using System.Windows.Threading;
using AudioAnalyzer.AAEventArgs;
using AudioAnalyzer.Engine;
using CommunityToolkit.Mvvm.Input;
using LumosControlsWPF.Base;
using LumosLIB.Tools.I18n;

namespace AudioAnalyzer.Wpf.ViewModels
{
    /// <summary>
    /// Root view model. Everything comes in through IAudioAnalyzer as events, nothing is
    /// polled
    /// </summary>
    public class AudioAnalyzerViewModel : BasePropertyNotification
    {
        private readonly IAudioAnalyzer analyzer;
        private readonly DispatcherTimer beatOffTimer;

        private double levelLeft;
        private double levelRight;
        private EBeatSource? beatSource;
        private RelayCommand toggleRunCommand;
        private RelayCommand syncCommand;

        public AudioAnalyzerViewModel(IAudioAnalyzer analyzer)
        {
            if (analyzer == null)
                throw new ArgumentNullException("analyzer");

            this.analyzer = analyzer;

            Spectrum = new SpectrumViewModel(analyzer);
            Device = new DeviceViewModel(analyzer);
            VuMeter = new VuMeterViewModel(analyzer);
            BeatDetection = new BeatDetectionViewModel(analyzer);
            Generator = new GeneratorViewModel(analyzer);

            beatOffTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(60) };
            beatOffTimer.Tick += onBeatOff;

            analyzer.LevelChanged += onLevel;
            analyzer.BeatDetected += onBeat;
            analyzer.BpmChanged += onBpmChanged;
            analyzer.RunningChanged += onSyncConditionChanged;
            analyzer.SettingsChanged += onSyncConditionChanged;
        }

        public SpectrumViewModel Spectrum { get; }

        public DeviceViewModel Device { get; }

        public VuMeterViewModel VuMeter { get; }

        public BeatDetectionViewModel BeatDetection { get; }

        public GeneratorViewModel Generator { get; }

        public RelayCommand ToggleRunCommand
        {
            get { return toggleRunCommand ?? (toggleRunCommand = new RelayCommand(Device.ToggleRun)); }
        }


        public RelayCommand SyncCommand
        {
            get { return syncCommand ?? (syncCommand = new RelayCommand(analyzer.Sync, canSync)); }
        }


        private bool canSync()
        {
            if (!analyzer.IsRunning)
                return false;

            return analyzer.GeneratorEnabled || analyzer.ForecastCount > 0;
        }

        private void onSyncConditionChanged(object sender, EventArgs e)
        {
            SyncCommand.NotifyCanExecuteChanged();
        }

        public double LevelLeft
        {
            get { return levelLeft; }
            private set
            {
                if (levelLeft == value)
                    return;

                levelLeft = value;
                InvokePropertyChanged();
            }
        }

        public double LevelRight
        {
            get { return levelRight; }
            private set
            {
                if (levelRight == value)
                    return;

                levelRight = value;
                InvokePropertyChanged();
            }
        }

        /// <summary>
        /// Source of the beat currently being shown, null while none is.
        /// </summary>
        public EBeatSource? BeatSource
        {
            get { return beatSource; }
            private set
            {
                if (beatSource == value)
                    return;

                beatSource = value;
                InvokePropertyChanged();
                InvokePropertyChanged(nameof(BeatSourceText));
            }
        }

        /// <summary>explains the colour, so it does not have to be learned</summary>
        public string BeatSourceText
        {
            get
            {
                switch (beatSource)
                {
                    case EBeatSource.Detected: return T._("Detected beat");
                    case EBeatSource.Predicted: return T._("Predicted beat");
                    case EBeatSource.GeneratorMain: return T._("Generated beat (bar)");
                    case EBeatSource.GeneratorSub: return T._("Generated beat");
                    default: return T._("No beat");
                }
            }
        }

        /// <summary>detected or generated tempo, "--" while unknown</summary>
        public string BpmText
        {
            get
            {
                int bpm = analyzer.Bpm;
                return bpm > 0 ? bpm.ToString() : "--";
            }
        }

        private void onBpmChanged(object sender, EventArgs e)
        {
            InvokePropertyChanged(nameof(BpmText));
        }

        private void onLevel(object sender, LevelEventArgs args)
        {
            LevelLeft = args.VolumeL;
            LevelRight = args.VolumeR;
        }

        private void onBeat(object sender, BeatEventArgs args)
        {
            BeatSource = args.Source;
            beatOffTimer.Stop();
            beatOffTimer.Start();
        }

        private void onBeatOff(object sender, EventArgs e)
        {
            beatOffTimer.Stop();
            BeatSource = null;
        }

        protected override void OnDispose()
        {
            analyzer.LevelChanged -= onLevel;
            analyzer.BeatDetected -= onBeat;
            analyzer.BpmChanged -= onBpmChanged;
            analyzer.RunningChanged -= onSyncConditionChanged;
            analyzer.SettingsChanged -= onSyncConditionChanged;
            beatOffTimer.Stop();
            Spectrum.Dispose();
            Device.Dispose();
            VuMeter.Dispose();
            BeatDetection.Dispose();
            Generator.Dispose();
        }
    }
}
