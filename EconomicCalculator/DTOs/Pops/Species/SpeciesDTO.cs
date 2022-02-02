﻿using EconomicCalculator.DTOs.Pops.Species.AttachedTagData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EconomicCalculator.DTOs.Pops.Species
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

        public decimal GrowthRate { get; set; }

        public decimal DeathRate { get; set; }

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

        public override string ToString()
        {
            return Name + "(" + VariantName + ")";
        }
    }
}
