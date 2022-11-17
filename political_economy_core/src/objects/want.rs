#[derive(Debug)]
pub struct Want {
    id: u64,
    name: String,
    description: String,

}

impl Want {
    pub fn id(&self) -> u64 {
        self.id
    }

    pub fn name(&self) -> &str {
        self.name.as_ref()
    }

    pub fn description(&self) -> &str {
        self.description.as_ref()
    }
}