using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EconomicCalculator.DTOs.Firms
{
    /// <summary>
    /// A productive economic organization in our system.
    /// </summary>
    public interface IFirm
    {
        #region ID

        /// <summary>
        /// The Unique Id of the Firm
        /// </summary>
        [JsonIgnore]
        int Id { get; }

        /// <summary>
        /// The name of the firm, should be unique.
        /// </summary>
        string Name { get; }

        // variant name?

        #endregion ID

        #region FirmStructure

        /// <summary>
        /// The rank of the firm.
        /// </summary>
        FirmRank FirmRank { get; }

        /// <summary>
        /// How the ownership of the firm is split or structured.
        /// </summary>
        OwnershipStructure OwnershipStructure { get; }

        /// <summary>
        /// How profits from the firm are distributed.
        /// </summary>
        ProfitStructure ProfitStructure { get; }

        // population groups attached.
        // populations connect to firms for sanity reasons 
        // (having to point to pops from here across multiple
        // territories when the pop groups most likely won't 
        // have nice names to point to is nightmarish)

        // board of directors / owner leaders

        // HQ Territory

        #endregion FirmStructure

        #region Production

        /// <summary>
        /// What jobs are part of this firm.
        /// <seealso cref="FirmRank.Firm"/> allows only 1 primary job.
        /// <seealso cref="FirmRank.Company"/> or higher allows for multiple jobs.
        /// </summary>
        IList<string> Jobs { get; }

        /// <summary>
        /// What wage type each job is under.
        /// </summary>
        IDictionary<string, WageType> JobWageType { get; }

        /// <summary>
        /// The wage of each job.
        /// </summary>
        IDictionary<string, decimal> JobWages { get; }

        /// <summary>
        /// What processes (stand alone or not) the Firm utilizes.
        /// </summary>
        IList<string> Processes { get; }

        #endregion Production

        #region Market

        /// <summary>
        /// The products the firm sells to the market.
        /// </summary>
        IList<string> Products { get; }

        /// <summary>
        /// The prices the firm sells it's product at.
        /// </summary>
        IDictionary<string, decimal> ProductPrices { get; }

        /// <summary>
        /// What resources the company has at it's disposal.
        /// Key is the product's full (unique) name.
        /// Value is how much it has stored across all of it's properties.
        /// </summary>
        IDictionary<string, decimal> Resources { get; }

        #endregion Market
    }
}
