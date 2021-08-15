using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodTuner.Model.Costs
{
    public class OceanCost : FoodCostBase
    {
        public override int BaseHealth => 25;
        public override int BaseStamina => 25;
        public override int BaseRegen => 1;
        public override int BaseDuration => 360;
        public override int BaseEndurance => 5;
    }
}
