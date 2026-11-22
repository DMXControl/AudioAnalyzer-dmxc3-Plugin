using System;

namespace AudioAnalyzer
{
    /// <summary>
    /// Estimates key, chord, mood (minor/major), energy and brightness from the semitone
    /// spectrum. Works on a 12-bin pitch class profile (chroma): a fast average for the
    /// current chord and a slow one for the key. Replaces the former peak/maximum-likelihood
    /// code, whose variance loops never accumulated and whose result never left the window.
    /// </summary>
    internal class MoodAnalyzer
    {
        private static readonly string[] NoteNames = { "C", "C#", "D", "Eb", "E", "F", "F#", "G", "G#", "A", "Bb", "B" };

        // Krumhansl-Kessler key profiles, index 0 = tonic
        private static readonly double[] MajorProfile = { 6.35, 2.23, 3.48, 2.33, 4.38, 4.09, 2.52, 5.19, 2.39, 3.66, 2.29, 2.88 };
        private static readonly double[] MinorProfile = { 6.33, 2.68, 3.52, 5.38, 2.60, 3.53, 2.54, 4.75, 3.98, 2.69, 3.34, 3.17 };
        private static readonly double[] TemperleyMajor = { 5.0, 2.0, 3.5, 2.0, 4.5, 4.0, 2.0, 4.5, 2.0, 3.5, 1.5, 4.0 };
        private static readonly double[] TemperleyMinor = { 5.0, 2.0, 3.5, 4.5, 2.0, 4.0, 2.0, 4.5, 3.5, 2.0, 1.5, 4.0 };
        private static readonly double[] AardenMajor = { 17.77, 0.15, 14.93, 0.15, 19.80, 11.36, 0.15, 25.88, 0.15, 15.0, 0.15, 14.91 };
        private static readonly double[] AardenMinor = { 18.26, 0.74, 14.05, 16.86, 0.70, 14.44, 0.70, 18.62, 4.57, 1.93, 7.38, 1.53 };

        private static readonly string[] ChordSuffix = { "", "m", "7", "m7" };
        private static readonly int[][] ChordShapes =
        {
            new[] { 0, 4, 7 },        // major
            new[] { 0, 3, 7 },        // minor
            new[] { 0, 4, 7, 10 },    // dominant 7
            new[] { 0, 3, 7, 10 },    // minor 7
        };
        private readonly double[][] _chordTemplates = new double[48][];

        // time constants in seconds
        private const double ChordTime = 0.5;
        private const double EnergyTime = 2.0;
        private const double SilenceLevel = 0.003;
        private const int FirstMidi = 40;           // E2

        // tunable (a test harness sets them, the plugin keeps the defaults)
        public double KeyTime = 16.0;
        public double MoodTime = 10.0;
        public double ChordMoodTime = 20.0;
        public int FirstChromaBand = 5;             // A2: lower notes are not resolved well by the 8192 point fft
        public int LastChromaBand = 60;             // up to about 2.6 kHz, above that only overtones
        public double PeakThreshold = 0.1;
        public double PeakPower = 0.5;
        public double KeyWeight = 0.75;             // share of the key vote in the mood, rest = chord vote
        public double KeyScale = 0.3;                // difference of the best major/minor fit that counts as a clear statement
        public int ProfileKind = 1;                 // 0 Krumhansl-Kessler, 1 Temperley, 2 Aarden-Essen

        private readonly double[] _chromaFast = new double[12];
        private readonly double[] _chromaSlow = new double[12];
        private double _lastChordScore;
        private double _chordMood = 0.5;    // share of major chords over the last seconds
        private double _keyMood = 0.5;
        private int _chordIndex = -1;
        private int _pendingChord = -1;
        private int _pendingCount;
        private double _changeRate;
        private readonly double[] _previousChroma = new double[12];

        public MoodAnalyzer()
        {
            for (int type = 0; type < 4; type++)
            {
                for (int root = 0; root < 12; root++)
                {
                    double[] t = new double[12];
                    foreach (int step in ChordShapes[type])
                        t[(root + step) % 12] = 1;
                    _chordTemplates[type * 12 + root] = t;
                }
            }
            Reset();
        }

        /// <summary>0 = minor / sad, 1 = major / happy, 0.5 = unknown</summary>
        public double Mood { get; private set; } = 0.5;

        /// <summary>0 = calm, 1 = fast / intense</summary>
        public double Energy { get; private set; }

        /// <summary>0 = dark, 1 = bright (spectral centroid)</summary>
        public double Brightness { get; private set; }

        /// <summary>pitch class 0-11 (C = 0) of the tonic of the detected key, -1 if unknown</summary>
        public int KeyRoot { get; private set; } = -1;

        public bool KeyMinor { get; private set; }

        /// <summary>pitch class of the current chord root, -1 if there is none</summary>
        public int ChordRoot { get; private set; } = -1;

        public bool ChordMinor { get; private set; }

