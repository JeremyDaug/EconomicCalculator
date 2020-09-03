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

        public IJob PrimaryJob { get; set; }

        public int Priority { get; set; }

        public IProductAmountCollection LifeNeeds { get; set; }

        public IProductAmountCollection DailyNeeds { get; set; }

        public IProductAmountCollection LuxuryNeeds { get; set; }

        public IList<IJob> SecondaryJobs { get; set; }

        public IProductAmountCollection Storage { get; set; }

        public string SkillName { get; set; }

        public int SkillLevel { get; set; }

        public IProduct JobLabor { get; set; }

        public PopulationGroup()
        {
            Id = Guid.NewGuid();
            SecondaryJobs = new List<IJob>();
        }

        #region Actions

        public IProductAmountCollection ProductionPhase()
        {
            // Assuming perfection, get full needs expected.
            // inputs times the population divided by the labor needed for the job.
            var requirements = PrimaryJob.Inputs.Multiply(Count / PrimaryJob.LaborRequirements);
            // Get Capital Requirements (only one needed per day of labor)
            requirements.AddProducts(PrimaryJob.Capital.Multiply(Count));

            // With expected inputs, see what we can actually satisfy.
            double sat = 1;
            foreach (var pair in requirements)
            {
                var product = pair.Item1;
                var amount = pair.Item2;

                // If storage doesn't have the product, don't bother, you can't meet any requirements.
                if (!Storage.Contains(product))
                    return new ProductAmountCollection();

                sat = Math.Min(sat, Storage.GetProductAmount(product) / amount);
            }

            // If something cannot be satisfied, we GTFO.
            if (sat == 0)
                return new ProductAmountCollection();

            // With Satisfaction, get and consume the inputs.
            var inputs = PrimaryJob.Inputs.Multiply(Count / PrimaryJob.LaborRequirements * sat);
            ConsumeGoods(inputs);
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
            }

            // Don't run breakdown of capital here, run it alongside all other breakdown chances.

            // return results
            return result;
        }

        public IProductAmountCollection BuyPhase(IMarket market)
        {
            if (market is null)
                throw new ArgumentNullException(nameof(market));

            var cart = new ProductAmountCollection();

            // Buy Life Needs
            BuyGoods(cart, market, LifeNeeds.Multiply(Count));

            // Buy Job Requirements
            // Check Capital Inputs are still avaliable.
            var CapitalRequirements = PrimaryJob.Capital.Multiply(Count);
            var JobInputs = PrimaryJob.Inputs.Multiply(Count);
            if (Storage.Any(x => x.Item2 < CapitalRequirements.GetProductAmount(x.Item1)))
                BuyGoods(cart, market, CapitalRequirements);
            // Get Daily inputs
            BuyGoods(cart, market, JobInputs);

            // Buy Daily Needs
            BuyGoods(cart, market, DailyNeeds.Multiply(Count));

            // Buy Luxuries
            BuyGoods(cart, market, LuxuryNeeds.Multiply(Count));

            return cart;
        }

        /// <summary>
        /// The Consumption action of the population group.
        /// </summary>
        /// <returns>A percentage of satisfaction for each good.</returns>
        public IProductAmountCollection Consume()
        {
            // life needs first
            var result = ConsumeGoods(LifeNeeds.Multiply(Count));

            // then daily needs
            result.AddProducts(ConsumeGoods(DailyNeeds.Multiply(Count)));

            // then luxuries
            result.AddProducts(ConsumeGoods(LuxuryNeeds.Multiply(Count)));

            // then return satisfaction.
            return result;
        }

        /// <summary>
        /// Consumes the given set of goods.
        /// </summary>
        /// <param name="goods">The goods to attempt to consume.</param>
        /// <returns>The satisfaction of each product.</returns>
        private IProductAmountCollection ConsumeGoods(IProductAmountCollection goods)
        {
            var result = new ProductAmountCollection();
            // for every good to consume.
            foreach (var pair in goods)
            {
                // Get the item and amount of the person.
                var product = pair.Item1;
                var amount = pair.Item2;

                // Assume All items being consumed are in storage already,
                // if they aren't we have a consistency problem.
                // get the satisfaction, capping it at 1.
                var sat = Math.Min(1, Storage.GetProductAmount(product) / amount);

                // If satisfaction can't be met, subtract what you can.
                if (sat < 1)
                {
                    Storage.SubtractProducts(product, 
                        Storage.GetProductAmount(product));
                }
                else // If greater than 1, then substract everything needed.
                {
                    Storage.SubtractProducts(product, amount);
                }

                // add the satisfaction to the collection.
                result.AddProducts(product, sat);
            }

            // Return satisfaction results.
            return result;
        }

        public void PopulationChange()
        {
            throw new NotImplementedException();
        }

        #endregion Actions

        public void BuyGoods(IProductAmountCollection cart, IMarket market, 
            IProductAmountCollection shoppingList)
        {
            // check for nulls
            if (cart is null)
                throw new ArgumentNullException(nameof(cart));
            if (market is null)
                throw new ArgumentNullException(nameof(market));
            if (shoppingList is null)
                throw new ArgumentNullException(nameof(shoppingList));

            // go through the shopping list and try to buy everything.
            foreach (var item in shoppingList)
            {
                var product = item.Item1;
                var amount = item.Item2;

                // remove already owned goods from shopping list and add extra demand.
                if (Storage.Contains(product))
                    amount = amount - Storage.GetProductAmount(product);

                // If the amount is already in storage, skip it and continue to the next item.
                if (amount <= 0)
                    continue;

                // Initiate Transaction between buyer and seller.
                var transaction = market.StartTransaction(this, product, amount);

                // Finalize buy from the market and add it to the cart.
                Storage.AddProducts(transaction);
            }
        }

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
    }
}
