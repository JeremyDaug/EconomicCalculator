//! Constants file.
//! 
//! This is partially intended as a placeholder to some things. If so desired
//! this can (and should) be updated to a config file

/// The ID for the required time product.
pub const TIME_PRODUCT_ID: usize = 0;
/// # Shopping Time Id
/// 
/// The id for the required Shopping Time product.
/// 
/// Shopping time is consumed any time someone goes out to actively buy,
/// sell, or exchange goods.
/// 
/// It cannot be traded directly, only consumed by the final user.
/// 
/// Current Uses:
/// - Standard Buying Trip: 0.2 hr cost
/// 
/// ## Planned Expansion
/// 
/// Eventually, every exchange of goods will come with a handling cost
/// that scales with the bulk of the good in question. 
/// Exact rates would need to be determined, but something small for each
/// would be best, in along the lines of (Mass * [0.01] + Bulk * [0.05]). 
/// This part may need to be separated later into a 'Handling Time', which 
/// focuses on this and allows for specialized handling jobs.
/// 
/// Selling Cost. This would be similar, but much smaller compared to Standard
/// Buy trip. Used as a way to limit and encourage those who sell more limited
/// variety of goods, though this may be just small or insignificant.
/// 
/// Auctions and Reverse Auctions are the biggest addition that would be added.
/// These are bigger events, which have a singular buyer or seller state their
/// order then sellers or buyers (whichever is opposite) get together to bid on
/// the order. The one setting it up spends a decent amount of time on it,
/// while the participants spend less time comparatively.
/// 
/// Salesmanning, This is the comparable to the standard buy trip, but with the
/// person looking to sell rather than buy. Making this comparable to Standard
/// Buy is reasonable, but making it more expensive may be better to encourage
pub const SHOPPING_TIME_PRODUCT_ID: usize = 1;
/// The ID for the Skill Discernment.
pub const DISCERNMENT_PRODUCT_ID: usize = 2;

/// The Id for the required Rest Want.
pub const REST_WANT_ID: usize = 0;
/// The ID for the Required Wealth Want.
pub const WEALTH_WANT_ID: usize = 1;

/// The ID for the Brainstorming base tech.
pub const BRAINSTORMING_TECH_ID: usize = 0;

/// The ID for the process which turns time to Shopping Time.
pub const SHOPPING_TIME_PROC_ID: usize = 0;
/// The ID for the process which turns time into Liesure.
pub const RESTING_PROC_ID: usize = 1;


// These are configuration constants. Could be floated off into a
// configuration file.

/// The ratio of value between one tier and an adjacent tier.
/// 
/// IE, a unit at tier 1 is worth 0.9 of an item from tier 0.
pub const TIER_RATIO: f64 = 0.9;

/// The standard cost in time for an individual to go shopping.
pub const SHOPPING_TIME_COST: f64 = 0.2;
/// The Salability threshold for an item to be considered a currency.
pub const SALABILITY_THRESHOLD: f64 = 0.75;
/// The standard price movement step we use.
pub const STD_PRICE_CHANGE: f64 = 1.0;

/// The target for overspend we want to aim below if at all
/// possible during the buyer's purchase logic.
pub const OVERSPEND_THRESHOLD: f64 = 0.025;

/// The amount by which a seller will consider a buyer as overspending and attempt
/// to return change. Once change removed puts the purchase below this threshold it
/// returns with that change.
/// 
/// If it cannot return between 1.0 and this threshold, then it does the best
/// it can otherwise.
pub const BUYER_OVERSPENT_THRESHOLD: f64 = 1.1;

/// The target price in the market a buyer will be unwilling to even attempt
/// a purchase.
pub const HARD_BUY_CAP: f64 = 2.0;

/// The Default Salability of products in a market if they don't have one to start.
pub const DEFAULT_SALABILITY: f64 = 0.05;
/// The minimum salability an item can have in AMV calculations.
pub const MIN_SALABILITY: f64 = 0.01;

// Constants used for Product Success rate alterations.

/// The correction constant which adjusts the success rate when a product was 
/// unable to be purchased
pub const UNABLE_TO_PURCHASE_REDUCTION: f64 = 0.99;
/// The Reduction Constant used when a buyer has cancelled a purchase
pub const CANCELLED_PURCHASE_REDUCTION: f64 = 0.98;
/// The Increase Constant used when a buyer has succeeded at purchasing a product.
pub const SUCCESSFUL_PURCHASE_INCREASE: f64 = 1.01;
/// The Target Reached Bonus for product Success rate. Applied at the end of 
/// day if the amount of achieved in buying a product during a day is greater
/// than or equal to BUY_TARGET_SUCCESS_THRESHOLD
pub const TARGET_REACHED_BONUS: f64 = 1.15;
/// Applied to success_rate at the end of the day if the amount achieved is
/// less than BUY_TARGET_FAILURE_THRESHOLD.
pub const TARGET_NOT_REACHED_MALUS: f64 = 0.8;
/// The threshold rate of Achievev / Target which is considered a success 
/// for the day.
pub const BUY_TARGET_SUCCESS_THRESHOLD: f64 = 0.9;
/// The Threshold rate of Achieved / Target which is considered a failure 
/// for the day.
pub const BUY_TARGET_FAILURE_THRESHOLD: f64 = 0.7;
/// Too Expensive Threshold > 0.95
pub const TOO_EXPENSIVE: f64 = 0.95;
/// Expensive Threshold > 0.75
pub const EXPENSIVE: f64 = 0.75;
/// Overpriced Threshold 0.6
pub const OVERPRICED: f64 = 0.6;
/// Reasonable Threashold  > 0.4
pub const REASONABLE: f64 = 0.4;
/// Cheap Threshold > 0.2
pub const CHEAP: f64 = 0.2;
// steal < 0.25


// ----------- Adapt future plan constants
/// # Adapt Plan Max (Target) Growth Factor
/// 
/// When changing the max target of a good, this is how much of the difference
/// between the current max_target and the achieved Peak is added to the
/// max for next time.
pub const APC_MAX_GROWTH_FACTOR: f64 = 0.5;
/// # Adapt Plan Max (Target) Soft Reduction Factor
/// 
/// When the peak achieved is between the Min target and Max target we
/// reduce the Max by the difference * this factor (rounded up).
pub const ACP_MAX_SOFT_REDUCTION_FACTOR: f64 = 0.1;
/// # Adapt Plan Max (target) Hard Reduction Factor
/// 
/// WHen the peak achieved is below even the minimum target, we reduce the
/// max_target by the difference * this factor (rounded down).
pub const ACP_MAX_HARD_REDUCTION_FACTOR: f64 = 0.5;
/// # Adapt Plan Min (target) Reduction Factor
/// 
/// This is the factor by which the difference between total losses 
/// (consumption and lost) today is multiplied.
pub const ACP_MIN_REDUCTION_FACTOR: f64 = 0.2;


// Common Functions, may split off into separate file.

/// # Lerp
/// 
/// Takes in a start value v0, an end value v1, and a t value from 0.0 
/// to 1.0 and returns the value between v0 and v1 t percent of the way 
/// through. 
/// 
/// At t == 0, the result should be v0, at t == 1, it should be v1.
/// 
/// ## Does not panic
/// 
/// T may be outside of the interval [0.0, 1.0], but results will not be
/// guaranteed.
pub fn lerp(v0: f64, v1: f64, t: f64) -> f64 {
    (1.0 - t) * v0 + t * v1
}