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

use std::{collections::{HashMap, HashSet}};

use itertools::Itertools;

use crate::{data_manager::DataManager, constants::UNABLE_TO_PURCHASE_REDUCTION};

use super::{desire::{Desire, DesireItem}, market::{MarketHistory}, process::PartItem};

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
    /// The highest tier to which these desires have been fully
    /// satisfied.
    /// 
    /// A rough measure of contentment. 
    /// 
    /// Low values with stuffed desires mark material wealth with
    /// low contentment. High values with sparse desires mark material
    /// poverty but personal contentment.
    pub full_tier_satisfaction: u64,
    /// How many tiers, skipping empty ones, which have been filled.
    /// 
    /// A rough measure of total satisfaction. Removes contentment and
    /// emphasizes material satisfaction.
    pub hard_satisfaction: u64,
    /// The sum of desires satisfied, a more accurate, if market insensitive
    /// measure of weath.
    pub quantity_satisfied: f64,
    /// The satisfaction gained above our self.full_tier_satisfaction,
    /// items are reduced by tier satisfaction calculations.
    pub partial_satisfaction: f64,
    /// The satisfaction of desires measured in abstract market
    /// value.
    /// 
    /// It's the sum of each product's price, not scaled by tier.
    pub market_satisfaction: f64,
    /// The highest tier of satisfaction reached by any desire.
    pub highest_tier: u64,
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
            full_tier_satisfaction: 0,
            hard_satisfaction: 0,
            quantity_satisfied: 0.0,
            partial_satisfaction: 0.0,
            market_satisfaction: 0.0,
            highest_tier: 0,
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
        // loop over the desires for the product so long as we have one to add to.
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

    /// Adds or subtracts a number of units to the property.
    /// if the property reaches 0.0, it removes it from property entirely.
    /// 
    /// If the amount subtracted results in a negative, it removes and returns 
    /// the excess back as positive. If no excess, it returns 0.0.
    /// 
    /// IE, 5.0 in property, 10.0 subtracted, returns 5.0.
    pub fn add_property(&mut self, product: usize, amount: &f64) -> f64 {
        let value = *self.property.entry(product)
        .and_modify(|x| *x += amount)
        .or_insert(*amount);
        if value <= 0.0 {
            self.property.remove(&product);
            return -value;
        }
        0.0
    }

    /// Removes a number of product units from property, if needed, it also
    /// removes it from satisfaction. Returns how much was successfully 
    /// removed.
    pub fn remove_property(&mut self, product: usize, amount: &f64) -> f64 {
        let item = DesireItem::Product(product);
        let mut target = self.property.get(&product)
            .unwrap_or(&0.0).min(*amount);
        let result = target;
        // with the target (minimum between asked and available) remove 
        // from property
        *self.property.entry(product).or_insert(0.0) -= target;

        // now remove from satisfaction.
        let start_tier = 
            self.get_highest_satisfied_tier_for_item(DesireItem::Product(product));

        let mut curr_coord = if start_tier.is_some() {
            DesireCoord{ tier: start_tier.unwrap(), idx: self.len() }
        } else {
            return result;
        };
        // remove satisfaction up to our target or we run out of satisfaction to remove.
        loop {
            match self.walk_down_tiers_for_item(&curr_coord, &item) {
                Some(res) => curr_coord = res,
                None => break,
            }
            let mut desire = self.desires.get_mut(curr_coord.idx)
                .expect("idx Not found?");
            // get the smaller between the target to remove and the amount 
            let available = target.min(
                desire.satisfaction_at_tier(curr_coord.tier)
                    .expect("Bad Tier?"));
            // subtract from satisfaction and the target.
            desire.satisfaction -= available;
            target -= available;
            if target == 0.0 {
                return result;

            }
        }

        result
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
    pub fn walk_down_tiers_for_item(&self, prev: &DesireCoord, 
    item: &DesireItem) -> Option<DesireCoord> {
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

    /// Whether a selected tier is fully satisfied or not.
    pub fn satisfied_at_tier(&self, tier: u64) -> bool {
        self.desires.iter()
        .filter(|x| x.steps_on_tier(tier))
        .all(|x| x.satisfaction_at_tier(tier).expect("Bad Step") == x.amount)
    }

    /// Get's the total satisfaction of all our desires at a specific tier.
    /// If nothing steps on it it return's 0.0.
    pub fn total_satisfaction_at_tier(&self, tier: u64) -> f64 {
        self.desires.iter().filter(|x| x.steps_on_tier(tier))
            .map(|x| x.satisfaction_at_tier(tier)
                        .expect("Doesn't Step on tier."))
            .sum()
    }

    /// Gets the total desired items for all desires at a specific tier.
    /// If nothing steps on that tier it returns 0.0.
    pub fn total_desire_at_tier(&self, tier: u64) -> f64 {
        self.desires.iter().filter(|x| x.steps_on_tier(tier))
            .map(|x| x.amount)
            .sum()
    }

    /// Updates the satisfactions for our desires.
    /// 
    /// Does not calculate satisfaction base on market history.
    pub fn update_satisfactions(&mut self) {
        // start with full tier satisfaction and highest tier.
        self.full_tier_satisfaction = u64::MAX;
        self.highest_tier = 0;
        // for each desire
        for desire in self.desires.iter() {
            let tier = desire.satisfaction_up_to_tier().expect("Not Satisfied somehow?");
            if !desire.is_fully_satisfied() {
                // get it's highest fully satisfied tier
                let tier = if desire.satisfaction_at_tier(tier)
                .expect("No satisfaction?") < 1.0 {
                    tier - 1
                } 
                else { tier };
                // and set result to the smaller between the current and
                // the new tier.
                self.full_tier_satisfaction = self.full_tier_satisfaction.min(tier);
            }
            // always check against the highest tier
            self.highest_tier = self.highest_tier.max(tier)
        }
        // get quantity satisfied
        self.quantity_satisfied = self.desires.iter()
        .map(|x| x.satisfaction).sum();

        // partial satisfaction
        self.partial_satisfaction = 0.0;

        for tier in self.full_tier_satisfaction..(self.highest_tier+1) {
            // go from the full_tier to the highest tier, summing as needed.
            let sat = self.total_satisfaction_at_tier(tier);
            let total = self.total_desire_at_tier(tier);
            if total > 0.0 {
                let sat_at_tier = sat *
                    Desires::tier_equivalence(self.full_tier_satisfaction, 
                        tier);
                self.partial_satisfaction += sat_at_tier;
            }
        }

        // finish with hard satisfaction
        let lowest = self.desires.iter()
            .map(|x| x.start).min().expect("Value not found!");
        let mut skipped = 0;
        for tier in lowest..self.full_tier_satisfaction {
            if self.desires.iter().filter(|x| x.steps_on_tier(tier)).count() == 0 {
                skipped += 1;
            }
        }
        // The highest full tier we satisfy, minus skipped tiers, +1 to correctly fence post it.
        self.hard_satisfaction = self.full_tier_satisfaction - skipped + 1;
    }

    /// Calculates, sets, and returns the market satisfaction for these 
    /// desires based on the market history given.
    /// 
    /// This is an estimate of how much satisfaction we have in terms 
    /// of AMV. This should be treated as a rough estimate of wealth being 
    /// used by the pop to satisfy their desires.
    pub fn market_satisfaction(&mut self, market: &MarketHistory) -> f64 {
        self.market_satisfaction = 0.0;
        for desire in self.desires.iter()
        .filter(|x| x.item.is_product()) {
            let product = desire.item.unwrap();
            self.market_satisfaction += 
                market.get_product_price(product, 0.0) * 
                desire.satisfaction;
        }
        self.market_satisfaction
    }

    /// Market Wealth, a measure of how much is owned in AMV. This is 
    /// everything, not just satisfaction, measured in it's wealth.
    /// 
    /// This is how much they own and how valuable it is in market value.
    pub fn market_wealth(&self, market: &MarketHistory) -> f64 {
        self.property.iter()
        // get the price * the amount owned.
        .map(|(product, quantity)| 
            quantity * market.get_product_price(product, 0.0))
        // then add together.
        .sum()
    }

    /// Clears self.desires
    pub fn clear_desires(&mut self) {
        self.desires.clear()
    }

    /// How many desires are contained.
    pub fn len(&self) -> usize {
        self.desires.len()
    }

    /// Gets the sum total satisfaction of a specific item.
    pub fn total_satisfaction_of_item(&self, item: DesireItem) -> f64 {
        self.desires.iter().filter(|x| x.item == item)
            .map(|x| x.satisfaction).sum()
    }

    /// # Consume and Shift Wants
    /// 
    /// This is used to satisfy our wants with our property. 
    /// 
    /// Currently, it attempts to do this in the following fashion.
    /// 
    /// 1. Consume any wants from our store.
    /// 2. Try to use products which satisfy it by owning it.
    /// 3. Try to satisfy by using products.
    /// 4. Try to satisfy by consuming prdoucts.
    /// 
    /// TODO Improve this by using product memory to prioritize sources for each want. Rather than following strict ordering.
    pub fn consume_and_sift_wants(&mut self, data: &DataManager) -> HashMap<usize, f64> {
        let mut consumed = HashMap::new();
        // get all our property and wants into untouched, used, and consumed.
        let mut used_products: HashMap<usize, f64> = HashMap::new();
        // create our tracker so we can tell when we're done.
        let mut tracker = HashSet::new();
        for (idx, desire) in self.desires.iter().enumerate() {
            // preemptively add products to our tracked set.
            if let DesireItem::Product(id) = desire.item {
                tracker.insert(idx);
            }
        }

        // with our untouched stuff sorted, begin sifting.
        let mut curr = self.walk_up_tiers(None);
        while let Some(coord) = curr {
            // if tracker has the same number of items as our desires, we've 
            // cleared out everything we can. Exit.
            if tracker.len() == self.desires.len() { break; }
            // if the index is already in our tracker, then we skip as we've 
            // already done what we can with it
            if tracker.contains(&coord.idx) { 
                curr = self.walk_up_tiers(curr);
                continue; 
            }
            // since it's not already dealt with, deal with it.
            // this should never be a product.
            let mut desire = self.desires
                .get_mut(coord.idx).unwrap();
            let want_id = desire.item.unwrap();
            let mut ext_target = desire.amount;

            // try for unused wants first
            if let Some(extant) = self.want_store.get_mut(want_id) {
                // with existing available to satisfy, push it in to satisfy our current tier.
                let shift = extant.min(desire.amount);
                // with our shift amount gotten, remove from property and add to the desire's satisfaction.
                desire.satisfaction += shift;
                ext_target -= shift;
                *self.want_store.get_mut(want_id).unwrap() -= shift;
                if *self.want_store.get(want_id).unwrap() == 0.0 {
                    // if now empty, remove it.
                    self.want_store.remove(want_id);
                }
            }
            if ext_target == 0.0 { // if that did it, go to next desire.
                if desire.past_end(coord.tier+1) { 
                    // if this is the last tier, or fully satisfied, add to tracker.
                    tracker.insert(coord.idx);
                }
                curr = self.walk_up_tiers(curr);
                continue;
            }

            // next try ownership sources
            let want_info = data.wants.get(want_id).expect("Want not in Data!");
            for product_id in want_info.ownership_sources.iter() {
                if let Some(available) = self.property.get_mut(product_id) { // if we have some, get it
                    let product_info = data.products.get(product_id).expect("Product Not found!");
                    let eff = product_info.wants.get(want_id).expect("Product Doesn't give want!");
                    let ratio = ext_target / eff; // how many products we need to match our target.
                    // get how many we can get 
                    let reserve = ratio.min(*available);
                    // move the reserve from property into used
                    used_products.entry(*product_id)
                        .and_modify(|x| *x += reserve).or_insert(reserve);
                    consumed.entry(*product_id)
                        .and_modify(|x| *x += reserve).or_insert(reserve);
                    let entry = self.property.get_mut(product_id).unwrap();
                    *entry -= reserve;
                    if *entry == 0.0 {
                        self.property.remove(product_id);
                    }
                    // update our wants
                    for (want, amount) in product_info.wants.iter() {
                        if want == want_id { // if it is our want, add to sat and remove from 
                            desire.satisfaction += amount * reserve;
                            ext_target -= amount * reserve;
                        } else { // if not our want, add to the store.
                            *self.want_store.entry(*want).or_insert(0.0) += amount * reserve;
                        }
                    }
                    // since we added to our satisfaction, check if we're done, if we are, break out of this
                    if ext_target == 0.0 {
                        break;
                    }
                }
            }
            if ext_target == 0.0 { // if that did it, go to next desire.
                if desire.past_end(coord.tier+1) { // if this is the last tier, add to tracker.
                    tracker.insert(coord.idx);
                }
                curr = self.walk_up_tiers(curr);
                continue;
            }
            // if that wasn't good enough, try Use Sources
            // TODO as part of the prioritization (or as a mid-step) this should prioritize the most efficient options rather than the order given by the want.
            for use_id in want_info.use_sources.iter() {
                let use_process = data.processes.get(use_id).expect("Process not found!");
                // get how many iterations of the process we need to do it.
                let outputs = use_process.effective_output_of(PartItem::Want(*want_id));
                let target = ext_target / outputs; // how many iterations we need to meet the desire.
                // throw our available property into the process to see if anythnig falls out.
                let results = use_process.do_process(&self.property, 
                    &self.want_store, &0.0, &0.0, Some(target), true);
                if results.iterations > 0.0 { // if anything was done, go through the parts and apply the changes.
                    for (product, quantity) in results.input_output_products.iter() {
                        // add outputs and subtract inputs (inputs are already negative.)
                        self.property.entry(*product)
                            .and_modify(|x| *x += quantity).or_insert(*quantity);
                        if *quantity < 0.0 {
                            // if being consumed, add to our consumed result.
                            consumed.entry(*product)
                                .and_modify(|x| *x -= quantity).or_insert(-quantity);
                        }
                        if *self.property.get(product).unwrap() == 0.0 {
                            self.property.remove(product);
                        }
                    }
                    for (product, quantity) in results.capital_products.iter() {
                        // shift used capital from property to used.
                        *self.property.get_mut(product).unwrap() -= quantity;
                        used_products.entry(*product)
                            .and_modify(|x| *x += quantity).or_insert(*quantity);
                        consumed.entry(*product)
                            .and_modify(|x| *x += quantity).or_insert(*quantity);
                        if *self.property.get(product).unwrap() == 0.0 {
                            self.property.remove(product);
                        }
                    }
                    for (want, quantity) in results.input_output_wants.iter() {
                        if want == want_id { // if it's our want, add to satisfaction
                            desire.satisfaction += quantity;
                        } else { // else, add to our store.
                            *self.want_store.entry(*want).or_insert(0.0) += quantity;
                        }
                    }
                }
                if ext_target == 0.0 {
                    break;
                }
            }
            if ext_target == 0.0 { // if that did it, go to next desire.
                if desire.past_end(coord.tier+1) { // if this is the last tier, add to tracker.
                    tracker.insert(coord.idx);
                }
                curr = self.walk_up_tiers(curr);
                continue;
            }
            // last thing to try, consumption
            // TODO as part of the prioritization (or as a mid-step) this should prioritize the most efficient options rather than the order given by the want.
            for consumption_id in want_info.consumption_sources.iter() {
                let use_process = data.processes.get(consumption_id).expect("Process not found!");
                // get how many iterations of the process we need to do it.
                let outputs = use_process.effective_output_of(PartItem::Want(*want_id));
                let target = ext_target / outputs; // how many iterations we need to meet the desire.
                // throw our available property into the process to see if anythnig falls out.
                let results = use_process.do_process(&self.property, 
                    &self.want_store, &0.0, &0.0, Some(target), true);
                if results.iterations > 0.0 { // if anything was done, go through the parts and apply the changes.
                    for (product, quantity) in results.input_output_products.iter() {
                        // add outputs and subtract inputs (inputs are already negative.)
                        self.property.entry(*product)
                            .and_modify(|x| *x += quantity).or_insert(*quantity);
                        if *quantity < 0.0 {
                            // if negative, add to consumed
                            consumed.entry(*product)
                                .and_modify(|x| *x -= quantity).or_insert(-quantity);
                        }
                        if *self.property.get(product).unwrap() == 0.0 {
                            self.property.remove(product);
                        }
                    }
                    for (product, quantity) in results.capital_products.iter() {
                        // shift used capital from property to used.
                        *self.property.get_mut(product).unwrap() -= quantity;
                        used_products.entry(*product)
                            .and_modify(|x| *x += quantity).or_insert(*quantity);
                        consumed.entry(*product)
                                .and_modify(|x| *x += quantity).or_insert(*quantity);
                        if *self.property.get(product).unwrap() == 0.0 {
                            self.property.remove(product);
                        }
                    }
                    for (want, quantity) in results.input_output_wants.iter() {
                        if want == want_id { // if it's our want, add to satisfaction
                            desire.satisfaction += quantity;
                        } else { // else, add to our store.
                            *self.want_store.entry(*want).or_insert(0.0) += quantity;
                        }
                    }
                }
                if ext_target == 0.0 {
                    break;
                }
            }
            if ext_target == 0.0 { // if that did it, go to next desire.
                if desire.past_end(coord.tier+1) { // if this is the last tier, add to tracker.
                    tracker.insert(coord.idx);
                }
                curr = self.walk_up_tiers(curr);
                continue;
            }
            // we got to the end without getting out, so put this desire into finished.
            tracker.insert(coord.idx);
            // get next one
            curr = self.walk_up_tiers(curr);
        }

        // at the end, add all of our used items back into property.
        for (item, quantity) in used_products {
            self.add_property(item, &quantity);
        }
        consumed
    }

    /// # Want consuming function. 
    /// 
    /// Takes the desire we are trying to satisfy, and at what tier.
    /// 
    /// It also takes the wants we have available and that have already
    /// been consumed. We update these as we go.
    /// 
    /// We return true if we were able to satisfy the desire 
    /// 
    /// FIXME Not Used, currently non-functional due to lifetime issues.
    pub fn shift_want_for_satisfaction(&mut self, desire_coord: &DesireCoord, 
    ext_target: &mut f64, untouched_wants: &mut HashMap<usize, f64>,
    consumed_wants: &mut HashMap<usize, f64>) -> bool{
        // get the desire we're modifying.
        let mut desire = self.desires.get_mut(desire_coord.idx).unwrap();
        // get the want we're looking at
        let want_id = desire.item.unwrap();
        if let Some(available) = untouched_wants.get_mut(want_id) {
            // get how much we can or need to shift for this tier
            let shift = available.min(*ext_target);
            // then add to consumed and the desire
            *consumed_wants.entry(*want_id).or_insert(0.0) += shift;
            desire.satisfaction += shift;
            // and remove from untouched and our target amount
            *available -= shift;
            *ext_target -= shift;
            if *ext_target == 0.0 {
                return true; 
            }
        }
        return false;
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