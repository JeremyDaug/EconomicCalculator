using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Objects.Territory.Coords
{
    public interface ICoordinate
    {
        int X { get; set; }

        int Y { get; set; }

        int z { get; set; }
    }
}
