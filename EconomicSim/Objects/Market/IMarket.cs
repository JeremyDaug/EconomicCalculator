using System.Text.Json.Serialization;
using EconomicSim.Objects.Firms;
using EconomicSim.Objects.Pops;
using EconomicSim.Objects.Products;
using EconomicSim.Objects.Territory;

namespace EconomicSim.Objects.Market
{
    /// <summary>
    /// Market Read Only Interface
    /// </summary>
    [JsonConverter(typeof(MarketJsonConverter))]
    public interface IMarket
    {
        /// <summary>
        /// ID of the market.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The name of the market, usually the name the region or city
        /// it covers.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The firms that are operating in this market.
        /// </summary>
        IReadOnlyList<IFirm> Firms { get; }

        /// <summary>
        /// The Population of the market.
        /// </summary>
        IReadOnlyList<IPopGroup> Pops { get; }

        /// <summary>
        /// The governing body of the market.
        /// </summary>
        //IGovernor Governor { get; }

        /// <summary>
        /// The Territories the Market Contains.
        /// </summary>
        IReadOnlyList<ITerritory> Territories { get; }

        /// <summary>
        /// The Markets which are considered adjacent to this one
        /// and their distance measured in km. 
        /// (Default distance is measured from center to center of
        /// the respective territories).
        /// These Neighbors should be reachable by foot assuming no
        /// rivers.
        /// </summary>
        IReadOnlyDictionary<IMarket, decimal> Neighbors { get; }

        /// <summary>
        /// The resources that are loose in the market, unowned.
        /// These resources may be picked up and used at will by anyone
        /// in the market.
        /// </summary>
        IReadOnlyDictionary<IProduct, decimal> Resources { get; }
        
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
        IReadOnlyDictionary<IProduct, decimal> MarketPrices { get; }
        
        /// <summary>
        /// The amount of product that was put up for sale today.
        /// </summary>
        IReadOnlyDictionary<IProduct, decimal> ProductsForSale { get; }
        
        /// <summary>
        /// The amount of the product that has been successfully sold on
        /// the market so far. Updated to match daily.
        /// </summary>
        IReadOnlyDictionary<IProduct, decimal> ProductSold { get; }
        
        /// <summary>
        /// The amount of product that has been produced, but not
        /// necessarily sold, in the market.
        /// </summary>
        IReadOnlyDictionary<IProduct, decimal> ProductOutput { get; }

        /// <summary>
        /// The Total value exchanged for a product in the market today.
        /// </summary>
        IReadOnlyDictionary<IProduct, decimal> ProductExchangeTotal { get; }

        #endregion
        
        #region SafeAccessors

        /// <summary>
        /// Gets the market prices of a product.
        /// If it currently doesn't exist, then it defaults to 1.
        /// </summary>
        /// <param name="product">The product to get</param>
        /// <returns></returns>
        decimal GetMarketPrice(IProduct product);

        #endregion

        #region DebugAndInfoLogging

        // Something something logging prices in a nice way.
        // Probably appending to a file or list somewhere. 

        #endregion

        void AddSeller(ICanSell seller);
    }
}
