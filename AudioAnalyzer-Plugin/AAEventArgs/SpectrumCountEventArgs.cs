using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioAnalyzer.AAEventArgs
{
    public class SpectrumCountEventArgs : EventArgs
    {
        public readonly int Count;
        public readonly bool Stereo;

        public SpectrumCountEventArgs(int count, bool stereo)
        {
            this.Count = count;
            this.Stereo = stereo;
        }
    }
}
