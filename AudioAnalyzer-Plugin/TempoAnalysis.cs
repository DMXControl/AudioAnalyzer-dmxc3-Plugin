using System;
using NAudio.Dsp;

namespace AudioAnalyzer
{
    /// <summary>
    /// Continuous onset strength signal (spectral flux over log-spaced bands). It is computed
    /// from the raw samples with a short window, so its time resolution is much better than
    /// the 8192-sample spectrum used for the display. Samples arrive on the audio thread,
    /// the tempo estimation reads on the UI thread, hence the lock.
    /// </summary>
    internal sealed class OnsetEnvelope
    {
        private const int Log2Size = 10;
        private const int FftSize = 1 << Log2Size;
        private const int Hop = FftSize / 2;
        private const int Bands = 20;
        private const double LowHz = 40;
        private const double HighHz = 10000;
        private const int Capacity = 4096;             // about 47 s at 86 frames per second

        private readonly object _lock = new object();
        private readonly float[] _ring = new float[FftSize];
        private readonly double[] _window = new double[FftSize];
        private readonly Complex[] _buffer = new Complex[FftSize];
        private readonly double[] _bandPrev = new double[Bands];
        private readonly double[] _bandNow = new double[Bands];
        private readonly double[] _values = new double[Capacity];
        private readonly double[] _low = new double[Capacity];         // flux of the lowest bands (kick, bass)
        private readonly double[] _times = new double[Capacity];
        private const int LowBands = 6;                                // up to about 200 Hz

        // loudness of the input (before any gain): short term and the loudest of the last minutes
        private const double RmsSeconds = 1.0;
        private const double ReferenceSeconds = 1800;
        private double _rms;
        private double _reference;
        private long _lastChunkEndMs;
        private int _pos;
        private int _sinceFrame;
        private long _samples;
        private double _origin;                        // wall clock (ms) of sample 0
        private bool _started;
        private const double MaxJitterMs = 40;         // later than this a chunk counts as a gap
        private const double MaxGapMs = 5000;          // longer gaps restart the envelope
        private int _count;                            // number of stored frames (max Capacity)
        private int _head;                             // index of the next write
        private int _sampleRate = 44100;
        private int[] _bandStart;
        private int[] _bandEnd;

        public OnsetEnvelope()
        {
            for (int i = 0; i < FftSize; i++)
                _window[i] = 0.5 - 0.5 * Math.Cos(2 * Math.PI * i / (FftSize - 1));
            ConfigureBands();
        }

        public double FramesPerSecond => _sampleRate / (double)Hop;

        public void Reset()
        {
            lock (_lock)
            {
                Array.Clear(_ring, 0, _ring.Length);
                Array.Clear(_bandPrev, 0, Bands);
                _pos = 0;
                _sinceFrame = 0;
                _count = 0;
                _head = 0;
                _samples = 0;
                _started = false;
                _rms = 0;
            }
        }

        /// <summary>forgets the loudness history as well (new device or a restart of the analysis)</summary>
        public void ResetLevels()
        {
            lock (_lock)
            {
                _rms = 0;
                _reference = 0;
            }
        }

        /// <summary>
        /// short term loudness of the input at the given time; falls off while the device
        /// delivers nothing
        /// </summary>
        public double RecentLevel(long nowMs)
        {
            double idle = Math.Max(0, (nowMs - _lastChunkEndMs) / 1000.0 - 0.1);
            return _rms * Math.Exp(-idle / RmsSeconds);
        }

        /// <summary>loudest short term loudness of the last minutes (very slow decay)</summary>
        public double ReferenceLevel => _reference;

        private void ConfigureBands()
        {
            _bandStart = new int[Bands];
            _bandEnd = new int[Bands];
            double binHz = _sampleRate / (double)FftSize;
            double ratio = Math.Pow(HighHz / LowHz, 1.0 / Bands);
            for (int b = 0; b < Bands; b++)
            {
                double lo = LowHz * Math.Pow(ratio, b);
                double hi = LowHz * Math.Pow(ratio, b + 1);
                int s = Math.Max(1, (int)Math.Round(lo / binHz));
                int e = Math.Max(s, (int)Math.Round(hi / binHz) - 1);
                _bandStart[b] = s;
                _bandEnd[b] = Math.Min(FftSize / 2 - 1, e);
            }
        }

        public void SetSampleRate(int sampleRate)
        {
            lock (_lock)
            {
                if (sampleRate == _sampleRate)
                    return;
                _sampleRate = sampleRate;
                ConfigureBands();
                _count = 0;
                _head = 0;
                Array.Clear(_bandPrev, 0, Bands);
            }
        }

