using System.Text.Json.Serialization;
using EconomicSim.Objects.Products;

namespace EconomicSim.Helpers;

[JsonConverter(typeof(NeedDesireJsonConverter))]
public interface INeedDesire : IDesire
{
    /// <summary>
    /// The product desired.
    /// </summary>
    IProduct Product { get; }
    
    /// <summary>
    /// Checks to see if the desire is equivalent to another
    /// Does not include checking amount or satisfaction, only the tiers it covers.
    /// </summary>
    /// <param name="desire"></param>
    /// <returns></returns>
    bool IsEquivalentTo(INeedDesire desire);
}