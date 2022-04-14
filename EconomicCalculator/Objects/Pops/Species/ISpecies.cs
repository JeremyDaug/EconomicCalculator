using EconomicCalculator.Helpers;
using EconomicCalculator.Objects.Products;
using EconomicCalculator.Objects.Wants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Objects.Pops.Species
{
    /// <summary>
    /// Read only interface for species.
    /// </summary>
    public interface ISpecies
    {
        /// <summary>
        /// Species Id
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The name of the Species
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The variant name of the species.
        /// </summary>
        string VariantName { get; set; }

        /// <summary>
        /// The natural growth rate of the Species.
        /// </summary>
        decimal GrowthRate { get; }

        /// <summary>
        /// The natural rate of death of the species.
        /// </summary>
        decimal DeathRate { get; }

        /// <summary>
        /// The Product desires a species has by default.
        /// </summary>
        IReadOnlyList<(IProduct product, DesireTier tier, decimal amount)> Needs { get; }

        /// <summary>
        /// The Wants desired by the species.
        /// </summary>
        IReadOnlyList<(IWant want, DesireTier tier, decimal amount)> Wants { get; }

        /// <summary>
        /// The tags which modify how a species acts more broadly.
        /// </summary>
        IReadOnlyList<ITagData<SpeciesTag>> Tags { get; }

        /// <summary>
        /// Related Species
        /// </summary>
        IReadOnlyList<ISpecies> RelatedSpecies { get; }
    }
}
