use std::collections::{HashSet, HashMap};

use super::{product::Product, skill_group::SkillGroup};

#[derive(Debug)]
pub struct Skill {
    id: u64,
    name: String,
    description: String,
    labor: Product,
    skill_group: HashSet<SkillGroup>,
    related_skills: HashMap<Skill, f64>,
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

    pub fn skill_group(&self) -> &HashSet<SkillGroup> {
        &self.skill_group
    }

    pub fn related_skills(&self) -> &HashMap<Skill, f64> {
        &self.related_skills
    }
}