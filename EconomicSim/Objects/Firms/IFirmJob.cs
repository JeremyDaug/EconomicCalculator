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
    /// The last recorded process target to reach.
    /// </summary>
    Dictionary<IProcess, decimal> Assignments { get; set; }
    IPopGroup Pop { get; set; }
}