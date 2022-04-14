using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EconomicCalculator.DTOs.Pops
{
    public class PopCulturePortion : IPopCulturePortion
    {
        [JsonIgnore]
        public int CultureId { get; set; }

        public string Culture { get; set; }

        public ulong Amount { get; set; }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Culture, Amount);
        }
    }
}
