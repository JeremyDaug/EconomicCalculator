using EconomicCalculator.Storage.Jobs;
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
        /// The daily growth rate of the population.
        /// </summary>
        double PopGrowthRate { get; }

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

        // Placeholder for possible 'capital' desires. Goods which don't get consumed,
        // but instead breakdown and must be replaced, or have maintenance costs.
        // The Daily needs and/or Luxury Needs
        // as they are longer lasting and easier to fulfill long term.

        #endregion Needs

        #region PastSatisfaction
        // These values represent the satisfaction of the pop ffrom the last run ConsumptionPhase.
        // It also includes helper functions to find how much shortfall there was.

        /// <summary>
        /// The average life need satisfaction of the pop.
        /// </summary>
        /// <returns>The Average of satisfaction of the needs.</returns>
        double AverageLifeSatisfaction();

        /// <summary>
        /// The average Daily need satisfaction of the pop.
        /// </summary>
        /// <returns>The Average of satisfaction of the needs.</returns>
        double AverageDailySatisfaction();

        /// <summary>
        /// The average Luxury need satisfaction of the pop.
        /// </summary>
        /// <returns>The Average of satisfaction of the needs.</returns>
        double AverageLuxurySatisfaction();

        /// <summary>
        /// The average Job satisfaction of the pop.
        /// </summary>
        /// <returns>The Average of satisfaction of the needs.</returns>
        double AverageJobInputSatisfaction();

        /// <summary>
        /// The average Capital need satisfaction for jobs.
        /// </summary>
        /// <returns>The Average of satisfaction of the needs.</returns>
        double AverageJobCapitalSatisfaction();

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

        #region JobSatisfaction

        /// <summary>
        /// The satisfaction of all the job inputs.
        /// </summary>
        IProductAmountCollection JobInputSatisfaction { get; }

        /// <summary>
        /// The satisfaction of all the job capital requirements.
        /// </summary>
        IProductAmountCollection JobCapitalSatisfaction { get; }

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

        /// <summary>
        /// Retrieve the Currency the population holds.
        /// </summary>
        /// <param name="Currencies">What counts as currency.</param>
        /// <returns></returns>
        IProductAmountCollection GetCash(IList<IProduct> Currencies);

        /// <summary>
        /// Get's the purchasing power of the population, returned in order of
        /// prefered buying order (most durable to least)
        /// </summary>
        /// <returns>The Items for sale in Descending chance of breaking.</returns>
        IProductAmountCollection PurchasingPower();

        /// <summary>
        /// The current profitability of the pop group.
        /// </summary>
        /// <param name="market">The current market, which holds prices.</param>
        /// <returns></returns>
        double Profitability(IMarket market);

        /// <summary>
        /// The entry cost to begin working the job.
        /// </summary>
        /// <param name="market"></param>
        /// <returns></returns>
        double EntryCost(IMarket market);

        /// <summary>
        /// The success of the pop group, measured in how many of it's needs
        /// are being met. Between 0 and 3.
        /// </summary>
        /// <returns>How successful the pop is.</returns>
        double Success();

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

        /// <summary>
        /// Get the price of a good by the pop
        /// </summary>
        /// <param name="good">The good to price.</param>
        /// <param name="v">The current market price of the good.</param>
        /// <returns>The population's price of the good.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="good"/> is null.</exception>
        double GetPrice(IProduct good, double v);

        /// <summary>
        /// Buys products based on the cash given to buy the good.
        /// </summary>
        /// <param name="cash">The cash to buy the goods.</param>
        /// <param name="good">The good being bought.</param>
        /// <param name="amount">The amount being bought.</param>
        /// <param name="market">The market we are buying in.</param>
        /// <returns>The resulting change in goods for the buyer.</returns>
        IProductAmountCollection BuyGood(IProductAmountCollection cash, IProduct good,
            double amount, IMarket market);

        /// <summary>
        /// Buys good via barter.
        /// </summary>
        /// <param name="buyerStock">The buyer's goods up for trade.</param>
        /// <param name="good">The good being traded for.</param>
        /// <param name="amount">The amount of the good being bought.</param>
        /// <param name="market">The market that the barter is taking place in.</param>
        /// <returns>The resulting change in goods for the buyer.</returns>
        IProductAmountCollection BarterGood(IProductAmountCollection buyerStock, IProduct good, double amount, IMarket market);

        /// <summary>
        /// Complete a transaction, adding what was bought, and removing what was paid.
        /// </summary>
        /// <param name="transaction">What to add and remove.</param>
        void CompleteTransaction(IProductAmountCollection transaction);

        /// <summary>
        /// Adds pops to popGroup
        /// </summary>
        /// <param name="born">The number of pops to add.</param>
        void AddPop(double born);

        #endregion Actions
    }
}