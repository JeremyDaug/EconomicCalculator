//! The collection which manages multiple desires.
//! 
//! This collection manages and organizes the desires of an
//! actor into a rational fashion. Here is also where 
//! the validity of a potential barter is decided or not.
//! 
//! The primary struct is the Desires struct, but we also have
//! additional structs in DesireCoord, a summary of tier and index within a desire,
//! this is mostly for internal use and helps walking up (or down) desires.
//! 
//! DesireInfo is also used to record product data when buying or selling items.
//! It's the weights we are modifying to improve the AI going forward.

use std::collections::HashMap;

use itertools::Itertools;

use super::desire::{Desire, DesireItem, DesireError};

/// The ratio of value between one tier and an adjacent tier.
/// 
/// IE, a unit at tier 1 is worth 0.9 of an item from tier 0.
pub const TIER_RATIO: f64 = 0.9;

/// Desires are the collection of an actor's Desires. Includes their property
/// excess / unused wants, and AI data for acting on buying and selling.
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

    /// Goes over the property contained within desires and sifts them into
    /// the various desires that need them.
    /// 
    /// Clears out old satisfactions first, so only use when a hard recalculation
    /// is desired.
    pub fn sift_products(&mut self) {
        // clear old satisfactions
        for desire in self.desires.iter_mut() {
            desire.satisfaction = 0.0;
        }
        // Get the keys
        let keys = self.property.keys()
            .copied().collect_vec();
        // iterate over all property and try to add them.
        for key in keys {
            // sift that product as much as you can.
            self.sift_product(&key);
        }
    }

    /// Sifts a singular product into the various desires that seek it.
    /// 
    /// ## Plan
    /// 
    /// This will need to be expanded to allow for both specific product 
    /// satisfaction as well as general product satisfaction.
    pub fn sift_product(&mut self, product: &usize) {
        // get the first step.
        let mut curr = self
            .walk_up_tiers_for_item(&None, &DesireItem::Product(*product));
        // get the available product
        let mut available = match self.property.get(product) {
            Some(val) => val.clone(),
            None => 0.0
        };
        // loop over the desires for the product.
        while let Some(coord) = curr {
            // get the desire we're adding to.
            let desire = self.desires
                .get_mut(coord.idx)
                .expect("Desire Not Found");
            // add to it at the current tier.
            available = desire.add_satisfaction_at_tier(available, coord.tier)
                .expect("Misstep Somehow Occurred.");
            // if none left, break out of the loop
            if available == 0.0 {
                break;
            }
            // since do have more available, get the next
            curr = self
            .walk_up_tiers_for_item(&curr, &DesireItem::Product(*product));
        }
        // we have either run out of desires to possibly satisfy
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
    /// It returns the Values in and out, letting the caller use that
    /// information at their leasure.
    pub fn barter_value_difference(&self, 
    items_in: Vec<(usize, f64)>, 
    items_out: Vec<(usize, f64)>) -> ValueInOut {
        // get the value of the items in
        let mut in_value = vec![];
        for item in items_in {
            let value = self.in_barter_value(&item.0, item.1);
            if let Some(product_value) = value {
                in_value.push(product_value);
            }
        }
        // do the same for the items in
        let mut out_value = vec![];
        for item in items_out {
            let value = self.out_barter_value(&item.0, item.1);
            if let Some(product_value) = value {
                out_value.push(product_value);
            }
        }

        // summarize the in values take all things down to tier 0, for simplicity.
        let mut sum_in_value = 0.0;
        for value in in_value {
            sum_in_value += value.1 * Desires::tier_equivalence(0, value.0);
        }
        let mut sum_out_value = 0.0;
        for value in out_value {
            sum_out_value += value.1 * Desires::tier_equivalence(0, value.0);
        }

        ValueInOut::new(sum_in_value, sum_out_value)
    }

    /// Retrieves the internal barter value for a specific product as though it were going
    /// out and becoming unable to satisfy desires.
    /// 
    /// It returns the tier at which it starts, and the internal satisfaction value
    /// it will lose.
    /// 
    /// If there is no desire for that product exists, or there is no satisfaction to pull from,
    /// it returns None.
    /// 
    /// # Example
    /// 
    /// A product is desired at at tiers 0 2 4 and 6. One unit each. and it has 2 units of
    /// satisfaction.
    /// 
    /// We want to remove 1.5 units of the item.
    /// 
    /// It finds that desire and sets the tier to 2, it then goes through, pretending to subtract and
    /// count up the satisfaction lost. In this case it has 1 from 2 and 0.5 * 0.9^-2
    /// for a total loss of roughly 1.61728.
    /// 
    /// The resulting output would be Some(2, 1.61728).
    pub fn out_barter_value(&self, product: &usize, amount: f64) -> Option<(u64, f64)> {
        // get those desires with reserves to remove work with.
        let possible = self.desires.iter()
            .filter(|x| x.item.is_this_product(product) && x.satisfaction != 0.0)
            .collect_vec();
        if possible.len() == 0 {
            return None; // if no possible items to barter, return none.
        }
        // get the amount we are trying to remove
        let mut amount = amount;
        
        let tier = self
            .get_highest_satisfied_tier_for_item(DesireItem::Product(*product))
            .expect("Highest Satisfied Tier Not Found.");
        let mut curr = DesireCoord{ tier, idx: self.desires.len() };
        let mut weight = 0.0;
        loop {
            // walk down desires for this product. We start at the highest satisfied tier for that
            // item.
            let temp = self
                .walk_down_tiers_for_item(&curr, &DesireItem::Product(*product));
            if amount == 0.0 || temp.is_none() {
                // if none left to remove or no next step.
                return Some((tier, weight));
            }
            // if we have a new step and more to remove go for it
            if let Some(step) = temp {
                curr = step;
                // get the satisfaction available
                let desire = self.desires.get(step.idx).expect("Invalid Index.");
                let available = desire.satisfaction_at_tier(step.tier).expect("Bad Tier found.");
                if available > 0.0 {
                    // since satisfaction is available, do work
                    // get tier equivalency 
                    let eqv = Desires::tier_equivalence(tier, step.tier);
                    // get the smaller between available and amount
                    let min = available.min(amount);
                    // add min times weight to weight
                    weight += eqv * min;
                    // subtract from amount
                    amount -= min;
                }
            }
        }
    }

    /// Finds the highest tier for a particular item which has satisfaction
    /// 
    /// Returns None if no satisfaction in any product found.
    pub fn get_highest_satisfied_tier_for_item(&self, item: DesireItem) -> Option<u64> {
        // get those desires which have any satisfaction
        let possible = self.desires.iter()
            .filter(|x| x.item == item && x.satisfaction > 0.0)
            .collect_vec();
        // if any possible, go over and select the one with the highest satisfaction.
        if possible.len() > 0 {
            let result = possible.iter().map(|x| {
                x.satisfaction_up_to_tier().expect("No Satisfaction Found!")
            }).max().expect("No Minimum Found.");
            return Some(result);
        }
        None
    }
    
    /// Finds the highest tier which has any satisfaction.
    /// 
    /// Returns None if no satisfaction in any product found.
    pub fn get_highest_satisfied_tier(&self) -> Option<u64> {
        // get those desires which have any satisfaction
        let possible = self.desires.iter()
            .filter(|x| x.satisfaction > 0.0)
            .collect_vec();
        // if any possible, go over and select the one with the highest satisfaction.
        if possible.len() > 0 {
            let result = possible.iter().map(|x| {
                x.satisfaction_up_to_tier().expect("No Satisfaction Found!")
            }).max().expect("No Minimum Found.");
            return Some(result);
        }
        None
    }

    /// Take in a location and walk from that location back down our tiers. This should
    /// result in a full reversal of self.walk_up_tiers().
    /// 
    /// This cannot accept a None value, as desires can be infinite, creating no guaranteed 
    /// that there is a 'last' desire.
    /// 
    /// The Desire Coord does not need to have an idx < self.desires.len(), if it
    /// has an idx > self.desires.len() it will drag it down to len, allowing you to
    /// start at a tier specifically, without touching a higher tier.
    pub fn walk_down_tiers(&self, prev: &DesireCoord) -> Option<DesireCoord> {
        // set the current equal to prev so we can edit safely.
        let mut curr = *prev;
        // if the current idx is past our endpoint, then smash it down.
        if curr.idx > self.desires.len() {
            curr.idx = self.desires.len();
        }
        
        // walk down the idx
        loop {
            if curr.idx == 0 && curr.tier == 0 {
                break; // if we are currently at 0,0, then break out and return None.
            }
            else if curr.idx == 0 { // if index is 0, then go to next tier.
                curr.tier -= 1;
                curr.idx = self.desires.len();
            }
            curr.idx -= 1; // subtract index

            if self.desires[curr.idx].steps_on_tier(curr.tier) {
                // if the desire steps on this tier, then return
                return Some(curr);
            }
            // if it's not try again.
        }

        None
    }

    /// Like walk_down_tiers(), this walks down the desires we have. But, it only returns
    /// the indexes and tiers for a specific desire, instead of all of them.
    /// 
    /// This cannot accept a None value, as desires can be infinite, creating no guaranteed 
    /// that there is a 'last' desire.
    /// 
    /// The Desire Coord does not need to have an idx < self.desires.len(), if it
    /// has an idx > self.desires.len() it will drag it down to len, allowing you to
    /// start at a tier specifically, without touching a higher tier.
    pub fn walk_down_tiers_for_item(&self, prev: &DesireCoord, item: &DesireItem) -> Option<DesireCoord> {
                // set the current equal to prev so we can edit safely.
                let mut curr = *prev;
                // if the current idx is past our endpoint, then smash it down.
                if curr.idx > self.desires.len() {
                    curr.idx = self.desires.len();
                }
                
                // walk down the idx
                loop {
                    if curr.idx == 0 && curr.tier == 0 {
                        break; // if we are currently at 0,0, then break out and return None.
                    }
                    else if curr.idx == 0 { // if index is 0, then go to next tier.
                        curr.tier -= 1;
                        curr.idx = self.desires.len();
                    }
                    curr.idx -= 1; // subtract index
        
                    if self.desires[curr.idx].steps_on_tier(curr.tier) && 
                        self.desires[curr.idx].item == *item {
                        // if the desire steps on this tier, then return
                        return Some(curr);
                    }
                    // if it's not try again.
                }
        
                None
    }

    /// Take an item and finds the lowest tier available which can still accept the item.
    /// 
    /// Used primarily to nicely find where to put an item when sifting.
    pub fn get_lowest_unsatisfied_tier_of_item(&self, item: DesireItem) -> Option<u64> {
        // get those desires which contain our item and are not fully satisfied.
        let possible = self.desires.iter()
            .filter(|x| x.item == item && !x.is_fully_satisfied())
            .collect_vec();
        // if any possible, go over them and select the one with the lowest unsatisfied tier.
        if possible.len() > 0 {
            let result = possible.iter().map(|x| {
                x.unsatisfied_to_tier().expect("Full Satisfaction found, check filter.")
            }).min().expect("Minimum Not Found. Panic!");
            return Some(result)
        }
        // if not found in any, return none, meaning there are no tiers which need more of the item.
        None
    }

    /// Finds the lowest unsatisfied tier of all of our desires.
    /// 
    /// If all desires are satisfied, it returns None.
    pub fn get_lowest_unsatisfied_tier(&self) -> Option<u64> {
        // get those desires which contain our item and are not fully satisfied.
        let possible = self.desires.iter()
            .filter(|x| !x.is_fully_satisfied())
            .collect_vec();
        // if any possible, go over them and select the one with the lowest unsatisfied tier.
        if possible.len() > 0 {
            let result = possible.iter().map(|x| {
                x.unsatisfied_to_tier().expect("Full Satisfaction found, check filter.")
            }).min().expect("Minimum Not Found. Panic!");
            return Some(result)
        }
        // if not found in any, return none, meaning there are no tiers which need more of the item.
        None
    }

    /// Retrieves the internal barter value for a specific product as though it were coming
    /// in and satisfying desires.
    /// 
    /// It returns the tier at which it starts, and the internal satisfaction value
    /// it satisfies.
    /// 
    /// If it is not desired at all, it returns None.
    /// 
    /// # Example
    /// 
    /// A product is desired at at tiers 5 7 9 and 11. One unit each.
    /// 
    /// It already has 1 unit in it (tier 5), and we want to insert 1.5 units more.
    /// 
    /// It finds that desire and sets the tier to 7, it then walks up the desire and
    /// calculates the weight (1 + 0.5*0.9 &#8773; 1.45).
    /// 
    /// The resulting output would be Some(7, 1.45).
    pub fn in_barter_value(&self, product: &usize, amount: f64) -> Option<(u64, f64)> {
        // get those desires which want it an can still be satisfied
        let possible = self.desires.iter()
            .filter(|x| x.item.is_this_product(&product) && !x.is_fully_satisfied())
            .collect_vec();
        // if no desires for that item exist, or they are all fully satisfied, return none.
        if possible.len() == 0 {
            return None;
        }
        // get the amount we are trying to check on adding.
        let mut amount = amount;
        let mut curr = None;
        let tier = self
            .get_lowest_unsatisfied_tier_of_item(DesireItem::Product(*product))
            .expect("Lowest unsatisfied desire not found.");
        let mut weight = 0.0;
        loop {
            // walk up for this desire. Currently, we cheat and just walk up from the base
            // and ignore anything that is satisfied at that step.
            curr = self.walk_up_tiers_for_item(&curr, &DesireItem::Product(*product));
            // if we have run out of amount, or have reached the end of the desires, break our
            // loop
            if amount == 0.0 || curr.is_none() {
                return Some((tier, weight));
            }
            // if we got back a new step, work.
            if let Some(step) = curr {
                // get the desire and the unsatisfied desire at this step.
                let desire = self.desires.get(step.idx).expect("Invalid index found.");
                let diff = desire.amount - desire.satisfaction_at_tier(step.tier).expect("No Value Given.");
                if diff > 0.0 {
                    // since the current step has missing satisfaction.
                    // get the equivalence ratio.
                    let equiv = Desires::tier_equivalence(tier, step.tier);
                    // get the smaller of the two
                    let min = diff.min(amount);
                    // add the min times equivelancy to the weight
                    weight += equiv * min;
                    // subtract the minimum from the amount
                    amount -= min;
                }
            }

            // since we have not reached an end go back to the top.
        }
        
    }

    /// Tier Equivalence between two tiers. 
    /// 
    /// With the start tier equal to 1, end tiers above it decline by 0.9 per level
    /// difference. Going down they increase by 1/0.9
    /// 
    /// This defines how many units at the end tier is considered equivalent to lose in return
    /// for 1 unit of the start tier.
    /// 
    /// IE
    /// - start 10, end 11 = 0.9^-1    1 start = 0.9 end
    /// - start 10, end 12 = 0.9^-2    1 start = 0.81 end
    /// - start 10, end 8 = 0.9^2      1 start = 1.23. end
    pub fn tier_equivalence(start: u64, end: u64) -> f64 {
        let start = start as f64;
        let end = end as f64;
        TIER_RATIO.powf(end - start)
    }

    /// Walk up the tiers of our desires.
    /// 
    /// It takes as a parameter our previous desire location.
    /// If given none, it selects the first and return that
    /// (lowest tier, first in vector order).
    /// 
    /// If there is no next step, it returns None.
    pub fn walk_up_tiers(&self, prev: Option<DesireCoord>) -> Option<DesireCoord> {
        // If no previous given, make it.
        // if given increment idx.
        let mut prev = if prev.is_none() { DesireCoord{tier: 0, idx: 0}} 
            else { prev.expect("Failed somehow!").increment_idx()};

        // any desire has a step above here, return true.
        while self.desires.iter().any(|x| {
            x.is_infinite() || matches!(x.end, Some(end) if end > prev.tier)
        }) {
            if prev.idx == self.desires.len() { 
                prev.idx = 0; 
                // Could make this smarter, but this will do for now.
                // if this becomes a problem. rework to be smarter.
                prev.tier += 1; 
            }
            while prev.idx < self.desires.len() {
                if self.desires[prev.idx].steps_on_tier(prev.tier) {
                    return Some(prev)
                }
                prev.idx += 1;
            }
        }
        None
    }

    /// Walk up the tiers for a specific desire item.
    /// 
    /// It takes in the tier we are starting at, and the index of the previous we looked
    /// at. The previous index need not be valid to be used.
    /// 
    /// If there is no next step, it returns None.
    pub fn walk_up_tiers_for_item(&self, prev: &Option<DesireCoord>, item: &DesireItem) -> Option<DesireCoord> {
        // If no previous given, make it.
        // if given increment idx.
        let mut curr = if prev.is_none() { DesireCoord{tier: 0, idx: 0}} 
            else { prev.expect("Failed somehow!").increment_idx()};

        // any desire has a step above here, return true.
        while self.desires.iter()
            .filter(|x| x.item == *item)
            .any(|x| {
            x.is_infinite() || matches!(x.end, Some(end) if end > curr.tier)
        }) {
            if curr.idx >= self.desires.len() { 
                curr.idx = 0; 
                // Could make this smarter, but this will do for now.
                // if this becomes a problem. rework to be smarter.
                curr.tier += 1; 
            }
            while curr.idx < self.desires.len() {
                if self.desires[curr.idx].item == *item &&
                 self.desires[curr.idx].steps_on_tier(curr.tier) {
                    return Some(curr)
                }
                curr.idx += 1;
            }
        }
        None
    }

    /// Clears self.desires
    pub fn clear_desires(&mut self) {
        self.desires.clear()
    }

    /// How many desires are contained.
    pub fn len(&self) -> usize {
        self.desires.len()
    }
}

