using EconomicCalculator.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage.ProductTags
{
    public static class ProductTagInfo
    {
        /// <summary>
        /// Get's the expected Regex for a tag.
        /// </summary>
        /// <param name="tag">The tag we want the Regex for.</param>
        /// <returns>The Desired Regex String.</returns>
        public static string GetRegex(ProductTag tag)
        {
            var result = "^" + tag.ToString();
            switch (tag)
            {
                case ProductTag.Bargain:
                case ProductTag.Luxury:
                    // Decimal; String extra checking needed on string.
                    result += "<" + RegexHelper.Decimal + ";" + RegexHelper.Word
                        + ">$";
                    return result;
                case ProductTag.Claim:
                    // String, Must be a checked against products/firms
                    result += "<" + RegexHelper.Word + ">$";
                    return result;
                case ProductTag.Atomic:
                    // Integer and Integer, check for negative.
                    result += "<" + RegexHelper.Integer + ";"
                        + RegexHelper.Integer + ">$";
                    return result;
                case ProductTag.Energy:
                    // Decimal, should be positive.
                    result += "<" + RegexHelper.Decimal + ">$";
                    return result;
                default: // default tag has no parameters, just the tagName.
                    return result + "$";
            }
        }

        /// <summary>
        /// Retrieve the Parameters expected by a tag.
        /// </summary>
        /// <param name="tag">The Tag in question.</param>
        /// <returns>The list of parameters Expected.</returns>
        public static IList<ParameterType> GetTagParameterTypes(ProductTag tag)
        {
            var result = new List<ParameterType>();

            switch (tag)
            {
                case ProductTag.Bargain:
                case ProductTag.Luxury:
                    // Decimal; String extra checking needed on string.
                    result.Add(ParameterType.Decimal);
                    result.Add(ParameterType.Want);
                    return result;
                case ProductTag.Claim:
                    // String, Must be a checked against products/firms
                    result.Add(ParameterType.Word);
                    return result;
                case ProductTag.Atomic:
                    // Integer and Integer, check for negative.
                    result.Add(ParameterType.Integer);
                    result.Add(ParameterType.Integer);
                    return result;
                case ProductTag.Energy:
                    // Decimal, should be positive.
                    result.Add(ParameterType.Decimal);
                    return result;
                default: // default tag has no parameters.
                    return result;
            }
        }

        public static IAttachedProductTag ProcessTagString(string tag)
        {
            var result = new AttachedProductTag();

            // if < contained, then it has parameters.
            if (tag.Contains("<"))
            {
                result.Tag = (ProductTag)Enum.Parse(typeof(ProductTag), tag.Split('<')[0]);
            }
            else // no params
            {
                result.Tag = (ProductTag)Enum.Parse(typeof(ProductTag), tag);
            }

            // with tag, double check regex validation.
            Regex rg = new Regex(GetRegex(result.Tag));
            if (!rg.IsMatch(tag))
            {
                throw new ArgumentException(
                    string.Format("Tag '{0}' is of an invalid format and must be of the form {1}",
                    tag, rg.ToString()));
            }

            // since it's valid, go through and get the parameters.
            var parameters = GetTagParameterTypes(result.Tag);

            // if no parameters sought, return the result so far.
            if (!parameters.Any())
                return result;

            // split parameters in text, everything after <, remove > at the end, and split along ';'
            var paramStrings = tag.Split('<')[1].TrimEnd('>').Split(';');

            // ensure params got and params expected match.
            if (paramStrings.Count() != parameters.Count())
                throw new ArgumentException(string.Format("Tag Parameter Count does not match. Expected {0}", parameters.Count()));

            // go through each param and parse it.
            for (int i = 0; i < parameters.Count(); ++i)
            {
                switch (parameters[i])
                {
                    case ParameterType.Decimal:
                        result.Add(decimal.Parse(paramStrings[i]));
                        break;
                    case ParameterType.Integer:
                        result.Add(int.Parse(paramStrings[i]));
                        break;
                    case ParameterType.Product:
                        var prodId = Manager.Instance.GetProductByName(paramStrings[i]).Id;
                        result.Add(prodId);
                        break;
                    case ParameterType.Want:
                        var wantId = Manager.Instance.GetWantByName(paramStrings[i]).Id;
                        result.Add(wantId);
                        break;
                    default:
                        result.Add(paramStrings[i]);
                        break;
                }
            }

            // everything has been gotten. Return new AttachedTag.
            return result;
        }
    }
}
