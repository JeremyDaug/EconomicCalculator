using EconomicCalculator.Objects.Pops;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EconomicCalculator.DTOs.Pops.Species
{
    public class SpeciesNeedDTO : ISpeciesNeedDTO
    {
        public string Product { get; set; }

        [JsonIgnore]
        public int ProductId { get; set; }

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

        public override string ToString()
        {
            var result = "{0}[{1}]->{2}";

            return string.Format(result, Product, Tier, Amount);
        }
    }
}