        /// <summary>
        /// adds a chunk of mono samples that arrived at arrivalMs (wall clock, end of the chunk).
        /// A capture device may deliver nothing while it is silent (WASAPI loopback does exactly
        /// that); counting samples would then swallow the pauses between two clicks. So a gap
        /// between the expected and the real start of a chunk is filled with silence, and the
        /// frame times stay on the wall clock.
        /// </summary>
        public void AddChunk(float[] mono, int count, long arrivalMs)
        {
            double duration = count * 1000.0 / _sampleRate;
            double start = arrivalMs - duration;

            if (!_started)
            {
                _origin = start;
                _started = true;
            }

            double expected = _origin + _samples * 1000.0 / _sampleRate;
            double gap = start - expected;
            if (gap > MaxGapMs)
            {
                // too long to fill: start over
                lock (_lock)
                {
                    _count = 0;
                    _head = 0;
                }
                Array.Clear(_ring, 0, _ring.Length);
                Array.Clear(_bandPrev, 0, Bands);
                _pos = 0;
                _sinceFrame = 0;
                _samples = 0;
                _origin = start;
            }
            else if (gap > MaxJitterMs)
            {
                int silent = (int)(gap * _sampleRate / 1000.0);
                for (int i = 0; i < silent; i++)
                    AddSample(0f);
            }

            for (int i = 0; i < count; i++)
                AddSample(mono[i]);

            // loudness bookkeeping, gaps count as silence
            double sum = 0;
            for (int i = 0; i < count; i++)
                sum += mono[i] * mono[i];
            double rms = count > 0 ? Math.Sqrt(sum / count) : 0;
            double seconds = duration / 1000.0;
            if (gap > MaxJitterMs && gap <= MaxGapMs)
                _rms *= Math.Exp(-(gap / 1000.0) / RmsSeconds);
            _rms += (1 - Math.Exp(-seconds / RmsSeconds)) * (rms - _rms);
            _reference = Math.Max(_rms, _reference * Math.Exp(-seconds / ReferenceSeconds));
            _lastChunkEndMs = arrivalMs;
        }

        private void AddSample(float sample)
        {
            _ring[_pos] = sample;
            _pos = (_pos + 1) % FftSize;
            _samples++;
            if (++_sinceFrame >= Hop)
            {
                _sinceFrame = 0;
                if (_samples >= FftSize)
                    ComputeFrame();
            }
        }

        private void ComputeFrame()
        {
            for (int i = 0; i < FftSize; i++)
            {
                _buffer[i].X = (float)(_ring[(_pos + i) % FftSize] * _window[i]);
                _buffer[i].Y = 0;
            }
            FastFourierTransform.FFT(true, Log2Size, _buffer);

            double flux = 0;
            double lowFlux = 0;
            for (int b = 0; b < Bands; b++)
            {
                double sum = 0;
                int n = 0;
                for (int k = _bandStart[b]; k <= _bandEnd[b]; k++)
                {
                    double re = _buffer[k].X;
                    double im = _buffer[k].Y;
                    sum += Math.Sqrt(re * re + im * im);
                    n++;
                }
                double level = Math.Log(1 + 1000.0 * (n > 0 ? sum / n : 0));
                double diff = level - _bandPrev[b];
                if (diff > 0)
                {
                    flux += diff;
                    if (b < LowBands)
                        lowFlux += diff;
                }
                _bandPrev[b] = level;
            }

            lock (_lock)
            {
                _values[_head] = flux;
                _low[_head] = lowFlux;
                _times[_head] = _origin + (_samples - FftSize / 2) * 1000.0 / _sampleRate;
                _head = (_head + 1) % Capacity;
                if (_count < Capacity)
                    _count++;
            }
        }

        /// <summary>copies the latest frames (oldest first); returns the number of frames</summary>
        public int Snapshot(double[] values, double[] times, int max)
        {
            return Snapshot(values, null, times, max);
        }

        /// <summary>like Snapshot, also copies the flux of the low bands (may be null)</summary>
        public int Snapshot(double[] values, double[] low, double[] times, int max)
        {
            lock (_lock)
            {
                int n = Math.Min(Math.Min(_count, max), values.Length);
                int start = (_head - n + Capacity) % Capacity;
                for (int i = 0; i < n; i++)
                {
                    values[i] = _values[(start + i) % Capacity];
                    if (low != null)
                        low[i] = _low[(start + i) % Capacity];
                    times[i] = _times[(start + i) % Capacity];
                }
                return n;
            }
        }
    }

