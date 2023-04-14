//! The storage unit of population groups.
//! 
//! Used for any productive, intellegent actor in the system. Does not include animal
//! populations.
use std::{collections::{VecDeque, HashMap}};

use barrage::{Sender, Receiver};

use crate::{demographics::Demographics, data_manager::DataManager, constants::{SHOPPING_TIME_COST, EXPENSIVE, TOO_EXPENSIVE, REASONABLE, OVERPRICED, CHEAP, OVERSPEND_THRESHOLD, TIME_ID, self}, objects::pop_memory::Knowledge};

use super::{desires::Desires, 
    pop_breakdown_table::PopBreakdownTable, 
    buyer::Buyer, seller::Seller, actor::Actor, 
    market::MarketHistory, 
    actor_message::{ActorMessage, ActorType, ActorInfo, FirmEmployeeAction, OfferResult}, 
    pop_memory::PopMemory, product::ProductTag, buy_result::BuyResult, 
};

/// Pops are the data storage for a population group.
/// 
/// Population groups are defines externally by what
/// market they are in, what firm they work in, and
/// what their job in that firm is.
/// 
/// Internally they are broken appart by the various of the
/// pop. It breaks them into a table to record details of how many are in each species/culture combo.
#[derive(Debug)]
pub struct Pop {
    /// Pop's unique id for navigation purposes.
    pub id: usize,
    /// The job of the pop.
    pub job: usize,
    /// Where the pop works.
    pub firm: usize,
    /// Which market they are in
    pub market: usize,
    /// The skill the pop uses.
    pub skill: usize,
    /// The lower bound of their skill level.
    pub lower_skill_level: f64,
    /// the upper bound of their skill level spread.
    pub higher_skill_level: f64,
    /// The total desires and property of the pop.
    pub desires: Desires,
    /// A breakdown of the Population's demographics.
    pub breakdown_table: PopBreakdownTable,
    // Mood
    /// Whether the pop is selling or not.
    pub is_selling: bool,
    /// The historical records (or rough estimate thereof).
    pub memory: PopMemory,
    /// Backlogs of messages, to help keep things clear.
    pub backlog: VecDeque<ActorMessage>,
}

impl Pop {
    /// Takes the current population table, and updates desires to match the population
    /// breakdown. This is a hard reset, so is advised to call only as needed.
    /// 
    /// Does not take sub-groups of species, culture, ideology into account currently.
    /// This will need to be updated when those are implemented.
    pub fn update_desires(&mut self, demos: Demographics) {
        // TODO when subgroups are added to these items, this will need to be updated to take them into account.
        self.desires.clear_desires();
        // add in each species desires
        for row in self.breakdown_table.species_makeup().iter() {
            let species = demos.species.get(row.0).expect("Species Id Not Found!");
            for desire in species.desires.iter() {
                let upped_desire = desire.create_multiple(*row.1);
                self.desires.add_desire(&upped_desire);
            }
        }
        // placeholder for civilization
        // add in culture desires
        for row in self.breakdown_table.culture_makeup().iter() {
            if let Some(id) = row.0 {
                let culture = demos.cultures.get(id).expect("Culture Id Not Found!");
                for desire in culture.desires.iter() {
                    let upped_desire = desire.create_multiple(*row.1);
                    self.desires.add_desire(&upped_desire);
                }
            }
        }

        // add in ideology desires
        for row in self.breakdown_table.ideology_makeup().iter() {
            if let Some(id) = row.0 {
                let ideology = demos.ideology.get(id).expect("Ideology Id Not Found!");
                for desire in ideology.desires.iter() {
                    let upped_desire = desire.create_multiple(*row.1);
                    self.desires.add_desire(&upped_desire);
                }
            }
        }

        // add in movements
    }

    /// Get's an automatically generated name for the pop group.
    /// 
    /// TODO update to pass in data from elsewhere to get more useful names.
    /// Possibly add in an option no name them specially.
    pub fn id_name(&self) -> String {
        format!("Job:{}|Firm:{}|Market:{}", self.job, self.firm, self.market)
    }

    /// Get's the total number of people in this pop.
    pub fn count(&self) -> usize {
        self.breakdown_table.total
    }

    /// A helper function to push a message to the market.
    /// Safely pushes without blocking.
    /// 
    /// Tries to send, if fails it reads one message, and if it's for us, puts it on the
    /// backlog. Then tries again, until it succeeds or the channel breaks.
    /// 
    /// ## Panics
    /// 
    /// If the send fails due to a disconnect.
    pub fn push_message(&mut self, rx: &Receiver<ActorMessage>, tx: &Sender<ActorMessage>, 
    msg: ActorMessage) {
        loop {
            let result = tx.try_send(msg);
            if let Ok(_) = result {
                break; // if message got sent out, break.
            }
            else if let Err(msg) = result {
                match msg { // failed to send, check why
                    // If disconnected, panic, there's nothing more we can do.
                    barrage::TrySendError::Disconnected(_) => panic!("Unexpected Disconnect"), 
                    barrage::TrySendError::Full(_) => (), // if just full, consume and try again.
                }
            }
            // if we get here, the queue is blocked, so read and if it's
            // for us, put it on the back burner.
            self.msg_catchup(rx);
        };
    }

    /// A shorthand function. 
    /// 
    /// Quickly consumes all messages from the queue it can, catching up
    /// with the current back of the queue.
    /// 
    /// If it finds something for us, it puts it in the backlog for later consumption.
    /// 
    /// This focuses on keeping the Broadcast Queue open to ensure it doesn't get backed
    /// up too much.
    pub fn msg_catchup(&mut self, rx: &Receiver<ActorMessage>) {
        loop {
            let result = rx.try_recv()
                .expect("Unexpected Disconnect"); // if disconnected, panic.

            if let Some(msg) = result { // if we recieved a message, check it's for us
                if msg.for_me(self.actor_info()) {
                    self.backlog.push_back(msg); // if it's for us, push it to the backlog.
                }
            }
            else { // if no messsage in queue, we've caught up so break out.
                return;
            }
        }
    }

    /// A shorthand function to recieve the next message from the queue for us.
    /// It returns it to us instead of putting it in the backlog.
    pub fn get_next_message(&self, rx: &Receiver<ActorMessage>) -> ActorMessage {
        loop {
            let msg = rx.recv().expect("Unexpected Disconnect.");
            if msg.for_me(self.actor_info()) {
                return msg;
            }
        }
    }

