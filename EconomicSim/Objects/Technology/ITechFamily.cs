﻿using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EconomicSim.Objects.Technology
{
    /// <summary>
    /// Read Only Tech Family Interface.
    /// </summary>
    public interface ITechFamily
    {
        /// <summary>
        /// Tech Family's Id.
        /// </summary>
        [JsonIgnore]
        int Id { get; }

        /// <summary>
        /// The Name of the Tech Family.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Related Tech Families
        /// </summary>
        IReadOnlyList<ITechFamily> Relations { get; }

        /// <summary>
        /// The techs in this family.
        /// </summary>
        IReadOnlyList<ITechnology> Techs { get; }

        /// <summary>
        /// A Description of the Tech Family.
        /// </summary>
        string Description { get; }
    }
}