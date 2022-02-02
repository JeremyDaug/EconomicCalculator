using EconomicCalculator.Enums;
using EconomicCalculator.Objects.Pops.Species;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.DTOs.Pops.Species.AttachedTagData
{
    public static class SpeciesTagInfo
    {
        public static string GetRegex(SpeciesTag tag)
        {
            var result = "^" + tag.ToString();
            switch (tag)
            {
                case SpeciesTag.CultureModifier:
                    result += string.Format("<{0};{1}>", 
                        RegexHelper.Culture, RegexHelper.Decimal);
                    return result;
                default:
                    return result + "$";
            }
        }
    }
}
