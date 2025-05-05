using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioAnalyzer {
    abstract class AbstractSoundSource {
        public delegate void SamplesAvailableHandler(object sender, float[] samples, int count, int channels, int samplerate);
        public event SamplesAvailableHandler SamplesAvailable;

        protected virtual void RaiseSamplesAvailable(float[] samples, int count, int channels, int samplerate) {
            SamplesAvailable?.Invoke(this, samples, count, channels, samplerate);
        }

        public abstract bool Playing { get; }
        public abstract bool StartRecord();
        public abstract bool StopRecord();
    }
}
