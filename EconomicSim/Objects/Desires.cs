using System.Net.Security;
using EconomicSim.Helpers;
using EconomicSim.Objects.Market;
using EconomicSim.Objects.Processes;
using EconomicSim.Objects.Products;
using EconomicSim.Objects.Wants;

namespace EconomicSim.Objects;

/// <summary>
/// The Desires class is a collection and organization of all desires a pop or
/// firm may have. Will be used throughout a day to organize and manage a
/// pop or firms property and know what they can buy or sell.
/// </summary>
public class Desires
{
    #region NeedOrg
    /// <summary>
    /// All Product Desires
    /// </summary>
    public readonly List<INeedDesire> Needs = new List<INeedDesire>();
    /// <summary>
    /// All Want Desires
    /// </summary>
    public readonly List<IWantDesire> Wants = new List<IWantDesire>();
    /// <summary>
    /// All Product Desires with a definite range.
    /// </summary>
    public readonly List<INeedDesire> StretchedNeeds = new List<INeedDesire>();
    /// <summary>
    /// All Want Desires with a definite range.
    /// </summary>
    public readonly List<IWantDesire> StretchedWants = new List<IWantDesire>();
    /// <summary>
    /// All Product Desires with a indefinite range.
    /// </summary>
    public readonly List<INeedDesire> InfiniteNeeds = new List<INeedDesire>();
    /// <summary>
    /// All Want Desires with a indefinite range.
    /// </summary>
    public readonly List<IWantDesire> InfiniteWants = new List<IWantDesire>();
    #endregion
    /// <summary>
    /// All products which we explicitly desire here.
    /// </summary>
    public readonly HashSet<IProduct> DesiredProducts = new();
    /// <summary>
    /// All wants which we desire.
    /// </summary>
    public readonly HashSet<IWant> DesiredWants = new();
    /// <summary>
    /// All property owned owned.
    /// </summary>
    public readonly Dictionary<IProduct, PropertyTriple> AllProperty = new();
    /// <summary>
    /// Excess wants which are available to be absorbed elsewhere.
    /// </summary>
    public readonly Dictionary<IWant, decimal> UnclaimedWants = new();
    /// <summary>
    /// The products which satisfy our desired Products.
    /// </summary>
    public readonly Dictionary<IProduct, decimal> ProductsSatisfied = new();
    /// <summary>
    /// Products saved specifically for ownership purposes.
    /// </summary>
    public readonly Dictionary<IProduct, decimal> OwnershipProducts = new();
    /// <summary>
    /// The Use Processes as used by to satisfy the want, order shouldn't really matter.
    /// </summary>
    public readonly Dictionary<IWant, Dictionary<IProcess, decimal>> UseProcesses = new();
    /// <summary>
    /// The Consumption processes used to satisfy the want, order shouldn't matter.
    /// </summary>
    public readonly Dictionary<IWant, Dictionary<IProcess, decimal>> ConsumedProcesses = new();
    /// <summary>
    /// How many of our products we want in total.
    /// </summary>
    public readonly Dictionary<IProduct, decimal> ProductTargets = new();
    /// <summary>
    /// Wants which have been claimed for processes.
    /// </summary>
    public readonly Dictionary<IWant, decimal> ClaimedWants = new();
    /// <summary>
    /// The current amount of wants which are (predicted) to be satisfied today.
    /// </summary>
    public readonly Dictionary<IWant, decimal> WantsSatisfied = new();
    /// <summary>
    /// How many of each want we want in total.
    /// </summary>
    public readonly Dictionary<IWant, decimal> WantTargets = new();

    #region Constructors

    /// <summary>
    /// Basic Constructor
    /// </summary>
    /// <param name="market">The Market we are evaluating prices in.</param>
    public Desires(IMarket market)
    {
        Market = market;
        FullTierSatisfaction = (int) DesireTier.NonTier;
    }

    /// <summary>
    /// Constructor which takes in existing list of desires.
    /// Sorts those lists so they are ready to use immediately.
    /// </summary>
    /// <param name="market">The Market we're operating in.</param>
    /// <param name="needs">The needs we're looking at.</param>
    /// <param name="wants">The wants we're looking at.</param>
    public Desires(IMarket market, 
        IReadOnlyList<INeedDesire> needs, 
        IReadOnlyList<IWantDesire> wants) : this(market)
    {
        foreach (var need in needs)
        { // copy over the needs so the originals aren't hurt.
            AddDesire(need);
        }
        foreach (var want in wants)
        { // copy over the wants so the originals aren't hurt.
            AddDesire(want);
        }
        // TODO come back here to consider adding in the savings pile.
        // Not everything has a savings pile, but most things should and making it 
    }
    
    /// <summary>
    /// Constructor which also takes in existing products and sets them into their desires.
    /// </summary>
    /// <param name="market"></param>
    /// <param name="needs"></param>
    /// <param name="wants"></param>
    /// <param name="currentProperty"></param>
    public Desires(IMarket market,
        IReadOnlyList<INeedDesire> needs,
        IReadOnlyList<IWantDesire> wants,
        IReadOnlyDictionary<IProduct, decimal> currentProperty) : this(market, needs, wants)
    {
        foreach (var (product, owned) in currentProperty)
        { // throw all property into excess property for now. We'll sift at the end of this.
            AllProperty[product] = new PropertyTriple(owned);
        }
        
    }

    #endregion
    
    /// <summary>
    /// The minimum satisfaction retention when exchanging satisfaction in one tier
    /// for satisfaction in a tier one lower.
    /// </summary>
    public const double TierRatio = 0.9;

    /// <summary>
    /// The market this desire is operating in.
    /// </summary>
    public IMarket Market { get; }

