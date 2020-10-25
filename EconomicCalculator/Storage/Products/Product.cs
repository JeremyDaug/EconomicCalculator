using EconomicCalculator.Enums;
using EconomicCalculator.Generators;
using EconomicCalculator.Randomizer;
using EconomicCalculator.Storage;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace EconomicCalculator.Storage.Products
{
    /// <summary>
    /// Products are goods which can be bought, sold, and consumed.
    /// </summary>
    internal class Product : IProduct
    {
        private readonly IRandomizer randomizer;

        #region GeneralData

        /// <summary>
        /// The Unique identifier of the product.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The name of the product.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A unique name to differentiate between the same product made through
        /// different means, in different locations, or by methods.
        /// </summary>
        public string VariantName { get; set; }

        /// <summary>
        /// What is the name or type of unit used to buy and
        /// sell this product.
        /// </summary> 
        public string UnitName { get; set; }

        /// <summary>
        /// The quality of the item
        /// </summary>
        public int Quality { get; set; }

        /// <summary>
        /// The default/starting price of the product.
        /// </summary>
        public double DefaultPrice { get; set; }

        /// <summary>
        /// The type of product this is.
        /// </summary>
        public ProductTypes ProductType { get; set; }

        /// <summary>
        /// If the product allows for maintenance.
        /// </summary>
        public bool Maintainable { get; set; }

        /// <summary>
        /// Can the good can be divided effectively indefinitely.
        /// </summary>
        public bool Fractional { get; set; }

        #endregion GeneralData

        #region FailureData

        /// <summary>
        /// The Average time for the item to break irrepairably.
        /// 0 cannot be stored beyond at all.
        /// -1 never breaks down.
        /// </summary>
        public int MTTF { get; set; }

        /// <summary>
        /// The chance that on any given day, the product will break.
        /// Calculated from <see cref="MTTF"/>.
        /// </summary>
        public double DailyFailureChance => 1.0d / MTTF;

        /// <summary>
        /// A unit of the product, when it fails or is consumed, turns into these products,
        /// and how much of each.
        /// </summary>
        /// <remarks>Services don't turn into</remarks>
        public IProductAmountCollection FailsInto { get; set; }

        #endregion FailureData

        #region FailureFunctions

        /// <summary>
        /// Takes a number of failed products, and returns the resulting outputs.
        /// </summary>
        /// <param name="failedProducts">How many failed products there are.</param>
        /// <returns>The products that have been failed into.</returns>
        public IProductAmountCollection FailureResults(double failedProducts)
        {
            return FailsInto.Multiply(failedProducts);
        }

        /// <summary>
        /// The probability that after a number of days, failure will occur.
        /// </summary>
        /// <param name="days">How many days to calculate for.</param>
        /// <returns>The probability that the product breaks.</returns>
        public double FailureProbability(int days)
        {
            return FailureProbability(days, 1);
        }

        /// <summary>
        /// The probability that after a number of days, failure will occur.
        /// </summary>
        /// <param name="days">How many days to calculate for.</param>
        /// <param name="MaintenanceMet">The percent of maintenance carried out.</param>
        /// <returns>The probability that the product breaks.</returns>
        public double FailureProbability(int days, double MaintenanceMet)
        {
            if (days < 1)
                throw new ArgumentOutOfRangeException("Parameter 'days' cannot be less than 1.");
            if (MTTF <= 1)
                return 0;

            // Not meeting maintenance doubles the failure chance.
            var chanceToNotHappen = 1 - (DailyFailureChance * (2 - MaintenanceMet));
            return 1 - Math.Pow(chanceToNotHappen, days);
        }

        /// <summary>
        /// Products destroyed by decay.
        /// </summary>
        /// <param name="amount">The amount being checked against.</param>
        /// <returns>The number of failed products.</returns>
        public double FailedProducts(double amount)
        {
            // The logic is the same if we set MaintenanceMet to 1.
            return FailedProducts(amount, 1);
        }

        /// <summary>
        /// Products destroyed by decay.
        /// </summary>
        /// <param name="amount">The amount being checked against.</param>
        /// <param name="MaintenanceMet">The percent of maintenance met.</param>
        /// <returns>The number of failed products.</returns>
        public double FailedProducts(double amount, double MaintenanceMet)
        {
            // get the current failure chance, modified by maintenance met.
            var currentFailChance = DailyFailureChance * (2 - MaintenanceMet);

            // Get the percent of failure today,
            // stopping at zero so new items don't magically come into existance.
            var normalizedFailure = Math.Max(randomizer.Normal(1, 1) * currentFailChance, 0);

            // How many failed today.
            var randFailures = Math.Floor(normalizedFailure * amount);

            // Get an additional base chance that something break to push towards failure.
            var baseResult = randomizer.NextDouble() >= currentFailChance ? 0 : 1;

            // Return the sum of our random Failures and our base, capping at the total amount for safety.
            return Math.Min(amount, randFailures + baseResult);
        }

        #endregion FailureFunctions

        #region MaintenanceData

        /// <summary>
        /// The maintenance required to keep a unit of the product working.
        /// Products used in maintenance must be consumed.
        /// This is added to the needs of anyone who holds the product.
        /// Not meeting maintenance increases failure chance.
        /// </summary>
        public IProductAmountCollection Maintenance { get; set; }

        #endregion MaintenanceData

        #region MaintenanceFunctions

        /// <summary>
        /// Runs maintenance on a number of products.
        /// </summary>
        /// <param name="products">The number of products we are maintaining.</param>
        /// <param name="MaintenanceProducts">The products being used to maintain the products.</param>
        /// <param name="Satisfaction">How much was satisfied.</param>
        /// <returns>The items consumed.</returns>
        public IProductAmountCollection RunMaintenance(double products, IProductAmountCollection MaintenanceProducts, out double Satisfaction)
        {
            // null check
            if (MaintenanceProducts is null)
                throw new ArgumentNullException(nameof(MaintenanceProducts));
            // if Maintenance is empty, return empty collection, and set satisfaction to 1.
            if (Maintenance.Count() == 0)
            {
                Satisfaction = 1;
                return new ProductAmountCollection();
            }

            // Get from all available maintenance goods, the exact goods we're looking for.
            var availableGoods = MaintenanceProducts.GetProducts(Maintenance.Products.ToList());

            // Get how many maintenance products are actually needed.
            var requiredGoods = Maintenance.Multiply(products);

            // Start the cumulative satisfaction.
            var cumulativeSat = 0.0;

            // Create our return collection
            var result = new ProductAmountCollection();

            // go through the available goods.
            foreach (var pair in availableGoods)
            {
                // for the product, available amount, and required amount
                var product = pair.Item1;
                var available = pair.Item2;
                var required = requiredGoods.GetProductValue(product);

                // get how well the good was satisfied
                var sat = Math.Min(1, available / required);

                // to the cumulative satisfaction, multiplying by the requried amount to weight it correctly.
                cumulativeSat += sat * required;

                // and add how much was consumed, choosing either available or required amount.
                if (sat == 1)
                    result.AddProducts(product, required);
                else
                    result.AddProducts(product, available);
            }

            // average out the cumulative satisfaction by all of the required goods.
            // if all are fully satisfied, then Satisfaction should be 1.
            // Products which have higher requirements should have a larger impact on the average.
            Satisfaction = cumulativeSat / requiredGoods.Sum(x => x.Item2);

            // return our result.
            return result;
        }

        #endregion MaintenanceFunctions

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public Product(IRandomizer randomizer)
        {
            Id = Guid.NewGuid();
            DefaultPrice = 1;
            UnitName = "Unit";
            MTTF = 0;
            ProductType = ProductTypes.Good;

            this.randomizer = randomizer;
        }

        public bool Equals(IProduct other)
        {
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            var result = string.Format("Name: {0}\n" +
                "Variant Name: {1}\n" +
                "Product Units: {2}\n" +
                "Default Price: {3}\n" +
                "Quality: {4}\n" +
                "Mean Time To Failure: {5}\n" +
                "Fractional Item: {6}\n",
                Name, VariantName, UnitName, DefaultPrice, Quality, MTTF);

            result += "--------------------\n";

            return result;
        }

        public void LoadFromSql(SqlConnection connection)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IProduct x, IProduct y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(IProduct obj)
        {
            return Id.GetHashCode();
        }
    }
}
