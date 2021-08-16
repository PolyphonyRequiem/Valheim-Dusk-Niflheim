using FoodTuner.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FoodTuner.View.Converters
{
    public class DurationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Duration)value switch
            {
                Duration.Snack => "Snack",
                Duration.VeryShort => "Very Short",
                Duration.Short => "Short",
                Duration.Average => "Average",
                Duration.Long => "Long",
                Duration.VeryLong => "Very Long",
                Duration.ExtremelyLong => "Extremely Long",
                _ => "Average"
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((string)value) switch
            {
                "Snack" => Duration.Snack,
                "Very Short" => Duration.VeryShort,
                "Short" => Duration.Short,
                "Average" => Duration.Average,
                "Long" => Duration.Long,
                "Very Long" => Duration.VeryLong,
                "Extremely Long" => Duration.ExtremelyLong,
                _ => Duration.Average
            };
        }
    }
}
