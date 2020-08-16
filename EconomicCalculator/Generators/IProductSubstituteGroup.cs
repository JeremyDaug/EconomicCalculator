using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Generators
{
    /// <summary>
    /// Product Substitute Groups define groups of products 
    /// </summary>
    public interface IProductSubstituteGroup
    {
        /// <summary>
        /// The Id of the product Substitution Group.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// The name of the group, usually a name of one of the products.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The Product Groups that are part of the substitute group. These are organized by relative quality then by name.
        /// </summary>
        IList<IProductGroup> Goods { get; }

        /// <summary>
        /// Retrieves the relative qualities of all product groups.
        /// </summary>
        IDictionary<string, double> GroupQualities();
    }
}
