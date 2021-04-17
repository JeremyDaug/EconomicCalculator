using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconModels.PopulationModel
{
    public class Culture
    {
        public Culture()
        {
            CultureNeeds = new List<CultureNeed>();
            CultureWants = new List<CultureWant>();
            Tags = new List<CultureTag>();
            RelationChild = new List<Culture>();
            RelationParent = new List<Culture>();
        }

        /// <summary>
        /// The Id of the culture
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// The culture's name.
        /// </summary>
        [Required, DisplayName("Culture Name")]
        [StringLength(30)]
        public string Name { get; set; }

        [DisplayName("Variant Name")]
        [StringLength(30)]
        public string VariantName { get; set; }

        /// <summary>
        /// The Base Growth Rate of the Population.
        /// </summary>
        [Required, Range(-1, 1)]
        public double CultureGrowthRate { get; set; }

        /// <summary>
        /// The Needs considered basic for the Culture.
        /// </summary>
        public virtual ICollection<CultureNeed> CultureNeeds { get; set; }

        /// <summary>
        /// The wants of the Culture, desires that are less specific, but still real.
        /// </summary>
        public virtual ICollection<CultureWant> CultureWants { get; set; }

        /// <summary>
        /// Tags attached to the Culture such as whether they are biological,
        /// Encourage progreation, lean towards certain regions, religions, or
        /// politics, and so on.
        /// </summary>
        public virtual ICollection<CultureTag> Tags { get; set; }

        /// <summary>
        /// The culture to culture relations, transitioning between related cultures
        /// is easier.
        /// </summary>
        public virtual ICollection<Culture> RelationChild { get; set; }
        public virtual ICollection<Culture> RelationParent { get; set; }

        public void AddRelatedCulture(Culture other)
        {
            if (RelationChild.Contains(other)) return;

            RelationChild.Add(other);
            RelationParent.Add(other);

            other.AddRelatedCulture(this);
        }

        public void RemoveCultureRelation(Culture culture)
        {
            if (!RelationChild.Contains(this))
                return;

            RelationChild.Remove(culture);
            RelationChild.Remove(culture);

            culture.RemoveCultureRelation(this);
        }

        public void ClearCultureRelations()
        {
            foreach (var culture in RelationChild)
            {
                RemoveCultureRelation(culture);
            }
        }
    }
}
