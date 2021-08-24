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
            [Biomes.BlackForest] = 1.25,
            [Biomes.Swamp] = 1.5,
            [Biomes.Ocean] = 1.7,
            [Biomes.Mountain] = 1.9,
            [Biomes.Plains] = 2.3,
            [Biomes.Mistlands] = 2.7,
            [Biomes.Ashlands] = 3.1,
            [Biomes.DeepNorth] = 3.5
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
            [FoodComplexity.VeryCommonMeal] = 5.5,
            [FoodComplexity.CommonMeal] = 8.0,
            [FoodComplexity.UncommonMeal] = 10.5,
            [FoodComplexity.RareMeal] = 14.0,
            [FoodComplexity.EpicMeal] = 17.5,
        };

        private static Dictionary<Duration, double> durationMultiplier = new Dictionary<Duration, double>()
        {
            [Duration.Snack] = 1.15,
            [Duration.VeryShort] = 1.1,
            [Duration.Short] = 1.05,
            [Duration.Average] = 1.0,
            [Duration.Long] = 0.925,
            [Duration.VeryLong] = 0.85,
            [Duration.ExtremelyLong] = 0.775
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
                >= FoodComplexity.VeryCommonMeal and
                <= FoodComplexity.EpicMeal => budgetBase * budgetMultiplier + budgetBonus,
                _ => budgetBase + budgetBonus,
            };
        }
    }
}
