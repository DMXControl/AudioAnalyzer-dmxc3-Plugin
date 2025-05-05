using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;


namespace AudioAnalyzer
{
	public partial class audioAnalysForm
	{

		internal void newBPM()
		{
            if (MaxBPM >= 30 && MaxBPM <=300)
    		    maxBPMBar.Value = (int)MaxBPM;

            if (maxBpmOn)
			{
				maxBPMCheckBox.Text = "max. BPM = " + MaxBPM;
			}
			else
			{
				maxBPMCheckBox.Text = "max BPM = no limit";
			}
            if (beatclearTimer != null)
            {
                beatclearTimer.Interval = (int)(60000 / MaxBPM);
            }
		}


		/// <summary>
		/// Method1
		/// detects the beat from actual value end averages, with Maiks method (from Google)
		/// </summary>
		bool Beat1()
		{
			double n;
			int i;

            bassvalue = 0;
            int lbound = FFTFrequency2Index(50);
            int ubound = FFTFrequency2Index(200);
            for (i = lbound; i <= ubound; i++)
            {
                highvalue += fft[i];
            }
            bassvalue = bassvalue / (ubound - lbound);	// 50 Hz - 200 Hz

            highvalue = 0;
            lbound = FFTFrequency2Index(5000);
            ubound = FFTFrequency2Index(8000);
            for (i = lbound; i <= ubound; i++)
            {
                highvalue += fft[i];
            }
            highvalue = highvalue / (ubound - lbound);	// 5 KHz - 8 KHz

			n = (bassvalue + highvalue * correction) / 2 * faktor * regler2;

			b = Math.Pow(n, sensitivity);		// actual Value

			// ehemals readings() - berechnet den Durchschnitt
			total1 = total1 - readings1[0];
			for (int z = 0; z < numReadings1 - 1; z++)
			{
				readings1[z] = readings1[z + 1];
			}
			readings1[199] = b;
			total1 = total1 + readings1[199];
			avg1 = total1 / numReadings1 * 2 * regler1;

			if (avg1 < minVol)
			{
				avg1 = minVol;
			}

			total2 = total2 - readings2[0];
			for (int z = 0; z < numReadings2 - 1; z++)
			{
				readings2[z] = readings2[z + 1];
			}
			readings2[49] = b;
			total2 = total2 + readings2[49];
			avg2 = total2 / numReadings2 * 2 * regler1;

			if (avg2 < minVol)
			{
				avg2 = minVol;
			}


			//Console.WriteLine("b: " + b + " avg22: " + avg22 + " avg2: " + avg2);

			if (((b * regler1) > (avg2)) && ((b * regler1) > (avg1)) && (beat_detected == false))
			{
				beat_detected = true;
                return true;
				//Mydelay = mytime;
			}
			else
			{
				beat_detected = false;
                return false;
			}
		}

		bool Beat3()
		{

			// aktuelle sound-energy berechnen
			energy = 0;
			for (int i = 0; i < fft.Length; i++)
			{
				energy = energy + fft[i] * fft[i] * regler2;
			}

			// untere Grenze setzen
			if (energy < minenergy)
				energy = minenergy;

			// Durchschnitt berechnen
            //average = 0;
            //for (int i = 0; i < energybuffer.Length; i++)
            //{
            //    average = average + energybuffer[i];
            //}
			average = energybuffer.Average();

			// Berechnen der Konstanten C
			variance = 0;
			for (int i = 0; i < energybuffer.Length; i++)
			{
				variance = (energybuffer[i] - average) * (energybuffer[i] - average) + variance;
			}
			variance = variance / energybuffer.Length;
            
			constant = (-0.0025714 * variance) + 1.5142857;

			// Shift Feld
			for (int i = energybuffer.Length - 1; i > 0; i--)
			{
				energybuffer[i] = energybuffer[i - 1];
			}
			energybuffer[0] = energy;
            
			// Entscheidung
			if ((energy > constant * average) && (beat_detected == false))
				return true;
			else
				return false;

		}

		bool Beat4()
        {

            // Shift Feld
            for (int j = 0; j < maxbands; j++)
            {
                for (int i = historylength - 1; i > 0; i--)
                {
                    subenergybuffer[i, j] = subenergybuffer[i - 1, j];
                }
                subenergybuffer[0, j] = subenergy[j];
            }

            // Durchschnitt berechnen
            for (int j = 0; j < maxbands; j++)
            {
                subaverage[j] = 0;
                for (int i = 0; i < historylength; i++)
                {
                    subaverage[j] = subaverage[j] + subenergybuffer[i, j];
                }
                subaverage[j] = subaverage[j] / historylength;

                // Berechnen der Konstanten C
                subvariance[j] = 0;
                for (int i = 0; i < historylength; i++)
                {
                    subvariance[j] = (subenergybuffer[i, j] - subaverage[j]) * (subenergybuffer[i, j] - subaverage[j]) + subvariance[j];
                }
                subvariance[j] = subvariance[j] / historylength;

                //subconstant[j] = (-0.0025714 * subvariance[j]) + 1.5142857;
                subconstant[j] = 7 - Math.Sqrt(subvariance[j]) / subaverage[j];

            }

            // Entscheidung
            // Auswertung aller Bänder
            for (int j = 0; j < maxbands; j++)
            {
                if ((subenergy[j] > subconstant[j] * subaverage[j]) && (beat_detected == false))
                {
                    return true;
                }
            }
            return false;
        }

	}
}