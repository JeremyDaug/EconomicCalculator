using EconomicCalculator.Randomizer;
using EconomicCalculator.Storage.Organizations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage.Products
{
    internal class Plot : Product, IPlot
    {
        /// <summary>
        /// Plot Constructor
        /// </summary>
        /// <param name="Territory">The Territory of the plot of land.</param>
        /// <param name="rand">Randomizer</param>
        public Plot(ITerritory Territory, IRandomizer rand) : base(rand)
        {
            // 
            this.Territory = Territory;
            Maintenance = new ProductAmountCollection();
            FailsInto = new ProductAmountCollection();
        }

        public new int MTTF => -1;

        public new bool Maintainable => false;
        
        public new IReadOnlyProductAmountCollection Maintenance { get; }

        public new IReadOnlyProductAmountCollection FailsInto { get; }

        public ITerritory Territory { get; }

        public double Size => 0.1;
    }
}
