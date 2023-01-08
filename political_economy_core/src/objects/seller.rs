use super::actor_message::{ActorType, ActorInfo};


/// The Seller trait denotes a struct as being capable of selling goods to the
/// market.
pub trait Seller {
    /// Get the the Actor Type for the seller.
    fn actor_type(&self) -> ActorType;
    /// Gets the Actor Info (type and Id).
    fn actor_info(&self) -> ActorInfo;
    /// get the seller's id.
    fn get_id(&self) -> usize;
}