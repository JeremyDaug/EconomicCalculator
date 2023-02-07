use std::collections::HashMap;

use super::actor::Actor;

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

    // Break for Deal Items

    /// An offer to the market of the specified product, in for the 
    /// specified quantity, at the specified amv unit price.
    SellOrder { sender: ActorInfo, product: usize, quantity: f64,
        amv: f64 },

    /// The find product message, recieved by the market.
    /// Contains the product id and the amount of the item desired.
    /// Also includes the sender and their type so
    /// a return message can be sent to them.
    FindProduct{ product: usize, quantity: f64, 
        sender: ActorInfo },
    /// Returned from an attempt to buy an item and unable to
    /// find said item at all.
    /// Returns all of the information from the Find Product so the buyer can
    /// be aware that the item is unavailable.
    ProductNotFound { product: usize, buyer: ActorInfo},
    /// A message to both buyer and seller that they should
    /// meet up and try to make a deal.
    /// Gives the product in question, the amount available to purchase, 
    /// the price / unit being offered, and the time left after finding it.
    /// 
    /// Starts the Deal Making Process
    FoundProduct{ seller: ActorInfo, buyer: ActorInfo, product: usize },

    /// Buyer Asks the seller for a barter hint,
    AskBarterHint { seller: ActorInfo, buyer: ActorInfo},

    BarterHint { seller: ActorInfo, buyer: ActorInfo, 
        product: usize, quantity: f64, followup: u64 },

    /// Starts an offer with only 1 item within. Send buy a buyer.
    BuyOfferOnly { buyer: ActorInfo, seller: ActorInfo, product: usize,
        price_opinion: OfferResult, quantity: f64, offer_product: usize, 
        offer_quantity: f64 },
    /// Starts a Buy offer with multiple items. This is the first and specifics 
    /// what is being bought.
    BuyOfferStart { buyer: ActorInfo, seller: ActorInfo, product: usize,
        price_opinion: OfferResult, quantity: f64, offer_product: usize, 
        offer_quantity: f64 },
    /// Middle section of a Buy Offer message chain. Includes just the item as
    /// the start dictates which item was taken.
    BuyOfferMiddle { buyer: ActorInfo, seller: ActorInfo,
        offer_product: usize, offer_quantity: f64 },
    /// End sectino of a Buy Offer message chain. Includes just the items being
    /// offered.
    BuyOfferEnd { buyer: ActorInfo, seller: ActorInfo,
        offer_product: usize, offer_quantity: f64 },

    /// A Response to the current offer. Details of the response are given by
    /// the result. Regardless, it closes out the current offer being made.
    /// Allows follow-up offers.
    OfferResponse { buyer: ActorInfo, seller: ActorInfo,
        result: OfferResult },

    /// Follow up with a different product then last time. 
    /// Useful for pops to buy multiple items at the same store, saving time.
    /// Also uses excess time from existing item instead of next item's time.
    CheckItem { buyer: ActorInfo, seller: ActorInfo,
        proudct: usize },
    /// Return from seller after ActorMessage::CheckItem if they have the item
    /// in stock. returns their price and available stock.
    InStock { buyer: ActorInfo, seller: ActorInfo,
        product: usize, price: f64, quantity: f64 },
    /// Returned from seller after ActorMessage::CheckItem if they do not have
    /// the item in stock. 
    NotInStock { buyer: ActorInfo, seller: ActorInfo },

    /// A Response to the current offer, but also closes out the deal entirely.
    /// Details of the response are given by the result. 
    OfferResponseAndCloseDeal { buyer: ActorInfo, seller: ActorInfo,
        result: OfferResult },
    /// A Close out Deal Message, sent by buyer or seller as needed.
    /// If sent, both sides should assume that the deal is over, don't try anything
    /// anymore. 
    CloseDeal { buyer: ActorInfo, seller: ActorInfo,
        product: usize, result: OfferResult },
    


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
    
    
}

/// Used to denote how an offer went and what the buyer felt like for it.
#[derive(Debug, Clone, Copy)]
pub enum OfferResult {
    /// Totally Rejected, typically by a seller who's not pleased with the offer
    /// Reduce Weight in selection -3.
    /// 
    /// Currently Not Used
    Rejected,
    /// Rejected for being too expensive, Send buy a Buyer.
    /// Reduce Weight in Selection -2
    TooExpensive,
    /// Accepted, but unhappy.
    /// Reduce Weight in selection -1.5
    Expensive,
    /// Accepted, but felt too expensive.
    /// Reduce Weight in selection -1.1
    Overpriced,
    /// Accepted, price was reasonable.
    /// Increase Weight +1.1
    Reasonable,
    /// Accepted, price was great.
    /// INcrease weight +1.5,
    Cheap,
    /// Accetped, price was effectively a steal.
    /// Increase Weight +3.
    Steal,
    /// The Seller cannot accept the offer because he is out of stock
    /// Reduce weight in selection -10
    OutOfStock,
}

impl ActorMessage {
    /// Checks whether a message is directed to whoever me is.
    /// Must actually be directed to me, not come from me.
    pub fn for_me(&self, me: ActorInfo) -> bool {
        match self {
            ActorMessage::StartDay => true,
            ActorMessage::Finished { sender } => me == *sender,
            ActorMessage::AllFinished => true,
            ActorMessage::FindProduct { .. } => false, // for market, sent by me
            ActorMessage::FoundProduct { seller, 
                buyer, .. } => {
                    *seller == me || *buyer == me
                }, // from market, created by FindProduct, you're buyer or seller.
            ActorMessage::ProductNotFound { buyer, 
                .. } => *buyer == me, // from market to buyer
            ActorMessage::SendProduct { reciever, 
                .. } => *reciever == me, // sends product to reciever
            ActorMessage::SendWant { reciever, 
                .. } => *reciever == me, // sends want to reciever
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
                offer_quantity: _,
                price_opinion: _} => *seller == me, // sent from buyer to seller
            ActorMessage::BuyOfferStart { buyer: _, seller, 
                product: _, quantity: _, offer_product: _, 
                offer_quantity: _,
                price_opinion, } => *seller == me, // buyer to seller
            ActorMessage::BuyOfferMiddle { buyer: _, seller, 
                offer_product: _, offer_quantity: _ } => *seller == me, // buyer to seller
            ActorMessage::BuyOfferEnd { buyer: _, seller, 
                offer_product: _, offer_quantity: _ } => *seller == me, // buyer to seller
            ActorMessage::OfferResponse { buyer, seller: _, 
                result: _, } => *buyer == me, // Seller to Buyer
            ActorMessage::RejectOffer { buyer, seller: _, 
                product: _ } => *buyer == me, // Seller to buyer
            ActorMessage::RejectAndCloseOffer { buyer, seller: _, 
                product: _ } => *buyer == me, // Seller to Buyer
            ActorMessage::CorrectOffer { buyer, seller: _, 
                product: _, corrected_quantity: _ } => *buyer == me, // Seller to Buyer
            ActorMessage::RejectPriceAndClose { buyer: _, 
                seller, product: _ } => *seller == me, // Buyer to Seller
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