    /// <summary>
    /// The highest tier to which these desires have been satisfied fully.
    /// </summary>
    public int FullTierSatisfaction { get; private set; }
    /// <summary>
    /// This is how many full tiers, skipping empty ones, which have been filled.
    /// </summary>
    public int HardSatisfaction { get; private set; }
    /// <summary>
    /// The raw number of products and wants which have been satisfied.
    /// </summary>
    public decimal QuantitySatisfied { get; private set; }
    /// <summary>
    /// How many products we own.
    /// </summary>
    public decimal ProductsOwned => AllProperty.Sum(x => x.Value.Total);
    /// <summary>
    /// The remaining satisfaction gained above full tier.
    /// </summary>
    public decimal PartialSatisfaction { get; private set; }
    /// <summary>
    /// Retrieves the satisfaction of our desires in their abstract market value equivalent.
    /// </summary>
    public decimal MarketSatisfaction { get; private set; }
    /// <summary>
    /// The total wealth contained in abstract market value terms.
    /// </summary>
    public decimal MarketWealth { get; private set; }
    /// <summary>
    /// The highest tier that any of our products begins satisfying.
    /// </summary>
    public int HighestTier { get; private set; }

    /// <summary>
    /// Sifts all products in our desires.
    /// </summary>
    public void SiftProducts()
    {
        foreach (var product in DesiredProducts)
        {
            SiftProduct(product);
        }
    }
    
    /// <summary>
    /// Sifts a product from Excess into satisfaction
    /// does not assume any sifting prior to work.
    /// </summary>
    /// <param name="product">The product we are attempting to sift.</param>
    public void SiftProduct(IProduct product)
    {
        if (!AllProperty.ContainsKey(product) || 
            AllProperty[product].Total == 0 ||
            !DesiredProducts.Contains(product))
            return; // if no available product exists or it's not desired, skip.
        // since we have product we can shift and a desire for said product, start sifting
        // shift from excess over into Satisfaction whatever we need/can
        if (ProductTargets[product] == -1)
        { // if infinite target
            var shifting = AllProperty[product].Total;
            ProductsSatisfied[product] += shifting;
        }
        else
        { // if finite target
            var missingSat = ProductTargets[product] - ProductsSatisfied[product];
            var shifting = Math.Min(missingSat, AllProperty[product].Total-ProductsSatisfied[product]);
            ProductsSatisfied[product] += shifting;
        }
        // get all needs for this product.
        var needs = Needs
            .Where(x => Equals(x.Product, product))
            .ToList();
        // clear out prior satisfaction values and recalculate for ease of use
        foreach (var need in needs)
            need.Satisfaction = 0;

        var unassignedProducts = ProductsSatisfied[product];

        // TODO upgrade this to something better. Perhaps max/divide evenly then push down from
        // those which are in higher tiers to those with empty tiers below it.
        foreach (var (tier, need) in WalkUpTiersForNeeds(needs))
        {
            if (unassignedProducts == 0)
                break; // if nothing left to assign, stop.
            // get the amount we need or have available
            var assigning = Math.Min(need.Amount, unassignedProducts);
            need.Satisfaction += assigning; // add to satisfaction
            unassignedProducts -= assigning; // remove from unassigned amount
        }
    }
    
