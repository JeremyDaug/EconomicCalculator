//! The storage unit of population groups.
//! 
//! Used for any productive, intellegent actor in the system. Does not include animal
//! populations.
use barrage::{Sender, Receiver};

use crate::{demographics::Demographics, data_manager::{self, DataManager}};

use super::{desires::Desires, 
    pop_breakdown_table::PopBreakdownTable, 
    buyer::Buyer, seller::Seller, actor::Actor, market::MarketHistory, actor_message::{ActorMessage, ActorType, ActorInfo}, pop_memory::PopMemory};


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
    /// A breakdown of the Population's demographics.
    pub breakdown_table: PopBreakdownTable,
    // Mood
    /// Whether the pop is selling or not.
    pub is_selling: bool,
    /// The historical records (or rough estimate thereof).
    pub memory: PopMemory,
}

impl Pop {
    /// Takes the current population table, and updates desires to match the population
    /// breakdown. This is a hard reset, so is advised to call only as needed.
    /// 
    /// Does not take sub-groups of species, culture, ideology into account currently.
    /// This will need to be updated when those are implemented.
    pub fn update_desires(&mut self, demos: Demographics) {
        self.desires.clear_desires();
        // add in each species desires
        for row in self.breakdown_table.species_makeup().iter() {
            let species = demos.species.get(row.0).expect("Species Id Not Found!");
            for desire in species.desires.iter() {
                let upped_desire = desire.create_multiple(*row.1);
                self.desires.add_desire(&upped_desire);
            }
        }
        // placeholder for civilization
        // add in culture desires
        for row in self.breakdown_table.culture_makeup().iter() {
            if let Some(id) = row.0 {
                let culture = demos.cultures.get(id).expect("Culture Id Not Found!");
                for desire in culture.desires.iter() {
                    let upped_desire = desire.create_multiple(*row.1);
                    self.desires.add_desire(&upped_desire);
                }
            }
        }

        // add in ideology desires
        for row in self.breakdown_table.ideology_makeup().iter() {
            if let Some(id) = row.0 {
                let ideology = demos.ideology.get(id).expect("Ideology Id Not Found!");
                for desire in ideology.desires.iter() {
                    let upped_desire = desire.create_multiple(*row.1);
                    self.desires.add_desire(&upped_desire);
                }
            }
        }

        // add in movements
    }

    /// Get's an automatically generated name for the pop group.
    /// 
    /// TODO update to pass in data from elsewhere to get more useful names.
    /// Possibly add in an option no name them specially.
    pub fn id_name(&self) -> String {
        format!("Job:{}|Firm:{}|Market:{}", self.job, self.firm, self.market)
    }

    /// Get's the total number of people in this pop.
    pub fn count(&self) -> usize {
        self.breakdown_table.total
    }
}

impl Buyer for Pop {

}

impl Seller for Pop {
    fn actor_type(&self) -> ActorType {
        ActorType::Pop
    }

    fn actor_info(&self) -> ActorInfo {
        ActorInfo::Pop(self.id)
    }

    fn get_id(&self) -> usize {
        self.id
    }
}

impl Actor for Pop {
    /// Runs the market day for the pop.
    /// 
    /// Called by the pop's market.
    /// 
    /// Starts by waiting for the market to spin up (to keep things clean)
    /// then it begins pre-calculations. For pops this means looking at their
    /// situation (resources available, demographic habits, Workplace rules)
    /// to decide whether they will offer their goods for exchange, or not.
    /// 
    /// After Precalculation it works for it's job, giving it's time and either
    /// getting a pay-stub or their paycheck, whichever the job gives. (pay 
    /// stub is a placeholder for a payment to simplify transfers forward).
    /// 
    /// Once they recieve their pay from work, they enter their normal day,
    /// rotating between buying what they desire, and completing processes to 
    /// use/consume products to get wants.
    /// 
    /// If they are putting up things for sale, they will also add selling
    /// into the rotation, though they are much more limited in how they can
    /// handle it.
    /// 
    /// They continue this cycle until they run out of time to use, in which
    /// case they tell the market they're done and enter a holding pattern, 
    /// waiting for either buying messages or for the market day to end.
    /// 
    /// # Selling Notes
    /// 
    /// If they are offering stuff for exchange, they will send up messages
    /// for barter on everything they are offering.
    /// 
    /// What they offer for exchange are the products which are either
    /// - not desired at all.
    /// - not reserved.
    /// - is excess above the full_tier_satisfied.
    /// 
    /// Products offered for sale have their AMV price set at yesterday's
    /// price, though their sell mechanism is far more fluid.
    /// 
    /// Items which have an AMV below the value of their time will be 
    /// trashed instead, thrown to the market for anyone to pick up.
    /// 
    /// # Panics
    /// 
    /// Panics if it recieves any message before ActorMessage::StartDay
    /// to ensure the broadcast queue is open.
    fn run_market_day(&mut self, 
    sender: Sender<ActorMessage>,
    reciever: &mut Receiver<ActorMessage>,
    data: &DataManager,
    demos: &Demographics,
    history: &MarketHistory) {
        // started up, so wait for the first message.
        loop {
            match reciever.recv().expect("Channel Broke.") {
                ActorMessage::StartDay => break,
                _ => panic!("Pop Recieved something before Day Start. Don't do something before the day starts.")
            }
        }
        // precalculate our plans for the day based on yesterday's results and
        // see if we want to sell and what we want to sell.
        self.desires.sift_products();
        self.is_selling = if self.memory.is_disorganized { true }
        else {
            // if the separation between the highest full tier and the highest 
            // tier

            false
        };

        // Wait for our job to poke us, asking/telling us what to give them 
        // and send it all over (will likely get a short lived channel for this)
        // then wait for the firm to get back.
        // The firm will return either with a paycheck, paystub if a wage 
        // employee, or if it's a disorganized owner, it's share of everything.

        // Start free time section, roll between processing for wants, going 
        // out to buy things, and dealing with recieved sale orders.

        // Once time has run out, send up a finished message.
        sender.send(ActorMessage::Finished { 
            sender: self.actor_info() 
        }).expect("Channel Closed Unexpectedly!");
        // Then enter a holding pattern, continuing to consume from the 
        // message queue, and dealing with sale orders shortly.
        // When we recieve the AllFinished, we know no more messages will 
        // come, so stop holding and fall out, possibly recording data as
        // needed.
    }
}

