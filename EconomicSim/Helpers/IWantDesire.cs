using System.Text.Json.Serialization;
using EconomicSim.Objects.Pops;
using EconomicSim.Objects.Wants;

namespace EconomicSim.Helpers;

[JsonConverter(typeof(WantDesireJsonConverter))]
public interface IWantDesire
{
    /// <summary>
    /// The product desired.
    /// </summary>
    IWant Want { get; }
    
    /// <summary>
    /// The tier of the desire.
    /// </summary>
    DesireTier Tier { get; }
    
    /// <summary>
    /// The amount requested
    /// </summary>
    decimal Amount { get; }
}