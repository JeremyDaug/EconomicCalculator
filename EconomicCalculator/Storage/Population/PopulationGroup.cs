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

        public PopulationGroup()
        {
            Id = Guid.NewGuid();
        }

        #region Actions

        public IProductAmountCollection ProductionPhase()
        {
            throw new NotImplementedException();
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

        public IProductAmountCollection Consume()
        {
            throw new NotImplementedException();
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
