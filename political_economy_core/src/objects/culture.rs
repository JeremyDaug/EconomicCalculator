use super::desire::Desire;

/// A Culture, the social nature of the pop. How they act unconsciously,
/// and how they view themselves.
/// 
/// Can be changed occasionally throughout a pop's life.
/// 
/// Will eventually contain data for Classes and Generations.
#[derive(Debug)]
pub struct Culture{
    /// The Id of the culture.
    pub id: usize,
    /// The name of the Culture, typically shared between sibling cultures.
    pub name: String,
    /// The Variant name of thu culture, used to distinguish it from it's 
    /// siblings.
    pub variant_name: String,
    /// Additional birth rate of the culture, is added to the Species bonus.
    /// May change this to multiplicative to mesh better with species.
    /// 
    /// May be less than 0, but cannot push total pop birth rate below 0.
    pub birth_rate_modifier: f64,
    /// The Additional Mortality Rate of the culture, how likely they are to
    /// die more generally for any reason.
    /// 
    /// Likely to be slowly outmoded by modifiers elsewhere.
    /// 
    /// May be less than 0, cannot push total mortality below 0.
    pub mortality_rate_modifier: f64,
    /// The Additional General Productivity modifier of this culture.
    /// 
    /// This is multiplicative with Species.
    pub productivity_modifier: f64,
    /// The desires which this culture has on top of their species requirements.
    pub desires: Vec<Desire>,
    // tags
    /// The ids of cultures which are related to this one. Parents, Siblings, 
    /// and Children.
    pub relations: Vec<usize>,
}

impl Culture {
    pub fn new(id: usize, name: String, variant_name: String, 
        birth_rate_modifier: f64, mortality_rate_modifier: f64, 
        productivity_modifier: f64, desires: Vec<Desire>, 
        relations: Vec<usize>) -> Result<Self, String> { 
            if name.trim().is_empty() {
                return Err("'name' cannot be empty or contain only whitespace.".into());
            }
            Ok(Self { id, name, variant_name, birth_rate_modifier, 
                mortality_rate_modifier, productivity_modifier, 
                desires, relations })
            }

    pub fn get_name(&self) -> String {
        format!("{}({})", self.name, self.variant_name)
    }
}