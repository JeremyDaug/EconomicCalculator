use std::{fmt::format, collections::HashMap, os::linux::process};

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
    pub tertiary_tech: Option<u64>,
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

    /// Checks if the Process takes output from the other process as input
    /// 
    /// Returns true if the process this is called on takes any of the outputs of 'other'.
    /// 
    /// Returns false otherwise.
    pub fn takes_input_from(&self, other: &Process) -> bool {
        let inputs = self.process_parts.iter().filter(|x| x.part.is_input());
        let mut outputs = other.process_parts.iter().filter(|x| x.part.is_output());

        for input in inputs {
            if outputs.any(|x| x.item == input.item) {
                return true;
            }
        }

        false
    }

    /// Checks if the Process takes output from the other process as capital
    /// 
    /// Wants are not checked as they have special rules as capital.
    /// 
    /// Returns true if the process this is called on takes any of the outputs of 'other'.
    /// 
    /// Returns false otherwise.
    pub fn takes_capital_from(&self, other: &Process) -> bool {
        // check that the product is a capital and a product (no want capitals handled yet)
        let capitals =
            self.process_parts.iter().filter(|x| x.part.is_capital() &&
                        x.item.is_product());
        let mut outputs 
            = other.process_parts.iter().filter(|x| x.part.is_output());

        for capital in capitals {
            if outputs.any(|x| x.item == capital.item) {
                return true;
            }
        }

        false
    }

    /// Checks if the Process outputs to the other process input
    /// 
    /// Returns true if the process this is called on takes any of the outputs of 'other'.
    /// 
    /// Returns false otherwise.
    pub fn gives_output_to_others_input(&self, other: &Process) -> bool {
        let outputs =
            self.process_parts.iter().filter(|x| x.part.is_output());
        // check against all inputs and capital products (not capital wants)
        let mut inputs 
            = other.process_parts.iter().filter(|x| x.part.is_input());

        for output in outputs {
            if inputs.any(|x| x.item == output.item) {
                return true;
            }
        }

        false
    }

    /// Checks if the Process outputs to the other process capital
    /// 
    /// Returns true if the process this is called on takes any of the outputs of 'other'.
    /// 
    /// Returns false otherwise.
    pub fn gives_output_to_others_capital(&self, other: &Process) -> bool {
        let outputs =
            self.process_parts.iter().filter(|x| x.part.is_output());
        // check against all inputs and capital products (not capital wants)
        let mut inputs 
            = other.process_parts.iter().filter(|x| x.part.is_capital() && x.item.is_product());

        for output in outputs {
            if inputs.any(|x| x.item == output.item) {
                return true;
            }
        }

        false
    }

    /// Checks if a process can feed itself.
    /// 
    /// If two items in our parts are the same, we check them.
    /// 
    /// If they are products and at least one is an input or capital and another is an output 
    /// then it returns true.
    /// 
    /// If they are wants and at least one is an input and another is an output, then we
    /// return true. Capital wants are not considered valid as they are a special condition.
    pub fn can_feed_self(&self) -> bool {
        let mut checked_products = HashMap::new();
        let mut checked_wants = HashMap::new();
        for part in self.process_parts.iter() {
            match part.item {
                PartItem::Product(prod) => {
                    let count = checked_products.entry(prod).or_insert(vec![]);
                    count.push(part);
                }
                PartItem::Want(want) => {
                    let count = checked_wants.entry(want).or_insert(vec![]);
                    count.push(part);
                }
            }
        }
        if checked_products.iter().any(|x| x.1.len() > 1) {
            for (_, possible) in checked_products.iter().filter(|x| x.1.len() > 1) {
                if possible.iter().any(|x| !x.part.is_output()) &&
                   possible.iter().any(|x| x.part.is_output()) {
                    // if any of them are not outputs and any other is also an output, then we can feed ourself.
                    return true; 
                }
            }
        }
        if checked_wants.iter().any(|x| x.1.len() > 1) {
            for (_, possible) in checked_wants.iter().filter(|x| x.1.len() > 1) {
                if possible.iter().any(|x| x.part.is_input()) &&
                   possible.iter().any(|x| x.part.is_output()) {
                    // if any want is both an input and an output, then we can feed orself
                    // capital wants are special and should not be checked.
                    return true;
                }
            }
        }
        false
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
#[derive(Debug, PartialEq, Eq)]
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