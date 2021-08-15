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
                FoodComplexity.PreparedSingleIngredientFoodItem => "Food",
                FoodComplexity.PreparedSimpleMeal => "Meal - Simple",
                FoodComplexity.PreparedCommonMeal => "Meal - Common",
                FoodComplexity.PreparedComplexMeal => "Meal - Complex",
                FoodComplexity.PreparedLegendaryMeal => "Meal - Legendary",
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
                "Food" => FoodComplexity.PreparedSingleIngredientFoodItem,
                "Meal - Simple" => FoodComplexity.PreparedSimpleMeal,
                "Meal - Common" => FoodComplexity.PreparedCommonMeal,
                "Meal - Complex" => FoodComplexity.PreparedComplexMeal,
                "Meal - Legendary" => FoodComplexity.PreparedLegendaryMeal,
                _ => FoodComplexity.Foragable
            };
        }
    }
}
