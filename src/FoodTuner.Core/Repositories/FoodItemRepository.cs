using FoodTuner.FileHandlers;
using FoodTuner.Model;

namespace FoodTuner.Repositories
{
    public class FoodItemRepository
    {
        private readonly FoodRebalanceFileHandler foodRebalanceFileHandler;
        private readonly EnduranceFileHandler enduranceFileHandler;

        public FoodItemRepository(FoodRebalanceFileHandler foodRebalanceFileHandler, EnduranceFileHandler enduranceFileHandler)
        {
            this.foodRebalanceFileHandler = foodRebalanceFileHandler;
            this.enduranceFileHandler = enduranceFileHandler;
        }

        public void SaveFood(FoodItem foodItem)
        {

        }
    }
}
