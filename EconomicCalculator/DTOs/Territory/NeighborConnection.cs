using EconomicCalculator.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.DTOs.Territory
{
    public class NeighborConnection : INeighboringConnection
    {
        public string Territory { get; set; }

        public decimal Distance { get; set; }

        public TerritoryConnectionType connectionType { get; set; }
    }
}
