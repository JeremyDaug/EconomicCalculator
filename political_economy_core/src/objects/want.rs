use std::fmt::Error;

/// A Want is a generic desire that can be sought after. It cannot be
/// bought, sold, or otherwise traded directly, but must be produced
/// by a product or process.
#[derive(Debug)]
pub struct Want {
    /// Th
    id: u64,
    pub name: String,
    pub description: String,
    decay: f64
}

impl Want {
    pub fn new(id: u64, name: String, description: String, decay: f64) -> Option<Self> { 
        if decay < 0.0 || decay > 1.0 {
            None
        }
        else {
            Some(Self { id, name, description, decay } )
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
}

impl PartialEq for Want {
    fn eq(&self, other: &Self) -> bool {
        self.id == other.id && self.name == other.name && self.description == other.description
    }
}