/// The coordinates of a desire, both it's tier and index in desires. Used for tier walking.
#[derive(Debug, Clone, Copy)]
pub struct DesireCoord {
    pub tier: u64,
    pub idx: usize
}

impl DesireCoord {
    /// Increments the index of the desire coord and returns it.
    /// 
    /// Easier when copy and increment are needed.
    fn increment_idx(self) -> Self{
        DesireCoord { tier: self.tier, idx: self.idx + 1}
    }
}

/// A simple sturct to make passing value difference around easier and
/// clearer.
/// 
/// Likely to move this to someplace more public later as uses elsewhere are foreseeable.
#[derive(Debug, Clone, Copy)]
pub struct ValueInOut {
    pub in_value: f64,
    pub out_value: f64
}

impl ValueInOut {
    pub fn new(in_value: f64, out_value: f64) -> Self { Self { in_value, out_value } }

    /// returns the difference between the in and out values.
    /// 
    /// If Positive then it means the in value is greater than the
    /// out value.
    pub fn diff(&self) -> f64 {
        self.in_value - self.out_value
    }

    /// A quick check to see if the value exchange is acceptable.
    /// returns true if self.in_value > self.out_value.
    pub fn acceptable(&self) -> bool {
        self.in_value > self.out_value
    }
}

/// The information on a product which is desired.
#[derive(Debug)]
pub struct DesireInfo {
    /// The target amount we want to buy.
    pub target: f64,
    /// The amount we have bought today.
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