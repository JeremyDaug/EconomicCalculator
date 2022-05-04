using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using EconomicSim.Helpers;
using EconomicSim.Objects.Products;
using EconomicSim.Objects.Wants;

namespace EconomicSim.Objects.Pops.Species
{
    /// <summary>
    /// Species Data Class
    /// </summary>
    [JsonConverter(typeof(SpeciesJsonConverter))]
    internal class Species : ISpecies
    {
        public Species()
        {
            Wants = new List<WantDesire>();
            Tags = new List<TagData<SpeciesTag>>();
            Relations = new List<Species>();
            Needs = new List<NeedDesire>();
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
        /// per year.
        /// </summary>
        public decimal GrowthRate { get; set; }

        /// <summary>
        /// The Rate at which the population dies naturally
        /// per year.
        /// </summary>
        public decimal DeathRate { get; set; }

        /// <summary>
        /// The Products desired by the species.
        /// </summary>
        public List<NeedDesire> Needs { get; set; }
        
        IReadOnlyList<INeedDesire> ISpecies.Needs => Needs;

        /// <summary>
        /// The wants desired by the species.
        /// </summary>
        public List<WantDesire> Wants { get; set; }
        IReadOnlyList<IWantDesire> ISpecies.Wants => Wants;

        /// <summary>
        /// The Tags attached to the species and  their data.
        /// </summary>
        public List<TagData<SpeciesTag>> Tags { get; set; }

        IReadOnlyList<ITagData<SpeciesTag>> ISpecies.Tags => Tags;

        /// <summary>
        /// Related Species
        /// </summary>
        public List<Species> Relations { get; set; }
        IReadOnlyList<ISpecies> ISpecies.Relations => Relations;

        public override string ToString()
        {
            if (string.IsNullOrEmpty(VariantName))
            {
                return Name;
            }
            return string.Format("{0}({1})", Name, VariantName);
        }

        public string GetName()
        {
            if (!string.IsNullOrWhiteSpace(VariantName))
                return $"{Name}({VariantName})";
            return Name;
        }
    }
}
