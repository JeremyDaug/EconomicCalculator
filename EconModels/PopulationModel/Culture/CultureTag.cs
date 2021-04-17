using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EconModels.PopulationModel
{
    public class CultureTag
    {
        /// <summary>
        /// The Culture this tag is attached to.
        /// </summary>
        [Required]
        public int CultureId { get; set; }

        /// <summary>
        /// The Culture Attached to.
        /// </summary>
        [ForeignKey("CultureId")]
        public virtual Culture Culture { get; set; }

        /// <summary>
        /// The tag of the culture with data inside.
        /// </summary>
        [Required, StringLength(30)]
        [DisplayName("Tag")]
        public string Tag { get; set; }
    }
}