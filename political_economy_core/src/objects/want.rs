/// A Want is a generic desire that can be sought after. It cannot be
/// bought, sold, or otherwise traded directly, but must be produced
/// by a product or process.
#[derive(Debug)]
pub struct Want {
    /// The unique id of the want
    id: u64,
    /// 
    pub name: String,
    pub description: String,
    pub decay: f64,
    pub ownership_sources: Vec<u64>,
    pub process_sources: Vec<u64>
}

impl Want {
    pub fn new(id: u64, name: String, 
        description: String, decay: f64) -> Result<Self, String> { 
        if decay < 0.0 || decay > 1.0 {
            Result::Err(String::from("Invalid Decay Rate, must be between 0 and 1 (inclusive)"))
        }
        else {
            Result::Ok(Self { id, name, description, decay, ownership_sources: Vec::new(), process_sources: Vec::new()} )
        }
    }

    /// Calculates the decay of a want, based on it's starting value.
    pub fn decay_wants(start: &f64, want: &Want) -> f64 {
        start * want.decay
    }


    pub fn decay(&self) -> f64 {
        self.decay
    }

    pub fn set_decay(&mut self, decay: f64) -> bool {
        if decay > 1.0 || decay < 0.0 {
            return false;
        }
        self.decay = decay;
        true
    }

    pub fn id(&self) -> u64 {
        self.id
    }

    /// adds a product to self.ownership_sources, ensures no duplicates.
    pub fn add_ownership_source(&mut self, product: &super::product::Product) {
        if self.ownership_sources.iter().all(|x| x != &product.id()) {
            self.ownership_sources.push(product.id());
        }
    }
}

impl PartialEq for Want {
    fn eq(&self, other: &Self) -> bool {
        self.id == other.id && self.name == other.name && self.description == other.description
    }
}