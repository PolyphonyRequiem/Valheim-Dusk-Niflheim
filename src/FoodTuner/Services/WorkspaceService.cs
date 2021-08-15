using FoodTuner.Core.FileHandlers;
using FoodTuner.FileHandlers;
using FoodTuner.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FoodTuner.Services
{
    public class WorkspaceService
    {
        private readonly FoodRebalanceFileHandler foodRebalanceFileHandler;
        private readonly EnduranceFileHandler enduranceFileHandler;

        public WorkspaceService(FoodRebalanceFileHandler foodRebalanceFileHandler, EnduranceFileHandler enduranceFileHandler)
        {
            this.foodRebalanceFileHandler = foodRebalanceFileHandler;
            this.enduranceFileHandler = enduranceFileHandler;
        }

        public IEnumerable<FoodRebalanceRecordInfo> LoadFoodItemRecords()
        {
            var results = this.foodRebalanceFileHandler.ReadPartiallyConfiguredFoodItems().ToArray();

            foreach (var food in results)
            {
                food.FoodItem.Endurance = this.enduranceFileHandler.GetEnduranceForFoodItem(food.FoodItem);
            }
            return results;
        }

        public void SaveFoodItem(FoodItem foodItem, FileInfo file)
        {
            this.foodRebalanceFileHandler.SaveFoodRebalanceFile(file, foodItem);
            this.enduranceFileHandler.UpdateFoodItemEndurance(foodItem);
        }
    }
}
