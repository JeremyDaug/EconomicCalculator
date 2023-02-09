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
    /// The order in which 
    pub product_priority: Vec<usize>,
}

impl PopMemory {
    pub(crate) fn create_empty() -> PopMemory {
        PopMemory { is_disorganized: false, work_time: 0.0,
            product_knowledge: HashMap::new(),
            product_priority: vec![]}
    }
}

/// Product knowledge for a pop.
#[derive(Debug, Clone, Copy)]
pub struct Knowledge {
    /// The amount targeted to own.
    pub target: f64,
    /// The amount we successfully had (bought or otherwise) by
    /// the end of the day.
    pub achieved: f64,
    /// How much of the item was given in exchange.
    pub spent: f64,
    /// how much of the item was lost for any reason,
    /// (taxed, failed etc)
    pub lost: f64,
    /// How much time we are willing to expend to get our target.
    pub time_budget: f64,
    /// How much AMV we are willing to spend to get our target.
    pub amv_budget: f64,
    /// How much time was spent. Time Budget - Time Spent = Remainder.
    pub time_spent: f64,
    /// How much AMV was spent. Budget - Spent = Remainder.
    pub amv_spent: f64,
    /// How successful we have been in the past in satisfying
    /// our target and explicit desires which seek this item.
    pub success_rate: f64,
}

impl Knowledge {
    pub fn new() -> Self { 
        Self { 
            target: 0.0, achieved: 0.0, 
            spent: 0.0, lost: 0.0, 
            time_budget: 0.0, amv_budget: 0.0, 
            time_spent: 0.0, 
            amv_spent: 0.0, success_rate: 0.0
    } }

    pub fn remaining_amv(&self) -> f64 {
        self.amv_budget - self.amv_spent
    }

    pub fn remaining_time(&self) -> f64 {
        self.time_budget - self.time_spent
    }

    /// The number of units to still buy.
    pub fn target_remaining(&self) -> f64 {
        self.target - self.achieved
    }

    /// The AMV budget / the target units to buy.
    pub fn unit_budget(&self) -> f64 {
        self.amv_budget / self.target
    }

    /// Gets the current budget per unit of item left to buy.
    /// If no target remaining it returns 0.0.
    pub fn current_unit_budget(&self) -> f64 {
        if self.target_remaining() == 0.0 {
            return 0.0;
        }
        self.remaining_amv() / self.target_remaining()
    }
}