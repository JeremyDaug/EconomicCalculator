using EconomicCalculator.DTOs.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.DTOs.Processes.ProcessTags
{
    public class AttachedProcessTag : IAttachedProcessTag
    {
        public AttachedProcessTag()
        {
            parameters = new List<object>();
        }

        private IList<object> parameters;

        /// <summary>
        /// Indexor
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public object this[int i]
        {
            get
            {
                return parameters[i];
            }
            set
            {
                parameters[i] = value;
            }
        }

        public void Add(object obj)
        {
            parameters.Add(obj);
        }

        public ProcessTag Tag { get; set; }

        public IList<ParameterType> TagParameterTypes { get; set; }

        public override string ToString()
        {
            var result = Tag.ToString();

            if (parameters.Count == 0)
                return result;

            result += "<";

            foreach (var item in parameters)
                result += item.ToString() + ";";

            result = result.TrimEnd(';');

            result += ">";

            return result;
        }
    }
}
