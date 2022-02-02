using EconomicCalculator.Enums;
using EconomicCalculator.Objects.Pops.Species;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.DTOs.Pops.Species.AttachedTagData
{
    public class AttachedSpeciesTag : IAttachedSpeciesTag
    {
        public AttachedSpeciesTag()
        {
            parameters = new List<object>();
        }

        private IList<object> parameters;

        public object this[int index]
        {
            get
            {
                return parameters[index];
            }
            set
            {
                parameters[index] = value;
            }
        }

        public void Add(object obj)
        {
            parameters.Add(obj);
        }

        public SpeciesTag Tag { get; set; }

        public IList<ParameterType> TagParameterTypes { get; set; }

        public override string ToString()
        {
            var result = Tag.ToString();

            if (parameters.Count == 0)
                return result;

            foreach (var param in parameters)
                result += param.ToString() + ";";

            result = result.TrimEnd(';');

            result += ">";

            return result;
        }
    }
}
