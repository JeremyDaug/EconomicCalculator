//! Demographics
//! 
//! Demographics is the storage container and manager common demographic data
//! like Species, Civilizations, Cultures, and Ideologies.
//! 
//! These are stored for reference here.

use super::{species::Species, culture::Culture, desire::{Desire, DesireItem, DesireTag}};

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
        let mut humie = Species{
            id: 0,
            name: "Humie".into(),
            variant_name: String::new(),
            desires: vec![],
            tags: vec![],
            relations: vec![],
            base_productivity: 1.0,
            birth_rate: 0.02,
            mortality_rate: 0.01,
            data_table: vec![],
        };

        // food desires, the only Required items for a species currently.
        // 1 unit prefered total, half is always necissary.
        let food = Desire{ 
            item: DesireItem::Want(1), 
            start: 0, 
            end: Some(1), 
            amount: 0.5, 
            satisfaction: 0.0, 
            reserved: 0.0, 
            step: 1, 
            tags: vec![] 
        };
        // Rest desire, 0 required, 4 for health,
        // 8 hours preferred.
        let rest = Desire{ 
            item: DesireItem::Want(0), 
            start: 1, 
            end: Some(2), 
            amount: 4.0, 
            satisfaction: 0.0, 
            reserved: 0.0, 
            step: 1, 
            tags: vec![] 
        };

        Ok("No Problemo".into())
    }
}