using EconomicCalculator.Helpers;
using EconomicCalculator.Objects.Products;
using EconomicCalculator.Objects.Wants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Objects.Pops.Culture
{
    /// <summary>
    /// Culture Data Class
    /// </summary>
    internal class Culture : ICulture
    {
        private readonly IReadOnlyList<ITagData<CultureTags>> tags;
        private readonly IReadOnlyList<(IWant want, DesireTier tier, decimal amount)> wants;
        private readonly IReadOnlyList<(IProduct product, DesireTier tier, decimal amount)> needs;

        /// <summary>
        /// The Culture's Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The Name of the Culture.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Variant Name of the Culture, if it's based on another.
        /// </summary>
        public string VariantName { get; set; }

        /// <summary>
        /// The growth bonus or penalty
        /// </summary>
        public decimal GrowthModifier { get; set; }

        /// <summary>
        /// The death bonus or penalty.
        /// </summary>
        public decimal DeathModifier { get; set; }

        /// <summary>
        /// The products desired by members of the culture.
        /// </summary>
        public IReadOnlyList<(IProduct product, DesireTier tier, decimal amount)> Needs => needs;

        /// <summary>
        /// Wants desired by members of the Culture.
        /// </summary>
        public IReadOnlyList<(IWant want, DesireTier tier, decimal amount)> Wants => wants;
        
        /// <summary>
        /// The Culture's Tags.
        /// </summary>
        public IReadOnlyList<ITagData<CultureTags>> Tags => tags;
    }
}
