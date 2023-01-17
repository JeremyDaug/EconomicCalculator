
/// A Helper for Pops, recording their data and memories for use in 
/// various calculations and to remember things which should be known
/// by the pop without 
#[derive(Debug, Clone)]
pub struct PopMemory {
    /// If the pop is part of disorganized firm or not.
    pub is_disorganized: bool,
    /// how much of the 
    pub work_time: f64,
}
impl PopMemory {
    pub(crate) fn create_empty() -> PopMemory {
        PopMemory { is_disorganized: false, work_time: 0.0 }
    }
}