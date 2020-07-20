using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Intermediaries
{
    /// <summary>
    /// Functions shared amonst various classes and items which are used for accounting arnd running.
    /// </summary>
    public interface IAccount
    {
        /// <summary>
        /// The total (Gross) Unit currency produced in one (averaged) cycle.
        /// </summary>
        double Revenue();

        /// <summary>
        /// The Total (Gross) unit currency produced in a (averaged) day.
        /// </summary>
        double DailyRevenue();

        /// <summary>
        /// The Costs in unit currency produced in one (averaged) cycle.
        /// </summary>
        double Costs();

        /// <summary>
        /// The Daily Costs in unit currency produced in one (averaged) day.
        /// </summary>
        double DailyCosts();

        /// <summary>
        /// The Net Unit Currency gained (or lost) in one (averaged) cycle.
        /// </summary>
        double Net();

        /// <summary>
        /// The Net Unit Currency gained (or lost) in one (averaged) day.
        /// </summary>
        double DailyNet();
    }
}
