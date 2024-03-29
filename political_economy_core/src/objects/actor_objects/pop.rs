//! The storage unit of population groups.
//!
//! Used for any productive, intellegent actor in the system. Does not include animal
//! populations.
use core::panic;
use std::{collections::{HashMap, HashSet, VecDeque}, fmt::Debug};

use barrage::{Sender, Receiver};
use itertools::Itertools;

use crate::{
    constants::{self, ACP_MAX_HARD_REDUCTION_FACTOR, ACP_MAX_SOFT_REDUCTION_FACTOR, ACP_MIN_REDUCTION_FACTOR, OVERSPEND_THRESHOLD, SHOPPING_TIME_PRODUCT_ID, TIME_PRODUCT_ID}, 
    data_manager::DataManager, 
    demographics::Demographics, 
    objects::{
        actor_objects::property::{
            DesireCoord,
            TieredValue
        },
        data_objects::{
            item::Item,
            product::ProductTag,
            want_info::WantInfo
        },
        environmental_objects::market::MarketHistory,
        actor_objects::buyer::Buyer,
        demographic_objects::pop_breakdown_table::PopBreakdownTable
    }
};

use super::{
    actor::Actor, 
    actor_message::{
        ActorInfo, 
        ActorMessage,
        ActorType, 
        FirmEmployeeAction, 
        OfferResult
    }, 
    buy_result::BuyResult, 
    property::Property, 
    property_info::PropertyInfo, 
    seller::Seller, 
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
    // pub skill: usize,
    /// The lower bound of their skill level.
    // pub lower_skill_level: f64,
    /// the upper bound of their skill level spread.
    // pub higher_skill_level: f64,
    /// The total desires and property of the pop.
    ///
    /// TODO Food For Thought. We include 2 infinite desires in all pops, wealth and Leisure, which act as sinks and help us balance our buy priorities. More thought is needed.
    pub property: Property,
    /// A breakdown of the Population's demographics.
    pub breakdown_table: PopBreakdownTable,

    /// Whether the pop is selling or not.
    pub is_selling: bool,

    // todo Mood related stuff goes here.

    /// The current total satisfaction of the pop.
    /// 
    /// This is updated after free_time().
    pub current_sat: TieredValue,
    /// The previous satisfaction of the pop from yesterday.
    /// 
    /// This is updated after free_time().
    pub prev_sat: TieredValue,
    /// The hypothetical satisfaction target this pop believes it could reach 
    /// with perfect information and amv equal trades.
    /// 
    /// This updates after free_time().
    pub hypo_change: TieredValue,

    /// Backlogs of messages, to help keep things clear.
    pub backlog: VecDeque<ActorMessage>,
}

