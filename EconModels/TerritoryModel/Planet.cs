using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconModels.TerritoryModel
{
    /// <summary>
    /// The organizer for planet class collections.
    /// Every territory within this should be reachable by either
    /// land or sea.
    /// </summary>
    public class Planet
    {
        public Planet()
        {
            Untapped = new List<PlanetResources>();
            Regions = new List<Region>();
            Territories = new List<Territory>();
        }

        /// <summary>
        /// The Id of the Planet
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The Name of the planet
        /// </summary>
        [Required, StringLength(30)]
        public string Name { get; set; }

        /// <summary>
        /// Marks a planet as being effectively fixed and
        /// dead. Actions will not occur barring events.
        /// This will save cycles.
        /// </summary>
        [Required, DefaultValue(true)]
        public bool Dead { get; set; }

        /// <summary>
        /// The Available Planet resources that can be found and
        /// tapped. When a territory creates a pile,
        /// </summary>
        public virtual ICollection<PlanetResources> Untapped { get; set; }

        /// <summary>
        /// The mass of the planet in kg. Only Updated when needed.
        /// </summary>
        [Required, Range(0, double.MaxValue)]
        public double Mass { get; set; }

        /// <summary>
        /// The area of the planet's surface in whole km^2.
        /// </summary>
        [Required, Range(0, double.MaxValue)]
        public decimal SurfaceArea { get; set; }

        /// <summary>
        /// The average air pressure of the planet. Mostly used
        /// for habitability.
        /// </summary>
        [Required, Range(0, double.MaxValue)]
        public decimal AirPressure { get; set; }

        /// <summary>
        /// The average tempurature of the planet.
        /// </summary>
        [Required]
        public decimal Tempurature { get; set; }

        /// <summary>
        /// The regions of the planet, disorganized.
        /// </summary>
        public virtual ICollection<Region> Regions { get; set; }

        /// <summary>
        /// The region at the head of the region tree.
        /// </summary>
        [Required]
        public int HeadRegionId { get; set; }

        /// <summary>
        /// The region at the head of the region tree.
        /// </summary>
        public virtual Region HeadRegion { get; set; }

        /// <summary>
        /// The Territories of the Planet
        /// </summary>
        public virtual ICollection<Territory> Territories { get; set; }

        // rows and columns are not needed in the DB, the territories
        // can be used to find them anyway.

        /// <summary>
        /// The North Pole Territory.
        /// </summary>
        public int? NorthPoleId { get; set; }

        /// <summary>
        /// The north pole Territory.
        /// </summary>
        public virtual Territory NorthPole { get; set; }

        /// <summary>
        /// The South Pole Id, only needed if there is no north pole.
        /// </summary>
        public int? SouthPoleId { get; set; }

        /// <summary>
        /// The south pole Territory.
        /// </summary>
        public virtual Territory SouthPole { get; set; }

        /// <summary>
        /// Generates the territory tiles from.
        /// </summary>
        public void GenerateTerritories()
        {

        }
    }
}
