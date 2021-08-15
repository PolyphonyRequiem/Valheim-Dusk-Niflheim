using FoodTuner.Model;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace FoodTuner.FileHandlers
{
    public class EnduranceFileHandler
    {
        private readonly FileInfo enduranceFile;

        public EnduranceFileHandler(FileInfo enduranceFile)
        {
            this.enduranceFile = enduranceFile;
        }

        public int GetEnduranceForFoodItem(FoodItem foodItem)
        {
            Regex enduranceExpression = new Regex(@$"{foodItem.Name}\s*=\s*(?<Endurance>[0-9.]+)");
            var content = File.ReadAllText(enduranceFile.FullName);
            var match = enduranceExpression.Match(content);

            if (match.Success)
            {
                return (int)(decimal.Parse(match.Groups["Endurance"].Value) * 10);
            }
            else
            {
                return 0;
            }
        }

        public void UpdateFoodItemEndurance(FoodItem foodItem)
        {
            Regex enduranceExpression = new Regex(@$"{foodItem.Name}\s*=\s*(?<Endurance>\d+)");
            var content = File.ReadAllText(enduranceFile.FullName);
            var match = enduranceExpression.Match(content);

            if (match.Success)
            {
                enduranceExpression.Replace(content, $"{foodItem.Name} = {foodItem.Endurance * 0.1d}");
            }
            else
            {
                throw new InvalidOperationException($"Unable to update endurance for foodItem {foodItem.Name} because we were unable to find an existing reference.");
            }
        }
    }
}