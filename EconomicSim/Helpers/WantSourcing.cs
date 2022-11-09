using EconomicSim.Objects.Processes;
using EconomicSim.Objects.Products;
using EconomicSim.Objects.Wants;

namespace EconomicSim.Helpers;

/// <summary>
/// A class for storing the sources of a want's satisfaction and the results expected.
/// </summary>
public class WantSourcing
{
    public WantSourcing(IWant want)
    {
        Want = want;
        // setup possible sources, since we know what their sources are.
        foreach (var val in want.OwnershipSources)
            OwnSource.Add(val, 0);
        foreach (var val in want.UseSources)
            UseSource.Add(val.UseProcess!, 0);
        foreach (var val in want.ConsumptionSources)
            ConsumptionSource.Add(val.ConsumptionProcess!, 0);
    }
    
    /// <summary>
    /// The want we are sourcing.
    /// </summary>
    public IWant Want { get; set; }
    
    /// <summary>
    /// How much of the want we are trying to get.
    /// </summary>
    public decimal Target { get; set; }
    
    /// <summary>
    /// How much of the want we can currently get.
    /// </summary>
    public decimal Projected { get; set; }
    
    /// <summary>
    /// Extras of this want produced elsewhere and available to us.
    /// </summary>
    public decimal Free { get; set; }

    /// <summary>
    /// The Ownership sources for this want, product and number of the product.
    /// </summary>
    public Dictionary<IProduct, decimal> OwnSource { get; set; } = new();
    
    /// <summary>
    /// The Use Process Sources for this want we are using.
    /// Process and number of cycles expected.
    /// </summary>
    public Dictionary<IProcess, decimal> UseSource { get; set; } = new();
    
    /// <summary>
    /// The Consumption Process Sources for this want we are using.
    /// Process and number of cycles expected.
    /// </summary>
    public Dictionary<IProcess, decimal> ConsumptionSource { get; set; } = new();

    /// <summary>
    /// Calculates what is required for all of these sources to properly function.
    /// Does not include outputs.
    /// </summary>
    /// <returns>
    /// productsEffected are the products used/consumed
    /// wantsChanged are the wants consumed.
    /// </returns>
    public (Dictionary<IProduct, ProcessEffect> productsEffected,
        Dictionary<IWant, decimal> wantsChanged)
        WantSourcingRequirements()
    {
        Dictionary<IWant, decimal> wantResult = new();
        Dictionary<IProduct, ProcessEffect> prodResult = new();
        
        // do ownership outputs first and mark the owned products as used.
        foreach (var (prod, amount) in OwnSource)
        { // only inputs
            if (!prodResult.ContainsKey(prod))
                prodResult.Add(prod, new ProcessEffect());
            prodResult[prod].Use += amount;
        }
        
        // then do use processes
        foreach (var (proc, iter) in UseSource)
        {
            foreach (var part in proc.ProcessProducts
                         .Where(x => x.Part != ProcessPartTag.Output))
            { // products
                if (!prodResult.ContainsKey(part.Product))
                    prodResult.Add(part.Product, new ProcessEffect());
                if (part.Part == ProcessPartTag.Input)
                    prodResult[part.Product].Change -= part.Amount * iter;
                else if (part.Part == ProcessPartTag.Capital)
                    prodResult[part.Product].Use += part.Amount * iter;
            }
            foreach (var part in proc.ProcessWants
                         .Where(x => x.Part != ProcessPartTag.Output))
            { // wants
                if (!wantResult.ContainsKey(part.Want))
                    wantResult.Add(part.Want, 0);
                if (part.Part == ProcessPartTag.Input)
                    wantResult[part.Want] -= part.Amount * iter;
            }
        }
        
        // then do consumption processes
        foreach (var (proc, iter) in ConsumptionSource)
        {
            foreach (var part in proc.ProcessProducts
                         .Where(x => x.Part != ProcessPartTag.Output))
            { // products
                if (!prodResult.ContainsKey(part.Product))
                    prodResult.Add(part.Product, new ProcessEffect());
                if (part.Part == ProcessPartTag.Input)
                    prodResult[part.Product].Change -= part.Amount * iter;
                else if (part.Part == ProcessPartTag.Capital)
                    prodResult[part.Product].Use += part.Amount * iter;
            }
            foreach (var part in proc.ProcessWants
                         .Where(x => x.Part != ProcessPartTag.Output))
            { // wants
                if (!wantResult.ContainsKey(part.Want))
                    wantResult.Add(part.Want, 0);
                if (part.Part == ProcessPartTag.Input)
                    wantResult[part.Want] -= part.Amount * iter;
            }
        }
        
        return (prodResult, wantResult);
    }

