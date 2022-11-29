#[derive(Debug)]
pub struct Firm {
    pub id: u64,
    pub name: String,
    pub variant_name: String
}

impl Firm {
    pub fn new(id: u64, name: String, variant_name: String) -> Self { Self { id, name, variant_name } }

    pub fn get_name(&self) -> String {
        format!("{}({})", self.name, self.variant_name)
    }
}