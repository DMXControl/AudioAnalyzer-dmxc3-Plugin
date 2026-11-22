using LumosLIB.Kernel;
using LumosLIB.Tools;
using LumosProtobuf.Input;
using org.dmxc.lumos.Kernel.Input.v2;
using LumosProtobuf;

namespace AudioAnalyzer.Input
{
    /// <summary>
    /// A numeric value of the mood analysis (mood, energy, brightness, key, chord).
    /// </summary>
    class AAMoodSource : AbstractAudioAnalyzerInputSource
    {
        private readonly double _min;
        private readonly double _max;

        public AAMoodSource(string id, string displayName, double min, double max, double initial)
            : base(id, displayName, initial, ParameterCategoryTools.FromName("Mood"))
        {
            _min = min;
            _max = max;
        }

        public void SetValue(double value)
        {
            this.CurrentValue = value.Limit(_min, _max);
        }

        public override EWellKnownInputType AutoGraphIOType => EWellKnownInputType.Numeric;
        public override object Min => _min;
        public override object Max => _max;
    }
}
