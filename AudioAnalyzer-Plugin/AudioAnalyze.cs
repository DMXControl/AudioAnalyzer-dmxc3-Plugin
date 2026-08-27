using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using AudioAnalyzer.AAEventArgs;
using AudioAnalyzer.Engine;
using NAudio.CoreAudioApi;
using NAudio.Wave;


namespace AudioAnalyzer
{
    /// <summary>
    /// The audio analysis. No window, no controls - this is the kernel half of the plugin.
    ///
    /// It used to be a WinForms ToolWindow, and the settings lived in its controls: a slider
    /// position WAS the gain, a checkbox WAS the flag. That made a second surface impossible
    /// without the window existing, so the values now live in plain fields and the surfaces
    /// reach them through IAudioAnalyzer.
    ///
    /// Still WinForms-shaped in one place: the timers are System.Windows.Forms.Timer, which
    /// ticks on the GUI thread. The whole event chain - and the WPF view models at the end of
    /// it - relies on that; none of them marshal. Whatever replaces these timers when the
    /// analysis moves into the kernel has to deliver on the GUI thread just the same.
    /// </summary>
    public partial class AudioAnalyzerEngine : IAudioAnalyzer, IDisposable
    {
        public AudioAnalyzerEngine()
        {
            tones[0] = A0;
            tones[1] = AIS0;
            tones[2] = B0;
            tones[3] = C1;
            tones[4] = CIS1;
            tones[5] = D1;
            tones[6] = DIS1;
            tones[7] = E1;
            tones[8] = F1;
            tones[9] = FIS1;
            tones[10] = G1;
            tones[11] = GIS1;
            tones[12] = A1;
            tones[13] = AIS1;
            tones[14] = B1;
            tones[15] = C2;
            tones[16] = CIS2;
            tones[17] = D2;
            tones[18] = DIS2;
            tones[19] = E2;
            tones[20] = F2;
            tones[21] = FIS2;
            tones[22] = G2;
            tones[23] = GIS2;
            tones[24] = A2;
            tones[25] = AIS2;
            tones[26] = B2;
            tones[27] = C3;
            tones[28] = CIS3;
            tones[29] = D3;
            tones[30] = DIS3;
            tones[31] = E3;
            tones[32] = F3;
            tones[33] = FIS3;
            tones[34] = G3;
            tones[35] = GIS3;
            tones[36] = A3;
            tones[37] = AIS3;
            tones[38] = B3;
            tones[39] = C4;
            tones[40] = CIS4;
            tones[41] = D4;
            tones[42] = DIS4;
            tones[43] = E4;
            tones[44] = F4;
            tones[45] = FIS4;
            tones[46] = G4;
            tones[47] = GIS4;
            tones[48] = A4;
            tones[49] = AIS4;
            tones[50] = B4;
            tones[51] = C5;
            tones[52] = CIS5;
            tones[53] = D5;
            tones[54] = DIS5;
            tones[55] = E5;
            tones[56] = F5;
            tones[57] = FIS5;
            tones[58] = G5;
            tones[59] = GIS5;
            tones[60] = A5;
            tones[61] = AIS5;
            tones[62] = B5;
            tones[63] = C6;
            tones[64] = CIS6;
            tones[65] = D6;
            tones[66] = DIS6;
            tones[67] = E6;
            tones[68] = F6;
            tones[69] = FIS6;
            tones[70] = G6;
            tones[71] = GIS6;
            tones[72] = A6;
            tones[73] = AIS6;
            tones[74] = B6;
            tones[75] = C7;
            tones[76] = CIS7;
            tones[77] = D7;
            tones[78] = DIS7;
            tones[79] = E7;
            tones[80] = F7;
            tones[81] = FIS7;
            tones[82] = G7;
            tones[83] = GIS7;
            tones[84] = A7;
            tones[85] = AIS7;
            tones[86] = B7;
            tones[87] = C8;
            tones[88] = CIS8;
            tones[89] = D8;
            tones[90] = DIS8;
            tones[91] = E8;
            tones[92] = F8;
            tones[93] = FIS8;
            tones[94] = G8;
            tones[95] = GIS8;
            tones[96] = A8;
            tones[97] = AIS8;
            tones[98] = B8;
            tones[99] = C9;
            tones[100] = CIS9;
            tones[101] = D9;
            tones[102] = DIS9;
            tones[103] = E9;
            tones[104] = F9;
            tones[105] = FIS9;
            tones[106] = G9;
            tones[107] = GIS9;
            tones[108] = A9;
            tones[109] = AIS9;
            tones[110] = B9;
            tones[111] = C10;
            tones[112] = CIS10;
            tones[113] = D10;
            tones[114] = DIS10;

            startBand = Array.IndexOf(tones, E2);

            initializeAnalysis();
            initializeDevices();
        }

