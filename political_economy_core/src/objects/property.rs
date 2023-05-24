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

use std::collections::{HashMap, HashSet};

use itertools::Itertools;

use crate::{data_manager::DataManager, constants::TIER_RATIO};

use super::{desire::{Desire, DesireItem}, market::MarketHistory, process::PartItem, property_info::PropertyInfo};

/// Desires are the collection of an actor's Desires. Includes their property
/// excess / unused wants, and AI data for acting on buying and selling.
#[derive(Debug)]
pub struct Property {
    /// All of the desires we are storing and looking over.
    pub desires: Vec<Desire>,
    /// The property currently owned bey the actor.
    pub property: HashMap<usize, PropertyInfo>,
    /// The wants stored and not used up yet.
    pub want_store: HashMap<usize, f64>,
    /// Is Disorganized
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
    pub process_expectations: HashMap<usize, f64>,
    /// The Expected inputs and outputs of our process plan for wants.
    pub process_want_expectations: HashMap<usize, f64>,
    /// The highest tier to which these desires have been fully
    /// satisfied.
    /// 
    /// A rough measure of contentment. 
    /// 
    /// Low values with stuffed desires mark material wealth with
    /// low contentment. High values with sparse desires mark material
    /// poverty but personal contentment.
    pub full_tier_satisfaction: usize,
    /// How many tiers, skipping empty ones, which have been filled.
    /// 
    /// A rough measure of total satisfaction. Removes contentment and
    /// emphasizes material satisfaction.
    pub hard_satisfaction: usize,
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
            full_tier_satisfaction: 0,
            hard_satisfaction: 0,
            quantity_satisfied: 0.0,
            partial_satisfaction: 0.0,
            market_satisfaction: 0.0,
            highest_tier: 0,
            process_plan: HashMap::new(),
            process_expectations: HashMap::new(),
            process_want_expectations: HashMap::new(),
            is_disorganized: true,
            work_time: 0.0,
            wage_estimate: 0.0,
            extra_benefits: 0.0,
            todays_wage: 0.0,
            pay_period: 1,
            is_sifted: true
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
            .walk_up_tiers_for_item(&None, &DesireItem::Product(*product));
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
            .walk_up_tiers_for_item(&curr, &DesireItem::Product(*product));
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
    /// TODO test this
    pub fn unsafe_add_property(&mut self, product: usize, amount: f64) {
        if amount == 0.0 { return; }
        self.is_sifted = false;
        if let Some(property) = self.property.get_mut(&product) {
            property.add_property(amount);
        }
    }

