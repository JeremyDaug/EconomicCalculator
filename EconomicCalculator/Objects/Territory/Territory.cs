﻿using EconomicCalculator.DTOs.Hexmap;
using EconomicCalculator.Enums;
using EconomicCalculator.Objects.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Objects.Territory
{
    /// <summary>
    /// Super basic Territory Data Class. For environmental data available in a market.
    /// No special traits or connections. Everything is explicit. No map generation basis.
    /// </summary>
    internal class Territory : ITerritory
    {
        private ulong land;
        public List<long> plots;
        public List<(ITerritory neighbor, decimal distance, TerritoryConnectionType type)> neighbors;
        public List<(IProduct resource, decimal stockpile, int depth)> nodes;
        public List<(IProduct resource, decimal amount)> resources;

        public Territory()
        {
            plots = new List<long>();
            neighbors = new List<(ITerritory neighbor, decimal distance, TerritoryConnectionType type)>();
            nodes = new List<(IProduct resource, decimal stockpile, int depth)>();
            resources = new List<(IProduct resource, decimal amount)>();
        }

        /// <summary>
        /// The Location of the territory in the hex grid which contains them.
        /// </summary>
        public HexCoord Coords { get; }

        /// <summary>
        /// The name of the territory (may be empty)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Whether the territory is coastal or not.
        /// </summary>
        public bool Coastal { get; set; }

        /// <summary>
        /// Whether the Territory contains water.
        /// </summary>
        public bool Lake { get; set; }

        /// <summary>
        /// The size of the Territory in Acres.
        /// </summary>
        public ulong Size { get; set; }

        /// <summary>
        /// The actual land available in the territory in acres.
        /// </summary>
        public ulong Land
        {
            get
            {
                return land;
            }
            set
            {
                land = value;
            }
        }

        /// <summary>
        /// The area that is taken up by water in acres.
        /// </summary>
        public ulong Water
        {
            get
            {
                return Size - land;
            }
            set
            {
                land = Size - value;
            }
        }

        public IReadOnlyList<long> Plots { get => plots; }

        public IReadOnlyList<(ITerritory neighbor, decimal distance, TerritoryConnectionType type)> Neighbors { get => neighbors; }

        public IReadOnlyList<(IProduct resource, decimal stockpile, int depth)> Nodes { get => nodes; }

        public IReadOnlyList<(IProduct resource, decimal amount)> Resources { get => resources; }
    }
}
