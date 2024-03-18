use core::panic;
use std::collections::{HashMap, HashSet};
use barrage::{Sender, Receiver};
use crossbeam::thread;
use itertools::Itertools;
use rand::{Rng, thread_rng};

use crate::{constants::{SALABILITY_THRESHOLD, STD_PRICE_CHANGE}, data_manager::DataManager, demographics::Demographics, objects::{actor_objects::{actor::Actor, actor_message::{ActorInfo, ActorMessage, OfferResult, WantSource}, firm::Firm, institution::Institution, pop::Pop, seller::Seller, state::State}, data_objects::item::Item}};
use crate::constants;

/// # The Market
/// 
/// A Market is a cohiesive unit of space in which time costs of transporting
/// people and goods (and services) are considered a non-issue. In a hardline,
/// low-tech limitation, this is roughly the distance an unaided human to
/// travel out and back within the same day.
/// 
/// This is not a universal rule, as the smallest size a market can actually
/// take is the equivalent to the smallest territory. This means that a
/// space based start could treat whole planets as unified markets immediately.
/// Starting at low tech on the other hand could go as low as the hypethetical
/// smallest map territory size of about 30km in radius.
/// 
/// Within a market are Pops, Firms, Institutions, and States all of whom
/// can act in the market, buying, selling, trading, and interacting with the
/// environment.
/// 
/// Pops are entirely contained in a Market, and can only exist or act in a 
/// singular market at any time. They can move and migrate between markets,
/// but this does not occur during a normal market day.
/// 
/// Firms are almost entirely contained in a single market. A larger, 
/// multimarket firm must have a lower ranked firm which is entirely contained
/// within the market they operate in. These child Firms then connect and
/// message their Parent for information, details, and orders. Day-to-day
/// activities like payments and production take place in the market here.
/// 
/// Institutions are Not limited to a singular market, instead they must have
/// local representatives to act within a market. This could be as simple as
/// an outpost or messenger who sticks around to observe and report, but
/// otherwise they must have a sub-component within a market to meaningfully 
/// act in any way.
/// 
/// States are much like institutions, and follow the same logic with one
/// addition. Since most state territory is mutually exclusive, a State must 
/// also have, create, or enforce a claim on a market. This is done by claiming
/// it's territory, rather than the market as a whole, a market makes claims
/// on other territory within it to be much cheaper for a state.
#[derive(Debug)]
pub struct Market {
    /// The unique id for the market.
    pub id: usize,
    /// The name of the market, typically named after the capital territory
    /// of the market.
    pub name: String,
    /// The firms within the market.
    pub firms: Vec<usize>,
    /// The pops within the market.
    pub pops: Vec<usize>,
    /// The institutions within the market.
    pub institutions: Vec<usize>,
    /// The States within the market.
    pub states: Vec<usize>,
    /// The Territories contained in the market.
    /// Needs to be the altered to take the map into account and
    /// ensure we don't kill ourselves with too many territories.
    pub territories: Vec<usize>,
    /// The Markets which are immediately adjacent to this market,
    /// This contains the ID and the connection type/data between them.
    pub neighbors: HashMap<usize, MarketConnection>,
    /// The open resources of the market, these are the trash items
    /// which are available for anyone to pick up, and includes surface
    /// resources from the environment.
    pub resources: HashMap<usize, f64>,
    // Available Nodes.

    /// The Current market prices in AMV.
    pub prices: HashMap<usize, f64>,
    /// The products which are available for sale, and how many of them.
    pub products_for_sale: HashMap<usize, f64>,
    /// how much of each proudct was demanded by buyers generally.
    /// May include double dipped demand.
    pub product_demanded: HashMap<usize, f64>,
    /// The products sold in this market so far.
    pub product_sold: HashMap<usize, f64>,
    /// The total products produced today.
    pub product_output: HashMap<usize, f64>,
    /// The total amount of products exchanged today.
    pub product_exchanged_total: HashMap<usize, f64>,
    /// The Salability of each item. Any itme above SALABILITY_THRESHOLD 
    /// is considered a currency for this market naturally. 
    pub salability: HashMap<usize, f64>,

    /// The Estimated price of various wants in the market, based on
    /// a weighted average for both products in processes and processes 
    /// used in the market.
    pub want_prices: HashMap<usize, f64>,
    /// How many times a want was requested.
    /// 
    /// Note: This is not necissarily how many units of the want are
    /// desired, but how many times it was called.
    pub want_requests: HashMap<usize, f64>,
    /// The various ways the want was satisfied in this market. Includes how
    /// many of each method was used.
    pub want_sources: HashMap<usize, Vec<(WantSource, f64)>>,

