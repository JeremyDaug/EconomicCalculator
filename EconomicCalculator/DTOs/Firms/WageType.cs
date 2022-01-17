using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.DTOs.Firms
{
    /// <summary>
    /// How a job in a firm is paid and treated.
    /// </summary>
    public enum WageType
    {
        /// <summary>
        /// Workers are paid based on their productivity,
        /// driving them to work harder.
        /// </summary>
        Productivity,
        /// <summary>
        /// Workers are paid based on daily participation,
        /// giving them more security and allowing them to
        /// work slower, but only a bit.
        /// </summary>
        Daily,
        /// <summary>
        /// Improved Daily, they are paid flatly, like a daily
        /// wage, but they can effectively stop working and
        /// still get paid, granting insurance.
        /// </summary>
        Salary,
        /// <summary>
        /// Gig Economy style position, pays like Daily, but
        /// when not needed they will be fired immediately 
        /// rather than sidlined or furloughed
        /// </summary>
        Contractor,
        /// <summary>
        /// Typically used for owners or shareholders,
        /// it gives a relatively small wage, but in
        /// return they get a cut of the company's 
        /// profits.
        /// </summary>
        ProfitSharing,
        /// <summary>
        /// Like <see cref="ProfitSharing"/>, but should
        /// the company have losses, they will also pay
        /// those losses out of pocket. Often used
        /// for private, or disorganized firms.
        /// </summary>
        LossSharing
    }
}
