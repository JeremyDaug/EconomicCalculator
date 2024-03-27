/// # Want Information
/// 
/// Like property info, this stores info about wants over a day in the market.
/// May be used by any actor.
/// 
/// Includes:
/// 
/// - day_start: how much of the want was had at the start of the day.
/// - gained: How much has been gained so far today.
/// - expectations: How much we are planning to produce at some later point.
/// - expended: how much was expended in processes
/// - consumed: how much was conumed for desires.
/// - total_current: How much is available in total
#[derive(Debug, Clone, Copy)]
pub struct WantInfo {
    /// The current total available to use.
    pub total_current: f64,
    /// How much of the want we had at day start.
    pub day_start: f64,
    /// How much we have explicitly gained.
    pub gained: f64,
    /// How much we expect to gain through processes.
    pub expected: f64,
    /// How much we have expended for processes
    pub expended: f64,
    /// How much we have consumed for desires.
    pub consumed: f64,
    /// How much was been lost, typically to decay.
    pub lost: f64
}

impl WantInfo {
    pub fn new(day_start: f64) -> Self { 
        Self { total_current: day_start, 
            day_start, 
            gained: 0.0, 
            expected: 0.0,
            expended: 0.0, 
            consumed: 0.0,
            lost: 0.0
        } 
    }

    /// # New Day
    /// 
    /// Clears out data, setting day start to the total current.
    /// 
    /// This does ignore expectations, assuming that if it hasn't been added
    /// then it's not going to be.
    pub fn new_day(&mut self) {
        self.day_start = self.total_current;
        self.gained = 0.0;
        self.expected = 0.0;
        self.expended = 0.0;
        self.consumed = 0.0;
    }

    /// # Consumable
    /// 
    /// Get how much from this is likely to be consumed, including 
    /// expectations.
    /// 
    /// If the value is zero, assume there is a problem.
    pub fn consumable(&self) -> f64 {
        self.total_current + self.expected
    }

    /// # Expendable
    /// 
    /// How much can be expended right now. Caps available at total_current,
    /// removes any that are expected to be spent.
    pub fn expendable(&self) -> f64 {
        self.total_current + self.expected.min(0.0)
    }

    /// # Expend
    /// 
    /// Moves value from total to expended.
    /// 
    /// It should only have positive values less than or equal to 
    /// self.total_current.
    pub fn expend(&mut self, value: f64) {
        debug_assert!(value > 0.0, "Value is negative.");
        self.total_current -= value;
        self.expended += value;
        debug_assert!( self.expendable() > 0.0, 
            "Expended more than was available.");
    }

    /// # Realize All
    /// 
    /// Takes current expectations and adds/subtracts it from the
    /// total_current.
    pub fn realize_all(&mut self) {
        self.total_current += self.expected;
        self.expected = 0.0;
        debug_assert!(self.total_current > 0.0, "Expected losses larger than current total available.")
    }

    /// # Realize
    /// 
    /// Realize takes the value given and shifts it to (or away) from
    /// expectations and into the total. Positive values take positive from
    /// expectation adds that to total_current and negative values take negative
    /// value from expectations and removes it from total_current.
    /// 
    /// # Debug Checks
    /// 
    /// Checks if the value it is trying to shift cannot be realized. IE,
    /// mismatched signes between value and self.expectations or same sign
    /// but value > self.expectations (abs)
    pub fn realize(&mut self, value: f64) {
        // if sign mismatch
        debug_assert!(self.expected.is_sign_negative() == value.is_sign_negative(), 
            "Value expectation sign mismatch, one is positive the other is negative.");
        // value to shift is 
        debug_assert!(self.expected.abs() > value.abs(),
            "Value to realize is too big.");
        self.expected -= value;
        self.total_current += value;
    }

    /// # Consume
    /// 
    /// Moves valee from total to consumed, also takes from expectations
    /// if it can. Consumes total current first.
    /// 
    /// # Panics
    /// 
    /// Panics if the value shifted results in a negative total_current.
    pub fn consume(&mut self, value: f64) {
        debug_assert!(self.consumable() >= value, 
            "Value to consume is greater than total available.");
        debug_assert!(value > 0.0,
            "Cannot consume a negative value.");

        self.total_current -= value;
        if self.total_current < 0.0 {
            self.expected += self.total_current;
            self.total_current = 0.0;
        }
        self.consumed += value;
    }

    /// # Add
    /// 
    /// Adds the given value to the total_current and
    /// gained.
    pub fn add(&mut self, value: f64) {
        debug_assert!(value > 0.0, 
            "Value cannot be zero.");
        self.total_current += value;
        self.gained += value;
    }
}