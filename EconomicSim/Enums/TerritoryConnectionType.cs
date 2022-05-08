namespace EconomicSim.Enums
{
    /// <summary>
    /// What kind of connection exists between two
    /// non-trivially connected territories.
    /// </summary>
    public enum TerritoryConnectionType
    {
        /// <summary>
        /// It is connected by land. Anything which can
        /// walk may travel along it.
        /// </summary>
        Land,
        /// <summary>
        /// The connection is water and requires
        /// a naval ship to pass through.
        /// </summary>
        Sea,
        /// <summary>
        /// The connection is in the air, requiring
        /// flight to pass through.
        /// </summary>
        Air,
        /// <summary>
        /// The connection is in a vacuum and it requires
        /// a space fairing ship to pass through.
        /// </summary>
        Space,
        /// <summary>
        /// The connection is by a tunnel which passes
        /// through the intervening space. 
        /// </summary>
        Tunnel
    }
}
