using CommunityToolkit.Mvvm.ComponentModel;
using FoodTuner.Services;
using System.Collections.ObjectModel;
using System.Linq;

namespace FoodTuner.ViewModels
{
    public class WorkspaceViewModel : ObservableRecipient
    {
        private readonly WorkspaceService workspaceService;

        public WorkspaceViewModel(WorkspaceService workspaceService)
        {
            this.workspaceService = workspaceService;
            this.RefreshFoodItems();
        }

        private void RefreshFoodItems()
        {
            var foodRecords = this.workspaceService.LoadFoodItemRecords();

            foreach (var foodRecord in foodRecords)
            {
                if (FoodItems.Any(f => f.GameId == foodRecord.FoodItem.GameId && f.Name == foodRecord.FoodItem.Name))
                {
                    var viewModelToUpdate = FoodItems.First(f => f.GameId == foodRecord.FoodItem.GameId && f.Name == foodRecord.FoodItem.Name);
                    viewModelToUpdate.Biome = foodRecord.FoodItem.Biome;
                    viewModelToUpdate.Complexity = foodRecord.FoodItem.Complexity;
                    viewModelToUpdate.BudgetBonus = foodRecord.FoodItem.BudgetBonus;
                    viewModelToUpdate.Health = foodRecord.FoodItem.Health;
                    viewModelToUpdate.Stamina = foodRecord.FoodItem.Stamina;
                    viewModelToUpdate.Regen = foodRecord.FoodItem.Regen;
                    viewModelToUpdate.Duration = foodRecord.FoodItem.Duration;
                    viewModelToUpdate.Endurance = foodRecord.FoodItem.Endurance;
                    viewModelToUpdate.Weight = foodRecord.FoodItem.Weight;
                    viewModelToUpdate.Stack = foodRecord.FoodItem.Stack;
                }
                else
                {
                    FoodItems.Add(new FoodItemViewModel(foodRecord.FoodItem, foodRecord.FileInfo, this.workspaceService));
                }
            }
        }

        public ObservableCollection<FoodItemViewModel> FoodItems { get; set; } = new ObservableCollection<FoodItemViewModel>();
    }
}
