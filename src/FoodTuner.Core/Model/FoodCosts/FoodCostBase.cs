using System;

namespace FoodTuner.Model.Costs
{
   public abstract class FoodCostBase
   {
      public abstract int BaseHealth { get; }

      public abstract int BaseStamina { get; }

      public abstract int BaseRegen { get; }

      public abstract int BaseEndurance { get; }

      public virtual double ComputeHealthCost(FoodItem foodItem)
      {
         var adjustedHealth = foodItem.Health - BaseHealth;

         if (adjustedHealth < 0)
         {
            return 0.0;
         }

         return adjustedHealth / FoodCostsConstants.HealthCostIncrement * FoodCostsConstants.HealthFlatCost;
      }

      public virtual double ComputeStaminaCost(FoodItem foodItem)
      {
         var adjustedStamina = foodItem.Stamina - BaseStamina;

         if (adjustedStamina < 0)
         {
            return 0.0;
         }

         return adjustedStamina / FoodCostsConstants.StaminaCostIncrement * FoodCostsConstants.StaminaFlatCost;
      }

      public virtual double ComputeRegenCost(FoodItem foodItem)
      {
         var adjustedRegen = foodItem.Regen - BaseRegen;

         if (adjustedRegen <= 0)
         {
            return 0.0;
         }
         return Math.Pow(adjustedRegen / 2.0, 2.0);
      }

      public virtual double ComputeEnduranceCost(FoodItem foodItem)
      {
         var adjustedEndurance = foodItem.Endurance - BaseEndurance;
         if (adjustedEndurance <= 0)
         {
            return 0.0;
         }
         return Math.Pow(adjustedEndurance / 8.0, 2.5);
      }

      public virtual double DetermineCost(FoodItem foodItem)
      {
         return ComputeHealthCost(foodItem) +
                ComputeStaminaCost(foodItem) +
                ComputeRegenCost(foodItem) +
                ComputeEnduranceCost(foodItem);
      }
   }
}