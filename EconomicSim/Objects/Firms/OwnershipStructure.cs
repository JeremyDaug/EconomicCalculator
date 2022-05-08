namespace EconomicSim.Objects.Firms
{
    /// <summary>
    /// The kinds of ownership that is available to firms.
    /// </summary>
    public enum OwnershipStructure
    {
        /// <summary>
        /// The firm is not 'owned' strictly, as it's a
        /// disorganized grouping of individuals doing the same job
        /// not a strict company of people.
        /// </summary>
        SelfEmployed,
        /// <summary>
        /// A loose association of business owners/operators
        /// who come together to lay down ground rules for competition.
        /// </summary>
        Association,
        /// <summary>
        /// A semi-official organization of small businesses who
        /// agree to many rules and regulations and often quash
        /// competition.
        /// </summary>
        Guild,
        /// <summary>
        /// A private company, not bound by any other contracts beyond
        /// what trade deals they've made.
        /// </summary>
        Private,
        /// <summary>
        /// A cooperative company, owned and operated by the workers
        /// collectively.
        /// </summary>
        Cooperative,
        /// <summary>
        /// The company is publicly traded, with stocks available to buy
        /// on the market.
        /// </summary>
        PubliclyTraded,
        /// <summary>
        /// The company is owned and controlled by the state who can direct
        /// it at it's whim.
        /// </summary>
        StateOwned
    }
}
