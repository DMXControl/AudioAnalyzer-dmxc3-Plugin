using System;
using AudioAnalyzer.AAEventArgs;


namespace AudioAnalyzer
{
    public partial class AudioAnalyzerEngine
    {

        public event EventHandler<BeatEventArgs> BeatDetected;
        public event EventHandler<LevelEventArgs> LevelChanged;
        public event EventHandler<SpectrumEventArgs> SpectrumChanged;

        /// <summary>band count or stereo mode changed, the band layout has to be rebuilt</summary>
        public event EventHandler<SpectrumCountEventArgs> SpectrumLayoutChanged;

        /// <summary>
        /// Raised whenever the tempo shown to the user changes - detected, generated or
        /// reset. Without this the GUI would have to poll, and a reset would go unnoticed.
        /// </summary>
        public event EventHandler BpmChanged;

        /// <summary>
        /// Raised when a setting was changed somewhere else, so a second surface can follow.
        /// </summary>
        public event EventHandler SettingsChanged;

        /// <summary>Raised when the analysis or the generator starts or stops.</summary>
        public event EventHandler RunningChanged;

        /// <summary>
        /// Raised once the device list exists. Enumerating it is slow enough that a remote
        /// kernel would deliver it asynchronously, so nobody may assume it is there already.
        /// </summary>
        public event EventHandler DevicesChanged;

        private void OnBeatDetected(int majorMinor, EBeatSource source)
        {
            if (BeatDetected != null)
                BeatDetected(this, new BeatEventArgs(majorMinor, source));
        }

        private void OnLevelChanged(float volL, float volR)
        {
            if (LevelChanged != null)
                LevelChanged(this, new LevelEventArgs(volL, volR));
        }

        private void OnSpectrumChanged(double[] dbsubLevel, ESpectrumChannel channel)
        {
            if (SpectrumChanged != null)
                SpectrumChanged(this, new SpectrumEventArgs(dbsubLevel, channel));
        }

        private void OnSpectrumLayoutChanged(int count)
        {
            if (SpectrumLayoutChanged != null)
                SpectrumLayoutChanged(this, new SpectrumCountEventArgs(count, stereoSpectrum));
        }

        internal void OnBpmChanged()
        {
            if (BpmChanged != null)
                BpmChanged(this, EventArgs.Empty);
        }

        internal void OnSettingsChanged()
        {
            if (SettingsChanged != null)
                SettingsChanged(this, EventArgs.Empty);
        }

        internal void OnRunningChanged()
        {
            if (RunningChanged != null)
                RunningChanged(this, EventArgs.Empty);
        }

        internal void OnDevicesChanged()
        {
            if (DevicesChanged != null)
                DevicesChanged(this, EventArgs.Empty);
        }
    }
}
