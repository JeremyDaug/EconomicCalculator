using EconomicCalculator.Refactor.Storage.Organizations;
using EconomicCalculator.DTOs.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Refactor.Storage.Products
{
    /// <summary>
    /// A building is a plot of land that has been built upon. It can be used for various means
    /// </summary>
    public interface IBuilding : IProduct
    {
        /*
         * Buildings are plots of land that work can be done on more effectively. They can even increase
         * the effective land available on a plot by vertical construction. Once built they lock up 
         * plots of land, requiring deconstruction to recover the land.
         * Buildings can also be subdivided into separate owned portions, but are usually done so by 
         * landlords.
         */

        // TODO: Add building height modifier for storage and cost calculations.

        /// <summary>
        /// The type of building it is and what it's purpose is.
        /// </summary>
        BuildingType BuildingType { get; }

        /// <summary>
        /// An estimate of how many people can work/live/own the place at one time.
        /// </summary>
        double PersonSpace { get; }

        /// <summary>
        /// The total storage space available (divided by number of available users).
        /// </summary>
        double AvailableStorageSpace { get; }

        /// <summary>
        /// Placeholder for later thoughts. 
        /// What the building can be used for. (Greenhouse Farms aren't equivalent to Car Factories).
        /// May also be used to limit storage (A random warehouse doesn't store grain as well as a Silo).
        /// </summary>
        IList<string> Specializations { get; }

        /// <summary>
        /// Where the building is located.
        /// </summary>
        ITerritory Location { get; }

        /// <summary>
        /// The number of plots the building takes up. To make traking used space easier.
        /// </summary>
        int PlotsRequired { get; }
    }
}
