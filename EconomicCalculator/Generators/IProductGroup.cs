using EconomicCalculator.Intermediaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Generators
{
    /// <summary>
    /// A collector and manager for groups of like goods.
    /// Allows and creates similar goods of differing qualities or variants of the same good.
    /// </summary>
    public interface IProductGroup
    {
        /// <summary>
        /// The Unique Id of the product group, used for quick navigation.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// The name of the products in the group.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The unit of measure the products use.
        /// </summary>
        string UnitName { get; }

        /// <summary>
        /// The Collective quality of the group, representing it's desirability
        /// relative to Substitute Good Groups.
        /// </summary>
        int CollectiveQuality { get; }

        /// <summary>
        /// The minimum quality the good can be, cannot be below 0.
        /// </summary>
        int MinimumQuality { get; }

        /// <summary>
        /// The highest quality the good can be,
        /// should not be higher than the Maximum Quality Level Setting.
        /// </summary>
        int MaximumQuality { get; }

        /// <summary>
        /// The current prices of each item at each level of quality.
        /// If a level doesn't exist, the value does not matter.
        /// </summary>
        IList<double> CurrentPrices { get; }

        /// <summary>
        /// The Mean Time to Failure of the products based on their quality.
        /// </summary>
        IList<int> MTTF { get; }

        /// <summary>
        /// All valid Alternative Item groups, items from other groups are 
        /// considered equivalent in meeting needs.
        /// </summary>
        IProductSubstituteGroup SubstituteGoods { get; }

        /// <summary>
        /// Retrieves all possible Products of the Group.
        /// </summary>
        IList<IProduct> GetProducts();

        /// <summary>
        /// Retrieve an item of specific quality from the Product Group.
        /// </summary>
        /// <param name="quality">The quality of the item being sought.</param>
        /// <returns>The resulting item.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the item is outside of the allowed quality range.
        /// </exception>
        IProduct GetItemOfQuality(int quality);
    }
}
