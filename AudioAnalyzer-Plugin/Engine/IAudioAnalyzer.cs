using System;
using System.Collections.Generic;
using AudioAnalyzer.AAEventArgs;

namespace AudioAnalyzer.Engine
{
    /// <summary>
    /// Everything the GUI needs from the analysis, and nothing else.
    /// </summary>
    public interface IAudioAnalyzer
    {
        #region Messwerte (Analyse -> GUI)

        event EventHandler<SpectrumEventArgs> SpectrumChanged;
        event EventHandler<LevelEventArgs> LevelChanged;
        event EventHandler<BeatEventArgs> BeatDetected;

        /// <summary>band count or stereo mode changed, the band layout has to be rebuilt</summary>
        event EventHandler<SpectrumCountEventArgs> SpectrumLayoutChanged;

        event EventHandler BpmChanged;

        /// <summary>new mood values are available (raised on every analysis step)</summary>
        event EventHandler MoodChanged;
        event EventHandler RunningChanged;

        /// <summary>a setting was changed elsewhere - re-read what you display</summary>
        event EventHandler SettingsChanged;

        /// <summary>detected or generated tempo, 0 while unknown</summary>
        int Bpm { get; }

        bool IsRunning { get; }

        /// <summary>tempo as sent to DMXControl: half / double speed applied, 0 while unknown</summary>
        double OutputBpm { get; }

        /// <summary>key, chord, mood, energy and brightness of the music</summary>
        MoodInfo Mood { get; }

        #endregion

        #region Transport

        void ToggleRun();

        /// <summary>starts or stops the analysis; does nothing if it is in that state already</summary>
        void SetRunning(bool run);

        #endregion

        #region Geraet

        /// <summary>
        /// The device list is not available immediately - it is enumerated when the
        /// analysis starts up, and a remote kernel would deliver it asynchronously.
        /// Read Devices again whenever this fires.
        /// </summary>
        event EventHandler DevicesChanged;

        IReadOnlyList<AudioDeviceInfo> Devices { get; }

        /// <summary>id of the selected device, null if none</summary>
        string SelectedDeviceId { get; set; }

        /// <summary>input channels of the selected device, empty unless ASIO</summary>
        IReadOnlyList<string> InputChannels { get; }

        int SelectedInputChannel { get; set; }

        /// <summary>main gain, 0..100</summary>
        int MainGain { get; set; }

        #endregion

        #region Spektrum

        IReadOnlyList<int> AvailableBandCounts { get; }

        int BandCount { get; set; }

        bool StereoSpectrum { get; set; }

        /// <summary>level correction for the spectrum, 0..100</summary>
        int SpectrumGain { get; set; }

        /// <summary>frequency range of a band as a label, e.g. "320-453 Hz"</summary>
        string GetBandRangeLabel(int bandNumber);

        #endregion

        #region VU-Meter

        /// <summary>0..100</summary>
        int VuGain { get; set; }

        /// <summary>"peak sink" - hold the peak and let it fall slowly</summary>
        bool PeakHold { get; set; }

        /// <summary>hold time in ms, 10..500</summary>
        int PeakHoldTime { get; set; }

        #endregion

        #region Beat-Erkennung

        IReadOnlyList<string> BeatAlgorithms { get; }

        int BeatAlgorithm { get; set; }

        /// <summary>false for the methods that ignore the sensitivity</summary>
        bool SensitivityRelevant { get; }

        /// <summary>0..100</summary>
        int Sensitivity { get; set; }

        bool MaxBpmLimit { get; set; }

        /// <summary>30..330</summary>
        int MaxBpm { get; set; }

        /// <summary>how many beats may be predicted, 0..100 (100 = unlimited)</summary>
        int ForecastCount { get; set; }

        bool DoubleSpeed { get; set; }

        bool HalfSpeed { get; set; }

        /// <summary>
        /// prefer the faster of two tempi that differ by a factor of two (metal, blast beats).
        /// Off: tempi near 120 BPM are preferred, fast music is often shown at half speed.
        /// </summary>
        bool FavorFastTempo { get; set; }

        void ResetBeatStatistics();

        #endregion

        #region Beat-Generator

        /// <summary>generator instead of detection</summary>
        bool GeneratorEnabled { get; set; }

        /// <summary>1..300</summary>
        int GeneratorBpm { get; set; }

        IReadOnlyList<string> Rhythms { get; }

        int Rhythm { get; set; }

        /// <summary>restart the generator or the prediction, to sync to the music</summary>
        void Sync();

        void Tap();

        #endregion
    }
}