        public string KeyName => KeyRoot < 0 ? "-" : NoteNames[KeyRoot] + (KeyMinor ? " minor" : " major");

        public string ChordName { get; private set; } = "-";

        public void Reset()
        {
            Array.Clear(_chromaFast, 0, 12);
            Array.Clear(_chromaSlow, 0, 12);
            Array.Clear(_previousChroma, 0, 12);
            Mood = 0.5;
            Energy = 0;
            Brightness = 0;
            KeyRoot = -1;
            ChordRoot = -1;
            ChordName = "-";
            _chordIndex = -1;
            _pendingChord = -1;
            _pendingCount = 0;
            _changeRate = 0;
            _lastChordScore = 0;
            _chordMood = 0.5;
            _keyMood = 0.5;
        }

        /// <summary>
        /// bands: amplitude per semitone band starting at E2. dt in seconds since the last call.
        /// bpm = 0 if no tempo is known. onsetsPerSecond = recently detected onsets per second.
        /// </summary>
        public void Update(double dt, double[] bands, double bpm, double onsetsPerSecond, double loudness)
        {
            if (dt <= 0 || dt > 1)
                dt = 0.03;

            if (loudness < SilenceLevel)
            {
                // silence: let everything fall back to neutral, the key memory fades as well
                Decay(_chromaFast, dt, ChordTime);
                Decay(_chromaSlow, dt, KeyTime * 2);
                ChordRoot = -1;
                ChordName = "-";
                _chordIndex = -1;
                _pendingChord = -1;
                _keyMood = Approach(_keyMood, 0.5, dt, MoodTime);
                _chordMood = Approach(_chordMood, 0.5, dt, ChordMoodTime);
                Mood = KeyWeight * _keyMood + (1 - KeyWeight) * _chordMood;
                Energy = Approach(Energy, 0, dt, EnergyTime);
                Brightness = Approach(Brightness, 0, dt, EnergyTime);
                return;
            }

            double[] chroma = ComputeChroma(bands);
            double sum = 0;
            for (int i = 0; i < 12; i++)
                sum += chroma[i];
            if (sum > 0)
            {
                for (int i = 0; i < 12; i++)
                    chroma[i] /= sum;
            }

            Smooth(_chromaFast, chroma, dt, ChordTime);
            Smooth(_chromaSlow, chroma, dt, KeyTime);

            // how fast does the harmony change: 1 - cosine similarity to a little earlier
            double change = 1 - Cosine(_chromaFast, _previousChroma);
            if (change < 0)
                change = 0;
            _changeRate = Approach(_changeRate, Math.Min(1, change * 8), dt, EnergyTime);
            Array.Copy(_chromaFast, _previousChroma, 12);

            UpdateChord();
            UpdateKey(dt);
            UpdateBrightness(bands, dt);
            UpdateEnergy(dt, bpm, onsetsPerSecond, loudness);
        }

        private double[] ComputeChroma(double[] bands)
        {
            double[] chroma = new double[12];
            int last = Math.Min(LastChromaBand, bands.Length - 1);

            double max = 0;
            for (int i = FirstChromaBand; i <= last; i++)
                max = Math.Max(max, bands[i]);
            if (max <= 0)
                return chroma;

            // only clear peaks count, the noise floor between them would blur the profile
            double threshold = max * PeakThreshold;
            for (int i = FirstChromaBand; i <= last; i++)
            {
                double a = bands[i];
                if (a < threshold)
                    continue;
                int pitchClass = (FirstMidi + i) % 12;
                chroma[pitchClass] += Math.Pow(a / max, PeakPower);
            }
            return chroma;
        }

        private void UpdateChord()
        {
            double best = 0;
            int bestIndex = -1;
            for (int c = 0; c < _chordTemplates.Length; c++)
            {
                double s = Cosine(_chromaFast, _chordTemplates[c]);
                // triads are contained in their seventh chord, so prefer the simpler one on a tie
                if (c / 12 >= 2)
                    s *= 0.97;
                if (s > best)
                {
                    best = s;
                    bestIndex = c;
                }
            }

            int current = _chordIndex;

            if (best < 0.65 || bestIndex < 0)
            {
                _pendingChord = -1;
                _pendingCount = 0;
                return;
            }

            if (bestIndex == current)
            {
                _lastChordScore = best;
                _pendingChord = -1;
                _pendingCount = 0;
                return;
            }

            // a new chord has to win a few updates in a row to avoid flicker
            if (bestIndex == _pendingChord)
                _pendingCount++;
            else
            {
                _pendingChord = bestIndex;
                _pendingCount = 1;
            }

            if (current < 0 || _pendingCount >= 6 && best > _lastChordScore * 1.05)
            {
                int type = bestIndex / 12;
                _chordIndex = bestIndex;
                ChordRoot = bestIndex % 12;
                ChordMinor = type == 1 || type == 3;
                ChordName = NoteNames[ChordRoot] + ChordSuffix[type];
                _lastChordScore = best;
                _pendingChord = -1;
                _pendingCount = 0;
            }
        }

