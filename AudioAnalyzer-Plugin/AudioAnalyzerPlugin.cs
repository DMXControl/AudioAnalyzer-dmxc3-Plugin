using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using Lumos.GUI.Connection;
using Lumos.GUI.Plugin;
using Lumos.GUI;
using Lumos.GUI.Input;
using org.dmxc.lumos.Kernel.Beat;
using org.dmxc.lumos.Kernel.Exceptions;
using org.dmxc.lumos.Kernel.Resource;
using Lumos.GUI.Resource;
using LumosLIB.Kernel.Log;
using Lumos.GUI.Run;
using LumosProtobuf.Resource;

namespace AudioAnalyzer
{
    public class AudioAnalyzerPlugin : GuiPluginBase
    {
        private static readonly ILumosLog log = LumosLogger.getInstance(typeof(AudioAnalyzerPlugin));

        private const string AudioAnalyzerPluginID = "{2D9C6D8C-431E-4d24-965C-AC4080C25CBA}";
        private static readonly LumosResourceMetadata AnalyzerSettings = new LumosResourceMetadata("AudioAnalyzerSettings.xml", EResourceContentType.ManagedTree);

        private readonly AudioAnalyzerEngine engine;
        private readonly AudioAnalyzer.Wpf.AudioAnalyzerWpfView wpfView;
        private readonly AudioAnalyzerInputSourceFactory inputFactory;

        public AudioAnalyzerPlugin()
            : base(AudioAnalyzerPluginID, "Audio Analyzer")
        {
            engine = new AudioAnalyzerEngine();

            wpfView = new AudioAnalyzer.Wpf.AudioAnalyzerWpfView(engine);

            inputFactory = new AudioAnalyzerInputSourceFactory(engine);
        }

        public override void saveProject(LumosGUIIOContext context)
        {
            base.saveProject(context);

            LumosResource r = new LumosResource(AnalyzerSettings.Name, readSettingsFromAnalyzer());
            Lumos.GUI.Resource.ResourceManager.getInstance().SaveResource(EResourceType.Project, r);
        }

        protected async override void initializePlugin()
        {
            // TODO
            await inputFactory.CreateInputs();
        }

        protected override void loadProjectOrEstablished(LumosGUIIOContext context)
        {
            base.loadProjectOrEstablished(context);

            if (Lumos.GUI.Resource.ResourceManager.getInstance().ExistsResource(EResourceType.Project, AnalyzerSettings))
            {
                LumosResource r = Lumos.GUI.Resource.ResourceManager.getInstance().TryLoadResource(EResourceType.Project, AnalyzerSettings);
                if (r != null && r.ManagedData != null)
                {
                    this.writeSettingsToAnalyzer(r.ManagedData);
                }
            }
        }

        public override void closeProject(LumosGUIIOContext context)
        {
            if (engine.IsRunning)
            {
                engine.stopAudioAnalysis();
            }
            base.closeProject(context);
        }

        protected async override void shutdownPlugin()
        {
            this.wpfView.Close();
            WindowManager.getInstance().RemoveWindow(this.wpfView);

            engine.Dispose();
        }

        protected async override void startupPlugin()
        {
            WindowManager.getInstance().AddWindow(this.wpfView);
        }

        /// <summary>
        /// writes the audio-Analyzer settings to the project
        /// </summary>
        private ManagedTreeItem readSettingsFromAnalyzer()
        {
            log.Info("Saving Data {0} {1}. {2}", "AudioAnalyzer", "Start", 3);
            ManagedTreeItem m = new ManagedTreeItem("AudioAnalyzerSettings.xml");

            ManagedTreeItem c = new ManagedTreeItem("Common");
            c.setValue("Gain", engine.MainGain);
            m.AddChild(c);

            c = new ManagedTreeItem("Device");
            string deviceId = engine.SelectedDeviceId;
            if (!String.IsNullOrEmpty(deviceId))
            {
                c.setValue("DeviceId", deviceId);
                c.setValue("InputChannel", engine.SelectedInputChannel);
            }
            m.AddChild(c);

            c = new ManagedTreeItem("Level");
            c.setValue("VUGain", engine.VuGain);
            c.setValue("PeakHold", engine.PeakHold);
            c.setValue("PeakHoldTime", engine.PeakHoldTime);
            m.AddChild(c);

            c = new ManagedTreeItem("Spectrum");
            c.setValue("SubBands", bandCountToIndex(engine.BandCount));
            c.setValue("SpecGain", engine.SpectrumGain);
            c.setValue("StereoSpectrum", engine.StereoSpectrum);
            m.AddChild(c);

            c = new ManagedTreeItem("Beat");
            c.setValue("Algorithm", engine.BeatAlgorithm);
            c.setValue("Sensitivity", engine.Sensitivity);
            c.setValue("MaxBpmOnOff", engine.MaxBpmLimit);
            c.setValue("MaxBPM", engine.MaxBpm);
            c.setValue("ForecastCount", engine.ForecastCount);
            c.setValue("DoubleSpeed", engine.DoubleSpeed);
            c.setValue("HalfSpeed", engine.HalfSpeed);
            m.AddChild(c);

            c = new ManagedTreeItem("Generator");
            c.setValue("GeneratorOnOff", engine.GeneratorEnabled);
            c.setValue("GeneratorBPM", engine.GeneratorBpm);
            c.setValue("Rhythm", engine.Rhythm);
            m.AddChild(c);

            log.Info("Saving Data {0} {1}. {2}", "AudioAnalyzer", "End", m.Children.Count);

            return m;
        }

