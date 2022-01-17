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
        public List<(IWant, DesireTier, decimal)> wants;
        public List<ITagData<SpeciesTag>> tags;
        public List<ISpecies> relatedSpecies;
        public List<(IProduct, DesireTier, decimal)> needs;

        public Species()
        {
            wants = new List<(IWant, DesireTier, decimal)>();
            tags = new List<ITagData<SpeciesTag>>();
            relatedSpecies = new List<ISpecies>();
            needs = new List<(IProduct, DesireTier, decimal)>();
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
        /// The natural growth rate of the Species
        /// </summary>
        public decimal GrowthRate { get; set; }

        /// <summary>
        /// The Rate at which the population dies naturally.
        /// </summary>
        public decimal DeathRate { get; set; }

        /// <summary>
        /// The Products desired by the species.
        /// </summary>
        public IReadOnlyList<(IProduct product, DesireTier tier, decimal amount)> Needs => needs;

        /// <summary>
        /// The wants desired by the species.
        /// </summary>
        public IReadOnlyList<(IWant want, DesireTier tier, decimal amount)> Wants => wants;

        /// <summary>
        /// The Tags attached to the species and  their data.
        /// </summary>
        public IReadOnlyList<ITagData<SpeciesTag>> Tags => tags;

        /// <summary>
        /// Related Species
        /// </summary>
        public IReadOnlyList<ISpecies> RelatedSpecies => relatedSpecies;
    }
}
