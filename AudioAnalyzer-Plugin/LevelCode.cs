using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Un4seen.Bass;
using Un4seen.BassAsio;
using Un4seen.Bass.Misc;
using Un4seen.Bass.AddOn.Tags;


namespace AudioAnalyzer
{
	public partial class audioAnalysForm
	{

        /// <summary>
        /// draws level bars
        /// </summary>
        void drawLevel()
		{
			int volL;
			int volR;
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
                if (levelhistory[0, 0] > outLevel[0])
                    outLevel[1] = levelhistory[1, 0];
            }

            volL = (int)(levelLBox.Width * outLevel[0]);
			volR = (int)(levelRBox.Width * outLevel[1]);

			// Zeichnen des Levels ...

			Graphics g1 = levelLBox.CreateGraphics();
			Graphics g2 = levelRBox.CreateGraphics();
			g1.Clear(Color.Black);
			g2.Clear(Color.Black);

			Brush b = new SolidBrush(levelActive);

			g1.FillRectangle(b, 0, 0, volL, levelLBox.Height);
			g2.FillRectangle(b, 0, 0, volR, levelRBox.Height);

            OnSendLevel(outLevel[0], outLevel[1]);
		}

	}
}

