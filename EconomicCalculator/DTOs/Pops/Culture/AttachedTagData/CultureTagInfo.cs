using EconomicCalculator.Objects.Pops.Culture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.DTOs.Pops.Culture.AttachedTagData
{
    public static class CultureTagInfo
    {
        public static string GetRegex(CultureTag tag)
        {
            var result = "^" + tag.ToString();
            switch (tag)
            {
                case CultureTag.BioPreference:
                    result += string.Format("<{0};{1}>$",
                        RegexHelper.Species, RegexHelper.Decimal);
                    return result;
                case CultureTag.JobPreference:
                    result += string.Format("<{0};{1}>$",
                        RegexHelper.Job, RegexHelper.Decimal);
                    return result;
                default:
                    return result + "$";
            }
        }
    }
}
