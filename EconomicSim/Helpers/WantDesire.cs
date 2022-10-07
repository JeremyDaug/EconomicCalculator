using System.Text.Json.Serialization;
using EconomicSim.Objects.Products;
using EconomicSim.Objects.Wants;

namespace EconomicSim.Helpers;

[JsonConverter(typeof(WantDesireJsonConverter))]
public class WantDesire : ADesire, IWantDesire
{
    public WantDesire() {}

    public WantDesire(IWantDesire other)
    {
        Want = other.Want;
        StartTier = other.StartTier;
        Step = other.Step;
        EndTier = other.EndTier;
        Amount = other.Amount;
    }
    
    public IWant Want { get; set; }
    public bool IsEquivalentTo(IWantDesire desire)
    {
        if (StartTier != desire.StartTier || // check it covers the same range
            EndTier != desire.EndTier ||
            Step != desire.Step ||
            !Equals(Want, desire.Want)) // and the same want
            return false;
        return true;
    }

    public override string ToString()
    {
        return Want.ToString() + " " + base.ToString();
    }
}