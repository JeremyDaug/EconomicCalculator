/// Actor Message is a message which can be passed between
/// two actor threads.
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
    /// A message which sends a product from the sender to the reciever.
    /// THe sender SHOULD delete their local item and the reciever SHOULD
    /// add it to their property.
    SendProduct {sender: ActorInfo, reciever: ActorInfo,
        product: usize, amount: f64 },
    /// Transfers a want from one actor to another. Mostly used for
    /// firms to employees (dangerous jobs giving bad wants.)
    SendWant {sender: ActorInfo, reciever: ActorInfo,
        want: usize, amount: f64 },
    /// Takes an item and dumps it into the market/environment for it
    /// to handle. May cause additional effects.
    DumpProduct {sender: ActorInfo, product: usize, amount: f64 },
    /// Takes a want and splashes it into the market, allowing everyone to
    /// take some of it's effect. IE, A person buys security for their home
    /// a bit of that security splashes into the market, making everyone just
    /// a little bit safer.
    WantSplash {sender: ActorInfo,
        want: usize, amount: f64 },
    /// Sends a message from a firm to an employee pop, making them do 
    /// something. Primarily used to demark the start of the work day,
    /// but can also be used to send messages like hirings, firings, and
    /// job changes (promotions, demotions, transfers).
    FirmToEmployee {sender: ActorInfo, reciever: ActorInfo,
        action: FirmEmployeeAction},
    /// A message from an employee to a firm.
    EmployeeToFirm {sender: ActorInfo, reciever: ActorInfo,
        action: FirmEmployeeAction},
    
}
impl ActorMessage {
    pub fn for_me(&self, me: ActorInfo) -> bool {
        match self {
            ActorMessage::StartDay => true,
            ActorMessage::Finished { sender } => me == *sender,
            ActorMessage::AllFinished => true,
            ActorMessage::FindProduct { product: _, amount: _, time: _, 
                sender } => *sender == me, // for market, sender is me.
            ActorMessage::FoundProduct { seller, 
                buyer: _ } => *seller == me, // from market, created by FindProduct
            ActorMessage::ProductNotFound { product: _, buyer, 
                change: _ } => *buyer == me, // from market to buyer
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
                action: _ } => *reciever == me, // reciever 
            
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