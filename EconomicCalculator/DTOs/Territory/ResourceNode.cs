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
    }
}
