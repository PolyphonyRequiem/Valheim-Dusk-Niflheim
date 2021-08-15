using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodTuner.Model.Costs
{
    public class MountainsCost : FoodCostBase
    {
        public override int BaseHealth => 30;
        public override int BaseStamina => 20;
        public override int BaseRegen => 0;
        public override int BaseDuration => 720;
        public override int BaseEndurance => 4;
    }
}