// TODO #66 issue Alter to make testing easier through Inverting functions which depend on message passing. In particular, free_time, shopping_loop, try_to_buy, and standard_buy
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
            // try to clear out prior msgs before sending.
            self.msg_catchup(rx);
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
        for (offer_item, offer_quantity) in offer.iter()
        .sorted_by(|a, b| a.0.cmp(b.0)) {
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

    /// # Specific wait function.
    ///
    /// First checks the backlog for the message requested, if it finds it, 
    /// it extracts it and returns that. If it doesn't, then it 
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
        // first, look through the backlog to ensure we haven't already gotten what we're looking for.
        for idx in 0..self.backlog.len() {
            if find.iter()
            .any(|x| {
                std::mem::discriminant(x) == std::mem::discriminant(&self.backlog[idx])
            }) {
                return self.backlog.remove(idx).expect("Somehow walked off end of backlog.");
            }
        }
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
                    product: TIME_PRODUCT_ID,
                    amount: self.property.work_time
                    });
                // and remove that time from our property as well
                self.property.remove_property(TIME_PRODUCT_ID, self.property.work_time, data);
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
                        amount: amount.total_current
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
                    self.property.want_store.entry(want)
                        .or_insert(WantInfo::new(0.0))
                        .add(amount);
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
    pub fn free_time(&mut self, rx: &mut Receiver<ActorMessage>, tx: &mut Sender<ActorMessage>,
    data: &DataManager,
    market: &MarketHistory,
    shopping_loop: fn(&mut Pop, 
        &mut Receiver<ActorMessage>, 
        &mut Sender<ActorMessage>, 
        &DataManager, 
        &MarketHistory,)) 
    {
        // start by organizing our property, reserve everything for our desires.
        self.prev_sat = self.property.sift_all(data);

        // After reserving for desires directly, measure excess wealth in AMV and 'sift' that.
        let mut surplus = HashMap::new();
        let mut amv_surplus = 0.0;
        for (&product, quant) in self.property.property.iter() {
            let amount = quant.available();
            surplus.insert(product, amount);
            amv_surplus += amount * market.get_product_price(&product, 1.0);
        }
        self.hypo_change = self.property.satisfaction_from_amv(amv_surplus, market);

        // put up our surplus for sale if we desire it
        if self.is_selling {
            for (&product, &amount) in surplus.iter()
            .filter(|(&id, _)| !data.products.get(&id).expect("Product Not found!").tags
            .contains(&ProductTag::NonTransferrable)) { // put everything that is transferrable up for sale.
                if amount.floor() == 0.0 {
                    continue;
                }
                self.push_message(rx, tx,
                ActorMessage::SellOrder { sender: self.actor_info(),
                    product,
                    quantity: amount.floor(), // floor it because only whole units may be transfered.
                    amv: market.get_product_price(&product, 1.0) });
                // non-firms offer at the current market price.
            }
        }

        // Go shopping to get more stuff
        shopping_loop(self, rx, tx, data, market);

        // measure overall success
        self.current_sat = self.property.sift_all(data);
        // this helps define economic sentiment (prospering/decaying)
        // todo modify mood based on results here.

        // after we run out of stuff to buy, send finished and leave, consumption comes later
        self.push_message(rx, tx, ActorMessage::Finished { sender: self.actor_info() });
        self.active_wait(rx, tx, data, market, &vec![
            ActorMessage::AllFinished
        ]);
    }

    /// # Shopping Loop
    ///
    /// Shopping loop function is a helper for free time and adjacent functions.
    ///
    /// It simply walks up it's desires, buying as it can.
    ///
    /// It gets the item and plans how much of it it wants to try and buy
    /// based on it's type.
    /// - if it's a class or specific product, it aims for a single shopping
    ///   trip for the whole order.
    /// - if it's a want it selects a random process suggested by the market
    ///   to get it, and tries to set up as many trips as products in it's
    ///   list.
    ///
    /// Currently, this does not set target amounts, only the targeted products.
    ///
    /// With the number of buys it needs and targets, it extracts time for
    /// shopping, then it spends it on shopping trips.
    ///
    /// It goes out for it's desired buys. Regardless of success or failure,
    /// it tries to buy each of it's things. AFter buying everything, it
    /// sifts it's goods again,
    ///
    /// TODO Sift Improvement Location: When Sifting is upgraded to not be destructive, come back here and upgrade the time extraction.
    /// TODO consider adding a 'grocery list' prebuy option which gets the most consistently bought items to improve efficiency and reduce the number of times it needs to go out and buy.
    /// TODO Once possible, allow this to buy multiple items at the same store before moving on to the next.
    pub fn shopping_loop(pop: &mut Pop, rx: &mut Receiver<ActorMessage>,
    tx: &mut Sender<ActorMessage>,
    data: &DataManager, market: &MarketHistory) {
        // with everything reserved begin trying to buy more stuff
        // prepare current desire for first possible purchase.
        let mut next_desire = pop.property.get_first_unsatisfied_desire();

        // setup our sanity check escape mechanism, primarily if we find ourselves 
        let mut completed_desires = HashSet::new();

        // preemptively get shopping time cost
        let shopping_time_cost = pop.standard_shop_time_cost();

        // also initialize shopping time, none should exist prior to here.
        let mut available_shopping_time = 0.0;
        // todo when the ablitiy to buy shopping time is available, add a first pass here.
        // start our buying loop
        let mut prev = None;
        let mut retry = false;
        // Should have our current desire coords in next_desire
        while let Some(curr_desire_coord) = next_desire {
            if completed_desires.len() == pop.property.desires.len() {
                break; // if all desires are marked complete, gtfo.
            }
            if prev == next_desire && !retry { // if we are retrying, note that
                retry = true;
            } else if prev == next_desire && retry { 
                // if already retried and came back for more, mark desire as complete
                // and move on to the next.
                completed_desires.insert(curr_desire_coord.idx);
                next_desire = pop.property.walk_up_tiers(next_desire);
                retry = false;
                continue;
            }
            // check that our desire is not in completed.
            if completed_desires.contains(&curr_desire_coord.idx) {
                prev = next_desire;
                next_desire = pop.property.walk_up_tiers(next_desire);
                continue;
            }
            // check that we have enough time to go shopping (either time itself or shopping time)
            if let Some(time) = pop.property.property.get(&TIME_PRODUCT_ID) {
                if time.available() < shopping_time_cost {
                    break;
                }
            } else if let Some(shopping_time) = pop.property.property.get(&SHOPPING_TIME_PRODUCT_ID) {
                if shopping_time.available() < shopping_time_cost {
                    break;
                }
            } else {
                // if we don't have either pure time or shopping time, stop.
                break;
            }
            // start by getting our desire
            let curr_desire = pop.property.desires
                .get(curr_desire_coord.idx).unwrap().clone();
            // if the current desire is already satisfied for wahtever reason move on
            if curr_desire.satisfied_at_tier(curr_desire_coord.tier) {
                // get the next, and continue.
                next_desire = pop.property.walk_up_tiers(next_desire);
                continue;
            }

            // get our current desire target
            let sat_target = curr_desire.missing_satisfaction(curr_desire_coord.tier);
            let current_desire_item = &curr_desire.item;
            let mut buy_targets: Vec<(usize, f64)> = vec![];
            // get the items we need to buy and how many.
            match current_desire_item {
                Item::Want(id) => { // for wants, we need to get the product inputs.
                    // if it's a want, go to the most common satisfaction
                    // of that want in the market.
                    pop.push_message(rx, tx,
                        ActorMessage::FindWant { want: *id, sender: pop.actor_info() });
                    // wait for the market to respond with either it's suggested process, or failure.
                    let result = pop.active_wait(rx, tx, data, market,
                        &vec![
                            ActorMessage::FoundWant { buyer: ActorInfo::Firm(0), want: 0, process: 0 },
                            ActorMessage::WantNotFound { want: 0, buyer: ActorInfo::Firm(0) }
                        ]);
                    if let ActorMessage::FoundWant { process, .. } = result {
                        // get the process suggested
                        let process_info = data.processes.get(&process).unwrap();
                        let needs = process_info.inputs_and_capital();
                        let mut things_to_get = 0.0;
                        // get what needs to be gotten
                        for part in needs.iter()
                        {
                            match part.item { // add the part item to our buy targets
                                Item::Want(_) =>
                                    panic!("Use/Consume should not have wants."),
                                Item::Class(id) => {
                                    // get class item which satisfies.
                                    let result = pop.find_class_product(rx, tx, id, data, market);
                                    if let Some(product) = result {
                                        buy_targets.push((product,
                                            part.amount * sat_target));
                                            things_to_get += 1.0;
                                    } // else don't add anything we can't use.
                                },
                                Item::Product(id) => {
                                    buy_targets.push((id, sat_target * part.amount));
                                    things_to_get += 1.0;
                                },
                            }
                        }
                        things_to_get
                    } else if let ActorMessage::WantNotFound { .. } = result {
                        // if the want is not found in the market, then move on to the next desire
                        0.0
                    } else { panic!("Should not be here.") }
                },
                Item::Class(id) => { // for class, any item of the class will be good enough.
                    let result = pop.find_class_product(rx, tx, *id, data, market);
                    if let Some(product) = result {
                        buy_targets.push((product, sat_target));
                        1.0
                    } else {
                        0.0
                    }
                },
                Item::Product(_) => {
                    buy_targets.push((current_desire_item.unwrap(), sat_target));
                    1.0
                }, // for specific product. only one item will be needed.
            };
            // then sift up to this desire point to free up excess resources.
            // TODO when sifting is improved, drop this.
            pop.property.sift_up_to(&curr_desire_coord, data);

            // prepare a check to see if we want to move on or not.
            let mut go_to_next = true;
            let mut emergency_buy = false;

            // Do one buy at a time, ignoring if we have enough time for all trips or not
            // TODO improve this to actually peak ahead to see if it can go shopping enough to get what it needs and satisfy that desire.
            for (buy_target, mut buy_quantity) in buy_targets {
                // check if we need to get the item or not
                // TODO consider testing this check specifically.
                let property_info = pop.property.property
                    .entry(buy_target)
                    .or_insert(PropertyInfo::new(0.0));
                if property_info.available() >= buy_quantity {
                    continue; // if not skip to next.
                } else if (property_info.upper_target - property_info.available()) > buy_quantity { 
                    // if our current target_quantity is less than our remaining max target, upgrade to the remainder
                    buy_quantity = property_info.upper_target - property_info.available();
                }
                // get a trip of time worth
                // TODO update to take more dynamic time payment into account.
                // TODO when purchasing shopping time is available, add an option for that here.
                available_shopping_time += pop.property.get_shopping_time(
                    shopping_time_cost - available_shopping_time,
                    data, market, Some(curr_desire_coord));
                // check that it's enough time to go out buying
                if shopping_time_cost > available_shopping_time {
                    // if we don't have enough time to go shopping for this desire we likely won't be able to go shopping for
                    // anything, so add the excess shopping time to our property and gtfo (consired allowing it to be refunded.)
                    // todo refund shopping time here as it hasn't actually been spend yet 
                    pop.property.add_property(SHOPPING_TIME_PRODUCT_ID, available_shopping_time, data);
                    break;
                } else {
                    // since we have enough time, go shopping.
                    // expend then return excess shopping time to property
                    let remaining_shop_time = available_shopping_time - shopping_time_cost;
                    if remaining_shop_time > 0.0 {
                        pop.property.add_property(SHOPPING_TIME_PRODUCT_ID, remaining_shop_time, data);
                    }
                    // regardless of our success or failure, add it to the cost.
                    pop.property.property.entry(buy_target)
                    .and_modify(|x| x.time_cost += shopping_time_cost)
                    .or_insert({
                        let mut temp = PropertyInfo::new(0.0);
                        temp.time_cost += shopping_time_cost;
                        temp
                    });
                    // TODO make use of buy_result instead of ignoring it.
                    let buy_result = pop.try_to_buy(rx, tx, data, market, 
                        buy_target, buy_quantity);
                    // the result if positive should do pretty much nothing. Target Success.
                    // If failure, we want to reduce the target by some measure, how much, will likely depend on the kind of failure.
                    match buy_result {
                        BuyResult::CancelBuy => { // do nothing, go to the next desire to possibly try again
                            go_to_next = true;
                            },
                        BuyResult::NotSuccessful { reason } => {
                            match reason {
                                OfferResult::Incomplete => { // deal cancelled preemtively for some reason, assume nothing good
                                    go_to_next = true;
                                }, 
                                OfferResult::Rejected | // deal was not in our favor, go shopping elsewhere.
                                OfferResult::TooExpensive | // Too expensive at current store
                                OfferResult::OutOfStock => { // incomplete due to insuffecient stock from buyer, try again
                                    go_to_next = false; // try again
                                },
                                OfferResult::NotInMarket => {
                                    go_to_next = false;
                                    emergency_buy = true;
                                }, // no one seems to be selling it in the market currently
                                _ => {
                                    go_to_next = false;
                                } // the rest are unlikely to be used here, try again.
                            }
                        },
                        BuyResult::SellerClosed => {
                            go_to_next = false;
                        }, // Hard Closed by seller, they possibly didn't like the deal and don't have room to haggle.
                        BuyResult::Successful => {
                            go_to_next = true;
                        }, // good. We like this
                        BuyResult::NoTime => {
                            break;
                        }, // Ran out of time, likely from haggling
                    }
                }
            }
            if emergency_buy {
                // do an emergency buy, because it's necissary.
                // push emergency find
                // get response
                // enter standard buy for that (if possible)
                // 
            }
            // always set our previous desire
            prev = next_desire;
            // check if the next tier is beyond the end of our current desire.
            if curr_desire.past_end(curr_desire_coord.tier + 1) {
                completed_desires.insert(curr_desire_coord.idx);
            }
            if go_to_next {
                // the next desire
                next_desire = pop.property.walk_up_tiers(next_desire);
            }
        }
    }

    /// # Find Class Product
    ///
    /// Helper Function, summarizes the sending of a class find and responding to it.
    /// returns the product which was returned. If no product was found it returns None instead.
    ///
    /// Not meant for public use, public for testing
    pub fn find_class_product(&mut self, rx: &mut Receiver<ActorMessage>,
    tx: &Sender<ActorMessage>, class: usize, data: &DataManager,
    market: &MarketHistory) -> Option<usize> {
        self.push_message(rx, tx,
            ActorMessage::FindClass { class,
                sender: self.actor_info() });
            let result = self.active_wait(rx, tx, data, market,
                &vec![
                    ActorMessage::FoundClass { buyer: ActorInfo::Firm(0),
                        product: 0 },
                    ActorMessage::ClassNotFound { class: 0,
                        buyer: ActorInfo::Firm(0) }
                ]);
            if let ActorMessage::FoundClass { buyer: _, product } = result {
                Some(product)
            } else if let ActorMessage::ClassNotFound { class: _,
            buyer: _ } = result {
                None
            } else { unreachable!("Should not reach here!"); }
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
                if seller == self.actor_info() {
                    // TODO When change is possible, deal with it here.
                    self.standard_sell(rx, tx, data, market, product, buyer);
                } else {
                    debug_assert!(false, "This message should only ever be recieved here while we are a seller.");
                }
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
                self.property.want_store.entry(want)
                    .or_insert(WantInfo::new(0.0))
                    .add(amount);
                return None;
            },
            ActorMessage::WantSplash { want, amount, .. } => {
                self.property.want_store.entry(want)
                    .or_insert(WantInfo::new(0.0))
                    .add(amount);
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
    pub fn try_to_buy(&mut self,
    rx: &mut Receiver<ActorMessage>,
    tx: &Sender<ActorMessage>,
    data: &DataManager,
    market: &MarketHistory,
    product: usize,
    buy_target: f64) -> BuyResult {
        let price_estimate = self.property.property
            .entry(product)
            .or_insert(PropertyInfo::new(0.0))
            .amv_unit_estimate;
        // with budget gotten, check if it's feasable for us to buy (market price < 2.0 budget)
        let market_price = market.get_product_price(&product, 0.0);
        if market_price > (price_estimate * constants::HARD_BUY_CAP) {
            // if unfeaseable, at current market price, cancel.
            return BuyResult::CancelBuy;
        }

        // since the current market price is within our budget, try to look for it.
        self.push_message(rx, tx, ActorMessage::FindProduct { product: product, sender: self.actor_info() });
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
            BuyResult::NotSuccessful { reason: OfferResult::NotInMarket }
        }
        else if let ActorMessage::FoundProduct { seller, .. } = result {
            self.standard_buy(rx, tx, data, market, buy_target, seller)
        }
        else { unreachable!("Somehow did not get FoundProduct or ProductNotFound."); }
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
    buy_target: f64,
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
            // buy up to the buy target or the remaining target (whichever is higher) or +1 if neither is high.
            let purchase_quantity = if product_info.remaining_target().max(buy_target) > 1.0 {
                stock_available.min(product_info.remaining_target().max(buy_target))
            } else { // if no remaining target, just get 1 unit capped at stock available.
                stock_available.min(1.0)
            };
            // Get how much this purchase would increase our satisfaction by.
            let mut sat_gain = self.property.predict_value_gained(sought_product,
                purchase_quantity, data);
            let mut sat_lost = TieredValue { tier: 0, value: 0.0 };
            // get the total AMV price of the purchase
            let purchase_price = purchase_quantity * price;
            // First use any property which we don't have a desire to keep to try and use them.
            for (product, info) in self.property.property.iter()
            .filter(|(_, info)| info.unreserved > 0.0) // get property which isn't reserved.
            .sorted_by(|a, b| {
                a.0.cmp(b.0) // sort by ID first to stabilize results ?
            })
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
                        if capped == 0.0 { // if we hit zero here
                            current_offer.remove(product); // remove entirely
                            break; // and break loop.
                        }
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
                            // todo recheck this to ensure it always end up no greater than the orignial amount and greater than the target.
                            *amount = (*amount * perfect_rat + 1.0).floor();
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
            // if current_amv_offer is below our target, reduce our buy target appropriately.
            // only reduce if the seller is a firm and thus unlikely to accept less.
            let final_target = if purchase_price > current_offer_amv && seller.is_firm() {
                let current_purchase_amount = current_offer_amv / price;
                let end_purchase = current_purchase_amount.floor();
                sat_gain = self.property.predict_value_gained(sought_product,
                    end_purchase, data);
                end_purchase
            } else {
                // if seller isn't a firm, try anayway, they may accept.
                purchase_quantity
            };
            // sanity check that our sat_gained is still higher than sat_lost
            if sat_gain < sat_lost {
                // if the final target results in a net loss in satisfaction, cancel the buy.
                self.push_message(rx, tx, ActorMessage::RejectPurchase { buyer: self.actor_info(),
                    seller: seller, product: sought_product, price_opinion: OfferResult::TooExpensive });
                return BuyResult::NotSuccessful { reason: OfferResult::TooExpensive };
            }
            // get an opinion estimate from how much satisfaction we are giving up vs
            let offer_result = Pop::offer_result_selector(sat_gain, sat_lost);
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
                    self.property.record_purchase(product, current_offer_amv);
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
                        current_offer.entry(prod)
                        .and_modify(|x| *x -= quant)
                        .or_insert(quant);
                        current_offer_amv -= market.get_product_price(&prod, 1.0) * quant;
                    }
                    resulting_change.insert(product, quantity);
                    let new_gain = self.property.predict_value_gained(product, quantity, data);
                    let new_loss = self.property.predict_value_changed(&current_offer, data);
                    let new_loss = new_loss.shift_tier(50);
                    if new_gain < new_loss { // if it results in a satisfaction decline, reject.
                        self.push_message(rx, tx, ActorMessage::RejectPurchase {
                            buyer: self.actor_info(),
                            seller,
                            product,
                            price_opinion: OfferResult::Rejected });
                        // record expended time trying to buy the target
                        self.property.record_purchase(sought_product, 0.0);
                        return BuyResult::CancelBuy;
                    } else { // if still positive satisfaction change, accept.
                        self.property.add_products(&resulting_change, data);
                        self.property.record_exchange(resulting_change);
                        self.property.record_purchase(product, current_offer_amv);
                        self.push_message(rx, tx, ActorMessage::FinishDeal { buyer, seller, product });
                        return BuyResult::Successful;
                    }
                },
                ActorMessage::RejectOffer {
                product, .. } => {
                    // TODO add some method of retrying, either recursing, or entering a special rebuy function.
                    self.property.record_purchase(product, 0.0);
                    return BuyResult::NotSuccessful { reason: OfferResult::Rejected };
                },
                ActorMessage::CloseDeal { .. } => {
                    // Deal closed, no reason given by seller.
                    self.property.record_purchase(sought_product, 0.0);
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
    /// Like Send Buy offer, this instead recieves change if
    /// ActorMessage::OfferAcceptedWithChange was recieved.
    fn retrieve_exchange_return(&mut self,
    rx: &mut Receiver<ActorMessage>,
    _tx: &Sender<ActorMessage>,
    seller: ActorInfo,
    followups: usize) -> HashMap<usize, f64> {
        let mut result = HashMap::new();
        for expected_remainder in (0..followups).rev() {
            let response = self.specific_wait(rx, &vec![
                ActorMessage::ChangeFollowup { buyer: ActorInfo::Firm(0),
                    seller: ActorInfo::Firm(0),
                    product: 0,
                    return_product: 0,
                    return_quantity: 0.0,
                    followups: 0 }
            ]);
            if let ActorMessage::ChangeFollowup { buyer,
            seller: s,
            product: _,
            return_product,
            return_quantity,
            followups: follows } = response {
                result.insert(return_product, return_quantity);
                debug_assert!(buyer == self.actor_info());
                debug_assert!(s == seller);
                debug_assert!(follows == expected_remainder);
            } else { panic!("Recieved something we shouldn't have.") }
        }
        result
    }

    /// # Recieve Offer Followups
    /// 
    /// A shorthand method used to gather a string of messages for us. 
    /// 
    /// The values returned should be positive (ie, gained by the buyer)
    pub fn recieve_offer_followups(&mut self, rx: &mut Receiver<ActorMessage>, _tx: &Sender<ActorMessage>, buyer: ActorInfo, followups: usize) -> HashMap<usize, f64> {
        let mut result = HashMap::new();
        for expected_remainder in (0..followups).rev() {
            let response = self.specific_wait(rx, &vec![
                ActorMessage::BuyOfferFollowup { buyer: ActorInfo::Firm(0),
                    seller: ActorInfo::Firm(0),
                    product: 0,
                    offer_product: 0,
                    offer_quantity: 0.0,
                    followup: 0 }
            ]);
            if let ActorMessage::BuyOfferFollowup { buyer: b,
            seller,
            product: _,
            offer_product,
            offer_quantity,
            followup: follows } = response {
                result.insert(offer_product, offer_quantity);
                debug_assert!(seller == self.actor_info());
                debug_assert!(b == buyer);
                debug_assert!(follows == expected_remainder);
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
    /// TODO upgrade this to take in the possibility of charity and/or givaways.
    /// TODO currently, this costs the seller no time, and they immediately close out. This should be updated to allow the buyer to retry and/or the seller to lose time to the deal.
    /// TODO Currently does not do change, accepts offer or rejects, no returning change.
    pub fn standard_sell(&mut self, rx: &mut Receiver<ActorMessage>,
    tx: &Sender<ActorMessage>, data: &DataManager,
    market: &MarketHistory,
    product: usize, buyer: ActorInfo) {
        // TODO do time check here when seller time cost added.
        // check we have product.
        if !self.property.property.contains_key(&product) {
            // send OOS
            self.push_message(rx, tx, 
                ActorMessage::NotInStock { buyer, seller: self.actor_info(), product });
            return;
        }
        let mut ret: HashMap<usize, f64>;
        // we have recieved a found product with us as the seller.
        // check how much we are willing to offer in exchange,
        let available:f64;
        // extract and remove all product which does to satisfying desires above this level.
        if let Some(hard_sat_lvl) = self.property.hard_satisfaction {
            // knock off the top of our satisfaction
            self.property.sift_up_to(&DesireCoord { tier: hard_sat_lvl, 
                idx: self.property.desires.len() }, 
                data);
            // get how much that makes available
            available = self.property.property.get(&product).unwrap().available();
            // actually remove it
            self.property.remove_property(product, available, data);
        } else { // if no existing hard_sat
            // release everything
            self.property.unsift();
            // get available
            available = self.property.property.get(&product).unwrap().available();
            // then remove what's available.
            self.property.remove_property(product, available, data);
        };
        // set the price at the current market price (pops cannot set their own explicit AMV price)
        let price = market.get_product_price(&product, 1.0);
        // then send back the response yay or nay
        if available < 1.0 { // if nay (cannot sell fractions of a unit)
            // send OOS
            self.push_message(rx, tx, 
                ActorMessage::NotInStock { buyer, seller: self.actor_info(), product });
            // Add property back
            self.property.add_property(product, available, data);
            // then GTFO
            return;
        }  else { // if yay
            // send In Stock
            self.push_message(rx, tx, 
                ActorMessage::InStock { buyer, seller: self.actor_info(), 
                    product, price, quantity: available });
        } // message was sent
        // get response
        let response = self.specific_wait(rx, &vec![
            ActorMessage::RejectPurchase { buyer, seller: self.actor_info(), product: 0, price_opinion: OfferResult::Cheap },
            ActorMessage::BuyOffer { buyer, seller: self.actor_info(), product, 
                price_opinion: OfferResult::Cheap, quantity: 1.0, followup: 0 }
        ]);
        // return our property preemtively for possible extraction.
        self.property.add_property(product, available, data);

        if let ActorMessage::RejectPurchase { .. } = response {
            return; // if negative response gtfo
        } else if let ActorMessage::BuyOffer { buyer, seller: _, 
        product, price_opinion, quantity, followup } = response {
            // if valid check if the trade is worth it.
            ret = self.recieve_offer_followups(rx, tx, buyer, followup);
            ret.insert(product, -quantity);
            let value_change = self.property.predict_value_changed(&ret, data);
            if value_change.value < 0.0 { // if loss less than gain, reject offer
                self.push_message(rx, tx, 
                    ActorMessage::RejectOffer { buyer, 
                        seller: self.actor_info(), product });
                ret.clear(); // clear out 
            } else { // if gain greater than loss, accept offer
                // TODO consider adding change here. 
                self.push_message(rx, tx, 
                    ActorMessage::SellerAcceptOfferAsIs { buyer, seller: self.actor_info(), 
                        product, offer_result: price_opinion });
            }
        } else { unreachable!("Should never be reached."); }
        self.property.add_products(&ret, data);
    }

    /// # Consume Goods
    ///
    /// Our end of daily activities. Goes through our goods, consuming them
    /// and adding to our satisfaction as dictated by our plans (calculated by sifting)
    pub fn consume_goods(&mut self, data: &DataManager, _history: &MarketHistory) {
        self.property.consume_goods(data, _history);
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

    /// # Adapt Future Plan
    ///
    /// Adapt future plan takes our existing knowledge base and our results
    /// from todays buying, selling, and consuming to modify our plan for
    /// tomorrow.
    ///
    /// For now, all this does is alter min and max targets for our owned 
    /// property. 
    /// 
    /// ## Max Target Alterations
    /// 
    /// When we hit or overshoot the target, we the increase the max by half 
    /// the difference (round up), capping at the consumed + current total + 1 
    /// (see TODO below). If we are below the max we lower it by a fraction of 
    /// the difference (1/10th currently rounded up (to 0.0)). If below Min, we 
    /// reduce max by half the difference.
    /// 
    /// ## Min target Alterations
    /// 
    /// While consumed is above Min, we increase our min by a fraction of the
    /// difference (1/5 currently (rounded up)). This is capped at
    /// zero and max_target. If consumed is below min it lowers it by 1/5 also
    /// 
    /// TODO: Alter Max_target cap to be limited by both consumed about a new factor, Security Factor (how many days we want to build up)
    /// TODO Add mood modifier for going below minimum. (Uncertainty/fear/anger)
    pub fn adapt_future_plan(&mut self, _data: &DataManager,
    _history: &MarketHistory) {
        for (_id, info) in self.property.property.iter_mut() {
            let old_max = info.upper_target;
            let old_min = info.lower_target;
            let total_lost = info.consumed + info.lost;
            let peak = info.total_property + total_lost;
            let max_diff = peak - old_max;
            if max_diff >= 0.0 { // if at or above old max
                // add half the difference rounded up (min 1.0)
                let diff = (max_diff * constants::APC_MAX_GROWTH_FACTOR)
                    .ceil().max(1.0);
                info.upper_target += diff;
            } else if old_min < peak { // below max, but above peak
                // reduce by a fraction of the difference (min 0.0)
                let diff = (max_diff * ACP_MAX_SOFT_REDUCTION_FACTOR).ceil();
                info.upper_target += diff;
            } else { // below both max and min, reduce by half diff round down
                let diff = (max_diff * ACP_MAX_HARD_REDUCTION_FACTOR).floor();
                info.upper_target += diff;
            }

            // get the difference between total_lost and min and shift 
            let min_diff = ((total_lost - info.lower_target) * ACP_MIN_REDUCTION_FACTOR)
                .ceil();
            info.lower_target += min_diff;
        }
    }

    /// # Product Initialization
    ///
    /// Initializes a product for use. It sets up the product with no amount
    /// a max target of 1.0, and no other information.
    ///
    /// If product alread exists, it skips making it again.
    pub fn add_target(&mut self, product: usize, target: f64) {
        self.property.add_target(product, target);
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
    tx: &mut Sender<ActorMessage>,
    rx: &mut Receiver<ActorMessage>,
    data: &DataManager,
    _demos: &Demographics,
    history: &MarketHistory) {
        // before we even begin, add in the time we have for the day.
        self.property.add_property(TIME_PRODUCT_ID, (self.breakdown_table.total as f64) *
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
        } else {
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
        self.free_time(rx, tx, data, history, Pop::shopping_loop);

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