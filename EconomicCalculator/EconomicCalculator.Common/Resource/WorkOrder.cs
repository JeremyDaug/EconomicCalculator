using System;
using System.Collections.Generic;
using System.Text;

namespace EconomicCalculator.Common.Resource
{
    public class WorkOrder
    {
        public string Name { get; set; }
        public double DailyWage { get; set; }
        public double MinimumWorkersNeeded { get; set; }
        public double TotalWorkLength { get; set; }
    }
}
