using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioAnalyzer.AAEventArgs
{
    public enum ESpectrumChannel
    {
        Mono,
        Left,
        Right
    }

    public class SpectrumEventArgs : EventArgs
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
