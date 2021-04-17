using EconModels.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EconModels.PopulationModel
{
    public class CultureWant
    {
        /// <summary>
        /// The Culture this want is attached to.
        /// </summary>
        [Required]
        public int CultureId { get; set; }

        /// <summary>
        /// The Culture Attached to.
        /// </summary>
        [ForeignKey("CultureId")]
        public virtual Culture Culture { get; set; }

        /// <summary>
        /// The want tag desired.
        /// </summary>
        [Required, StringLength(20)]
        public string Want { get; set; }

        /// <summary>
        /// The amount of the want desired per day per person.
        /// </summary>
        [Required, Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        /// <summary>
        /// The Type of need, Life, Daily, Luxury.
        /// </summary>
        [Required]
        public NeedType NeedType { get; set; }
    }
}