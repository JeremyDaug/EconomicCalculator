using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconDTOs.DTOs.Pops.Species
{
    /// <summary>
    /// The various tags that can be attached to a species
    /// Subtype, or Cohort.
    /// </summary>
    public enum SpeciesTags
    {
        /// <summary>
        /// The species is not born, but made.
        /// </summary>
        Unborn,
        /// <summary>
        /// This group is incapable of producing offpring.
        /// </summary>
        Sterile,
        /// <summary>
        /// This species can only be killed, it will not
        /// die naturally.
        /// </summary>
        Deathless,
        /// <summary>
        /// This Group has no indpendent thought and has no
        /// happiness, politics, culture, or any other form
        /// of independence. Instead of measuring crime,
        /// this group measures against deviancy.
        /// </summary>
        Drone,
        PreferredCulture,
        InnateCulture,

    }
}
