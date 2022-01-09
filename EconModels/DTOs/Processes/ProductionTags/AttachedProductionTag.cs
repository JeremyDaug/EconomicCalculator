using EconDTOs.DTOs.Enums;
using System.Collections.Generic;

namespace EconDTOs.DTOs.Processes.ProductionTags
{
    public class AttachedProductionTag : IAttachedProductionTag
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public AttachedProductionTag()
        {
            parameters = new List<object>();
        }

        /// <summary>
        /// Indexor.
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

        /// <summary>
        /// Add object to list of parameter values.
        /// </summary>
        /// <param name="obj"></param>
        public void Add(object obj)
        {
            parameters.Add(obj);
        }

        /// <summary>
        /// The attached tag.
        /// </summary>
        public ProductionTag Tag { get; set; }

        /// <summary>
        /// List of parameter types.
        /// if Empty, there are no expected parameters.
        /// </summary>
        public IList<ParameterType> TagParameterTypes { get; set; }

        private IList<object> parameters;

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