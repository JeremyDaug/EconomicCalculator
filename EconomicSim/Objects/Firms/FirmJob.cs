using System.Text.Json.Serialization;
using EconomicSim.Objects.Jobs;
using EconomicSim.Objects.Pops;

namespace EconomicSim.Objects.Firms;

[JsonConverter(typeof(FirmJobJsonConverter))]
public class FirmJob : IFirmJob
{
    public FirmJob() {}
    
    public Job Job { get; set; }
    IJob IFirmJob.Job => Job;
    public WageType WageType { get; set; }
    public decimal Wage { get; set; }
    public PopGroup Pop { get; set; }
    IPopGroup IFirmJob.Pop
    {
        get => this.Pop;
        set => this.Pop = (PopGroup) value;
    }
}