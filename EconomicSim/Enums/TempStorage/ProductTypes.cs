using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicSim.Enums
{
    /// <summary>
    /// The Kinds of products and their traits.
    /// </summary>
    public enum ProductTypes
    {
        /// <summary>
        /// Storable, but requires no special storage, is consumed upon use.
        /// </summary>
        Consumable,
        /// <summary>
        /// Storable, but requires special storage (Refrigeration/Dry storage), consumed on use.
        /// </summary>
        ColdConsumable,
        /// <summary>
        /// Storable, no special storage, not consumed on use. Breaks down instead.
        /// </summary>
        Good, 
        /// <summary>
        /// Storable, special storage requirements, not consumed on use. Breaks down instead.
        /// </summary>
        ColdGood,
        /// <summary>
        /// Special Good Subtype, not consumed on use. Takes up CapitalStorage. Breaks down instead.
        /// </summary>
        CapitalGood,
        /// <summary>
        /// Not Storable, Consumed immediately.
        /// </summary>
        Service, 
        /// <summary>
        /// Has Storage, not consumed, not storable, not transferrable.
        /// </summary>
        Land,
        /// <summary>
        /// Has Storage, Not Consumed, Not Storable, Not Transferrable, Requires maintenance.
        /// </summary>
        Building, 
        /// <summary>
        /// Can be Stored, Not Consumed in most uses, No Maintenance Required.
        /// </summary>
        Currency

        /*              | Consumed   | Not Consumed
         * -------------+------------+--------------
         * Storable     | Consumable | Land
         * -------------+------------+--------------
         * Non-Storable | Service    | Good
         */
    }
}