    /// <summary>
    /// Tries to fill a want at a specified tier.
    /// </summary>
    /// <param name="desire">The desire to fill.</param>
    /// <param name="tier">The tier we're filling at.</param>
    /// <returns>True if satisfied, false in unable to satisfy completely.</returns>
    private bool FulfillWant(IWantDesire desire, int tier)
    {
        // sanity check that we need to satisfy at this tier at all.
        var satisfaction = desire.SatisfiedAtTier(tier);
        if (satisfaction == 1)
            return true; // if we are already satisfied, just skip out.
        // if any remains, set our target amount to satisfy to the unsatisfied amount.
        var amount = desire.Amount - desire.Amount * satisfaction;
        
        // try to satisfy from excess Wants available, if possible.
        if (UnclaimedWants.TryGetValue(desire.Want, out var availableWant) && availableWant > 0)
        {
            var available = Math.Min(amount, availableWant);
            amount -= available;
            WantsSatisfied[desire.Want] += available;
            UnclaimedWants[desire.Want] -= available;
            desire.Satisfaction += available;
        }
        if (amount == 0)
            return true; // if satisfied, break out.

        // try to fulfill from owning products first.
        if (desire.Want.OwnershipSources.Any() &&
            AllProperty.Keys.Any(x => desire.Want.OwnershipSources.Contains(x)))
        {
            // check that any possible products have a value
            var validProducts = AllProperty
                .Where(x => desire.Want.OwnershipSources.Contains(x.Key))
                .Where(x => x.Value.Available > 0)
                .Select(x => x.Key)
                .ToList();
            foreach (var product in validProducts)
            {
                // get how efficient the product is per unit
                var wantEff = product.Wants[desire.Want];
                // get either the amount of product needed, or the amount available.
                // Should have a value above 0 available.
                var available = Math.Min(desire.Amount / wantEff, AllProperty[product].Available);
                // reserve it
                AllProperty[product].Reserved += available;
                OwnershipProducts.AddOrInclude(product, available);
                // and update our satisfaction from our product
                foreach (var (satisfiedWant, rate) in product.Wants)
                {
                    var created = rate * available;
                    // add entry to satisfied wants and unclaimed wants if they aren't there.
                    if (Equals(desire.Want, satisfiedWant))
                    { // the satisfiedWant is what we are trying to satisfy
                        WantsSatisfied.AddOrInclude(satisfiedWant, created); // add it to our satisfaction
                        desire.Satisfaction += created;
                        amount -= created; // and remove it from the amount
                    }
                    else // if not what we are trying to satisfy, save it for later possible use.
                        UnclaimedWants.AddOrInclude(satisfiedWant, created);
                }
            }
            if (amount == 0)
                return true; // if completed amount, break out!
        }
        
        // if we get here and there is desire remaining, check for use processes
        if (desire.Want.UseSources.Any())
        {
            var useProcs = desire.Want.UseSources // from the usable products
                .Where(x => AllProperty.ContainsKey(x) && AllProperty[x].Available > 0) // get the items we can even use
                .Select(x => x.UseProcess!); // select their use processes (which are guaranteed to exist)
                // these processes output our want by definition (they wouldn't be in the want's use processes otherwise)
            foreach (var process in useProcs)
            { // try each process
                // get how much it outputs on average.
                var output = process.ProjectedWantAmount(desire.Want, ProcessPartTag.Output);
                var target = amount / output; // how many times we would need to do it
                var processData = process.DoProcess(target, 
                    AllProperty.ToDictionary(x => x.Key,
                        y => y.Value.Available),
                    UnclaimedWants);
                if (processData.successes == 0)
                    continue; // if no wants can be gotten, try the next.
                // with some number possible, reserve the inputs/capital
                foreach (var (prod, change) in processData.productChange)
                {
                    // if it's consumed, remove from available/reserve it (created items shouldn't be recorded yet)
                    if (change < 0)
                    {
                        AllProperty[prod].Available += change;
                    } // don't include new products for sanity purposes
                }
                // do the same with capital
                foreach (var (prod, used) in processData.productUsed)
                {
                    AllProperty[prod].Reserved += used;
                }
                // claim any input wants also
                foreach (var (want, consumed) in processData.wantsChange.Where(x => x.Value < 0))
                { // if we're consuming a want, then claim it and remove it from unclaimed.
                    UnclaimedWants[want] -= consumed;
                    // move consumed wants into claimed wants.
                    ClaimedWants.AddOrInclude(want, consumed);
                }
                // and record that we're using this process for later uses
                if (!UseProcesses.ContainsKey(desire.Want))
                    UseProcesses.Add(desire.Want, new Dictionary<IProcess, decimal>());
                UseProcesses[desire.Want].AddOrInclude(process, processData.successes);
                // get how much we expect to be output for this
                var expectation = processData.wantsChange[desire.Want]; // this Must output the want, otherwise it's connection is false.
                WantsSatisfied.AddOrInclude(desire.Want, expectation);
                desire.Satisfaction += expectation;
                amount -= expectation;
                if (amount == 0)
                    return true; // if no want left, breakout. 
            }
        }
        
        // lastly, try for consumption processes
        if (desire.Want.ConsumptionSources.Any())
        {
            var consumptionProcs = desire.Want.ConsumptionSources // from the usable products
                .Where(x => AllProperty.ContainsKey(x) && AllProperty[x].Available > 0) // get the items we can even use
                .Select(x => x.ConsumptionProcess!); // select their consumption processes (which are guaranteed to exist)
            // these processes are guaranteed to 
            foreach (var process in consumptionProcs)
            {
                // how much it outputs on average.
                var output = process.ProjectedWantAmount(desire.Want, ProcessPartTag.Output);
                var target = amount / output; // how many times we would need to do it
                var processData = process.DoProcess(target, 
                    AllProperty.ToDictionary(x => x.Key,
                        x => x.Value.Available),
                    UnclaimedWants);
                if (processData.successes == 0)
                    continue; // if no wants can be gotten, try the next.
                // with some number possible, reserve the inputs/capital
                foreach (var (prod, change) in processData.productChange)
                {
                    // if it's consumed, add to reserve (created items shouldn't be recorded yet)
                    if (change < 0)
                    {
                        AllProperty[prod].Available += change;
                    } // don't include new products for sanity purposes
                }
                foreach (var (prod, used) in processData.productUsed)
                { // do the same with capital
                    AllProperty[prod].Reserved += used;
                }
                // claim any input wants also 
                foreach (var (want, consumed) in processData.wantsChange.Where(x => x.Value < 0))
                { // if we're consuming a want, then we must have claimed it earlier
                    UnclaimedWants[want] -= consumed;
                }
                // and record that we're using this process for later uses
                if (!ConsumedProcesses.ContainsKey(desire.Want))
                    ConsumedProcesses.Add(desire.Want, new Dictionary<IProcess, decimal>());
                ConsumedProcesses[desire.Want].AddOrInclude(process, processData.successes);
                // get how much we expect to be output for this
                var expectation = processData.wantsChange[desire.Want]; // this Must output the want, otherwise it's connection is false.
                WantsSatisfied.AddOrInclude(desire.Want, expectation);
                desire.Satisfaction += expectation;
                amount -= expectation;
                if (amount == 0)
                    return true; // if no want left, breakout. 
            }
        }

        return false;
    }

    /// <summary>
    /// Sifts excess property into the proper places for a specified want to be satisfied.
    /// Do Assumes products have been run first, do not run before products have been sifted.
    /// Does not consume products here, must run <seealso cref="SatisfyWants"/> to consume what
    /// was calculated here.
    /// </summary>
    public void SiftWants()
    {
        var LastTier = -1001;

        var wantsToSatisfy = new HashSet<IWant>(DesiredWants);
        foreach (var (tier, wantDesire) in WalkUpTiersForWants(Wants))
        {
            // if we can't satisfy the want anymore, go to the next.
            if (!wantsToSatisfy.Contains(wantDesire.Want))
                continue;
            // try to satisfy the want
            var success = FulfillWant(wantDesire, tier);
            // if we are able to satisfy, continue to the next want. If not remove it from our wants to satisfy list.
            if (!wantDesire.FullySatisfied && success)
                continue;
            // since we didn't succeed, remove it.
            wantsToSatisfy.Remove(wantDesire.Want);
            // if we've run out of wants we can satisfy, break out.
            if (!wantsToSatisfy.Any())
                break;
        }
        
        // since wants always run last, update our Tier Satisfaction.
        UpdateSatisfaction();
    }

