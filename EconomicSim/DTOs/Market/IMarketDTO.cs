using EconomicSim.DTOs.Pops;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EconomicSim.DTOs.Market
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

        [JsonIgnore]
        IList<int> FirmIds { get; set; }

        IList<string> Firms { get; set; }

        [JsonIgnore]
        string FirmString { get; }

        // Because pops do not have meaningful names
        // they cannot be referenced by markets.
        // Pops reference Markets.
        [JsonIgnore]
        IList<int> PopIds { get; }

        [JsonIgnore]
        decimal PopTotal { get; set; }

        // governor placeholder.

        IList<string> Territories { get; }

        [JsonIgnore]
        string TerritoriesString { get; }

        /// <summary>
        /// The markets which can be reached directly from
        /// this market.
        /// </summary>
        IDictionary<string, decimal> Neighbors { get; }

        [JsonIgnore]
        string NeighborString { get; }

        /// <summary>
        /// The resources which anyone in the market can access
        /// without work (considered already extracted).
        /// </summary>
        IDictionary<string, decimal> Resources { get; }

        [JsonIgnore]
        string ResourcesString { get; }
    }
}
