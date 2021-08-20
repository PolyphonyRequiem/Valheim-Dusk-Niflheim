using System;
using System.Collections.Generic;
using System.Linq;

namespace FoodTuner.Model.Budgeting
{
    public static class FoodBudgeter
    {
        private static Dictionary<Biomes, double> biomeMultiplier = new Dictionary<Biomes, double>() 
        {
            [Biomes.Meadows] = 1.0,
            [Biomes.BlackForest] = 1.20,
            [Biomes.Swamp] = 1.4,
            [Biomes.Ocean] = 1.5,
            [Biomes.Mountain] = 1.6,
            [Biomes.Plains] = 1.85,
            [Biomes.Mistlands] = 2.1,
            [Biomes.Ashlands] = 2.35,
            [Biomes.DeepNorth] = 2.65
        };

        private static Dictionary<Biomes, double> biomeBonuses = new Dictionary<Biomes, double>()
        {
            [Biomes.Meadows] = 0.5,
            [Biomes.BlackForest] = 1.5,
            [Biomes.Swamp] = 2.5,
            [Biomes.Ocean] = 3.0,
            [Biomes.Mountain] = 3.5,
            [Biomes.Plains] = 4.5,
            [Biomes.Mistlands] = 5.5,
            [Biomes.Ashlands] = 6.5,
            [Biomes.DeepNorth] = 7.5
        };

        private static Dictionary<FoodComplexity, double> complexityBasis = new Dictionary<FoodComplexity, double>()
        {
            [FoodComplexity.Foragable] = 4.5,
            [FoodComplexity.SimpleIngredient] = 1.5,
            [FoodComplexity.CommonIngredient] = 2.5,
            [FoodComplexity.ComplexIngredient] = 3.5,
            [FoodComplexity.PreparedSingleIngredientFoodItem] = 5.5,
            [FoodComplexity.PreparedSimpleMeal] = 8.0,
            [FoodComplexity.PreparedCommonMeal] = 10.5,
            [FoodComplexity.PreparedComplexMeal] = 14.0,
            [FoodComplexity.PreparedLegendaryMeal] = 17.5,
        };

        private static Dictionary<Duration, double> durationMultiplier = new Dictionary<Duration, double>()
        {
            [Duration.Snack] = 1.3,
            [Duration.VeryShort] = 1.2,
            [Duration.Short] = 1.1,
            [Duration.Average] = 1.0,
            [Duration.Long] = 0.9,
            [Duration.VeryLong] = 0.75,
            [Duration.ExtremelyLong] = 0.6
        };

        public static double GetBudget(FoodItem foodItem, bool useBudgetBoostBonus)
        {
            var duration = Enum.GetValues<Duration>().ToList().Cast<int>().Contains(foodItem.Duration) ?
                                            (Duration)foodItem.Duration :
                                            Duration.Average;

            var budgetBase = complexityBasis[foodItem.Complexity];
            var budgetMultiplier = biomeMultiplier[foodItem.Biome] * durationMultiplier[duration];
            var budgetBonus = useBudgetBoostBonus? biomeBonuses[foodItem.Biome] : 0.0;

            return foodItem.Complexity switch
            {
                FoodComplexity.Foragable or
                >= FoodComplexity.PreparedSingleIngredientFoodItem and
                <= FoodComplexity.PreparedLegendaryMeal => budgetBase * budgetMultiplier + budgetBonus,
                _ => budgetBase + budgetBonus,
            };
        }
    }
}
