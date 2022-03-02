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
        public List<(IProduct product, DesireTier tier, decimal amount)> Needs { get; set; }
        IReadOnlyList<(IProduct product, DesireTier tier, decimal amount)> ICulture.Needs => Needs;

        /// <summary>
        /// Wants desired by members of the Culture.
        /// </summary>
        public List<(IWant want, DesireTier tier, decimal amount)> Wants { get; set; }
        IReadOnlyList<(IWant want, DesireTier tier, decimal amount)> ICulture.Wants => Wants;

        /// <summary>
        /// The Culture's Tags.
        /// </summary>
        public List<ITagData<CultureTag>> Tags { get; set; }
        IReadOnlyList<ITagData<CultureTag>> ICulture.Tags => Tags;

        public override string ToString()
        {
            if (string.IsNullOrEmpty(VariantName))
            {
                return Name;
            }
            return string.Format("{0}({1})", Name, VariantName);
        }
    }
}