    // Todo put class price info here for possible use. may not be necessary.

    /// Any Currencies which have been declared as currency for this market.
    /// Typically done by either a state, or another particularly powerful
    /// entitiy.
    pub state_currencies: Vec<usize>,
    /// The info of the market from yesterday, stored for general
    /// information.
    pub previous_day: MarketHistory,

    /// Stores products offered for sale, and a list of weighted actors to 
    /// help with selection. Values are the total weight available, followed by
    /// the list of available sellers.
    seller_weights: HashMap<usize, (f64, Vec<WeightedActor>)>,
    /// All pops in the system, weighted by their current wealth.
    /// 
    /// TODO May be updated to a rolling average instead of a daily, perfectly
    /// accurate, measure.
    pop_wealth_weight: Vec<WeightedActor>,
    /// Ongoing record of deals, used to keep track more easily and allows us to update
    /// market data more easily. Why send the same messages twice afterall?.
    ongoing_deals: Vec<DealRecord>
}

impl Market {
    /// Runs the market day for this market. This manages the various actors in the market
    pub fn run_market_day(&mut self, 
        sender: Sender<MarketMessage>,
        reciever: &mut Receiver<MarketMessage>,
        data: &DataManager, 
        demos: &Demographics, 
        pops: &mut Vec<Pop>, 
        firms: &mut Vec<Firm>, 
        institutions: &mut Vec<Institution>,
        states: &mut Vec<State>) {
        // get the lengths of our actors for later use.
        let pop_count = pops.len();
        let firm_count = firms.len();
        let institution_count = institutions.len();
        let state_count = states.len();
        // get our thread scope for our children.
        thread::scope(|scope| {
            // make our history immutable so we can hand it out elsewhere.
            // TODO get rid of history cloning if possible.
            let shared_history = MarketHistory::create(self, data);
            // get our sender and recievers for the threads
            let (lcl_sender, 
                lcl_receiver) 
                    = barrage::bounded(1000);
            // spin up everything first, before letting them loose.
            let mut threads = vec![];
            // spin up the actors
            for firm in firms.iter_mut() {
                let history = shared_history.clone();
                let mut firm_sender = lcl_sender.clone();
                let mut firm_rcvr = lcl_receiver.clone();
                threads.push(scope.spawn(move |_| {
                    firm.run_market_day(&mut firm_sender, &mut firm_rcvr,
                        data, demos, &history);
                }));
            }
            for pop in pops.iter_mut() {
                // add pop to popWealthWeight for later possible use
                let history = shared_history.clone();
                self.pop_wealth_weight.push(WeightedActor { actor: pop.actor_info(), 
                    weight: pop.total_wealth(&history) });
                let mut pop_sender = lcl_sender.clone();
                let mut pop_recv = lcl_receiver.clone();
                threads.push(scope.spawn(move |_| {
                    pop.run_market_day(&mut pop_sender, &mut pop_recv,
                    data, demos, &history);
                }));

            }
            for inst in institutions.iter_mut() {
                let mut sender = lcl_sender.clone();
                let mut recv = lcl_receiver.clone();
                let history = shared_history.clone();
                threads.push(scope.spawn(move |_| {
                    inst.run_market_day(&mut sender, &mut recv, 
                        data, demos, &history);
                }));
            }
            for state in states.iter_mut() {
                let mut sender = lcl_sender.clone();
                let mut recv = lcl_receiver.clone();
                let history = shared_history.clone();
                threads.push(scope.spawn(move |_| {
                    state.run_market_day(&mut sender, &mut recv, 
                        data,  demos, &history);
                }));
            }

            // once we spin up all actors, send them the OK message.
            lcl_sender.send(ActorMessage::StartDay).expect("Somehow Closed. Panic!");

            // Enter holding pattern while the children do their work
            let mut completed_firms = HashSet::new();
            let mut completed_pops = HashSet::new();
            let mut completed_insts = HashSet::new();
            let mut completed_states = HashSet::new();
            while completed_firms.len() >= firm_count && 
                completed_pops.len() >= pop_count && 
                completed_insts.len() >= institution_count &&
                completed_states.len() >= state_count {
                let msg = lcl_receiver.recv().expect("Unexpected Disconnect!");
                match msg {
                    ActorMessage::Finished { sender } => { // actor is done, mark them.
                        match sender {
                            ActorInfo::Firm(id) => completed_firms.insert(id),
                            ActorInfo::Pop(id) => completed_pops.insert(id),
                            ActorInfo::Institution(id) => completed_insts.insert(id),
                            ActorInfo::State(id) => completed_states.insert(id),
                        };
                    },

                    ActorMessage::SellOrder { sender, product, 
                    quantity, amv } => self.add_seller_weight(&sender, product,quantity, amv),

                    ActorMessage::DumpProduct { sender: _, product, amount } => {
                        // product dumped into the environment
                        *self.resources.entry(product).or_insert(0.0) += amount;
                    },

                    ActorMessage::FindProduct { product, sender} => { 
                        // buyer is looking for product, send back a seller to them.
                        let result = self.find_seller(product, sender);
                        lcl_sender.send(result).expect("Send Error!");
                    },
                    ActorMessage::FindWant { want, sender } => {
                        // buyer is looking to satisfy a want, try to find an option for them.
                        let result = self.find_want_seller(want, sender);
                        lcl_sender.send(result).expect("Send Error!");
                    }
                    // Product not found, no reaction needed to that message.
                    ActorMessage::FoundProduct { seller, buyer, 
                        product } => {
                        // A product was previously found, record it here.
                        self.ongoing_deals.push(DealRecord::new(
                            vec![seller, buyer], 
                            product, 0.0, 
                            0.0, 
                            HashMap::new(), 
                            OfferResult::Incomplete));
                    },


                    ActorMessage::InStock { buyer, seller, product, 
                    price, .. } => {
                        // Seller in a deal says he's in stock, record his response.
                        let deal = self.find_deal_mut(buyer, seller, product);
                        deal.request_product = product;
                        deal.unit_price = price;
                    },
                    ActorMessage::NotInStock { buyer, seller, product } => {
                        // seller from a previous deal says he's not in stock.
                        // Close out the deal, and remove them from the seller's list.
                        let idx = self.ongoing_deals.iter()
                            .filter(|x| x.request_product == product) // narrow to those with that product
                            .find_position(|x| x.actors.contains(&seller) && x.actors.contains(&buyer))// find one with both buyer and seller
                            .expect("Deal Not Found, PROBLEM!").0;
                        self.ongoing_deals.remove(idx);
                        self.remove_seller(seller, product);
                    },

                    ActorMessage::BuyOffer { buyer, seller, product, 
                    price_opinion, quantity, followup: _ } => {
                        // initial offer info
                        let deal = self.find_deal_mut(buyer, seller, product);
                        deal.request_quantity = quantity;
                        deal.current_result = price_opinion;
                        *self.product_demanded.entry(product).or_insert(0.0) += quantity;
                    },
                    ActorMessage::BuyOfferFollowup { buyer, seller, product, 
                    offer_product, offer_quantity, followup: _  } => {
                        // add offer part to deal
                        let deal = self.find_deal_mut(buyer, seller, product);
                        deal.offer.insert(offer_product, offer_quantity);
                    },

                    ActorMessage::SellerAcceptOfferAsIs { buyer, seller, 
                    product, offer_result: _ } => {
                        // finish the deal, but don't delete it just yet.
                        // wait for the
                        self.finish_offered_deal_from_info(buyer, seller, product);
                    },

                    ActorMessage::OfferAcceptedWithChange { buyer, seller, 
                    product, quantity, followups } => {
                        // update the requested item if needed. 
                        // (if no change, it will be the same value)
                        let deal_idx = self.find_deal(buyer, seller, product);
                        self.ongoing_deals.get_mut(deal_idx).unwrap()
                        .request_quantity = quantity;
                        if followups == 0 { // then finish the deal if last.
                            self.finish_offered_deal(deal_idx);
                        }
                    },
                    ActorMessage::ChangeFollowup { buyer, seller, 
                    product, return_product, return_quantity, followups } => {
                        // followup from chaneg above, 
                        // note the change and finish if no more followups.
                        let deal_idx = self.find_deal(buyer, seller, product);

                        *self.ongoing_deals.get_mut(deal_idx).unwrap()
                        .offer.entry(return_product).or_insert(0.0) = return_quantity;
                        if followups == 0 {
                            self.finish_offered_deal(deal_idx);
                        }
                    },

                    ActorMessage::RejectOffer { buyer, seller, 
                    product } => {
                        // Seller has rejected the offer outright. 
                        // Record this rejection, then finish.
                        let deal_idx = self.find_deal(buyer, seller, product);
                        self.ongoing_deals.get_mut(deal_idx)
                            .unwrap().current_result = OfferResult::Rejected;
                        self.finish_offered_deal(deal_idx);
                    },

                    ActorMessage::FinishDeal { buyer, seller, 
                    product } => {
                        // buyer has recieved the acceptace message from the seller,
                        // Close out the deal here.
                        let idx = self.ongoing_deals.iter()
                            .filter(|x| x.request_product == product) // narrow to those with that product
                            .find_position(|x| x.actors.contains(&seller) && x.actors.contains(&buyer))// find one with both buyer and seller
                            .expect("Deal not found!").0;
                        self.ongoing_deals.remove(idx);
                    },

                    // CheckItem doesn't do anything and it's a private message.
                    // SendProduct, and SendWant doesn't do anything for us. This is
                    // a gift or transfer between actors outside of normal market
                    // mechanisms (IE charity or internal firm operations).
                    // WantSplash, FirmToEmployee, and EmployeeToFirm are also
                    // not for us, but for internal operations with firms.
                    _ => (),
                }
            }

            // after all actors message done, send our message done up and wait
            sender.send(MarketMessage { sender: self.id, reciever: 0,
                 message: MarketMessageEnum::CloseMarket}).expect("Closed, Big Problem.");
            loop {
                // with the close sent, read messages and wait for the all clear. 
                // respond to any messages directed to us.
                let result = reciever.recv().expect("Unexpected Close.");
                match result.message {
                    MarketMessageEnum::ConfirmClose => {
                        break;
                    },
                    _ => panic!("Message other than confirm close found after end of day.")// do nothing on everything else.
                }
            }
            // if we got here, then we're done. Do any clean and info 
            // clean out sellers
            self.seller_weights.clear();
            // product info will be needed for later use, so don't clear out just yet.
            // consolidation outside of this thread scope so we can edit stuff.
        }).unwrap();
    }

