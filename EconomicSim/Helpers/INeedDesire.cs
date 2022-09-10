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
}