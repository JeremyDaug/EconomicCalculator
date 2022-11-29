use super::desire::Desire;

#[derive(Debug)]
pub struct Culture{
    pub id: u64,
    pub name: String,
    pub variant_name: String,
    pub birth_rate_mod: f64,
    pub death_rate_mod: f64,
    pub desires: Vec<Desire>,
    // tags
    pub relations: Vec<Culture>
}

impl Culture {
    pub fn get_name(&self) -> String {
        format!("{}({})", self.name, self.variant_name)
    }
}