use std::collections::HashMap;


/// A job stored in a firm.
/// 
/// Used to store the data for a firm's jobs.
#[derive(Debug)]
pub struct FirmJob {
    /// The pop which is in this firm job.
    pub pop: usize,
    /// The Job (id) in question the pop is doing.
    pub job: usize,
    /// The logic of the wage they recieve.
    pub wage_type: WageType,
    /// The wage given to the pop. This is on a per person basis and multiplied
    /// by a factor decided by WageType.
    /// 
    /// - Slaves, Salaried, and Profit sharing workers recieve a flat payment 
    /// per day regardless of work done.
    /// - LossSharing recieves no wage at all, instead recieving all property 
    /// of the firm at the end of the work day.
    ///   Productivity is paid by the number of iterations they do in their assignment.
    /// - Daily and Contractor are paid by the number of hours they sell in a day.
    pub wage: HashMap<usize, f64>,
    /// Acceptable conversions take place between wage goods, to cover fractional
    /// or alternative forms of payment.
    /// 
    /// Only needed for wages which are more flexible
    pub accetped_conversions: Vec<(usize, usize, f64)>,
    /// The exact assignments of the job, The process Id is the key.
    pub assignments: HashMap<usize, AssignmentInfo>,
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
    _progress: f64,
}

impl AssignmentInfo {
    pub fn new(iterations: f64, progress: f64) -> Self { 
        Self { 
            iterations, 
            _progress: progress 
        }
    }
}