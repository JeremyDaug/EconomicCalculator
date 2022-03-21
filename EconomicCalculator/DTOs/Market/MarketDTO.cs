using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EconomicCalculator.DTOs.Market
{
    public class MarketDTO : IMarketDTO
    {
        public MarketDTO()
        {
            PopIds = new List<int>();
            Territories = new List<string>();
            Neighbors = new Dictionary<string, decimal>();
            Resources = new Dictionary<string, decimal>();
        }

        [JsonIgnore]
        public int Id { get; set; }

        public string Name { get; set; }

        // firm placeholder

        [JsonIgnore]
        public List<int> PopIds { get; set; }

        [JsonIgnore]
        public decimal PopTotal { get; set; }

        // governor placeholders

        public List<string> Territories { get; set; }

        public Dictionary<string, decimal> Neighbors { get; set; }

        public Dictionary<string, decimal> Resources { get; set; }
    }
}