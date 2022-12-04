use core::num::dec2flt::float;


/// Desires
/// 
/// Desires are things that are desired and used in a the Desires class.
/// It contains an item, either a want or product, and at minimum a tier
/// at which it applies.
#[derive(Debug)]
pub struct Desire {
    pub item: DesireItem,
    pub start: u64,
    pub end: Option<u64>,
    pub amount: f64,
    pub satisfaction: f64,
    pub reserved: f64,
    pub step: u64,
    pub tags: Vec<DesireTag>
}

impl Desire {
    pub fn steps(&self) -> u64 {
        if self.end.is_none() {
            return 0;
        }
        (self.end.unwrap() - self.start) / self.step + 1
    }

    /// If the Desire covers more than 1 tier.
    pub fn is_stretched(&self) -> bool {
        self.step > 0
    }

    /// If the desire is infinite, with no end tier.
    pub fn is_infinite(&self) -> bool {
        self.is_stretched() && self.end.is_none()
    }


    pub fn total_desire_at_tier(&self, tier: &u64) -> f64 {
        // if tier is below starting tier
        if tier < &self.start {
            return 0.0;
        }

        // if it isn't stretched and we're at or above the
        // start
        if !self.is_stretched() {
            return self.amount;
        }

        // get the steps up to the given tier, +1
        let current_steps = (tier - self.start) / self.step + 1;
        // if current steps after last 
        if self.end.is_some() && self.end.unwrap() < *tier {
            return self.amount * (self.steps() as f64);
        }
        (current_steps as f64) * self.amount
    }
}

/// The tags a desire can be marked by, modifying how the desire is viewed.
/// Many of these alter how they are treated, removing them from common
/// calculations.
#[derive(Debug)]
pub enum DesireTag{
    /// The person does not actually desire this item, if this item is 
    /// satisfied, it instead increases their chance of sickness and death.
    Toxic,
    /// The desire is not needed every day, instead they only desire it
    /// occasionally. How likely they are on any given ady is equal to
    /// the value inside it.
    Sporadic(u64),
    /// The item is not needed consistently, but instead periodically and
    /// consistently. The value given is how often they need 
    Periodic { offset: u64, cycle: u64},
}


#[derive(Debug)]
pub enum DesireItem {
    Want(usize),
    Product(usize)
}

/// Defines what 
impl DesireItem {

    pub fn unwrap(&self) -> &usize {
        match self {
            DesireItem::Product(prod) => prod,
            DesireItem::Want(want) => want
        }
    }

    /// Checks if the item is a Want.
    pub fn is_want(&self) -> bool {
        match self {
            DesireItem::Product(_) => false,
            DesireItem::Want(_) => true,
        }
    }

    /// Checks if the item is a Product.
    pub fn is_product(&self) -> bool {
        match self {
            DesireItem::Product(_) => true,
            DesireItem::Want(_) => false,
        }
    }
}