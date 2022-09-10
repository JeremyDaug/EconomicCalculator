namespace EconomicSim.Helpers;

/// <summary>
/// A Consolidation class for Both NeedDesire and WantDesire Functionality
/// </summary>
public abstract class ADesire : IDesire
{
    public bool IsConsumed { get; set; }
    public int StartTier { get; set; }
    public int? EndTier { get; set; }
    public decimal Amount { get; set; }
    public int Step { get; set; }

    public int Steps
    {
        get
        {
            if (EndTier == null) return -1;
            return EndTier.Value - StartTier / Step;
        }
    }

    public bool IsStretched => Step > 0;
    public bool IsInfinite => !EndTier.HasValue && IsStretched;
    
    public decimal Satisfaction { get; set; }
    
    public decimal Reserved { get; set; }

    public decimal TotalDesireAtTier(int tier)
    {
        // if tier is below our starting tier, return 0.
        if (tier < StartTier) return 0;
        // get the steps this tier takes
        var currentSteps = (tier - StartTier) / Step;
        if (currentSteps > Steps) // if this oversteps, cap
            return Steps * Amount;
        return currentSteps * Amount; // otherwise just continue.
    }

    public decimal TotalSatisfaction()
    {
        return Satisfaction / Amount;
    }

    public decimal SatisfiedAtTier(int tier)
    {
        if ((tier - StartTier) % Step == 0 &&
            StartTier <= tier &&
            tier <= EndTier) 
        {// if a valid step (= Tier + n * step and between Tier and Stop)
            // If we're in here, then no remainder is possible.
            var stepCount = Decimal.Divide(tier - StartTier, Step);
            var result = TotalSatisfaction() - stepCount;
            if (result > 1) return 1; // Cap satisfaction at 1
            if (result < 0) return 0; // cap sat at 0 also
            return result; // if between 0 and 1, return the value.
        }

        return 1;
    }

    public bool StepsOnTier(int tier)
    {
        if (StartTier == tier)
        { // if the given tier is our start, we guaranteed step on it.
            return true;
        }
        if (tier < StartTier) // if the tier is before our start, we can't step on it.
            return false;
        if (IsStretched)
        { // if we cover multiple tiers
            if ((tier - StartTier) % Step == 0)
            {// if we are on the step
                if (IsInfinite)
                    return true;
                if (EndTier >= tier)
                    return true;
                return false;
            } 
            // if we aren't on the step
            return false;
        }
        // if after the start, but we aren't stretched
        return false;
    }

    public int GetNextTier(int tier)
    {
        if (tier < StartTier) // if below the start tier, then it's the start tier next.
            return StartTier;

        if ((EndTier.HasValue && tier > EndTier) || // if after our last possible tier
            !IsStretched) // if we don't have multiple tiers (we should've gotten the first)
            return (int) DesireTier.NonTier;

        // since we are stretched and after our starting point, and before our ending tier, do the math
        var next = tier + Step - ((tier - StartTier) % Step);

        if (EndTier < next) // if the next is after our end tier (EndTier should be equal to the last), return none.
            return (int) DesireTier.NonTier;
        return next;
    }
}