    /// Send Buy Offer
    /// 
    /// Small helper function to simplify sending our purchase offers.
    pub fn send_buy_offer(&mut self, rx: &Receiver<ActorMessage>, tx: &Sender<ActorMessage>,
    product: usize, seller: ActorInfo, offer: &HashMap<usize, f64>, offer_result: OfferResult, target: f64) {
        let mut offer_len = offer.len();
        self.push_message(rx, tx, ActorMessage::BuyOffer { buyer: self.actor_info(), seller, 
            product, price_opinion: offer_result, quantity: target,
            followup: offer_len });
        for (offer_item, offer_quantity) in offer.iter() {
            offer_len -= 1;
            self.push_message(rx, tx, ActorMessage::BuyOfferFollowup { buyer: self.actor_info(), seller, 
                product, offer_product: *offer_item, offer_quantity: *offer_quantity, followup: offer_len })
        }
    }

    /// Active Wait Function, used whenever we need to wait for a particular 
    /// result or message. Takes in all the standard stuff for free time, while
    /// also taking in whatever it's looking to find. If it recieves one of the
    /// message types requested, it returns it.
    /// 
    /// If it gets a message other than what it's looking for, it deals with it
    /// via the process_common_message. 
    /// 
    /// This only returns if it recieves the message it's looking for, it it does 
    /// not recieve it, it will be stuck in it's loop.
    pub fn active_wait(&mut self, 
    rx: &mut Receiver<ActorMessage>, 
    tx: &Sender<ActorMessage>, 
    data: &DataManager, 
    market: &MarketHistory, 
    keep: &mut HashMap<usize, f64>,
    spend: &mut HashMap<usize, f64>,
    returned: &mut HashMap<usize, f64>,
    find: &Vec<ActorMessage>) -> ActorMessage {
        loop {
            // catchup on messages for good measure
            self.msg_catchup(rx);
            // next deal with the first backlog
            let popped = self.backlog.pop_front();
            if let Some(msg) = popped {
                if find.iter()
                .any(|x| std::mem::discriminant(x) == std::mem::discriminant(&msg)) {
                    return msg;
                }
                else {
                    self.process_common_msg(rx, tx, data, market, msg, keep, spend, returned);
                }
            }
        }
    }

    /// Specific wait function.
    /// 
    /// Waits on a specific message or messages to be recieved directed for us.
    /// If it's any other message for us, it's put onto the backlog.
    /// 
    /// Meant to be used primarily when we are locked into a state where we 
    /// shouldn't respond to anything else but what we're focusing on.
    /// 
    /// May be improved by making find work with incomplete ActorMessages or some
    /// other mechanism that removes the need to created dummies to make it work.
    pub fn specific_wait(&mut self,
    rx: &Receiver<ActorMessage>,
    find: &Vec<ActorMessage>) -> ActorMessage {
        // TODO Look into improving Find Parameter so it doesn't need a fully filled out ActorMessage to function.
        loop {
            let msg = self.get_next_message(rx);
            if find.iter()
            .any(|x| std::mem::discriminant(x) == std::mem::discriminant(&msg)) {
                return msg;
            }
            else {
                self.backlog.push_back(msg);
            }
        }
    }

    /// Processes firm messages for standard day work. 
    /// 
    /// Returns true if the workday has ended.
    pub fn process_firm_message(&mut self, 
    rx: &Receiver<ActorMessage>, 
    tx: &Sender<ActorMessage>, 
    firm: ActorInfo, 
    action: FirmEmployeeAction) -> bool {
        match action {
            FirmEmployeeAction::WorkDayEnded => return true, // work day over, we can move on.
            FirmEmployeeAction::RequestTime => {
                // send over our work time
                self.push_message(rx, tx, ActorMessage::SendProduct { sender: self.actor_info(),
                    reciever: firm, 
                    product: TIME_ID, 
                    amount: self.memory.work_time
                    });
                // and remove that time from our property as well
                self.desires.property.entry(TIME_ID)
                .and_modify(|x| *x -= self.memory.work_time );
            },
            FirmEmployeeAction::RequestEverything => {
                // loop over everything and send it to the firm.
                let mut to_move = HashMap::new();
                for (product, amount) in self.desires.property.iter() {
                    to_move.insert(*product, *amount);
                }
                for (product, amount) in to_move {
                    self.push_message(rx, tx, 
                    ActorMessage::SendProduct { 
                        sender: self.actor_info(), 
                        reciever: firm, 
                        product,
                        amount 
                    });
                    self.desires.property.remove(&product)
                    .expect("Not found?");
                }
                // also send over the wants
                let mut to_move = HashMap::new();
                for (want, amount) in self.desires.want_store.iter() {
                    to_move.insert(*want, *amount);
                }
                for (want, amount) in to_move {
                    self.push_message(rx, tx, 
                    ActorMessage::SendWant { 
                        sender: self.actor_info(), 
                        reciever: firm, 
                        want,
                        amount 
                    });
                    self.desires.want_store.remove(&want)
                    .expect("Not found?");
                }
                // Tell the firm we've sent everything to them and they can continue on.
                self.push_message(rx, tx, ActorMessage::EmployeeToFirm { 
                    employee: self.actor_info(), 
                    firm, 
                    action: FirmEmployeeAction::RequestSent });
            },
            FirmEmployeeAction::RequestItem { product } => {
                // firm is requesting a specifc item, send it to them, 
                // if we don't have it, then send the empty anyway.
                let amount = match self.desires.property.remove(&product) {
                    Some(amount) => amount,
                    None => 0.0,
                };
                self.push_message(rx, tx, 
                ActorMessage::SendProduct { sender: self.actor_info()       , 
                    reciever: firm, 
                    product, 
                    amount 
                }); // no need to send more
            },
            _ => ()
        }
        false
    }

