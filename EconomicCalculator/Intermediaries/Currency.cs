using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Intermediaries
{
    internal class Currency : ICurrency
    {
        public string Name { get; set; }

        public string VariantName { get; set; }

        public IProduct Cash { get; set; }

        public IProduct Backing { get; set; }

        public double Value { get; set; }

        public double ConstructionCost()
        {
            return Cash.CurrentPrice;
        }
    }
}
