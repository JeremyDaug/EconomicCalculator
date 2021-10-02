﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage.Skills
{
    /// <summary>
    /// Skill Group
    /// </summary>
    public interface ISkillGroup
    {
        /// <summary>
        /// The Id of the skill Group
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The name of the skill group
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The Default transfer rate between any two Skills
        /// within this skill group.
        /// </summary>
        decimal Default { get; }

        /// <summary>
        /// The Description of the Skill Group.
        /// </summary>
        string Description { get; }
    }
}