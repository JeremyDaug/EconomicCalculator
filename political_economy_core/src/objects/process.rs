///! Processes transform Products and Wants into other products and wants.
use std::{collections::{HashMap, HashSet}, option};

use itertools::{zip_eq, Itertools};

use crate::{constants::{self, lerp, reverse_lerp}, data_manager::DataManager};

use super::{property_info::PropertyInfo, item::Item};

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
    //pub skill: Option<usize>,
    //pub skill_minimum: f64,
    //pub skill_maximum: f64,
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
        self.process_parts.iter().filter(|x| x.part.is_input() && x.item.is_product()).collect_vec()
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
        self.process_parts.iter().filter(|x| !x.part.is_output() && x.item.is_product()).collect_vec()
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
        self.process_parts.iter().filter(|x| x.part.is_capital() && x.item.is_product()).collect_vec()
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
        self.process_parts.iter().filter(|x| x.part.is_output() && x.item.is_product()).collect_vec()
    }

    /// # Outputs Proudct
    /// 
    /// Check if this process outputs a particular product in any way shape or form.
    /// 
    /// Returns true if any output is the product, false otherwise.
    pub fn outputs_product(&self, product: usize) -> bool {
        self.process_parts.iter()
        .filter(|x| x.part.is_output() && x.item.is_product())
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
    pub fn can_feed_self(&self, data: &DataManager) -> bool {
        let mut checked_products = HashMap::new();
        let mut checked_wants = HashMap::new();
        let mut checked_classes = HashSet::new();
        for part in self.process_parts.iter() {
            match part.item {
                Item::Product(prod) => {
                    let count = checked_products.entry(prod).or_insert(vec![]);
                    count.push(part);
                }
                Item::Want(want) => {
                    let count = checked_wants.entry(want).or_insert(vec![]);
                    count.push(part);
                }
                Item::Class(class) => {
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
        .filter(|&x| x.part.is_output() && x.item.is_product())
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
    /// Currently, it always uses optional goods when possible.(tm) It's not 
    /// very smart about it yet though. Optional Good Bonuses are 
    /// multiplicative, not additive.
    /// 
    /// TODO Ideally, optional goods are selected either here (or by the actor calling this) such that the additional expense is made up for the additional output gained, measured in AMV, presumably.
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
    /// TODO Include logic for process part tags: Consumption, Investment, Pollutant, Chance(char, usize)
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
    available_wants: &HashMap<usize, f64>, 
    _other_efficiency_boni: f64, target: Option<f64>, _hard_cap: bool, 
    data: &DataManager) 
    -> ProcessOutputs {
        let mut results = ProcessOutputs::new();
        // get how many cycles we can do in total
        // TODO check and take optional and fixed items into account here.
        // the (rolling) throughput modifier from optional products
        // optional items will need to be ignored if unavailable, but add to the target of all non-fixed items
        // fixed items ignore any efficiency gains from optional
        let mut ratios = HashMap::new();
        let mut normals = HashMap::new(); // The highest we can get 
        let mut lowest_normal = f64::INFINITY;
        let mut max_poss_fixed = f64::INFINITY; // the highest possible fixed iterations.
        let mut optional_iters = HashMap::new();
        let mut optional_mods = HashMap::new();
        let mut initial_penalty = 1.0; // the penalty which we have if no optional parts are used.
        for (idx, process_part) in self.process_parts.iter().enumerate() {
            if let ProcessSectionTag::Capital = process_part.part { // skip capital wants for now.
                // todo add capital want handling here.
                if let Item::Want(_id) = process_part.item {
                    debug_assert!(false, "Should not use wants in capital yet.");
                    continue;
                }
            }
            if let ProcessSectionTag::Output = process_part.part { // if output, ignore
                continue;
            }
            let mut optional = None;
            let mut fixed = false;
            for tag in process_part.part_tags.iter() { // get whether it's optional or fixed for later.
                match tag {
                    ProcessPartTag::Optional { missing_penalty, final_bonus } => {
                        optional = Some((missing_penalty, final_bonus));
                    },
                    ProcessPartTag::Fixed => {
                        fixed = true;
                    },
                    _ => ()
                }
            }
            debug_assert!(!(optional.is_some() && fixed), "Cannot be both optional and fixed.");
            let mut ratio = 0.0;
            match process_part.item { // get the amount of times we can do the part.
                Item::Product(id) => {
                    // take lower between current ratio available and available_product / cycle_target.
                    ratio = available_products.get(&id).unwrap_or(&0.0) 
                        / process_part.amount;
                    ratios.insert(idx, ratio);
                },
                Item::Want(id) => { // input wants are taken directly from storage.
                    // take lower between current ratio available and available_product / cycle_target.
                    ratio = available_wants.get(&id).unwrap_or(&0.0) / process_part.amount;
                    ratios.insert(idx, ratio);
                },
                Item::Class(id) => {
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
                    ratio = sum / process_part.amount;
                    ratios.insert(idx, ratio);
                }
            }
            if let Some((penalty, bonus)) = optional { // if optional, get that info and record it as optional
                optional_iters.insert(idx, ratio);
                optional_mods.insert(idx, (penalty, bonus));
                initial_penalty = initial_penalty * (1.0 + penalty);
            } else { // non optional
                if fixed { // and fixed, modify max possible fixed to be the lower between this and it's current vaule.
                    max_poss_fixed = max_poss_fixed.min(ratio);
                } else { // if normal, record it for later use and check it for highest possible.
                    normals.insert(idx, ratio);
                    lowest_normal = lowest_normal.min(ratio);
                }
            }
        }
        if max_poss_fixed == 0.0 || lowest_normal == 0.0 {
            // if no possible iterations, skip to output nothing.
            return ProcessOutputs::new();
        }
        // TODO #67 Add Load Check on processes, which ensures that an optional part tag is paired with a meaningful fixed or other limitation elsewhere.
        // TODO cap is always treated as hard, need to do work to make it soft.
        let cap = target.unwrap_or(f64::INFINITY); // get cap.
        lowest_normal = lowest_normal.min(cap); // reduce max poss fixed to the cap.

        // TODO Note area for soon to come rewrite of this calculation section
        // Get the minimum number of Fixed iterations we can do
        // get the minimum number of Normal(Throughput modified) iterations we can do.
        // Get the iterations we can do with bonuses.
        // walk up the lowest possible options with each, using as much as possible.
        let mut fixed_iters = 0.0; // how many iterations of fixed done.
        let mut normal_iters = 0.0; // how many normal iterations to do.
        let mut total_bonus = 1.0;
        // how much of each bonus we consumed (organized by key).
        let mut bonus_iters = HashMap::new();
        loop {
            // get lowest between normal, fixed, and optionals
            let mut lowest = lowest_normal.min(max_poss_fixed)
                .min(*optional_iters.values()
                    .min_by(|a, b| a.total_cmp(b))
                    .unwrap_or(&f64::INFINITY));
            // with lowest gotten, record the results and subtract from others.
            let mut current_bonus = initial_penalty;
            let mut cap_reached = false;
            // iterate by key over the optionals
            for (&key, (&penalty, &bonus)) in optional_mods.iter()
                .sorted_by(|a, b| a.0.cmp(b.0)) {
                let &cur_bon_iter = optional_iters.get(&key).unwrap();
                if cur_bon_iter > 0.0 && !cap_reached{
                    // remove penalty first
                    current_bonus = current_bonus / (1.0 + penalty);
                    current_bonus = current_bonus * (1.0 + bonus);
                    // include in bonus iters fully
                    bonus_iters.entry(key)
                        .and_modify(|x| *x += lowest)
                        .or_insert(lowest);
                } // no else, penalty is included by default.
                // check if we've overshot the normal target
                if current_bonus * lowest > lowest_normal {
                    if lowest == lowest_normal { 
                        // if overflows because this normal lowest is lowest, maximize savings
                        bonus_iters.entry(key).and_modify(|x| *x -= lowest); // remove iters
                        // reduce effective lowest to maximize effect.
                        lowest = lowest_normal / current_bonus;
                        bonus_iters.entry(key).and_modify(|x| *x += lowest); // add this ratio of iters back in.
                    } else { // else, reduce to cap at normal
                        // reduce the current bonus to match normal iters, then leave loop
                        bonus_iters.entry(key).and_modify(|x| *x -= lowest); // remove iters
                        current_bonus = current_bonus / (1.0 + bonus);
                        let target_bonus = lowest_normal / (lowest * current_bonus); // get the bonus needed
                        current_bonus = current_bonus * (target_bonus); // add target back into current.
                        let ratio = reverse_lerp(penalty, bonus, target_bonus-1.0); // get the ratio of iteration needed.
                        bonus_iters.entry(key).and_modify(|x| *x += ratio * lowest); // add this ratio of iters back in.
                        cap_reached = true; // set cap reached = true for future needs.
                    }
                    break; // get out of optional loop, no benefit can come from going further.
                }
            }
            // update total bonuses (add bonus via average.)
            total_bonus = ((total_bonus * fixed_iters) + (current_bonus * lowest)) / (fixed_iters + lowest);
            // with bonii gotten, apply fixed alteration
            max_poss_fixed -= lowest;
            fixed_iters += lowest;
            // same with normals, but don't forget the bonus throughput.
            lowest_normal -= lowest * current_bonus;
            normal_iters += lowest * current_bonus;
            if max_poss_fixed == 0.0 || lowest_normal == 0.0 {
                // if we cannot get more fixed or normal iterations in, bounce.
                break;
            } 
            debug_assert!(max_poss_fixed >= 0.0, "Max Possible fixed somehow got below 0.");
            debug_assert!(lowest_normal >= 0.0, "Lowest normal somehow got below 0.");
        }

        // TODO make consider adding profitability check here for optional products vs the extra output.
        
        // without efficiency gains, our ratio is our fixed target
        results.iterations = fixed_iters;
        // efficiency is equal to the total possible gain we were able to achieved.
        results.efficiency = total_bonus;
        // effective iterations is our iterations after applying efficiency gains.
        results.effective_iterations = normal_iters;

        // with our target ratio gotten, create the return results for inputs and outputs
        // TODO fixed items will also need to be taken into account here.
        for (idx, process_part) in self.process_parts.iter().enumerate() {
            let mut in_out_sign = 1.0;
            let mut fixed = false;
            let mut optional = false;
            let mut consumed = false;
            for tag in process_part.part_tags.iter() {
                match tag {
                    ProcessPartTag::Fixed => fixed = true,
                    ProcessPartTag::Optional { .. } => optional = true,
                    ProcessPartTag::Consumed => consumed = true,
                    _ => ()
                }
            }
            match process_part.part {
                ProcessSectionTag::Capital => {
                    if let Item::Product(_id) = process_part.item {
                        // add used capital products
                        if fixed {
                            results.capital_products
                                .insert(process_part.item.unwrap(), process_part.amount * fixed_iters);
                        } else if optional {
                            let optional_val = bonus_iters.get(&idx).unwrap();
                            results.capital_products
                                .insert(process_part.item.unwrap(), process_part.amount * optional_val);
                        } else {
                            results.capital_products
                                .insert(process_part.item.unwrap(), process_part.amount * normal_iters);
                        }
                        

                    } else if let Item::Want(_id) = process_part.item { 
                        // TODO add capital want handling here also.
                    }
                    continue;
                },
                ProcessSectionTag::Input => { in_out_sign = -1.0; }, // subtract inputs
                ProcessSectionTag::Output => { in_out_sign = 1.0; }, // add outputs
            } 
            // if not capital, add to appropriate input_output
            match process_part.item {
                Item::Product(id) => {
                    if fixed {
                        results.input_output_products.entry(id)
                            .and_modify(|x| *x += in_out_sign * process_part.amount * fixed_iters)
                            .or_insert(in_out_sign * process_part.amount * fixed_iters);
                    } else if optional {
                        let optional_val = bonus_iters.get(&idx).unwrap();
                        results.input_output_products.entry(id)
                            .and_modify(|x| *x += in_out_sign * process_part.amount * optional_val)
                            .or_insert(in_out_sign * process_part.amount * optional_val);
                    } else { // consumed or others
                        results.input_output_products.entry(id)
                            .and_modify(|x| *x += in_out_sign * process_part.amount * normal_iters)
                            .or_insert(in_out_sign * process_part.amount * normal_iters);
                    }
                    // if optional or consumed, add the failure outputs.
                    if (consumed && fixed) { // consume process on these.
                        Process::get_consumed_outputs(id, data, &mut results, fixed_iters);
                    } else if optional {
                        let &optional_val = bonus_iters.get(&idx).unwrap();
                        Process::get_consumed_outputs(id, data, &mut results, optional_val);
                    } else if consumed { // consume process on these.
                        Process::get_consumed_outputs(id, data, &mut results, normal_iters);
                    }
                },
                Item::Want(id) => {
                    // Consumed are not considerde valid.
                    if fixed {
                        results.input_output_wants.entry(id)
                        .and_modify(|x| *x += in_out_sign * process_part.amount * fixed_iters)
                        .or_insert(in_out_sign * process_part.amount * fixed_iters);
                    } else { // not fixed
                        results.input_output_wants.entry(id)
                        .and_modify(|x| *x += in_out_sign * process_part.amount * normal_iters)
                        .or_insert(in_out_sign * process_part.amount * normal_iters);
                    }
                },
                Item::Class(id) => { // TODO test this part of the code!
                    debug_assert!(process_part.part.is_output(), "Class cannot be an output.");
                    // TODO improve this to deal with overlap and quality management.
                    // remove from inputs
                    let removed_products = if fixed {
                        Process::class_part_processing(id, &available_products, data, 
                            fixed_iters, process_part, &mut results)
                    } else if optional {
                        let &optional_val = bonus_iters.get(&idx).unwrap();
                        Process::class_part_processing(id, &available_products, data, 
                            optional_val, process_part, &mut results)
                    } else { // not fixed
                        Process::class_part_processing(id, &available_products, data, 
                            normal_iters, process_part, &mut results)
                    };
                    // if marked consumed, also fail process those specific items as well.
                    for (&id, &amount) in removed_products.iter() {
                        Process::get_consumed_outputs(id, data, &mut results, amount);
                    }
                },
            }
        }

        results
    }

    /// # Get consumed outputs
    /// 
    /// Takes an ID of the product we want to consume (fail), the result 
    /// outputs we are going to add to, and the iterations (amount) of 
    /// the item we are going to fail.
    /// 
    /// This does not destroy the product given, instead assuming that it
    /// was consumed elsewhere.
    fn get_consumed_outputs(product_id: usize, data: &DataManager, results: &mut ProcessOutputs, iterations: f64) {
        let prod = data.products.get(&product_id).unwrap();
        let process_id = prod.failure_process;
        if let Some(proc_id) = process_id {
            // just get outputs as that's all we need.
            let proc = data.processes.get(&proc_id).expect("Failure Process Not Found.");
            // shortcut our process. add our outputs and remove this input.
            let proc_outs = proc.outputs();
            for prod_out in proc_outs.iter() {
                match prod_out.item {
                    Item::Product(id) => {
                        results.input_output_products.entry(id)
                        .and_modify(|x| *x += prod_out.amount * iterations)
                        .or_insert(prod_out.amount * iterations);
                    },
                    Item::Want(id) => {
                        results.input_output_wants.entry(id)
                        .and_modify(|x| *x += prod_out.amount * iterations)
                        .or_insert(prod_out.amount * iterations);
                    },
                    _ => unreachable!("Should never be reached!")
                }
            }
        }
    }

    fn class_part_processing(class_id: usize, available_products: &HashMap<usize, f64>, data: &DataManager, 
    iterations: f64, process_part: &ProcessPart, results: &mut ProcessOutputs) -> HashMap<usize, f64> {
        let mut ret = HashMap::new();
        // get the class products
        let class_mates = available_products.iter()
        .filter(|(&prod_id, _)| {
            let class = data.get_product_class(prod_id);
            if let Some(val) = class {
                class_id == val
            } else { false }
        });
        // get items up to our needs
        let mut target = process_part.amount * iterations;
        for (&product_id, &quantity) in class_mates {
            let add = quantity.min(target);
            results.input_output_products.entry(product_id)
            .and_modify(|x| *x += add).or_insert(add);
            ret.insert(product_id, add);
            target -= add;
            if target == 0.0 { break; }
        }
        ret
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
    /// Allow Reserves bool defines whether it limits itself to unreserved items or not.
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
    available_wants: &HashMap<usize, f64>, _other_efficiency_boni: f64, 
    target: Option<f64>, _hard_cap: bool, data: &DataManager,
    allow_reserves: bool) 
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
                if let Item::Want(_id) = process_part.item {
                    continue;
                }
            }
            if let ProcessSectionTag::Output = process_part.part { // if output, ignore
                continue;
            }
            match process_part.item { // TODO add optional check here.
                Item::Product(id) => {
                    // take lower between current ratio available and available_product / cycle_target.
                    if let Some(prod_info) = available_products.get(&id) {
                        if allow_reserves {
                            ratio_available = ratio_available
                                .min(prod_info.available_for_want() / process_part.amount);
                        } else {
                            ratio_available = ratio_available
                                .min(prod_info.unreserved / process_part.amount)
                        }
                    } else { // if we don't have that item in property, return to 0.0
                        ratio_available = 0.0;
                    }
                },
                Item::Want(id) => {
                    // take lower between current ratio available and available_product / cycle_target.
                    ratio_available = ratio_available
                        .min(available_wants.get(&id).unwrap_or(&0.0) / process_part.amount);
                },
                Item::Class(id) => {
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
                    let sum: f64 = if allow_reserves {
                         class_mates.map(|x| x.1.available_for_want()).sum()
                    } else {
                        class_mates.map(|x| x.1.unreserved).sum()
                    };
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
                    if let Item::Product(_id) = process_part.item {
                        // add used capital products
                        results.capital_products
                        .insert(process_part.item.unwrap(), process_part.amount * ratio_available);
                    } else if let Item::Want(_id) = process_part.item { 
                        // TODO add capital want handling here also.
                    }
                    continue;
                },
                ProcessSectionTag::Input => { _in_out_sign = -1.0; }, // subtract inputs
                ProcessSectionTag::Output => { _in_out_sign = 1.0; }, // add outputs
            } 
            // if not capital, add to appropriate input_output
            match process_part.item {
                Item::Product(id) => {
                    results.input_output_products.entry(id)
                    .and_modify(|x| *x += _in_out_sign * process_part.amount * ratio_available)
                    .or_insert(_in_out_sign * process_part.amount * ratio_available);
                },
                Item::Want(id) => {
                    results.input_output_wants.entry(id)
                    .and_modify(|x| *x += _in_out_sign * process_part.amount * ratio_available)
                    .or_insert(_in_out_sign * process_part.amount * ratio_available);
                },
                Item::Class(id) => { // TODO test this part of the code!
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
    pub fn effective_output_of(&self, item: Item) -> f64 {
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
                (x.item.is_product() && x.item.unwrap() == product) || 
                (x.item.is_class() && x.item.unwrap() == class)
            });
        } else {
            // if product does not have a class, just look at the specifics.
            return self.process_parts.iter()
            .filter(|x| !x.part.is_output())
            .any(|x| x.item.is_product() && x.item.unwrap() == product);
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
    pub item: Item,
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
    /// 
    /// missing_penalty is a penalty it applies if the optional product is 
    /// missing. Should be non-positive (0.0 means no penalty.)
    /// Final_bonus is the total throughput bonus granted after all of the 
    /// requested amount is met.
    /// The line between the penalty and final bonus is flat.
    /// 
    /// Does not effect optional, fixed, or investment parts.
    /// Input items marked optional are not output, but instead consumed or 
    /// failed instead.
    /// 
    /// Optional goods are also Consumed.
    /// 
    /// ## Applicable to:
    /// - Inputs
    /// - Capital
    Optional{missing_penalty: f64, final_bonus: f64},
    /// Marks an item as Cosumed, rather than destroyed by the process.
    /// Used particularly for items which don't directly go into the end
    /// product, but are still used to create the end product. IE, making 
    /// steel requires using a catalyst to remove the impurities.
    /// 
    /// Consumed goods are instantly failed upon use. 
    /// TODO may need to change this over to consumed instead of used, but will require special consumption process or way to make special consupmtion. Maybe include it as parameter.
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

impl ProcessPartTag {
    /// # Optional lerp
    /// 
    /// Takes in an Optional ProcessPart Tag and a lerp value t.
    /// 
    /// ## Panics
    /// 
    /// Panics if given anything other than an Option ProcessPartTag,
    pub fn optional_lerp(option_val: &ProcessPartTag, t: f64) -> f64 {
        if let ProcessPartTag::Optional { missing_penalty, final_bonus } = option_val {
            constants::lerp(*missing_penalty, *final_bonus, t)
        } else {
            panic!("fn optional_lerp MUST be given an Optional Tag. Nothing else should be accepted.")
        }
    }
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