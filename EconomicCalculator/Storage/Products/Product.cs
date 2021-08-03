using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage.Products
{
    /// <summary>
    /// Generic Product Class. Includes all data needed for products.
    /// </summary>
    public class Product : IProduct
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public Product()
        {

        }

        /// <summary>
        /// Copy Constructor.
        /// </summary>
        /// <param name="old">The product to copy from.</param>
        public Product(Product old)
        {
            Id = old.Id;
            Name = old.Name;
            VariantName = old.VariantName;
            UnitName = old.UnitName;
            Quality = old.Quality;
            Mass = old.Mass;
            Bulk = old.Bulk;
            Fractional = old.Fractional;
            Icon = old.Icon;
        }

        /// <summary>
        /// The Unique Id of the Product.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The Name of the Product, cannot be empty.
        /// </summary>
        /// <remarks>
        /// Name and VariantName should be a unique combo among all products.
        /// </remarks>
        public string Name { get; set; }

        /// <summary>
        /// Variant Name of the product. 
        /// </summary>
        /// <remarks>
        /// Name and VariantName should be a unique combo among all products.
        /// </remarks>
        public string VariantName { get; set; }

        /// <summary>
        /// The name of the unit the product is measured in.
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// The Quality of the product, the lower the value, the lower the quality and
        /// vice versa.
        /// </summary>
        public int Quality { get; set; }

        // Default Price? Skip for now, may not need.

        /// <summary>
        /// The Mass of the product in Kg.
        /// </summary>
        public decimal Mass { get; set; }

        /// <summary>
        /// The space the product takes up in m^3.
        /// </summary>
        public decimal Bulk { get; set; }

        /// <summary>
        /// If the product can be divided into subunit sizes.
        /// </summary>
        public bool Fractional { get; set; }

        // TODO Technology Connection Placeholder.

        // TODO, rework Failure and Maintenance into process based methodology.
        /// <summary>
        /// The average time for the product to break in days.
        /// -1 means it never breaks, 0 means it cannot be stored and breaksdown immediately.
        /// 1 means it can be stored but will breakdown immediately.
        /// </summary>
        //public int MTTF { get; set; }

        /// <summary>
        /// The products this product fails into or is consumed into.
        /// The key is the <see cref="Id"/> of the product is fails into.
        /// The Value is the amount it fails into per unit of this product.
        /// </summary>
        //public IReadOnlyDictionary<int, decimal> FailsInto { get; set; }

        /// <summary>
        /// If the product can be maintained.
        /// </summary>
        //public bool Maintainable { get; set; }

        /// <summary>
        /// The products which maintain this product.
        /// The Key is the <see cref="Id"/> of the product which maintains this.
        /// The amount of that product needed per unit of this product.
        /// </summary>
        //public IReadOnlyDictionary<int, decimal> Maintenance { get; }

        /// <summary>
        /// The Icon used by the product.
        /// </summary>
        public string Icon { get; set; }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override string ToString()
        {
            var result = Name;
            if (!string.IsNullOrEmpty(VariantName))
                result += " : " + VariantName;
            return result;
        }
    }
}
