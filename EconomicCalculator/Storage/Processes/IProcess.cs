using EconomicCalculator.Storage.Processes.ProcessTags;
using EconomicCalculator.Storage.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage.Processes
{
    /// <summary>
    /// Process Interface, the core of any action which creates or destroys
    /// products and resources.
    /// </summary>
    public interface IProcess
    {
        /// <summary>
        /// The Id of the process
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The name of the process, must form a unique combination 
        /// with <seealso cref="VariantName"/>.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The variant name of this process, if it's similar, but not
        /// exactly the same.
        /// </summary>
        /// <remarks>
        /// Must form unique combination with <seealso cref="Name"/>.
        /// </remarks>
        string VariantName { get; }

        /// <summary>
        /// The minimum amount of time the process takes up.
        /// 1 equals 1 day.
        /// </summary>
        decimal MinimumTime { get; }

        // waste heat?

        /// <summary>
        /// Input Products
        /// </summary>
        List<IProcessProduct> InputProducts { get; }

        /// <summary>
        /// Input Wants
        /// </summary>
        List<IProcessWant> InputWants { get; }

        /// <summary>
        /// String form of the inputs for nice viewing.
        /// </summary>
        [JsonIgnore]
        string InputString { get; }

        /// <summary>
        /// Capital Products.
        /// </summary>
        List<IProcessProduct> CapitalProducts { get; }

        /// <summary>
        /// Capital Wants (IE, non-consumable products with the proper want attached).
        /// </summary>
        List<IProcessWant> CapitalWants { get; }

        /// <summary>
        /// String for of capital goods for nice viewing.
        /// </summary>
        [JsonIgnore]
        string CapitalString { get; }

        /// <summary>
        /// Product Outputs.
        /// </summary>
        List<IProcessProduct> OutputProducts { get; set; }

        /// <summary>
        /// Want outputs of the process.
        /// </summary>
        List<IProcessWant> OutputWants { get; set; }

        /// <summary>
        /// String form of outputs for nice view.
        /// </summary>
        [JsonIgnore]
        string OutputString { get; }

        /// <summary>
        /// String form of our process's tags.
        /// </summary>
        List<string> TagStrings { get; }

        /// <summary>
        /// Process Tags
        /// </summary>
        [JsonIgnore]
        List<IAttachedProcessTag> Tags { get; }

        /// <summary>
        /// String form of all our tags.
        /// </summary>
        [JsonIgnore]
        string TagString { get; }

        /// <summary>
        /// The Skill the process uses.
        /// </summary>
        [JsonIgnore]
        int SkillId { get; }

        /// <summary>
        /// The name of the skill the process uses.
        /// </summary>
        string SkillName { get; }

        /// <summary>
        /// The minimum level of the skill.
        /// </summary>
        decimal SkillMinimum { get; }

        /// <summary>
        /// The maximum level of the skill
        /// </summary>
        decimal SkillMaximum { get; }

        /// <summary>
        /// Whether the process is allowed to produce fractional goods.
        /// </summary>
        [JsonIgnore]
        bool Fractional { get; }

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
        string Description { get; }

        /// <summary>
        /// The Icon for the process
        /// </summary>
        string Icon { get; }

        /// <summary>
        /// Get the Name(VariantName) of the process in
        /// standard form.
        /// </summary>
        /// <returns></returns>
        string GetName();
    }
}
