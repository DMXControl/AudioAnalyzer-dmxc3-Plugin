using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LumosLIB.Kernel;
using LumosProtobuf;
using org.dmxc.lumos.Kernel.Input.v2;

namespace AudioAnalyzer.Input
{
    abstract class AbstractAudioAnalyzerInputSource : AbstractInputSource
    {
        protected AbstractAudioAnalyzerInputSource(string id, string displayName, object initialValue, ParameterCategory subCategory = null)
            : base(id, displayName, ParameterCategoryTools.FromName("Audio Analyzer").Combine(subCategory, false), initialValue)
        {
        }
    }
}
