using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AudioAnalyzer.AAEventArgs;
using AudioAnalyzer.Engine;
using AudioAnalyzer.Input;
using Lumos.GUI.Input;
using Lumos.GUI.Input.v2;
using org.dmxc.lumos.Kernel.Beat;
using org.dmxc.lumos.Kernel.Input;
using System.Threading.Tasks;

namespace AudioAnalyzer
{
    /// <summary>
    /// Publishes the measured values to DMXControl as input sources.
    /// </summary>
    public class AudioAnalyzerInputSourceFactory : IDisposable
    {
        private readonly IAudioAnalyzer _analyzer;

        private readonly EventHandler<BeatEventArgs> _sendBeatDelegate;
        private readonly EventHandler<LevelEventArgs> _sendLevelDelegate;

        private readonly List<AASpectrumSource> _spectrum = new List<AASpectrumSource>();
        private readonly List<AASpectrumSource> _spectrumRight = new List<AASpectrumSource>();
        private AAVolumeSource volL, volR;
        private AABeatSource beat;

        public AudioAnalyzerInputSourceFactory(IAudioAnalyzer analyzer)
        {
            this._analyzer = analyzer;

            _sendBeatDelegate = (s, args)
                => beat?.IncrementBeat();
            analyzer.BeatDetected += _sendBeatDelegate;

            _sendLevelDelegate = (s, args)
                =>
                {
                    volL?.SetVolume(args.VolumeL);
                    volR?.SetVolume(args.VolumeR);
                };
            analyzer.LevelChanged += _sendLevelDelegate;

            analyzer.SpectrumChanged += UpdateSpectrum;

            analyzer.SpectrumLayoutChanged += UpdateSpectrumInputs;
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

                SyncSpectrumSources(_analyzer.BandCount, _analyzer.StereoSpectrum);
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
                _spectrumRight.Clear();
            });
        }

        private void UpdateSpectrumInputs(object sender, SpectrumCountEventArgs args)
        {
            SyncSpectrumSources(args.Count, args.Stereo);
        }

        /// <summary>
        /// brings the registered spectrum sources in line with band count and stereo mode.
        /// In stereo mode the existing set carries the left channel and only renames itself,
        /// so just one additional set is needed instead of two - that saves a third of the
        /// value updates compared to keeping a separate downmix alongside L and R.
        /// </summary>
        private void SyncSpectrumSources(int count, bool stereo)
        {
            // Vorhandene Quellen zuerst umbenennen
            foreach (var s in _spectrum)
            {
                s.SetStereoNaming(stereo);
            }

            SyncSpectrumChannel(_spectrum, ESpectrumChannel.Mono, count, stereo);
            SyncSpectrumChannel(_spectrumRight, ESpectrumChannel.Right, stereo ? count : 0, stereo);

            // Die Bereiche im Label hängen an der Bandanzahl, müssen also mitgezogen werden
            UpdateBandRanges(_spectrum);
            UpdateBandRanges(_spectrumRight);
        }

        private void UpdateBandRanges(List<AASpectrumSource> sources)
        {
            foreach (var s in sources)
            {
                s.SetBandRange(_analyzer.GetBandRangeLabel(s.Number));
            }
        }

        private void SyncSpectrumChannel(List<AASpectrumSource> sources, ESpectrumChannel channel, int count, bool stereo)
        {
            if (count > sources.Count)
            {
                var added = new List<AASpectrumSource>();
                for (int i = sources.Count + 1; i <= count; i++)
                {
                    var s = new AASpectrumSource(i, channel, stereo);
                    s.SetBandRange(_analyzer.GetBandRangeLabel(i));
                    sources.Add(s);
                    added.Add(s);
                }
                InputManager.getInstance().RegisterSources(added);
            }
            else if (count < sources.Count)
            {
                var removed = sources.GetRange(count, sources.Count - count);
                sources.RemoveRange(count, sources.Count - count);
                InputManager.getInstance().UnregisterSources(removed);
            }
        }

        private void UpdateSpectrum(object sender, SpectrumEventArgs args)
        {
            // Mono und Left landen im selben Satz: im Stereo-Modus tragen diese Quellen
            // den linken Kanal, sonst den Downmix
            List<AASpectrumSource> sources = args.Channel == ESpectrumChannel.Right
                ? _spectrumRight
                : _spectrum;

            for (int index = 0; index < args.SubLevel.Length && index < sources.Count; index++)
            {
                sources[index].SetLevel(args.SubLevel[index]);
            }
        }

        #region IDisposable Members

        public bool IsDisposed;

        public void Dispose()
        {
            if (IsDisposed)
                return;
            IsDisposed = true;

            _analyzer.BeatDetected -= _sendBeatDelegate;
            _analyzer.LevelChanged -= _sendLevelDelegate;
            _analyzer.SpectrumChanged -= UpdateSpectrum;
            _analyzer.SpectrumLayoutChanged -= UpdateSpectrumInputs;
        }

        #endregion
    }
}