    /// Work Day Processor.
    /// 
    /// During this function, the pop focuses on completing the work day. They don't act within the
    /// market, instead focusing on reacting to their workplace.
    /// 
    /// Consumes Want Splashes (as they are easy to deal with), SendProduct messages directed towards
    /// the pop. It also waits for messages from the firm.
    /// 
    /// All other messages are added to the backlog for later.
    pub fn work_day_processing(&mut self, rx: &mut Receiver<ActorMessage>, tx: &Sender<ActorMessage>) {
        loop {
            // It's working time, so focus on the firm, don't worry about caluclating more
            // just block on recieving until we know we've given/gotten everything we need to
            // give.
            let msg = rx.recv().expect("Unexpectedly Closed.");
            // check that it's for us.
            if !msg.for_me(self.actor_info()) {
                continue; // if not try again.
            }
            // if the message is for me, processes it
            match msg {
                ActorMessage::WantSplash { sender: _, want, amount } => {
                    // catch any splashed wants for a while.
                    *self.desires.want_store.entry(want).or_insert(0.0) += amount;
                },
                ActorMessage::SendProduct { 
                    sender: _, 
                    reciever: _, 
                    product, 
                    amount } => {
                        *self.desires.property.entry(product)
                        .or_insert(0.0) = amount;
                },
                ActorMessage::FirmToEmployee { firm: sender, 
                employee: _, action } => {
                    if self.process_firm_message(rx, tx, sender, action) {
                        break;
                    }
                },
                _ => { // everything else, push to the backlog for later
                    self.backlog.push_back(msg);
                },
            }
        }
    }

    /// Goes through the free time that the pop has available to it.
    /// 
    /// Free time is spent primarily on buying stuff and organizing their
    /// property.
    /// 
    /// They'll focus most of their early effort on shopping. This means 
    /// looking at what they target themselves having, then looking at what
    /// they are missing. Whatever they're missing they'll attempt to buy. Whatever                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 
    /// they've already got, they'll reserve.
    /// 
    /// Before they go anywhere, they'll split their property into 3 camps. Keep, Spend, and 
    /// Spare. 
    /// 
    /// Keep is all the stuff they know they'll need and will refuse to give up 
    /// without external force. 
    /// 
    /// Spend are those things they are willing to give up, either having no use for 
    /// them, or having enough of them to overflow their desires.
    /// 
    /// Spare is the inbetween of the two, things that they want, but would be willing to give
    /// up for other things. Spare goods are those goods which are desired, but have other desires
    /// below them.
    /// 
    /// Keep and spend are defined/recorded by self.memory rather than calculated, and corrected
    /// as successes or failure comes in.
    /// 
    /// ## Not Tested due to complexity.
    fn free_time(&mut self, rx: &mut Receiver<ActorMessage>, tx: &Sender<ActorMessage>, 
    data: &DataManager,
    market: &MarketHistory) {
        // start by splitting property up into keep, and spend;
        let mut keep: HashMap<usize, f64> = HashMap::new();
        let mut spend: HashMap<usize, f64> = HashMap::new();
        let mut change: HashMap<usize, f64> = HashMap::new();
        // get what we remember over
        for (id, know) in self.memory.product_knowledge.iter_mut() {
            let available = *self.desires.property.get(id).unwrap_or(&0.0); // get how much we have
            let capped = available.min(know.target); // cap at our target to keep
            if available != capped {
                // if there will be some available leftover after min, then add to spend
                spend.insert(*id, available - capped);
            }
            // what we want to keep, add to keep
            keep.insert(*id, capped);
            // and record the amount we kept as also being achieved already and record rollover as well
            know.achieved += capped;
            know.rollover += capped;
        }
        // repeat for the items which we don't have any memory for so we can offer them up.
        for (id, excess) in self.desires.property
        .iter().filter(|x| !keep.contains_key(x.0)) {
            spend.insert(*id, *excess);
        }
        // with that done, put our spend stuff up for sale, if we are selling
        if self.is_selling {
            for (id, amount) in spend.iter() {
                let info = data.products.get(id).expect("Product Not Found!");
                // if nontransferable, don't offer for sale.
                if info.tags.contains(&ProductTag::NonTransferrable) { continue; }
                // since it can be sold, offer it up.
                self.push_message(rx, tx, 
                    ActorMessage::SellOrder { sender: self.actor_info(),
                    product: *id, quantity: *amount, 
                    amv: market.get_product(id).price });
                // private sellers offer at current estimated market price.
            }
        }
        // TODO Consider waiting here for a shopping day start msg here, if the buyers outpace the sellers too much. This may occur if most sellers are selling pops.
        // enter a loop and work on buying up to our targets and 
        // expending our time while handling any orders coming our way.
        let mut curr_buy_idx = 0;
        let shopping_time_cost = self.standard_shop_time_cost();
        loop {
            // if time to spend has been used up, break out.
            // TODO update to use the smalest possble expendable unit of time instead of 0.0.
            if *spend.get(&0).unwrap_or(&0.0) < shopping_time_cost { break; }
            // catch up with the broadcast queue to help keep the queue clear.
            // don't actually process, just push anything for us into the backlog.
            self.msg_catchup(rx);
            // now clear out the backlog messages
            while let Some(msg) = self.backlog.pop_front() {
                self.process_common_msg(rx, tx, data, market, msg,
                    &mut keep, &mut spend, &mut change);
                // if this last processed message ate all our time, then gtfo and move
                // to the end of day holding pattern.
                if *spend.get(&0).unwrap_or(&0.0) <= 0.0 { break; }
            }
            // With the backlog caught up, do whatever we need want to do here

            // buy stuff first, and repeat until we have tried buying everything we can.
            if self.memory.product_priority.len() > curr_buy_idx { // if anything left to buy
                let product = *self.memory.product_priority
                    .get(curr_buy_idx).expect("Product not found?");
                let buy_result = self.try_to_buy(rx, tx, data, market, &mut keep, &mut spend, 
                    &mut change, &product);
                // with our stuff bought, jump back to the top and do more.
                match buy_result {
                    BuyResult::CancelBuy => { // if we cancelled
                        curr_buy_idx += 1; // add to the current buy index and go to the next one.
                        continue;
                    },
                    _ => { // for anything but cancellation from ourselves
                        // check if we still have time to try again
                        if self.memory.product_knowledge.get(&product).unwrap().remaining_time() > shopping_time_cost {
                            continue;
                        } else { // we need to move on.
                            curr_buy_idx += 1;
                            continue;
                        }
                    },
                }
            }

            // with nothing on our list left to buy, check if we have enough to do more
        }
    }

