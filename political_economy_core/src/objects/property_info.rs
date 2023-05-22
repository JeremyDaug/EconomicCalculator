/// # Property Info
/// 
/// A Helper which is used ot help sort/divide property between
/// unreserved, reserved, and used for specific, abstract, or want desire.
/// It also stores any purchase, use, and target information alongside it.
/// 
/// The Total Property should be equal to the Unreserved + Reserved + the 
/// highest between specific, abstract, and want.
/// 
/// We only handle shifting from unreserved to the reserves.
/// When adding to a reserve, we allow each reserve to pull from the others 
/// (non-destructively) until they are equal. Once they are, they pull out 
/// of reserve. If none remains in reserve, it removes from unreserved.
#[derive(Debug, Copy, Clone)]
pub struct PropertyInfo {
    /// The total available to us. IE, Unreserved + reserved + max(specific, abstract, want)
    pub total_property: f64,
    /// The amount that is unreserved, available to be spent or shifted elsewhere.
    pub unreserved: f64,
    /// The amount that has been reserved for our desires, but not yet placed. Anywhere
    pub reserved: f64,
    /// The amount that has been reserved for specific product desires.
    pub specific_reserve: f64,
    /// The amount that has been reserved to satisfy abstract product desires.
    pub class_reserve: f64,
    /// The amount that has been reserved to satisfy want desires.
    pub want_reserve: f64,

    /// The target to own for this at the end of each day.
    pub target: f64, 
    /// The number we maintained from yesterday.
    pub rollover: f64,
    /// The number we got by any means today.
    pub recieved: f64,
    /// The amount spent today in exchange for other things.
    pub spent: f64,
    /// The amount which has been consumed today.
    pub consumed: f64,
    /// The amount which was used today, Removed from total_property, meant to be added back in at th end of the day.
    pub used: f64,
    /// How much of the item was lost by any means (taxed, decayed, stolen)
    pub lost: f64,
}

impl PropertyInfo {
    /// # New Property Breakdown
    /// 
    /// News up a property breakdown given an available value.
    /// Automatically sets the total and unreserved.
    pub fn new(available: f64) -> Self { 
        Self { total_property: available, 
            unreserved: available, 
            reserved: 0.0, 
            specific_reserve: 0.0, 
            class_reserve: 0.0, 
            want_reserve: 0.0,
            consumed: 0.0,
            used: 0.0,
            spent: 0.0,
            target: 0.0,
            rollover: 0.0,
            recieved: 0.0,
            lost: 0.0, 
        } 
    }
    
    /// The number of units to still buy.
    pub fn target_remaining(&self) -> f64 {
        self.target - (self.recieved + self.rollover)
    }

    /// # Reset Reserves
    /// 
    /// Removes everything from the reserves, adding them back to unreserved.
    pub fn reset_reserves(&mut self) {
        self.unreserved = self.total_property;
        self.reserved = 0.0;
        self.want_reserve = 0.0;
        self.class_reserve = 0.0;
        self.specific_reserve = 0.0;
    }

    /// # Remove
    /// 
    /// Removes a set number of items from the breakdown.
    /// Removes from Unreserved first, then reserved, then the other reserves.
    /// 
    /// Returns any excess for sanity checking.
    pub fn remove(&mut self, quantity: f64) -> f64 {
        let mut remainder = quantity;
        if self.unreserved > 0.0 { // if any unreserved, remove from there first
            let remove = self.unreserved.min(remainder);
            self.unreserved -= remove;
            self.total_property -= remove;
            remainder -= remove;
            // if remainder used up, exit out.
            if remainder == 0.0 { return 0.0; }
        }
        // if unreserved is not enough, remove from general reserve
        if self.reserved > 0.0 {
            let remove = self.reserved.min(remainder);
            self.reserved -= remove;
            self.total_property -= remove;
            remainder -= remove;
            if remainder == 0.0 { return 0.0; }
        }
        // if unreserved and reserved not enough, remove from specifc reserves
        if self.max_spec_reserve() > 0.0 {
            // TODO rework to use clamp?
            let remove = self.max_spec_reserve().min(remainder);
            self.class_reserve -= remove;
            if self.class_reserve.is_sign_negative() { self.class_reserve = 0.0; }
            self.want_reserve -= remove;
            if self.want_reserve.is_sign_negative() { self.want_reserve = 0.0; }
            self.specific_reserve -= remove;
            if self.specific_reserve.is_sign_negative() { self.specific_reserve = 0.0; }
            self.total_property -= remove;
            remainder -= remove;
        }
        remainder
    }

    /// Adds to both total available and to unreserved
    /// If given negative value, it will not remove anything.
    pub fn add_property(&mut self, quantity: f64) {
        if quantity.is_sign_negative() { return; }
        self.total_property += quantity;
        self.unreserved += quantity;
    }

    /// Shifts the given quantity from unreserved to reserved.
    /// 
    /// If unable to shift all it returns the excess.
    pub fn shift_to_reserved(&mut self, quantity: f64) -> f64 {
        let available = self.unreserved.min(quantity);
        let excess = quantity - available;
        self.unreserved -= available;
        self.reserved += available;
        excess
    }

    /// Gets the highest of our 3 reserves.
    pub fn max_spec_reserve(&self) -> f64 {
        self.specific_reserve
            .max(self.class_reserve)
            .max(self.want_reserve)
    }

