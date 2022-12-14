//! The storage unit of population groups.
//! 
//! Used for any productive, intellegent actor in the system. Does not include animal
//! populations.
use std::collections::HashMap;


/// Pops are the data storage for a population group.
/// 
/// Population groups are defines externally by what
/// market they are in, what firm they work in, and
/// what their job in that firm is.
/// 
/// Internally they are broken appart by the various of the
/// pop. It breaks them into a table of 
#[derive(Debug)]
pub struct Pop {
    pub id: usize,
    pub name: String,
    pub count: usize,
    pub job: usize,
    pub firm: usize,
    pub market: usize,
    pub skill: usize,
    pub lower_skill_level: f64,
    pub higher_skill_level: f64,
    pub property: HashMap<usize, f64>,
    pub species: HashMap<usize, usize>,
    // civilization
    pub culture: HashMap<usize, usize>
    // Ideology
    // Movements
    // Mood
    // desires
    // for sale?
}

impl Pop {
}