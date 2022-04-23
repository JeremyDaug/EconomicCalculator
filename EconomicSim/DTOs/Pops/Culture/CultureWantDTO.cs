using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using EconomicSim.Objects.Pops;

namespace EconomicSim.DTOs.Pops.Culture
{
    public class CultureWantDTO : ICultureWantDTO
    {
        public string Want { get; set; }

        [JsonIgnore]
        public int WantId { get; set; }

        [JsonIgnore]
        public DesireTier TierEnum { get; set; }

        public string Tier
        {
            get
            {
                return TierEnum.ToString();
            }
            set
            {
                TierEnum = (DesireTier)Enum.Parse(typeof(DesireTier), value);
            }
        }

        public decimal Amount { get; set; }

        // Need Tag modifiers

        public override string ToString()
        {
            var result = "{0}[{1}]->{2}";

            return string.Format(result, Want, Tier, Amount);
        }
    }
}
