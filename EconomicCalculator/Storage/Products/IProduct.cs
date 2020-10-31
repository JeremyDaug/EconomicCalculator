using EconomicCalculator.Enums;
using System;
using System.Collections.Generic;

namespace EconomicCalculator.Storage.Products
{
    /// <summary>
    /// Products are goods which can be bought, sold, and consumed.
    /// </summary>
    public interface IProduct : IEquatable<IProduct>, ISqlReader, IEqualityComparer<IProduct>
    {
        #region GeneralData

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
        /// How much the product weighs, used primarily for transportation, in Kg.
        /// </summary>
        double Mass { get; }

        // TODO, consider adding a Mass Breakdown to the product.

        /// <summary>
        /// How much space it takes up, used in both storage and transportation.
        /// </summary>
        double Bulk { get; }

        /// <summary>
        /// The type of product this is.
        /// </summary>
        ProductTypes ProductType { get; }

        /// <summary>
        /// If the product allows for maintenance.
        /// </summary>
        bool Maintainable { get; }

        /// <summary>
        /// Can the good can be divided effectively indefinitely.
        /// </summary>
        bool Fractional { get; }

        #endregion GeneralData

        #region FailureData

        /// <summary>
        /// The Average time for the item to break irrepairably.
        /// 0 cannot be stored beyond at all.
        /// -1 never breaks down.
        /// </summary>
        int MTTF { get; }

        /// <summary>
        /// The chance that on any given day, the product will break.
        /// Calculated from <see cref="MTTF"/>.
        /// </summary>
        double DailyFailureChance { get; }

        /// <summary>
        /// A unit of the product, when it fails or is consumed, turns into these products,
        /// and how much of each.
        /// </summary>
        /// <remarks>Services don't turn into</remarks>
        IProductAmountCollection FailsInto { get; }

        #endregion FailureData

        #region FailureFunctions

        /// <summary>
        /// Takes a number of failed products, and returns the resulting outputs.
        /// </summary>
        /// <param name="failedProducts">How many failed products there are.</param>
        /// <returns>The products that have been failed into.</returns>
        IProductAmountCollection FailureResults(double failedProducts);

        /// <summary>
        /// The probability that after a number of days, failure will occur.
        /// </summary>
        /// <param name="days">How many days to calculate for.</param>
        /// <returns>The probability that the product breaks.</returns>
        double FailureProbability(int days);

        /// <summary>
        /// The probability that after a number of days, failure will occur.
        /// </summary>
        /// <param name="days">How many days to calculate for.</param>
        /// <param name="MaintenanceMet">The percent of maintenance carried out.</param>
        /// <returns>The probability that the product breaks.</returns>
        double FailureProbability(int days, double MaintenanceMet);

        /// <summary>
        /// Products destroyed by decay.
        /// </summary>
        /// <param name="amount">The amount being checked against.</param>
        /// <returns>The number of failed products.</returns>
        double FailedProducts(double amount);

        /// <summary>
        /// Products destroyed by decay.
        /// </summary>
        /// <param name="amount">The amount being checked against.</param>
        /// <param name="MaintenanceMet">The percent of maintenance met.</param>
        /// <returns>The number of failed products.</returns>
        double FailedProducts(double amount, double MaintenanceMet);

        #endregion FailureFunctions

        #region MaintenanceData

        /// <summary>
        /// The maintenance required to keep a unit of the product working.
        /// Products used in maintenance must be consumed.
        /// This is added to the needs of anyone who holds the product.
        /// Not meeting maintenance increases failure chance.
        /// </summary>
        IProductAmountCollection Maintenance { get; }

        // TODO, consider adding unmaintained failure chance.
        // The current system just doubles the failure chance if unmaintained.

        #endregion MaintenanceData

        #region MaintenanceFunctions

        /// <summary>
        /// Runs maintenance on a number of products.
        /// </summary>
        /// <param name="products">The number of products we are maintaining.</param>
        /// <param name="MaintenanceProducts">The products being used to maintain the products.</param>
        /// <param name="Satisfaction">How much was satisfied.</param>
        /// <returns>The items consumed.</returns>
        IProductAmountCollection RunMaintenance(double products, IReadOnlyProductAmountCollection MaintenanceProducts, out double Satisfaction);

        #endregion MaintenanceFunctions
    }
}