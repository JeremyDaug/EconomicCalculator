//! A Desire for a want or product.

use core::fmt;
use std::error::Error;

use crate::objects::data_objects::item::Item;

/// Desires
/// 
/// Desires are things that are desired and used in a the Desires class.
/// It contains an item, either a want or product, and at minimum a tier
/// at which it applies.
/// 
/// This will likely be broken up and simplified in the future, distinguishing
/// between recorded desires, such as the desires of species, Cultures, etc, and
/// working desires, which are used for satisfaction and trade guarantees.
#[derive(Debug, Clone)]
pub struct Desire {
    /// The item (Product or Want) sought out.
    pub item: Item,
    /// The tier this desire starts at.
    pub start: usize,
    /// The last tier which has this desire.
    /// 
    /// If null, then it either has no last tier, or does not have
    /// multiple steps.
    pub end: Option<usize>,
    /// How much it desires at each tier it steps on.
    pub amount: f64,
    /// How much has been satisfied so far.
    pub satisfaction: f64,
    /// The The step size our desire takes.
    /// 
    /// Start + step * n = valid tiers
    /// 
    /// where n &#8805; 0
    /// 
    /// This is up to end, if there is one.
    pub step: usize,
    /// This defines additional properties of the desire for a pop or 
    /// Demographic.
    /// 
    /// This will likely be broken appart when Demographic desires and pop
    /// desires are split.
    pub tags: Vec<DesireTag>
}

impl Desire {
    pub fn new(item: Item, 
        start: usize, 
        end: Option<usize>, 
        amount: f64, 
        satisfaction: f64, 
        step: usize, 
        tags: Vec<DesireTag>) -> Result<Self, DesireError> { 
            // check that the end is a valid step.
            if let Some(val) = end {
                if (val - start) % step != 0 {
                    return Err(DesireError::TierMisstep(val));
                }
            }
            Ok( Self {
                item,
                start,
                end,
                amount,
                satisfaction,
                step,
                tags
            } )
        }

    /// Checks if a desire is a match for the current desire.
    /// 
    /// It matches on the 
    /// 
    /// - item (product/Want and Id)
    /// - Start
    /// - End
    /// - Step
    /// 
    /// No other factors are used. This is used to make adding desires together easier.
    /// 
    /// Currently tags are not used. Be aware when using this.
    pub fn is_match(&self, other: &Desire) -> bool {
        if self.item != other.item ||
            self.start != other.start ||
            self.end != other.end ||
            self.step != other.step {
            // if any of these are not the same, we aren't equivalent.
            return false;
        }
        true
    }

    /// Adds the given value to our satisfaction, capping at our maximum and
    /// returning the excess.
    /// 
    /// Not meant for negative values, but they are accepted.
    pub fn add_satisfaction(&mut self, add: f64) -> f64 {
        // if infinite, add it all, then return none
        if self.is_infinite() {
            self.satisfaction += add;
            return 0.0;
        }

        // since it's not infinite, we need to find how much it can accept
        // then limit ourselves to that.
        let unsatisfied = self.total_desire()
            .expect("Infinite desire given. How'd you get here?") - self.satisfaction;

        let take = add.min(unsatisfied);

        self.satisfaction += take;
        add - take
    }

    /// Adds the given value to our desire at a specific tier.
    /// 
    /// Adds all it can until it fills that tier. If it has too much, it returns
    /// the excess.
    /// 
    /// If it adds to a tier that the desire doesn't step on it returns an error.
    pub fn add_satisfaction_at_tier(&mut self, add: f64, tier: usize) -> f64 {
        // check that we stepped on the right tier, return add, if we don't
        if !self.steps_on_tier(tier) { return add; }
        // since we do step on it, get our satisfaction at that tier.
        let sat_at_tier = self.satisfaction_at_tier(tier);
        if sat_at_tier == self.amount {
            // if it's a correct tier, but the tier is full, return our amount safely.
            return add;
        }
        // since there is missing satisfaction, get it
        let unsatisfied = self.amount - sat_at_tier;
        // get the smaller between what we can add and what we want to satisfy
        let take = add.min(unsatisfied);
        // add our available up to our satisfaction needed.
        self.satisfaction += take;
        // return the remainder
        add-take
    }

