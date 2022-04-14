using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EconomicCalculator.DTOs.Territory
{
    public class ResourceNode : IResourceNode
    {
        [JsonIgnore]
        public int ResourceId { get; set; }

        public string Resource { get; set; }

        public decimal Stockpile { get; set; }

        public int Depth { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} at depth {2}", Resource, Stockpile, Depth);
        }
    }
}
