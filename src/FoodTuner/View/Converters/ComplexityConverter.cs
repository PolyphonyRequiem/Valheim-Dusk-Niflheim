using FoodTuner.Model;
using System;
using System.Globalization;
using System.Windows.Data;

namespace FoodTuner.View.Converters
{
    public class ComplexityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (FoodComplexity)value switch
            {
                FoodComplexity.Foragable => "Foragable",
                FoodComplexity.SimpleIngredient => "Ingredient - Simple",
                FoodComplexity.CommonIngredient => "Ingredient - Common",
                FoodComplexity.ComplexIngredient => "Ingredient - Complex",
                FoodComplexity.VeryCommonMeal => "Meal - Very Common",
                FoodComplexity.CommonMeal => "Meal - Common",
                FoodComplexity.UncommonMeal => "Meal - Uncommon",
                FoodComplexity.RareMeal => "Meal - Rare",
                FoodComplexity.EpicMeal => "Meal - Epic",
                _ => "Unknown Complexity"
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (string)value switch
            {
                "Foragable" => FoodComplexity.Foragable,
                "Ingredient - Simple" => FoodComplexity.SimpleIngredient,
                "Ingredient - Common" => FoodComplexity.CommonIngredient,
                "Ingredient - Complex" => FoodComplexity.ComplexIngredient,
                "Meal - Very Common" => FoodComplexity.VeryCommonMeal,
                "Meal - Common" => FoodComplexity.CommonMeal,
                "Meal - Uncommon" => FoodComplexity.UncommonMeal,
                "Meal - Rare" => FoodComplexity.RareMeal,
                "Meal - Epic" => FoodComplexity.EpicMeal,
                _ => FoodComplexity.Foragable
            };
        }
    }
}
