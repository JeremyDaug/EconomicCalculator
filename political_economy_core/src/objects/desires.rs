//! The collection which manages multiple desires.
//! 
//! This collection manages and organizes the desires of an
//! actor into a rational fashion. Here is also where 
//! the validity of a potential barter is decided or not.

use std::collections::HashMap;

use super::desire::{Desire};

#[derive(Debug)]
pub struct Desires {
    /// All of the desires we are storing and looking over.
    pub items: Vec<Desire>,
    /// The property currently owned bey the actor.
    pub property: HashMap<u64, f64>,
    /// The data of our succes in shopping.
    pub shopping_data: HashMap<u64, f64>,
}

impl Desires {
    pub fn new(desires: Vec<Desire>) -> Self {
        Desires {
            items: desires,
            property: HashMap::new(),
            shopping_data: HashMap::new()
        }
    }

    pub fn add_property(&mut self, product: u64, amount: f64) {
        self.property.entry(product).
    }
}

/// The information on a product which is desired.
pub struct DesireInfo {
    /// The target amount we want to buy.
    pub target: f64,
    /// The amount we previously bought.
    pub bought: f64,
    // The amount we gave away in exchange.
    // pub exchanged: f64,
    // Moved to a record of the market.
    /// The amount of time put into trying to buy the item.
    pub time_budget: f64,
    /// The Abstract Market Value budget considered valid for the exchange
    /// if barter is not being used.
    pub amv_budget: f64,
    /// The amount of time returned as excess.
    pub time_returned: f64,
    /// The amount of Abstract Market Value returned as change from a 
    /// successful exchange.
    pub amv_returned: f64,
    /// The rate of success and how well it went yesterday. 
    /// 
    /// Should be between 0 and 1 inclusive.
    /// 
    /// - Increases if it satisfies desires fully or reasonably.
    /// - Decreases if it is over satisfied, other desires of lower levels
    ///   are not being satisfied.
    /// - Decreases if it was unable to buy up to our target for any reason.
    pub success: f64,
}

impl DesireInfo {
    /// Updates the sucess rate of the desire info.
    /// 
    /// Returns the new value and sets self.success to it as well.
    /// 
    /// The updated value is calculated as an estimated rolling average equal to
    /// (old * 9 + new) / 10
    pub fn update_sucess_rate(&mut self, success: f64) -> f64 {
        let result = (self.success * 9.0 + success) / 10.0;
        self.success = result;
        result
    }
}