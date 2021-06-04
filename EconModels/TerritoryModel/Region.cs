using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EconModels.TerritoryModel
{
    /// <summary>
    /// Region Groups of Territories.
    /// </summary>
    public class Region
    {
        /// <summary>
        /// The Id of the Region
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the Region
        /// </summary>
        [Required, StringLength(30)]
        public string Name { get; set; }

        /// <summary>
        /// The rank (height) of the region in the
        /// organizational tree.
        /// </summary>
        [Required, Range(0, int.MaxValue)]
        public int Rank { get; set; }

        /// <summary>
        /// The parent to the region. If null, then this
        /// is the entire planet.
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// The parent of the Region, if null then this region is
        /// the planet.
        /// </summary>
        [ForeignKey("ParentId")]
        public virtual Region Parent { get; set; }

        /// <summary>
        /// This region's Children. 
        /// If empty, then there should be territories.
        /// </summary>
        public virtual ICollection<Region> Children { get; set; }

        /// <summary>
        /// The territories in the region.
        /// If this is empty, <see cref="Children"/> should not be empty.
        /// </summary>
        public virtual ICollection<Territory> Territories { get; set; }

        /// <summary>
        /// The planet this region is attached to.
        /// </summary>
        [Required]
        public int PlanetId { get; set; }

        /// <summary>
        /// The Planet this region is attached to.
        /// </summary>
        public virtual Planet Planet { get; set; }
    }
}