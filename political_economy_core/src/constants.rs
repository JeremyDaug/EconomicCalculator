/// The ID for the required time product.
pub const TIME_ID: usize = 0;
/// the id for the required Shopping Time product.
pub const SHOPPING_TIME_ID: usize = 1;
/// The Id for the required Rest Want product.
pub const REST_WANT_ID: usize = 0;
/// The ID for the process which turns time to Shopping Time.
pub const SHOPPING_TIME_PROC_ID: usize = 0;


// These are configuration constants. Could be floated off into a
// configuration file.

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
/// Too Expensive Threshold
pub const TOO_EXPENSIVE: f64 = 1.5;
/// Expensive Threshold
pub const EXPENSIVE: f64 = 1.2;
/// Overpriced Threshold.
pub const OVERPRICED: f64 = 1.0;
/// Reasonable Threashold
pub const REASONABLE: f64 = 0.8;
/// Cheap Threshold
pub const CHEAP: f64 = 0.5;
// steal


/// ----------- Adapt future plan constants

/// The reducted weight of Loss to success rate today.
pub const LOSS_TO_SUCCESS_WEIGHT: f64 = 0.25;
/// The threshold value (0.75 - 1.0) for a major success in reaching a target.
pub const MAJOR_TARGET_SUCCESS_THRESHOLD: f64 = 0.75;
/// The threshold value (0.5 - 0.75) for normal success in reaching a target.
pub const STANDARD_TARGET_SUCCESS_THRESHOLD: f64 = 0.5;
/// The Threshold value (0.25 - 0.5) for normal failure in reaching a target.
pub const STANDARD_TARGET_FAILURE_THRESHOLD: f64 = 0.25;
// Major TargetFailure THreshold

pub const TARGET_MINIMUM_THRESHOLD: f64 = 0.1;