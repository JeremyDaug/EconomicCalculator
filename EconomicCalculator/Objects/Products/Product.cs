using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconomicCalculator.Objects.Products.ProductTags;
using EconomicCalculator.Objects.Wants;

namespace EconomicCalculator.Objects.Products
{
    /// <summary>
    /// Product Data Class
    /// </summary>
    internal class Product : IProduct
    {
        private List<IProductTagData> _productTags;
        private List<Tuple<IWant, decimal>> _wants;

        public Product()
        {
            _productTags = new List<IProductTagData>();
            _wants = new List<Tuple<IWant, decimal>>();
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
        public IReadOnlyList<IProductTagData> ProductTags { get => _productTags; }

        /// <summary>
        /// What wants the product satisfies by just owning.
        /// Item1 is the want it applies to
        /// Item2 is how much it gives (or takes)
        /// </summary>
        public IReadOnlyList<Tuple<IWant, decimal>> Wants { get => _wants; }
    }
}