    /// Processes common messages from the ActorMessages for current free time.
    /// Function assumes that msg is for us, so be sure to collect just those.
    fn process_common_msg(&mut self, rx: &mut Receiver<ActorMessage>, 
    tx: &Sender<ActorMessage>, data: &DataManager, 
    market: &MarketHistory, msg: ActorMessage, keep: &mut HashMap<usize, f64>,
    spend: &mut HashMap<usize, f64>, returned: &mut HashMap<usize, f64>) {
        match msg {
            ActorMessage::FoundProduct { seller, buyer, 
            product } => {
                if buyer == self.actor_info() {
                    panic!("Product Found message with us as the buyer should not be found outside of deal state.");
                } else if seller == self.actor_info() {
                    self.buyer_approaches(rx, tx, data, market, keep, spend, product, buyer);
                } else {
                    panic!("How TF did we get here? We shouldn't have recieved this FoundProduct Message!");
                }
            },
            ActorMessage::SendProduct { product, amount, .. } => {
                // we're recieving a product, so check if we desire it. if we do, add it to keep and property
                if let Some(prod_mem) = self.memory.product_knowledge.get_mut(&product) {
                    prod_mem.achieved += amount;
                    *self.desires.property.entry(product).or_insert(0.0) += amount;
                    let remaining_target = prod_mem.target_remaining();
                    let to_target = remaining_target.min(amount);
                    let to_spend = amount - to_target;
                    *spend.entry(product).or_insert(0.0) += to_spend;
                    *keep.entry(product).or_insert(0.0) += to_target;
                } else { // if we have no memory of it, add it to memory and our spend pile
                    let prod_mem = self.memory.product_knowledge.entry(product).or_insert(Knowledge::new());
                    prod_mem.achieved += amount;
                    *self.desires.property.entry(product).or_insert(0.0) += amount;
                    *spend.entry(product).or_insert(0.0) += amount;
                }
            },
            ActorMessage::SendWant { want, amount, .. } => {
                *self.desires.want_store.entry(want).or_insert(0.0) += amount;
            },
            ActorMessage::WantSplash { want, amount, .. } => {
                *self.desires.want_store.entry(want).or_insert(0.0) += amount;
            },
            ActorMessage::FirmToEmployee { firm, employee: _,
            action } => {
                self.process_firm_message(rx, tx, firm, action);
            },
            _ => panic!("Recieved Bad msg.")
        }
    }

    /// ## Try to buy
    /// 
    /// Tries to setup a buy deal through normal means.
    /// 
    /// Starts by sending the FindProduct request, then actively waits for 
    /// a response, consuming and processing messages for itself until it 
    /// recieves ProductFound or ProductNotFound.
    /// 
    /// If the product is found, it will enter the deal state, and try to buy tho item.
    /// 
    /// Both the buyer and seller will be locked into the deal state once they 
    /// send/recieve the ProductFound message.
    /// 
    /// If the product is not found, but the desire is considered high priority
    /// then it will go into an emergency search. 
    /// 
    /// It has 2 options when it starts.
    /// - Standard Search, it asks the market to find a guaranteed seller,
    /// who we'll get in touch with and try to make a deal.
    /// - Emergency Search, this occurs when either the product being sought 
    /// is unavailable through sellers and the product sought is important
    fn try_to_buy(&mut self, 
    rx: &mut Receiver<ActorMessage>, 
    tx: &Sender<ActorMessage>, 
    data: &DataManager, 
    market: &MarketHistory, 
    keep: &mut HashMap<usize, f64>,
    spend: &mut HashMap<usize, f64>,
    returned: &mut HashMap<usize, f64>,
    product: &usize) -> BuyResult {
        // get time cost for later
        let time_cost = self.standard_shop_time_cost();
        // get the amount we want to get and the unit price budget.
        let mem = self.memory.product_knowledge
        .get_mut(product).expect("Product not found?");
        let price = mem.current_unit_budget();
        // with budget gotten, check if it's feasable for us to buy (market price < 2.0 budget)
        let market_price = market.get_product_price(product, 0.0);
        if market_price > (price * constants::HARD_BUY_CAP) {
            mem.cancelled_purchase(); // if unfeaseable, at current market price, cancel.
            return BuyResult::CancelBuy;
        }
        // subtract the time from our stock and add it to time spent
        // TODO Consider updating this to scale not just with population buying but also the 
        // TODO cheat and just subtract from time right now, this should subtract from Shopping_time not normal time.
        *self.desires.property.get_mut(&constants::TIME_ID).unwrap() -= time_cost;
        mem.time_spent += time_cost;
        // TODO update self.breakdown.total to instead use 'working population' instead of total to exclude dependents.
        *spend.get_mut(&0).unwrap() -= SHOPPING_TIME_COST * self.breakdown_table.total as f64;
        // since the current market price is within our budget, try to look for it.
        self.push_message(rx, tx, ActorMessage::FindProduct { product: *product, sender: self.actor_info() });
        // with the message sent, wait for the response back while in our standard holding pattern.
        let result = self.active_wait(rx, tx, data, market, keep, spend, returned, 
            &vec![ActorMessage::ProductNotFound { product: 0, buyer: ActorInfo::Firm(0) },
            ActorMessage::FoundProduct { seller: ActorInfo::Firm(0), buyer: ActorInfo::Firm(0), product: 0 }]);
        // result is now either FoundProduct or ProductNotFound, deal with it and return the result to the caller
        // TODO update this to be smarter about doing emergency buy searches.
        if let ActorMessage::ProductNotFound { product, .. } 
        = result {
            // TODO update to use emergency buy in dire situations here.
            // if product not found, do an emergency search instead.
            //self.emergency_buy(rx, tx, data, market, spend, &product, returned)
            BuyResult::NotSuccessful { reason: OfferResult::OutOfStock }
        }
        else if let ActorMessage::FoundProduct { seller, .. } = result {
            self.standard_buy(rx, tx, data, market, seller, spend, returned)
        }
        else { panic!("Somehow did not get FoundProduct or ProductNotFound."); }
    }

    /// Gets the standard shopping time cost for this pop.
    /// 
    /// This is currently calculated as being equal to 
    /// SHOPPING_TIME_COST (0.2) * self.total_population
    pub fn standard_shop_time_cost(&self) -> f64 {
        constants::SHOPPING_TIME_COST * self.count() as f64
    }

