use std::collections::{HashSet, HashMap};

use crate::data_manager::DataManager;

use super::{product::Product, skill_group::SkillGroup};

#[derive(Debug)]
pub struct Skill {
    id: u64,
    pub name: String,
    pub description: String,
    pub labor: u64,
    pub skill_group: HashSet<u64>,
    pub related_skills: HashMap<u64, f64>,
}

impl Skill {
    pub fn new(id: u64, 
        name: String, 
        description: String, 
        labor: u64) -> Self {

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

    pub fn labor<'a>(&self, manager: &'a DataManager) -> &'a Product {
        manager.products.get(&self.id()).unwrap()
    }

    pub fn set_relation(&mut self, relation: &Skill, rate: f64) {
        *self.related_skills.entry(relation.id).or_insert(rate) = rate;
    }

    pub fn set_mutual_relation(&mut self, relation: &mut Skill, rate: f64) {
        self.set_relation(relation, rate);
        relation.set_relation(&self, rate);
    }

    pub(crate) fn insert_skill_group(&mut self, skill_group: &SkillGroup) -> bool {
        self.skill_group.insert(skill_group.id())
    }

    pub fn connect_skill_group(&mut self, skill_group: &mut SkillGroup) {
        self.skill_group.insert(skill_group.id());
        skill_group.add_skill(&self);
    }
}