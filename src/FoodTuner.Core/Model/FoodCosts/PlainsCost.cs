using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodTuner.Model.Costs
{
    public class PlainsCost : FoodCostBase
    {
        public override int BaseHealth => 30;
        public override int BaseStamina => 30;
        public override int BaseRegen => 1;
        public override int BaseEndurance => 4;
    }
}