    /// <summary>
    /// Completes satisfying the wants, using or consuming any products which remain.
    /// Does no recalculation and return false if we overdraw our products in our current iteration.
    /// </summary>
    public decimal SatisfyWants()
    {
        // since we assume that all products are properly placed, just run everything
        // and then check if we succeeded or not.
        
        // we don't consume any products to satisfy desires. Leave them in place.
        // go through the processes and consume as given.

        // since calculation is complete (hypothetically) by this point,
        // just run the process. No need to run ownership or consume other wants
        
        foreach (var (want, useSources) in UseProcesses)
        { // for each want
            foreach (var source in useSources)
            {
                if (source.Value == 0)
                    continue; // if no uses included, skip.
                var result = source.Key.DoProcess(source.Value,
                    AllProperty.ToDictionary(x => x.Key,
                        x => x.Value.Total),
                    ClaimedWants);
                // with results gotten, add and remove products and wants
                foreach (var (product, change) in result.productChange)
                {
                    // add or remove from total as needed
                    if (AllProperty.ContainsKey(product))
                    { // if it already exists, add/sub from total and reserve
                        AllProperty[product].Total += change;
                        AllProperty[product].Reserved += change;
                    }
                }
                foreach (var (product, used) in result.productUsed)
                { // also move product from all to used, to ensure no double dipping.
                    AllProperty[product].Reserved -= used;
                    AllProperty[product].Exhausted += used;
                }
                foreach (var (changedWant, amount) in result.wantsChange)
                { // update wants also, remove from claimed, add to unclaimed
                    if (amount > 0)
                        UnclaimedWants.AddOrInclude(changedWant, amount);
                    else
                        ClaimedWants.AddOrInclude(changedWant, amount);
                }
            }
        }

        foreach (var (want, consumptionSources) in ConsumedProcesses)
        { // for each want
            foreach (var source in consumptionSources )
            {
                if (source.Value == 0)
                    continue; // if no uses included, skip.
                var result = source.Key.DoProcess(source.Value,
                    AllProperty.ToDictionary(x => x.Key,
                        x => x.Value.Total),
                    ClaimedWants);
                // with results gotten, add and remove products and wants
                foreach (var (product, change) in result.productChange)
                {
                    // add or remove from total as needed
                    if (AllProperty.ContainsKey(product))
                    { // if it already exists, add/sub from total and reserve
                        AllProperty[product].Total += change;
                        AllProperty[product].Reserved += change;
                    }
                }
                foreach (var (product, used) in result.productUsed)
                { // also move product from all to used, to ensure no double dipping.
                    AllProperty[product].Reserved -= used;
                    AllProperty[product].Exhausted += used;
                }
                foreach (var (changedWant, amount) in result.wantsChange)
                { // update wants also, remove from claimed, add to unclaimed
                    if (amount > 0)
                        UnclaimedWants.AddOrInclude(changedWant, amount);
                    else
                        ClaimedWants.AddOrInclude(changedWant, amount);
                }
            }
        }
        // any leftover wants are saved for the next day.
        // check that we haven't overdrawn, if we have, return by how much.
        if (AllProperty.Any(x => x.Value.Total < 0))
        {
            return AllProperty
                .Where(x => x.Value.Total < 0)
                .Sum(x => x.Value.Total);
        }

        return 0;
    }

    public void UpdateSatisfaction()
    {
        // start with FullTierSat and highest tier.
        FullTierSatisfaction = int.MaxValue;
        HighestTier = -1001;
        foreach (var need in Needs)
        { // check needs
            var tier = need.SatisfactionUpToTier();
            if (!need.FullySatisfied)
            { // if not fully satisfied, run against FullTierSatisfaction
                FullTierSatisfaction = Math.Min(
                    need.SatisfiedAtTier(tier) < 1 ? tier - 1 : tier,
                    FullTierSatisfaction);
            }
            // always check against highest tier reached
            HighestTier = Math.Max(HighestTier, tier);
        }
        foreach (var want in Wants)
        { // as above so below.
            var tier = want.SatisfactionUpToTier();
            if (!want.FullySatisfied)
            {
                FullTierSatisfaction = Math.Min(
                    want.SatisfiedAtTier(tier) < 1 ? tier - 1 : tier,
                    FullTierSatisfaction);
            }
            HighestTier = Math.Max(HighestTier, tier);
        }
        // get quantity satisfied real quick also.
        QuantitySatisfied = Needs.Sum(x => x.Satisfaction) + Wants.Sum(x => x.Satisfaction);
        
         // get partial satisfaction
         PartialSatisfaction = 0;
         for (int i = FullTierSatisfaction + 1; i <= HighestTier && i < FullTierSatisfaction + 50; ++i)
         { // go from our highest max tier to our highest tier, capping at 50 steps for accuracy reasons.
             // TODO consider pruning desires we can't touch as we walk up.
             // TODO consider replacing total desire with relative desire at tier.
             var (satisfaction, total) = TotalDesireAtTier(i);
             if (total > 0)
             {
                 var satATtTier = ScaleSatisfactionByTier(i, FullTierSatisfaction, satisfaction);
                 PartialSatisfaction += satATtTier;
             }
         }
         
         // get market satisfaction
         MarketSatisfaction = 0;
         foreach (var need in Needs)
             MarketSatisfaction += Market.GetMarketPrice(need.Product) * need.Satisfaction;
        
         // get total wealth while we're at it.
         MarketWealth = AllProperty.Sum(x => Market.GetMarketPrice(x.Key) * x.Value.Total);
         
         // finish by getting the Hard Satisfaction
         var lowest = Math.Min(Needs.Min(x => x.StartTier), Wants.Min(x => x.StartTier));
         var skipped = 0;
         for (int i = lowest; i <= FullTierSatisfaction; ++i)
         {
             if (Wants.Any(x => x.StepsOnTier(i)) ||
                 Needs.Any(x => x.StepsOnTier(i)))
                 continue;
             skipped += 1;
         }

         HardSatisfaction = FullTierSatisfaction - skipped;
    }

    private decimal ScaleSatisfactionByTier(int startTier, int targetTier, decimal satisfaction)
    {
        return (decimal)Math.Pow((double)satisfaction, targetTier - startTier);
    }

    private (decimal satisfaction, decimal desired) TotalDesireAtTier(int tier)
    {
        decimal satisfied = 0;
        decimal desired = 0;
        foreach (var want in Wants.Where(x => x.StepsOnTier(tier)))
        {
            satisfied += want.SatisfiedAtTier(tier);
            desired += want.Amount;
        }

        foreach (var need in Needs.Where(x => x.StepsOnTier(tier)))
        {
            satisfied += need.SatisfiedAtTier(tier);
            desired += need.Amount;
        }
        return (satisfied, desired);
    }

