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
    /// Read Only Culture Interface
    /// </summary>
    public interface ICulture
    {
        /// <summary>
        /// The Id of the Culture
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The name of the Culture
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The variant name for a subculture.
        /// </summary>
        string VariantName { get; }

        /// <summary>
        /// Additional Growth rate from the Culture.
        /// </summary>
        decimal GrowthModifier { get; }

        /// <summary>
        /// Additional Death rate from Culture.
        /// </summary>
        decimal DeathModifier { get; }

        /// <summary>
        /// The products desired by this culture.
        /// </summary>
        IReadOnlyList<(IProduct, decimal, DesireTier)> Needs { get; }

        /// <summary>
        /// The wants desired by this culture.
        /// </summary>
        IReadOnlyList<(IWant, decimal, DesireTier)> Wants { get; }

        /// <summary>
        /// The Culture's Tags.
        /// </summary>
        IReadOnlyList<ITagData<CultureTags>> Tags { get; }
    }
}
