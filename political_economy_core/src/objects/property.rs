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

use std::{collections::{HashMap, HashSet}, ops::{AddAssign, Add, Sub, SubAssign, Div}};

use itertools::Itertools;

use crate::{data_manager::DataManager, constants::{TIER_RATIO, SHOPPING_TIME_ID}};

use super::{desire::Desire, market::MarketHistory, property_info::PropertyInfo, item::Item};

/// Desires are the collection of an actor's Desires. Includes their property
/// excess / unused wants, and AI data for acting on buying and selling.
#[derive(Debug, Clone)]
pub struct Property {
    /// All of the desires we are storing and looking over.
    pub desires: Vec<Desire>,
    /// The property currently owned bey the actor.
    pub property: HashMap<usize, PropertyInfo>,
    /// The wants stored and not used up yet.
    pub want_store: HashMap<usize, f64>,
    /// Whether the pop who owns this property is a disorganized firm or not.
    pub is_disorganized: bool,
    /// How much time we have worked on average over the past few days.
    pub work_time: f64,
    /// How much we were paid 'today' in AMV, Updated with each paycheck.
    pub todays_wage: f64,
    /// The divisor of our total paycheck when not paid daily.
    pub pay_period: usize,
    /// How much we have been paid directly, in AMV for the past few days.
    pub wage_estimate: f64,
    /// How much we have recieved from our employer indirectly, IE, Benefits packages
    /// Want Splash, etc.
    pub extra_benefits: f64,
    /// The processes we are planning to complete for consumption purposes.
    /// Includes the number of iterations we intend to do.
    /// Should be updated with changing property and reservations.
    pub process_plan: HashMap<usize, f64>,
    /// The expected inputs and outputs of our process plan.
    /// Only includes inputs and outputs, not capital for sanity reasons.
    pub product_expectations: HashMap<usize, f64>,
    /// The Expected inputs and outputs of our process plan for wants.
    pub want_expectations: HashMap<usize, f64>,
    /// The highest tier of satisfaction which is fully satisfied.
    /// 
    /// No tier is fully satisfied, then it returns None.
    /// 
    /// If desires are totally satiated, it returns the highest end tier 
    /// desire.
    /// 
    /// A rough measure of contentment. 
    /// 
    /// Low values with stuffed desires mark material wealth with
    /// low contentment. High values with sparse desires mark material
    /// poverty but personal contentment.
    pub full_tier_satisfaction: Option<usize>,
    /// How many tiers, skipping empty ones, which have been filled.
    /// 
    /// If full_tier_satisfaction is None, then this will be also.
    /// 
    /// If all tiers between 0 and full_tier_satisfaction are filled
    /// then this would be full_tier_satisfaction + 1
    /// 
    /// A rough measure of total satisfaction. Removes contentment and
    /// emphasizes material satisfaction.
    pub hard_satisfaction: Option<usize>,
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
    pub highest_tier: usize,
    /// The satisfaction of our desires in TieredValue format.
    /// This is set at the same time as other satisfactions
    pub tiered_satisfaction: TieredValue,
    /// A sanity check bool.
    /// 
    /// If true, then our property is (hypothetically) correctly sifted.
    /// Any added or removed property should correctly be added or removed 
    /// from satisfaction as well as general property.
    /// 
    /// If false, then our property is certainly not sifted. Property we have
    /// may not be correctly placed in satisfaction or our satisfaction
    /// may be counting on property we no longer have.
    /// 
    /// Adding or removing desires, always unsifts our property. We also have
    /// unsafe add or remove property, which adds or removes property but does
    /// not sift it into desires.
    pub is_sifted: bool,
}

impl Property {
    /// Creates a new desire collection based on a list of desires.
    pub fn new(desires: Vec<Desire>) -> Self {
        Property {
            desires,
            property: HashMap::new(),
            want_store: HashMap::new(),
            full_tier_satisfaction: None,
            hard_satisfaction: None,
            quantity_satisfied: 0.0,
            partial_satisfaction: 0.0,
            market_satisfaction: 0.0,
            highest_tier: 0,
            process_plan: HashMap::new(),
            product_expectations: HashMap::new(),
            want_expectations: HashMap::new(),
            is_disorganized: true,
            work_time: 0.0,
            wage_estimate: 0.0,
            extra_benefits: 0.0,
            todays_wage: 0.0,
            pay_period: 1,
            is_sifted: true,
            tiered_satisfaction: TieredValue { tier: 0, value: 0.0 },
        }
    }

    /// # Sift Specific Products
    /// 
    /// Goes over our property and sifts it into the products which have 
    /// specific desires.
    /// 
    /// # Assumptions
    /// 
    /// This, like other Sift Functions assume that, 1 all property is either 
    /// unreserved or reserved. and that the desire satisfaction is currently 
    /// empty.
    /// 
    /// Be sure that this data is cleared or reset before calling this, else
    /// it may act incorrectly.
    /// 
    /// # Notes
    /// 
    /// This function does not need external data as it matches for specific 
    /// products and does not consume anything.
    pub fn sift_specific_products(&mut self) {
        // TODO Remove? this
        todo!("Come back here later.")
    }

    /// Sifts a singular product into the various desires that seek it.
    /// 
    /// ## Plan
    /// 
    /// This will need to be expanded to allow for both specific product 
    /// satisfaction as well as general product satisfaction.
    pub fn sift_product(&mut self, product: &usize) {
        // TODO update or remove this.
        // get the first step.
        let mut curr = self
            .walk_up_tiers_for_item(&None, &Item::Product(*product));
        // get the available product
        let mut available = match self.property.get(product) {
            Some(val) => val.unreserved,
            None => 0.0
        };
        // loop over the desires for the product so long as we have one to add to.
        while let Some(coord) = curr {
            // get the desire we're adding to.
            let desire = self.desires
                .get_mut(coord.idx)
                .expect("Desire Not Found");
            // add to it at the current tier.
            available = desire.add_satisfaction_at_tier(available, coord.tier);
            // if none left, break out of the loop
            if available == 0.0 {
                break;
            }
            // since do have more available, get the next
            curr = self
            .walk_up_tiers_for_item(&curr, &Item::Product(*product));
        }
        // we have either run out of desires to possibly satisfy
    }

    /// # Unsafe Add Property
    /// 
    /// Adds (or removes) a items from our property unsafely. Removes from
    /// unreserved first, then reserved, then specific reserves. Adds to
    /// Unreserved.
    /// 
    /// This will break the sift on our poperty.
    /// 
    /// If subtracting more property than we have, this will clamp at 0.0.
    /// 
    /// This will not delete property data.
    pub fn unsafe_add_property(&mut self, product: usize, amount: f64) {
        if amount == 0.0 { return; }
        self.is_sifted = false;
        self.property.entry(product)
        .and_modify(|x| x.add_property(amount))
        .or_insert(PropertyInfo::new(amount));
    }

    /// # Predict Value Gained
    /// 
    /// Adds or removes an item to property, returning the value gained (or lost)
    /// by the addition (or subtraction).
    /// 
    /// If property is currently not sifted, it will just call unsafe_add_property
    /// 
    /// If property is sifted, it maintains the sifted status.
    /// 
    /// Returns the amount that would be gained by the addition.
    /// 
    /// TODO Not Tested: calls unsafe when not sifted, sifts correctly and returns the tieredvalue
    /// 
    /// TODO currently flawed as it cannot test releasing higher ranking wants to satisfy lower ranking wants
    pub fn predict_value_gained(&self, product: usize, amount: f64, data: &DataManager) -> TieredValue {
        if amount < 0.0 {
            self.predict_value_lost(product, amount, data);
        }
        let mut clone = self.cheap_clone();

        clone.add_property(product, amount, data)
    }

    /// # Predict Value Lost
    /// 
    /// Predicts the value lost by removing some amount of a product.
    /// 
    /// Returns the difference in value gained.
    /// 
    /// # Panics
    /// 
    /// Panics if the amount to remove is larger than the amount available.
    pub fn predict_value_lost(&self, product: usize, amount: f64, data: &DataManager) -> TieredValue {
        if amount < 0.0 {
            self.predict_value_gained(product, amount, data);
        }
        let mut clone = self.cheap_clone();

        clone.remove_property(product, amount, data)
    }

    /// # Predict Value Changed
    /// 
    /// A consolidation function for predicting the effects of multiple additions 
    /// and subtractions. It does all of them before returning the total value.
    /// 
    /// # Panics
    /// 
    /// Panics if it tries to subtract more of a product than is available.
    pub fn predict_value_changed(&self, alterations: &HashMap<usize, f64>, data: &DataManager) -> TieredValue {
        // create our clone first
        let mut clone = self.cheap_clone();
        for (&product, &amount) in alterations.iter() {
            clone.add_property(product, amount, data);
        }

        clone.sift_all(data) - self.total_estimated_value()
    }

