using NAudio.Dsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioAnalyzer {
    class FFTCircularBuffer {
        private double[] _buffer;
        private int _m;
        private int _pos;
        private bool _isFull;

        public FFTCircularBuffer(int fftLength) {
            if (!IsPowerOfTwo(fftLength)) {
                throw new ArgumentException("FFT Length must be a power of two");
            }
            _m = (int)Math.Log(fftLength, 2);
            _buffer = new double[fftLength];
            _pos = 0;
            _isFull = false;
        }

        bool IsPowerOfTwo(int x) {
            return (x & (x - 1)) == 0;
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
            Complex[] fftBuffer = new Complex[_buffer.Length];

            lock (_buffer) {
                int toEnd = _isFull ? (_buffer.Length - _pos) : 0;
                if (toEnd > 0) {
                    // From the current position to the end
                    for (int i = 0; i < toEnd; ++i) {
                        fftBuffer[i].X = (float)(_buffer[_pos + i] * FastFourierTransform.HannWindow(i, _buffer.Length));
                        fftBuffer[i].Y = 0;
                    }
                }
                if (_pos > 0) {
                    // From the start to the current position
                    for (int i = 0; i < _pos; ++i) {
                        fftBuffer[toEnd + i].X = (float)(_buffer[i] * FastFourierTransform.HannWindow(toEnd + i, _buffer.Length));
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
