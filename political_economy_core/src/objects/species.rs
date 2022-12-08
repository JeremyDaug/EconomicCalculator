use super::desire::Desire;


/// A Species, the biological Reality of a Pop which they are built with.
/// Requires either long time frames or biotech to overwrite biology.
#[derive(Debug)]
pub struct Species {
    /// The Id of the species.
    pub id: u64,
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
    pub relations: Vec<u64>,
    /// The cohorts which make up this species, aranged by the order in which they occur.
    pub cohorts: Vec<Cohort>,
    /// The Subtypes which the species can have and be born into.
    pub subtypes: Vec<Subtype>,

    /// How productive the species is at a basic level. 
    /// 
    /// A value of 1 representing being able to do 1 human hour's worth of work per hour.
    /// 
    /// Extreme values are discouraged, and negative values are invalid.
    pub base_productivity: f64,
    /// The birthrate of the species assuming 'ideal' environment and no
    /// contravening effects. 
    /// 
    /// Cannot be negative, a value of 0 means no new pops are born naturally.
    pub birth_rate: f64,
    // Preferences, Fill in later and Iron them out properly.
}

impl Species {
    pub fn get_name(&self) -> String {
        format!("{}({})", self.name, self.variant_name)
    }
}

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
    /// How likely a member of this cohort is to die due to various, inescapable
    /// causes (illness, excess risk taking, whatever)
    pub mortality_rate: f64,
    /// how much more or less productive this cohort is from the baseline of the
    /// species.
    pub productivity_modifier: f64,
    /// The various modifiers and tags that effect this cohort.
    pub cohort_tags: Vec<CohortTag>,
}

#[derive(Debug)]
pub struct Subtype {

}

/// Tags which are available to a species and modify how they work in the system.
#[derive(Debug)]
pub enum SpeciesTag {
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
    /// This cohort cannot produce children for whatever reason, perhaps they have yet to 
    /// develop, or perhaps their species ages out of fertility.
    Infertile,
    
}

/// Tags which are available to a Subtype of a species and modify how they work in the system.
#[derive(Debug)]
pub enum SubtypeTag {
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