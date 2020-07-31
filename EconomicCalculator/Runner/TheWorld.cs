using EconomicCalculator.Enums;
using EconomicCalculator.Generators;
using EconomicCalculator.Intermediaries;
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
    public class TheWorld
    {
        /// <summary>
        /// The name of the world.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Unified market that is the world.
        /// </summary>
        public IMarket WorldMarket;

        /// <summary>
        /// The connection to the database.
        /// </summary>
        private SqlConnection connection;

        public void LoadMarkets()
        {
            // Units
            var units = LoadUnits();
            Console.WriteLine("--- Units Loaded");

            // Products
            var products = LoadProducts(units);
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
            var Populations = LoadPopulations(products, crops, mines, processes);
            Console.WriteLine("--- Populations Loaded");

            // Markets

            // Finalize Market
            WorldMarket = new Market
            {
                AvailableGoods = products,
                AvailableCrops = crops,
                AvailableMines = mines,
                AvailableProcesses = processes,
                AvailableCurrencies = Currencies,
                Pops = Populations
            };
            Console.WriteLine("--- Global Market Finalized Loaded");
        }

        private IList<IPopulation> LoadPopulations(IList<IProduct> products, IList<ICrops> crops, IList<IMine> mines, IList<IProcess> processes)
        {
            var Populations = new List<Population>();
            var sql = "Select Name, Count, JobCategory, Populations.JobName, CropJob, MineJob, ProcessJob\n" +
                      "From Populations\n" +
                      "Join JobBoard\n" +
                      "On Populations.JobName = JobBoard.JobName";
            var sqlCommand = new SqlCommand(sql, connection);
            using (var reader = sqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    var popName = reader.GetString(reader.GetOrdinal("Name"));
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
                        JobCategory = jobCategory,
                        Count = popCount,
                        JobName = jobName,
                        Job = job
                    };
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
                        processes.Add(process);
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
                        mines.Add(mine);
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
                            LaborRequirements = Labor,
                            CropLifecycle = LifeCycle,
                            HarvestProducts = new List<IProduct> { OutputProduct },
                            HarvestAmounts = new Dictionary<string, double> { { OutputProduct.Name, OutputAmount } }
                        };
                        crops.Add(crop);
                    }
                }
                return new List<ICrops>(crops);
            }
        }

        private IList<IProduct> LoadProducts(IDictionary<int, string> units)
        {
            var productsSql = "Select * from Products";
            var productsSqlCommand = new SqlCommand(productsSql, connection);
            using (var productsDataReader = productsSqlCommand.ExecuteReader())
            {
                var products = new List<IProduct>();
                while (productsDataReader.Read())
                {
                    var product = new Product
                    {
                        Name = (string)productsDataReader.GetValue(0),
                        UnitName = units[(int)productsDataReader.GetValue(1)],
                        CurrentPrice = (double)productsDataReader.GetValue(2),
                        MTTF = (int)productsDataReader.GetValue(3)
                    };
                    products.Add(product);
                }
                return products;
            }
        }

        private IDictionary<int, string> LoadUnits()
        {
            // Units
            var unitsSql = "Select * from Units";
            var unitsSqlCommand = new SqlCommand(unitsSql, connection);
            using (var unitsDataReader = unitsSqlCommand.ExecuteReader())
            {
                var units = new Dictionary<int, string>();
                while (unitsDataReader.Read())
                {
                    units[(int)unitsDataReader.GetValue(0)] = (string)unitsDataReader.GetValue(1);
                }
                return units;
            }
        }

        public void Open()
        {
            connection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\Projects\EconomicCalculator\EconomicCalculator\Storage\EconomicDB.mdf;Integrated Security=True");
            connection.Open();
        }

        public void Close()
        {
            connection.Close();
        }

        static void Main(string[] args)
        {

        }
    }
}
