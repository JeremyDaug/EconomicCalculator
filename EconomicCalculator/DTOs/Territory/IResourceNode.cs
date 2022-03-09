using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EconomicCalculator.DTOs.Territory
{
    public interface IResourceNode
    {
        [JsonIgnore]
        int ResourceId { get; }

        string Resource { get; }

        decimal Stockpile { get; }

        int Depth { get; }
    }
}
