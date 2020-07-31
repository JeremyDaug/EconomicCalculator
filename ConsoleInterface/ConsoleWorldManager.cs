using EconomicCalculator.Intermediaries;
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
        private TheWorld theWorld;

        public ConsoleWorldManager()
        {
            theWorld = new TheWorld();
            theWorld.Name = "ConsoleWorld";
        }

        public void LoadDB()
        {
            theWorld.LoadMarkets();
        }

        public void Open()
        {
            theWorld.Open();
        }

        public void Close()
        {
            theWorld.Close();
        }
    }
}
