using EconModels.DTOs.Pops.PopNeeds;
using EconomicSim.DTOs.Pops.PopWants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using EconomicSim.DTOs.Pops.Culture.AttachedTagData;

namespace EconomicSim.DTOs.Pops.Culture
{
    public interface ICultureDTO
    {
        /// <summary>
        /// The Id of the Culture
        /// </summary>
        [JsonIgnore]
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
        decimal BirthModifier { get; }

        /// <summary>
        /// Additional Death rate from Culture.
        /// </summary>
        decimal DeathModifier { get; }

        /// <summary>
        /// Culture Description
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// The products desired by this culture.
        /// </summary>
        List<ICultureNeedDTO> Needs { get; }

        [JsonIgnore]
        string NeedsString { get; }

        /// <summary>
        /// The wants desired by this culture.
        /// </summary>
        List<ICultureWantDTO> Wants { get; }

        [JsonIgnore]
        string WantsString { get; }

        /// <summary>
        /// The Culture's Tags.
        /// </summary>
        [JsonIgnore]
        List<IAttachedCultureTag> Tags { get; }

        List<string> TagsStrings { get; }

        [JsonIgnore]
        string TagsString { get; }

        List<string> RelatedCultures { get; }

        [JsonIgnore]
        List<int> RelatedCulturesIds { get; }

        [JsonIgnore]
        string RelatedCulturesString { get; }

        // Classes
    }
}