    /// Shifts the given quantity into specific reserve.
    /// 
    /// Pulls from other sub-reserves up to the highest, without subtracting 
    /// (allowing overlap), then from the reserve, then from the unreserved.
    pub fn shift_to_specific_reserve(&mut self, quantity: f64) -> f64 {
        let mut quantity = quantity;
        let other_max = self.class_reserve.max(self.want_reserve);
        if other_max > self.specific_reserve { 
            // if we have some from other reserves, get from there first.
            let shift = (other_max-self.specific_reserve).min(quantity);
            self.specific_reserve += shift;
            quantity -= shift;
            if quantity == 0.0 { return 0.0; }
        }
        // not enough from overlap alone, shift from reserve.
        if self.reserved > 0.0 {
            // get the smaller between available, and shift target.
            let shift = self.reserved.min(quantity);
            // remove from reserve, add to spec reserve, and remove from quantity
            self.reserved -= shift;
            self.specific_reserve += shift;
            quantity -= shift;
            if quantity == 0.0 { return 0.0; }
        }
        // if not enough from reserve, then from unreserved.
        if self.unreserved > 0.0 {
            let shift = self.unreserved.min(quantity);
            self.unreserved -= shift;
            self.specific_reserve += shift;
            quantity -= shift;
        }
        quantity
    }

    /// Shifts the given quantity into abstract reserve.
    /// 
    /// Pulls from other sub-reserves up to the highest, without subtracting 
    /// (allowing overlap), then from the reserve, then from the unreserved.
    pub fn shift_to_class_reserve(&mut self, quantity: f64) -> f64 {
        let mut quantity = quantity;
        let other_max = self.specific_reserve.max(self.want_reserve);
        if other_max > self.class_reserve { 
            // if we have some from other reserves, get from there first.
            let shift = (other_max-self.class_reserve).min(quantity);
            self.class_reserve += shift;
            quantity -= shift;
            if quantity == 0.0 { return 0.0; }
        }
        // not enough from overlap alone, shift from reserve.
        if self.reserved > 0.0 {
            // get the smaller between available, and shift target.
            let shift = self.reserved.min(quantity);
            // remove from reserve, add to spec reserve, and remove from quantity
            self.reserved -= shift;
            self.class_reserve += shift;
            quantity -= shift;
            if quantity == 0.0 { return 0.0; }
        }
        // if not enough from reserve, then from unreserved.
        if self.unreserved > 0.0 {
            let shift = self.unreserved.min(quantity);
            self.unreserved -= shift;
            self.class_reserve += shift;
            quantity -= shift;
        }
        quantity
    }

    /// Shifts the given quantity into want reserve.
    /// 
    /// Pulls from other sub-reserves up to the highest, without subtracting 
    /// (allowing overlap), then from the reserve, then from the unreserved.
    pub fn shift_to_want_reserve(&mut self, quantity: f64) -> f64 {
        let mut quantity = quantity;
        let other_max = self.specific_reserve.max(self.class_reserve);
        if other_max > self.want_reserve { 
            // if we have some from other reserves, get from there first.
            let shift = (other_max-self.want_reserve).min(quantity);
            self.want_reserve += shift;
            quantity -= shift;
            if quantity == 0.0 { return 0.0; }
        }
        // not enough from overlap alone, shift from reserve.
        if self.reserved > 0.0 {
            // get the smaller between available, and shift target.
            let shift = self.reserved.min(quantity);
            // remove from reserve, add to spec reserve, and remove from quantity
            self.reserved -= shift;
            self.want_reserve += shift;
            quantity -= shift;
            if quantity == 0.0 { return 0.0; }
        }
        // if not enough from reserve, then from unreserved.
        if self.unreserved > 0.0 {
            let shift = self.unreserved.min(quantity);
            self.unreserved -= shift;
            self.want_reserve += shift;
            quantity -= shift;
        }
        quantity
    }

    /// # Available
    /// 
    /// The amount available for trade or explicit use.
    pub fn available(&self) -> f64 {
        self.unreserved + self.reserved
    }

    /// # Expend
    /// 
    /// A function to expend the product safely from the unreserved
    /// amount.
    /// 
    /// Returns the excess value which could not be gotten from unreserved.
    pub fn expend(&mut self, expense: f64) -> f64 {
        let available = self.unreserved.min(expense);
        let excess = expense - available;
        self.unreserved -= available;
        excess
    }

    /// # Safe Remove
    /// 
    /// Removes a quantity from Unreserved and Reserved, but not the
    /// specific reserves.
    /// 
    /// Returns the excess which could not be removed.
    pub fn safe_remove(&mut self, remove: f64) -> f64 {
        // remove from unreserved first
        let excess = self.expend(remove);
        // if no excess, return 0.0.
        if excess == 0.0 { return 0.0; }
        // if excess remains, remove from reserve.
        let reserve_removal = self.reserved.min(excess);
        self.reserved -= reserve_removal;
        // get whatever remainder there is, and return it.
        return excess - reserve_removal;
    }

    /// # Shift to Used
    /// 
    /// Shifts property from the reserves and unreserved pool to used.
    fn _shift_to_used(&mut self, quantity: f64) {
        let excess = self.remove(quantity);
        self.used += quantity - excess;
    }
}