    /// Adds seller info, also calculating it's weight in the market selection process.
    fn add_seller_weight(&mut self, _sender: &ActorInfo, product: usize, quantity: f64, amv: f64) {
        let _list = self.seller_weights.entry(product).or_insert((0.0, vec![]));
        let market_price = self.previous_day.get_product(&product).price;
        let mut _weight = 0.0;
        if market_price <= 0.0 && amv <= 0.0 { // TODO 0 or negative amv or market price for product found.
            todo!("Do later, I can't be bothered right now."); 
        } else { // Positive market value and price
            // Inversely proportional to market value. AMV = 1/2 => weight == 2
            _weight = (market_price / amv) * 100.0;
        }
        // finish by adding to the total market supply.
        *self.products_for_sale.entry(product).or_default() += quantity;
    }

    /// Does the work of finding a seller for a buyer as well as recording 
    /// their demand for future needs.
    pub fn find_seller(&mut self, product: usize, sender: ActorInfo) -> ActorMessage {
        // if we have any sellers, select one at random,
        // check that we have any in the first place
        let sellers = if let Some(sellers) = self.seller_weights.get(&product) {
            sellers
        } else { // if no sellers, return not found.
            return ActorMessage::ProductNotFound { product, buyer: sender };
        };
        // with sellers available, check if we have many or one
        if sellers.1.len() == 1 {
            let seller = sellers.1.first().unwrap();
            // sanity check that it's not the sender
            if seller.actor == sender { // if it is, then return not found, the buyer and seller should never be the same actor.
                return ActorMessage::ProductNotFound { product, buyer: sender };
            }
            return ActorMessage::FoundProduct { product, seller: seller.actor, buyer: sender };
        } else { // many
            loop { // loop until we find someone who isn't the buyer (The same buyer should not be in the list twice.)
                let mut rng = thread_rng();
                let select: f64 = rng.gen();
                let select = select * sellers.0;
                let mut sum = 0.0;
                for actor in sellers.1.iter() {
                    sum += actor.weight;
                    if sum > select {
                        if actor.actor == sender { break; } // if the selected seller is the sender, try again.
                        return ActorMessage::FoundProduct { seller: actor.actor, buyer: sender, product };
                    }
                }
            }
        }
    }

