using System.Text.Json.Serialization;
using EconomicSim.Enums;

namespace EconomicSim.Objects.Territory;

[JsonConverter(typeof(NeighborConnectionJsonConverter))]
public interface INeighborConnection
{
    ITerritory Neighbor { get; }
    
    decimal Distance { get; }
    
    TerritoryConnectionType Type { get; }
}