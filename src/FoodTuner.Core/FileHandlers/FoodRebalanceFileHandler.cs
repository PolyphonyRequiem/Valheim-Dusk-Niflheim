using FoodTuner.Core.FileHandlers;
using FoodTuner.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace FoodTuner.FileHandlers
{
    public class FoodRebalanceFileHandler
    {
        private record FoodRebalanceRecord (string name, double weight, int stack, int health, int stamina, int regen, int duration);

        Regex fileExpression = new Regex(@"\d+_(?<Biome>[\w ]+?)_(?<Complexity>[12345]|_i_1|_i_2|_i_3|_f)(?<BonusFlag>\+?)_(?<FoodName>[\w\s'-]*)\.json");

        private readonly DirectoryInfo foodRebalanceDirectory;
        
        public FoodRebalanceFileHandler(DirectoryInfo foodRebalanceDirectory)
        {
            this.foodRebalanceDirectory = foodRebalanceDirectory;
        }         
        
        public IEnumerable<FoodRebalanceRecordInfo> ReadPartiallyConfiguredFoodItems()
        {
            var foodRebalanceFiles = foodRebalanceDirectory.GetFiles("*.json");

            foreach (var file in foodRebalanceFiles)
            {
                FoodRebalanceRecord record = JsonSerializer.Deserialize<FoodRebalanceRecord>(File.ReadAllText(file.FullName))!;
                var foodItem = new FoodItem(GetNameFromFileInfo(file), record.name)
                {
                    Biome = GetBiomeFromFileInfo(file),
                    Complexity = GetComplexityFromFileInfo(file),
                    BudgetBonus = GetBudgetBonusFromFileInfo(file),
                    GameId = record.name,
                    Health = record.health,
                    Stamina = record.stamina,
                    Duration = record.duration,
                    Regen = record.regen,
                    Weight = record.weight,
                    Stack = record.stack,
                    Endurance = 0 // it's in another file annoyingly...                    
                };

                yield return new FoodRebalanceRecordInfo(foodItem, file);
            }
        }

        public void SaveFoodRebalanceFile(FileInfo fileInfo, FoodItem foodItem)
        {
            var fileName = BuildFileNameFromFoodItem(foodItem);

            if (!fileName.Equals(fileInfo.Name, StringComparison.Ordinal))
            {
                // remove the old one
                fileInfo.Delete();

                // write the new one
                WriteFoodRebalanceFile(new FileInfo($"{fileInfo.DirectoryName}/{fileName}"), foodItem);
            }
            else
            {
                // write the new one
                WriteFoodRebalanceFile(fileInfo, foodItem);
            }
        }

        private void WriteFoodRebalanceFile(FileInfo fileInfo, FoodItem foodItem)
        {
            var record = GetFoodRebalanceRecordFromFoodItem(foodItem);
            string contents = JsonSerializer.Serialize(record, new JsonSerializerOptions() {WriteIndented=true })!;
            File.WriteAllText(fileInfo.FullName, contents);
        }

        private FoodRebalanceRecord GetFoodRebalanceRecordFromFoodItem(FoodItem foodItem)
        {
            return new FoodRebalanceRecord(foodItem.GameId, foodItem.Weight, foodItem.Stack, foodItem.Health, foodItem.Stamina, foodItem.Regen, foodItem.Duration);
        }

        private string BuildFileNameFromFoodItem(FoodItem foodItem)
        {
            var budgetBonusString = foodItem.BudgetBonus ? "+" : String.Empty;
            return $"{(int)foodItem.Biome}_{foodItem.Biome}_{GetComplexityString(foodItem.Complexity)}{budgetBonusString}_{foodItem.Name}.json";
        }

        private string GetComplexityString(FoodComplexity complexity)
        {
            return complexity switch
            {
                FoodComplexity.Foragable => "_f",
                FoodComplexity.SimpleIngredient => "_i_1",
                FoodComplexity.CommonIngredient => "_i_2",
                FoodComplexity.ComplexIngredient => "_i_3",
                FoodComplexity.PreparedSingleIngredientFoodItem => "1",
                FoodComplexity.PreparedSimpleMeal => "2",
                FoodComplexity.PreparedCommonMeal => "3",
                FoodComplexity.PreparedComplexMeal => "4",
                FoodComplexity.PreparedLegendaryMeal => "5",
                _ => "_f",
            };
        }

        private Biomes GetBiomeFromFileInfo(FileInfo fileInfo)
        {
            var match = fileExpression.Match(fileInfo.Name);

            if (match.Success)
            {
                return (Biomes)Enum.Parse(typeof(Biomes), match.Groups["Biome"].Value);
            }
            else
            {
                return Biomes.Meadows;
            }
        }

        private FoodComplexity GetComplexityFromFileInfo(FileInfo fileInfo)
        {
            var match = fileExpression.Match(fileInfo.Name);

            if (match.Success)
            {
                return match.Groups["Complexity"].Value switch
                {
                    "_f" => FoodComplexity.Foragable,
                    "_i_1" => FoodComplexity.SimpleIngredient,
                    "_i_2" => FoodComplexity.CommonIngredient,
                    "_i_3" => FoodComplexity.ComplexIngredient,
                    "1" => FoodComplexity.PreparedSingleIngredientFoodItem,
                    "2" => FoodComplexity.PreparedSimpleMeal,
                    "3" => FoodComplexity.PreparedCommonMeal,
                    "4" => FoodComplexity.PreparedComplexMeal,
                    "5" => FoodComplexity.PreparedLegendaryMeal,
                    _ => FoodComplexity.Foragable,
                };
            }
            else
            {
                return FoodComplexity.Foragable;
            }
        }

        private bool GetBudgetBonusFromFileInfo(FileInfo fileInfo)
        {
            var match = fileExpression.Match(fileInfo.Name);

            if (match.Success)
            {
                return match.Groups["BonusFlag"].Value == "+";
            }
            else
            {
                return false;
            }
        }

        private string GetNameFromFileInfo(FileInfo fileInfo)
        {
            var match = fileExpression.Match(fileInfo.Name);

            if (match.Success)
            {
                return match.Groups["FoodName"].Value;
            }
            else
            {
                return "";
            }
        }
    }
}
