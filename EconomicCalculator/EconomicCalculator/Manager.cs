using EconomicCalculator.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator
{
    public class Manager
    {
        // Primary Resources
        public List<Crop> Crops;
        public List<Resource> Resources;
        public List<Animal> Animals;

        // Processing
        public List<Process> Processes;
        public List<Craft> Crafts;

        // Final Product List
        public List<IProduct> Products;
    }
}
