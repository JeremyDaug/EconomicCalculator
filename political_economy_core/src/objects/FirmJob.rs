use std::collections::HashMap;


/// A job stored in a firm.
/// 
/// Used to store the data for a firm's jobs.
#[derive(Debug)]
pub struct FirmJob {
    /// The Job (id) in question.
    pub job: usize,
    /// The type of wage they recieve.
    pub wage_type: WageType,
    /// the value of the wage given in AMV.
    pub wage: f64,
    /// The unit(s) of the wage,
    pub wage_unit_priority: Vec<usize>,
    /// The exact assignments of the job.
    pub assignments: HashMap<usize, AssignmentInfo>,
    /// The pop which is in this firm job.
    pub pop: usize,
}

/// What kind of wage types are available for jobs.
#[derive(Debug)]
pub enum WageType {
    /// The Pop does not recieve a wage, cannot find a new job, can be 
    /// bought or sold as property, and are fed by resources gathered by the
    /// firm. The firm absorbs their desires in return for a captive workforce
    /// and the slaves in turn give a half-hearted effort.
    Slave,
    /// Like ProfitSharing, but losses to the firm are also applied to the
    /// pop with this wage type. Generally used for Private firms on the
    /// owners, and onto the workers of a Disorganized Firm.
    LossSharing,
    /// Workers are paid based on their productivity.
    Productivity,
    /// Workers are paid hourly, and recieve their wage daily.
    Daily,
    /// The Worker is paid a consistent salary, not based on hours, but 
    /// instead on consistent, long term, productivity.
    Salary,
    /// Gig Economy Style position, like daily, but with a lower bar
    /// to fire and quick to hire.
    Contractor,
    /// Typically usef for owners or shareholders with limited liability.
    /// They also tend to have a salary wage also to ensure a minimum level
    /// of income.
    ProfitSharing
}

/// Assignment Information for Firm Jobs to record and upkeep their work.
#[derive(Debug)]
pub struct AssignmentInfo {
    /// how many iterations are being attempted.
    iterations: f64,
    /// how much progress is leftover from yesterday.
    progress: f64,
}

impl AssignmentInfo {
    pub fn new(iterations: f64, progress: f64) -> Self { 
        Self { 
            iterations, 
            progress 
        }
    }
}