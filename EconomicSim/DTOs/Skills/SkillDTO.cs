using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EconomicSim.DTOs.Skills
{
    /// <summary>
    /// Skill interface.
    /// </summary>
    [Obsolete]
    public class SkillDTO : ISkillDTO
    {
        public SkillDTO()
        {
            Groups = new List<int>();
            GroupStrings = new List<string>();
            Related = new Dictionary<int, decimal>();
            RelatedStrings = new Dictionary<string, decimal>();
        }

        /// <summary>
        /// The Expected Pattern on a skill name.
        /// </summary>
        public static string NamePattern = @"^[a-zA-Z]+$";

        /// <summary>
        /// Ensures that a given name is valid.
        /// Skills can only have letters in it.
        /// </summary>
        /// <param name="skill"></param>
        /// <returns></returns>
        public static bool NameIsValid(string skill)
        {
            Regex rg = new Regex(NamePattern);

            return rg.IsMatch(skill);
        }

        /// <summary>
        /// Skill Id
        /// </summary>
        [JsonIgnore]
        public int Id { get; set; }

        /// <summary>
        /// Skill's Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The groups the skill belongs to.
        /// </summary>
        public List<string> GroupStrings { get; set; }

        /// <summary>
        /// The Groups the skill Belongs to.
        /// </summary>
        [JsonIgnore]
        public List<int> Groups { get; set; }

        /// <summary>
        /// The Groups it belongs to in string form.
        /// </summary>
        [JsonIgnore]
        public string GroupString
        {
            get
            {
                var val = "";

                foreach (var group in GroupStrings)
                    val += group + ";";

                val = val.TrimEnd(';');

                return val;
            }
        }

        /// <summary>
        /// Skills that this skill is related to.
        /// The key is the Id of the skill
        /// The value is the transfer rate between this skill
        /// and the other.
        /// </summary>
        [JsonIgnore]
        public Dictionary<int, decimal> Related { get; set; }

        /// <summary>
        /// Skill Relations as strings
        /// </summary>
        public Dictionary<string, decimal> RelatedStrings { get; set; }

        /// <summary>
        /// The Description of the skill.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The Related Items in singular string from.
        /// </summary>
        [JsonIgnore]
        public string RelatedString
        {
            get
            {
                var val = "";
                foreach (var rel in RelatedStrings)
                    val += rel.Key + "<" + rel.Value.ToString() + ">;";

                val = val.TrimEnd(';');

                return val;
            }
        }

        /// <summary>
        /// Sets the detailed data of this skill from 
        /// String data.
        /// </summary>
        public void SetDataFromStrings()
        {
            // process each group name into it's id.
            foreach (var group in GroupStrings)
            {
                Groups.Add(DTOManager.Instance.GetSkillGroupByName(group).Id);
            }

            // Process each related string into id relations.
            foreach (var related in RelatedStrings)
            {
                int id = DTOManager.Instance.GetSkillByName(related.Key).Id;

                Related[id] = related.Value;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
