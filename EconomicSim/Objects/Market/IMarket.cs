using EconomicSim.Helpers;
using EconomicSim.Objects.Government;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconomicSim.Objects.Firms;
using EconomicSim.Objects.Pops;
using EconomicSim.Objects.Products;
using EconomicSim.Objects.Territory;

namespace EconomicSim.Objects.Market
{
    /// <summary>
    /// Market Read Only Interface
    /// </summary>
    public interface IMarket
    {
        /// <summary>
        /// ID of the market.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The name of the market, usually the name the region or city
        /// it covers.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The firms that are operating in this market.
        /// </summary>
        IReadOnlyList<IFirm> Firms { get; }

        /// <summary>
        /// The Population of the market.
        /// </summary>
        IReadOnlyList<IPopGroup> Pops { get; }

        /// <summary>
        /// The governing body of the market.
        /// </summary>
        //IGovernor Governor { get; }

        /// <summary>
        /// The Territories the Market Contains.
        /// </summary>
        IReadOnlyList<ITerritory> Territories { get; }

        /// <summary>
        /// The Markets which are considered adjacent to this one
        /// and their distance measured in km. 
        /// (Default distance is measured from center to center of
        /// the respective territories).
        /// These Neighbors should be reachable by foot assuming no
        /// rivers.
        /// </summary>
        IReadOnlyList<(IMarket, decimal)> Neighbors { get; }

        /// <summary>
        /// The resources that are loose in the market, unowned.
        /// </summary>
        IReadOnlyList<(IProduct, decimal)> Resources { get; }
    }
}
