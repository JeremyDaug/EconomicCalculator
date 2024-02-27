use std::collections::HashSet;

use super::skill::Skill;

#[derive(Debug)]
#[deprecated]
pub struct SkillGroup {
    id: usize,
    pub name: String,
    pub description: String,
    pub default: f64,
    pub skills: HashSet<usize>
}

impl SkillGroup {
    pub fn new(id: usize, name: String, description: String,
        default: f64, skills: HashSet<usize>) -> Option<Self> {
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
    
    pub fn id(&self) -> usize {
        self.id
    }

    pub fn add_skill(&mut self, skill: &Skill) -> bool {
        self.skills.insert(skill.id)
    }

    pub fn connect_skill(&mut self, skill: &mut Skill) -> bool {
        return self.skills.insert(skill.id) & skill.insert_skill_group(&self);

    }
}