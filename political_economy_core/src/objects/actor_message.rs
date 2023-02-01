use std::collections::HashMap;

/// Actor Message is a message which can be passed between
/// two actor threads.
/// 
/// # Complex Buy Offers
/// 
/// Because Messages cannot pass collections safely, instead we pass chains
/// off messages. there are 4 offer messages. 
/// 
/// 1 for singular item offers, so we can have a shorthand.
/// 
/// 3 for chains of items to offer, one open, one next, one close.
/// 
/// # Future Options
/// 
/// TODO
/// 
/// May be worth it to break some of these messages out, specifically move most inter-actor
/// messages or offer messages to another enum and consolidate them into a more common
/// message type for here. May do that later, not sure.
#[derive(Debug, Clone, Copy)]
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
    Finished{ sender: ActorInfo },
    /// Sent by the market when all Actors have sent their Finished message.
    /// ONce this is nent
    AllFinished,
    /// The find product message, recieved by the market.
    /// Contains the product id, the amount of the item desired, and the 
    /// amount of time the actor is willing to pay for to find them.
    /// Also includes the sender and their type so
    /// a return message can be sent to them.
    FindProduct{ product: usize, amount: f64, time: f64, 
        sender: ActorInfo },
    /// A message to both buyer and seller that they should
    /// meet up and try to make a deal.
    /// Gives the product in question, the amount available to purchase, 
    /// and the time left after finding it.
    FoundProduct{ seller: ActorInfo, buyer: ActorInfo, product: usize,
        quantity: f64, time_change: f64 },
    /// Returned from an attempt to buy an item and unable to
    /// find said item at all.
    /// Returns all of the information from the Find Product so the buyer can
    /// be aware that the item is unavailable.
    ProductNotFound { product: usize, buyer: ActorInfo, 
        time_remaining: f64},
    /// A message which sends a product from the sender to the reciever.
    /// THe sender SHOULD delete their local item and the reciever SHOULD
    /// add it to their property.
    SendProduct { sender: ActorInfo, reciever: ActorInfo,
        product: usize, amount: f64 },
    /// Transfers a want from one actor to another. Mostly used for
    /// firms to employees (dangerous jobs giving bad wants.)
    SendWant { sender: ActorInfo, reciever: ActorInfo,
        want: usize, amount: f64 },
    /// Takes an item and dumps it into the market/environment for it
    /// to handle. May cause additional effects.
    DumpProduct { sender: ActorInfo, product: usize, amount: f64 },
    /// Takes a want and splashes it into the market, allowing everyone to
    /// take some of it's effect. IE, A person buys security for their home
    /// a bit of that security splashes into the market, making everyone just
    /// a little bit safer.
    WantSplash { sender: ActorInfo,
        want: usize, amount: f64 },
    /// Sends a message from a firm to an employee pop, making them do 
    /// something. Primarily used to demark the start of the work day,
    /// but can also be used to send messages like hirings, firings, and
    /// job changes (promotions, demotions, transfers).
    FirmToEmployee { sender: ActorInfo, reciever: ActorInfo,
        action: FirmEmployeeAction },
    /// A message from an employee to a firm.
    EmployeeToFirm { sender: ActorInfo, reciever: ActorInfo,
        action: FirmEmployeeAction },
    
    /// An offer to the market of the specified product, in for the 
    /// specified quantity, at the specified amv unit price.
    SellOrder { sender: ActorInfo, product: usize, quantity: f64,
        amv: f64 },
    /// Buy offer with only 1 item within.
    BuyOfferOnly { buyer: ActorInfo, seller: ActorInfo, product: usize,
        quantity: f64, offer_product: usize, offer_quantity: f64 },
    /// Buy offer with multiple items. This is the first and specifics 
    /// what is being bought.
    BuyOfferStart { buyer: ActorInfo, seller: ActorInfo, product: usize,
        quantity: f64, offer_product: usize, offer_quantity: f64 },
    /// Middle section of a Buy Offer message chain. Includes just the item as
    /// the start dictates which item was taken.
    BuyOfferMiddle { buyer: ActorInfo, seller: ActorInfo,
        offer_product: usize, offer_quantity: f64 },
    /// End sectino of a Buy Offer message chain. Includes just the items being
    /// offered.
    BuyOfferEnd { buyer: ActorInfo, seller: ActorInfo,
        offer_product: usize, offer_quantity: f64 },
    /// The Buy offer has been accepted by the seller.
    AcceptOffer { buyer: ActorInfo, seller: ActorInfo, product: usize },
    /// The Buy offer has been rejected by the seller, but a new offer will 
    /// be allowed.
    RejectOffer { buyer: ActorInfo, seller: ActorInfo, product: usize },
    /// The offer has been rejected and closed, the seller does not
    /// want to or cannot deal with the buyer again today.
    RejectAndCloseOffer { buyer: ActorInfo, seller: ActorInfo, product: usize },
    /// The offer has been rebuffed, but only to give the buyer more information.
    /// This is used for when the buyer is asking for more than the seller has.
    CorrectOffer { buyer: ActorInfo, seller: ActorInfo, product: usize, 
        corrected_quantity: usize },
}

