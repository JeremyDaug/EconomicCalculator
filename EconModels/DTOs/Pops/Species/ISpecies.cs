using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconDTOs.DTOs.Pops.Species
{
    /// <summary>
    /// The information for a species in the system.
    /// </summary>
    public interface ISpecies
    {
        /// <summary>
        /// Unique ID of the species.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The name of the species, must be unique with it's variant
        /// name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The variant name of the species, may be empty
        /// must be unique when comiben with Name.
        /// </summary>
        string VariantName { get; }
    }
}
