using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using LumosLIB.GUI.Windows;
using Lumos.GUI.BaseWindow;
using NAudio.CoreAudioApi;
using NAudio.Wave;


namespace AudioAnalyzer
{
	public partial class audioAnalysForm : ToolWindow
	{
        public audioAnalysForm()
        {
            this.MenuIconKey = "music_red";

            InitializeComponent();

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

            for (int i = 0; i < rhythmNames.Length; i++)
            {
                rhythmTypeComboBox.Items.Add(rhythmNames[i]);
            }

        }

        public override Lumos.GUI.EMenuGroups MenuGroup
        {
            get { return Lumos.GUI.EMenuGroups.Control; }
        }


        /// <summary>
        /// load form, initialize values, get sound devices
        /// </summary>
		private void audioAnalysForm_Load(object sender, EventArgs e)
		{
            // create a high performance timer for measuring
            beatClock = new Stopwatch();

			// create a timer for beat-detection
			beatTimer = new Timer();
            beatTimer.Interval = beatInterval;
            beatTimer.Tick += new EventHandler(beatTimer_Tick);

			// create a timer for BPM
			beatclearTimer = new Timer();
            beatclearTimer.Interval = (int)(60000 / MaxBPM);
            beatclearTimer.Tick += new EventHandler(beatclearTimer_Tick);

			// create a timer for turning off Beat-PictureBox
			beatclearTimer2 = new Timer();
            beatclearTimer2.Interval = 60;
			beatclearTimer2.Tick += new EventHandler(beatclearTimer2_Tick);

			// create a timer for suppressing the first second
			startTimer = new Timer();
            startTimer.Interval = 1000;
			startTimer.Tick += new EventHandler(startTimer_Tick);

            // create a timer for suppressing the first second
            //infoTimer = new BASSTimer(1000);
            //infoTimer.Tick += new EventHandler(infoTimer_Tick);

            // create a timer for suppressing the first second
            addBeatTimer = new Timer();
            addBeatTimer.Interval = 1000;
            addBeatTimer.Tick += new EventHandler(addBeatTimer_Tick);

            // create a timer for main Beat Generator
            mainBeatTimer = new Timer();
            mainBeatTimer.Interval = 4000;
            mainBeatTimer.Tick += new EventHandler(mainBeatTimer_Tick);

            // create a timer for main Beat Generator
            subBeatTimer = new Timer();
            subBeatTimer.Interval = 4000;
            subBeatTimer.Tick += new EventHandler(subBeatTimer_Tick);

            // create a timer for tap button timeout
            tapTimeout = new Timer();
            tapTimeout.Interval = 4000;
            tapTimeout.Tick += new EventHandler(tapTimeout_Tick);

            // create a stopwatch for tapButton
            tapWatch = new Stopwatch();
            tapValues = new long[4];

            // fill rhythm types combobox
            //for (int i = 0; i < rhythmNames.Length; i++)
            //{
            //    rhythmTypeComboBox.Items.Add(rhythmNames[i]);
            //}
            rhythmTypeComboBox.SelectedIndex = 0;
            formBackColor = generatorTabPage.BackColor;

			// create a List for sorted indices for Beat4()
			sortedindex = new List<int>(256);

            // create an array for beat memory
            beatMemory = new TimeSpan[200];
            bpmMemory = new int[200];
            bpmDistribution = new int[20];
            beatClock.Start();
            actualBeatTime = beatClock.ElapsedMilliseconds;
            analysisHistory = new Queue<historyEntry>();

            // initialize for baseTone
            maximumsList = new List<Tone>();
            baseToneMemory = new int[memoryLength];
            for (int i = 0; i < memoryLength; i++)
            {
                baseToneMemory[i] = 0;
            }


            // init for chord
            baseChordMemory = new int[memoryLength];
            for (int i = 0; i < memoryLength; i++)
            {
                baseChordMemory[i] = 0;
            }

            // init Mood-Picture
            moodBitmap = new Bitmap(moodPictureBox.Width, moodPictureBox.Height);

			maxBPMBar.Value = (int)MaxBPM;
			newBPM();

            _aggregatorLeft = new LumosLIB.Tools.FastFourierTransform.SampleAggregator();
            _aggregatorLeft.NotificationCount = 1024;
            _aggregatorLeft.MaximumCalculated += AggregatorLeft_MaximumCalculated;
            _aggregatorRight = new LumosLIB.Tools.FastFourierTransform.SampleAggregator();
            _aggregatorRight.NotificationCount = 1024;
            _aggregatorRight.MaximumCalculated += AggregatorRight_MaximumCalculated;
            _fftBuffer = new FFTCircularBuffer(fftLength);

            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            foreach (MMDevice wasapi in enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active)) {
                devicesBox.Items.Add(new WasapiSoundSource(wasapi));
            }
            if (AsioOut.isSupported()) {
                foreach (string asio in AsioOut.GetDriverNames()) {
                    devicesBox.Items.Add(new AsioSoundSource(asio));
                }
            }

