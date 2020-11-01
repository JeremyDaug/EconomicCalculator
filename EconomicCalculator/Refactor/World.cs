using EconomicCalculator.Enums;
using EconomicCalculator.Storage;
using EconomicCalculator.Storage.Jobs;
using EconomicCalculator.Storage.Products;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EconomicCalculator.Runner
{
    
    /// <summary>
    /// The world Acts as the head to the markets
    /// </summary>
    public class World
    {
        private bool Debug = true;
        /// <summary>
        /// The name of the world.
        /// </summary>
        public string Name { get; set; }

        private const string missingMarketMsg = "Market: '{0}' Does not Exist.";

        /// <summary>
        /// The Unified market that is the world.
        /// </summary>
        public IMarket WorldMarket;

        public IList<IMarket> markets;

        /// <summary>
        /// The connection to the database.
        /// </summary>
        private SqlConnection connection;

        #region AlterationFunctions

        public void AddMarket(IMarket market)
        {
            // Check if market already exists in world.
            if (markets.Contains(market))
                return;
        }

        #endregion AlterationFunctions

        #region PrintFunctions

        public string PrintCurrencies(string market)
        {
            if (string.IsNullOrEmpty(market) || string.Equals(market, WorldMarket.Name))
                return WorldMarket.PrintCurrencies();
            var currMark = markets.SingleOrDefault(x => x.Name == market);
            if (currMark == null)
                return string.Format(missingMarketMsg, market);
            return "--------------------\n" + currMark.PrintCurrencies();
        }

        public string PrintProducts(string market)
        {
            if (string.IsNullOrEmpty(market) || string.Equals(market, WorldMarket.Name))
                return WorldMarket.PrintProducts();
            var currMark = markets.SingleOrDefault(x => x.Name == market);
            if (currMark == null)
                return string.Format(missingMarketMsg, market);
            return "--------------------\n" + currMark.PrintProducts();
        }

        public string PrintPops(string market)
        {
            if (string.IsNullOrEmpty(market) || string.Equals(market, WorldMarket.Name))
                return WorldMarket.PrintPops();
            var currMark = markets.SingleOrDefault(x => x.Name == market);
            if (currMark == null)
                return string.Format(missingMarketMsg, market);
            return "--------------------\n" + currMark.PrintPops();
        }

        public string PrintProcesses(string market)
        {
            if (string.IsNullOrEmpty(market) || string.Equals(market, WorldMarket.Name))
                return WorldMarket.PrintProcesses();
            var currMark = markets.SingleOrDefault(x => x.Name == market);
            if (currMark == null)
                return string.Format(missingMarketMsg, market);
            return "--------------------\n" + currMark.PrintProcesses();
        }

        public string PrintMines(string market)
        {
            if (string.IsNullOrEmpty(market) || string.Equals(market, WorldMarket.Name))
                return WorldMarket.PrintMines();
            var currMark = markets.SingleOrDefault(x => x.Name == market);
            if (currMark == null)
                return string.Format("Market: '{0}' Does not Exist.", market);
            return "--------------------\n" + currMark.PrintMines();
        }

        public string PrintMarkets()
        {
            var result = WorldMarket.ToString();

            foreach (var market in markets)
                result += market.ToString();

            return "--------------------\n" + result;
        }

        public string PrintMarketNames()
        {
            string result = WorldMarket.Name + "\n";
            
            foreach (var market in markets)
            {
                result += market.Name + "\n";
            }

            return result;
        }

        public string PrintCrops(string market)
        {
            if (string.IsNullOrEmpty(market) || string.Equals(market, WorldMarket.Name))
                return WorldMarket.PrintCrops();
            var currMark = markets.SingleOrDefault(x => x.Name == market);
            if (currMark == null)
                return string.Format("Market: '{0}' Does not Exist.", market);
            return "--------------------\n" + currMark.PrintCrops();
        }

        #endregion PrintFunctions

        public void Open()
        {
            connection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\Projects\EconomicCalculator\EconomicCalculator\Storage\EconomicDB.mdf;Integrated Security=True");
            connection.Open();
        }

        public void Close()
        {
            connection.Close();
        }

        public void RunFor(int dayCount)
        {
            int i = 0;
            while (i < dayCount)
            {
                foreach (var market in markets)
                {
                    // Run each market's internal activity and production cycle. (World Market is excluded from all of this activity.
                    //market.ProductionCycle();

                    // Do all consumption inside each market that is possible.
                    // market.InternalConsumption();
                }

                // Merchants buy and/or sell
                // market.MerchantTurn();

                // Try secondary consumption run from merchants.
                // market.ImportTurn();
            }
        }

        static void Main(string[] args)
        {

        }
    }
}