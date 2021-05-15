using EconModels.PopulationModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EconModels.TerritoryModel
{
    /// <summary>
    /// This is the table connecting territories to those who own the land.
    /// </summary>
    public class LandOwner
    {
        [Required]
        public int TerritoryId { get; set; }

        /// <summary>
        /// The Territory the land is in.
        /// </summary>
        [ForeignKey("TerritoryId")]
        public virtual Territory Territory { get; set; }

        [Required]
        public int OwnerId { get; set; }

        /// <summary>
        /// The Pop who owns the the land.
        /// </summary>
        [ForeignKey("OwnerId")]
        public virtual PopulationGroup Owner { get; set; }

        /// <summary>
        /// How much they own.
        /// </summary>
        [Required, Range(0, int.MaxValue)]
        public decimal Amount { get; set; }

        // What types of land they own?
        // This should be covered by the pop's
        // owned goods.
    }
}