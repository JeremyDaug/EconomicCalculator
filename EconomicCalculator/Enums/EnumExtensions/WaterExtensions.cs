using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Enums.EnumExtensions
{
    public static class WaterExtensions
    {
        public static bool IsSaltwater(this WaterFlag water)
        {
            return (water & WaterFlag.SaltWater) == WaterFlag.SaltWater;
        }

        public static bool IsFreshwater(this WaterFlag water)
        {
            return (water & WaterFlag.SaltWater) != WaterFlag.SaltWater;
        }

        public static bool HasNEWater(this WaterFlag water)
        {
            return (water & WaterFlag.NE) == WaterFlag.NE;
        }

        public static bool HasEWater(this WaterFlag water)
        {
            return (water & WaterFlag.E) == WaterFlag.E;
        }

        public static bool HasSEWater(this WaterFlag water)
        {
            return (water & WaterFlag.SE) == WaterFlag.SE;
        }

        public static bool HasSWWater(this WaterFlag water)
        {
            return (water & WaterFlag.SW) == WaterFlag.SW;
        }

        public static bool HasWWater(this WaterFlag water)
        {
            return (water & WaterFlag.W) == WaterFlag.W;
        }

        public static bool HasNWWater(this WaterFlag water)
        {
            return (water & WaterFlag.NW) == WaterFlag.NW;
        }
    }
}
