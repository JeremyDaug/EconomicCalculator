using System.Text.Json.Serialization;
using EconomicSim.Objects.Jobs;

namespace EconomicSim.Objects.Firms;

[JsonConverter(typeof(FirmJobJsonConverter))]
public class FirmJob : IFirmJob
{
    public FirmJob() {}
    
    public Job Job { get; set; }
    IJob IFirmJob.Job => Job;
    public WageType WageType { get; set; }
    public decimal Wage { get; set; }
}