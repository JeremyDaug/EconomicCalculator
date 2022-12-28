//! Species covers the biological nature of a pop.

use super::desire::Desire;

/// A Species, the biological Reality of a Pop which they are built with.
/// 
/// Requires either long time frames or biotech to overwrite biology.
/// 
/// Will eventually contain data for Cohorts and Subtypes.
/// 
/// Some of it is written, but it has been commented out.
#[derive(Debug)]
pub struct Species {
    /// The Id of the species.
    pub id: usize,
    /// The Primary name of the species.
    pub name: String,
    /// The Secondary Name of the species, used for sibling species.
    pub variant_name: String,
    /// The desires of the species, shared by all subdivisions. Subdivisions may
    /// modify this, but should not replace it entirely.
    pub desires: Vec<Desire>,
    /// The Tags of the species, modifying how the species is treated.
    /// Will eventually include special properties as part of Enironmental Expansion.
    /// 
    /// Mostly Placeholder.
    pub tags: Vec<SpeciesTag>,
    /// Related variant species, used for organization.
    pub relations: Vec<usize>,
    /// The cohorts which make up this species, aranged by the order in which they occur.
    //pub cohorts: Vec<Cohort>,
    /// The Subtypes which the species can have and be born into.
    //pub subtypes: Vec<Subtype>,
    /// How productive the species is at a basic level. 
    /// 
    /// A value of 1 representing being able to do 1 human hour's worth of work per hour.
    /// 
    /// Extreme values are discouraged, and negative values are invalid.
    pub base_productivity: f64,
    /// The birthrate of the species assuming 'ideal' environment and no
    /// contravening effects. This is per annum growth rate.
    /// 
    /// Cannot be negative, a value of 0 means no new pops are born naturally.
    pub birth_rate: f64,
    /// The base mortality rate of the species, representing the base line chance of
    /// death for the species.
    /// 
    /// Cannot be negative values, extreme values should be avoided.
    pub mortality_rate: f64,
    // Preferences, Fill in later and Iron them out properly.

    // The summarized effects of a species and cohort combo. 
    // The indices of these are defined such that
    // cohort_index + (subtype_index * len(cohorts)) = index of cohort + subtype.
    // pub data_table: Vec<SpeciesSubentry>,
}

/*
#[derive(Debug)]
pub struct SpeciesSubentry {
    /// The summarized desires for a member of this species+cohort+subtype
    pub desires: Vec<Desire>,
    /// The final productivity for a member of this species+cohort+subtype.
    pub productivity: f64,
    /// The final birth rate for a member of this species+cohort+subtype.
    pub birth_rate: f64,
    /// The final mortality rate for a member of this species+cohort+subtype.
    pub mortality_rate: f64,
    /// The final skill modifiers for a member of this species+cohort+subtype.
    pub skill_modifiers: HashMap<u64, f64>,
    /// The final skill group modifiers for a member of this species+cohort+subtype.
    pub skill_group_modifiers: HashMap<u64, f64>,
}
*/

impl Species {
    pub fn new(id: usize, name: String, 
        variant_name: String, desires: Vec<Desire>, 
        tags: Vec<SpeciesTag>, relations: Vec<usize>, 
        base_productivity: f64, birth_rate: f64, 
        mortality_rate: f64) -> Result<Self, String> { 
            if birth_rate < 0.0 {
                return Err("birth_rate must be 0 or greater.".into());
            }
            else if mortality_rate < 0.0 {
                return Err("'mortality_rate' must be 0 or greater.".into());
            }
            else if base_productivity < 0.0 {
                return Err("'base_productivity' must be 0 or greater.".into());
            }
            else if name.trim().is_empty() {
                return Err("'name' cannot be empty or whitespace.".into());
            }

            Ok(Self { 
                id, name, variant_name, desires, 
                tags, relations, base_productivity, 
                birth_rate, mortality_rate } )
            }

    pub fn get_name(&self) -> String {
        format!("{}({})", self.name, self.variant_name)
    }