    /// <summary>
    /// Calculates the outputs of using all our sources.
    /// Does not include inputs.
    /// </summary>
    /// <returns>
    /// productsEffected is how the products are created.
    /// wantsChanged are the number of wants output.
    /// </returns>
    public (Dictionary<IProduct, ProcessEffect> productsEffected,
        Dictionary<IWant, decimal> wantsChanged)
        WantSourcingOutputs()
    {
                Dictionary<IWant, decimal> wantResult = new();
        Dictionary<IProduct, ProcessEffect> prodResult = new();
        
        // do ownership outputs first
        foreach (var (prod, amount) in OwnSource)
        {
            foreach (var (want, output) in prod.Wants)
            { // just wants outputted
                if (!wantResult.ContainsKey(want))
                    wantResult.Add(want, 0);
                wantResult[want] += output * amount;
            }
        }
        
        // then do use processes
        foreach (var (proc, iter) in UseSource)
        {
            foreach (var part in proc.ProcessProducts
                         .Where(x => x.Part == ProcessPartTag.Output))
            { // products
                if (!prodResult.ContainsKey(part.Product))
                    prodResult.Add(part.Product, new ProcessEffect());
                prodResult[part.Product].Change += part.Amount * iter;
            }
            foreach (var part in proc.ProcessWants
                         .Where(x => x.Part == ProcessPartTag.Output))
            { // wants
                if (!wantResult.ContainsKey(part.Want))
                    wantResult.Add(part.Want, 0);
                wantResult[part.Want] += part.Amount * iter;
            }
        }
        
        // then do consumption processes
        foreach (var (proc, iter) in ConsumptionSource)
        {
            foreach (var part in proc.ProcessProducts
                         .Where(x => x.Part == ProcessPartTag.Output))
            { // products
                if (!prodResult.ContainsKey(part.Product))
                    prodResult.Add(part.Product, new ProcessEffect());
                prodResult[part.Product].Change += part.Amount * iter;
            }
            foreach (var part in proc.ProcessWants
                         .Where(x => x.Part == ProcessPartTag.Output))
            { // wants
                if (!wantResult.ContainsKey(part.Want))
                    wantResult.Add(part.Want, 0);
                wantResult[part.Want] += part.Amount * iter;
            }
        }
        
        return (prodResult, wantResult);
    }
    
    /// <summary>
    /// Calculates the total results of using all our sources,
    /// combining inputs and outputs.
    /// Also updates projected Output
    /// </summary>
    /// <returns>
    /// productsEffected is how the products are used/consumed/created.
    /// wantsChanged are the changed number of wants net (outputs - inputs).
    /// </returns>
    public (Dictionary<IProduct, ProcessEffect> productsEffected,
        Dictionary<IWant, decimal> wantsChanged)
        WantSourcingSummary()
    {
        Dictionary<IWant, decimal> wantResult = new();
        Dictionary<IProduct, ProcessEffect> prodResult = new();
        
        // do ownership outputs first and mark the owned products as used.
        foreach (var (prod, amount) in OwnSource)
        {
            if (!prodResult.ContainsKey(prod))
                prodResult.Add(prod, new ProcessEffect());
            prodResult[prod].Use += amount;
            foreach (var (want, output) in prod.Wants)
            {
                if (!wantResult.ContainsKey(want))
                    wantResult.Add(want, 0);
                wantResult[want] += output * amount;
            }
        }
        
        // then do use processes
        foreach (var (proc, iter) in UseSource)
        {
            foreach (var part in proc.ProcessProducts)
            { // products
                if (!prodResult.ContainsKey(part.Product))
                    prodResult.Add(part.Product, new ProcessEffect());
                if (part.Part == ProcessPartTag.Input)
                    prodResult[part.Product].Change -= part.Amount * iter;
                else if (part.Part == ProcessPartTag.Capital)
                    prodResult[part.Product].Use += part.Amount * iter;
                else // if output
                    prodResult[part.Product].Change += part.Amount * iter;
            }
            foreach (var part in proc.ProcessWants)
            { // wants
                if (!wantResult.ContainsKey(part.Want))
                    wantResult.Add(part.Want, 0);
                if (part.Part == ProcessPartTag.Input)
                    wantResult[part.Want] -= part.Amount * iter;
                else // output 
                    wantResult[part.Want] += part.Amount * iter;
            }
        }
        
        // then do consumption processes
        foreach (var (proc, iter) in ConsumptionSource)
        {
            foreach (var part in proc.ProcessProducts)
            { // products
                if (!prodResult.ContainsKey(part.Product))
                    prodResult.Add(part.Product, new ProcessEffect());
                if (part.Part == ProcessPartTag.Input)
                    prodResult[part.Product].Change -= part.Amount * iter;
                else if (part.Part == ProcessPartTag.Capital)
                    prodResult[part.Product].Use += part.Amount * iter;
                else // if output
                    prodResult[part.Product].Change += part.Amount * iter;
            }
            foreach (var part in proc.ProcessWants)
            { // wants
                if (!wantResult.ContainsKey(part.Want))
                    wantResult.Add(part.Want, 0);
                if (part.Part == ProcessPartTag.Input)
                    wantResult[part.Want] -= part.Amount * iter;
                else // output 
                    wantResult[part.Want] += part.Amount * iter;
            }
        }

        Projected = wantResult[Want];
        
        return (prodResult, wantResult);
    }
}