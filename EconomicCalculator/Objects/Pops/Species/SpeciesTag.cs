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
        /// <summary>
        /// CultureModifire<Culture; Attraction>
        /// This species has a culture that is particularly attractive or
        /// repulsive to it innately. This culture cannot change, though child cultures
        /// can form. The child cultures will share the modifier from this.
        /// Attraction Range [-1,1] At -1 it will totally repulse the species,
        /// making them leave immediately if they are in it, and making the chance
        /// of them joining in the first place, 0. 1 will make the culture innate to the
        /// species, they will always join it if given the chance, and will not leave it.
        /// </summary>
        CultureModifier
    }
}
