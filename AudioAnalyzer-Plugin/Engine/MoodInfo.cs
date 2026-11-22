namespace AudioAnalyzer.Engine
{
    /// <summary>
    /// Result of the mood analysis for a surface or the input sources.
    /// </summary>
    public class MoodInfo
    {
        public MoodInfo(double mood, double energy, double brightness, int keyRoot, bool keyMinor, int chordRoot, bool chordMinor, string keyName, string chordName)
        {
            Mood = mood;
            Energy = energy;
            Brightness = brightness;
            KeyRoot = keyRoot;
            KeyMinor = keyMinor;
            ChordRoot = chordRoot;
            ChordMinor = chordMinor;
            KeyName = keyName;
            ChordName = chordName;
        }

        /// <summary>0 = minor / sad, 1 = major / happy, 0.5 = unknown</summary>
        public double Mood { get; }

        /// <summary>0 = calm, 1 = fast / intense</summary>
        public double Energy { get; }

        /// <summary>0 = dark, 1 = bright (spectral centroid)</summary>
        public double Brightness { get; }

        /// <summary>pitch class 0-11 (C = 0) of the tonic of the detected key, -1 if unknown</summary>
        public int KeyRoot { get; }

        public bool KeyMinor { get; }

        /// <summary>pitch class of the current chord root, -1 if there is none</summary>
        public int ChordRoot { get; }

        public bool ChordMinor { get; }

        public string KeyName { get; }

        public string ChordName { get; }
    }
}
