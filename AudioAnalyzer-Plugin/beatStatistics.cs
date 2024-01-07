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
        private void doBeatStatisticsSimple(long beatTime)
        {
            int n;
            int treffer;

            if (beatCount > 0)
            {
                for (int i = beatMemory.Length - 2; i <= 0; i--)
                {
                    bpmMemory[i + 1] = bpmMemory[i];
                }
            }

            bpmMemory[0] = (int)(60000 / beatTime);

            //Console.WriteLine("BPM: " + bpmMemory[0]);

            if (beatCount < 200)
                beatCount += 1;

            for (int i = 0; i < beatCount; i++)
            {
                n = (bpmMemory[i] - 50) / 10;
                if (n < bpmDistribution.Length && n >= 0)
                {
                    bpmDistribution[n] += 1;
                }
            }

            treffer = bpmDistribution.Sum();

            // Erwartungswert midBPM
            if (treffer > 0)
            {
                midBpm = 0;
                for (int i = 0; i < bpmDistribution.Length; i++)
                {
                    midBpm = midBpm + (50 + i * 10) * bpmDistribution[i];
                }
                midBpm = midBpm / treffer;

                baseBeat = midBpm;

                actualBPMLabel.Text = Convert.ToString(baseBeat);

                if (forecast)
                {
                    addBeatTimer.Stop();
                    addBeatTimer.Interval = (int)(60000 / (baseBeat - tolBpm));
                    if (halfSpeed)
                    {
                        addBeatTimer.Interval = addBeatTimer.Interval * 2;
                        generatedBpmLabel.Text = Convert.ToString(baseBeat / 2);
                    }
                    if (doubleSpeed)
                    {
                        addBeatTimer.Interval = addBeatTimer.Interval / 2;
                        generatedBpmLabel.Text = Convert.ToString(baseBeat * 2);
                    }
                    else
                    {
                        addBeatTimer.Interval = addBeatTimer.Interval;
                        generatedBpmLabel.Text = Convert.ToString(baseBeat);
                    }

                    addBeatTimer.Start();
                }
            }
        }

        private void doBeatStatisticsAdvanced()
        {
            throw new NotImplementedException();
        }

        private void resetStatistics()
        {
            beatCount = 0;
            baseBeat = 0;
            midBpm = 0;

            //TimeSpan leer=TimeSpan.FromTicks(0);
            if (beatMemory != null)
            {
                for (int i = 0; i < beatMemory.Length; i++)
                {
                    bpmMemory[i] = 0;
                }
            }

            if (bpmDistribution != null)
            {
                for (int i = 0; i < bpmDistribution.Length; i++)
                    bpmDistribution[i] = 0;
            }
        }
    }
}
