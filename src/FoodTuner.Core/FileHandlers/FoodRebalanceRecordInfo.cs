using FoodTuner.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodTuner.Core.FileHandlers
{
    public class FoodRebalanceRecordInfo
    {
        public FoodRebalanceRecordInfo(FoodItem foodItem, FileInfo fileInfo)
        {
            FoodItem = foodItem;
            FileInfo = fileInfo;
        }

        public FoodItem FoodItem { get; set; }
        public FileInfo FileInfo { get; set; }
    }
}
