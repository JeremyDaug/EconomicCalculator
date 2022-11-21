use std::collections::HashSet;

use super::skill::Skill;

#[derive(Debug)]
pub struct SkillGroup {
    id: u64,
    pub name: String,
    pub description: String,
    pub default: f64,
    pub skills: HashSet<u64>
}

impl SkillGroup {
    pub fn new(id: u64, name: String, description: String,
        default: f64, skills: HashSet<u64>) -> Option<Self> {
        if default < 0.0 || default > 1.0 {
            return None;
        }
        Some(Self {
            id,
            name,
            description,
            default,
            skills
        })
    }
    
    pub fn id(&self) -> u64 {
        self.id
    }

    pub(crate) fn add_skill(&self, skill: &Skill) -> bool {
        self.skills.insert(skill.id())
    }

    pub fn connect_skill(&mut self, skill: &mut Skill) -> bool {
        return self.skills.insert(skill.id()) & skill.insert_skill_group(&self);

    }
}