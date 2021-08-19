﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodTuner.Model.Costs
{
    public class DeepNorthCost : FoodCostBase
    {
        public override int BaseHealth => 50;
        public override int BaseStamina => 50;
        public override int BaseRegen => 3;
        public override int BaseEndurance => 4;
    }
}