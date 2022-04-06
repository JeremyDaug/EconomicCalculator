using EconomicCalculator.Objects.Firms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EconomicCalculator.DTOs.Firms
{
    public class JobWageData : IJobWageData
    {
        [JsonIgnore]
        public int JobId { get; set; }

        public string JobName { get; set; }

        [JsonIgnore]
        public WageType WageTypeEnum { get; set; }

        public string WageType 
        {
            get
            {
                return WageTypeEnum.ToString();
            }
            set
            {
                WageTypeEnum = (WageType)Enum.Parse(typeof(WageType), value);
            }
        }

        public decimal Wage { get; set; }

        public override string ToString()
        {
            return String.Format("{0} for {1} {2}",
                JobName, Wage, WageType);
        }
    }
}
