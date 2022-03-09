using EconomicCalculator.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.DTOs.Territory
{
    public interface INeighboringConnection
    {
        string Territory { get; }

        decimal Distance { get; }

        TerritoryConnectionType connectionType { get; }
    }
}
