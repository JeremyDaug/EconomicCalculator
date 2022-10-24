using System.Text.Json.Serialization;
using EconomicSim.Helpers;
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

        /// <summary>
        /// Adds another process to this process and outputs the resulting fusion.
        /// Outputs of one cancel the inputs of the other both ways.
        /// </summary>
        /// <param name="other">The process we are adding to this one</param>
        /// <returns>The resulting sum between the two.</returns>
        IProcess AddProcess(IProcess other);

        /// <summary>
        /// Feeds the other process into this process. Any outputs of other which are
        /// inputs for this cancel, otherwise inputs, capital, and outputs are added
        /// together.
        /// </summary>
        /// <param name="other">The process to bring in as an input to this one.</param>
        /// <returns>The combined process resulting from the combination.</returns>
        IProcess InputProcess(IProcess other);
        
        /// <summary>
        /// Feeds the other process into this process. Any outputs of other which are
        /// inputs for this cancel, otherwise inputs, capital, and outputs are added
        /// together.
        /// </summary>
        /// <param name="other">The process to bring in as an input to this one.</param>
        /// <param name="target">
        /// What product we are trying to satisfy.
        /// The other process will be multiplied to satisfy this.
        /// </param>
        /// <returns>The combined process resulting from the combination.</returns>
        IProcess InputProcess(IProcess other, IProduct target);
        
        /// <summary>
        /// Feeds the other process into this process. Any outputs of other which are
        /// inputs for this cancel, otherwise inputs, capital, and outputs are added
        /// together.
        /// </summary>
        /// <param name="other">The process to bring in as an input to this one.</param>
        /// <param name="target">
        /// What want we are trying to satisfy.
        /// The other process will be multiplied to satisfy this.
        /// </param>
        /// <returns>The combined process resulting from the combination.</returns>
        IProcess InputProcess(IProcess other, IWant target);

        /// <summary>
        /// Feeds this process's outputs into the <see cref="other"/> process as
        /// inputs. If this process's outputs are inputs to other, they cancel out,
        /// otherwise they add together.
        /// </summary>
        /// <param name="other">The process we are moving our outputs to.</param>
        /// <returns>The resulting combined process.</returns>
        IProcess OutputToProcess(IProcess other);
        
        /// <summary>
        /// Feeds this process's outputs into the <see cref="other"/> process as
        /// inputs. If this process's outputs are inputs to other, they cancel out,
        /// otherwise they add together.
        /// </summary>
        /// <param name="other">The process we are moving our outputs to.</param>
        /// <param name="target">
        /// What product we are trying to satisfy.
        /// The other process will be multiplied to satisfy this.
        /// </param>
        /// <returns>The resulting combined process.</returns>
        IProcess OutputToProcess(IProcess other, IProduct target);
        
        /// <summary>
        /// Feeds this process's outputs into the <see cref="other"/> process as
        /// inputs. If this process's outputs are inputs to other, they cancel out,
        /// otherwise they add together.
        /// </summary>
        /// <param name="other">The process we are moving our outputs to.</param>
        /// <param name="target">
        /// What want we are trying to satisfy.
        /// The other process will be multiplied to satisfy this.
        /// </param>
        /// <returns>The resulting combined process.</returns>
        IProcess OutputToProcess(IProcess other, IWant target);

        /// <summary>
        /// Feeds the <see cref="other"/> process into this process as a source
        /// for capital wants.
        /// </summary>
        /// <remarks>
        /// This does not remove capital good product requirements.
        /// </remarks>
        /// <param name="other">The process we are taking in for capital goods.</param>
        /// <returns></returns>
        IProcess TakeAsCapital(IProcess other);
        
        /// <summary>
        /// Feeds the <see cref="other"/> process into this process as a source
        /// for capital wants.
        /// </summary>
        /// <remarks>
        /// This does not remove capital good product requirements.
        /// </remarks>
        /// <param name="other">The process we are taking in for capital goods.</param>
        /// <param name="target">
        /// What want we are trying to satisfy.
        /// The other process will be multiplied to satisfy this.
        /// </param>
        /// <returns></returns>
        IProcess TakeAsCapital(IProcess other, IWant target);

        IReadOnlyList<IProcessProduct> GetProductsByName(string name);
        IReadOnlyList<IProcessProduct> GetProductsByName(string name, ProcessPartTag part);

        /// <summary>
        /// Gets the amount of a product part, taking chance into account.
        /// </summary>
        /// <param name="part">The part we are calculating.</param>
        /// <returns>the projected amount.</returns>
        decimal ProjectedPartAmount(IProcessProduct part);
        
        /// <summary>
        /// Gets the amount of a product part, taking chance into account.
        /// </summary>
        /// <param name="part">The part we are calculating.</param>
        /// <returns>the projected amount.</returns>
        decimal ProjectedPartAmount(IProcessWant part);

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
        /// Gets the projected amount of product involved in a part of the process.
        /// </summary>
        /// <param name="want">The want in question.</param>
        /// <param name="part">Which part (input, capital, output) to return.</param>
        /// <returns>The total amount, inputs are negative.</returns>
        decimal ProjectedWantAmount(IWant want, ProcessPartTag part);
        
        /// <summary>
        /// Do the process's work. Takes in inputs and capitals,
        /// outputs the product made/consumed, capital used, and wants made or consumed.
        /// Does not remove items from inputs, leaves that to the caller.
        /// </summary>
        /// <param name="iterations">How many iterations to try and complete, fractions allowed.</param>
        /// <param name="products">The products available to be consumed/used.</param>
        /// <param name="wants">The wants available to be used directly.</param>
        /// <returns>
        /// Successes is how many iterations were completed. Fractional values represent incomplete processes.
        /// productChange is the product that has been created/consumed.
        /// productUsed is the products that have been used as capital.
        /// wantsChange is the change in wants, cap wants are still consumed.
        /// </returns>
        (decimal successes,
            Dictionary<IProduct, decimal> productChange,
            Dictionary<IProduct, decimal> productUsed,
            Dictionary<IWant, decimal> wantsChange)
            DoProcess(decimal iterations,
                Dictionary<IProduct, decimal> products,
                Dictionary<IWant, decimal> wants);

        /// <summary>
        /// Calculates the bonus or penalty for a process given a
        /// particular skill level.
        /// </summary>
        /// <param name="skillLevel"></param>
        /// <returns></returns>
        decimal SkillThroughputModifier(decimal skillLevel);
    }
}
