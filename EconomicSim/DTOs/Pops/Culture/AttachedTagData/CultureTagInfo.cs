using EconomicSim.Objects.Pops.Culture;

namespace EconomicSim.DTOs.Pops.Culture.AttachedTagData
{
    [Obsolete]
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
