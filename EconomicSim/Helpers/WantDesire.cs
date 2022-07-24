using System.Text.Json.Serialization;
using EconomicSim.Objects.Wants;

namespace EconomicSim.Helpers;

[JsonConverter(typeof(WantDesireJsonConverter))]
public class WantDesire : IWantDesire
{
    public IWant Want { get; set; }
    public int Tier { get; set; }
    public decimal Amount { get; set; }
}