    /// # Find Want Seller
    /// 
    /// A helper function which looks at available possible items which satisfy 
    /// a want. It selects the product at random. If it's available in the market
    /// and a seller is available, it returns that seller.
    /// 
    /// TODO Needs testing!!
    fn find_want_seller(&self, _want: usize, _sender: ActorInfo) -> ActorMessage {
        // TODO Pick up Here!!!!!!!
        todo!()
    }

    /// Finds an ongoing deal in our list of deals.
    /// 
    /// Panics if deal was not found.
    fn find_deal(&self, buyer: ActorInfo, seller: ActorInfo, product: usize) -> usize {
        self.ongoing_deals.iter()
        .filter(|x| x.request_product == product) // narrow to those with that product
        .find_position(|x| x.actors.contains(&seller) && x.actors.contains(&buyer))// find one with both buyer and seller
        .expect("Deal Not Found, PROBLEM!").0
    }

    /// Finds a mutable deal, returns it for simplicity
    fn find_deal_mut(&mut self, buyer: ActorInfo, seller: ActorInfo, product: usize) -> &mut DealRecord {
        self.ongoing_deals.iter_mut()
        .filter(|x| x.request_product == product) // narrow to those with that product
        .find(|x| x.actors.contains(&seller) && x.actors.contains(&buyer))// find one with both buyer and seller
        .expect("Deal Not Found, PROBLEM!")
    }

