using System.Reflection.PortableExecutable;
using System.Security.Claims;
using Avalonia.Animation.Animators;
using Avalonia.Input;
using Avalonia.Media.Transformation;
using Avalonia.OpenGL;
using EconomicSim.Helpers;
using EconomicSim.Objects.Market;
using EconomicSim.Objects.Processes;
using EconomicSim.Objects.Processes.ProcessTags;
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
    public readonly Dictionary<IProduct, decimal> AllProperty = new();
    /// <summary>
    /// The products which satisfy our Needs.
    /// </summary>
    public readonly Dictionary<IProduct, decimal> ProductsSatisfied = new();
    /// <summary>
    /// Products saved to satisfy wants by some means.
    /// </summary>
    public readonly Dictionary<IProduct, decimal> ProductsReserved = new();
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
    /// Product which has been expended to satisfy wants and are unavailable going forward.
    /// </summary>
    public readonly Dictionary<IProduct, decimal> ProductExpended = new();
    /// <summary>
    /// Excess wants which are available to be absorbed elsewhere.
    /// </summary>
    public readonly Dictionary<IWant, decimal> UnclaimedWants = new();
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
        FullTier = (int) DesireTier.NonTier;
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
            AllProperty[product] = owned;
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
    public int FullTier { get; private set; }
    
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
            AllProperty[product] == 0 ||
            !DesiredProducts.Contains(product))
            return; // if no available product exists or it's not desired, skip.
        // since we have product we can shift and a desire for said product, start sifting
        // shift from excess over into Satisfaction whatever we need/can
        if (ProductTargets[product] == -1)
        { // if infinite target
            var shifting = AllProperty[product];
            ProductsSatisfied[product] += shifting;
        }
        else
        { // if finite target
            var missingSat = ProductTargets[product] - ProductsSatisfied[product];
            var shifting = Math.Min(missingSat, AllProperty[product]-ProductsSatisfied[product]);
            ProductsSatisfied[product] += shifting;
        }
        // with the product shifted over, break it up into Consumed and Owned
        // get all needs for this product.
        var needs = Needs
            .Where(x => Equals(x.Product, product))
            .ToList();
        // clear out prior satisfaction values and recalculate for ease of use
        foreach (var need in needs)
            need.Satisfaction = 0;

        var unassignedProducts = ProductsSatisfied[product];

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
        if (UnclaimedWants.TryGetValue(desire.Want, out var availableWant))
        {
            var available = Math.Min(amount, availableWant);
            amount -= available;
            WantsSatisfied[desire.Want] += available;
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
                .Where(x => desire.Want.ConsumptionSources.Contains(x.Key))
                .Where(x => x.Value-ProductsReserved[x.Key] > 0)
                .Select(x => x.Key);
            foreach (var product in validProducts)
            {
                // get how efficient the product is per unit
                var wantEff = product.Wants[desire.Want];
                // get either the amount of product needed, or the amount available.
                // Should have a value above 0 available.
                var available = Math.Min(desire.Amount / wantEff, AllProperty[product] - ProductsReserved[product]);
                // reserve it
                ProductsReserved.AddOrInclude(product, available);
                // and update our satisfaction from our product
                foreach (var (satisfiedWant, rate) in product.Wants)
                {
                    var created = rate * available;
                    // add entry to satisfied wants and unclaimed wants if they aren't there.
                    if (!WantsSatisfied.ContainsKey(satisfiedWant))
                        WantsSatisfied[satisfiedWant] = 0;
                    if (!UnclaimedWants.ContainsKey(satisfiedWant))
                        UnclaimedWants[satisfiedWant] = 0;
                    if (Equals(desire.Want, satisfiedWant))
                    { // the satisfiedWant is what we are trying to satisfy
                        WantsSatisfied[satisfiedWant] += created; // add it to our satisfaction
                        desire.Satisfaction += created;
                        amount -= created; // and remove it from the amount
                    }
                    else // if not what we are trying to satisfy, save it for later possible use.
                        UnclaimedWants[satisfiedWant] += created;
                }
            }
            if (amount == 0)
                return true; // if completed amount, break out!
        }
        
        // if we get here and there is desire remaining, check for use processes
        if (desire.Want.UseSources.Any())
        {
            var useProcs = desire.Want.UseSources // from the usable products
                .Where(x => AllProperty.ContainsKey(x)) // get the items we can even use
                .Select(x => x.UseProcess!); // select their use processes (which are guaranteed to exist)
                // these processes output our want by definition (they wouldn't be in the want's use processes otherwise)
            foreach (var process in useProcs)
            { // try each process
                // get how much it outputs on average.
                var output = process.ProjectedWantAmount(desire.Want, ProcessPartTag.Output);
                var target = amount / output; // how many times we would need to do it
                var processData = process.DoProcess(target, AllProperty,
                    UnclaimedWants);
                if (processData.successes == 0)
                    continue; // if no wants can be gotten, try the next.
                // with some number possible, reserve the inputs/capital
                foreach (var (prod, change) in processData.productChange)
                {
                    // if it's consumed, add to reserve (created items shouldn't be recorded yet)
                    if (change < 0)
                    {
                        ProductsReserved.AddOrInclude(prod, change);
                    } // don't include new products for sanity purposes
                }
                // do the same with capital
                foreach (var (prod, used) in processData.productUsed)
                {
                    ProductsReserved.AddOrInclude(prod, used);
                }
                // claim any input wants also
                foreach (var (want, consumed) in processData.wantsChange.Where(x => x.Value < 0))
                { // if we're consuming a want, then claim it and remove it from unclaimed.
                    UnclaimedWants[want] -= consumed;
                    // move consumed wants into claimed wants.
                    ClaimedWants.AddOrInclude(want, consumed);
                }
                // and record that we're using this process for later uses
                UseProcesses[desire.Want][process] = processData.successes;
                // get how much we expect to be output for this
                var expectation = processData.wantsChange[desire.Want]; // this Must output the want, otherwise it's connection is false.
                WantsSatisfied[desire.Want] = expectation;
                amount -= expectation;
                if (amount == 0)
                    return true; // if no want left, breakout. 
            }
        }
        
        // lastly, try for consumption processes
        if (desire.Want.ConsumptionSources.Any())
        {
            var consumptionProcs = desire.Want.ConsumptionSources // from the usable products
                .Where(x => AllProperty.ContainsKey(x)) // get the items we can even use
                .Select(x => x.ConsumptionProcess!); // select their consumption processes (which are guaranteed to exist)
            // these processes are guaranteed to 
            foreach (var process in consumptionProcs)
            {
                // how much it outputs on average.
                var output = process.ProjectedWantAmount(desire.Want, ProcessPartTag.Output);
                var target = amount / output; // how many times we would need to do it
                var processData = process.DoProcess(target, AllProperty,
                    UnclaimedWants);
                if (processData.successes == 0)
                    continue; // if no wants can be gotten, try the next.
                // with some number possible, reserve the inputs/capital
                foreach (var (prod, change) in processData.productChange)
                {
                    // if it's consumed, add to reserve (created items shouldn't be recorded yet)
                    if (change < 0)
                    {
                        ProductsReserved.AddOrInclude(prod, change);
                    } // don't include new products for sanity purposes
                }
                foreach (var (prod, used) in processData.productUsed)
                { // do the same with capital
                    ProductsReserved.AddOrInclude(prod, used);
                }
                // claim any input wants also 
                foreach (var (want, consumed) in processData.wantsChange.Where(x => x.Value < 0))
                { // if we're consuming a want, then we must have claimed it earlier
                    UnclaimedWants[want] -= consumed;
                }
                // and record that we're using this process for later uses
                ConsumedProcesses[desire.Want][process] = processData.successes;
                // get how much we expect to be output for this
                var expectation = processData.wantsChange[desire.Want]; // this Must output the want, otherwise it's connection is false.
                WantsSatisfied[desire.Want] = expectation;
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
            if (success)
                continue;
            // since we didn't succeed, remove it.
            wantsToSatisfy.Remove(wantDesire.Want);
            // if we've run out of wants we can satisfy, break out.
            if (!wantsToSatisfy.Any())
                break;
        }
        
        // since wants always run last, update our Tier Satisfaction.
    }

    /// <summary>
    /// Completes satisfying the wants, using or consuming any products which remain.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void SatisfyWants()
    {
        throw new NotImplementedException();
    }
    
    public void UpdateFullTier()
    {/*
        FullTier = -1001;
        foreach (var need in _needs)
        {
            var tier = need.SatisfactionUpToTier();
            var satisfaction = need.SatisfiedAtTier(tier);
            if (satisfaction < 1)
                tier -= 1; // if satisfaction is incomplete at the level given, then subtract down.
            FullTier = Math.Max(FullTier, tier);
        }
        foreach (var want in _wants)
        {
            var tier = want.SatisfactionUpToTier();
            var satisfaction = want.SatisfiedAtTier(tier);
            if (satisfaction < 1)
                tier -= 1; // if satisfaction is incomplete at the level given, then subtract down.
            FullTier = Math.Max(FullTier, tier);
        }*/
    }

    public void UpdateHighestTier()
    { // not tested
        throw new NotImplementedException();
    }

    public IEnumerable<(int tier, IWantDesire desire)> WalkUpTiersForWants(IList<IWantDesire> wants)
    {
        int tier = -1001;
        Dictionary<int, List<IWantDesire>> tieredDesires = new();

        foreach (var want in wants)
        { // go through and add to the list of the tiers.
            if (tieredDesires.ContainsKey(want.StartTier))
                tieredDesires[want.StartTier].Add(want);
            else
                tieredDesires[want.StartTier] = new List<IWantDesire> {want};
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
            var nextTier = want.GetNextTier(tier);
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

    public IEnumerable<(int tier, INeedDesire desire)> WalkUpTiersForNeeds(IList<INeedDesire> needs)
    {
        int tier = -1001;
        Dictionary<int, List<INeedDesire>> tieredDesires = new();

        foreach (var need in needs)
        { // go through and add to the list of the tiers.
            if (tieredDesires.ContainsKey(need.StartTier))
                tieredDesires[need.StartTier].Add(need);
            else
            {
                tieredDesires[need.StartTier] = new List<INeedDesire> {need};
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
            var nextTier = need.GetNextTier(tier);
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
        throw new NotImplementedException();
        /*if (FullTier >= tier) return 1;

        List<IDesire> desires = new();
        foreach (var need in _needs.Where(x => x.StepsOnTier(tier)))
            desires.Add(need);
        foreach (var want in _wants.Where(x => x.StepsOnTier(tier)))
            desires.Add(want);
        // If no desires in this tier, return -1, marking it as empty
        if (!desires.Any()) return -1;

        return desires.Average(x => x.SatisfiedAtTier(tier));
        */
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
    {
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
                ProductTargets[prod] += newNeed.Amount;
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
    { // untested
        throw new NotImplementedException();
        //return _needs.Where(x => x.StepsOnTier(tier)).ToList();
    }
    
    /// <summary>
    /// Returns the products desired at the selected tier.
    /// </summary>
    /// <param name="tier">The tier selected.</param>
    /// <returns>All the products which can be satisfied at that tier.</returns>
    public IReadOnlyList<IWantDesire> WantsAtTier(int tier)
    { // untested
        throw new NotImplementedException();
        //return _wants.Where(x => x.StepsOnTier(tier)).ToList();
    }
    
    /// <summary>
    /// Gets the total amount of a specific desire at a particular tier across multiple wants.
    /// </summary>
    /// <param name="product">The want sought</param>
    /// <param name="tier">The tier we are looking at.</param>
    /// <returns>The summarized want and how satisfied it is at this tier.</returns>
    public decimal TotalNeedDesiredAtTier(IProduct product, int tier)
    { // untested
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
    /// Given a product and an amount of it, how much satisfaction do we gain.
    /// </summary>
    /// <param name="product">The product to check adding.</param>
    /// <param name="amount">The number of units to check for that item.</param>
    /// <returns>The starting tier and satisfaction gained based on that tier.</returns>
    public (int tier, decimal satisfaction) SatisfactionGainedFrom(IProduct product, decimal amount)
    { // Untested
        throw new NotImplementedException();
    }
}