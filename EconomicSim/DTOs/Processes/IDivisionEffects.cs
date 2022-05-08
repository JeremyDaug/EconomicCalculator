namespace EconomicSim.DTOs.Processes
{
    [Obsolete]
    public interface IDivisionEffects
    {
        /// <summary>
        /// The process this division effect is attached to.
        /// </summary>
        IProcessDTO Parent { get; }

        /// <summary>
        /// When applying division of labor to the process
        /// how far can the skill level be reduced.
        /// <seealso cref="SkillMinimum"/> minus <seealso cref="DivisionSkillReduction"/>
        /// should be equivalent to the simplest skill the process
        /// can be reduced to.
        /// </summary>
        int DivisionSkillReduction { get; }

        /// <summary>
        /// When applying division of labor, this is how much each level of
        /// skill downwards increases all requirements of the process.
        /// This application is exponential for each level downwards.
        /// <seealso cref="ProductionTags.ProductionTag.DivisionInput"/>
        /// </summary>
        decimal DivisionLevelMultiplier { get; }
    }
}
