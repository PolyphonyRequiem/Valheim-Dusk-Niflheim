using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodTuner.Model.Budgeting
{
    public static class FoodBudgeter
    {
        private static Dictionary<Biomes, double> biomeMultiplier = new Dictionary<Biomes, double>() 
        {
            [Biomes.Meadows] = 1.0,
            [Biomes.BlackForest] = 1.15,
            [Biomes.Swamp] = 1.35,
            [Biomes.Ocean] = 1.5,
            [Biomes.Mountain] = 1.6,
            [Biomes.Plains] = 1.9,
            [Biomes.Mistlands] = 2.25,
            [Biomes.Ashlands] = 2.5,
            [Biomes.DeepNorth] = 2.75
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
            [FoodComplexity.Foragable] = 5.5,
            [FoodComplexity.SimpleIngredient] = 1.5,
            [FoodComplexity.CommonIngredient] = 3.5,
            [FoodComplexity.ComplexIngredient] = 5.5,
            [FoodComplexity.PreparedSingleIngredientFoodItem] = 8.0,
            [FoodComplexity.PreparedSimpleMeal] = 11.5,
            [FoodComplexity.PreparedCommonMeal] = 15.0,
            [FoodComplexity.PreparedComplexMeal] = 18.0,
            [FoodComplexity.PreparedLegendaryMeal] = 21.0,
        };

        public static double GetBudget(FoodItem foodItem, bool useBudgetBoostBonus)
        {
            var budgetBase = complexityBasis[foodItem.Complexity];
            var budgetMultiplier = biomeMultiplier[foodItem.Biome];
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
