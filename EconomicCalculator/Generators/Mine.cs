using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconomicCalculator.Enums;
using EconomicCalculator.Intermediaries;

namespace EconomicCalculator.Generators
{
    internal class Mine : IMine
    {
        public string Name { get; set; }

        public MineType MineType { get; set; }

        public RockType RockType { get; set; }

        public double LaborRequirements { get; set; }

        public IList<IProduct> Products { get; set; }

        public IDictionary<string, double> ProductAmounts { get; set; }

        public IList<IProduct> Outputs => Products;

        public IDictionary<string, double> OutputResults => ProductAmounts;

        public IList<IProduct> Requirements { get; set; }

        public IDictionary<string, double> RequirementAmounts { get; set; }

        public IList<IProduct> Inputs => Requirements;

        public IDictionary<string, double> InputRequirements => RequirementAmounts;

        public Mine()
        {
            Products = new List<IProduct>();
            ProductAmounts = new Dictionary<string, double>();
            Requirements = new List<IProduct>();
            RequirementAmounts = new Dictionary<string, double>();
        }
    }
}
