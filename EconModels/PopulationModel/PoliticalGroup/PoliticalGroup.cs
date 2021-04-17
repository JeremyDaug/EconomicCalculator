using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconModels.PopulationModel
{
    /// <summary>
    /// Placeholder, TODO Later.
    /// </summary>
    public class PoliticalGroup
    {
        public PoliticalGroup()
        {
            Tags = new List<PoliticalTag>();
            
            Allies = new List<PoliticalGroup>();
            AlliesRev = new List<PoliticalGroup>();

            Enemies = new List<PoliticalGroup>();
            EnemiesRev = new List<PoliticalGroup>();
        }

        /// <summary>
        /// Id of the Political Group.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the Political Group.
        /// </summary>
        [Required, StringLength(30)]
        public string Name { get; set; }

        /// <summary>
        /// Variant name of the political group.
        /// </summary>
        [StringLength(30)]
        public string VariantName { get; set; }

        /// <summary>
        /// The level of radicalism inate to the political group.
        /// The higher the radicalisation the more likely they are
        /// to create rebellions, sepratist groups, terrorist cells
        /// and become activily millitant.
        /// This ranges from 0 to 5 for math purposes.
        /// </summary>
        [Required, Range(0, 5)]
        public double Radicalism { get; set; }

        /// <summary>
        /// This is a measure of how nationalistic and self
        /// interested the group is. Moderate levels creates patriots
        /// High levels create Nationalists or ethno-nationalists.
        /// Negative levels tend towards globalisation and the
        /// dismantling of national borders.
        /// </summary>
        [Required, Range(-1, 1)]
        public double Nationalism { get; set; }

        /// <summary>
        /// How much the political group wishes to centralize power
        /// into a single organization, or destribute it across
        /// the territory. Centralized tend towards dictatorial 
        /// and imperial/monarchal while decentralized tend toward
        /// democracy and Republics.
        /// </summary>
        [Required, Range(-1, 1)]
        public double Centralization { get; set; }

        /// <summary>
        /// How the society views authority and whether it comes
        /// from the people or the institutions. High Authority
        /// parties view institutions and specific groups as
        /// the arbitors of authority and legitimacy. Parties with
        /// negative authority believe no one has legitimate
        /// authority. Neutral tend towards the support of the
        /// people or elected officials being seen as most valid.
        /// </summary>
        [Required, Range(-1, 1)]
        public double Authority { get; set; }

        /// <summary>
        /// Whether the party desires Planned or Unplanned 
        /// Economics. Positive tends towards planning and 
        /// centralized control of the economy. Negative tends
        /// towards markets and similar unplanned economic systems.
        /// </summary>
        [Required, Range(-1, 1)]
        public double Planning { get; set; }

        /// <summary>
        /// How pro-military the Party is. High values means the 
        /// party is pro military and pro expansion. Low values
        /// mean they are pacifist. Around 0, the party is
        /// mostly defensive.
        /// Note: this is not a measure of how militant they are. It
        /// will modify their willingness to fight. Pacifists are
        /// less likely to become militant, Militarists more likely.
        /// </summary>
        [Required, Range(-1, 1)]
        public double Militarism { get; set; }

        /// <summary>
        /// What tags the Party Has
        /// </summary>
        public ICollection<PoliticalTag> Tags { get; set; }

        /// <summary>
        /// Allied Political Groups.
        /// </summary>
        public ICollection<PoliticalGroup> Allies { get; set; }

        public ICollection<PoliticalGroup> AlliesRev { get; set; }

        /// <summary>
        /// Aversary Political Groups.
        /// </summary>
        public ICollection<PoliticalGroup> Enemies { get; set; }

        public ICollection<PoliticalGroup> EnemiesRev { get; set; }

        public void AddAlly(PoliticalGroup other)
        {
            if (Allies.Contains(other))
                return;

            Allies.Add(other);
            AlliesRev.Add(other);

            other.AddAlly(this);
        }

        public void RemoveAlly(PoliticalGroup other)
        {
            if (!Allies.Contains(other))
                return;

            Allies.Remove(other);
            AlliesRev.Remove(other);

            other.RemoveAlly(this);
        }

        public void ClearAllies()
        {
            foreach(var ally in Allies)
            {
                RemoveAlly(ally);
            }
        }

        public void AddEnemy(PoliticalGroup other)
        {
            if (Enemies.Contains(other))
                return;

            Enemies.Add(other);
            EnemiesRev.Add(other);

            other.AddEnemy(other);
        }

        public void RemoveEnemy(PoliticalGroup other)
        {
            if (!Enemies.Contains(other))
                return;

            Enemies.Remove(other);
            EnemiesRev.Remove(other);

            other.RemoveEnemy(this);
        }

        public void ClearEnemies()
        {
            foreach (var enemy in Enemies)
            {
                RemoveEnemy(enemy);
            }
        }

        public PoliticalTag AddTag(string v)
        {
            // ensure tag isn't already there.
            if (Tags.Select(x => x.Tag).Any(x => x == v))
                return Tags.Single(x => x.Tag == v);

            // if it isn't create a new tag and add it.
            var tag = new PoliticalTag
            {
                GroupId = Id,
                Group = this,
                Tag = v
            };
            Tags.Add(tag);

            return tag;
        }
    }
}
