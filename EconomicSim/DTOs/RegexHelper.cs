using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicSim.DTOs
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
        /// Regex for Cultures
        /// </summary>
        public static string Culture => @"[a-zA-Z]+(\([a-zA-Z]+\))?";

        /// <summary>
        /// Regex for Species
        /// </summary>
        public static string Species => @"[a-zA-Z]+(\([a-zA-Z]+\))?";

        /// <summary>
        /// Regex for Job
        /// </summary>
        public static string Job => @"[a-zA-Z]+(\([a-zA-Z]+\))?";

        /// <summary>
        /// Regex for Wants
        /// </summary>
        public static string Want => @"[a-zA-Z]+";

        /// <summary>
        /// Regex for individual Character.
        /// </summary>
        public static string Character => @"\w";

        /// <summary>
        /// Regex for Phrase of letters and whitespace.
        /// Any characters except parentheses.
        /// </summary>
        public static string Phrase => @"[^()]+";

        /// <summary>
        /// Regex for Full Process Name.
        /// Any Character except parentheses, no space, then parentheses and another name.
        /// </summary>
        public static string Process => @"[^()]+[^() ](\([a-zA-Z ]+\))?";
    }
}
