using System.Text.Json.Serialization;
using EconomicSim.Enums;

namespace EconomicSim.DTOs.Territory
{
    public interface INeighborConnection
    {
        string Territory { get; }

        decimal Distance { get; }

        string ConnectionType { get; }

        [JsonIgnore]
        TerritoryConnectionType ConnectionEnum { get; }
    }
}
