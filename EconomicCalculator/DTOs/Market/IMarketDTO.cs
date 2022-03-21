using EconomicCalculator.DTOs.Pops;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EconomicCalculator.DTOs.Market
{
    public interface IMarketDTO
    {
        /// <summary>
        /// Id of the market
        /// </summary>
        [JsonIgnore]
        int Id { get; }

        /// <summary>
        /// The unique name of the market.
        /// </summary>
        string Name { get; }

        // firm placeholder

        // Because pops do not have meaningful names
        // they cannot be referenced by markets.
        // Pops reference Markets.
        [JsonIgnore]
        List<int> PopIds { get; }

        [JsonIgnore]
        decimal PopTotal { get; set; }

        // governor placeholder.

        List<string> Territories { get; }

        /// <summary>
        /// The markets which can be reached directly from
        /// this market.
        /// </summary>
        Dictionary<string, decimal> Neighbors { get; }

        /// <summary>
        /// The resources which anyone in the market can access
        /// without work (considered already extracted).
        /// </summary>
        Dictionary<string, decimal> Resources { get; }
    }
}