    /// Finds and removes an ongoing deal, meaning it should be closed out.
    /// 
    /// Panics if deal was not found.
    fn _remove_deal(&mut self, buyer: ActorInfo, seller: ActorInfo, product: usize) {
        let idx = self.ongoing_deals.iter()
        .filter(|x| x.request_product == product) // narrow to those with that product
        .find_position(|x| x.actors.contains(&seller) && x.actors.contains(&buyer))// find one with both buyer and seller
        .expect("Deal Not Found, PROBLEM!").0;
        self.ongoing_deals.remove(idx);
    }

    /// Processes a price opinion recieved and applies that modification to the seller's weight.
    fn _process_price_opinion(&mut self, seller: ActorInfo, 
    product: usize, price_opinion: OfferResult) {
        let weights = self.seller_weights.get_mut(&product)
        .expect("Product not Found, Panic!");
        if let OfferResult::OutOfStock = price_opinion { // if the seller is out of stock, remove them and their weight.
            let idx = weights.1.iter()
            .find_position(|x| x.actor == seller).expect("Seller not found?").0;
            let weight = weights.1.remove(idx);
            weights.0 -= weight.weight;
            return;
        }
        // seller is not out of stock, alter the weight appropriately to the message.
        let weight = weights.1.iter_mut().find(|x| x.actor == seller)
        .expect("Seller not found? PANIC!");
        let mut alteration = 0.0;
        match price_opinion {
            OfferResult::TooExpensive => alteration += -5.0,
            OfferResult::Expensive => alteration += -2.0,
            OfferResult::Overpriced => alteration += -1.0,
            OfferResult::Reasonable => alteration += 1.0,
            OfferResult::Cheap => alteration += 5.0,
            OfferResult::Steal => alteration += 10.0,
            _ => ()
        }
        weight.weight += alteration;
        weights.0 += alteration;
    }

    /// Finishes out a deal, processing the results for market info and price 
    /// adjustments, clear out the deal also, but don't close it out totally just yet.
    /// 
    /// TODO Test this function
    fn finish_offered_deal_from_info(&mut self, buyer: ActorInfo, seller: ActorInfo, product: usize) {
        let deal = self.find_deal(buyer, seller, product);
        self.finish_offered_deal(deal);
    }

