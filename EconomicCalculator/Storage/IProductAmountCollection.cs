﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconomicCalculator.Intermediaries;

namespace EconomicCalculator.Storage
{
    /// <summary>
    /// Storage for products and number counts for them.
    /// </summary>
    public interface IProductAmountCollection : IEnumerable<Tuple<IProduct, double>>
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
        /// Creates a copy of the product amount collection.
        /// </summary>
        /// <returns>A copy of the Collection.</returns>
        IProductAmountCollection Copy();

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

        // TODO add IsEmpty() function if need be. It should be easy to do.
        // bool IsEmpty();
    }
}