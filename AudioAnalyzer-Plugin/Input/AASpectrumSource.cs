using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LumosLIB.Kernel;
using LumosLIB.Tools;
using LumosProtobuf;
using LumosProtobuf.Input;
using org.dmxc.lumos.Kernel.Input.v2;

namespace AudioAnalyzer.Input
{
    class AASpectrumSource : AbstractAudioAnalyzerInputSource
    {
        public AASpectrumSource(int number) 
            : base("{6B78ABC0-D9E4-4c02-997E-023417092587}-" + number, "Spectrum " + number, minimum, ParameterCategoryTools.FromName("Spectrum"))
        {
            Number = number;
        }

        public int Number { get; }

        public void SetLevel(double level)
        {
            this.CurrentValue = level.Limit(0, 1);
        }

        private static object minimum => 0.0;

        public override EWellKnownInputType AutoGraphIOType => EWellKnownInputType.Numeric;
        public override object Min => minimum;
        public override object Max => 1.0;
    }
}
