namespace EconomicSim.Helpers;

/// <summary>
/// A class which holds how a product or want is to be used or changed
/// for a process. Either Used (capital) or changed (input/output).
/// </summary>
public class ProcessEffect
{
    public decimal Use { get; set; }
    public decimal Change { get; set; }
}