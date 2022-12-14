//! Demographics is the storage container and manager common demographic data
//! like Species, Civilizations, Cultures, and Ideologies.
//! 
//! These are stored for reference here.

use std::collections::HashMap;

use super::{species::Species, culture::Culture, desire::{Desire, DesireItem}};

/// Demographics is the data handler for our demographic data. It stores all of our
/// shared population data, making it available for reading during most phases and
/// listing and recording during the population change phase.
pub struct Demographics {
    /// Non-specific Data for Species.
    pub species: HashMap<usize, Species>,
    // civilization
    pub cultures: HashMap<usize, Culture>,
    // Ideology
}

impl Demographics {
    /// Species Loader.
    /// 
    /// Currently does not load from file, just loads hard coded data.
    /// 
    /// Will add in file loading later.
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
            //data_table: vec![],
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
        // Rest desire, 0 required, 2 for health every level up,
        // 8 hours preferred.
        let rest = Desire{ 
            item: DesireItem::Want(0), 
            start: 1, 
            end: Some(4), 
            amount: 2.0, 
            satisfaction: 0.0, 
            reserved: 0.0, 
            step: 1, 
            tags: vec![] 
        };
        humie.desires.push(food);
        humie.desires.push(rest);

        self.species.insert(humie.id, humie);

        Ok("No Problemo".into())
    }

    /// Culture Loader function.
    /// 
    /// Currently just loads hard coded data. Will improve later.
    pub fn load_cultures(&mut self, _file_name: String) -> Result<String, String> {
        let mut normie = Culture{
            id: 0,
            name: "Normie".into(),
            variant_name: String::new(),
            desires: vec![],
            //tags: vec![],
            relations: vec![],
            productivity_modifier: 1.0,
            birth_rate_modifier: 0.02,
            death_rate_modifier: 0.01,
            //data_table: vec![],
        };

        // ambrosia preference, 
        let ambrosia = Desire{ 
            item: DesireItem::Product(2), 
            start: 10, 
            end: Some(100), 
            amount: 1.0, 
            satisfaction: 0.0, 
            reserved: 0.0, 
            step: 10, 
            tags: vec![] 
        };
        // Housing, a bigger house is always wanted.
        let housing = Desire{ 
            item: DesireItem::Want(2), 
            start: 5, 
            end: None, 
            amount: 0.5, 
            satisfaction: 0.0, 
            reserved: 0.0, 
            step: 10, 
            tags: vec![] 
        };
        // Clothing, some clothing is just needed, 2 sets is enough for clothing.
        let clothing = Desire{ 
            item: DesireItem::Want(3), 
            start: 5, 
            end: Some(20), 
            amount: 0.5, 
            satisfaction: 0.0, 
            reserved: 0.0, 
            step: 10, 
            tags: vec![] 
        };
        // Fashion, once you get enough clothes,
        // nice clothes are the next option, and you never have
        // enough.
        let fashion = Desire{ 
            item: DesireItem::Want(3), 
            start: 23, 
            end: None, 
            amount: 0.1, 
            satisfaction: 0.0, 
            reserved: 0.0, 
            step: 5, 
            tags: vec![] 
        };
        // Wealth, auspicious wealth is a good secondary
        // desire which never ends after everything else has been
        // met.
        let wealth = Desire{ 
            item: DesireItem::Want(3), 
            start: 25, 
            end: None, 
            amount: 0.5, 
            satisfaction: 0.0, 
            reserved: 0.0, 
            step: 5, 
            tags: vec![] 
        };

        normie.desires.push(ambrosia);
        normie.desires.push(housing);
        normie.desires.push(clothing);
        normie.desires.push(fashion);
        normie.desires.push(wealth);

        self.cultures.insert(normie.id, normie);

        Ok("No Problemo".into())
    }
}