using System;


namespace AudioAnalyzer
{
    public partial class AudioAnalyzerEngine
    {
        // bpmMemory enthaelt die Zeitstempel (ms, Analyse-Uhr) der letzten erkannten Onsets, neuester vorn.
        // Sie dienen der Impulsdichte der Stimmungsanalyse; das Tempo kommt aus der Onset-Huellkurve.
        private const int BpmWindowSize = 64;
        private const double OnsetWindowMs = 12000;   // aelteste Onsets, die noch zaehlen
        private const double SilenceResetMs = 2500;   // so lange ohne Pegel, dann gilt kein Tempo mehr
        private const long NoDataMs = 200;            // so lange ohne Daten vom Geraet gilt der Pegel als 0
        private long lastChunkMs;
        private const double SilenceLevel = 0.003;
        private const double RelativeSilence = 0.2;    // a fifth of the earlier loudness (-14 dB)
        private const double TempoWindowSeconds = 8;
        private const double TempoUpdateMs = 500;

        // phasenstarres Beat-Raster, siehe UpdateBeatGrid
        private double gridRef;      // Zeit eines Rasterbeats (ms, Analyse-Uhr)
        private double gridPeriod;   // Rasterabstand (ms)
        private double gridNext;     // Zeit des naechsten generierten Beats
        private bool gridValid;
        private double lastGridBeat = double.MinValue;   // scheduled time of the last beat that was sent

        private readonly OnsetEnvelope envelope = new OnsetEnvelope();
        private float[] monoChunk = new float[4096];
        private readonly TempoEstimator tempoEstimator = new TempoEstimator(60, 260);
        private readonly double[] envValues = new double[4096];
        private readonly double[] envLow = new double[4096];
        private readonly double[] envTimes = new double[4096];
        private long lastTempoUpdateMs;
        private long silentSinceMs;

        /// <summary>
        /// Registriert einen erkannten Onset (fuer Impulsdichte und Ausgabe, nicht fuer das Tempo).
        /// </summary>
        private void RecordOnset()
        {
            double now = NowMs;

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
        }

        /// <summary>
        /// Schaetzt das Tempo aus der Onset-Huellkurve (Autokorrelation, siehe TempoEstimator) und
        /// aktualisiert Anzeige, Input Assignment und Beat-Raster. Laeuft alle halbe Sekunde.
        /// </summary>
        private void updateTempo()
        {
            long now = NowMs;

            // Stille: Tempo verwerfen und BPM auf 0 setzen
            // Quiet = no signal at all, or much quieter than the music was a moment ago. The second
            // rule catches a device with a noise floor, which never gets below the absolute limit.
            double loud = Math.Max(levelhistory[0, 0], levelhistory[1, 0]);
            double recent = envelope.RecentLevel(now);
            double reference = envelope.ReferenceLevel;
            bool quiet = loud < SilenceLevel || (reference > 0.002 && recent < reference * RelativeSilence);
            if (quiet)
            {
                if (silentSinceMs == 0)
                    silentSinceMs = now + 1;
                if (now - silentSinceMs > SilenceResetMs && midBpm != 0)
                {
                    resetStatistics();
                }
                return;
            }
            silentSinceMs = 0;

            if (now - lastTempoUpdateMs < TempoUpdateMs)
                return;
            lastTempoUpdateMs = now;

            double fps = envelope.FramesPerSecond;
            int count = envelope.Snapshot(envValues, envLow, envTimes, (int)(fps * TempoWindowSeconds));
            double confidence;
            double bpm = tempoEstimator.Estimate(envValues, count, fps, TempoUpdateMs / 1000.0, out confidence);
            if (bpm <= 0)
                return;

            // a rhythm is there: the grid may keep running, independent of the onset detector
            addedBeats = 0;

            // Kick and snare alternate, so the repetition over two beats is the strongest and the
            // tempo comes out at half speed. If the pulse at twice the tempo repeats nearly as
            // strongly, the fast reading is taken (only on request, it also doubles some tempi
            // around 100-125 BPM that are better left alone).
            if (favorFastTempo && bpm * 2 <= FavorFastMaxBpm
                && tempoEstimator.Repetition(bpm * 2, fps) >= FavorFastRatio * tempoEstimator.Repetition(bpm, fps))
            {
                bpm *= 2;
            }

            int newBpm = (int)Math.Round(bpm);

            // Hysterese: das aktuelle Tempo nur verlassen, wenn ein anderes deutlich besser passt
            if (!tempoEstimator.ShouldSwitch(midBpm, bpm, TempoSwitchFactor, OctaveSwitchFactor))
                return;

            if (midBpm != newBpm && Math.Abs(newBpm - midBpm) > 2)
            {
                // for diagnosing tempo flips: what the tempogram looked like at the switch
                log.Info("Tempo {0} -> {1} BPM (tempogram value {2:F2} -> {3:F2}, confidence {4:F2})",
                    midBpm, newBpm, tempoEstimator.Integrated(midBpm), tempoEstimator.Integrated(bpm), confidence);
            }

            midBpm = newBpm;
            baseBeat = midBpm;

            double speedFactor = halfSpeed ? 2.0 : doubleSpeed ? 0.5 : 1.0;
            double outputTempo = baseBeat / speedFactor;

            // Anzeige und Input Assignment (inkl. Half/Double) informieren
            OnBpmChanged();

            if (forecast)
            {
                double period = 60000.0 / outputTempo;
                double envBeat = TempoEstimator.BestBeatTime(envValues, envLow, envTimes, count, fps, period);
                double anchor = double.IsNaN(envBeat) ? now : envBeat + (now - envTimes[count - 1]);
                UpdateBeatGrid(now, period, anchor);
            }
        }

