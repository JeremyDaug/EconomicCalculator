using System.Text.Json.Serialization;
using EconomicSim.Objects.Processes;
using EconomicSim.Objects.Processes.ProductionTags;
using EconomicSim.Objects.Products;
using EconomicSim.Objects.Skills;
using EconomicSim.Objects.Wants;

namespace EconomicSim.Objects.Jobs
{
    /// <summary>
    /// Job Data Class
    /// </summary>
    [JsonConverter(typeof(JobJsonConverter))]
    public class Job : IJob
    {
        private Dictionary<Product, decimal>? _inputProducts;
        private Dictionary<Product, decimal>? _optionalInputProducts;
        private Dictionary<Product, decimal>? _capitalProducts;
        private Dictionary<Product, decimal>? _optionalCapitalProducts;
        private Dictionary<Product, decimal>? _outputProducts;
        
        private Dictionary<Want, decimal>? _inputWants;
        private Dictionary<Want, decimal>? _optionalInputWants;
        private Dictionary<Want, decimal>? _capitalWants;
        private Dictionary<Want, decimal>? _optionalCapitalWants;
        private Dictionary<Want, decimal>? _outputWants;

        public Job()
        {
            Processes = new List<Process>();
        }

        /// <summary>
        /// Job Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the job.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Variant Name of a job, should be unique with Primary name.
        /// </summary>
        public string VariantName { get; set; }

        /// <summary>
        /// The primary Labor Product of the job.
        /// </summary>
        public IProduct Labor { get; set; }

        /// <summary>
        /// The skill the job uses.
        /// </summary>
        public ISkill Skill { get; set; }

        /// <summary>
        /// The Processes that are done by the job.
        /// </summary>
        public List<Process> Processes { get; set; }
        IReadOnlyList<IProcess> IJob.Processes => Processes;

        #region ProcessProducts

        public IReadOnlyDictionary<Product, decimal> InputProducts
        {
            get
            {
                if (_inputProducts == null)
                {
                    _inputProducts = new Dictionary<Product, decimal>();
                    foreach (var proc in Processes)
                    {
                        foreach (var product in proc.InputProducts)
                        {
                            if (product.TagData.All(x => x.tag != ProductionTag.Optional))
                                _inputProducts.Add(product.Product, product.Amount);
                        }
                    }
                }

                return _inputProducts;
            }
        }
        public IReadOnlyDictionary<Product, decimal> OptionalInputProducts
        {
            get
            {
                if (_optionalInputProducts == null)
                {
                    _optionalInputProducts = new Dictionary<Product, decimal>();
                    foreach (var proc in Processes)
                    {
                        foreach (var product in proc.InputProducts)
                        {
                            if (product.TagData.Any(x => x.tag == ProductionTag.Optional))
                                _optionalInputProducts.Add(product.Product, product.Amount);
                        }
                    }
                }

                return _optionalInputProducts;
            }
        }
        public IReadOnlyDictionary<Product, decimal> CapitalProducts
        {
            get
            {
                if (_capitalProducts == null)
                {
                    _capitalProducts = new Dictionary<Product, decimal>();
                    foreach (var proc in Processes)
                    {
                        foreach (var product in proc.CapitalProducts)
                        {
                            if (product.TagData.Any(x => x.tag == ProductionTag.Optional))
                                _capitalProducts.Add(product.Product, product.Amount);
                        }
                    }
                }

                return _capitalProducts;
            }
        }
        public IReadOnlyDictionary<Product, decimal> OptionalCapitalProducts
        {
            get
            {
                if (_optionalCapitalProducts == null)
                {
                    _optionalCapitalProducts = new Dictionary<Product, decimal>();
                    foreach (var proc in Processes)
                    {
                        foreach (var product in proc.CapitalProducts)
                        {
                            if (product.TagData.Any(x => x.tag == ProductionTag.Optional))
                                _optionalCapitalProducts.Add(product.Product, product.Amount);
                        }
                    }
                }

                return _optionalCapitalProducts;
            }
        }
        public IReadOnlyDictionary<Product, decimal> OutputProducts
        {
            get
            {
                if (_outputProducts == null)
                {
                    _outputProducts = new Dictionary<Product, decimal>();
                    foreach (var proc in Processes)
                    {
                        foreach (var product in proc.OutputProducts)
                        {
                            _outputProducts.Add(product.Product, product.Amount);
                        }
                    }
                }

                return _outputProducts;
            }
        }

        #endregion

        #region ProcessWants

        public IReadOnlyDictionary<Want, decimal> InputWants
        {
            get
            {
                if (_inputWants == null)
                {
                    _inputWants = new Dictionary<Want, decimal>();
                    foreach (var proc in Processes)
                    {
                        foreach (var product in proc.InputWants)
                        {
                            if (product.TagData.All(x => x.tag != ProductionTag.Optional))
                                _inputWants.Add(product.Want, product.Amount);
                        }
                    }
                }

                return _inputWants;
            }
        }

        public IReadOnlyDictionary<Want, decimal> OptionalInputWants
        {
            get
            {
                if (_optionalInputWants == null)
                {
                    _optionalInputWants = new Dictionary<Want, decimal>();
                    foreach (var proc in Processes)
                    {
                        foreach (var product in proc.InputWants)
                        {
                            if (product.TagData.Any(x => x.tag == ProductionTag.Optional))
                                _optionalInputWants.Add(product.Want, product.Amount);
                        }
                    }
                }

                return _optionalInputWants;
            }
        }

        public IReadOnlyDictionary<Want, decimal> CapitalWants
        {
            get
            {
                if (_capitalWants == null)
                {
                    _capitalWants = new Dictionary<Want, decimal>();
                    foreach (var proc in Processes)
                    {
                        foreach (var product in proc.InputWants)
                        {
                            if (product.TagData.All(x => x.tag != ProductionTag.Optional))
                                _capitalWants.Add(product.Want, product.Amount);
                        }
                    }
                }

                return _capitalWants;
            }
        }

        public IReadOnlyDictionary<Want, decimal> OptionalCapitalWants
        {
            get
            {
                if (_optionalCapitalWants == null)
                {
                    _optionalCapitalWants = new Dictionary<Want, decimal>();
                    foreach (var proc in Processes)
                    {
                        foreach (var product in proc.InputWants)
                        {
                            if (product.TagData.Any(x => x.tag == ProductionTag.Optional))
                                _optionalCapitalWants.Add(product.Want, product.Amount);
                        }
                    }
                }

                return _optionalCapitalWants;
            }
        }

        public IReadOnlyDictionary<Want, decimal> OutputWants
        {
            get
            {
                if (_outputWants == null)
                {
                    _outputWants = new Dictionary<Want, decimal>();
                    foreach (var proc in Processes)
                    {
                        foreach (var product in proc.InputWants)
                        {
                            _outputWants.Add(product.Want, product.Amount);
                        }
                    }
                }

                return _outputWants;
            }
        }

        #endregion

        public void Refresh()
        {
            _inputProducts = null;
            _optionalInputProducts = null;
            _capitalProducts = null;
            _optionalCapitalProducts = null;
            _outputProducts = null;
            
            _inputWants = null;
            _optionalInputWants = null;
            _capitalWants = null;
            _optionalCapitalWants = null;
            _outputWants = null;
        }
        
        public string GetName()
        {
            if (!string.IsNullOrWhiteSpace(VariantName))
                return $"{Name}({VariantName})";
            return Name;
        }
    }
}
