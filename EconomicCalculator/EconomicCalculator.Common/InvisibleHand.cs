using EconomicCalculator.Common.Processes;
using EconomicCalculator.Common.Resource;
using EconomicCalculator.Common.Sources;
using System;
using System.Collections.Generic;

namespace EconomicCalculator.Common
{
    public class InvisibleHand
    {
        // Sources
        public List<Mine> Resources { get; set; }
        public List<Animal> Animals { get; set; }
        public List<Crop> Crops { get; set; }

        // Product Master List
        public List<Product> Products { get; set; }

        // Processes
        public List<Butcher> Butchers { get; set; }
        public List<Craft> Crafters { get; set; }
        public List<Process> Processers { get; set; }

        public List<Job> Jobs { get; set; }
        public List<Currency> Currencies { get; set; }
        public List<Market> Markets { get; set; }
    }
}
