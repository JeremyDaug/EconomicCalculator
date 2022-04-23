using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
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
