﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EconomicCalculator.DTOs.Skills
{
    /// <summary>
    /// Skill Group
    /// </summary>
    public interface ISkillGroupDTO
    {
        /// <summary>
        /// The Id of the skill Group
        /// </summary>
        [JsonIgnore]
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