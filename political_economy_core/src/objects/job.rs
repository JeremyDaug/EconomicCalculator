use std::fmt::format;

use super::{product::Product, skill::Skill, process::Process};

#[derive(Debug)]
pub struct Job {
    pub id: u64,
    pub name: String,
    pub variant_name: String,
    pub labor: Product,
    pub skill: Skill,
    pub processes: Vec<Process>
}

impl Job {
    pub fn new(id: u64,
        name: String, 
        variant_name: String, 
        labor: Product, 
        skill: Skill, 
        processes: Vec<Process>) -> Self {

        Self {
            id, 
            name, 
            variant_name, 
            labor, 
            skill, 
            processes 
        } 
    }

    pub fn get_name(&self) -> String {
        format!("{}({})", self.name, self.variant_name)
    }
}