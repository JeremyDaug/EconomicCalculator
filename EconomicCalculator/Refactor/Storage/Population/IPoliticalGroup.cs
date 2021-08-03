using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Refactor.Storage.Population
{
    public interface IPoliticalGroup
    {
        /// <summary>
        /// The name of the political Group
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Allied Political groups they will likely team up
        /// with when voting occurs.
        /// </summary>
        IList<IPoliticalGroup> Allies { get; set; }

        /// <summary>
        /// Rival Political groups they will be hard pressed to
        /// team up with under any circumstances.
        /// </summary>
        IList<IPoliticalGroup> Enemies { get; set; }

        /// <summary>
        /// The unique features of a political group which modify 
        /// it's functionality.
        /// </summary>
        IList<string> Tags { get; set; }

        /// <summary>
        /// How Radical the political group is.
        /// </summary>
        int Radicalism { get; set; }

        // Placeholder
        // Desired Policies
    }
}
