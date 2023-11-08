//! The storage unit of population groups.
//!
//! Used for any productive, intellegent actor in the system. Does not include animal
//! populations.
use core::panic;
use std::{collections::{VecDeque, HashMap, HashSet}, ops::Add, thread::current};

use barrage::{Sender, Receiver};
use itertools::Itertools;

use crate::{demographics::Demographics, data_manager::DataManager, constants::{OVERSPEND_THRESHOLD, TIME_ID, self, SHOPPING_TIME_COST, SHOPPING_TIME_ID}, objects::{property::{DesireCoord, TieredValue}, desire::DesireItem}};

use super::{property::Property,
    pop_breakdown_table::PopBreakdownTable,
    buyer::Buyer, seller::Seller, actor::Actor,
    market::MarketHistory,
    actor_message::{ActorMessage, ActorType, ActorInfo, FirmEmployeeAction, OfferResult},
    buy_result::BuyResult, property_info::PropertyInfo, product::ProductTag,
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
    pub property: Property,
    /// A breakdown of the Population's demographics.
    pub breakdown_table: PopBreakdownTable,
    // Mood
    /// Whether the pop is selling or not.
    pub is_selling: bool,
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
        self.property.clear_desires();
        // add in each species desires
        for row in self.breakdown_table.species_makeup().iter() {
            let species = demos.species.get(row.0).expect("Species Id Not Found!");
            for desire in species.desires.iter() {
                let upped_desire = desire.create_multiple(*row.1);
                self.property.add_desire(&upped_desire);
            }
        }
        // placeholder for civilization
        // add in culture desires
        for row in self.breakdown_table.culture_makeup().iter() {
            if let Some(id) = row.0 {
                let culture = demos.cultures.get(id).expect("Culture Id Not Found!");
                for desire in culture.desires.iter() {
                    let upped_desire = desire.create_multiple(*row.1);
                    self.property.add_desire(&upped_desire);
                }
            }
        }

        // add in ideology desires
        for row in self.breakdown_table.ideology_makeup().iter() {
            if let Some(id) = row.0 {
                let ideology = demos.ideology.get(id).expect("Ideology Id Not Found!");
                for desire in ideology.desires.iter() {
                    let upped_desire = desire.create_multiple(*row.1);
                    self.property.add_desire(&upped_desire);
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

    /// # Message Catchup
    /// 
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

    /// # Send Buy Offer
    ///
    /// Small helper function to simplify sending our purchase offers.
    /// 
    /// ## Paratemers:
    /// * `rx`: Main Reciever
    /// * `tx`: Main Sender
    /// * `product`: the product we're attempting to buy.
    /// * `seller`: the person we are making an offer to.
    /// * `offer`; what all we are offering
    /// * `offer_result`: what we (the buyer) think of the offer.
    /// * `target`: how much we are trying to buy with the offer.
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

    /// # Active Wait
    /// 
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
    /// 
    /// It ignores the data of the message, only looking at the message type
    /// and that it is for us. 
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
                    amount: self.property.work_time
                    });
                // and remove that time from our property as well
                self.property.remove_property(TIME_ID, -self.property.work_time, data);
            },
            FirmEmployeeAction::RequestEverything => {
                // loop over everything and send it to the firm.
                let mut to_move = HashMap::new();
                for (product, amount) in self.property.property.iter() {
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
                    self.property.property.remove(&product)
                    .expect("Not found?");
                }
                // also send over the wants
                let mut to_move = HashMap::new();
                for (want, amount) in self.property.want_store.iter() {
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
                    self.property.want_store.remove(&want)
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
                let amount = match self.property.property.remove(&product) {
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
                    *self.property.want_store.entry(want).or_insert(0.0) += amount;
                },
                ActorMessage::SendProduct {
                    sender: _,
                    reciever: _,
                    product,
                    amount } => {
                        self.property.property.entry(product)
                        .and_modify(|x| {x.add_property(amount);})
                        .or_insert(PropertyInfo::new(amount));
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
    pub fn free_time(&mut self, rx: &mut Receiver<ActorMessage>, tx: &Sender<ActorMessage>,
    data: &DataManager,
    market: &MarketHistory) {
        // start by organizing our property, reserve everything for our desires.
        let initial_sat = self.property.sift_all(data);

        // After reserving for desires directly, measure excess wealth in AMV and 'sift' that.
        let mut surplus = HashMap::new();
        let mut amv_surplus = 0.0;
        for (&product, quant) in self.property.property.iter() {
            let amount = quant.available();
            surplus.insert(product, amount);
            amv_surplus += amount * market.get_product_price(&product, 1.0);
        }
        let hypothetical_satisfaction = self.property.satisfaction_from_amv(amv_surplus, market);

        // put up our surplus for sale if we desire it
        if self.is_selling {
            for (&product, &amount) in surplus.iter()
            .filter(|(&id, _)| data.products.get(&id).expect("Product Not found!").tags
            .contains(&ProductTag::NonTransferrable)) { // put everything that is transferrable up for sale.
                self.push_message(rx, tx, 
                ActorMessage::SellOrder { sender: self.actor_info(), 
                    product, 
                    quantity: amount, 
                    amv: market.get_product_price(&product, 1.0) });
                // non-firms offer at the current market price.
            }
        }

        self.shopping_loop(rx, tx, data, market);

        // after we run out of stuff to buy, send finished and leave, consumption comes later
        self.push_message(rx, tx, ActorMessage::Finished { sender: self.actor_info() });
        self.active_wait(rx, tx, data, market, &vec![
            ActorMessage::AllFinished
        ]);
    }

    /// # Shopping Loop
    /// 
    /// Shopping loop function is a helper for free time and adjacent fns.
    /// 
    /// It starts at the first tier with unsatisfied desires within. Satisfied
    /// desires are skipped.
    /// 
    /// It gets the item and plans how much of it it wants to try and buy 
    /// based on it's type.
    /// - if it's a class or specific product, it aims for a single shopping 
    ///   trip for the whole order.
    /// - if it's a want it selects a random process suggested by the market 
    ///   to get it, and tries to set up as many trips as products in it's 
    ///   list.
    /// 
    /// How much of each item it will buy is dependent on how much the desire 
    /// might need. It aims for at least a few tiers of the desire so that it
    /// can save time. It aims for either maxing out the desire, or halfway to 
    /// the highest tier + 1. This will need to be explored and tested. Maybe
    /// remember how high we got yesterday and aim for that +1.
    /// 
    /// With the number of buys it needs and targets, it extracts time for 
    /// shopping, then it spends it on shopping trips.
    /// 
    /// It goes out for it's desired buys. Regardless of success or failure,
    /// it tries to buy each of it's things. AFter buying everything, it 
    /// sifts it's goods again,
    /// 
    /// TODO Sift Improvement Location: When Sifting is upgraded to not be destructive, come back here and upgrade the time extraction.
    pub fn shopping_loop(&mut self, rx: &mut Receiver<ActorMessage>, tx: &Sender<ActorMessage>,
        data: &DataManager,
        market: &MarketHistory) {
        // TODO redo this stuff and sanity check it.
        // with everything reserved begin trying to buy more stuff
        // prepare current desire for first possible purchase.
        let mut next_desire = self.property.get_first_unsatisfied_desire();
        // also initialize shopping time, none should exist prior to here.
        let mut available_shopping_time = 0.0;
        // start our buying loop.
        while let Some(curr_desire_coord) = next_desire { // Should have our current desire coords in next_desire
            // start by getting our desire
            let curr_desire = self.property.desires
                .get(curr_desire_coord.idx).unwrap().clone();
            // if the current desire is already satisfied for wahtever reason move on
            if !curr_desire.satisfied_at_tier(curr_desire_coord.tier) {
                // this loop should never 
                // get the next, and continue.
                next_desire = self.property.walk_up_tiers(next_desire);
                continue;
            }
            
            // get our current desire target
            let current_desire_item = &curr_desire.item;
            let mut multiple_buys = vec![];
            // get how many items we need to buy for this desire.
            let buy_targets = match current_desire_item {
                DesireItem::Want(id) => { // for wants, we need to get the product inputs.
                    // if it's a want, go to the most common satisfaction 
                    // of that want in the market.
                    self.push_message(rx, tx, 
                        ActorMessage::FindWant { want: *id, sender: self.actor_info() });
                    // get the process the market suggests then 
                    let result = self.active_wait(rx, tx, data, market, 
                        &vec![
                            ActorMessage::FoundWant { buyer: ActorInfo::Firm(0), want: 0, prcocess: 0 },
                            ActorMessage::WantNotFound { want: 0, buyer: ActorInfo::Firm(0) }
                        ]);
                    if let ActorMessage::FoundWant { buyer: _, want, prcocess: _ } = result {
                        // get the process
                        let process_info = data.processes.get(&want).unwrap();
                        let needs = process_info.inputs_and_capital();
                        // get what needs to be gotten
                        for part in needs.iter()
                         {
                            multiple_buys.push(&part.item);
                        }
                        1.0 * needs.len() as f64
                    } else if let ActorMessage::WantNotFound { want: _, buyer: _ } = result {
                        // if the want is not found in the market, then move on to the next desire
                        // debug record anytime we get this here later.
                        0.0
                    } else { panic!("Should not be here.") }
                },
                DesireItem::Class(_) => 1.0, // for class, any item of the class will be good enough.
                DesireItem::Product(_) => 1.0, // for specific product. only one item will be needed.
            };
            // preemptively get the next desire
            // next_desire = self.property.walk_up_tiers(next_desire);
            // then sift up to this desire point to free up excess resources.
            // TODO when sifting is improved, drop this.
            self.property.sift_up_to(&curr_desire_coord, data);
            // get a trip of time worth 
            // TODO update to take more dynamic time payment into account.
            available_shopping_time += self.property.get_shopping_time(
                SHOPPING_TIME_COST * buy_targets - available_shopping_time, 
                data, market, self.skill_average(), self.skill, Some(curr_desire_coord));
            // check that it's enough time to go out buying
            if SHOPPING_TIME_COST > available_shopping_time {
                // if we don't have enough time to go shopping, break out, we won't
                // resolve that problem here.
                // return the time to our property and gtfo.
                self.property.add_property(SHOPPING_TIME_ID, available_shopping_time, data);
                break;
            }
            if multiple_buys.len() > 0 {
                // loop over the buys and react to the results of them.
                break;
            } else {
                // Do the buy
                let result = self.try_to_buy(rx, tx, data, market, current_desire_item, buy_targets);
                // react to the result and subtract from our shopping time.
                // TODO ^^
                match result {
                    BuyResult::CancelBuy => todo!(),
                    BuyResult::NotSuccessful { reason } => todo!(),
                    BuyResult::SellerClosed => todo!(),
                    BuyResult::Successful => todo!(),
                    BuyResult::NoTime => todo!(),
                }
                // with result of buy gotten, go to the next loop.
            }
        }
    }

    /// # Skill Average
    /// 
    /// Get's the effective (average) skill for the pop
    /// ( higher + lower ) / 2.0
    pub fn skill_average(&self) -> f64 {
        (self.lower_skill_level + self.higher_skill_level) / 2.0
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
                self.property.property.entry(product)
                .and_modify(|x| { x.add_property(amount); })
                .or_insert(PropertyInfo::new(amount));
                return None;
            },
            ActorMessage::SendWant { want, amount, .. } => {
                *self.property.want_store.entry(want).or_insert(0.0) += amount;
                return None;
            },
            ActorMessage::WantSplash { want, amount, .. } => {
                *self.property.want_store.entry(want).or_insert(0.0) += amount;
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
    /// This does not spend shopping time, instead letting the caller deal
    /// with the time expenditure.
    ///
    /// It has 2 options when it starts.
    /// - Standard Search, it asks the market to find a guaranteed seller,
    /// who we'll get in touch with and try to make a deal.
    /// - Emergency Search, this occurs when either the product being sought
    /// is unavailable through sellers and the product sought is important.
    fn try_to_buy(&mut self,
    rx: &mut Receiver<ActorMessage>,
    tx: &Sender<ActorMessage>,
    data: &DataManager,
    market: &MarketHistory,
    item: &DesireItem,
    target: f64) -> BuyResult {
        if let DesireItem::Product(product) = item {
            // get time cost for later
            let time_cost = self.standard_shop_time_cost();
            // let price = mem.current_unit_budget();
            let price = 1.0;
            // with budget gotten, check if it's feasable for us to buy (market price < 2.0 budget)
            let market_price = market.get_product_price(product, 0.0);
            if market_price > (price * constants::HARD_BUY_CAP) {
                // if unfeaseable, at current market price, cancel.
                return BuyResult::CancelBuy;
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
                self.standard_buy(rx, tx, data, market, seller)
            }
            else { unreachable!("Somehow did not get FoundProduct or ProductNotFound."); }
        }
        else {
            BuyResult::NotSuccessful { reason: OfferResult::Incomplete }
        }
    }

    /// Gets the standard shopping time cost for this pop.
    ///
    /// This is currently calculated as being equal to
    /// SHOPPING_TIME_COST (0.2) * self.total_population
    pub fn standard_shop_time_cost(&self) -> f64 {
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
    ///
    /// TODO will need to be updated when price estimates for wants and classes are added
    /// TODO Update to take into account storage gained/lost from the exchange also.
    pub fn standard_buy(&mut self,
    rx: &mut Receiver<ActorMessage>,
    tx: &Sender<ActorMessage>,
    data: &DataManager,
    market: &MarketHistory,
    _seller: ActorInfo) -> BuyResult {
        // We don't send CheckItem message as FindProduct msg includes that in the logic.
        // wait for deal start or preemptive close.
        let result = self.specific_wait(rx, &vec![
            ActorMessage::InStock { buyer: ActorInfo::Firm(0), seller:
                ActorInfo::Firm(0), product: 0, price: 0.0, quantity: 0.0 },
            ActorMessage::NotInStock { buyer: ActorInfo::Firm(0), seller:
                ActorInfo::Firm(0), product: 0 }
        ]);
        if let ActorMessage::NotInStock { .. } = result {
            // maybe record failure
            return BuyResult::NotSuccessful { reason: OfferResult::OutOfStock };
        } else if let ActorMessage::InStock { buyer: _, seller,
        product: sought_product, 
        price, 
        quantity: stock_available } = result { // Deal Making Section
            // TODO if we add Want Price Estimates in the Market, update this to use them!!!!!!!
            // setup current offer and current offer amv
            let mut current_offer: HashMap<usize, f64> = HashMap::new();
            let mut current_offer_amv = 0.0;
            // get the the property_info for the product we are buying
            let product_info = self.property.property
                .entry(sought_product).or_insert(PropertyInfo::new(0.0));
            // buy up to the remaining target or +1, if no remaining target.
            let purchase_quantity = if product_info.remaining_target() > 0.0 {
                stock_available.min(product_info.remaining_target()) 
            } else { // if no remaining target, just get 1 unit capped at stock available.
                stock_available.min(1.0)
            };
            // Get how much this purchase would increase our satisfaction by.
            let sat_gain = self.property.predict_value_gained(sought_product, 
                purchase_quantity, data);
            let mut sat_lost = TieredValue { tier: 0, value: 0.0 };
            // get the total AMV price of the purchase
            let purchase_price = purchase_quantity * price;
            // First use any property which we don't have a desire to keep to try and use them.
            for (product, info) in self.property.property.iter()
            .filter(|(_, info)| info.unreserved > 0.0) // get property which isn't reserved.
            .sorted_by(|a, b| {
                // sort by sal in descending order.
                // TODO improve to also take practicality of the exchange, ie value density
                let a_val = market.get_product_salability(&a.0);
                let b_val = market.get_product_salability(&b.0);
                b_val.partial_cmp(&a_val).unwrap_or(std::cmp::Ordering::Equal)
            }) {
                // get amv for the current product being given up.
                let eff_amv = market.get_product_price(product, 1.0);
                // add units up to amv_price, then round up.
                let perfect_ratio = (purchase_price / eff_amv).ceil();
                let capped = perfect_ratio.min(info.unreserved); // cap the ratio at what is available.
                // TODO Fractional Good Rework: are reworked, also rework this.
                let mut capped = if data.products.get(product).unwrap().fractional {
                    capped // if fractional, add all of it
                } else {
                    capped.floor() // if not, add up to a valid unit.
                };
                // add to the current total offer
                current_offer.entry(*product)
                    .and_modify(|x| *x += capped)
                    .or_insert(capped);
                // add the amv to the offer.
                let capped_amv = eff_amv * capped;
                current_offer_amv += capped_amv;
                // get how much satisfaction this would give us hypothetically
                let mut amv_sat = self.property.satisfaction_from_amv(current_offer_amv, market);
                // update the total
                sat_lost += amv_sat;
                // if the current total amv_satisfaction lost is > satisfaction gained
                if sat_lost > sat_gain {
                    // remove until the former is less than the latter.
                    while capped > 0.0 {
                        // remove previous amv_sat lost
                        sat_lost -= amv_sat;
                        // remove from offer
                        current_offer_amv -= capped * eff_amv;
                        current_offer.entry(*product)
                            .and_modify(|x| *x -= capped);
                        // reduce by half, round down for good measure.
                        capped = (capped / 2.0).floor();
                        // correct offer and amv
                        current_offer_amv += capped * eff_amv;
                        current_offer.entry(*product)
                            .and_modify(|x| *x += capped);
                        // re-get sat lost
                        amv_sat = self.property.satisfaction_from_amv(current_offer_amv, market);
                        sat_lost += amv_sat;
                        // check that it's below the target again.
                        if sat_lost < sat_gain {
                            break; // if yes, break, else try again.
                            // Note, this will always be reached eventually as 
                            // it will eventually remove the total value of the
                            // item added, which is (definitionally) lower than
                            // the original sat gain.
                        }
                    }
                }
                // stop early when the current_offer_amv > target
                if current_offer_amv > purchase_price {
                    break;
                }
            }

            // check that we surpassed the amv target, should not be overpaying in satisfaction.
            if purchase_price > current_offer_amv { 
                // if still not enough, start pulling from satisfaction
                // If continuing, add items from our desired items
                // create copy of property for good measure.
                let mut property_copy = self.property.cheap_clone();
                // remove those items which have already been added to the offer
                for (&offer_id, &off_amount) in current_offer.iter() {
                    property_copy.remove_property(offer_id, off_amount, data);
                }
                property_copy.sift_all(data);
                // also create shortcut current to making GTFOing a bit faster.
                let mut completed = HashSet::new();
                let invalid = property_copy.desires // get our desires
                    .iter().enumerate()
                    .filter(|(_, x)| x.satisfaction == 0.0) // skip those which are already empty
                    .map(|(id, _)| id).collect_vec(); // and collect those desire's idxs into a list
                for idx in invalid.into_iter() { completed.insert(idx); } // put them into the completed hashset.
                // start our loop, walking down our desires
                let mut prev = DesireCoord { tier: self.property.highest_tier, 
                    idx: self.property.desires.len() };
                while let Some(coord) = self.property.walk_down_tiers(&prev) {
                    // if no remaining possible desires, gtfo
                    if completed.len() == property_copy.desires.len() { break; }
                    // update previous with current for the next loop.
                    prev = coord;
                    if completed.contains(&coord.idx) {
                        continue; // if in completed, then skip.
                    }
                    // get the desire from the copy.
                    let current_desire = property_copy.desires
                        .get(coord.idx).unwrap().clone();
                    if current_desire.satisfaction_at_tier(coord.tier) == 0.0 {
                        // desire was reached early, probably.
                        continue;
                    }
                    // satisfaction lost is always equal to the current tier and the amount of satisfaction at that tier
                    let hypo_loss = TieredValue { tier: coord.tier, 
                        value: property_copy.desires.get(coord.idx).unwrap()
                            .satisfaction_at_tier(coord.tier) };
                    // if the satisfaction lost is too much for us to handle, try the next.
                    if (hypo_loss + sat_lost) > sat_gain {
                        // no lower tier will be better as it will always release the same or greater 
                        // amount of satisfaction, but at a lower tier, thus more expensive.
                        completed.insert(coord.idx);
                        continue;
                    }
                    // if a valid loss, add to sat lost
                    sat_lost += hypo_loss;
                    // copy processes for possible want releasing
                    // let mut processes_changed = property_copy.process_plan.clone();
                    // release the desire and the resources released
                    // TODO fix seemingly nondetermenistic results.
                    let mut released = property_copy
                        .release_desire_at(&coord, market, data);
                    // remove from those changed processes, clearing out 0.0s
                    /* DEBUG commented out to use release instead.
                    for (&id, &amount) in property_copy.process_plan.iter() {
                        processes_changed.entry(id)
                        .and_modify(|x| *x -= amount); // subtract the new process plan
                        if processes_changed[&id] == 0.0 { // if no change, remove entirely.
                            processes_changed.remove(&id);
                        }
                    } 
                    // get those resources which were actually used to satisfy the current desire and amount.
                    let result = match current_desire.item {
                        DesireItem::Want(_) => {
                            // try to remove the released products from our property.
                            self.property.remove_properties(&released, data);
                            // go through each process and try to remove what they need, if we can
                            for (proc_id, iterations) in processes_changed.iter() {
                                let process = data.processes.get(proc_id)
                                    .expect("Process not found.");
                                // add specific products up to the expectation.
                                for &input in process.input_products().iter() {
                                    // get how much we are expected to use
                                    let expectation = input.amount * iterations;
                                    // add to actual_release
                                    actual_released_products.insert(input.item.unwrap(), expectation);
                                }
                                // do the same for class products, selecting whatever we can
                                for &input in process.inputs().iter()
                                .filter(|x| x.item.is_class()) {
                                    // get how much it should want.
                                    let mut expectation = input.amount * iterations;
                                    // get class products
                                    let class_products = data.product_classes
                                        .get(&input.item.unwrap()).unwrap();
                                    // select class members up to our expectation from released products.
                                    for (product, amount) in released.iter()
                                    .filter(|(id, _)| class_products.contains(&id)) {
                                        // get how much is available from release capped at our current expectation
                                        let mut shift = amount.min(expectation);
                                        // remove what we're already planning on adding in extra release
                                        shift -= actual_released_products.get(product).unwrap_or(&0.0);
                                        // then add back to actual
                                        actual_released_products.entry(*product)
                                        .and_modify(|x| *x += shift)
                                        .or_insert(shift);
                                        // and reduce current expectation
                                        expectation -= shift;
                                    }
                                }
                            }

                            released
                        },
                        DesireItem::Class(class_id) => {
                            // get how much we need to remove
                            let mut remove = current_desire.satisfaction_at_tier(coord.tier);
                            // get all products which satisfy this desire.
                            let mates = data.product_classes.get(&class_id)
                                .expect("Class Id Not found.");
                            // prep both what we are releasing and how much we've released for the counting.
                            let mut actual_release = HashMap::new();
                            for (&prod_id, amount) in released.iter() // get what we have available
                            .filter(|(id, _)| mates.contains(&id)) { // and are members of the class
                                let shift = amount.min(remove); // how much we can shift up to remaining removal.
                                remove -= shift; // remove from shift for future purposes
                                actual_release.insert(prod_id, shift); // add to actual release.
                                if remove == 0.0 {
                                    break; // if nothing left to remove, get out of here.
                                }
                            }
                            actual_release
                        },
                        DesireItem::Product(prod_id) => {
                            // specific product, get only those items
                            let mut actual_release = HashMap::new();
                            // remove up to the current satisfaction at our current tier.
                            actual_release.insert(prod_id, released.get(&prod_id).unwrap()
                            .min(current_desire.satisfaction_at_tier(coord.tier)));
                            actual_release
                        },
                    };
                    */
                    // get how much we released
                    let mut amv_released = 0.0;
                    for (id, amount) in released.iter() {
                        amv_released += market.get_product_price(id, 1.0) * amount;
                    }
                    if current_offer_amv + amv_released > purchase_price {
                        // if greater than our target, reduce to the minimum required
                        let excess = current_offer_amv + amv_released - purchase_price;
                        let perfect_rat = 1.0 - excess / amv_released;
                        for (_, amount) in released.iter_mut() {
                            // divide by the 'prefect' ratio, and round up.
                            *amount = (*amount * perfect_rat).ceil();
                        }
                    }
                    // with the products specifically released in result, try adding them to the offer.
                    let mut amv_released = 0.0;
                    for (id, amount) in released.into_iter() {
                        current_offer.entry(id)
                            .and_modify(|x| *x += amount)
                            .or_insert(amount);
                        amv_released += market.get_product_price(&id, 1.0) * amount;
                    }
                    current_offer_amv += amv_released;
                    // check that the amv offered is enough or that the satisfaction lost is too much.
                    if current_offer_amv  > purchase_price {
                        break;
                    }
                    // if it's not enough, continue onto the next loop.
                }
            }
            // get an oppinion estimate from how much satisfaction we are giving up vs 
            let offer_result = Pop::offer_result_selector(sat_gain, sat_lost);
            // if current_amv_offer is below our target, reduce our buy target appropriately.
            // only reduce if the seller is a firm and thus unlikely to accept less.
            let final_target = if purchase_price > current_offer_amv && seller.is_firm() {
                let current_purchase_amount = current_offer_amv / price;
                current_purchase_amount.floor()
            } else {
                purchase_quantity
            };
            // after the previous section, we either have enough AMV to try and purchase,
            // or ran out of options which wouldn't overdraw our satisfaction.
            // send offer and wait for response
            self.send_buy_offer(rx, tx, sought_product, seller, &current_offer, offer_result, final_target);
            // TODO add in possibility of rejection here. For now they will always try, if nothing else.

            // deal with responses
            let response = self.specific_wait(rx, &vec![
                ActorMessage::SellerAcceptOfferAsIs { buyer: ActorInfo::Firm(0), 
                    seller: ActorInfo::Firm(0), 
                    product: 0, 
                    offer_result: OfferResult::Cheap },
                ActorMessage::OfferAcceptedWithChange { buyer: ActorInfo::Firm(0),
                    seller: ActorInfo::Firm(0), 
                    product: 0, 
                    quantity: 0.0, 
                    followups: 0 },
                ActorMessage::RejectOffer { buyer: ActorInfo::Firm(0),
                    seller: ActorInfo::Firm(0), 
                    product: 0 },
                ActorMessage::CloseDeal { buyer: ActorInfo::Firm(0),
                    seller: ActorInfo::Firm(0), 
                    product: 0 }
            ]);
            // summarize the exchange in full for later use
            // invert our current offer so those are subtracted.
            let mut resulting_change = HashMap::new();
            for (&prod, &quant) in current_offer.iter() {
                resulting_change.insert(prod, -quant);
            }
            // add how much we bought to the resulting_change.
            resulting_change.insert(sought_product, final_target);

            // if accepted, complete exchange
            // if outright rejected, leave
            match response {
                ActorMessage::SellerAcceptOfferAsIs { buyer, 
                seller, 
                product,
                offer_result: _ } => {
                    let _gain = self.property.add_products(&resulting_change, data);
                    self.property.record_exchange(resulting_change);
                    self.property.record_purchase(product, current_offer_amv, 
                        self.standard_shop_time_cost());
                    // send back close
                    self.push_message(rx, tx, ActorMessage::FinishDeal { buyer, seller, product });
                    return BuyResult::Successful;
                },
                ActorMessage::OfferAcceptedWithChange { buyer, 
                seller, 
                product, 
                followups, 
                quantity } => {
                    // TODO This is not tested or checked just yet. Until firms sell logic and change logic is done, it's not going to be touched.
                    // offer is accepted, but there is a change in the exchange, get the change
                    for (prod, quant) in self.retrieve_exchange_return(rx, tx, seller, followups) {
                        // add to resulting change to remove it from our losses
                        resulting_change.entry(prod)
                        .and_modify(|x| *x += quant)
                        .or_insert(quant);
                        current_offer_amv -= market.get_product_price(&prod, 1.0) * quant;
                    }
                    resulting_change.insert(product, quantity);
                    // update the effective cost and expenditure in satisfaction and amv
                    let new_cost = market.get_product_price(&product, 1.0) * quantity;
                    let new_sat_change = self.property.predict_value_changed(&resulting_change, data);
                    // given new sat decide whether we'll accept or reject the exchange and respond.
                    if new_sat_change.value < 0.0 { // if it results in a satisfaction decline, reject.
                        self.push_message(rx, tx, ActorMessage::RejectPurchase { 
                            buyer: self.actor_info(), 
                            seller, 
                            product, 
                            price_opinion: OfferResult::Rejected });
                        return BuyResult::CancelBuy;
                    } else { // if still positive satisfaction change, accept.
                        self.property.add_products(&resulting_change, data);
                        self.property.record_exchange(resulting_change);
                        self.property.record_purchase(product, current_offer_amv, self.standard_shop_time_cost());
                        self.push_message(rx, tx, ActorMessage::FinishDeal { buyer, seller, product });
                        return BuyResult::Successful;
                    }
                },
                ActorMessage::RejectOffer { buyer, 
                seller, 
                product } => {
                    // TODO add some method of retrying, either recursing, or entering a special rebuy function.
                    self.property.record_purchase(product, 0.0, self.standard_shop_time_cost());
                    return BuyResult::NotSuccessful { reason: OfferResult::Rejected };
                },
                ActorMessage::CloseDeal { .. } => {
                    // Deal closed 
                    self.property.record_purchase(sought_product, 0.0, self.standard_shop_time_cost());
                    return BuyResult::SellerClosed;
                },
                _ => panic!("Incorrect Response. Impossible to get here.")
            }
            // todo if haggling is done, it would be done here.
        }

        panic!("Standard Buy: This should never be reached, Specific Wait has returned an incorrect MSG.")
    }

    /// # Retrieve Change
    /// 
    /// Like Send Buy offer, this instead rocieves change if
    /// ActorMessage::OfferAcceptedWithChange was recieved.
    fn retrieve_exchange_return(&mut self, 
    rx: &mut Receiver<ActorMessage>, 
    tx: &Sender<ActorMessage>, 
    seller: ActorInfo,
    followups: usize) -> HashMap<usize, f64> {
        let mut result = HashMap::new();
        for _ in (0..followups).rev() {
            let response = self.specific_wait(rx, &vec![
                ActorMessage::ChangeFollowup { buyer: ActorInfo::Firm(0), 
                    seller: ActorInfo::Firm(0), 
                    product: 0, 
                    return_product: 0, 
                    return_quantity: 0.0, 
                    followups: 0 }
            ]);
            // TODO pick up here.
            if let ActorMessage::ChangeFollowup { buyer, 
            seller: s, 
            product, 
            return_product, 
            return_quantity, 
            followups: follows } = response {
                result.insert(return_product, return_quantity);
                debug_assert!(buyer == self.actor_info());
                debug_assert!(s == seller);
                debug_assert!(follows == followups);
            } else { panic!("Recieved something we shouldn't have.") }
        }
        result
    }

    fn offer_result_selector(sat_gain: TieredValue, sat_lost: TieredValue) -> OfferResult {
        let sat_ratio = sat_lost / sat_gain;
        if sat_ratio > constants::TOO_EXPENSIVE { OfferResult::TooExpensive }
        else if sat_ratio > constants::EXPENSIVE { OfferResult::Expensive }
        else if sat_ratio > constants::OVERPRICED { OfferResult::Overpriced }
        else if sat_ratio > constants::REASONABLE { OfferResult::Reasonable }
        else if sat_ratio > constants::CHEAP { OfferResult::Cheap }
        else { OfferResult::Steal }
    }

    /// # Emergency Buy
    ///
    /// Emergency buy means that we NEED the product being requested asap.
    ///
    /// This removes the sanctity of all items in keep and offers everything
    /// less important than that item (of higher tier)
    /// TODO Currently not built, should be slightly simpler version of Standard Buy.
    /// TODO this is on the backburner, 
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
        self.property.market_wealth(history)
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
    ///
    /// TODO this will likely need to change with the property update, but does still currently function.
    pub fn create_offer(&self, product: usize, target: f64,
    records: &HashMap<usize, PropertyInfo>, data: &DataManager,
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
    pub fn consume_goods(&mut self, _data: &DataManager, _history: &MarketHistory) {
        todo!("This when we get back to it.")
    }

    /// # Decay Goods
    ///
    /// Decay goods goes through all of our current products and wants and
    /// decays, reduces, or otherwise checks them for failure.
    ///
    /// Any products lost this way are recorded as losses in that product's knowledge.
    ///
    /// TODO when upgrading to add rolling, add RNG back as a parameter.
    pub fn decay_goods(&mut self, data: &DataManager) {
        self.property.decay_goods(data);
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
        self.property.add_property(TIME_ID, (self.breakdown_table.total as f64) *
            24.0 * self.breakdown_table.average_productivity(), data);

        // started up, so wait for the first message.
        match rx.recv().expect("Channel Broke.") {
            ActorMessage::StartDay => (), // wait for start day, throw otherwise.
            _ => panic!("Pop Recieved something before Day Start. Don't do something before the day starts.")
        }
        // precalculate our plans for the day based on yesterday's results and
        // see if we want to sell and what we want to sell.
        self.property.sift_specific_products();
        self.is_selling = if self.property.is_disorganized {
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