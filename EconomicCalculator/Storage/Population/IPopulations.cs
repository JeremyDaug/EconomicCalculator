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
        /// The list of pops, organized by Priority. Merchants are not included.
        /// </summary>
        IList<IPopulationGroup> PopsByPriority { get; }

        /// <summary>
        /// The Population by the job they have.
        /// </summary>
        IDictionary<Guid, IPopulationGroup> PopsByJobs { get; }

        /// <summary>
        /// The merchants in the population group, easily accessible.
        /// </summary>
        IPopulationGroup Merchants { get; }

        /// <summary>
        /// The Money changers which are part of the group, easily accessible.
        /// </summary>
        IPopulationGroup MoneyChangers { get; }

        #region Actions

        /// <summary>
        /// Gets the pops selling a specific product.
        /// </summary>
        /// <param name="product">The product we want to find.</param>
        /// <returns>The populations selling that product, in priority order.</returns>
        IList<IPopulationGroup> GetPopsSellingProduct(IProduct product);

        /// <summary>
        /// The Production Phase of the populations.
        /// They consume already bought products, and produce new products.
        /// </summary>
        /// <returns>The change in products in the market.</returns>
        IProductAmountCollection ProductionPhase();

        /// <summary>
        /// Makes the Populations consume their non-job related needs and returns
        /// the change in products..
        /// </summary>
        /// <returns>The change in products of the market.</returns>
        IProductAmountCollection Consume();

        /// <summary>
        /// Changes in the population due to the market. Placeholder.
        /// </summary>
        void PopulationChanges();

        /// <summary>
        /// Makes each population go through it's product loss phase.
        /// </summary>
        /// <returns>The products lost to decay.</returns>
        IProductAmountCollection LossPhase();

        /// <summary>
        /// The Sell phase for the populations.
        /// </summary>
        /// <returns>The goods up for sale.</returns>
        IProductAmountCollection SellPhase();

        #endregion Actions
    }
}
