using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using NAudio.Wave;
using NAudio.Utils;


namespace AudioAnalyzer
{
	public partial class audioAnalysForm
	{
		// LOCAL VARS
		private AbstractSoundSource audioStream = null;
        private LumosLIB.Tools.FastFourierTransform.SampleAggregator _aggregatorLeft;
        private LumosLIB.Tools.FastFourierTransform.SampleAggregator _aggregatorRight;
        private FFTCircularBuffer _fftBuffer;

        internal bool running = false;
        private int fftLength = 8192;
        private int sampleRate = 48000; //44100;
        private float[] fft = new float[4096];
		private bool beat_detected;
		private bool starting = false;
		private float[] level = new float[2]; // for Level
        private float peakHoldTime = 0.02f;
        private float[,] levelhistory = new float[2, 4] { { 0.0F, 0.0F, 0.0F, 0.0F }, { 0.0F, 0.0F, 0.0F, 0.0F } }; // for Level history
        internal bool peakHold = false; // for Level
        int[] peakWait = new int[2] { 0, 0 }; // for Level
        int peakWaitMax = 5; // for Level PeakHold = 5x20ms = 0.1s
        internal double reglerSpectrum = 1;	// for Spectrum (0,5 - 3)
		private int beatInterval = 20; // 20ms
		private Timer beatTimer = null;
		private Timer beatclearTimer = null;
		private Timer beatclearTimer2 = null;
		private Timer startTimer = null;
//        private Timer infoTimer = null;
        private Color beatActive = Color.Red;
        private Color forecastBeatActive = Color.LightBlue;
        private Color levelActive = Color.PaleVioletRed;
		private Color spectrumActive = Color.SpringGreen;
		private Color spectrumLineActive = Color.Yellow;
		private Color beatPassive = Color.Black;
//		private string[] inputs;
		internal double MaxBPM = 200;

        private int startBand;
        private int maxbands = 96;     // E2 bis Eb10, 82Hz - 19912Hz
        private static int historylength = 43;

        #region spectrum;

        internal int usedbands = 32;
	    private double maxSpecVal = 0;

        #endregion

		# region beatvars
        // common
        internal int algorithm = 0;
	    internal bool maxBpmOn = true;
 
		// for Method Beat1()
        private int detectMode = 0;
		private double sensitivity = 4;
		private double regler1 = 100;	// for Beat1()
		private double regler2 = 1;		// for Beat3(), Beat4() 
		internal double reglerVU = 1;	// for VU-Meter (0,2 - 5)
		private int correction = 3;	// correction between HiHat & Bass-Level
		private int faktor = 6;
        private double minVol = 1.0E-11;

        private int draw = 0;

		private double b;
		private double avg1 = 1;
		private double avg2 = 1;

		private double total1;
		private double total2;

		private float bassvalue;
		private float highvalue;
		private float maxbassvalue;
		private float maxhighvalue;

		private int numReadings1 = 200;
		private int numReadings2 = 50;

		private double[] readings1 = new double[200];
		private double[] readings2 = new double[50];


		// for method beat3() + beat4()
		private double energy;
		private double average;
		private double[] energybuffer = new double[43];
		private double variance;
		private double constant;
		// for method beat4() and spectrum
		private double[] subenergy;
		private List<int> sortedindex;
		private double[,] subenergybuffer;
		private double[] subaverage;
		private double[] subvariance;
		private double[] subconstant;
		private double[] sublevel;
        private double[] dbSubLevel;
        private int[,] subbands;
        private double minenergy = 1.0E-12;
        private double minsubenergy = 1.0E-14;

        // for automatic mode
        private int autoSensitivity=2;

		# endregion beatvars

        # region statistics

        int beatCount;
        private Stopwatch beatClock;
        private long actualBeatTime;
        private long lastBeatTime;
        private TimeSpan[] beatMemory;
        private int[] bpmMemory;
        private int[] bpmDistribution;
        private int tolBpm = 3;
        private int midBpm;
	    internal bool halfSpeed = false;
	    internal bool doubleSpeed = false;
        private Color bpmFont = Color.Blue;
        private Color bpmGrid = Color.DarkBlue;
        private Color bpmBar = Color.Yellow;
        private Color bpmBPM = Color.Red;
        private Color bpmRange = Color.DarkGreen;
        private Color bpmLimit = Color.SpringGreen;

        # endregion statistics

        # region forecast

