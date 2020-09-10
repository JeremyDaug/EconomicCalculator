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
        /// The name of the Population's Skill.
        /// </summary>
        string SkillName { get; }

        /// <summary>
        /// The Skill Level of the population.
        /// </summary>
        int SkillLevel { get; }

        /// <summary>
        /// The Labor the population can produce (overflow from their job).
        /// </summary>
        IProduct JobLabor { get; }

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

        #region PastSatisfaction
        // These values represent the satisfaction of the pop ffrom the last run ConsumptionPhase.
        // It also includes helper functions to find how much shortfall there was.

        /// <summary>
        /// The satisfaction of the population's life needs.
        /// </summary>
        IProductAmountCollection LifeSatisfaction { get; }

        /// <summary>
        /// The satisfaction of the population's daily needs.
        /// </summary>
        IProductAmountCollection DailySatisfaction { get; }

        /// <summary>
        /// The satisfaction of the population's luxury needs.
        /// </summary>
        IProductAmountCollection LuxurySatisfaction { get; }

        /// <summary>
        /// The life need shortfall (if any).
        /// </summary>
        IProductAmountCollection LifeShortfall { get; }

        /// <summary>
        /// The Daily need shortfall (if any).
        /// </summary>
        IProductAmountCollection DailyShortfall { get; }

        /// <summary>
        /// The Daily need shortfall (if any).
        /// </summary>
        IProductAmountCollection LuxuryShortfall { get; }

        #region JobSatisfaction

        /// <summary>
        /// The satisfaction of all the job inputs.
        /// </summary>
        IProductAmountCollection JobInputSatisfaction { get; }

        /// <summary>
        /// The satisfaction of all the job capital requirements.
        /// </summary>
        IProductAmountCollection JobCapitalSatisfaction { get; }

        /// <summary>
        /// The Job Input shortfall (if any).
        /// </summary>
        IProductAmountCollection JobInputShortfall { get; }

        /// <summary>
        /// The Job Input shortfall (if any).
        /// </summary>
        IProductAmountCollection JobCapitalShortfall { get; }

        #endregion JobSatisfaction

        #endregion PastSatisfaction

        #region MarketExpansions

        /// <summary>
        /// The secondary jobs available to the population in their
        /// downtime.
        /// </summary>
        IList<IJob> SecondaryJobs { get; }

        /// <summary>
        /// Products currently stored by the Population.
        /// Should never have a negative storage value.
        /// </summary>
        IProductAmountCollection Storage { get; }

        /// <summary>
        /// The products owned by the population up for sale.
        /// Goods bought are removed from this list as we go. 
        /// What was traded for goes to storage.
        /// </summary>
        IProductAmountCollection ForSale { get; }

        /// <summary>
        /// The total summed needs of the population for the day.
        /// </summary>
        IProductAmountCollection TotalNeeds { get; }

        #endregion MarketExpansions

        // Placeholder for additional Information, demographic breakdowns, and the like.

        #region Helpers

        /// <summary>
        /// Initializes the storage to ensure it includes all products from 
        /// Needs, and job. Run after any change to needs or jobs.
        /// </summary>
        void InitializeStorage();

        #endregion Helpers

        #region Actions

        /// <summary>
        /// The Production Phase of the population.
        /// </summary>
        /// <returns>The change in products because of the phase.</returns>
        IProductAmountCollection ProductionPhase();

        /// <summary>
        /// The Consumption action of the population group.
        /// </summary>
        /// <returns>A percentage of satisfaction for each good.</returns>
        IProductAmountCollection ConsumptionPhase();

        /// <summary>
        /// Puts the goods stored by the pop up for sale on the market.
        /// </summary>
        /// <returns>The goods being put up on sale.</returns>
        IProductAmountCollection UpForSale();

        /// <summary>
        /// The Loss phase of the population, calculates and removes items due to
        /// failure and breakdown.
        /// </summary>
        /// <returns>The goods lost in the phase.</returns>
        IProductAmountCollection LossPhase();

        // TODO Taxes Phase.

        /// <summary>
        /// A Placeholder for later Population Change Function.
        /// </summary>
        void PopulationChange();

        #endregion Actions
    }
}