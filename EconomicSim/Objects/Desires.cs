using EconomicSim.Helpers;
using EconomicSim.Objects.Market;
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
    private readonly List<INeedDesire> _needs = new List<INeedDesire>();
    private readonly List<IWantDesire> _wants = new List<IWantDesire>();
    
    private readonly List<INeedDesire> _stretchedNeeds = new List<INeedDesire>();
    private readonly List<IWantDesire> _stretchedWants = new List<IWantDesire>();
    
    private readonly List<INeedDesire> _infiniteNeeds = new List<INeedDesire>();
    private readonly List<IWantDesire> _infiniteWants = new List<IWantDesire>();
    
    private readonly HashSet<IProduct> _desiredProducts = new HashSet<IProduct>();
    private readonly Dictionary<IProduct, OwnConsumePair> _productConsumeVsOwn =
        new Dictionary<IProduct, OwnConsumePair>(); 
    private readonly HashSet<IWant> _desiredWants = new HashSet<IWant>();
    // placeholder spot for distinguishing between owned and consumed wants.
    // The distinction means nothing as all wants are destroyed at the end of the day regardless.

    /// <summary>
    /// The products which have been reserved to satisfy a need.
    /// </summary>
    private readonly Dictionary<IProduct, decimal> _satisfiedNeeds = new Dictionary<IProduct, decimal>();
    /// <summary>
    /// The how many products have been reserved for owning or consuming.
    /// </summary>
    private readonly Dictionary<IProduct, OwnConsumePair> _satisfiedOwnConsume =
        new Dictionary<IProduct, OwnConsumePair>();
    /// <summary>
    /// Products which have been reserved, on top of satisfied needs, for wants.
    /// </summary>
    private readonly Dictionary<IProduct, decimal> _productsForWants = new Dictionary<IProduct, decimal>();
    /// <summary>
    /// wants that are being satisfied by either products in need or productsForWants.
    /// </summary>
    private readonly Dictionary<IWant, decimal> _satisfiedWants = new Dictionary<IWant, decimal>();
    
    private readonly Dictionary<IProduct, decimal> _needTargets = new Dictionary<IProduct, decimal>();
    private readonly Dictionary<IWant, decimal> _wantTargets = new Dictionary<IWant, decimal>();
    
    private readonly Dictionary<IProduct, decimal> _excessProperty = new Dictionary<IProduct, decimal>();

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
            var newNeed = new NeedDesire(need);
            _needs.Add(newNeed);
            if (newNeed.IsStretched) // if it has steps, note that.
                _stretchedNeeds.Add(newNeed);
            if (newNeed.IsInfinite) // if it's infinite, note that.
                _infiniteNeeds.Add(newNeed);
            // add to our total list of products for easier searching.
            _desiredProducts.Add(need.Product);
            // add to our targets
            if (!_productConsumeVsOwn.ContainsKey(need.Product)) // if it doesn't already contain that product add it
                _productConsumeVsOwn[need.Product] = new();
            if (need.IsConsumed)
            {
                // TODO Fix me!
            }
        }
        foreach (var want in wants)
        { // copy over the wants so the originals aren't hurt.
            var newWant = new WantDesire(want);
            _wants.Add(newWant);
            if (newWant.IsStretched) // if it has steps, note that.
                _stretchedWants.Add(newWant);
            if (newWant.IsInfinite) // if it's infinite, note that.
                _infiniteWants.Add(newWant);
            // add to our total list of products for easier searching.
            _desiredWants.Add(want.Want);
            // add to our targets
            if (_wantTargets.ContainsKey(want.Want))
            { // if it already exists
                if (want.IsInfinite)
                { // and the need is inifite, set it to -1
                    _wantTargets[want.Want] = -1;
                }
                if (_wantTargets[want.Want] != -1)
                { // if it's not infinite and not set to infinite, add to it.
                    _wantTargets[want.Want] += want.Amount;
                }
            }
            else
            {
                if (want.IsInfinite) // if infinite, set to -1
                    _wantTargets[want.Want] = -1;
                else // if not infinite, set to the amount desired.
                    _wantTargets[want.Want] = want.Amount;
            }
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
            _excessProperty[product] = owned;
        }

        // Sift the satisfaction downwards and get our highest full satisfaction
        SiftItems();
    }

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
    /// All products desired in disorganized list.
    /// </summary>
    public IReadOnlyList<INeedDesire> Needs => _needs;

    /// <summary>
    /// All wants desired in disorganized list.
    /// </summary>
    public IReadOnlyList<IWantDesire> Wants => _wants;
    
    /// <summary>
    /// The highest tier to which these desires have been satisfied fully.
    /// </summary>
    public int FullTier { get; private set; }

    /// <summary>
    /// Takes the existing list of items and 'shakes' them to compact them down further.
    /// Starts at the bottom and works it way up. Also sets where our <see cref="FullTier"/> is.
    /// Try to run only once per day near the start if possible.
    /// </summary>
    public void SiftItems()
    {
        // reset our full tier
        FullTier = -1001;
        // clear out satisfaction to ensure consistent use
        _needs.ForEach(x => x.Satisfaction = 0);
        _wants.ForEach(x => x.Satisfaction = 0);
        // Clear our satisfied needs, wants, and productsForWants
        _satisfiedNeeds.Clear();
        _productsForWants.Clear();
        _satisfiedWants.Clear();
        // copy over excess property, and clear excess for later storage.
        // var available = new Dictionary<IProduct, decimal>(_excessProperty);
        // _excessProperty.Clear();
        // now start sifting. Start with needs to consume/use products first then satisfy wants
        // get the starting levels of all needs and order them lowest to highest.
        var orderedStarts = new SortedSet<int>(_needs
            .Select(x => x.StartTier)
            .Distinct());
        var needsToSatisfy = new List<INeedDesire>(_needs);
        while (orderedStarts.Any())
        { // go through until all needStarts are cleared
            var tier = orderedStarts.First(); // get the lowest tier
            // then get all of the needs on that tier
            var needsOnTier = needsToSatisfy
                .Where(x => x.StepsOnTier(tier));
            foreach (var need in needsOnTier)
            {
                // check that the need can be satisfied at all
                if (!_excessProperty.ContainsKey(need.Product))
                { // if it doesn't exist in the excess,
                    // remove it all needs which desire that product from the working set
                    // we'll still do any of this product in the same tier,
                    // but the effects should be minimal.
                    needsToSatisfy.RemoveAll(x => Equals(x.Product, need.Product));
                    continue; // and skip to next loop
                }
                // get the lower between the available goods and the desired amount at this tier
                var available = _excessProperty[need.Product];
                var take = Math.Min(available, need.Amount);
                // move those goods from storage to the proper category shared category and add
                _excessProperty[need.Product] -= take;
                // TODO Pick up Here!
            }

            // finish the loop by removing the tier we are currently on.
            orderedStarts.Remove(tier);
        }

    }

    /// <summary>
    /// Gets how satisfied we are at a specific desire tier.
    /// </summary>
    /// <param name="tier">The tier we are looking at.</param>
    /// <returns>The satisfaction between 0 and 1. If no desires in tier, we return -1.</returns>
    public decimal SatisfactionAtTier(int tier)
    {
        // shortcut! If FullTier is above or equal to the tier given, we can assume full satisfaction.
        if (FullTier >= tier) return 1;

        List<IDesire> desires = new();
        foreach (var need in _needs.Where(x => x.StepsOnTier(tier)))
            desires.Add(need);
        foreach (var want in _wants.Where(x => x.StepsOnTier(tier)))
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
    {
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
    }

    /// <summary>
    /// Returns the products desired at the selected tier.
    /// </summary>
    /// <param name="tier">The tier selected.</param>
    /// <returns>All the products which can be satisfied at that tier.</returns>
    public IReadOnlyList<INeedDesire> NeedsAtTier(int tier)
    {
        return _needs.Where(x => x.StepsOnTier(tier)).ToList();
    }
    
    /// <summary>
    /// Returns the products desired at the selected tier.
    /// </summary>
    /// <param name="tier">The tier selected.</param>
    /// <returns>All the products which can be satisfied at that tier.</returns>
    public IReadOnlyList<IWantDesire> WantsAtTier(int tier)
    {
        return _wants.Where(x => x.StepsOnTier(tier)).ToList();;
    }
}