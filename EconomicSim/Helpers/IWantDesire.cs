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
}