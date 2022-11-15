#[derive(Debug)]
pub struct Product {
    Id: u64,
    Name: String,
    VariantName: String,
    Description: String,
    UnitName: String,
    Quality: i32,
    Mass: f64,
    Bulk: f64,
    MeanTimeToFailure: Option<u32>,
    Fractional: bool,
    // icon
    // product tags
    // wants satisfied
    // processes
        // failure
        // use
        // consume
        // maintenance
    // Techrequirements
}

enum ProductTag {
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

impl Product {
    get_name(&self, )
}