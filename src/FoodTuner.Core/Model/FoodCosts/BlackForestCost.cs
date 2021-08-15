using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodTuner.Model.Costs
{
    public class BlackForestCost : FoodCostBase
    {
        public override int BaseHealth => 15;
        public override int BaseStamina => 15;
        public override int BaseRegen => 1;
        public override int BaseDuration => 480;
        public override int BaseEndurance => 0;
    }
}
