using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.DTOs.Firms
{
    /// <summary>
    /// Profit Structures define how profits are distributed among
    /// possible recievers.
    /// </summary>
    public enum ProfitStructure
    {
        /// <summary>
        /// Profits are distributed among all equally.
        /// Everyone in the business get's 1 share.
        /// </summary>
        Distributed,
        /// <summary>
        /// Profits are destributed among all shareholders.
        /// Shares may be bought and sold on the market.
        /// </summary>
        Shares,
        /// <summary>
        /// Profits are destributed to the owners of the
        /// business.
        /// </summary>
        PrivatelyOwned,
        /// <summary>
        /// Profits are split between the shareholders/owners and
        /// the workers.
        /// </summary>
        ProfitSharing,
        /// <summary>
        /// Profits are not destributed, instead profits go to
        /// offset costs and reduce prices.
        /// </summary>
        NonProfit
    }
}
