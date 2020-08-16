using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Intermediaries
{
    internal class Product : IProduct
    {
        public string Name { get; set; }

        public string VariantName { get; set; }

        public string UnitName { get; set; }

        public double CurrentPrice { get; set; }

        public int Quality { get; set; }

        public int MTTF { get; set; }

        public double DailyFailureChance => 1.0d / MTTF;

        public double FailureProbability(int days)
        {
            if (days < 1)
                throw new ArgumentOutOfRangeException("Parameter 'days' cannot be less than 1.");
            if (MTTF <= 1)
                return 0;

            var chanceToNotHappen = 1 - DailyFailureChance;
            return 1 - Math.Pow(chanceToNotHappen, days);
        }

        public int GetHashCode(object obj)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            var result = string.Format("Name: {0}\n" +
                "Product Units: {1}\n" +
                "Current Price: {2}\n" +
                "Mean Time To Failure: {3}\n",
                Name, UnitName, CurrentPrice, MTTF);

            result += "--------------------\n";

            return result;
        }
    }
}
