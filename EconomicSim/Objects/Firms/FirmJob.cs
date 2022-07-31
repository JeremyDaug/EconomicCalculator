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
        Assignments = new Dictionary<IProcess, IAssignmentInfo>();
    }
    
    public Job Job { get; set; }
    IJob IFirmJob.Job => Job;
    public WageType WageType { get; set; }
    public decimal Wage { get; set; }
    
    /// <summary>
    /// Stores progress for the processes in assignments.
    /// Investments and optional goods are consumed at the start.
    /// If a process is cancelled, it should refund the non-service inputs.
    /// Currently not saved in Json.
    /// </summary>
    public Dictionary<IProcess, IAssignmentInfo> Assignments { get; set; }

    // Don't convert this, let pops connect to here.
    public PopGroup Pop { get; set; }
    IPopGroup IFirmJob.Pop
    {
        get => this.Pop;
        set => this.Pop = (PopGroup) value;
    }

    public override string ToString()
    {
        return $"{Job.GetName()}";
    }
}