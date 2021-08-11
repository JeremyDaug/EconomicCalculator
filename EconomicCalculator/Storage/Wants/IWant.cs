using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage.Wants
{
    /// <summary>
    /// The Wants available to the system.
    /// </summary>
    public interface IWant
    {
        /// <summary>
        /// The Want Id
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The Want Name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Takes the want and creates a satisfaction string for
        /// it.
        /// </summary>
        /// <param name="d">The value to place in the "< >" </param>
        /// <returns></returns>
        string ToSatisfactionString(decimal d);
    }
}