impl ActorMessage {
    /// Checks whether a message is directed to whoever me is.
    /// Must actually be directed to me, not come from me.
    pub fn for_me(&self, me: ActorInfo) -> bool {
        match self {
            ActorMessage::StartDay => true,
            ActorMessage::Finished { sender } => me == *sender,
            ActorMessage::AllFinished => true,
            ActorMessage::FindProduct { product: _, amount: _, time: _, 
                sender: _, } => false, // for market, sent by me
            ActorMessage::FoundProduct { seller, 
                buyer, product: _, quantity: _, time_change: _ } => {
                    *seller == me || *buyer == me
                }, // from market, created by FindProduct, you're buyer or seller.
            ActorMessage::ProductNotFound { product: _, buyer, 
                time_remaining: _ } => *buyer == me, // from market to buyer
            ActorMessage::SendProduct { sender: _, reciever, 
                product: _, amount: _ } => *reciever == me, // sends product to reciever
            ActorMessage::SendWant { sender: _, reciever, 
                want: _, amount: _ } => *reciever == me, // sends want to reciever
            ActorMessage::DumpProduct { sender: _, 
                product: _, amount: _ } => false, // dupms product onto market
            ActorMessage::WantSplash { sender: _, want: _, 
                amount: _ } => true, // dumps want into market, hits everyone.
            ActorMessage::FirmToEmployee { sender: _, reciever, 
                action: _ } => *reciever == me, // reciever 
            ActorMessage::EmployeeToFirm { sender: _, reciever, 
                action: _ } => *reciever == me, // firm can recieve

            ActorMessage::SellOrder { sender: _, product: _, 
                quantity: _, amv: _ } => false,
            ActorMessage::BuyOfferOnly { buyer: _, seller, 
                product: _, quantity: _, offer_product: _, 
                offer_quantity: _ } => *seller == me,
            ActorMessage::BuyOfferStart { buyer: _, seller, 
                product: _, quantity: _, offer_product: _, 
                offer_quantity: _ } => *seller == me,
            ActorMessage::BuyOfferMiddle { buyer: _, seller, 
                offer_product: _, offer_quantity: _ } => *seller == me,
            ActorMessage::BuyOfferEnd { buyer: _, seller, 
                offer_product: _, offer_quantity: _ } => *seller == me,
            ActorMessage::AcceptOffer { buyer, seller: _, 
                product: _ } => *buyer == me,
            ActorMessage::RejectOffer { buyer, seller: _, 
                product: _ } => *buyer == me,
            ActorMessage::RejectAndCloseOffer { buyer, seller: _, 
                product: _ } => *buyer == me,
            ActorMessage::CorrectOffer { buyer, seller: _, 
                product: _, corrected_quantity: _ } => *buyer == me,
        }
    }
}

/// The actions which a can be sent between firms and employees
#[derive(Debug, Clone, Copy)]
pub enum FirmEmployeeAction {
    /// Work day has finished and gotten what it needs from it's pops, move 
    /// along.
    WorkDayEnded,
    /// Requests time from the firm, simplifies the transfer as it's common.
    RequestTime,
    /// Used by Disorganized firms or for owners without limited liability
    /// to take everything from the pop.
    RequestEverything,
    /// Requests all of a specific item from the pop.
    RequestItem {product: usize},
    /// Used by one to tell the other that they have completed sending the
    /// requested item(s). Intended primarily to end 
    /// FirmEmployeeAction::RequestEverything logic.
    RequestSent,

    // TODO consider removing these.
    /// The firm has hired more people into this pop.
    Hire,
    /// The firm has fired a people from this pop.
    Fire,
    /// The firm is moving some members of this pop elsewhere in
    /// the firm, may be promotion or demotion.
    TransferTo,
}

/// Information about an actor in a nice package.
#[derive(Debug, Clone, Copy, PartialEq, Eq)]
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

    pub fn is_pop(&self) -> bool {
        match self {
            ActorInfo::Pop(_) => true,
            _ => false,
        }
    }

    pub fn is_firm(&self) -> bool {
        match self {
            ActorInfo::Firm(_) => true,
            _ => false,
        }
    }

    pub fn is_institution(&self) -> bool {
        match self {
            ActorInfo::Institution(_) => true,
            _ => false,
        }
    }

    pub fn is_state(&self) -> bool {
        match self {
            ActorInfo::State(_) => true,
            _ => false,
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