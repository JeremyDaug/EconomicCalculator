using EconomicCalculator.Enums;
using EconomicCalculator.Generators;
using EconomicCalculator.Intermediaries;
using EconomicCalculator.Storage;
using EconomicCalculator.Storage.Jobs;
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

        #region LoadData

        public void LoadData(string worldName)
        {
            // Product seeding
            if (Debug)
            {
                
            }
            else
            {
                // TODO working on an more progress, SQL DB being sidelined to functionality.
                /*
                // Products
                var products = LoadProducts();
                Console.WriteLine("--- Products Loaded");

                // crops
                var crops = LoadCrops(products);
                Console.WriteLine("--- Crops Loaded");

                // Mines
                var mines = LoadMines(products);
                Console.WriteLine("--- Mines Loaded");

                // Processes
                var processes = LoadProcesses(products);
                Console.WriteLine("--- Processes Loaded");

                // Currencies
                var Currencies = LoadCurrencies(products);
                Console.WriteLine("--- Currencies Loaded");

                // Populations
                var Populations = LoadPopulations(products, crops, mines, processes, Currencies);
                Console.WriteLine("--- Populations Loaded");

                // Markets
                markets = LoadMarkets(products, crops, mines, processes, Currencies, Populations);
                Console.WriteLine("--- Markets Loaded");

                // Finalize Market
                WorldMarket = new Market
                {
                    Name = worldName,
                    AvailableGoods = products,
                    AvailableCrops = crops,
                    AvailableMines = mines,
                    AvailableProcesses = processes,
                    AvailableCurrencies = Currencies,
                    Pops = Populations,
                    TotalPopulation = Populations.Sum(x => x.Count)
                };
                Console.WriteLine("--- Global Market Finalized");
                */
            }
        }

        private IList<IMarket> LoadMarkets(IList<IProduct> products, IList<ICrops> crops,
            IList<IMine> mines, IList<IProcess> processes, IList<ICurrency> currencies,
            IList<IPopulation> populations)
        {
            var markets = new List<Market>();
            // base market
            var sql = "Select * from Markets";
            var sqlCommand = new SqlCommand(sql, connection);
            using (var reader = sqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    var marketName = reader.GetString(reader.GetOrdinal("Name"));
                    var population = reader.GetDouble(reader.GetOrdinal("TotalPopulation"));

                    var market = new Market
                    {
                        Name = marketName,
                        TotalPopulation = population
                    };
                    markets.Add(market);
                }
            }

            // Crops
            sql = "Select * from MarketCrops";
            sqlCommand = new SqlCommand(sql, connection);
            using (var reader = sqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    var name = reader.GetString(reader.GetOrdinal("MarketName"));
                    var crop = reader.GetString(reader.GetOrdinal("CropName"));

                    var market = markets.Single(x => x.Name == name);
                    //market.AvailableCrops.Add(crops.Single(x => x.Name == crop));
                }
            }

            // mines
            sql = "Select * from MarketMines";
            sqlCommand = new SqlCommand(sql, connection);
            using (var reader = sqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    var name = reader.GetString(reader.GetOrdinal("MarketName"));
                    var mine = reader.GetString(reader.GetOrdinal("MineName"));

                    var market = markets.Single(x => x.Name == name);
                    //market.AvailableMines.Add(mines.Single(x => x.Name == mine));
                }
            }

            // Processes
            sql = "Select * from MarketProcesses";
            sqlCommand = new SqlCommand(sql, connection);
            using (var reader = sqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    var name = reader.GetString(reader.GetOrdinal("MarketName"));
                    var process = reader.GetString(reader.GetOrdinal("ProcessName"));

                    var market = markets.Single(x => x.Name == name);
                    //market.AvailableProcesses.Add(processes.Single(x => x.Name == process));
                }
            }

            // Currency
            sql = "Select * from MarketCurrencies";
            sqlCommand = new SqlCommand(sql, connection);
            using (var reader = sqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    var name = reader.GetString(reader.GetOrdinal("MarketName"));
                    var currency = reader.GetString(reader.GetOrdinal("CurrencyName"));

                    var market = markets.Single(x => x.Name == name);
                    //market.AvailableCurrencies.Add(currencies.Single(x => x.Name == currency));
                }
            }

            // populations
            sql = "Select Name, Variant, Market from Populations";
            sqlCommand = new SqlCommand(sql, connection);
            using (var reader = sqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    var name = reader.GetString(reader.GetOrdinal("Name"));
                    var variant = reader.GetString(reader.GetOrdinal("Variant"));
                    var marketName = reader.GetString(reader.GetOrdinal("Market"));

                    var pop = (Population)populations.Single(x => x.Name == name && x.VariantName == variant);
                    var market = markets.Single(x => x.Name == marketName);

                    //market.Pops.Add(pop);
                    pop.Market = market;
                }
            }

            return new List<IMarket>(markets);
        }

        private IList<IPopulation> LoadPopulations(IList<IProduct> products, IList<ICrops> crops,
            IList<IMine> mines, IList<IProcess> processes, IList<ICurrency> currencies)
        {
            // base population
            var Populations = new List<Population>();
            var sql = "Select Name, Variant, Market, Count, JobCategory, Populations.JobName, CropJob, MineJob, ProcessJob\n" +
                      "From Populations\n" +
                      "Join JobBoard\n" +
                      "On Populations.JobName = JobBoard.JobName";
            var sqlCommand = new SqlCommand(sql, connection);
            using (var reader = sqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    var popName = reader.GetString(reader.GetOrdinal("Name"));
                    var popVariant = reader.GetString(reader.GetOrdinal("Variant"));
                    var market = reader.GetString(reader.GetOrdinal("Market"));
                    var popCount = reader.GetInt32(reader.GetOrdinal("Count"));
                    var jobCatStr = reader.GetString(reader.GetOrdinal("JobCategory"));
                    var jobCategory = (JobCategory)Enum.Parse(typeof(JobCategory), jobCatStr);
                    var jobName = reader.GetString(reader.GetOrdinal("JobName"));
                    IJob job;

                    if (!reader.IsDBNull(reader.GetOrdinal("CropJob")))
                    {
                        var crop = reader.GetString(reader.GetOrdinal("CropJob"));
                        job = crops.Single(x => x.Name == crop);
                    }
                    else if (!reader.IsDBNull(reader.GetOrdinal("MineJob")))
                    {
                        var mine = reader.GetString(reader.GetOrdinal("MineJob"));
                        job = mines.Single(x => x.Name == mine);
                    }
                    else
                    {
                        var process = reader.GetString(reader.GetOrdinal("ProcessJob"));
                        job = processes.Single(x => x.Name == process);
                    }

                    var Population = new Population
                    {
                        Name = popName,
                        VariantName = popVariant,
                        JobCategory = jobCategory,
                        Count = popCount,
                        JobName = jobName,
                        Job = job
                    };

                    Populations.Add(Population);
                }
            }

            // goods
            sql = "Select * From PopulationGoods";
            var goodsSqlCommand = new SqlCommand(sql, connection);
            using (var goodReader = goodsSqlCommand.ExecuteReader())
            {
                while (goodReader.Read())
                {
                    var popName = goodReader.GetString(goodReader.GetOrdinal("PopulationName"));
                    var popVar = goodReader.GetString(goodReader.GetOrdinal("VariantName"));
                    var pop = Populations.Single(x => x.Name == popName && x.VariantName == popVar);
                    var productName = goodReader.GetString(goodReader.GetOrdinal("ProductName"));
                    var good = products.Single(x => x.Name == productName);
                    var amount = goodReader.GetDouble(goodReader.GetOrdinal("Amount"));

                    pop.GoodsForSale.Add(good);
                    pop.GoodAmounts[productName] = amount;
                }
            }

            // Life Needs
            sql = "Select * From PopulationLifeNeeds";
            var needsSqlCommand = new SqlCommand(sql, connection);
            using (var needReader = needsSqlCommand.ExecuteReader())
            {
                while (needReader.Read())
                {
                    var popName = needReader.GetString(needReader.GetOrdinal("PopulationName"));
                    var popVar = needReader.GetString(needReader.GetOrdinal("VariantName"));
                    var pop = Populations.Single(x => x.Name == popName && x.VariantName == popVar);
                    var productName = needReader.GetString(needReader.GetOrdinal("ProductName"));
                    var lifeNeed = products.Single(x => x.Name == productName);
                    var amount = needReader.GetDouble(needReader.GetOrdinal("Amount"));

                    pop.LifeNeeds.Add(lifeNeed);
                    pop.LifeNeedAmounts[productName] = amount;
                }
            }

            // Money
            sql = "Select * From PopulationMoney";
            var moneySqlCommand = new SqlCommand(sql, connection);
            using (var moneyReader = moneySqlCommand.ExecuteReader())
            {
                while (moneyReader.Read())
                {
                    var popName = moneyReader.GetString(moneyReader.GetOrdinal("PopulationName"));
                    var popVar = moneyReader.GetString(moneyReader.GetOrdinal("VariantName"));
                    var pop = Populations.Single(x => x.Name == popName && x.VariantName == popVar);
                    var currencyName = moneyReader.GetString(moneyReader.GetOrdinal("CurrencyName"));
                    var currency = currencies.Single(x => x.Name == currencyName);
                    var amountCol = moneyReader.GetOrdinal("Amount");
                    var amount = moneyReader.GetDouble(moneyReader.GetOrdinal("Amount"));

                    pop.Currencies.Add(currency);
                    pop.CurrencyAmounts[currencyName] = amount;
                }
            }

            return new List<IPopulation>(Populations);
        }

        private IList<IProcess> LoadProcesses(IList<IProduct> products)
        {
            var sql =
                "Select Name, Labor, InputName, ProcessInputs.Amount, " +
                "OutputName, ProcessOutputs.Amount " +
                "From Processes " +
                "inner join ProcessInputs " +
                "on Processes.Name=ProcessInputs.ProcessName " +
                "inner join ProcessOutputs " +
                "on Processes.Name=ProcessOutputs.ProcessName";
            var command = new SqlCommand(sql, connection);
            var processLabor = new Dictionary<string, double>();
            using (var reader = command.ExecuteReader())
            {
                var processes = new List<IProcess>();
                while (reader.Read())
                {
                    var ProcessName = reader.GetString(reader.GetOrdinal("Name"));
                    var Labor = reader.GetDouble(reader.GetOrdinal("Labor"));
                    var InputName = reader.GetString(reader.GetOrdinal("InputName"));
                    var Input = products.Single(x => x.Name == InputName);
                    var InputAmount = reader.GetDouble(3);
                    var OutputName = reader.GetString(reader.GetOrdinal("OutputName"));
                    var Output = products.Single(x => x.Name == OutputName);
                    var OutputAmount = reader.GetDouble(5);

                    if (processes.Any(x => x.Name == ProcessName))
                    {
                        var process = processes.Single(x => x.Name == ProcessName);
                        if (!process.Inputs.Any(x => x.Name == InputName))
                        {
                            process.Inputs.Add(Input);
                            process.InputRequirements[InputName] = InputAmount;
                        }
                        if (!process.Outputs.Any(x => x.Name == OutputName))
                        {
                            process.Outputs.Add(Output);
                            process.OutputResults[OutputName] = OutputAmount;
                        }
                    }
                    else
                    {
                        var process = new Process
                        {
                            Name = ProcessName,
                            LaborRequirements = Labor,
                            Inputs = new List<IProduct> { Input },
                            InputRequirements = new Dictionary<string, double> { { InputName, InputAmount } },
                            Outputs = new List<IProduct> { Output },
                            OutputResults = new Dictionary<string, double> { { OutputName, OutputAmount } }
                        };
                        //processes.Add(process);
                    }
                }

                return processes;
            }
        }

        private IList<IMine> LoadMines(IList<IProduct> products)
        {
            var sql = "Select * from Mines join MineProducts On Mines.Name=MineProducts.MineName";
            var command = new SqlCommand(sql, connection);
            using (var reader = command.ExecuteReader())
            {
                var mines = new List<IMine>();
                while (reader.Read())
                {
                    var MineName = reader.GetString(reader.GetOrdinal("Name"));
                    var MineType = (MineType)Enum.Parse(typeof(MineType), reader.GetString(reader.GetOrdinal("MineType")));
                    var RockType = (RockType)Enum.Parse(typeof(RockType), reader.GetString(reader.GetOrdinal("RockType")));
                    var Labor = reader.GetDouble(reader.GetOrdinal("LaborRequirements"));
                    var OutputName = reader.GetString(reader.GetOrdinal("ProductName"));
                    var OutputProduct = products.Single(x => x.Name == OutputName);
                    var OutputAmount = reader.GetDouble(reader.GetOrdinal("Amount"));

                    if (mines.Any(x => x.Name == MineName))
                    {
                        var mine = mines.Single(x => x.Name == MineName);
                        mine.Products.Add(OutputProduct);
                        mine.ProductAmounts[OutputProduct.Name] = OutputAmount;
                    }
                    else
                    {
                        var mine = new Mine
                        {
                            Name = MineName,
                            MineType = MineType,
                            RockType = RockType,
                            LaborRequirements = Labor,
                            Products = new List<IProduct> { OutputProduct },
                            ProductAmounts = new Dictionary<string, double> { { OutputName, OutputAmount } }
                        };
                        //mines.Add(mine);
                    }
                }
                return mines;
            }
        }

        private IList<ICurrency> LoadCurrencies(IList<IProduct> products)
        {
            var sql = "Select * from Currencies";
            var command = new SqlCommand(sql, connection);
            using (var reader = command.ExecuteReader())
            {
                var currencies = new List<ICurrency>();

                while (reader.Read())
                {
                    var name = (string)reader.GetValue(0);
                    var CashName = (string)reader.GetValue(1);
                    var BackingName = (string)reader.GetValue(2);
                    var value = (double)reader.GetValue(3);

                    var currency = new Currency
                    {
                        Name = name,
                        Cash = products.Single(x => x.Name == CashName),
                        Backing = products.Single(x => x.Name == BackingName),
                        Value = value
                    };
                    currencies.Add(currency);
                }
                return currencies;
            }
        }

        private IList<ICrops> LoadCrops(IList<IProduct> products)
        {
            var sql = "Select * from Crops Join CropOutputs On Crops.Name=CropOutputs.CropName";
            var sqlCommand = new SqlCommand(sql, connection);
            using (var reader = sqlCommand.ExecuteReader())
            {
                var crops = new List<Crops>();

                while (reader.Read())
                {
                    var CropName = (string)reader.GetValue(reader.GetOrdinal("Name"));
                    var CropTypeStr = (string)reader.GetValue(reader.GetOrdinal("Type"));
                    var CropType = (CropType)Enum.Parse(typeof(CropType), CropTypeStr);
                    var Planting = reader.GetDouble(reader.GetOrdinal("Planting"));
                    var SeedName = (string)reader.GetValue(reader.GetOrdinal("Seed"));
                    var SeedProduct = products.Single(x => x.Name == SeedName);
                    var Labor = (double)reader.GetValue(reader.GetOrdinal("Labor"));
                    var LifeCycle = (int)reader.GetValue(reader.GetOrdinal("LifeCycle"));
                    var OutputName = (string)reader.GetValue(reader.GetOrdinal("OutputProduct"));
                    var OutputProduct = products.Single(x => x.Name == OutputName);
                    var OutputAmount = (double)reader.GetValue(reader.GetOrdinal("Amount"));

                    if (crops.Any(x => x.Name == CropName))
                    {
                        crops.Single(x => x.Name == CropName).HarvestProducts.Add(OutputProduct);
                        crops.Single(x => x.Name == CropName).HarvestAmounts[OutputName] = OutputAmount;
                    }
                    else
                    {
                        var crop = new Crops
                        {
                            Name = CropName,
                            CropType = CropType,
                            Seeding = new List<IProduct> { SeedProduct },
                            Planting = new Dictionary<string, double> { { SeedName, Planting } },
                            LaborRequirements = new List<double>(),
                            CropLifecycle = LifeCycle,
                            HarvestProducts = new List<IProduct> { OutputProduct },
                            HarvestAmounts = new Dictionary<string, double> { { OutputProduct.Name, OutputAmount } }
                        };
                        crops.Add(crop);
                    }
                }
                return new List<ICrops>(null);
            }
        }

        private IList<IProduct> LoadProducts()
        {
            var productsSql = "Select * from Products";
            var productsSqlCommand = new SqlCommand(productsSql, connection);
            using (var reader = productsSqlCommand.ExecuteReader())
            {
                var products = new List<IProduct>();
                while (reader.Read())
                {
                    var name = reader.GetString(reader.GetOrdinal("Name"));
                    var unit = reader.GetString(reader.GetOrdinal("Unit"));
                    var currentPrice = reader.GetDouble(reader.GetOrdinal("CurrentPrice"));
                    var MTTF = reader.GetInt32(reader.GetOrdinal("MeanTimeToFailure"));

                    var product = new Product
                    {
                        Name = name,
                        UnitName = unit,
                        DefaultPrice = currentPrice,
                        MTTF = MTTF
                    };
                    products.Add(product);
                }
                return products;
            }
        }

        #endregion LoadData

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
