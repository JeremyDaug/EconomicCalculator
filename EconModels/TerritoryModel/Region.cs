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
        /// The parent to the region. IF null, then this
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
        /// </summary>
        public virtual ICollection<Region> Children { get; set; }

        /// <summary>
        /// The planet this region is attached to.
        /// </summary>
        [Required]
        public int PlanetId { get; set; }

        /// <summary>
        /// The Planet this region is attached to.
        /// </summary>
        public virtual Planet Planet { get; set; }

        /// <summary>
        /// The Id of the connected territory (if any).
        /// </summary>
        public int? TerritoryId { get; set; }

        /// <summary>
        /// The territory of this region, only applies if rank is 0.
        /// In it's non DB counterpart this connects to a virtual
        /// territory which sums all contained territories below.
        /// </summary>
        public virtual Territory Territory { get; set; }
    }
}