using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Common
{
    public class Craft
    {
        string Name { get; set; }
        public Guid Id { get; set; }
        public Job Job { get; set; }
        public ProductAmount Inputs { get; set; }
        public ProductAmount Outputs { get; set; }
        public double TimeInDays { get; set; }
        public double PriceMultiplier { get; set; }
    }
}
