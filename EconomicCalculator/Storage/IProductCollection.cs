using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage
{
    /// <summary>
    /// Storage for products and number counts for them.
    /// </summary>
    public interface IProductAmountCollection
    {
        /// <summary>
        /// The products in the collection.
        /// </summary>
        IReadOnlyList<IProduct> Products { get; }

        /// <summary>
        /// Product Guids to the values.
        /// </summary>
        IReadOnlyDictionary<Guid, double> ProductDict { get; }

        /// <summary>
        /// Sets a product amount to the given value.
        /// </summary>
        /// <param name="product">The product to set.</param>
        /// <param name="value">The value to set the product at.</param>
        /// <remarks>
        /// If product does not exist, the item will be added.
        /// </remarks>
        void SetProductAmount(IProduct product, double value);

        /// <summary>
        /// Retrieve a count for an item in the collection
        /// </summary>
        /// <param name="toFind">The Product to Find.</param>
        /// <returns>The amount of the item attached.</returns>
        /// <exception cref="ArgumentNullException">
        /// If product is null.
        /// </exception> 
        double GetProductAmount(IProduct toFind);

        /// <summary>
        /// Adds a product to the collection with a connected value of zero.
        /// </summary>
        /// <param name="product">The product to include.</param>
        /// <exception cref="ArgumentNullException">
        /// If product is null.
        /// </exception>
        void IncludeProduct(IProduct product);

        /// <summary>
        /// Add a product or number of products to the collection.
        /// If product already exists, then it is added to the existing value.
        /// </summary>
        /// <param name="product">The product to add or add to.</param>
        /// <param name="value">The amount to add.</param>
        /// <exception cref="ArgumentNullException">
        /// If product is null.
        /// </exception>
        void AddProducts(IProduct product, double value);

        /// <summary>
        /// Removes a number of products from the collection.
        /// </summary>
        /// <remarks>
        /// Will not remove or delete products even if they go into the negative.
        /// </remarks>
        /// <param name="product">The product to remove from.</param>
        /// <param name="value">The amount to remove.</param>
        /// <exception cref="ArgumentNullException">
        /// If product is null.
        /// </exception>
        /// <exception cref="KeyNotFoundException">
        /// The product does not exist in the collection.
        /// </exception>
        void RemoveProducts(IProduct product, double value);

        /// <summary>
        /// Deletes a product entirely from the collectino.
        /// </summary>
        /// <param name="product">The product to delete.</param>
        /// <exception cref="ArgumentNullException">
        /// Product is null.
        /// </exception>
        /// <exception cref="KeyNotFoundException">
        /// The product doesn't exist in the collection.
        /// </exception>
        void DeleteProduct(IProduct product);
    }
}
