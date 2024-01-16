use barrage::{Receiver, Sender};

use crate::{demographics::Demographics, data_manager::DataManager};

use super::{seller::Seller, actor_message::{ActorType, ActorInfo, ActorMessage}, buyer::Buyer, actor::Actor, market::MarketHistory};



/// A state is a governmental entity of our system.
/// 
/// It is often composed of different Institutions working togteher
/// or created by one of them gaining a monopoly on Violence.
/// 
/// Instead of taking tithes from just it's members, it takes taxes from
/// all pops which live in it's territory. 
#[derive(Debug, Clone)]
pub struct State {
    pub id: usize,
    pub name: String,
    pub variant_name: String,
}

impl State {
    
}

impl Seller for State {
    fn actor_type(&self) -> ActorType {
        ActorType::State
    }

    fn actor_info(&self) -> ActorInfo {
        ActorInfo::State(self.id)
    }

    fn get_id(&self) -> usize {
        self.id
    }
}

impl Buyer for State {
    
}

impl Actor for State {
    /// Run Market Day for States.
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
        }).expect("Failed to send");
    }
}