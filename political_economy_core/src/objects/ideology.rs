use super::desire::Desire;

/// The worldview of a Pop. How they view the world, what they believe in
/// and how they think things should be.
/// 
/// This could be a political ideology, religion, or similar worldview 
/// producing set of ideas, metaphysics, philosophies, or the like.
/// 
/// Can change relatively often, many people should be expected to change
/// this throughout their life.
/// 
/// Will eventually contain data for Waves and Factions.
#[derive(Debug)]
pub struct Ideology {
    /// The id of the Ideology.
    pub id: usize,
    /// The name of the Ideology, generally shared by related ideologies.
    pub name: String,
    /// The variant name of the ideology, used to differentiate ideologies
    /// with the same primary name.
    pub variant_name: String,
    /// The birth rate modification from the ideology. Added to other
    /// birth rates. May be made multiplicative.
    /// 
    /// Can be negative, but cannot push birth rate below 0.
    pub birth_rate_modifier: f64,
    /// The mortality rate modification from the Ideology. Added to other
    /// mortality rates for a pop. May be made multiplicative.
    /// 
    /// Can be negative, but cannot push mortality rates below 0.
    pub mortality_rate_modifier: f64,
    /// The proudctivity modifier of the ideology, altering how much labor
    /// they can produce per unit of time.
    /// 
    /// Multiplicative with other productivity modifiers.
    pub productivity_modifier: f64,
    /// The desires of the ideology, for a singular pop.
    pub desires: Vec<Desire>,
    /// The Related Ideologies. Parents, children, and siblings.
    pub relations: Vec<usize>,
    // tags
}

impl Ideology {
    pub fn new(id: usize, name: String, variant_name: String, 
        birth_rate_modifier: f64, mortality_rate_modifier: f64, 
        productivity_modifier: f64, desires: Vec<Desire>, 
        relations: Vec<usize>) -> Result<Self, String> { 
            if name.trim().is_empty() {
                return Err("'name' cannot be empty or whitespace.".into());
            }
            Ok(Self { id, name, variant_name,
                birth_rate_modifier, mortality_rate_modifier, 
                productivity_modifier, desires, relations } )
            }

    pub fn get_name(&self) -> String {
        format!("{}({})", self.name, self.variant_name)
    }
}