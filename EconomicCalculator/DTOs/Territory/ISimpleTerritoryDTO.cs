using EconomicCalculator.DTOs.Hexmap;
using EconomicCalculator.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EconomicCalculator.DTOs.Territory
{
    /// <summary>
    /// Simple Territory DTO Interface for simplistic uses.
    /// Covers a basic, no frill territories connected via a hexgrid.
    /// </summary>
    public interface ISimpleTerritoryDTO
    {
        /// <summary>
        /// The coordinates of our simple territory.
        /// For Simple territories, this should be unique.
        /// </summary>
        HexCoord Coords { get; }

        /// <summary>
        /// Name of the Territory, may be null.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Whether the Territory is costal or not.
        /// </summary>
        bool IsCoastal { get; }

        /// <summary>
        /// Whether the Territory has a static body of water.
        /// </summary>
        bool HasLake { get; }

        /// <summary>
        /// The total size in acres, including water.
        /// </summary>
        ulong Size { get; }

        /// <summary>
        /// The land available in acres.
        /// </summary>
        ulong Land { get; }

        /// <summary>
        /// The surface area claimed by water in the territory.
        /// </summary>
        ulong Water { get; }

        /// <summary>
        /// The number of plots of each kind. 
        /// Should match up with available land options.
        /// Must equal Land * 8.
        /// </summary>
        List<ulong> Plots { get; }

        [JsonIgnore]
        string PlotsString { get; }

        /// <summary>
        /// The Id of the territory
        /// </summary>
        List<INeighborConnection> Neighbors { get; }

        [JsonIgnore]
        string NeighborsString { get; }

        /// <summary>
        /// The Nodes within the territory and the depth at which it can be mined.
        /// -1 means inifinite size.
        /// </summary>
        List<IResourceNode> Nodes { get; }
        
        [JsonIgnore]
        string NodesString { get; }

        /// <summary>
        /// The Resources available on the surface of the territory
        /// Amount of -1 means infinite.
        /// </summary>
        List<ITerritoryResource> Resources { get; }

        [JsonIgnore]
        string ResourcesString { get; }
    }
}
