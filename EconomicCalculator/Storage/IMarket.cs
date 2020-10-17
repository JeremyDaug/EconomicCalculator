using EconomicCalculator.Generators;
using EconomicCalculator.Intermediaries;
using System;
using System.Collections.Generic;

namespace EconomicCalculator.Storage

{
    /// <summary>
    /// The Market Interface, stores information about markets.
    /// </summary>
    public interface IMarket : IEquatable<IMarket>
    {
        /// <summary>
        /// The Unique Id of the market.
        /// </summary>
        Guid Id { get; }

        #region GeneralInfo

        /// <summary>
        /// The name of the market.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The Total Population in the market.
        /// Any population not in a job is considered unemployed.
        /// </summary>
        double TotalPopulation { get; }

        /// <summary>
        /// The Territorial extent of the Market in acres. -1 means infinite.
        /// </summary>
        double Territory { get; }

        #endregion GeneralInfo

        #region LegalInfo

        /// <summary>
        /// Whether Bartering is legal or not.
        /// </summary>
        bool BarterLegal { get; }

        #endregion LegalInfo

        #region InfoDetails

        /// <summary>
        /// The population of the market and their breakdown.
        /// </summary>
        IPopulations Populous { get; }

        // Territory Breakdown and management.

        #endregion InfoDetails

        #region TheMarket

        /// <summary>
        /// What products are available, and how many are available.
        /// Positive values represent available goods and surplus.
        /// Negative Values represent Shortages.
        /// </summary>
        IProductAmountCollection ProductSupply { get; }

        /// <summary>
        /// The effective universal demands.
        /// Calculated based off of population needs and job inputs.
        /// </summary>
        IProductAmountCollection ProductDemand { get; }

        /// <summary>
        /// The price of each product in absolute units.
        /// </summary>
        IProductAmountCollection ProductPrices { get; }

        /// <summary>
        /// The total production that can be done in a perfect day.
        /// </summary>
        IProductAmountCollection ProductionCapacity { get; }

        /// <summary>
        /// What currencies are accepted in the area. 
        /// This isn't necissarily the tax currency.
        /// Only Money Changers can work in all currencies regardless of the market.
        /// The price of money can only change based on Money Changer Transactions.
        /// </summary>
        IList<IProduct> AcceptedCurrencies { get; }

        /// <summary>
        /// Get's the Value of the currencies in the market.
        /// </summary>
        /// <returns>all currencies and their market value.</returns>
        IProductAmountCollection CurrencyValues();

        /// <summary>
        /// Runs a market day through it's cycle.
        /// </summary>
        void RunMarketDay();

        /// <summary>
        /// Run through all productions that can be done and add them to the market.
        /// </summary>
        void ProductionPhase();

        /// <summary>
        /// Run through all purchasing that takes place.
        /// </summary>
        void BuyPhase();

        /// <summary>
        /// Readjust and Recalulate Prices.
        /// </summary>
        void RecalculatePrices();

        /// <summary>
        /// Gets a price for a good.
        /// </summary>
        /// <param name="product">The product we are pricing</param>
        /// <param name="amount">How much are we getting.</param>
        /// <returns>The price in abstract currency.</returns>
        /// <exception cref="ArgumentNullException">
        /// If Product is null.
        /// </exception>
        double GetPrice(IProduct product, double amount);

        /// <summary>
        /// Gets a price for a good.
        /// </summary>
        /// <param name="product">The product we are pricing</param>
        /// <param name="amount">How much are we getting.</param>
        /// <returns>The price in abstract currency.</returns>
        /// <exception cref="ArgumentNullException">
        /// If Product is null.
        /// </exception>
        double GetPrice(IProduct good);

        /// <summary>
        /// Buys good from the market.
        /// </summary>
        /// <param name="product">The product to buy.</param>
        /// <param name="amount">How many units to buy.</param>
        /// <param name="sellable">The available products to trade for the product.</param>
        /// <returns>The change in <paramref name="sellable"/> items, plus the amount of good bought.</returns>
        /// <remarks>Buying goods prioritizes using currency to buy over barter of goods.</remarks>
        IProductAmountCollection BuyGood(IProduct product, double amount, IProductAmountCollection sellable);

        #endregion TheMarket

        #region AvailableOptions

        // Crops
        // Mines
        // Processes
        // Accepted Currencies

        #endregion AvailableOptions

        #region PracticalShortcuts

        /// <summary>
        /// Given a set of cash and a desired price, get the amount of the cash
        /// to meet the price (rounded up for safety)
        /// </summary>
        /// <param name="AvailableCash">The amount of cash avaliable.</param>
        /// <param name="price">The price to meet (roughly)</param>
        /// <returns>The appropriate cash for the price.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="AvailableCash"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="price"/> is less than or equal to 0.</exception>
        IProductAmountCollection ChangeForPrice(IProductAmountCollection AvailableCash, double price);

        /// <summary>
        /// Function to Exchange Cash for an alternative via money changers.
        /// </summary>
        /// <param name="cash">The product being exchanged.</param>
        /// <param name="amount">How much is being exchanged.</param>
        /// <param name="target">What is being sought.</param>
        /// <returns>The number of target currency recieved.</returns>
        /// <exception cref="InvalidOperationException">
        /// If the source or target moneys are invalid for exchange.
        /// </exception>
        /// <remarks>
        /// May return 0 if no currency can be exchanged.
        /// </remarks>
        double ExchangeCash(IProduct source, double amount, IProduct target);

        /// <summary>
        /// Quick Access to a Money Changer.
        /// </summary>
        IPopulationGroup MoneyChangers { get; }

        #endregion

        #region PrintFunctions

        /// <summary>
        /// Prints the mines in the market.
        /// </summary>
        string PrintMines();

        /// <summary>
        /// Prints the crops
        /// </summary>
        string PrintCrops();

        /// <summary>
        /// Prints the Processes
        /// </summary>
        string PrintProcesses();

        /// <summary>
        /// Prints the Currencies
        /// </summary>
        string PrintCurrencies();

        /// <summary>
        /// Prints the Populations
        /// </summary>
        string PrintPops();

        /// <summary>
        /// Prints the Products
        /// </summary>
        string PrintProducts();

        #endregion PrintFunctions
    }
}
