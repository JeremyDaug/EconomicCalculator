using System.Text.Json.Serialization;
using EconomicSim.DTOs.Pops.Species.AttachedTagData;

namespace EconomicSim.DTOs.Pops.Species
{
    public class SpeciesDTO : ISpeciesDTO
    {
        public SpeciesDTO()
        {
            Needs = new List<ISpeciesNeedDTO>();
            Wants = new List<ISpeciesWantDTO>();
            Tags = new List<IAttachedSpeciesTag>();
            TagStrings = new List<string>();
            RelatedSpecies = new List<string>();
            RelatedSpeciesIds = new List<int>();
        }

        [JsonIgnore]
        public int Id { get; set; }

        public string Name { get; set; }

        public string VariantName { get; set; }

        public decimal BirthRate { get; set; }

        public int LifeSpan { get; set; }

        public string Description { get; set; }

        #region Needs

        /// <summary>
        /// The Products desired by the species.
        /// </summary>
        public List<ISpeciesNeedDTO> Needs { get; set; }

        /// <summary>
        /// String form of our needs for nicer display.
        /// </summary>
        [JsonIgnore]
        public string NeedsString 
        {
            get
            {
                var result = "";

                foreach (var need in Needs)
                    result += need.ToString() + ";";
                return result;
            }

        }

        #endregion Needs

        #region wants

        /// <summary>
        /// The wants desired by the species.
        /// </summary>
        public List<ISpeciesWantDTO> Wants { get; set; }

        /// <summary>
        /// Wants in nicer string form.
        /// </summary>
        [JsonIgnore]
        public string WantsString 
        {
            get
            {
                var result = "";
                foreach (var want in Wants)
                    result += want.ToString() + ";";
                return result;
            }
        }

        #endregion wants

        public List<string> TagStrings { get; set; }

        /// <summary>
        /// The Tags attached to the species and  their data.
        /// </summary>
        [JsonIgnore]
        public List<IAttachedSpeciesTag> Tags { get; set; }

        /// <summary>
        /// String form of tags
        /// </summary>
        [JsonIgnore]
        public string TagsString 
        {
            get
            {
                var result = "";
                foreach (var tag in TagStrings)
                    result += tag + ";";
                return result;
            }
        }

        [JsonIgnore]
        public List<int> RelatedSpeciesIds { get; set; }

        /// <summary>
        /// Related Species
        /// </summary>
        public List<string> RelatedSpecies { get; set; }

        [JsonIgnore]
        public string RelatedSpeciesString 
        {
            get
            {
                var result = "";

                foreach (var rel in RelatedSpecies)
                    result += rel + ";";

                return result;
            }
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(VariantName))
                return Name;
            return Name + "(" + VariantName + ")";
        }

        public static (string Name, string VariantName) ProcessName(string fullName)
        {
            // if it has a varaint,
            if (fullName.Contains('('))
            {
                var results = fullName.Split('(');

                var variant = results[1].TrimEnd(')');

                return (results[0], variant);
            }
            else
            {
                return (fullName, "");
            }
        }
    }
}
