use crate::data_manager::DataManager;

use super::process::Process;

/// # The Job class
/// 
/// ## Purpose
/// 
/// The Job struct is how the system stores related processes.
/// A firm stores jobs, which store processes available to them.
/// It does not define how people are assigned or rewarded from it, that's
/// a Firm's task.
#[derive(Debug)]
pub struct Job {
    pub id: u64,
    pub name: String,
    pub variant_name: String,
    // labor not needed, default attached to the skill
    //pub labor: u64,
    pub skill: u64,
    pub processes: Vec<u64>,
    pub consistency_modifier: f64
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
                processes:  vec![],
                consistency_modifier: 1.0
            } 
        }

    /// Adds a process's id, ensuring no duplication.
    pub fn insert_process(&mut self, process_id: u64) {
        if !self.processes.contains(&process_id) {
            self.processes.push(process_id);
        }
    }

    pub fn get_name(&self) -> String {
        format!("{}({})", self.name, self.variant_name)
    }
}