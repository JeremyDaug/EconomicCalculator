using System.Text.Json.Serialization;
using EconomicSim.Objects.Market;
using EconomicSim.Objects.Products;
using EconomicSim.Objects.Technology;
using EconomicSim.Objects.Wants;

namespace EconomicSim.Objects.Firms
{
    /// <summary>
    /// Firm Data Class
    /// </summary>
    [JsonConverter(typeof(FirmJsonConverter))]
    public class Firm : IFirm
    {
        public Firm()
        {
            _remnantWants = new Dictionary<IWant, decimal>();
            Regions = new List<Market.Market>();
            Resources = new Dictionary<IProduct, decimal>();
            Products = new Dictionary<Product, decimal>();
            Jobs = new List<FirmJob>();
            Children = new List<Firm>();
            Techs = new List<(Technology.Technology tech, int research)>();
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
        public List<Firm> Children { get; set; }
        IReadOnlyList<IFirm> IFirm.Children => Children;

        /// <summary>
        /// The Firm that owns this firm.
        /// </summary>
        public Firm? Parent { get; set; }
        IFirm? IFirm.Parent => Parent;

        /// <summary>
        /// The Jobs the Firm oversees, how it pays them, 
        /// and at what rate it pays.
        /// </summary>
        public List<FirmJob> Jobs { get; set; }
        IReadOnlyList<IFirmJob> IFirm.Jobs => Jobs;

        /// <summary>
        /// The products that this firm tries to sell.
        /// </summary>
        public Dictionary<Product, decimal> Products { get; set; }
        IReadOnlyDictionary<IProduct, decimal> IFirm.Products 
            => Products.ToDictionary(x => (IProduct)x.Key,
                x => x.Value);

        /// <summary>
        /// What resources the Firm owns. Bought goods go here,
        /// made goods go here and are sold from here.
        /// </summary>
        public Dictionary<IProduct, decimal> Resources { get; set; }
        IReadOnlyDictionary<IProduct, decimal> IFirm.Resources 
            => Resources.ToDictionary(x => (IProduct)x.Key,
                x => x.Value); 

        /// <summary>
        /// The market which the Firm is centered out of.
        /// </summary>
        public Market.Market HeadQuarters { get; set; }
        IMarket IFirm.HeadQuarters => HeadQuarters;

        /// <summary>
        /// The regions where the company operates, buying, selling,
        /// and working. Must have at least one piece of property in
        /// this market to do so.
        /// </summary>
        public List<Market.Market> Regions { get; set; }
        IReadOnlyList<IMarket> IFirm.Regions => Regions;

        /// <summary>
        /// The techs available to the Firm.
        /// </summary>
        public List<(Technology.Technology tech, int research)> Techs { get; set; }
        IReadOnlyList<(ITechnology tech, int research)> IFirm.Techs => Techs
            .Select(x => ((ITechnology)x.tech, x.research)).ToList();

        #region AssistantData

        /// <summary>
        /// Wants left over by processes. Cleared at the end of each day.
        /// </summary>
        private readonly Dictionary<IWant, decimal> _remnantWants;

        /// <summary>
        /// The products used as capital and unable to be used elsewhere again today.
        /// drawn from for maintenance phase. 
        /// </summary>
        private readonly Dictionary<IProduct, decimal> _expendedProducts = new();

        #endregion

        public async Task ProductionPhase()
        {
            var dc = DataContext.Instance;
            // 'purchase' the labor from pops
            foreach (var job in Jobs)
            {
                // get the total time desired to buy and attempt to get it. 
                var targetTime = job.ConsumedTime();
                var timeReceived = new Dictionary<IProduct, decimal>();
                
                if (OwnershipStructure == OwnershipStructure.SelfEmployed)
                { // if self-employed, the wage type doesn't actually matter.
                    if (dc.DebugMode && Jobs.Count != 1) // TODO replace this with preprocessor if?
                    { // sanity check in debug that the self-employed firm has only one job.
                        if (Jobs.Count != 1)
                            throw new InvalidDataException("Self-Employed Firms can only have 1 job.");
                        if (FirmRank != FirmRank.Firm)
                            throw new InvalidDataException("Self-Employed Firms can only be of rank 'Firm'.");
                    }
                    while (job.Pop.Property.Any())
                    { // move all property from the pop to the firm.
                        var pair = job.Pop.Property.First();
                        if (Resources.ContainsKey(pair.Key))
                            Resources[pair.Key] += pair.Value;
                        else
                            Resources[pair.Key] = pair.Value;
                        job.Pop.Property.Remove(pair.Key);
                    } // Wants do not get stored between days, so do not need to be moved.
                }
                else if (job.WageType == WageType.LossSharing)
                { // loss sharing transfers any losses from the firm to the pop, so give the firm everything.
                    while (job.Pop.Property.Any())
                    { // move all property from the pop to the firm.
                        var pair = job.Pop.Property.First();
                        if (Resources.ContainsKey(pair.Key))
                            Resources[pair.Key] += pair.Value;
                        else
                            Resources[pair.Key] = pair.Value;
                        job.Pop.Property.Remove(pair.Key);
                    } // Wants do not get stored between days, so do not need to be moved.
                }
                else if (job.WageType == WageType.ProfitSharing ||
                         job.WageType == WageType.Slave ||
                         job.WageType == WageType.Productivity)
                { 
                    // Profit sharing get paid out of profits after everything
                    // is done
                    // Productivity is paid later based on total production.
                    // Slaves are not paid at all, merely given stuff as though they were
                    // expenditures.
                    // just get the time from them first, ask nicely (for now)
                    Resources[dc.Time] = job.Pop.GetRequestedProduct(dc.Time, targetTime)[dc.Time];
                }
                else if (job.WageType == WageType.Salary)
                { // Salary workers are paid the same amount regardless of time bought.
                    // The pay is flat regardless of time received.
                }
                else
                {// Daily, Contract have their time purchased.
                    // buy time from them by the unit.
                }

                // get the processes in the most useful order
                var assignmentOrder = job.AssignmentsInOrder();
                
                // go through the assignments in order so that the lower
                // levels feed into higher levels
                foreach (var step in assignmentOrder)
                {
                    foreach (var proc in step)
                    { // attempt to do each process to the maximum capabilities.
                        if (job.Assignments[proc].Iterations == 0)
                            continue; // if no iterations are required for the process, skip.
                        (int successes,
                            decimal progress,
                            Dictionary<IProduct, decimal> productChange,
                            Dictionary<IProduct, decimal> productUsed,
                            Dictionary<IWant, decimal> wantsChange) 
                            outcome
                                = proc.DoProcess(job.Assignments[proc].Iterations,
                                    job.Assignments[proc].Progress, Resources, _remnantWants);
                        // if nothing was done, skip over the remainder.
                        if (outcome.successes == 0 && outcome.progress == 0)
                            continue;
                        // update the progress
                        job.Assignments[proc].Progress = outcome.progress;
                        // update the change in products
                        foreach (var (key, val) in outcome.productChange)
                        {
                            if (Resources.ContainsKey(key))
                                Resources[key] += val;
                            else
                                Resources[key] = val;
                        }
                        // move used products to expended items
                        foreach (var (key, val) in outcome.productUsed)
                        {
                            // since capital must exist to be used, we can guarantee it
                            // is in resources
                            Resources[key] -= val;
                            if (_expendedProducts.ContainsKey(key))
                                _expendedProducts[key] += val;
                            else
                                _expendedProducts[key] = val;
                        }
                        // change wants appropriately.
                        foreach (var (key, val) in outcome.wantsChange)
                        {
                            if (_remnantWants.ContainsKey(key))
                                _remnantWants[key] += val;
                            else
                                _remnantWants[key] = val;
                        }
                    } // move on to next process
                } // go to the next set of processes to do in this job.
            } // move on to the next job
            
            // we have gone through each job, ran through each process, and done what 
            // needed to be done for each 
        }
        
        /// <summary>
        /// Gets change for a targeted abstract value.
        /// Assumes standard market value and priority.
        /// Does not remove from the firm.
        /// </summary>
        /// <param name="value">The abstract value to target.</param>
        /// <returns></returns>
        public Dictionary<IProduct, decimal> GetProductEquivalent(IProduct product, decimal value)
        {
            var result = new Dictionary<IProduct, decimal>();
            // TODO this function.
            // get market prices for the product in question
            // get product equivalent to the abstract value given.

            return result;
        }

        /// <summary>
        /// Gets change for a targeted abstract value.
        /// Assumes standard market value and priority.
        /// Does not remove from the firm's stocks.
        /// </summary>
        /// <param name="value">The abstract value to target.</param>
        /// <returns></returns>
        public Dictionary<IProduct, decimal> GetChange(decimal value)
        {
            var result = new Dictionary<IProduct, decimal>();
            // TODO this function.
            // get market prices for the firm's available resources
            // prioritize those items which are seen as more desirable as currency
            // then do a standard change function to approximate the returns 

            return result;
        }
    }
}
