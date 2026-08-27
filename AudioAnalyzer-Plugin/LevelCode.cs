using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;


namespace AudioAnalyzer
{
	public partial class AudioAnalyzerEngine
	{

        void sendLevel()
		{
            float[] outLevel = new float[2];

            // Maxiamlwert der letzten 4 Werte ermitteln (nicht bei PeakHold)
            if (peakHold)
            {
                outLevel[0] = levelhistory[0, 0];
                outLevel[1] = levelhistory[1, 0];
            }
            else
            {
                outLevel[0] = levelhistory[0, 3];
                if (levelhistory[0, 2] > outLevel[0])
                    outLevel[0] = levelhistory[0, 2];
                if (levelhistory[0, 1] > outLevel[0])
                    outLevel[0] = levelhistory[0, 1];
                if (levelhistory[0, 0] > outLevel[0])
                    outLevel[0] = levelhistory[0, 0];

                outLevel[1] = levelhistory[1, 3];
                if (levelhistory[1, 2] > outLevel[1])
                    outLevel[1] = levelhistory[1, 2];
                if (levelhistory[1, 1] > outLevel[1])
                    outLevel[1] = levelhistory[1, 1];
                if (levelhistory[1, 0] > outLevel[1])
                    outLevel[1] = levelhistory[1, 0];
            }

            OnLevelChanged(outLevel[0], outLevel[1]);
		}

	}
}

