using EconomicSim.Objects.Products;

namespace EconomicSim.Helpers;

public class OwnUseConsume
{
    public IProduct Product { get; set; }
    public decimal Own { get; set; }
    public decimal Use { get; set; }
    public decimal Consume { get; set; }
    
}