namespace EconomicSim.Helpers
{
    /// <summary>
    /// The level that a desire is for a pop.
    /// </summary>
    public enum DesireTier
    {
        /// <summary>
        /// A debug Tier, used as a 'non' tier.
        /// </summary>
        NonTier = -1001,
        /// <summary>
        /// Desires needed by a pop to survive, they will often give anything
        /// to get these, and if they don't get them they begin starving quickly.
        /// </summary>
        LifeTier = -1000,
        /// <summary>
        /// The value at which the productive tier of goods begins.
        /// Used for self-owned or subsistence firms.
        /// </summary>
        ProductiveTierStart = -999,
        /// <summary>
        /// The level at which Productive desires ends.
        /// Used for self-owned or subsistence firms.
        /// </summary>
        ProductiveTierEnd = -1,
        /// <summary>
        /// The level for items that make life bearable start.
        /// Will be selected over savings.
        /// </summary>
        DailyTierStart = 0,
        /// <summary>
        /// The Last tier for daily items.
        /// </summary>
        DailyTierEnd = 999,
        /// <summary>
        /// The nice things in life that are not necessary.
        /// Might be given up for savings.
        /// </summary>
        LuxuryTierStart = 1000
    }
}
