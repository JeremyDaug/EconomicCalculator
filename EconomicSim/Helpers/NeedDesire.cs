using System.Text.Json.Serialization;
using EconomicSim.Objects.Products;

namespace EconomicSim.Helpers;

[JsonConverter(typeof(NeedDesireJsonConverter))]
public class NeedDesire : INeedDesire
{
    public IProduct Product { get; set; }
    public int Tier { get; set; }
    public decimal Amount { get; set; }
    public decimal Satisfaction { get; set; }
}