    /// Finishes out a deal, processing the results for market info and price 
    /// adjustments, clear out the deal also, but don't close it out totally just yet.
    fn finish_offered_deal(&mut self, deal_idx: usize) {
        let deal = self.ongoing_deals.get_mut(deal_idx)
            .expect("Deal not found?");
        let product = deal.request_product;
        // get the price of the merchandise.
        let product_price = deal.request_quantity * deal.unit_price;
        // summarize the price of items offered in current market value.
        let offer_value: f64 = deal.offer.iter()
        .map(|(prod, quant)| quant * self.prices.get(prod).unwrap_or(&1.0))
        .sum();
        
        if let OfferResult::Rejected = deal.current_result { 
            // if rejected, push requested item's AMV up and offered items' AMV down.
            // reduce prices for items offered, scaled inversly with their Salability and weighted by value in offer
            for (item, quantity) in deal.offer.iter() {
                // 1-Salability (min 0.1)
                let change_scale: f64 = 0.1_f64.max(1.0 - *self.salability.get(&item).unwrap_or(&0.5));
                let weighted_change = -STD_PRICE_CHANGE * change_scale
                    * (self.prices.get(item).expect("Product not found.") * quantity) / offer_value; // drive price down weighted by offer.
                *self.prices.entry(*item).or_insert(1.0) += weighted_change;
            }
            // 1-Salability (min 0.05) Highly Salable items are less mobile in AMV.
            let change_scale : f64 = 0.05_f64.max(1.0 - *self.salability.get(&deal.request_product).unwrap_or(&0.5));
            let price_change = STD_PRICE_CHANGE * change_scale ; // drive price up.
            *self.prices.entry(product).or_insert(1.0) += price_change;
        } else if let OfferResult::OutOfStock = deal.current_result {
            // if out of stock, push item's value up a tiny bit.
            let change_scale : f64 = 0.05_f64.max(1.0 - *self.salability.get(&deal.request_product).unwrap_or(&0.5));
            let price_change = STD_PRICE_CHANGE * change_scale / 2.0; // OOS gives smaller boost than normal.
            *self.prices.entry(product).or_insert(1.0) += price_change;
        } else {
            // if Accepted, push AMV of items offered together.
            // Depending on the current value of items, push them together.
            let mut push = 1.0;
            if product_price > offer_value {
                push = -1.0;
            }
            // iterate over offer items
            for (item, quantity) in deal.offer.iter() {
                // modify their price towards requested item, scaled with salability and weight in offer.
                let change = 0.1_f64.max(-push - *self.salability.get(&item).unwrap_or(&0.5));
                let weighted = -STD_PRICE_CHANGE * change 
                    * (self.prices.get(item).expect("Product Not Found.") * quantity) / offer_value;
                *self.prices.entry(*item).or_insert(1.0) += weighted;
            }
            // modify the price towards the offer scaled with salability.
            let change_scale : f64 = 0.05_f64.max(1.0 - *self.salability.get(&deal.request_product).unwrap_or(&0.5));
            let price_change = STD_PRICE_CHANGE * change_scale ; // drive price up.
            *self.prices.entry(product).or_insert(1.0) += price_change;
        }
    }

    /// Remove Seller from list of sellers.
    /// 
    /// It also removes the seller's weight from the accumulated weight for that item.
    /// 
    /// If the item's total weight is 0, it removes it entirely.
    fn remove_seller(&mut self, seller: ActorInfo, product: usize) {
        let data = self.seller_weights.get_mut(&product)
            .expect("Product not found.");
        let seller_data = data.1.iter()
            .find_position(|x| x.actor == seller)
            .expect("Seller not found.");
        data.0 -= seller_data.1.weight;
        data.1.remove(seller_data.0);
    }
}

/// Market History is all the information contained by the market from the
/// previous day. This data is updated in the market at the end of the day
/// and passed to the Actors in the market during the day so they have 
/// access to this data.
#[derive(Debug, Clone)]
pub struct MarketHistory {
    /// The info for each product we store in memory.
    pub product_info: HashMap<usize, ProductInfo>,
    /// The info for each class available in this market.
    pub class_info: HashMap<usize, ClassInfo>,
    /// The info for each want we want to store in memory.
    pub want_info: HashMap<usize, MarketWantInfo>,
    /// Organizes products by their sale priority (Salability Highest to lowest).
    /// TODO Perhaps change this to take AMV into account in some fashion.
    pub sale_priority: Vec<usize>,
    /// The products which are Currencies in our market for whatever reason.
    /// Sorted by Salability (highest to lowest)
    pub currencies: Vec<usize>,
}