        private void UpdateKey(double dt)
        {
            double bestMajor = double.MinValue;
            double bestMinor = double.MinValue;
            int majorRoot = -1;
            int minorRoot = -1;

            for (int root = 0; root < 12; root++)
            {
                double[] majorProfile = ProfileKind == 1 ? TemperleyMajor : ProfileKind == 2 ? AardenMajor : MajorProfile;
                double[] minorProfile = ProfileKind == 1 ? TemperleyMinor : ProfileKind == 2 ? AardenMinor : MinorProfile;
                double major = Correlation(_chromaSlow, majorProfile, root);
                double minor = Correlation(_chromaSlow, minorProfile, root);
                if (major > bestMajor)
                {
                    bestMajor = major;
                    majorRoot = root;
                }
                if (minor > bestMinor)
                {
                    bestMinor = minor;
                    minorRoot = root;
                }
            }

            if (double.IsNaN(bestMajor) || double.IsNaN(bestMinor))
                return;

            if (bestMajor >= bestMinor)
            {
                KeyRoot = majorRoot;
                KeyMinor = false;
            }
            else
            {
                KeyRoot = minorRoot;
                KeyMinor = true;
            }

            // difference of the best fits. Relative keys (B minor / D major) share their notes, so the
            // profiles alone tip over easily: only a clear difference counts, the rest stays neutral.
            double d = Math.Max(-1, Math.Min(1, (bestMajor - bestMinor) / KeyScale));
            _keyMood = Approach(_keyMood, 0.5 + 0.5 * d, dt, MoodTime);

            // the chord qualities over the last seconds are the second vote
            if (ChordRoot >= 0)
                _chordMood = Approach(_chordMood, ChordMinor ? 0.0 : 1.0, dt, ChordMoodTime);

            Mood = KeyWeight * _keyMood + (1 - KeyWeight) * _chordMood;
        }

        private void UpdateBrightness(double[] bands, double dt)
        {
            double weight = 0;
            double weighted = 0;
            for (int i = 0; i < bands.Length; i++)
            {
                weight += bands[i];
                weighted += bands[i] * i;
            }
            if (weight <= 0)
                return;

            // centroid in semitones above E2, about 1 to 6 octaves covers the usual range
            double centroid = weighted / weight;
            double target = Math.Max(0, Math.Min(1, (centroid - 12) / 60.0));
            Brightness = Approach(Brightness, target, dt, EnergyTime);
        }

        private void UpdateEnergy(double dt, double bpm, double onsetsPerSecond, double loudness)
        {
            double sum = 0;
            double weight = 0;

            if (bpm > 0)
            {
                sum += 0.45 * Math.Max(0, Math.Min(1, (bpm - 60) / 120.0));
                weight += 0.45;
            }

            // 4 onsets per second is already a very busy rhythm
            sum += 0.25 * Math.Max(0, Math.Min(1, onsetsPerSecond / 4.0));
            weight += 0.25;

            sum += 0.15 * _changeRate;
            weight += 0.15;

            sum += 0.15 * Math.Max(0, Math.Min(1, loudness));
            weight += 0.15;

            Energy = Approach(Energy, sum / weight, dt, EnergyTime);
        }

        private static double Correlation(double[] chroma, double[] profile, int root)
        {
            double meanC = 0;
            double meanP = 0;
            for (int i = 0; i < 12; i++)
            {
                meanC += chroma[i];
                meanP += profile[i];
            }
            meanC /= 12;
            meanP /= 12;

            double num = 0;
            double denC = 0;
            double denP = 0;
            for (int i = 0; i < 12; i++)
            {
                double c = chroma[(i + root) % 12] - meanC;
                double p = profile[i] - meanP;
                num += c * p;
                denC += c * c;
                denP += p * p;
            }
            double den = Math.Sqrt(denC * denP);
            return den <= 1e-12 ? 0 : num / den;
        }

        private static double Cosine(double[] a, double[] b)
        {
            double dot = 0;
            double na = 0;
            double nb = 0;
            for (int i = 0; i < a.Length; i++)
            {
                dot += a[i] * b[i];
                na += a[i] * a[i];
                nb += b[i] * b[i];
            }
            double den = Math.Sqrt(na * nb);
            return den <= 1e-12 ? 0 : dot / den;
        }

        private static void Smooth(double[] state, double[] input, double dt, double timeConstant)
        {
            double alpha = 1 - Math.Exp(-dt / timeConstant);
            for (int i = 0; i < state.Length; i++)
                state[i] += alpha * (input[i] - state[i]);
        }

        private static void Decay(double[] state, double dt, double timeConstant)
        {
            double f = Math.Exp(-dt / timeConstant);
            for (int i = 0; i < state.Length; i++)
                state[i] *= f;
        }

        private static double Approach(double current, double target, double dt, double timeConstant)
        {
            return current + (target - current) * (1 - Math.Exp(-dt / timeConstant));
        }
    }
}
