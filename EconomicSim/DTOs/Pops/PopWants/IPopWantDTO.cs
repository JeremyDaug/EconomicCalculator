namespace EconomicSim.DTOs.Pops.PopWants
{
    /// <summary>
    /// The Holder for pop wants.
    /// </summary>
    public interface IPopWantDTO
    {
        /// <summary>
        /// What want is desired.
        /// </summary>
        string Want { get; }

        /// <summary>
        /// The amount of the want desired.
        /// Negative values mean an aversion to this want.
        /// </summary>
        decimal Amount { get; }

        /// <summary>
        /// The Modifiers that effect having the want or not.
        /// </summary>
        IList<string> ModifierStrings { get; }
    }
}
