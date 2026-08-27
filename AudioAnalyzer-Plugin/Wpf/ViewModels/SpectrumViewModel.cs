using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AudioAnalyzer.AAEventArgs;
using AudioAnalyzer.Engine;
using LumosControlsWPF.Base;
using LumosLIB.Tools.I18n;

namespace AudioAnalyzer.Wpf.ViewModels
{
    /// <summary>
    /// Spectrum settings and the live band values.
    /// </summary>
    public class SpectrumViewModel : BasePropertyNotification
    {
        private readonly IAudioAnalyzer analyzer;

        private int bandCount;
        private bool stereo;
        private int gain;

        public SpectrumViewModel(IAudioAnalyzer analyzer)
        {
            this.analyzer = analyzer;

            Bands = new ObservableCollection<SpectrumBandViewModel>();
            BandsRight = new ObservableCollection<SpectrumBandViewModel>();

            bandCount = analyzer.BandCount;
            stereo = analyzer.StereoSpectrum;
            gain = analyzer.SpectrumGain;
            rebuildBands();

            analyzer.SpectrumChanged += onSpectrum;
            analyzer.SpectrumLayoutChanged += onSpectrumLayout;
            analyzer.SettingsChanged += onSettingsChanged;
        }

        public IReadOnlyList<int> AvailableBandCounts
        {
            get { return analyzer.AvailableBandCounts; }
        }

        public ObservableCollection<SpectrumBandViewModel> Bands { get; }

        /// <summary>only filled in stereo mode; Bands then carries the left channel</summary>
        public ObservableCollection<SpectrumBandViewModel> BandsRight { get; }

        public int BandCount
        {
            get { return bandCount; }
            set
            {
                if (!SetProperty(ref bandCount, value))
                    return;

                analyzer.BandCount = value;   // meldet SpectrumLayoutChanged zurueck
            }
        }

        public bool Stereo
        {
            get { return stereo; }
            set
            {
                if (!SetProperty(ref stereo, value))
                    return;

                analyzer.StereoSpectrum = value;
                InvokePropertyChanged(nameof(PrimaryChannelLabel));
            }
        }

        /// <summary>
        /// What the first row of bands actually carries: the downmix, or the left channel.
        /// Matches how the input sources are named.
        /// </summary>
        public string PrimaryChannelLabel
        {
            get { return stereo ? "L" : T._("Mix"); }
        }

        /// <summary>"Level correction", 0..100</summary>
        public int Gain
        {
            get { return gain; }
            set
            {
                if (!SetProperty(ref gain, value))
                    return;

                analyzer.SpectrumGain = value;
            }
        }

        private void onSpectrumLayout(object sender, SpectrumCountEventArgs args)
        {
            SetProperty(ref bandCount, args.Count, nameof(BandCount));
            SetProperty(ref stereo, args.Stereo, nameof(Stereo));
            InvokePropertyChanged(nameof(PrimaryChannelLabel));
            rebuildBands();
        }

        private void onSettingsChanged(object sender, EventArgs e)
        {
            SetProperty(ref gain, analyzer.SpectrumGain, nameof(Gain));
        }

        private void onSpectrum(object sender, SpectrumEventArgs args)
        {
            var target = args.Channel == ESpectrumChannel.Right ? BandsRight : Bands;

            for (int i = 0; i < target.Count && i < args.SubLevel.Length; i++)
            {
                target[i].Level = args.SubLevel[i];
            }
        }

        private void rebuildBands()
        {
            syncBands(Bands, bandCount);
            syncBands(BandsRight, stereo ? bandCount : 0);
        }

        private void syncBands(ObservableCollection<SpectrumBandViewModel> bands, int count)
        {
            while (bands.Count > count)
            {
                bands[bands.Count - 1].Dispose();
                bands.RemoveAt(bands.Count - 1);
            }
            while (bands.Count < count)
            {
                bands.Add(new SpectrumBandViewModel(bands.Count + 1));
            }

            foreach (var b in bands)
            {
                b.Range = analyzer.GetBandRangeLabel(b.Number);
            }
        }

        protected override void OnDispose()
        {
            analyzer.SpectrumChanged -= onSpectrum;
            analyzer.SpectrumLayoutChanged -= onSpectrumLayout;
            analyzer.SettingsChanged -= onSettingsChanged;
        }
    }
}
