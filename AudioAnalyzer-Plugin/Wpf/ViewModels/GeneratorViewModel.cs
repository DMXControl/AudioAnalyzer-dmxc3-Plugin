using System;
using System.Collections.Generic;
using AudioAnalyzer.Engine;
using CommunityToolkit.Mvvm.Input;
using LumosControlsWPF.Base;

namespace AudioAnalyzer.Wpf.ViewModels
{
    /// <summary>
    /// Beat generator: replaces the detection with a fixed tempo and rhythm.
    /// </summary>
    public class GeneratorViewModel : BasePropertyNotification
    {
        private readonly IAudioAnalyzer analyzer;

        private bool enabled;
        private int bpm;
        private int rhythm;
        private bool applying;
        private RelayCommand tapCommand;

        public GeneratorViewModel(IAudioAnalyzer analyzer)
        {
            this.analyzer = analyzer;

            reload();
            analyzer.SettingsChanged += onSettingsChanged;
        }

        /// <summary>switching this on disables the beat detection</summary>
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (!SetProperty(ref enabled, value) || applying)
                    return;

                analyzer.GeneratorEnabled = value;
            }
        }

        /// <summary>1..300</summary>
        public int Bpm
        {
            get { return bpm; }
            set
            {
                if (!SetProperty(ref bpm, value) || applying)
                    return;

                analyzer.GeneratorBpm = value;
            }
        }

        public IReadOnlyList<string> Rhythms
        {
            get { return analyzer.Rhythms; }
        }

        public int Rhythm
        {
            get { return rhythm; }
            set
            {
                if (!SetProperty(ref rhythm, value) || applying)
                    return;

                analyzer.Rhythm = value;
            }
        }

        public RelayCommand TapCommand
        {
            get { return tapCommand ?? (tapCommand = new RelayCommand(analyzer.Tap)); }
        }

        private void reload()
        {
            applying = true;
            try
            {
                SetProperty(ref enabled, analyzer.GeneratorEnabled, nameof(Enabled));
                SetProperty(ref bpm, analyzer.GeneratorBpm, nameof(Bpm));
                SetProperty(ref rhythm, analyzer.Rhythm, nameof(Rhythm));
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
