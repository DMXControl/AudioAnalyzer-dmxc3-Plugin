using System;
using LumosLIB.Kernel;
using LumosProtobuf;
using LumosProtobuf.Input;
using org.dmxc.lumos.Kernel.Input.v2;

namespace AudioAnalyzer.Input
{
    /// <summary>
    /// Switches the audio analysis on and off from the Input Assignment (true = running).
    /// </summary>
    class AAActiveSink : AbstractInputSink
    {
        private readonly Action<bool> _setActive;

        public AAActiveSink(Action<bool> setActive)
            : base("{7148676B-4AE1-49FE-ABD3-A3A6B1BB8A0B}", "Analysis active", ParameterCategoryTools.FromName("Audio Analyzer"))
        {
            _setActive = setActive;
        }

        public override bool UpdateValue(object newValue)
        {
            bool? value = ConvertToBoolResult(newValue);
            if (value == null)
                return false;

            _setActive(value.Value);
            return true;
        }

        public override EWellKnownInputType AutoGraphIOType => EWellKnownInputType.Bool;
        public override object Min => false;
        public override object Max => true;
    }
}
