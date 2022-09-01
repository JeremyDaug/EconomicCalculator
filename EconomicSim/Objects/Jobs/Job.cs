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
        private Dictionary<IProduct, decimal>? _inputProducts;
        private Dictionary<IProduct, decimal>? _optionalInputProducts;
        private Dictionary<IProduct, decimal>? _capitalProducts;
        private Dictionary<IProduct, decimal>? _optionalCapitalProducts;
        private Dictionary<IProduct, decimal>? _outputProducts;
        
        private Dictionary<IWant, decimal>? _inputWants;
        private Dictionary<IWant, decimal>? _optionalInputWants;
        private Dictionary<IWant, decimal>? _capitalWants;
        private Dictionary<IWant, decimal>? _optionalCapitalWants;
        private Dictionary<IWant, decimal>? _outputWants;

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

        public IReadOnlyDictionary<IProduct, decimal> InputProducts
        {
            get
            {
                if (_inputProducts == null)
                {
                    _inputProducts = new Dictionary<IProduct, decimal>();
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
        public IReadOnlyDictionary<IProduct, decimal> OptionalInputProducts
        {
            get
            {
                if (_optionalInputProducts == null)
                {
                    _optionalInputProducts = new Dictionary<IProduct, decimal>();
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
        public IReadOnlyDictionary<IProduct, decimal> CapitalProducts
        {
            get
            {
                if (_capitalProducts == null)
                {
                    _capitalProducts = new Dictionary<IProduct, decimal>();
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
        public IReadOnlyDictionary<IProduct, decimal> OptionalCapitalProducts
        {
            get
            {
                if (_optionalCapitalProducts == null)
                {
                    _optionalCapitalProducts = new Dictionary<IProduct, decimal>();
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
        public IReadOnlyDictionary<IProduct, decimal> OutputProducts
        {
            get
            {
                if (_outputProducts == null)
                {
                    _outputProducts = new Dictionary<IProduct, decimal>();
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

        public IReadOnlyDictionary<IWant, decimal> InputWants
        {
            get
            {
                if (_inputWants == null)
                {
                    _inputWants = new Dictionary<IWant, decimal>();
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

        public IReadOnlyDictionary<IWant, decimal> OptionalInputWants
        {
            get
            {
                if (_optionalInputWants == null)
                {
                    _optionalInputWants = new Dictionary<IWant, decimal>();
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

        public IReadOnlyDictionary<IWant, decimal> CapitalWants
        {
            get
            {
                if (_capitalWants == null)
                {
                    _capitalWants = new Dictionary<IWant, decimal>();
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

        public IReadOnlyDictionary<IWant, decimal> OptionalCapitalWants
        {
            get
            {
                if (_optionalCapitalWants == null)
                {
                    _optionalCapitalWants = new Dictionary<IWant, decimal>();
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

        public IReadOnlyDictionary<IWant, decimal> OutputWants
        {
            get
            {
                if (_outputWants == null)
                {
                    _outputWants = new Dictionary<IWant, decimal>();
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
