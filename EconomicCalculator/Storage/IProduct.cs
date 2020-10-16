using EconomicCalculator.Enums;
using System;
using System.Collections.Generic;

namespace EconomicCalculator.Storage
{
    /// <summary>
    /// Products are goods which can be bought, sold, and consumed.
    /// </summary>
    public interface IProduct : IEquatable<IProduct>, ISqlReader, IEqualityComparer<IProduct>
    {
        /// <summary>
        /// The Unique identifier of the product.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// The name of the product.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// A unique name to differentiate between the same product made through
        /// different means, in different locations, or by methods.
        /// </summary> 
        string VariantName { get; }

        /// <summary>
        /// What is the name or type of unit used to buy and
        /// sell this product.
        /// </summary>
        string UnitName { get; }

        /// <summary>
        /// The quality of the item
        /// </summary>
        int Quality { get; }

        /// <summary>
        /// The default/starting price of the product.
        /// </summary>
        double DefaultPrice { get; }

        /// <summary>
        /// The Average time for the item to break irrepairably.
        /// 0 cannot be stored beyond at all.
        /// -1 never breaks down.
        /// </summary>
        int MTTF { get; }

        /// <summary>
        /// The type of product this is.
        /// </summary>
        ProductTypes ProductType { get; }

        /// <summary>
        /// Can th good can be divided effectively indefinitely.
        /// </summary>
        bool Fractional { get; }

        /// <summary>
        /// The chance that on any given day, the product will break. Calculated from <see cref="MTTF"/>.
        /// </summary>
        double DailyFailureChance { get; }

        /// <summary>
        /// The probability that after a number of days, failure will occur.
        /// </summary>
        /// <param name="days">How many days to calculate for.</param>
        /// <returns>The probability that the product breaks.</returns>
        double FailureProbability(int days);

        /// <summary>
        /// Products destroyed by decay.
        /// </summary>
        /// <param name="amount">The amount being checked against.</param>
        /// <returns>The number of failed products.</returns>
        double FailedProducts(double amount);
    }
}