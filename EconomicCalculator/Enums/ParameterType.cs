using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EconomicCalculator.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ParameterType
    {
        /// <summary>
        /// Expects the Parameter to be an integer.
        /// </summary>
        Integer,

        /// <summary>
        /// Expects the parameter to be a decimal value.
        /// </summary>
        Decimal,

        /// <summary>
        /// Expects the Parameter to be a product, variant may or may not
        /// be included.
        /// </summary>
        Product,

        /// <summary>
        /// Expects the parameter to be a want.
        /// </summary>
        Want
    }

    public static class ParameterHelper
    {
        public static string RegexType(ParameterType param)
        {
            switch (param)
            {
                case ParameterType.Integer:
                    return @"-?\d*"; // any integer.
                case ParameterType.Decimal:
                    return @"\d*(\.\d*)?";  // any decimal.
                case ParameterType.Product:
                    return @"\w+(\(\w+\))?"; // any string with another string in ( )
                case ParameterType.Want:
                    return @"\w+"; // any string
                default:
                    throw new ArgumentException("Parameter does not exist.");
            }
        }
    }
}
