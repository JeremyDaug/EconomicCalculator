using EconomicCalculator.Helpers;
using EconomicCalculator.Objects.Firms;
using EconomicCalculator.Objects.Government;
using EconomicCalculator.Objects.Pops;
using EconomicCalculator.Objects.Products;
using EconomicCalculator.Objects.Territory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Objects.Market
{
    /// <summary>
    /// Market Data Class
    /// </summary>
    internal class Market : IMarket
    {
        public List<IInstitution> _institutions;
        public List<ITerritory> _territories;
        public List<IMarket> _neighbors;
        public List<Pair<IProduct, decimal>> _resources;
        public List<IPopGroup> _pops;
        public List<IFirm> _firms;

        public Market()
        {
            _institutions = new List<IInstitution>();
            _territories = new List<ITerritory>();
            _neighbors = new List<IMarket>();
            _resources = new List<Pair<IProduct, decimal>>();
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
        /// The institutions which have sway over this region.
        /// </summary>
        public IReadOnlyList<IInstitution> Institutions => _institutions;

        /// <summary>
        /// The Territories the Market Contains.
        /// </summary>
        public IReadOnlyList<ITerritory> Territories => _territories;

        /// <summary>
        /// The Markets which are considered adjacent to this one.
        /// </summary>
        public IReadOnlyList<IMarket> Neighbors => _neighbors;

        /// <summary>
        /// The resources that are loose in the market, unowned.
        /// </summary>
        public IReadOnlyList<Pair<IProduct, decimal>> Resources => _resources;
    }
}
