use super::{product::Product, skill::Skill, process::Process};

#[derive(Debug)]
pub struct Job {
    id: u64,
    name: String,
    variant_name: String,
    labor: Product,
    skill: Skill,
    processes: Vec<Process>
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

    pub fn id(&self) -> u64 {
        self.id
    }

    pub fn name(&self) -> &str {
        self.name.as_ref()
    }

    pub fn variant_name(&self) -> &str {
        self.variant_name.as_ref()
    }

    pub fn labor(&self) -> &Product {
        &self.labor
    }

    pub fn skill(&self) -> &Skill {
        &self.skill
    }

    pub fn processes(&self) -> &[Process] {
        self.processes.as_ref()
    }
}