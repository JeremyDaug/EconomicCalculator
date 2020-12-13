using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconomicCalculator.Enums;

namespace EconomicCalculator.Storage.Population
{
    internal class Culture : ICulture
    {
        private readonly Dictionary<Guid, NeedType> _needTypes;
        private readonly IProductAmountCollection _needs;

        public Culture()
        {
            Id = Guid.NewGuid();
            _needs = new ProductAmountCollection();
            _needTypes = new Dictionary<Guid, NeedType>();
        }

        /// <summary>
        /// The id of the Culture
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// The name of the culture Group.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The rate at which people of this culture multiply naturally.
        /// </summary>
        public double CultureGrowthRate { get; set; }

        #region Needs

        /// <summary>
        /// What needs are needed by the population, 
        /// product may only be used once for any need.
        /// </summary>
        public IReadOnlyProductAmountCollection Needs => _needs;
        /// <summary>
        /// The type of need the product fulfills.
        /// </summary>
        public IReadOnlyDictionary<Guid, NeedType> NeedTypes => _needTypes;

        // TODO, consider section for needs that are limited to
        // subtypes of the population.

        #endregion Needs
    }
}
