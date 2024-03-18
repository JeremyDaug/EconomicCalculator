use std::collections::{HashMap, HashSet};
use crossbeam::thread;

use crate::{data_manager::DataManager, 
    demographics::Demographics, 
    objects::{
        actor_objects::{
            firm::Firm, 
            institution::Institution, 
            pop::Pop,
            state::State
        }, 
        environmental_objects::market::{Market, MarketMessage, MarketMessageEnum},
    }
};

/// Actors, these have AI attached to them, make active decisions, and try
/// to satisfy desires either of their own or of others.
/// 
/// It contains
/// - Markets, which don't act, but are a nexus of action for all of
/// these actors.
/// - Pops, the people who work in the markets as well as buy and consume
/// the goods from the market.
/// - Firms, the productive organizations where people work to produce various
/// things.
/// - Institutions, various groups who are not inherently economic in
/// organization.
/// - States, formalized institution(s) who have gained a domineering position
/// in a market, and have gained the right to monopolize legitimate violence,
/// and extract taxes.
/// 
/// Pops and Firms must be contained by the market. Institutions and States
/// do not need to be contained, but should have delegate/representative in
/// the market to represent and communicate with their them in their capital.
#[derive(Debug)]
pub struct ActorManager {
    /// The markets managed here.
    pub markets: HashMap<usize, Market>,
    /// The Pops within the managed markets.
    pub pops: HashMap<usize, Pop>,
    /// The firms within the Managed Markets
    pub firms: HashMap<usize, Firm>,
    /// The Institutions witihn the managed Markets
    pub institutions: HashMap<usize, Institution>,
    /// The Substates in the Managed Markets
    pub states: HashMap<usize, State>,
}

impl ActorManager {
    /// Runs the market day for our actors. 
    /// Splits up the work based on the markets, threads each to their own 
    /// portion, and then waits on them to return.
    pub fn run_market_day(&mut self, 
    data_manager: &DataManager, 
    demographics: &Demographics, 
    _map: &mut ()) {
        // get our thread scope, threads cannot leave here.
        thread::scope(|scope| {
            // get our thread holder we'll be getting our info back from.
            let mut threads = vec![];
            // also get the many to many broadcaster
            let (local_sender, 
                local_reciever) 
                    = barrage::bounded(1000);
            // for each market
            for market in self.markets.values_mut() {
                // get the pops
                let mut pops = vec![];
                for pop_id in market.pops.iter() {
                    pops.push(self.pops.remove(&pop_id)
                    .expect("Pop Not Found!"));
                }
                // firms
                let mut firms = vec![];
                for firm_id in market.firms.iter() {
                    firms.push(self.firms.remove(&firm_id)
                    .expect("Firm Not Found!"));
                }
                // institutions
                let mut insts = vec![];
                for inst_id in market.institutions.iter() {
                    insts.push(self.institutions.remove(&inst_id)
                    .expect("Institution Not Found!"));
                }
                // and states within it
                let mut states = vec![];
                for state_id in market.states.iter() {
                    states.push(self.states.remove(&state_id)
                    .expect("State Not Found!"));
                }
                // get a channel between us here and the 
                let sender = local_sender.clone();
                let mut reciever = local_reciever.clone();
                // spin up the thread
                threads.push(scope.spawn(move |_| {
                    market.run_market_day(
                        sender,
                        &mut reciever,
                        data_manager, 
                        demographics, 
                        &mut pops, 
                        &mut firms, 
                        &mut insts, 
                        &mut states);
                        // return back from the thread the market, and the 
                        // pops firms, institutions, and states which were 
                        // acting in it.
                        (pops, firms, insts, states)
                }));
            }
            // alternate between checking for messages to pass up or around
            // or for additional information.
            let mut completed = HashSet::new();
            // loop until all of them return CloseMarket.
            while completed.len() != threads.len() {
                // get the message sent, error if we disconnected prematurely.
                let message = local_reciever.recv()
                .expect("Error! Market Threads Disconnected before closing.");
                // get the message out and see if it's important
                match message.message {
                    MarketMessageEnum::CloseMarket => {
                        // market is closing, mark it down and continue.
                        completed.insert(message.sender);
                    },
                    _ => ()
                }
            }
            // Once all markets are closed, send down confirmations for them to shut down
            for id in completed {
                local_sender.send(MarketMessage{ sender: 0, reciever: id, 
                    message: MarketMessageEnum::ConfirmClose})
                    .expect("Problem, could not send.");
            }
            // Then wait for them all to close.
            let mut results = vec![];
            for thread in threads.into_iter() {
                results.push(thread.join().expect("Error recieved"));
            }
            // With them all complete, move their data back to storage.
            for group in results {
                for pop in group.0 {
                    self.pops.insert(pop.id, pop);
                }
                for firm in group.1 {
                    self.firms.insert(firm.id, firm);
                }
                for inst in group.2 {
                    // TODO update these when Institutions are made.
                    self.institutions.insert(0, inst);
                }
                for state in group.3 {
                    // TODO update when states are made.
                    self.states.insert(0, state);
                }
            }
            // with all data back, do any movements between markets
            // TODO do this later. Then pass up the changes to the runner and
            // master so the master can share or transfer across runners.
        }).unwrap();
    }
}