using EconomicCalculator.Intermediaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Generators
{
    public class Process : IProcess
    {
        public string Name { get; set; }

        public IList<IProduct> Inputs { get; set; }

        public IDictionary<string, double> InputRequirements { get; set; }

        public IList<IProduct> Outputs { get; set; }

        public IDictionary<string, double> OutputResults { get; set; }

        public double LaborRequirements { get; set; }

        public Process()
        {
            Inputs = new List<IProduct>();
            InputRequirements = new Dictionary<string, double>();
            Outputs = new List<IProduct>();
            OutputResults = new Dictionary<string, double>();
        }

        public double ProductionCost()
        {
            return Inputs
                .ToDictionary(x => x.Name, x => x.CurrentPrice * InputRequirements[x.Name])
                .Sum(x => x.Value);
        }
    }
}
