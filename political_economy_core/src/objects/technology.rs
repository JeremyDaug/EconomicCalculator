use std::collections::HashSet;
use crate::objects::technology_family::TechnologyFamily;

#[derive(Debug)]
pub struct Technology {
    id: u64,
    name: String,
    description: String,
    base_cost: i64,
    tier: i64,
    families: HashSet<TechnologyFamily>,
    children: HashSet<Technology>,
    parents: HashSet<Technology>
}

impl Technology {
    pub fn id(&self) -> u64 {
        self.id
    }

    pub fn name(&self) -> &str {
        self.name.as_ref()
    }

    pub fn description(&self) -> &str {
        self.description.as_ref()
    }

    pub fn tier(&self) -> i64 {
        self.tier
    }

    pub fn base_cost(&self) -> i64 {
        self.base_cost
    }

    pub fn families(&self) -> &HashSet<TechnologyFamily> {
        &self.families
    }

    pub fn children(&self) -> &HashSet<Technology> {
        &self.children
    }

    pub fn parents(&self) -> &HashSet<Technology> {
        &self.parents
    }
}

pub enum TechnologyCategory {
    Primary,
    Secondary,
    Tertiary { level: u64 }
}