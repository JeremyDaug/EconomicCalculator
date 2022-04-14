using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Enums
{
    /// <summary>
    /// Available planet types which have predefined material breakdowns.
    /// </summary>
    public enum PlanetType
    {
        /// <summary>
        /// A black hole, the most extreme entity. This strips almost
        /// all information from the planetary body.
        /// </summary>
        BlackHole,
        /// <summary>
        /// Earth Like, has everything needed to support life,
        /// but may not have it.
        /// </summary>
        Terran,
        /// <summary>
        /// Rocky planet, little to no chance for life.
        /// </summary>
        Terrestrial,
        /// <summary>
        /// Planet is a gas giant like jupiter, high in hydrogen
        /// and helium.
        /// </summary>
        Jovian,
        /// <summary>
        /// Gas planet made primarily of Methane and like gasses.
        /// </summary>
        IceGiant,
        /// <summary>
        /// Asteroid made of carbon, silicon, and similar materials.
        /// </summary>
        SilicateAsteroid,
        /// <summary>
        /// Metal and Silicate Heavy.
        /// </summary>
        StonyAsteroid,
        /// <summary>
        /// Heavy metal asteroid.
        /// </summary>
        MetalAsteroid,
        /// <summary>
        /// A small body primarily made of ices.
        /// </summary>
        Comet
    }
}
