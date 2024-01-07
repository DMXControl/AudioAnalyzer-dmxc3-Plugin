using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Un4seen.Bass;
using Un4seen.BassAsio;
using Un4seen.Bass.Misc;
using Un4seen.Bass.AddOn.Tags;


namespace AudioAnalyzer
{
    public partial class audioAnalysForm
    {

        private void computeMainAndSubBeatTimes(double BPM, RhythmType actualRythm)
        {
            switch (actualRythm)
            {
                case RhythmType.noRhythm:
                    mainBeatTimer.Interval = (int)(60000 / BPM);
                    numberOfSubbeats = 0;
                    numberOfDrawnBeats = 8;
                    break;
                case RhythmType.fourQuarter:
                    mainBeatTimer.Interval = (int)(60000 * 4 / BPM);
                    subBeatTimer.Interval = (int)(60000 / BPM);
                    numberOfSubbeats = 3;
                    numberOfDrawnBeats = 8;
                    break;
                case RhythmType.threeQuarter:
                    mainBeatTimer.Interval = (int)(60000 * 3 / BPM);
                    subBeatTimer.Interval = (int)(60000 / BPM);
                    numberOfSubbeats = 2;
                    numberOfDrawnBeats = 6;
                    break;
                case RhythmType.twoQuarter:
                    mainBeatTimer.Interval = (int)(60000 * 2 / BPM);
                    subBeatTimer.Interval = (int)(60000 / BPM);
                    numberOfSubbeats = 1;
                    numberOfDrawnBeats = 8;
                    break;
                case RhythmType.bluesRhythm:
                    mainBeatTimer.Interval = (int)(60000 * 2 / BPM);
                    subBeatTimer.Interval = (int)(60000 * 1.33 / BPM);
                    numberOfSubbeats = 1;
                    numberOfDrawnBeats = 8;
                    break;
                case RhythmType.fiveQuarter:
                    mainBeatTimer.Interval = (int)(60000 * 5 / BPM);
                    subBeatTimer.Interval = (int)(60000 / BPM);
                    numberOfSubbeats = 4;
                    numberOfDrawnBeats = 5;
                    break;
            }
        }


        private void mainBeatTimer_Tick(object sender, EventArgs e)
        {
            OnSendBeat(MajorBeat);
            if (numberOfSubbeats > 0)
            {
                subbeatCount = 0;
                subBeatTimer.Start();
            }
            mainBeatTimer.Start();

            drawBeatPicture(drawnBeatsCount);

            if (drawnBeatsCount >= numberOfDrawnBeats - 1)
                drawnBeatsCount = 0;
            else
                drawnBeatsCount += 1;

            beatBox.BackColor = mainBeatColor;
            beatclearTimer2.Start();
        }


        private void subBeatTimer_Tick(object sender, EventArgs e)
        {
            OnSendBeat(MinorBeat);
            subBeatTimer.Stop();
            if (subbeatCount < numberOfSubbeats - 1)
            {
                subBeatTimer.Start();
                subbeatCount += 1;
            }

            drawBeatPicture(drawnBeatsCount);

            if (drawnBeatsCount >= numberOfDrawnBeats - 1)
                drawnBeatsCount = 0;
            else
                drawnBeatsCount += 1;

            beatBox.BackColor = subBeatColor;
            beatclearTimer2.Start();

        }

        private void drawBeatPicture(int step)
        {
            int big = 14;
            int small = 10;
            Pen p = new Pen(Color.Black, 1);
            Color fillColor;
            Color beatColor = Color.Red;
            Color baseColor = Color.Black;
            SolidBrush b;
            SolidBrush b1 = new SolidBrush(baseColor);
            SolidBrush b2 = new SolidBrush(beatColor);


            Graphics g = generatorPictureBox.CreateGraphics();
            int i;

            g.Clear(formBackColor);

            int dx = generatorPictureBox.Width / 8;
            int sx = dx / 2;
            int y = generatorPictureBox.Height / 2;

            for (i = 0; i < numberOfDrawnBeats; i++)
            {
                if (i == drawnBeatsCount)
                {
                    fillColor = beatColor;
                    b = b2;
                }
                else
                {
                    fillColor = baseColor;
                    b = b1;
                }

                if (i < numberOfDrawnBeats - 1)
                {
                    g.DrawLine(p, sx + i * dx, y, sx + (i + 1) * dx + 5, y);
                }

                switch (generatorRhythm)
                {
                    case RhythmType.noRhythm:
                        g.FillEllipse(b, sx + i * dx - big / 2, y - big / 2, big, big);
                        break;
                    case RhythmType.fourQuarter:

                        if (i==0 || i==4)
                            g.FillEllipse(b, sx + i * dx - big / 2, y - big / 2, big, big);
                        else
                            g.FillEllipse(b, sx + i * dx - small / 2, y - small / 2, small, small);

                        break;
                    case RhythmType.threeQuarter:

                        if (i == 0 || i == 3)
                            g.FillEllipse(b, sx + i * dx - big / 2, y - big / 2, big, big);
                        else
                            g.FillEllipse(b, sx + i * dx - small / 2, y - small / 2, small, small);

                        break;
                    case RhythmType.twoQuarter:

                        if (i == 0 || i == 2 || i == 4 || i==6)
                            g.FillEllipse(b, sx + i * dx - big / 2, y - big / 2, big, big);
                        else
                            g.FillEllipse(b, sx + i * dx - small / 2, y - small / 2, small, small);

                        break;
                    case RhythmType.bluesRhythm:

                        if (i == 0 || i == 2 || i == 4 || i == 6)
                            g.FillEllipse(b, sx + i * dx - big / 2, y - big / 2, big, big);
                        else
                            g.FillEllipse(b, sx + i * dx + 5 , y - small / 2, small, small);

                        break;
                    case RhythmType.fiveQuarter:

                        if (i == 0)
                            g.FillEllipse(b, sx + i * dx - big / 2, y - big / 2, big, big);
                        else
                            g.FillEllipse(b, sx + i * dx - small / 2, y - small / 2, small, small);

                        break;
                }
            }

        }


        private void tapTimeout_Tick(object sender, EventArgs e)
        {
            tapWatch.Stop();
            tapWatch.Reset();
            tapCount = 0;
        }

    }
}