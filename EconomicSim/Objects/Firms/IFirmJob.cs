using System.Text.Json.Serialization;
using EconomicSim.Objects.Jobs;
using EconomicSim.Objects.Market;
using EconomicSim.Objects.Pops;
using EconomicSim.Objects.Processes;
using EconomicSim.Objects.Products;

namespace EconomicSim.Objects.Firms;

[JsonConverter(typeof(FirmJobJsonConverter))]
public interface IFirmJob
{
    IJob Job { get; }
    WageType WageType { get; }
    
    /// <summary>
    /// The size of the wage. If a wage product exists, it is in units of that product or
    /// market value equivalent.
    /// </summary>
    public decimal Wage { get; set; }
    
    /// <summary>
    /// The product of payment used as the unit of pay.
    /// if null, wage is in the 'abstract' value of the local market.
    /// </summary>
    IProduct? WageUnit { get; set; }
    
    /// <summary>
    /// Stores progress for the processes in assignments.
    /// Investments and optional goods are consumed at the start.
    /// If a process is cancelled, it should refund the non-service inputs.
    /// Currently not saved in Json.
    /// </summary>
    Dictionary<IProcess, IAssignmentInfo> Assignments { get; set; }

    /// <summary>
    /// Gets the processes of the job with the value of the outputs attached.
    /// </summary>
    /// <param name="market">The market we get our value from.</param>
    /// <returns></returns>
    IReadOnlyDictionary<IProcess, decimal> ProcessesByOutputValue(IMarket market);

    IPopGroup Pop { get; set; }
}