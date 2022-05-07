using System.Text.Json.Serialization;
using EconomicSim.Enums;

namespace EconomicSim.Objects.Territory;

[JsonConverter(typeof(NeighborConnectionJsonConverter))]
internal class NeighborConnection : INeighborConnection
{
    public Territory Neighbor { get; set; }
    ITerritory INeighborConnection.Neighbor => Neighbor;
    public decimal Distance { get; set; }
    public TerritoryConnectionType Type { get; set; }
}