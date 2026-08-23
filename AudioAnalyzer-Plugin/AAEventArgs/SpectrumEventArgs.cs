using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioAnalyzer.AAEventArgs
{
    enum ESpectrumChannel
    {
        Mono,
        Left,
        Right
    }

    class SpectrumEventArgs : EventArgs
    {
        public readonly double[] SubLevel;
        public readonly ESpectrumChannel Channel;

        public SpectrumEventArgs(double[] spectrum, ESpectrumChannel channel)
        {
            this.SubLevel = spectrum;
            this.Channel = channel;
        }
    }
}
