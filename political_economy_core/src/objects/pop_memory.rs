use std::collections::HashMap;

use crate::constants;


/// A Helper for Pops, recording their data and memories for use in 
/// various calculations and to remember things which should be known
/// by the pop without 
/// 
/// It remembers if we're disorganized, how much time we're saving for
/// work, as well as product knowledge and product priority.
/// 
/// Time for purchases are stored in product_knowledge just like
/// every other product knowledge, though it typ
#[derive(Debug, Clone)]
pub struct PopMemory {
    /// If the pop is part of disorganized firm or not.
    pub is_disorganized: bool,
    /// how much time the pop is willing to send over,
    pub work_time: f64,
    /// The various data for product information. Includes both history
    /// and targets for tomorrow.
    pub product_knowledge: HashMap<usize, Knowledge>,
    /// The order in which products are to be bought.
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
/// 
/// Reset at the start of each day.
/// rollover (which is then set to the target reached by existing owned stock.)
/// achieved
/// spent
/// lost
/// time_spent
/// amv_spent
/// 
/// All other data is updated at the end of the day and retained
/// between days.
/// 
/// TODO make it possible for a pop to reduce shopping trips by buying for multiple days then delaying until more is needed.
#[derive(Debug, Clone, Copy)]
pub struct Knowledge {
    /// The amount targeted to own at the end of the day before consumption.
    pub target: f64,
    /// The amount of the product we kept from yesterday.
    pub rollover: f64,
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
            amv_spent: 0.0, success_rate: 0.5
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

    /// # Unable to purchase
    /// 
    /// This function is called to adjust the self.success_rate
    /// appropriately when the item in unable to be bought
    /// either because the buyer refuses the offer, or because the
    /// seller 
    /// 
    /// When done, it reduces the success rate by 10% of current value.
    pub fn unable_to_purchase(&mut self) {
        self.success_rate *= constants::UNABLE_TO_PURCHASE_REDUCTION;
    }

    /// # Cancelled purchase
    /// 
    /// Called when a buyer decided to not purchase the item they were
    /// seeking, typically due to either running out of products to exchange
    /// or the market price being too high. 
    /// (Market Price * UNABLE_TO_PURCHASE < current_unit_price)
    /// 
    /// Reduces Success Rate by CANCEL_PURCHASE_REDUCTION
    pub fn cancelled_purchase(&mut self) {
        self.success_rate *= constants::CANCELLED_PURCHASE_REDUCTION;
    }

    /// # Successful Purchase
    /// 
    /// Called by a buyer when the purchase attempt was successful.
    /// 
    /// Increases the Success rate by SUCCESSFUL_PURCHASE_INCREASE
    /// Caps the increase at 1.0.
    pub fn successful_purchase(&mut self) {
        self.success_rate *= constants::SUCCESSFUL_PURCHASE_INCREASE;
        // cap it at 1.0
        self.success_rate = self.success_rate.min(1.0);
    }
}