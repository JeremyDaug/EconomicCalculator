using EconomicCalculator.Enums;
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
        /// Default Ctor
        /// </summary>
        public ProductTagInfo() {}

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="selected">What we are copying.</param>
        public ProductTagInfo(ProductTagInfo c)
        {
            Id = c.Id;
            Tag = c.Tag;
            Params = c.Params;
            Description = c.Description;
        }

        /// <summary>
        /// The tag's Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The tag's name and text.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// How many Parameters it expects.
        /// </summary>
        public List<ParameterType> Params { get; set; }

        /// <summary>
        /// Describes the product tag and how it's parameters are meant to function.
        /// </summary>
        public string Description { get; set; }

        public int ParamCount => Params.Count();

        public string RegexPattern
        {
            get
            {
                string result = Tag;

                if (ParamCount > 0)
                {
                    result += "<";

                    foreach (var param in Params)
                    {
                        result += ParameterHelper.RegexType(param) + ";";
                    }

                    result.TrimEnd(';');

                    result += ">";
                }

                return result;
            }
        }
    }
}
