using System.ComponentModel.DataAnnotations;

namespace EconModels.TerritoryModel
{
    public class RegionHexCoord
    {
        /// <summary>
        /// The region it belongs to.
        /// </summary>
        [Required]
        public int RegionId { get; set; }

        public virtual Region Region { get; set; }

        [Required]
        public int X { get; set; }

        [Required]
        public int Y { get; set; }

        [Required]
        public int Z { get; set; }
    }
}