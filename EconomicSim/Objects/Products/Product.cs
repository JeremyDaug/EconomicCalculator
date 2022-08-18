using System.Text.Json.Serialization;
using EconomicSim.Objects.Processes;
using EconomicSim.Objects.Processes.ProcessTags;
using EconomicSim.Objects.Products.ProductTags;
using EconomicSim.Objects.Technology;
using EconomicSim.Objects.Wants;

namespace EconomicSim.Objects.Products
{
    /// <summary>
    /// Product Data Class
    /// </summary>
    [JsonConverter(typeof(ProductJsonConverter))]
    public class Product : IProduct
    {
        public Product()
        {
            ProductTags = new List<(ProductTag tag, Dictionary<string, object>? parameters)>();
            Wants = new Dictionary<IWant, decimal>();
            ProductProcesses = new List<IProcess>();
        }

        /// <summary>
        ///  It's unique id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The Name of the product, cannot be empty.
        /// </summary>
        /// <remarks>
        /// Name and Variant Name should be a unique combo.
        /// </remarks>
        public string Name { get; set; } = "";

        /// <summary>
        /// The Variant Name of the product, may be empty.
        /// </summary>
        /// <remarks>
        /// Name and VariantName should be a unique combo.
        /// </remarks>
        public string VariantName { get; set; } = "";

        /// <summary>
        /// The name of the unit the product is measured in.
        /// </summary>
        public string UnitName { get; set; } = "";

        /// <summary>
        /// The quality of the product. Related to how luxurious or
        /// cheap it is seen as.
        /// </summary>
        public int Quality { get; set; }

        /// <summary>
        /// The mass of the product in Kg.
        /// </summary>
        public decimal Mass { get; set; }

        /// <summary>
        /// The space the object takes up in m^3.
        /// </summary>
        public decimal Bulk { get; set; }

        /// <summary>
        /// Whether the product can be devided into decmial units.
        /// </summary>
        public bool Fractional { get; set; }

        /// <summary>
        /// The location of the Product's Icon.
        /// </summary>
        public string Icon { get; set; } = "";

        /// <summary>
        /// THe tags of the product.
        /// </summary>
        public List<(ProductTag tag, Dictionary<string, object>? parameters)> ProductTags { get; set; }
        IReadOnlyList<(ProductTag tag, Dictionary<string, object>? parameters)> IProduct.ProductTags => ProductTags;

        /// <summary>
        /// What wants the product satisfies by just owning.
        /// Item1 is the want it applies to
        /// Item2 is how much it gives (or takes)
        /// </summary>
        public Dictionary<IWant, decimal> Wants { get; set; }

        IReadOnlyDictionary<IWant, decimal> IProduct.Wants => Wants;

        #region RelatedProcesses

        /// <summary>
        /// The processes related to this product.
        /// IE, Failure, Use, Consumption, and Maintenance processes.
        /// product.
        /// </summary>
        public List<IProcess> ProductProcesses { get; }
        IReadOnlyList<IProcess> IProduct.ProductProcesses => ProductProcesses;

        /// <summary>
        /// The failure Process of the product, may be empty.
        /// </summary>
        public IProcess? FailureProcess
        {
            get
            {
                return ProductProcesses
                    .Where(x => x.ProcessTags.ContainsKey(ProcessTag.Failure))
                    .SingleOrDefault(x => ((Product) x.ProcessTags[ProcessTag.Failure]["Product"]).GetName() == GetName());
            }
        }

        /// <summary>
        /// The Use Processes of the Product
        /// </summary>
        public IReadOnlyList<IProcess> UseProcesses
        {
            get
            {
                return ProductProcesses
                    .Where(x => x.ProcessTags.ContainsKey(ProcessTag.Use))
                    .Where(x => ((Product) x.ProcessTags[ProcessTag.Use]["Product"]).GetName() == GetName())
                    .ToList();
            }
        }

        /// <summary>
        /// The ways the product can be used.
        /// </summary>
        public IReadOnlyList<IProcess> ConsumptionProcesses
        {
            get
            {
                return ProductProcesses
                    .Where(x => x.ProcessTags.ContainsKey(ProcessTag.Consumption))
                    .Where(x => ((Product) x.ProcessTags[ProcessTag.Consumption]["Product"]).GetName() == GetName())
                    .ToList();
            }
        }

        /// <summary>
        /// They ways the product can be maintained.
        /// </summary>
        public IReadOnlyList<IProcess> MaintenanceProcesses
        {
            get
            {
                return ProductProcesses
                    .Where(x => x.ProcessTags.ContainsKey(ProcessTag.Maintenance))
                    .Where(x => ((Product) x.ProcessTags[ProcessTag.Maintenance]["Product"]).GetName() == GetName())
                    .ToList();
            }
        }

        #endregion RelatedProcesses

        /// <summary>
        /// The Technology required for this product to be know about or build
        /// may be null.
        /// </summary>
        public ITechnology? TechRequirement { get; set; }

        ITechnology? IProduct.TechRequirement => TechRequirement;

        public override string ToString()
        {
            return GetName();
        }
        
        public string GetName()
        {
            // if has variant name, add that to name.
            if (!string.IsNullOrWhiteSpace(VariantName))
                return Name + "(" + VariantName + ")";
            return Name;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Product);
        }

        public bool Equals(Product? obj)
        {
            if (obj == null)
                return false;
            return string.Equals(GetName(), obj.GetName());
        }

        public override int GetHashCode()
        {
            return GetName().GetHashCode();
        }

        public static Product ServiceExample()
        {
            return new Product
            {
                Name = "Service",
                VariantName = "Variant",
                UnitName = "Man Hour",
                Bulk = 0,
                Mass = 0,
                Quality = 1,
                ProductTags = new List<(ProductTag tag, Dictionary<string, object>? parameters)>
                {
                    ( ProductTag.Service, null )
                }, 
                Fractional = true
            };
        }
    }
}
