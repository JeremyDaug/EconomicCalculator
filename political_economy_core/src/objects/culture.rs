use super::desire::Desire;

#[derive(Debug)]
pub struct Culture{
    id: u64,
    name: String,
    variant_name: String,
    birth_rate_mod: f64,
    death_rate_mod: f64,
    desires: Vec<Desire>,
    // tags
    relations: Vec<Culture>
}

impl Culture {
    pub fn id(&self) -> u64 {
        self.id
    }

    pub fn name(&self) -> &str {
        self.name.as_ref()
    }

    pub fn variant_name(&self) -> &str {
        self.variant_name.as_ref()
    }

    pub fn birth_rate(&self) -> f64 {
        self.birth_rate_mod
    }

    pub fn death_rate(&self) -> f64 {
        self.death_rate_mod
    }

    pub fn desires(&self) -> &[Desire] {
        self.desires.as_ref()
    }

    pub fn relations(&self) -> &[Culture] {
        self.relations.as_ref()
    }
}