    /// # Add Property
    /// 
    /// Adds or removes an item to property, returning the value gained (or lost)
    /// by the addition (or subtraction).
    /// 
    /// If property is sifted, it maintains the sifted status.
    /// 
    /// ## Note
    /// 
    /// # Panics
    /// 
    /// Panics if the value being subtracted is greater than our available property.
    /// 
    /// TODO Test This
    pub fn add_property(&mut self, product: usize, amount: f64, data: &DataManager) -> TieredValue {
        if amount < 0.0 { // if removing property, jump to remove.
            return self.remove_property(product, -amount, data);
        }
        if !self.is_sifted { // if we aren't sifted, don't bother trying to properly sift.
            self.unsafe_add_property(product, amount);
            return TieredValue{ tier: 0, value: 0.0 };
        }

        // put into our property,
        self.property.entry(product)
            .and_modify(|x| { x.add_property(amount); })
            .or_insert(PropertyInfo::new(amount));
        // break out amount for each so we can try to satsify them appropriately.
        let mut specific_amount = amount;
        let mut class_amount = amount;
        let mut want_amount = amount;
        // get class for checking
        let prod_class = data.get_product_class(product);
        // also extract property into more usable form for updating processes
        let mut property_sum = HashMap::new();
        for (&product, info) in self.property.iter() {
            property_sum.insert(product, info.total_property);
        }
        // and pre-emptively remove based on our previously planned processes.
        for (proc_id, iters) in self.process_plan.iter() {
            let process = data.processes.get(proc_id).unwrap();
            let output = process.do_process(&property_sum, 
                &self.want_store, 0.0, 0.0, Some(*iters), 
                true, data);
            for (input, amount) in output.input_output_products.iter()
            .filter(|(_, &amount)| amount < 0.0) {
                property_sum.entry(*input).and_modify(|x| *x += amount);
            }
            for (capital, amount) in output.capital_products.iter() {
                property_sum.entry(*capital).and_modify(|x| *x -= amount);
            }
        }
        // and record desires we've already visited and failed to do anything with
        let mut cleared = HashSet::new();
        // then sift the product into our desires
        let mut current_coord = 
            Some(DesireCoord { tier: self.hard_satisfaction-1, idx: self.desires.len() });
        // quickly get all those disers which we already passed up
        for (idx, desire) in self.desires.iter().enumerate() {
            if desire.past_end(self.hard_satisfaction) {
                cleared.insert(idx);
            }
        }
        while let Some(coords) = self.walk_up_tiers(current_coord) {
            current_coord = Some(coords);
            if specific_amount == 0.0 && class_amount == 0.0 && want_amount == 0.0 {
                break; // if we've used all of our amounts, gtfo.
            }
            if cleared.len() == self.desires.len() {
                break; // if we cleared all desires, get out.
            }
            if cleared.contains(&coords.idx) {
                continue; // If the desire is already in skip and go to the next step.
            }
            let mut desire = self.desires.get_mut(coords.idx).unwrap();
            match desire.item {
                DesireItem::Want(want) => {
                    // if it's a want, check that this product can satisfy that want.
                    let want_info = data.wants.get(&want).unwrap();
                    // start with ownership sources
                    if want_info.ownership_sources.contains(&product) {
                        let prod_info = data.products.get(&product).unwrap();
                        // our product can satisfy this, so reserve what is needed to satisfy.
                    }

                    //let procs = Vec![];
                    for proc in want_info.use_sources
                    // TODO Pick up here !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                },
                DesireItem::Class(class) => { // if product class
                    // check stuff left to spent
                    if class_amount == 0.0 {
                        cleared.insert(coords.idx);
                        continue;
                    }
                    if let None = prod_class { // check if we have the class
                        cleared.insert(coords.idx);
                        continue;
                    }
                    // if we do, then check the classes match.
                    let prod_class = prod_class.unwrap();
                    if prod_class != class {
                        cleared.insert(coords.idx);
                        continue;
                    }
                    // if it is our class and we have some amount to shift left, do so
                    let remainder = (desire.amount - desire.satisfaction_at_tier(coords.tier))
                        .min(class_amount);
                    class_amount -= remainder; // remove from local
                    self.property.get_mut(&product) // shift to class reserve
                        .unwrap().shift_to_class_reserve(remainder);
                    desire.satisfaction += remainder; // add to satisfaction
                    if let Some(last) = desire.end { // check if last tier
                        if last == coords.tier {
                            cleared.insert(coords.idx);
                            continue;
                        }
                    }

                },
                DesireItem::Product(specific) => { // if specic item.
                    // if we don't have any more specific amount to spend, record and continue.
                    if specific_amount == 0.0 {
                        cleared.insert(coords.idx);
                        continue;
                    }
                    // if product, check that it's our specific product
                    if product != specific { // if not, add to cleared and continue.
                        cleared.insert(coords.idx);
                        continue; 
                    }
                    // if it is, try to satisfy.
                    // get the amount we need to satisfy (capped at our ability to satisfy)
                    let remainder = (desire.amount - desire.satisfaction_at_tier(coords.tier))
                        .min(specific_amount);
                    specific_amount -= remainder; // remove from local
                    self.property.get_mut(&product) // shift property
                        .unwrap().shift_to_specific_reserve(remainder);
                    desire.satisfaction += remainder; // add to satisfaction.
                    if let Some(last) = desire.end { // check if last tier.
                        if last == coords.tier {
                            cleared.insert(coords.idx);
                            continue;
                        }
                    }
                },
            }
        }
        // then class desires
        // then wants.

        todo!("Fix this to work correctly.") 
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
    /// # Panics
    /// 
    /// Panics if the amount given is greater than our property available.
    pub fn remove_property(&mut self, _product: usize, _amount: f64, 
        _data: &DataManager) -> TieredValue {
        todo!("Not made")
    }

    /// # Remove Satisfaction
    /// 
    /// Removes satisfaction dependent on a product, and the amount given.
    /// 
    /// Returns the total amount of the product successfully removed and the effective value lost by it's removal.
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
            let mut specific_reduction = prop_data.specific_reserve - target;
            let mut class_reduction = prop_data.class_reserve - target;
            let want_reduction = prop_data.want_reserve - target;
            // then, if value is positive, remove for each
            if specific_reduction > 0.0 {
                let mut specific_desire_coord = DesireCoord{ tier: self.highest_tier, idx: self.desires.len() };
                specific_desire_coord = self.walk_down_tiers_for_item(&specific_desire_coord, &DesireItem::Product(product)).unwrap();
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
                        &DesireItem::Product(product));
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
                class_desire_coord = self.walk_down_tiers_for_item(&class_desire_coord, &DesireItem::Class(class)).unwrap();
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
                        &DesireItem::Class(class));
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
                    process.accepts_as_input(product, data)
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
                    let want = data.wants.get(desire.item.unwrap()).expect("Want not found.");
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
                        let output = proc.effective_output_of(PartItem::Want(*desire.item.unwrap()));
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
            sum_in_value += value.1 * Property::tier_equivalence(0, value.0);
        }
        let mut sum_out_value = 0.0;
        for value in out_value {
            sum_out_value += value.1 * Property::tier_equivalence(0, value.0);
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
    pub fn out_barter_value(&self, product: &usize, amount: f64) -> Option<(usize, f64)> {
        // get those desires with reserves to remove work with.
        let possible = self.desires.iter()
            .filter(|x| x.item.is_this_specific_product(product) && x.satisfaction != 0.0)
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
                let available = desire.satisfaction_at_tier(step.tier);
                if available > 0.0 {
                    // since satisfaction is available, do work
                    // get tier equivalency 
                    let eqv = Property::tier_equivalence(tier, step.tier);
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
    pub fn get_highest_satisfied_tier_for_item(&self, item: DesireItem) -> Option<usize> {
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
    pub fn get_lowest_unsatisfied_tier_of_item(&self, item: DesireItem) -> Option<usize> {
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
    pub fn in_barter_value(&self, product: &usize, amount: f64) -> Option<(usize, f64)> {
        // get those desires which want it an can still be satisfied
        let possible = self.desires.iter()
            .filter(|x| x.item.is_this_specific_product(&product) && !x.is_fully_satisfied())
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
                let diff = desire.amount - desire.satisfaction_at_tier(step.tier);
                if diff > 0.0 {
                    // since the current step has missing satisfaction.
                    // get the equivalence ratio.
                    let equiv = Property::tier_equivalence(tier, step.tier);
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
        self.full_tier_satisfaction = usize::MAX;
        self.highest_tier = 0;
        // for each desire
        for desire in self.desires.iter() {
            let tier = desire.satisfaction_up_to_tier().expect("Not Satisfied somehow?");
            if !desire.is_fully_satisfied() {
                // get it's highest fully satisfied tier
                let tier = if desire.satisfaction_at_tier(tier) < 1.0 {
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
                    Property::tier_equivalence(self.full_tier_satisfaction, 
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
        .filter(|x| x.item.is_specific()) {
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
    pub fn total_satisfaction_of_item(&self, item: DesireItem) -> f64 {
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
    pub fn sift_all(&mut self, _data: &DataManager) {
        // which desires we have either fully satisfied, or can no longer satisfy.
        //let finished = HashSet::new();
        // Reset satisfaction, and property breakdown
        todo!("Come back here!")
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
    pub fn consume_and_sift_wants(&mut self, _data: &DataManager) -> HashMap<usize, f64> {
        todo!("Redo this with new property system.")
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
}

/// The coordinates of a desire, both it's tier and index in desires. Used for tier walking.
#[derive(Debug, Clone, Copy)]
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
impl TieredValue {
    /// # Add Value
    /// 
    /// Adds a value from the given tier and amount of satisfaction at that 
    /// tier which we are adding.
    /// 
    /// This Maintains the Starting Tier of the Tiered Value.
    /// TODO come back here.
    pub fn add_value(&mut self, tier: usize, amount: f64) {
        let _tier_diff = amount * TieredValue::tier_equivalence(self.tier, tier);
        todo!("Come back here!")
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