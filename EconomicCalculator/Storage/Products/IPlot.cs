using EconomicCalculator.Storage.Organizations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage.Products
{
    /// <summary>
    /// The productified form of land, with expanded information needed for it to be stored.
    /// </summary>
    public interface IPlot : IProduct
    {
        /*
         * Plots are chunks of undeveloped land, they cannot be created trivially.
         * The only ways to make land are through land reclamation
         * (requiring water to reclaim the land from).
         * Once all of the land in a territory has been reclaimed and no land remains, no more plots can
         * be made.
         * Plots have a specified size.
         */

        /// <summary>
        /// The parent territory of the plot of land, which defines it's stats.
        /// </summary>
        ITerritory Territory { get; }

        /// <summary>
        /// The smallest size of a plot in Acres, This is universal across all plots.
        /// </summary>
        double Size { get; }
    }
}