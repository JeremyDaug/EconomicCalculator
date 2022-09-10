using System.Text.Json.Serialization;
using EconomicSim.Objects.Wants;

namespace EconomicSim.Helpers;

[JsonConverter(typeof(WantDesireJsonConverter))]
public class WantDesire : ADesire, IWantDesire
{
    public WantDesire() {}

    public WantDesire(IWantDesire other)
    {
        Want = other.Want;
        IsConsumed = other.IsConsumed;
        StartTier = other.StartTier;
        Step = other.Step;
        EndTier = other.EndTier;
        Amount = other.Amount;
    }
    
    public IWant Want { get; set; }
}