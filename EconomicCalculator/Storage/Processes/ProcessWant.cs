using EconomicCalculator.Storage.Processes.ProductionTags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage.Processes
{
    /// <summary>
    /// Wants desired or produced by a process
    /// </summary>
    public class ProcessWant : IProcessWant
    {
        /// <summary>
        /// The want's name.
        /// </summary>
        public string WantName
        {
            get
            {
                return Manager.Instance.Wants[WantId].Name;
            }
            set
            {
                WantId = Manager.Instance.GetWantByName(value).Id;
            }
        }

        /// <summary>
        /// The Want it desires.
        /// </summary>
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
    }
}
