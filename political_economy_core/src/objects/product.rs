use std::collections::HashMap;
use std::collections::HashSet;
use std::hash::Hash;

use crate::data_manager::DataManager;

use super::firm::Firm;
use super::process::Process;
use super::technology::Technology;
use super::want::Want;

#[derive(Debug)]
pub struct Product {
    id: u64,
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
    pub(crate) tags: Vec<ProductTag>,
    pub(crate) wants: HashMap<u64, f64>,

    pub(crate) processes: HashSet<u64>,
    pub(crate) failure_process: Option<u64>,
    pub(crate) use_processes: HashSet<u64>,
    pub(crate) consumption_processes: HashSet<u64>,
    pub(crate) maintenance_processes: HashSet<u64>,

    pub(crate) tech_required: Option<u64>
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
    pub fn new(id: u64,
         name: String, 
         variant_name: String, 
         description: String, 
         unit_name: String, 
         quality: i32, 
         mass: f64, 
         bulk: f64, 
         mean_time_to_failure: Option<u32>, 
         fractional: bool, 
         tags: Vec<ProductTag>, 
         wants: HashMap<u64, f64>, 
         processes: HashSet<u64>, 
         failure_process: Option<u64>, 
         use_processes: HashSet<u64>, 
         consumption_processes: HashSet<u64>, 
         maintenance_processes: HashSet<u64>, 
         tech_required: Option<u64>) -> Option<Self> {
            if !tags.contains(&ProductTag::Magic) &&
                (mass < 0.0 || bulk < 0.0) {
                    return None
            }
             Some(Self { 
                id, 
                name, 
                variant_name, 
                description, 
                unit_name, 
                quality, 
                mass, 
                bulk, 
                mean_time_to_failure, 
                fractional, 
                tags, 
                wants, 
                processes, 
                failure_process, 
                use_processes, 
                consumption_processes, 
                maintenance_processes, 
                tech_required 
            } )
        }

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
            return false;
        }
        true
    }

    pub fn get_name(&self) -> String {
        String::from("{self.name}({self.variant_name})")
    }

    pub fn id(&self) -> u64 {
        self.id
    }
}

#[derive(Debug, PartialEq)]
pub enum ProductTag {
    /// The item has improved efficiency at satisfying itself as it's price increases.
    SelfLuxury{efficiency: f64},
    /// The item has improved efficiency at satisfying a want as it's price increases.
    WantLuxury{want: u64, efficiency: f64},
    /// The item has improved efficiency at satisfying a different product as it's price increases.
    ProductLuxury{product: u64, efficiency: f64},
    /// The item has improved efficiency at satisfying itself as it's price decreases.
    SelfBargain{efficiency: f64},
    /// The item has improved efficiency at satisfying a want as it's price decreases.
    WantBargain{want: u64, efficiency: f64},
    /// The item has improved efficiency at satisfying a different product as it's price decreases.
    ProductBargain{product: u64, efficiency: f64},
    /// The item is a public good, the value it contains is how many people it can be shared
    /// between without losing efficiency. Decay is how quickly it falls per person after it
    /// maxes out.
    Public{capacity: u64, decay: f64},
    // The product isn't an item, but a claim on another item, which is being rented out.
    // This is being shifted on hold for later reasons.
    //Claim{product: u64, renter: Pop},
    /// The product is a share of ownership in a company. The company manages the details of 
    /// the share's effects on the firms
    Share{firm: u64},
    /// The item is actually a living creature (placed her for later purposes.)
    Animate,
    /// The item is a consumer good, making variants is more open ended, but the product
    /// cannot be used as an input in a process. Only Failure processes are allowed.
    ConsumerGood,
    /// The item is a military good, this gives it more flexibility in variation, but removes
    /// it as an input or capital processes. The only processes it allows are failure processes.
    MilitaryGood,
    /// The item is immobile, it can only be traded within a market or territory, and cannot
    /// be transferred out of a market or territory.
    Fixed,
    /// The item is a currency, marking it as the preferred item to use to buy or sell items.
    Currency,
    /// The item is a service, it fails each day, turns into nothing, and cannot be transferred
    /// outside of the market.
    Service,
    /// The item is a service, but can be sold outside of the market via communication methods.
    Remote,
    /// The item cannot be made distinguishable from any other and cannot have variants.
    Invariant,
    /// The item is an abstract ideal, it can have vairants, but not a 'default'.
    Abstract,
    /// The item is a liquid, requiring special storage to keep. It also acts as a liquid once it
    /// enters the environment.
    Liquid,
    /// The item is a gas, requiring special storage to keep. It also acts as a gas once
    /// it enters the environment.
    Gas,
    /// The item is a pure atomic material and is thus available for matter smasher stuff.
    Atomic{protons: u32, neutrons: u32},
    /// The item can store other items in it. It defines what kind of storage it offers,
    /// and the amount it can store interms of mass and valume.
    /// Storage negates or reduces degredation effects from the environment and vulnerability.
    Storage{storage_type: StorageType, mass: f64, volume: f64},
    /// The product has particular vulnerabilities, 
    Vulnerable{to: ProductVulnerability},
    /// The item is magical, and thus can break our normal rules and requirements for an item.
    Magic
}

/// The possible Storage Types available.
#[derive(Debug, PartialEq)]
pub enum StorageType {
    /// Standard Storage, no special abilities, stops trivial theft.
    Standard,
    /// Wet Storage, holds liquids.
    Wet,
    /// Gas Storage, holds gases
    Gas,
    /// Dry storage, negates Moisture damage.
    Dry,
    /// Cold Storage, negates heat and rot damage.
    Cold
}

#[derive(Debug, PartialEq)]
pub enum ProductVulnerability {
    /// Moisture vulnerability, the item decays faster if it in humid environments or if wet.
    Moisture,
    /// Rot Vulnerability, the item rots if not stored well.
    Rot,
    /// Heat Vulnerability, the item doesn't handle heat well and must be kept in cold storage.
    Heat
}