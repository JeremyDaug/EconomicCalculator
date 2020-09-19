﻿using System;
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

        private IProductAmountCollection _productSupply;

        /// <summary>
        /// What products are available, and how many are available.
        /// Positive values represent available goods and surplus.
        /// Negative Values represent Shortages.
        /// </summary>
        public IProductAmountCollection ProductSupply
        {
            get
            {
                return _productSupply;
            }
        }
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

        /*
         * How the Market Works: 
         * - The market works by simple procedures. Pops take what they have to 
         * produce what they can.
         * - Once goods are produced, they try to meet their needs for the day.
         * - Then taxes come out (if the must) this is either done on a day to day
         * bases (dividing yearly tax) or it is does periodically, with the tax
         * owed becoming debt (maybe? I need to think on this).
         * - Then the Sell Phase. Everyone takes what's left over from the day
         * after jobs and consumption, and puts it up what they don't need for
         * tomorrow up on the market. This allows everything to be ready for
         * purchase.
         * - Then we get to the Buy phase which has 3 portions.
         * - - First, local merchants buy up what they can from the market,
         * they buy more than they need for every step and put what they don't
         * need for tomorrow up on the market immediately at a higher price. 
         * They also buy the most in demand goods after they have met all of
         * their own needs, helping ensure a good outflow.
         * - - Then the general populace buy up goods. They buy from merchants
         * first (if possible) then the rest of the market. This goes in
         * Pop Priority order, meaning that (usually) the richest go first.
         * - - Then travelling merchants buy up the remains. They do this so
         * they can take it elsewhere to sell at a profit.
         * - After the buy phase we have the Pop Growth phase. Here, the pops
         * grow is size slightly, and if possible change to more profitable jobs.
         * - After all of this the price of goods change and alter to correct for
         * over or under production of goods relative to demand for the day.
         * - - Special Jobs:
         * Local Merchants: Local merchants are given first buy and sell options
         * on the market. They always sell above Standard market price to make a
         * profit. They also only buy and sell in coin, no goods.
         * Travelling Merchants: Travelling merchants are the last to buy from the
         * market, but sell alongside the local merchants, selling at or around
         * their price. Any goods they don't sell, they take to their next location.
         * Travelling Merchants also often sell to local merchants if they are able
         * to.
         * Money Changers: Money Changers are unique in that they are a service, but
         * they don't produce a labor to be consumed elsewhere.
         */

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
            // Sell what you don't need to make what you can.
            SellPhase();

            // Let Local Merchants buy what they can for the local market.
            LocalMerchantsBuy();

            // Let the market buy up what it can now.
            BuyPhase();

            // let the travelling merchants pick through what's left.
            TravellingMerchantPhase();

            // Population shifting phase

            // Price Correction Phase, where prices of goods are updated
        }

        public void SellPhase()
        {
            // Get all goods up for sale.
            _productSupply = Populations.SellPhase();

            // Fill out all
        }

        public void LocalMerchantsBuy()
        {
            // todo, 
        }

        /// <summary>
        /// Run through all purchasing that takes place.
        /// </summary>
        public void BuyPhase()
        {
            // go through each pop in order of priority
            foreach (var buyer in Populations.PopsByPriority)
            {
                // go through their list of needs
                foreach (var needPair in buyer.TotalNeeds)
                {
                    // Check we can keep going and there is stuff that the pop
                    // can trade for goods
                    if (buyer.ForSale.All(x => x.Item2 <= 0))
                        break;

                    // get the product and amount
                    var need = needPair.Item1;
                    var desired = needPair.Item2;

                    // If it's not in the Product Supply, add it at 0. It should be anyway.
                    if (!ProductSupply.Contains(need))
                    {
                        ProductSupply.IncludeProduct(need);
                    }

                    // Check if the product is available to buy, else just subtract and move on.
                    if (ProductSupply.GetProductValue(need) <= 0)
                    {
                        ProductSupply.SubtractProducts(need, desired);
                        continue;
                    }

                    // since it's available to buy go to the merchants Local first
                    // if they have any to sell
                    if (Populations.Merchants.ForSale.Contains(need))
                    {
                        // get the price from the merchants 
                        // Merchants.GetGoodPrice()
                        // Price defaults to twice for now.
                        var price = ProductPrices.GetProductValue(need) * 2 * desired;
                        // TODO we'll come back to this. Merchants are not done yet..
                    }

                    // there is more to buy, go to the rest.
                    if (desired > 0)
                    {
                        // get market price
                        var absPrice = ProductPrices.GetProductValue(need) * desired;

                        // try to buy what is needed going through each pop who's selling
                        // until you get what you need, or nothing is left.
                        foreach (var seller in Populations.GetPopsSellingProduct(need))
                        {
                            // whe know the seller is selling,
                            // so we get what we want or what they have
                            // whichever's higher
                            var available = Math.Min(desired, seller.ForSale.GetProductValue(need));

                            // with the amount we can buy, get the price
                            var price = GetPrice(need, available);

                            // With the price get the currency of the pop first, if available.
                            var cash = buyer.GetCash(AcceptedCurrencies);

                            // If there is any cash available, buy with cash
                            if (cash.Any(x => x.Item2 > 0))
                            {
                                var toBuy = ChangeForPrice(cash, price);
                            }

                            // else, just go to bartering.
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Given a set of cash and a desired price, get the amount of the cash
        /// to meet the price (rounded up for safety)
        /// </summary>
        /// <param name="AvailableCash">The amount of cash avaliable.</param>
        /// <param name="price">The price to meet (roughly)</param>
        /// <returns>The appropriate cash for the price.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="AvailableCash"/> is null.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="AvailableCash"/> contains a null product.</exception>
        /// 
        public IProductAmountCollection ChangeForPrice(IProductAmountCollection AvailableCash, double price)
        {
            // ensure cash is not null.
            if (AvailableCash is null)
                throw new ArgumentNullException(nameof(AvailableCash));
            // ensure nothing in cash is null
            if (AvailableCash.Any(x => x.Item1 is null))
                throw new ArgumentNullException("Prodcut in AvailableCash is null.");
            // ensure that the price is greater than 0
            if (price <= 0)
                throw new ArgumentOutOfRangeException("Price must be greater than 0.");

            // first, check that all available cash can meet the price.
            var totalCash = AvailableCash.Sum(x => x.Item2 * ProductPrices.GetProductValue(x.Item1));

            // If the total cash available is less than the price sought, return a copy of the available cash.
            if (totalCash < price)
                return AvailableCash.Copy();

            // since we have more than we need,
            var result = new ProductAmountCollection();

            // start from the best and start making change, highest to lowest value.
            foreach (var coin in AvailableCash.OrderByDescending(x => ProductPrices.GetProductValue(x.Item1)))
            {
                // if none of that coin exist
                if (coin.Item2 == 0)
                {
                    // add it as zero
                    result.AddProducts(coin.Item1, 0);
                    // and skip to the next loop
                    continue;
                }

                // coin type
                var curr = coin.Item1;
                // coin value
                var val = ProductPrices.GetProductValue(curr);
                // coin amount
                var amt = coin.Item2;

                // how many whole coins can fit into the price.
                var count = Math.Floor(price / val);

                // select cap coins at the available level.
                count = Math.Min(count, amt);

                // add to our change
                result.AddProducts(curr, count);

                // subtract the value we took out
                price -= val * count;
                // then go to the next coin.
            }

            // if there is a remainder
            if (price > 0)
            {
                // find the smallest coin available
                foreach (var coin in AvailableCash.OrderBy(x => ProductPrices.GetProductValue(x.Item1)))
                {
                    // if we have any available
                    if (result.GetProductValue(coin.Item1) < AvailableCash.GetProductValue(coin.Item1))
                    {
                        // add one 
                        result.AddProducts(coin.Item1, 1);

                        // and move on.
                        break;

                        // by logic, only one should be needed as we guaranteed have
                        // more value in coins then the requested price and the 
                        // remainder should be smaller than the smallest coin.
                    }
                }
            }

            // return the change.
            return result;
        }

        public void TravellingMerchantPhase()
        {
            // todo
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
        /// Travelling merchants are a unique group, and are split apart.
        /// </summary>
        public IList<IPopulationGroup> TravellingMerchants { get; }

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