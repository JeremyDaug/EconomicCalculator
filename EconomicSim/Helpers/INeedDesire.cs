using System.Text.Json.Serialization;
using EconomicSim.Objects.Products;

namespace EconomicSim.Helpers;

[JsonConverter(typeof(NeedDesireJsonConverter))]
public interface INeedDesire
{
    /// <summary>
    /// The product desired.
    /// </summary>
    IProduct Product { get; }
    
    /// <summary>
    /// The tier of the desire.
    /// </summary>
    int Tier { get; }
    
    /// <summary>
    /// The amount requested
    /// </summary>
    decimal Amount { get; set; }
    
    /// <summary>
    /// How much of the Desire has been satisfied so far. Resets before daily consumption
    /// for record purposes. Not Stored in Json.
    /// </summary>
    decimal Satisfaction { get; set; }
}