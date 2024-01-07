using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Drawing;
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
        /// computes basetone (not really working)
        /// </summary>
        private void computeBaseTune()
        {
            //lokal Variable
            double[] ML;
            double AA;
            double TT;
            double NN;
            double avg;
            double variance;

            // find Maxima in Spectrum
            findMaxima1();
            //findMaxima2();

            // compute Maximum-Likelihood-Schaetzwerte
            //nur für die zehn höchsten Werte
            int n = maximumsList.Count;
            if (n > 10)
                n = 10;

            if (n > 0)
            {
                ML = new double[n];

                double maxFreq = 0;
                for (int i = 0; i < n; i++)
                {
                    if (maximumsList[i].frequency > maxFreq)
                        maxFreq = maximumsList[i].frequency;
                }

                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (maximumsList[j].frequency >= maximumsList[i].frequency)
                        {
                            AA = 100 * Math.Log(maximumsList[j].amplitude);

                            double lastDist = double.MaxValue;
                            double harmonic = maximumsList[i].frequency;


                            while (harmonic <= maxFreq)
                            {
                                double dist = Math.Abs(harmonic - maximumsList[j].frequency);
                                if (dist < lastDist)
                                    lastDist = dist;
                                harmonic = harmonic + maximumsList[i].frequency;
                            }
                            TT = 169 * Math.Exp(-0.5 * (lastDist * lastDist));

                            NN = 10 / (Math.Exp(maximumsList[j].frequency / 1000));

                            double product = AA * TT * NN;

                            ML[i] = ML[i] + product;
                        }
                    }
                    //Console.WriteLine("Likelihood : " + ML[i]);
                }

                int maxi = 0;
                double maxML = 0;
                for (int i = 0; i < n; i++)
                {
                    if (ML[i] > maxML)
                    {
                        maxML = ML[i];
                        maxi = i;
                    }
                }

                // Berechne die Varianz der gefundenen Grundtoene über die Zeit

                if (memoryLength > 1)
                {
                    for (int i = 1; i < memoryLength; i++)
                    {
                        baseToneMemory[i - 1] = baseToneMemory[i];
                    }
                    baseToneMemory[memoryLength - 1] = freq2Midi(maximumsList[maxi].frequency);
                }

                avg = baseToneMemory.Average();

                variance = 0;
                for (int i = 1; i < memoryLength; i++)
                {
                    variance = (baseToneMemory[i] - avg) * (baseToneMemory[i] - avg);
                }
                variance = variance / memoryLength;

                if (variance < maximumToneVariance)
                {
                    // Grundton ermittelt !!!
                    baseToneFreq = maximumsList[maxi].frequency;
                    baseToneMidi = freq2Midi(baseToneFreq);
                    baseToneName = midi2note(baseToneMidi);
                    noBaseToneCount = 0;
                }
                else
                {
                    if (noBaseToneCount >= 50)
                    {
                        baseToneFreq = 0;
                        baseToneMidi = -1;
                        baseToneName = "";
                    }
                    else
                    {
                        noBaseToneCount++;
                    }
                }
            }

            // compute PCP from maximums
            for (int i = 0; i < PCPactual.Length; i++)
            {
                PCPactual[i] = 0;
            }

            if (maximumsList.Count > 0)
            {

                foreach (Tone to in maximumsList)
                {
                    //Tone to = maximumsList[i];
                    int midi = freq2Midi(to.frequency);
                    midi = midi % 12;
                    PCPactual[midi] = PCPactual[midi] + to.amplitude;
                }
            }

            computeChordFromPCP();
        }


        /// <summary>
        /// find maxima in spectrum (complicated version)
        /// </summary>
        private void findMaxima1()
        {
            int increasedBins = 0;
            int wereIncreasedBins = 0;
            int decreasedBins = 0;
            double lastValue = 0;

            double ampThresholdRatio = 0.01;
            double ampThreshold = sublevel.Max() * ampThresholdRatio;
            // TODO reactivate ampThreshold computation
            //ampThreshold = 0.0;

            //double noiseGateThreshold = 0.04;

            maximumsList.Clear();

            for (int i = 0; i < sublevel.Length; i++)
            {

                if (sublevel[i] > (lastValue + increaseThreshhold))
                {
                    increasedBins += 1;
                    decreasedBins = 0;
                }
                else if (sublevel[i] < (lastValue - decreaseThreshhold))
                {
                    decreasedBins += 1;
                    if (increasedBins != 0)
                        wereIncreasedBins = increasedBins;
                    increasedBins = 0;
                }

                if ((wereIncreasedBins >= binsToIncrease) && (decreasedBins >= binsToDecrease))
                {
                    // Maximum gefunden
                    //Console.WriteLine("Test :" + sublevel[i - decreasedBins] + ">" + ampThreshold);

                    // zu kleine Werte entfernen
                    if (sublevel[i - decreasedBins] > ampThreshold)
                    {
                        Tone to = new Tone();
                        to.band = i - decreasedBins;
                        to.amplitude = sublevel[i - decreasedBins];
                        to.frequency = (double)tones[startBand + i - decreasedBins];
                        maximumsList.Add(to);
                        //Console.WriteLine("Maximum at band " + to.band + "with Hz. " + to.frequency.ToString("F2"));
                    }
                    wereIncreasedBins = 0;
                    increasedBins = 0;
                    decreasedBins = 0;
                }

                lastValue = sublevel[i];
            }
            maximumsList.Sort(compareTones);
        }

        /// <summary>
        /// find maxima in spectrum (simple version)
        /// </summary>
        private void findMaxima2()
        {
            double avg = 0;
            double faktor = 3;

            maximumsList.Clear();
            avg = sublevel.Average();


            for (int i = 1; i < maxbands - 1; i++)
            {
                if (sublevel[i] > faktor * avg && sublevel[i] > sublevel[i - 1] && sublevel[i] > sublevel[i + 1])
                {
                    Tone t = new Tone();
                    t.band = i;
                    t.amplitude = sublevel[i];
                    t.frequency = (double)tones[startBand + i];
                    maximumsList.Add(t);
                }
            }
            maximumsList.Sort(compareTones);

        }

        /// <summary>
        /// computes the base chord
        /// </summary>
        private void computeChordFromPCP()
        {
            int tmpBaseChord = 0;
            double avg;

            // PCP Max-Wert auf 1 normieren
            double faktor = 1 / PCPactual.Max();
            for (int i = 0; i < PCPactual.Length; i++)
            {
                PCPactual[i] = PCPactual[i] * faktor;
            }

            // compare PCP to reference-PCPs and return chord 
            tmpBaseChord = comparePCP(PCPactual);


            // Berechne die Varianz der gefundenen akkorde über die Zeit

            if (memoryLength > 1)
            {
                for (int i = 1; i < memoryLength; i++)
                {
                    baseChordMemory[i - 1] = baseChordMemory[i];
                }
                baseChordMemory[memoryLength - 1] = tmpBaseChord;
            }

            avg = baseChordMemory.Average();

            variance = 0;
            for (int i = 1; i < memoryLength; i++)
            {
                variance = (baseChordMemory[i] - avg) * (baseChordMemory[i] - avg);
            }
            variance = variance / memoryLength;

            if (variance < maximumChordVariance && tmpBaseChord != -1)
            {
                // neuer Akkord gefunden !!!
                baseChord = tmpBaseChord;
                baseChordName = icord2name(baseChord);
                noChordCount = 0;
            }
            else
            {
                if (noChordCount >= 50)     // holds the chord for about 1s
                {
                    baseChord = -1;
                    baseChordName = "";
                }
                else
                {
                    noChordCount++;
                }
            }
        }

        /// <summary>
        /// computes a value for the felt speed between 0=slow -> 1=fast
        /// based on basetone-change, chord-change and BPM
        /// </summary>
        private void computeSpeedFeeling()
        {
            double beatfactor = 0.0;
            double toneFactor = 0.0;
            double chordFactor = 0.0;

            beatfactor = (midBpm - 50) / (MaxBPM - 50);
            if (beatfactor > 1)
                beatfactor = 1.0;
            else if (beatfactor < 0)
                beatfactor = 0.0;

            int changes = 0;
            if (baseToneMemory.Length > 0)
            {
                int last = 0;
                for (int i = 0; i < baseToneMemory.Length; i++)
                {
                    if (baseToneMemory[i] != last)
                    {
                        changes += 1;
                    }
                    last = baseToneMemory[i];
                }
            }
            toneFactor = changes / baseToneMemory.Length;

            changes = 0;
            if (baseChordMemory.Length > 0)
            {
                int last = 0;
                for (int i = 0; i < baseChordMemory.Length; i++)
                {
                    if (baseChordMemory[i] != last)
                    {
                        changes += 1;
                    }
                    last = baseChordMemory[i];
                }
            }
            chordFactor = changes / baseChordMemory.Length;

            tempo = (beatfactor + toneFactor + chordFactor) / 3;
        }

        /// <summary>
        /// computes the felt mood from major - minor in chordhistory 0=minor - 1=major
        /// </summary>
        private void computeMoodFeeling()
        {
            // computes the felt mood from major - minor in chordhistory
        }

        /// <summary>
        /// draws the frequency(y), time(x), soundenergy(color) diagram
        /// </summary>
        /// <param name="init"></param>
        private void drawMoodPicture(bool init)
        {
            Graphics g = moodPictureBox.CreateGraphics();
            Graphics bg = Graphics.FromImage(moodBitmap);
            Brush b = new SolidBrush(Color.Yellow);
            Pen p0 = new Pen(Color.Black);
            Pen p1 = new Pen(Color.Blue);
            Pen p2 = new Pen(Color.LawnGreen);
            Pen p3 = new Pen(Color.OrangeRed);
            Pen p4 = new Pen(Color.White);

            Font f = new Font("Verdana", 8);

            if (init)
            {
                bg.Clear(Color.Black);
                bg.Flush();
                g.DrawImageUnscaled(moodBitmap, 0, 0);
            }
            else
            {
                g.Clear(Color.Black);

                //bg.DrawImageUnscaled(moodBitmap, 1, 0);
                bg.DrawImageUnscaled(moodBitmap, -1, 0);

                //bg.TranslateTransform(1,0);
                int vscale = 1;

                int yoff = (moodBitmap.Height - (maxbands * vscale)) / 2;
                if (yoff < 1)
                    yoff = 1;

                for (int i = 0; i < maxbands; i++)
                {
                    if (subenergy[i] < 0.000001)
                        bg.DrawLine(p0, moodBitmap.Width - 1, moodBitmap.Height - yoff - (i * vscale), moodBitmap.Width - 1, moodBitmap.Height - yoff - ((i * vscale) + 1));
                    else if (subenergy[i] < 0.000005)
                        bg.DrawLine(p1, moodBitmap.Width - 1, moodBitmap.Height - yoff - (i * vscale), moodBitmap.Width - 1, moodBitmap.Height - yoff - ((i * vscale) + 1));
                    else if (subenergy[i] < 0.00001)
                        bg.DrawLine(p2, moodBitmap.Width - 1, moodBitmap.Height - yoff - (i * vscale), moodBitmap.Width - 1, moodBitmap.Height - yoff - ((i * vscale) + 1));
                    else if (subenergy[i] < 0.00005)
                        bg.DrawLine(p3, moodBitmap.Width - 1, moodBitmap.Height - yoff - (i * vscale), moodBitmap.Width - 1, moodBitmap.Height - yoff - ((i * vscale) + 1));
                    else
                        bg.DrawLine(p4, moodBitmap.Width - 1, moodBitmap.Height - yoff - (i * vscale), moodBitmap.Width - 1, moodBitmap.Height - yoff - ((i * vscale) + 1));
                }

                bg.Flush();

                g.DrawImageUnscaled(moodBitmap, 0, 0);
            }
        }

        /// <summary>
        /// fills the history
        /// </summary>
        private void fillStatistics()
        {
            historyEntry mhe = new historyEntry();
            mhe.maxima = new double[maxbands];

            mhe.time = actualTime;
            mhe.beat = beat_detected;
            mhe.basetone = baseToneMidi;
            mhe.chord = baseChord;
            foreach (Tone t in maximumsList)
            {
                mhe.maxima[t.band] = 1;
            }
            // not more then 200 history values
            if (analysisHistory.Count >= 200)
            {
                analysisHistory.Dequeue();
            }
            analysisHistory.Enqueue(mhe);
        }

        /// <summary>
        /// Analyzes the statistics
        /// </summary>
        private void AnalyzeStatistics()
        {

        }

        /// <summary>
        /// returns the Midi-Value of a frequency
        /// </summary>
        /// <param name="freq"></param>
        /// <returns></returns>
        private int freq2Midi(double freq)
        {
            return Convert.ToInt32(69 + 12 * Math.Log((freq / 440), 2));
        }

        /// <summary>
        /// returns the note (as string) from a Midi value
        /// </summary>
        /// <param name="midi"></param>
        /// <returns></returns>
        private string midi2note(int midi)
        {
            string note = "";
            int oktave = 0;
            int noteInOktave = 0;

            //oktave = 8 - floor((131 - MIDI) / 12);
            //note_in_oktave = MIDI - 12 * (oktave + 2);

            oktave = 8 - ((131 - midi) / 12);
            noteInOktave = midi - (12 * (oktave + 2));

            switch (noteInOktave)
            {
                case 0:
                    note = "C";
                    break;

                case 1:
                    note = "C#";
                    break;

                case 2:
                    note = "D";
                    break;

                case 3:
                    note = "Eb";
                    break;

                case 4:
                    note = "E";
                    break;

                case 5:
                    note = "F";
                    break;

                case 6:
                    note = "F#";
                    break;

                case 7:
                    note = "G";
                    break;

                case 8:
                    note = "G#";
                    break;

                case 9:
                    note = "A";
                    break;

                case 10:
                    note = "Bb";
                    break;

                case 11:
                    note = "B";
                    break;

            }
            return (note + " (" + oktave.ToString() + ")");
        }

        /// <summary>
        /// compares 2 tones for sort of tones
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        private static int compareTones(Tone t1, Tone t2)
        {
            if (t1.amplitude < t2.amplitude)
                return 1;
            else if (t1.amplitude == t2.amplitude)
                return 0;
            else
                return -1;
        }

        /// <summary>
        /// shifts a PCP "shift" halftones
        /// </summary>
        /// <param name="PCP"></param>
        /// <param name="shift"></param>
        private void shiftPCP(ref double[] PCP, int shift)
        {
            double[] tmpPCP = new double[12];

            if (shift < 12)
            {
                for (int i = 0; i < PCP.Length; i++)
                {
                    int n = i + shift;
                    if (n >= 12)
                        n = n - 12;

                    tmpPCP[n] = PCP[i];
                }
                PCP = tmpPCP;
            }
        }

        /// <summary>
        /// returns the best matching chord chord compares a PCP to a reference PCP on all 12 halftones
        /// and all 4 chord types (major, minor, major7, minor7
        /// </summary>
        /// <param name="pcp"></param>
        /// <returns></returns>
        private int comparePCP(double[] pcp)
        {
            int imax = -1;
            double max = 0;
            for (int n = 0; n < 4; n++)
            {
                double[] tmpPCP = new double[12];
                for (int i = 0; i < 12; i++)
                {
                    tmpPCP[i] = PCPref[n, i];
                }
                for (int j = 0; j < 12; j++)
                {
                    double p = 0;
                    for (int i = 0; i < pcp.Length; i++)
                    {
                        p = p + pcp[i] * tmpPCP[i];
                    }
                    if (p > max)
                    {
                        max = p;
                        imax = n * 12 + j;
                    }
                    shiftPCP(ref tmpPCP, 1);
                }
            }
            return imax;
        }

        /// <summary>
        /// returns the name of the chord "icord"
        /// </summary>
        /// <param name="icord"></param>
        /// <returns></returns>
        private string icord2name(int icord)
        {
            string bc = "";
            string ct = "";
            int basecord = icord % 12;
            int cordtype = icord / 12;

            switch (cordtype)
            {
                case 0:
                    ct = "";
                    break;
                case 1:
                    ct = "m";
                    break;
                case 2:
                    ct = "7";
                    break;
                case 3:
                    ct = "m7";
                    break;
                default:
                    ct = "";
                    break;
            }

            switch (basecord)
            {
                case 0:
                    bc = "C";
                    break;
                case 1:
                    bc = "C#";
                    break;
                case 2:
                    bc = "D";
                    break;
                case 3:
                    bc = "Eb";
                    break;
                case 4:
                    bc = "E";
                    break;
                case 5:
                    bc = "F";
                    break;
                case 6:
                    bc = "F#";
                    break;
                case 7:
                    bc = "G";
                    break;
                case 8:
                    bc = "G#";
                    break;
                case 9:
                    bc = "A";
                    break;
                case 10:
                    bc = "Bb";
                    break;
                case 11:
                    bc = "B";
                    break;
                default:
                    bc = "";
                    break;
            }
            return (bc + ct);
        }

    }
}