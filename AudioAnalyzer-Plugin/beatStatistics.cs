using System;


namespace AudioAnalyzer
{
    public partial class audioAnalysForm
    {
        // bpmMemory enthaelt die Zeitstempel (ms, beatClock) der letzten Onsets, neuester vorn.
        private const int BpmWindowSize = 64;
        private const double OnsetWindowMs = 12000;   // aelteste Onsets, die noch zaehlen
        private const double BpmTimeoutMs = 3000;     // so lange ohne Onset, dann gilt kein Tempo mehr
        private const int MinTrackBpm = 70;           // Suchbereich legt die Oktave fest
        private const int MaxTrackBpm = 180;
        private const int MinOnsetsForTempo = 5;

        // phasenstarres Beat-Raster, siehe UpdateBeatGrid
        private double gridRef;      // Zeit eines Rasterbeats (ms, beatClock)
        private double gridPeriod;   // Rasterabstand (ms)
        private double gridNext;     // Zeit des naechsten generierten Beats
        private bool gridValid;

        /// <summary>
        /// Registriert einen erkannten Onset, schaetzt daraus das Tempo und aktualisiert Anzeige,
        /// Input Assignment und Beat-Raster.
        /// </summary>
        private void doBeatStatisticsSimple()
        {
            double now = beatClock.ElapsedMilliseconds;

            // Alte Onsets verwerfen
            while (beatCount > 0 && now - bpmMemory[beatCount - 1] > OnsetWindowMs)
                beatCount--;

            // Neuen Onset vorn einfuegen
            int filled = Math.Min(beatCount, BpmWindowSize - 1);
            for (int i = filled; i > 0; i--)
                bpmMemory[i] = bpmMemory[i - 1];
            bpmMemory[0] = now;
            if (beatCount < BpmWindowSize)
                beatCount += 1;

            if (beatCount < MinOnsetsForTempo)
                return;

            // Tempo schaetzen: Kammfilter ueber alle Onset-Abstaende. Ein Tempo passt gut, wenn
            // viele Abstaende (nahezu) ganzzahlige Vielfache der Periode sind. Das ist robust gegen
            // Fehl-Onsets und gegen die Mindestabstands-Begrenzung (Max BPM), die sonst die
            // Intervalle auf 60000/MaxBPM zieht.
            double bestScore = 0;
            int bestBpm = 0;
            for (int bpm = MinTrackBpm; bpm <= MaxTrackBpm; bpm++)
            {
                double score = ScoreTempo(bpm);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestBpm = bpm;
                }
            }
            if (bestBpm == 0)
                return;

            // Hysterese: das aktuelle Tempo nur verlassen, wenn ein anderes deutlich besser passt
            if (midBpm >= MinTrackBpm && midBpm <= MaxTrackBpm && Math.Abs(bestBpm - midBpm) > 2)
            {
                double current = 0;
                for (int bpm = midBpm - 2; bpm <= midBpm + 2; bpm++)
                {
                    if (bpm >= MinTrackBpm && bpm <= MaxTrackBpm)
                        current = Math.Max(current, ScoreTempo(bpm));
                }
                if (bestScore < current * 1.25)
                    return;
            }

            midBpm = bestBpm;
            baseBeat = midBpm;
            actualBPMLabel.Text = Convert.ToString(baseBeat);

            double speedFactor = halfSpeed ? 2.0 : doubleSpeed ? 0.5 : 1.0;
            double outputBpm = baseBeat / speedFactor;

            // Tempo an das Input Assignment geben (inkl. Half/Double)
            OnSendBpm(outputBpm);

            if (forecast)
            {
                generatedBpmLabel.Text = Convert.ToString(outputBpm);
                UpdateBeatGrid(now, 60000.0 / outputBpm);
            }
        }

        /// <summary>
        /// Phasenstarres Beat-Raster: richtet das Raster an den Onsets aus, die am besten zum
        /// Tempo passen, und plant den naechsten generierten Beat. Die Roh-Onsets (auch Achtel
        /// und Off-Beats) werden nicht mehr direkt ausgegeben.
        /// </summary>
        private void UpdateBeatGrid(double now, double period)
        {
            // Onset waehlen, der am besten zum Raster der letzten Onsets passt
            int n = Math.Min(beatCount, 24);
            double bestScore = -1;
            double anchor = now;
            for (int c = 0; c < n; c++)
            {
                double score = 0;
                for (int o = 0; o < n; o++)
                {
                    double e = WrapPhase(bpmMemory[o] - bpmMemory[c], period) / period / 0.08;
                    score += Math.Exp(-e * e);
                }
                if (score > bestScore)
                {
                    bestScore = score;
                    anchor = bpmMemory[c];
                }
            }

            if (!gridValid || Math.Abs(gridPeriod - period) > 1e-6 && Math.Abs(WrapPhase(anchor - gridRef, period)) > period * 0.35)
            {
                gridRef = anchor;
                gridValid = true;
            }
            else
            {
                // sanft zum neuen Anker ziehen, damit Jitter das Raster nicht wackeln laesst
                gridRef += 0.25 * WrapPhase(anchor - gridRef, period);
            }
            gridPeriod = period;

            ScheduleNextGridBeat(now);
        }

        private void ScheduleNextGridBeat(double now)
        {
            if (!gridValid || gridPeriod <= 0)
                return;
            double next = gridRef + Math.Ceiling((now - gridRef) / gridPeriod) * gridPeriod;
            if (next - now < 1)
                next += gridPeriod;
            gridNext = next;
            addBeatTimer.Stop();
            addBeatTimer.Interval = Math.Max(1, (int)(next - now));
            addBeatTimer.Start();
        }

        /// <summary>Abstand zu naechstem Rasterpunkt, normiert auf [-period/2, period/2).</summary>
        private static double WrapPhase(double diff, double period)
        {
            return diff - Math.Floor(diff / period + 0.5) * period;
        }

        /// <summary>
        /// Kammfilter-Wertung fuer ein Tempo. Dazu ein weicher Tempo-Prior um 120 BPM, damit bei
        /// gleich gut passenden Kandidaten (z.B. 2 gegen 3 Achtel pro Schlag: 116 gegen 77) das
        /// musikalisch uebliche Tempo gewinnt.
        /// </summary>
        private double ScoreTempo(int bpm)
        {
            double period = 60000.0 / bpm;
            double score = 0;
            for (int i = 0; i < beatCount - 1; i++)
            {
                for (int j = i + 1; j < beatCount; j++)
                {
                    double d = bpmMemory[i] - bpmMemory[j];
                    int k = (int)Math.Round(d / period);
                    if (k < 1)
                        continue;
                    if (k > 4)
                        break;
                    double err = (d - k * period) / period / 0.06;
                    score += Math.Exp(-err * err) / k;
                }
            }
            double octaves = Math.Log(bpm / 120.0, 2);
            return score * Math.Exp(-0.5 * octaves * octaves / (0.5 * 0.5));
        }

        private void doBeatStatisticsAdvanced()
        {
            throw new NotImplementedException();
        }

        private void resetStatistics()
        {
            beatCount = 0;
            gridValid = false;
            OnSendBpm(0);
            if (addBeatTimer != null)
                addBeatTimer.Stop();
            baseBeat = 0;
            midBpm = 0;

            if (bpmMemory != null)
                Array.Clear(bpmMemory, 0, bpmMemory.Length);
        }
    }
}