    /// # Add Property
    /// 
    /// Adds or removes an item to property, returning the value gained (or lost)
    /// by the addition (or subtraction).
    /// 
    /// If property is currently not sifted, it will just call unsafe_add_property
    /// 
    /// If property is sifted, it maintains the sifted status.
    /// 
    /// ## Returns
    /// 
    /// It returns the value added to our existing estimated value. Be sure to 
    /// record the old if you want to compare.
    /// 
    /// # Panics
    /// 
    /// Panics if the amount given is greater than our property available.
    /// 
    /// 
    /// TODO Improve to literally not just sift again.
    pub fn add_property(&mut self, product: usize, amount: f64, data: &DataManager) -> TieredValue {
        if amount < 0.0 { // if negative value, add instead (negate value)
            return self.remove_property(product, -amount, data);
        }
        // get originial value
        let original_value = self.total_estimated_value();
        // don't bother checking for sifted.
        self.property.entry(product)
        .and_modify(|x| x.add_property(amount))
        .or_insert(PropertyInfo::new(amount));

        self.sift_all(data) - original_value
    }

    /// # Add Products
    /// 
    /// Adds or removes a list of products from our property then sifts and
    /// returns the change in value.
    /// 
    /// ## Panics
    /// 
    /// Panics if it tries to remove more of a product than we actually have.
    pub fn add_products(&mut self, products: &HashMap<usize, f64>, data: &DataManager) -> TieredValue {
        let original_value = self.total_estimated_value();
        for (&product, &amount) in products.iter() {
            if amount < 0.0 { // if it's being removed check that we can remove without going below 0.0
                let default = PropertyInfo::new(0.0);
                let available = self.property.get(&product).unwrap_or(&default);
                if -amount > available.total_property {
                    panic!("Not enough product within property.")
                }
            }
            self.property.entry(product)
            .and_modify(|x| x.add_property(amount))
            .or_insert(PropertyInfo::new(amount));
        }

        self.sift_all(data) - original_value
    }

    /// # Remove Properties
    /// 
    /// Removes a list of property from property, then sifts the product.
    /// 
    /// ## Panics
    /// 
    /// If it tries to remove more than what is actually available.
    pub fn remove_properties(&mut self, products: &HashMap<usize, f64>, data: &DataManager) -> TieredValue {
        let mut reverse = HashMap::new();
        for (&prod, val) in products.iter() {
            reverse.insert(prod, -val);
        }
        self.add_products(&reverse, data)
    }

    /// # Remove Property
    /// 
    /// Removes a number of product units from property, if needed, it also
    /// removes it from satisfaction and reserves. 
    /// 
    /// Returns the Value lost by this removal.
    /// 
    /// If amount given is negative, it instead redirects, calling add property.
    /// 
    /// If the amount being removed is greater than the amount available, it clamps the
    /// amount removed to what is available.
    /// 
    /// ## Returns
    /// 
    /// It returns the difference in value between 
    /// 
    /// # Panics
    /// 
    /// Panics if the amount given is greater than our property available.
    /// 
    /// TODO Improve this to not just literally resift property.
    pub fn remove_property(&mut self, product: usize, amount: f64, 
    data: &DataManager) -> TieredValue {
        if amount < 0.0 { // if negative value, add instead (negate value)
            return self.add_property(product, -amount, data);
        }
        // get old estimated value
        let original_value = self.total_estimated_value();
        // don't bother checking for sifted.
        let property = self.property.get_mut(&product);
        // check that we have the property, panic if we don't.
        let property = if property.is_none() {
            panic!("Product not in pop's property.")
        } else {
            property.unwrap()
        };
        if property.total_property < amount {
            panic!("Pop does not have enough of product to remove all.")
        }
        // remove the property up to the maximum available to 
        property.remove(amount);

        self.sift_all(data) - original_value
    }

    /// # Remove Satisfaction
    /// 
    /// Removes satisfaction dependent on a product, and the amount given.
    /// 
    /// Returns the total amount of the product successfully removed and 
    /// the effective value lost by it's removal.
    /// 
    /// ## Note
    /// 
    /// Assumes that unreserved and reserved products have alread been removed.
    /// 
    /// TODO Test This.
    pub fn remove_satisfaction(&mut self, product: usize, amount: f64, data: &DataManager) -> (f64, TieredValue) {
        let _amount_removed = 0.0;
        let mut value_removed = TieredValue { tier: 0, value: 0.0 };
        // get which kinds of satisfaction we need to remove from.
        if let Some(prop_data) = self.property.get_mut(&product) {
            // get how much we can remove in total.
            let available = prop_data.max_spec_reserve().min(amount);
            // get what everything should be at or below
            let target = prop_data.max_spec_reserve() - available;
            let mut specific_reduction = prop_data.product_reserve - target;
            let mut class_reduction = prop_data.class_reserve - target;
            let want_reduction = prop_data.want_reserve - target;
            // then, if value is positive, remove for each
            if specific_reduction > 0.0 {
                let mut specific_desire_coord = DesireCoord{ tier: self.highest_tier, idx: self.desires.len() };
                specific_desire_coord = self.walk_down_tiers_for_item(&specific_desire_coord, &Item::Product(product)).unwrap();
                while specific_reduction > 0.0 {
                    // walk down our desires, subtracting from satisfaction and our total reduction
                    let mut desire = self.desires
                    .get_mut(specific_desire_coord.idx).unwrap();
                    let reduce = desire.satisfaction_at_tier(specific_desire_coord.tier).min(specific_reduction);
                    desire.satisfaction -= reduce;
                    specific_reduction -= reduce;
                    // record the reduction 
                    value_removed.add_value(specific_desire_coord.tier, reduce);
                    // if no more reduction needed, gtfo
                    if specific_reduction == 0.0 { break; }
                    // get next desire, if no next, break out.
                    let temp = self
                    .walk_down_tiers_for_item(&specific_desire_coord, 
                        &Item::Product(product));
                    specific_desire_coord = if let Some(val) = temp {
                        val
                    } else { break; }
                }
            }
            if class_reduction > 0.0 {
                // if we get in here, there must be a class it applies to.
                let mut class_desire_coord = DesireCoord{ tier: self.highest_tier, idx: self.desires.len() };
                let class = data.products.get(&product).unwrap().product_class.unwrap();
                // get the class desires.
                class_desire_coord = self.walk_down_tiers_for_item(&class_desire_coord, &Item::Class(class)).unwrap();
                while class_reduction > 0.0 {
                    // walk down our desires, subtracting from satisfaction and our total reduction
                    let mut desire = self.desires
                    .get_mut(class_desire_coord.idx).unwrap();
                    let reduce = desire.amount.min(class_reduction);
                    desire.satisfaction -= reduce;
                    class_reduction -= reduce;
                    // record the reduction 
                    value_removed.add_value(class_desire_coord.tier, reduce);
                    // if no more reduction needed, gtfo
                    if class_reduction == 0.0 { break; }
                    // get next desire, if no next, break out.
                    let temp = self
                    .walk_down_tiers_for_item(&class_desire_coord, 
                        &Item::Class(class));
                    class_desire_coord = if let Some(val) = temp {
                        val
                    } else { break; }
                }
            }
            if want_reduction > 0.0 {
                // TODO improvement idea, rebuild to our new reduced target, rather than trying to reverse our process
                // 
                // for want, we need to go over our processes and check which use this product
                // go through our processes, get the process, but filter out those which don't 
                // take what we're removing.
                let mut valid_processes = HashSet::new();
                let mut invalid_desires = HashSet::new();
                for (process, _iterations) in self.process_plan.iter()
                .map(|(id, iterations)| {
                    (data.processes.get(id).expect("Process Not Found."), iterations)
                })
                .filter(|(process, _iterations)| {
                    process.uses_product(product, data)
                }) {
                    // if the process takes this product, record it.
                    valid_processes.insert(process.id);
                }
                // with the processes that use this item retrieved, walk down the tiers, focusing on wants
                let mut current_coord = DesireCoord{ tier: self.highest_tier, idx: self.desires.len() };
                current_coord = self.walk_down_tiers(&current_coord).unwrap();
                while want_reduction > 0.0 {
                    // if desire is already marked as invalid, skip.
                    if invalid_desires.contains(&current_coord.idx) {
                        current_coord = self.walk_down_tiers(&current_coord).unwrap();
                        continue;
                    }
                    // walk down the desires
                    let desire = self.desires.get_mut(current_coord.idx).unwrap();
                    // if the desire is not a want, skip.
                    if !desire.item.is_want() { 
                        invalid_desires.insert(current_coord.idx);
                        current_coord = self.walk_down_tiers(&current_coord).unwrap();
                        continue;
                    }
                    let want = data.wants.get(&desire.item.unwrap()).expect("Want not found.");
                    // check if any of it's process is ours.
                    let shared_processes = want.process_sources.intersection(&valid_processes)
                        .collect_vec(); // TODO see if there's a better way to check if the intersection is empty.
                    if shared_processes.is_empty() {
                        // if there is no overlapping match, move onto the next.
                        invalid_desires.insert(current_coord.idx);
                        current_coord = self.walk_down_tiers(&current_coord).unwrap();
                        continue;
                    }
                    // since this one has at least one of our processes, 
                    let mut result = HashMap::new();
                    for (&id, &prop) in self.property.iter() {
                        result.insert(id, prop.total_property);
                    }
                    for proc in shared_processes.iter()
                    .map(|x| data.processes.get(x).unwrap()) {
                        // get how many we need to undo this satisfaction
                        let output = proc.effective_output_of(Item::Want(desire.item.unwrap()));
                        let ratio = desire.amount / output;
                        // cap it at how many iterations we need at how many we originally planned.
                        let _change = proc.do_process(&result, 
                            &self.want_store, 0.0, 0.0, Some(ratio), true, data);
                        // remove those iterations (undo reservations, expectations, and the plan)
                        // if not enough to empty our satisfaction, go to the next process.
                    }
                }
            }
            self.update_satisfactions();
            return (available, TieredValue{ tier: 0, value: 0.0 });
        } else {
            return (0.0, TieredValue{ tier: 0, value: 0.0 });
        }
    }

