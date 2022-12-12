//! Demographics
//! 
//! Demographics is the storage container and manager common demographic data
//! like Species, Civilizations, Cultures, and Ideologies.
//! 
//! These are stored for reference here.

use super::{species::Species, culture::Culture};

/// Demographics is the data handler for our demographic data. It stores all of our
/// shared population data, making it available for reading during most phases and
/// listing and recording during the population change phase.
pub struct Demographics {
    /// Non-specific Data for Species.
    pub Species: Vec<Species>,
    // civilization
    pub Cultures: Vec<Culture>,
    // Ideology
}

impl Demographics {
    pub fn load_species(&mut self, _file_name: String) -> Result<String, String> {

        Ok("No Problemo".into())
    }
}