using EconomicCalculator.Common.Resource;
using System;
using System.Collections.Generic;

namespace EconomicCalculator.Common
{
    public class Market
    {
        public string Name { get; set; }
        public string Details { get; set; }
        public List<Product> FoundationProducts { get; set; }
        public List<Currency> Currencies { get; set; }
    }
}
