using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconomicCalculator.Enums;
using EconomicCalculator.Intermediaries;
using EconomicCalculator.Storage;

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

        public JobTypes JobType => JobTypes.Mine;

        IList<double> IJob.LaborRequirements => throw new NotImplementedException();

        public Guid Id => throw new NotImplementedException();

        public Mine()
        {
            Products = new List<IProduct>();
            ProductAmounts = new Dictionary<string, double>();
            Requirements = new List<IProduct>();
            RequirementAmounts = new Dictionary<string, double>();
        }

        public IDictionary<string, double> GetExpectedInputs(int count)
            => InputRequirements.ToDictionary(x => x.Key, x => x.Value * count / LaborRequirements);

        public IDictionary<string, double> Work(IDictionary<string, double> availableGoods, int Pops)
        {
            // Check consumption is possible. If not and something is missing, return an empty Dict.
            if (InputRequirements.Any(x => !availableGoods.ContainsKey(x.Key)))
                return new Dictionary<string, double>();

            // Calculate possible consumption, how many instances of the inputs we can meet for each.
            var inputs = InputRequirements.ToDictionary(x => x.Key, x => Math.Floor(availableGoods[x.Key] / x.Value));

            // How much labor can be satisfied.
            var doableWork = Math.Floor(Pops / LaborRequirements);

            // get the smallest value between input satisfaction and doable work.
            var maxWork = Math.Min(doableWork, inputs.Min(x => x.Value));

            // With the most work that we can do found, actually do it.
            // Subtract costs
            var result = InputRequirements.ToDictionary(x => x.Key, x => -x.Value * maxWork);

            // Add productions
            foreach (var product in OutputResults)
            {
                if (result.ContainsKey(product.Key))
                    result[product.Key] += product.Value * maxWork;
                else
                    result[product.Key] = product.Value * maxWork;
            }

            return result;
        }

        public override string ToString()
        {
            var result = string.Format("Name: {0}\n" +
                "Mine Type: {1}\n" +
                "Rock Type: {2}\n" +
                "Labor Requirements: {3}\n",
                Name, MineType, RockType, LaborRequirements);

            // Outputs
            result += "Outputs:\n";
            foreach (var item in Products)
            {
                result += string.Format("\t{0} : {1} {2}\n", item.Name, ProductAmounts[item.Name], item.UnitName);
            }

            // Inputs (requirements / capital)
            result += "Inputs:\n";
            foreach (var item in Inputs)
            {
                result += string.Format("\t{0} : {1} {2}\n", item.Name, ProductAmounts[item.Name], item.UnitName);
            }
            if (!Inputs.Any())
                result += "\t None\n";
            result += "--------------------\n";

            return result;
        }

        public double TotalLaborRequired()
        {
            throw new NotImplementedException();
        }
    }
}
