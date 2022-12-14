//! The collection which manages multiple desires.
//! 
//! This collection manages and organizes the desires of an
//! actor into a rational fashion. Here is also where 
//! the validity of a potential barter is decided or not.

use std::collections::HashMap;

use super::desire::{DesireItem, Desire};

#[derive(Debug)]
pub struct Desires {
    /// All of the desires we are storing and looking over.
    pub items: Vec<Desire>,
    /// The property currently owned bey the actor.
    pub property: HashMap<DesireItem, f64>,
    pub targets: HashMap<DesireItem, f64>
}

pub struct DesireInfo {
    pub target: f64,
    pub bought: f64,
    pub exchanged: f64,
    pub time_budget: f64,
    pub amv_budget: f64,
    pub time_returned: f64,
    pub amv_returned: f64,
    pub success: f64,
}