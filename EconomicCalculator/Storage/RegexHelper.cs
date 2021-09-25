using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage
{
    public static class RegexHelper
    {
        /// <summary>
        /// Regex for Decimal Values
        /// </summary>
        public static string Decimal => @"-?\d+(\.\d+)?";

        /// <summary>
        /// Regex for Integer Values
        /// </summary>
        public static string Integer => @"-?\d+";

        /// <summary>
        /// Regex for arbitrary words
        /// </summary>
        public static string Word => @"\w+";

        /// <summary>
        /// Regex for Products
        /// </summary>
        public static string Product => @"[a-zA-Z]+(\([a-zA-Z]+\))?";

        /// <summary>
        /// Regex for Wants
        /// </summary>
        public static string Want => @"[a-zA-Z]+";

        /// <summary>
        /// 
        /// </summary>
        public static string Character => @"\w";
    }
}