            if (devicesBox.Items.Count > 0)
			{
				devicesBox.SelectedIndex = 0;
			}

			methodBox.SelectedIndex = 3;
			subBandBox.SelectedIndex = 2;

            noOfLabel.Text = "max. " + numberOfBeatsBar.Value + " additional beats";

			configSubBands();

            OnSendSpectrumCount(usedbands);

            //infoTimer.Start();

		}

        private void AudioStream_SamplesAvailable(object sender, float[] samples, int count, int channels, int samplerate) {
            if (this.sampleRate != samplerate) {
                this.sampleRate = samplerate;
                configSubBands();
            }
            float left = 0;
            for (int i = 0; i < count; ++i) {
                if (channels == 1 || ((i & 0x1) == 0x0)) {
                    _aggregatorLeft.Add(samples[i]);
                    left = samples[i];
                    if (channels == 1) {
                        _fftBuffer.Add(samples[i]);
                    }
                } else {
                    _aggregatorRight.Add(samples[i]);
                    _fftBuffer.Add((left * 0.5f) + (samples[i] * 0.5f));
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

        //void infoTimer_Tick(object sender, EventArgs e)
        //{
        //    //infoLabel.Text = maxSpecVal.ToString();
        //    //maxSpecVal = 0;
        //}

		# region beat

		/// <summary>
		/// Main timer task, computes beat, level, spectrum every 20ms, draws graphics every 80ms
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void beatTimer_Tick(object sender, EventArgs e)
        {
            // here we gather info about the stream, when it is playing...
            if (audioStream.Playing)
            {
                // the stream is still playing...
                computeSubbands();

                // compute Beat
                ComputeBeat();

                // Compute mood
                computeBaseTune();

                // nur jeden (2./3.) 4. Durchgang das Level, Spektrum und Stimmung malen
                if (draw == 0)
                {
                    drawLevel();
                    drawSpectrum();
                    //spectrumPicture.Image = spectrum.CreateSpectrum(_stream, spectrumPicture.Width, spectrumPicture.Height, Color.Green, Color.Red, Color.DarkBlue, false, false, false);
                    drawMoodPicture(false);
                    baseToneLabel.Text = baseToneName;
                    chordLabel.Text = baseChordName;
                }
                draw++;
                if (draw >= 3)
                    draw = 0;

                // fill the statistics (max 200 values = 4 seconds)
                fillStatistics();
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
                    OnSendBeat(MinorBeat);

                    if (maxBPMCheckBox.Enabled) 
                        beatclearTimer.Start();
                    beatBox.BackColor = beatActive;
                    beatclearTimer2.Start();

                    lastBeatTime = actualBeatTime;
                    actualBeatTime = beatClock.ElapsedMilliseconds;

                    //Console.WriteLine("vorher: " + lastBeatTime);
                    //Console.WriteLine("jetzt : " + actualBeatTime);

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
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void beatclearTimer_Tick(object sender, EventArgs e)
		{
			beatclearTimer.Stop();
			beat_detected = false;
		}

		/// <summary>
		/// resets color of beat detected pictureBox
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void beatclearTimer2_Tick(object sender, EventArgs e)
		{
			beatclearTimer2.Stop();
			beatBox.BackColor = beatPassive;

            outputDebugInfo();
		}

        /// <summary>
        /// sends an artificial beat signal
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void addBeatTimer_Tick(object sender, EventArgs e)
        {
            if (addedBeats < nbForecast && forecast && baseBeat > 0)
            {
                // send beat
                OnSendBeat(MinorBeat);

                addedBeats += 1;

                if (addedBeats == 1)
                {
                    if (doubleCheckBox.Checked)
                        addBeatTimer.Interval = (int)(30000 / baseBeat);
                    else if (halfCheckBox.Checked)
                        addBeatTimer.Interval = (int)(120000 / baseBeat);
                    else
                        addBeatTimer.Interval = (int)(60000 / baseBeat);
                }

                beatclearTimer.Start();
                beatBox.BackColor = forecastBeatActive;

                beatclearTimer2.Start();
            }
            else
            {
                addBeatTimer.Stop();
            }
        }

        #endregion


        /// <summary>
        /// selected input has changed
        /// </summary>
		private void inputsBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (audioStream != null)
			{
				startButton_Click(this, new EventArgs());
			}

			startButton.Text = "Start";

			startButton.Focus();

		}


        /// <summary>
        /// selected device has changed
        /// </summary>
		private void devicesBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (audioStream != null)
			{
				startButton_Click(this, new EventArgs());
			}

            inputsBox.Items.Clear();
            AbstractSoundSource source = (AbstractSoundSource)devicesBox.SelectedItem;
            if (source == null) return;
            if (source is AsioSoundSource) {
                inputsBox.Items.AddRange(((AsioSoundSource)source).getInputNames());
                inputsBox.Visible = true;
                inputsBox.Enabled = inputsBox.Items.Count > 0;
                if (inputsBox.Items.Count == 0) {
                    inputsBox.Text = "< Device not connected or failed >";
                }
            } else {
                inputsBox.Visible = false;
            }
            label7.Visible = inputsBox.Visible;

            if (inputsBox.Items.Count > 0) {
                inputsBox.SelectedIndex = 0;
                inputsBox_SelectedIndexChanged(this, new EventArgs());
                startButton.Enabled = true;
            } else if (!inputsBox.Visible) {
                startButton.Enabled = true;
            } else {
                startButton.Enabled = false;
            }

			startButton.Focus();
		}


        /// <summary>
        /// starts/stops the action 
        /// </summary>
		private void startButton_Click(object sender, EventArgs e)
		{
            if (generatorMode == false)
            {
                if (running)
                {
                    stopAudioAnalysis();
                    startButton.Image = global::AudioAnalyzer.Properties.Resources.media_play;
                    startButton.Text = "Start";
                    actualBPMLabel.Text = "0";
                    baseToneLabel.Text = "-";
                    chordLabel.Text = "-";
                }
                else
                {
                    startAudioAnalysis();
                    startButton.Image = global::AudioAnalyzer.Properties.Resources.media_stop;
                    startButton.Text = "Stop";
                }
            }
            else
            {
                if (running==false)
                {
                    startGenerator();
                    startButton.Image = global::AudioAnalyzer.Properties.Resources.media_stop;
                    startButton.Text = "Stop";
                }
                else
                {
                    stopGenerator();
                    startButton.Text = "Start";
                    startButton.Image = global::AudioAnalyzer.Properties.Resources.media_play;
                    actualBPMLabel.Text = "0";
                    baseToneLabel.Text = "-";
                    chordLabel.Text = "-";
                }
            }
		}

        /// <summary>
        /// selected beat detection mode has changed
        /// </summary>
		private void methodBox_SelectedIndexChanged(object sender, EventArgs e)
		{
		    algorithm = methodBox.SelectedIndex;

			if (methodBox.SelectedIndex == 0)
			{
                sensitivityBar.Visible = true;
                sensitivityLabel.Visible = true;
                sensitivityBar.Value = (int)((sensitivity - 3.0) * 50);
            }
			else if (methodBox.SelectedIndex==1)
			{
                sensitivityBar.Visible = false;
                sensitivityLabel.Visible = false;
            }
			else if (methodBox.SelectedIndex==2)
			{
                sensitivityBar.Visible = false;
                sensitivityLabel.Visible = false;
            }
			else if (methodBox.SelectedIndex==3)
            {
                sensitivityBar.Visible = true;
                sensitivityLabel.Visible = true;

                if (autoSensitivity==1)
                    sensitivityBar.Value = 100;
                else if (autoSensitivity==2)
                    sensitivityBar.Value = 75;
                else if (autoSensitivity==3)
                    sensitivityBar.Value = 50;
                else
                    sensitivityBar.Value = 0;

            }

            resetStatistics();
            detectMode = methodBox.SelectedIndex;
			maxBPMBar.Focus();
		}


        /// <summary>
        /// selected beat detection mode has changed
        /// </summary>
        internal void algorithm_changed(int algorithmIndex)
        {
            methodBox_SelectedIndexChanged(this,new EventArgs());
        }


        /// <summary>
        /// beat detection sensitivty has changed
        /// </summary>
		private void sensitivityBar_Scroll(object sender, EventArgs e)
		{
            sensitivity = 3.0 + ((double)sensitivityBar.Value / 50);

            if (sensitivityBar.Value < 25)
                autoSensitivity = 4;
            else if (sensitivityBar.Value < 50)
                autoSensitivity = 3;
            else if (sensitivityBar.Value < 75)
                autoSensitivity = 2;
            else
                autoSensitivity = 1;
		}


        /// <summary>
        /// number of spectrum channels has changed
        /// </summary>
		private void subBandBox_SelectedIndexChanged(object sender, EventArgs e)
		{
            usedbands = (Convert.ToInt32(subBandBox.SelectedItem));

            OnSendSpectrumCount(usedbands);

			startButton.Focus();
		}

		internal void gainBar_ValueChanged(object sender, EventArgs e)
		{
            regler1 = 100 * scaleToFactor(gainBar.Value, 0.25, 5.0);
			
            //if (gainBar.Value <= 50)
            //    minVol = 1 + (gainBar.Value - 50) * 2;
            //else
            //    minVol = 1 - (50 - b) * 0.01;

            //Console.WriteLine(minVol);

            //regler2 = scaleToFactor(gainBar.Value, 0.25, 1.75);
		}

        /// <summary>
        /// main gain has changed
        /// </summary>
        private void gainBar_Scroll(object sender, EventArgs e)
        {
            gainBar_ValueChanged(sender, e);
        }

        /// <summary>
        /// VU gain has changed
        /// </summary>
		private void gainVUBar_ValueChanged(object sender, EventArgs e)
		{
            if (sender==(object)gainVUBar)
                reglerVU = scaleToFactor(gainVUBar.Value, 0.5, 8.0);
		}

        /// <summary>
        /// VU gain has changed
        /// </summary>
        private void gainVUBar_Scroll(object sender, EventArgs e)
        {
            gainVUBar_ValueChanged(sender, e);
        }

        /// <summary>
        /// Spectrum gain has changed
        /// </summary>
        private void gainSpectrumBar_Scroll(object sender, EventArgs e)
        {
            gainSpectrumBar_ValueChanged(sender, e);
        }

        /// <summary>
        /// Spectrum gain has changed
        /// </summary>
        private void gainSpectrumBar_ValueChanged(object sender, EventArgs e)
        {
            if (sender == (object)gainSpectrumBar)
                reglerSpectrum = scaleToFactor(gainSpectrumBar.Value, 0.25, 20.0);
        }

        /// <summary>
        /// maxBPM value has changed
        /// </summary>
		private void maxBPMBar_Scroll(object sender, EventArgs e)
		{
            if (sender.Equals((object)maxBPMBar))
            {
                if (maxBpmOn)
                {
                    MaxBPM = maxBPMBar.Value;
                }
                else
                {
                    MaxBPM = 1000;		// = 60 ms o. maxBPM=1000
                }
                newBPM();
            }
		}

        /// <summary>
        /// gibt einen Wert zwischen min und max aus basierend auf einem Standard ScrollBar mit 0-100
        /// mit Mittelwert 50 => 1, 0 => min, 100 => max
        /// </summary>
        /// <param name="value"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
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
        /// resize of form
        /// </summary>
		private void audioAnalysForm_Resize(object sender, EventArgs e)
		{
			//this.MinimumSize = new Size(deviceGroup.Width, beatGroup.Height);

			//double ratio = this.Width / this.Height;

            //if (ratio > 1.5 && this.Height<deviceGroup.Height+levelGroup.Height)
            //{
            //    // alle Gruppen nebeneinander anordnen
            //    //levelGroup.Left = deviceGroup.Width;
            //    //levelGroup.Top = 0;
            //}
            //else if (ratio < 0.666 && this.Width < deviceGroup.Width + beatGroup.Width)
            //{
            //    // alle Gruppen untereinander anordnen
            //}
            //else
            //{
            //    // Standard-Anordnung im Viereck
            //}
		}

        /// <summary>
        /// sets the number of automatically send beats
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numberOfBeatsBar_Scroll(object sender, EventArgs e)
        {
            if (numberOfBeatsBar.Value < numberOfBeatsBar.Maximum)
            {
                if (numberOfBeatsBar.Value == 0)
                {
                    if (addBeatTimer != null)
                        addBeatTimer.Stop();
                    forecast = false;
                    noOfLabel.Text = "no additional beats";
                }
                else
                {
                    forecast = true;
                    nbForecast = numberOfBeatsBar.Value;
                    noOfLabel.Text = "max. " + numberOfBeatsBar.Value + " additional beats";
                }
            }
            else
            {
                forecast = true;
                nbForecast = 10000000;
                noOfLabel.Text = "unlimited additional beats";
            }
        }

        /// <summary>
        /// double the speed of the generated beats
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void doubleCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (doubleCheckBox.Checked)
            {
                if (halfCheckBox.Checked)
                {
                    halfCheckBox.Checked = false;
                    halfSpeed = false;
                }
                doubleSpeed = true;

            }
            else
            {
                doubleSpeed = false;
            }
        }

        /// <summary>
        /// halfs the speed of the generated beats
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void halfCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (halfCheckBox.Checked)
            {
                if (doubleCheckBox.Checked)
                {
                    doubleSpeed = false;
                    doubleCheckBox.Checked = false;
                }
                halfSpeed = true;
            }
            else
            {
                halfSpeed = false;
            }
        }

        /// <summary>
        /// activatec the beat generator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void activateBeatGeneratorCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            changeMode(activateBeatGeneratorCheckbox.Checked);
        }

