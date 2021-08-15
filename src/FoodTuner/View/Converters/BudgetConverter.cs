using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace FoodTuner.View.Converters
{
    public class BudgetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value switch
            {
                > .05 => Brushes.YellowGreen,
                <= .05 and >= -0.05 => Brushes.Green,
                < -0.05 => Brushes.LightCoral,
                _ => throw new NotImplementedException()
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("Can't convert back.");
        }
    }
}
