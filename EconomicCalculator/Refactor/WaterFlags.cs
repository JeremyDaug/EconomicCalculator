using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Enums
{
    [Flags]
    public enum WaterFlags : int
    {
        /// <summary>
        /// Water to the North East
        /// </summary>
        NE = 0b00000001,
        /// <summary>
        /// Water to the East
        /// </summary>
        E  = 0b00000010,
        /// <summary>
        /// Water to the South East
        /// </summary>
        SE = 0b00000100,
        /// <summary>
        /// Water to the South West
        /// </summary>
        SW = 0b00001000,
        /// <summary>
        /// Water to the West
        /// </summary>
        W  = 0b00010000,
        /// <summary>
        /// Water to the North West
        /// </summary>
        NW = 0b00100000,
        /// <summary>
        /// The water adjacent to here is saltwater.
        /// </summary>
        SaltWater = 0b01000000
    }
}
