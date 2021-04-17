using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconModels.PopulationModel
{
    public class PoliticalTag
    {
        /// <summary>
        /// The Political Group Id.
        /// </summary>
        [Required]
        public int GroupId { get; set; }

        /// <summary>
        /// The Political Group attached to.
        /// </summary>
        [Required, ForeignKey("GroupId")]
        public virtual PoliticalGroup Group { get; set; }

        /// <summary>
        /// The Tag attached to the Political Group.
        /// </summary>
        [Required, StringLength(30)]
        public string Tag { get; set; }
    }
}
