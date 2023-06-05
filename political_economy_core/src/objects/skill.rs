use std::{collections::{HashSet, HashMap}, vec};

use super::{product::Product, skill_group::SkillGroup, process::{Process, ProcessPart, ProcessPartTag}};

#[derive(Debug)]
pub struct Skill {
    pub id: usize,
    pub name: String,
    pub description: String,
    pub labor: usize,
    pub skill_group: HashSet<usize>,
    pub related_skills: HashMap<usize, f64>,
}

impl Skill {
    pub fn new(id: usize, 
        name: String, 
        description: String, 
        labor: usize) -> Self {

        Self {
            id, 
            name, 
            description, 
            labor, 
            skill_group: HashSet::new(), 
            related_skills : HashMap::new()
        } 
    }

    /// Creates a default labor for the skill, if it doesn't have one.
    /// 
    /// # Default Product
    /// 
    /// The product defaults to sharing the skill's name and description, 
    /// It has no variant name, 
    /// Unit defaults to "Hour(s)"
    /// Fractional is true
    /// It has a Mean Time To Failure of 0 (not entirely necessary)
    /// It is tagged as a Service
    /// It has no required tech.
    pub fn build_skill_labor(&self, id: usize) -> Option<Product> {
        // if labor == 0, then it must not have a labor (0 is time which is not a labor.)
        if self.labor != 0 {
            return Option::None;
        }

        Product::new(id,
            self.name.clone(),
            String::new(),
            self.description.clone(),
            String::from("Hour(s)"),
            0, 0.0, 0.0,
            Some(0),
            true,
            vec![super::product::ProductTag::Service],
            None,
            None)
    }

    /// Creates a default process for the skill and it's labor.
    /// 
    /// # Defalts
    /// 
    /// The process defaults to taking 1 Time(fixed) (always id 0) 
    /// and outputting 1 unit of labor.
    /// 
    /// 
    /// The name is Labor, while vairant_name and description are the skill's name.
    /// 
    /// 
    /// Minimum time = 0, skill min = 0, skill max = 3, and no techs attached.
    /// 
    /// The skill is also connected as the process's skill
    /// 
    /// # Notes
    /// 
    /// Edit to meet your needs later, as needed.
    pub fn build_skill_process(&self, id: usize) -> Result<Process, String> {
        if self.labor == 0 {
            return Err(format!("Skill '{}' has no Labor.", self.name))
        }
        let input = ProcessPart {
            item: super::process::PartItem::Specific(0),
            amount: 1.0,
            part_tags: vec![ProcessPartTag::Fixed],
            part: super::process::ProcessSectionTag::Input,
        };
        let output = ProcessPart{
            item: super::process::PartItem::Specific(self.labor),
            amount: 1.0,
            part_tags: vec![],
            part: super::process::ProcessSectionTag::Output,
        };
        Ok(Process {
                    id: id,
                    name: "Labor".into(),
                    variant_name: self.name.clone(),
                    description: self.description.clone(),
                    minimum_time: 0.0,
                    process_parts: vec![input, output],
                    process_tags: vec![],
                    skill: Some(self.id),
                    skill_minimum: 0.0,
                    skill_maximum: 3.0,
                    technology_requirement: None,
                    tertiary_tech: None,
                })
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