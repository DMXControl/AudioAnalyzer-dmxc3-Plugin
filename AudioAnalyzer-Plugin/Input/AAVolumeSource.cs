using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LumosLIB.Tools;
using LumosProtobuf.Input;
using org.dmxc.lumos.Kernel.Input.v2;

namespace AudioAnalyzer.Input
{
    class AAVolumeSource : AbstractAudioAnalyzerInputSource
    {
        public AAVolumeSource(string id, string displayName) : base(id, displayName, minimum)
        {

        }

        public void SetVolume(double volume)
        {
            this.CurrentValue = volume.Limit(0, 1);
        }

        private static object minimum => 0.0;

        public override EWellKnownInputType AutoGraphIOType => EWellKnownInputType.Numeric;
        public override object Min => minimum;
        public override object Max => 1.0;
    }
}
