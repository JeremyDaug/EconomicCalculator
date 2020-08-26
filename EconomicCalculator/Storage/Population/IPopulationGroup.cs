using System;
using System.Collections.Generic;

namespace EconomicCalculator.Storage
{
    /// <summary>
    /// A population group maintaining it's job, 
    /// </summary>
    public interface IPopulationGroup : IEquatable<IPopulationGroup>, ISqlReader, IEqualityComparer<IPopulationGroup>
    {
        #region GeneralData

        /// <summary>
        /// The Id of the job.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Population Group Name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The number of people in the Group.
        /// </summary>
        double Count { get; }

        /// <summary>
        /// The Primary Job of the Population that defines them.
        /// </summary>
        IJob PrimaryJob { get; }

        /// <summary>
        /// The Population's buy priority (the higher the number the earlier they go).
        /// </summary>
        int Priority { get; }

        #endregion GeneralData

        #region Needs

        /// <summary>
        /// The products needed (and consumed) every day to keep a pop alive.
        /// Final values multiplied by <see cref="Count"/>.
        /// </summary>
        IProductAmountCollection LifeNeeds { get; }

        /// <summary>
        /// The products needed (and consumed) every day to keep a pop Content.
        /// Final values multiplied by <see cref="Count"/>.
        /// </summary>
        IProductAmountCollection DailyNeeds { get; }

        /// <summary>
        /// The products needed (and consumed) that represent the pop being successful.
        /// Final values multiplied by <see cref="Count"/>.
        /// </summary>
        IProductAmountCollection LuxuryNeeds { get; }

        #endregion Needs

        #region MarketExpansions

        /// <summary>
        /// The secondary jobs available to the population in their
        /// downtime.
        /// </summary>
        IList<IJob> SecondaryJobs { get; }

        /// <summary>
        /// Products currently stored by the Population.
        /// </summary>
        IProductAmountCollection Storage { get; }

        #endregion MarketExpansions

        // Placeholder for storage space.

        // Placeholder for additional Information, demographic breakdowns, and the like.

        #region Actions

        /// <summary>
        /// The Production Phase of the population.
        /// </summary>
        /// <returns>The change in products because of the phase.</returns>
        IProductAmountCollection ProductionPhase();

        /// <summary>
        /// Population Buys from the market to meet it's own demands.
        /// </summary>
        /// <param name="market">The market the pop is buying from.</param>
        /// <returns>The change in goods bought/traded.</returns>
        IProductAmountCollection BuyPhase(IMarket market);

        /// <summary>
        /// The Consumption action of the population group.
        /// </summary>
        /// <returns>A persentage of satisfaction for each good.</returns>
        IProductAmountCollection Consume();

        /// <summary>
        /// Calculates the Change in the population based off of
        /// Needs Met, Items Sold, and availablility of better jobs.
        /// </summary>
        void PopulationChange();

        #endregion Actions
    }
}