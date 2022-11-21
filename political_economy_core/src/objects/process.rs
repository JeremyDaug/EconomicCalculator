#[derive(Debug)]
pub struct Process {
    id: u64,
    pub name: String,
    pub variant_name: String,
    pub description: String,
    // icon
    pub minimum_time: f64,
    pub process_parts: Vec<ProcessPortion>,
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
}

#[derive(Debug)]
pub struct ProcessPortion {
    pub item: PartItem,
    pub amount: f64,
    pub part_tags: Vec<ProcessPortionTag>,
    pub part: ProcessPartTag
}

impl ProcessPortion {
}

#[derive(Debug)]
pub enum ProcessPartTag {
    Input,
    Capital,
    Output
}

#[derive(Debug)]
pub enum ProcessPortionTag {
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

#[derive(Debug)]
pub enum PartItem {
    Product(u64),
    Want(u64)
}