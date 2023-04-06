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
/// possible during purchase logic.
pub const OVERSPEND_THRESHOLD: f64 = 0.025;

/// The target price in the market a buyer will be unwilling to even attempt
/// a purchase.
pub const HARD_BUY_CAP: f64 = 2.0;
/// The correction constant which adjusts the success rate when a product was 
/// unable to be purchased
pub const UNABLE_TO_PURCHASE_REDUCTION: f64 = 0.9;
/// The Reduction Constant used when a buyer has cancelled a purchase
pub const CANCELLED_PURCHASE_REDUCTION: f64 = 0.8;

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