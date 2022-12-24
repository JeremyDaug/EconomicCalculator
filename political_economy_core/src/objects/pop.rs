//! The storage unit of population groups.
//! 
//! Used for any productive, intellegent actor in the system. Does not include animal
//! populations.
use std::collections::HashMap;

use super::desires::Desires;


/// Pops are the data storage for a population group.
/// 
/// Population groups are defines externally by what
/// market they are in, what firm they work in, and
/// what their job in that firm is.
/// 
/// Internally they are broken appart by the various of the
/// pop. It breaks them into a table to record details of how many are in each species/culture combo.
#[derive(Debug)]
pub struct Pop {
    /// Pop's unique id for navigation purposes.
    pub id: usize,
    /// Pop's total population (should be equal to the sum of it's breakdown table.)
    pub count: usize,
    /// The job of the pop.
    pub job: usize,
    /// Where the pop works.
    pub firm: usize,
    /// Which market they are in
    pub market: usize,
    /// The skill the pop uses.
    pub skill: usize,
    /// The lower bound of their skill level.
    pub lower_skill_level: f64,
    /// the upper bound of their skill level spread.
    pub higher_skill_level: f64,
    /// The total desires and property of the pop.
    pub desires: Desires,
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

pub struct PopBreakdownTable {

}