use std::fmt::format;

use super::{product::Product, skill::Skill, process::Process};

#[derive(Debug)]
pub struct Job {
    pub id: u64,
    pub name: String,
    pub variant_name: String,
    // labor not needed, default attached to the skill
    //pub labor: u64,
    pub skill: u64,
    pub processes: Vec<u64>
}

impl Job {
    pub fn new(id: u64,
        name: String,
        variant_name: String,
        skill: u64) -> Self {
            Self {
                id, 
                name, 
                variant_name, 
                skill, 
                processes:  vec![]
            } 
        }

    pub fn get_name(&self) -> String {
        format!("{}({})", self.name, self.variant_name)
    }
}