using EconomicCalculator.Storage.Jobs;
using EconomicCalculator.Storage.Products;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage
{
    internal class PopulationGroup : IPopulationGroup
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public double Count { get; set; }

        public double PopGrowthRate { get; set; }

        public IJob PrimaryJob { get; set; }

        public int Priority { get; set; }

        public IProductAmountCollection LifeNeeds { get; set; }

        public IProductAmountCollection DailyNeeds { get; set; }

        public IProductAmountCollection LuxuryNeeds { get; set; }

        public IList<IJob> SecondaryJobs { get; set; }

        /// <summary>
        /// The total available storage bulk available.
        /// Calculated from land and buildings owned.
        /// </summary>
        public double TotalAvailableStorage
        {
            get
            {
                return AvailableStorageDetails.Sum(x => x.Value);
            }
        }

        /// <summary>
        /// The breakdown of storage by specialization.
        /// Only 3 kinds exist. Unspecialized, Cold, and Capital.
        /// Unspecialized can be used for anything, but doesn't alter decay.
        /// Cold Slows decay for those that need it. 
        /// Capital is active storage where the capital good can be used.
        /// Calculated from owned land and buildings.
        /// Update on buy and sell.
        /// </summary>
        public IDictionary<string, double> AvailableStorageDetails { get; }

        /// <summary>
        /// Products currently stored by the Population.
        /// Should never have a negative storage value.
        /// </summary>
        public IProductAmountCollection Storage { get; set; }

        public IProductAmountCollection ForSale { get; set; }

        public string SkillName { get; set; }

        public int SkillLevel { get; set; }

        public IProduct JobLabor { get; set; }

        public PopulationGroup()
        {
            Id = Guid.NewGuid();
            SecondaryJobs = new List<IJob>();
            AvailableStorageDetails = new Dictionary<string, double>();
            Storage = new ProductAmountCollection();
            LifeSatisfaction = new ProductAmountCollection();
            DailySatisfaction = new ProductAmountCollection();
            LuxurySatisfaction = new ProductAmountCollection();
            JobInputSatisfaction = new ProductAmountCollection();
            JobCapitalSatisfaction = new ProductAmountCollection();
        }

        #region Helper

        public IProductAmountCollection TotalNeeds
        {
            get
            {
                var result = LifeNeeds.Multiply(Count);
                result.AddProducts(PrimaryJob.Capital.Multiply(Count));
                result.AddProducts(PrimaryJob.Inputs.Multiply(Count));
                result.AddProducts(DailyNeeds.Multiply(Count));
                result.AddProducts(LuxuryNeeds.Multiply(Count));
                return result;
            }
        }

        /// <summary>
        /// Initializes the storage to ensure it includes all products from 
        /// Needs, and job. Run after any change to needs or jobs.
        /// </summary>
        /// <remarks>
        /// If this is used, then we can assume that all actions related to
        /// needs or job products are included in storage if nothing else.
        /// </remarks>
        public void InitializeStorage()
        {
            Storage.IncludeProducts(LifeNeeds.Products.ToList());
            Storage.IncludeProducts(DailyNeeds.Products.ToList());
            Storage.IncludeProducts(LuxuryNeeds.Products.ToList());
            Storage.IncludeProducts(PrimaryJob.Inputs.Products.ToList());
            Storage.IncludeProducts(PrimaryJob.Capital.Products.ToList());
            Storage.IncludeProducts(PrimaryJob.Outputs.Products.ToList());
            Storage.IncludeProduct(JobLabor);
            // Placeholder for secondary jobs.
        }

        #endregion Helper

        #region PastSatisfaction
        // These values represent the satisfaction of the pop ffrom the last run ConsumptionPhase.
        // It also includes helper functions to find how much shortfall there was.

        /// <summary>
        /// The average life need satisfaction of the pop.
        /// </summary>
        /// <returns>The Average of satisfaction of the needs.</returns>
        public double AverageLifeSatisfaction()
        {
            return LifeSatisfaction.Average(x => x.Item2);
        }

        /// <summary>
        /// The average Daily need satisfaction of the pop.
        /// </summary>
        /// <returns>The Average of satisfaction of the needs.</returns>
        public double AverageDailySatisfaction()
        {
            return DailySatisfaction.Average(x => x.Item2);
        }

        /// <summary>
        /// The average Luxury need satisfaction of the pop.
        /// </summary>
        /// <returns>The Average of satisfaction of the needs.</returns>
        public double AverageLuxurySatisfaction()
        {
            return LuxurySatisfaction.Average(x => x.Item2);
        }

        /// <summary>
        /// The average Job satisfaction of the pop.
        /// </summary>
        /// <returns>The Average of satisfaction of the needs.</returns>
        public double AverageJobSatisfaction()
        { // May split this into input and capital satisfaction.
            var combo = JobInputSatisfaction.Copy();
            combo.AddProducts(JobCapitalSatisfaction);
            return combo.Average(x => x.Item2);
        }

        /// <summary>
        /// The satisfaction of the population's life needs.
        /// </summary>
        public IProductAmountCollection LifeSatisfaction { get; set; }

        /// <summary>
        /// The satisfaction of the population's daily needs.
        /// </summary>
        public IProductAmountCollection DailySatisfaction { get; set; }

        /// <summary>
        /// The satisfaction of the population's luxury needs.
        /// </summary>
        public IProductAmountCollection LuxurySatisfaction { get; set; }

        /// <summary>
        /// The life need shortfall (if any).
        /// </summary>
        public IProductAmountCollection LifeShortfall
        {
            get
            {
                return LifeNeeds.MultiplyBy(LifeSatisfaction).Multiply(Count);
            }
        }

        /// <summary>
        /// The Daily need shortfall (if any).
        /// </summary>
        public IProductAmountCollection DailyShortfall
        {
            get
            {
                return DailyNeeds.MultiplyBy(DailySatisfaction).Multiply(Count);
            }
        }

        /// <summary>
        /// The Daily need shortfall (if any).
        /// </summary>
        public IProductAmountCollection LuxuryShortfall
        {
            get
            {
                return LuxuryNeeds.MultiplyBy(LuxurySatisfaction).Multiply(Count);
            }
        }

        #region JobSatisfaction

        /// <summary>
        /// The satisfaction of all the job inputs.
        /// </summary>
        public IProductAmountCollection JobInputSatisfaction { get; }

        /// <summary>
        /// The satisfaction of all the job capital requirements.
        /// </summary>
        public IProductAmountCollection JobCapitalSatisfaction { get; }

        /// <summary>
        /// The Job Input shortfall (if any).
        /// </summary>
        public IProductAmountCollection JobInputShortfall
        {
            get
            {
                return PrimaryJob.Inputs.MultiplyBy(JobInputSatisfaction).Multiply(Count);
            }
        }

        /// <summary>
        /// The Job Input shortfall (if any).
        /// </summary>
        public IProductAmountCollection JobCapitalShortfall
        {
            get
            {
                return PrimaryJob.Capital.MultiplyBy(JobCapitalSatisfaction).Multiply(Count);
            }
        }

        #endregion JobSatisfaction

        #endregion PastSatisfaction

        #region Actions

        /// <summary>
        /// The Production Phase of the population.
        /// </summary>
        /// <returns>The change in products because of the phase.</returns>
        public IProductAmountCollection ProductionPhase()
        {
            // Assuming perfection, get full needs expected.
            // inputs times the population divided by the labor needed for the job.
            var requirements = PrimaryJob.DailyInputNeedsForPops(Count);
            // Get Capital Requirements (only one needed per day of labor)
            requirements.AddProducts(PrimaryJob.CapitalNeedsForPops(Count));

            // With expected inputs, see what we can actually satisfy.
            double sat = 1;
            foreach (var pair in requirements)
            {
                // get product and amount
                var product = pair.Item1;
                var amount = pair.Item2;

                // check if current satisfaction is less than the stored amount of the product
                // the product should be there BC of InitializeStorage
                sat = Math.Min(sat, Storage.GetProductValue(product) / amount);
            }

            // If something cannot be satisfied, we GTFO.
            if (sat == 0)
                return new ProductAmountCollection();

            // With Satisfaction, get and consume the inputs.
            var inputs = PrimaryJob.Inputs.Multiply(Count / PrimaryJob.LaborRequirements * sat);
            ConsumeGoods(inputs, JobInputSatisfaction);
            // Add what was consumed to the result.
            var result = new ProductAmountCollection();
            result.AddProducts(inputs.Multiply(-1));

            // Get the resulting outputs
            var outputs = PrimaryJob.Outputs.Multiply(Count / PrimaryJob.LaborRequirements * sat);
            Storage.AddProducts(outputs);
            result.AddProducts(outputs);

            // Calculate remaining labor and add that to results
            if (sat < 1)
            {
                var remainder = sat * Count;

                result.AddProducts(JobLabor, remainder);
                Storage.AddProducts(JobLabor, remainder);
            }

            // Don't run breakdown of capital here, run it alongside all other breakdown chances.

            // return results
            return result;
        }

        /// <summary>
        /// The Consumption action of the population group.
        /// </summary>
        /// <returns>The resulting change in goods.</returns>
        public IProductAmountCollection ConsumptionPhase()
        {
            // life needs first
            var result = ConsumeGoods(LifeNeeds.Multiply(Count), LifeSatisfaction);

            // then daily needs
            result.AddProducts(ConsumeGoods(DailyNeeds.Multiply(Count), DailySatisfaction));

            // then luxuries
            result.AddProducts(ConsumeGoods(LuxuryNeeds.Multiply(Count), LuxurySatisfaction));

            // then return satisfaction.
            return result;
        }

        /// <summary>
        /// Consumes the given set of goods.
        /// </summary>
        /// <param name="goods">The goods to attempt to consume.</param>
        /// <param name="satisfaction">The satisfaction we'll be filling out as we go.</param>
        /// <returns>The change in products stored.</returns>
        private IProductAmountCollection ConsumeGoods(IProductAmountCollection goods,
            IProductAmountCollection satisfaction)
        {
            var result = new ProductAmountCollection();

            // for each good to consume.
            foreach (var pair in goods)
            {
                // Get the item and amount of the person.
                var product = pair.Item1;
                var amount = pair.Item2;

                // Assume All items being consumed are in storage already,
                // if they aren't we have a consistency problem.
                // get the satisfaction, capping it at 1.
                var sat = Math.Min(1, Storage.GetProductValue(product) / amount);

                // If satisfaction can't be met, subtract what you can.
                if (sat < 1)
                {
                    result.SubtractProducts(product,
                        Storage.GetProductValue(product));

                    Storage.SubtractProducts(product,
                        Storage.GetProductValue(product));
                }
                else // If greater than 1, then substract everything needed.
                {
                    result.SubtractProducts(product, amount);

                    Storage.SubtractProducts(product, amount);
                }

                // Finally, set it's satisfaction.
                satisfaction.SetProductAmount(product, sat);
            }

            // Return change in products stored.
            return result;
        }

        public IProductAmountCollection LossPhase()
        {
            var result = new ProductAmountCollection();

            // for each item in storage
            foreach (var pair in Storage)
            {
                var product = pair.Item1;
                var amount = pair.Item2;

                // get a random amount of failure
                var failedAmount = product.FailedProducts(amount);

                if (failedAmount == 0)
                    continue;

                // remove the failed items from storage
                Storage.SubtractProducts(product, failedAmount);

                // Subtract the removed itmes from result.
                result.AddProducts(product, -failedAmount);
            }

            return result;
        }

        public IProductAmountCollection UpForSale()
        {
            // Clear old sale space just in case.
            ForSale = new ProductAmountCollection();

            // for each good
            foreach (var pair in Storage)
            {
                // get each item
                var product = pair.Item1;
                var amount = pair.Item2;

                // if it is desired by the population.
                if (TotalNeeds.Contains(product))
                {
                    // Subtract what is needed.
                    amount = amount - TotalNeeds.GetProductValue(product);
                }

                // If the amount left after removing needs is greater than 0
                // then add to our list for sale.
                if (amount > 0)
                {
                    ForSale.AddProducts(product, amount);
                }
            }

            return ForSale;
        }

        /// <summary>
        /// Retrieve the Currency the population holds.
        /// </summary>
        /// <param name="Currencies">What counts as currency.</param>
        /// <returns></returns>
        public IProductAmountCollection GetCash(IList<IProduct> Currencies)
        {
            // TODO this will most likely be changed when products
            // have more implementations.
            return ForSale.GetProducts(Currencies);
        }

        /// <summary>
        /// Get's the purchasing power of the population, returned in order of
        /// prefered buying order (most durable to least)
        /// </summary>
        /// <returns>The Items for sale in Descending chance of breaking.</returns>
        public IProductAmountCollection PurchasingPower()
        { // TODO delete this, it's not used.
            // Get the products for sale and order them by the chance of failure
            // (how long they last) 
            // Maybe give a special sorting to put Currency Up Front?
            return ForSale.OrderProductsBy(x => x.DailyFailureChance);
        }

        public void PopulationChange()
        {
            throw new NotImplementedException();
        }
        
        #endregion Actions

        public bool Equals(IPopulationGroup other)
        {
            return this.Id == other.Id;
        }

        public bool Equals(IPopulationGroup x, IPopulationGroup y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(IPopulationGroup obj)
        {
            return Id.GetHashCode();
        }

        public void LoadFromSql(SqlConnection connection)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the price of a good by the pop
        /// </summary>
        /// <param name="good">The good to price.</param>
        /// <param name="v">The current market price of the good.</param>
        /// <returns>The population's price of the good.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="good"/> is null.</exception>
        public double GetPrice(IProduct good, double v)
        { // TODO This is currently not tested.
            if (good is null)
                throw new ArgumentNullException(nameof(good));

            // For a generic pop, this is just the market price, no special logic yet.
            return v;
        }

        public IProductAmountCollection BuyGood(IProductAmountCollection cash,
            IProduct good, double amount, IMarket market)
        {
            // Sanity check for nulls.
            if (cash is null) throw new ArgumentNullException(nameof(cash));
            if (good is null) throw new ArgumentNullException(nameof(good));
            if (market is null) throw new ArgumentNullException(nameof(market));
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));

            // The collection we're returning.
            var result = new ProductAmountCollection();

            // get how much to buy, what's available, or what's desired.
            var available = Math.Min(amount, ForSale.GetProductValue(good));

            // get the price of that good
            var totalPrice = GetPrice(good, market.GetPrice(good)) * available;

            // get the cash needed for the goods
            var money = market.ChangeForPrice(cash, totalPrice);

            // get our available money total
            var totalMoney = money.Sum(x => market.GetPrice(x.Item1, x.Item2));

            // if the money is enough, just buy outright
            if (totalMoney >= totalPrice)
            {
                // Add what they're buying.
                result.AddProducts(good, available);
                // remove what they spent
                result.AddProducts(money.Multiply(-1));
            }
            else if (!good.Fractional && totalMoney < market.GetPrice(good))
            {// If the total money is not enough for a single unit, and the good can't be divided
                // we can't actually buy anything, so just return an empty transaction
                return new ProductAmountCollection();
            }
            else // if it's not enough
            {
                // get the buyable units of the good
                double buyableUnits = 0;

                // if we can buy fractionally
                if (good.Fractional)
                {
                    // just do the math straight.
                    buyableUnits = totalMoney / market.GetPrice(good);
                }
                else // if we can't 
                {
                    // take what we can and round down, seller should always make more than the buyer here.
                    buyableUnits = Math.Floor(totalMoney / market.GetPrice(good));
                }

                // if we can buy any units
                if (buyableUnits > 0)
                {
                    // buy them add the units to the results
                    result.AddProducts(good, buyableUnits);
                    // subtract the cash.
                    result.AddProducts(money.Multiply(-1));
                }
            }

            // Get change, if any.
            var change = totalMoney - market.GetPrice(good) * result.GetProductValue(good);

            // get the change, if any
            IProductAmountCollection buyersChange = new ProductAmountCollection();

            // if change is greater than 0.
            if (change > 0) // Make said change.
                buyersChange = market.ChangeForPrice(GetCash(market.AcceptedCurrencies), change);

            // add back the buyer's change
            result.AddProducts(buyersChange);

            // TODO, maybe add check to see if the seller shortchanged the buyer.

            // complete the transaction for the seller, and subtract the result.
            CompleteTransaction(result.Multiply(-1));

            // we're done, return the change in the buyer's goods.
            return result;
        }

        /// <summary>
        /// Buys good via barter.
        /// </summary>
        /// <param name="buyerStock">The buyer's goods up for trade.</param>
        /// <param name="good">The good being traded for.</param>
        /// <param name="amount">The amount of the good being bought.</param>
        /// <param name="market">The market that the barter is taking place in.</param>
        /// <returns>The resulting change in goods for the buyer.</returns>
        public IProductAmountCollection BarterGood(IProductAmountCollection buyerStock,
            IProduct good, double amount, IMarket market)
        {
            // TODO, update to use the coincidence of wants,
            // it will discourage barter more than anything.
            if (buyerStock == null)
                throw new ArgumentNullException(nameof(buyerStock));
            if (good == null)
                throw new ArgumentNullException(nameof(good));
            if (market == null)
                throw new ArgumentNullException(nameof(market));
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            // the return result of the barter
            var result = new ProductAmountCollection();

            // TODO a barter modifier, to discourage or encourage bartering.
            // This should become more flexible later.
            var BarterMod = 1;

            // Get how much we can or want to get
            amount = Math.Min(amount, ForSale.GetProductValue(good));

            // get the price of the good.
            var totalPrice = GetPrice(good, market.GetPrice(good)) * amount * BarterMod;

            // get the total price of what's offered.
            double barterVal = 0;
            foreach (var product in buyerStock)
            {
                barterVal += market.GetPrice(product.Item1) * product.Item2;
            }

            // the barter we're trading for the goods.
            IProductAmountCollection barter;
            // if the available barter is greater than the price, begin bartering
            if (barterVal >= totalPrice)
            {
                // Use get change for the easiest option.
                barter = market.ChangeForPrice(buyerStock, totalPrice);

                // don't go more accurate, barter isn't supposed to be more accurate to coins,
                // no one would accept change in bits of wheat.

                // Add the goods being bought
                result.AddProducts(good, amount);

                // Remove the goods being traded.
                result.AddProducts(barter.Multiply(-1));
            }
            else
            {
                // if it's not enough, throw it all in, and buy what you can.
                double buyableUnits = 0;

                // if the good is fractional
                if (good.Fractional)
                {
                    // just divide
                    buyableUnits = barterVal / (market.GetPrice(good) * BarterMod);
                }
                else
                {
                    // round down
                    buyableUnits = Math.Floor(barterVal / (market.GetPrice(good) * BarterMod));
                }

                // If we can buy anything, do so.
                if (buyableUnits > 0)
                {
                    // add the goods
                    result.AddProducts(good, buyableUnits);
                    // subtract the goods
                    result.AddProducts(buyerStock.Multiply(-1));
                }
            }

            // No change, this is bartering.

            // We've done what we can, move on.
            return result;
        }

        public void CompleteTransaction(IProductAmountCollection transaction)
        {
            if (transaction is null)
                throw new ArgumentNullException(nameof(transaction));

            // add and remove the trasaction to storage
            Storage.AddProducts(transaction);
            // Add to for sale, rather than recalculating it entirely
            ForSale.AddProducts(transaction);

            // quickly remove any products from for sale that are 0 or less.
            if (ForSale.Any(x => x.Item2 <= 0))
            {
                var gone = ForSale.Where(x => x.Item2 == 0).ToList();
                foreach (var product in gone)
                {
                    ForSale.DeleteProduct(product.Item1);
                }
            }
        }

        public double Success()
        {
            // get average need satisfaction
            var success = AverageLifeSatisfaction();
            success += AverageDailySatisfaction();
            success += AverageLuxurySatisfaction();
            return success;
        }

        // Here down is not tested as functionality is simple.

        public double AverageJobInputSatisfaction()
        {
            return JobInputSatisfaction.Average(x => x.Item2);
        }

        public double AverageJobCapitalSatisfaction()
        {
            return JobCapitalSatisfaction.Average(x => x.Item2);
        }

        public double Profitability(IMarket market)
        {
            // Requires knowing how merchants work.
            throw new NotImplementedException();
        }

        public double EntryCost(IMarket market)
        {
            // Requires knowing how merchants work.
            throw new NotImplementedException();
        }

        public void AddPop(double born)
        {
            // Add new pops to population.
            Count += born;
        }
    }
}