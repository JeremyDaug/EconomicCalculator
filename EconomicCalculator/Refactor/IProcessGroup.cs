using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Generators
{
    /// <summary>
    /// A group of processes which produce the same good(s).
    /// </summary>
    public interface IProcessGroup
    {
        /// <summary>
        /// The name of the group of processes.
        /// </summary>
        string Name { get; }
    }
}
