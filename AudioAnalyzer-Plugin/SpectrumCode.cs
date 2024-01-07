using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using Un4seen.Bass;
using Un4seen.BassAsio;
using Un4seen.Bass.Misc;
using Un4seen.Bass.AddOn.Tags;


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

            startBand = Utils.FFTFrequency2Index((int)E2, fftLength, sampleRate);

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
            dbSubLevel = new double[maxbands];

            int lower = Utils.FFTFrequency2Index((int)tones[0], fftLength, sampleRate);
            int middle = Utils.FFTFrequency2Index((int)tones[0], fftLength, sampleRate);
            int upper = Utils.FFTFrequency2Index((int)tones[1], fftLength, sampleRate);
            subbands[0, 0] = lower;
            subbands[0, 1] = lower + (upper - lower) / 2;
            for (int i = 1; i < tones.Length - 1; i++)
            {
                lower = middle;
                middle = upper;
                upper = Utils.FFTFrequency2Index((int)tones[i + 1], fftLength, sampleRate);
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
        /// computes the harmonic subbands and energies from the fft
        /// </summary>
        void computeSubbands()
        {

            // Auflösung BASS_DATA_FFT8192  fft[0] -> 0 Hz, fft[1] -> 5.3832 Hz, fft[i] -> i* 5.3832 Hz --> eindeutig ab G2=98 Hz
            // TODO: Test mit höherer Auflösung BASS_DATA_FFT16384 fft[0] -> 0 Hz, fft[1] -> 2.6916 Hz, fft[i] -> i* 2.6916 Hz --> eindeutig ab F1=43 Hz
            Bass.BASS_ChannelGetData(audioStream, fft, (int)BASSData.BASS_DATA_FFT8192);
            actualTime = beatClock.ElapsedMilliseconds;

            // aktuelle Energy for subbands
            for (int i = 0; i < maxbands; i++)
            {
                subenergy[i] = 0;
                sublevel[i] = 0;
                int count = 0;
                for (int j = subbands[startBand + i, 0]; j <= subbands[startBand + i, 1] && j < fft.Length; j++)
                {
                    subenergy[i] = subenergy[i] + fft[j] * fft[j] * regler2;
                    sublevel[i] = sublevel[i] + fft[j];
                    count++;
                }
                subenergy[i] = subenergy[i] * count / fft.Length;
                sublevel[i] = sublevel[i] / count;
            }
            minsubenergy = subenergy.Min();
        }

        /// <summary>
        /// draws the spectrum
        /// </summary>
        void drawSpectrum()
        {
            double db;
            int scaledDB;

            Graphics g = spectrumPicture.CreateGraphics();
            g.Clear(Color.Black);

            int dx = (int)(spectrumPicture.Width / usedbands);
            if (dx <= 0)
                dx = 1;

            int sx = (int)((spectrumPicture.Width - (usedbands * dx)) / 2);
            Brush b = new SolidBrush(spectrumActive);
            Pen p = new Pen(spectrumLineActive);
            Font f = new Font("Verdana", 8);

            int n = 0;
            for (int i = 0; i < usedbands; i++)
            {
                double wert = 0;

                for (int j = 0; j < maxbands / usedbands; j++)
                {
                    wert = wert + sublevel[n];
                }
                wert = wert / (maxbands / usedbands);
                n++;

                // Spectrum malen
                db = Utils.LevelToDB(wert * reglerSpectrum, 1);

                // Werte von -60 bis 0

                int dbLimit = -50;
                if (db > dbLimit)
                {
                    scaledDB = (int)(db * spectrumPicture.Height / dbLimit);

                    g.FillRectangle(b, sx + i * dx, scaledDB, dx, spectrumPicture.Height - scaledDB);

                    dbSubLevel[i] = 1 - Math.Abs(db / dbLimit);
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
            //if (dbSubLevel.Max() > debugV)
            //    debugV = dbSubLevel.Max();
            //g.DrawString(debugV.ToString("F2"),f,b,50,5);

            OnSendSpectrum(dbSubLevel);

            //debugLabel.Text = dbSubLevel.Max().ToString();
        }
    }
}