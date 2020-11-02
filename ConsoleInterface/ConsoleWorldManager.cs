using EconomicCalculator.Runner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleInterface
{
    public class ConsoleWorldManager
    {
        private World theWorld;

        public ConsoleWorldManager()
        {
            theWorld = new World();
            theWorld.Name = "ConsoleWorld";
        }

        public void Open() => theWorld.Open();

        public void Close() => theWorld.Close();

        public string PrintCrops(string market) => theWorld.PrintCrops(market);

        public string PrintMarkets() => theWorld.PrintMarkets();

        public string PrintMines(string market) => theWorld.PrintMines(market);

        public string PrintProcesses(string market) => theWorld.PrintProcesses(market);

        internal string PrintCurrencies(string market) => theWorld.PrintCurrencies(market);

        internal string PrintPops(string market) => theWorld.PrintPops(market);

        internal string PrintProducts(string market) => theWorld.PrintProducts(market);

        internal string PrintMarketNames() => theWorld.PrintMarketNames();

        internal void RunFor(int dayCount) => theWorld.RunFor(dayCount);
    }
}
