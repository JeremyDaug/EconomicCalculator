using EconomicCalculator.DTOs.Pops.Culture.AttachedTagData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EconomicCalculator.DTOs.Pops.Culture
{
    public class CultureDTO : ICultureDTO
    {
        public CultureDTO()
        {
            Needs = new List<ICultureNeedDTO>();
            Wants = new List<ICultureWantDTO>();
            Tags = new List<IAttachedCultureTag>();
            TagsStrings = new List<string>();
            RelatedCultures = new List<string>();
            RelatedCulturesIds = new List<int>();
        }

        [JsonIgnore]
        public int Id { get; set; }

        public string Name { get; set; }

        public string VariantName { get; set; }

        public decimal BirthModifier { get; set; }

        public decimal DeathModifier { get; set; }

        public string Description { get; set; }

        public List<ICultureNeedDTO> Needs { get; set; }

        [JsonIgnore]
        public string NeedsString
        {
            get
            {
                var result = "";
                foreach (var item in Needs)
                    result += item.ToString() + "; ";
                return result;
            }
        }

        public List<ICultureWantDTO> Wants { get; set; }

        [JsonIgnore]
        public string WantsString 
        { 
            get
            {
                var result = "";
                foreach (var item in Wants)
                    result += item.ToString() + "; ";
                return result;
            }
        }

        [JsonIgnore]
        public List<IAttachedCultureTag> Tags { get; set; }

        public List<string> TagsStrings { get; set; }

        [JsonIgnore]
        public string TagsString
        {
            get
            {
                var result = "";
                foreach (var item in TagsStrings)
                    result += result + "; ";
                return result;
            }
        }

        public List<string> RelatedCultures { get; set; }

        [JsonIgnore]
        public List<int> RelatedCulturesIds { get; set; }

        [JsonIgnore]
        public string RelatedCulturesString
        {
            get
            {
                var result = "";
                foreach (var item in RelatedCultures)
                    result += item + "; ";
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

        // Classes
    }
}
