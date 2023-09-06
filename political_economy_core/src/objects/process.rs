///! Processes transform Products and Wants into other products and wants.
use std::collections::{HashMap, HashSet};

use itertools::Itertools;

use crate::data_manager::DataManager;

use super::property_info::PropertyInfo;

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

    /// # Inputs
    /// 
    /// Gets the process's inputs
    pub fn inputs(&self) -> Vec<&ProcessPart> {
        self.process_parts.iter().filter(|x| x.part.is_input()).collect_vec()
    }

    /// # Input Products
    /// 
    /// Gets the process's input products
    pub fn input_products(&self) -> Vec<&ProcessPart> {
        self.process_parts.iter().filter(|x| x.part.is_input() && x.item.is_specific()).collect_vec()
    }

    /// # Inputs and Capital
    /// 
    /// Gets the process's inputs and Capital
    pub fn inputs_and_capital(&self) -> Vec<&ProcessPart> {
        self.process_parts.iter().filter(|x| !x.part.is_output()).collect_vec()
    }

    /// # Input and Capital Products
    /// 
    /// Gets the process's input and capital products
    pub fn input_and_capital_products(&self) -> Vec<&ProcessPart> {
        self.process_parts.iter().filter(|x| !x.part.is_output() && x.item.is_specific()).collect_vec()
    }

    /// # Capital
    /// 
    /// Gets the process's capital
    pub fn capital(&self) -> Vec<&ProcessPart> {
        self.process_parts.iter().filter(|x| x.part.is_capital()).collect_vec()
    }

    /// # Capital Products
    /// 
    /// Gets the process's capital products
    pub fn capital_products(&self) -> Vec<&ProcessPart> {
        self.process_parts.iter().filter(|x| x.part.is_capital() && x.item.is_specific()).collect_vec()
    }

    /// # Outputs
    /// 
    /// Gets the process's output
    pub fn outputs(&self) -> Vec<&ProcessPart> {
        self.process_parts.iter().filter(|x| x.part.is_output()).collect_vec()
    }

    /// # Output Products
    /// 
    /// Gets the process's output products
    pub fn output_products(&self) -> Vec<&ProcessPart> {
        self.process_parts.iter().filter(|x| x.part.is_output() && x.item.is_specific()).collect_vec()
    }

    /// # Outputs Proudct
    /// 
    /// Check if this process outputs a particular product in any way shape or form.
    /// 
    /// Returns true if any output is the product, false otherwise.
    pub fn outputs_product(&self, product: usize) -> bool {
        self.process_parts.iter()
        .filter(|x| x.part.is_output() && x.item.is_specific())
        .any(|x| x.item.unwrap() == product)
    }

    /// # Maintenance Product
    /// 
    /// If this product is a Maintenance process, it get's the product. Otherwise, None.
    pub fn maintenance_product(&self) -> Option<usize> {
        for tag in self.process_tags.iter() {
            if let ProcessTag::Maintenance(id) = tag {
                return Some(*id);
            }
        }
        None
    }

    /// # Failure Product
    /// 
    /// If this product is a Failure process, it get's the product. Otherwise, None.
    pub fn failure_product(&self) -> Option<usize> {
        for tag in self.process_tags.iter() {
            if let ProcessTag::Failure(id) = tag {
                return Some(*id);
            }
        }
        None
    }

    /// # Consumed Product
    /// 
    /// If this product is a Consumption process, it get's the product. Otherwise, None.
    pub fn consumed_product(&self) -> Option<usize> {
        for tag in self.process_tags.iter() {
            if let ProcessTag::Consumption(id) = tag {
                return Some(*id);
            }
        }
        None
    }

    /// # Used Product
    /// 
    /// If this product is a use process, it get's the product. Otherwise, None.
    pub fn used_product(&self) -> Option<usize> {
        for tag in self.process_tags.iter() {
            if let ProcessTag::Use(id) = tag {
                return Some(*id);
            }
        }
        None
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
                        x.item.is_specific());
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
            = other.process_parts.iter().filter(|x| x.part.is_capital() && x.item.is_specific());

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
    pub fn can_feed_self(&self, data: &DataManager) -> bool {
        let mut checked_products = HashMap::new();
        let mut checked_wants = HashMap::new();
        let mut checked_classes = HashSet::new();
        for part in self.process_parts.iter() {
            match part.item {
                PartItem::Specific(prod) => {
                    let count = checked_products.entry(prod).or_insert(vec![]);
                    count.push(part);
                }
                PartItem::Want(want) => {
                    let count = checked_wants.entry(want).or_insert(vec![]);
                    count.push(part);
                }
                PartItem::Class(class) => {
                    checked_classes.insert(class);
                },
            }
        }
        // check specific products
        if checked_products.iter().any(|x| x.1.len() > 1) {
            for (_, possible) in checked_products.iter().filter(|x| x.1.len() > 1) {
                if possible.iter().any(|x| !x.part.is_output()) &&
                   possible.iter().any(|x| x.part.is_output()) {
                    // if any of them are not outputs and any other is also an output, then we can feed ourself.
                    return true; 
                }
            }
        }
        // check specific wants.
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
        // check classes
        // Class must be an input or capital, so only check output products.
        if self.process_parts.iter()
        // only get specific outputs
        .filter(|&x| x.part.is_output() && x.item.is_specific())
        // if any of them are in one of our classes, return true.
        .any(|x| {
            if let Some(product_class) = data.get_product_class(x.item.unwrap()) {
                checked_classes.contains(&product_class) } else { false }}) 
            {
                return true;
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
    /// ## Notes
    /// 
    /// Class Items are only valid as Capital or Input, not as output. Class 
    /// Items are assumed to be 'materially equvialent' meaning that they are
    /// made of the same materials in the same ratio and have the same mass.
    /// Note, this is an assumption, not a hard rule, so may be ignored.
    /// The 'Consumed' tag may help to sidestep this issue.
    /// 
    /// Currently it's missing many parts, it takes no tags into account and does not handle duplicate 
    /// input/capitals or overlap between specific products and that product's class.
    /// 
    /// Use with caution.
    /// 
    /// TODO Include logic for process part tags: Consumption, Optional(f64), Fixed, Investment, Pollutant, Chance(char, usize)
    /// 
    /// TODO pop_skill and other_efficiency_boni are currently not taken into account.
    /// 
    /// TODO hard_cap is not taken into account, assumed to always be true currently.
    /// 
    /// TODO Currently incapable of dealing with Capital Wants.
    /// 
    /// TODO Process.minimum_time is not taken into account.
    /// 
    /// TODO Look into making a companion / better function which generates functions instead. May be faster in long run.
    /// 
    /// FIXME Does not handle Overlapping or duplicate products at all. Do Not Use Overlapping Classes or Duplicate Products
    pub fn do_process(&self, available_products: &HashMap<usize, f64>, 
    available_wants: &HashMap<usize, f64>, _pop_skill: f64,
    _other_efficiency_boni: f64, target: Option<f64>, _hard_cap: bool, data: &DataManager) 
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
                if let PartItem::Want(_id) = process_part.item {
                    continue;
                }
            }
            if let ProcessSectionTag::Output = process_part.part { // if output, ignore
                continue;
            }
            match process_part.item { // TODO add optional check here.
                PartItem::Specific(id) => {
                    // take lower between current ratio available and available_product / cycle_target.
                    ratio_available = ratio_available
                        .min(available_products.get(&id).unwrap_or(&0.0) / process_part.amount);
                },
                PartItem::Want(id) => {
                    // take lower between current ratio available and available_product / cycle_target.
                    ratio_available = ratio_available
                        .min(available_wants.get(&id).unwrap_or(&0.0) / process_part.amount);
                },
                PartItem::Class(id) => {
                    // TODO add quality check here!
                    // TODO add way to check against repeat products here.
                    // get all possible items of the class in our products
                    let class_mates = available_products.iter()
                    .filter(|(&prod_id, _)| {
                        let class = data.get_product_class(prod_id);
                        if let Some(val) = class {
                            id == val
                        } else { false }
                    });
                    // how many items within the class we have,
                    let sum: f64 = class_mates.map(|x| x.1).sum();
                    ratio_available = ratio_available.min(sum / process_part.amount);
                }
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
        // without efficiency gains, our ratio available == possible iteratons.
        results.iterations = ratio_available;
        // efficiency is equal to the total possible gain we were able to achieved.
        results.efficiency = 1.0;
        // effective iterations is our iterations after applying efficiency gains.
        results.effective_iterations = ratio_available;

        // with our target ratio gotten, create the return results for inputs and outputs
        // TODO fixed items will also need to be taken into account here.
        for process_part in self.process_parts.iter() {
            let mut _in_out_sign = 1.0;
            match process_part.part {
                ProcessSectionTag::Capital => {
                    if let PartItem::Specific(_id) = process_part.item {
                        // add used capital products
                        results.capital_products
                        .insert(process_part.item.unwrap(), process_part.amount * ratio_available);
                    } else if let PartItem::Want(_id) = process_part.item { 
                        // TODO add capital want handling here also.
                    }
                    continue;
                },
                ProcessSectionTag::Input => { _in_out_sign = -1.0; }, // subtract inputs
                ProcessSectionTag::Output => { _in_out_sign = 1.0; }, // add outputs
            } 
            // if not capital, add to appropriate input_output
            match process_part.item {
                PartItem::Specific(id) => {
                    results.input_output_products.entry(id)
                    .and_modify(|x| *x += _in_out_sign * process_part.amount * ratio_available)
                    .or_insert(_in_out_sign * process_part.amount * ratio_available);
                },
                PartItem::Want(id) => {
                    results.input_output_wants.entry(id)
                    .and_modify(|x| *x += _in_out_sign * process_part.amount * ratio_available)
                    .or_insert(_in_out_sign * process_part.amount * ratio_available);
                },
                PartItem::Class(id) => { // TODO test this part of the code!
                    debug_assert!(process_part.part.is_output(), "Class cannot be an output.");
                    // TODO improve this to deal with overlap and quality management.
                    // get the plass products
                    let class_mates = available_products.iter()
                    .filter(|(&prod_id, _)| {
                        let class = data.get_product_class(prod_id);
                        if let Some(val) = class {
                            id == val
                        } else { false }
                    });
                    // get items up to our needs
                    let mut target = process_part.amount * ratio_available;
                    for (&product_id, &quantity) in class_mates {
                        let remove = quantity.min(target);
                        results.input_output_products.entry(product_id)
                        .and_modify(|x| *x -= remove).or_insert(-remove);
                        target -= remove;
                        if target == 0.0 { break; }
                    }
                },
            }
        }

        results
    }

    /// # The Do Process With Property
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
    /// Returns a ProcessOutputs item, which contains all changes the process would make if applied
    /// as well as some useful information about the process in general.
    /// 
    /// ## Notes
    /// 
    /// This is the same as do_process, but it takes in a hashmap of PropertyInfo instead of f64s.
    /// 
    /// Class Items are only valid as Capital or Input, not as output. Class 
    /// Items are assumed to be 'materially equvialent' meaning that they are
    /// made of the same materials in the same ratio and have the same mass.
    /// Note, this is an assumption, not a hard rule, so may be ignored.
    /// The 'Consumed' tag may help to sidestep this issue.
    /// 
    /// Currently it's missing many parts, it takes no tags into account and does not handle duplicate 
    /// input/capitals or overlap between specific products and that product's class.
    /// 
    /// Use with caution.
    /// 
    /// TODO Include logic for process part tags: Consumption, Optional(f64), Fixed, Investment, Pollutant, Chance(char, usize)
    /// 
    /// TODO pop_skill and other_efficiency_boni are currently not taken into account.
    /// 
    /// TODO hard_cap is not taken into account, assumed to always be true currently.
    /// 
    /// TODO Currently incapable of dealing with Capital Wants.
    /// 
    /// TODO Process.minimum_time is not taken into account.
    /// 
    /// TODO Look into making a companion / better function which generates functions instead. May be faster in long run.
    /// 
    /// FIXME Does not handle Overlapping or duplicate products at all. Do Not Use Overlapping Classes or Duplicate Products
    pub fn do_process_with_property(&self, available_products: &HashMap<usize, PropertyInfo>, 
    available_wants: &HashMap<usize, f64>, _pop_skill: f64,
    _other_efficiency_boni: f64, target: Option<f64>, _hard_cap: bool, data: &DataManager) 
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
                if let PartItem::Want(_id) = process_part.item {
                    continue;
                }
            }
            if let ProcessSectionTag::Output = process_part.part { // if output, ignore
                continue;
            }
            match process_part.item { // TODO add optional check here.
                PartItem::Specific(id) => {
                    // take lower between current ratio available and available_product / cycle_target.
                    if let Some(prod_info) = available_products.get(&id) {
                        ratio_available = ratio_available
                            .min(prod_info.available_for_want() / process_part.amount);
                    } else { // if we don't have that item in property, return to 0.0
                        ratio_available = 0.0;
                    }
                },
                PartItem::Want(id) => {
                    // take lower between current ratio available and available_product / cycle_target.
                    ratio_available = ratio_available
                        .min(available_wants.get(&id).unwrap_or(&0.0) / process_part.amount);
                },
                PartItem::Class(id) => {
                    // TODO add quality check here!
                    // TODO add way to check against repeat products here.
                    // get all possible items of the class in our products
                    let class_mates = available_products.iter()
                    .filter(|(&prod_id, _)| {
                        let class = data.get_product_class(prod_id);
                        if let Some(val) = class {
                            id == val
                        } else { false }
                    });
                    // how many items within the class we have,
                    let sum: f64 = class_mates.map(|x| x.1.available_for_want()).sum();
                    ratio_available = ratio_available.min(sum / process_part.amount);
                }
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
        // without efficiency gains, our ratio available == possible iteratons.
        results.iterations = ratio_available;
        // efficiency is equal to the total possible gain we were able to achieved.
        results.efficiency = 1.0;
        // effective iterations is our iterations after applying efficiency gains.
        results.effective_iterations = ratio_available;

        // with our target ratio gotten, create the return results for inputs and outputs
        // TODO fixed items will also need to be taken into account here.
        for process_part in self.process_parts.iter() {
            let mut _in_out_sign = 1.0;
            match process_part.part {
                ProcessSectionTag::Capital => {
                    if let PartItem::Specific(_id) = process_part.item {
                        // add used capital products
                        results.capital_products
                        .insert(process_part.item.unwrap(), process_part.amount * ratio_available);
                    } else if let PartItem::Want(_id) = process_part.item { 
                        // TODO add capital want handling here also.
                    }
                    continue;
                },
                ProcessSectionTag::Input => { _in_out_sign = -1.0; }, // subtract inputs
                ProcessSectionTag::Output => { _in_out_sign = 1.0; }, // add outputs
            } 
            // if not capital, add to appropriate input_output
            match process_part.item {
                PartItem::Specific(id) => {
                    results.input_output_products.entry(id)
                    .and_modify(|x| *x += _in_out_sign * process_part.amount * ratio_available)
                    .or_insert(_in_out_sign * process_part.amount * ratio_available);
                },
                PartItem::Want(id) => {
                    results.input_output_wants.entry(id)
                    .and_modify(|x| *x += _in_out_sign * process_part.amount * ratio_available)
                    .or_insert(_in_out_sign * process_part.amount * ratio_available);
                },
                PartItem::Class(id) => { // TODO test this part of the code!
                    debug_assert!(process_part.part.is_output(), "Class cannot be an output.");
                    // TODO improve this to deal with overlap and quality management.
                    // get the class products
                    let class_mates = available_products.iter()
                    .filter(|(&prod_id, _)| {
                        let class = data.get_product_class(prod_id);
                        if let Some(val) = class {
                            id == val
                        } else { false }
                    });
                    // get items up to our needs
                    let mut target = process_part.amount * ratio_available;
                    for (&product_id, &quantity) in class_mates {
                        let remove = quantity.available_for_want().min(target);
                        results.input_output_products.entry(product_id)
                        .and_modify(|x| *x -= remove).or_insert(-remove);
                        target -= remove;
                        if target == 0.0 { break; }
                    }
                },
            }
        }

        results
    }

    /// # Effective Output Of 
    /// 
    /// This is a helper function, given an item, it calculates how much of 
    /// that itmes is produced on average per cycle.
    /// 
    /// If it does not produce that item, then it returns 0.0.
    /// 
    /// TODO add in logic to handle chance outputs when chance is created.
    /// 
    /// TODO when optional inputs are available, return a min and max instead of just 1 value.
    pub fn effective_output_of(&self, item: PartItem) -> f64 {
        let outputs = self.process_parts.iter()
            .filter(|x| x.part.is_output() && x.item == item).collect_vec();
        // since we don't have chance products yet, just sum them together.
        outputs.iter().map(|x| x.amount).sum()
    }

    /// # Uses Product
    /// 
    /// Checks that a given product is accepted as an input or capital for 
    /// this process returns true if it is, either specifically, or as a 
    /// member in a class.
    pub fn uses_product(&self, product: usize, 
    data: &crate::data_manager::DataManager) -> bool {
        let process_class = data.get_product_class(product);
        // split between having a class and not having a class.
        if let Some(class) = process_class {
            // If product has a class, check both specific and class.
            return self.process_parts.iter()
            .filter(|x| !x.part.is_output())
            .any(|x| {
                (x.item.is_specific() && x.item.unwrap() == product) || 
                (x.item.is_class() && x.item.unwrap() == class)
            });
        } else {
            // if product does not have a class, just look at the specifics.
            return self.process_parts.iter()
            .filter(|x| !x.part.is_output())
            .any(|x| x.item.is_specific() && x.item.unwrap() == product);
        }
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
    /// The capital products which are used. The values here are positive.
    pub capital_products: HashMap<usize, f64>,
    /// How many iterations the process was able to complete in total.
    /// 
    /// IE Iterations * any fixed product in the process == that product in our 
    /// output tables.
    pub iterations: f64,
    /// The efficiency of the process, ie what our total multiplier we had for 
    /// our processes.
    pub efficiency: f64,
    /// How many iterations were were able to effectively do. IE our iterations
    /// after efficiency gains.
    /// 
    /// IE eff_iterations * non-fixed product = our result
    pub effective_iterations: f64,
}

