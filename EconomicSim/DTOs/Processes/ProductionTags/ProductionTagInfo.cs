using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EconomicSim.DTOs.Enums;

namespace EconomicSim.DTOs.Processes.ProductionTags
{
    [Obsolete]
    public static class ProductionTagInfo
    {
        /// <summary>
        /// Gets the regex for a tag.
        /// </summary>
        /// <param name="tag">The tag we want regex for.</param>
        /// <returns>The regex string for that tag.</returns>
        public static string GetRegex(ProductionTag tag)
        {
            var result = "^" + tag.ToString();
            switch (tag)
            {
                case ProductionTag.Chance:
                    result += string.Format("<{0};{1}>$",
                        RegexHelper.Character,
                        RegexHelper.Integer);
                    return result;
                case ProductionTag.Optional:
                    result += string.Format("<{0}>$",
                        RegexHelper.Decimal);
                    return result;
                default:
                    return result + "$";
            }
        }

        /// <summary>
        /// Retrieve the parameters expected by a tag.
        /// </summary>
        /// <param name="tag">The tag we want parameters for.</param>
        /// <returns>A list of parameters for the tag.</returns>
        public static IList<ParameterType> GetTagParameterTypes(ProductionTag tag)
        {
            var result = new List<ParameterType>();
            switch (tag)
            {
                case ProductionTag.Chance:
                    result.Add(ParameterType.Character);
                    result.Add(ParameterType.Integer);
                    return result;
                case ProductionTag.Optional:
                    result.Add(ParameterType.Decimal);
                    return result;
                // Automation and Division of Labor not included yet.
                default:
                    return result;
            }
        }

        /// <summary>
        /// Gets a list of all ProductionTags in string form.
        /// Parameters not included.
        /// </summary>
        public static List<string> GetProductionTags()
        {
            return Enum.GetNames(typeof(ProductionTag)).ToList();
        }

        /// <summary>
        /// Get an example tag string based on a tag.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static string GetProductionTagExample(string tag)
        {
            return GetProductionTagExample((ProductionTag)Enum.Parse(typeof(ProductionTag), tag));
        }

        /// <summary>
        /// Get an example tag string based on a tag.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static string GetProductionTagExample(ProductionTag tag)
        {
            var example = tag.ToString();

            var parameters = GetTagParameterTypes(tag);

            if (!parameters.Any())
                return example;
            example += "<";

            // do each param
            foreach (var param in parameters)
            {
                example += ParameterHelper.ParameterExample(param) + ";";
            }
            // remove extra ;
            example = example.Remove(example.Length - 1);

            example += ">";

            return example;
        }

        public static IAttachedProductionTag GenerateFromTag(ProductionTag tag)
        {
            var result = new AttachedProductionTag();

            result.Tag = tag;

            var parameters = GetTagParameterTypes(tag);

            // if no tags
            if (!parameters.Any())
                return result;

            // tags
            foreach (var parameter in parameters)
            {
                switch (parameter)
                {
                    case ParameterType.Decimal:
                        result.Add(0.0M);
                        break;
                    case ParameterType.Integer:
                        result.Add(0);
                        break;
                    case ParameterType.Character:
                        result.Add('a');
                        break;
                }
            }

            return result;
        }
           
        /// <summary>
        /// Processes a raw tag string into an <seealso cref="IAttachedProductionTag"/>.
        /// </summary>
        /// <param name="tag">The tag to process.</param>
        /// <returns></returns>
        public static IAttachedProductionTag ProcessTagString(string tag)
        {
            var result = new AttachedProductionTag();

            // if < contained then it has parameters.
            if (tag.Contains("<"))
            {
                result.Tag = (ProductionTag)Enum.Parse(typeof(ProductionTag), tag.Split('<')[0]);
            }
            else // no params
            {
                result.Tag = (ProductionTag)Enum.Parse(typeof(ProductionTag), tag);
            }

            // with tag, double check regex validation.
            Regex rg = new Regex(GetRegex(result.Tag));
            if (!rg.IsMatch(tag))
            {
                throw new ArgumentException(
                    string.Format("Tag '{0}' is of an invalid format and must be of the form {1}.",
                    tag, rg.ToString()));
            }

            // since it's valid, go through and get the parameters.
            var parameters = GetTagParameterTypes(result.Tag);

            // if no parameters sought, then return the result so far.
            if (!parameters.Any())
                return result;

            // split parameters in text, everything after <, remove > at the end, and split on ;
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
                        var prodId = DTOManager.Instance.GetProductByFullName(paramStrings[i]).Id;
                        result.Add(prodId);
                        break;
                    case ParameterType.Want:
                        var wantId = DTOManager.Instance.GetWantByName(paramStrings[i]).Id;
                        result.Add(wantId);
                        break;
                    case ParameterType.Character:
                        result.Add(char.Parse(paramStrings[i]));
                        break;
                    default:
                        result.Add(paramStrings[i]);
                        break;
                }

                // Extra Checking here.
                // Non currnetly needed
            }

