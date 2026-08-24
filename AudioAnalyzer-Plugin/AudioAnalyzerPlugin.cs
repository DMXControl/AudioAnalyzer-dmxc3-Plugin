using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Windows.Forms;
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
        
        private readonly audioAnalysForm pluginForm;
        private readonly AudioAnalyzerInputSourceFactory inputFactory;

        public AudioAnalyzerPlugin()
            : base(AudioAnalyzerPluginID, "Audio Analyzer")
        {
            pluginForm = new audioAnalysForm();
            
            inputFactory = new AudioAnalyzerInputSourceFactory(pluginForm);
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
            if (pluginForm.running)
            {
                pluginForm.stopAudioAnalysis();
            }
            base.closeProject(context);
        }

        protected async override void shutdownPlugin()
        {
            if (pluginForm.running)
            {
                pluginForm.stopAudioAnalysis();
            }
            this.pluginForm.Close();
            WindowManager.getInstance().RemoveWindow(this.pluginForm);
        }

        protected async override void startupPlugin()
        {
            WindowManager.getInstance().AddWindow(this.pluginForm);
        }

        /// <summary>
        /// writes the audio-Analyzer settings to the project
        /// </summary>
        /// <returns></returns>
        private ManagedTreeItem readSettingsFromAnalyzer()
        {
            log.Info("Saving Data {0} {1}. {2}", "AudioAnalyzer", "Start", 3);
            ManagedTreeItem m = new ManagedTreeItem("AudioAnalyzerSettings.xml");

            ManagedTreeItem c = new ManagedTreeItem("Common");
            c.setValue("Gain", pluginForm.gainBar.Value);
            m.AddChild(c);

            c = new ManagedTreeItem("Device");
            string deviceId = pluginForm.getSelectedDeviceId();
            if (!String.IsNullOrEmpty(deviceId))
            {
                c.setValue("DeviceId", deviceId);
                c.setValue("InputChannel", pluginForm.getSelectedInputChannel());
            }
            m.AddChild(c);

            c = new ManagedTreeItem("Level");
            c.setValue("VUGain", pluginForm.gainVUBar.Value);
            c.setValue("PeakHold", pluginForm.PeakHoldCheckBox.Checked);
            c.setValue("PeakHoldTime", pluginForm.PeakHoldBar.Value);
            m.AddChild(c);

            c = new ManagedTreeItem("Spectrum");
            c.setValue("SubBands", pluginForm.subBandBox.SelectedIndex);
            c.setValue("SpecGain", pluginForm.gainSpectrumBar.Value);
            c.setValue("StereoSpectrum", pluginForm.stereoSpectrumCheckBox.Checked);
            m.AddChild(c);

            c = new ManagedTreeItem("Beat");
            c.setValue("Algorithm", pluginForm.methodBox.SelectedIndex);
            c.setValue("Sensitivity", pluginForm.sensitivityBar.Value);
            c.setValue("MaxBpmOnOff", pluginForm.maxBPMCheckBox.Checked);
            c.setValue("MaxBPM", pluginForm.maxBPMBar.Value);
            c.setValue("ForecastCount", pluginForm.numberOfBeatsBar.Value);
            c.setValue("DoubleSpeed", pluginForm.doubleCheckBox.Checked);
            c.setValue("HalfSpeed", pluginForm.halfCheckBox.Checked);
            m.AddChild(c);

            c = new ManagedTreeItem("Generator");
            c.setValue("GeneratorOnOff", pluginForm.activateBeatGeneratorCheckbox.Checked);
            c.setValue("GeneratorBPM", Convert.ToInt32(pluginForm.beatGeneratorUpDown.Value));
            c.setValue("Rhythm", pluginForm.rhythmTypeComboBox.SelectedIndex);
            m.AddChild(c);

            log.Info("Saving Data {0} {1}. {2}", "AudioAnalyzer", "End", m.Children.Count);

            return m;
        }

        /// <summary>
        /// clamps to the bar's range. A value outside it makes the setter throw, and that
        /// would abort the rest of the restore.
        /// </summary>
        private static void setBarValue(TrackBar bar, int value)
        {
            bar.Value = Math.Min(bar.Maximum, Math.Max(bar.Minimum, value));
        }

        /// <summary>
        /// reads the audio-Analyzer settings from project
        /// </summary>
        /// <param name="m"></param>
        private void writeSettingsToAnalyzer(ManagedTreeItem m)
        {
            // Kann auch aus connectionEstablished() heraus kommen, das ist nicht zwingend
            // der UI-Thread. Ohne Handle ist InvokeRequired false und der direkte Zugriff
            // ist unbedenklich.
            if (pluginForm.InvokeRequired)
            {
                pluginForm.Invoke(new Action<ManagedTreeItem>(writeSettingsToAnalyzer), m);
                return;
            }

            log.Info("Reading AudioAnalyzer Settings Start: " + m.Name);

            foreach (ManagedTreeItem i in m.GetChildren("Common"))
            {
                if (i.hasValue<int>("Gain"))
                {
                    setBarValue(pluginForm.gainBar, i.getValue<int>("Gain"));
                }
            }

            foreach (ManagedTreeItem i in m.GetChildren("Device"))
            {
                if (i.hasValue<string>("DeviceId"))
                {
                    int channel = i.hasValue<int>("InputChannel") ? i.getValue<int>("InputChannel") : 0;
                    pluginForm.setDevice(i.getValue<string>("DeviceId"), channel);
                }
            }

            foreach (ManagedTreeItem i in m.GetChildren("Level"))
            {
                if (i.hasValue<int>("VUGain"))
                {
                    setBarValue(pluginForm.gainVUBar, i.getValue<int>("VUGain"));
                }
                if (i.hasValue<bool>("PeakHold"))
                {
                    pluginForm.PeakHoldCheckBox.Checked = i.getValue<bool>("PeakHold");
                    //MessageBox.Show("PeakHold: " + i.getValue<bool>("PeakHold"));
                    pluginForm.peakHold = i.getValue<bool>("PeakHold");
                }
                if (i.hasValue<int>("PeakHoldTime"))
                {
                    setBarValue(pluginForm.PeakHoldBar, i.getValue<int>("PeakHoldTime"));
                }
            }

            foreach (ManagedTreeItem i in m.GetChildren("Spectrum"))
            {
                if (i.hasValue<bool>("StereoSpectrum"))
                {
                    pluginForm.stereoSpectrumCheckBox.Checked = i.getValue<bool>("StereoSpectrum");
                    pluginForm.stereoSpectrum = i.getValue<bool>("StereoSpectrum");
                }

                if (i.hasValue<int>("SubBands"))
                {
                    // gespeichert wird der Index der Combobox, nicht die Bandanzahl
                    int bands;
                    switch (i.getValue<int>("SubBands"))
                    {
                        case 0:
                            bands = 8;
                            break;
                        case 1:
                            bands = 16;
                            break;
                        case 3:
                            bands = 64;
                            break;
                        case 4:
                            bands = 96;
                            break;
                        default:
                            bands = 32;
                            break;
                    }

                    pluginForm.setUsedBands(bands);
                }

                if (i.hasValue<int>("SpecGain"))
                {
                    setBarValue(pluginForm.gainSpectrumBar, i.getValue<int>("SpecGain"));
                }

            }

            foreach (ManagedTreeItem i in m.GetChildren("Beat"))
            {
                if (i.hasValue<bool>("MaxBpmOnOff"))
                {
                    pluginForm.maxBPMCheckBox.Checked = i.getValue<bool>("MaxBpmOnOff");
                    //pluginForm.newBPM();
                }

                if (i.hasValue<int>("MaxBPM"))
                {
                    setBarValue(pluginForm.maxBPMBar, i.getValue<int>("MaxBPM"));
                }

                if (i.hasValue<int>("Algorithm"))
                {
                    int alg = i.getValue<int>("Algorithm");
                    if (alg >= 0 && alg < pluginForm.methodBox.Items.Count)
                        pluginForm.methodBox.SelectedIndex = alg;
                }

                if (i.hasValue<int>("Sensitivity"))
                {
                    setBarValue(pluginForm.sensitivityBar, i.getValue<int>("Sensitivity"));
                }

                if (i.hasValue<int>("ForecastCount"))
                {
                    setBarValue(pluginForm.numberOfBeatsBar, i.getValue<int>("ForecastCount"));
                }

                if (i.hasValue<bool>("DoubleSpeed"))
                {
                    pluginForm.doubleCheckBox.Checked = i.getValue<bool>("DoubleSpeed");
                }

                if (i.hasValue<bool>("HalfSpeed"))
                {
                    pluginForm.halfCheckBox.Checked = i.getValue<bool>("HalfSpeed");
                }
            }

            foreach (ManagedTreeItem i in m.GetChildren("Generator"))
            {
                if (i.hasValue<int>("GeneratorBPM"))
                {
                    // auf den Wertebereich des Controls begrenzen, ausserhalb wirft der Setter
                    decimal bpm = i.getValue<int>("GeneratorBPM");
                    if (bpm >= pluginForm.beatGeneratorUpDown.Minimum
                        && bpm <= pluginForm.beatGeneratorUpDown.Maximum)
                    {
                        pluginForm.beatGeneratorUpDown.Value = bpm;
                    }
                }

                if (i.hasValue<int>("Rhythm"))
                {
                    pluginForm.setRhythm(i.getValue<int>("Rhythm"));
                }

                if (i.hasValue<bool>("GeneratorOnOff"))
                {
                    pluginForm.activateBeatGeneratorCheckbox.Checked = i.getValue<bool>("GeneratorOnOff");
                }

            }

            pluginForm.applyControlValues();

            log.Info("Reading AudioAnalyzer Settings End ");

        }
    }
}
