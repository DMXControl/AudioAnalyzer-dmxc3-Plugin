using System;
using System.Collections.Generic;
using AudioAnalyzer.Engine;
using CommunityToolkit.Mvvm.Input;
using LumosControlsWPF.Base;
using LumosLIB.Tools.I18n;

namespace AudioAnalyzer.Wpf.ViewModels
{
    /// <summary>Beat detection: method, sensitivity, tempo limit and prediction.</summary>
    public class BeatDetectionViewModel : BasePropertyNotification
    {
        private readonly IAudioAnalyzer analyzer;

        private int algorithm;
        private int sensitivity;
        private bool maxBpmLimit;
        private int maxBpm;
        private int forecastCount;
        private bool doubleSpeed;
        private bool halfSpeed;
        private bool applying;
        private RelayCommand resetCommand;

        public BeatDetectionViewModel(IAudioAnalyzer analyzer)
        {
            this.analyzer = analyzer;

            reload();
            analyzer.SettingsChanged += onSettingsChanged;
        }

        public IReadOnlyList<string> Algorithms
        {
            get { return analyzer.BeatAlgorithms; }
        }

        public int Algorithm
        {
            get { return algorithm; }
            set
            {
                if (!SetProperty(ref algorithm, value) || applying)
                    return;

                analyzer.BeatAlgorithm = value;
                InvokePropertyChanged(nameof(SensitivityRelevant));
            }
        }

        /// <summary>two of the four methods ignore the sensitivity</summary>
        public bool SensitivityRelevant
        {
            get { return analyzer.SensitivityRelevant; }
        }

        public int Sensitivity
        {
            get { return sensitivity; }
            set
            {
                if (!SetProperty(ref sensitivity, value) || applying)
                    return;

                analyzer.Sensitivity = value;
            }
        }

        public bool MaxBpmLimit
        {
            get { return maxBpmLimit; }
            set
            {
                if (!SetProperty(ref maxBpmLimit, value) || applying)
                    return;

                analyzer.MaxBpmLimit = value;
            }
        }

        public int MaxBpm
        {
            get { return maxBpm; }
            set
            {
                if (!SetProperty(ref maxBpm, value) || applying)
                    return;

                analyzer.MaxBpm = value;
            }
        }

        /// <summary>0 = off, 100 = unlimited, matching the old slider</summary>
        public int ForecastCount
        {
            get { return forecastCount; }
            set
            {
                if (!SetProperty(ref forecastCount, value) || applying)
                    return;

                analyzer.ForecastCount = value;
                InvokePropertyChanged(nameof(ForecastText));
            }
        }

        public string ForecastText
        {
            get
            {
                if (forecastCount <= 0)
                    return T._("no additional beats");
                if (forecastCount >= 100)
                    return T._("unlimited additional beats");

                return T._n("max. {0} additional beat", "max. {0} additional beats",
                            forecastCount, forecastCount);
            }
        }

        public bool DoubleSpeed
        {
            get { return doubleSpeed; }
            set
            {
                if (!SetProperty(ref doubleSpeed, value) || applying)
                    return;

                analyzer.DoubleSpeed = value;
            }
        }

        public bool HalfSpeed
        {
            get { return halfSpeed; }
            set
            {
                if (!SetProperty(ref halfSpeed, value) || applying)
                    return;

                analyzer.HalfSpeed = value;
            }
        }

        public RelayCommand ResetCommand
        {
            get { return resetCommand ?? (resetCommand = new RelayCommand(analyzer.ResetBeatStatistics)); }
        }

        private void reload()
        {
            applying = true;
            try
            {
                SetProperty(ref algorithm, analyzer.BeatAlgorithm, nameof(Algorithm));
                SetProperty(ref sensitivity, analyzer.Sensitivity, nameof(Sensitivity));
                SetProperty(ref maxBpmLimit, analyzer.MaxBpmLimit, nameof(MaxBpmLimit));
                SetProperty(ref maxBpm, analyzer.MaxBpm, nameof(MaxBpm));
                SetProperty(ref forecastCount, analyzer.ForecastCount, nameof(ForecastCount));
                SetProperty(ref doubleSpeed, analyzer.DoubleSpeed, nameof(DoubleSpeed));
                SetProperty(ref halfSpeed, analyzer.HalfSpeed, nameof(HalfSpeed));

                InvokePropertyChanged(nameof(SensitivityRelevant));
                InvokePropertyChanged(nameof(ForecastText));
            }
            finally
            {
                applying = false;
            }
        }

        private void onSettingsChanged(object sender, EventArgs e)
        {
            reload();
        }

        protected override void OnDispose()
        {
            analyzer.SettingsChanged -= onSettingsChanged;
        }
    }
}
