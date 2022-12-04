#[derive(Debug)]
pub struct Firm {
    pub id: usize,
    pub name: String,
    pub variant_name: String
}

impl Firm {
    pub fn new(id: usize, name: String, variant_name: String) -> Self { Self { id, name, variant_name } }

    pub fn get_name(&self) -> String {
        format!("{}({})", self.name, self.variant_name)
    }
}