    /// Standard Buy Function
    /// 
    /// This has been reached when we have successfully recieved a 
    /// FoundProduct message. We are therefore in a deal and must focus on it.
    /// 
    /// So now we enter here to manage our next steps.
    /// 
    /// 1. Wait for the response from the seller. This should be
    ///    either ActorMessage::InStock or ActorMessage::NotInStock.
    /// 
    /// 2. If Not in stock, break out and fail. If InStock, begin our deal 
    /// making in step 3.
    /// 
    /// 3. Look at the price given and the quantity they have to offer
    /// and make an offer for that based on their estimates.
    /// 
    /// 4. Specifically wait for the response of ActorMessage::SellerAcceptAsIs,
    /// ActorMessage::OfferAcceptedWithChange, ActorMessage::RejectOffer, or
    /// ActorMessage::CloseDeal.
    /// 
    /// 5. React to the response appropriately.
    ///   a. If CloseDeal, record failure then exit out with that info.
    ///   b. If Rejected, Record Failure, then exit out with that info.
    ///   c. If Accepted, finish out and accept change, record what was
    ///      spent and recieved back in memory, then exit out with success.
    /// 
    /// ## Results
    /// 
    /// This will handle all parts of a deal once ActorMessage::ProductFound is
    /// recieved. Exiting out only when it has sent or recieved a close message.
    /// 
    /// It also will add or remove anything offered/requested in the deal if it
    /// goes through, including accepting any change from the seller.
    /// 
    /// If there is any new items in change that were not already in spend, it 
    /// adds them to the returned parameter so that other functions can place
    /// those items where we wish (either into keep or spend).
    /// 
    /// Additionally, it handles updating the pop's memory of the product for
    /// the day, adding any recieved to that item's achieved and adding to spent
    /// if spent, as well as updating the target it's AMV spent on it.
    /// 
    /// It will not expend time (or at least it currently doesn't. This may
    /// be changed if time for a deal scales with items exchanged.)
    pub fn standard_buy(&mut self, 
    rx: &mut Receiver<ActorMessage>, 
    tx: &Sender<ActorMessage>, 
    data: &DataManager, 
    market: &MarketHistory, 
    seller: ActorInfo,
    spend: &mut HashMap<usize, f64>,
    returned: &mut HashMap<usize, f64>) -> BuyResult {
        // We don't send CheckItem message as FindProduct msg includes that in the logic.
        // wait for deal start or preemptive close.
        let result = self.specific_wait(rx, 
        &vec![ 
            ActorMessage::InStock { buyer: ActorInfo::Firm(0), seller: ActorInfo::Firm(0), product: 0, price: 0.0, quantity: 0.0 }, 
            ActorMessage::NotInStock { buyer: ActorInfo::Firm(0), seller: ActorInfo::Firm(0), product: 0 }]);
        if let ActorMessage::NotInStock { product, .. } = result { // if not in stock
            // record our failure
            self.memory.product_knowledge.get_mut(&product)
                .unwrap().unable_to_purchase();
            return BuyResult::NotSuccessful { reason: OfferResult::OutOfStock };
        }
        // if in stock, continue with the deal
        if let ActorMessage::InStock { buyer: _, seller: _, 
        product, price, quantity } = result {
            // get our original budget to check against.
            let unit_budget = self.memory.product_knowledge
                .get(&product).expect("Product Not found?")
                .unit_budget();
            // quickly check if the current price is to see if it's overpriced for us.
            if price > unit_budget * 1.5 {
                // If overpriced to our original budget, cancel.
                self.push_message(rx, tx, ActorMessage::RejectPurchase 
                    { buyer: self.actor_info(), seller, product, 
                        price_opinion: OfferResult::TooExpensive });
                // and close out immediately.
                // TODO consider allowing this to, instead of closing out immediately, try for a different item in our list.
                self.push_message(rx, tx, ActorMessage::CloseDeal
                    { buyer: self.actor_info(), seller, product });
                // return not successful with Too Expensive as the reason.
                return BuyResult::NotSuccessful { reason: OfferResult::TooExpensive };
            }
            // get our current unit budget
            let curr_unit_budget = self.memory.product_knowledge
            .get(&product).expect("Product Not found?")
                .current_unit_budget();
            // get our target, capped at the quantity available.
            let target = self.memory.product_knowledge.get(&product)
            .expect("Product Not Found")
                .target_remaining().min(quantity);
            // and our remaining total budget.
            let remaining_budget = self.memory.product_knowledge.get(&product)
            .expect("Product Not Found")
                .remaining_amv();
            // get our current price opinion.
            // TODO return here to check on this, maybe return to unit_budget.
            let threshold = price / curr_unit_budget;
            let offer_result = 
            if threshold > TOO_EXPENSIVE { OfferResult::TooExpensive }
            else if threshold > EXPENSIVE { OfferResult::Expensive }
            else if threshold > OVERPRICED { OfferResult::Overpriced }
            else if threshold > REASONABLE { OfferResult::Reasonable }
            else if threshold > CHEAP { OfferResult::Cheap }
            else /* threshold <= CHEAP */ { OfferResult::Steal };
            // get how much we might pay
            let total_price = target * price;
            // cap our target to what we have budgeted
            let mut adjusted_target_price = total_price.min(remaining_budget);
            // get our corrected target the adjusted target price
            let mut target = adjusted_target_price / price;
            if !data.products.get(&product).unwrap()
            .fractional { // floor it if the product is not fractional
                target = target.floor();
                adjusted_target_price = target * price;
            }
            // with our new target, build up the offer
            // TODO consider making the length of the list cost time, so the more items kinds and quantity the more time it costs to purchase.
            let (mut offer, sent_amv) = self.create_offer(product, adjusted_target_price,
                spend, data, market);
            // With our offer built, send it over
            self.send_buy_offer(rx, tx, product, seller, &offer, offer_result, target);
            // wait for the seller to respond, he either accepts as is, accepts with change, rejects, or closes.
            let response = self.specific_wait(rx, &vec![
                ActorMessage::SellerAcceptOfferAsIs { buyer: ActorInfo::Firm(0), seller: ActorInfo::Firm(0), 
                    product: 0, offer_result: OfferResult::Cheap },
                ActorMessage::OfferAcceptedWithChange { buyer: ActorInfo::Firm(0), seller: ActorInfo::Firm(0), 
                    product: 0, quantity: 0.0, followups: 0 },
                ActorMessage::RejectOffer { buyer: ActorInfo::Firm(0), seller: ActorInfo::Firm(0), product: 0 },
                ActorMessage::CloseDeal { buyer: ActorInfo::Firm(0), seller: ActorInfo::Firm(0), product: 0 }
            ]);
            match response {
                // TODO Infowars Expansion results of buying in here or in caller.
                ActorMessage::SellerAcceptOfferAsIs { .. } => {
                    // offer accepted as is,
                    // add what we purchased.
                    *self.desires.property.entry(product).or_insert(0.0) += target;
                    // update our achieved target property.
                    self.memory.product_knowledge.get_mut(&product)
                        .unwrap().achieved += target;
                    // for everything we spent
                    for (offer_prod, amount) in offer.iter() {
                        // remove it from property.
                        *self.desires.property.entry(*offer_prod)
                            .or_insert(0.0) -= amount;
                        // if item is now 0 remove it
                        if *self.desires.property.get(offer_prod).unwrap() == 0.0 { 
                            self.desires.property.remove(offer_prod);
                        }
                        // subtract the amount from our spend entry.
                        *spend.get_mut(offer_prod).unwrap() -= amount; // since we spent it it must be there
                        if *spend.get(offer_prod).unwrap() == 0.0 { // if we spent all of it, remove it.
                            spend.remove(offer_prod);
                        }
                        // update memory for these products spent
                        self.memory.product_knowledge.entry(*offer_prod)
                            .or_insert(Knowledge::new()).spent += amount;
                    }
                    // get the AMV total spent in memory.
                    self.memory.product_knowledge.get_mut(&product)
                        .unwrap().amv_spent += sent_amv;
                    // update the success rate
                    self.memory.product_knowledge.get_mut(&product)
                        .unwrap().successful_purchase();
                    // return success
                    return BuyResult::Successful;
                },
                ActorMessage::OfferAcceptedWithChange { followups, .. } => {
                    // TODO consider adding in a 'reject change' option.
                    // Offer accepted, but there's some change.
                    // Get the returned items and update the offer
                    let mut left = followups;
                    while left > 0 { // insert all the change into our offer (subtract what is given back)
                        if let ActorMessage::ChangeFollowup { return_product, return_quantity, 
                        followups, .. } = self.specific_wait(rx, 
                        &vec![
                        ActorMessage::ChangeFollowup { buyer: ActorInfo::Firm(0), 
                            seller: ActorInfo::Firm(0), product: 0, return_product: 0, 
                            return_quantity: 1.0, followups: 0 }]) 
                        {
                            // with the followup message recieved, update our offer 
                            *offer.entry(return_product).or_insert(0.0) -= return_quantity;
                            left = followups;
                        }
                    }
                    // with the offer adjusted for change
                    let mut resulting_amv = 0.0;
                    for (offer_prod, quant) in offer.iter() {
                        // remove from property
                        *self.desires.property.entry(*offer_prod).or_insert(0.0) -= quant;
                        if *self.desires.property.get(offer_prod).unwrap() == 0.0 {
                            self.desires.property.remove(offer_prod);
                        }
                        // also record whether we spent or recieved the offer item.
                        let mem = self.memory.product_knowledge.entry(*offer_prod)
                            .or_insert(Knowledge::new());
                        if *quant > 0.0 { // if being spent
                            mem.spent += quant; // add to spend
                            let val = spend.get_mut(offer_prod).unwrap(); // and spend it
                            *val -= quant;
                            if *val == 0.0 {
                                spend.remove(offer_prod).expect("WTFH?");
                            }
                        } else { // if recieved, add to achieved
                            mem.achieved -= quant;
                            // and return
                            *returned.entry(*offer_prod).or_insert(0.0) -= quant;
                        }
                        // and get it's price for AMV uses later
                        resulting_amv += market.get_product_price(offer_prod, 0.0) * quant;
                    }
                    // add what we purchased.
                    *self.desires.property.entry(product).or_insert(0.0) += target;
                    // add what we achieved to it's memory
                    self.memory.product_knowledge.get_mut(&product)
                        .unwrap().achieved += target;
                    // add AMV to the product's memory spent
                    self.memory.product_knowledge.get_mut(&product)
                        .unwrap().amv_spent += resulting_amv;
                    // update the success rate
                    self.memory.product_knowledge.get_mut(&product)
                        .unwrap().successful_purchase();
                    return BuyResult::Successful;
                },
                ActorMessage::RejectOffer { .. } => {
                    // offer rejected, don't remove anything and get out.
                    self.memory.product_knowledge.get_mut(&product)
                        .unwrap().unable_to_purchase();
                    return BuyResult::NotSuccessful { reason: OfferResult::Rejected };
                },
                ActorMessage::CloseDeal { .. } => {
                    self.memory.product_knowledge.get_mut(&product)
                        .unwrap().unable_to_purchase();
                    return BuyResult::SellerClosed;
                }
                _ => { panic!("Incorrect message recieved from Buy offer?")}
            }
        }
        panic!("Should never get here!");
    }

