using EconomicCalculator.DTOs.Hexmap;
using EconomicCalculator.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.DTOs.Territory
{
    public class SimpleTerritoryDTO : ISimpleTerritoryDTO
    {
        public SimpleTerritoryDTO()
        {
            Plots = new List<ulong>();
            Neighbors = new List<INeighboringConnection>();
            Nodes = new List<IResourceNode>();
            Resources = new List<ITerritoryResource>();
        }

        /// <summary>
        /// The coordinates of our simple territory.
        /// For Simple territories, this should be unique.
        /// </summary>
        public HexCoord Coords { get; set; }

        /// <summary>
        /// Name of the Territory, may be null.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Whether the Territory is costal or not.
        /// </summary>
        public bool Coastal { get; set; }

        /// <summary>
        /// Whether the Territory has a static body of water.
        /// </summary>
        public bool Lake { get; set; }

        /// <summary>
        /// The total size in acres, including water.
        /// </summary>
        public ulong Size { get; set; }

        /// <summary>
        /// The land available in acres.
        /// </summary>
        public ulong Land { get; set; }

        /// <summary>
        /// The surface area claimed by water in the territory.
        /// </summary>
        public ulong Water { get; set; }

        /// <summary>
        /// The number of plots of each kind. 
        /// Should match up with available land options.
        /// Must equal Land * 8.
        /// </summary>
        public List<ulong> Plots { get; set; }

        /// <summary>
        /// The Id of the territory
        /// </summary>
        public List<INeighboringConnection> Neighbors { get; set; }

        /// <summary>
        /// The Nodes within the territory and the depth at which it can be mined.
        /// -1 means inifinite size.
        /// </summary>
        public List<IResourceNode> Nodes { get; set; }

        /// <summary>
        /// The Resources available on the surface of the territory
        /// Amount of -1 means infinite.
        /// </summary>
        public List<ITerritoryResource> Resources { get; set; }
    }
}
