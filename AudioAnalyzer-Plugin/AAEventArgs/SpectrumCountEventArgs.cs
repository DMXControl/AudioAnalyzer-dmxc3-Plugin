using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioAnalyzer.AAEventArgs
{
    public class SpectrumCountEventArgs : EventArgs
    {
        public readonly int Count;

        public SpectrumCountEventArgs(int count)
        {
            this.Count = count;
        }
    }
}
