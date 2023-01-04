use std::{collections::HashMap, sync::Arc};

use crate::{demographics::Demographics, data_manager::DataManager};

use super::{pop::Pop, firm::Firm};

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
    pub market_prices: HashMap<usize, f64>,
    /// The products which are available for sale, and how many of them.
    pub products_for_sale: HashMap<usize, f64>,
    /// The products sold in this market so far.
    pub product_sold: HashMap<usize, f64>,
    /// The total products produced today.
    pub product_output: HashMap<usize, f64>,
    /// The total amount of products exchanged today.
    pub product_exchanged_total: HashMap<usize, f64>
}

impl Market {
    pub(crate) fn run_market_day(&self, 
        data: &DataManager, 
        demos: &Demographics, 
        pops: &mut Vec<Pop>, 
        firms: &mut Vec<Firm>, 
        institutions: &mut Vec<()>,
        states: &mut Vec<()>) {
        todo!()
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