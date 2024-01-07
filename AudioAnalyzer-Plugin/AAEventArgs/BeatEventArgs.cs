using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioAnalyzer.AAEventArgs
{
    class BeatEventArgs : EventArgs
    {
        public readonly int MajorBeat;

        public BeatEventArgs(int majorBeat)
        {
            this.MajorBeat = majorBeat;
        }
    }
}