    public IEnumerable<(int tier, IWantDesire desire)> WalkUpTiersForWants(IList<IWantDesire> wants, int tier = -1001)
    {
        Dictionary<int, List<IWantDesire>> tieredDesires = new();

        foreach (var want in wants.Where(x => x.GetNextTierUp(tier - 1) != (int)DesireTier.NonTier))
        { // go through and add to the list of the tiers.
            if (tieredDesires.ContainsKey(want.GetNextTierUp(tier-1)))
                tieredDesires[want.GetNextTierUp(tier-1)].Add(want);
            else
                tieredDesires[want.GetNextTierUp(tier-1)] = new List<IWantDesire> {want};
        }

        while (tieredDesires.Any())
        {
            // update the tier to the new minimum
            tier = tieredDesires.Keys.Min();
            // get the first in our list
            var want = tieredDesires[tier].First();
            yield return (tier, want);
            // remove from it's current location
            tieredDesires[tier].Remove(want);
            // when we come back, get the next tier of the current need
            var nextTier = want.GetNextTierUp(tier);
            // if last item in tier, remove that tier
            if (!tieredDesires[tier].Any())
                tieredDesires.Remove(tier);
            if (nextTier == (int) DesireTier.NonTier)
                continue; // if there is no next tier, skip the rest
            // add the need back in at the appropriate tier.
            if (tieredDesires.ContainsKey(nextTier))
                tieredDesires[nextTier].Add(want);
            else
                tieredDesires[nextTier] = new List<IWantDesire> {want};
            // then go back to start
        }
    }

    public IEnumerable<(int tier, INeedDesire desire)> WalkUpTiersForNeeds(IList<INeedDesire> needs, int tier = -1001)
    {
        Dictionary<int, List<INeedDesire>> tieredDesires = new();
        
        foreach (var need in needs.Where(x => x.GetNextTierUp(tier - 1) != (int)DesireTier.NonTier))
        { // go through and add to the need to the tiered desires
          // if they have a tier at or after the given one.
            if (tieredDesires.ContainsKey(need.GetNextTierUp(tier-1)))
                tieredDesires[need.GetNextTierUp(tier-1)].Add(need);
            else
            {
                tieredDesires[need.GetNextTierUp(tier-1)] = new List<INeedDesire> {need};
            }
        }

        while (tieredDesires.Any())
        {
            // update the tier to the new minimum
            tier = tieredDesires.Keys.Min();
            // get the first in our list
            var need = tieredDesires[tier].First();
            yield return (tier, need);
            // remove from it's current location
            tieredDesires[tier].Remove(need);
            // when we come back, get the next tier of the current need
            var nextTier = need.GetNextTierUp(tier);
            // if last item in tier, remove that tier
            if (!tieredDesires[tier].Any())
                tieredDesires.Remove(tier);
            if (nextTier == (int) DesireTier.NonTier)
                continue; // if there is no next tier, skip the rest
            // add the need back in at the appropriate tier.
            if (tieredDesires.ContainsKey(nextTier))
                tieredDesires[nextTier].Add(need);
            else
                tieredDesires[nextTier] = new List<INeedDesire> {need};
            // then go back to start
        }
    }
    
    public IEnumerable<(int tier, IWantDesire desire)> WalkDownTiersForWants(IList<IWantDesire> wants, int tier)
    {
        if (tier < -1000)
            throw new InvalidOperationException("Tier must be greater than -1000");
        
        Dictionary<int, List<IWantDesire>> tieredDesires = new();

        foreach (var want in wants.Where(x => x.StartTier > tier))
        { // go through and add to the list of the tiers.
            if (tieredDesires.ContainsKey(want.GetNextTierUp(tier-1)))
                tieredDesires[want.GetNextTierUp(tier-1)].Add(want);
            else
                tieredDesires[want.GetNextTierUp(tier-1)] = new List<IWantDesire> {want};
        }

        while (tieredDesires.Any())
        {
            // update the tier to the new minimum
            tier = tieredDesires.Keys.Min();
            // get the first in our list
            var want = tieredDesires[tier].First();
            yield return (tier, want);
            // remove from it's current location
            tieredDesires[tier].Remove(want);
            // when we come back, get the next tier of the current need
            var nextTier = want.GetNextTierUp(tier);
            // if last item in tier, remove that tier
            if (!tieredDesires[tier].Any())
                tieredDesires.Remove(tier);
            if (nextTier == (int) DesireTier.NonTier)
                continue; // if there is no next tier, skip the rest
            // add the need back in at the appropriate tier.
            if (tieredDesires.ContainsKey(nextTier))
                tieredDesires[nextTier].Add(want);
            else
                tieredDesires[nextTier] = new List<IWantDesire> {want};
            // then go back to start
        }
    }

    public IEnumerable<(int tier, INeedDesire desire)> WalkDownTiersForNeeds(IList<INeedDesire> needs, int tier)
    {
        if (tier < -1000)
            throw new InvalidOperationException("Tier must be greater than -1000");
        
        Dictionary<int, List<INeedDesire>> tieredDesires = new();
        
        foreach (var need in needs.Where(x => x.GetNextTierUp(tier - 1) != (int)DesireTier.NonTier))
        { // go through and add to the need to the tiered desires
          // if they have a tier at or after the given one.
            if (tieredDesires.ContainsKey(need.GetNextTierUp(tier-1)))
                tieredDesires[need.GetNextTierUp(tier-1)].Add(need);
            else
            {
                tieredDesires[need.GetNextTierUp(tier-1)] = new List<INeedDesire> {need};
            }
        }

        while (tieredDesires.Any())
        {
            // update the tier to the new minimum
            tier = tieredDesires.Keys.Min();
            // get the first in our list
            var need = tieredDesires[tier].First();
            yield return (tier, need);
            // remove from it's current location
            tieredDesires[tier].Remove(need);
            // when we come back, get the next tier of the current need
            var nextTier = need.GetNextTierUp(tier);
            // if last item in tier, remove that tier
            if (!tieredDesires[tier].Any())
                tieredDesires.Remove(tier);
            if (nextTier == (int) DesireTier.NonTier)
                continue; // if there is no next tier, skip the rest
            // add the need back in at the appropriate tier.
            if (tieredDesires.ContainsKey(nextTier))
                tieredDesires[nextTier].Add(need);
            else
                tieredDesires[nextTier] = new List<INeedDesire> {need};
            // then go back to start
        }
    }
    
