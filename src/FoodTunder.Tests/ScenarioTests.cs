using FoodTuner.FileHandlers;
using FoodTuner.Services;
using FoodTuner.ViewModels;
using System.Linq;
using Xunit;

namespace FoodTunder.Tests
{
    public class ScenarioTests
    {
        [Fact]
        public void TestFoodRebalanceLoadOperations()
        {
            var foodRebalFile = new FoodRebalanceFileHandler(new System.IO.DirectoryInfo(@".\FoodRebalance\Items\"));
            var enduranceFile = new EnduranceFileHandler(new System.IO.FileInfo(@".\config\org.bepinex.plugins.foodstaminaregen.cfg"));

            var results = foodRebalFile.ReadPartiallyConfiguredFoodItems();

            foreach (var food in results)
            {
                food.FoodItem.Endurance = enduranceFile.GetEnduranceForFoodItem(food.FoodItem);
            }

            Assert.True(results.Count() > 10);
        }

        [Fact]
        public void TestWorkspaceViewModel()
        {
            var foodRebalFile = new FoodRebalanceFileHandler(new System.IO.DirectoryInfo(@".\FoodRebalance\Items\"));
            var enduranceFile = new EnduranceFileHandler(new System.IO.FileInfo(@".\config\org.bepinex.plugins.foodstaminaregen.cfg"));

            var workspaceService = new WorkspaceService(foodRebalFile, enduranceFile);
            var workspaceViewModel = new WorkspaceViewModel(workspaceService);

            Assert.True(workspaceViewModel.FoodItems.Count() > 10);
        }
    }
}
