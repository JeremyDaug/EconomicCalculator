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

        public IList<IPopulationGroup> PopsByPriority { get; set; }

        #region Actions

        public IProductAmountCollection Consume()
        {
            var result = new ProductAmountCollection();

            foreach (var pop in Pops)
            {
                result.AddProducts(pop.ConsumptionPhase());
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

        #endregion Actions
    }
}
