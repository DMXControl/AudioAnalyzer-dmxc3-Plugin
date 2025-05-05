using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using AudioAnalyzer.AAEventArgs;


namespace AudioAnalyzer
{
    public partial class audioAnalysForm
    {
        internal event EventHandler<BeatEventArgs> SendBeat;
        internal event EventHandler<LevelEventArgs> SendLevel;
        internal event EventHandler<SpectrumEventArgs> SendSpectrum;
        internal event EventHandler<SpectrumCountEventArgs> SendSpectrumCount;

        private void OnSendBeat(int majorMinor)
        {
            if (SendBeat != null)
                SendBeat(this, new BeatEventArgs(majorMinor));
        }

        private void OnSendLevel(float volL, float volR)
        {
            if (SendLevel != null)
                SendLevel(this, new LevelEventArgs(volL, volR));
        }

        private void OnSendSpectrum(double[] dbsubLevel)
        {
            if (SendSpectrum != null)
                SendSpectrum(this, new SpectrumEventArgs(dbsubLevel));
        }

        public void OnSendSpectrumCount(int count)
        {
            if(SendSpectrumCount != null)
                SendSpectrumCount(this, new SpectrumCountEventArgs(count));
        }
    }
}