    /// Calculates what tier this desire is satisfied to, stopping at the last tier
    /// that has any satisfaction in it. If it fully fills it's highest tier
    /// it returns that tier.
    /// 
    /// IE. Start = 1, Step size 1, amount / level 1.0
    /// 
    /// Satisfaction = 100. Highest level satisfied = 100 (not 101).
    /// 
    /// If the satisfaction is 0, it returns None.
    pub fn satisfaction_up_to_tier(&self) -> Option<usize> {
        // sanity check for 0 on tier 0.
        if self.satisfaction == 0.0 {
            return None;
        }

        let total_satisfaction = self.total_satisfaction();
        let satisfied_steps = total_satisfaction.ceil() as usize - 1;

        if self.is_stretched() {
            if self.is_infinite() {
                return Some(self.start + satisfied_steps as usize * self.step);
            }
            let cap = std::cmp::min(self.steps() - 1, satisfied_steps);
            return Some(self.start + cap as usize * self.step);
        }

        // If not stretched, then it can only go up to start.
        return Some(self.start);
    }

    /// Get Next Tier up takes a given tier and gets the next valid tier up.
    /// 
    /// Returns None if no tier is next. Otherwise, returns the next valid tier.
    pub fn get_next_tier_up(&self, tier: usize) -> Option<usize> {
        if tier < self.start {
            return Some(self.start);
        }

        if (self.end.is_some() && tier >= self.end.unwrap()) ||
            !self.is_stretched() {
            // if we are at or after the end or at the start and no later steps,
            // then we have no valid tiers.
            return None;
        }

        let next = tier + self.step - ((tier - self.start) % self.step);

        if let Some(end) = self.end {
            if end < next {
                // if the predicted next tier is after our
                return None;
            }
        }

        Some(next)
    }

    /// Changes the end and step for the desire. 
    pub fn change_end(&mut self, end: Option<usize>, step: usize) -> Result<(), DesireError> {
        // check that the end can be stepped on by the new values
        if end.is_some() && ((end.unwrap() - self.start) % step) != 0 {
            return Err(DesireError::TierMisstep(end.unwrap()));
        }

        self.end = end;
        self.step = step;

        Ok(())
    }

    /// Gets how many steps the desire has in total.
    /// 
    /// If the desire is infinite, it return 0.
    /// 
    /// If the desire has only 1 tier, then it's 1.
    pub fn steps(&self) -> usize {
        if self.end.is_none() {
            if self.step > 0 {
                return 0;
            }
            return 1;
        }
        (self.end.unwrap() - self.start) / self.step + 1
    }

    /// If the Desire covers more than 1 tier.
    pub fn is_stretched(&self) -> bool {
        self.step > 0
    }

    /// If the desire is infinite, with no end tier.
    pub fn is_infinite(&self) -> bool {
        self.is_stretched() && self.end.is_none()
    }

    /// How much satisfaction this desire has at a specific tier.
    /// 
    /// Returns the amount in units requested satisfied at this level.
    /// It caps at 0 and self.amount. 
    /// 
    /// Returns 0.0 if it does not step no the tier.
    pub fn satisfaction_at_tier(&self, tier: usize) -> f64 {
        // since we know we step on a valid tier, get the total satisfaction
        let total = self.total_satisfaction();

        // get how many steps we have taken at this tier (start == tier = 0)
        let steps = if let Ok(steps) = self.steps_to_tier(tier) {
            steps
        } else { return 0.0; };

        // get our tier's satisfaction, clamping between 0.0 and 1.0 (total or part)
        // and return it times our amount desired per tier.
        f64::clamp(total - steps as f64, 0.0, 1.0) * self.amount
    }

