﻿using System.Text.Json.Serialization;
using EconomicSim.DTOs.Processes.ProductionTags;

namespace EconomicSim.DTOs.Processes
{
    /// <summary>
    /// Wants desired or produced by a process
    /// </summary>
    [Obsolete]
    public class ProcessWantDTO : IProcessWantDTO
    {
        public ProcessWantDTO()
        {
            TagStrings = new List<string>();
            Tags = new List<IAttachedProductionTag>();
        }

        /// <summary>
        /// The want's name.
        /// </summary>
        public string WantName
        {
            get
            {
                return DTOManager.Instance.Wants[WantId].Name;
            }
            set
            {
                WantId = DTOManager.Instance.GetWantByName(value).Id;
            }
        }

        /// <summary>
        /// The Want it desires.
        /// </summary>
        [JsonIgnore]
        public int WantId { get; set;  }

        /// <summary>
        /// How much of that want it desires.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// String form of our tags
        /// </summary>
        public List<string> TagStrings { get; set; }

        /// <summary>
        /// What tags this product has for the production process.
        /// </summary>
        [JsonIgnore]
        public List<IAttachedProductionTag> Tags { get; set; }

        /// <summary>
        /// String form of all our tags
        /// </summary>
        [JsonIgnore]
        public string TagString
        {
            get
            {
                var result = "";

                foreach (var tag in TagStrings)
                    result += tag + ";";

                result = result.TrimEnd(';');

                return result;
            }
        }

        public override string ToString()
        {
            var result =
                String.Format("{0} <{1}> -> {2}",
                    WantName, TagString, Amount);

            return result;
        }

        public void AddTag(AttachedProductionTag tag)
        {
            Tags.Add(tag);

            TagStrings.Add(tag.ToString());
        }

        public void SetTagsFromStrings()
        {
            foreach (var tag in TagStrings)
            {
                Tags.Add(ProductionTagInfo.ProcessTagString(tag));
            }
        }
    }
}