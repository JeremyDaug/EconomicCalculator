using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Enums
{
    /// <summary>
    /// The Kinds of products
    /// </summary>
    public enum ProductTypes
    {
        Consumable, // Storable, but consumed upon use. IE, food Chemical products, etc.
        Food, // Consumable Subtype, uses lots of land and some labor, but has minimal inputs.
        Good, // Storable, durable, but can break with use or time.
        CapitalGood, // A subtype of Good which is used to produce other goods and nothing else.
        Service, // Non-storable, and consumed immediately. Also known as labor.
        Land, // Not Storable (you can't move land) and unconsumable/Indestructable
        Building, // Subtype of Land, Not storable, but can store other things. A type of capital good.

        /*              | Consumed   | Not Consumed
         * -------------+------------+--------------
         * Storable     | Consumable | Land
         * -------------+------------+--------------
         * Non-Storable | Service    | Good
         */
    }
}
