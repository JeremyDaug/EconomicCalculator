use super::desire::Desire;

#[derive(Debug)]
pub struct Species{
    pub id: u64,
    pub name: String,
    pub variant_name: String,
    pub birth_rate: f64,
    pub death_rate: f64,
    pub desires: Vec<Desire>,
    // tags
    pub relations: Vec<Species>
}

impl Species {
    pub fn get_name(&self) -> String {
        format!("{}({})", self.name, self.variant_name)
    }
}