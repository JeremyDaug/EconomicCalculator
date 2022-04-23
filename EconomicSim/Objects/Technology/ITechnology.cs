using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicSim.Objects.Technology
{
    /// <summary>
    /// Readonly Technology Interface
    /// </summary>
    public interface ITechnology
    {
        /// <summary>
        /// The Unique Id of the Tech
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The Name of the Technology.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// What tier of tech it is.
        /// </summary>
        TechCategory Category { get; }

        /// <summary>
        /// The Tier of the tech.
        /// </summary>
        int Tier { get; }

        /// <summary>
        /// The families of the tech.
        /// </summary>
        IReadOnlyList<ITechFamily> Families { get; }

        /// <summary>
        /// Technologies which come from this one.
        /// Outgoing connections.
        /// </summary>
        IReadOnlyList<ITechnology> Children { get; }

        /// <summary>
        /// Technologies which this tech can come from.
        /// Incoming connections.
        /// </summary>
        IReadOnlyList<ITechnology> Parents { get; }

        /// <summary>
        /// The base cost of the Technology to research.
        /// </summary>
        int TechCostBase { get; }

        /// <summary>
        /// A Description of the Technology.
        /// </summary>
        string Description { get; }

        // Connections to what the tech gives.
    }
}
