using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconModels.PopulationModel
{
    public class Species
    {
        public Species()
        {
            Anathemas = new List<SpeciesAnathema>();
            Aversions = new List<SpeciesAversion>();
            LifeNeeds = new List<SpeciesNeed>();
            LifeWants = new List<SpeciesWant>();
            Tags = new List<SpeciesTag>();
        }

        public int Id { get; set; }

        /// <summary>
        /// The name of the Species.
        /// </summary>
        [Required, DisplayName("Species Name")]
        [StringLength(30)]
        public string Name { get; set; }

        /// <summary>
        /// The variant name of the species.
        /// </summary>
        [DisplayName("Variant Name")]
        [StringLength(30)]
        public string VariantName { get; set; }

        /// <summary>
        /// The Natural Growth Rate of the Species each day.
        /// </summary>
        [Required, Range(-1, 1)]
        [DisplayName("Growth Rate")]
        public decimal SpeciesGrowthRate { get; set; }

        /// <summary>
        /// The Tempurature preference of the species, In K
        /// </summary>
        [Required, Range(0, 1000)]
        [DisplayName("Tempurature Preference")]
        public double TempuraturePreference { get; set; }

        /// <summary>
        /// The Gravity Preferences in g (9.8m/s^2 or 1 earth g).
        /// </summary>
        [Required, Range(0, 50)]
        [DisplayName("Gravity Preference")]
        public double GravityPreference { get; set; }

        /// <summary>
        /// How long the species stays as babies in days.
        /// </summary>
        [Required, Range(0, double.MaxValue)]
        [DisplayName("Infant Phase")]
        public double InfantPhaseLength { get; set; }

        /// <summary>
        /// How long the species stays as children in days.
        /// </summary>
        [Required, Range(0, double.MaxValue)]
        [DisplayName("Child Phase")]
        public double ChildPhaseLength { get; set; }

        /// <summary>
        /// How long they stay as adults in days.
        /// </summary>
        [Required, Range(0, double.MaxValue)]
        [DisplayName("Adult Phase")]
        public double AdultPhaseLength { get; set; }

        /// <summary>
        /// How long the average unaided lifespan of the species is in days.
        /// </summary>
        [Required, Range(0, double.MaxValue)]
        [DisplayName("Lifespan")]
        public double AverageLifeSpan { get; set; }

        /// <summary>
        /// What want tags they are penalized by, requiring extra products
        /// to avoid any problems created.
        /// </summary>
        [DisplayName("Aversions")]
        public virtual ICollection<SpeciesAversion> Aversions { get; set; }

        /// <summary>
        /// What specific prdoucts they are repelled by, requiring extra
        /// products to avoid these problems.
        /// </summary>
        [DisplayName("Anathemas")]
        public virtual ICollection<SpeciesAnathema> Anathemas { get; set; }

        /// <summary>
        /// The specific products this species needs to survive, this
        /// is stuff like water, or air.
        /// </summary>
        [DisplayName("Needs")]
        public virtual ICollection<SpeciesNeed> LifeNeeds { get; set; }

        /// <summary>
        /// The more generic desires of this species, this is not specific
        /// things, but more general things like food.
        /// </summary>
        [DisplayName("Wants")]
        public virtual ICollection<SpeciesWant> LifeWants { get; set; }

        /// <summary>
        /// Any Additional Tags the Species has. These modify the basic
        /// activities of the species and mark out specific acitons they
        /// do.
        /// </summary>
        [DisplayName("Tags")]
        public ICollection<SpeciesTag> Tags { get; set; }
    }
}
