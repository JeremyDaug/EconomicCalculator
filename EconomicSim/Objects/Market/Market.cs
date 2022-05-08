using System.Text.Json.Serialization;
using EconomicSim.Objects.Firms;
using EconomicSim.Objects.Pops;
using EconomicSim.Objects.Products;
using EconomicSim.Objects.Territory;

namespace EconomicSim.Objects.Market
{
    /// <summary>
    /// Market Data Class
    /// </summary>
    [JsonConverter(typeof(MarketJsonConverter))]
    internal class Market : IMarket
    {
        public Market()
        {
            Territories = new List<Territory.Territory>();
            Neighbors = new Dictionary<Market, decimal>();
            Resources = new Dictionary<Product, decimal>();
            Pops = new List<PopGroup>();
            Firms = new List<Firm>();
        }

        /// <summary>
        /// ID of the market.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the market, usually the name the region or city
        /// it covers or which acts as it's core.
        /// </summary>
        /// <remarks>
        /// Name must be unique among all other markets.
        /// </remarks>
        public string Name { get; set; }

        /// <summary>
        /// The firms that are operating in this market.
        /// </summary>
        public List<Firm> Firms { get; set; }
        IReadOnlyList<IFirm> IMarket.Firms => Firms;

        /// <summary>
        /// The Population of the market.
        /// </summary>
        public List<PopGroup> Pops { get; set; }
        IReadOnlyList<IPopGroup> IMarket.Pops => Pops;

        /// <summary>
        /// The governing body of the market.
        /// </summary>
        //public IGovernor Governor { get; set; }

        /// <summary>
        /// The Territories the Market Contains.
        /// </summary>
        public List<Territory.Territory> Territories { get; set; }
        IReadOnlyList<ITerritory> IMarket.Territories => Territories;

        /// <summary>
        /// The Markets which are considered adjacent to this one
        /// and their distance measured in km. 
        /// (Default distance is measured from center to center of
        /// the respective territories).
        /// These Neighbors should be reachable by foot assuming no
        /// rivers.
        /// </summary>
        public Dictionary<Market, decimal> Neighbors { get; set; }
        IReadOnlyDictionary<IMarket, decimal> IMarket.Neighbors
            => Neighbors.ToDictionary(x => (IMarket) x.Key,
                x => x.Value);

        /// <summary>
        /// The resources that are loose in the market, unowned.
        /// These resources may be picked up and used at will by anyone
        /// in the market.
        /// </summary>
        public Dictionary<Product, decimal> Resources { get; set; }
        IReadOnlyDictionary<IProduct, decimal> IMarket.Resources
            => Resources.ToDictionary(
                x => (IProduct) x.Key,
                x => x.Value);

        public override bool Equals(object? obj)
        {
            return Equals(obj as Market);
        }

        public bool Equals(Market obj)
        {
            return string.Equals(Name, obj.Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
