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
        StartTier = other.StartTier;
        Step = other.Step;
        EndTier = other.EndTier;
        Amount = other.Amount;
    }

    public IProduct Product { get; set; }
    public bool IsEquivalentTo(INeedDesire desire)
    {
        if (StartTier != desire.StartTier || // check it covers the same range
            EndTier != desire.EndTier ||
            Step != desire.Step ||
            !Equals(Product, desire.Product)) // and the same want
            return false;
        return true;
    }

    public override string ToString()
    {
        return Product.ToString() + " " + base.ToString();
    }
}