        internal bool forecast = true;      // BPM Generierung aktiv
        internal int nbForecast = 16;         // max. Anzahl an zu generierenden Beats
        private Timer addBeatTimer = null;
        private int addedBeats;             // Zähler für künstliche Beats
        private int baseBeat=150;           // Vorgabe BPM für Generierung

        # endregion forecast

        # region generator

        private bool generatorMode = false;
        const int MinorBeat = 1;              // major-Beat
        const int MajorBeat=0;              // minor-Beat
        private int tapBpm = 0;             // tapped BPM
	    private double generatorBPM = 120;        // generated BPM
	    public RhythmType generatorRhythm = RhythmType.noRhythm; // choosen rhythm

        public enum RhythmType
        {
            noRhythm,fourQuarter,threeQuarter,twoQuarter,bluesRhythm,fiveQuarter
        }
        private string[] rhythmNames = new string[6] {"no Rythm", "four quarters", "three quarters", "two quarters", "Blues Rhythm", "five quarters"};

        private Timer mainBeatTimer;
        private Timer subBeatTimer;
        private Timer tapTimeout;
        private System.Diagnostics.Stopwatch tapWatch;
        private int numberOfSubbeats = 3;
        private int numberOfDrawnBeats = 8;
        private int drawnBeatsCount = 0;
        private int subbeatCount = 0;

        private Color mainBeatColor = Color.Yellow;
        private Color subBeatColor = Color.Orange;
        private Color formBackColor;

        private int tapCount;
        private long lastTap;
        private long[] tapValues;

        # endregion generator

        #region mood

        private long actualTime;

        private Bitmap moodBitmap;

        private double tempo = 0.5;         // 0=slow, 1=fast
        private double mainMood = 0.5;      // 0=sad, 1=happy

        public struct Tone
        {
            public int band;
            public double amplitude;
            public double frequency;
        }

        public struct historyEntry
        {
            public long time;
            public double[] maxima;
            public bool beat;
            public int basetone;
            public int chord;
        }

        private int memoryLength = 8;
        private double maximumToneVariance = 0.1;
        private double maximumChordVariance = 0.05;
        private List<Tone> maximumsList;
        //private int maximums = 10;

        private int binsToIncrease = 1;
        private int binsToDecrease = 1;
        private double increaseThreshhold = 0.00375;
        private double decreaseThreshhold = 0.00375;

        private double baseToneFreq = 0;
        private string baseToneName = "";
        private int baseToneMidi = 0;
        private int noBaseToneCount = 0;


        private int baseChord = -1;     //  0-11 = Cmajor to Bmajor
        // 12-23 = Cminor to Bminor
        // 24-35 = Cmajor7 to Bmajor7
        // 36-47 = Cminor7 to Bminor7
        private string baseChordName = "";
        private int noChordCount = 0;

        private int[] baseToneMemory;
        private int[] baseChordMemory;

        private Queue<historyEntry> analysisHistory;
 
	    #endregion

        #region chords

        // reference PCPs for C, Cm, C7, Cm7
        private double[,] PCPref = new double[4, 12]
	                                   {
	                                       {1.00, 0.00, 0.03, 0.04, 0.30, 0.17, 0.00, 0.39, 0.03, 0.16, 0.07, 0.02},
	                                       {1.00, 0.10, 0.14, 0.30, 0.08, 0.24, 0.08, 0.35, 0.38, 0.00, 0.08, 0.18},
	                                       {1.00, 0.00, 0.03, 0.04, 0.30, 0.17, 0.00, 0.39, 0.03, 0.16, 0.48, 0.02},
	                                       {1.00, 0.10, 0.14, 0.30, 0.08, 0.24, 0.08, 0.30, 0.26, 0.00, 0.48, 0.04}
	                                   };

        // theoretical reference PCPs for C, Cm, C7, Cm7
        private double[,] PCPrefT = new double[4, 12]
	                                   {
	                                       {1.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0},
	                                       {1.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0},
	                                       {1.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0},
	                                       {1.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0}
	                                   };

        private double[] PCPactual = new double[12];

        #endregion

        #region tones

        // Tonabstände
        private double prime = 1.0;
        private double sekunde = 9.0 / 8;
        private double kleineTerz = 6.0 / 5;
        private double grosseTerz = 5.0 / 4;
        private double quarte = 4.0 / 3;
        private double kleineSexte = 8.0 / 5;
        private double grosseSexte = 5.0 / 3;
        private double kleineSeptime = 9.0 / 5;
        private double grosseSeptime = 15.0 / 8;
        private double oktave = 2.0;

