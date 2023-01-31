use core::panic;
use std::collections::{HashMap, HashSet};
use barrage::{Sender, Receiver};
use crossbeam::thread;

use crate::{demographics::Demographics, data_manager::DataManager};
use super::{pop::Pop, firm::Firm, actor::Actor, actor_message::{ActorMessage, ActorInfo}, institution::Institution, state::State};

const SHOPPING_TIME_COST: f64 = 0.2;

/// The Salability threshold for an item to be considered a currency.
const SALABILITY_THRESHOLD: f64 = 0.75;

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
    // Available Notdes.
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
    /// Any Currencies which have been declared as currency for this market.
    /// Typically done by either a state, or another particularly powerful
    /// entitiy.
    pub state_currencies: Vec<usize>,
    /// The info of the market from yesterday, stored for general
    /// information.
    pub previous_day: MarketHistory,
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
            // TODO get rid of this clone if possible.
            // get our sender and recievers for the threads
            let (lcl_sender, 
                lcl_receiver) 
                    = barrage::bounded(1000);
            // spin up everything first, before letting them loose.
            let mut threads = vec![];
            // spin up the actors
            for firm in firms.iter_mut() {
                let history = MarketHistory::create(self);
                let firm_sender = lcl_sender.clone();
                let mut firm_rcvr = lcl_receiver.clone();
                threads.push(scope.spawn(move |_| {
                    firm.run_market_day(firm_sender, &mut firm_rcvr,
                        data, demos, &history);
                }));
            }
            for pop in pops.iter_mut() {
                let pop_sender = lcl_sender.clone();
                let mut pop_recv = lcl_receiver.clone();
                let history = MarketHistory::create(self);
                threads.push(scope.spawn(move |_| {
                    pop.run_market_day(pop_sender, &mut pop_recv,
                    data, demos, &history);
                }));
            }
            for inst in institutions.iter_mut() {
                let sender = lcl_sender.clone();
                let mut recv = lcl_receiver.clone();
                let history = MarketHistory::create(self);
                threads.push(scope.spawn(move |_| {
                    inst.run_market_day(sender, &mut recv, 
                        data, demos, &history);
                }));
            }
            for state in states.iter_mut() {
                let sender = lcl_sender.clone();
                let mut recv = lcl_receiver.clone();
                let history = MarketHistory::create(self);
                threads.push(scope.spawn(move |_| {
                    state.run_market_day(sender, &mut recv, 
                        data,  demos, &history);
                }));
            }

            // once we spin up all actors, send them the OK message.
            lcl_sender.send(ActorMessage::StartDay).expect("Someho Closed. Panic!");

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
                    ActorMessage::Finished { sender } => {
                        match sender {
                            ActorInfo::Firm(id) => completed_firms.insert(id),
                            ActorInfo::Pop(id) => completed_pops.insert(id),
                            ActorInfo::Institution(id) => completed_insts.insert(id),
                            ActorInfo::State(id) => completed_states.insert(id),
                        };
                    },
                    ActorMessage::FindProduct { product, 
                        amount, time, sender} => {
                            // record the amount sought from them.
                            *self.product_demanded.entry(product).or_insert(0.0)
                                += amount;
                            let outcome = self.find_product(product,
                                time, sender);
                            lcl_sender.send(outcome)
                                .expect("Local Channel Closed.");
                    },
                    _ => ()
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
            // consolidation outside of this thread scope so we can edit stuff.
        }).unwrap();
    }

    pub fn find_product(&mut self, product: usize, time: f64, sender: ActorInfo) -> ActorMessage {
        // check that we have any sellers in the first place.
        
        // if no success (ran out of time or no sellers), return Failure
        ActorMessage::ProductNotFound { product, buyer: sender, time_remaining: time-0.2 }
    }
}

/// Market History is all the information contained by the market from the
/// previous day. This data is updated in the market at the end of the day
/// and passed to the Actors in the market during the day so they have 
/// access to this data.
#[derive(Debug, Clone)]
pub struct MarketHistory {
    /// The open resources of the market, these are the 'trash' items
    /// which are available for anyone to pick up, and includes surface
    /// resources from the environment.
    pub resources: HashMap<usize, f64>,
    /// The Market prices in AMV yesterday.
    pub market_prices: HashMap<usize, f64>,
    /// How many of each product was offered in the market yesterday.
    pub product_offered: HashMap<usize, f64>,
    /// The products sold in this market yesterday.
    pub product_sold: HashMap<usize, f64>,
    /// The currencies in the market and their trust rating.
    /// All values are at above our threshold (currently 0.75) and
    /// no greater than 1. If any exist here, than we have at least one currency.
    pub currencies: HashMap<usize, f64>,
}

impl MarketHistory {

    /// Creates a market history of yesterday based on the current market given
    /// to it.
    pub fn create(market: &Market) -> Self {
        // TODO update this to actually take all this information.
        let mut ret = MarketHistory { resources: market.resources.clone(),
            market_prices: market.prices.clone(), 
            product_offered: HashMap::new(), 
            product_sold: HashMap::new(),
            currencies: HashMap::new()
        };
        // add in moneys
        for money in market.salability
        .iter().filter(|x| *x.1 > SALABILITY_THRESHOLD) {
            ret.currencies.insert(*money.0, *money.1);
        }
        // add in currencies
        for currency in market.state_currencies.iter() {
            let value = ret.currencies.entry(*currency).or_insert(0.0);
            *value = *market.salability.get(currency).unwrap_or(&0.5);
        }
        // BREAK OUT!
        ret
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