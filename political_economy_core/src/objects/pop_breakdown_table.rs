use std::{collections::HashMap, ops::{AddAssign}};

/// Pop Breakdown Table
/// 
/// Stores the information and 'rows' of a population's data table. 
/// Each
#[derive(Debug)]
pub struct PopBreakdownTable {
    /// The table of all our data, broken up by the row's categories.
    /// Species(Cohort, Subtype), Culture(Class, Generation), Ideology(Wave, Faction)
    pub table: Vec<PBRow>,
    /// The sum of all the people in the table.
    /// Keep updated with any change.
    pub total: usize,
}

impl PopBreakdownTable {
    /// Inserts a pop into the table. If a row in the table matches it's 
    /// demographics, then add to that pop instead of a new row.
    pub fn insert_pops(&mut self, row: PBRow) {
        // check if there is an existing one.
        let existing = self.table.iter_mut()
        .find(|x| x.is_match(&row));

        if let Some(extant) = existing {
            extant.count += row.count; // if it exists, add
        }
        else {
            self.table.push(row); // else, add that row.
        }
        self.total += row.count;
    }

    /// Removes pops from the table. If this empties the row, then it removes
    /// that row.
    pub fn remove_pops(&mut self, row: &PBRow) {
        // check if there is an existing one.
        let existing = self.table.iter_mut()
        .find(|x| x.is_match(row));

        if let Some(extant) = existing {
            let min = extant.count.min(row.count);
            extant.count -= min; // if it exists, subtract (cap at existing)
            self.total -= min;
            if extant.count == 0 {
                let idx = self.table.iter().position(|x| x.is_match(row))
                .expect("Index Not Found");
                self.table.remove(idx);
            }
        }
    }

    /// Gets the number of pops in each species.
    pub fn species_makeup(&self) -> HashMap<usize, usize> {
        let mut result = HashMap::new();
        for row in self.table.iter() {
            result.entry(row.species).or_insert(0).add_assign(row.count);
        }
        result
    }

    /// Gets the number of pops in each species.
    pub fn culture_makeup(&self) -> HashMap<Option<usize>, usize> {
        let mut result = HashMap::new();
        for row in self.table.iter() {
            result.entry(row.culture).or_insert(0).add_assign(row.count);
        }
        result
    }

    /// Gets the number of pops in each species.
    pub fn ideology_makeup(&self) -> HashMap<Option<usize>, usize> {
        let mut result = HashMap::new();
        for row in self.table.iter() {
            result.entry(row.ideology).or_insert(0).add_assign(row.count);
        }
        result
    }

    /// Counts up and returns the percentile division into each species.
    pub fn species_division(&self) -> HashMap<usize, f64> {
        let mut result = HashMap::new();
        for group in self.table.iter() {
            *result.entry(group.species).or_insert(0.0) += (group.count as f64) / (self.total as f64);
        }
        result
    }

    /// Counts up and returns the percentile division into each culture.
    /// Includes pops who have no Ideology.
    pub fn culture_division(&self) -> HashMap<Option<usize>, f64> {
        let mut result = HashMap::new();
        for group in self.table.iter() {
            *result.entry(group.culture).or_insert(0.0) += (group.count as f64) / (self.total as f64);
        }
        result
    }

    /// Conuts up and returns the percentile division into each ideology.
    /// Includes pops who have no Ideology.
    pub fn ideology_division(&self) -> HashMap<Option<usize>, f64> {
        let mut result = HashMap::new();
        for group in self.table.iter() {
            *result.entry(group.ideology).or_insert(0.0) += (group.count as f64) / (self.total as f64);
        }
        result
    }
}

/// Pop Breakdown Table Row
/// 
/// Allows us to divide the number of people by 
/// - Species
///   - Cohont
///   - Subtype
/// - Culture
///   - Generation
///   - Class
/// - Ideology
///   - Faction
///   - Wave
/// 
/// Each row also contains a number, which is how many people
/// it contains.
#[derive(Debug, Clone, Copy)]
pub struct PBRow {
    /// The species id
    pub species: usize,
    /// The Id of the Species Cohort
    pub species_cohort: Option<usize>,
    // The Id of the Species Subtype
    pub species_subtype: Option<usize>,

    /// Culture Id
    pub culture: Option<usize>,
    /// Culture Generation Id
    pub culture_generation: Option<usize>,
    /// Culture Class Id
    pub culture_class: Option<usize>,

    /// Ideology Id
    pub ideology: Option<usize>,
    /// Ideology wave Id\
    pub ideology_wave: Option<usize>,
    /// Ideology faction Id
    pub ideology_faction: Option<usize>,

    /// How many people meet this row.
    pub count: usize,
}

impl PBRow {
    pub fn new(species: usize, species_cohort: Option<usize>,
        species_subtype: Option<usize>, culture: Option<usize>, 
        culture_generation: Option<usize>, culture_class: Option<usize>, 
        ideology: Option<usize>, ideology_wave: Option<usize>, 
        ideology_faction: Option<usize>, count: usize) -> Self { 
            Self { species, species_cohort, species_subtype, 
                culture, culture_generation, culture_class, ideology, 
                ideology_wave, ideology_faction, count } 
            }

    pub fn is_match(&self, other: &PBRow) -> bool {
        self.species == other.species &&
        self.species_cohort == other.species_cohort &&
        self.species_subtype == other.species_subtype &&
        self.culture == other.culture &&
        self.culture_class == other.culture_class &&
        self.culture_generation == other.culture_generation &&
        self.ideology == other.ideology &&
        self.ideology_faction == other.ideology_faction &&
        self.ideology_wave == other.ideology_wave
    }
}