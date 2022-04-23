using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EconomicSim.DTOs.Hexmap
{
    /// <summary>
    /// Hex Grid Coordinates, based on here https://www.redblobgames.com/grids/hexagons/
    /// 0,0,0 is upper left corner. like so (x is <-> y is ^-V
    /// 
    ///  +(0,0,0) -(1,0,-1) -(2,0,-2) +(3,0,-3)
    ///   \(0,1,-1) \(1,1,-2) \(2,1,-3) \(3,1,-4)
    ///    \(0,2,-2) \(1,0,-1) \(2,2,-4) \(3,2,-5)
    ///     \(0,3,-3) \(1,0,-1) \(2,3,-5) \(3,3,-6)
    ///      +(0,4,-4) -(1,0,-1) -(2,4,-6) +(3,4,-7)
    ///      
    /// 
    ///      (0,-1,1) (1,-1,0)
    ///  (-1,0,1) (0,0,0) {1,0,-1)
    ///      (-1,1,0) (0, 1,-1)
    /// </summary>
    public struct HexCoord
    {
        #region CommonAdjacentModifiers

        /// <summary>
        /// NE hex from Origin
        /// </summary>
        public static HexCoord NE => new HexCoord(1, -1, 0);

        /// <summary>
        /// E hex from Origin
        /// </summary>
        public static HexCoord E  => new HexCoord(1, 0, -1);
        
        /// <summary>
        /// SE hex from Origin
        /// </summary>
        public static HexCoord SE => new HexCoord(0, 1, -1);
        
        /// <summary>
        /// SW hex from Origin
        /// </summary>
        public static HexCoord SW => new HexCoord(-1, 1, 0);
        
        /// <summary>
        /// W hex from Origin
        /// </summary>
        public static HexCoord W  => new HexCoord(-1, 0, 1);
        
        /// <summary>
        /// NW hex from Origin
        /// </summary>
        public static HexCoord NW => new HexCoord(0, -1, 1);

        #endregion CommonAdjacentModifiers

        public HexCoord(int x, int y)
        {
            this.x = x;
            this.y = y;
            z = -x - y;
        }

        public HexCoord(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            if (x + y + z != 0)
                throw new ArgumentException("x, y, and z must add to 0.");
        }

        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }

        #region Operators

        /// <summary>
        /// addition operator.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static HexCoord operator +(HexCoord a, HexCoord b)
        {
            return new HexCoord(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        /// <summary>
        /// Subtraction Operator
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static HexCoord operator -(HexCoord a, HexCoord b)
        {
            return new HexCoord(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        /// <summary>
        /// Negative Operator
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static HexCoord operator -(HexCoord a)
        {
            return new HexCoord(-a.x, -a.y, -a.z);
        }

        /// <summary>
        /// Multiplies the coordinate by i.
        /// </summary>
        /// <param name="i">the scalar.</param>
        /// <returns></returns>
        public HexCoord Scale(int i)
        {
            return new HexCoord(x * i, y * i, z * i);
        }

        public override string ToString()
        {
            return $"({x},{y},{z})";
        }

        public HexCoord RotateClockwise()
        {
            return new HexCoord(-z, -x, -y);
        }

        public HexCoord RotateClockwiseTwice()
        {
            return new HexCoord(y, z, x);
        }

        public HexCoord RotateCounterclockwise()
        {
            return new HexCoord(-y, -z, -x);
        }

        public HexCoord RotateCounterclockwiseTwice()
        {
            return new HexCoord(z, x, y);
        }

        #endregion Operators

        #region DirectionalSteps

        [JsonIgnore]
        public HexCoord ToNE => this + NE;
        [JsonIgnore]
        public HexCoord ToE => this + E;
        [JsonIgnore]
        public HexCoord ToSE => this + SE;
        [JsonIgnore]
        public HexCoord ToSW => this + SW;
        [JsonIgnore]
        public HexCoord ToW => this + W;
        [JsonIgnore]
        public HexCoord ToNW => this + NW;

        #endregion DirectionalSteps

        /// <summary>
        /// Distance to another <see cref="HexCoord"/>.
        /// </summary>
        /// <param name="other">The other coordinates.</param>
        /// <returns></returns>
        public int DistanceTo(HexCoord other)
        {
            return (Math.Abs(x - other.x) + Math.Abs(y - other.y) + Math.Abs(z - other.z))/2;
        }

        public static int DistanceBetween(HexCoord a, HexCoord b)
        {
            return a.DistanceTo(b);
        }

        /// <summary>
        /// Gets adjacent hexes starting at NE and moving clockwise.
        /// </summary>
        /// <returns>The hexes adjacent to this hex.</returns>
        public HexCoord[] AdjacentHexes()
        {
            HexCoord[] results = new HexCoord[6];

            results[0] = ToNE;
            results[1] = ToE;
            results[2] = ToSE;
            results[3] = ToSW;
            results[4] = ToW;
            results[5] = ToNW;

            return results;
        }

        public static bool operator ==(HexCoord b1, HexCoord b2)
        {
            if (b1 == null)
                return b2 == null;

            return b1.Equals(b2);
        }

        public static bool operator !=(HexCoord b1, HexCoord b2)
        {
            return !(b1 == b2);
        }

        public override bool Equals(object obj)
        {
            // ensure it's not null and of type Hexcoord
            if (obj == null || obj.GetType() != typeof(HexCoord))
                return false;

            var check = (HexCoord)obj;
            // check it's values are equal
            if (x == check.x && y == check.y && z == check.z)
                return true;
            // if not it's not equal
            return false;
        }

        // TODO Ring Function
    }
}
