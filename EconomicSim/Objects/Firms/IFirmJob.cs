using System.Text.Json.Serialization;
using EconomicSim.Objects.Jobs;

namespace EconomicSim.Objects.Firms;

[JsonConverter(typeof(FirmJobJsonConverter))]
public interface IFirmJob
{
    IJob Job { get; }
    WageType WageType { get; }
    decimal Wage { get; }
}