namespace EconomicCalculator.Objects.Pops.Culture
{
    /// <summary>
    /// Modifier Tags for Cultures.
    /// </summary>
    public enum CultureTags
    {
        /// <summary>
        /// BioPreference<Species, W>, This culture attracts a particular
        /// species. Those of that species have double the chance to migrate
        /// to this culture. Species is the one it depends on.
        /// W is the bonus attraction it gets (can be negative).
        /// </summary>
        BioPreference,
        /// <summary>
        /// BioDependant<Species>, this culture is dependant
        /// on some aspect of a biology, and is thus dependant
        /// on a  certain species. Other species cannot assimilate
        /// to it. (Culture Whitelist)
        /// </summary>
        BioDependance,
        /// <summary>
        /// BioExclusion<Species>, this culture is incompatable 
        /// with a species. The selected species has no chance
        /// of assimilating to this culture, and even if it does
        /// it will immediately assimilate to a different
        /// culture.
        /// </summary>
        BioExclusion,
        /// <summary>
        /// JobPreference<Job, W>, this culture likes or dislikes
        /// a particular job, increasing or decreasing the chance
        /// that they migrate to such a job. 
        /// W is the modifier added to the chances. 
        /// </summary>
        JobPreference
    }
}