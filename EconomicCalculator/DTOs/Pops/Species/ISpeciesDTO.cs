using EconomicCalculator.DTOs.Pops.Species.AttachedTagData;
using EconomicCalculator.Objects.Pops;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EconomicCalculator.DTOs.Pops.Species
{
    /// <summary>
    /// The information for a species in the system.
    /// </summary>
    public interface ISpeciesDTO
    {
        /// <summary>
        /// Unique ID of the species.
        /// </summary>
        [JsonIgnore]
        int Id { get; }

        /// <summary>
        /// The name of the species, must be unique with it's variant
        /// name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The variant name of the species, may be empty
        /// must be unique when comiben with Name.
        /// </summary>
        string VariantName { get; }

        /// <summary>
        /// The natural growth rate of the Species
        /// </summary>
        decimal BirthRate { get; }

        /// <summary>
        /// How long a member of thes species naturally survives for.
        /// </summary>
        int LifeSpan { get; }

        /// <summary>
        /// A description of the species.
        /// </summary>
        string Description { get; }

        #region Needs

        /// <summary>
        /// The Products desired by the species.
        /// </summary>
        List<ISpeciesNeedDTO> Needs { get; }

        /// <summary>
        /// String form of our needs for nicer display.
        /// </summary>
        [JsonIgnore]
        string NeedsString { get; }

        #endregion Needs

        #region wants

        /// <summary>
        /// The wants desired by the species.
        /// </summary>
        List<ISpeciesWantDTO> Wants { get; }

        /// <summary>
        /// Wants in nicer string form.
        /// </summary>
        [JsonIgnore]
        string WantsString { get; }

        #endregion wants

        List<string> TagStrings { get; }

        /// <summary>
        /// The Tags attached to the species and  their data.
        /// </summary>
        [JsonIgnore] 
        List<IAttachedSpeciesTag> Tags { get; set; }

        /// <summary>
        /// String form of tags
        /// </summary>
        [JsonIgnore] 
        string TagsString { get; }

        [JsonIgnore]
        List<int> RelatedSpeciesIds { get; }

        /// <summary>
        /// Related Species
        /// </summary>
        List<string> RelatedSpecies { get; }

        /// <summary>
        /// Related Species in string form.
        /// </summary>
        [JsonIgnore]
        string RelatedSpeciesString { get; }
    }
}