    /// <summary>
    /// Gets how satisfied we are at a specific desire tier.
    /// </summary>
    /// <param name="tier">The tier we are looking at.</param>
    /// <returns>The satisfaction between 0 and 1. If no desires in tier, we return -1.</returns>
    public decimal SatisfactionAtTier(int tier)
    { // shortcut! If FullTier is above or equal to the tier given, we can assume full satisfaction.
        if (FullTierSatisfaction >= tier) return 1;

        List<IDesire> desires = new();
        foreach (var need in Needs.Where(x => x.StepsOnTier(tier)))
            desires.Add(need);
        foreach (var want in Wants.Where(x => x.StepsOnTier(tier)))
            desires.Add(want);
        // If no desires in this tier, return -1, marking it as empty
        if (!desires.Any()) return -1;

        return desires.Average(x => x.SatisfiedAtTier(tier));
    }

    /// <summary>
    /// Calculates the effective equivalence between 2 tiers.
    /// </summary>
    /// <param name="originalTier">The tier we are starting at.</param>
    /// <param name="targetTier">The tier we are transferring value to.</param>
    /// <returns>
    /// TierRatio^(originalTier - TargetTier).
    /// </returns>
    public double TierDesireEquivalence(int originalTier, int targetTier)
    {
        return Math.Pow(TierRatio, originalTier - targetTier);
    }

    /// <summary>
    /// Looks at the desires to find the next level above the given tier which has something in it.
    /// </summary>
    /// <param name="tier">The tier we are looking above.</param>
    /// <returns>The next lowest tier above our given. If no tier found, returns -1001.</returns>
    public int NextLowestTierAbove(int tier)
    { // TODO consider using or deleting this.
        throw new NotImplementedException();
        /*
        // if any infinite desire starts at or before our tier, then we must have a a tier+1
        if (_infiniteNeeds.Any(x => x.StartTier <= tier && x.Step == 1) ||
            _infiniteWants.Any(x => x.StartTier <= tier && x.Step == 1))
            return tier + 1;
        
        // do similar with stretched desires,
        // if we aren't infinite, are between our posts
        // and have a step of 1, we must have a Tier+1
        if (_stretchedNeeds.Any(x => !x.IsInfinite && x.StartTier <= tier 
                                                   && x.EndTier > tier && x.Step == 1) ||
            _stretchedWants.Any(x => !x.IsInfinite && x.StartTier <= tier 
                                                   && x.EndTier > tier && x.Step == 1))
            return tier + 1;

        // lastly, check that any desire starts at Tier+1, just to remove all except non-stretched
        if (_needs.Any(x => x.StartTier == tier + 1) ||
            _needs.Any(x => x.StartTier == tier + 1))
            return tier + 1;
        
        // No 1 steps cover this region, and no need steps on it to start, so look at stretched values
        // and begin working from there.
        List<IDesire> desires = new();
        foreach (var want in _stretchedNeeds)
            desires.Add(want);
        foreach (var need in _stretchedWants)
            desires.Add(need);
        // if there are no stretched desires at all, return
        if (desires.Any()) return (int)DesireTier.NonTier;
        
        // remove all non-infinite desires which end below our value.
        desires.RemoveAll(x => !x.IsInfinite && x.EndTier <= tier);
        // check that we still have something
        if (!desires.Any()) return (int)DesireTier.NonTier;
        
        // since we have something, run through the list getting the next tier
        return desires.Select(x => x.GetNextTier(tier)).Min();
        */
    }

    /// <summary>
    /// Adds desire to our sets.
    /// </summary>
    /// <param name="desire">The desire to add</param>
    public void AddDesire(INeedDesire desire)
    {
        // check to see if we can consolidate this new desire into an existing desire
        var dupe = Needs.SingleOrDefault(x =>
            x.IsEquivalentTo(desire));
        if (dupe != null)
        { // if a desire just like it already exists
            dupe.Amount += desire.Amount;
            if (!desire.IsInfinite)
                ProductTargets[dupe.Product] += desire.TotalDesire();
            return;
        }
        // if not a duplicate of an existing need, add it.
        // add to our list a duplicate for safe keeping 
        var newNeed = new NeedDesire(desire); 
        Needs.Add(newNeed);
        if (newNeed.IsStretched) // if it has steps, note that.
            StretchedNeeds.Add(newNeed);
        if (newNeed.IsInfinite) // if it's infinite, note that.
            InfiniteNeeds.Add(newNeed);
        // get our product for ease of working forward.
        var prod = desire.Product;
        // add to the set of desired products if possible.
        DesiredProducts.Add(prod);
        // add to our satisfaction for later use.
        if (!ProductsSatisfied.ContainsKey(prod))
            ProductsSatisfied[prod] = 0;
        // add to our targets
        if (!ProductTargets.ContainsKey(newNeed.Product))
            ProductTargets[prod] = 0;
        if(ProductTargets[prod] != -1)
        {
            if (newNeed.IsInfinite)
                ProductTargets[prod] = -1;
            else
                ProductTargets[prod] += newNeed.TotalDesire();
        }
    }

    /// <summary>
    /// Adds desire to our sets.
    /// </summary>
    /// <param name="desire">The desire to add</param>
    public void AddDesire(IWantDesire desire)
    {
        // check to see if we can consolidate this new desire into an existing desire
        var dupe = Wants.SingleOrDefault(x =>
            x.IsEquivalentTo(desire));
        if (dupe != null)
        {
            // if a desire just like it already exists
            dupe.Amount += desire.Amount;
            if (!desire.IsInfinite)
                WantTargets[dupe.Want] += desire.TotalDesire();
            return;
        }

        // add to our list a duplicate for safe keeping
        var newWant = new WantDesire(desire);
        Wants.Add(newWant);
        if (newWant.IsStretched) // if it has steps, note that.
            StretchedWants.Add(newWant);
        if (newWant.IsInfinite) // if it's infinite, note that.
            InfiniteWants.Add(newWant);
        // get our product for ease of working forward.
        var want = desire.Want;
        // add to the set of desired products if possible.
        DesiredWants.Add(want);
        // add our satisfaction if it's not there
        if (!WantsSatisfied.ContainsKey(want))
            WantsSatisfied[want] = 0;
        // add to targets if it's not there.
        if (!WantTargets.ContainsKey(want))
            WantTargets[want] = 0;
        if (WantTargets[want] != -1)
        {
            if ( desire.IsInfinite) // if new desire is infinite, set to infinite
                WantTargets[want] = -1;
            else
                // if not infinite, add to desire and that's it.
                WantTargets[want] += desire.TotalDesire();
        }
    }

