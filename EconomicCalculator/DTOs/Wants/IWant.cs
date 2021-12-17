using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EconomicCalculator.DTOs.Wants
{
    /// <summary>
    /// The Wants available to the system.
    /// </summary>
    public interface IWant
    {
        /// <summary>
        /// The Want Id
        /// </summary>
        [JsonIgnore]
        int Id { get; }

        /// <summary>
        /// The Want Name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// A short description of the want and how it is should be
        /// used elsewhere.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Takes the want and creates a satisfaction string for
        /// it.
        /// </summary>
        /// <param name="d">The value to place in the "< >" </param>
        /// <returns></returns>
        string ToSatisfactionString(decimal d);
    }
}
