using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicSim.Objects.Firms
{
    public enum ProfitStructure
    {
        /// <summary>
        /// Profits are split equally among all members of all jobs in 
        /// the firm.
        /// </summary>
        Distributed,
        /// <summary>
        /// Profits are distributed to shareholders as Dividends.
        /// These shares may be bought or sold.
        /// </summary>
        Shares,
        /// <summary>
        /// Profits are given to the owner directly. One share is made
        /// and it is given to the owner. If he sells it, he also
        /// sells the ownership of the business.
        /// If the State owns the business entirely, this is the option they
        /// use.
        /// </summary>
        PrivatelyOwned,
        /// <summary>
        /// Profits are partially shared out to shareholders
        /// and partially given out to the workers of the business.
        /// The split is 50/50. May upgrade it to more flexible system.
        /// </summary>
        ProfitSharing,
        /// <summary>
        /// The firm does not take profits, instead they are reinvested
        /// into either growing the business or reducing prices.
        /// </summary>
        NonProfit
    }
}
