//! A Desire for a want or product.

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
    pub item: DesireItem,
    pub start: u64,
    pub end: Option<u64>,
    pub amount: f64,
    pub satisfaction: f64,
    pub reserved: f64,
    pub step: u64,
    pub tags: Vec<DesireTag>
}

impl Desire {
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

    /// Calculates what tier this desire is satisfied to, stopping at the last tier
    /// that has any satisfaction in it. If it fully fills it's highest tier
    /// it returns that tier.
    /// 
    /// IE. Start = 1, Step size 1, amount / level 1.0
    /// 
    /// Satisfaction = 100. Highest level satisfied = 100 (not 101).
    /// 
    /// If the satisfaction is 0, it returns None.
    pub fn satisfaction_up_to_tier(&self) -> Option<u64> {
        // sanity check for 0 on tier 0.
        if self.satisfaction == 0.0 {
            return None;
        }

        let total_satisfaction = self.total_satisfaction();
        let satisfied_steps = total_satisfaction.ceil() as u64 - 1;

        if self.is_stretched() {
            if self.is_infinite() {
                return Some(self.start + satisfied_steps as u64 * self.step);
            }
            let cap = std::cmp::min(self.steps() - 1, satisfied_steps);
            return Some(self.start + cap as u64 * self.step);
        }

        // If not stretched, then it can only go up to start.
        return Some(self.start);
    }

    /// Get Next Tier up takes a given tier and gets the next valid tier up.
    /// 
    /// Returns err if no tier is next. Otherwise, returns the next valid tier.
    pub fn get_next_tier_up(&self, tier: u64) -> Option<u64> {
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
    pub fn change_end(&mut self, end: Option<u64>, step: u64) -> Result<(), String> {
        // check that the end can be stepped on by the new values
        if end.is_some() && ((end.unwrap() - self.start) % step) != 0 {
            return Err("End does not get stepped on by the step. Correct so they do.".into());
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
    pub fn steps(&self) -> u64 {
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
    pub fn satisfaction_at_tier(&self, tier: u64) -> Result<f64, String> {// since we know we step on a valid tier, get the total satisfaction
        let total = self.total_satisfaction();

        // get how many steps we have taken at this tier (start == tier = 0)
        let steps = self.steps_to_tier(tier)?;

        let mut at_tier = total - steps as f64;
        // cap the satisfaction at amount.
        if at_tier > 1.0 {
            at_tier = 1.0;
        }
        else if at_tier < 0.0 {
            at_tier = 0.0;
        }

        Ok(at_tier)
    }

    /// How many steps it takes for this desire to reach the given tier.
    pub fn steps_to_tier(&self, tier: u64) -> Result<u64, String> {
        if !self.steps_on_tier(tier) {
            return Err("Does not Step on Tier".into());
        }

        // the current tier reduced by the start tier, divided by the step.
        // we know it is a whole number, so this MUST be safe.
        Ok((tier - self.start) / self.step)
    }

    /// Checks whether a given value steps on a tier of this desire.
    /// 
    /// # Examples
    ///
    /// ```
    /// use political_economy_core::objects::desire::Desire;
    /// use political_economy_core::objects::desire::DesireItem;
    ///
    /// let desire = Desire{item: DesireItem::Product(0),start: 2, end: Some(10),
    ///     amount: 5.0, satisfaction: 15.0, reserved: 0.0, step: 2, tags: vec![]};
    /// assert_eq!(desire.steps_on_tier(4), true);
    /// ```
    pub fn steps_on_tier(&self, tier: u64) -> bool {
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
    pub fn total_desire_at_tier(&self, tier: u64) -> Result<f64, String> {
        // if tier is below starting tier
        if !self.steps_on_tier(tier) {
            return Err("Does not step on tier!".into());
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
    pub fn unsatisfied_to_tier(&self) -> Option<u64> {

        if self.is_fully_satisfied() {
            return None;
        }

        let total_satisfaction = self.total_satisfaction();
        let satisfied_steps = total_satisfaction.floor() as u64;

        if self.is_stretched() {
            if self.is_infinite() {
                return Some(self.start + satisfied_steps as u64 * self.step);
            }
            let cap = std::cmp::min(self.steps() - 1, satisfied_steps);
            return Some(self.start + cap as u64 * self.step);
        }

        // If not stretched, then it can only go up to start.
        return Some(self.start);
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


#[derive(Debug, PartialEq, Eq, Clone)]
pub enum DesireItem {
    Want(usize),
    Product(usize)
}

/// Defines what 
impl DesireItem {

    pub fn unwrap(&self) -> &usize {
        match self {
            DesireItem::Product(prod) => prod,
            DesireItem::Want(want) => want
        }
    }

    /// Checks if the item is a Want.
    pub fn is_want(&self) -> bool {
        match self {
            DesireItem::Product(_) => false,
            DesireItem::Want(_) => true,
        }
    }

    /// Checks if the item is a Product.
    pub fn is_product(&self) -> bool {
        match self {
            DesireItem::Product(_) => true,
            DesireItem::Want(_) => false,
        }
    }

    /// Checks if this is a specific product.
    pub fn is_this_product(&self, item: &usize) -> bool {
        match self {
            DesireItem::Product(val) => val == item,
            DesireItem::Want(_) => false
        }
    }

    /// Checks if this is a specific want.
    pub fn is_this_want(&self, item: &usize) -> bool {
        match self {
            DesireItem::Product(_) => false,
            DesireItem::Want(val) => val == item
        }
    }
}