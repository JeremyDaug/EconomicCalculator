use super::actor_message::OfferResult;

/// Message enum to 
pub enum BuyResult {
    /// Cancel the purchase, it's no longer feasable to purchase it today.
    CancelBuy, 
    /// Did not succeed in purchasing everything desired, try again.
    /// Includes the reason why.
    NotSuccessful { reason: OfferResult },
    /// The Seller Closed the Deal, we cannot try with them again and must
    /// look elsewhere.
    SellerClosed,
    /// The Successful
    Successful
}