        /// <summary>
        /// The band count is stored as the position in the list of offered counts, not as the
        /// count itself - that is how the old combobox wrote it, and existing projects have to
        /// keep loading.
        /// </summary>
        private int bandCountToIndex(int bandCount)
        {
            var counts = engine.AvailableBandCounts;
            for (int i = 0; i < counts.Count; i++)
            {
                if (counts[i] == bandCount)
                    return i;
            }
            return 2;   // 32, der Vorgabewert
        }

        private int indexToBandCount(int index)
        {
            var counts = engine.AvailableBandCounts;
            return (index >= 0 && index < counts.Count) ? counts[index] : 32;
        }

        /// <summary>
        /// reads the audio-Analyzer settings from project
        /// </summary>
        private void writeSettingsToAnalyzer(ManagedTreeItem m)
        {
            log.Info("Reading AudioAnalyzer Settings Start: " + m.Name);

            foreach (ManagedTreeItem i in m.GetChildren("Common"))
            {
                if (i.hasValue<int>("Gain"))
                {
                    engine.MainGain = i.getValue<int>("Gain");
                }
            }

            foreach (ManagedTreeItem i in m.GetChildren("Device"))
            {
                if (i.hasValue<string>("DeviceId"))
                {
                    int channel = i.hasValue<int>("InputChannel") ? i.getValue<int>("InputChannel") : 0;
                    engine.setDevice(i.getValue<string>("DeviceId"), channel);
                }
            }

            foreach (ManagedTreeItem i in m.GetChildren("Level"))
            {
                if (i.hasValue<int>("VUGain"))
                {
                    engine.VuGain = i.getValue<int>("VUGain");
                }
                if (i.hasValue<bool>("PeakHold"))
                {
                    engine.PeakHold = i.getValue<bool>("PeakHold");
                }
                if (i.hasValue<int>("PeakHoldTime"))
                {
                    engine.PeakHoldTime = i.getValue<int>("PeakHoldTime");
                }
            }

            foreach (ManagedTreeItem i in m.GetChildren("Spectrum"))
            {
                if (i.hasValue<bool>("StereoSpectrum"))
                {
                    engine.StereoSpectrum = i.getValue<bool>("StereoSpectrum");
                }

                if (i.hasValue<int>("SubBands"))
                {
                    engine.BandCount = indexToBandCount(i.getValue<int>("SubBands"));
                }

                if (i.hasValue<int>("SpecGain"))
                {
                    engine.SpectrumGain = i.getValue<int>("SpecGain");
                }
            }

            foreach (ManagedTreeItem i in m.GetChildren("Beat"))
            {
                if (i.hasValue<bool>("MaxBpmOnOff"))
                {
                    engine.MaxBpmLimit = i.getValue<bool>("MaxBpmOnOff");
                }

                if (i.hasValue<int>("MaxBPM"))
                {
                    engine.MaxBpm = i.getValue<int>("MaxBPM");
                }

                if (i.hasValue<int>("Algorithm"))
                {
                    engine.BeatAlgorithm = i.getValue<int>("Algorithm");
                }

                if (i.hasValue<int>("Sensitivity"))
                {
                    engine.Sensitivity = i.getValue<int>("Sensitivity");
                }

                if (i.hasValue<int>("ForecastCount"))
                {
                    engine.ForecastCount = i.getValue<int>("ForecastCount");
                }

                if (i.hasValue<bool>("DoubleSpeed"))
                {
                    engine.DoubleSpeed = i.getValue<bool>("DoubleSpeed");
                }

                if (i.hasValue<bool>("HalfSpeed"))
                {
                    engine.HalfSpeed = i.getValue<bool>("HalfSpeed");
                }
            }

            foreach (ManagedTreeItem i in m.GetChildren("Generator"))
            {
                if (i.hasValue<int>("GeneratorBPM"))
                {
                    engine.GeneratorBpm = i.getValue<int>("GeneratorBPM");
                }

                if (i.hasValue<int>("Rhythm"))
                {
                    engine.Rhythm = i.getValue<int>("Rhythm");
                }

                if (i.hasValue<bool>("GeneratorOnOff"))
                {
                    engine.GeneratorEnabled = i.getValue<bool>("GeneratorOnOff");
                }
            }

            engine.recomputeDerivedValues();

            log.Info("Reading AudioAnalyzer Settings End ");
        }
    }
}
