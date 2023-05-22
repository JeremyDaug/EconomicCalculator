//! The storage unit of population groups.
//! 
//! Used for any productive, intellegent actor in the system. Does not include animal
//! populations.
use std::{collections::{VecDeque, HashMap}};

use barrage::{Sender, Receiver};

use crate::{demographics::Demographics, data_manager::DataManager, constants::{OVERSPEND_THRESHOLD, TIME_ID, self}};

use super::{desires::{Desires, PropertyBreakdown}, 
    pop_breakdown_table::PopBreakdownTable, 
    buyer::Buyer, seller::Seller, actor::Actor, 
    market::MarketHistory, 
    actor_message::{ActorMessage, ActorType, ActorInfo, FirmEmployeeAction, OfferResult}, 
    pop_memory::PopMemory, buy_result::BuyResult, 
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
    /// 
    /// TODO Food For Thought. We include 2 infinite desires in all pops, wealth and Leisure, which act as sinks and help us balance our buy priorities. More thought is needed.
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
        // get the offer length 
        let mut offer_len = offer.len();
        // send the opener (request item and quantity)
        self.push_message(rx, tx, ActorMessage::BuyOffer { buyer: self.actor_info(), seller, 
            product, price_opinion: offer_result, quantity: target,
            followup: offer_len });
        // then loop over what we're sending to them.
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
                    self.process_common_msg(rx, tx, data, market, msg);
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
    action: FirmEmployeeAction,
    data: &DataManager) -> bool {
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
                self.desires.remove_property(TIME_ID, -self.memory.work_time, data);
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
                        amount: amount.total_property 
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
                    Some(amount) => amount.total_property,
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
    pub fn work_day_processing(&mut self, rx: &mut Receiver<ActorMessage>, tx: &Sender<ActorMessage>, data: &DataManager) {
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
                        self.desires.property.entry(product)
                        .and_modify(|x| x.add_property(amount))
                        .or_insert(PropertyBreakdown::new(amount));
                },
                ActorMessage::FirmToEmployee { firm: sender, 
                employee: _, action } => {
                    if self.process_firm_message(rx, tx, sender, action, data) {
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
    pub fn free_time(&mut self, _rx: &mut Receiver<ActorMessage>, _tx: &Sender<ActorMessage>, 
    _data: &DataManager,
    _market: &MarketHistory) -> HashMap<usize, PropertyBreakdown>{
        todo!("Redo this!")
    }

    /// Processes common messages from the ActorMessages for current free time.
    /// Function assumes that msg is for us, so be sure to collect just those.
    /// 
    /// Returns any messages that we don't handle here. Currently, 
    fn process_common_msg(&mut self, rx: &mut Receiver<ActorMessage>, 
    tx: &Sender<ActorMessage>, data: &DataManager, 
    market: &MarketHistory, msg: ActorMessage) -> Option<ActorMessage> {
        match msg {
            ActorMessage::FoundProduct { seller, buyer, 
            product } => {
                if buyer == self.actor_info() {
                    panic!("Product Found message with us as the buyer should not be found outside of deal state.");
                } else if seller == self.actor_info() {
                    // TODO When change is possible, deal with it here.
                    let _accepted = self.standard_sell(rx, tx, data, market, product, buyer);
                } else { unreachable!("How TF did we get here? We shouldn't have recieved this FoundProduct Message!"); }
                return None;
            },
            // TODO add Seller Approaches Logic Here.
            ActorMessage::SendProduct { product, amount, .. } => {
                // We're recieving a product, add to our unreserved amount.
                self.desires.property.entry(product)
                .and_modify(|x| x.add_property(amount))
                .or_insert(PropertyBreakdown::new(amount));
                return None;
            },
            ActorMessage::SendWant { want, amount, .. } => {
                *self.desires.want_store.entry(want).or_insert(0.0) += amount;
                return None;
            },
            ActorMessage::WantSplash { want, amount, .. } => {
                *self.desires.want_store.entry(want).or_insert(0.0) += amount;
                return None;
            },
            ActorMessage::FirmToEmployee { firm, employee: _,
            action } => {
                self.process_firm_message(rx, tx, firm, action, data);
                return None;
            },
            _ => ()
        }
        Some(msg)
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
    /// If the product is not found, it will return Not Successful with OutOfStock 
    /// as the reason.
    /// 
    /// It has 2 options when it starts.
    /// - Standard Search, it asks the market to find a guaranteed seller,
    /// who we'll get in touch with and try to make a deal.
    /// - Emergency Search, this occurs when either the product being sought 
    /// is unavailable through sellers and the product sought is important
    fn _try_to_buy(&mut self, 
    rx: &mut Receiver<ActorMessage>, 
    tx: &Sender<ActorMessage>, 
    data: &DataManager, 
    market: &MarketHistory, 
    records: &mut HashMap<usize, PropertyBreakdown>,
    product: &usize) -> BuyResult {
        // get time cost for later
        let time_cost = self.standard_shop_time_cost(data);
        // get the amount we want to get and the unit price budget.
        let mem = self.memory.product_knowledge
        .get_mut(product).expect("Product not found?");
        let price = mem.current_unit_budget();
        // with budget gotten, check if it's feasable for us to buy (market price < 2.0 budget)
        let market_price = market.get_product_price(product, 0.0);
        if market_price > (price * constants::HARD_BUY_CAP) {
            // if unfeaseable, at current market price, cancel.
            return BuyResult::CancelBuy;
        }
        // check if we have enough time available to do the purchase.
        if let Some(record) = records.get_mut(&TIME_ID) {
            if record.unreserved < time_cost {
                return BuyResult::NoTime;
            }
            // subtract the time from our stock and add it to time spent
            // TODO Consider updating this to scale not just with population buying but also pop_efficiency gains.
            // TODO cheat and just subtract from time right now, this should subtract from Shopping_time not normal time.
            self.desires.add_property(TIME_ID, -time_cost);
            mem.time_spent += time_cost;
            // TODO update self.breakdown.total to instead use 'working population' instead of total to exclude dependents.
            let _result = record.expend(time_cost);
        }
        
        // since the current market price is within our budget, try to look for it.
        self.push_message(rx, tx, ActorMessage::FindProduct { product: *product, sender: self.actor_info() });
        // with the message sent, wait for the response back while in our standard holding pattern.
        let result = self.active_wait(rx, tx, data, market,
            &vec![ActorMessage::ProductNotFound { product: 0, buyer: ActorInfo::Firm(0) },
            ActorMessage::FoundProduct { seller: ActorInfo::Firm(0), buyer: ActorInfo::Firm(0), product: 0 }]);
        // result is now either FoundProduct or ProductNotFound, deal with it and return the result to the caller
        // TODO update this to be smarter about doing emergency buy searches.
        if let ActorMessage::ProductNotFound { product: _, .. } 
        = result {
            // TODO update to use emergency buy in dire situations here.
            // if product not found, do an emergency search instead.
            //self.emergency_buy(rx, tx, data, market, spend, &product, returned)
            BuyResult::NotSuccessful { reason: OfferResult::OutOfStock }
        }
        else if let ActorMessage::FoundProduct { seller, .. } = result {
            self.standard_buy(rx, tx, data, market, seller, records)
        }
        else { unreachable!("Somehow did not get FoundProduct or ProductNotFound."); }
    }

    /// Gets the standard shopping time cost for this pop.
    /// 
    /// This is currently calculated as being equal to 
    /// SHOPPING_TIME_COST (0.2) * self.total_population
    pub fn standard_shop_time_cost(&self, _data: &DataManager) -> f64 {
        // TODO Update to get the time cost for Shopping process.
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
    _rx: &mut Receiver<ActorMessage>, 
    _tx: &Sender<ActorMessage>, 
    _data: &DataManager, 
    _market: &MarketHistory, 
    _seller: ActorInfo,
    _records: &mut HashMap<usize, PropertyBreakdown>) -> BuyResult {
        // We don't send CheckItem message as FindProduct msg includes that in the logic.
        // wait for deal start or preemptive close.
        todo!("Redo")
    }

    /// # Emergency Buy
    /// 
    /// Emergency buy means that we NEED the product being requested asap.
    /// 
    /// This removes the sanctity of all items in keep and offers everything 
    /// less important than that item (of higher tier)
    /// TODO Currently not built, should be slightly simpler version of Standard Buy.
    pub fn emergency_buy(&mut self, 
    _rx: &mut Receiver<ActorMessage>, 
    _tx: &Sender<ActorMessage>, 
    _data: &DataManager, 
    _market: &MarketHistory, 
    _spend: &mut HashMap<usize, f64>,
    _product: &usize,
    _returned: &mut HashMap<usize, f64>) -> BuyResult {
        todo!("Emergency Buy here!")
    }

    /// gets the total current wealth of the pop in question.
    pub fn total_wealth(&self, history: &MarketHistory) -> f64 {
        self.desires.market_wealth(history)
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
    records: &HashMap<usize, PropertyBreakdown>, data: &DataManager, 
    market: &MarketHistory) -> (HashMap<usize, f64>, f64) {
        let mut offer = HashMap::new();
        let mut total = 0.0;
        // copy over items for sale (minus the product in question) and their prices.
        let mut available = HashMap::new();
        let mut prices = HashMap::new();
        for (product, quantity) in records.iter()
        .filter(|(a, _)| **a != product) {
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
            let prod_avail = available.get(offer_item).expect("Product not found?").unreserved;
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
                prod_avail
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
            let prod_avail = available.get(offer_item).expect("Product not found?").unreserved
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

    /// # Standard Sell
    /// 
    /// Used when a buyer approaches us as a normal seller. It sells the items at
    /// market value.
    /// 
    /// Items it's accepting are rated based on their salability as well as the
    /// pop's demand for those items. Desired items get their full AMV, while
    /// undesired items have their price modified by their salability.
    /// 
    /// Returns the payment recieved so we can sort it into keep and spend.
    /// 
    /// TODO upgrade this to take in the possibility of charity and/or givaways.
    /// TODO currently, this costs the seller no time, and they immediately close out. This should be updated to allow the buyer to retry and/or the seller to lose time to the deal.
    /// TODO Currently does not do change, accepts offer or rejects, no returning change.
    pub fn standard_sell(&mut self, _rx: &mut Receiver<ActorMessage>, 
    _tx: &Sender<ActorMessage>, _data: &DataManager,
    _market: &MarketHistory,
    _product: usize, _buyer: ActorInfo) -> HashMap<usize, f64> {
        todo!("Redo for property update!")
    }

    /// # Consume Goods
    /// 
    /// Our end of daily activities. Goes through our goods, consuming them
    /// and adding to our satisfaction.
    pub fn consume_goods(&mut self, data: &DataManager, _history: &MarketHistory) {
        // start by clearing out our old satisfaction
        self.desires.clear_desires();
        // put our property into the specific product desire slots
        self.desires.sift_specific_products();
        // TODO add non-specific satisfaction slots here.
        // Consume Property to satisfy wants.
        // TODO This should return products which were used or not used so that the remainder can possibly be maintained.
        let used = self.desires.consume_and_sift_wants(data);
        // track the consumed/used items from above and record that info for our purposes.
        for (product, quant) in used {
            if let Some(know) = self.memory.product_knowledge.get_mut(&product) {
                know.used += quant;
            }
        }
        // update our tiers satisfied
        self.desires.update_satisfactions();
        // TODO Maintenance would also go here.
        // TODO should separate maintained products from unmaintained products here.
    }

    /// # Decay Goods
    /// 
    /// Decay goods goes through all of our current products and wants and
    /// decays, reduces, or otherwise checks them for failure.
    /// 
    /// Any products lost this way are recorded as losses in that product's knowledge.
    /// 
    /// TODO when upgrading to add rolling, add RNG back as a parameter.
    pub fn decay_goods(&mut self, _data: &DataManager) {
        todo!("Redo for property change!")
    }

    /// # Adapt future Plan
    /// 
    /// Adapt future plan takes our existing knowledge base and our results
    /// from todays buying, selling, and consuming to modify our plan for 
    /// tomorrow.
    /// 
    /// Using both our desires and knowledge of what we achieved today
    /// in particular, we seek to improve our efficiency at achieving our
    /// desires by altering how much time and/or AMV we budget to them as
    /// well as alter our buy ordering by swaping Product Knowledge in our
    /// list and altering our buy targets we want to reach.
    /// 
    /// We start by updating how successful we were. The ratio of achieved to
    /// the target is our current success rate, then apply that to our previous
    /// success rate. This should be a weighted sum, giving the prior days 
    /// priority over today.
    /// 
    /// With the success rate updated, we then alter our targets, and budgets.
    pub fn adapt_future_plan(&mut self, _data: &DataManager, 
    _history: &MarketHistory) {
        todo!("Either Redo or drop after Property Update!")
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
    _demos: &Demographics,
    history: &MarketHistory) {
        // before we even begin, add in the time we have for the day.
        self.desires.add_property(TIME_ID, (self.breakdown_table.total as f64) * 
            24.0 * self.breakdown_table.average_productivity());

        // started up, so wait for the first message.
        match rx.recv().expect("Channel Broke.") {
            ActorMessage::StartDay => (), // wait for start day, throw otherwise.
            _ => panic!("Pop Recieved something before Day Start. Don't do something before the day starts.")
        }
        // precalculate our plans for the day based on yesterday's results and
        // see if we want to sell and what we want to sell.
        self.desires.sift_specific_products();
        for (_product_id, know) in self.memory.product_knowledge.iter_mut() {
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

        // TODO Consider altering these actions to remove from property and only add back in at the end of the day after decay. Would need to think carefully about how best this would be done.

        // Wait for our job to poke us, asking/telling us what to give them 
        // and send it all over (will likely get a short lived channel for this)
        // then wait for the firm to get back.
        self.work_day_processing(rx, &tx, data);

        // The firm will return either with a paycheck, paystub if a wage 
        // employee, or if it's a disorganized owner, it's share of everything.
        // Start free time section, roll between processing for wants, going 
        // out to buy things, and dealing with recieved sale orders.
        self.free_time(rx, &tx, data, history);

        // TODO Taxes will either be done here, or in free_time above. Methods of Taxation will need to be looked into for the system.

        // With our free time used up and the finish message recieved, begin
        // following through with our consumption. This will both record
        // satisfaction, and consume items as needed.
        self.consume_goods(data, history);

        // with buying, selling, taxation, and consumption completed, 
        // run decay chances for our goods.
        self.decay_goods(data);

        // With these things consumed, we've done what we can. Process our
        // results to hopefully improve our situation tomorrow. 
        self.adapt_future_plan(data, history);
    }
}