impl MarketHistory {
    /// # Create
    /// 
    /// Creates a market history of based on the current market given
    /// to it. 
    /// 
    /// TODO Not Tested
    pub fn create(market: &Market, data: &DataManager) -> Self {
        let mut ret = MarketHistory { product_info: HashMap::new(),
            want_info: HashMap::new(),
            class_info: HashMap::new(),
            sale_priority: vec![],
            currencies: vec![],
        };
        // go through each product and copy over it's info from the market.
        // also add class prices.
        for (product, price) in market.prices.iter() {
            let avail = market.resources.get(product).unwrap_or(&0.0);
            let offered = market.products_for_sale.get(product).unwrap_or(&0.0);
            let sold = market.product_sold.get(product).unwrap_or(&0.0);
            let sal = market.salability.get(product).unwrap_or(&0.0);
            let currency = if *sal > SALABILITY_THRESHOLD {
                true
            } else if market.state_currencies.contains(product) {
                true
            } else { false };

            ret.product_info.insert(*product, ProductInfo { available: *avail, 
                price: *price,  offered: *offered, sold: *sold, 
                salability: *sal, is_currency: currency });
            // add to the class it's a part of (if any)
            if let Some(class) = data.products.get(product)
            .expect("Product not found!").product_class {
                ret.class_info.entry(class)
                .and_modify(|x| {
                    x.price = (x.price * x.sold + price * sold) / (x.sold + sold); 
                    x.options += 1;
                    x.offered += offered;
                    x.sold += sold;
                })
                .or_insert(ClassInfo { price: *price, options: 1, offered: *offered, sold: *sold  });
            }
        }

        // go through each want and calculate the estimated value of the want in the current market.
        for (want, sources) in market.want_sources.iter() {
            let mut total = 0.0;
            let mut total_weight = 0.0;
            for (source, amount) in sources.iter()
            .filter(|(_, amount)| *amount > 0.0) {
                let price = match source {
                    WantSource::Product(id) => {
                        *market.prices.get(id).unwrap()
                    },
                    WantSource::Process(id) => {
                        calculate_want_price(market, data, *id, &ret)
                    },
                };
                total = (total * total_weight + price * amount) / (total_weight + amount);
                total_weight += amount;
            }
            ret.want_info.insert(*want, MarketWantInfo { est_price: total, est_products: sources.len() as f64 });
        }

        // add in those currencies which are dictated to be currencies by the market.
        for (product, info) in ret.product_info.iter()
        .sorted_by(|a, b| {
            // sort buy salability, highest to lowest
            // TODO, perhaps have this sort by Salability * AMV Part of TODO Line:537 above.
            b.1.salability.partial_cmp(&a.1.salability).expect("Bad NAN!")
        }) {
            // add to sale priority, for general purposes.
            ret.sale_priority.push(*product);
            if info.is_currency { // if it's a currency, also add it to currencies.
                ret.currencies.push(*product);
            }
        }
        ret
    }

    

    /// Helper function, gets a product from our history.
    pub fn get_product(&self, product: &usize) -> &ProductInfo {
        self.product_info.get(product).expect("Product Not Found!")
    }

    pub fn get_product_price(&self, product: &usize, default: f64) -> f64 {
        if let Some(result) = self.product_info.get(product) {
            result.price
        } else {
            default
        }
    }

    /// # Get Product Salability
    /// 
    /// A quick function to get the Salability of an item from the market.
    /// 
    /// If the item does not have a salability in the market it returns the
    /// DEFAULT_SALABILITY.
    pub fn get_product_salability(&self, product: &usize) -> f64 {
        if let Some(result) = self.product_info.get(product) {
            result.salability
        } else {
            constants::DEFAULT_SALABILITY
        }
    }

    /// # Get Class Price
    /// 
    /// Gets an estimated price of a class of products.
    /// 
    /// Updated at the end of each day, weighted based on the availability of
    /// the various products.
    /// 
    /// TODO update this when class prices are added, this is a placeholder.
    pub fn get_class_price(&self, id: usize, default: f64) -> f64 {
        if let Some(result) = self.class_info.get(&id) {
            result.price
        } else {
            default
        }
    }

    /// # Get Want Price
    /// 
    /// Gets the estimated price of a want.
    /// 
    /// Updated at the end of each day, weighted based on the likelyhood of
    /// the processes which make it.
    /// 
    /// TODO update this when want prices are added, this is a placeholder.
    pub fn get_want_price(&self, id: usize, default: f64) -> f64 {
        if let Some(result) = self.want_info.get(&id) {
            result.est_price
        } else {
            default
        }
    }
}

/// Helper function
/// 
/// Calculates the value of a want given the market data and id of the want in question.
fn calculate_want_price(_market: &Market, data: &DataManager, id: usize, history: &MarketHistory) -> f64 {
    let mut total_price = 0.0;
    let mut total_quantity = 0.0;
    let proc_info = data.processes.get(&id).unwrap();
    for part in proc_info.input_and_capital_products()
    .iter() {
        let price = match part.item {
            Item::Want(_id) => 0.0, // todo consider adding in recursive calculation for other wants.
            Item::Class(id) => history.class_info.get(&id)
                .unwrap_or(&ClassInfo::new(0.0)).price,
            Item::Product(id) => history.product_info.get(&id)
                .unwrap_or(&ProductInfo::new(0.0)).price,
        };
        total_price = (total_price * total_quantity + price * part.amount) / (total_quantity + part.amount);
        total_quantity += part.amount;
    }
    total_price
}

/// Market info for product classes.
#[derive(Debug, Copy, Clone)]
pub struct ClassInfo {
    /// The average price of a product in this class, weighted by availility of
    /// the product in the market.
    pub price: f64,
    /// The variety of products within this class in the market.
    pub options: usize,
    /// The number of this class which were offered in the market yesterday.
    pub offered: f64,
    /// The total number of products in this class which was sold in
    /// the market.
    pub sold: f64,
}
impl ClassInfo {
    fn new(price: f64) -> ClassInfo {
        ClassInfo {
            price,
            options: 0,
            offered: 0.0,
            sold: 0.0,
        }
    }
}

