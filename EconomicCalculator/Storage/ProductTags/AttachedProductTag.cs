using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconomicCalculator.Enums;

namespace EconomicCalculator.Storage.ProductTags
{
    /// <summary>
    /// Product Tag attached to a product.
    /// </summary>
    internal class AttachedProductTag : IAttachedProductTag
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public AttachedProductTag()
        {
            parameters = new List<object>();
        }

        /// <summary>
        /// The Tag attached to the product.
        /// </summary>
        public ProductTag Tag { get; set; }

        /// <summary>
        /// List of parameter types.
        /// If empty, there are no expected parameters.
        /// </summary>
        public IList<ParameterType> TagParameterTypes => ProductTagInfo.GetTagParameterTypes(Tag);

        /// <summary>
        /// Indexor
        /// </summary>
        /// <param name="i">The index To Access</param>
        /// <returns>The Parameter at that index.</returns>
        /// <exception cref="IndexOutOfRangeException"/>
        public object this[int i] {
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
        /// Add object to parameter value list.
        /// </summary>
        /// <param name="obj"></param>
        public void Add(object obj)
        {
            parameters.Add(obj);
        }

        private IList<object> parameters;
    }
}
