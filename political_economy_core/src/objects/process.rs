use super::{skill::Skill, technology::Technology, product::Product, want::Want};

#[derive(Debug)]
pub struct Process {
    id: u64,
    name: String,
    variant_name: String,
    description: String,
    // icon
    minimum_time: f64,
    process_parts: Vec<ProcessPart>,
    process_tags: Vec<ProcessTag>,
    skill: Skill,
    skill_minimum: f64,
    skill_maximum: f64,
    fractional: bool,
    technology_requirement: Option<Technology>,
    tertiary_tech: Option<Technology>
}

impl Process {
    pub fn id(&self) -> u64 {
        self.id
    }

    pub fn name(&self) -> &str {
        self.name.as_ref()
    }

    pub fn variant_name(&self) -> &str {
        self.variant_name.as_ref()
    }

    pub fn description(&self) -> &str {
        self.description.as_ref()
    }

    pub fn minimum_time(&self) -> f64 {
        self.minimum_time
    }

    pub fn process_parts(&self) -> &[ProcessPart] {
        self.process_parts.as_ref()
    }

    pub fn process_tags(&self) -> &[ProcessTag] {
        self.process_tags.as_ref()
    }

    pub fn skill(&self) -> &Skill {
        &self.skill
    }

    pub fn skill_minimum(&self) -> f64 {
        self.skill_minimum
    }

    pub fn skill_maximum(&self) -> f64 {
        self.skill_maximum
    }

    pub fn fractional(&self) -> bool {
        self.fractional
    }

    pub fn technology_requirement(&self) -> Option<&Technology> {
        self.technology_requirement.as_ref()
    }

    pub fn tertiary_tech(&self) -> Option<&Technology> {
        self.tertiary_tech.as_ref()
    }
}

#[derive(Debug)]
pub struct ProcessPortion {
    item: PartItem,
    amount: f64,
    part_tags: Vec<ProcessPartTag>,
    part: ProcessPart
}

impl ProcessPortion {
    pub fn part(&self) -> &ProcessPart {
        &self.part
    }

    pub fn part_tags(&self) -> &[ProcessPartTag] {
        self.part_tags.as_ref()
    }

    pub fn amount(&self) -> f64 {
        self.amount
    }

    pub fn item(&self) -> &PartItem {
        &self.item
    }

    pub fn set_item(&mut self, item: PartItem) {
        self.item = item;
    }
}

#[derive(Debug)]
pub enum ProcessPart {
    Input,
    Capital,
    Output
}

#[derive(Debug)]
pub enum ProcessPartTag {
    Optional(f64),
    Fixed,
    Investment,
    Pollutant,
    Chance(char, u64)
}

#[derive(Debug)]
pub enum ProcessTag {
    Failure(Product),
    Maintenance(Product),
    Consumption(Product),
    Use(Product),
    Chance(u64),
    Crop,
    Mine,
    Extractor,
    Tap,
    Refiner,
    Sorter,
    Scrapping,
    Scrubber
}

#[derive(Debug)]
pub enum PartItem {
    Product(Product),
    Want(Want)
}