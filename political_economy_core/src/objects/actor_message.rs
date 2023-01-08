

/// Actor Message is a message which can be passed between
/// two actor threads.
#[derive(Debug, Copy, Clone)]
pub enum ActorMessage {
    /// The start message so that all actors in a market know 
    /// that all other actors are up and running and they can
    /// begin messaging back and forth.
    StartDay,
    /// Sent from the Actors when they are either out of time
    /// or they have nothing left they wish to do.
    /// When they send this, they'll enter a holding pattern,
    /// responding if proded by other Actors or the market, but with otherwise
    /// wait for the AllDone Message to arrive, letting them close out for the
    /// day.
    Finished{sender: ActorInfo},
    /// Sent by the market when all Actors have sent their Finished message.
    /// ONce this is nent
    AllFinished,
    /// The find product message, recieved by the market.
    /// Contains the product id, the amount of the item desired, and the 
    /// amount of time the actor is willing to pay for to find them.
    /// Also includes the sender and their type so
    /// a return message can be sent to them.
    FindProduct{product: usize, amount: f64, time: f64, 
        sender: ActorInfo},
    /// A message to both buyer and seller that they should
    /// meet up and try to make a deal.
    FoundProduct{seller: ActorInfo,
        buyer: ActorInfo},
    /// Returned from an attempt to buy an item and unable to
    /// find said item at all.
    /// Returns all of the information from the Find Product so the buyer can
    /// be aware that the item is unavailable.
    ProductNotFound {product: usize, buyer: ActorInfo, 
        change: f64},
}

/// Information about an actor in a nice package.
#[derive(Debug, Clone, Copy)]
pub enum ActorInfo {
    Firm(usize),
    Pop(usize),
    Institution(usize),
    State(usize),
}

impl ActorInfo {
    pub fn get_id(&self) -> usize {
        match self {
            ActorInfo::Firm(id) => *id,
            ActorInfo::Pop(id) => *id,
            ActorInfo::Institution(id) => *id,
            ActorInfo::State(id) => *id,
        }
    }
}

#[derive(Debug, Clone, Copy)]
pub enum ActorType {
    Pop,
    Firm,
    Institution,
    State,
}