namespace EconomicSim.Enums
{
    /// <summary>
    /// The topology of the map.
    /// </summary>
    public enum PlanetTopography
    {
        /// <summary>
        /// a roughly x by 2x-1 grid with pole caps. Wraps horizontally.
        /// </summary>
        Sphere,
        /// <summary>
        /// An x by y ring with no caps. No Excess needed. Wraps horizontally.
        /// </summary>
        Ring,
        /// <summary>
        /// a circle with radius x, excess added in circle from center. No Wrapping.
        /// </summary>
        Flat,
        /// <summary>
        /// an x by y ring which connects north-south as well as east-west Wraps both vertically and horizontally.
        /// </summary>
        Torus,
        /// <summary>
        /// Undefined shape, form defined by user, no generation.
        /// </summary>
        None
    }
}
