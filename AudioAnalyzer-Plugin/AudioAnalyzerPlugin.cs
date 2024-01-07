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

        public override void loadProject(LumosGUIIOContext context)
        {
            base.loadProject(context);

            if (Lumos.GUI.Resource.ResourceManager.getInstance().ExistsResource(EResourceType.Project, AnalyzerSettings))
            {
                LumosResource r = Lumos.GUI.Resource.ResourceManager.getInstance().TryLoadResource(EResourceType.Project, AnalyzerSettings);
                this.writeSettingsToAnalyzer(r.ManagedData);
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
            int gain = (int)pluginForm.gainBar.Invoke(new Func<int>(() => pluginForm.gainBar.Value));
            c.setValue("Gain", gain);
            m.AddChild(c);

            c = new ManagedTreeItem("Level");
            int vu = (int)pluginForm.gainVUBar.Invoke(new Func<int>(() => pluginForm.gainVUBar.Value));
            c.setValue("VUGain", vu);
            m.AddChild(c);
            bool peakHold = (bool)pluginForm.PeakHoldCheckBox.Invoke(new Func<bool>(() => pluginForm.PeakHoldCheckBox.Checked));
            c.setValue("PeakHold", peakHold);
            m.AddChild(c);
            int hold = (int)pluginForm.PeakHoldBar.Invoke(new Func<int>(() => pluginForm.PeakHoldBar.Value));
            c.setValue("PeakHoldTime", hold);
            m.AddChild(c);

            c = new ManagedTreeItem("Spectrum");
            int spec = (int)pluginForm.subBandBox.Invoke(new Func<int>(() => pluginForm.subBandBox.SelectedIndex));
            c.setValue("SubBands", spec);
            int svu = (int)pluginForm.gainSpectrumBar.Invoke(new Func<int>(() => pluginForm.gainSpectrumBar.Value));
            c.setValue("SpecGain", svu);
            m.AddChild(c);

            c = new ManagedTreeItem("Beat");
            int alg = (int)pluginForm.methodBox.Invoke(new Func<int>(() => pluginForm.methodBox.SelectedIndex));
            c.setValue("Algorithm", alg);
            int sens = (int)pluginForm.sensitivityBar.Invoke(new Func<int>(() => pluginForm.sensitivityBar.Value));
            c.setValue("Sensitivity", sens);
            bool max = (bool) pluginForm.maxBPMCheckBox.Invoke(new Func<bool>(() => pluginForm.maxBPMCheckBox.Checked));
            c.setValue("MaxBpmOnOff", pluginForm.maxBPMCheckBox.Checked);
            int maxbpm = (int)pluginForm.maxBPMBar.Invoke(new Func<int>(() => pluginForm.maxBPMBar.Value));
            c.setValue("MaxBPM", maxbpm);
            int forecast = (int)pluginForm.numberOfBeatsBar.Invoke(new Func<int>(() => pluginForm.numberOfBeatsBar.Value));
            log.Info("Writing ForecastCount: " + forecast.ToString());
            c.setValue("ForecastCount", forecast);
            bool doub = (bool)pluginForm.doubleCheckBox.Invoke(new Func<bool>(() => pluginForm.doubleCheckBox.Checked));
            c.setValue("DoubleSpeed", doub);
            bool half = (bool)pluginForm.halfCheckBox.Invoke(new Func<bool>(() => pluginForm.halfCheckBox.Checked));
            c.setValue("HalfSpeed", pluginForm.halfCheckBox.Checked);
            m.AddChild(c);

            c = new ManagedTreeItem("Generator");
            bool gen = (bool)pluginForm.activateBeatGeneratorCheckbox.Invoke(new Func<bool>(() => pluginForm.activateBeatGeneratorCheckbox.Checked));
            c.setValue("GeneratorOnOff", gen);

            int gbud = (int)pluginForm.beatGeneratorUpDown.Invoke(new Func<int>(() => Convert.ToInt32(pluginForm.beatGeneratorUpDown.Value)));
            c.setValue("GeneratorBPM", gbud);

            int rhythm = (int)pluginForm.rhythmTypeComboBox.Invoke(new Func<int>(() => pluginForm.rhythmTypeComboBox.SelectedIndex));
            c.setValue("Rhythm", rhythm);
            m.AddChild(c);

            log.Info("Saving Data {0} {1}. {2}", "AudioAnalyzer", "End", m.Children.Count);

            return m;
        }

        /// <summary>
        /// reads the audio-Analyzer settings from project
        /// </summary>
        /// <param name="m"></param>
        private void writeSettingsToAnalyzer(ManagedTreeItem m)
        {
            log.Info("Reading AudioAnalyzer Settings Start: " + m.Name);

            foreach (ManagedTreeItem i in m.GetChildren("Common"))
            {
                if (i.hasValue<int>("Gain"))
                {
                    int gain = i.getValue<int>("Gain");
                    pluginForm.gainBar.Value = gain;
                }
            }

            foreach (ManagedTreeItem i in m.GetChildren("Level"))
            {
                if (i.hasValue<int>("VUGain"))
                {
                    pluginForm.gainVUBar.Value = i.getValue<int>("VUGain");
                }
                if (i.hasValue<bool>("PeakHold"))
                {
                    pluginForm.PeakHoldCheckBox.Checked = i.getValue<bool>("PeakHold");
                    //MessageBox.Show("PeakHold: " + i.getValue<bool>("PeakHold"));
                    pluginForm.peakHold = i.getValue<bool>("PeakHold");
                }
                if (i.hasValue<int>("PeakHoldTime"))
                {
                    pluginForm.PeakHoldBar.Value = i.getValue<int>("PeakHoldTime");
                }
            }

            foreach (ManagedTreeItem i in m.GetChildren("Spectrum"))
            {
                if (i.hasValue<int>("SubBands"))
                {
                    //MessageBox.Show("1");
                    switch (i.getValue<int>("SubBands"))
                    {
                        case 0:
                            pluginForm.usedbands = 8;
                            break;
                        case 1:
                            pluginForm.usedbands = 16;
                            break;
                        case 2:
                            pluginForm.usedbands = 32;
                            break;
                        case 3:
                            pluginForm.usedbands = 64;
                            break;
                        case 4:
                            pluginForm.usedbands = 96;
                            break;
                        default:
                            pluginForm.usedbands = 8;
                            break;
                    }
                    //pluginForm.usedbands = i.getValue<int>("SubBands");
                    //pluginForm.subBandBox.SelectedIndex = i.getValue<int>("SubBands");
                    //MessageBox.Show("2");
                    //pluginForm.configSubBands();
                    //MessageBox.Show("3");

                    pluginForm.OnSendSpectrumCount(pluginForm.usedbands);
                }

                if (i.hasValue<int>("SpecGain"))
                {
                    pluginForm.gainSpectrumBar.Value = i.getValue<int>("SpecGain");
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
                    //MessageBox.Show(i.getValue<int>("MaxBPM").ToString());
                    pluginForm.maxBPMBar.Value = i.getValue<int>("MaxBPM");
                }

                if (i.hasValue<int>("Algorithm"))
                {
                    pluginForm.methodBox.SelectedIndex = i.getValue<int>("Algorithm");
                }

                if (i.hasValue<int>("Sensitivity"))
                {
                    pluginForm.sensitivityBar.Value = i.getValue<int>("Sensitivity");
                }

                if (i.hasValue<int>("ForecastCount"))
                {
                    pluginForm.numberOfBeatsBar.Value = i.getValue<int>("ForecastCount");
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
                    int updownval = i.getValue<int>("GeneratorBPM");
                    //pluginForm.beatGeneratorUpDown.Value = i.getValue<decimal>("GeneratorBPM");
                    //pluginForm.beatGeneratorUpDown.Value = Convert.ToDecimal(updownval);
                    //pluginForm.beatGeneratorUpDown.Value = (decimal)updownval;
                }

                if (i.hasValue<int>("Rhythm"))
                {
                    //MessageBox.Show(i.getValue<int>("Rhythm").ToString());
                    pluginForm.generatorRhythm = (audioAnalysForm.RhythmType) i.getValue<int>("Rhythm");
                    //pluginForm.rhythmTypeComboBox.SelectedIndex = i.getValue<int>("Rhythm");
                }

                if (i.hasValue<bool>("GeneratorOnOff"))
                {
                    pluginForm.activateBeatGeneratorCheckbox.Checked = i.getValue<bool>("GeneratorOnOff");
                }

            }

            log.Info("Reading AudioAnalyzer Settings End ");

        }
    }
}
