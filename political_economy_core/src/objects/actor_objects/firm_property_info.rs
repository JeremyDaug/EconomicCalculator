/// # Firm Property Info
/// 
/// Used to store data on a firms property. A simplified version of PropertyInfo 
/// for pops as firms do not need to consume products and are more interested in 
/// using, 
#[derive(Debug, Clone)]
pub struct FirmPropertyInfo {
    /// The total currently owned property.
    pub total_property: f64,
    /// Marks the product this is attached to as to not be offered for sale under normal circumstances.
    pub is_reserved: bool,
    /// The minimum needed for plans.
    pub minimum: f64,
    /// How many multiples of the minimum it will build up to if all minimums reached.
    pub stockpile_mult: f64,

    /// How many were produced, Positive Value
    pub produced: f64,
    /// How many were lost in a day, Negative Value
    pub lost: f64,
    /// How many of this product was used today in purchases. Negative Value
    pub spent: f64,
    /// How many were consumed in work. Negative Value
    pub consumed: f64,
    /// How many were used as capital and are otherwise unavailable. Neutral, but negative.
    pub expended: f64,

    /// How many of this product were purchased.
    pub purchased: f64,
    /// How much time was spent to get 
    pub time_cost: f64,
    /// How much amv was spent to get these goods today.
    pub amv_cost: f64,
    /// The rolling average cost of the unit over the past 10 days.
    /// (9*y + T) / 10
    pub amv_unit_estimate: f64,
}

impl FirmPropertyInfo {
    /// # New
    /// 
    /// Creates a new default FirmPropertyInfo
    pub fn new() -> Self {
        FirmPropertyInfo {
            total_property: 0.0,
            is_reserved: false,
            minimum: 0.0,
            stockpile_mult: 1.0,
            lost: 0.0,
            spent: 0.0,
            consumed: 0.0,
            expended: 0.0,
            purchased: 0.0,
            time_cost: 0.0,
            amv_cost: 0.0,
            amv_unit_estimate: 0.0,
            produced: 0.0,
        }
    }

    /// # Is Reserve
    /// 
    /// Sets is_reserved to true.
    pub fn is_reserved(mut self) -> Self {
        self.is_reserved = true;
        self
    }
    
    /// # With Total Property
    /// 
    /// Sets total Property, consuming and returning the data.
    pub fn with_total_property(mut self, value: f64) -> Self {
        self.total_property = value;
        self
    }
    
    /// # Expend Capital
    /// 
    /// Removes what is expended from the total and adds it to expended.
    pub fn expend_capital(&mut self, expended: f64) {
        debug_assert!(expended > 0.0);
        self.total_property -= expended;
        self.expended += expended;
    }

    /// # Production Change
    /// 
    /// Adds and records a change in the property info caused by
    /// production.
    pub fn production_change(&mut self, change: f64) {
        self.total_property += change;
        if change > 0.0 {
            self.produced += change;
        } else {
            self.consumed -= change;
        }
    }
    
    /// # Release Expended
    /// 
    /// Returns any expended property safely to our total_property.
    pub fn release_expended(&mut self) {
        self.add_property(self.expended);
        self.expended = 0.0;
    }
    
    /// # Add Property
    /// 
    /// Adds an amount to our property safely.
    pub fn add_property(&mut self, amt: f64) {
        debug_assert!(self.total_property + amt >= 0.0);
        self.total_property += amt;
    }
    
    /// Remove
    /// 
    /// Subtracts the amount given, value expected to be negative, but
    /// either accepted.
    pub fn remove(&mut self, amt: f64) {
        debug_assert!(self.total_property - amt >= 0.0);
        self.total_property -= amt;
    }
    
    pub fn total_and_expended(&self) -> f64 {
        self.total_property + self.expended
    }
}

impl Default for FirmPropertyInfo {
    fn default() -> Self {
        FirmPropertyInfo::new()
    }
}