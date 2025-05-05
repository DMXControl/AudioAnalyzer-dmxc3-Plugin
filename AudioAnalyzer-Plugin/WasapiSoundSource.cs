using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioAnalyzer {
    class WasapiSoundSource : AbstractSoundSource {
        private MMDevice _device;
        private WasapiCapture _capture = null;

        public WasapiSoundSource(MMDevice device) {
            this._device = device;
        }

        public override bool Playing {
            get {
                if (_capture == null) return false;
                return _capture.CaptureState.Equals(CaptureState.Capturing);
            }
        }

        public override bool StartRecord() {
            if (_capture != null) StopRecord();

            if (_device.DataFlow.Equals(DataFlow.Render)) {
                _capture = new WasapiLoopbackCapture(_device);
            } else {
                _capture = new WasapiCapture(_device);
            }
            _capture.DataAvailable += Capture_DataAvailable;
            _capture.StartRecording();

            return true;
        }

        private void Capture_DataAvailable(object sender, WaveInEventArgs args) {
            WaveBuffer buffer = new WaveBuffer(args.Buffer);
            // interpret as 32 bit floating point audio
            RaiseSamplesAvailable(buffer.FloatBuffer, args.BytesRecorded / 4, _capture.WaveFormat.Channels, _capture.WaveFormat.SampleRate);
        }

        public override bool StopRecord() {
            if (_capture == null) return false;

            _capture.StopRecording();
            _capture.Dispose();
            _capture = null;

            return true;
        }

        override public String ToString() {
            return _device.FriendlyName + " (WASAPI)";
        }
    }
}
