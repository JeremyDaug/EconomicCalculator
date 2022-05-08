using System.Text.Json.Serialization;

namespace EconomicSim.DTOs.Market
{
    public class MarketDTO : IMarketDTO
    {
        public MarketDTO()
        {
            FirmIds = new List<int>();
            Firms = new List<string>();
            PopIds = new List<int>();
            Territories = new List<string>();
            Neighbors = new Dictionary<string, decimal>();
            Resources = new Dictionary<string, decimal>();
        }

        [JsonIgnore]
        public int Id { get; set; }

        public string Name { get; set; }

        [JsonIgnore]
        public IList<int> FirmIds { get; set; }

        public IList<string> Firms { get; set; }

        [JsonIgnore]
        public string FirmString 
        {
            get
            {
                var result = "";
                foreach (var firm in Firms)
                    result += firm + "\n";
                return result;
            }
        }

        [JsonIgnore]
        public IList<int> PopIds { get; set; }

        [JsonIgnore]
        public decimal PopTotal { get; set; }

        // governor placeholders

        public IList<string> Territories { get; set; }

        [JsonIgnore]
        public string TerritoriesString 
        {
            get
            {
                var result = "";
                foreach (var terr in Territories)
                    result += terr + "\n";
                return result;
            }
        }

        public IDictionary<string, decimal> Neighbors { get; set; }

        [JsonIgnore]
        public string NeighborString 
        {
            get
            {
                var result = "";
                foreach (var neig in Neighbors)
                    result += string.Format("{0} : {1} km(s)\n", neig.Key, neig.Value.ToString());
                return result;
            }
        }

        public IDictionary<string, decimal> Resources { get; set; }

        [JsonIgnore]
        public string ResourcesString 
        {
            get
            {
                var result = "";
                foreach (var resource in Resources)
                    result += string.Format("{0} : {1} unit(s)\n", resource.Key, resource.Value.ToString());
                return result;
            }
        }
    }
}