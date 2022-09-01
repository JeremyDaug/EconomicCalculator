using EconomicSim.Objects.Market;
using EconomicSim.Objects.Products;

namespace EconomicSim.Objects;

public interface ICanBuy
{
    /// <summary>
    /// Whether we are buying or not.
    /// </summary>
    bool IsBuying { get; }
    
    /// <summary>
    /// The items we are willing to exchange for goods.
    /// </summary>
    IReadOnlyDictionary<IProduct, decimal> ForExchange { get; }

    // maybe a second for specifically currencies.
    
    /// <summary>
    /// How much was spent this day.
    /// </summary>
    decimal Expenditures { get; }
    
    // Expenditure record here?

    /// <summary>
    /// The 'good' budget the buyer makes. This amount is used to calculate other
    /// budget meters also.
    /// </summary>
    decimal Budget { get; }
    
    /// <summary>
    /// What the buyer is seeking to purchase.
    /// </summary>
    IReadOnlyDictionary<IProduct, decimal> ShoppingList { get; }

    /// <summary>
    /// We go to our market to try and buy goods that we desire.
    /// </summary>
    void BuyGoods();
}