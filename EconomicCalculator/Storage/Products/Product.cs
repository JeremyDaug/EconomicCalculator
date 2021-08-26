using EconomicCalculator.Storage.ProductTags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
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
            Wants = new Dictionary<int, decimal>();
            WantStrings = new List<string>();
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
            Wants = new Dictionary<int, decimal>(old.Wants);
            WantStrings = new List<string>(old.WantStrings);
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

        /// <summary>
        /// The wants the product satisfies. The Key is the Want Id.
        /// The Value is the amount satisfied per unit.
        /// </summary>
        public Dictionary<int, decimal>  Wants { get; set; }

        /// <summary>
        /// Alternative storage method for wants.
        /// Ordered by Id number of the want.
        /// </summary>
        public List<string> WantStrings { get; set; }

        /// <summary>
        /// A Helper to view the want strings.
        /// Ordered in the same way as <seealso cref="WantStrings"/>.
        /// </summary>
        [JsonIgnore]
        public string WantString
        {
            get
            {
                var result = "";
                foreach (var s in WantStrings)
                    result += s + ";";
                return result;
            }
        }

        /// <summary>
        /// String form of our tags.
        /// </summary>
        public List<string> TagStrings { get; set; }

        /// <summary>
        /// Product Tags in proper form.
        /// </summary>
        [JsonIgnore]
        public List<IAttachedProductTag> Tags { get; }

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

        /// <summary>
        /// Characters a product's name and variant name cannot accept.
        /// <> () [] ; , . \whitespace
        /// </summary>
        public static char[] InvalidNameCharacters
        {
            get
            {

                string val = "<>;";

                return val.ToCharArray();
            }
        }

        /// <summary>
        /// Given a name, it returns it and any variant name contained in a string.
        /// </summary>
        /// <param name="name">The name(vairant) name we are processing.</param>
        /// <returns></returns>
        public static Tuple<string, string> GetProductNames(string name)
        {
            if (name.Contains("("))
            {
                var prodNames = name.Split('(');
                var prodName = prodNames[0];
                var varName = prodNames[1].TrimEnd(')');
                return new Tuple<string, string>(prodName, varName);
            }
            else
                return new Tuple<string, string>(name, null);
        }
    }
}
