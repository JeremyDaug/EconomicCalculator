using System.Text.Json.Serialization;
using EconomicSim.Objects.Jobs;
using EconomicSim.Objects.Pops;
using EconomicSim.Objects.Processes;

namespace EconomicSim.Objects.Firms;

[JsonConverter(typeof(FirmJobJsonConverter))]
public class FirmJob : IFirmJob
{
    public FirmJob()
    {
        Assignments = new Dictionary<IProcess, decimal>();
    }
    
    public Job Job { get; set; }
    IJob IFirmJob.Job => Job;
    public WageType WageType { get; set; }
    public decimal Wage { get; set; }
    
    /// <summary>
    /// The last recorded process target to reach.
    /// </summary>
    public Dictionary<IProcess, decimal> Assignments { get; set; }

    // Don't convert this, let pops connect to here.
    public PopGroup Pop { get; set; }
    IPopGroup IFirmJob.Pop
    {
        get => this.Pop;
        set => this.Pop = (PopGroup) value;
    }
}