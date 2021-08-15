using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodTuner.Model.Costs
{
    public class AshlandsCost : FoodCostBase
    {
        public override int BaseHealth => 50;
        public override int BaseStamina => 40;
        public override int BaseRegen => 2;
        public override int BaseDuration => 480;
        public override int BaseEndurance => 2;
    }
}
