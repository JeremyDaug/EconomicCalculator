namespace EconomicSim.Objects.Products.ProductTags;

public enum StorageType
{
    /// <summary>
    /// Basic storage, nothing special, but counts as storage
    /// for most goods.
    /// </summary>
    Basic,
    /// <summary>
    /// Liquid Storage, holds liquids.
    /// </summary>
    Liquid,
    /// <summary>
    /// Gas Storage, for Gas Storage.
    /// </summary>
    Gas,
    /// <summary>
    /// Energy storage, stores energy over time.
    /// </summary>
    Energy,
    /// <summary>
    /// Cold storage, keeps things cold and reduces
    /// rot.
    /// </summary>
    Cold,
    /// <summary>
    /// Storage that is secure, keep them from being stolen
    /// by most thieves.
    /// </summary>
    Secure,
    /// <summary>
    /// Perfect magical storage, stores anything and loses nothing.
    /// </summary>
    Perfect
}