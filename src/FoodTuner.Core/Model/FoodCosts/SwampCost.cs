using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodTuner.Model.Costs
{
    public class SwampCost : FoodCostBase
    {
        public override int BaseHealth => 20;
        public override int BaseStamina => 20;
        public override int BaseRegen => 1;
        public override int BaseEndurance => 0;
    }
}
