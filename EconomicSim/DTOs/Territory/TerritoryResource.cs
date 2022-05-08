using System.Text.Json.Serialization;

namespace EconomicSim.DTOs.Territory
{
    public class TerritoryResource : ITerritoryResource
    {
        [JsonIgnore]
        public int ResourceId { get; set; }

        public string Resource { get; set; }

        public decimal Amount { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}", Resource, Amount);
        }
    }
}
