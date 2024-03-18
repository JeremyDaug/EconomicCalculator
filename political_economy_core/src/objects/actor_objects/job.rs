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
    pub id: usize,
    pub name: String,
    pub variant_name: String,
    // labor not needed, default attached to the skill
    //pub labor: usize,
    pub skill: usize,
    pub processes: Vec<usize>,
    pub consistency_modifier: f64
}

impl Job {
    pub fn new(id: usize,
        name: String,
        variant_name: String,
        skill: usize) -> Self {
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
    pub fn insert_process(&mut self, process_id: usize) {
        if !self.processes.contains(&process_id) {
            self.processes.push(process_id);
        }
    }

    pub fn get_name(&self) -> String {
        if self.variant_name.len() == 0 {
            return self.name.clone();
        }
        format!("{}({})", self.name, self.variant_name)
    }
}