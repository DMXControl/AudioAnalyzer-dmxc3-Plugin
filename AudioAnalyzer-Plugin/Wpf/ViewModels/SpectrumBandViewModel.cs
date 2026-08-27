using LumosControlsWPF.Base;

namespace AudioAnalyzer.Wpf.ViewModels
{
    /// <summary>
    /// One bar of the spectrum display. Number and Range are what the matching input
    /// source in DMXControl is called, so the display and the assignment list line up.
    /// </summary>
    public class SpectrumBandViewModel : BasePropertyNotification
    {
        private double level;
        private string range;

        public SpectrumBandViewModel(int number)
        {
            Number = number;
        }

        public int Number { get; }

        /// <summary>0..1, exactly the value that goes to the input source</summary>
        public double Level
        {
            get { return level; }
            set
            {
                if (level == value)
                    return;

                level = value;
                InvokePropertyChanged();
            }
        }

        /// <summary>e.g. "320-453 Hz", null while unknown</summary>
        public string Range
        {
            get { return range; }
            set { SetProperty(ref range, value); }
        }

        protected override void OnDispose()
        {
        }
    }
}
