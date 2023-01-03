use std::{collections::HashMap, thread, sync::Arc};

use crate::{data_manager::DataManager, 
    demographics::Demographics, objects::{market::Market, pop::Pop, firm::Firm}};

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
pub struct Actors {
    /// The markets managed here.
    pub markets: HashMap<usize, Market>,
    /// The Pops within the managed markets.
    pub pops: HashMap<usize, Pop>,
    /// The firms within the Managed Markets
    pub firms: HashMap<usize, Firm>,
    /// The Institutions witihn the managed Markets
    pub institutions: HashMap<usize, ()>,
    /// The Substates in the Managed Markets
    pub states: HashMap<usize, ()>,
}
impl Actors {
    /// Runs the market day for our actors. 
    /// Splits up the work based on the markets, threads each to their own 
    /// portion, and then waits on them to return.
    pub fn run_market_day(&mut self, 
        data_manager: Arc<&DataManager>, 
        demographics: Arc<&Demographics>, 
        map: &mut ()) {
        let mut threads = vec![];
        // spin up market threads
        for market in self.markets.drain().map(|x| x.1) {
            // steal pops and firms from the list so they can be used properly
            // by their parent markets.
            let mut pops = vec![];
            for pop_id in market.pops.iter() {
                pops.push(self.pops.remove(pop_id).expect("Pop Not Found."));
            }
            let mut firms = vec![];
            for firm_id in market.firms.iter() {
                firms.push(self.firms.remove(firm_id).expect("Firm Not Found."));
            }
            let mut institutions = vec![];
            for inst_id in market.institutions.iter() {
                institutions.push(self.institutions.remove(inst_id)
                    .expect("Institution Not Found."));
            }
            let mut states = vec![];
            for state_ids in market.states.iter() {
                states.push(self.states.remove(state_ids).expect("States Not Found."));
            }
            let data = Arc::clone(&data_manager);
            let demos = demographics.clone();
            let handle = thread::spawn(move || {
                // get immutable references to data and demos
                market.run_market_day(data, demos, &mut pops, &mut firms,
                    &mut institutions, &mut states);
                (market, pops, firms, institutions, states)
            });
            threads.push(handle);
        }
        // wait for them to finish and return the borrowed values to the manager.
        let mut returns = vec![];
        for thread in threads {
            returns.push(thread.join().unwrap());
        }
        // return the borrowed values to the manager
        for set in returns {
            for pop in set.1 {
                //self.pops. = pop;
            }
            for firm in set.2 {

            }
            for institution in set.3 {

            }
            for state in set.4 {

            }
        }
    }
}