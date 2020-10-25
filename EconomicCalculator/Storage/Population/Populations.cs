using EconomicCalculator.Storage.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage.Population
{
    public class Populations : IPopulations
    {
        public string Name { get; set; }

        public IMarket Market { get; set; }

        public double TotalPopulation { get; set; }

        public IList<IPopulationGroup> Pops { get; set; }

        public IList<IPopulationGroup> PopsByPriority
        {
            get
            {
                return Pops.OrderBy(x => x.Priority).ToList();
            }
        }

        #region SpecialPops

        // These pops are special in that they are placed uniquely in the market.
        // Merchants always buy first and get to sell first.
        // Money changers don't produce goods, but instead trade currency as needed.

        public IPopulationGroup MoneyChangers { get; }

        public IPopulationGroup Merchants { get; }

        #endregion SpecialPops

        public IDictionary<Guid, IPopulationGroup> PopsByJobs { get; set; }

        // Not Tested as it's just a pass through for a linq function.
        public double PopGrowthRate
        {
            get
            {
                return Pops.Average(x => x.PopGrowthRate);
            }
        }

        #region Actions

        /// <summary>
        /// Gets the pops selling a specific product.
        /// </summary>
        /// <param name="product">The product we want to find.</param>
        /// <returns>The populations selling that product, in priority order.</returns>
        public IList<IPopulationGroup> GetPopsSellingProduct(IProduct product)
        {
            return PopsByPriority.Where(x => x.ForSale.Contains(product)).ToList();
        }

        public IProductAmountCollection Consume()
        {
            var result = new ProductAmountCollection();

            foreach (var pop in Pops)
            {
                result.AddProducts(pop.ConsumptionPhase());
            }

            return result;
        }

        public IProductAmountCollection LossPhase()
        {
            var result = new ProductAmountCollection();
            foreach (var pop in Pops)
            {
                result.AddProducts(pop.LossPhase());
            }

            return result;
        }

        public void PopulationChanges()
        {
            // changes the population around based on skill,
            // satisfuction in the job, 
            // and profit to be made elsewhere.
            // As well as random shifts to keep it from stagnating.
            throw new NotImplementedException();
        }

        public IProductAmountCollection ProductionPhase()
        {
            var result = new ProductAmountCollection();

            foreach (var pop in Pops)
            {
                result.AddProducts(pop.ProductionPhase());
            }

            return result;
        }

        public IProductAmountCollection SellPhase()
        {
            var result = new ProductAmountCollection();

            // For all pops, put everything they have stored on the market.
            foreach (var pop in Pops)
            {
                result.AddProducts(pop.UpForSale());
            }

            return result;
        }

        public IProductAmountCollection TotalDemand()
        {
            // get result to eventually return
            var result = new ProductAmountCollection();

            // add Normal Pop Demands
            foreach (var pop in Pops)
            {
                result.AddProducts(pop.TotalNeeds);
            }

            // add merchants
            result.AddProducts(Merchants.TotalNeeds);

            // Add Money Changers
            result.AddProducts(MoneyChangers.TotalNeeds);

            // return results.
            return result;
        }

        public IProductAmountCollection TotalProduction()
        {
            // the result to eventually return
            var result = new ProductAmountCollection();

            // for every pop.
            foreach (var pop in Pops)
            {
                // get the hypothetical max production for the pop
                var maxOutput = pop.PrimaryJob.Outputs.Multiply(pop.Count);

                // and add it to the result
                result.AddProducts(maxOutput);
            }

            return result;
        }

        public double LifeNeedsSatisfaction()
        {
            // average weighted by population.
            double totalPopulation = 0;
            double totalWeight = 0;
            foreach (var pop in Pops)
            {
                totalPopulation += pop.Count;
                totalWeight += pop.AverageLifeSatisfaction() * pop.Count;
            }
            return totalWeight / totalPopulation;
        }

        public double DailyNeedsSatisfaction()
        {
            // average weighted by population.
            double totalPopulation = 0;
            double totalWeight = 0;
            foreach (var pop in Pops)
            {
                totalPopulation += pop.Count;
                totalWeight += pop.AverageDailySatisfaction() * pop.Count;
            }
            return totalWeight / totalPopulation;
        }

        public double LuxuryNeedsSatisfaction()
        {
            // average weighted by population.
            double totalPopulation = 0;
            double totalWeight = 0;
            foreach (var pop in Pops)
            {
                totalPopulation += pop.Count;
                totalWeight += pop.AverageLuxurySatisfaction() * pop.Count;
            }
            return totalWeight / totalPopulation;
        }

        public double JobInputSatisfaction()
        {
            // average weighted by population.
            double totalPopulation = 0;
            double totalWeight = 0;
            foreach (var pop in Pops)
            {
                totalPopulation += pop.Count;
                totalWeight += pop.AverageJobInputSatisfaction() * pop.Count;
            }
            return totalWeight / totalPopulation;
        }

        public double JobCapitalSatisfaction()
        {
            // average weighted by population.
            double totalPopulation = 0;
            double totalWeight = 0;
            foreach (var pop in Pops)
            {
                totalPopulation += pop.Count;
                totalWeight += pop.AverageJobCapitalSatisfaction() * pop.Count;
            }
            return totalWeight / totalPopulation;
        }

        #endregion Actions
    }
}
