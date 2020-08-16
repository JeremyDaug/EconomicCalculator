using EconomicCalculator.Intermediaries;

namespace EconomicCalculator.Generators
{
    /// <summary>
    /// A product and how much of it there is in this instance.
    /// </summary>
    public interface IProductCount
    {
        /// <summary>
        /// The number of the product.
        /// </summary>
        double Count { get; set; }

        /// <summary>
        /// The product being counted.
        /// </summary>
        IProduct Product { get; set; }
    }
}