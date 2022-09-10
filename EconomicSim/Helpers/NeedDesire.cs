using System.Text.Json.Serialization;
using EconomicSim.Objects.Products;

namespace EconomicSim.Helpers;

[JsonConverter(typeof(NeedDesireJsonConverter))]
public class NeedDesire : ADesire, INeedDesire
{
    public NeedDesire() {}

    public NeedDesire(INeedDesire other)
    {
        Product = other.Product;
        IsConsumed = other.IsConsumed;
        StartTier = other.StartTier;
        Step = other.Step;
        EndTier = other.EndTier;
        Amount = other.Amount;
    }

    public IProduct Product { get; set; }
}