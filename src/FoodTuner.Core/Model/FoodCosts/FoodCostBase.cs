using System;

namespace FoodTuner.Model.Costs
{
    public abstract class FoodCostBase
    {
        public abstract int BaseHealth { get; }

        public abstract int BaseStamina { get; }

        public abstract int BaseRegen { get; }

        public abstract int BaseDuration { get; }

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

            return adjustedRegen switch
            {
                <= 0 =>           0.0,
                > 0 and <= 2 =>   (Math.Min(adjustedRegen, 2)) * 0.5,
                > 2 and <= 4 =>   (Math.Min(adjustedRegen, 2)) * 0.5 +
                                  (Math.Min(adjustedRegen - 2, 2)) * 1.0,
                > 4 =>            (Math.Min(adjustedRegen, 2)) * 0.5 +
                                  (Math.Min(adjustedRegen - 2, 2)) * 1.0 +
                                  (Math.Min(adjustedRegen - 2, 2)) * 2.0
            };
        }

        public virtual double ComputeDurationCost(FoodItem foodItem)
        {
            var adjustedDuration = foodItem.Duration - BaseDuration;

            if (adjustedDuration < 0)
            {
                return 0.0;
            }

            if (adjustedDuration % FoodCostsConstants.DurationCostIncrement != 0)
            {
                adjustedDuration = adjustedDuration - (adjustedDuration % FoodCostsConstants.DurationCostIncrement) + FoodCostsConstants.DurationCostIncrement;
            }

            return adjustedDuration / FoodCostsConstants.DurationCostIncrement * FoodCostsConstants.DurationFlatCost;
                
        }

        public virtual double ComputeEnduranceCost(FoodItem foodItem)
        {

            var adjustedEndurance = foodItem.Endurance - BaseEndurance;

            return adjustedEndurance switch
            {
                <= 0 =>           0.0,
                > 0 and <= 6 =>   (Math.Min(adjustedEndurance, 6)) * 0.1,
                > 6 and <= 10 =>  (Math.Min(adjustedEndurance, 6)) * 0.1 +
                                  (Math.Min(adjustedEndurance - 6, 4)) * 0.3,
                > 10 and <= 12 => (Math.Min(adjustedEndurance, 6)) * 0.1 +
                                  (Math.Min(adjustedEndurance - 6, 4)) * 0.3 +
                                  (Math.Min(adjustedEndurance - 10, 2)) * 0.5,
                > 12 =>           (Math.Min(adjustedEndurance, 6)) * 0.1 +
                                  (Math.Min(adjustedEndurance - 6, 4)) * 0.3 +
                                  (Math.Min(adjustedEndurance - 10, 2)) * 0.5 +
                                  (adjustedEndurance - 12) * 1.0
            };
        }

        public virtual double DetermineCost(FoodItem foodItem)
        {
            return ComputeHealthCost(foodItem) +
                   ComputeStaminaCost(foodItem) +
                   ComputeRegenCost(foodItem) +
                   ComputeDurationCost(foodItem) +
                   ComputeEnduranceCost(foodItem);
        }
    }
}