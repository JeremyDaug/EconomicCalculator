using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Enums.EnumExtensions
{
    public static class WaterExtensions
    {
        public static bool IsSaltwater(this WaterFlags water)
        {
            return (water & WaterFlags.SaltWater) == WaterFlags.SaltWater;
        }

        public static bool IsFreshwater(this WaterFlags water)
        {
            return (water & WaterFlags.SaltWater) != WaterFlags.SaltWater;
        }

        public static bool HasNEWater(this WaterFlags water)
        {
            return (water & WaterFlags.NE) == WaterFlags.NE;
        }

        public static bool HasEWater(this WaterFlags water)
        {
            return (water & WaterFlags.E) == WaterFlags.E;
        }

        public static bool HasSEWater(this WaterFlags water)
        {
            return (water & WaterFlags.SE) == WaterFlags.SE;
        }

        public static bool HasSWWater(this WaterFlags water)
        {
            return (water & WaterFlags.SW) == WaterFlags.SW;
        }

        public static bool HasWWater(this WaterFlags water)
        {
            return (water & WaterFlags.W) == WaterFlags.W;
        }

        public static bool HasNWWater(this WaterFlags water)
        {
            return (water & WaterFlags.NW) == WaterFlags.NW;
        }
    }
}
