using EconomicCalculator.Common;
using EconomicCalculator.Common.Processes;
using EconomicCalculator.Common.Resource;
using EconomicCalculator.Common.Sources;
using System.Collections.Generic;

namespace EconomicCalculator
{
    public class Manager
    {
        // Primary Resources
        public List<Crop> Crops { get; set; }
        public List<Mine> Resources { get; set; }
        public List<Animal> Animals { get; set; }

        // Processing
        public List<Process> Processes { get; set; }
        public List<Craft> Crafts { get; set; }
        public List<Butcher> Butchers { get; set; }

        // Final Product List
        public List<Product> Products { get; set; }
        public List<Market> Markets { get; set; }
    }
}
