using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodTuner.Configuration
{
    public class WorkspaceConfiguration
    {
        public string FilePathForEnduranceConfiguration { get; set; } = String.Empty;

        public string DirectoryPathForFoodConfiguration { get; set; } = String.Empty;

        public string FilePathForRecipeRewriterConfigurations { get; set; } = String.Empty;
    }
}
