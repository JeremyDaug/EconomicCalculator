using System.ComponentModel.DataAnnotations;

namespace EconModels.TerritoryModel
{
    public class RegionHexCoord
    {
        public RegionHexCoord(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
            Z = -X - Y;
        }

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