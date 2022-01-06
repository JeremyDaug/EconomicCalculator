using EconomicCalculator.Helpers;
using EconomicCalculator.Objects.Jobs;
using EconomicCalculator.Objects.Market;
using EconomicCalculator.Objects.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Objects.Firms
{
    /// <summary>
    /// Firm Data Class
    /// </summary>
    internal class Firm : IFirm
    {
        public List<IMarket> _regions;
        public List<Pair<IProduct, decimal>> _resources;
        public List<Pair<IProduct, decimal>> _products;
        public List<Pair<IJob, WageType, decimal>> _jobs;
        public List<IFirm> _children;

        public Firm()
        {
            _regions = new List<IMarket>();
            _resources = new List<Pair<IProduct, decimal>>();
            _products = new List<Pair<IProduct, decimal>>();
            _jobs = new List<Pair<IJob, WageType, decimal>>();
            _children = new List<IFirm>();
        }

        /// <summary>
        /// Id of the firm
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the firm.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The rank of the firm.
        /// </summary>
        public FirmRank FirmRank { get; set; }

        /// <summary>
        /// How ownership of the firm is structured.
        /// </summary>
        public OwnershipStructure OwnershipStructure { get; set; }

        /// <summary>
        /// How Profits from the firm are distributed.
        /// </summary>
        public ProfitStructure ProfitStructure { get; set; }

        /// <summary>
        /// The firms this firm owns.
        /// </summary>
        public IReadOnlyList<IFirm> Children { get => _children; }

        /// <summary>
        /// The Firm that owns this firm.
        /// </summary>
        public IFirm Parent { get; set; }

        /// <summary>
        /// The Jobs the Firm oversees, how it pays them, 
        /// and at what rate it pays.
        /// </summary>
        public IReadOnlyList<Pair<IJob, WageType, decimal>> Jobs { get => _jobs; }
        // TODO, bring pops into here.

        /// <summary>
        /// The products that this firm tries to sell.
        /// </summary>
        public IReadOnlyList<Pair<IProduct, decimal>> Products { get => _products; }

        /// <summary>
        /// What resources the Firm owns. Bought goods go here,
        /// made goods go here and are sold from here.
        /// </summary>
        public IReadOnlyList<Pair<IProduct, decimal>> Resources { get => _resources; }

        /// <summary>
        /// The market which the Firm is centered out of.
        /// </summary>
        public IMarket HeadQuarters { get; set; }

        /// <summary>
        /// The regions where the company operates, buying, selling,
        /// and working. Must have at least one piece of property in
        /// this market to do so.
        /// </summary>
        public IReadOnlyList<IMarket> Regions { get => _regions; }
    }
}
