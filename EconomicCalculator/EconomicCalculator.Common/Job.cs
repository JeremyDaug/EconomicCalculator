using System;

namespace EconomicCalculator.Common.Resource
{
    /// <summary>
    /// Job class
    /// </summary>
    public class Job
    {
        /// <summary>
        /// Name of the job
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Variant of the job
        /// </summary>
        public string Variant { get; set; }

        /// <summary>
        /// What kind of job it is.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// How much UC per day (hoped for).
        /// </summary>
        public double DailyWage { get; set; }
    }
}