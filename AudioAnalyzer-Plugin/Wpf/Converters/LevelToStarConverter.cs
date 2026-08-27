using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AudioAnalyzer.Wpf.Converters
{
    /// <summary>
    /// Turns a 0..1 level into a proportional GridLength, so a bar can be built from two
    /// star-sized rows: the filled part and the empty part above it.
    ///
    /// Deliberately not "level * ActualHeight": that made the bar's size depend on its
    /// container's measured size while the container sized itself to its content, which is
    /// a layout feedback loop - at full level the bars grew without bound. Proportions carry
    /// no measured size, so the loop cannot form.
    ///
    /// Pass "invert" as the converter parameter for the empty part.
    /// </summary>
    public class LevelToStarConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double level = value is double ? (double)value : 0.0;

            if (double.IsNaN(level) || level < 0)
                level = 0;
            else if (level > 1)
                level = 1;

            bool invert = String.Equals(parameter as string, "invert", StringComparison.OrdinalIgnoreCase);

            return new GridLength(invert ? 1.0 - level : level, GridUnitType.Star);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
