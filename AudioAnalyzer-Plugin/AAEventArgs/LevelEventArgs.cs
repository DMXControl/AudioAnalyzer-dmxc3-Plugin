using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioAnalyzer.AAEventArgs
{
    class LevelEventArgs : EventArgs
    {
        public readonly float VolumeL, VolumeR;

        public LevelEventArgs(float volL, float volR)
        {
            this.VolumeL = volL;
            this.VolumeR = volR;
        }
    }
}
