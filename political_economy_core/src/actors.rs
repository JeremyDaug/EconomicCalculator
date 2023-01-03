use std::{collections::HashMap, thread};

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
    pub fn run_market_day(&self, 
        data_manager: &DataManager, 
        demographics: &Demographics, 
        map: &mut ()) {
        let mut threads = vec![];
        // spin up market threads
        for market in self.markets.values_mut() {
            // steal pops and firms from the list so they can be used properly
            // by their parent markets.
            let mut pops = vec![];
            for pop_id in market.pops.iter() {
                pops.push(self.pops.get_mut(pop_id));
            }
            let mut firms = vec![];
            for firm_id in market.firms.iter() {
                firms.push(self.firms.get_mut(firm_id));
            }
            let handle = thread::spawn(|| {
                // get immutable references to data and demos
                let data = data_manager;
                let demos = demographics;
                market.run_market_day(&data, &demos, &mut pops, &mut firms)

            });
            threads.push(handle);
        }
        // 
    }
}