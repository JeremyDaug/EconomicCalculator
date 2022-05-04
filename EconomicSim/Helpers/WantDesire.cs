using System.Text.Json.Serialization;
using EconomicSim.Objects.Pops;
using EconomicSim.Objects.Wants;

namespace EconomicSim.Helpers;

[JsonConverter(typeof(WantDesireJsonConverter))]
internal class WantDesire : IWantDesire
{
    public IWant Want { get; set; }
    public DesireTier Tier { get; set; }
    public decimal Amount { get; set; }
}