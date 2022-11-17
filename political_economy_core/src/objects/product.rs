use std::collections::HashMap;
use std::collections::HashSet;
use std::hash::Hash;

use super::technology::Technology;
use super::want::Want;

#[derive(Debug)]
pub struct Product {
    pub(crate) id: u64,
    pub(crate) name: String,
    pub(crate) variant_name: String,
    pub(crate) description: String,
    pub(crate) unit_name: String,
    pub(crate) quality: i32,
    pub(crate) mass: f64,
    pub(crate) bulk: f64,
    pub(crate) mean_time_to_failure: Option<u32>,
    pub(crate) fractional: bool,
    // icon
    pub(crate) tags: HashSet<ProductTag>,
    pub(crate) wants: HashMap<Want, f64>,
    //Processes: HashSet<Process>,
        // failure
        // use
        // consume
        // maintenance
    pub(crate) tech_required: Technology
}

#[derive(Debug)]
pub enum ProductTag {
    SelfLuxury(f64),
    Luxury(),
    Public,
    Consumable,
    ConsumerGood,
    MilitaryGood,
    Fixed,
    Currency,
    Service,
    Remote
}

impl Hash for Product {
    fn hash<H: std::hash::Hasher>(&self, state: &mut H) {
        self.id.hash(state);
    }
}

impl Eq for Product {
    fn assert_receiver_is_total_eq(&self) {}
}

impl PartialEq for Product {
    fn eq(&self, other: &Self) -> bool {
        self.id == other.id
    }
}

impl Product {
    pub fn is_equal_to(&self, other: Product) -> bool{
        if self.id != other.id ||
           self.name != other.name ||
           self.variant_name != other.variant_name ||
           self.description != other.description ||
           self.unit_name != other.unit_name ||
           self.quality != other.quality ||
           self.mass != other.mass ||
           self.bulk != other.bulk || 
           self.mean_time_to_failure != other.mean_time_to_failure ||
           self.fractional != other.fractional 
        {
            return true;
        }

        if self.tags.len() != other.tags.len() {
            return false;
        }
        todo!("Pick up here!");
        self.tags == other.tags && 
        self.wants == other.wants && 
        self.tech_required == other.tech_required
    }

    pub fn get_name(&self) -> String {
        String::from("{self.name}({self.variant_name})")
    }
}