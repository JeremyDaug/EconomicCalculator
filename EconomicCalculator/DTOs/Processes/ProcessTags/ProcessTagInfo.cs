using EconomicCalculator.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EconomicCalculator.DTOs.Processes.ProcessTags
{
    /// <summary>
    /// Information on available process Tags.
    /// </summary>
    public static class ProcessTagInfo
    {
        public static string GetRegex(ProcessTag tag)
        {
            var result = "^" + tag.ToString();
            switch (tag)
            {
                case ProcessTag.Scrapping:
                    result += string.Format("<{0}>$",
                        RegexHelper.Product);
                    return result;
                default:
                    return result + "$";
            }
        }

        public static IList<ParameterType> GetTagParameterTypes(ProcessTag tag)
        {
            var result = new List<ParameterType>();
            switch (tag)
            {
                case ProcessTag.Scrapping:
                    result.Add(ParameterType.Product);
                    return result;
                default:
                    return result;
            }
        }

        public static List<string> GetProcessTags()
        {
            return Enum.GetNames(typeof(ProcessTag)).ToList();
        }

        public static string GetProcessTagExample(ProcessTag tag)
        {
            var example = tag.ToString();

            var parameters = GetTagParameterTypes(tag);

            if (!parameters.Any())
                return example;

            example += "<";

            // params
            foreach (var param in parameters)
            {
                example += ParameterHelper.ParameterExample(param) + ";";
            }
            // remove extra ;
            example = example.Remove(example.Length - 1);

            example += ">";

            return example;
        }

        public static string GetProcessTagExample(string tag)
        {
            return GetProcessTagExample((ProcessTag)Enum.Parse(typeof(ProcessTag), tag));
        }

        public static IAttachedProcessTag ProcessTagString (string tag)
        {
            var result = new AttachedProcessTag();

            // if < contained then it has params
            if (tag.Contains("<"))
            {
                result.Tag = (ProcessTag)Enum.Parse(typeof(ProcessTag), tag.Split('<')[0]);
            }
            else // no params
                result.Tag = (ProcessTag)Enum.Parse(typeof(ProcessTag), tag);

            // with tag, check regex is valid.
            Regex rg = new Regex(GetRegex(result.Tag));
            if (!rg.IsMatch(tag))
            {
                throw new ArgumentException(
                    string.Format("Tag '{0}' is of an invalid format and must be of the form {1}.",
                    tag, rg.ToString()));
            }

            // since it's valid, get the params
            var parameters = GetTagParameterTypes(result.Tag);

            // if no params, we're done.
            if (!parameters.Any())
                return result;

            var paramStrings = tag.Split('<')[1].TrimEnd('>').Split(';');

            if (paramStrings.Count() != parameters.Count())
                throw new ArgumentException(string.Format("Tag Parameter Count does not match. Expected {0}", parameters.Count()));

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
                    case ParameterType.Character:
                        result.Add(char.Parse(paramStrings[i]));
                        break;
                    default:
                        result.Add(paramStrings[i]);
                        break;
                }

                // extra checking here maybe.
            }

            // complete
            return result;
        }

        public static IList<ProcessTag> InvalidTags(ProcessTag tag)
        {
            // all tags are exclusive with each other
            var result = new List<ProcessTag>
            {
                ProcessTag.Failure,
                ProcessTag.Consumption,
                ProcessTag.Maintenance,
                ProcessTag.Chance,
                ProcessTag.Crop,
                ProcessTag.Mine,
                ProcessTag.Extractor,
                ProcessTag.Tap,
                ProcessTag.Refiner,
                ProcessTag.Sorter,
                ProcessTag.Scrapping
            };

            // exept
            // Extractor or Tap which can be combined with
            // Refiner or Sorter (exclusively).
            if (ProcessTag.Extractor == tag ||
                ProcessTag.Tap == tag)
            {
                result.Remove(ProcessTag.Sorter);
                result.Remove(ProcessTag.Refiner);
            }
            else if (tag == ProcessTag.Refiner ||
                     tag == ProcessTag.Sorter)
            {
                result.Remove(ProcessTag.Extractor);
                result.Remove(ProcessTag.Tap);
            }

            return result;
        }

        public static IAttachedProcessTag ConstructTag(ProcessTag tag)
        {
            // currently, no process tags have parameters
            switch (tag)
            {
                case ProcessTag.Scrapping:
                    var temp = new AttachedProcessTag
                    {
                        Tag = tag
                    };
                    temp.Add(Manager.Instance.Products.Values.First().GetName());
                    return temp;
                default:
                    return new AttachedProcessTag
                    {
                        Tag = tag,
                    };
            }
        }
    }
}
