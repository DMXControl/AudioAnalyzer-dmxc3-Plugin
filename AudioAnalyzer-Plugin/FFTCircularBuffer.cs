using NAudio.Dsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioAnalyzer {
    class FFTCircularBuffer {
        private double[] _buffer;
        private Complex[] _fftBuffer;
        private double[] _window;
        private int _m;
        private int _pos;
        private bool _isFull;

        public FFTCircularBuffer(int fftLength) {
            if (!IsPowerOfTwo(fftLength)) {
                throw new ArgumentException("FFT Length must be a power of two");
            }
            _m = (int)Math.Log(fftLength, 2);
            _buffer = new double[fftLength];
            _fftBuffer = new Complex[fftLength];
            // Das Fenster ist konstant, es je Aufruf neu zu berechnen kostet die Hälfte
            // der gesamten FFT-Zeit (8192 Math.Cos pro Durchlauf)
            _window = new double[fftLength];
            for (int i = 0; i < fftLength; ++i) {
                _window[i] = FastFourierTransform.HannWindow(i, fftLength);
            }
            _pos = 0;
            _isFull = false;
        }

        bool IsPowerOfTwo(int x) {
            return (x & (x - 1)) == 0;
        }

        public void Reset() {
            lock (_buffer) {
                Array.Clear(_buffer, 0, _buffer.Length);
                _pos = 0;
                _isFull = false;
            }
        }

        public void Add(float value) {
            lock (_buffer) {
                _buffer[_pos] = value;
                // move the current position (advances by 1 OR resets to zero if the length of the buffer was reached)
                _pos = (_pos + 1) % _buffer.Length;
                // flag if the buffer is full
                _isFull |= (_pos == 0);
            }
        }

        public bool CalculateFft(in float[] fft) {
            if (!_isFull) return false;
            Complex[] fftBuffer = _fftBuffer;

            lock (_buffer) {
                int toEnd = _isFull ? (_buffer.Length - _pos) : 0;
                if (toEnd > 0) {
                    // From the current position to the end
                    for (int i = 0; i < toEnd; ++i) {
                        fftBuffer[i].X = (float)(_buffer[_pos + i] * _window[i]);
                        fftBuffer[i].Y = 0;
                    }
                }
                if (_pos > 0) {
                    // From the start to the current position
                    for (int i = 0; i < _pos; ++i) {
                        fftBuffer[toEnd + i].X = (float)(_buffer[i] * _window[toEnd + i]);
                        fftBuffer[toEnd + i].Y = 0;
                    }
                }
            }

            FastFourierTransform.FFT(true, _m, fftBuffer);

            for (int i = 0; i < fft.Length; ++i) {
                Complex c = fftBuffer[i];
                float magnitude = (float)Math.Sqrt(c.X * c.X + c.Y * c.Y);
                fft[i] = magnitude;
            }

            return true;
        }
    }
}
