using System;
using AudioAnalyzer.Engine;
using LumosControlsWPF.Base;

namespace AudioAnalyzer.Wpf.ViewModels
{
    /// <summary>VU meter settings: gain, peak sink and its hold time.</summary>
    public class VuMeterViewModel : BasePropertyNotification
    {
        private readonly IAudioAnalyzer analyzer;

        private int gain;
        private bool peakHold;
        private int peakHoldTime;
        private bool applying;

        public VuMeterViewModel(IAudioAnalyzer analyzer)
        {
            this.analyzer = analyzer;

            reload();
            analyzer.SettingsChanged += onSettingsChanged;
        }

        /// <summary>0..100</summary>
        public int Gain
        {
            get { return gain; }
            set
            {
                if (!SetProperty(ref gain, value) || applying)
                    return;

                analyzer.VuGain = value;
            }
        }

        public bool PeakHold
        {
            get { return peakHold; }
            set
            {
                if (!SetProperty(ref peakHold, value) || applying)
                    return;

                analyzer.PeakHold = value;
            }
        }

        /// <summary>hold time in ms, 10..500</summary>
        public int PeakHoldTime
        {
            get { return peakHoldTime; }
            set
            {
                if (!SetProperty(ref peakHoldTime, value) || applying)
                    return;

                analyzer.PeakHoldTime = value;
            }
        }

        private void reload()
        {
            applying = true;
            try
            {
                SetProperty(ref gain, analyzer.VuGain, nameof(Gain));
                SetProperty(ref peakHold, analyzer.PeakHold, nameof(PeakHold));
                SetProperty(ref peakHoldTime, analyzer.PeakHoldTime, nameof(PeakHoldTime));
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
