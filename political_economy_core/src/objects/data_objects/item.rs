use std::fmt::Display;

/// # Item Enum
/// 
/// A Common storage enum for products, classes, and wants so that we can 
/// pass it around more nicely.
#[derive(Debug, PartialEq, Eq, Clone, Hash, Copy)]
pub enum Item {
    /// A desire for a want (Food). A generic, abstract thing that is 
    /// often tied to many things, but is nothing specific.
    Want(usize),
    /// A desire for a class of good (Bread). A generic, but otherwise 
    /// concrete thing which can have many kinds contined within. Products
    /// may or may not be variants of the same product.
    /// 
    /// # Note
    /// 
    /// NEVER assume products within the same class have the same bulk or
    /// mass.
    /// 
    /// When used as an input to a process, it is treated like a product
    /// and just cosumed by the process. To consume for a specific variant
    /// of product, apply ProcessPartTag::Consumption.
    Class(usize),
    /// A desire for a specific good (Wonderbread).
    Product(usize),
}

impl Display for Item {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        write!(f, "{:?}", self)
    }
}

/// Defines what 
impl Item {
    /// unwraps the value from a DesireItem..
    pub fn unwrap(&self) -> usize {
        match self {
            Item::Product(prod) => *prod,
            Item::Want(want) => *want,
            Item::Class(prod) => *prod,
        }
    }

    /// Checks if the item is a Want.
    pub fn is_want(&self) -> bool {
        match self {
            Item::Want(_) => true,
            _ => false
        }
    }

    /// Checks if the item is a Product.
    pub fn is_product(&self) -> bool {
        match self {
            Item::Product(_) => true,
            _ => false,
        }
    }

    pub fn is_class(&self) -> bool {
        match self {
            Item::Class(_) => true,
            _ => false
        }
    }

    /// Checks if this is a specific product.
    pub fn is_this_specific_product(&self, item: &usize) -> bool {
        match self {
            Item::Product(val) => val == item,
            _ => false
        }
    }

    /// Checks if this is a specific want.
    pub fn is_this_want(&self, item: &usize) -> bool {
        match self {
            Item::Want(val) => val == item,
            _ => false
        }
    }
}