        /// <summary>
        /// changtes the rhytm type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rhythmTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MessageBox.Show("hier 1");

            generatorRhythm = (RhythmType) rhythmTypeComboBox.SelectedIndex;
            if (generatorMode && running)
            {
                startGenerator();
            }

            //MessageBox.Show("hier 2");
            drawBeatPicture(0);

        }

        /// <summary>
        /// restarts the timer for beatforecast or generator to sync to the music
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void syncButton_Click(object sender, EventArgs e)
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tapButton_Click(object sender, EventArgs e)
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

                Console.WriteLine(tapWatch.ElapsedMilliseconds);
                Console.WriteLine(tapValues[0] + " , " + tapValues[1] + " , " + tapValues[2] + " , " + tapValues[3]);

                tapCount += 1;
                if (tapCount > 3)
                {
                    midValue = tapValues.Average();
                    tapBpm = (int)(60000 / midValue);

                    if (tapBpm > beatGeneratorUpDown.Maximum)
                        tapBpm = (int)beatGeneratorUpDown.Maximum;

                    if (tapBpm < beatGeneratorUpDown.Minimum)
                        tapBpm = (int)beatGeneratorUpDown.Minimum;

                    beatGeneratorUpDown.Value = (decimal)tapBpm;

                    if (mainBeatTimer.Enabled == false && generatorMode == true)
                    {
                        startButton_Click(this, new EventArgs());
                    }
                }
            }
        }

        /// <summary>
        /// changes the generated BPM
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void beatGeneratorUpDown_ValueChanged(object sender, EventArgs e)
        {
            generatorBPM = (double)beatGeneratorUpDown.Value;
            actualBPMLabel.Text = beatGeneratorUpDown.Value.ToString();

            computeMainAndSubBeatTimes(generatorBPM, generatorRhythm);
        }

        /// <summary>
        /// resets the statistics and BPM value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void resetButton_Click(object sender, EventArgs e)
        {
            resetStatistics();
        }

        /// <summary>
        /// starts the AudioAnalyze
        /// </summary>
        private void startAudioAnalysis()
        {
            resetStatistics();

            AbstractSoundSource source = (AbstractSoundSource)devicesBox.SelectedItem;
            if (source == null) return;
            audioStream = source;

            if (source is AsioSoundSource) {
                ((AsioSoundSource)source).InputChannel = inputsBox.SelectedIndex;
            }

            // Starts the beat detection
            draw = 0;
            if (audioStream.StartRecord()) {
                audioStream.SamplesAvailable += AudioStream_SamplesAvailable;

                _aggregatorLeft.Reset();
                _aggregatorRight.Reset();

                starting = true;
                startTimer.Enabled = true;

                //updateTimer.Start();
                beatTimer.Start();

                running = true;

                beatBox.BackColor = beatPassive;
                startButton.Text = "Stop";
            }
            else
            {
                Console.WriteLine("Could not start recording");
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

                beatBox.BackColor = Color.Gray;
                spectrumPicture.Image = null;
                levelLBox.Image = null;
                levelRBox.Image = null;

                level[0] = 0.0F;
                level[1] = 0.0F;
                //OnSendLevel(level[0],level[1]);

                for (int i = 0; i < usedbands;i++ )
                {
                    dbSubLevel[i] = 0.0F;
                }
                OnSendSpectrum(dbSubLevel);

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
            drawnBeatsCount = 0;
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
        /// <param name="generator"></param>
        private void changeMode(bool generator)
        {
            if (running)
            {
                if (generator)
                {
                    stopAudioAnalysis();
                    startGenerator();
                    generatorMode = true;
                    actualBPMLabel.Text = beatGeneratorUpDown.Value.ToString();
                }
                else
                {
                    stopGenerator();
                    startAudioAnalysis();
                    generatorMode = false;
                    actualBPMLabel.Text = Convert.ToString(baseBeat);
                }
            }
            else
            {
                if (generator)
                {
                    generatorMode = true;
                    actualBPMLabel.Text = beatGeneratorUpDown.Value.ToString();
                }
                else
                {
                    generatorMode = false;
                    actualBPMLabel.Text = Convert.ToString(baseBeat);
                }
            }
        }


        /// <summary>
        /// switches Max BPM limit on/off
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void maxBPMCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (maxBPMCheckBox.Checked)
            {
                maxBpmOn = true;
                maxBPMBar.Enabled = true;
            }
            else
            {
                maxBpmOn = false;
                maxBPMBar.Enabled = false;
            }
        }

        /// <summary>
        /// zur Ausgabe von Debug-Informationen
        /// </summary>
        private void outputDebugInfo()
        {
            //for (int i = 0; i < maxbands; i++)
            //{
            //    Console.WriteLine(sublevel[i]);
            //}

            //foreach (Tone t in maximumsList)
            //{
            //    Console.WriteLine("Band: " + t.band + " Freq: " + t.frequency + " Level: " + t.amplitude);
            //}
        }

        /// <summary>
        /// main form is shown
        /// </summary>
        private void audioAnalysForm_Shown(object sender, EventArgs e)
        {
            switch (usedbands)
            {
                case 8:
                    subBandBox.SelectedIndex = 0;
                    break;
                case 16:
                    subBandBox.SelectedIndex = 1;
                    break;
                case 32:
                    subBandBox.SelectedIndex = 2;
                    break;
                case 64:
                    subBandBox.SelectedIndex = 3;
                    break;
                case 96:
                    subBandBox.SelectedIndex = 4;
                    break;
                default:
                    usedbands = 16;
                    subBandBox.SelectedIndex = 1;
                    break;

            }
        }

        /// <summary>
        /// level PeakHold on/off
        /// </summary>
        private void PeakHoldCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (PeakHoldCheckBox.Checked)
            {
                peakHold = true;
            }
            else
            {
                peakHold = false;
            }
        }

        private void PeakHoldBar_Scroll(object sender, EventArgs e)
        {
            peakHoldTime = (float)PeakHoldBar.Value / 1000;
            //MessageBox.Show(peakHoldTime.ToString());
        }
    }
}