    /// <summary>
    /// Tempo from the onset envelope: autocorrelation, harmonic comb over the lag multiples and a
    /// soft tempo prior. Needs no beat decisions, so it is independent of the onset detector.
    /// </summary>
    internal sealed class TempoEstimator
    {
        public readonly double MinBpm;
        public readonly double MaxBpm;

        private readonly double[] _x = new double[4096];
        private readonly double[] _acf = new double[2048];
        private const double Step = 0.5;
        private readonly double[] _current;
        private readonly double[] _integrated;

        public TempoEstimator(double minBpm = 65, double maxBpm = 190)
        {
            MinBpm = minBpm;
            MaxBpm = maxBpm;
            _current = new double[(int)((maxBpm - minBpm) / Step) + 1];
            _integrated = new double[(int)((maxBpm - minBpm) / Step) + 1];
        }

        /// <summary>best tempo of the latest estimate alone, without the integration over time</summary>
        public double InstantBpm { get; private set; }

        /// <summary>time constant of the tempogram integration in seconds</summary>
        public double IntegrationSeconds = 4;   // 10 s took 13 s to follow a song change, 4 s about 8 s at the same stability

        /// <summary>weight of the 1st, 2nd, 3rd ... multiple of the beat period in the comb</summary>
        public double[] Weights = { 1.0, 0.6, 0.4 };

        /// <summary>centre and width (in octaves) of the tempo prior</summary>
        public double PriorBpm = 120;
        public double PriorOctaves = 0.9;

        /// <summary>
        /// values: onset envelope with fps frames per second. Returns the best tempo (0 if there is
        /// no usable rhythm) and fills score for any bpm via ScoreAt afterwards.
        /// </summary>
        public double Estimate(double[] values, int count, double fps, double updateSeconds, out double confidence)
        {
            confidence = 0;
            int maxLag = (int)Math.Ceiling(Weights.Length * 60.0 * fps / MinBpm) + 2;
            if (count < maxLag * 1.5 || maxLag >= _acf.Length)
                return 0;

            // remove the local mean (about 0.5 s) and keep only the rises
            int w = Math.Max(3, (int)(fps * 0.5));
            double running = 0;
            for (int i = 0; i < count; i++)
            {
                running += values[i];
                if (i >= w)
                    running -= values[i - w];
                double mean = running / Math.Min(i + 1, w);
                double d = values[i] - mean;
                _x[i] = d > 0 ? d : 0;
            }

            double energy = 0;
            for (int i = 0; i < count; i++)
                energy += _x[i] * _x[i];
            if (energy < 1e-9 * count)
                return 0;

            for (int lag = 0; lag <= maxLag; lag++)
            {
                double s = 0;
                int n = count - lag;
                for (int i = 0; i < n; i++)
                    s += _x[i] * _x[i + lag];
                _acf[lag] = s / n;
            }
            double zero = _acf[0];
            if (zero <= 0)
                return 0;

            // tempogram of this moment, normalised to its maximum (only the shape counts)
            double max = 0;
            for (int i = 0; i < _current.Length; i++)
            {
                _current[i] = Score(MinBpm + i * Step, fps, zero);
                if (_current[i] > max)
                    max = _current[i];
            }
            if (max <= 0)
                return 0;

            // leaky integration over several seconds: a single odd bar does not change the tempo
            // what this moment alone says (no memory): used to notice a change of the music
            double instBest = 0;
            for (int i = 0; i < _current.Length; i++)
            {
                if (_current[i] > instBest)
                {
                    instBest = _current[i];
                    InstantBpm = MinBpm + i * Step;
                }
            }

            double alpha = Math.Exp(-updateSeconds / IntegrationSeconds);
            double best = 0;
            double bestBpm = 0;
            for (int i = 0; i < _current.Length; i++)
            {
                _integrated[i] = alpha * _integrated[i] + (1 - alpha) * _current[i] / max;
                if (_integrated[i] > best)
                {
                    best = _integrated[i];
                    bestBpm = MinBpm + i * Step;
                }
            }
            confidence = best;
            return bestBpm;
        }

        public void Reset()
        {
            Array.Clear(_integrated, 0, _integrated.Length);
        }

        /// <summary>
        /// Decides whether to leave the current tempo for the candidate. A tempo twice or half the
        /// current one is almost always the same rhythm counted differently and both peaks are
        /// strong: that switch needs a much clearer advantage than a change to any other tempo.
        /// </summary>
        public bool ShouldSwitch(int currentBpm, double candidateBpm, double switchFactor, double octaveFactor)
        {
            if (currentBpm <= 0 || Math.Abs(candidateBpm - currentBpm) <= 2)
                return true;

            double current = 0;
            for (double b = currentBpm - 2; b <= currentBpm + 2; b += 0.5)
                current = Math.Max(current, Integrated(b));

            double ratio = candidateBpm / currentBpm;
            bool octave = Math.Abs(ratio - 2) < 0.12 || Math.Abs(ratio - 0.5) < 0.06;
            double factor = octave ? octaveFactor : switchFactor;
            return Integrated(candidateBpm) >= current * factor;
        }

