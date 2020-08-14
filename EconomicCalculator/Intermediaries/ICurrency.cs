using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Intermediaries
{
    public interface ICurrency
    {
        /// <summary>
        /// The name of the currency.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The Variant name for the currency.
        /// </summary>
        string VariantName { get; }

        /// <summary>
        /// The monetary symbol, or shorthand for the currency. (IE $, GP, SP, JPY, USD etc.)
        /// </summary>
        string Symbol { get; }

        /// <summary>
        /// The physical currency
        /// </summary>
        IProduct Cash { get; }

        /// <summary>
        /// The Backing, of the currency. If the same as Commodity, then there is no backing.
        /// If null, then the currency is a Fiat Currency.
        /// </summary>
        IProduct Backing { get; }

        /// <summary>
        /// The Value of the currency in Abstract currency units.
        /// </summary>
        double Value { get; }

        /// <summary>
        /// The construction cost of the currency.
        /// </summary>
        /// <returns>The cost to construct and usually the minimum value it will have.</returns>
        double ConstructionCost();
    }
}
