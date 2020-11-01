using EconomicCalculator.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage.Products
{
    public interface ICurrency : IProduct
    {
        /// <summary>
        /// The type of currency it is.
        /// </summary>
        CashType CashType { get; }

        /// <summary>
        /// A value from 0 to 1 defining how trusted the currency is.
        /// The higher the trust the more people will want it and use it,
        /// the more valuable it becomes.
        /// The lower the trust, the less people will want it and the less
        /// valuable it is.
        /// </summary>
        double Trust { get; }

        /// <summary>
        /// The Symbol Shorthand for the currency.
        /// </summary>
        char Symbol { get; }

        /// <summary>
        /// What is backing the currency, the price of this currency is fixed
        /// to the price of that good.
        /// If Backing is itself, then it's a commodity currency (coinage).
        /// If backed by something else, then it's a token or banknote.
        /// If backed by nothing then it's a fiat currency.
        /// </summary>
        IProduct Backing { get; }

        /// <summary>
        /// The value of the currency when broken down.
        /// </summary>
        double RawValue();
    }
}
