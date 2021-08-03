using EconomicCalculator.Refactor.Storage.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Refactor.Storage.Population
{
    /// <summary>
    /// The manager and storgae for population groups in a market.
    /// </summary>
    public interface IPopulations
    {
        // Should not need stored by DB as this is just a holder for pop Groups.

        #region GeneralData

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
        /// The average growth rate across all the pop groups.
        /// </summary>
        double PopGrowthRate { get; }

        /// <summary>
        /// The Population Groups
        /// </summary>
        IList<IPopulationGroup> Pops { get; }

        #endregion GeneralData

        /// <summary>
        /// The list of pops, organized by Priority. Merchants are not included.
        /// </summary>
        /// <remarks>
        /// <see cref="Pops"/>, sorted by priority.
        /// </remarks>
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

        /// <summary>
        /// The Total demands of the population.
        /// </summary>
        /// <returns></returns>
        IProductAmountCollection TotalDemand();

        /// <summary>
        /// The total production capability of the populations.
        /// </summary>
        /// <returns></returns>
        IProductAmountCollection TotalProduction();

        /// <summary>
        /// The satisfaction of the total population's life needs.
        /// </summary>
        /// <returns>The percent of satisfaction.</returns>
        double LifeNeedsSatisfaction();

        /// <summary>
        /// The satisfaction of the total population's daily needs.
        /// </summary>
        /// <returns>The percent of satisfaction.</returns>
        double DailyNeedsSatisfaction();

        /// <summary>
        /// The satisfaction of the total population's luxury needs.
        /// </summary>
        /// <returns>The percent of satisfaction.</returns>
        double LuxuryNeedsSatisfaction();

        /// <summary>
        /// The satisfaction of the total population's job inputs.
        /// </summary>
        /// <returns>The percent of satisfaction.</returns>
        double JobInputSatisfaction();

        /// <summary>
        /// The satisfaction of the total population's job capital.
        /// </summary>
        /// <returns>The percent of satisfaction.</returns>
        double JobCapitalSatisfaction();

        #endregion Actions
    }
}
