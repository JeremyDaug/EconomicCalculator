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
    FindProduct{ product: usize, sender: ActorInfo },
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

    /// Return from seller after ActorMessage::CheckItem if they have the item
    /// in stock. returns their price and available stock.
    InStock { buyer: ActorInfo, seller: ActorInfo,
        product: usize, price: f64, quantity: f64 },
    /// Returned from seller after ActorMessage::CheckItem if they do not have
    /// the item in stock. If a deal is open, closes out the deal.
    NotInStock { buyer: ActorInfo, seller: ActorInfo, product: usize },

    /// Buyer Asks the seller for a barter hint,
    AskBarterHint { seller: ActorInfo, buyer: ActorInfo},
    /// Seller gives barter hint for the buyer to act on.
    /// Follow up tells the Buyer to expect more.
    BarterHint { seller: ActorInfo, buyer: ActorInfo, 
        product: usize, quantity: f64, followup: u64 },

    /// From Buyer to seller.
    /// 
    /// Buyer Rejects as price, unable to purchase at the current price.
    /// 
    /// It is either too expensive in Absolute terms (total budget) or 
    /// too expensive currently (current budget and can't buy).
    RejectPurchase { buyer: ActorInfo, seller: ActorInfo, product: usize,
        price_opinion: OfferResult },

    /// The start of a Buy offer from a buyer. Includes buyer, seller, and product 
    /// to make finding easy. It also includes price opinion, the amount requested,
    /// and how many followup messages to expect.
    BuyOffer { buyer: ActorInfo, seller: ActorInfo, product: usize,
        price_opinion: OfferResult, quantity: f64, followup: usize },
    /// The Followup messages for a buy offer. Includes buyer, seller, and product.
    /// Also incrludes the id of the product offered in return, it's quantity, and
    /// how many followup messages to expect. 0 means it's the last.
    BuyOfferFollowup { buyer: ActorInfo, seller: ActorInfo, product: usize,
        offer_product: usize, offer_quantity: f64, followup: usize },

    /// The Seller accepts his current deal's offer as is, replying with the
    /// buyer's offer result to simplify closeout processes.
    SellerAcceptOfferAsIs { buyer: ActorInfo, seller: ActorInfo,
        product: usize, offer_result: OfferResult },

    /// The offer has been accepted with changes. 
    /// 
    /// Includes buyer, seller, and the product being purchased.
    /// The quantity is what is being given by the buyer. If the value
    /// is the same, then the there is no reduction in items being bought.
    /// It also has followup, which defines how many followup changes 
    /// to expect.
    OfferAcceptedWithChange { buyer: ActorInfo, seller: ActorInfo,
        product: usize, quantity: f64, followups: usize },
    /// A followup message to ActorMessage::OfferAcceptedWithChange.
    /// 
    /// Used to return change to a buyer. via the return_product and return_quantity.
    /// New items may be added here for alternative re-payments, such as paying
    /// back smaller denomination items.
    /// 
    /// returned_quantity should be positive in value, even if adding a new item.
    ChangeFollowup { buyer: ActorInfo, seller: ActorInfo, 
        product: usize, return_product: usize, return_quantity: f64,
        followups: usize },

    /// The seller is rejecting the offer for whatever reason,
    /// 
    /// Most likely to be used if the seller is unwilling to accept the price offered.
    /// 
    /// Could hypethetically be used to when unable to reduce the purchase or unable 
    /// to return enough change to satisfy the seller. Rejecting being less terrible
    /// than overcharging in some cases.
    RejectOffer { buyer: ActorInfo, seller: ActorInfo, 
        product: usize },

    /// The buyer has recieved the deal (with or without change) and sends out
    /// a confirmation to close the deal for good.
    FinishDeal { buyer: ActorInfo, seller: ActorInfo, product: usize },
    
    /// A Close out Deal Message, sent by buyer or seller as needed.
    /// If sent, both sides should assume that the deal is over, don't try anything
    /// anymore. 
    CloseDeal { buyer: ActorInfo, seller: ActorInfo,
        product: usize },

    /// Follow up with a different product then last time. 
    /// Useful for pops to buy multiple items at the same store, saving time.
    /// Also uses excess time from existing item instead of next item's time.
    CheckItem { buyer: ActorInfo, seller: ActorInfo,
        proudct: usize },
    

    /// A message which sends a product from the sender to the reciever.
    /// The sender SHOULD delete their local item and the reciever SHOULD
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
    FirmToEmployee { firm: ActorInfo, employee: ActorInfo,
        action: FirmEmployeeAction },
    /// A message from an employee to a firm.
    EmployeeToFirm { employee: ActorInfo, firm: ActorInfo,
        action: FirmEmployeeAction },
    
    
}

/// Used to denote how an offer went and what the buyer felt like for it.
#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub enum OfferResult {
    /// Neutral result, primarily used for initializing deal results, but also a
    /// placeholder elsewhere.
    Incomplete,
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
    /// Special case, price is ignored and items are being requested out of generosity.
    Generous,
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
            ActorMessage::FirmToEmployee { firm: _, employee, 
                action: _ } => *employee == me, // reciever 
            ActorMessage::EmployeeToFirm { employee: _, firm, 
                action: _ } => *firm == me, // firm can recieve

            ActorMessage::SellOrder { sender: _, product: _, 
                quantity: _, amv: _ } => false,
            ActorMessage::BuyOffer { buyer: _, seller, 
            product: _, price_opinion: _, quantity: _, 
            followup: _ } => *seller == me, // sent from buyer to seller
            ActorMessage::BuyOfferFollowup { seller, ..
            } => *seller == me, // buyer to seller
                
            ActorMessage::InStock { buyer, .. } => *buyer == me,
            ActorMessage::NotInStock { buyer, .. } => *buyer == me,
            ActorMessage::AskBarterHint { seller,.. } => *seller == me,
            ActorMessage::BarterHint { buyer, .. } => *buyer == me,
            ActorMessage::SellerAcceptOfferAsIs { buyer, .. } => *buyer == me,
            ActorMessage::OfferAcceptedWithChange { buyer, .. } => *buyer == me,
            ActorMessage::ChangeFollowup { buyer, .. } => *buyer == me,

            ActorMessage::CheckItem { buyer: _, seller, 
                proudct: _ } => *seller == me,
            ActorMessage::CloseDeal { buyer, seller, 
                product: _} => *buyer == me || *seller == me, // Could be sent by either.
            ActorMessage::RejectOffer { buyer, seller: _, 
                product: _ } => *buyer == me,  // Buyer Rejects without question.
            ActorMessage::FinishDeal { buyer, seller, 
                product: _ } => *buyer == me || *seller == me,
            ActorMessage::RejectPurchase { buyer: _, seller, 
                product: _, price_opinion: _ } => *seller == me, // buyer to seller
                
        }
    }
}

/// The actions which a can be sent between firms and employees
#[derive(Debug, Clone, Copy, PartialEq, Eq)]
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