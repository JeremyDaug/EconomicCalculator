using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Objects.Pops.Worldview
{
    /// <summary>
    /// Read only interface for species.
    /// </summary>
    public interface ISpecies
    {
        /// <summary>
        /// Species Id
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The name of the Species
        /// </summary>
        string Name { get; }
    }
}
