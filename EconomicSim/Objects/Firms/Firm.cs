using System.Resources;
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
    public class Firm : IFirm, ICanSell
    {
        #region PrivateDataStorage
        
        private HashSet<IProduct>? _firmOutputs;

        #endregion
        
        public Firm()
        {
            _remnantWants = new Dictionary<IWant, decimal>();
            Regions = new List<Market.Market>();
            Resources = new Dictionary<IProduct, decimal>();
            Products = new Dictionary<IProduct, decimal>();
            Jobs = new List<FirmJob>();
            Children = new List<Firm>();
            Techs = new List<(Technology.Technology tech, int research)>();
            Reserves = new Dictionary<IProduct, decimal>();
            ForSale = new Dictionary<IProduct, decimal>();
            SellWeight = new Dictionary<IProduct, decimal>();
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
        /// The products that the firm produces with intent to sell.
        /// </summary>
        public IReadOnlyList<IProduct> FirmOutputs
        {
            get
            {
                if (_firmOutputs != null)
                    return _firmOutputs.ToList();

                foreach (var job in Jobs)
                { // add all outputs for processes
                    foreach (var process in job.Assignments.Keys)
                    {
                        foreach (var output in process.OutputProducts)
                        {
                            _firmOutputs.Add(output.Product);
                        }
                    }
                }
                // Maybe Remove Products which are inputs to other processes 
                // this will require a bit of work to ensure that cycles don't
                // eat themselves. Possibly make it dynamic to add/remove 
                // products which are consumed internally but desired more abroad.

                return _firmOutputs.ToList();
            }
        }

        /// <summary>
        /// The products and price at which it sells them at. May include
        /// items which this firm does not produce.
        /// </summary>
        public Dictionary<IProduct, decimal> Products { get; set; }
        IReadOnlyDictionary<IProduct, decimal> IFirm.Products 
            => Products.ToDictionary(x => (IProduct)x.Key,
                x => x.Value);

        /// <summary>
        /// What resources the Firm owns. Bought goods go here,
        /// made goods go here and are sold from here.
        /// TODO Refine usage to minimise additions and deletions. It should have everything in it before we start.
        /// </summary>
        public Dictionary<IProduct, decimal> Resources { get; set; }
        IReadOnlyDictionary<IProduct, decimal> IFirm.Resources 
            => Resources.ToDictionary(x => (IProduct)x.Key,
                x => x.Value); 

        /// <summary>
        /// The market which the Firm is centered out of.
        /// </summary>
        public IMarket HeadQuarters { get; set; }

        IMarket ICanSell.Market
        {
            get => HeadQuarters;
        }

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
                        // TODO limit the time moved over to the amount desired by the pop.
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
                            // remove it from Resources and put it into used.
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

        #region SellPhase
        
        public async Task<ICanSell> SellPhase()
        {
            // Reserve what we already have and need for tomorrow's (inaccurate) projection.
            // reserve inputs first
            foreach (var job in Jobs)
            {
                var inputReqs = job.InputProductRequirements();
                foreach (var (product, target) in inputReqs)
                {
                    if (!Resources.TryGetValue(product, out var available))
                        continue; // if it doesn't exist in resources right now, skip.
                    // get the min between the available and the target
                    var min = Math.Min(available, target);
                    // add the min to the reserve
                    if (Reserves.ContainsKey(product))
                        Reserves[product] += min;
                    else
                        Reserves[product] = min;
                    // remove min from resources
                    Resources[product] -= min;
                } // onto the next product in the job
            
                // reserve capital that hasn't been expended, but is still needed for the current plans
                var capReqs = job.CapitalProductRequirements();
                foreach (var (product, target) in capReqs)
                {
                    if (!Resources.TryGetValue(product, out var available)) 
                        continue;
                    // get the min between available and the difference between target and expended
                    _expendedProducts.TryGetValue(product, out var expended);
                    var diff = target - expended;
                    var min = Math.Min(diff, available);
                    // if nothing, skip.
                    if (min == 0) continue;
                    // with min, remove them from resources and add them to the reserve.
                    Resources[product] -= min;
                    if (Reserves.ContainsKey(product))
                        Reserves[product] += min;
                    else
                        Reserves[product] = min;
                    // if none left, remove from resources.
                    if (Resources[product] == 0) Resources.Remove(product);
                } // on to the next capital.
                // capital for tomorrow has been reserved.
            } // the next job in the firm.
            
            // reserve products which are needed for maintaining what has already been reserved.
            // don't worry about maintaining what we add here.
            // add expended products
            var temp = _expendedProducts
                .ToDictionary(x => x.Key,
                    x => x.Value);
            // add all reserved products
            foreach (var (reserve, amount) in Reserves)
            {
                if (temp.ContainsKey(reserve))
                    temp[reserve] += amount;
                else
                    temp[reserve] = amount;
            }
            // go through each reserved product
            foreach (var (product, amount) in temp)
            {
                // if it can't be maintained, skip it.
                if (!product.MaintenanceProcesses.Any())
                    continue;
                // if it can, get it's maintenance products and reserve them as well
                // TODO update this to be able to select from multiple maintenance options.
                var process = product.MaintenanceProcesses.First();
                foreach (var input in process.InputProducts)
                {
                    // of none can be moved, skip.
                    if (!Resources.TryGetValue(input.Product, out var available)) continue;
                    // get the min between the amount requested and available.
                    var min = Math.Min(available, input.Amount * amount);
                    if (Reserves.ContainsKey(input.Product)) // add that amount to the reserve.
                        Reserves[input.Product] += min;
                    else
                        Reserves[input.Product] = min;
                    // remove it from Resources
                    Resources[input.Product] -= min;
                    if (Resources[input.Product] == 0) 
                        Resources.Remove(input.Product); // remove resource if empty.
                }
                // do the same for capital requirements
                foreach (var capital in process.CapitalProducts)
                {
                    // of none can be moved, skip.
                    if (!Resources.TryGetValue(capital.Product, out var available)) continue;
                    // get the min between the amount requested and available.
                    var min = Math.Min(available, capital.Amount * amount);
                    if (Reserves.ContainsKey(capital.Product)) // add that amount to the reserve.
                        Reserves[capital.Product] += min;
                    else
                        Reserves[capital.Product] = min;
                    // remove it from Resources
                    Resources[capital.Product] -= min;
                    if (Resources[capital.Product] == 0) 
                        Resources.Remove(capital.Product); // remove resource if empty.
                }
            } // repeat for each product in reserves.
            
            // if the Firm is self-employed add that pop's desires to the reserve.
            if (OwnershipStructure == OwnershipStructure.SelfEmployed)
            { // since we're self employed there should be only one pop.
                var pop = Jobs.First().Pop;
                foreach (var desire in pop.Needs)
                {
                    // of none can be moved, skip.
                    if (!Resources.TryGetValue(desire.Product, out var available)) continue;
                    // get the min between the amount requested and available.
                    var min = Math.Min(available, desire.Amount * pop.Count);
                    if (Reserves.ContainsKey(desire.Product)) // add that amount to the reserve.
                        Reserves[desire.Product] += min;
                    else
                        Reserves[desire.Product] = min;
                    // remove it from Resources
                    Resources[desire.Product] -= min;
                    if (Resources[desire.Product] == 0) 
                        Resources.Remove(desire.Product); // remove resource if empty.
                }
            }
            
            // if the firm has any slaves, add their desires to the reserve.
            // TODO slave desire reserve.

            // Move all remaining, unneeded, resources to the sell pile.
            // If a product has no price, quickly set one based on market price
            while (Resources.Any())
            { // for each product
                var (product, amount) = Resources.First();
                if (amount == 0)
                { // if none is left in resources, remove and continue.
                    Resources.Remove(product);
                    continue; // if none is left in resources, skip.
                }
                // if there is some left, add it to the ForSale list.
                if (ForSale.ContainsKey(product))
                    ForSale[product] += amount;
                else
                    ForSale[product] = amount;
                // and remove it from resources
                Resources.Remove(product);
                // check that there's a price for it already
                if (!Products.ContainsKey(product))
                { // if not, set it to the current market price for ease of guessing.
                    Products.Add(product, HeadQuarters.GetMarketPrice(product));
                }
            } 
            // once all resources have been either reserved, used, or liquidated
            // we are done with our sell phase.
            IsSelling = true;
            return this;
        }
        
        private IDictionary<IProduct, decimal> Reserves { get; set; }

        public Dictionary<IProduct, decimal> SellWeight { get; set; }
        public bool IsSelling { get; set; }
        public IDictionary<IProduct, decimal> ForSale { get; }
        public decimal SalePrice(IProduct product)
        {
            if (Products.ContainsKey(product))
                return Products[product];
            else
            {// if it (for whatever reason) doesn't have a price, base it on the market 
                // and add it to our products while we're at it.
                return Products[product] = HeadQuarters.GetMarketPrice(product);
            }
        }
        
        #endregion
        
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
