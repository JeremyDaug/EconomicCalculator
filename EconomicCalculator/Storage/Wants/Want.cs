﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage.Wants
{
    /// <summary>
    /// The Wants available to the system.
    /// </summary>
    public class Want : IWant
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public Want() { }

        /// <summary>
        /// Copy Constructor.
        /// </summary>
        /// <param name="other">What we are copying.</param>
        public Want(Want other)
        {
            Id = other.Id;
            Name = other.Name;
        }

        /// <summary>
        /// The Want Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The Want Name
        /// </summary>
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public static readonly string Pattern = @"\w+<\d+>$";

        public static readonly string FormatPattern = @"{0}<{1}>";

        public static bool IsValid(string s)
        {
            Regex rg = new Regex(Pattern);

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

        public static Want FromString(string s)
        {
            if (!IsValid(s))
                throw new ArgumentException("String does not match Pattern 'Want<#>'.");

            var result = new Want
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
