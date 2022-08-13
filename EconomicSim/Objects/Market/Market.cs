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
    public class Market : IMarket
    {
        private IDictionary<IProduct, decimal> _paymentPreference;

        public Market()
        {
            Territories = new List<Territory.Territory>();
            Neighbors = new Dictionary<Market, decimal>();
            Resources = new Dictionary<Product, decimal>();
            Pops = new List<PopGroup>();
            Firms = new List<Firm>();
            MarketPrices = new Dictionary<IProduct, decimal>();
            ProductSold = new Dictionary<IProduct, decimal>();
            ProductOutput = new Dictionary<Product, decimal>();
            _paymentPreference = new Dictionary<IProduct, decimal>();
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

        // Assistant Props are properties which are not saved, but are used
        // in calculations during running.
        #region AssistantProperties

        /// <summary>
        /// The average price of goods on the market, based on
        /// the price of firms within the market and their relative
        /// market share.
        /// Recalculated on the run each day to allow for more accurate
        /// values rolling forward (values at startup are approximations).
        /// </summary>
        public Dictionary<IProduct, decimal> MarketPrices { get; set; }
        IReadOnlyDictionary<IProduct, decimal> IMarket.MarketPrices
            => ProductSold.ToDictionary(
                x => (IProduct)x.Key,
                x => x.Value);
        
        /// <summary>
        /// The amount of the product that has been successfully sold on
        /// the market so far. Updated to match daily.
        /// </summary>
        public Dictionary<IProduct, decimal> ProductSold { get; set; }
        IReadOnlyDictionary<IProduct, decimal> IMarket.ProductSold
            => ProductSold.ToDictionary(
                x => (IProduct)x.Key,
                x => x.Value);
        
        /// <summary>
        /// The amount of product that has been produced, but not
        /// necessarily sold, in the market.
        /// </summary>
        private Dictionary<Product, decimal> ProductOutput { get; set; }
        IReadOnlyDictionary<IProduct, decimal> IMarket.ProductOutput 
            => ProductOutput.ToDictionary(
                x => (IProduct)x.Key,
                x => x.Value);

        #endregion

        #region SafeAccessors

        /// <summary>
        /// Gets the market prices of a product.
        /// If it currently doesn't exist, then it defaults to 1.
        /// </summary>
        /// <param name="product">The product to get</param>
        /// <returns></returns>
        public decimal GetMarketPrice(IProduct product)
        {
            decimal result = 1;
            MarketPrices.TryGetValue(product, out result);
            return result;
        }

        #endregion

        /// <summary>
        /// Returns the list of products in payment preference order.
        /// The value is the preference for said product in payment,
        /// the higher the value the more it is preferred.
        /// </summary>
        public IDictionary<IProduct, decimal> PaymentPreference
        {
            get => _paymentPreference;
            private set => _paymentPreference = value;
        }

        /// <summary>
        /// The list of products which have reached currency status.
        /// </summary>
        public IList<IProduct> Currencies { get; set; }

        public override string ToString()
        {
            return Name;
        }

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
