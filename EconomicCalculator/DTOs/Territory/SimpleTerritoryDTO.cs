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
            Neighbors = new List<(string neighbor, decimal distance, TerritoryConnectionType conType)>();
            Nodes = new List<(string resource, decimal stockpile, int depth)>();
            Resources = new List<(string resource, decimal amount)>();
        }

        public HexCoord Coords { get; set; }

        public string Name { get; set; }

        public bool Coastal { get; set; }

        public bool Lake { get; set; }

        public ulong Size { get; set; }

        public ulong Land { get; set; }

        public ulong Water { get; set; }

        public List<ulong> Plots { get; set; }

        public List<(string neighbor, decimal distance, TerritoryConnectionType conType)> Neighbors { get; set; }

        public List<(string resource, decimal stockpile, int depth)> Nodes { get; set; }

        public List<(string resource, decimal amount)> Resources { get; set; }
    }
}
