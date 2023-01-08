use super::{seller::Seller, actor_message::{ActorType, self, ActorInfo}, buyer::Buyer};



/// An institution, a non-economic entity which acts within the world.
/// 
/// An institution can produce and consume goods, and 
#[derive(Debug, Clone)]
pub struct Institution {
    pub id: usize,
    pub name: String,
    pub variant_name: String,
}

impl Institution {

}

impl Seller for Institution {
    fn actor_type(&self) -> ActorType {
        ActorType::Institution
    }

    fn actor_info(&self) -> ActorInfo {
        ActorInfo::Institution(self.id)
    }

    fn get_id(&self) -> usize {
        self.id
    }
}

impl Buyer for Institution {
    
}