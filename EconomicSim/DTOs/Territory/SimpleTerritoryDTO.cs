using System.Text.Json.Serialization;
using EconomicSim.DTOs.Hexmap;

namespace EconomicSim.DTOs.Territory
{
    public class SimpleTerritoryDTO : ISimpleTerritoryDTO
    {
        public SimpleTerritoryDTO()
        {
            // ew, that initialization.
            Plots = new List<ulong>(new ulong[6]);
            Neighbors = new List<NeighborConnection>();
            Nodes = new List<ResourceNode>();
            Resources = new List<TerritoryResource>();
        }

        /// <summary>
        /// The coordinates of our simple territory.
        /// For Simple territories, this should be unique.
        /// </summary>
        public HexCoord Coords { get; set; }

        /// <summary>
        /// Name of the Territory, may be null.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Whether the Territory is costal or not.
        /// </summary>
        public bool IsCoastal { get; set; }

        /// <summary>
        /// Whether the Territory has a static body of water.
        /// </summary>
        public bool HasLake { get; set; }

        /// <summary>
        /// The total size in acres, including water.
        /// </summary>
        public ulong Size { get; set; }

        /// <summary>
        /// The land available in acres.
        /// </summary>
        public ulong Land { get; set; }

        /// <summary>
        /// The surface area claimed by water in the territory.
        /// </summary>
        public ulong Water { get; set; }

        /// <summary>
        /// The number of plots of each kind. 
        /// Should match up with available land options.
        /// Must equal Land * 8.
        /// </summary>
        public List<ulong> Plots { get; set; }

        [JsonIgnore]
        public string PlotsString
        {
            get
            {
                var result = "";
                foreach (var plot in Plots)
                {
                    result += plot.ToString() + "\n";
                }
                return result;
            }
        }

        /// <summary>
        /// Neighboring connections.
        /// </summary>
        public List<NeighborConnection> Neighbors { get; set;}
        
        List<INeighborConnection> ISimpleTerritoryDTO.Neighbors => new List<INeighborConnection>(Neighbors);

        [JsonIgnore]
        public string NeighborsString
        {
            get
            {
                var result = "";
                foreach (var neighbor in Neighbors)
                {
                    result += neighbor.ToString() + "\n";
                }
                return result;
            }
        }

        /// <summary>
        /// The Nodes within the territory and the depth at which it can be mined.
        /// -1 means inifinite size.
        /// </summary>
        public List<ResourceNode> Nodes { get; set; }
        List<IResourceNode> ISimpleTerritoryDTO.Nodes => new List<IResourceNode>(Nodes);

        [JsonIgnore]
        public string NodesString
        {
            get
            {
                var result = "";
                foreach (var node in Nodes)
                {
                    result += node.ToString() + "\n";
                }
                return result;
            }
        }

        /// <summary>
        /// The Resources available on the surface of the territory
        /// Amount of -1 means infinite.
        /// </summary>
        public List<TerritoryResource> Resources { get; set; }
        List<ITerritoryResource> ISimpleTerritoryDTO.Resources => new List<ITerritoryResource>(Resources);

        [JsonIgnore]
        public string ResourcesString
        {
            get
            {
                var result = "";
                foreach (var resource in Resources)
                {
                    result += resource.ToString() + "\n";
                }
                return result;
            }
        }
    }
}
