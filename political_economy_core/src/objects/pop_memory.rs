use std::collections::HashMap;


/// A Helper for Pops, recording their data and memories for use in 
/// various calculations and to remember things which should be known
/// by the pop without 
#[derive(Debug, Clone)]
pub struct PopMemory {
    /// If the pop is part of disorganized firm or not.
    pub is_disorganized: bool,
    /// how much time the pop is willing to send over,
    pub work_time: f64,
    /// The various data for product information. Includes both history
    /// and targets for tomorrow.
    pub product_knowledge: HashMap<usize, Knowledge>,
}

impl PopMemory {
    pub(crate) fn create_empty() -> PopMemory {
        PopMemory { is_disorganized: false, work_time: 0.0,
            product_knowledge: HashMap::new(),}
    }
}

/// Product knowledge for a pop.
#[derive(Debug, Clone, Copy)]
pub struct Knowledge {
    /// The amount targeted to own.
    pub target: f64,
    /// How much was spent.
    pub spent: f64,
    /// How much time we are willing to expend to get our target.
    pub time_budget: f64,
    /// How much AMV we are willing to spend to get our target.
    pub amv_budget: f64,
    /// How much time was left over from yesterday.
    pub time_remainder: f64,
    /// How much AMV was not spent yesterday.
    pub amv_excess: f64,
    /// How successful we have been in the past in satisfying
    /// our target and explicit desires which seek this item.
    pub success_rate: f64,
}