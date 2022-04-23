using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using EconomicSim.DTOs.Processes.ProductionTags;

namespace EconomicSim.DTOs.Processes
{
    /// <summary>
    /// Product desired or produced by a process.
    /// </summary>
    public class ProcessProductDTO : IProcessProductDTO
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public ProcessProductDTO()
        {
            TagStrings = new List<string>();
            Tags = new List<IAttachedProductionTag>();
        }

        /// <summary>
        /// The name of the product desired.
        /// </summary>
        public string ProductName
        {
            get
            {
                return DTOManager.Instance.Products[ProductId].GetName();
            }
            set
            {
                ProductId = DTOManager.Instance.GetProductByFullName(value).Id;
            }
        }

        /// <summary>
        /// The product it desires.
        /// </summary>
        [JsonIgnore]
        public int ProductId { get; set; }

        /// <summary>
        /// How much it desires.
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
                String.Format("{0} <{1}> -> {2} {3}",
                    ProductName, TagString, Amount,
                    DTOManager.Instance.Products[ProductId].UnitName);

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