    /*
    /// Updates the data table in the species, consolidating the cohort and subtype effects
    /// into a singular lookup table.
    pub fn update_data_table(&mut self) {
        // clear out the old data for simplicity
        self.data_table.clear();
        // iterate over both and summarize their effects.
        for cohort in self.cohorts.iter() {
            for subtype in self.subtypes.iter() {
                let mut entry = SpeciesSubentry{ 
                    desires: vec![], 
                    productivity: self.base_productivity, 
                    birth_rate: self.birth_rate, 
                    mortality_rate: self.mortality_rate, 
                    skill_modifiers: HashMap::new(), 
                    skill_group_modifiers: HashMap::new()
                };
            }
        }
    }

    pub fn get_subentry(&self, cohort_idx: usize, subtype_idx: usize) -> Result<&SpeciesSubentry, String> {
        match self.data_table.get(cohort_idx + (subtype_idx * self.cohorts.len())) {
            Some(val) => Ok(val),
            None => Err("Invalid Cohort or Species Index given".into())
        }
    } */
}
/*
/// Cohort is a subdivision of a Species. Pops are born into the first cohort listed
/// in the Species, and as time passes they eventually move upwards into different
/// cohorts. The last cohort is the one which continues and it's average span is the
/// average lifespan of an individual pop.
#[derive(Debug)]
pub struct Cohort {
    /// The name of the cohort grouping.
    pub name: String,
    /// How long the an individual member is expected to stay in this cohort,
    /// measured in standard in game days.
    /// 
    /// If this cohort is the last cohort of the species, this is how long they 
    /// live on average after entering this cohort.
    pub span: u64, 
    /// The birth rate bonus or malus for this cohort. This is additive with the
    /// base species and Subtype birth rates. This cannot push Birthrate below 0.
    pub birth_rate: f64,
    /// How likely a member of this cohort is to die due to various natural 
    /// causes (illness, excess risk taking, whatever). This is additive with 
    /// Mortality Rate from Subtype.
    pub mortality_rate: f64,
    /// how much more or less productive this cohort is from the baseline of the
    /// species.
    /// 
    /// This is multiplicative 
    pub productivity_modifier: f64,
    /// The various modifiers and tags that effect this cohort.
    pub tags: Vec<CohortTag>,
    /// The desires the cohort has different from the base species.
    /// 
    /// Desires here may have negative values, and if they do they cancel out the desire
    /// from the species. This subtraction cannot push the desire below 0.
    pub desires: Vec<Desire>,
}

impl Cohort {
}

/// Subtype is a subgroup of the species which divides based on innate phenotypes.
/// This is comparable to the difference in sex between humans or the differences
/// in caste for ants. Pops have their subtype selected at birth, and can't be changed
/// without special circumstances such as technology or magic.
#[derive(Debug)]
pub struct Subtype {
    /// The name of the Subtype
    pub name: String,
    /// The weight chance of a member of this species being born as this subtype.
    /// 
    /// Chance = weight / sum(species subtypes)
    pub weight: f64,
    /// The birth rate generated by this subtype. This is added to or subtracted from
    /// the species rate and stacks with the cohort modifier. Cannot push Birthrate below 0.
    pub birth_rate: f64,
    /// The modifier to mortality rate for the subtype.
    pub mortality_rate: f64,
    /// The productivity modifier for the subtype. Multiplictative with Species and
    /// cohort productivity modifiers.
    pub productivity_modifier: f64,
    /// The tags of the Subtype, defining additional properties of the subtype.
    pub subtype_tags: Vec<SubtypeTag>,
    /// The subtype's additional or reduced desires.
    /// Any desires which overlap with the species either add or remove. A desire cannot
    /// reach below 0.
    pub desires: Vec<Desire>,
}

impl Subtype {
} */

