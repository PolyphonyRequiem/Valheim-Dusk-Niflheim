using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodTuner.Model;
using FoodTuner.Model.Budgeting;
using FoodTuner.Model.Costs;
using FoodTuner.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Duration = FoodTuner.Model.Duration;

namespace FoodTuner.ViewModels
{
    public class FoodItemViewModel : ObservableValidator
    {
        private static Dictionary<Biomes, FoodCostBase> BiomeCostMap = new()
        {
            [Biomes.Meadows] = new MeadowsCost(),
            [Biomes.BlackForest] = new BlackForestCost(),
            [Biomes.Swamp] = new SwampCost(),
            [Biomes.Ocean] = new OceanCost(),
            [Biomes.Mountain] = new MountainsCost(),
            [Biomes.Plains] = new PlainsCost(),
            [Biomes.Mistlands] = new MistlandsCost(),
            [Biomes.Ashlands] = new AshlandsCost(),
            [Biomes.DeepNorth] = new DeepNorthCost()
        };

        private static string[] biomeNames = new string[]
        {
            "Meadows",
            "Black Forest",
            "Swamp",
            "Mountain",
            "Plains",
            "Ocean",
            "Mistlands",
            "Ashlands",
            "Deep North"
        };

        private static string[] complexities = new string[]
        {
            "Foragable",
            "Ingredient - Simple",
            "Ingredient - Common",
            "Ingredient - Complex",
            "Meal - Very Common",
            "Meal - Common",
            "Meal - Uncommon",
            "Meal - Rare",
            "Meal - Epic"
        };

        private static string[] durations = new string[]
        {
            "Snack",
            "Very Short",
            "Short",
            "Average",
            "Long",
            "Very Long",
            "Extremely Long"
        };

        private readonly FoodItem foodItem;
        private bool isUpdated;

        public FoodItemViewModel(FoodItem foodItem, FileInfo fileOfOrigin, WorkspaceService workspaceService)
        {
            this.foodItem = foodItem;
            this.SaveCommand = new RelayCommand(
                () => {
                    try
                    {
                        workspaceService.SaveFoodItem(this.foodItem, fileOfOrigin);
                    }
                    catch (Exception exc)
                    {
                        // handle this.
                        MessageBox.Show(exc.Message);
                    }
                    IsUpdated = false;
                },
                () => IsUpdated);
        }

        public IEnumerable<string> BiomeNames => biomeNames;

        public IEnumerable<string> Complexities => complexities;

        public IEnumerable<string> Durations => durations;

        public IRelayCommand SaveCommand { get; set; }

        public bool IsUpdated
        {
            get => isUpdated;
            set
            {
                SetProperty(ref isUpdated, value);
                this.SaveCommand.NotifyCanExecuteChanged();
            }
        }

        public string Name {
            get => foodItem.Name;
            set => UpdatePropertyAndMarkUpdated(foodItem.Name, value, foodItem, (f, n) => f.Name = n);
        }

        public string GameId
        {
            get => foodItem.GameId;
            set => UpdatePropertyAndMarkUpdated(foodItem.GameId, value, foodItem, (f, n) => f.GameId = n);
        }

        public Biomes Biome
        {
            get => foodItem.Biome;
            set => UpdatePropertyAndMarkUpdated(foodItem.Biome, value, foodItem, (f, n) => f.Biome = n);
        }

        public FoodComplexity Complexity
        {
            get => foodItem.Complexity;
            set => UpdatePropertyAndMarkUpdated(foodItem.Complexity, value, foodItem, (f, n) => f.Complexity = n);
        }

        public bool BudgetBonus
        {
            get => foodItem.BudgetBonus;
            set => UpdatePropertyAndMarkUpdated(foodItem.BudgetBonus, value, foodItem, (f, n) => f.BudgetBonus = n);
        }

        public int Health
        {
            get => foodItem.Health;
            set => UpdatePropertyAndMarkUpdated(foodItem.Health, value, foodItem, (f, n) => f.Health = n);
        }

        public int Stamina
        {
            get => foodItem.Stamina;
            set => UpdatePropertyAndMarkUpdated(foodItem.Stamina, value, foodItem, (f, n) => f.Stamina = n);
        }

        public int Regen
        {
            get => foodItem.Regen;
            set => UpdatePropertyAndMarkUpdated(foodItem.Regen, value, foodItem, (f, n) => f.Regen = n);
        }

        public Duration Duration
        {
            get => Enum.GetValues<Duration>().ToList().Cast<int>().Contains(foodItem.Duration) ?
                                            (Duration)foodItem.Duration :
                                            Duration.Average;
            set => UpdatePropertyAndMarkUpdated(foodItem.Duration, (int)value, foodItem, (f, n) => f.Duration = n);
        }

        public int Endurance
        {
            get => foodItem.Endurance;
            set => UpdatePropertyAndMarkUpdated(foodItem.Endurance, value, foodItem, (f, n) => f.Endurance = n);
        }

        public double Weight
        {
            get => foodItem.Weight;
            set => UpdatePropertyAndMarkUpdated(foodItem.Weight, value, foodItem, (f, n) => f.Weight = n);
        }

        public int Stack
        {
            get => foodItem.Stack;
            set => UpdatePropertyAndMarkUpdated(foodItem.Stack, value, foodItem, (f, n) => f.Stack = n);
        }
        
        public double BudgetBalance => TotalBudget - BudgetSpent;
        public double TotalBudget => FoodBudgeter.GetBudget(this.foodItem, this.BudgetBonus);
        public double BudgetSpent => BiomeCostMap[this.Biome].DetermineCost(this.foodItem);

        public bool UpdatePropertyAndMarkUpdated<TModel, T>(T oldValue, T newValue, TModel model, Action<TModel, T> callback, [CallerMemberName] string? propertyName = null) where TModel : class
        {
            this.IsUpdated = true;
            var result = SetProperty(oldValue, newValue, model, callback, propertyName);
            OnPropertyChanged("BudgetBalance");
            OnPropertyChanged("TotalBudget");
            OnPropertyChanged("BudgetSpent");
            return result;
        }        
    }
}
