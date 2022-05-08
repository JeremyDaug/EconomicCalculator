using System.Text.Json.Serialization;

namespace EconomicSim.DTOs.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Flags]
    public enum ParameterType
    {
        /// <summary>
        /// Placeholder, it expects anything.
        /// </summary>
        Any = 0,

        /// <summary>
        /// Expects the Parameter to be an integer.
        /// </summary>
        Integer = 1,

        /// <summary>
        /// Expects the parameter to be a decimal value.
        /// </summary>
        Decimal = 2,

        /// <summary>
        /// Expects the Parameter to be a product, variant may or may not
        /// be included.
        /// </summary>
        Product = 4,

        /// <summary>
        /// Expects the parameter to be a want.
        /// </summary>
        Want = 8,

        /// <summary>
        /// Expects a word or words in CamelCase.
        /// </summary>
        Word = 16,

        /// <summary>
        /// Expects a singular Character.
        /// </summary>
        Character = 32,
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
                    return @"-?\d*(\.\d*)?";  // any decimal.
                case ParameterType.Product:
                    return @"\w+(\(\w+\))?"; // any string with another string in ( )
                case ParameterType.Want:
                case ParameterType.Word:
                    return @"\w+"; // any string
                case ParameterType.Character:
                    return @"\w";
                default:
                    throw new ArgumentException("Parameter does not exist.");
            }
        }

        /// <summary>
        /// Get's an example of a parameter.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string ParameterExample(ParameterType param)
        {
            switch (param)
            {
                case ParameterType.Integer:
                    return "1.1";
                case ParameterType.Decimal:
                    return "1";
                case ParameterType.Product:
                    return "Product(Variant)";
                case ParameterType.Want:
                    return "Want";
                case ParameterType.Word:
                    return "Word";
                case ParameterType.Character:
                    return "A";
                default:
                    throw new ArgumentException("Parameter does not exist.");
            }
        }
    }
}