    /// Helper function, creates a copy of our property as a hashset.
    /// 
    /// Uses total property, not some
    pub fn property_to_hashmap(&self) -> HashMap<usize, f64> {
        let mut result = HashMap::new();
        for (&id, &prop) in self.property.iter() {
            result.insert(id, prop.total_property);
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
        // unsift our property
        self.is_sifted = false;

        // if it doesn't already exist, duplicate and insert.
        let dup = desire.clone();
        self.desires.push(dup);
    }

    /// Finds the highest tier for a particular item which has satisfaction
    /// 
    /// Returns None if no satisfaction in any product found.
    pub fn get_highest_satisfied_tier_for_item(&self, item: Item) -> Option<usize> {
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
    pub fn get_highest_satisfied_tier(&self) -> Option<usize> {
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
    item: &Item) -> Option<DesireCoord> {
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
    pub fn get_lowest_unsatisfied_tier_of_item(&self, item: Item) -> Option<usize> {
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
    pub fn get_lowest_unsatisfied_tier(&self) -> Option<usize> {
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

    /// Tier Equivalence between two tiers. 
    /// 
    /// With the start tier equal to 1, end tiers above it decline by 0.9 per level
    /// difference. Going down they increase by 1/0.9
    /// 
    /// This defines how many units at the end tier is considered equivalent to lose in return
    /// for 1 unit of the start tier.
    /// 
    /// IE
    /// - start 10, end 11 = 0.9^1    1 start = 0.9 end
    /// - start 10, end 12 = 0.9^2    1 start = 0.81 end
    /// - start 10, end 8 = 0.9^-2      1 start = 1.23. end
    pub fn tier_equivalence(start: usize, end: usize) -> f64 {
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
    /// If given a coord who's idx is > self.desires.len(), then
    /// we still return the next valid step of the next tier.
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
            if prev.idx >= self.desires.len() { 
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
    pub fn walk_up_tiers_for_item(&self, prev: &Option<DesireCoord>, item: &Item) -> Option<DesireCoord> {
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
    pub fn satisfied_at_tier(&self, tier: usize) -> bool {
        self.desires.iter()
        .filter(|x| x.steps_on_tier(tier))
        .all(|x| x.satisfaction_at_tier(tier) == x.amount)
    }

    /// Get's the total satisfaction of all our desires at a specific tier.
    /// If nothing steps on it it return's 0.0.
    pub fn total_satisfaction_at_tier(&self, tier: usize) -> f64 {
        self.desires.iter().filter(|x| x.steps_on_tier(tier))
            .map(|x| x.satisfaction_at_tier(tier))
            .sum()
    }

    /// Gets the total desired items for all desires at a specific tier.
    /// If nothing steps on that tier it returns 0.0.
    pub fn total_desire_at_tier(&self, tier: usize) -> f64 {
        self.desires.iter().filter(|x| x.steps_on_tier(tier))
            .map(|x| x.amount)
            .sum()
    }

    /// Updates the satisfactions for our desires.
    /// 
    /// Does not calculate satisfaction base on market history.
    pub fn update_satisfactions(&mut self) {
        // start with full tier satisfaction and highest tier.
        self.full_tier_satisfaction = Some(usize::MAX);
        self.highest_tier = 0;
        self.tiered_satisfaction = TieredValue { tier: 0, value: 0.0 };
        // for each desire
        for desire in self.desires.iter() {
            let tier = desire.satisfaction_up_to_tier().unwrap_or(0);
            if !desire.is_fully_satisfied() && self.full_tier_satisfaction.is_some() {
                // if it's not fully satisfied, and our full_tier_sat is not currently None
                let tier = if desire.satisfaction_at_tier(tier) < 1.0 {
                    if tier == 0 { // if not fully satisfied, and tier is 0, return None
                        None
                    } else {
                        Some(tier - 1)
                    }
                } 
                else { Some(tier) };

                if let Some(tier) = tier {
                    // if we have a tier, select the smaller between the current and new.
                    self.full_tier_satisfaction = Some(self.full_tier_satisfaction.unwrap().min(tier));
                } else { // if no tier, set to none.
                    self.full_tier_satisfaction = None;
                }
            }
            // always check against the highest tier
            self.highest_tier = self.highest_tier.max(tier);
        }
        // if highest tier is still max, lower it to the highest end tier
        if self.full_tier_satisfaction.unwrap_or(0) == usize::MAX {
            for desire in self.desires.iter() {
                let end = if let Some(end) = desire.end {
                    end 
                } else { desire.start };
                if self.full_tier_satisfaction.unwrap() > end {
                    self.full_tier_satisfaction = Some(end);
                }
            }
        }
        // update tiered_satisfaction
        self.tiered_satisfaction = self.total_estimated_value();
        // get quantity satisfied
        self.quantity_satisfied = self.desires.iter()
        .map(|x| x.satisfaction).sum();

        // partial satisfaction
        self.partial_satisfaction = 0.0;

        for tier in self.full_tier_satisfaction.unwrap_or(0)..(self.highest_tier+1) {
            // go from the full_tier to the highest tier, summing as needed.
            let sat = self.total_satisfaction_at_tier(tier);
            let total = self.total_desire_at_tier(tier);
            if total > 0.0 {
                let sat_at_tier = sat *
                    Property::tier_equivalence(self.full_tier_satisfaction.unwrap_or(0), 
                        tier);
                self.partial_satisfaction += sat_at_tier;
            }
        }

        // finish with hard satisfaction
        let lowest = self.desires.iter()
            .map(|x| x.start).min().expect("Value not found!");
        let mut skipped = 0;
        for tier in lowest..self.full_tier_satisfaction.unwrap_or(0) {
            if self.desires.iter().filter(|x| x.steps_on_tier(tier)).count() == 0 {
                skipped += 1;
            }
        }
        // The highest full tier we satisfy, minus skipped tiers, +1 to correctly fence post it.
        if let Some(tier) = self.full_tier_satisfaction {
            self.hard_satisfaction = Some(tier - skipped + 1);
        } else {
            self.hard_satisfaction = None;
        }
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
                market.get_product_price(&product, 0.0) * 
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
            quantity.total_property * market.get_product_price(product, 0.0))
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
    pub fn total_satisfaction_of_item(&self, item: Item) -> f64 {
        self.desires.iter().filter(|x| x.item == item)
            .map(|x| x.satisfaction).sum()
    }

    /// # Sift All 
    /// 
    /// Used to sift our property and predict our uses for them.
    /// 
    /// Does not consume products, but should record satisfactions for each
    /// desire, the property use breakdown into each, and get our expected
    /// consumption and outputs.
    /// 
    /// This will allows us to more easily do barter trades and allow pops to 
    /// deal with exchanges personally going forward. Especially in
    /// trying to improve their situation by giving up either undesired or
    /// less immediately desired items.
    /// 
    /// ## Notes
    /// 
    /// Products reserved in want_reserve can be thought of the products that
    /// will be used or consumed by the process to satisfy wants.
    /// 
    /// Resets everything, so this will be pretty heavy on the cpu.
    /// 
    /// todo https://github.com/JeremyDaug/EconomicCalculator/issues/64
    pub fn sift_all(&mut self, data: &DataManager) -> TieredValue {
        // start by resetting property and satisfactions
        for (_, info) in self.property.iter_mut() {
            info.reset_reserves();
        }
        for desire in self.desires.iter_mut() {
            desire.satisfaction = 0.0;
        }
        self.process_plan.clear();
        self.product_expectations.clear();
        self.want_expectations.clear();
        // with data cleared, walk up our tiers and reserve items for our desires as needed.
        // we are allowed to satisfy our wants from the expectations, but not products expected.
        let mut cleared = HashSet::new();
        let mut current_opt = None;
        while let Some(current) = self.walk_up_tiers(current_opt) {
            current_opt = Some(current); // update for next step preemptively.
            if cleared.len() == self.desires.len() {
                break; // if cleared and desires are the same length, end.
            }
            if cleared.contains(&current.idx) {
                continue; // if in cleared, move on to the next.
            }
            let desire = self.desires.get_mut(current.idx).unwrap();
            match desire.item {
                Item::Want(want) => { // if want
                    // start by pulling out of the expected wants, to improve efficiency
                    // TODO this can be improved with some minor lookaheads. For example, a one process produces just X another produces both X and Y, check that we want Y, if we do, use the latter, else the former.
                    if self.want_expectations.contains_key(&want) {
                        let expectation = self.want_expectations.get_mut(&want).unwrap();
                        if *expectation > 0.0 { // if positive expectation, use
                            let shift = expectation
                            .min(desire.amount - desire.satisfaction_at_tier(current.tier));
                            *expectation -= shift;
                            desire.satisfaction += shift;
                        }
                    }
                    if desire.satisfied_at_tier(current.tier) {
                        if desire.past_end(current.tier + 1) {
                            cleared.insert(current.idx);
                        }
                        continue;
                    }
                    // get our want's info
                    let want_info = data.wants.get(&want).unwrap();
                    // start with ownership sources
                    for own_source_id in want_info.ownership_sources.iter() {
                        if !self.property.contains_key(own_source_id) {
                            continue; // if don't have it, skip.
                        }
                        // get our property info
                        let prop_info = self.property.get_mut(own_source_id).unwrap();
                        let available_product = prop_info.available_for_want();
                        if available_product == 0.0 {
                            continue; // if none available for shift, skip.
                        }
                        // get the product's data
                        let product_info = data.products.get(own_source_id).unwrap();
                        // how much satisfaction we have left
                        let remaining_sat = desire.amount - desire.satisfaction_at_tier(current.tier);
                        let eff = product_info.wants.get(&want).unwrap(); // how efficient the product is at satisfying this want.
                        let target = remaining_sat / eff; // how many of our product we need to satisfy
                        let target = target.min(available_product); // how many we can actually get
                        for (own_want, eff) in product_info.wants.iter() {
                            if *own_want == want { // if our want, add to sat
                                desire.satisfaction += eff * target;
                            } else { // if not, add to expectations
                                self.want_expectations.entry(*own_want)
                                .and_modify(|x| *x += eff * target)
                                .or_insert(eff * target);
                            }
                        }
                        prop_info.shift_to_want_reserve(target); // shift property to want.
                        if desire.satisfied_at_tier(current.tier) { // if satisfied, break the loop
                            break;
                        }
                    }
                    // check for completion
                    if desire.satisfied_at_tier(current.tier) {
                        if desire.past_end(current.tier + 1) {
                            cleared.insert(current.idx);
                        }
                        continue;
                    }
                    // if uncompleted, go to use processes.
                    for proc_id in want_info.use_sources.iter() {
                        let process = data.processes.get(proc_id).unwrap();
                        // get how much the process outputs
                        let eff = process.effective_output_of(Item::Want(want));
                        // how many iterations we need to reach the target.
                        let target_iter = (desire.amount - desire.satisfaction_at_tier(current.tier)) / eff;
                        let mut combined_wants = self.want_store.clone();
                        for (want_id, amount) in self.want_expectations.iter() {
                            combined_wants.entry(*want_id)
                            .and_modify(|x| *x += amount)
                            .or_insert(*amount);
                        }
                        let outputs = process.do_process_with_property(&self.property, 
                            &combined_wants, 
                            0.0, 0.0, Some(target_iter), true, data);
                        if outputs.iterations == 0.0 {
                            continue; // if no iterations possible, skip
                        }
                        // we do some iterations, so update stuff
                        self.process_plan.entry(*proc_id)
                            .and_modify(|x| *x += outputs.iterations)
                            .or_insert(outputs.iterations);
                        for (&product, &quant) in outputs.input_output_products.iter() {
                            if quant < 0.0 { // if negative, shift
                                self.property.get_mut(&product).unwrap().shift_to_want_reserve(quant);
                            }
                            self.product_expectations.entry(product)
                            .and_modify(|x| *x += quant)
                            .or_insert(quant);
                        }
                        for (&product, &quant) in outputs.capital_products.iter() {
                            // if capital, just shift to want reserve
                            self.property.get_mut(&product).unwrap()
                            .shift_to_want_reserve(-quant);
                        }
                        for (&edited_want, &quant) in outputs.input_output_wants.iter() {
                            if edited_want == want { // if the want is what we're trying to satisy, add it
                                desire.satisfaction += quant;
                            } else {
                                self.want_expectations.entry(want)
                                .and_modify(|x| *x += quant)
                                .or_insert(quant);
                            }
                        }
                        if desire.satisfied_at_tier(current.tier) {
                            break; // if satified, break out
                        }
                    }
                    // we got out check for completion
                    if desire.satisfied_at_tier(current.tier) {
                        if desire.past_end(current.tier + 1) {
                            cleared.insert(current.idx);
                        }
                        continue;
                    }
                    // if we get here, then try consumption processes
                    for proc_id in want_info.consumption_sources.iter() {
                        let process = data.processes.get(proc_id).unwrap();
                        // get how much the process outputs
                        let eff = process.effective_output_of(Item::Want(want));
                        // how many iterations we need to reach the target.
                        let target_iter = (desire.amount - desire.satisfaction_at_tier(current.tier)) / eff;
                        let mut combined_wants = self.want_store.clone();
                        for (want_id, amount) in self.want_expectations.iter() {
                            combined_wants.entry(*want_id)
                            .and_modify(|x| *x += amount)
                            .or_insert(*amount);
                        }
                        let outputs = process.do_process_with_property(&self.property, 
                            &combined_wants, 
                            0.0, 0.0, Some(target_iter), true, data);
                        if outputs.iterations == 0.0 {
                            continue; // if no iterations possible, skip
                        }
                        // we do some iterations, so update stuff
                        self.process_plan.entry(*proc_id)
                            .and_modify(|x| *x += outputs.iterations)
                            .or_insert(outputs.iterations);
                        for (&product, &quant) in outputs.input_output_products.iter() {
                            if quant < 0.0 { // if negative, shift
                                self.property.get_mut(&product).unwrap().shift_to_want_reserve(quant);
                            }
                            self.product_expectations.entry(product)
                            .and_modify(|x| *x += quant)
                            .or_insert(quant);
                        }
                        for (&product, &quant) in outputs.capital_products.iter() {
                            // if capital, just shift to want reserve
                            self.property.get_mut(&product).unwrap()
                            .shift_to_want_reserve(-quant);
                        }
                        for (&edited_want, &quant) in outputs.input_output_wants.iter() {
                            if edited_want == want { // if the want is what we're trying to satisy, add it
                                desire.satisfaction += quant;
                            } else {
                                self.want_expectations.entry(want)
                                .and_modify(|x| *x += quant)
                                .or_insert(quant);
                            }
                        }
                        if desire.satisfied_at_tier(current.tier) {
                            break; // if satified, break out
                        }
                    }
                    // we've done what we can
                    if !desire.satisfied_at_tier(current.tier) || // if unable to be fully satisfied
                    desire.past_end(current.tier + 1) { // or there is no next tier
                        cleared.insert(current.idx); // add to cleared.
                    }
                },
                Item::Class(class) => { // if class item
                    // get that class's products
                    let class = data.product_classes.get(&class).unwrap();
                    // if there is no overlap between our property and add to cleared
                    if !class.iter().any(|x| self.property.contains_key(x)) {
                        cleared.insert(current.idx);
                        continue;
                    }
                    // since there is some overlap, try to shift that
                    let mut shifted = 0.0;
                    for product_id in class.iter() { 
                        // try each product we have
                        let info_opt = self.property.get_mut(product_id);
                        if info_opt.is_none() {
                            continue; // if we don't have the product, go to next
                        }
                        let info = info_opt.unwrap();
                        let available_shift = info.available_for_class();
                        if available_shift == 0.0 {
                            continue; // if nothing available to shift, go to next
                        }
                        // since we have something to get, get what we can to attempt to shift
                        let shift = available_shift
                            .min(desire.amount - desire.satisfaction_at_tier(current.tier));
                        // with our shift amount, do the shift
                        info.shift_to_class_reserve(shift);
                        desire.satisfaction += shift;
                        shifted += shift;
                        if desire.satisfied_at_tier(current.tier) {
                            break; // if we satisfied this tier, break out.
                        }
                    }
                    // check if we succeeded or not
                    if shifted == 0.0 || // if shifted nothing
                    !desire.satisfied_at_tier(current.tier) || // or unable to fully satisfy
                    desire.past_end(current.tier + 1) {
                        cleared.insert(current.idx); // add to cleared and gtfo
                    }
                },
                Item::Product(product) => { // if specific item
                    // get our info for this product
                    let info_opt = self.property.get_mut(&product);
                    if info_opt.is_none() { // if we have none of this item, set this as cleared.
                        cleared.insert(current.idx);
                        continue;
                    }
                    let info = info_opt.unwrap();
                    // how much we can shift vs how much we want to shift
                    let shift = info.available_for_specific()
                        .min(desire.amount - desire.satisfaction_at_tier(current.tier));
                    if shift == 0.0 { // if nothing to shift, add this to cleared and gtfo
                        cleared.insert(current.idx);
                        continue;
                    }
                    // if any shift, shift to reserved and add to satisfaction
                    info.shift_to_specific_reserve(shift); // reserve from property
                    desire.satisfaction += shift; // add to satisfaction
                    // wrap up with completion checks
                    if desire.past_end(current.tier + 1) || // if past end
                    !desire.satisfied_at_tier(current.tier) { // or unable to satisfy, clear
                        cleared.insert(current.idx);
                    }
                }
            }
        }
        self.update_satisfactions();
        self.is_sifted = true;
        self.tiered_satisfaction
    }

    /// # Next Stepped on Tier
    /// 
    /// Gets the next tier that any of our desires steps on.
    /// 
    /// If no desire steps on a tier above the given value, then it returns 
    /// none.
    /// TODO Not Tested.
    pub fn next_stepped_on_tier(&self, end: usize) -> Option<usize> {
        // ensure that we can reach any from end (ie any end after)
        let mut next = usize::MAX;
        for des in self.desires.iter() {
            let possible = des.get_next_tier_up(end);
            if let Some(val) = possible {
                if val < next {
                    next = val;
                }
            }
        }
        if next == usize::MAX {
            None
        } else {
            Some(next)
        }
    }

    /// # Total Estimated Value
    /// 
    /// This function takes the our existing satisfactions and caluclates the effective
    /// total value of this satisfaction.
    /// 
    /// For balance purposes, the value returned is set at our full tier satisfaction.
    pub fn total_estimated_value(&self) -> TieredValue {
        if !self.is_sifted {
            return TieredValue { tier: 0, value: 0.0 };
        }
        let mut tier = 0;
        let mut result = TieredValue { tier: 0, value: 0.0};
        while tier <= self.highest_tier {
            let val = self.desires.iter()
            .filter(|x| x.steps_on_tier(tier))
            .map(|x| x.satisfaction_at_tier(tier))
            .sum();
            result.add_value(tier, val);
            tier += 1;
        }

        if let Some(tier) = self.full_tier_satisfaction {
            result.shift_tier(tier)
        } else {
            result
        }
    }

    /// # Print Satisfactions
    /// 
    /// A Helper function which creates a visual output of our satisfactions.
    /// 
    /// Lower is the starting tier, if None, 
    pub fn print_satisfactions(&self, lower: Option<usize>, upper: Option<usize>) -> String {
        let start = if let Some(start) = lower {
            start
        } else { 0 };
        let end = if let Some(end) = upper {
            end
        } else { self.highest_tier };
        let mut result = String::new();

        let mut i = end;
        while i >= start {
            if !self.desires.iter().all(|x| x.steps_on_tier(i)) {
                if i == 0 {
                    break;
                }
                i -= 1;
                continue;
            }
            result += format!("{:03} ", i).as_str();
            for desire in self.desires.iter() {
                if desire.steps_on_tier(i) {
                    result += "| ";
                    if desire.satisfied_at_tier(i) {
                        result += " 1  ";
                    } else if desire.satisfaction_at_tier(i) > 1.0 {
                        result += " X  ";
                    } else {
                        result += "    "
                    }
                } else {
                    result += "|    |"
                }
            }
            result += "\n";
            if i == 0 { break; }
            i -= 1;
        }
        result += str::repeat("-", (1 + self.desires.len()) * 4).as_str();
        result += "\n";
        result += "    |";

        for idx in 0..self.desires.len() {
            let desire = self.desires.get(idx).unwrap();
            match desire.item {
                Item::Want(id) => {
                    result += format!("W{:04}|", id).as_str();
                },
                Item::Class(id) => {
                    result += format!("C{:04}|", id).as_str();
                },
                Item::Product(id) => {
                    result += format!("P{:04}|", id).as_str();
                },
            }
        }

        result
    }

    /// # Cheap Clone
    /// 
    /// Creates a cheap clone of ourself, copying over property and desires.
    /// 
    /// This is for 
    pub fn cheap_clone(&self) -> Self {
        // copy desires
        let mut result = Property::new(self.desires.clone());
        // copy property
        for (&id, info) in self.property.iter() {
            result.property.insert(id, PropertyInfo::new(info.total_property));
        }
        // copy wants
        for (&id, &info) in self.want_store.iter() {
            result.want_store.insert(id, info);
        }
        // copy satisfactions
        result.full_tier_satisfaction = self.full_tier_satisfaction;
        result.hard_satisfaction = self.hard_satisfaction;
        result.quantity_satisfied = self.quantity_satisfied;
        result.partial_satisfaction = self.partial_satisfaction;
        result.market_satisfaction = self.market_satisfaction;
        result.highest_tier = self.highest_tier;
        result.is_sifted = true;

        result
    }

    /// # Decay Goods
    /// 
    /// Goes through the property contained and goes through decay and failure
    /// effects for each want and good stored.
    pub fn decay_goods(&mut self, data: &DataManager) {
        // start by decaying wants 
        for (want, quant) in self.want_store.iter_mut() {
            let want_info = data.wants.get(want).unwrap();
            let decay = 1.0 - want_info.decay;
            *quant *= decay; // multiply by decay and assign again.
        }
        // get a copy of our existing property for processing
        let original_property = self.property_to_hashmap();
        let mut property_change = HashMap::new();
        let mut want_change = HashMap::new();
        // then decay/fail products
        for (product, info) in self.property.iter_mut() {
            let prod_info = data.products.get(product).unwrap();
            if prod_info.mean_time_to_failure.is_some() {
                // TODO add in random chance roll here.
                let failed = prod_info.failure_chance() * info.total_property;
                // if it has a failure process, use that
                if let Some(proc_id) = prod_info.failure_process {
                    let fail_proc = data.processes.get(&proc_id).unwrap();
                    let results = fail_proc
                    .do_process(&original_property, 
                        &self.want_store, 0.0, 
                        0.0, Some(failed), 
                        true, data);
                    for (&product, &amount) in results.input_output_products.iter() {
                        // add to current property.
                        property_change.entry(product)
                        .and_modify(|x| *x += amount)
                        .or_insert(amount);
                    }
                    for (&want, &amount) in results.input_output_wants.iter() {
                        want_change.entry(want)
                        .and_modify(|x| *x += amount)
                        .or_insert(amount);
                    }
                } else { // if no process, just shift failed products to lost.
                    property_change.entry(*product)
                    .and_modify(|x| *x -= failed)
                    .or_insert(-failed);
                }
            }
        }
        // wrap up by adding/removing what was changed
        for (&product, &amount) in property_change.iter() {
            if amount < 0.0 { // if being removed
                self.property.entry(product)
                .and_modify(|x| {
                    x.remove(-amount);
                    x.lost -= amount;
                });
            } else { // if being added
                self.property.entry(product)
                .and_modify(|x| x.add_property(amount))
                .or_insert(PropertyInfo::new(amount));
            }
        }
        for (&want, &amount) in want_change.iter() {
            self.want_store.entry(want)
            .and_modify(|x| *x += amount)
            .or_insert(amount);
        }
    }

    /// # Satisfaction from AMV
    /// 
    /// Gives a very rough estimate of how much Satisfaction AMV will allow
    /// us to get from the market. It acts by spending the AMV on known prices for
    /// goods to satisfy.
    /// 
    /// Currently, it only works for specific desires, not wants or classes.
    /// 
    /// TODO consider improving this to instead 'virtually' buy then add and sift to get the satisfaction.
    pub fn satisfaction_from_amv(&self, amv: f64, market: &MarketHistory) -> TieredValue {
        let mut result = TieredValue { tier: 0, value: 0.0 };
        let mut remaining_amv = amv;

        // walk up the desires
        let mut current_opt = None;
        while let Some(current) = self.walk_up_tiers(current_opt) {
            // if setup the current opt for the next
            current_opt = Some(current);
            let desire = self.desires.get(current.idx).unwrap();
            let tier = current.tier;
            if desire.satisfied_at_tier(tier) {
                continue; // if already satisfied, skip.
            }
            // get how much satisfaction is left to satisfy here.
            // placeholder catch for wants and classes as we do not have access to their estimate prices
            // get the price per unit.
            let unit_price = match desire.item {
                Item::Product(id) => market
                    .get_product_price(&id, 1.0),
                Item::Class(id) => market
                    .get_class_price(id, 1.0),
                Item::Want(id) => market
                    .get_class_price(id, 1.0)
            };
            // get how much we need to satisfy.
            let units_left = desire.amount - desire.satisfaction_at_tier(tier);
            // get and cap the cost at remaining AMV
            let cost = (units_left * unit_price).min(remaining_amv);
            let units_gained = cost / unit_price;
            // 'purchase' the item and add the satisfaction to our result
            remaining_amv -= cost;
            result += TieredValue { tier, value: units_gained};
            if remaining_amv == 0.0 {
                break; // if no remaining amv, gtfo.
            }
        }
        // nothing left to do, return our result.
        result
    }

    /// # Release Desire At
    /// 
    /// Releases a desire at particular coordinate.
    /// 
    /// Currently
    /// 
    /// Returns the products which are no longer needed.
    /// 
    /// ## Warning
    /// 
    /// If you are using this, it will completely remove the products that are
    /// returned from this.
    /// 
    /// TODO currently lazy, uses self.sift_up_to() to get our correction
    /// TODO improve this to actually be targeted rather than imprecise.
    pub fn release_desire_at(&mut self, coord: &DesireCoord, 
    _market: &MarketHistory, 
    data: &DataManager) 
    -> HashMap<usize, f64> {
        let mut result = HashMap::new();

        if self.desires.len() <=  coord.idx { // if not a valid desire, return nothing
            return result;
        } 
        let desire = self.desires.get(coord.idx).unwrap();
        if desire.satisfaction_at_tier(coord.tier) == 0.0 { // if no satisfaction to remove, break
            return result;
        }
        self.sift_up_to(coord, data);
        // collect released desires and remove them from property.

        for (&id, info) in self.property.iter_mut() {
            if info.unreserved > 0.0 {
                let shift = info.unreserved;
                info.safe_remove(shift);
                result.entry(id)
                .and_modify(|x| *x += shift)
                .or_insert(shift);
            }
        }

        result
    }

    /// # Sift up to
    /// 
    /// Sifts our property innto our desires up to the selected desire coord.
    /// 
    /// It does not include the coord selected.
    /// 
    /// If the coord is Invalid it will complete the tier
    /// 
    /// In all other ways, it acts like self.silt_all()
    /// todo https://github.com/JeremyDaug/EconomicCalculator/issues/64
    pub fn sift_up_to(&mut self, coord: &DesireCoord, 
    data: &DataManager) -> TieredValue {
        // start by resetting property and satisfactions
        for (_, info) in self.property.iter_mut() {
            info.reset_reserves();
        }
        for desire in self.desires.iter_mut() {
            desire.satisfaction = 0.0;
        }
        self.process_plan.clear();
        self.product_expectations.clear();
        self.want_expectations.clear();
        // with data cleared, walk up our tiers and reserve items for our desires as needed.
        // we are allowed to satisfy our wants from the expectations, but not products expected.
        let mut cleared = HashSet::new();
        let mut current_opt = None;
        while let Some(current) = self.walk_up_tiers(current_opt) {
            current_opt = Some(current); // update for next step preemptively.
            if cleared.len() == self.desires.len() {
                break; // if cleared and desires are the same length, end.
            }
            if cleared.contains(&current.idx) {
                continue; // if in cleared, move on to the next.
            }
            if current.tier > coord.tier {
                // if after our coordinate tier, exit out as we passed up the coord
                break;
            }
            if current.tier == coord.tier && coord.idx == current.idx {
                // if our selected end coordinate, GTFO.
                break;
            }
            let mut desire = self.desires.get_mut(current.idx).unwrap();
            match desire.item {
                Item::Want(want) => { // if want
                    // start by pulling out of the expected wants, to improve efficiency
                    // TODO this can be improved with some minor lookaheads. For example, a one process produces just X another produces both X and Y, check that we want Y, if we do, use the latter, else the former.
                    if self.want_expectations.contains_key(&want) {
                        let expectation = self.want_expectations.get_mut(&want).unwrap();
                        if *expectation > 0.0 { // if positive expectation, use
                            let shift = expectation
                            .min(desire.amount - desire.satisfaction_at_tier(current.tier));
                            *expectation -= shift;
                            desire.satisfaction += shift;
                        }
                    }
                    if desire.satisfied_at_tier(current.tier) {
                        if desire.past_end(current.tier + 1) {
                            cleared.insert(current.idx);
                        }
                        continue;
                    }
                    // get our want's info
                    let want_info = data.wants.get(&want).unwrap();
                    // start with ownership sources
                    for own_source_id in want_info.ownership_sources.iter().sorted() {
                        if !self.property.contains_key(own_source_id) {
                            continue; // if don't have it, skip.
                        }
                        // get our property info
                        let prop_info = self.property.get_mut(own_source_id).unwrap();
                        let available_product = prop_info.available_for_want();
                        if available_product == 0.0 {
                            continue; // if none available for shift, skip.
                        }
                        // get the product's data
                        let product_info = data.products.get(own_source_id).unwrap();
                        // how much satisfaction we have left
                        let remaining_sat = desire.amount - desire.satisfaction_at_tier(current.tier);
                        let eff = product_info.wants.get(&want).unwrap(); // how efficient the product is at satisfying this want.
                        let target = remaining_sat / eff; // how many of our product we need to satisfy
                        let target = target.min(available_product); // how many we can actually get
                        for (own_want, eff) in product_info.wants.iter() {
                            if *own_want == want { // if our want, add to sat
                                desire.satisfaction += eff * target;
                            } else { // if not, add to expectations
                                self.want_expectations.entry(*own_want)
                                .and_modify(|x| *x += eff * target)
                                .or_insert(eff * target);
                            }
                        }
                        prop_info.shift_to_want_reserve(target); // shift property to want.
                        if desire.satisfied_at_tier(current.tier) { // if satisfied, break the loop
                            break;
                        }
                    }
                    // check for completion
                    if desire.satisfied_at_tier(current.tier) {
                        if desire.past_end(current.tier + 1) {
                            cleared.insert(current.idx);
                        }
                        continue;
                    }
                    // if uncompleted, go to use processes.
                    for proc_id in want_info.use_sources.iter().sorted() {
                        let process = data.processes.get(proc_id).unwrap();
                        // get how much the process outputs
                        let eff = process.effective_output_of(Item::Want(want));
                        // how many iterations we need to reach the target.
                        let target_iter = (desire.amount - desire.satisfaction_at_tier(current.tier)) / eff;
                        let mut combined_wants = self.want_store.clone();
                        for (want_id, amount) in self.want_expectations.iter() {
                            combined_wants.entry(*want_id)
                            .and_modify(|x| *x += amount)
                            .or_insert(*amount);
                        }
                        let outputs = process.do_process_with_property(&self.property, 
                            &combined_wants, 
                            0.0, 0.0, Some(target_iter), true, data);
                        if outputs.iterations == 0.0 {
                            continue; // if no iterations possible, skip
                        }
                        // we do some iterations, so update stuff
                        self.process_plan.entry(*proc_id)
                            .and_modify(|x| *x += outputs.iterations)
                            .or_insert(outputs.iterations);
                        for (&product, &quant) in outputs.input_output_products.iter() {
                            if quant < 0.0 { // if negative, shift
                                self.property.get_mut(&product).unwrap().shift_to_want_reserve(quant);
                            }
                            self.product_expectations.entry(product)
                            .and_modify(|x| *x += quant)
                            .or_insert(quant);
                        }
                        for (&product, &quant) in outputs.capital_products.iter() {
                            // if capital, just shift to want reserve
                            self.property.get_mut(&product).unwrap()
                            .shift_to_want_reserve(-quant);
                        }
                        for (&edited_want, &quant) in outputs.input_output_wants.iter() {
                            if edited_want == want { // if the want is what we're trying to satisy, add it
                                desire.satisfaction += quant;
                            } else {
                                self.want_expectations.entry(want)
                                .and_modify(|x| *x += quant)
                                .or_insert(quant);
                            }
                        }
                        if desire.satisfied_at_tier(current.tier) {
                            break; // if satified, break out
                        }
                    }
                    // we got out check for completion
                    if desire.satisfied_at_tier(current.tier) {
                        if desire.past_end(current.tier + 1) {
                            cleared.insert(current.idx);
                        }
                        continue;
                    }
                    // if we get here, then try consumption processes
                    for proc_id in want_info.consumption_sources.iter().sorted() {
                        let process = data.processes.get(proc_id).unwrap();
                        // get how much the process outputs
                        let eff = process.effective_output_of(Item::Want(want));
                        // how many iterations we need to reach the target.
                        let target_iter = (desire.amount - desire.satisfaction_at_tier(current.tier)) / eff;
                        let mut combined_wants = self.want_store.clone();
                        for (want_id, amount) in self.want_expectations.iter() {
                            combined_wants.entry(*want_id)
                            .and_modify(|x| *x += amount)
                            .or_insert(*amount);
                        }
                        let outputs = process.do_process_with_property(&self.property, 
                            &combined_wants, 
                            0.0, 0.0, Some(target_iter), true, data);
                        if outputs.iterations == 0.0 {
                            continue; // if no iterations possible, skip
                        }
                        // we do some iterations, so update stuff
                        self.process_plan.entry(*proc_id)
                            .and_modify(|x| *x += outputs.iterations)
                            .or_insert(outputs.iterations);
                        for (&product, &quant) in outputs.input_output_products.iter() {
                            if quant < 0.0 { // if negative, shift
                                self.property.get_mut(&product).unwrap().shift_to_want_reserve(quant);
                            }
                            self.product_expectations.entry(product)
                            .and_modify(|x| *x += quant)
                            .or_insert(quant);
                        }
                        for (&product, &quant) in outputs.capital_products.iter() {
                            // if capital, just shift to want reserve
                            self.property.get_mut(&product).unwrap()
                            .shift_to_want_reserve(-quant);
                        }
                        for (&edited_want, &quant) in outputs.input_output_wants.iter() {
                            if edited_want == want { // if the want is what we're trying to satisy, add it
                                desire.satisfaction += quant;
                            } else {
                                self.want_expectations.entry(want)
                                .and_modify(|x| *x += quant)
                                .or_insert(quant);
                            }
                        }
                        if desire.satisfied_at_tier(current.tier) {
                            break; // if satified, break out
                        }
                    }
                    // we've done what we can
                    if !desire.satisfied_at_tier(current.tier) || // if unable to be fully satisfied
                    desire.past_end(current.tier + 1) { // or there is no next tier
                        cleared.insert(current.idx); // add to cleared.
                    }
                },
                Item::Class(class) => { // if class item
                    // get that class's products
                    let class = data.product_classes.get(&class).unwrap();
                    // if there is no overlap between our property and add to cleared
                    if !class.iter().any(|x| self.property.contains_key(x)) {
                        cleared.insert(current.idx);
                        continue;
                    }
                    // since there is some overlap, try to shift that
                    let mut shifted = 0.0;
                    for product_id in class.iter().sorted() { 
                        // try each product we have
                        let info_opt = self.property.get_mut(product_id);
                        if info_opt.is_none() {
                            continue; // if we don't have the product, go to next
                        }
                        let info = info_opt.unwrap();
                        let available_shift = info.available_for_class();
                        if available_shift == 0.0 {
                            continue; // if nothing available to shift, go to next
                        }
                        // since we have something to get, get what we can to attempt to shift
                        let shift = available_shift
                            .min(desire.amount - desire.satisfaction_at_tier(current.tier));
                        // with our shift amount, do the shift
                        info.shift_to_class_reserve(shift);
                        desire.satisfaction += shift;
                        shifted += shift;
                        if desire.satisfied_at_tier(current.tier) {
                            break; // if we satisfied this tier, break out.
                        }
                    }
                    // check if we succeeded or not
                    if shifted == 0.0 || // if shifted nothing
                    !desire.satisfied_at_tier(current.tier) || // or unable to fully satisfy
                    desire.past_end(current.tier + 1) { // or no next tier
                        cleared.insert(current.idx); // add to cleared and gtfo
                    }
                },
                Item::Product(product) => { // if specific item
                    // get our info for this product
                    let info_opt = self.property.get_mut(&product);
                    if info_opt.is_none() { // if we have none of this item, set this as cleared.
                        cleared.insert(current.idx);
                        continue;
                    }
                    let info = info_opt.unwrap();
                    // how much we can shift vs how much we want to shift
                    let shift = info.available_for_specific()
                        .min(desire.amount - desire.satisfaction_at_tier(current.tier));
                    if shift == 0.0 { // if nothing to shift, add this to cleared and gtfo
                        cleared.insert(current.idx);
                        continue;
                    }
                    // if any shift, shift to reserved and add to satisfaction
                    info.shift_to_specific_reserve(shift); // reserve from property
                    desire.satisfaction += shift; // add to satisfaction
                    // wrap up with completion checks
                    if desire.past_end(current.tier + 1) || // if past end
                    !desire.satisfied_at_tier(current.tier) { // or unable to satisfy, clear
                        cleared.insert(current.idx);
                    }
                }
            }
        }
        self.update_satisfactions();
        self.is_sifted = true;
        self.total_estimated_value()
    }

    /// # Available Shopping Time
    /// 
    /// Using only unreserved products, this get's how much shopping time we can get.
    /// 
    /// This does not consume anything, merely checks how much we can get.
    /// 
    /// # Note
    /// 
    /// Currently unused as we've chosen a less efficient, but simpler method of dealing with available shopping time.
    pub fn _available_shopping_time(&self, data: &DataManager, _skill_level: f64, _skill: usize) -> f64 {
        // first extract our available resources
        let mut available_products = HashMap::new();
        for (&id, &info) in self.property.iter() {
            available_products.insert(id, info.available());
        }
        // then extract wants which might feed into it.
        let mut available_wants = HashMap::new();
        for (&id, &avail) in self.want_store.iter() {
            available_wants.insert(id, avail);
        }
        for (&id, &change) in self.want_expectations.iter()
        .filter(|x| *x.1 < 0.0) { // remove those we expect to use.
            available_wants.entry(id)
            .and_modify(|x| *x += change)
            .or_insert(change);
        }
        // then start counting up how much shopping time we might be able to get.
        let mut max_available = 0.0;
        for process in data.products.get(&SHOPPING_TIME_ID).unwrap() // The product
        .processes.iter() // the process IDs which time is related to
        .map(|x| data.processes.get(x).unwrap()) // the process info
        .filter(|x| x.outputs_product(SHOPPING_TIME_ID)) { // the processes which output it.
            // todo, pass in pop_skill info later. Ignore for now
            let result = process.do_process(&available_products, 
                &available_wants, 0.0, 
                0.0, None, false, data);
            // check result availability
            if result.iterations > 0.0 {
                // if any iterations done, see how much shopping time we got.
                let output = result.input_output_products.get(&SHOPPING_TIME_ID).unwrap();
                // add to our output, then remove any change from our available products and wants
                max_available += output;
                for (&id, &change) in result.input_output_products.iter() {
                    available_products.entry(id)
                    .and_modify(|x| *x += change)
                    .or_insert(change);
                }
                for (&id, &change) in result.capital_products.iter() {
                    available_products.entry(id)
                    .and_modify(|x| *x -= change);
                }
                for (&id, &change) in result.input_output_wants.iter() {
                    available_wants.entry(id)
                    .and_modify(|x| *x += change)
                    .or_insert(change);
                }
            }
        }

        max_available
    }

    /// # Get Shopping Time
    /// 
    /// As available_shopping_time(), but instead of returning a total estimate it tries to get
    /// enough shopping time to meet the given target.
    /// 
    /// It prioritizes excess Shopping time in our property first, then goes 
    /// through the various ways to make it in ID order.
    /// 
    /// Shopping Time taken from storage and/or products consumed will be removed.
    /// 
    /// Items used as capital will be shifted into capital.
    /// 
    /// If it is unable to reach the target, it returns what it's able to without 
    /// touching existing satisfaction.
    /// 
    /// ## Note
    /// 
    /// Processes are selected in ID order.
    /// 
    /// TODO this can likely be repurposed into a more general "get X units of y product function"
    /// TODO Improve to prioritize by market cost of the process. Eventually.
    /// TODO Improve this to only take from satisfaction above a certain point, not more.
    /// TODO Sift Improvement: When Sift has been improved to function non-destructively, this can be upgraded to take from satisfaction as well.
    pub fn get_shopping_time(&mut self, target: f64, data: &DataManager, 
    _market: &MarketHistory, skill_level: f64, skill: usize, _cutoff: Option<DesireCoord>) -> f64 {
        // get the final output ready.
        let mut final_result = 0.0;
        // first extract from storage any Shopping Time we have available and waiting to use.
        if let Some(info) = self.property.get_mut(&SHOPPING_TIME_ID) {
            let shift = info.available().min(target);
            final_result += info.available().min(target); // add existing to final
            info.remove(shift); // remove from info
            info.spent += shift; // add to spent as well while we're at it.
        }
        if final_result == target { // if at our target, gtfo.
            return final_result;
        }
        // If here, we need to try and get more from processing things.
        // extract wants which might feed into it.
        let mut available_wants = HashMap::new();
        for (&id, &avail) in self.want_store.iter() {
            available_wants.insert(id, avail);
        }
        for (&id, &change) in self.want_expectations.iter()
        .filter(|x| *x.1 < 0.0) { // subtract those which we expect to use elsewhere.
            available_wants.entry(id)
            .and_modify(|x| *x += change)
            .or_insert(change);
        }
        for process in data.products.get(&SHOPPING_TIME_ID).unwrap() // The product
        .processes.iter() // the process IDs which time is related to
        .map(|x| data.processes.get(x).unwrap()) // the process info
        .filter(|x| x.outputs_product(SHOPPING_TIME_ID)) // the processes which output it.
        .sorted_by(|a, b| a.id.cmp(&b.id)) { // ID order.
            // try to do the process up to our target output.
            let eff_skill_level = if let Some(skill_id) = process.skill {
                data.translate_skill(skill, skill_id, skill_level)
            } else {
                0.0 // if nothing to translate into, then skill level doesn't matter.
            };
            let iter_target = (target - final_result) / // the remaining target
                process.effective_output_of(Item::Product(SHOPPING_TIME_ID)); // how much an iteration completes
            let proc_result = process.do_process_with_property(&self.property, 
                &available_wants, eff_skill_level, 0.0, Some(iter_target), false, data);
            if proc_result.iterations == 0.0 {
                continue; // if no successful iterations, skip.
            }
            // else remove/reserve property for the output
            for (&product, &amount) in proc_result.input_output_products.iter() {
                if product == SHOPPING_TIME_ID {
                    // if our product, add to the final result, don't add to property.
                    final_result += amount;
                    // add our time to expenditures.
                    self.property.entry(SHOPPING_TIME_ID)
                        .and_modify(|x| x.spent += amount);
                } else if amount > 0.0 { // if adding, then add to property.
                    self.property.entry(product)
                        .and_modify(|x| x.add_property(amount))
                        .or_insert(PropertyInfo::new(amount));
                } else { // if removing, just remove in total from property. Panic if pulling from reserves just in case.
                    let temp = self.property.get_mut(&product).unwrap();
                    debug_assert!(temp.available() >= -amount, "Trying to use more of product than we have available.");
                    temp.expend(-amount);
                }
            }
            // shift captial goods
            for (&product, &amount) in proc_result.capital_products.iter() {
                // all of this is shifting to captial expended
                self.property.entry(product)
                    .and_modify(|x| x.shift_to_used(amount));
            }
            // add/remove consumed/expended wants
            for (&want, &amount) in proc_result.input_output_wants.iter() {
                // wants consumed here are definitely safe, probably.
                let test = self.want_store.entry(want)
                    .and_modify(|x| *x += amount)
                    .or_insert(amount);
                debug_assert!(*test > 0.0, "Want was made negative.");
            }
        }
        final_result
    }

    /// # First Desire
    /// 
    /// Gets the Desire Coords of the first desire, IE the desire with the 
    /// lowest starting tier, then lowest index.
    /// 
    /// # Panics
    /// 
    /// Panics if property has no desires. All pops should have at least
    /// some desires. Those which truly have no desires are not people, but
    /// objects.
    pub fn first_desire(&self) -> DesireCoord {
        if self.desires.len() == 0 {
            panic!("No Desires in pop!")
        }
        let mut result = DesireCoord{ tier: usize::MAX, idx: usize::MAX };
        // check all desires
        for (idx, desire) in self.desires.iter()
        .enumerate() {
            // if the desire is at or below our current best, update.
            if result.tier >= desire.start {
                if result.tier > desire.start {
                    // if desire is totally below our best, update
                    result.tier = desire.start;
                    result.idx = idx;
                }
                // no need to check lowest idx, as we are going in 
                // idx order already.
            }
            // if our tier is zero, then we can breakout early.
            if result.tier == 0 { break; }
        }
        result
    }

    /// # Get First Unsatisfied Desire
    /// 
    /// Helper function, gets the first desire (lowest tier then lowest index)
    /// which has unsatisfied space. The tier is not the start, but the 
    /// unsatisfied tier.
    /// 
    /// TODO needs to check for the possibility that all desires are satisfied.
    /// 
    /// # Panics
    /// 
    /// Panics if we have no desires.
    pub fn get_first_unsatisfied_desire(&self) -> Option<DesireCoord> {
        if self.desires.len() == 0 {
            panic!("No Desires in pop!")
        }
        let mut result = DesireCoord {
            tier: usize::MAX,
            idx: usize::MAX,
        };
        // check all desires
        for (idx, desire) in self.desires.iter()
        .enumerate()
        .filter(|(_, x)| !x.is_fully_satisfied()) {
            let unsat_tier = desire.satisfaction_up_to_tier();
            if let Some(tier) = unsat_tier {
                if desire.satisfied_at_tier(tier) {
                    let next_tier = desire.get_next_tier_up(tier).unwrap_or(usize::MAX);
                    if next_tier < result.tier {
                        result.idx = idx;
                        result.tier = next_tier;
                    }
                } else {
                    if tier < result.tier {
                        result.idx = idx;
                        result.tier = tier;
                    }
                }
            }
        }
        if result.tier == usize::MAX {
            None
        } else {
            Some(result)
        }
    }

    /// # Record Exchange
    /// 
    /// Records the results of an exchange in various property data.
    /// 
    /// - Positive values are recorded as recieved.
    /// 
    /// - Negative vaules are recorded as Spent.
    /// 
    /// This does not remove or add products to property, merely updates 
    /// spent and recieved.
    /// 
    /// # Panics
    /// 
    /// Panics if any item given is not fonud.
    pub fn record_exchange(&mut self, exchange_results: HashMap<usize, f64>) {
        for (prod, quant) in exchange_results {
            if let Some(info) = self.property.get_mut(&prod) {
                if quant < 0.0 {
                    info.spent -= quant;
                } else {
                    info.recieved += quant;
                }
            } else {
                panic!("Product {} not found in property info.", prod);
            }
        }
    }

    /// # Record Purchase
    /// 
    /// Records a purchase that was made.
    /// 
    /// Adds amv_expended to amv_cost and time_expended to time_cost.
    /// 
    /// Not Tested.
    /// 
    /// # Panics
    /// 
    /// Panics if product given does not currently exist in our property data.
    pub fn record_purchase(&mut self, product: usize, 
    amv_expended: f64) {
        if let Some(info) = self.property.get_mut(&product) {
            info.amv_cost += amv_expended;
        } else {
            panic!("Product {} not found in property info.", product);
        }
    }

    /// # Add Target
    /// 
    /// Adds to the target of a product. If product doesn't currently exist in
    /// property, it adds it with the target set.
    pub fn add_target(&mut self, product: usize, target: f64) {
        if !self.property.contains_key(&product) {
            let mut insert = PropertyInfo::new(0.0);
            insert.max_target = target;
            self.property.insert(product, insert);
        }
    }
}

/// # Time Breakdown
/// 
/// 
#[derive(Debug, Clone, Copy)]
pub struct TimeBreakdown {
    pub unclaimed: f64,
    pub soft_reserved: f64,
    pub hard_reserved: f64
}

/// The coordinates of a desire, both it's tier and index in desires. Used for tier walking.
#[derive(Debug, Clone, Copy, PartialEq)]
pub struct DesireCoord {
    pub tier: usize,
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

/// A simple struct which pairs a value and the tier of that value.
#[derive(Debug, Clone, Copy)]
pub struct TieredValue {
    pub tier: usize,
    pub value: f64,
}

impl Div<TieredValue> for TieredValue {
    type Output = f64;

    fn div(self, rhs: Self) -> Self::Output {
        let other = rhs.shift_tier(self.tier);
        self.value / other.value
    }
}

impl AddAssign for TieredValue {
    fn add_assign(&mut self, rhs: Self) {
        let copy = *self + rhs;
        *self = copy;
    }
}

impl SubAssign for TieredValue {
    fn sub_assign(&mut self, rhs: Self) {
        let copy = *self - rhs;
        *self = copy;
    }
}

impl PartialOrd for TieredValue {
    fn partial_cmp(&self, other: &Self) -> Option<std::cmp::Ordering> {
        // get their tiers equal to each other.
        let updated_other = other.shift_tier(self.tier);
        // then compare their values
        self.value.partial_cmp(&updated_other.value)
    }
}

impl Add for TieredValue {
    type Output = Self;

    /// # TieredValue Add
    /// 
    /// Adds tiered values together.
    /// 
    /// Tier takes the averaged between LHS and RHS (round down).
    /// 
    /// Values are added after tier matching
    fn add(self, rhs: Self) -> Self::Output {
        let middle = (self.tier + rhs.tier) / 2;
        let mut result = TieredValue { tier: middle, value: 0.0 };

        result.value = self.shift_tier(middle).value + rhs.shift_tier(middle).value;

        result
    }
}

impl Sub for TieredValue {
    type Output = Self;

    /// # TieredValue Sub
    /// 
    /// Subtracts tiered values.
    /// 
    /// Takes the tier of Self which is more useful in general.
    /// 
    /// values are subtracted after tier matching.
    /// 
    /// ## Note
    /// 
    /// Can return negative values, meaning that RHS was bigger than LHS.
    fn sub(self, rhs: Self) -> Self::Output {
        let mut result = TieredValue { tier: self.tier, value: self.value };
        result.value -= rhs.shift_tier(self.tier).value;
        result
    }
}

impl PartialEq for TieredValue {
    fn eq(&self, other: &Self) -> bool {
        if self.tier != other.tier {
            let correction = other.shift_tier(self.tier);
            self.tier == correction.tier && self.value == correction.value
        } else {
            self.tier == other.tier && self.value == other.value
        }
    }
}

impl TieredValue {
    /// # Near Eq
    /// 
    /// A helper to allow one to quickly tell if two TieredValues are 
    /// equal to each other within a margin of error (delta).
    pub fn near_eq(&self, other: &Self, delta: f64) -> bool {
        // correct the sign if needed.
        let delta = delta.abs();
        // correct other's tier if needed to match.
        let corrected = if self.tier != other.tier {
            other.shift_tier(self.tier)
        } else { *other };
        // get the upper and lower bounds
        let upper = self.value * (1.0 + delta);
        let lower = self.value * (1.0 - delta);

        if self.value > 0.0 { // if value is positive
            lower <= corrected.value && corrected.value <= upper
        } else { // if negative
            upper <= corrected.value && corrected.value <= lower
        }
    }

    /// # Add Value
    /// 
    /// Adds a value from the given tier and amount of satisfaction at that 
    /// tier which we are adding.
    /// 
    /// This Maintains the Starting Tier of the Tiered Value.
    /// TODO come back here.
    pub fn add_value(&mut self, tier: usize, amount: f64) {
        if self.value == 0.0 { // if amount is already zero, set to the values given.
            self.tier = tier;
            self.value = amount;
            return;
        }
        // amount * (0.9^(self.start-tier))
        let added_val = amount * TieredValue::tier_equivalence(self.tier, tier);
        self.value += added_val;
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
    pub fn tier_equivalence(start: usize, end: usize) -> f64 {
        let start = start as f64;
        let end = end as f64;
        TIER_RATIO.powf(end - start)
    }

    /// # Shift Tier
    /// 
    /// Creates a copy of our tiered value, shifted to a different tier.
    /// 
    /// When shifting, shifting up increases the value by 1/0.9^(change in tier), 
    /// shifting down decreases it by 0.9^(change in tier).
    pub fn shift_tier(&self, tier: usize) -> TieredValue {
        // when shifting is the inverse ratio to tier equivalence.
        let value = TieredValue::tier_equivalence(self.tier, tier);
        TieredValue { tier, value: self.value / value}
    }

    /// # Normalize
    /// 
    /// Normalizes the tier'd value so that it's value is between
    /// 1 and the bound given.
    /// 
    /// If the value is negative, it returns 
    /// 
    /// # Panics
    /// 
    /// If bound <= 1, then it panics.
    pub fn normalize(&self, bound: f64) -> TieredValue {
        if bound <= 1.0 {
            panic!("Bound must be Greater than 1.0.");
        }
        if self.value == 0.0 {
            return self.clone();
        } else if self.value > 0.0 {
            let mut value = self.value;
            let mut alteration = 0;
            if value > bound {
                while value > bound {
                    value *= TIER_RATIO;
                    alteration += 1;
                }
                return TieredValue { tier: self.tier + alteration, value };
            } else {
                while value < 1.0 {
                    value /= TIER_RATIO;
                    alteration += 1;
                }
                return TieredValue { tier: self.tier - alteration, value };
            }
        } else {
            let mut value = -self.value;
            let mut alteration = 0;
            if value > bound {
                while value > bound {
                    value *= TIER_RATIO;
                    alteration += 1;
                }
                return TieredValue { tier: self.tier + alteration, value:-value };
            } else {
                while value < 1.0 {
                    value /= TIER_RATIO;
                    alteration += 1;
                }
                return TieredValue { tier: self.tier - alteration, value: -value };
            }
        }
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