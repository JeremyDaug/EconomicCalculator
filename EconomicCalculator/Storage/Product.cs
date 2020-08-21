using EconomicCalculator.Enums;
using EconomicCalculator.Generators;
using EconomicCalculator.Storage;
using System;
using System.Data.SqlClient;

namespace EconomicCalculator.Storage
{
    internal class Product : IProduct
    {
        #region Data

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string VariantName { get; set; }

        public double DefaultPrice { get; set; }

        public string UnitName { get; set; }

        public int Quality { get; set; }

        public int MTTF { get; set; }

        public ProductTypes ProductType { get; set; }

        public bool Fractional { get; set; }

        #endregion Data

        public double DailyFailureChance => 1.0d / MTTF;

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public Product()
        {
            Id = Guid.NewGuid();
            DefaultPrice = 1;
            UnitName = "Unit";
            MTTF = 0;
            ProductType = ProductTypes.Good;
        }

        public bool Equals(IProduct other)
        {
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public double FailureProbability(int days)
        {
            if (days < 1)
                throw new ArgumentOutOfRangeException("Parameter 'days' cannot be less than 1.");
            if (MTTF <= 1)
                return 0;

            var chanceToNotHappen = 1 - DailyFailureChance;
            return 1 - Math.Pow(chanceToNotHappen, days);
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
