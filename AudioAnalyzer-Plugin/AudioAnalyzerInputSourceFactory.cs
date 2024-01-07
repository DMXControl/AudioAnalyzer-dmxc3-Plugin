using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AudioAnalyzer.AAEventArgs;
using AudioAnalyzer.Input;
using Lumos.GUI.Input;
using Lumos.GUI.Input.v2;
using org.dmxc.lumos.Kernel.Beat;
using org.dmxc.lumos.Kernel.Input;
using System.Threading.Tasks;

namespace AudioAnalyzer
{
    public class AudioAnalyzerInputSourceFactory : IDisposable
    {
        private readonly audioAnalysForm _form;

        private readonly EventHandler<BeatEventArgs> _sendBeatDelegate;
        private readonly EventHandler<LevelEventArgs> _sendLevelDelegate;

        private readonly List<AASpectrumSource> _spectrum = new List<AASpectrumSource>();
        private AAVolumeSource volL, volR;
        private AABeatSource beat;

        public AudioAnalyzerInputSourceFactory(audioAnalysForm form)
        {
            this._form = form;

            _sendBeatDelegate = (s, args)
                => beat?.IncrementBeat();
            form.SendBeat += _sendBeatDelegate;

            _sendLevelDelegate = (s, args)
                =>
                {
                    volL?.SetVolume(args.VolumeL);
                    volR?.SetVolume(args.VolumeR);
                };
            form.SendLevel += _sendLevelDelegate;

            form.SendSpectrum += UpdateSpectrum;

            form.SendSpectrumCount += UpdateSpectrumInputs;
        }

        public async Task CreateInputs()
        {
            await Task.Run(async () =>
            {
                await RemoveInputs(); //Remove all First

                beat = new AABeatSource();
                InputManager.getInstance().RegisterSource(beat);

                volL = new AAVolumeSource("{384DDEDC-378E-4306-9999-E05632FFF91F}", "Volume L");
                volR = new AAVolumeSource("{9B3C13BC-9FBF-4b93-A18D-4BEA86616EA8}", "Volume R");
                InputManager.getInstance().RegisterSource(volL);
                InputManager.getInstance().RegisterSource(volR);

                for (int i = 1; i <= 32; i++)
                {
                    var s = new AASpectrumSource(i);
                    this._spectrum.Add(s);
                }
                InputManager.getInstance().RegisterSources(this._spectrum);
            });
        }

        public async Task RemoveInputs()
        {
            await Task.Run(() =>
            {
                var all = InputManager.getInstance().Sources
                    .OfType<AbstractAudioAnalyzerInputSource>()
                    .ToList();

                InputManager.getInstance().UnregisterSources(all);

                beat = null;
                volL = null;
                volR = null;
                _spectrum.Clear();
            });
        }

        private void UpdateSpectrumInputs(object sender, SpectrumCountEventArgs args)
        {
            if (args.Count >= _spectrum.Count)
            {
                for (int i = _spectrum.Count + 1; i <= args.Count; i++)
                {
                    var s = new AASpectrumSource(i);
                    this._spectrum.Add(s);
                    InputManager.getInstance().RegisterSource(s);
                }
            }
            else if (args.Count < _spectrum.Count)
            {
                do
                {
                    int i = _spectrum.Count - 1;
                    var x = _spectrum[i];
                    InputManager.getInstance().UnregisterSource(x);
                    _spectrum.RemoveAt(i);
                } while (args.Count < _spectrum.Count);
            }
        }

        private void UpdateSpectrum(object sender, SpectrumEventArgs args)
        {
            for (int index = 0; index < args.SubLevel.Length && index < _spectrum.Count; index++)
            {
                _spectrum[index].SetLevel(args.SubLevel[index]);
            }
        }

        #region IDisposable Members

        public bool IsDisposed;

        public void Dispose()
        {
            if (IsDisposed)
                return;
            IsDisposed = true;

            _form.SendBeat -= _sendBeatDelegate;
            _form.SendLevel -= _sendLevelDelegate;
            _form.SendSpectrum -= UpdateSpectrum;
            _form.SendSpectrumCount -= UpdateSpectrumInputs;
        }

        #endregion
    }
}
