using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.DTOs.Pops
{
    /// <summary>
    /// Interface for population groups
    /// </summary>
    public interface IPopulationGroup
    {
        /// <summary>
        /// The Unique ID of the population.
        /// </summary>
        int Id { get; }
    }
}
