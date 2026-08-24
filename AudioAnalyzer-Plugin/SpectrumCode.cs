using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using AudioAnalyzer.AAEventArgs;


namespace AudioAnalyzer
{
	public partial class audioAnalysForm
	{

        /// <summary>
        /// configures the subband-widths to match the harmonic tones
        /// </summary>
        internal void configSubBands()
        {
            bool wasActive = false;

            // startBand indiziert tones/subbands, nicht fft: E2 bis Eb10 ergibt
            // startBand + maxbands == tones.Length

            if (beatclearTimer != null)
            {
                if (beatclearTimer.Enabled == true)
                {
                    wasActive = true;
                    beatclearTimer.Enabled = false;
                }
            }

            subenergy = new double[maxbands];
            subenergybuffer = new double[historylength, maxbands];
            subaverage = new double[maxbands];
            subvariance = new double[maxbands];
            subconstant = new double[maxbands];
            subbands = new int[tones.Length, 2];

            sublevel = new double[maxbands];
            sublevelLeft = new double[maxbands];
            sublevelRight = new double[maxbands];
            dbSubLevel = new double[maxbands];
            dbSubLevelLeft = new double[maxbands];
            dbSubLevelRight = new double[maxbands];

            int lower = FFTFrequency2Index((int)tones[0]);
            int middle = FFTFrequency2Index((int)tones[0]);
            int upper = FFTFrequency2Index((int)tones[1]);
            subbands[0, 0] = lower;
            subbands[0, 1] = lower + (upper - lower) / 2;
            for (int i = 1; i < tones.Length - 1; i++)
            {
                lower = middle;
                middle = upper;
                upper = FFTFrequency2Index((int)tones[i + 1]);
                subbands[i, 0] = lower + (middle - lower) / 2;
                subbands[i, 1] = middle + (upper - middle) / 2;
                if (subbands[i - 1, 1] == subbands[i, 0] && subbands[i, 1] > subbands[i, 0])
                    subbands[i, 0] += 1;

                //Console.WriteLine(i + ".Band: " + subbands[i,0] + "," + subbands[i,1] + "," + middle);
            }
            subbands[tones.Length - 1, 0] = middle + (upper - middle) / 2;
            subbands[tones.Length - 1, 1] = upper;

            for (int i = 0; i < maxbands; i++)
            {
                subenergy[i] = minsubenergy;
            }

            if (beatclearTimer != null)
            {
                if (wasActive == true)
                {
                    beatclearTimer.Enabled = true;
                }
            }
        }

        /// <summary>
        /// Nominal frequency range of a display band (1-based) as a label, e.g. "80-113 Hz".
        /// Derived from the tone table, so independent of the sample rate - configSubBands
        /// only rounds these edges onto the fft bin raster.
        /// </summary>
        internal string getBandRangeLabel(int number)
        {
            int i = number - 1;
            if (i < 0 || i >= usedbands)
                return null;

            // dieselbe Gruppierung wie computeDbSubLevel
            int from = i * maxbands / usedbands;
            int to = (i + 1) * maxbands / usedbands;

            return formatRange(toneBandLow(startBand + from), toneBandHigh(startBand + to - 1));
        }

        /// <summary>
        /// lower edge of a semitone band: the midpoint to the tone below, matching how
        /// configSubBands halves the distance between neighbouring bin indices
        /// </summary>
        private double toneBandLow(int tone)
        {
            return tone > 0 ? (tones[tone - 1] + tones[tone]) / 2 : tones[0];
        }

        private double toneBandHigh(int tone)
        {
            // das letzte Tonband endet in configSubBands genau auf dem Ton, nicht auf der Mitte
            return tone >= tones.Length - 1
                ? tones[tones.Length - 1]
                : (tones[tone] + tones[tone + 1]) / 2;
        }

        private static string formatRange(double low, double high)
        {
            if (high < 1000)
                return String.Format("{0:F0}-{1:F0} Hz", low, high);
            if (low >= 1000)
                return String.Format("{0}-{1} kHz", formatKHz(low), formatKHz(high));
            return String.Format("{0:F0} Hz-{1} kHz", low, formatKHz(high));
        }

        private static string formatKHz(double hz)
        {
            return (hz / 1000).ToString("0.0");
        }

        /// <summary>
        /// computes the harmonic subbands and energies from the fft
        /// </summary>
        void computeSubbands() {

            // Auflösung FFT8192  fft[0] -> 0 Hz, fft[1] -> 5.3832 Hz, fft[i] -> i* 5.3832 Hz --> eindeutig ab G2=98 Hz
            // TODO: Test mit höherer Auflösung FFT16384 fft[0] -> 0 Hz, fft[1] -> 2.6916 Hz, fft[i] -> i* 2.6916 Hz --> eindeutig ab F1=43 Hz
            if (!_fftBuffer.CalculateFft(fft)) {
                return;
            }

            actualTime = beatClock.ElapsedMilliseconds;

            // Beat-Erkennung und Stimmung arbeiten immer auf dem Mono-Mix
            computeSubbands(fft, sublevel, subenergy);
            minsubenergy = subenergy.Min();
        }

        /// <summary>
        /// computes the additional per-channel ffts. Only drawSpectrum() reads their result,
        /// so this runs at the drawing rate instead of every beat tick.
        /// </summary>
        private void computeChannelSubbands()
        {
            if (_fftBufferLeft.CalculateFft(fftLeft))
                computeSubbands(fftLeft, sublevelLeft, null);
            if (_fftBufferRight.CalculateFft(fftRight))
                computeSubbands(fftRight, sublevelRight, null);
        }

