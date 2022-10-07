using System.Text.Json.Serialization;
using EconomicSim.Objects.Wants;

namespace EconomicSim.Helpers;

[JsonConverter(typeof(WantDesireJsonConverter))]
public interface IWantDesire : IDesire
{
    /// <summary>
    /// The product desired.
    /// </summary>
    IWant Want { get; }
    
    /// <summary>
    /// Checks to see if the desire is equivalent to another
    /// Does not include checking amount or satisfaction, only the tiers it covers.
    /// </summary>
    /// <param name="desire"></param>
    /// <returns></returns>
    bool IsEquivalentTo(IWantDesire desire);
}