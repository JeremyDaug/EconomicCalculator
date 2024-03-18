use super::technology::Technology;

#[derive(Debug)]
pub struct TechnologyFamily {
    id: u64,
    name: String,
    description: String,
    related_families: Vec<Technology>
    
    // Related Techs
}

impl TechnologyFamily {
    pub fn id(&self) -> u64 {
        self.id
    }

    pub fn name(&self) -> &str {
        self.name.as_ref()
    }

    pub fn description(&self) -> &str {
        self.description.as_ref()
    }

    pub fn related_families(&self) -> &[Technology] {
        self.related_families.as_ref()
    }
}