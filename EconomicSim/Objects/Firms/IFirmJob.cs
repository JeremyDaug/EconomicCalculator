using System.Text.Json.Serialization;
using EconomicSim.Objects.Jobs;
using EconomicSim.Objects.Pops;

namespace EconomicSim.Objects.Firms;

[JsonConverter(typeof(FirmJobJsonConverter))]
public interface IFirmJob
{
    IJob Job { get; }
    WageType WageType { get; }
    decimal Wage { get; }
    IPopGroup Pop { get; set; }
}