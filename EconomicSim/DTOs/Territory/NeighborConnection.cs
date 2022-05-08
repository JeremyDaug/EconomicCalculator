using System.Text.Json.Serialization;
using EconomicSim.Enums;

namespace EconomicSim.DTOs.Territory
{
    public class NeighborConnection : INeighborConnection
    {
        public string Territory { get; set; }

        public decimal Distance { get; set; }

        public string ConnectionType 
        {
            get
            {
                return ConnectionEnum.ToString();
            }
            set
            {
                ConnectionEnum = (TerritoryConnectionType)Enum.Parse(typeof(TerritoryConnectionType), value);
            }
        }

        [JsonIgnore]
        public TerritoryConnectionType ConnectionEnum { get; set; }

        public override string ToString()
        {
            return string.Format("{0}: {1} Km by {2}", Territory, Distance, ConnectionType);
        }
    }
}