    /// # Emergency Buy
    /// 
    /// Emergency buy means that we NEED the product being requested asap.
    /// 
    /// This removes the sanctity of all items in keep and offers everything 
    /// less important than that item (of higher tier)
    /// TODO Currently not built, should be slightly simpler version of Standard Buy.
    pub fn emergency_buy(&mut self, 
    rx: &mut Receiver<ActorMessage>, 
    tx: &Sender<ActorMessage>, 
    data: &DataManager, 
    market: &MarketHistory, 
    spend: &mut HashMap<usize, f64>,
    product: &usize,
    returned: &mut HashMap<usize, f64>) -> BuyResult {
        todo!("Emergency Buy here!")
    }

    /// gets the total current wealth of the pop in question.
    pub fn total_wealth(&self, history: &MarketHistory) -> f64 {
        self.desires.property.iter()
        .map(|x| history.get_product_price(x.0, 0.0))
        .sum()
    }

    /// Gets the wealth of the pop on a per-capita basis.
    pub fn per_capita_wealth(&self, history: &MarketHistory) -> f64 {
        self.total_wealth(history) / (self.count() as f64)
    }

    /// ## Create Offer
    /// 
    /// Parameters
    /// - product is the item we're looking at.
    /// - target is the target AMV we are trying to meet.
    /// - spend is what items we can spend
    /// - data is product data mostly
    /// - market is market data.
    /// 
    /// Creates a purchase offer for a product.
    /// It takes the product we're working with,
    /// the budget we need to meet, as well as 
    /// info for the product and market.
    /// 
    /// The amount returned is allowed to run over
    /// but should be as close as possible.
    /// 
    /// OVERSPEND_THRESHOLD % or less is considered a valid target.
    /// 
    /// Returns a hashmap of the offer as well as the final price.
    pub fn create_offer(&self, product: usize, target: f64,
    spend: &HashMap<usize, f64>, data: &DataManager, 
    market: &MarketHistory) -> (HashMap<usize, f64>, f64) {
        let mut offer = HashMap::new();
        let mut total = 0.0;
        // copy over items for sale (minus the product in question) and their prices.
        let mut available = HashMap::new();
        let mut prices = HashMap::new();
        for (product, quantity) in spend.iter()
        .filter(|(a, b)| **a != product) {
            available.insert(*product, *quantity);
            let price = market.get_product_price(product, 0.0);
            prices.insert(*product, price);
        }
        // With prices and amounts try to buy with currency first.
        for offer_item in market.currencies.iter()
        .filter(|x| available.contains_key(x))
        {
            // get the price for the currency
            let offer_prod_price = market.get_product(offer_item).price;
            let prod_avail = available.get(offer_item).expect("Product not found?");
            let available_amv = offer_prod_price * prod_avail;
            // if availible price overshoots, then deal with that.
            let spend = if available_amv >= (target - total) {
                // reduce to a perfect match.
                let ratio = (target - total) / available_amv;
                let prod_perfect = prod_avail * ratio;
                // if fractional, perfect, if not do a rounding check
                if data.products.get(&offer_item).expect("Product not found?")
                .fractional {
                    prod_perfect
                } else { // If rounding up is greater than OVERSPEND_THRESHOLD round down and continue
                    let prod_ceiling = prod_perfect.ceil();
                    let ceiling_price = prod_ceiling * offer_prod_price;
                    let ceiling_total = total + ceiling_price;
                    let ratio = ceiling_total / target;
                    if ratio < (1.0 + OVERSPEND_THRESHOLD) { 
                        // TODO add factor to increase or reduce threshold based on pop despiration.
                        // if current price with cieling is under our threshold
                        // use that, otherwise, use the next step down.
                        prod_ceiling
                    } else {
                        prod_ceiling - 1.0
                    }
                }
            } else { // if still not enough, 
                *prod_avail
            };
            // sanity check that our spend product is not zero, if it is, skip.
            if spend == 0.0 { continue; }
            // with the amount to spend gotten, add that to our total
            total += spend * offer_prod_price;
            // then add that spend amount to our offer
            offer.insert(*offer_item, spend);
            // if the total is now above our target (implying it's within overspend territory)
            if total > target {
                return (offer, total);
            }
        }

        for offer_item in market.sale_priority.iter()
        .filter(|x| available.contains_key(x) && !market.currencies.contains(x)) {
            // get the price for the currency
            let offer_prod_price = market.get_product(offer_item).price;
            let prod_avail = available.get(offer_item).expect("Product not found?")
                - offer.get(offer_item).unwrap_or(&0.0);
            let available_amv = offer_prod_price * prod_avail;
            // if availible price overshoots, then deal with that.
            let spend = if available_amv > (target - total) {
                // reduce to a perfect match.
                let ratio = (target - total) / available_amv;
                let prod_perfect = prod_avail * ratio;
                // if fractional, perfect, if not do a rounding check
                if data.products.get(&offer_item).expect("Product not found?")
                .fractional {
                    prod_perfect
                } else { // If rounding up is greater than OVERSPEND_THRESHOLD round down and continue
                    let prod_ceiling = prod_perfect.ceil();
                    let ceiling_price = prod_ceiling * offer_prod_price;
                    let ceiling_total = total + ceiling_price;
                    let ratio = ceiling_total / target;
                    if ratio < (1.0 + OVERSPEND_THRESHOLD) { 
                        // TODO add factor to increase or reduce threshold based on pop despiration.
                        // if current price with cieling is under our threshold
                        // use that, otherwise, use the next step down.
                        prod_ceiling
                    } else {
                        prod_ceiling - 1.0
                    }
                }
            } else { // if still not enough, 
                prod_avail
            };
            // sanity check that our spend product is not zero, if it is, skip.
            if spend == 0.0 { continue; }
            // with the amount to spend gotten, add that to our total
            total += spend * offer_prod_price;
            // then add that spend amount to our offer
            offer.insert(*offer_item, spend);
            // if the total is now above our target (implying it's within overspend territory)
            if total >= target {
                return (offer, total);
            }
        }
        // if we got here, return whatever we got anyway.
        (offer, total)
    }

