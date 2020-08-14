using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Enums
{
    public enum JobTypes
    {
        Crop, // Works on time tables and yearly cycles.
        Mine, // Works on raw labor input
        Craft, // Relies on Resource Inputs and Labor only makes in fixed increments.
        Shipping, // Relies on capiatal to start, buys goods and moves them elsewhere in days.
        Service // Raw Labor input, get's paid a wage, any extra 'items' not sold are lost immediately.
    }
}