        /// <summary>
        /// Everything the analysis needs to run.
        /// </summary>
        private void initializeAnalysis()
        {
            beatClock = new Stopwatch();

            // Takt der Analyse
            beatTimer = new Timer();
            beatTimer.Interval = beatInterval;
            beatTimer.Tick += new EventHandler(beatTimer_Tick);

            beatclearTimer = new Timer();
            beatclearTimer.Interval = (int)(60000 / maxBpmEffective);
            beatclearTimer.Tick += new EventHandler(beatclearTimer_Tick);

            startTimer = new Timer();
            startTimer.Interval = 1000;
            startTimer.Tick += new EventHandler(startTimer_Tick);

            addBeatTimer = new Timer();
            addBeatTimer.Interval = 1000;
            addBeatTimer.Tick += new EventHandler(addBeatTimer_Tick);

            // Generator
            mainBeatTimer = new Timer();
            mainBeatTimer.Interval = 4000;
            mainBeatTimer.Tick += new EventHandler(mainBeatTimer_Tick);

            subBeatTimer = new Timer();
            subBeatTimer.Interval = 4000;
            subBeatTimer.Tick += new EventHandler(subBeatTimer_Tick);

            tapTimeout = new Timer();
            tapTimeout.Interval = 4000;
            tapTimeout.Tick += new EventHandler(tapTimeout_Tick);

            tapWatch = new Stopwatch();
            tapValues = new long[4];

            // Beat-Statistik
            sortedindex = new List<int>(256);
            beatMemory = new TimeSpan[200];
            bpmMemory = new int[200];
            bpmDistribution = new int[20];
            beatClock.Start();
            actualBeatTime = beatClock.ElapsedMilliseconds;
            newBPM();

            // Aufnahme und FFT
            _aggregatorLeft = new LumosLIB.Tools.FastFourierTransform.SampleAggregator();
            _aggregatorLeft.NotificationCount = aggregatorNotificationCount;
            _aggregatorLeft.MaximumCalculated += AggregatorLeft_MaximumCalculated;
            _aggregatorRight = new LumosLIB.Tools.FastFourierTransform.SampleAggregator();
            _aggregatorRight.NotificationCount = aggregatorNotificationCount;
            _aggregatorRight.MaximumCalculated += AggregatorRight_MaximumCalculated;
            _fftBuffer = new FFTCircularBuffer(fftLength);
            _fftBufferLeft = new FFTCircularBuffer(fftLength);
            _fftBufferRight = new FFTCircularBuffer(fftLength);

            configSubBands();
            updatePeakHoldSteps();

            applyAlgorithm(algorithm);
            setUsedBands(usedbands);
            setRhythm((int)generatorRhythm);
            computeMainAndSubBeatTimes(generatorBPM, generatorRhythm);
        }

