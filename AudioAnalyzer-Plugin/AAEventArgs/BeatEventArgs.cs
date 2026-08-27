using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioAnalyzer.AAEventArgs
{
    /// <summary>
    /// Where a beat came from.
    /// </summary>
    public enum EBeatSource
    {
        /// <summary>found in the audio signal</summary>
        Detected,
        /// <summary>predicted between detected beats</summary>
        Predicted,
        /// <summary>generator, beat on the bar</summary>
        GeneratorMain,
        /// <summary>generator, beat within the bar</summary>
        GeneratorSub
    }

    public class BeatEventArgs : EventArgs
    {
        public readonly int MajorBeat;
        public readonly EBeatSource Source;

        public BeatEventArgs(int majorBeat, EBeatSource source)
        {
            this.MajorBeat = majorBeat;
            this.Source = source;
        }
    }
}
