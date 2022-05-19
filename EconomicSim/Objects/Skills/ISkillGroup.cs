using System.Text.Json.Serialization;

namespace EconomicSim.Objects.Skills
{
    /// <summary>
    /// Read Only Skill Group Interface
    /// </summary>
    [JsonConverter(typeof(SkillGroupJsonConverter))]
    public interface ISkillGroup
    {
        /// <summary>
        /// The Id of the Skill Group
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Skill Group's Name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Description of the Skill Group.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The Default Transfer rate of skill level between
        /// two skills within the skill group.
        /// </summary>
        decimal Default { get; }

        /// <summary>
        /// The Skills within this group.
        /// </summary>
        IReadOnlyList<ISkill> Skills { get; }
    }
}