        private const double FavorFastRatio = 0.5;
        private const double FavorFastMaxBpm = 260;
        private const double TempoSwitchFactor = 1.25;
        private const double OctaveSwitchFactor = 1.25;  // 2.0 kept a wrong first octave for good (Ready Aim Fire: 86 instead of 172)

        /// <summary>
        /// tempo as it is sent to DMXControl: detected tempo with half / double speed applied,
        /// or the generator tempo while the generator is active; 0 while unknown
        /// </summary>
        internal double outputBpm
        {
            get
            {
                if (generatorMode)
                    return generatorBPM;
                if (baseBeat <= 0)
                    return 0;
                return halfSpeed ? baseBeat / 2.0 : doubleSpeed ? baseBeat * 2.0 : baseBeat;
            }
        }

        /// <summary>
        /// Phasenstarres Beat-Raster: richtet das Raster an der Onset-Huellkurve aus und plant den
        /// naechsten generierten Beat. Die Roh-Onsets (auch Achtel und Off-Beats) werden nicht mehr
        /// direkt ausgegeben, solange das Raster laeuft.
        /// </summary>
        private void UpdateBeatGrid(double now, double period, double anchor)
        {
            if (!gridValid || Math.Abs(gridPeriod - period) > period * 0.01)
            {
                gridRef = anchor;
                gridValid = true;
                lastGridBeat = double.MinValue;
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
            // The next beat comes right after the last one that was sent. A beat that is due but not
            // sent yet (the recalculation just ran between its time and the timer tick) is sent
            // as soon as possible instead of being skipped.
            double next = gridRef + Math.Ceiling((now - gridRef) / gridPeriod) * gridPeriod;
            if (lastGridBeat > double.MinValue)
            {
                double afterLast = gridRef + Math.Ceiling((lastGridBeat + 0.5 * gridPeriod - gridRef) / gridPeriod) * gridPeriod;
                if (afterLast < next && afterLast > now - 0.5 * gridPeriod)
                    next = afterLast;
            }
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

        private void doBeatStatisticsAdvanced()
        {
            throw new NotImplementedException();
        }

        private void resetStatistics()
        {
            beatCount = 0;
            gridValid = false;
            envelope.Reset();   // the loudness history stays, it tells silence from a noise floor
            tempoEstimator.Reset();
            lastTempoUpdateMs = 0;
            silentSinceMs = 0;
            OnBpmChanged();
            if (addBeatTimer != null)
                addBeatTimer.Stop();
            baseBeat = 0;
            midBpm = 0;

            if (bpmMemory != null)
                Array.Clear(bpmMemory, 0, bpmMemory.Length);
        }
    }
}
