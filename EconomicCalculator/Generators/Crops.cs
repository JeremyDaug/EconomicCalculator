using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconomicCalculator.Enums;
using EconomicCalculator.Intermediaries;

namespace EconomicCalculator.Generators
{
    internal class Crops : ICrops
    {
        /// <summary>
        /// The name of the Crop.
        /// </summary>
        public string Name { get; set; }

        public CropType CropType { get; set; }

        public IList<IProduct> Seeding { get; set; }

        public IDictionary<string, double> Planting { get; set; }

        public IList<IProduct> HarvestProducts { get; set; }

        public IDictionary<string, double> HarvestAmounts { get; set; }

        #region Alts

        public IList<IProduct> Inputs => Seeding;

        public IDictionary<string, double> InputRequirements
            => Planting.ToDictionary(x => x.Key, x => x.Value / CropLifecycle);

        public IList<IProduct> Outputs => HarvestProducts;

        public IDictionary<string, double> OutputResults => HarvestAmounts;

        #endregion Alts

        public double LaborRequirements { get; set; }

        public double CropLifecycle { get; set; }

        public Crops()
        {
            HarvestProducts = new List<IProduct>();
            HarvestAmounts = new Dictionary<string, double>();
        }

        public IReadOnlyDictionary<string, double> AveragedDailyOutput()
        {
            return HarvestAmounts.ToDictionary(x => x.Key, x => x.Value / CropLifecycle);
        }

        public double AveragedDailyOutputFor(string product)
        {
            double result = 0;
            HarvestAmounts.TryGetValue(product, out result);
            return result / CropLifecycle;
        }

        public JobTypes JobType => JobTypes.Crop;

        public IDictionary<string, double> GetExpectedInputs(int count)
            => InputRequirements.ToDictionary(x => x.Key, x => x.Value * count / CropLifecycle);

        public IDictionary<string, double> Work(IDictionary<string, double> availableGoods, int Pops)
        {
            // TODO, change this to not have any daily requirements, instead inputs based on cycles.
            // Check Daily consumption is possible. If not and something is missing, return an empty Dict.
            if (InputRequirements.Any(x => !availableGoods.ContainsKey(x.Key)))
                return new Dictionary<string, double>();

            // Calculate possible consumption, how many instances of the inputs we can meet for each.
            var inputs = InputRequirements
                .ToDictionary(x => x.Key, x => Math.Floor(availableGoods[x.Key] / x.Value));

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
                    result[product.Key] += product.Value * maxWork / CropLifecycle;
                else
                    result[product.Key] = product.Value * maxWork / CropLifecycle;
            }

            return result;
        }


        public override string ToString()
        {
            var result = string.Format("Name: {0}\n" +
                "CropType: {1}\n" +
                "Seed Products:\n", Name, CropType);
            foreach (var product in Seeding)
            {
                result += string.Format("\t {0} : {1} {2}\n", product.Name, Planting[product.Name], product.UnitName);
            }

            result += "Harvest Products:\n";

            foreach (var product in HarvestProducts)
            {
                result += string.Format("\t {0} : {1} {2}\n", product.Name, HarvestAmounts[product.Name], product.UnitName);
            }

            result += string.Format("Daily Labor Requirement: {0}\nCrop Life Cycle: {1}\n--------------------\n", LaborRequirements, CropLifecycle);

            return result;
        }
    }
}
