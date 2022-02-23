using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EconomicCalculator.DTOs.Pops
{
    public class PopSpeciesPortion : IPopSpeciesPortion
    {
        [JsonIgnore]
        public int SpeciesId { get; set; }

        public string Species { get; set; }

        public ulong Amount { get; set; }
    }
}
