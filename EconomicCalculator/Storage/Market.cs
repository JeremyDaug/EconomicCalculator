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
        public double Territory { get; set; } // not actually used yet.

        #endregion GeneralInfo

        #region InfoDetails

        /// <summary>
        /// The population of the market and their breakdown.
        /// </summary>
        public IPopulations Populations { get; set; }

        // Territory Breakdown and management.

        #endregion InfoDetails

        public Market()
        {
            ProductPrices = new ProductAmountCollection();
            Shortfall = new ProductAmountCollection();
            _surplus = new ProductAmountCollection();
            _productSupply = new ProductAmountCollection();
            _purchasedGoods = new ProductAmountCollection();
            _productDemand = new ProductAmountCollection();
        }

        #region TheMarket

        private IProductAmountCollection _productSupply;
        private IProductAmountCollection _purchasedGoods;
        private IProductAmountCollection _surplus;
        private IProductAmountCollection _productDemand;
        private IProductAmountCollection _productionCapacity;

        /// <summary>
        /// What goods were up for sale today.
        /// </summary>
        public IProductAmountCollection ProductSupply
        {
            get
            {
                return _productSupply;
            }
        }

        /// <summary>
        /// How many goods were bought throughout the day.
        /// </summary>
        public IProductAmountCollection PurchasedGoods
        {
            get
            {
                return _purchasedGoods;
            }
        }

        /// <summary>
        /// The desired goods that couldn't be gotten due to lack of supply.
        /// </summary>
        public IProductAmountCollection Shortfall { get; set; }

        /// <summary>
        /// Of what was put up for sale today, how much remains.
        /// </summary>
        public IProductAmountCollection Surplus
        {
            get
            {
                return _surplus;
            }
        }

        /// <summary>
        /// The total demands of all pops.
        /// </summary>
        public IProductAmountCollection ProductDemand
        {
            get
            {
                return _productDemand;
            }
        }

        /// <summary>
        /// The price of each product in Abstract units.
        /// </summary>
        public IProductAmountCollection ProductPrices { get; }

        /// <summary>
        /// The total production that can be done in a perfect day.
        /// </summary>
        public IProductAmountCollection ProductionCapacity => _productionCapacity;

        /// <summary>
        /// The official currencies of the location, may be empty.
        /// </summary>
        public IList<IProduct> OfficialCurrencies { get; set; }

        /// <summary>
        /// What currencies are accepted in the area. 
        /// This isn't necissarily the tax currency.
        /// Only Money Changers can work in all currencies regardless of the market.
        /// The price of money can only change based on Money Changer Transactions.
        /// The point at which currency becomes acceptes is when there is more of
        /// that currency than others.
        /// </summary>
        public IList<IProduct> AcceptedCurrencies { get; set; }

        /// <summary>
        /// Get's the Value of the currencies in the market.
        /// </summary>
        /// <returns>all currencies and their market value.</returns>
        public IProductAmountCollection CurrencyValues()
        {
            var result = new ProductAmountCollection();

            // for each coin
            foreach (var coin in AcceptedCurrencies)
            {
                // get add that coin and it's current market price to the result.
                result.AddProducts(coin, GetPrice(coin));
            }

            // and return it.
            return result;
        }

        /// <summary>
        /// Kickstarts the economy from absolutely nothing.
        /// </summary>
        public void Kickstart()
        {

        }

        /* How the Market Works: 
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
            PopChanges();

            // Price Correction Phase, where prices of goods are updated
            RecalculatePrices();
        }

        public void PopChanges()
        {
            // get the growth rate from Populations
            var popGrowth = Populations.PopGrowthRate;

            // get the number of new pops, min 1 expected.
            var newPops = Math.Min(TotalPopulation * popGrowth, 1);

            // Create a storage for pop success
            var popSuccess = new Dictionary<Guid, double>();

            // for each pop, check how successful/profitable they are.
            foreach (var pop in Populations.Pops)
            {
                // get the pop's id for use.
                var id = pop.Id;

                // how satisfied is the pop's needs
                double lifeSat = pop.AverageLifeSatisfaction();
                double jobSat = pop.AverageJobSatisfaction();
                double dailySat = pop.AverageDailySatisfaction();
                double LuxSat = pop.AverageLuxurySatisfaction();

                // Sum all satisfaction to find how much people want to be there.
                // Subtract 1 to really hammer home the jobs that aren't even meeting their basic needs.
                var totalSuccess = lifeSat + jobSat + dailySat + LuxSat - 1;

                // add that to our resulting selection.
                popSuccess[id] = totalSuccess;

                // TODO !! pick up here !!
            }
        }

        public void SellPhase()
        {
            // Get all goods up for sale.
            _productSupply = Populations.SellPhase();

            // Reset Shortfall to zero.
            Shortfall = new ProductAmountCollection();

            // Fill Surplus preemtively, we'll remove bought products later.
            _surplus = ProductSupply.Copy();

            // While we're at it, also get total demand of all products
            _productDemand = Populations.TotalDemand();

            // And the hypothetical total production available.
            _productionCapacity = Populations.TotalProduction();
        }

        public void LocalMerchantsBuy()
        {
            // Hold off on this section. Let's focus on just sellers first.
            throw new NotImplementedException();
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
                    // Check that there is stuff that the pop
                    // can trade for goods
                    if (buyer.ForSale.All(x => x.Item2 <= 0))
                        break; // if they don't they stop

                    // get the product and amount
                    var need = needPair.Item1;
                    var desired = needPair.Item2; // the units desired

                    try
                    {
                        // Check if the product is available to buy
                        if (ProductSupply.GetProductValue(need) <= 0)
                        {
                            // if nothing is available, add it to the shortfall.
                            Shortfall.AddProducts(need, desired);
                            continue; // and go to the next need
                        }
                    }
                    catch (KeyNotFoundException) // If it doesn't exist in the supply at all.
                    {
                        // Add it in at 0
                        ProductSupply.IncludeProduct(need);
                        // Subtract the missing product from shortfall.
                        Shortfall.AddProducts(need, desired);
                        continue; // and skip it here.
                    }

                    // Go to the market and buy what we can.
                    var reciept = BuyGoodsFromMarket(buyer, need, desired);

                    // process our reciept, getting how satisfied our need was.
                    var satisfaction = reciept.GetProductValue(need);

                    // Add satisfaction to our purchased good recorder
                    _purchasedGoods.AddProducts(need, satisfaction);

                    // Remove bought good from supply
                    ProductSupply.SubtractProducts(need, satisfaction);

                    // get the number of goods that couldn't be bought.
                    var shortfall = desired - satisfaction;

                    // add that to shortfall
                    Shortfall.AddProducts(need, shortfall);
                }
            }

            // with all buying done, get surplus supply available by removing bought goods.
            _surplus.AddProducts(PurchasedGoods.Multiply(-1));
        }

        /// <summary>
        /// Buys good from the market.
        /// </summary>
        /// <param name="buyer">The one buying the good.</param>
        /// <param name="good">The good they are trying to buy.</param>
        /// <param name="amount">How much they are trying to buy.</param>
        /// <returns>The Receipt of purchases</returns>
        public IProductAmountCollection BuyGoodsFromMarket(IPopulationGroup buyer,
            IProduct good, double amount)
        {
            if (buyer is null)
                throw new ArgumentNullException(nameof(buyer));
            if (good is null)
                throw new ArgumentNullException(nameof(good));
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            // The result of the purchases.
            IProductAmountCollection result = new ProductAmountCollection();

            // First buy from local merchants, they only accept cash.
            result = Populations.Merchants
                .BuyGood(buyer.GetCash(AcceptedCurrencies), good, amount, this);

            // see how much was bought.
            double remainder = 0;

            // if any was bought, update what we are seeking.
            if (result.Contains(good))
            {
                remainder = amount - result.GetProductValue(good);
                // and complete the transaction
                buyer.CompleteTransaction(result);
            }

            // if no remainder, return
            if (remainder <= 0)
                return result;

            // Then buy from everyone else via both cash and barter.
            foreach (var seller in Populations.GetPopsSellingProduct(good))
            {
                // If someone is selling the good, buy or barter with them.
                var reciept = BuyGoods(buyer, good, remainder, seller);

                // if something was bought
                if (reciept.Count() > 0)
                {
                    // remove it from the remainder
                    remainder -= reciept.GetProductValue(good);

                    // add it to our result
                    result.AddProducts(reciept);
                }

                // if nothing remains, we're done.
                if (remainder <= 0)
                    return result;
            }

            // Finish buy going to the travelling merchants, if all else fails.
            foreach (var travSeller in TravellingMerchantsSelling(good))
            {
                var reciept = travSeller.BuyGood(buyer.GetCash(AcceptedCurrencies), good, remainder, this);

                // if something was bought
                if (reciept.Count() > 0)
                {
                    // remove it from remainder
                    remainder -= reciept.GetProductValue(good);

                    // Complete the transaction for the buyer
                    buyer.CompleteTransaction(reciept);

                    // add it to the result
                    result.AddProducts(reciept);
                }

                // if nothing remains, return
                if (remainder <= 0)
                    return result;
            }

            // return the ultimate receipt.
            return result;
        }

        public IEnumerable<IPopulationGroup> TravellingMerchantsSelling(IProduct good)
        {
            if (good is null)
                throw new ArgumentNullException(nameof(good));

            // TODO, this isn't tested fully, but this is just a shortcut.
            return TravellingMerchants.Where(x => x.ForSale.Contains(good));
        }

        public IProductAmountCollection BuyGoods(IPopulationGroup buyer, IProduct good, double amount,
            IPopulationGroup seller)
        {
            if (buyer is null)
                throw new ArgumentNullException(nameof(buyer));
            if (good is null)
                throw new ArgumentNullException(nameof(good));
            if (seller is null)
                throw new ArgumentNullException(nameof(seller));
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            // get the cash we have available.
            var cash = buyer.GetCash(AcceptedCurrencies);

            var result = new ProductAmountCollection();

            // if we have any cash, try to buy with that first.
            if (cash.Any(x => x.Item2 > 0))
            {
                // Buy what we can with our cash.
                var transaction = seller.BuyGood(cash, good, amount, this);

                // With our transaction initiated, complete it on the buyer's end.
                buyer.CompleteTransaction(transaction);

                // Add the transaction to our return value
                result.AddProducts(transaction);

                // Update our desired amount
                amount -= transaction.GetProductValue(good);
            }

            // if we still have more to buy, it means we are out of cash. Begin bartering.
            // check we have things to barter.
            if (amount > 0 && buyer.ForSale.Any(x => x.Item2 > 0) && BarterLegal)
            {
                // Begin Bartering
                var barter = seller.BarterGood(buyer.ForSale, good, amount, this);

                // with the barter complete, finish for buyer.
                buyer.CompleteTransaction(barter);

                // add the transaction to the result
                result.AddProducts(barter);
            }

            // we've bought what we could from the pop, so return.
            return result;
        }

        /// <summary>
        /// Given a set of cash and a desired price, get the amount of the cash
        /// to meet the price (rounded up for safety)
        /// </summary>
        /// <param name="AvailableCash">The amount of cash avaliable.</param>
        /// <param name="price">The price to meet (roughly)</param>
        /// <returns>The appropriate cash for the price.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="AvailableCash"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="price"/> is less than or equal to 0.</exception>
        public IProductAmountCollection ChangeForPrice(IProductAmountCollection AvailableCash, double price)
        {
            // ensure cash is not null.
            if (AvailableCash is null)
                throw new ArgumentNullException(nameof(AvailableCash));
            // ensure that the price is greater than 0
            if (price <= 0) // TODO, allow this to savely give change for 0. It's not that hard.
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
                // if value is higher then remaining price.
                if (val > price)
                {
                    // add it as zero
                    result.AddProducts(curr, 0);
                    // and skip
                    continue;
                }

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
            // Run through production
            Populations.ProductionPhase();
        }

        public void ConsumptionPhase()
        {
            // Kick down to populations.
            Populations.Consume();
        }

        public void LossPhase()
        {
            // Carry out losses.
            var losses = Populations.LossPhase();
        }

        /// <summary>
        /// Readjust and Recalulate Prices.
        /// </summary>
        public void RecalculatePrices()
        {
            // TODO Allow for this to be modified by variations
            // TODO Create a way to allow for faster Price changes, when supply/demand differences are large.
            // for each product in the market.
            foreach (var pair in ProductPrices)
            {
                // Get the product
                var product = pair.Item1;

                double surplus;
                double shortfall;

                try
                {
                    // get surplus product not spent
                    surplus = Surplus.GetProductValue(product);

                    // get product that was desired to buy.
                    shortfall = Shortfall.GetProductValue(product);
                }
                catch (KeyNotFoundException)
                {
                    // if the item does not exist in surplus or shortfal, then it probably was not
                    // sold or desired in the market. Give it a boost to denote it's rarity, and try and encourage it.
                    ProductPrices.AddProducts(product, 0.01);
                    continue;
                }
                // the amount of change to make to the good's price.
                double priceChange = 0;

                // If any surplus and shortfall exists, price was too high
                if (surplus > 0 && shortfall > 0)
                {
                    priceChange += -0.01;
                }
                else if (surplus > 0)
                { 
                    // If no shortfall but still surplus, try lowering price to sell it, oversupply is not good.
                    priceChange += -0.01;
                }
                else if (shortfall > 0)
                { // if shortfall but no surplus, price is too low.
                    priceChange += 0.01;
                }
                // In no surplus nor shortfall, then we have hit equilibrium.
                // No change in price.

                // add the change in price to the new price
                // TODO make this more flexible and reactive.
                // going in 0.01 ABS price unit sized steps is too small
                // and may make prices too stagnant
                var newPrice = ProductPrices.GetProductValue(product) + priceChange;

                // update to said price.
                ProductPrices.SetProductAmount(product, newPrice);
            }
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
            if (product is null)
                throw new ArgumentNullException(nameof(product));

            return ProductPrices.GetProductValue(product) * amount;
        }

        /// <summary>
        /// Gets a price for a good.
        /// </summary>
        /// <param name="product">The product we are pricing</param>
        /// <returns>The price in abstract currency.</returns>
        /// <exception cref="ArgumentNullException">
        /// If Product is null.
        /// </exception>
        public double GetPrice(IProduct product)
        {
            if (product is null)
                throw new ArgumentNullException(nameof(product));

            return ProductPrices.GetProductValue(product);
        }

        /// <summary>
        /// The external option to buy from the market.
        /// </summary>
        /// <param name="product">The product to buy.</param>
        /// <param name="amount">How many units to buy.</param>
        /// <param name="sellable">The available products to trade for the product.</param>
        /// <returns>The change in <paramref name="sellable"/> items, plus the amount of good bought.</returns>
        /// <remarks>Buying goods prioritizes using currency to buy over barter of goods.</remarks>
        public IProductAmountCollection BuyGood(IProduct product, double amount, IProductAmountCollection cash)
        {
            throw new NotImplementedException();
        }

        #endregion TheMarket

        #region PracticalShortcuts

        /// <summary>
        /// Get's the minimal cost for the item in the current market,
        /// assuming the inputs (but not capital) was bought.
        /// </summary>
        /// <param name="product">The product we want the pure cost of.</param>
        /// <returns>The current minimal market cost.</returns>
        public double ProductMinimalCost(IProduct product)
        {
            throw new NotImplementedException();
        }

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
        public IPopulationGroup MoneyChangers { get; set; }

        /// <summary>
        /// Travelling merchants are a unique group, and are split apart.
        /// </summary>
        public IList<IPopulationGroup> TravellingMerchants { get; set; }

        /// <summary>
        /// Whether Barter is legal or not in the market.
        /// </summary>
        public bool BarterLegal { get; set; }

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