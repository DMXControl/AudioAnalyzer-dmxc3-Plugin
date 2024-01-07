using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioAnalyzer.AAEventArgs
{
    class SpectrumEventArgs : EventArgs
    {
        public readonly double[] SubLevel;

        public SpectrumEventArgs(double[] spectrum)
        {
            this.SubLevel = spectrum;
        }
    }
}
