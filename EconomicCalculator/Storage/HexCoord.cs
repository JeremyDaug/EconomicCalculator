using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage
{
    /// <summary>
    /// Hex Grid Coordinates, based on here https://www.redblobgames.com/grids/hexagons/
    /// </summary>
    public struct HexCoord
    { 
        public HexCoord(int x, int y)
        {
            this.x = x;
            this.y = y;
            z = -x - y;
        }

        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }

        /// <summary>
        /// Distance to another <see cref="HexCoord"/>.
        /// </summary>
        /// <param name="other">The other coordinates.</param>
        /// <returns></returns>
        public int DistanceTo(HexCoord other)
        {
            return Math.Abs(x - other.x) + Math.Abs(y - other.y) + Math.Abs(z - other.z);
        }
    }
}
