use std::collections::HashSet;

use super::skill::Skill;

#[derive(Debug)]
pub struct SkillGroup {
    id: u64,
    name: String,
    description: String,
    default: f64,
    skills: HashSet<Skill>
}

impl SkillGroup {
    
    pub fn id(&self) -> u64 {
        self.id
    }

    pub fn name(&self) -> &str {
        self.name.as_ref()
    }

    pub fn description(&self) -> &str {
        self.description.as_ref()
    }

    pub fn default(&self) -> f64 {
        self.default
    }

    pub fn skills(&self) -> &HashSet<Skill> {
        &self.skills
    }
}