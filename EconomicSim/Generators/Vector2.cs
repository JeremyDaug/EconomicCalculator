using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicSim.Generators
{
    public struct Vector2
    {
        /// <summary>
        /// The x component of the vector.
        /// </summary>
        public double x { get; }

        /// <summary>
        /// The y component of the vector.
        /// </summary>
        public double y { get; }

        /// <summary>
        /// The Magnitude of the vector.
        /// </summary>
        public double Magnitude
        {
            get
            {
                return Math.Sqrt(x * x + y * y);
            }
        }

        /// <summary>
        /// The Normalized Vector.
        /// </summary>
        public Vector2 Normalized
        {
            get
            {
                return new Vector2(x / Magnitude, y / Magnitude);
            }
        }

        public Vector2(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
