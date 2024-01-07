using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LumosLIB.Kernel;
using LumosProtobuf.Input;
using org.dmxc.lumos.Kernel.Input.v2;

namespace AudioAnalyzer.Input
{
    class AABeatSource : AbstractAudioAnalyzerInputSource
    {
        public AABeatSource() 
            : base("{AB3DAFB8-EF18-42d4-935E-DC2088023438}", "Beat", ulong.MinValue)
        {
        }

        public void IncrementBeat()
        {
            if (!(this.CurrentValue is ulong))
                this.CurrentValue = (ulong)1;
            else
                this.CurrentValue = (ulong)this.CurrentValue + 1;
        }

        public override EWellKnownInputType AutoGraphIOType => EWellKnownInputType.Beat;

        public override object Min => ulong.MinValue;

        public override object Max => ulong.MaxValue;
    }
}
