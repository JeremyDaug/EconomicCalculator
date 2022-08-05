using System.Text.Json.Serialization;
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
    int Tier { get; }
    
    /// <summary>
    /// The amount requested
    /// </summary>
    decimal Amount { get; set; }
    
    /// <summary>
    /// How much of the Desire has been satisfied so far. Resets before daily consumption
    /// for record purposes. Not Stored in JSon.
    /// </summary>
    decimal Satisfaction { get; set; }
}