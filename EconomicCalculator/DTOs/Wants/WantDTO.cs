using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EconomicCalculator.DTOs.Wants
{
    /// <summary>
    /// The Wants available to the system.
    /// </summary>
    public class WantDTO : IWantDTO
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public WantDTO() { }

        /// <summary>
        /// Copy Constructor.
        /// </summary>
        /// <param name="other">What we are copying.</param>
        public WantDTO(WantDTO other)
        {
            Id = other.Id;
            Name = other.Name;
        }

        /// <summary>
        /// The Want Id
        /// </summary>
        [JsonIgnore]
        public int Id { get; set; }

        /// <summary>
        /// The Want Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A short description of the want and how it is should be
        /// used elsewhere.
        /// </summary>
        public string Description { get; set; }

        public override string ToString()
        {
            return Name;
        }

        /// Want<1.0> Want name can only have letters. 
        /// Parameter must be a number (with or without decimals).
        /// </summary>
        public static readonly string Pattern = @"^[a-zA-Z]+<\d+(\.\d+)?>$";

        public static readonly string FormatPattern = @"{0}<{1}>";

        public static bool IsValid(string s)
        {
            Regex rg = new Regex(Pattern);

            return rg.IsMatch(s);
        }

        public static bool NameIsValid(string s)
        {
            Regex rg = new Regex("[a-zA-Z]+");

            return rg.IsMatch(s);
        }

        public string ToSatisfactionString(decimal d)
        {
            return string.Format(FormatPattern, Name, d);
        }

        public static Tuple<string, decimal> DataFromString(string s)
        {
            if (!IsValid(s))
                throw new ArgumentException("String does not match Pattern 'Want<#>'.");

            var split = s.Split('<');
            var Name = split[0];
            var value = decimal.Parse(split[1].TrimEnd('>'));
            return new Tuple<string, decimal>(Name, value);
        }

        public static WantDTO FromString(string s)
        {
            if (!IsValid(s))
                throw new ArgumentException("String does not match Pattern 'Want<#>'.");

            var result = new WantDTO
            {
                Name = s.Split('<')[0]
            };

            return result;
        }

        public static string NameFromString(string s)
        {
            if (!IsValid(s))
                throw new ArgumentException("String does not match Pattern 'Want<#>'.");

            var name = s.Split('<')[0];

            return name;
        }

        public static decimal GetValue(string s)
        {
            if (!IsValid(s))
                throw new ArgumentException("String does not match Pattern 'Want<#>'.");

            var val = s.Substring(s.IndexOf('<')).TrimEnd('>');

            return decimal.Parse(val);
        }
    }
}
