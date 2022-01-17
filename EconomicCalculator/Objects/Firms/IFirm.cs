using EconomicCalculator.Helpers;
using EconomicCalculator.Objects.Jobs;
using EconomicCalculator.Objects.Market;
using EconomicCalculator.Objects.Products;
using EconomicCalculator.Objects.Technology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Objects.Firms
{
    /// <summary>
    /// Read Only Firm Interface
    /// </summary>
    public interface IFirm
    {
        /// <summary>
        /// Id of the firm
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Name of the firm.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The rank of the firm.
        /// </summary>
        FirmRank FirmRank { get; }

        /// <summary>
        /// How ownership of the firm is structured.
        /// </summary>
        OwnershipStructure OwnershipStructure { get; }

        /// <summary>
        /// How Profits from the firm are distributed.
        /// </summary>
        ProfitStructure ProfitStructure { get; }

        /// <summary>
        /// The firms this firm owns.
        /// </summary>
        IReadOnlyList<IFirm> Children { get; }

        /// <summary>
        /// The Firm that owns this firm.
        /// </summary>
        IFirm Parent { get; }

        /// <summary>
        /// The Jobs the Firm oversees, how it pays them, 
        /// and at what rate it pays.
        /// </summary>
        IReadOnlyList<(IJob, WageType, decimal)> Jobs { get; }
        // TODO, bring pops into here.

        /// <summary>
        /// The products that this firm tries to sell.
        /// </summary>
        IReadOnlyList<(IProduct, decimal)> Products { get; }

        /// <summary>
        /// What resources the Firm owns. Bought goods go here,
        /// made goods go here and are sold from here.
        /// </summary>
        IReadOnlyList<(IProduct, decimal)> Resources { get; }

        /// <summary>
        /// The market which the Firm is centered out of.
        /// </summary>
        IMarket HeadQuarters { get; }

        /// <summary>
        /// The regions where the company operates, buying, selling,
        /// and working. Must have at least one piece of property in
        /// this market to do so.
        /// </summary>
        IReadOnlyList<IMarket> Regions { get; }

        /// <summary>
        /// The techs available to the Firm.
        /// </summary>
        IReadOnlyList<ITechnology> Techs { get; }

        // Research stuff here.
    }
}
