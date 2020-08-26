using EconomicCalculator.Enums;
using EconomicCalculator.Intermediaries;
using EconomicCalculator.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Generators
{
    public class Process
    {
        public string Name { get; set; }

        public IList<IProduct> Inputs { get; set; }

        public IDictionary<string, double> InputRequirements { get; set; }

        public IList<IProduct> Outputs { get; set; }

        public IDictionary<string, double> OutputResults { get; set; }

        public double LaborRequirements { get; set; }

        public JobTypes JobType => JobTypes.Craft;

        public Guid Id => throw new NotImplementedException();

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
                .ToDictionary(x => x.Name, x => x.DefaultPrice * InputRequirements[x.Name])
                .Sum(x => x.Value);
        }

        public IDictionary<string, double> GetExpectedInputs(int count)
            => InputRequirements.ToDictionary(x => x.Key, x => x.Value * count / LaborRequirements);

        public IDictionary<string, double> Work(IDictionary<string, double> availableGoods, int Pops)
        {
            // Check consumption is possible. If not and something is missing, return an empty Dict.
            if (InputRequirements.Any(x => !availableGoods.ContainsKey(x.Key)))
                return new Dictionary<string, double>();

            // Calculate possible consumption, how many instances of the inputs we can meet for each.
            var inputs = InputRequirements.ToDictionary(x => x.Key, x => Math.Floor(availableGoods[x.Key] / x.Value));

            // How much labor can be satisfied.
            var doableWork = Math.Floor(Pops / LaborRequirements);

            // get the smallest value between input satisfaction and doable work.
            var maxWork = Math.Min(doableWork, inputs.Min(x => x.Value));

            // With the most work that we can do found, actually do it.
            // Subtract costs
            var result = InputRequirements.ToDictionary(x => x.Key, x => -x.Value * maxWork);

            // Add productions
            foreach (var product in OutputResults)
            {
                if (result.ContainsKey(product.Key))
                    result[product.Key] += product.Value * maxWork;
                else
                    result[product.Key] = product.Value * maxWork;
            }

            return result;
        }

        public override string ToString()
        {
            var result = string.Format("Name: {0}\n" +
                "LaborRequirements: {1}\n", Name, LaborRequirements);

            // Inputs
            result += "Input(s):\n";
            foreach(var input in Inputs)
            {
                result += string.Format("\t{0}: {1} {2}\n",
                    input.Name, InputRequirements[input.Name], input.UnitName);
            }

            // Outputs
            result += "Output(s):\n";
            foreach (var output in Outputs)
            {
                result += string.Format("\t{0}: {1} {2}\n",
                    output.Name, OutputResults[output.Name], output.UnitName);
            }

            result += "--------------------\n";

            return result;
        }

        public double TotalLaborRequired()
        {
            throw new NotImplementedException();
        }
    }
}
