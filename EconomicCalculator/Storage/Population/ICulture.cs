using EconomicCalculator.Enums;
using EconomicCalculator.Storage.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage.Population
{
    /// <summary>
    /// The Population Culture, defines features of a population
    /// </summary>
    public interface ICulture
    {
        /// <summary>
        /// The id of the Culture
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// The name of the culture Group.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The rate at which people of this culture multiply naturally.
        /// </summary>
        double CultureGrowthRate { get; }

        #region Needs

        /// <summary>
        /// What needs are needed by the population, 
        /// product may only be used once for any need.
        /// </summary>
        IReadOnlyProductAmountCollection Needs { get; }

        /// <summary>
        /// The type of need the product fulfills.
        /// </summary>
        IReadOnlyDictionary<Guid, NeedType> NeedTypes { get; }

        // TODO, consider section for needs that are limited to
        // subtypes of the population.

        #endregion Needs
    }
}
