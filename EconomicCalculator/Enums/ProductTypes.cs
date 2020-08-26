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
        Consumable, // Storable, but consumed upon use.
        Good, // Storable, durable, but can break with use or time.
        Service, // Non-storable, and consumed immediately.
        Land, // Indefinitely storable, and unconsumable.
    }
}
