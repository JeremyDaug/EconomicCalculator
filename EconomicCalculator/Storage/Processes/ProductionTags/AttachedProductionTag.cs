using System.Collections.Generic;
using EconomicCalculator.Enums;

namespace EconomicCalculator.Storage.Processes.ProductionTags
{
    internal class AttachedProductionTag : IAttachedProductionTag
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
    }
}