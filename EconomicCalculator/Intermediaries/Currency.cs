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

        public string Symbol { get; set; }

        public IProduct Cash { get; set; }

        public IProduct Backing { get; set; }

        public double Value { get; set; }

        public double ConstructionCost()
        {
            return Cash.CurrentPrice;
        }

        public override string ToString()
        {
            var result = string.Format("Name: {0}\n" +
                "Cash: {1}\n" +
                "Backing: {2}\n" +
                "Value: {3}\n" +
                "Symbol: {4}\n" +
                "--------------------\n", 
                Name, Cash.Name, Backing.Name, Value, Symbol);

            return result;
        }
    }
}
