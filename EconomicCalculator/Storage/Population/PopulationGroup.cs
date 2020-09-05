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

        /// <summary>
        /// Products currently stored by the Population.
        /// Should never have a negative storage value.
        /// </summary>
        public IProductAmountCollection Storage { get; set; }

        public string SkillName { get; set; }

        public int SkillLevel { get; set; }

        public IProduct JobLabor { get; set; }

        public PopulationGroup()
        {
            Id = Guid.NewGuid();
            SecondaryJobs = new List<IJob>();
        }

        #region Helper

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
        /// The satisfaction of the population's life needs.
        /// </summary>
        public IProductAmountCollection LifeSatisfaction { get; }

        /// <summary>
        /// The satisfaction of the population's daily needs.
        /// </summary>
        public IProductAmountCollection DailySatisfaction { get; }

        /// <summary>
        /// The satisfaction of the population's luxury needs.
        /// </summary>
        public IProductAmountCollection LuxurySatisfaction { get; }

        /// <summary>
        /// The life need shortfall (if any).
        /// </summary>
        public IProductAmountCollection LifeShortfall { get
            {
                return LifeNeeds.MultiplyBy(LifeSatisfaction);
            }
        }

        /// <summary>
        /// The Daily need shortfall (if any).
        /// </summary>
        public IProductAmountCollection DailyShortfall { get; }

        /// <summary>
        /// The Daily need shortfall (if any).
        /// </summary>
        public IProductAmountCollection LuxuryShortfall { get; }

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
        public IProductAmountCollection JobInputShortfall { get; }

        /// <summary>
        /// The Job Input shortfall (if any).
        /// </summary>
        public IProductAmountCollection JobCapitalShortfall { get; }

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
                sat = Math.Min(sat, Storage.GetProductAmount(product) / amount);
            }

            // If something cannot be satisfied, we GTFO.
            if (sat == 0)
                return new ProductAmountCollection();

            // With Satisfaction, get and consume the inputs.
            var inputs = PrimaryJob.Inputs.Multiply(Count / PrimaryJob.LaborRequirements * sat);
            ConsumeGoods(inputs);
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
        /// <returns>A percentage of satisfaction for each good.</returns>
        public IProductAmountCollection ConsumptionPhase()
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
        /// <returns>The satisfaction of each product for happiness measurements</returns>
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

        public IProductAmountCollection LossPhase()
        {
            throw new NotImplementedException();
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
    }
}
