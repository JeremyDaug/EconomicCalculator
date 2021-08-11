using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage.ProductTags
{
    /// <summary>
    /// A Product tag and it's information.
    /// </summary>
    public class ProductTagInfo : IProductTagInfo
    {
        /// <summary>
        /// The tag's Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The tag's name and text.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// How many Parameters it can accept.
        /// </summary>
        public int Params { get; set; }

        /// <summary>
        /// The data of the Product Tag
        /// </summary>
        public string Description { get; set; }
    }
}
