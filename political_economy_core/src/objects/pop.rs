use std::collections::HashMap;

use super::{job::Job, firm::Firm, market::Market, skill::Skill, product::Product, species::Species, culture::Culture};

#[derive(Debug)]
pub struct Pop {
    name: String,
    count: u64,
    job: Job,
    firm: Firm,
    market: Market,
    skill: Skill,
    lower_skill_level: f64,
    higher_skill_level: f64,
    property: HashMap<Product, f64>,
    species: HashMap<Species, u64>,
    // civilization
    culture: HashMap<Culture, u64>
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

