use barrage::{Sender, Receiver};

use crate::{data_manager::DataManager, demographics::Demographics};
use crate::objects::environmental_objects::market::MarketHistory;

use super::actor_message::ActorMessage;



/// A trait to demark a class as capable of acting in a market.
pub trait Actor {
    /// Runs the market day for the actor. 
    /// Takes in a Barrage Sender and Reciever for message passing
    fn run_market_day(&mut self, 
        sender: &mut Sender<ActorMessage>,
        reciever: &mut Receiver<ActorMessage>,
        data: &DataManager,
        demos: &Demographics,
        history: &MarketHistory);
}

pub enum Something<'a> {
    val { thing: &'a dyn Actor }
}