using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EconomicCalculator.DTOs.Technology
{
    public interface ITechFamilyDTO
    {
        /// <summary>
        /// Id of the Tech Family.
        /// </summary>
        [JsonIgnore] 
        int Id { get; }

        /// <summary>
        /// The name of the Tech Family.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The Related Families to this tech family.
        /// </summary>
        [JsonIgnore]
        List<int> RelatedFamilies { get; }

        /// <summary>
        /// The Families related to this family.
        /// </summary>
        List<string> RelatedFamilyStrings { get; }

        /// <summary>
        /// All families related to this family in nice string form.
        /// </summary>
        [JsonIgnore]
        string RelatedFamilyString { get; }

        /// <summary>
        /// The techs in this family.
        /// </summary>
        [JsonIgnore]
        List<int> Techs { get; }

        /// <summary>
        /// The Techs in this family, but strings this time.
        /// </summary>
        List<string> TechStrings { get; }

        /// <summary>
        /// The Techs in this family in nicer string form.
        /// </summary>
        [JsonIgnore]
        string TechString { get; }

        /// <summary>
        /// A description of the Family.
        /// </summary>
        string Description { get; }
    }
}
