namespace EconomicCalculator.Objects.Technology
{
    public enum TechCategory
    {
        /// <summary>
        /// This is a big tech and often more a discovery of theory,
        /// rather than a source of new processes immediately.
        /// </summary>
        Primary,
        /// <summary>
        /// Concreet ideas, which add new processes and products to
        /// the collective repretoir of tools.
        /// </summary>
        Secondary,
        /// <summary>
        /// Incremental techs, ideas which don't make new things, but
        /// improve existing processes or techs, allowing for new variant
        /// products, and so on.
        /// </summary>
        Tertiary
    }
}