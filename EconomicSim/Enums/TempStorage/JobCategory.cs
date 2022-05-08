namespace EconomicSim.Enums
{
    /// <summary>
    /// The general types of job types.
    /// </summary>
    public enum JobCategory
    {
        /// <summary>
        /// Own and work a farm, owning land and tools to work the land.
        /// </summary>
        Farmer,
        /// <summary>
        /// Own and work a mine, owning the land and tools to work it.
        /// </summary>
        Miner,
        /// <summary>
        /// One who sells their labor to others for a wage.
        /// </summary>
        Laborer,
        /// <summary>
        /// One who is owned by another and works purely for their life needs.
        /// </summary>
        Slave,
        /// <summary>
        /// A lone worker who buys inputs and capital to produce output goods.
        /// </summary>
        Craftsman,
        /// <summary>
        ///  A lone laborer of higher skill who buys inputs and capital to produce high value labor.
        /// </summary>
        Practitioner,
        /// <summary>
        /// A retail owner who owns storage. They buy goods to resell at profit.
        /// </summary>
        Merchant,
        /// <summary>
        /// A merchant who buys goods to transport elsewhere and sell at a profit.
        /// </summary>
        TravellingMerchant,
        /// <summary>
        /// A soldier in the military who products the nation.
        /// </summary>
        Soldier,
        /// <summary>
        /// A leader of the military who improves their efficiency.
        /// </summary>
        Officer,
        /// <summary>
        /// A middle manager who improves efficiency in a company.
        /// </summary>
        Manager,
        /// <summary>
        /// An owner in a company who reaps profits after all other workers are paid.
        /// </summary>
        Owner,
        /// <summary>
        /// One who owns land, buildings, or houses and rents them out to others.
        /// </summary>
        LandLord,
        /// <summary>
        /// A banker who gives others money so he can reap some of the profits.
        /// </summary>
        Investor,
        /// <summary>
        /// A government worker who helps maintain order in the government.
        /// </summary>
        Bureaucrat,
        /// <summary>
        /// A religious figure who helps organize and teach the people.
        /// </summary>
        Clergy,
        /// <summary>
        /// A person who is paid to research science and discover greater things.
        /// </summary>
        Researcher,
        /// <summary>
        /// One who uses science to create and improve products and processes.
        /// </summary>
        Engineer
    }
}
