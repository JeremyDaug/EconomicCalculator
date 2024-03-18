use barrage::{Receiver, Sender};

use crate::{data_manager::DataManager, demographics::Demographics, objects::environmental_objects::market::MarketHistory};

use super::{seller::Seller, actor_message::{ActorType, ActorInfo, ActorMessage}, buyer::Buyer, actor::Actor};



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

impl Actor for Institution {
    /// Run Market Day for Institution.
    /// 
    /// This is a placeholder. Currently it just sends Finished, and ends
    /// it there.
    fn run_market_day(&mut self, 
        sender: &mut Sender<ActorMessage>,
        _reciever: &mut Receiver<ActorMessage>,
        _data: &DataManager,
        _demos: &Demographics,
        _history: &MarketHistory) {
        // TODO this function needs to be completed
        // Send finished to keep things running, then gtfo.
        sender.send(ActorMessage::Finished { 
            sender: self.actor_info() 
        }).expect("Failure!");
    }
}