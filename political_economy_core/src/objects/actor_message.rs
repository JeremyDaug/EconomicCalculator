
/// Actor Message is a message which can be passed between
/// two actor threads.
#[derive(Debug, Copy, Clone)]
pub enum ActorMessage {
    /// The find product message, recieved by the market.
    /// Contains the product id and the amount of time
    /// the actor is willing to pay for to find them.
    /// Also includes the sender and their type so
    /// a return message can be sent to them.
    FindProduct{product: usize, time: f64, 
        sender: usize, sender_type: ActorType},
    /// A message to both buyer and seller that they should
    /// meet up and try to make a deal.
    FoundProduct{seller: usize, seller_type: ActorType,
        buyer:usize, buyer_type: ActorType}
}

pub enum ActorType {
    Pop,
    Firm,
    Institution,
    State,
}