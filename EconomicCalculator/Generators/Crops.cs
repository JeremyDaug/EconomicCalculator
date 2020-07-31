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

        public IList<IProduct> Inputs => Seeding;

        public IDictionary<string, double> InputRequirements => Planting;

        public IList<IProduct> Outputs => HarvestProducts;

        public IDictionary<string, double> OutputResults => HarvestAmounts;

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
    }
}
