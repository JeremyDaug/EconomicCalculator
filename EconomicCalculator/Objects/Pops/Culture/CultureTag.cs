namespace EconomicCalculator.Objects.Pops.Culture
{
    /// <summary>
    /// Modifier Tags for Cultures.
    /// </summary>
    public enum CultureTag
    {
        /// <summary>
        /// BioPreference<Species, W>, This culture attracts a particular
        /// species. Those of that species have double the chance to migrate
        /// to this culture. Species is the one it depends on.
        /// W is the bonus attraction it gets (can be negative).
        /// </summary>
        BioPreference,
        /// <summary>
        /// JobPreference<Job, W>, this culture likes or dislikes
        /// a particular job, increasing or decreasing the chance
        /// that they migrate to such a job. 
        /// W is the modifier added to the chances. 
        /// </summary>
        JobPreference
    }
}