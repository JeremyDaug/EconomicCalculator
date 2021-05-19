using EconModels.ProductModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EconModels.TerritoryModel
{
    /// <summary>
    /// An untapped resource of a planet.
    /// </summary>
    public class PlanetResources
    {
        /// <summary>
        /// The Id of the planet it belongs to.
        /// </summary>
        [Required]
        public int PlanetId { get; set; }

        /// <summary>
        /// The planet it belongs to.
        /// </summary>
        [ForeignKey("PlanetId")]
        public virtual Planet Planet { get; set; }

        /// <summary>
        /// The Id of the resource.
        /// </summary>
        [Required]
        public int ResourceId { get; set; }

        /// <summary>
        /// The Resource
        /// </summary>
        [ForeignKey("ResourceId")]
        public virtual Product Resource { get; set; }

        /// <summary>
        /// The amount of the resource in the planet.
        /// </summary>
        [Required, Range(0, int.MaxValue)]
        public decimal Amount { get; set; }
    }
}