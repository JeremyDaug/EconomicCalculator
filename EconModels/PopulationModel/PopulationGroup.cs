using EconModels.JobModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconModels.PopulationModel
{
    public class PopulationGroup
    {
        public PopulationGroup()
        {
            CultureBreakdown = new List<PopulationCultureBreakdown>();
        }

        public int Id { get; set; }

        // May remove this
        [StringLength(30, MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        public decimal Count { get; set; }

        // Growth Rate depends on culture, defaulting to 2%/year

        [Required, StringLength(30, MinimumLength = 3)]
        public string SkillName { get; set; }

        [Required, Range(0, int.MaxValue)]
        public int SkillLevel { get; set; }

        // job labor? hold off on this, may turn it into base process.

        [Required]
        public int PrimaryJobId { get; set; }
        public virtual Job PrimaryJob { get; set; }

        [Required, Range(0, int.MaxValue)]
        public int Priority { get; set; }

        [Required]
        public virtual ICollection<PopulationCultureBreakdown> CultureBreakdown { get; set; }

        // Population Group Property, may go over storage limits later,
        public virtual ICollection<OwnedProperty>  OwnedProperties { get; set; }
    }
}
