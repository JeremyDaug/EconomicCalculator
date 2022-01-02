using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Objects.Processes.ProductionTags
{
    public interface IProductionTagData
    {
        /// <summary>
        /// The tag attached to the production item.
        /// </summary>
        ProductionTag Tag { get; }

        /// <summary>
        /// Indexor
        /// </summary>
        /// <param name="i"></param>
        /// <returns>The parameter at that index.</returns>
        object this[int i] { get; }
    }
}
