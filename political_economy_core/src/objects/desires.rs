//! The collection which manages multiple desires.
//! 
//! This collection manages and organizes the desires of an
//! actor into a rational fashion. Here is also where 
//! the validity of a potential barter is decided or not.

use std::collections::HashMap;

use itertools::Itertools;

use super::desire::{Desire};

#[derive(Debug)]
pub struct Desires {
    /// All of the desires we are storing and looking over.
    pub desires: Vec<Desire>,
    /// The property currently owned bey the actor.
    pub property: HashMap<usize, f64>,
    /// The wants stored and not used up yet.
    pub want_store: HashMap<usize, f64>,
    /// The data of our succes in shopping.
    pub shopping_data: HashMap<usize, DesireInfo>,
}

impl Desires {
    /// Creates a new desire collection based on a list of desires.
    pub fn new(desires: Vec<Desire>) -> Self {
        let mut shopping_data: HashMap<usize, DesireInfo> = HashMap::new();
        for product in desires.iter()
            .filter(|x| x.item.is_product()) {
                shopping_data.insert(product.item.unwrap().clone(), 
                DesireInfo::new());
            }
        Desires {
            desires,
            property: HashMap::new(),
            want_store: HashMap::new(),
            shopping_data,
        }
    }

    /// Adds a number of units to the property.
    pub fn add_property(&mut self, product: usize, amount: &f64) {
        *self.property.entry(product).or_insert(0.0) += amount;
    }

    /// Adds a number of wants.
    pub fn add_want(&mut self, want: usize, amount: &f64) {
        *self.want_store.entry(want).or_insert(0.0) += amount;
    }

    /// Adds a desire to our collection.
    /// 
    /// If the desire is a match for an existing desire, it will 
    /// combine it with the existing match, adding the amount
    /// desired per level.
    /// 
    /// This can also be used to subtract a desire from the collection.
    /// 
    /// If a desire is reduced to 0 or less, then it will be removed.
    pub fn add_desire(&mut self, desire: &Desire) {
        let existing_position = self.desires.iter().position(|x| x.is_match(desire));
        
        // if the desire is a match for an exsiting one, add to that.
        if let Some(pos) = existing_position {
            self.desires[pos].amount += desire.amount;
            // if this match was subtracted such that we are now at or below 0, remove it entirely.
            if self.desires[pos].amount <= 0.0 {
                self.desires.remove(pos);
            }
            return;
        }

        // if it doesn't already exist, duplicate and insert.
        let dup = desire.clone();
        self.desires.push(dup);
    }

    /// Gets the difference in value between the items in and the 
    /// items out given the current state of the desires.
    /// 
    /// If items are on an equal tier then they are equal on a 
    /// unit-per-unit basis. For each tier of separation the value
    /// diverges geometrically. For every tier of separation the
    /// value of an item declines by 0.9^(n) per unit, where n is
    /// the difference between the lower and higher tier. 
    /// 
    /// If item_out is not currently desired (either having no desires 
    /// which want it or a desire which matches, but it is totally 
    /// satisfied) then it's value is equal to 0.
    /// 
    /// The value returned is the difference between the item_in's 
    /// value and the item_out's value. If the value is negative, 
    /// than the items out are more valueable than the items in at
    /// this moment, if positive then they are less valueable.
    /// 
    /// # Example
    /// 
    /// Item **A** is on Tier 5, Item **B** is on tier 6.
    /// 
    /// The value of **A** = 1 / unit and **B** = 0.9 / unit
    /// 
    /// If **B** is on tier 10 then
    /// 
    /// **B** = 0.9^5 / unit.
    pub fn barter_value(&self, item_in: (usize, f64), item_out: (usize,f64)) -> f64 {
        todo!("Needs a function to find the lowest desire tier which can accept an item. Products only.")
        // 0.0
    }

    pub fn get_lowest_unsatisfied_tier(&self, item: usize) -> Option<u64> {
        let possible = self.desires.iter()
            .filter(|x| x.item.is_this_product(&item)).collect_vec();
        // if any possible, go over them and select the one with the lowest unsatisfied tier.
        if possible.len() > 0 {
            let result = possible.iter().map(|x| {
                let sat = x.unsatisfied_to_tier();
                match sat {
                    Some(val) => val,
                    None => u64::MAX
                }
            });
            
        }
        // if not found in any, return none, meaning there are no tiers which need more of the item.
        None
    }

    /// Checks the desires to see if the one being asked would accept
    /// a barter. If the items in are considered 
    pub fn check_barter(&self, item_out: (usize, f64), items_in: HashMap<usize, f64>) -> f64 {
        0.0
    }

    /// Used to walk up the tiers of our desires.
    fn walk_up_tiers(&mut self, tier: u64, idx: usize) -> Option<(u64, usize)> {
        // any desire has a step above here, return true.
        while self.desires.iter().any(|x| {
            x.is_infinite() || matches!(x.end, Some(end) if end > tier)
        }) {

        }

        None
    }
}

/// The information on a product which is desired.
#[derive(Debug)]
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
    pub fn new() -> Self {
        DesireInfo { 
            target: 0.0, 
            bought: 0.0, 
            time_budget: 0.0, 
            amv_budget: 0.0, 
            time_returned: 0.0, 
            amv_returned: 0.0, 
            success: 0.5 }
    }

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