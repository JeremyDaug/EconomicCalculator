using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Migrations;
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
        /// The seed for the terrain in generation.
        /// </summary>
        [Required]
        public int Seed { get; set; }

        /// <summary>
        /// The planet type, lots of varieties here.
        /// </summary>
        [Required, DefaultValue(PlanetType.Terrestrial)]
        public PlanetType Type { get; set; }

        /// <summary>
        /// The Shape of the planet.
        /// </summary>
        [Required, DefaultValue(PlanetTopography.None)]
        public PlanetTopography Shape { get; set; }

        /// <summary>
        /// Marks a planet as being effectively fixed and
        /// dead. Actions will not occur barring events.
        /// This will save cycles.
        /// </summary>
        [Required, DefaultValue(true)]
        public bool Dead { get; set; }

        /// <summary>
        /// The Available Planet resources that can be found and
        /// tapped. When a territory creates a pile. When generated, untapped resources
        /// should add up to the mass of the planet (allow an error of 0.0000001% for sanity)
        /// Note, The mass of the planet is a double, while this is a list of decimals.
        /// Doubles have a lot more room, but any object above 10^32 is a black hole and
        /// has no breakdown.
        /// </summary>
        public virtual ICollection<PlanetResources> Untapped { get; set; }

        /// <summary>
        /// The mass of the planet in kg. Only Updated when needed, IE, when mass leaves the
        /// planet.
        /// </summary>
        [Required, Range(0, double.MaxValue)]
        public double Mass { get; set; }

        /// <summary>
        /// A catch to ensure no mass removed from the planet is lost. Removed mass is
        /// first moved to here, then, if this will change the Actual mass, then it is removed
        /// from that mass.
        /// </summary>
        [Required, Range(0, double.MaxValue), DefaultValue(0)]
        public double LossSafe { get; set; }

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
        /// The number of rows in the territory grid.
        /// </summary>
        public int Rows { get; set; }

        /// <summary>
        /// The number of colums in the grid.
        /// </summary>
        public int Columns { get; set; }

        /// <summary>
        /// The regions of the planet, disorganized.
        /// </summary>
        public virtual ICollection<Region> Regions { get; set; }

        /// <summary>
        /// The region at the head of the region tree.
        /// Not required so single territory planets can exist.
        /// </summary>
        [NotMapped]
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

        // Helper Functions below.

        private Territory TerrGen(int? x, int? y, int? z, string name)
        {
            return new Territory
            {
                Name = name,
                X = x,
                Y = y,
                Z = z,
                Elevation = 0,
                Extent = 25 * 250,
                Humidity = 10,
                Planet = this,
                PlanetId = Id,
                Roughness = 2,
                Tempurature = 60,
                WaterQuantity = 0,
                WaterCoverage = 0.00F,
                ExploitationLevel = 0,
                HasRiver = false,
            };
        }

        /// <summary>
        /// Adds a territory to the Planet
        /// x and y are bound to between 0 and column(x)/Rows(y)
        /// If location is taken, it returns false.
        /// </summary>
        /// <param name="x"/>
        /// <param name="y"/>
        /// <param name="terr">The territory to add.</param>
        /// <param name="context">The context of the planet.</param>
        public bool AddTerritoryToLocation(int x, int y, Territory terr, EconSimContext context)
        {
            // if the location is already taken, return false.
            if (Territories.Any(t => t.X == x && t.Y == y && t.Z == HexZ(x, y)))
                return false;

            // it's not taken, so add it.
            terr.PlanetId = Id;
            Territories.Add(terr);

            context.Territories.AddOrUpdate(terr);
            context.SaveChanges();

            return true;
        }

        private int HexZ(int x, int y)
        {
            return -x - y;
        }

        /// <summary>
        /// Generates the territory tiles for the planet.
        /// </summary>
        public void GeneratePlanetSphere()
        {
            // set rows and columns these need to be 0 no matter what.
            Rows = 0;
            Columns = 0;
            Shape = PlanetTopography.Sphere;
            // Hexes are 250km^2 (30km in radius)
            // For earth size planet that is 20,402,578
            var hexCount = Math.Floor(SurfaceArea / 250);

            if (hexCount <= 1) // only one hex, just make north pole.
            {
                // Create North Pole
                var NP = TerrGen(null, null, null, "North Pole");
                // update it's extent in acres.
                NP.Extent = SurfaceArea * 250;
                // Add to territories.
                Territories.Add(NP);
                // set NP
                NorthPole = NP;
            }
            else if (hexCount <= 8) // the smallest grid is 2x3
            {
                // Do north
                var NP = TerrGen(null, null, null, "North Pole");
                NP.Extent = Math.Floor(SurfaceArea) / 2 * 250;
                Territories.Add(NP);
                NorthPole = NP;
                // Do south
                var SP = TerrGen(null, null, null, "South Pole");
                SP.Extent = Math.Floor(SurfaceArea) / 2 * 250;
                Territories.Add(SP);
                SouthPole = SP;
            }
            else // enough for the smallest grid.
            {
                // get the height and width of our map.
                var height = Math.Sqrt((double)SurfaceArea / 2);
                Rows = (int)Math.Floor(height);
                Columns = (int)Math.Floor(2 * height);
                // We do not add any territories unless needed.
            }
        }
    }
}
