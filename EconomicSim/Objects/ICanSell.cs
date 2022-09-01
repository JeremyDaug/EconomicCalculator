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
    IDictionary<IProduct, decimal> SellWeight { get; }
    
    /// <summary>
    /// Whether the Seller is actually selling or not.
    /// </summary>
    bool IsSelling { get; }
    
    /// <summary>
    /// The items available for sale and in what quantity.
    /// </summary>
    IReadOnlyDictionary<IProduct, decimal> ForSale { get; }
    
    /// <summary>
    /// The total put up for sale at the end of the sell phase.
    /// </summary>
    IReadOnlyDictionary<IProduct, decimal> OriginalStock { get; }
    
    /// <summary>
    /// The Items the seller has sold today.
    /// </summary>
    IReadOnlyDictionary<IProduct, decimal> GoodsSold { get; }

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
    
    /// <summary>
    /// How much in abstract value the seller has earned today.
    /// </summary>
    decimal Revenue { get; }

    /// <summary>
    /// Starts an exchange between a buyer and a seller.
    /// </summary>
    /// <param name="buyer"></param>
    /// <returns></returns>
    Task StartExchange(ICanBuy buyer);
}