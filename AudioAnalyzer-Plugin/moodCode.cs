using System;
using AudioAnalyzer.Engine;

namespace AudioAnalyzer
{
    public partial class AudioAnalyzerEngine
    {
        /// <summary>current mood analysis (key, chord, mood, energy, brightness)</summary>
        public MoodInfo Mood => new MoodInfo(mood.Mood, mood.Energy, mood.Brightness, mood.KeyRoot, mood.KeyMinor, mood.ChordRoot, mood.ChordMinor, mood.KeyName, mood.ChordName);

        /// <summary>
        /// feeds the mood analysis with the current semitone spectrum and reports the result.
        /// Runs on every analysis tick while the analysis is active.
        /// </summary>
        private void updateMood()
        {
            long now = NowMs;
            double dt = (now - lastMoodTime) / 1000.0;
            lastMoodTime = now;

            // onsets per second over the last four seconds
            int recent = 0;
            for (int i = 0; i < beatCount; i++)
            {
                if (now - bpmMemory[i] > 4000)
                    break;
                recent++;
            }

            double loudness = Math.Max(levelhistory[0, 0], levelhistory[1, 0]);
            mood.Update(dt, sublevel, midBpm, recent / 4.0, loudness);

            OnMoodChanged();
        }

        private void resetMood()
        {
            mood.Reset();
            lastMoodTime = beatClock != null || TimeSource != null ? NowMs : 0;
            OnMoodChanged();
        }
    }
}