        // Tonfrequenzen
        private const double A0 = 27.5;
        private const double AIS0 = 29.1353;
        private const double B0 = 30.8677;
        private const double C1 = 32.7032;
        private const double CIS1 = 34.6479;
        private const double D1 = 36.7081;
        private const double DIS1 = 38.8909;
        private const double E1 = 41.2035;
        private const double F1 = 43.6536;
        private const double FIS1 = 46.2493;
        private const double G1 = 48.9995;    //~50 Hz
        private const double GIS1 = 51.9130;
        private const double A1 = 55.0000;
        private const double AIS1 = 58.270;
        private const double B1 = 61.735;
        private const double C2 = 65.406;
        private const double CIS2 = 69.295;
        private const double D2 = 73.416;
        private const double DIS2 = 77.781;
        private const double E2 = 82.406;
        private const double F2 = 87.307;
        private const double FIS2 = 92.499;
        private const double G2 = 97.998;
        private const double GIS2 = 103.82;
        private const double A2 = 110.0;
        private const double AIS2 = 116.54;
        private const double B2 = 123.47;
        private const double C3 = 130.81;
        private const double CIS3 = 138.6;
        private const double D3 = 146.8;
        private const double DIS3 = 155.6;
        private const double E3 = 164.8;
        private const double F3 = 174.6;
        private const double FIS3 = 185.0;
        private const double G3 = 196.0;
        private const double GIS3 = 207.7;
        private const double A3 = 220.0;
        private const double AIS3 = 233.1;
        private const double B3 = 246.9;
        private const double C4 = 261.6;
        private const double CIS4 = 277.2;
        private const double D4 = 293.7;
        private const double DIS4 = 311.1;
        private const double E4 = 329.6;
        private const double F4 = 349.2;
        private const double FIS4 = 370.0;
        private const double G4 = 392.0;
        private const double GIS4 = 415.3;
        private const double A4 = 440.0;
        private const double AIS4 = 466.2;
        private const double B4 = 493.9;
        private const double C5 = 523.3;
        private const double CIS5 = 554.4;
        private const double D5 = 587.3;
        private const double DIS5 = 622.3;
        private const double E5 = 659.3;
        private const double F5 = 698.5;
        private const double FIS5 = 740.0;
        private const double G5 = 784.0;
        private const double GIS5 = 830.6;
        private const double A5 = 880.0;
        private const double AIS5 = 932.3;
        private const double B5 = 987.8;
        private const double C6 = 1046.5;
        private const double CIS6 = 1108.7;
        private const double D6 = 1174.7;
        private const double DIS6 = 1244.5;
        private const double E6 = 1318.5;
        private const double F6 = 1396.9;
        private const double FIS6 = 1480.0;
        private const double G6 = 1568.0;
        private const double GIS6 = 1661.2;
        private const double A6 = 1760.0;
        private const double AIS6 = 1864.7;
        private const double B6 = 1975.5;
        private const double C7 = 2093.0;
        private const double CIS7 = 2217.5;
        private const double D7 = 2349.3;
        private const double DIS7 = 2489.0;
        private const double E7 = 2637.0;
        private const double F7 = 2793.8;
        private const double FIS7 = 2960.0;
        private const double G7 = 3136.0;
        private const double GIS7 = 3322.4;
        private const double A7 = 3520.0;
        private const double AIS7 = 3729.3;
        private const double B7 = 3951.1;
        private const double C8 = 4186.0;
        private const double CIS8 = 4434.9;
        private const double D8 = 4698.6;
        private const double DIS8 = 4978.0;
        private const double E8 = 5274.0;
        private const double F8 = 5587.7;
        private const double FIS8 = 5919.9;
        private const double G8 = 6271.9;
        private const double GIS8 = 6644.9;
        private const double A8 = 7040.0;
        private const double AIS8 = 7458.6;
        private const double B8 = 7902.1;
        private const double C9 = 8372.0;
        private const double CIS9 = 8869.8;
        private const double D9 = 9397.3;
        private const double DIS9 = 9956.1;
        private const double E9 = 10548.1;
        private const double F9 = 11175.3;
        private const double FIS9 = 11839.8;
        private const double G9 = 12543.9;
        private const double GIS9 = 13289.8;
        private const double A9 = 14080.0;
        private const double AIS9 = 14917.2;
        private const double B9 = 15804.3;
        private const double C10 = 16744.0;
        private const double CIS10 = 17339.7;
        private const double D10 = 18794.5;
        private const double DIS10 = 19912.1;

        private readonly double[] tones = new double[115];

        #endregion

    }

}
