using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage
{
    /// <summary>
    /// The manager and storgae for population groups in a market.
    /// </summary>
    public interface IPopulations
    {
        /// <summary>
        /// The name of the population group.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The market the populations are a part of.
        /// </summary>
        IMarket Market { get; }

        /// <summary>
        /// The total population of the market.
        /// </summary>
        double TotalPopulation { get; }

        /// <summary>
        /// The Population Groups
        /// </summary>
        IList<IPopulationGroup> Pops { get; }

        /// <summary>
        /// The list of pops, organized by Priority.
        /// </summary>
        IList<IPopulationGroup> PopsByPriority { get; }

        #region Actions

        /// <summary>
        /// The Production Phase of the populations.
        /// They consume already bought products, and produce new products.
        /// </summary>
        /// <returns>The change in products in the market.</returns>
        IProductAmountCollection ProductionPhase();

        /// <summary>
        /// The Populations buy goods from the markets for their needs and jobs.
        /// </summary>
        /// <param name="market">The market they are buying from.</param>
        /// <returns>The change in products in the market.</returns>
        IProductAmountCollection BuyPhase(IMarket market);

        /// <summary>
        /// Makes the Populations consume their non-job related needs and returns
        /// their satisfaction.
        /// </summary>
        /// <returns>The satisfaction of the populations overall.</returns>
        IProductAmountCollection Consume();

        /// <summary>
        /// Changes in the population due to the market. Placeholder.
        /// </summary>
        void PopulationChanges();

        #endregion Actions
    }
}
