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
    /// Species Data Class
    /// </summary>
    internal class Species : ISpecies
    {
        public Species()
        {
            Wants = new List<(IWant, DesireTier, decimal)>();
            Tags = new List<ITagData<SpeciesTag>>();
            RelatedSpecies = new List<ISpecies>();
            Needs = new List<(IProduct, DesireTier, decimal)>();
        }

        /// <summary>
        /// The Id of the species.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The Name of the Species
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The variant name of the species.
        /// </summary>
        public string VariantName { get; set; }

        /// <summary>
        /// The natural growth rate of the Species
        /// Percent.
        /// </summary>
        public decimal GrowthRate { get; set; }

        /// <summary>
        /// The Rate at which the population dies naturally.
        /// Percent
        /// </summary>
        public decimal DeathRate { get; set; }

        /// <summary>
        /// The Products desired by the species.
        /// </summary>
        public List<(IProduct product, DesireTier tier, decimal amount)> Needs { get; set; }
        
        IReadOnlyList<(IProduct product, DesireTier tier, decimal amount)> ISpecies.Needs => Needs;

        /// <summary>
        /// The wants desired by the species.
        /// </summary>
        public List<(IWant want, DesireTier tier, decimal amount)> Wants { get; set; }
        IReadOnlyList<(IWant want, DesireTier tier, decimal amount)> ISpecies.Wants => Wants;

        /// <summary>
        /// The Tags attached to the species and  their data.
        /// </summary>
        public List<ITagData<SpeciesTag>> Tags { get; set; }

        IReadOnlyList<ITagData<SpeciesTag>> ISpecies.Tags => Tags;

        /// <summary>
        /// Related Species
        /// </summary>
        public List<ISpecies> RelatedSpecies { get; set; }
        IReadOnlyList<ISpecies> ISpecies.RelatedSpecies => RelatedSpecies;
    }
}
