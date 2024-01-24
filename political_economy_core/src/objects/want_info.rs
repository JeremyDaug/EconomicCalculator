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
#[derive(Debug, Copy)]
pub struct WantInfo {
    /// The current total available to use.
    pub total_current: f64,
    /// How much of the want we had at day start.
    pub day_start: f64,
    /// How much we have explicitly gained.
    pub gained: f64,
    /// How much we expect to gain through processes.
    pub expectations: f64,
    /// How much we have expended for processes
    pub expended: f64,
    /// How much we have consumed for desires.
    pub consumed: f64,
}

impl WantInfo {
    pub fn new(day_start: f64) -> Self { 
        Self { total_current: day_start, 
            day_start, 
            gained: 0.0, 
            expectations: 0.0,
            expended: 0.0, 
            consumed: 0.0 
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
        self.expectations = 0.0;
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
        self.total_current + self.expectations
    }

    /// # Expendable
    /// 
    /// How much can be expended right now. Caps available at total_current,
    /// removes any that are expected to be spent.
    pub fn expendable(&self) -> f64 {
        self.total_current + self.expectations.min(0.0)
    }

    /// # Expend
    /// 
    /// Moves value from total to 
    pub fn expend(&mut self, value: f64) {

    }
}