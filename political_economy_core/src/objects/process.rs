use std::{collections::HashMap};

use super::{pop::Pop, pop_breakdown_table::PopBreakdownTable};

#[derive(Debug)]
pub struct Process {
    pub id: usize,
    pub name: String,
    pub variant_name: String,
    pub description: String,
    // icon
    pub minimum_time: f64,
    pub process_parts: Vec<ProcessPart>,
    pub process_tags: Vec<ProcessTag>,
    pub skill: Option<usize>,
    pub skill_minimum: f64,
    pub skill_maximum: f64,
    // Processes are always fractional, fractional items are handled on the product end.
    pub technology_requirement: Option<usize>,
    pub tertiary_tech: Option<usize>,
}

impl Process {
    pub fn id(&self) -> usize {
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

    /// # The Do Process Function
    /// 
    /// Do process function takes in the products and wants available for
    /// the process as well as the pop who is doing the process.
    /// 
    /// The target parameter is how many iterations it will attepmt to 
    /// complete, assuming no modifications upward, and hard_cap dictates
    /// whether it will allow it to push higher with an efficiency boost or
    /// not.
    /// 
    /// ## Returns
    /// 
    /// This function returns 3 HashMaps in a Tuple.
    /// - First is all products consumed or created by the process (inputs/outputs).
    /// - Second is all wants consumed or created by the process (inputs/outputs).
    /// - Third is all products used as capital in the process (Used Capital).
    /// 
    /// TODO Both make and Test this function.
    /// TODO Include logic for process part tags: Optional(f64), Fixed, Investment, Pollutant, Chance(char, usize)
    /// TODO pop_skill and other_efficiency_boni are currently not taken into account.
    /// TODO hard_cap is not taken into account, assumed to always be true currently.
    /// TODO Currently incapable of dealing with Capital Wants.
    /// TODO Process.minimum_time is not taken into account.
    pub fn do_process(&self, available_products: &HashMap<usize, f64>, 
        available_wants: &HashMap<usize, f64>, _pop_skill: &f64,
        _other_efficiency_boni: &f64, target: Option<f64>, _hard_cap: bool) 
        -> ProcessOutputs {
            let mut results = ProcessOutputs::new();
            // get how many cycles we can do in total
            // TODO check and take optional and fixed items into account here.
            // optional items will need to be ignored if unavailable, but add to the target of all non-fixed items
            // fixed items ignore any efficiency gains from 
            let mut ratio_available = f64::INFINITY;
            for process_part in self.process_parts.iter() {
                if let ProcessSectionTag::Capital = process_part.part {
                    // todo add capital want handling here.
                    if let PartItem::Want(id) = process_part.item {
                        continue;
                    }
                }
                if let ProcessSectionTag::Output = process_part.part { // if output, ignore
                    continue;
                }
                match process_part.item { // TODO add optional check here.
                    PartItem::Product(id) => {
                        // take lower between current ratio available and available_product / cycle_target.
                        ratio_available = ratio_available
                            .min(available_products.get(&id).unwrap_or(&0.0) / process_part.amount);
                    },
                    PartItem::Want(id) => {
                        // take lower between current ratio available and available_product / cycle_target.
                        ratio_available = ratio_available
                            .min(available_wants.get(&id).unwrap_or(&0.0) / process_part.amount);
                    },
                }
                if ratio_available == 0.0 { // if ratio is 0, gtfo, we can't do anything.
                    return ProcessOutputs::new();
                }
            }
            // cap our ratio at the target
            // TODO add check for hard_cap here.
            if let Some(val) = target { // we assume that hard_cap == true for now
                ratio_available = ratio_available.min(val);
            }

            // with our target ratio gotten, create the return results for inputs and outputs
            // TODO fixed items will also need to be taken into account here.
            for process_part in self.process_parts.iter() {
                let mut in_out_sign = 1;
                match process_part.part {
                    ProcessSectionTag::Capital => {
                        if let PartItem::Product(id) = process_part.item {
                            // add used capital products
                            results.capital_products
                            .insert(process_part.item.unwrap(), process_part.amount * ratio_available);
                        } else if let PartItem::Want(id) = process_part.item { 
                            // TODO add capital want handling here also.
                        }
                        continue;
                    },
                    ProcessSectionTag::Input => { in_out_sign = -1; }, // subtract inputs
                    ProcessSectionTag::Output => { in_out_sign = 1; }, // add outputs
                } 
                // if not capital, add to appropriate input_output
                match process_part.item {
                    PartItem::Product(id) => {
                        results.input_output_products.insert(id, process_part.amount * ratio_available);
                    },
                    PartItem::Want(id) => {
                        results.input_output_wants.insert(id, process_part.amount * ratio_available);
                    },
                }
            }

            results
        }
}

/// Helper struct used for process outputs
pub struct ProcessOutputs {
    /// The products being input and output by the process. Negative are 
    /// Inputs and Positives are Outputs.
    pub input_output_products: HashMap<usize, f64>,
    /// The wants being input and output by the process. Negatives are
    /// inputs and positives are outputs.
    pub input_output_wants: HashMap<usize, f64>,
    /// The capital products which are used.
    pub capital_products: HashMap<usize, f64>
}

impl ProcessOutputs {
    pub fn new() -> ProcessOutputs {
        ProcessOutputs{ input_output_products: HashMap::new(), 
            input_output_wants: HashMap::new(), capital_products: HashMap::new() }
    }
}

/// # Process Part
/// 
/// Process Part is an input, capital, or output of a process.
/// 
/// It contains:
/// - the item (Product or Want and it's Id)
/// - the amount
/// - the tags for this part
/// - the part of the process it goes to (input/capital/output)
#[derive(Debug)]
pub struct ProcessPart {
    /// The item of this part, may be either a product or a want.
    pub item: PartItem,
    /// The amount it takes in
    pub amount: f64,
    /// the tags for this part of the process.
    pub part_tags: Vec<ProcessPartTag>,
    /// 
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
    Chance(char, usize)
}

#[derive(Debug)]
pub enum ProcessTag {
    Failure(usize),
    Maintenance(usize),
    Consumption(usize),
    Use(usize),
    Chance(usize),
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
    Product(usize),
    /// The Part is a want and should be treated as such.
    Want(usize)
}

impl PartItem {
    /// Unwraps the id in either want or product Enum.
    /// 
    /// # Note
    /// 
    /// If id's are replaced with references, this will need to be changed to 
    /// 
    /// accept the difference.
    pub fn unwrap(&self) -> usize {
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