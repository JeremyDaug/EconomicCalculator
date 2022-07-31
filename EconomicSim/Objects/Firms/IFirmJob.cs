using System.Text.Json.Serialization;
using EconomicSim.Objects.Jobs;
using EconomicSim.Objects.Pops;
using EconomicSim.Objects.Processes;

namespace EconomicSim.Objects.Firms;

[JsonConverter(typeof(FirmJobJsonConverter))]
public interface IFirmJob
{
    IJob Job { get; }
    WageType WageType { get; }
    decimal Wage { get; }
    /// <summary>
    /// Stores progress for the processes in assignments.
    /// Investments and optional goods are consumed at the start.
    /// If a process is cancelled, it should refund the non-service inputs.
    /// Currently not saved in Json.
    /// </summary>
    Dictionary<IProcess, IAssignmentInfo> Assignments { get; set; }
    
    IPopGroup Pop { get; set; }
}