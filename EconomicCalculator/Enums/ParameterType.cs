﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EconomicCalculator.Enums
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
        Word = 16
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
                    return @"\w+"; // any string
                default:
                    throw new ArgumentException("Parameter does not exist.");
            }
        }
    }
}