        /// <summary>
        /// computes level (and optionally energy) per subband from a single fft
        /// </summary>
        private void computeSubbands(float[] fft, double[] sublevel, double[] subenergy)
        {
            for (int i = 0; i < maxbands; i++)
            {
                double energy = 0;
                double level = 0;
                int count = 0;
                for (int j = subbands[startBand + i, 0]; j <= subbands[startBand + i, 1] && j < fft.Length; j++)
                {
                    energy = energy + fft[j] * fft[j] * regler2;
                    level = level + fft[j];
                    count++;
                }
                if (subenergy != null)
                    subenergy[i] = energy * count / fft.Length;
                sublevel[i] = level / count;
            }
        }

        /// <summary>
        /// draws the spectrum
        /// </summary>
        void drawSpectrum()
        {
            if (stereoSpectrum)
            {
                computeChannelSubbands();
                computeDbSubLevel(sublevelLeft, dbSubLevelLeft);
                computeDbSubLevel(sublevelRight, dbSubLevelRight);
            }
            else
            {
                computeDbSubLevel(sublevel, dbSubLevel);
            }

            if (spectrumBitmap == null
                || spectrumBitmap.Width != spectrumPicture.Width
                || spectrumBitmap.Height != spectrumPicture.Height)
            {
                if (spectrumBitmap != null)
                    spectrumBitmap.Dispose();
                spectrumBitmap = new Bitmap(spectrumPicture.Width, spectrumPicture.Height);
            }

            // Erst vollständig in die Bitmap, dann in einem Zug auf den Bildschirm.
            using (Graphics bg = Graphics.FromImage(spectrumBitmap))
            using (Brush b = new SolidBrush(spectrumActive))
            {
                bg.Clear(Color.Black);

                if (stereoSpectrum)
                {
                    // L in die obere, R in die untere Hälfte
                    int half = spectrumBitmap.Height / 2;
                    drawSpectrumChannel(bg, b, dbSubLevelLeft, 0, half);
                    drawSpectrumChannel(bg, b, dbSubLevelRight, half, spectrumBitmap.Height - half);
                }
                else
                {
                    drawSpectrumChannel(bg, b, dbSubLevel, 0, spectrumBitmap.Height);
                }
            }

            using (Graphics g = spectrumPicture.CreateGraphics())
            {
                g.DrawImageUnscaled(spectrumBitmap, 0, 0);
            }
            //if (dbSubLevel.Max() > debugV)
            //    debugV = dbSubLevel.Max();
            //g.DrawString(debugV.ToString("F2"),f,b,50,5);

            if (stereoSpectrum)
            {
                OnSendSpectrum(dbSubLevelLeft, ESpectrumChannel.Left);
                OnSendSpectrum(dbSubLevelRight, ESpectrumChannel.Right);
            }
            else
            {
                OnSendSpectrum(dbSubLevel, ESpectrumChannel.Mono);
            }

            //debugLabel.Text = dbSubLevel.Max().ToString();
        }

        /// <summary>
        /// converts the subband levels into the normalized values sent to DMXControl
        /// </summary>
        private void computeDbSubLevel(double[] sublevel, double[] dbSubLevel)
        {
            const int dbLimit = -50;

            for (int i = 0; i < usedbands; i++)
            {
                int from = i * maxbands / usedbands;
                int to = (i + 1) * maxbands / usedbands;

                double wert = 0;
                for (int n = from; n < to; n++)
                {
                    wert = wert + sublevel[n];
                }
                wert = wert / (to - from);

                // Level2DB, Werte von -60 bis 0
                double db = 20.0 * Math.Log10(wert * reglerSpectrum);

                if (db > dbLimit)
                {
                    // kein Math.Abs: bei db > 0 muss der Wert über 1 laufen, damit er
                    // vom Limit unten auf 1 gezogen wird statt wieder abzufallen
                    dbSubLevel[i] = 1 - db / dbLimit;
                    if (dbSubLevel[i] > 1)
                        dbSubLevel[i] = 1;

                    // for debugging
                    if (i == 1 || i == 3 || i == 5 || i == 7)
                    {
                        if (dbSubLevel[i] > maxSpecVal)
                            maxSpecVal = dbSubLevel[i];
                    }
                }
                else
                {
                    dbSubLevel[i] = 0;
                }
            }
        }

        /// <summary>
        /// draws the bars of one channel into a horizontal band of the picture
        /// </summary>
        private void drawSpectrumChannel(Graphics g, Brush b, double[] dbSubLevel, int top, int height)
        {
            int dx = (int)(spectrumBitmap.Width / usedbands);
            if (dx <= 0)
                dx = 1;

            int sx = (int)((spectrumBitmap.Width - (usedbands * dx)) / 2);

            for (int i = 0; i < usedbands; i++)
            {
                int scaledDB = (int)(height * (1 - dbSubLevel[i]));

                g.FillRectangle(b, sx + i * dx, top + scaledDB, dx, height - scaledDB);
            }
        }

        public int FFTFrequency2Index(int frequency) {
            int idx = (int)Math.Round((double)fftLength * (double)frequency / (double)sampleRate);
            if (idx > fftLength / 2 - 1)
                idx = fftLength / 2 - 1;
            return idx;
        }
    }
}