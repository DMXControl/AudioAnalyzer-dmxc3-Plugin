using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace AudioAnalyzer.Wpf.Views
{
    /// <summary>
    /// Draws one channel of the spectrum as bars. Kept separate so the stereo mode can
    /// simply show two of them.
    /// </summary>
    public partial class SpectrumDisplayControl : UserControl
    {
        public static readonly DependencyProperty BandsProperty =
            DependencyProperty.Register(
                "Bands",
                typeof(IEnumerable),
                typeof(SpectrumDisplayControl),
                new PropertyMetadata(null));

        public SpectrumDisplayControl()
        {
            InitializeComponent();
        }

        public IEnumerable Bands
        {
            get { return (IEnumerable)GetValue(BandsProperty); }
            set { SetValue(BandsProperty, value); }
        }
    }
}
