using System.Text.Json.Serialization;
using EconomicSim.DTOs.Hexmap;
using EconomicSim.Objects.Market;
using EconomicSim.Objects.Products;

namespace EconomicSim.Objects.Territory
{
    /// <summary>
    /// Super basic Territory Data Class. For environmental data available in a market.
    /// No special traits or connections. Everything is explicit. No map generation basis.
    /// </summary>
    [JsonConverter(typeof(TerritoryJsonConverter))]
    internal class Territory : ITerritory
    {
        private ulong land;

        public Territory()
        {
            Plots = new Dictionary<Product, long>();
            Neighbors = new List<NeighborConnection>();
            Nodes = new List<Node>();
            Resources = new Dictionary<Product, decimal>();
        }

        /// <summary>
        /// The Location of the territory in the hex grid which contains them.
        /// </summary>
        public HexCoord Coords { get; }

        /// <summary>
        /// The name of the territory (may be empty)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Whether the territory is coastal or not.
        /// </summary>
        public bool Coastal { get; set; }

        /// <summary>
        /// Whether the Territory contains water.
        /// </summary>
        public bool Lake { get; set; }

        /// <summary>
        /// The size of the Territory in Acres.
        /// </summary>
        public ulong Size { get; set; }

        /// <summary>
        /// The actual land available in the territory in acres.
        /// </summary>
        public ulong Land
        {
            get
            {
                return land;
            }
            set
            {
                land = value;
            }
        }

        /// <summary>
        /// The area that is taken up by water in acres.
        /// </summary>
        public ulong Water
        {
            get
            {
                return Size - land;
            }
            set
            {
                land = Size - value;
            }
        }

        /// <summary>
        /// The market the territory is in.
        /// </summary>
        public Market.Market Market { get; set; }
        IMarket ITerritory.Market => Market;

        /// <summary>
        /// The plots in the territory.
        /// The Key is the kind of land
        /// Value is the number of plots of that land.
        /// </summary>
        public Dictionary<Product, long> Plots { get; set; }
        IReadOnlyDictionary<IProduct, long> ITerritory.Plots => Plots.ToDictionary(pair => (IProduct)pair.Key, pair => pair.Value);

        public List<NeighborConnection> Neighbors { get; set; }

        IReadOnlyList<INeighborConnection> ITerritory.Neighbors => Neighbors;

        public List<Node> Nodes { get; set; }
        IReadOnlyList<INode> ITerritory.Nodes => Nodes; 

        public Dictionary<Product, decimal> Resources { get; set; }

        IReadOnlyDictionary<IProduct, decimal> ITerritory.Resources
            => Resources.ToDictionary(x => (IProduct) x.Key,
                x => x.Value);
    }
}
