using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioAnalyzer.AAEventArgs;
using LumosLIB.Kernel;
using LumosLIB.Tools;
using LumosProtobuf;
using LumosProtobuf.Input;
using org.dmxc.lumos.Kernel.Input.v2;

namespace AudioAnalyzer.Input
{
    class AASpectrumSource : AbstractAudioAnalyzerInputSource
    {
        // Die IDs dürfen sich nicht ändern, sonst verlieren bestehende Projekte ihre
        // Zuweisungen. Der vorhandene Satz trägt im Stereo-Modus den linken Kanal und
        // benennt sich dann nur um - deshalb gibt es kein eigenes Präfix für links.
        private const string MonoPrefix = "{6B78ABC0-D9E4-4c02-997E-023417092587}";
        private const string RightPrefix = "{B0FFCB22-B7F1-45E7-B0C9-46C6E8C79DC1}";

        private const string LeftName = "Spectrum L";

        private bool stereoNaming;
        private string bandRange;
        private ParameterCategory leftCategory;

        /// <summary>
        /// channel is either Mono (the existing set, carrying the downmix or, in stereo
        /// mode, the left channel) or Right (only registered while stereo mode is active)
        /// </summary>
        public AASpectrumSource(int number, ESpectrumChannel channel, bool stereo)
            : base(idPrefix(channel) + "-" + number, displayName(channel) + " " + number, minimum,
                   ParameterCategoryTools.FromName(displayName(channel)))
        {
            Number = number;
            Channel = channel;
            stereoNaming = stereo && channel == ESpectrumChannel.Mono;
        }

        public int Number { get; }

        public ESpectrumChannel Channel { get; }

        public void SetLevel(double level)
        {
            this.CurrentValue = level.Limit(0, 1);
        }

        /// <summary>
        /// switches label and folder of the existing set between downmix and left channel.
        /// The ID stays the same, so assignments survive the change.
        /// </summary>
        public void SetStereoNaming(bool stereo)
        {
            if (Channel != ESpectrumChannel.Mono || stereoNaming == stereo)
                return;

            stereoNaming = stereo;
            // null = "alle Eigenschaften", der InputSourceProxy überträgt die Quelle neu
            OnPropertyChanged(new PropertyChangedEventArgs(null));
        }

        /// <summary>
        /// sets the frequency range shown in the label, e.g. "80-113 Hz". Only the display
        /// name changes - the ID stays the same, so assignments are unaffected.
        /// </summary>
        public void SetBandRange(string range)
        {
            if (String.Equals(bandRange, range))
                return;

            bandRange = range;
            OnPropertyChanged(new PropertyChangedEventArgs(null));
        }

        protected override string DisplayNameHook()
        {
            string name = (stereoNaming ? LeftName : displayName(Channel)) + " " + Number;
            if (!String.IsNullOrEmpty(bandRange))
                name += " (" + bandRange + ")";
            return name;
        }

        protected override ParameterCategory CategoryHook()
        {
            if (!stereoNaming)
                return null;

            if (leftCategory == null)
            {
                leftCategory = ParameterCategoryTools.FromName("Audio Analyzer")
                    .Combine(ParameterCategoryTools.FromName(LeftName), false);
            }
            return leftCategory;
        }

        private static string idPrefix(ESpectrumChannel channel)
        {
            return channel == ESpectrumChannel.Right ? RightPrefix : MonoPrefix;
        }

        private static string displayName(ESpectrumChannel channel)
        {
            return channel == ESpectrumChannel.Right ? "Spectrum R" : "Spectrum";
        }

        private static object minimum => 0.0;

        public override EWellKnownInputType AutoGraphIOType => EWellKnownInputType.Numeric;
        public override object Min => minimum;
        public override object Max => 1.0;
    }
}
