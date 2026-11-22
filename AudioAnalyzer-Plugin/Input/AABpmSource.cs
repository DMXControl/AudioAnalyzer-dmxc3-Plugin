using LumosLIB.Tools;
using LumosProtobuf.Input;
using org.dmxc.lumos.Kernel.Input.v2;

namespace AudioAnalyzer.Input
{
    /// <summary>
    /// Provides the detected tempo in beats per minute (0 = no tempo detected).
    /// </summary>
    class AABpmSource : AbstractAudioAnalyzerInputSource
    {
        public AABpmSource() : base("{04E3CB95-21BD-4588-9814-99C7A111374E}", "BPM", minimum)
        {
        }

        public void SetBpm(double bpm)
        {
            this.CurrentValue = bpm.Limit(0, 300);
        }

        private static object minimum => 0.0;

        public override EWellKnownInputType AutoGraphIOType => EWellKnownInputType.Numeric;
        public override object Min => minimum;
        public override object Max => 300.0;
    }
}