        /// <summary>
        /// Enumerates the audio devices, in the constructor: anything asking for the list -
        /// the WPF surface, and later the kernel - must not have to wait for a window.
        /// </summary>
        private void initializeDevices()
        {
            try
            {
                using (MMDeviceEnumerator enumerator = new MMDeviceEnumerator()) {
                    foreach (MMDevice wasapi in enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active)) {
                        deviceList.Add(new WasapiSoundSource(wasapi));
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Could not enumerate WASAPI devices: {0}", e, e.Message);
            }

            try
            {
                if (AsioOut.isSupported()) {
                    foreach (string asio in AsioOut.GetDriverNames()) {
                        deviceList.Add(new AsioSoundSource(asio));
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Could not enumerate ASIO drivers: {0}", e, e.Message);
            }

            selectWantedDevice();
            OnDevicesChanged();
        }

        private void AudioStream_SamplesAvailable(object sender, float[] samples, int count, int channels, int samplerate) {
            if (this.sampleRate != samplerate) {
                this.sampleRate = samplerate;
                configSubBands();
                updatePeakHoldSteps();
            }
            float left = 0;
            for (int i = 0; i < count; ++i) {
                // alles jenseits der ersten beiden Kanäle (z.B. 5.1-Loopback) wird ignoriert
                int channel = i % channels;
                if (channel == 0) {
                    _aggregatorLeft.Add(samples[i]);
                    left = samples[i];
                    if (channels == 1) {
                        // Mono
                        _fftBuffer.Add(samples[i]);
                        _fftBufferLeft.Add(samples[i]);
                        _fftBufferRight.Add(samples[i]);
                    }
                } else if (channel == 1) {
                    _aggregatorRight.Add(samples[i]);
                    _fftBuffer.Add((left * 0.5f) + (samples[i] * 0.5f));
                    _fftBufferLeft.Add(left);
                    _fftBufferRight.Add(samples[i]);
                }
            }
        }

        private void AggregatorLeft_MaximumCalculated(object sender, LumosLIB.Tools.FastFourierTransform.MaxSampleEventArgs e) {
            ComputeLevel(e.MaxSample, 0);
        }

        private void AggregatorRight_MaximumCalculated(object sender, LumosLIB.Tools.FastFourierTransform.MaxSampleEventArgs e) {
            ComputeLevel(e.MaxSample, 1);
        }

        /// <summary>
        /// wait short time to fill history buffers before computing values
        /// </summary>
        void startTimer_Tick(object sender, EventArgs e)
        {
            startTimer.Enabled = false;
            starting = false;
        }

        #region beat

        /// <summary>
        /// Main timer task, computes the beat every 20 ms, level and spectrum every 60 ms.
        /// </summary>
        void beatTimer_Tick(object sender, EventArgs e)
        {
            // here we gather info about the stream, when it is playing...
            if (audioStream.Playing)
            {
                // the stream is still playing...
                computeSubbands();

                // compute Beat
                ComputeBeat();

                if (draw == 0)
                {
                    sendLevel();
                    sendSpectrum();
                }
                draw++;
                if (draw >= 3)
                    draw = 0;
            }
            else
            {
                // the stream is NOT playing anymore...
                beatTimer.Stop();
                return;
            }
        }


        /// <summary>
        /// computes the level values
        /// </summary>
        private void ComputeLevel(float newLevel, int channel)
        {
            float speed = (float)0.15; // Geschwindigkeit des PeakSink, zwischen 0.03 und 0.3

            float levelAlt;

            levelAlt = level[channel];

            // History zur Ausgabe
            levelhistory[channel, 3] = levelhistory[channel, 2];
            levelhistory[channel, 2] = levelhistory[channel, 1];
            levelhistory[channel, 1] = levelAlt;

            level[channel] = (float)(newLevel * regler2 * reglerVU);

            if (level[channel] > 1.0)
                level[channel] = (float)1.0;

            if (peakHold)
            {
                if (levelAlt > level[channel])
                {
                    if (peakWait[channel] < peakWaitMax)
                    {
                        peakWait[channel]++;
                    }
                    else
                    {
                        levelAlt = levelAlt - speed;
                    }
                    level[channel] = levelAlt;
                }
                else
                {
                    peakWait[channel] = 0;
                }
            }

            if (level[channel] < 0.0)
                level[channel] = (float)0.0;

            // History zur Ausgabe (aktueller Wert)
            levelhistory[channel, 0] = level[channel];
        }

        /// <summary>
        /// starts the beat detection methods and then the statistics
        /// </summary>
        private void ComputeBeat()
        {

            if (algorithm == 0)
            {
                beat_detected = Beat1();
            }
            else if (algorithm == 1)
            {
                beat_detected = Beat3();
            }
            else if (algorithm == 2)
            {
                beat_detected = Beat4();
            }
            else if (algorithm == 3)
            {
                int quali = 0;

                if (Beat1())
                    quali += 1;

                if (Beat3())
                    quali += 1;

                if (Beat4())
                    quali += 1;

                if (quali >= autoSensitivity)
                    beat_detected = true;
            }

            // Beat entdeckt !
            if (algorithm <= 3)  //TODO: change to 2 and add automatic to the new advanced algorithm
            {
                if (beatclearTimer.Enabled == false && beat_detected == true && starting == false)
                {
                    OnBeatDetected(MinorBeat, EBeatSource.Detected);

                    beatclearTimer.Start();

                    lastBeatTime = actualBeatTime;
                    actualBeatTime = beatClock.ElapsedMilliseconds;

                    doBeatStatisticsSimple(actualBeatTime - lastBeatTime);

                    // Vorhersage aktiv
                    if (forecast && baseBeat >= 0)
                    {
                        addedBeats = 0;
                        addBeatTimer.Stop();
                        addBeatTimer.Start();
                    }
                }
            }
            else
            {
                doBeatStatisticsAdvanced();
            }
        }

        /// <summary>
        /// resets beat_detected
        /// </summary>
        void beatclearTimer_Tick(object sender, EventArgs e)
        {
            beatclearTimer.Stop();
            beat_detected = false;
        }

        /// <summary>
        /// sends an artificial beat signal
        /// </summary>
        void addBeatTimer_Tick(object sender, EventArgs e)
        {
            if (addedBeats < nbForecast && forecast && baseBeat > 0)
            {
                // send beat
                OnBeatDetected(MinorBeat, EBeatSource.Predicted);

                addedBeats += 1;

                if (addedBeats == 1)
                {
                    if (doubleSpeed)
                        addBeatTimer.Interval = (int)(30000 / baseBeat);
                    else if (halfSpeed)
                        addBeatTimer.Interval = (int)(120000 / baseBeat);
                    else
                        addBeatTimer.Interval = (int)(60000 / baseBeat);
                }

                beatclearTimer.Start();
            }
            else
            {
                addBeatTimer.Stop();
            }
        }

        #endregion

        #region Device selection
        /// <summary>
        /// Selects a device by list position. Replaces devicesBox_SelectedIndexChanged: it
        /// also refills the input channels, which only ASIO devices have.
        /// </summary>
        private void setDeviceIndex(int index)
        {
            if (audioStream != null)
                stopAudioAnalysis();

            selectedDeviceIndex = (index >= 0 && index < deviceList.Count) ? index : -1;

            inputChannelNames.Clear();
            selectedInputChannelIndex = -1;

            AbstractSoundSource source = selectedDevice;
            if (source == null)
                return;

            AsioSoundSource asio = source as AsioSoundSource;
            if (asio != null)
                inputChannelNames.AddRange(asio.getInputNames());

            if (inputChannelNames.Count > 0)
                setInputChannelIndex(0);

            OnSettingsChanged();
        }

        /// <summary>
        /// Selects an input channel. Like the old inputsBox handler, changing it stops a
        /// running recording - the channel is picked when the stream starts.
        /// </summary>
        private void setInputChannelIndex(int index)
        {
            if (audioStream != null)
                stopAudioAnalysis();

            selectedInputChannelIndex =
                (index >= 0 && index < inputChannelNames.Count) ? index : -1;
        }

        private AbstractSoundSource selectedDevice
        {
            get
            {
                return (selectedDeviceIndex >= 0 && selectedDeviceIndex < deviceList.Count)
                    ? deviceList[selectedDeviceIndex]
                    : null;
            }
        }

        /// <summary>
        /// selects the remembered device, falling back to the first entry
        /// </summary>
        private void selectWantedDevice()
        {
            if (deviceList.Count == 0)
                return;

            int idx = 0;
            if (!String.IsNullOrEmpty(wantedDeviceId))
            {
                for (int i = 0; i < deviceList.Count; i++)
                {
                    if (String.Equals(deviceList[i].Id, wantedDeviceId))
                    {
                        idx = i;
                        break;
                    }
                }
            }

            if (selectedDeviceIndex != idx)
                setDeviceIndex(idx);

            if (wantedInputChannel >= 0 && wantedInputChannel < inputChannelNames.Count)
                setInputChannelIndex(wantedInputChannel);
        }

        /// <summary>available audio devices as plain data</summary>
        internal List<AudioDeviceInfo> getDevices()
        {
            var list = new List<AudioDeviceInfo>();
            foreach (AbstractSoundSource s in deviceList)
            {
                list.Add(new AudioDeviceInfo(s.Id, s.ToString()));
            }
            return list;
        }

        /// <summary>input channel names of the selected device, empty for non-ASIO</summary>
        internal List<string> getInputChannelNames()
        {
            return new List<string>(inputChannelNames);
        }

        /// <summary>
        /// identifier of the selected device, or the remembered one while the list is empty
        /// </summary>
        internal string getSelectedDeviceId()
        {
            AbstractSoundSource s = selectedDevice;
            return s != null ? s.Id : wantedDeviceId;
        }

        internal int getSelectedInputChannel()
        {
            return selectedInputChannelIndex >= 0 ? selectedInputChannelIndex : wantedInputChannel;
        }

        /// <summary>
        /// remembers the wanted device and applies it if the list is already filled
        /// </summary>
        internal void setDevice(string deviceId, int inputChannel)
        {
            wantedDeviceId = deviceId;
            wantedInputChannel = inputChannel;

            selectWantedDevice();
        }

        #endregion

        #region Transport

        /// <summary>starts or stops</summary>
        internal void toggleRun()
        {
            if (generatorMode == false)
            {
                if (running)
                    stopAudioAnalysis();
                else
                    startAudioAnalysis();
            }
            else
            {
                if (running == false)
                    startGenerator();
                else
                    stopGenerator();
            }
        }

        /// <summary>
        /// starts the AudioAnalyze
        /// </summary>
        private void startAudioAnalysis()
        {
            resetStatistics();

            AbstractSoundSource source = selectedDevice;
            if (source == null) return;
            audioStream = source;

            AsioSoundSource asio = source as AsioSoundSource;
            if (asio != null) {
                asio.InputChannel = selectedInputChannelIndex;
            }

            // Starts the beat detection
            draw = 0;
            if (audioStream.StartRecord()) {
                audioStream.SamplesAvailable += AudioStream_SamplesAvailable;

                _aggregatorLeft.Reset();
                _aggregatorRight.Reset();
                _fftBuffer.Reset();
                _fftBufferLeft.Reset();
                _fftBufferRight.Reset();

                starting = true;
                startTimer.Enabled = true;

                beatTimer.Start();

                running = true;
            }
            else
            {
                log.Error("Could not start recording on device {0}", source.ToString());
                audioStream = null;
            }
        }

        /// <summary>
        /// stops the AudioAnalyze
        /// </summary>
        internal void stopAudioAnalysis()
        {
            if (audioStream != null)
            {
                audioStream.StopRecord();
                audioStream.SamplesAvailable -= AudioStream_SamplesAvailable;
                beatTimer.Stop();
                addBeatTimer.Stop();

                audioStream = null;

                running = false;

                level[0] = 0.0F;
                level[1] = 0.0F;
                for (int c = 0; c < 2; c++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        levelhistory[c, i] = 0.0F;
                    }
                    peakWait[c] = 0;
                }
                OnLevelChanged(level[0], level[1]);

                for (int i = 0; i < usedbands;i++ )
                {
                    dbSubLevel[i] = 0.0F;
                    dbSubLevelLeft[i] = 0.0F;
                    dbSubLevelRight[i] = 0.0F;
                }
                if (stereoSpectrum)
                {
                    OnSpectrumChanged(dbSubLevelLeft, ESpectrumChannel.Left);
                    OnSpectrumChanged(dbSubLevelRight, ESpectrumChannel.Right);
                }
                else
                {
                    OnSpectrumChanged(dbSubLevel, ESpectrumChannel.Mono);
                }

                resetStatistics();
            }
        }

        /// <summary>
        /// starts the beatGenerator
        /// </summary>
        private void startGenerator()
        {
            subBeatTimer.Stop();
            computeMainAndSubBeatTimes(generatorBPM, generatorRhythm);
            subbeatCount = 0;
            running = true;
            mainBeatTimer_Tick(this, new EventArgs());
        }

        /// <summary>
        /// stops the beatGenerator
        /// </summary>
        private void stopGenerator()
        {
            running = false;
            mainBeatTimer.Stop();
            subBeatTimer.Stop();
        }

        /// <summary>
        /// changes the mode from analysis to generator (and back)
        /// </summary>
        private void changeMode(bool generator)
        {
            if (running)
            {
                if (generator)
                {
                    stopAudioAnalysis();
                    startGenerator();
                    generatorMode = true;
                }
                else
                {
                    stopGenerator();
                    startAudioAnalysis();
                    generatorMode = false;
                }
            }
            else
            {
                generatorMode = generator;
            }

            OnBpmChanged();
        }

        #endregion

        #region Appy settings

        private static int clamp(int value, int min, int max)
        {
            return value < min ? min : (value > max ? max : value);
        }

        private void applyMainGain()
        {
            regler1 = 100 * scaleToFactor(mainGainValue, 0.25, 5.0);
        }

        private void applyVuGain()
        {
            reglerVU = scaleToFactor(vuGainValue, 0.5, 8.0);
        }

        private void applySpectrumGain()
        {
            reglerSpectrum = scaleToFactor(spectrumGainValue, 0.25, 20.0);
        }

        private void applyPeakHoldTime()
        {
            peakHoldTime = (float)peakHoldTimeMs / 1000;
            updatePeakHoldSteps();
        }

        private void applySensitivity()
        {
            sensitivity = 3.0 + ((double)sensitivityValue / 50);

            if (sensitivityValue < 25)
                autoSensitivity = 4;
            else if (sensitivityValue < 50)
                autoSensitivity = 3;
            else if (sensitivityValue < 75)
                autoSensitivity = 2;
            else
                autoSensitivity = 1;
        }

        private void applyForecastCount()
        {
            if (forecastCountValue < ForecastCountMax)
            {
                if (forecastCountValue == 0)
                {
                    if (addBeatTimer != null)
                        addBeatTimer.Stop();
                    forecast = false;
                }
                else
                {
                    forecast = true;
                    nbForecast = forecastCountValue;
                }
            }
            else
            {
                forecast = true;
                nbForecast = 10000000;
            }
        }

        private void applyMaxBpm()
        {
            if (maxBpmOn)
            {
                maxBpmEffective = maxBpmValue;
            }
            else
            {
                maxBpmEffective = 1000;		// = 60 ms o. maxBPM=1000
            }
            newBPM();
        }

        /// <summary>
        /// Applies a beat detection algorithm. Replaces methodBox_SelectedIndexChanged,
        /// including its side effect on the sensitivity: methods 0 and 3 snap the slider
        /// onto the position matching the value they actually use, the other two ignore it.
        /// </summary>
        private void applyAlgorithm(int index)
        {
            algorithm = index;

            if (index == 0)
            {
                sensitivityValue = clamp((int)((sensitivity - 3.0) * 50), SensitivityMin, SensitivityMax);
            }
            else if (index == 3)
            {
                if (autoSensitivity == 1)
                    sensitivityValue = 100;
                else if (autoSensitivity == 2)
                    sensitivityValue = 75;
                else if (autoSensitivity == 3)
                    sensitivityValue = 50;
                else
                    sensitivityValue = 0;
            }

            resetStatistics();
            detectMode = index;
        }

        /// <summary>
        /// sets the number of spectrum bands, falling back to 32 for an unknown value
        /// (e.g. out of a damaged project)
        /// </summary>
        internal void setUsedBands(int bands)
        {
            int idx = Array.IndexOf(bandCounts, bands);
            if (idx < 0)
                idx = Array.IndexOf(bandCounts, 32);
            if (idx < 0)
                return;

            usedbands = bandCounts[idx];

            OnSpectrumLayoutChanged(usedbands);
        }

        /// <summary>
        /// sets the generator rhythm. Like the old combobox handler, a change restarts a
        /// running generator so the new pattern begins on a beat.
        /// </summary>
        internal void setRhythm(int index)
        {
            if (index < 0 || index >= rhythmNames.Length)
                index = 0;

            if ((int)generatorRhythm == index)
                return;

            generatorRhythm = (RhythmType)index;

            if (generatorMode && running)
            {
                startGenerator();
            }
        }

        private void applyStereoSpectrum()
        {
            _fftBufferLeft?.Reset();
            _fftBufferRight?.Reset();

            // Nullen ausgeben, solange die R-Quellen noch registriert sind.
            if (!stereoSpectrum && dbSubLevelRight != null)
            {
                for (int i = 0; i < usedbands; i++)
                {
                    dbSubLevelRight[i] = 0.0F;
                }
                OnSpectrumChanged(dbSubLevelRight, ESpectrumChannel.Right);
            }

            OnSpectrumLayoutChanged(usedbands);
        }

        /// <summary>
        /// Recomputes everything derived from the settings. Every setter does this for its
        /// own value already, so this is only the safety net at the end of a project restore.
        /// Idempotent by design.
        /// </summary>
        internal void recomputeDerivedValues()
        {
            applyMainGain();
            applyVuGain();
            applySpectrumGain();
            applyPeakHoldTime();
            applySensitivity();
            applyForecastCount();
            applyMaxBpm();

            computeMainAndSubBeatTimes(generatorBPM, generatorRhythm);
        }

        /// <summary>
        /// gibt einen Wert zwischen min und max aus basierend auf einem Standard ScrollBar mit 0-100
        /// mit Mittelwert 50 => 1, 0 => min, 100 => max
        /// </summary>
        private double scaleToFactor(int value, double minValue, double maxValue)
        {
            double dvalue;

            if (minValue < 0 || minValue >= 1 || maxValue <= 1)
            {
                throw new ArgumentOutOfRangeException();
            }
            else
            {
                dvalue = Convert.ToDouble(value);
                if (value < 0 || value > 100)
                {
                    if (value < 0)
                        return minValue;
                    else
                        return maxValue;
                }
                else
                {
                    if (value < 50)
                    {
                        return minValue + dvalue * (1 - minValue) / 50;
                    }
                    else
                    {
                        return 1 + (dvalue - 50) * (maxValue - 1) / 50;
                    }
                }
            }
        }

        /// <summary>
        /// Turns the hold time into a number of ComputeLevel calls. That method is driven by
        /// the sample aggregator, so the conversion depends on the sample rate - it has to be
        /// redone whenever the device changes.
        /// </summary>
        private void updatePeakHoldSteps()
        {
            int steps = (int)Math.Round(peakHoldTime * sampleRate / aggregatorNotificationCount);
            peakWaitMax = Math.Max(1, steps);
        }

        #endregion

        #region Generator and statistics

        /// <summary>
        /// restarts the timer for beat forecast or generator to sync to the music
        /// </summary>
        internal void syncGenerator()
        {
            if (generatorMode)
            {
                if (running == true)
                {
                    startGenerator();
                }
            }
            else
            {
                if (forecast)
                {
                    addBeatTimer.Stop();
                    addBeatTimer.Start();
                }
            }
        }

        /// <summary>
        /// measures the tapped beat
        /// </summary>
        internal void tapBeat()
        {
            double midValue;
            long newValue;

            newValue = tapWatch.ElapsedMilliseconds;
            tapTimeout.Start();

            if (tapCount == 0)
            {
                tapWatch.Start();
                tapValues[0] = 0;
                tapCount = 1;
                lastTap = 0;
            }
            else
            {
                tapValues[3] = tapValues[2];
                tapValues[2] = tapValues[1];
                tapValues[1] = tapValues[0];
                tapValues[0] = newValue - lastTap;
                lastTap = newValue;

                tapCount += 1;
                if (tapCount > 3)
                {
                    midValue = tapValues.Average();
                    tapBpm = clamp((int)(60000 / midValue), GeneratorBpmMin, GeneratorBpmMax);

                    GeneratorBpm = tapBpm;

                    if (mainBeatTimer.Enabled == false && generatorMode == true)
                    {
                        toggleRun();
                    }
                }
            }
        }

        internal void resetBeatStatistics()
        {
            resetStatistics();
        }

        /// <summary>
        /// BPM: the detected tempo, or the generated one while
        /// the generator is active. 0 while nothing has been detected yet.
        /// </summary>
        internal int currentBpm
        {
            get { return generatorMode ? (int)generatorBPM : baseBeat; }
        }

        /// <summary>
        /// The sensitivity only has a meaning for the weighted and the automatic method -
        /// the other two ignore it, and the GUI greys the slider out for them.
        /// </summary>
        internal bool sensitivityRelevant
        {
            get { return algorithm == 0 || algorithm == 3; }
        }

        #endregion

        #region IAudioAnalyzer


        public int Bpm
        {
            get { return currentBpm; }
        }

        public bool IsRunning
        {
            get { return running; }
        }

        public void ToggleRun()
        {
            toggleRun();
        }

        public IReadOnlyList<AudioDeviceInfo> Devices
        {
            get { return getDevices(); }
        }

        public string SelectedDeviceId
        {
            get { return getSelectedDeviceId(); }
            set { setDevice(value, getSelectedInputChannel()); }
        }

        public IReadOnlyList<string> InputChannels
        {
            get { return getInputChannelNames(); }
        }

        public int SelectedInputChannel
        {
            get { return getSelectedInputChannel(); }
            set { setDevice(getSelectedDeviceId(), value); }
        }

        public int MainGain
        {
            get { return mainGainValue; }
            set
            {
                int v = clamp(value, GainMin, GainMax);
                if (mainGainValue == v)
                    return;

                mainGainValue = v;
                applyMainGain();
                OnSettingsChanged();
            }
        }

        public IReadOnlyList<int> AvailableBandCounts
        {
            get { return Array.AsReadOnly(bandCounts); }
        }

        public int BandCount
        {
            get { return usedbands; }
            set
            {
                if (usedbands == value)
                    return;

                setUsedBands(value);
                OnSettingsChanged();
            }
        }

        public bool StereoSpectrum
        {
            get { return stereoSpectrum; }
            set
            {
                if (stereoSpectrum == value)
                    return;

                stereoSpectrum = value;
                applyStereoSpectrum();
                OnSettingsChanged();
            }
        }

        public int SpectrumGain
        {
            get { return spectrumGainValue; }
            set
            {
                int v = clamp(value, GainMin, GainMax);
                if (spectrumGainValue == v)
                    return;

                spectrumGainValue = v;
                applySpectrumGain();
                OnSettingsChanged();
            }
        }

        public string GetBandRangeLabel(int bandNumber)
        {
            return getBandRangeLabel(bandNumber);
        }

        public int VuGain
        {
            get { return vuGainValue; }
            set
            {
                int v = clamp(value, GainMin, GainMax);
                if (vuGainValue == v)
                    return;

                vuGainValue = v;
                applyVuGain();
                OnSettingsChanged();
            }
        }

        public bool PeakHold
        {
            get { return peakHold; }
            set
            {
                if (peakHold == value)
                    return;

                peakHold = value;
                OnSettingsChanged();
            }
        }

        public int PeakHoldTime
        {
            get { return peakHoldTimeMs; }
            set
            {
                int v = clamp(value, PeakHoldTimeMin, PeakHoldTimeMax);
                if (peakHoldTimeMs == v)
                    return;

                peakHoldTimeMs = v;
                applyPeakHoldTime();
                OnSettingsChanged();
            }
        }

        /// <summary>
        /// Translated at the boundary, like the rhythms: beatAlgorithmNames stays the English
        /// msgid source and the selection runs on the index, never on the text.
        /// </summary>
        public IReadOnlyList<string> BeatAlgorithms
        {
            get { return getBeatAlgorithmNames(); }
        }

        public int BeatAlgorithm
        {
            get { return algorithm; }
            set
            {
                if (value < 0 || value >= beatAlgorithmNames.Length)
                    return;
                if (algorithm == value)
                    return;

                applyAlgorithm(value);
                OnSettingsChanged();
            }
        }

        public bool SensitivityRelevant
        {
            get { return sensitivityRelevant; }
        }

        public int Sensitivity
        {
            get { return sensitivityValue; }
            set
            {
                int v = clamp(value, SensitivityMin, SensitivityMax);
                if (sensitivityValue == v)
                    return;

                sensitivityValue = v;
                applySensitivity();
                OnSettingsChanged();
            }
        }

        public bool MaxBpmLimit
        {
            get { return maxBpmOn; }
            set
            {
                if (maxBpmOn == value)
                    return;

                maxBpmOn = value;

                applyMaxBpm();
                OnSettingsChanged();
            }
        }

        public int MaxBpm
        {
            get { return maxBpmValue; }
            set
            {
                int v = clamp(value, MaxBpmMin, MaxBpmMax);
                if (maxBpmValue == v)
                    return;

                maxBpmValue = v;
                applyMaxBpm();
                OnSettingsChanged();
            }
        }

        public int ForecastCount
        {
            get { return forecastCountValue; }
            set
            {
                int v = clamp(value, ForecastCountMin, ForecastCountMax);
                if (forecastCountValue == v)
                    return;

                forecastCountValue = v;
                applyForecastCount();
                OnSettingsChanged();
            }
        }

        public bool DoubleSpeed
        {
            get { return doubleSpeed; }
            set
            {
                if (doubleSpeed == value)
                    return;

                doubleSpeed = value;
                if (doubleSpeed && halfSpeed)   // schliessen sich gegenseitig aus
                    halfSpeed = false;

                OnSettingsChanged();
            }
        }

        public bool HalfSpeed
        {
            get { return halfSpeed; }
            set
            {
                if (halfSpeed == value)
                    return;

                halfSpeed = value;
                if (halfSpeed && doubleSpeed)
                    doubleSpeed = false;

                OnSettingsChanged();
            }
        }

        public void ResetBeatStatistics()
        {
            resetBeatStatistics();
        }

        public bool GeneratorEnabled
        {
            get { return generatorMode; }
            set
            {
                if (generatorMode == value)
                    return;

                changeMode(value);
                OnSettingsChanged();
            }
        }

        public int GeneratorBpm
        {
            get { return (int)generatorBPM; }
            set
            {
                int v = clamp(value, GeneratorBpmMin, GeneratorBpmMax);
                if ((int)generatorBPM == v)
                    return;

                generatorBPM = v;

                OnBpmChanged();
                OnSettingsChanged();

                computeMainAndSubBeatTimes(generatorBPM, generatorRhythm);
            }
        }

        public IReadOnlyList<string> Rhythms
        {
            get { return getRhythmNames(); }
        }

        public int Rhythm
        {
            get { return (int)generatorRhythm; }
            set
            {
                if ((int)generatorRhythm == value)
                    return;

                setRhythm(value);
                OnSettingsChanged();
            }
        }

        public void Sync()
        {
            syncGenerator();
        }

        public void Tap()
        {
            tapBeat();
        }

        #endregion

        #region Translated lists

        /// <summary>names of the beat detection algorithms, in selection order</summary>
        internal List<string> getBeatAlgorithmNames()
        {
            var list = new List<string>();
            foreach (string name in beatAlgorithmNames)
            {
                list.Add(LumosLIB.Tools.I18n.T._(name));
            }
            return list;
        }

        internal List<string> getRhythmNames()
        {
            var list = new List<string>();
            foreach (string name in rhythmNames)
            {
                list.Add(LumosLIB.Tools.I18n.T._(name));
            }
            return list;
        }

        #endregion

        public void Dispose()
        {
            stopAudioAnalysis();
            stopGenerator();

            disposeTimer(ref beatTimer);
            disposeTimer(ref beatclearTimer);
            disposeTimer(ref startTimer);
            disposeTimer(ref addBeatTimer);
            disposeTimer(ref mainBeatTimer);
            disposeTimer(ref subBeatTimer);
            disposeTimer(ref tapTimeout);

            if (_aggregatorLeft != null)
                _aggregatorLeft.MaximumCalculated -= AggregatorLeft_MaximumCalculated;
            if (_aggregatorRight != null)
                _aggregatorRight.MaximumCalculated -= AggregatorRight_MaximumCalculated;
        }

        private static void disposeTimer(ref Timer timer)
        {
            if (timer == null)
                return;

            timer.Stop();
            timer.Dispose();
            timer = null;
        }
    }
}