impl ProcessOutputs {
    pub fn new() -> ProcessOutputs {
        ProcessOutputs{ input_output_products: HashMap::new(), 
            input_output_wants: HashMap::new(), 
            capital_products: HashMap::new(),
            iterations: 0.0,
            efficiency: 1.0,
            effective_iterations:0.0,
            }
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
    /// The part of the process this is involved in, Input/Capital/Output.
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

/// # Process Part Tags
/// 
/// An enum to store data about process items.
#[derive(Debug)]
pub enum ProcessPartTag {
    /// Used to mark an input or capital as optional.
    /// The value stored is the throughput gain for all other items.
    /// Does not effect optional, fixed, or investment parts.
    /// Input items marked optional are not output, but instead consumed or 
    /// failed instead.
    /// 
    /// ## Applicable to:
    /// - Inputs
    /// - Capital
    Optional(f64),
    /// Marks an item as Cosumed, rather than destroyed by the process.
    /// Used particularly for items which don't directly go into the end
    /// product, but are still used to create the end product. IE, making 
    /// steel requires using a catalyst to remove the impurities.
    /// 
    /// ## Applicable to:
    /// - Inputs
    Consumed,
    /// Used to mark an input, capital, or output as fixed. it does not get
    /// effected by any throughput bonuses.
    /// 
    /// ## Applicable to:
    /// - Inputs
    /// - Capital
    /// - Outputs
    Fixed,
    /// Marks an input for a process as being required in totality for the 
    /// process to start.
    /// 
    /// TODO needs more work and thought put into this.
    /// 
    /// ## Applicable to:
    /// - Inputs
    /// - Capital
    Investment,
    /// Marks an output as a pollutant, causing it to be thrown into the
    /// environment directly unless captured.
    /// 
    /// ## Applicable to:
    /// - Outputs
    Pollutant,
    /// Marks an output as being a possible output rather than guaranteed.
    /// Should not be alone, but have alternatives generally.
    /// The char is the probability group it's in.
    /// The usize in the weight chance of this item occuring.
    /// Items which share a group add their weights together.
    /// 
    /// ## Applicable to:
    /// - Outputs
    Chance(char, usize),
    /// Marks an input or captial as gaining efficiency based on
    /// the quality of the item in question. Should typically be used only for
    /// Class products with valid variations.
    /// 
    /// The value included is the throughput gain for the process per level of
    /// the item's quality.
    /// 
    /// This does not effect Optional products, nor is this effected by optional.
    /// 
    /// ## Applicable to:
    /// - Inputs
    /// - Capital
    QualityBased(f64)
}

#[derive(Debug)]
pub enum ProcessTag {
    /// A Failure process for the given product, Should have only 1 input, 
    /// the product connected, and only take 1 unit of it.
    Failure(usize),
    /// A maintenance process for the product contained.
    /// Must have 1 unit as input. May have a unit as output or an improved variant as output.
    /// This output may be a chance.
    Maintenance(usize),
    /// A consumption process for the product contained.
    /// Should take 1.0 unit of the product as input and only time as a second input.
    /// May have optional (Complementary) goods attached to it.
    Consumption(usize),
    /// A use for a product contained.
    /// Should take 1.0 unit of the product as a capital, and only time as an input.
    /// May have optional goods attached to it.
    Use(usize),
    /// This is a chance process
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
    /// This cannot be used with QualityBased as this would require allowing 
    /// alterantive products.
    /// 
    /// TODO this restriction may be changed with the introduction of a packet system.
    Specific(usize),
    /// The part is a class of product, and any product within this class can function.
    /// 
    /// # Not for use as Output!!
    Class(usize),
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
            PartItem::Specific(item) => *item,
            PartItem::Want(item) => *item,
            PartItem::Class(item) => *item,
        }
    }

    /// Checks if the item is a Want.
    pub fn is_want(&self) -> bool {
        match self {
            PartItem::Want(_) => true,
            _ => false
        }
    }

    /// Checks if the item is a Product.
    pub fn is_specific(&self) -> bool {
        match self {
            PartItem::Specific(_) => true,
            _ => false,
        }
    }

    pub fn is_class(&self) -> bool {
        match self {
            PartItem::Class(_) => true,
            _ => false
        }
    }
}