/// The Ways in which a market can connect to another market directly.
/// Each has a values attached to them for additional information.
#[derive(Debug)]
pub enum MarketConnection{
    /// A Land Connection, the value is the estimated average distance between
    /// all points in this market to any point in the other market.
    Land(f64),
    /// A Water Connection, the value is the estimated distance between them.
    /// This is primarily small fresh-water connections like lakes or rivers.
    Water(f64),
    /// A Sea Connection, the value being the distance between the two markets.
    /// This is primarily meant to be used as a shortcut between major ports.
    Sea(f64),
    /// An air connecction. The strangest. Typically used for landlocked
    /// regions (think Berlin Airlift), or for flying city style locations.
    Air(f64),
    /// A Tunnel connection, think the Chunnel between France and the UK.
    Tunnel(f64),
    /// A connection through space, Space.0 is the distance. Space.1 is
    /// the estimated Delta-V distance to move from the current location
    /// to the other.
    Space(f64, f64),
    /// A Magical or technological connection which does not cross the 
    /// intervening space.
    /// The first value is the distance to travel, the second is the 
    /// throughput limit in mass, the third is the throughput limit
    /// in volume.
    Portal(f64, f64, f64),
}

/// Messages meant to be passed between markets.
#[derive(Debug, Clone, Copy)]
pub struct MarketMessage {
    /// The Sender's id, so responses can be addressed appropriately.
    pub sender: usize,
    // The Reciever's id, so we know who should recieve it.
    pub reciever: usize,
    /// What is being asked or otherwise requested.
    pub message: MarketMessageEnum,
}

/// What actions and information can be passed around by the market across 
/// or up the hierarchy.
#[derive(Debug, Clone, Copy)]
pub enum MarketMessageEnum {
    /// Tells the main thread that the market thread is complete and ready 
    /// to close out.
    CloseMarket,
    /// Tells the market threads that they will not recieve any more messages 
    /// and to close out.
    ConfirmClose,
}

/// helper struct for storing actors and their weight. Primarily used for 
/// seller selection in the Market class.
/// 
/// Contains the ActorInfo and the weight of that actor. Bigger number is better.
#[derive(Debug, Clone, Copy)]
pub struct WeightedActor {
    pub actor: ActorInfo,
    pub weight: f64,
}

#[derive(Debug, Clone)]
pub struct DealRecord {
    pub actors: Vec<ActorInfo>,
    pub request_product: usize,
    pub request_quantity: f64,
    pub unit_price: f64,
    pub offer: HashMap<usize, f64>,
    pub current_result: OfferResult,
}

impl DealRecord {
    pub fn new(actors: Vec<ActorInfo>, 
    request_product: usize, 
    request_quantity: f64, 
    unit_price: f64,
    offer: HashMap<usize, f64>,
    current_result: OfferResult) -> Self {
        Self { 
           actors, 
           request_product, 
           request_quantity, 
           unit_price,
           offer,
           current_result
        } 
    }
}

/// Market History for wants to make estimating the price of a want easier 
/// to find.
#[derive(Debug, Clone, Copy)]
pub struct MarketWantInfo {
    /// The estimated price of the product, created from the 
    /// weighted average of the constituent product and possible processes.
    /// 
    /// In calculation, each process is given a price equivalent to the 
    /// weight of each product in the process.
    pub est_price: f64,
    /// The estimated number of products in the process, allowing one to 
    /// create an average price of the products involved. 
    pub est_products: f64
}

impl MarketWantInfo {
    pub fn new(est_price: f64) -> Self { 
            Self { est_price, est_products: 1.0} 
        }
}

/// Market History info for our products, to make getting info more easy.
#[derive(Debug, Clone, Copy)]
pub struct ProductInfo {
    /// How many are available in the environment to grab.
    pub available: f64,
    /// Yesterday's price for the item.
    pub price: f64,
    /// How many were offered yesterday.
    pub offered: f64,
    /// How many were sold yesterday.
    pub sold: f64,
    /// The item's Salability Rating.
    pub salability: f64,
    /// If the item is a currency. May be true even if Salability isn't
    /// above threshold.
    pub is_currency: bool,
}

impl ProductInfo {
    pub fn new(price: f64) -> Self { 
            Self { available: 0.0, 
                price, 
                offered: 0.0, 
                sold: 0.0, 
                salability: 0.5, 
                is_currency: false } 
        }
}