    /// # Buyer Approaches
    /// 
    /// Used when a buyer approaches us as a seller. Used for both emergency 
    /// buy and standard buy. For Pops the distinction of whether a buyer is
    /// coming out of an emergency, or from standard purchasing makes no
    /// difference currently.
    /// 
    /// TODO upgrade this to take in the possibility of charity and/or givaways.
    pub fn buyer_approaches(&mut self, rx: &mut Receiver<ActorMessage>, 
    tx: &Sender<ActorMessage>, data: &DataManager,
    market: &MarketHistory, keep: &mut HashMap<usize, f64>, 
    spend: &mut HashMap<usize, f64>, product: usize, buyer: ActorInfo) {
        let seller = self.actor_info();
        let product_price = market.get_product_price(&product, 1.0);
        // we have recieved a FoundProduct MSG and we're the seller.
        // send back our available stock (if any)
        if let Some(quantity) = spend.get_mut(&product) {
            self.push_message(rx, tx, ActorMessage::InStock { buyer, seller, product, price: product_price, quantity: *quantity });
        } else { // if we don't have the item, send our OOS message
            // This currently closes the deal
            self.push_message(rx, tx, ActorMessage::NotInStock { buyer, seller, product });
            return;
        }
        // with stock message pushed, we wait for the buy offer or close message from them.
        let result = self.specific_wait(rx, &vec![
            ActorMessage::RejectPurchase { buyer: seller, seller: seller, product: 0, price_opinion: OfferResult::Cheap },
            ActorMessage::BuyOffer { buyer: seller, seller: seller, product: 0, 
            price_opinion: OfferResult::Cheap, quantity: 0.0, followup: 0 }
        ]);

        if let ActorMessage::RejectPurchase { .. } = result { // if purchase rejected,
            // TODO add check item followup option here when check item followup is added.
            // recieve the close deal message to confirm.
            let result = self.specific_wait(rx, &vec![
                ActorMessage::CloseDeal { buyer: seller, seller: seller, product: 0 }
            ]);
            if let ActorMessage::CloseDeal { .. } = result {
                // Nothing meaningful to do here, return
                return;
            }
            return; // if anything else, for now, return as well.
        } else if let ActorMessage::BuyOffer { buyer, seller, 
        product, price_opinion, quantity,
        followup: mut current_step } = result { // if it's a buy offer, get their offer
            let mut offer = HashMap::new();
            while current_step > 0 {
                // get the next, it should be decreasing, but we'll just do this to maintain our sanity.
                if let ActorMessage::BuyOfferFollowup { offer_product, offer_quantity,
                followup, .. } = self.specific_wait(rx, &vec![
                    ActorMessage::BuyOfferFollowup { buyer: seller, seller: seller, 
                        product: 0, offer_product: 0, offer_quantity: 0.0, followup: 0 }
                ]) {
                    offer.insert(offer_product, offer_quantity);
                    current_step = followup;
                }
            }
            // with the entirety of the offer gotten, we can check to 
            // see if it is acceptable.
        }
    }
}

