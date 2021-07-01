namespace EconModels.TerritoryModel
{
    public enum PlanetTopography
    {
        /// <summary>
        /// a roughly x by 2x-1 grid with pole caps.
        /// </summary>
        Sphere,
        /// <summary>
        /// An x by y ring with no caps. No Excess needed.
        /// </summary>
        Ring,
        /// <summary>
        /// a circle with radius x, excess added in circle from center.
        /// </summary>
        Flat,
        /// <summary>
        /// an x by y ring which connects north-south as well as east-west
        /// </summary>
        Torus,
        /// <summary>
        /// Undefined shape, form defined by user, no generation.
        /// </summary>
        None
    }
}