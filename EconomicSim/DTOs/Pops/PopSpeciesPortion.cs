using System.Text.Json.Serialization;

namespace EconomicSim.DTOs.Pops
{
    public class PopSpeciesPortion : IPopSpeciesPortion
    {
        [JsonIgnore]
        public int SpeciesId { get; set; }

        public string Species { get; set; }

        public ulong Amount { get; set; }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Species, Amount);
        }
    }
}