    /// <summary>
    /// Returns the products desired at the selected tier.
    /// </summary>
    /// <param name="tier">The tier selected.</param>
    /// <returns>All the products which can be satisfied at that tier.</returns>
    public IReadOnlyList<INeedDesire> NeedsAtTier(int tier)
    { 
        return Needs.Where(x => x.StepsOnTier(tier)).ToList();
    }
    
    /// <summary>
    /// Returns the products desired at the selected tier.
    /// </summary>
    /// <param name="tier">The tier selected.</param>
    /// <returns>All the products which can be satisfied at that tier.</returns>
    public IReadOnlyList<IWantDesire> WantsAtTier(int tier)
    {
        return Wants.Where(x => x.StepsOnTier(tier)).ToList();
    }
    
    /// <summary>
    /// Gets the total amount of a specific desire at a particular tier across multiple wants.
    /// </summary>
    /// <param name="product">The want sought</param>
    /// <param name="tier">The tier we are looking at.</param>
    /// <returns>The summarized want and how satisfied it is at this tier.</returns>
    public decimal TotalNeedDesiredAtTier(IProduct product, int tier)
    { // untested, not going to bother yet.
        var onTier = Needs.Where(x => x.Product == product)
            .Where(x => x.StepsOnTier(tier));

        return onTier.Sum(x => x.Amount);
    }
    
    /// <summary>
    /// Gets the total amount of a specific desire at a particular tier across multiple wants.
    /// </summary>
    /// <param name="want">The want sought</param>
    /// <param name="tier">The tier we are looking at.</param>
    /// <returns>The summarized want and how satisfied it is at this tier.</returns>
    public decimal TotalWantDesiredAtTier(IWant want, int tier)
    { // untested
        var onTier = Wants.Where(x => x.Want == want)
            .Where(x => x.StepsOnTier(tier));

        return onTier.Sum(x => x.Amount);
    }
    
    /// <summary>
    /// Gets the total amount of a specific desire at a particular tier across multiple wants.
    /// </summary>
    /// <param name="product">The want sought</param>
    /// <param name="tier">The tier we are looking at.</param>
    /// <returns>The summarized want and how satisfied it is at this tier.</returns>
    public decimal TotalNeedSatisfiedAtTier(IProduct product, int tier)
    { // untested
        return Needs.Where(x => x.Product == product)
            .Where(x => x.StepsOnTier(tier))
            .Sum(x => x.SatisfiedAtTier(tier));
    }
    
    /// <summary>
    /// Gets the total amount of a specific desire at a particular tier across multiple wants.
    /// </summary>
    /// <param name="want">The want sought</param>
    /// <param name="tier">The tier we are looking at.</param>
    /// <returns>Satisfaction of that product on this tier</returns>
    public decimal TotalWantSatisfiedAtTier(IWant want, int tier)
    { // untested
        return Wants.Where(x => x.Want == want)
            .Where(x => x.StepsOnTier(tier))
            .Sum(x => x.SatisfiedAtTier(tier));
    }

    /// <summary>
    /// Takes a product, adds it to our property, and sifts it immediately into our desires.
    /// </summary>
    /// <param name="product">The product we are adding and Sifting.</param>
    /// <param name="amount">how many of the product we are adding and sifting.</param>
    /// <returns>The satisfaction generated from the addition and sifting.</returns>
    public Dictionary<int, decimal> AddAndSiftProduct(IProduct product, decimal amount)
    {
        // start by adding to our property
        if (!AllProperty.ContainsKey(product))
            AllProperty[product] = new PropertyTriple(amount);
        else
            AllProperty[product].Total += amount;
        
        if (DesiredProducts.Contains(product))
        { // if we want the product at all, then sift it.
            decimal canSift = 0;
            if (ProductTargets[product] == -1)
            { // if target is infinite, then mark all of it for sifting
                canSift = amount;
                ProductsSatisfied[product] += canSift;
            }
            else // if finite target, see how much we need to sift vs can sift
            {
                var missingSat = ProductTargets[product] - ProductsSatisfied[product];
                canSift = Math.Min(missingSat, amount);
                ProductsSatisfied[product] += canSift;
            }
            // get all needs for this product
            var needs = Needs
                .Where(x => Equals(x.Product, product))
                .ToList();
            // sift into the needs, start from our lowest satisfied need
            foreach (var (tier, need) in WalkUpTiersForNeeds(needs,
                         needs.Min(x => x.SatisfactionUpToTier())))
            {
                if (canSift == 0) break; // if nothing left to sift, gfto.
                // assign what we need from what we have
                var assigning = Math.Min(need.Amount, canSift);
                need.Satisfaction += assigning;
                canSift -= assigning;
            }
        }
        
        // then try and add to our wants.
        // get all wants where the product could be used, be generous.
        var possibleWants = Wants
            .Where(x => x.Want.OwnershipSources.Contains(product) || // whose we can get by ownership
                        x.Want.ConsumptionSources // those we might be able to satisfy via consumption
                            .Any(x => x.ConsumptionProcess
                                .ProcessProducts
                                .Any(x => x.Product == product &&
                                          x.Part != ProcessPartTag.Output)) ||
                        x.Want.UseSources // those we might be able to satisfy by use.
                            .Any(x => x.UseProcess
                                .ProcessProducts
                                .Any(x => x.Product == product &&
                                          x.Part != ProcessPartTag.Output)))
            .ToList();
        foreach (var (tier, desire) in WalkUpTiersForWants(possibleWants))
        { // go through each possible want and try to satisfy the with our new product added to our property.
            // check ownership for our want
            if (desire.Want.OwnershipSources.Contains(product))
            { // if product is a valid ownership source, try to use it here first
                
            }
        }
    }

