using EconomicSim.Objects.Processes;
using EconomicSim.Objects.Products;
using EconomicSim.Objects.Wants;

namespace EconomicSim.Helpers;

/// <summary>
/// A class for storing the sources of a want's satisfaction and the results expected.
/// </summary>
public class WantSourcing
{
    /// <summary>
    /// The Ownership sources for this want, product and number of the product.
    /// </summary>
    public Dictionary<IProduct, decimal> OwnSource { get; set; }
    
    /// <summary>
    /// The Use Process Sources for this want we are using.
    /// Process and number of cycles expected.
    /// </summary>
    public Dictionary<IProcess, decimal> UseSource { get; set; }
    
    /// <summary>
    /// The Consumption Process Sources for this want we are using.
    /// Process and number of cycles expected.
    /// </summary>
    public Dictionary<IProcess, decimal> ConsumptionSource { get; set; }

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
                    prodResult[part.Product].Change -= part.Amount;
                else if (part.Part == ProcessPartTag.Capital)
                    prodResult[part.Product].Use += part.Amount;
            }
            foreach (var part in proc.ProcessWants
                         .Where(x => x.Part != ProcessPartTag.Output))
            { // wants
                if (!wantResult.ContainsKey(part.Want))
                    wantResult.Add(part.Want, 0);
                if (part.Part == ProcessPartTag.Input)
                    wantResult[part.Want] -= part.Amount;
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
                    prodResult[part.Product].Change -= part.Amount;
                else if (part.Part == ProcessPartTag.Capital)
                    prodResult[part.Product].Use += part.Amount;
            }
            foreach (var part in proc.ProcessWants
                         .Where(x => x.Part != ProcessPartTag.Output))
            { // wants
                if (!wantResult.ContainsKey(part.Want))
                    wantResult.Add(part.Want, 0);
                if (part.Part == ProcessPartTag.Input)
                    wantResult[part.Want] -= part.Amount;
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
                wantResult[want] += output;
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
                prodResult[part.Product].Change += part.Amount;
            }
            foreach (var part in proc.ProcessWants
                         .Where(x => x.Part == ProcessPartTag.Output))
            { // wants
                if (!wantResult.ContainsKey(part.Want))
                    wantResult.Add(part.Want, 0);
                wantResult[part.Want] += part.Amount;
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
                prodResult[part.Product].Change += part.Amount;
            }
            foreach (var part in proc.ProcessWants
                         .Where(x => x.Part == ProcessPartTag.Output))
            { // wants
                if (!wantResult.ContainsKey(part.Want))
                    wantResult.Add(part.Want, 0);
                wantResult[part.Want] += part.Amount;
            }
        }
        
        return (prodResult, wantResult);
    }
    
    /// <summary>
    /// Calculates the total results of using all our sources,
    /// combining inputs and outputs.
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
                wantResult[want] += output;
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
                    prodResult[part.Product].Change -= part.Amount;
                else if (part.Part == ProcessPartTag.Capital)
                    prodResult[part.Product].Use += part.Amount;
                else // if output
                    prodResult[part.Product].Change += part.Amount;
            }
            foreach (var part in proc.ProcessWants)
            { // wants
                if (!wantResult.ContainsKey(part.Want))
                    wantResult.Add(part.Want, 0);
                if (part.Part == ProcessPartTag.Input)
                    wantResult[part.Want] -= part.Amount;
                else // output 
                    wantResult[part.Want] += part.Amount;
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
                    prodResult[part.Product].Change -= part.Amount;
                else if (part.Part == ProcessPartTag.Capital)
                    prodResult[part.Product].Use += part.Amount;
                else // if output
                    prodResult[part.Product].Change += part.Amount;
            }
            foreach (var part in proc.ProcessWants)
            { // wants
                if (!wantResult.ContainsKey(part.Want))
                    wantResult.Add(part.Want, 0);
                if (part.Part == ProcessPartTag.Input)
                    wantResult[part.Want] -= part.Amount;
                else // output 
                    wantResult[part.Want] += part.Amount;
            }
        }
        
        return (prodResult, wantResult);
    }
}