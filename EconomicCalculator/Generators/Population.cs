using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconomicCalculator.Enums;
using EconomicCalculator.Intermediaries;

namespace EconomicCalculator.Generators
{
    internal class Population : IPopulation
    {
        public string Name { get; set; }

        public JobCategory JobCategory { get; set; }

        public string JobName { get; set; }

        public IJob Job { get; set; }

        public int Count { get; set; }

        public IList<ICurrency> Currencies { get; set; }

        public IDictionary<string, double> CurrencyAmounts { get; set; }

        public IList<IProduct> GoodsForSale { get; set; }

        public IDictionary<string, double> GoodAmounts { get; set; }

        public IList<IProduct> LifeNeeds { get; set; }

        public IDictionary<string, double> LifeNeedAmounts { get; set; }

        public Population()
        {
            Currencies = new List<ICurrency>();
            CurrencyAmounts = new Dictionary<string, double>();
            GoodsForSale = new List<IProduct>();
            GoodAmounts = new Dictionary<string, double>();
            LifeNeeds = new List<IProduct>();
            LifeNeedAmounts = new Dictionary<string, double>();
        }
    }
}
