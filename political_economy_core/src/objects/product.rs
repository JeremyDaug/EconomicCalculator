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
    TechRequired: Technology
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
    pub fn get_name(&self) -> String {
        self.Name + "(" + self.VariantName + ")"
    }

    pub fn get_tech(&self) -> String {
        self.TechRequired.Name
    }
}