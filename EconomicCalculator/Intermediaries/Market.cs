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

        public IList<IProcess> AvailableProcesses { get; set; }

        public IList<ICurrency> AvailableCurrencies { get; set; }

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

        #region MarketActivity

        public void ProductionCycle()
        {
            // Each pop does it's job to the best of it's capabilities.
            IList<Task> tasks = new List<Task>();
            foreach (var pop in Pops)
            {
                tasks.Add(Task.Run(() => pop.DoWork()));
            }
        }

        #endregion MarketActivity

        #region PrintComponents

        public string PrintMines()
        {
            var result = "";
            foreach (var mine in AvailableMines)
                result += mine.ToString();
            return result;
        }

        public string PrintCrops()
        {
            var result = "";
            
            foreach (var crop in AvailableCrops)
                result += crop.ToString();

            return result;
        }

        public string PrintProcesses()
        {
            var result = "";
            foreach (var process in AvailableProcesses)
                result += process.ToString();
            return result;
        }

        public string PrintCurrencies()
        {
            var result = "";
            foreach (var currency in AvailableCurrencies)
                result += currency.ToString();
            return result;
        }

        public string PrintPops()
        {
            var result = "";
            foreach (var pop in Pops)
                result += pop.ToString();
            return result;
        }

        public string PrintProducts()
        {
            var result = "";
            foreach (var product in AvailableGoods)
                result += product.ToString();
            return result;
        }

        #endregion PrintComponents

        public override string ToString()
        {
            var result = string.Format("Name: {0}\n" +
                "Total Population: {1}\n",
                Name, TotalPopulation);

            // populations
            result += "Population Breakdown:\n";
            foreach (var pop in Pops)
            {
                result += string.Format("\tName: {0} ---- \tVariant: {1} ---- \tCount: {2}\n",
                    pop.Name, pop.VariantName, pop.Count);
            }

            // Available Crops
            result += "Available Crops:\n";
            foreach (var crop in AvailableCrops)
                result += string.Format("\tCrop: {0}\n", crop.Name);

            // Available Mines
            result += "Available Mines:\n";
            foreach (var mine in AvailableMines)
                result += string.Format("\tMine: {0}\n", mine.Name);

            // processes
            result += "Processes:\n";
            foreach (var process in AvailableProcesses)
                result += string.Format("\tProcess: {0}\n", process.Name);

            // Currencies
            result += "Currencies:\n";
            foreach (var currency in AvailableCurrencies)
                result += string.Format("\tCurrency: {0}\n", currency.Name);

            // Available Goods
            result += "Goods:\n";
            foreach (var good in AvailableGoods)
                result += string.Format("\tGoods: {0}\n", good.Name);

            result += "--------------------\n";

            return result;
        }
    }
}
