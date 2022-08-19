using EconomicSim.Objects.Market;
using EconomicSim.Objects.Products;

namespace EconomicSim.Objects;

/// <summary>
/// Used by those who can put things for sale in the market.
/// </summary>
public interface ICanSell
{
    /// <summary>
    /// The total weight (chances) of the seller being selected.
    /// </summary>
    Dictionary<IProduct, decimal> SellWeight { get; set; }
    
    /// <summary>
    /// Whether the Seller is actually selling or not.
    /// </summary>
    bool IsSelling { get; set; }
    
    /// <summary>
    /// The items available for sale and in what quantity.
    /// </summary>
    IDictionary<IProduct, decimal> ForSale { get; }

    /// <summary>
    /// The market the Seller is operating in.
    /// </summary>
    IMarket Market { get; }

    /// <summary>
    /// Retrieves the prices of a product for sale.
    /// </summary>
    /// <param name="product">The product which is being priced.</param>
    /// <returns>The price in abstract units.</returns>
    decimal SalePrice(IProduct product);

    /// <summary>
    /// Sets the products for sale and sets up for pricing them as well.
    /// </summary>
    /// <returns></returns>
    Task<ICanSell> SellPhase();
}