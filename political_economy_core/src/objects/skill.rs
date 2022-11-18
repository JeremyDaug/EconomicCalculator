use std::collections::{HashSet, HashMap};

use super::{product::Product, skill_group::SkillGroup};

#[derive(Debug)]
pub struct Skill {
    pub(crate) id: u64,
    pub(crate) name: String,
    pub(crate) description: String,
    pub(crate) labor: Product,
    pub(crate) skill_group: HashSet<u64>,
    pub(crate) related_skills: HashMap<u64, f64>,
}

impl Skill {
    pub fn new(id: u64, 
        name: String, 
        description: String, 
        labor: Product) -> Self {

        Self {
            id, 
            name, 
            description, 
            labor, 
            skill_group: HashSet::new(), 
            related_skills : HashMap::new()
        } 
    }


    pub fn id(&self) -> u64 {
        self.id
    }

    pub fn name(&self) -> &str {
        self.name.as_ref()
    }

    pub fn description(&self) -> &str {
        self.description.as_ref()
    }

    pub fn labor(&self) -> &Product {
        &self.labor
    }
}