impl Buyer for Pop {
}

impl Seller for Pop {
    fn actor_type(&self) -> ActorType {
        ActorType::Pop
    }

    fn actor_info(&self) -> ActorInfo {
        ActorInfo::Pop(self.id)
    }

    fn get_id(&self) -> usize {
        self.id
    }
}

impl Actor for Pop {
    /// Runs the market day for the pop.
    /// 
    /// Called by the pop's market.
    /// 
    /// Starts by waiting for the market to spin up (to keep things clean)
    /// then it begins pre-calculations. For pops this means looking at their
    /// situation (resources available, demographic habits, Workplace rules)
    /// to decide whether they will offer their goods for exchange, or not.
    /// 
    /// After Precalculation it works for it's job, giving it's time and either
    /// getting a pay-stub or their paycheck, whichever the job gives. (pay 
    /// stub is a placeholder for a payment to simplify transfers forward).
    /// 
    /// Once they recieve their pay from work, they enter their normal day,
    /// rotating between buying what they desire, and completing processes to 
    /// use/consume products to get wants.
    /// 
    /// If they are putting up things for sale, they will also add selling
    /// into the rotation, though they are much more limited in how they can
    /// handle it.
    /// 
    /// They continue this cycle until they run out of time to use, in which
    /// case they tell the market they're done and enter a holding pattern, 
    /// waiting for either buying messages or for the market day to end.
    /// 
    /// # Selling Notes
    /// 
    /// If they are offering stuff for exchange, they will send up messages
    /// for barter on everything they are offering.
    /// 
    /// What they offer for exchange are the products which are either
    /// - not desired at all.
    /// - not reserved.
    /// - is excess above the full_tier_satisfied.
    /// 
    /// Products offered for sale have their AMV price set at yesterday's
    /// price, though their sell mechanism is far more fluid.
    /// 
    /// Items which have an AMV below the value of their time will be 
    /// trashed instead, thrown to the market for anyone to pick up.
    /// 
    /// # Panics
    /// 
    /// Panics if it recieves any message before ActorMessage::StartDay
    /// to ensure the broadcast queue is open.
    fn run_market_day(&mut self, 
    tx: Sender<ActorMessage>,
    rx: &mut Receiver<ActorMessage>,
    data: &DataManager,
    demos: &Demographics,
    history: &MarketHistory) {
        // before we even begin, add in the time we have for the day.
        self.desires.add_property(TIME_ID, &((self.breakdown_table.total as f64) * 
            24.0 * self.breakdown_table.average_productivity()));

        // started up, so wait for the first message.
        match rx.recv().expect("Channel Broke.") {
            ActorMessage::StartDay => (), // wait for start day, throw otherwise.
            _ => panic!("Pop Recieved something before Day Start. Don't do something before the day starts.")
        }
        // precalculate our plans for the day based on yesterday's results and
        // see if we want to sell and what we want to sell.
        self.desires.sift_products();
        for (key, know) in self.memory.product_knowledge.iter_mut() {
            know.achieved = 0.0; // reset knowledge info for the day.
            know.spent = 0.0;
            know.lost = 0.0;
            know.time_spent = 0.0;
            know.amv_spent = 0.0;
        }
        self.is_selling = if self.memory.is_disorganized { 
            true
        }
        else {
            // TODO add check here. 
            // Checks would probably be a panic check, (has resources but 
            // is starving)
            // or if they have a bunch of excess resources and a derth of 
            // support resources (space, security, etc) then they'll also
            // sell
            false
        };

        // Wait for our job to poke us, asking/telling us what to give them 
        // and send it all over (will likely get a short lived channel for this)
        // then wait for the firm to get back.
        self.work_day_processing(rx, &tx);

        // The firm will return either with a paycheck, paystub if a wage 
        // employee, or if it's a disorganized owner, it's share of everything.
        // Start free time section, roll between processing for wants, going 
        // out to buy things, and dealing with recieved sale orders.
        self.free_time(rx, &tx, data, history);

        // Once time has run out, send up a finished message.
        tx.send(ActorMessage::Finished { 
            sender: self.actor_info() 
        }).expect("Channel Closed Unexpectedly!");
        // Then enter a holding pattern, continuing to consume from the 
        // message queue, and dealing with sale orders shortly.
        // When we recieve the AllFinished, we know no more messages will 
        // come, so stop holding and fall out, possibly recording data as
        // needed.
    }
}

