using System.Text.Json.Serialization;
using EconomicSim.Objects.Processes.ProcessTags;
using EconomicSim.Objects.Products;
using EconomicSim.Objects.Skills;
using EconomicSim.Objects.Technology;
using EconomicSim.Objects.Wants;

namespace EconomicSim.Objects.Processes
{
    /// <summary>
    /// Readonly Process Interface
    /// </summary>
    [JsonConverter(typeof(ProcessJsonConverter))]
    public interface IProcess
    {
        /// <summary>
        /// The Id of the Process
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

        #region InCapOut

        /// <summary>
        /// All the products that go into or come out of the process.
        /// </summary>
        IReadOnlyList<IProcessProduct> ProcessProducts { get; }

        /// <summary>
        /// All the wants that go into or come out of the process.
        /// </summary>
        IReadOnlyList<IProcessWant> ProcessWants { get; }

        /// <summary>
        /// Input Products, consumed by the process.
        /// </summary>
        IReadOnlyList<IProcessProduct> InputProducts { get; }

        /// <summary>
        /// Capital Products, used, but not consumed by the process.
        /// </summary>
        IReadOnlyList<IProcessProduct> CapitalProducts { get; }

        /// <summary>
        /// Output Products, resulting products from the process.
        /// </summary>
        IReadOnlyList<IProcessProduct> OutputProducts { get; }

        /// <summary>
        /// Input Wants, consumed by the process.
        /// </summary>
        IReadOnlyList<IProcessWant> InputWants { get; }

        /// <summary>
        /// Capital Wants, used by the process.
        /// </summary>
        IReadOnlyList<IProcessWant> CapitalWants { get; }

        /// <summary>
        /// Output Wants, created by the process.
        /// </summary>
        IReadOnlyList<IProcessWant> OutputWants { get; }

        #endregion InCapOut

        /// <summary>
        /// The process Tags of the Process
        /// </summary>
        Dictionary<ProcessTag, Dictionary<string, object>?> ProcessTags { get; }

        /// <summary>
        /// The Skill the Process Uses.
        /// </summary>
        ISkill? Skill { get; }

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
        /// The tech required see/do this process.
        /// </summary>
        ITechnology? TechRequirement { get; }

        IProcessNode? Node { get; set; }

        /// <summary>
        /// Get the Name(VariantName) of the process in
        /// standard form.
        /// </summary>
        /// <returns></returns>
        string GetName();

        IReadOnlyList<IProcessProduct> GetProductsByName(string name);
        IReadOnlyList<IProcessProduct> GetProductsByName(string name, ProcessPartTag part);

        /// <summary>
        /// Gets the amount of a product part, taking chance into account.
        /// </summary>
        /// <param name="part">The part we are calculating.</param>
        /// <returns>the projected amount.</returns>
        decimal ProjectedPartAmount(IProcessProduct part);

        /// <summary>
        /// Get's the projected amount of product produced and/or consumed by the
        /// process
        /// </summary>
        /// <param name="product">The product in question.</param>
        /// <returns>The total amount, subtracts inputs from outputs.</returns>
        decimal ProjectedProductAmount(IProduct product);

        /// <summary>
        /// Gets the projected amount of product involved in a part of the process.
        /// </summary>
        /// <param name="product">The product in question.</param>
        /// <param name="part">Which part (input, capital, output) to return.</param>
        /// <returns>The total amount, inputs are negative.</returns>
        decimal ProjectedProductAmount(IProduct product, ProcessPartTag part);

        /// <summary>
        /// Do the process's work. Takes in inputs and capitals,
        /// outputs the product made/consumed, capital used, and wants made or consumed.
        /// </summary>
        /// <param name="iterations">How many iterations to try and complete.</param>
        /// <param name="progress">The progress being brought in (will round up iterations sought).</param>
        /// <param name="products">The products available to be consumed/used.</param>
        /// <param name="wants">The wants available to be used directly.</param>
        /// <returns>
        /// Successes is how many iterations were completed.
        /// Progress is how much overflow remains.
        /// productChange is the product that has been created/consumed.
        /// productUsed is the products that have been used as capital.
        /// wantsChange is the change in wants, cap wants are still consumed.
        /// </returns>
        (int successes, decimal progress,
            Dictionary<IProduct, decimal> productChange,
            Dictionary<IProduct, decimal> productUsed,
            Dictionary<IWant, decimal> wantsChange)
            DoProcess(decimal iterations, decimal progress,
                Dictionary<IProduct, decimal> products,
                Dictionary<IWant, decimal> wants);
    }
}