        /// <summary>
        /// strength of the repetition at the beat period of this tempo, relative to the zero lag
        /// (from the last Estimate call); unlike Score it has no comb and no prior
        /// </summary>
        public double Repetition(double bpm, double fps)
        {
            double zero = _acf[0];
            if (zero <= 0)
                return 0;
            return Interpolate(60.0 * fps / bpm) / zero;
        }

        /// <summary>value of the integrated tempogram at a tempo</summary>
        public double Integrated(double bpm)
        {
            double pos = (bpm - MinBpm) / Step;
            int i = (int)Math.Floor(pos);
            if (i < 0 || i + 1 >= _integrated.Length)
                return 0;
            double f = pos - i;
            return _integrated[i] * (1 - f) + _integrated[i + 1] * f;
        }

        public double Score(double bpm, double fps, double zeroLag)
        {
            double lag = 60.0 * fps / bpm;
            double sum = 0;
            for (int k = 1; k <= Weights.Length; k++)
                sum += Weights[k - 1] * Interpolate(lag * k) / zeroLag;
            double octaves = Math.Log(bpm / PriorBpm, 2);
            return sum * Math.Exp(-0.5 * octaves * octaves / (PriorOctaves * PriorOctaves));
        }

        private double Interpolate(double lag)
        {
            int i = (int)lag;
            if (i + 1 >= _acf.Length)
                return 0;
            double f = lag - i;
            return _acf[i] * (1 - f) + _acf[i + 1] * f;
        }

        /// <summary>
        /// time (ms, envelope time base) of the most recent beat position of a grid with the given
        /// period that fits the onset envelope best over the last seconds. The low bands (kick,
        /// bass) count most: loud hi-hats on the off-beat would otherwise pull the grid there.
        /// </summary>
        public static double BestBeatTime(double[] values, double[] low, double[] times, int count, double fps, double periodMs)
        {
            double periodFrames = periodMs * fps / 1000.0;
            int span = Math.Min(count, (int)(fps * 8));
            if (periodFrames < 2 || span < periodFrames * 3)
                return double.NaN;

            int first = count - span;

            // both curves on the same scale
            double meanAll = 0;
            double meanLow = 0;
            for (int i = first; i < count; i++)
            {
                meanAll += values[i];
                meanLow += low[i];
            }
            meanAll = Math.Max(meanAll / span, 1e-9);
            meanLow = Math.Max(meanLow / span, 1e-9);

            int phases = (int)Math.Round(periodFrames);
            double[] scores = new double[phases];
            for (int phase = 0; phase < phases; phase++)
            {
                double score = 0;
                for (double pos = first + phase; pos < count - 1; pos += periodFrames)
                {
                    int i = (int)pos;
                    double f = pos - i;
                    double all = values[i] * (1 - f) + values[i + 1] * f;
                    double lo = low[i] * (1 - f) + low[i + 1] * f;
                    score += LowWeight * lo / meanLow + all / meanAll;
                }
                scores[phase] = score;
            }

            int bestPhase = 0;
            for (int phase = 1; phase < phases; phase++)
            {
                if (scores[phase] > scores[bestPhase])
                    bestPhase = phase;
            }

            // parabola through the neighbours for a position finer than one frame
            double l = scores[(bestPhase + phases - 1) % phases];
            double c = scores[bestPhase];
            double r = scores[(bestPhase + 1) % phases];
            double denom = l - 2 * c + r;
            double shift = Math.Abs(denom) > 1e-12 ? 0.5 * (l - r) / denom : 0;
            shift = Math.Max(-1, Math.Min(1, shift));

            // last grid position inside the data
            double last = first + bestPhase + shift;
            while (last + periodFrames < count - 1)
                last += periodFrames;
            int li = Math.Max(0, Math.Min(count - 2, (int)last));
            double lf = last - li;
            return times[li] * (1 - lf) + times[li + 1] * lf - PhaseLatencyMs;
        }

        /// <summary>weight of the low bands against all bands when the beat phase is searched</summary>
        public static double LowWeight = 2.0;

        /// <summary>
        /// the flux frame lies a little after the onset (window of 1024 samples), the grid is
        /// moved back by this much
        /// </summary>
        public static double PhaseLatencyMs = 12;
    }
}
