using EconomicCalculator.Enums;
using EconomicCalculator.Intermediaries;
using System.Collections.Generic;

namespace EconomicCalculator.Generators
{
    public interface ICrops
    {
        /// <summary>
        /// The name of the crop
        /// </summary>
        string Name { get; }

        /// <summary>
        /// A variant name for a specific instance of a crop or field.
        /// </summary> Todo Implement Later
        // string VariantName { get; }

        /// <summary>
        /// The type of crop
        /// </summary>
        CropType CropType { get;  }

        /// <summary>
        /// How many units of seed is needed to plant. If -1, it's perenial, if 0 it's minimal.
        /// </summary>
        double Planting { get; }

        /// <summary>
        /// The Seed product (if any), may be null if Planting is -1 or 0.
        /// </summary>
        IProduct Seed { get; }

        /// <summary>
        /// What products are produced by the crop. Must have at least 1.
        /// </summary>
        IList<IProduct> HarvestProducts { get; }

        /// <summary>
        /// How much of each product is produced.
        /// Key is product name, Value is how many units of that product are produced.
        /// All products in <see cref="HarvestProducts"/> must be represented here.
        /// </summary>
        IDictionary<string, double> HarvestAmounts { get; }

        #region Capital

        // TODO implement later.
        /// <summary>
        /// The capital required to plant, care for, and harvest a crop. May be empty.
        /// </summary> 
        // IList<IProduct> Capital { get; }

        /// <summary>
        /// The unit requirements of the capital.
        /// </summary>
        // IDictionary<string, double> CapitalRequirements { get; }

        #endregion Capital

        /// <summary>
        /// How much, and what skill level, of labor
        /// required to work an acre of land measured in days per crop cycle.
        /// </summary>
        double LaborRequirements { get; }

        /// <summary>
        /// How long a crop takes to grow from planting to
        /// harvest, measured in days.
        /// </summary>
        double CropLifecycle { get; }
    }
}