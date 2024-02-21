use std::collections::HashSet;
use crate::objects::technology_family::TechnologyFamily;

#[derive(Debug)]
pub struct Technology {
    pub id: usize,
    pub name: String,
    pub description: String,
    pub base_cost: i64,
    pub tier: i64,
    pub families: HashSet<TechnologyFamily>,
    pub children: HashSet<Technology>,
    pub parents: HashSet<Technology>
}

impl Technology {
}

pub enum TechnologyCategory {
    Primary,
    Secondary,
    Tertiary { level: u64 }
}