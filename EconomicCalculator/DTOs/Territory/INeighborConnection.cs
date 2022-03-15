using EconomicCalculator.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EconomicCalculator.DTOs.Territory
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
