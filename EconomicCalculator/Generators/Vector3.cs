using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Generators
{
    public struct Vector3
    {
        /// <summary>
        /// The x component of the Vector.
        /// </summary>
        public double x { get; }

        /// <summary>
        /// The y component of the Vector.
        /// </summary>
        public double y { get; }

        /// <summary>
        /// The z component of the Vector.
        /// </summary>
        public double z { get; }

        /// <summary>
        /// The Length of the Vector
        /// </summary>
        public double Magnitude
        {
            get
            {
                return Math.Sqrt(x * x + y * y + z * z);
            }
        }

        /// <summary>
        /// The Normalized Vector
        /// </summary>
        public Vector3 Normalized
        {
            get
            {
                return new Vector3(x / Magnitude, y / Magnitude, z / Magnitude);
            }
        }

        /// <summary>
        /// The Square of the Magnitude.
        /// </summary>
        public double sqrMagnitude
        {
            get
            {
                return x * x + y * y + z * z;
            }
        }

        public Vector3(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Vector3 back => new Vector3(0, 0, -1);
        public static Vector3 down => new Vector3(0, -1, 0);
        public static Vector3 forward => new Vector3(0, 0, 1);

        public static Vector3 operator *(Vector3 v, double a)
        {
            return new Vector3(v.x * a, v.y * a, v.z * a);
        }
    }
}
