using System;
using AudioAnalyzer.AAEventArgs;


namespace AudioAnalyzer
{
    public partial class AudioAnalyzerEngine
    {

        private void computeMainAndSubBeatTimes(double BPM, RhythmType actualRythm)
        {
            // Die Timer entstehen in initializeAnalysis. Ein Projekt kann eine BPM setzen,
            // bevor das durch ist - dann holt initializeAnalysis den Aufruf am Ende nach.
            if (mainBeatTimer == null || subBeatTimer == null)
                return;

            switch (actualRythm)
            {
                case RhythmType.noRhythm:
                    mainBeatTimer.Interval = (int)(60000 / BPM);
                    numberOfSubbeats = 0;
                    break;
                case RhythmType.fourQuarter:
                    mainBeatTimer.Interval = (int)(60000 * 4 / BPM);
                    subBeatTimer.Interval = (int)(60000 / BPM);
                    numberOfSubbeats = 3;
                    break;
                case RhythmType.threeQuarter:
                    mainBeatTimer.Interval = (int)(60000 * 3 / BPM);
                    subBeatTimer.Interval = (int)(60000 / BPM);
                    numberOfSubbeats = 2;
                    break;
                case RhythmType.twoQuarter:
                    mainBeatTimer.Interval = (int)(60000 * 2 / BPM);
                    subBeatTimer.Interval = (int)(60000 / BPM);
                    numberOfSubbeats = 1;
                    break;
                case RhythmType.bluesRhythm:
                    mainBeatTimer.Interval = (int)(60000 * 2 / BPM);
                    subBeatTimer.Interval = (int)(60000 * 1.33 / BPM);
                    numberOfSubbeats = 1;
                    break;
                case RhythmType.fiveQuarter:
                    mainBeatTimer.Interval = (int)(60000 * 5 / BPM);
                    subBeatTimer.Interval = (int)(60000 / BPM);
                    numberOfSubbeats = 4;
                    break;
            }
        }


        private void mainBeatTimer_Tick(object sender, EventArgs e)
        {
            OnBeatDetected(MajorBeat, EBeatSource.GeneratorMain);
            if (numberOfSubbeats > 0)
            {
                subbeatCount = 0;
                subBeatTimer.Start();
            }
            mainBeatTimer.Start();
        }


        private void subBeatTimer_Tick(object sender, EventArgs e)
        {
            OnBeatDetected(MinorBeat, EBeatSource.GeneratorSub);
            subBeatTimer.Stop();
            if (subbeatCount < numberOfSubbeats - 1)
            {
                subBeatTimer.Start();
                subbeatCount += 1;
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