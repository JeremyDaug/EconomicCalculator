using EconomicSim.Objects.Products;

namespace EconomicSim.Objects.Skills
{
    /// <summary>
    /// Skill Data Class
    /// </summary>
    public class Skill : ISkill
    {
        public Skill()
        {
            Relations = new List<(Skill, decimal)>();
            Groups = new List<SkillGroup>();
        }

        /// <summary>
        /// Skill Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The Name of the Skill.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the Skill.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The Groups this skill is in.
        /// </summary>
        public List<SkillGroup> Groups { get; set; }
        IReadOnlyList<ISkillGroup> ISkill.Groups => Groups;

        /// <summary>
        /// The other skills related to this skill.
        /// </summary>
        public List<(Skill relation, decimal rate)> Relations { get; set; }
        IReadOnlyList<(ISkill relation, decimal rate)> ISkill.Relations => Relations
            .Select(x => ((ISkill)x.relation, x.rate)).ToList();

        /// <summary>
        /// The Labor which represents application of the skill.
        /// </summary>
        public IProduct Labor { get; set; }
    }
}
