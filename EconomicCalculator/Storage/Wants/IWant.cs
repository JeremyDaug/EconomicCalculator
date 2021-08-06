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
    }
}
