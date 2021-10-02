﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using EconomicCalculator.Storage.Processes.ProcessTags;
using EconomicCalculator.Storage.Skills;

namespace EconomicCalculator.Storage.Processes
{
    /// <summary>
    /// Process Class, the core of any which creates or destroys products and
    /// resources.
    /// </summary>
    public class Process : IProcess
    {
        public Process()
        {
            InputProducts = new List<IProcessProduct>();
            InputWants = new List<IProcessWant>();
            CapitalProducts = new List<IProcessProduct>();
            CapitalWants = new List<IProcessWant>();
            Outputs = new List<IProcessProduct>();
            Tags = new List<IAttachedProcessTag>();
        }

        /// <summary>
        /// The Id of the process
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the process, must form a unique combination 
        /// with <seealso cref="VariantName"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The variant name of this process, if it's similar, but not
        /// exactly the same.
        /// </summary>
        /// <remarks>
        /// Must form unique combination with <seealso cref="Name"/>.
        /// </remarks>
        public string VariantName { get; set; }

        /// <summary>
        /// The minimum amount of time the process takes up.
        /// 1 equals 1 day.
        /// </summary>
        public decimal MinimumTime { get; set; }

        // waste heat?

        /// <summary>
        /// Input Products
        /// </summary>
        public List<IProcessProduct> InputProducts { get; set; }

        /// <summary>
        /// Input Wants
        /// </summary>
        public List<IProcessWant> InputWants { get; set; }

        /// <summary>
        /// String form of the inputs for nice viewing.
        /// </summary>
        [JsonIgnore]
        public string InputString
        {
            get
            {
                var result = "";

                foreach (var input in InputProducts)
                    result += input.ToString() + "\n";
                foreach (var input in InputWants)
                    result += input.ToString() + "\n";

                return result;
            }
        }

        /// <summary>
        /// Capital Products.
        /// </summary>
        public List<IProcessProduct> CapitalProducts { get; set; }

        /// <summary>
        /// Capital Wants (IE, non-consumable products with the proper want attached).
        /// </summary>
        public List<IProcessWant> CapitalWants { get; set; }

        /// <summary>
        /// String for of capital goods for nice viewing.
        /// </summary>
        [JsonIgnore]
        public string CapitalString
        {
            get
            {
                var result = "";

                foreach (var cap in CapitalProducts)
                    result += cap.ToString() + "\n";
                foreach (var cap in CapitalWants)
                    result += cap.ToString() + "\n";

                return result;
            }
        }

        /// <summary>
        /// Product Outputs.
        /// </summary>
        public List<IProcessProduct> Outputs { get; set; }
        
        /// <summary>
        /// String form of outputs for nice view.
        /// </summary>
        [JsonIgnore]
        public string OutputString
        {
            get
            {
                var result = "";

                foreach (var output in Outputs)
                    result += output.ToString() + "\n";

                return result;
            }
        }

        /// <summary>
        /// String form of our process's tags.
        /// </summary>
        public List<string> TagStrings { get; set; }

        /// <summary>
        /// Process Tags
        /// </summary>
        [JsonIgnore]
        public List<IAttachedProcessTag> Tags { get; set; }

        /// <summary>
        /// String form of all our tags.
        /// </summary>
        [JsonIgnore]
        public string TagString
        {
            get
            {
                var result = "";

                foreach (var val in TagStrings)
                    result += val + ";";

                result = result.TrimEnd(';');

                return result;
            }
        }

        /// <summary>
        /// The Skill the process uses.
        /// </summary>
        [JsonIgnore]
        public int SkillId { get; set; }

        /// <summary>
        /// The name of the skill the process uses.
        /// </summary>
        public string SkillName { get; set; }

        /// <summary>
        /// The minimum level of the skill.
        /// </summary>
        public int SkillMinimum { get; set; }

        /// <summary>
        /// The maximum level of the skill
        /// </summary>
        public int SkillMaximum { get; set; }

        /// <summary>
        /// Whether the process is allowed to produce fractional goods.
        /// If any input, capital, or output is not fractional, then this process
        /// cannot be fractional.
        /// </summary>
        [JsonIgnore]
        public bool Fractional
        {
            get
            {
                // if any input or capital good is not fractional, then the process cannot be.
                if (InputProducts.Any(x => !Manager.Instance.Products[x.ProductId].Fractional))
                    return false;
                if (CapitalProducts.Any(x => !Manager.Instance.Products[x.ProductId].Fractional))
                    return false;
                if (Outputs.Any(x => !Manager.Instance.Products[x.ProductId].Fractional))
                    return false;
                return true;
            }
        }

        // Division of Labor
        // split into it's own class for more flexible modification.
        // TODO Division of labor

        // Automation
        // Split into it's own class for easier modification.
        // TODO Automation

        // Technology connections

        /// <summary>
        /// Description of the process
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The Icon for the process
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Given a name, it returns it and any variant name contained in a string.
        /// </summary>
        /// <param name="name">The name(vairant) name we are processing.</param>
        /// <returns></returns>
        public static Tuple<string, string> GetProcessNames(string name)
        {
            if (name.Contains("("))
            {
                var prodNames = name.Split('(');
                var prodName = prodNames[0];
                var varName = prodNames[1].TrimEnd(')');
                return new Tuple<string, string>(prodName, varName);
            }
            else
                return new Tuple<string, string>(name, null);
        }
    }
}
