using EconomicCalculator.Enums;
using EconomicCalculator.Intermediaries;
using EconomicCalculator.Storage;
using System.Collections.Generic;

namespace EconomicCalculator.Generators
{
    public interface ICrops : IJob
    {
        /// <summary>
        /// The name of the crop
        /// </summary>
        new string Name { get; }

        /// <summary>
        /// A variant name for a specific instance of a crop or field.
        /// </summary> Todo Implement Later
        // string VariantName { get; }

        /// <summary>
        /// The type of crop
        /// </summary>
        CropType CropType { get;  }

        /// <summary>
        /// The Products required to seed the crop every cycle.
        /// </summary>
        IList<IProduct> Seeding { get; }

        /// <summary>
        /// How many units of seed is needed to plant. If -1, it's perenial, if 0 it's minimal.
        /// </summary>
        IDictionary<string, double> Planting { get; }

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
        /// How much labor is required each day to maintain an acre of the crop.
        /// </summary>
        new IList<double> LaborRequirements { get; }

        /// <summary>
        /// How long a crop takes to grow from planting to
        /// harvest, measured in days.
        /// </summary>
        int CropLifecycle { get; }

        /// <summary>
        /// The Averaged output of the crop.
        /// </summary>
        /// <returns>The Output </returns>
        IReadOnlyDictionary<string, double> AveragedDailyOutput();

        /// <summary>
        /// Retrieves the average of a product of the crop.
        /// </summary>
        /// <param name="Product">The name of the product to get.</param>
        /// <returns>The average daily producton of the product.</returns>
        double AveragedDailyOutputFor(string product);
    }
}