using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Objects.Pops.Species
{
    /// <summary>
    /// Various modifiers and tags that can be placed on a species.
    /// </summary>
    public enum SpeciesTag
    {
        /// <summary>
        /// This Species cannot die under any circumstance but combat.
        /// </summary>
        Immortal,
        /// <summary>
        /// This species is not born, but made.
        /// </summary>
        Unborn,
        /// <summary>
        /// This species is a drone, incapable of having culture or 
        /// rebelling.
        /// </summary>
        Drone
    }
}
