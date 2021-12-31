using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconDTOs.DTOs.Pops.PopModifiers
{
    public enum PopModifiers
    {
        /// <summary>
        /// The rate that population increases.
        /// Growth rate cannot go below 0.
        /// </summary>
        GrowthRate,
        /// <summary>
        /// The chance that a population dies.
        /// Cannot push rate below 0.
        /// </summary>
        DeathRate,
        /// <summary>
        /// The desire of a pop to leave for somewhere else.
        /// Immigration rate cannot be pushed below 0.
        /// </summary>
        EmmmigrationRate,
        /// <summary>
        /// The bonus rate a pop gains skill.
        /// Cannot push education below 0.
        /// </summary>
        EducationRate,
        /// <summary>
        /// Modifies the rate that a skill is lost to time.
        /// Retention cannot go above 100% or below 0%.
        /// </summary>
        SkillRetensionRate
    }
}
