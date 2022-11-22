use std::collections::HashSet;

use super::process::{Process, ProcessTag, PartItem};

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
    /// The products which produce it via owning it.
    pub ownership_sources: HashSet<u64>,
    /// All processes which produce it.
    pub process_sources: Vec<u64>,
    /// All use processes which produce it.
    pub use_sources: Vec<u64>,
    // All consumption processes which produce it.
    pub consumption_sources: Vec<u64>
}

impl Want {
    pub fn new(id: u64, name: String, 
        description: String, decay: f64) -> Result<Self, String> { 
        if decay < 0.0 || decay > 1.0 {
            Result::Err(String::from("Invalid Decay Rate, must be between 0 and 1 (inclusive)"))
        }
        else {
            Result::Ok(Self { id, name, description, decay, 
                ownership_sources: HashSet::new(), 
                process_sources: Vec::new(),
                use_sources: vec![],
                consumption_sources: vec![]} )
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
            self.ownership_sources.insert(product.id());
    }

    /// Not Tested
    /// Adds a process to this want if that want is an output of the process
    /// if it's not it won't add it and will return an Result::Err().
    pub fn add_process_source<'a>(&mut self, process: &Process) 
        -> Result<(), &'a str> {
        
        // sanity check that the process has us in it.
        let mut contains_want = false;
        for output in process.process_parts.iter()
            .filter(|x| x.part.is_output()) {
            if let PartItem::Want(id) = output.item {
                if id == self.id {
                    contains_want = true;
                }
            }
        };
        if !contains_want {
            return Result::Err("Process does not contain Want!");
        }
        // since it does, go through it's tags and add it to the appropriate sections.
        for tag in process.process_tags.iter() {
            match tag {
                ProcessTag::Use(_prod) => {
                    self.use_sources.push(process.id);
                },
                ProcessTag::Consumption(_prod) => {
                    self.consumption_sources.push(process.id);
                },
                _ => {}
            }
        }
        // always add the process to all since we do output the want.
        self.process_sources.push(process.id);
        Ok(())
    }
}

impl PartialEq for Want {
    fn eq(&self, other: &Self) -> bool {
        self.id == other.id && self.name == other.name && self.description == other.description
    }
}