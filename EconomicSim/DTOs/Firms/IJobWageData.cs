using System.Text.Json.Serialization;
using EconomicSim.Objects.Firms;

namespace EconomicSim.DTOs.Firms
{
    public interface IJobWageData
    {
        [JsonIgnore]
        int JobId { get; }

        string JobName { get; }

        [JsonIgnore]
        WageType WageTypeEnum { get; }

        string WageType { get; }

        decimal Wage { get; }
    }
}