    /// # Steps To Tier
    /// 
    /// Gets how many steps it takes to reach at valid tier.
    /// 
    /// If it does not step on that tier, it returns an Err<DesireErr::TierMisstep(tier)>.
    pub fn steps_to_tier(&self, tier: usize) -> Result<usize, DesireError> {
        if !self.steps_on_tier(tier) {
            // if we don't step on it, return an err with the tier we misstepped on.
            return Err(DesireError::TierMisstep(tier));
        }
        if self.step == 0 {
            // if the desire doesn't step at all, return OK(0)
            Ok(0)
        } else {
            // the current tier reduced by the start tier, divided by the step.
            // we know it is a whole number, so this MUST be safe.
            Ok((tier - self.start) / self.step)
        }
    }

    /// Checks whether a given value steps on a tier of this desire.
    /// 
    /// # Examples
    pub fn steps_on_tier(&self, tier: usize) -> bool {
        if tier < self.start {
            return false; // if before the start, we can't step on anything.
        }

        if self.start == tier {
            return true; // we are on the starting step.
        }

        if self.is_stretched() {
            if (tier - self.start) % self.step == 0 {
                // if we are on a valid step
                if self.is_infinite() || 
                    self.end.expect("End messing!") >= tier {
                    
                    return true; 
                }
            }
        }

        false
    }

    /// Checks if the desire is fully satisfied or not.
    /// 
    /// If overfilled (satisfaction > total_desire) then it still returns 
    /// true for safety reasons.
    pub fn is_fully_satisfied(&self) -> bool {
        if self.is_infinite() {
            return false;
        }

        if let Some(value) = self.total_desire() {
            return self.satisfaction >= value;
        }

        false
    }

    /// Retrieves the total amount desired across all tiers for the desire.
    /// 
    /// If the desire is infinite, then it returns -1.
    pub fn total_desire(&self) -> Option<f64> {
        if self.is_infinite() { return None; }

        if !self.is_stretched() { return Some(self.amount); }

        Some(self.steps() as f64 * self.amount)
    }

    /// Calculates the total satisfaction of the desire, returning it
    /// measured in the number of tiers satisfied.
    /// 
    /// Does not include skipped tiers
    pub fn total_satisfaction(&self) -> f64 {
        self.satisfaction / self.amount
    }

    /// Gets the total amonut desired up to the tier given.
    /// 
    /// If the tier is below the starting tier it returns 0.
    /// 
    /// If the tier is between its start and end tier, it returns
    /// the amount * the tiers belowe it's current tier.
    pub fn total_desire_at_tier(&self, tier: usize) -> Result<f64, DesireError> {
        // if tier is below starting tier
        if !self.steps_on_tier(tier) {
            return Err(DesireError::TierMisstep(tier));
        }

        // if it isn't stretched and we're at or above the
        // start
        if !self.is_stretched() {
            return Ok(self.amount);
        }

        // get the steps up to the given tier, +1
        let current_steps = (tier - self.start) / self.step + 1;
        // if current steps after last 
        if self.end.is_some() && self.end.unwrap() < tier {
            return Ok(self.amount * (self.steps() as f64));
        }
        Ok((current_steps as f64) * self.amount)
    }

    /// Walking up the desire, it gets the first tier which has any unsatisfied
    /// quantity.
    /// 
    /// If totally sastisfied at a tier, it will return the next available tier,
    /// or None.
    /// 
    /// If it is totally satisfied, it returns None.
    pub fn unsatisfied_to_tier(&self) -> Option<usize> {

        if self.is_fully_satisfied() {
            return None;
        }

        let total_satisfaction = self.total_satisfaction();
        let satisfied_steps = total_satisfaction.floor() as usize;

        if self.is_stretched() {
            if self.is_infinite() {
                return Some(self.start + satisfied_steps as usize * self.step);
            }
            let cap = std::cmp::min(self.steps() - 1, satisfied_steps);
            return Some(self.start + cap as usize * self.step);
        }

        // If not stretched, then it can only go up to start.
        return Some(self.start);
    }

