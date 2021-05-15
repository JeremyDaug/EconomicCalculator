using EconModels.PopulationModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
            Pops = new List<PopulationGroup>();
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

        /// <summary>
        /// The elevation of the territory in meters relative to the planet.
        /// </summary>
        [Required]
        public int Elevation { get; set; }

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

        /// <summary>
        /// The amount of water that is stored in the province, in cubic m.
        /// This is how much will be retained before flowing into adjacent
        /// territories. Special storage, as it's GOING to be used.
        /// </summary>
        [Required, Range(0, int.MaxValue)]
        public decimal WaterStorage { get; set; }

        /// <summary>
        /// The space that this natural water storage takes up in acre.
        /// This space is reserved by the land.
        /// </summary>
        [Required, Range(0, int.MaxValue)]
        public decimal WaterStorageSpace { get; set; }

        /// <summary>
        /// The amount of water flowing into the territory from neighbors.
        /// Measured in m^3/day.
        /// </summary>
        [Range(0, int.MaxValue)]
        public decimal WaterInFlow { get; set; }

        /// <summary>
        /// The amount of water flowing into the territory from neighbors.
        /// </summary>
        [Range(0, int.MaxValue)]
        public decimal WaterOutFlow { get; set; }

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

        // Adjacent Territories
        /// <summary>
        /// Territories that this territory connects to indirectly.
        /// </summary>
        public virtual ICollection<TerritoryConnection> OutgoingConnections { get; set; }

        /// <summary>
        /// Territories that are connected to this territory inderectly.
        /// </summary>
        public virtual ICollection<TerritoryConnection> IncomingConnections { get; set; }

        // Market Connection Placeholder.
        /// <summary>
        /// The population groups in the territory. If integrated, moves to integrated
        /// territory.
        /// </summary>
        public virtual ICollection<PopulationGroup> Pops { get; set; }

        // Governor Placeholder

        // Planetary parent placeholder.

        /// <summary>
        /// The Available land, unclaimed by landowners. Includes both land and
        /// water territories.
        /// </summary>
        [Required, Range(0, int.MaxValue)]
        public decimal AvailableLand { get; set; }

        /// <summary>
        /// The Table of Land Owners, pointing to the populations who own them and
        /// what exactly they own.
        /// </summary>
        public virtual ICollection<LandOwner> LandOwners { get; set; }

        // TODO parent integration/Consolidation structure.

        // Claimants handled by governor not territory.
    }
}
