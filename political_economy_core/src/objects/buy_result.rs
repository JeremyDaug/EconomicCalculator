use super::actor_message::OfferResult;

/// Message enum to 
pub enum BuyResult {
    /// Cancel the purchase, it's no longer feasable to purchase it today.
    CancelBuy, 
    /// Did not succeed in purchasing everything desired, try again.
    /// Includes the reason why.
    NotSuccessful { reason: OfferResult },
    /// The Successful
    Successful
}