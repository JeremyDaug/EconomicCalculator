using EconModels.MarketModel;
using EconModels.PopulationModel;
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
    public class Territory
    {
        public Territory()
        {
            OutgoingConnections = new List<TerritoryConnection>();
            IncomingConnections = new List<TerritoryConnection>();
            LandOwners = new List<LandOwner>();
            LocalResources = new List<LocalResource>();
            PublicGoods = new List<PublicGood>();
        }

        public int Id { get; set; }

        /// <summary>
        /// The name of the territory
        /// </summary>
        [Required, StringLength(40)]
        public string Name { get; set; }

        /// <summary>
        /// The Id of the territory's planet.
        /// </summary>
        [Required]
        public int PlanetId { get; set; }

        /// <summary>
        /// The Planet this territory is attached to.
        /// </summary>
        public virtual Planet Planet { get; set; }

        // Coordinates for within an organized group
        public int? X { get; set; }
        public int? Y { get; set; }
        public int? Z { get; set; }

        /// <summary>
        /// What is the maximum size of the Territory, assuming perfect
        /// development, measured in acres.
        /// </summary>
        [Required]
        public decimal Extent { get; set; }

        // availble land is not stored, instead it is calculated based on
        // the extent * (1-waterCaverage) - Land Owned.

        /// <summary>
        /// The elevation of the territory in meters relative to the planet.
        /// </summary>
        [Required]
        public int Elevation { get; set; }

        /// <summary>
        /// The percent of the land covered in water.
        /// Includes space for rivers, which if they exist take up
        /// a minimum of 5%.
        /// </summary>
        [Required, Range(0, 1)]
        public float WaterCoverage { get; set; }

        /// <summary>
        /// The amount of water stored in the territory at all times.
        /// Does not include flowing water, only stationary.
        /// </summary>
        [Required, Range(0, double.MaxValue)]
        public decimal WaterQuantity { get; set; }

        /// <summary>
        /// Whether the territory has a river flowing through it or
        /// not.
        /// </summary>
        [Required, DefaultValue(false)]
        public bool HasRiver { get; set; }

        /// <summary>
        /// The average humidity of the territory, in the form of average cm of rainfall in a year.
        /// </summary>
        [Required]
        public int Humidity { get; set; }

        /// <summary>
        /// The average tempurature(C) of the territory over a year. Variance is dependant on
        /// proximity to water and average humidity.
        /// </summary>
        [Required]
        public int Tempurature { get; set; }

        /// <summary>
        /// The difficulty of the terrain, the more rough, the less passable to traffic. 
        /// </summary>
        [Required]
        public int Roughness { get; set; }

        // Plot is not stored as it's a composite of existing info here
        // and the generic land product.

        /// <summary>
        /// This is the public infrastructure of the location. Includes everything
        /// from roads to electrical wires. Anything that is public and expansive
        /// but difficult to impossible to protect actively.
        /// </summary>
        public virtual ICollection<PublicGood> PublicGoods { get; set; }

        /// <summary>
        /// The available local resources in the territory. Anything beyond this
        /// uses the common environmental breakdown to gather and extract resources.
        /// </summary>
        public virtual ICollection<LocalResource> LocalResources { get; set; }

        // Infrastructure is removed, it is based on the public goods in the territory and
        // is not a singular value.

        /// <summary>
        /// The level of exploitation reached in the territory.
        /// </summary>
        [Required, Range(0, int.MaxValue)]
        public int ExploitationLevel { get; set; }

        // Adjacent Territories not covered by planetary grid connection.
        /// <summary>
        /// Territories that this territory connects to indirectly.
        /// </summary>
        public virtual ICollection<TerritoryConnection> OutgoingConnections { get; set; }

        /// <summary>
        /// Territories that are connected to this territory inderectly.
        /// </summary>
        public virtual ICollection<TerritoryConnection> IncomingConnections { get; set; }

        public int MarketId { get; set; }

        public virtual Market Market { get; set; }

        // No population Connection as pops live in markets, not territories.

        // Governor Placeholder

        /// <summary>
        /// The Table of Land Owners, pointing to the populations who own them and
        /// what exactly they own.
        /// </summary>
        public virtual ICollection<LandOwner> LandOwners { get; set; }

        /// <summary>
        /// The <see cref="Region"/> node which owns this.
        /// </summary>
        [Required]
        public int RegionId { get; set; }

        /// <summary>
        /// The <see cref="Region"/> node which owns this.
        /// </summary>
        public virtual Region Region { get; set; }

        // Claimants handled by governor not territory.
    }
}