    /// Takes this desire, creates a copy, and increases the amount desired.
    /// 
    /// Used to easily and safely copy and multiply a desire for pop desires.
    pub fn create_multiple(&self, factor: usize) -> Self {
        let mut copy = self.clone();
        copy.amount *=  factor as f64;
        copy
    }

    /// # Past End
    /// 
    /// Helper function which checks if the given tier is beyond the last tier 
    /// that this desire can reach.
    pub fn past_end(&self, tier: usize) -> bool {
        if let Some(last) = self.end { // if we have an end, check against that
            return last < tier;
        }
        // if we are infinite, we can never get past the end.
        if self.is_infinite() {
            return false;
        }
        // if we have no end and are not infinite, then we must have 1 tier, our start.
        return self.start < tier;
    }

    /// # Before Start
    /// 
    /// Helper function which checks that the given tier is before our first tier.
    pub fn before_start(&self, tier: usize) -> bool {
        self.start > tier
    }

    /// # Steps in interval
    /// 
    /// Helper function, checks if a desire steps in an interval between 
    /// start to end (inclusive).
    /// 
    /// Returns None if start > end
    pub fn steps_in_interval(&self, start: usize, end: usize) -> bool {
        if end < start { return false; }
        if start == 0 { // if start is 0
            // check for 0 start
            if self.start == start {
                return true;
            } else if let Some(val) = self.get_next_tier_up(start) {
                return val <= end;
            } else { return false; }
        } else {
            if let Some(val) = self.get_next_tier_up(start - 1) {
                return start <= val && val <= end;
            }
            false
        }
    }

    /// # Satisfied at Tier
    /// 
    /// Checks if we are satisfied at a specific tier or not.
    /// 
    /// No satisfaction, it's always false.
    /// 
    /// If tier is below the highest tier with satisfaction, it's true.
    /// 
    /// if above the highest tier with satisfaction, it's false.
    /// 
    /// if it matches, then it checks if that specific tier is fully satisfied.
    pub fn satisfied_at_tier(&self, tier: usize) -> bool {
        if let Some(upto) = self.satisfaction_up_to_tier() {
            match tier.cmp(&upto) {
                std::cmp::Ordering::Less => true,
                std::cmp::Ordering::Equal => {
                    self.amount == self.satisfaction_at_tier(tier)
                },
                std::cmp::Ordering::Greater => false,
            }
        } else { // if no satisfaction, cannot be satisfied anywhere.
            false
        }
    }

    /// # Missing Satisfaction
    /// 
    /// Calculates how much satisfaction we need to satisfy the given tier.
    /// 
    /// If already oversatisfied, return 0.
    pub fn missing_satisfaction(&self, tier: usize) -> f64 {
        let steps = self.steps_to_tier(tier).expect("Doesn't step on Tier!");
        let total_needed = ((steps+1) as f64) * self.amount;
        (total_needed - self.satisfaction).max(0.0)
    }
}

/// The tags a desire can be marked by, modifying how the desire is viewed.
/// Many of these alter how they are treated, removing them from common
/// calculations.
#[derive(Debug, Clone)]
pub enum DesireTag{
    /// The person does not actually desire this item, if this item is 
    /// satisfied, it instead increases their chance of sickness and death.
    Toxic,
    /// The desire is not needed every day, instead they only desire it
    /// occasionally. How likely they are on any given ady is equal to
    /// the value inside it.
    Sporadic(u64),
    /// The item is not needed consistently, but instead periodically and
    /// consistently. The value given is how often they need 
    Periodic { offset: u64, cycle: u64 },
}


/// Errors for desires, so we know more explicitly how and where we messed up.
#[derive(Debug)]
pub enum DesireError {
    /// An error for a tier mistep, the value contained is where it tried to land
    /// the desire did not have that step.
    TierMisstep(usize)
}

impl fmt::Display for DesireError {
    fn fmt(&self, f: &mut fmt::Formatter) -> fmt::Result {
        match *self {
            DesireError::TierMisstep(tier) => write!(f, "Tier Misstepped on: {}", tier),
        }
    }
}

impl Error for DesireError {}