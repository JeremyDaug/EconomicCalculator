using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconomicCalculator.Intermediaries;

namespace EconomicCalculator.Storage
{
    internal class Market : IMarket
    {
        /// <summary>
        /// The Unique Id of the market.
        /// </summary>
        public Guid Id { get; set; }

        #region GeneralInfo

        /// <summary>
        /// The name of the market.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Total Population in the market.
        /// Any population not in a job is considered unemployed.
        /// </summary>
        public double TotalPopulation { get; set; }

        /// <summary>
        /// The Territorial extent of the Market in acres. -1 means infinite.
        /// </summary>
        public int Territory { get; set; }

        #endregion GeneralInfo

        #region InfoDetails

        /// <summary>
        /// The population of the market and their breakdown.
        /// </summary>
        public IPopulations Populations { get; set; }

        // Territory Breakdown and management.

        #endregion InfoDetails

        #region TheMarket

        /// <summary>
        /// What products are available, and how many are available.
        /// Positive values represent available goods and surplus.
        /// Negative Values represent Shortages.
        /// </summary>
        public IProductAmountCollection ProductSupply { get; }

        /// <summary>
        /// The effective universal demands.
        /// Calculated based off of population needs and job inputs.
        /// </summary>
        public IProductAmountCollection ProductDemand { get; }

        /// <summary>
        /// The price of each product in Abstract units.
        /// </summary>
        public IProductAmountCollection ProductPrices { get; }

        /// <summary>
        /// The total production that can be done in a perfect day.
        /// </summary>
        public IProductAmountCollection ProductionCapacity { get; }

        /// <summary>
        /// What currencies are accepted in the area. 
        /// This isn't necissarily the tax currency.
        /// Only Money Changers can work in all currencies regardless of the market.
        /// The price of money can only change based on Money Changer Transactions.
        /// </summary>
        public IList<IProduct> AcceptedCurrencies { get; }

        /// <summary>
        /// Kickstarts the economy from absolutely nothing.
        /// </summary>
        public void Kickstart()
        {

        }

        /// <summary>
        /// Runs a market day through it's cycle.
        /// </summary>
        public void RunMarketDay()
        {
            // Work, consume goods to make goods.
            // Put first to allow for an easy kickstart, 
            // and ensure money to buy needs.
            ProductionPhase(); // Done, don't touch

            // consume what we need/want for the day
            ConsumptionPhase(); // Done, Don't touch

            // Loss Phase, where goods decay and breakdown randomly
            LossPhase();

            // Asset Tax Phase, for taxing assets.
            // Asset taxes are taken out in money first, then goods.
            // It sucks, but it needs to be done somewhere, here is best.

            // Offer what remains up to the market for the day.
            // Sell it all, don't think about tomorrow.
            SellPhase();

            // Let Local Merchants buy what they can for the local market.
            // LocalMerchantsBuy(); Todo

            // Let the market buy up what it can now.
            BuyPhase();

            // let the travelling merchants pick through what's left.
            // TravellingMerchantPhase(); todo
        }

        public void SellPhase()
        {
            // For all pops, put everything they have stored on the market.
            foreach (var pop in Populations.Pops)
            {
                pop.UpForSale();
            }
        }

        /// <summary>
        /// Run through all purchasing that takes place.
        /// </summary>
        public void BuyPhase()
        {
            
        }

        /// <summary>
        /// Run through all productions that can be done and add them to the market.
        /// </summary>
        public void ProductionPhase()
        {
            Populations.ProductionPhase();
        }

        public void ConsumptionPhase()
        {
            // Kick down to populations.
            Populations.Consume();
        }

        public void LossPhase()
        {
            var losses = Populations.LossPhase();
        }

        /// <summary>
        /// Readjust and Recalulate Prices.
        /// </summary>
        public void RecalculatePrices()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a price for a good.
        /// </summary>
        /// <param name="product">The product we are pricing</param>
        /// <param name="amount">How much are we getting.</param>
        /// <returns>The price in abstract currency.</returns>
        /// <exception cref="ArgumentNullException">
        /// If Product is null.
        /// </exception>
        public double GetPrice(IProduct product, double amount)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Buys good from the market.
        /// </summary>
        /// <param name="product">The product to buy.</param>
        /// <param name="amount">How many units to buy.</param>
        /// <param name="sellable">The available products to trade for the product.</param>
        /// <returns>The change in <paramref name="sellable"/> items, plus the amount of good bought.</returns>
        /// <remarks>Buying goods prioritizes using currency to buy over barter of goods.</remarks>
        public IProductAmountCollection BuyGood(IProduct product, double amount, IProductAmountCollection sellable)
        {
            throw new NotImplementedException();
        }

        #endregion TheMarket

        #region AvailableOptions

        // Crops
        // Mines
        // Processes
        // Accepted Currencies

        #endregion AvailableOptions

        #region PracticalShortcuts

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
        public double ExchangeCash(IProduct source, double amount, IProduct target)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Quick Access to a Money Changer.
        /// </summary>
        public IPopulationGroup MoneyChangers { get; }

        /// <summary>
        /// Quick access to Merchants in the market (those who don't produce, only
        /// buy and sell goods.
        /// </summary>
        public IPopulationGroup Merchants { get; }

        #endregion

        #region PrintFunctions

        /// <summary>
        /// Prints the mines in the market.
        /// </summary>
        public string PrintMines()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Prints the crops
        /// </summary>
        public string PrintCrops()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Prints the Processes
        /// </summary>
        public string PrintProcesses()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Prints the Currencies
        /// </summary>
        public string PrintCurrencies()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Prints the Populations
        /// </summary>
        public string PrintPops()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Prints the Products
        /// </summary>
        public string PrintProducts()
        {
            throw new NotImplementedException();
        }

        #endregion PrintFunctions

        public bool Equals(IMarket other)
        {
            return Id == other.Id;
        }
    }
}
