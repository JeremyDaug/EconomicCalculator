using EconomicCalculator.Storage.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage
{
    /// <summary>
    /// Storage for products and number counts for them.
    /// Collection is not open to editing.
    /// </summary>
    /// TODO: Consider replacing the Tuple in IEnumerable with a ProductAmountPair class.
    public interface IReadOnlyProductAmountCollection : IEnumerable<Tuple<IProduct, double>>
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
        /// Retrieve a products from the list.
        /// If the collection doesn't contain the item, it includes it with a value of 0.
        /// </summary>
        /// <param name="products">The Sought Products.</param>
        /// <returns>The Subset of items sought.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="products"/> is null.
        /// </exception>
        IProductAmountCollection GetProducts(IList<IProduct> products);

        /// <summary>
        /// Retrieve a count for an item in the collection
        /// </summary>
        /// <param name="product">The Product to Find.</param>
        /// <returns>The amount of the item attached.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="product"/> is null.
        /// </exception>
        /// <exception cref="KeyNotFoundException">
        /// If product is not found in the dictionary.
        /// </exception>
        double GetProductValue(IProduct product);

        /// <summary>
        /// Try to retrieve a count for an item in the collection.
        /// </summary>
        /// <param name="product">The product to find.</param>
        /// <param name="count">The Count of the Product.</param>
        /// <returns>True if successful, false otherwise.</returns>
        bool TryGetProductValue(IProduct product, out double count);

        /// <summary>
        /// Creates a copy of the product amount collection.
        /// </summary>
        /// <returns>A copy of the Collection.</returns>
        IProductAmountCollection Copy();

        /// <summary>
        /// Multiplies the entire collection of items by a scalar.
        /// </summary>
        /// <param name="value">The scalar to multiply.</param>
        /// <returns>A new collection of multiplied values.</returns>
        IProductAmountCollection Multiply(double value);

        /// <summary>
        /// Multiplies the current collection by another on a product by product basis.
        /// </summary>
        /// <param name="a">The collection to add.</param>
        /// <returns>The resulting multiplication.</returns>
        IProductAmountCollection MultiplyBy(IProductAmountCollection other);

        /// <summary>
        /// Checks that teh collection contains a product.
        /// </summary>
        /// <param name="product">The product being sought.</param>
        /// <returns>
        /// True if it contains it in the collection, regardless of the value.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// if <paramref name="product"/> is null.
        /// </exception>
        bool Contains(IProduct product);

        /// <summary>
        /// Returns a duplicate of the Collection, ordered buy the requested action 
        /// in ascending order.
        /// </summary>
        /// <param name="action">How to organize the products.</param>
        /// <returns>The collection, organized in ascending order.</returns>
        IProductAmountCollection OrderProductsBy(Func<IProduct, object> func);

        /// <summary>
        /// The number of products in the collection.
        /// </summary>
        /// <returns>The number of items in the collection.</returns>
        int Count();
    }
}
