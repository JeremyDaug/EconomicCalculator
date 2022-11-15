#[derive(Debug)]
pub struct Technology {
    Id: u64,
    Name: String,
    Description: String,
    BaseCost: i64,
    Tier: i64,
    Families: HashSet<TechnologyFamily>,
    Children: HashSet<Technology>,
    Parents: HashSet<Technology>
}

pub enum TechnologyCategory {
    Primary,
    Secondary,
    Tertiary { level: u64 }
}