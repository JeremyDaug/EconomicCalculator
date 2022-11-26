use std::fmt::format;

#[derive(Debug)]
pub struct Process {
    pub id: u64,
    pub name: String,
    pub variant_name: String,
    pub description: String,
    // icon
    pub minimum_time: f64,
    pub process_parts: Vec<ProcessPart>,
    pub process_tags: Vec<ProcessTag>,
    pub skill: Option<u64>,
    pub skill_minimum: f64,
    pub skill_maximum: f64,
    // Processes are always fractional, fractional items are handled on the product end.
    pub technology_requirement: Option<u64>,
    pub tertiary_tech: Option<u64>
}

impl Process {
    pub fn id(&self) -> u64 {
        self.id
    }

    pub fn get_name(&self) -> String {
        if self.variant_name.len() > 0 {
            return format!("{}({})", self.name, self.variant_name);
        }
        self.name.clone()
    }
}

#[derive(Debug)]
pub struct ProcessPart {
    pub item: PartItem,
    pub amount: f64,
    pub part_tags: Vec<ProcessPartTag>,
    pub part: ProcessSectionTag
}

impl ProcessPart {
}

#[derive(Debug, PartialEq, Eq)]
pub enum ProcessSectionTag {
    Input,
    Capital,
    Output
}

impl ProcessSectionTag {
    pub fn is_output(&self) -> bool {
        match self {
            ProcessSectionTag::Output => true,
            _ => false
        }
    }

    pub fn is_input(&self) -> bool {
        match self {
            ProcessSectionTag::Input => true,
            _ => false
        }
    }

    pub fn is_capital(&self) -> bool {
        match self {
            ProcessSectionTag::Capital => true,
            _ => false
        }
    }
}

#[derive(Debug)]
pub enum ProcessPartTag {
    Optional(f64),
    Fixed,
    Investment,
    Pollutant,
    Chance(char, u64)
}

#[derive(Debug)]
pub enum ProcessTag {
    Failure(u64),
    Maintenance(u64),
    Consumption(u64),
    Use(u64),
    Chance(u64),
    Crop,
    Mine,
    Extractor,
    Tap,
    Refiner,
    Sorter,
    Scrapping,
    Scrubber
}

/// Part item defines what kind of item the part requests.
/// 
/// Products or Wants.
#[derive(Debug)]
pub enum PartItem {
    /// The Part is a Product and should be treated as such.
    Product(u64),
    /// The Part is a want and should be treated as such.
    Want(u64)
}
impl PartItem {
    /// Unwraps the id in either want or product Enum.
    /// 
    /// # Note
    /// 
    /// If id's are replaced with references, this will need to be changed to 
    /// 
    /// accept the difference.
    pub fn unwrap(&self) -> u64 {
        match self {
            PartItem::Product(item) => *item,
            PartItem::Want(item) => *item,
        }
    }

    /// Checks if the item is a Want.
    pub fn is_want(&self) -> bool {
        match self {
            PartItem::Product(_) => false,
            PartItem::Want(_) => true,
        }
    }

    /// Checks if the item is a Product.
    pub fn is_product(&self) -> bool {
        match self {
            PartItem::Product(_) => true,
            PartItem::Want(_) => false,
        }
    }
}