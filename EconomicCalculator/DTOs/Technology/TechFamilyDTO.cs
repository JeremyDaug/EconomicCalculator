using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EconomicCalculator.DTOs.Technology
{
    /// <summary>
    /// Tech family Data Transfer object and Editor Interface Class.
    /// </summary>
    public class TechFamilyDTO : ITechFamilyDTO
    {
        public TechFamilyDTO()
        {
            RelatedFamilies = new List<int>();
            RelatedFamilyStrings = new List<string>();
            Techs = new List<int>();
            TechStrings = new List<string>();
        }

        /// <summary>
        /// Id of the Tech Family
        /// </summary>
        [JsonIgnore]
        public int Id { get; set; }

        /// <summary>
        /// Name of the Tech Family.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Id's of the families related to this one.
        /// </summary>
        [JsonIgnore] 
        public List<int> RelatedFamilies { get; set; }

        /// <summary>
        /// The Names of the related families.
        /// </summary>
        public List<string> RelatedFamilyStrings { get; set; }

        /// <summary>
        /// The Related families in nice single string format.
        /// </summary>
        [JsonIgnore]
        public string RelatedFamilyString
        {
            get 
            {
                var result = "";

                foreach (var family in RelatedFamilyStrings)
                {
                    result += family + "; ";
                }

                result = result.Trim(';');
                return result;
            }
        }

        /// <summary>
        /// The techs in this family.
        /// </summary>
        [JsonIgnore] 
        public List<int> Techs { get; set; }

        /// <summary>
        /// The names of techs in this family.
        /// </summary>
        public List<string> TechStrings { get; set; }

        /// <summary>
        /// The names of techs in this family, with nicer string format.
        /// </summary>
        [JsonIgnore]
        public string TechString
        {
            get
            {
                var result = "";

                foreach (var tech in TechStrings)
                {
                    result += tech + "; ";
                }

                result = result.Trim(';');
                return result;
            }
        }

        /// <summary>
        /// A description of the Tech Family.
        /// </summary>
        public string Description { get; set; }

        public void SetRelatedFamiliesFromStrings()
        {
            foreach (var rel in RelatedFamilyStrings)
            {
                RelatedFamilies.Add(DTOManager.Instance.TechFamilies
                    .Values.Single(x => x.Name == rel).Id);
            }
        }
    }
}
