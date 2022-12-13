//! The storage unit of population groups.
//! 
//! Used for any productive, intellegent actor in the system. Does not include animal
//! populations.
use std::collections::HashMap;

use super::{job::Job, firm::Firm, market::Market, skill::Skill, product::Product, species::Species, culture::Culture};

#[derive(Debug)]
pub struct Pop {
    pub id: u64,
    pub name: String,
    pub count: u64,
    pub job: Job,
    pub firm: Firm,
    pub market: Market,
    pub skill: Skill,
    pub lower_skill_level: f64,
    pub higher_skill_level: f64,
    pub property: HashMap<Product, f64>,
    pub species: HashMap<Species, u64>,
    // civilization
    pub culture: HashMap<Culture, u64>
    // Ideology
    // Movements
    // Mood
    // desires
    // for sale?
}

impl Pop {
    pub fn name(&self) -> &str {
        self.name.as_ref()
    }

    pub fn count(&self) -> u64 {
        self.count
    }

    pub fn job(&self) -> &Job {
        &self.job
    }

    pub fn firm(&self) -> &Firm {
        &self.firm
    }

    pub fn market(&self) -> &Market {
        &self.market
    }

    pub fn skill(&self) -> &Skill {
        &self.skill
    }

    pub fn lower_skill_level(&self) -> f64 {
        self.lower_skill_level
    }

    pub fn higher_skill_level(&self) -> f64 {
        self.higher_skill_level
    }

    pub fn property(&self) -> &HashMap<Product, f64> {
        &self.property
    }

    pub fn species(&self) -> &HashMap<Species, u64> {
        &self.species
    }

    pub fn culture(&self) -> &HashMap<Culture, u64> {
        &self.culture
    }
}

