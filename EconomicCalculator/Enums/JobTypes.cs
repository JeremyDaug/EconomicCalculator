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
        Mine, // Works on raw labor input, arbitrary values of labor produce goods linearly scaled.
        Craft, // Relies on Resource Inputs and Labor only makes in fixed increments.
        Processing, // Works raw resources and labor into something else in arbitrary increments.
        Shipping, // Relies on capiatal to start, buys goods and moves them elsewhere in days.
        Service // Raw Labor input, get's paid a wage, any extra 'items' not sold are lost immediately.
    }
}
