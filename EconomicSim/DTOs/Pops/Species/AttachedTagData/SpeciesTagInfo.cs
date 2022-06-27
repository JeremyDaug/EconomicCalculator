using EconomicSim.Objects.Pops.Species;

namespace EconomicSim.DTOs.Pops.Species.AttachedTagData
{
    [Obsolete]
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
