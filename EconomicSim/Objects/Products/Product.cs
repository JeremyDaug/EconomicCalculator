using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconomicSim.Helpers;
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
    internal class Product : IProduct
    {
        public Product()
        {
            ProductTags = new Dictionary<ProductTag, Dictionary<string, object>>();
            Wants = new List<(IWant want, decimal amount)>();
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
        public string Name { get; set; }

        /// <summary>
        /// The Variant Name of the product, may be empty.
        /// </summary>
        /// <remarks>
        /// Name and VariantName should be a unique combo.
        /// </remarks>
        public string VariantName { get; set; }

        /// <summary>
        /// The name of the unit the product is measured in.
        /// </summary>
        public string UnitName { get; set; }

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
        public string Icon { get; set; }

        /// <summary>
        /// THe tags of the product.
        /// </summary>
        public Dictionary<ProductTag, Dictionary<string, object>> ProductTags { get; set; }
        IReadOnlyDictionary<ProductTag, Dictionary<string, object>> IProduct.ProductTags => ProductTags;

        /// <summary>
        /// What wants the product satisfies by just owning.
        /// Item1 is the want it applies to
        /// Item2 is how much it gives (or takes)
        /// </summary>
        public List<(IWant want, decimal amount)> Wants { get; set; }

        IReadOnlyList<(IWant want, decimal amount)> IProduct.Wants => Wants;

        #region RelatedProcesses

        /// <summary>
        /// The processes related to this product.
        /// IE, Failure, Use, Consumption, and Maintenance processes.
        /// product.
        /// </summary>
        public List<IProcess> ProductProcesses { get; set; }
        IReadOnlyList<IProcess> IProduct.ProductProcesses => ProductProcesses;

        /// <summary>
        /// The failure Process of the product, may be empty.
        /// </summary>
        public IProcess FailureProcess
        {
            get
            {
                return ProductProcesses.SingleOrDefault(x => x.ProcessTags.Contains(ProcessTag.Failure));
            }
        }

        /// <summary>
        /// The Use Processes of the Product
        /// </summary>
        public IReadOnlyList<IProcess> UseProcesses
        {
            get
            {
                return ProductProcesses.Where(x => x.ProcessTags.Contains(ProcessTag.Use)).ToList();
            }
        }

        /// <summary>
        /// The ways the product can be used.
        /// </summary>
        public IReadOnlyList<IProcess> ConsumptionProcesses
        {
            get
            {
                return ProductProcesses.Where(x => x.ProcessTags.Contains(ProcessTag.Consumption)).ToList();
            }
        }

        /// <summary>
        /// They ways the product can be maintained.
        /// </summary>
        public IReadOnlyList<IProcess> MaintenanceProcesses
        {
            get
            {
                return ProductProcesses.Where(x => x.ProcessTags.Contains(ProcessTag.Maintenance)).ToList();
            }
        }

        #endregion RelatedProcesses

        /// <summary>
        /// The Technology required for this product to be know about or build
        /// may be null.
        /// </summary>
        public ITechnology TechRequirement { get; set; }

        ITechnology IProduct.TechRequirement => TechRequirement;

        public override string ToString()
        {
            return Name + ":" + VariantName;
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

        public bool Equals(Product obj)
        {
            return string.Equals(GetName(), obj.GetName());
        }

        public override int GetHashCode()
        {
            return GetName().GetHashCode();
        }
    }
}
