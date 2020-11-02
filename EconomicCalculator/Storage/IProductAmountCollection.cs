using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconomicCalculator.Storage.Products;

namespace EconomicCalculator.Storage
{
    /// <summary>
    /// Storage for products and number counts for them.
    /// </summary>
    public interface IProductAmountCollection : IReadOnlyProductAmountCollection
    {
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
        /// Adds a product to the collection with a connected value of zero.
        /// </summary>
        /// <param name="product">The product to include.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="product"/> is null.
        /// </exception>
        void IncludeProduct(IProduct product);

        /// <summary>
        /// Adds a product to the collection with a connected value of zero.
        /// </summary>
        /// <param name="products">The products to include.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="products"/> is null.
        /// </exception>
        void IncludeProducts(IList<IProduct> products);

        /// <summary>
        /// Add a product or number of products to the collection.
        /// If product already exists, then it is added to the existing value.
        /// </summary>
        /// <param name="product">The product to add or add to.</param>
        /// <param name="value">The amount to add.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="product"/> is null.
        /// </exception>
        void AddProducts(IProduct product, double value);

        /// <summary>
        /// Add products from another collection to this one.
        /// </summary>
        /// <param name="products">The products to add.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="products"/> or any product in the list is null.
        /// </exception>
        void AddProducts(IProductAmountCollection products);

        /// <summary>
        /// Removes a number of products from the collection.
        /// </summary>
        /// <remarks>
        /// Will not remove or delete products even if they go into the negative.
        /// </remarks>
        /// <param name="product">The product to remove from.</param>
        /// <param name="value">The amount to remove.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="product"/> is null.
        /// </exception>
        void SubtractProducts(IProduct product, double value);

        /// <summary>
        /// Deletes a product entirely from the collectino.
        /// </summary>
        /// <param name="product">The product to delete.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="product"/> is null.
        /// </exception>
        /// <exception cref="KeyNotFoundException">
        /// The <paramref name="product"/> doesn't exist in the collection.
        /// </exception>
        void DeleteProduct(IProduct product);
    }
}
