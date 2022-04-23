using EconomicSim.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconomicSim.Objects.Firms;
using EconomicSim.Objects.Government;
using EconomicSim.Objects.Pops;
using EconomicSim.Objects.Products;
using EconomicSim.Objects.Territory;

namespace EconomicSim.Objects.Market
{
    /// <summary>
    /// Market Data Class
    /// </summary>
    internal class Market : IMarket
    {
        public List<ITerritory> _territories;
        public List<(IMarket, decimal)> _neighbors;
        public List<(IProduct, decimal)> _resources;
        public List<IPopGroup> _pops;
        public List<IFirm> _firms;

        public Market()
        {
            _territories = new List<ITerritory>();
            _neighbors = new List<(IMarket, decimal)>();
            _resources = new List<(IProduct, decimal)>();
            _pops = new List<IPopGroup>();
            _firms = new List<IFirm>();
        }

        /// <summary>
        /// ID of the market.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the market, usually the name the region or city
        /// it covers.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The firms that are operating in this market.
        /// </summary>
        public IReadOnlyList<IFirm> Firms => _firms;

        /// <summary>
        /// The Population of the market.
        /// </summary>
        public IReadOnlyList<IPopGroup> Pops => _pops;
        
        /// <summary>
        /// The governing body of the market.
        /// </summary>
        public IGovernor Governor { get; set; }

        /// <summary>
        /// The Territories the Market Contains.
        /// </summary>
        public IReadOnlyList<ITerritory> Territories => _territories;

        /// <summary>
        /// The Markets which are considered adjacent to this one
        /// and their distance measured in km. 
        /// (Default distance is measured from center to center of
        /// the respective territories).
        /// These Neighbors should be reachable by foot assuming no
        /// rivers.
        /// </summary>
        public IReadOnlyList<(IMarket, decimal)> Neighbors => _neighbors;

        /// <summary>
        /// The resources that are found loose in the market, unowned.
        /// </summary>
        public IReadOnlyList<(IProduct, decimal)> Resources => _resources;
    }
}
