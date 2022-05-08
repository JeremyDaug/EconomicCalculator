﻿using System.Text.Json.Serialization;
using EconomicSim.DTOs.Hexmap;
using EconomicSim.Objects.Products;

namespace EconomicSim.Objects.Territory
{
    /// <summary>
    /// Territory Read Only Interface
    /// </summary>
    [JsonConverter(typeof(TerritoryJsonConverter))]
    public interface ITerritory
    {
        /// <summary>
        /// The Location of the territory in the hex grid which contains them.
        /// </summary>
        HexCoord Coords { get; }

        /// <summary>
        /// The name of the territory, may be ignored or autogenerated.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// If the Territory is adjacent to a sea or body of water.
        /// </summary>
        bool Coastal { get; }

        /// <summary>
        /// Whether the territory has a static lake or sea contained within it.
        /// </summary>
        bool Lake { get; }

        /// <summary>
        /// The total size of the Territory in acres, including sea.
        /// </summary>
        ulong Size { get; }

        /// <summary>
        /// The land available in acres.
        /// </summary>
        ulong Land { get; }

        /// <summary>
        /// The Water in the territory measured in acres.
        /// </summary>
        ulong Water { get; }

        /// <summary>
        /// The number of each plots available. 
        /// (each is equal to 1/8 an acre) index.
        /// The index in <see cref="RequiredItems.LandOptions"/> is the same as here.
        /// </summary>
        IReadOnlyDictionary<IProduct, long> Plots { get; }

        /// <summary>
        /// Neighboring Territories to this one.
        /// Includes a reference to the neighbor, the distance to it.
        /// As well as how the neighbors are connected.
        /// </summary>
        IReadOnlyList<INeighborConnection> Neighbors { get; }

        /// <summary>
        /// The nodes within the territory which can be mined.
        /// -1 stockpile means the stockpile is infinite.
        /// </summary>
        IReadOnlyList<INode> Nodes { get; }

        /// <summary>
        /// The resources available at the surface of the territory.
        /// Amount of -1 means infinite.
        /// </summary>
        IReadOnlyDictionary<IProduct, decimal> Resources { get; }
    }
}