    /// <summary>
    /// Given a product and an amount of it, ESTIMATE how much satisfaction we gain.
    /// </summary>
    /// <param name="product">The product to check adding.</param>
    /// <param name="amount">The number of units to check for that item.</param>
    /// <returns>The starting tier and satisfaction gained based on that tier.</returns>
    public Dictionary<int, decimal> SatisfactionGainedFrom(IProduct product, decimal amount)
    {
        var result = new Dictionary<int, decimal>();
        var available = amount;
        // check products first
        if (DesiredProducts.Contains(product))
        { // if we desire the product, check where it can go.
            var validNeeds = Needs
                .Where(x => x.Product == product)
                .Where(x => !x.FullySatisfied)
                .OrderBy(x => x.SatisfactionUpToTier())
                .ToList();
            foreach (var (tier, need) in WalkUpTiersForNeeds(validNeeds,
                         validNeeds.Min(x => x.SatisfactionUpToTier())))
            { // check each valid need it could satisfy and see how much we could add ot each level
                var satAtTier = need.SatisfiedAtTier(tier);
                if (satAtTier < 1)
                { // if the need has satisfaction left
                    // record how much we satisfy
                    var remainder = need.Amount - satAtTier * need.Amount;
                    var min = Math.Min(remainder, available);
                    result.AddOrInclude(tier, min);
                    available -= min; // and remove from our available going forward.
                }

                if (available == 0)
                    break; // if no product left, continue on to wants.
            }
        }
        // reset available (products and wants can call the same product).
        available = amount;
        // then check for wants
        // those wants which could be satisfied by this new product in any way,
        var possibleDesire = Wants
            .Where(x => x.Want.OwnershipSources.Contains(product) ||
                        x.Want.UseSources.Contains(product) ||
                        x.Want.ConsumptionSources.Contains(product))
            .Where(x => !x.FullySatisfied).ToList();
        foreach (var (tier, desire) in WalkUpTiersForWants(possibleDesire,
                     possibleDesire.Min(x => x.SatisfactionUpToTier())))
        { // go through each want, and attempt to satisfy it with this new product (assume other products met)
            if (desire.Want.OwnershipSources.Contains(product))
            { // first try ownership
                var eff = product.Wants[desire.Want]; // how many want / prod
                var needed = desire.Amount / eff; // how many prod needed
                var target = Math.Min(needed, available); // how many prod we have vs available
                available -= target; // remove from the available
                var satisfaction = desire.Amount * target / needed; 
                result.AddOrInclude(tier, satisfaction); // and add to results
            }
            if (desire.Want.UseSources.Contains(product))
            { // then try use
                var proc = product.UseProcess!;
                var inEff = proc.CapitalProducts.Single(x => x.Product == product).Amount;
                var outEff = proc.OutputWants.Single(x => x.Want == desire.Want).Amount;
                var productNeeded = desire.Amount / outEff * inEff;
                var minProd = Math.Min(productNeeded, available);
                available -= minProd;
                var satisfaction = desire.Amount * minProd / productNeeded;
                result.AddOrInclude(tier, satisfaction);
            }
            if (desire.Want.ConsumptionSources.Contains(product))
            { // then consumption
                var proc = product.ConsumptionProcess!;
                var inEff = proc.InputProducts.Single(x => x.Product == product).Amount;
                var outEff = proc.OutputWants.Single(x => x.Want == desire.Want).Amount;
                var productNeeded = desire.Amount / outEff * inEff;
                var minProd = Math.Min(productNeeded, available);
                available -= minProd;
                var satisfaction = desire.Amount * minProd / productNeeded;
                result.AddOrInclude(tier, satisfaction);
            }

            if (available == 0)
                break;
        }

        return result;
    }

    public Dictionary<int, decimal> SatisfactionLostFrom(IProduct product, decimal amount)
    {
        var result = new Dictionary<int, decimal>();
        var removalTarget = amount;
        // start by removing from desired products
        if (DesiredProducts.Contains(product))
        {
            var validNeeds = Needs
                .Where(x => x.Product == product)
                .Where(x => x.Satisfaction != 0)
                .ToList();
            foreach (var (tier, need) in WalkDownTiersForNeeds(validNeeds,
                         validNeeds
                             .Max(x => x.SatisfactionUpToTier())))
            { // check each valid need it could come from and see how much we can remove at each level.
                var satAtTier = need.SatisfiedAtTier(tier);
                if (satAtTier > 0)
                { // if there is satisfaction to take from, subtract
                    var available = need.Amount - satAtTier * need.Amount;
                    var min = Math.Min(available, removalTarget);
                    result.AddOrInclude(tier, min);
                    removalTarget -= min; // remove from our removal target.
                }

                if (removalTarget == 0)
                    break;
            }
        }

        // reset removal target
        removalTarget = amount;
        // then remove from wants
        var possibleDesires = Wants
            .Where(x => x.Want.OwnershipSources.Contains(product) ||
                        x.Want.UseSources.Contains(product) ||
                        x.Want.ConsumptionSources.Contains(product))
            .Where(x => x.Amount > 0).ToList();
        foreach (var (tier, desire) in WalkDownTiersForWants(possibleDesires,
                     possibleDesires.Max(x => x.SatisfactionUpToTier())))
        { // go through desires
            
        }

        throw new NotImplementedException("BRB. Coming back to this function.");
        return result;
    }

    /// <summary>
    /// Adds a product to the desires safely.
    /// </summary>
    /// <param name="product"></param>
    /// <param name="p1"></param>
    public void AddProducts(IProduct product, int p1)
    {
        if (!AllProperty.ContainsKey(product))
            AllProperty[product] = new PropertyTriple(p1);
        else
            AllProperty[product].Total += p1;
    }

    public void AddWant(IWant want, int amount)
    { 
        UnclaimedWants.AddOrInclude(want, amount);
    }
}