            // Got what we needed, return new AttachedTag.
            return result;
        }

        /// <summary>
        /// Which tags are invalid for Inputs
        /// </summary>
        /// <returns></returns>
        public static IList<ProductionTag> InvalidInputTags()
        {
            return new List<ProductionTag>
            {
                ProductionTag.Pollutant,
                ProductionTag.Chance,
                ProductionTag.Offset,
                ProductionTag.DivisionCapital,
                ProductionTag.AutomationCapital,
            };
        }
        
        /// <summary>
        /// Which tags are invalid for Capital.
        /// </summary>
        /// <returns></returns>
        public static IList<ProductionTag> InvalidCapitalTags()
        {
            return new List<ProductionTag>
            {
                ProductionTag.Consumed,
                ProductionTag.Pollutant,
                ProductionTag.Chance,
                ProductionTag.Offset,
                ProductionTag.DivisionInput,
                ProductionTag.AutomationInput
            };
        }

        /// <summary>
        /// Which tags are invalid for Outputs
        /// </summary>
        /// <returns></returns>
        public static IList<ProductionTag> InvalidOutputTags()
        {
            return new List<ProductionTag>
            {
                ProductionTag.AutomationCapital,
                ProductionTag.AutomationInput,
                ProductionTag.Consumed,
                ProductionTag.Optional,
                ProductionTag.Fixed,
                ProductionTag.DivisionCapital,
                ProductionTag.DivisionInput,
                ProductionTag.Investment
            };
        }

        /// <summary>
        /// Retrieves tags which are incompatable with the current tag.
        /// </summary>
        /// <param name="tag">The tag we are checking against for incompatability.</param>
        /// <returns></returns>
        public static IList<ProductionTag> IncompatableTags(ProductionTag tag)
        {
            // TODO, come back and refine this later.
            var result = new List<ProductionTag>();
            switch (tag)
            {
                case ProductionTag.Optional:
                case ProductionTag.Consumed:
                    if (tag != ProductionTag.Optional)
                        result.Add(ProductionTag.Optional);
                    if (tag != ProductionTag.Consumed)
                        result.Add(ProductionTag.Consumed);
                    result.Add(ProductionTag.Fixed);
                    result.Add(ProductionTag.Pollutant);
                    result.Add(ProductionTag.Chance);
                    result.Add(ProductionTag.Offset);
                    result.Add(ProductionTag.DivisionInput);
                    result.Add(ProductionTag.DivisionCapital);
                    result.Add(ProductionTag.AutomationCapital);
                    result.Add(ProductionTag.AutomationInput);
                    return result;
                case ProductionTag.Fixed:
                    result.Add(ProductionTag.Optional);
                    result.Add(ProductionTag.Consumed);
                    result.Add(ProductionTag.Pollutant);
                    result.Add(ProductionTag.Chance);
                    result.Add(ProductionTag.Offset);
                    result.Add(ProductionTag.DivisionInput);
                    result.Add(ProductionTag.DivisionCapital);
                    result.Add(ProductionTag.AutomationCapital);
                    result.Add(ProductionTag.AutomationInput);
                    return result;
                case ProductionTag.Investment:
                    return result;
                case ProductionTag.Pollutant:
                    result.AddRange(InvalidOutputTags());
                    result.Add(ProductionTag.Offset);
                    return result;
                case ProductionTag.Chance:
                    result.AddRange(InvalidOutputTags());
                    return result;
                case ProductionTag.Offset:
                    result.AddRange(InvalidOutputTags());
                    result.Add(ProductionTag.Pollutant);
                    return result;
                default:
                    return result;
            }
        }

        /// <summary>
        /// Ensures that a process product is valid, ensuring that all tags are
        /// respected.
        /// </summary>
        /// <param name="product">What is being checked.</param>
        /// <param name="placement"></param>
        /// <returns></returns>
        public static bool ProcessProductIsValid(IProcessProductDTO product, ProcessSection placement)
        {
            var processed = new HashSet<ProductionTag>();
            var invalid = new HashSet<ProductionTag>();

            if (placement == ProcessSection.Input)
                invalid.UnionWith(InvalidInputTags());
            if (placement == ProcessSection.Capital)
                invalid.UnionWith(InvalidCapitalTags());
            if (placement == ProcessSection.Output)
                invalid.UnionWith(InvalidOutputTags());

            foreach (var tag in product.Tags)
            {
                // if it's in the invalid bag, it's not valid.
                if (invalid.Contains(tag.Tag))
                    return false;
                // if it's already in the processed bag, no need to check again.
                else if (processed.Contains(tag.Tag))
                    continue;

                // if not processed or invalid, get the tags which are invalid and
                // add them to the invalid bag
                invalid.UnionWith(IncompatableTags(tag.Tag));
            }

            return true;
        }

        /// <summary>
        /// Ensures that a process want is valid, all tags are 
        /// respected appropriately.
        /// </summary>
        /// <param name="want">What is being checked.</param>
        /// <returns></returns>
        public static bool ProcesswantIsValid(IProcessWantDTO want, ProcessSection placement)
        {
            var processed = new HashSet<ProductionTag>();
            var invalid = new HashSet<ProductionTag>();

            if (placement == ProcessSection.Input)
                invalid.UnionWith(InvalidInputTags());
            if (placement == ProcessSection.Capital)
                invalid.UnionWith(InvalidCapitalTags());
            if (placement == ProcessSection.Output)
                invalid.UnionWith(InvalidOutputTags());

            foreach (var tag in want.Tags)
            {
                // if it's in the invalid bag, it's not valid.
                if (invalid.Contains(tag.Tag))
                    return false;
                // if it's already in the processed bag, no need to check again.
                else if (processed.Contains(tag.Tag))
                    continue;

                // if not processed or invalid, get the tags which are invalid and
                // add them to the invalid bag
                invalid.UnionWith(IncompatableTags(tag.Tag));
            }

            return true;
        }
    }
}
