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
    pub product_reserve: f64,
    /// The amount that has been reserved to satisfy abstract product desires.
    pub class_reserve: f64,
    /// The amount that has been reserved to satisfy want desires.
    pub want_reserve: f64,

    /// The upper target we will buy up to. 
    pub max_target: f64,
    /// The target to own for this at the end of each day.
    pub min_target: f64, 
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
    /// How much time was spent today to get what we have.
    pub time_cost: f64,
    /// How much AMV was spent today to get what we have.
    pub amv_cost: f64,
    /// How much AMV we typically spend per unit.
    /// 
    /// This is updated at the end of the day by the formula
    /// 
    /// ( 9*Y + T ) / 10
    pub amv_unit_estimate: f64,
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
            product_reserve: 0.0, 
            class_reserve: 0.0, 
            want_reserve: 0.0,
            consumed: 0.0,
            used: 0.0,
            spent: 0.0,
            max_target: 1.0,
            min_target: 0.0,
            rollover: 0.0,
            recieved: 0.0,
            lost: 0.0, 
            time_cost: 0.0,
            amv_cost: 0.0,
            amv_unit_estimate: 1.0
        } 
    }

    /// # Start of Day
    /// 
    /// Does all start of day work needed for property.
    /// 
    /// The Reset of our daily record data which need to be 0.0 at the 
    /// start of the day.
    /// 
    /// Also sets rollover to the total property right now.
    pub fn start_of_day(&mut self) {
        // TODO update amv_unit_estimate
        self.recieved = 0.0;
        self.spent = 0.0;
        self.consumed = 0.0;
        self.used = 0.0;
        self.lost = 0.0;
        self.time_cost = 0.0;
        self.amv_cost = 0.0;
        self.rollover = self.total_property;
    }

    /// # Reset Reserves
    /// 
    /// Removes everything from the reserves, adding them back to unreserved.
    pub fn reset_reserves(&mut self) {
        self.unreserved = self.total_property;
        self.reserved = 0.0;
        self.want_reserve = 0.0;
        self.class_reserve = 0.0;
        self.product_reserve = 0.0;
    }

    /// # Add Property
    /// 
    /// Adds property to our total property and unreserved pool.
    /// 
    /// If value given is negative, it calls remove.
    /// 
    /// If removal, it returns the excess which could not be removed.
    pub fn add_property(&mut self, quantity: f64) {
        if quantity.is_sign_negative() { self.remove(-quantity); }
        else {
            self.total_property += quantity;
            self.unreserved += quantity;
        }
    }

    /// # Shift To Reserved
    /// 
    /// Shifts the given quantity from unreserved to reserved.
    pub fn shift_to_reserved(&mut self, quantity: f64) {
        let available = self.unreserved.min(quantity);
        self.unreserved -= available;
        self.reserved += available;
    }

    /// Gets the highest of our 3 reserves.
    pub fn max_spec_reserve(&self) -> f64 {
        self.product_reserve
            .max(self.class_reserve)
            .max(self.want_reserve)
    }

    /// Shifts the given quantity into specific reserve.
    /// 
    /// Pulls from other sub-reserves up to the highest, without subtracting 
    /// (allowing overlap), then from the reserve, then from the unreserved.
    /// 
    /// Shifts based on the absolute value.
    pub fn shift_to_specific_reserve(&mut self, quantity: f64) {
        let mut quantity = quantity.abs();
        let other_max = self.class_reserve.max(self.want_reserve);
        if other_max > self.product_reserve { 
            // if we have some from other reserves, get from there first.
            let shift = (other_max-self.product_reserve).min(quantity);
            self.product_reserve += shift;
            quantity -= shift;
            if quantity == 0.0 { return; }
        }
        // not enough from overlap alone, shift from reserve.
        if self.reserved > 0.0 {
            // get the smaller between available, and shift target.
            let shift = self.reserved.min(quantity);
            // remove from reserve, add to spec reserve, and remove from quantity
            self.reserved -= shift;
            self.product_reserve += shift;
            quantity -= shift;
            if quantity == 0.0 { return; }
        }
        // if not enough from reserve, then from unreserved.
        if self.unreserved > 0.0 {
            let shift = self.unreserved.min(quantity);
            self.unreserved -= shift;
            self.product_reserve += shift;
        }
    }

    /// Shifts the given quantity into abstract reserve.
    /// 
    /// Pulls from other sub-reserves up to the highest, without subtracting 
    /// (allowing overlap), then from the reserve, then from the unreserved.
    /// 
    /// Shifts based on the absolute value.
    pub fn shift_to_class_reserve(&mut self, quantity: f64) {
        let mut quantity = quantity.abs();
        let other_max = self.product_reserve.max(self.want_reserve);
        if other_max > self.class_reserve { 
            // if we have some from other reserves, get from there first.
            let shift = (other_max-self.class_reserve).min(quantity);
            self.class_reserve += shift;
            quantity -= shift;
            if quantity == 0.0 { return; }
        }
        // not enough from overlap alone, shift from reserve.
        if self.reserved > 0.0 {
            // get the smaller between available, and shift target.
            let shift = self.reserved.min(quantity);
            // remove from reserve, add to spec reserve, and remove from quantity
            self.reserved -= shift;
            self.class_reserve += shift;
            quantity -= shift;
            if quantity == 0.0 { return; }
        }
        // if not enough from reserve, then from unreserved.
        if self.unreserved > 0.0 {
            let shift = self.unreserved.min(quantity);
            self.unreserved -= shift;
            self.class_reserve += shift;
        }
    }

    /// Shifts the given quantity into want reserve.
    /// 
    /// Pulls from other sub-reserves up to the highest, without subtracting 
    /// (allowing overlap), then from the reserve, then from the unreserved.
    /// 
    /// Shifts based on the absolute value.
    pub fn shift_to_want_reserve(&mut self, quantity: f64) {
        let mut quantity = quantity.abs();
        let other_max = self.product_reserve.max(self.class_reserve);
        if other_max > self.want_reserve { 
            // if we have some from other reserves, get from there first.
            let shift = (other_max-self.want_reserve).min(quantity);
            self.want_reserve += shift;
            quantity -= shift;
            if quantity == 0.0 { return; }
        }
        // not enough from overlap alone, shift from reserve.
        if self.reserved > 0.0 {
            // get the smaller between available, and shift target.
            let shift = self.reserved.min(quantity);
            // remove from reserve, add to spec reserve, and remove from quantity
            self.reserved -= shift;
            self.want_reserve += shift;
            quantity -= shift;
            if quantity == 0.0 { return; }
        }
        // if not enough from reserve, then from unreserved.
        if self.unreserved > 0.0 {
            let shift = self.unreserved.min(quantity);
            self.unreserved -= shift;
            self.want_reserve += shift;
        }
    }

    /// # Available
    /// 
    /// The amount available that has not been specifically reserved,
    /// or used.
    pub fn available(&self) -> f64 {
        self.unreserved + self.reserved
    }

    /// # Expend
    /// 
    /// A function to expend the product safely from the unreserved
    /// amount. Will not reduce property below available unreserved.
    pub fn expend(&mut self, expense: f64) {
        let available = self.unreserved.min(expense);
        self.unreserved -= available;
        self.total_property -= available
    }

    /// # Safe Remove
    /// 
    /// Removes a quantity from Unreserved and Reserved, but not the
    /// specific reserves.
    /// 
    /// Removes from unreserved first, then reserved.
    pub fn safe_remove(&mut self, remove: f64) {
        // remove from unreserved first
        let unreserved_removal = self.unreserved.min(remove);
        
        if unreserved_removal > 0.0 {
            self.unreserved -= unreserved_removal;
            self.total_property -= unreserved_removal;
        }
        let reserved_removal = self.reserved.min(remove - unreserved_removal);
        if reserved_removal == 0.0 { return; }
        self.reserved -= reserved_removal;
        self.total_property -= reserved_removal;
    }

    /// # Remove
    /// 
    /// Removes a set number of items from the breakdown.
    /// Removes from Unreserved first, then reserved, then the other reserves.
    /// 
    /// If value is negative, it instead adds it via self.add_property().
    /// 
    /// # Note
    /// 
    /// Does not allow property or reserves to enter the negative. Be sure
    /// that you check it correctly.
    pub fn remove(&mut self, quantity: f64) {
        if quantity.is_sign_negative() {
            return self.add_property(-quantity);
        }
        let mut remainder = quantity;
        if self.unreserved > 0.0 { // if any unreserved, remove from there first
            let remove = self.unreserved.min(remainder);
            self.unreserved -= remove;
            self.total_property -= remove;
            remainder -= remove;
            // if remainder used up, exit out.
            if remainder == 0.0 { return; }
        }
        // if unreserved is not enough, remove from general reserve
        if self.reserved > 0.0 {
            let remove = self.reserved.min(remainder);
            self.reserved -= remove;
            self.total_property -= remove;
            remainder -= remove;
            if remainder == 0.0 { return; }
        }
        // if unreserved and reserved not enough, remove from specifc reserves
        let max_spec_reserve = self.max_spec_reserve();
        if max_spec_reserve > 0.0 {
            // TODO rework to use clamp?
            let remove = max_spec_reserve.min(remainder);
            self.class_reserve -= remove;
            if self.class_reserve.is_sign_negative() { self.class_reserve = 0.0; }
            self.want_reserve -= remove;
            if self.want_reserve.is_sign_negative() { self.want_reserve = 0.0; }
            self.product_reserve -= remove;
            if self.product_reserve.is_sign_negative() { self.product_reserve = 0.0; }
            self.total_property -= remove;
        }
    }

    /// # Shift to Used
    /// 
    /// Shift to used is meant to 'expend' but not consume property. It
    /// removes from it from all possible noraml piles, adds it to used, but
    /// does not remove it from total_property.
    /// 
    /// # Panics
    /// 
    /// Panics if the amount being shifted is creater than the total property.
    /// 
    /// TODO test this once we actually use it.
    pub fn shift_to_used(&mut self, quantity: f64) {
        // remove from reserves
        self.remove(quantity);
        // add back to total property
        self.total_property += quantity;
        // add to used
        self.used += quantity;
    }

    /// # Available for Specific pool
    /// 
    /// How much of our product can be shifted into 
    /// specific reserve pool.
    pub fn available_for_specific(&self) -> f64 {
        self.total_property - self.product_reserve
    }

    /// # Available for Class Pool
    /// 
    /// How much of our product can be shifted into the class desire pool.
    pub fn available_for_class(&self) -> f64 {
        self.total_property - self.class_reserve
    }

    /// # Available for Want Pool
    /// 
    /// How much of our porduct can be shifted into the want pool.
    pub fn available_for_want(&self) -> f64 {
        self.total_property - self.want_reserve
    }

    /// # Remaining Target
    /// 
    /// How many units we are needed to reach our max target.
    pub fn remaining_target(&self) -> f64 {
        self.max_target - self.total_property
    }
}