/// Tags which are available to a species and modify how they work in the system.
#[derive(Debug)]
pub enum SpeciesTag {
    /// This species has sexual reproduction, requiring multiple members for it to
    /// produce children. The string contained defines what members are needed and how
    /// they are split.
    /// 
    /// It functions with a simple ruleset
    /// () represents valid groupings
    /// / between characters represent the groupings needed
    /// xy character pairings like this represent either/or pairings
    /// The first in any grouping is the egg producer/carrier.
    /// 
    /// # Examples
    /// 
    /// "x", asexual reproduction. Not necissary if alone, but still valid.
    /// "f/m", standard Female-male pairing. (only valid option currently)
    /// "f/lm", three sexes, f is always needed, l or m are needed to complete the pairing.
    /// "(h)(f/m)", three sexes, one which reproduces hermaphroditically or asexually, the other two more standard.
    /// "(x/q)(a/t)" 4 sexes, x and a are the 'females' and need q and t to reproduce respectively.
    /// "(a/b/c/d)" 4 sexes, a carries the egg, but all of them are needed to produce children.
    ReproductiveGroup(String),
    /// The species are egg laying, producing an egg instead of live children.
    /// 
    /// The value contained represents the egg they produce.
    /// 
    /// This system in non-functional.
    Oviparous(u64),
    /// The species is a mindless drone, effectively incapable of meaningful
    /// individual action.
    Drone,
    /// This species or subgroup has a specific civilization which is biologically
    /// ingrained into them.
    InnateCivilization(u64),
    /// This species or subgroup has a specific culture which is biologically ingrained into
    /// them.
    InnateCulture(u64),
    /// A Specific Ideology which is innate to a species or subgroup at a biological level.
    InnateIdeology(u64),
    /// A particular skill that is noteable in their talent, or lack thereof, for a species.
    /// 
    /// Modifire should be non-negative, with 0 representing them being incapable of learning
    /// that skill. 
    /// 
    /// If at 0, they are incapable of learning that skill, and it automatically defaults to 0.
    /// If below 1 they are penalized in their learning.
    /// If above 1 they are particularly gifted in that skill and pick it up faster.
    SkillModifier{skill: u64, modifier: f64},
    /// A particular group of skills that this species is noteworthy in, either for their
    /// lack of skill or innate talent in doing it.
    /// 
    /// Modifire should be non-negative, with 0 representing them being incapable of learning
    /// skills from this group. 
    /// 
    /// If at 0, they are incapable of learning that skill, and it automatically defaults to 0.
    /// If below 1 they are penalized in their learning.
    /// If above 1 they are particularly gifted in that skill and pick it up faster.
    SkillGroupModifier{skill_group: u64, modifier: f64}
}
/*
/// Tags which are available to a Species' Cohort and modify how they work in the system.
#[derive(Debug)]
pub enum CohortTag {
    /// The species is a mindless drone, effectively incapable of meaningful
    /// individual action.
    Drone,
    /// This species or subgroup has a specific civilization which is biologically
    /// ingrained into them.
    InnateCivilization(u64),
    /// This species or subgroup has a specific culture which is biologically ingrained into
    /// them.
    InnateCulture(u64),
    /// A Specific Ideology which is innate to a species or subgroup at a biological level.
    InnateIdeology(u64),
    /// A particular skill that is noteable in their talent, or lack thereof, for a species.
    /// 
    /// Modifire should be non-negative, with 0 representing them being incapable of learning
    /// that skill. 
    /// 
    /// If at 0, they are incapable of learning that skill, and it automatically defaults to 0.
    /// If below 1 they are penalized in their learning.
    /// If above 1 they are particularly gifted in that skill and pick it up faster.
    SkillModifier{skill: u64, modifier: f64},
    /// A particular group of skills that this species is noteworthy in, either for their
    /// lack of skill or innate talent in doing it.
    /// 
    /// Modifire should be non-negative, with 0 representing them being incapable of learning
    /// skills from this group. 
    /// 
    /// If at 0, they are incapable of learning that skill, and it automatically defaults to 0.
    /// If below 1 they are penalized in their learning.
    /// If above 1 they are particularly gifted in that skill and pick it up faster.
    SkillGroupModifier{skill_group: u64, modifier: f64},
    
}

/// Tags which are available to a Subtype of a species and modify how they work in the system.
#[derive(Debug)]
pub enum SubtypeTag {
    /// The sex of the subtype, if any. If none, then the subtype is a neuter.
    /// If the species doesn't have the SpeciesTag::ReproductiveGroup() tag, 
    /// then the species reproduces asexually.
    Sex(String),
    /// The species is a mindless drone, effectively incapable of meaningful
    /// individual action.
    Drone,
    /// This species or subgroup has a specific civilization which is biologically
    /// ingrained into them.
    InnateCivilization(u64),
    /// This species or subgroup has a specific culture which is biologically ingrained into
    /// them.
    InnateCulture(u64),
    /// A Specific Ideology which is innate to a species or subgroup at a biological level.
    InnateIdeology(u64),
    /// A particular skill that is noteable in their talent, or lack thereof, for a species.
    /// 
    /// Modifire should be non-negative, with 0 representing them being incapable of learning
    /// that skill. 
    /// 
    /// If at 0, they are incapable of learning that skill, and it automatically defaults to 0.
    /// If below 1 they are penalized in their learning.
    /// If above 1 they are particularly gifted in that skill and pick it up faster.
    SkillModifier{skill: u64, modifier: f64},
    /// A particular group of skills that this species is noteworthy in, either for their
    /// lack of skill or innate talent in doing it.
    /// 
    /// Modifire should be non-negative, with 0 representing them being incapable of learning
    /// skills from this group. 
    /// 
    /// If at 0, they are incapable of learning that skill, and it automatically defaults to 0.
    /// If below 1 they are penalized in their learning.
    /// If above 1 they are particularly gifted in that skill and pick it up faster.
    SkillGroupModifier{skill_group: u64, modifier: f64}
}
*/