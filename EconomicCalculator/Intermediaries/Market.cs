using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconomicCalculator.Generators;

namespace EconomicCalculator.Intermediaries
{
    internal class Market : IMarket
    {
        public string Name { get; set; }

        public double TotalPopulation { get; set; }

        public IList<IPopulation> Pops { get; set; }

        public IList<ICrops> AvailableCrops { get; set; }

        public IList<IMine> AvailableMines { get; set; }

        public IList<ICurrency> AvailableCurrencies { get; set; }

        public IList<IProcess> AvailableProcesses { get; set; }

        public IList<IProduct> AvailableGoods { get; set; }

        public IDictionary<string, double> AvailableGoodAmounts { get; set; }

        public Market()
        {
            Pops = new List<IPopulation>();
            AvailableCrops = new List<ICrops>();
            AvailableMines = new List<IMine>();
            AvailableCurrencies = new List<ICurrency>();
            AvailableProcesses = new List<IProcess>();
            AvailableGoods = new List<IProduct>();
            AvailableGoodAmounts = new Dictionary<string, double>();
        }
    }
}
