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
    public class BiomeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Biomes)value switch
            {
                Biomes.Meadows => "Meadows",
                Biomes.BlackForest => "Black Forest",
                Biomes.Swamp => "Swamp",
                Biomes.Mountain => "Mountain",
                Biomes.Plains => "Plains",
                Biomes.Ocean => "Ocean",
                Biomes.Mistlands => "Mistlands",
                Biomes.Ashlands => "Ashlands",
                Biomes.DeepNorth => "Deep North",
                _ => "Meadows",
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (string)value switch
            {
                "Meadows"  => Biomes.Meadows,
                "Black Forest" => Biomes.BlackForest,
                "Swamp" => Biomes.Swamp,
                "Mountain" => Biomes.Mountain,
                "Plains" => Biomes.Plains,
                "Ocean" => Biomes.Ocean,
                "Mistlands" => Biomes.Mistlands,
                "Ashlands" => Biomes.Ashlands,
                "Deep North" => Biomes.DeepNorth ,
                _ => Biomes.Meadows,
            };
        }
    }
}
