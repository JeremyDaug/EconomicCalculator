namespace EconomicSim.Helpers;

public interface IDesire
{
    /// <summary>
    /// If the product requested will be consumed or not (just owned)
    /// TODO flip this later to be IsConsumed, rather than IsNotConsumed. I'm an idiot.
    /// </summary>
    bool IsConsumed { get; }
    
    /// <summary>
    /// The tier of the desire.
    /// -1000 is for the absolute minimum needed for the pop to survive (starvation ration level)
    /// -999 - -1 is space for productive needs, where the inputs/capital requirements for self-owned firms go.
    /// 0 - 999 is 'daily' needs, things which one would desire now and would give up saving to get.
    /// 1000 + is savings and Luxury desires, things which may be given up for savings.
    /// </summary>
    int StartTier { get; }
    
    /// <summary>
    /// The Tier that the desire stops being applied (only used if <see cref="Step"/> > 0).
    /// </summary>
    int? EndTier { get; }
    
    /// <summary>
    /// The amount requested
    /// </summary>
    decimal Amount { get; set; }
    
    /// <summary>
    /// The step we repeat this need in each tier.
    /// </summary>
    int Step { get; set; }
    
    /// <summary>
    /// How many steps the desire takes in total.
    /// </summary>
    int Steps { get; }
    
    /// <summary>
    /// Whether this covers multiple tiers or not.
    /// </summary>
    bool IsStretched { get; }

    /// <summary>
    /// Whether this desire is infinite or not.
    /// </summary>
    bool IsInfinite { get; }
    
    /// <summary>
    /// How many units of the Desire has been satisfied so far. Resets before daily consumption
    /// for record purposes. Not Stored in Json.
    /// </summary>
    decimal Satisfaction { get; set; }
    
    /// <summary>
    /// How many units of the desire has been reserved.
    /// Used by buyers and sellers to show how many/much has been reserved.
    /// </summary>
    decimal Reserved { get; set; }

    /// <summary>
    /// The sum of all desires at and below this tier we seek to satisfy.
    /// still returns if between tiers
    /// </summary>
    /// <param name="tier">The tier we are looking at.</param>
    /// <returns>How much we desire at that tier in total.</returns>
    decimal TotalDesireAtTier(int tier);
    
    /// <summary>
    /// Calculates how much satisfaction has been met as a percent.
    /// Greater than 1 equates to steps beyond the first having been satisfied.
    /// 1 is full satisfaction for starting tier.
    /// +1 for each level above the first satisfied.
    /// 0.XX is partial satisfaction.
    /// </summary>
    /// <returns>
    /// 1 is full satisfaction for starting tier.
    /// +1 for each level above the first satisfied.
    /// 0.XX is partial satisfaction.
    /// </returns>
    decimal TotalSatisfaction();

    /// <summary>
    /// Calculates how the total amount desired.
    /// </summary>
    /// <returns>Returns the total amount desired, if infinite it returns -1.</returns>
    decimal TotalDesire();

    /// <summary>
    /// Calculates how much of a desire has been met at a specific tier level.
    /// If tier is not valid (it's between steps) or above the stopping point it
    /// returns 1 to ensure no excess searches at higher tiers.
    /// </summary>
    /// <param name="tier">The tier we are looking at.</param>
    /// <returns>
    /// Calculates how much of a desire has been met at a specific tier level.
    /// If tier is not valid (it's between steps) or above the stopping point it
    /// returns 1 to ensure no excess searches at higher tiers.
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    decimal SatisfiedAtTier(int tier);
    
    /// <summary>
    /// Checks whether the need steps on the tier or not.
    /// </summary>
    /// <param name="tier">The tier to check.</param>
    /// <returns>True if it steps, false otherwise.</returns>
    bool StepsOnTier(int tier);

    /// <summary>
    /// Get's the next tier above the parameter. If there is none it returns -1001.
    /// </summary>
    /// <param name="tier"></param>
    /// <returns></returns>
    int GetNextTier(int tier);
}