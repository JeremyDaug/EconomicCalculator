using EconModels.ProductModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EconModels.TerritoryModel
{
    public class PublicGood
    {
        /// <summary>
        /// Territory Id.
        /// </summary>
        [Required]
        public int TerritoryId { get; set; }

        /// <summary>
        /// The Owning Territory.
        /// </summary>
        [ForeignKey("TerritoryId")]
        public virtual Territory Territory { get; set; }

        /// <summary>
        /// The Public Good ID.
        /// </summary>
        [Required]
        public int GoodId { get; set; }

        /// <summary>
        /// The Good owned.
        /// </summary>
        [ForeignKey("GoodId")]
        public virtual Product Good { get; set; }

        /// <summary>
        /// The amount of the good in the territory currently.
        /// </summary>
        public decimal Amount { get; set; }
    }
}