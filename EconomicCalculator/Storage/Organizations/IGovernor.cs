using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage
{
    /// <summary>
    /// The Governer is the control point of a government, nation, or similar entity.
    /// </summary>
    public interface IGovernor
    {
        /// <summary>
        /// The name of Government.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The Unique Id of the Governor.
        /// </summary>
        Guid Id { get; }

        #region GovernmentTerritory

        /// <summary>
        /// The Governor's Internal Markets.
        /// </summary>
        IDictionary<Guid, IMarket> Markets { get; }

        /// <summary>
        /// The Territory Directly Owned by the Governor.
        /// </summary>
        IDictionary<Guid, ITerritory> OwnedLand { get; }

        /// <summary>
        /// The Territory Claimed by the Governor.
        /// </summary>
        IDictionary<Guid, ITerritory> ClaimedLand { get; }

        /// <summary>
        /// The level of each Claim by the Governor.
        /// </summary>
        IDictionary<Guid, int> ClaimLevel { get; }

        #endregion GovernmentTerritory

        #region Taxes

        /// <summary>
        /// A minor placeholder, used to tax land and/or land value.
        /// </summary>
        double LandTax { get; }

        /// <summary>
        /// Taxes on specific products in the Governance.
        /// </summary>
        double SalesTax { get; }

        /// <summary>
        /// Placeholder for income taxes, should be more complicated and able to be made
        /// progressive/flat/whatever.
        /// </summary>
        double IncomeTax { get; }

        #endregion Taxes

        #region BalanceSheet

        /// <summary>
        /// The resources (in products, money, land, etc), available to the governor.
        /// </summary>
        IDictionary<Guid, double> GovernmentResources { get; }

        /// <summary>
        /// Placeholder for technologies the government has available to it.
        /// </summary>
        IList<string> Technologies { get; }

        #endregion BalanceSheet
    }
}
