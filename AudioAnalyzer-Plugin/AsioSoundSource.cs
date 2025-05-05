using NAudio.Wave;
using NAudio.Wave.Asio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioAnalyzer {
    class AsioSoundSource : AbstractSoundSource {
        private string _driverName;
        private int _channel;
        AsioOut _capture = null;
        float[] _buffer = null;

        public AsioSoundSource(string driverName) {
            this._driverName = driverName;
            this._channel = 0;
        }

        public override bool Playing => true;

        public string[] getInputNames() {
            List<string> channels = new List<string>();

            try {
                AsioOut asioOut = new AsioOut(_driverName);
                int channelCount = asioOut.DriverInputChannelCount;
                for (int i = 0; i < channelCount; ++i) {
                    channels.Add(asioOut.AsioInputChannelName(i));
                }
            } catch (Exception) {
            }
            return channels.ToArray();
        }

        public int InputChannel {
            get { return _channel; }
            set { _channel = value; }
        }

        public override bool StartRecord() {
            if (_capture != null) StopRecord();

            _capture = new AsioOut(_driverName);
            _capture.InputChannelOffset = _channel;
            _capture.InitRecordAndPlayback(null, 1, 48000);
            _capture.AudioAvailable += Capture_DataAvailable;
            _capture.Play();

            return true;
        }

        private void Capture_DataAvailable(object sender, AsioAudioAvailableEventArgs args) {
            if (_buffer == null) {
                _buffer = new float[args.SamplesPerBuffer * args.InputBuffers.Length];
            }

            args.GetAsInterleavedSamples(_buffer);
            RaiseSamplesAvailable(_buffer, _buffer.Length, 1, 48000);
        }

        public override bool StopRecord() {
            if (_capture == null) return false;

            _capture.Stop();
            _capture.Dispose();
            _capture = null;

            if (_buffer != null) {
                _buffer = null;
            }

            return true;
        }

        public override string ToString() {
            return _driverName + " (ASIO)";
        }
    }
}
