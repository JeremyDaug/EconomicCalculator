use std::collections::{HashMap, HashSet, VecDeque};

use barrage::{Sender, Receiver};

use crate::{data_manager::DataManager, demographics::Demographics, objects::{data_objects::{item::Item, process::ProcessPartTag}, environmental_objects::market::MarketHistory}};

use super::{actor::Actor, actor_message::{ActorInfo, ActorMessage, ActorType, FirmEmployeeAction}, buyer::Buyer, firm_job::FirmJob, seller::Seller};

/// Firms are the productive actors of our system.
/// 
/// They can represent disorganized groups of solo firms, like family farms,
/// to large companies and corporations. When other groups need to act, they
/// have a firm contained within them to do the work.
/// 
/// They buy inputs for the jobs they have, and sell the goods produced at a
/// price. They are harder to please through barter, making indirect exchange
/// more encouraged.
#[derive(Debug)]
pub struct Firm {
    /// The Unique Id for a firm.
    pub id: usize,
    /// The Firm's name
    pub name: String,
    /// The name for a firm, used if the Firm is a child of another 
    /// firm or other organization.
    /// 
    /// Sub firms share their primary name.
    pub sub_name: String,
    /// What kind of firm it is, alters the logic of the firm 
    pub firm_kind: FirmKind,
    /// The rank of the firm. How high it is in it's firm tree. Also alters it's logic.
    pub firm_rank: FirmRank,
    /// How is the firm owned.
    pub ownership_type: OwnershipStructure,
    /// How the profit of the Firm is distributed.
    pub profit_structure: ProfitStructure,
    /// How the firm is organized, or how it attempts to organize itself.
    pub organization_structure: OrganizationalStructure,
    /// The ids for the firm's children (subfirms).
    /// May be empty.
    pub children: Vec<usize>,
    /// The id for a parent firm, Firm may not have a parent.
    pub parent: Option<usize>,
    /// The jobs in the firm. Rank 1 firms can only have 1 here.
    /// Also contains the pops and assignments.
    pub jobs: Vec<FirmJob>,
    /// The management jobs the firm uses. The first in the list is it's 
    /// preferred, the others are those which it either must have or which
    /// it is transitioning away from.
    /// 
    /// Also contains the pops and assignments.
    /// 
    /// Unavailable for Disorganized Firms.
    //pub management: Vec<FirmJob>,
    /// The ownership jobs the firm uses. The first is the primary and preferred
    /// option, the others are either required by the structure, or that is being
    /// transitioned away from.
    /// 
    /// Also contains the pops and assignments.
    //pub ownership: Vec<FirmJob>,
    /// The prices of the products the firm sells.
    /// Stores the ID of the product and the price in AMV it seeks from
    /// the market.
    pub prices: HashMap<usize, f64>,
    /// The property owned or otherwise managed by the firm.
    /// If the firm is not Disorganized or otherwise a distinct entity from
    /// the pop, this is where all of it's inputs and capital is stored.
    pub property: HashMap<usize, f64>,
    /// The property which was used as capital today and has been expended, making it
    /// no longer useful today.
    pub expended: HashMap<usize, f64>,
    /// The wants currently owned by the firm. Like Property, if the firm is Disorganized, 
    /// all wants will shift to the owner/workers instead of being stored.
    pub wants: HashMap<usize, f64>,
    /// Message Backlog for storing messages we aren't handling right this second.
    pub backlog: VecDeque<ActorMessage>,
    _firm_outputs: Vec<usize>,
}

impl Firm {
    /// # Get Production Requirements
    /// 
    /// Gets all of the products needed to meet the pre-existing plan.
    pub fn get_production_requirements(&self, data: &DataManager) 
    -> HashMap<Item, f64> {
        let mut result = HashMap::new();

        for job in self.jobs.iter() {
            for (proc, Assgn) in job.assignments.iter() {
                let process = data.processes.get(proc).expect("Process not found.");
                for part in process.input_and_capital_products().iter()
                .filter(|x| !x.is_optional()) {
                    // if not optional, add to our result
                    result.entry(part.item)
                    .and_modify(|x| *x += part.amount * Assgn.iterations)
                    .or_insert(part.amount * Assgn.iterations);
                }
            }
        }

        result
    }

    /// # Get Optional Production Goods
    /// 
    /// Get the goods which are considered optional for our needs. 
    pub fn get_optional_production_goods(&self, data: &DataManager)
    -> HashMap<Item, f64> {
        let mut result = HashMap::new();

        for job in self.jobs.iter() {
            for (proc, Assgn) in job.assignments.iter() {
                let process = data.processes.get(proc).expect("Process not found.");
                for part in process.input_and_capital_products().iter()
                .filter(|x| x.is_optional()) {
                    // if not optional, add to our result
                    result.entry(part.item)
                    .and_modify(|x| *x += part.amount * Assgn.iterations)
                    .or_insert(part.amount * Assgn.iterations);
                }
            }
        }

        result
    }

    /// # Get Production Output
    /// 
    /// Get the products which we expect to output from our processes.
    pub fn get_production_goods(&self, data: &DataManager)
    -> HashMap<Item, f64> {
        let mut result = HashMap::new();

        for job in self.jobs.iter() {
            for (proc, assgn) in job.assignments.iter() {
                let process = data.processes.get(proc).expect("Process not found.");
                for part in process.outputs().iter() {
                    // if not optional, add to our result
                    result.entry(part.item)
                        .and_modify(|x| *x += part.amount * assgn.iterations)
                        .or_insert(part.amount * assgn.iterations);
                }
            }
        }

        result
    }

    /// # Get full name
    /// 
    /// Shorthand function to get the firm's full name.
    pub fn get_full_name(&self) -> String {
        format!("{}({})", self.name, self.sub_name)
    }

    /// A helper function to push a message to the market.
    /// Safely pushes without blocking.
    ///
    /// Does a quick message catchup, reading up to the newest messages and 
    /// adding any for us to the backlog.
    /// 
    /// Afterwards, it tries to push the message.If it fails, it tries the 
    /// entire thing again.
    ///
    /// ## Panics
    ///
    /// If the send fails due to a disconnect.
    pub fn push_message(&mut self, rx: &Receiver<ActorMessage>, tx: &Sender<ActorMessage>,
    msg: ActorMessage) {
        loop {
            // try to clear out prior msgs before sending.
            self.quick_msg_catchup(rx);
            let result = tx.try_send(msg);
            if let Ok(_) = result {
                break; // if message got sent out, break.
            }
            else if let Err(msg) = result {
                match msg { // failed to send, check why
                    // If disconnected, panic, there's nothing more we can do.
                    barrage::TrySendError::Disconnected(_) => panic!("Unexpected Disconnect"),
                    barrage::TrySendError::Full(_) => (), // if just full, consume and try again.
                }
            }
        };
    }
    
    /// # Quick Message Catchup
    ///
    /// A shorthand function.
    ///
    /// Quickly consumes all messages from the queue it can, catching up
    /// with the current back of the queue.
    ///
    /// If it finds something for us, it puts it in the backlog for later consumption.
    ///
    /// This focuses on keeping the Broadcast Queue open to ensure it doesn't get backed
    /// up too much.
    pub fn quick_msg_catchup(&mut self, rx: &Receiver<ActorMessage>) {
        loop {
            let result = rx.try_recv()
                .expect("Unexpected Disconnect"); // if disconnected, panic.

            if let Some(msg) = result { // if we recieved a message, check it's for us
            //if cfg!(debug_assertions) { println!("Pop {} recieves: {}", self.id, msg); }
                if msg.for_me(self.actor_info()) {
                    self.backlog.push_back(msg); // if it's for us, push it to the backlog.
                }
            }
            else { // if no messsage in queue, we've caught up so break out.
                return;
            }
        }
    }
    
    /// A shorthand function to recieve the next message from the queue for us.
    /// It returns it to us instead of putting it in the backlog.
    pub fn get_next_message(&self, rx: &Receiver<ActorMessage>) -> ActorMessage {
        loop {
            let msg = rx.recv().expect("Unexpected Disconnect.");
            if msg.for_me(self.actor_info()) {
                return msg;
            }
        }
    }
    
    /// # Active Wait
    ///
    /// Active Wait Function, used whenever we need to wait for a particular
    /// result or message. Takes in all the standard stuff for free time, while
    /// also taking in whatever it's looking to find. If it recieves one of the
    /// message types requested, it returns it.
    ///
    /// If it gets a message other than what it's looking for, it deals with it
    /// via the process_common_message.
    ///
    /// This only returns if it recieves the message it's looking for, it it does
    /// not recieve it, it will be stuck in it's loop.
    ///
    /// It ignores the data of the message, only looking at the message type
    /// and that it is for us.
    /// 
    /// ## Panics if message channel breaks.
    /// 
    /// ## Notes
    /// 
    /// As it stands, only some products would ever be sought by us. All of them 
    pub fn active_wait(&mut self,
    rx: &mut Receiver<ActorMessage>,
    tx: &Sender<ActorMessage>,
    data: &DataManager,
    market: &MarketHistory,
    find: &Vec<ActorMessage>) -> ActorMessage {
        loop {
            // catchup on messages for good measure
            self.quick_msg_catchup(rx);
            // next deal with the first backlog
            let popped = self.backlog.pop_front();
            if let Some(msg) = popped {
                let result = self.process_common_msg(rx, tx, data, market, msg);

                if let Some(msg) = result {
                    // if we recieved something back, then check it's what we should be getting back.
                    if find.iter()
                    .any(|x| std::mem::discriminant(x) == std::mem::discriminant(&msg)) {
                        return msg; // if yes, return the message.
                    } else {
                        // if somehow not, panic, this should NEVER happen.
                        panic!("Pop {} panicked! It recieved {} instead of what it should've and couldn't handle it.", 
                            self.id, msg);
                    }
                }
            }
        }
    }

    /// # Exclusive Wait function.
    ///
    /// First checks the backlog for the message requested, if it finds it, 
    /// it extracts it and returns that. If it doesn't, then it 
    /// Waits on a specific message or messages to be recieved directed for us.
    /// If it's any other message for us, it's put onto the backlog.
    ///
    /// Meant to be used primarily when we are locked into a state where we
    /// shouldn't respond to anything else but what we're focusing on.
    /// 
    /// May be improved by making find work with incomplete ActorMessages or some
    /// other mechanism that removes the need to created dummies to make it work.
    pub fn exclusive_wait(&mut self,
    rx: &mut Receiver<ActorMessage>,
    find: &Vec<ActorMessage>) -> ActorMessage {
        // TODO Look into improving Find Parameter so it doesn't need a fully filled out ActorMessage to function.
        // first, look through the backlog to ensure we haven't already gotten what we're looking for.
        for idx in 0..self.backlog.len() {
            if find.iter()
            .any(|x| {
                std::mem::discriminant(x) == std::mem::discriminant(&self.backlog[idx])
            }) {
                return self.backlog.remove(idx).expect("Somehow walked off end of backlog.");
            }
        }
        loop {
            let result = rx.try_recv()
                .expect("Unexpected Disconnect.");
            if let Some(msg) = result {
                // if not for me, skip, don't add to our backlog.
                if !msg.for_me(self.actor_info()) { continue; }
                // if for me and matches what we're looking for, return.
                if find.iter()
                .any(|x| std::mem::discriminant(x) == std::mem::discriminant(&msg)) {
                    return msg;
                } else { // if not what we're looking for right now, put onto backlog.
                    self.backlog.push_back(msg);
                }
            } 
        }
    }
    
    /// # Process Common Messages
    /// 
    /// All messages that can be handled at any time (reactively)
    fn process_common_msg(&mut self, 
    _rx: &mut Receiver<ActorMessage>, 
    _tx: &Sender<ActorMessage>,
    _data: &DataManager, 
    _market: &MarketHistory, 
    msg: ActorMessage) -> Option<ActorMessage> {
        match msg {
            ActorMessage::FoundProduct { seller,
            buyer: _, product: _ } => {
                // if we are the seller, enter sell state here.
                if seller == self.actor_info() {
                    // sell product
                } else {
                    return Some(msg);
                }
            },
            ActorMessage::SendProduct { sender: _, 
            reciever: _, product, amount } => {
                // it's for us, so add.
                self.property.entry(product)
                    .and_modify(|x| *x += amount)
                    .or_insert(amount);
            },
            ActorMessage::SendWant { sender: _, 
            reciever: _, want, amount } => {
                self.wants.entry(want)
                    .and_modify(|x| *x += amount)
                    .or_insert(amount);
            },
            _ => {
                // Start Day, We recieve only at day start
                // Finished, We send out
                // All Finished, Recieved at day end.
                // Sell Order, Sent by us
                // Find Product, Sent by us
                // Find Class, we send out
                // Find Want, we send out.
                // Product Not Found, explicitly waited on
                // Class Not Found, explicitly waited on
                // Want Not Found, Explicitly Waited on
                // Found Class, always waited on.
                // Found Want, always waited on.
                // In Stock, Sent and waited on explicitly.
                // Not In Stock, Sent and waited on Explicitly.
                // Ask Barter Hint, Sent and Waited on Explicitly.
                // Barter Hint, sent and Waited on Explicitly.
                // Reject Purchase, Explicit both ways.
                // Buy Offer
                // Buy Offer Followup
                // Seller Accept Offer As Is
                // Offer Accepted With Change
                // Change Followup
                // Reject Offer
                // Finished Deal
                // Close Deal
                // Check item, followup in deal space.
                // Want Splash, firms should not absorb any splash
                // Dump Product, sent, not recieved.
                // Firm to Employee, only sent, never recieved
            }
        }
        None
    }

    /// # Work Time Processing
    /// 
    /// Work time processing does the standard work needed of the firm.
    pub fn work_time_processing(&mut self, 
    rx: &mut Receiver<ActorMessage>, 
    tx: &Sender<ActorMessage>, 
    data: &DataManager, 
    demos: &Demographics,
    history: &MarketHistory) {
        let actor_info = self.actor_info();
        if let OrganizationalStructure::Disorganized 
        = self.organization_structure {
            // if disorganized, ask for everything
            let pop = ActorInfo::Pop(self.jobs.first().expect("Disorganized Firm Has No jobs?")
            .pop);
            self.push_message(rx, tx, 
                ActorMessage::FirmToEmployee { 
                    firm: self.actor_info(), employee: pop, 
                    action: FirmEmployeeAction::RequestEverything });
            // products and wants sent by this are recievd via SendWant and SendProduct msgs
            // these sendings end when EmployeeToFirm sends the RequestSent action. 
            // Only 1 EtF msg should be recieved, it should be from our employee and say RequestSent
            let response = self.active_wait(rx, tx, data, history, &vec![
                ActorMessage::EmployeeToFirm { employee: ActorInfo::Firm(0), firm: self.actor_info(), 
                    action: FirmEmployeeAction::RequestSent }
            ]);
            // Sanity check via debugs. These should never fire under normal circumstances
            if let ActorMessage::EmployeeToFirm { employee, firm, 
            action } = response {
                debug_assert!(employee == pop, "Employee doesn't match.");
                debug_assert!(firm == self.actor_info(), "Firm doesn't match.");
                debug_assert!(action == FirmEmployeeAction::RequestSent, "Action Returned does not match.");
            }// don't look for others, it can't come.
            // with RequestSent recieved and all that we need here, follow our plans and do our stuff.
            let plan_results = self.do_plan(data, demos, history);
            // with our plan carried out to the best of our ability, return everything to the pop.
            let prop_copy = self.property.clone();
            for (product, amount) in prop_copy.into_iter() {
                // send to pop
                self.push_message(rx, tx, 
                ActorMessage::SendProduct { sender: actor_info, 
                    reciever: pop, product: product, amount: amount });
            }
            let want_copy = self.wants.clone();
            for (want, amount) in want_copy.into_iter() {
                self.push_message(rx, tx, 
                ActorMessage::SendWant { sender: actor_info, 
                    reciever: pop, want, amount });
            }
            // With want and products sent back, clear out current
            //* NOTE: Expended capital is not included and should be included in the pop buy request.
            self.property.clear();
            self.wants.clear();
            // TODO Pick up here !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            // given our results, reduce downwards to match our abilities.
            // send over the needs of the firm to the pops so they can buy what we need as well.
            // then exit work_time_processing
        } else { // the firm is organized, therefore labor is properly used. Send everything.
            // if payday, send pop agreed upon wage
            // always requests for their time and skills (record who sent what and how much)
            // Try to do our plan to the best of our abilities.
            // return any skills earned and whatever splashed onto them.
            // then we're done being productive for the day. Move on.
        }
    }

    /// # Do Plan
    /// 
    /// Does the production plan laid out previously.
    /// 
    /// Priority is defined by the order of the job and nothing else.
    /// Instead of priorities expecting to fail, it creates limited plans
    /// it can succeed with.
    /// 
    pub fn do_plan(&mut self, data: &DataManager, 
    demos: &Demographics,
    history: &MarketHistory) -> PlanResults {
        // The results (success or failure) of our processes so we can build
        // or reduce as needed.
        let mut plan_results: HashMap<usize, HashMap<usize, f64>> = HashMap::new();
        // expenses in the form of specific products, wants are not counted
        let mut expenses: HashMap<usize, f64> = HashMap::new();
        // The capital goods locked up in plan can be gotten from self.expended
        // Total production of goods from our proceses
        let mut production:HashMap<usize, f64> = HashMap::new();
        // The final net results of production to allows us to do some economic calculation.
        //let mut net = HashMap::new();
        for job in self.jobs.iter() {
            let pop_id = job.pop;
            plan_results.insert(pop_id, HashMap::new());
            for (proc_id, i) in job.assignments.iter() {
                plan_results.get_mut(&pop_id).unwrap()
                    .entry(*proc_id)
                    .and_modify(|x| *x += i.iterations)
                    .or_insert(i.iterations);
                let proc = data.processes.get(proc_id).expect("Process not found");
                let proc_results = proc.do_process(&self.property, 
                    &self.wants, 0.0, Some(i.iterations), 
                    false, data);
                // with results gotten, deal with changes
                for (prod, change) in proc_results.input_output_products.iter() {
                    let temp = self.property.entry(*prod)
                        .and_modify(|x| *x += change)
                        .or_insert(*change);
                    if *change < 0.0 { // record expenses
                        expenses.entry(*prod)
                        .and_modify(|x| *x -= change)
                        .or_insert(-change);
                    } else { // and production
                        production.entry(*prod)
                        .and_modify(|x| *x += change)
                        .or_insert(*change);
                    }
                    debug_assert!(*temp > 0.0, "Got to negative value of product.");
                }
                // expend capital
                for (prod, change) in proc_results.capital_products.iter() {
                    *self.property.get_mut(prod).unwrap() -= change;
                    let temp = self.expended.entry(*prod)
                        .and_modify(|x| *x += change)
                        .or_insert(*change);
                    debug_assert!(*temp > 0.0, "Got to negative product expended.");
                }
                // add/remove wants
                for (want, change) in proc_results.input_output_wants.iter() {
                    let temp = self.wants.entry(*want)
                        .and_modify(|x| *x += change)
                        .or_insert(*change);
                    debug_assert!(*temp > 0.0, "Got to negative Wants used.");
                }
            }
        }
        // Package our results and return
        PlanResults {
            expenses,
            production,
            used: self.expended.clone(),
            plan_results,
        }
    }
    
    /// # Buy and Sell Processing
    /// 
    /// This function has 2 purposes. Selling it's outputs and buying up it's 
    /// inputs.
    /// 
    /// It wants to focus on getting it's inputs primarily. Once it does, it 
    /// just holds, focusing on selling.
    /// 
    /// This function is unused by Firms which are disorganized as 
    /// disorganize firms transfer all goods to the pop it employs
    fn buy_and_sell_processing(&self, 
    rx: &mut Receiver<ActorMessage>, tx: &mut Sender<ActorMessage>, 
    data: &DataManager, demos: &Demographics, history: &MarketHistory) {
        if self.organization_structure == OrganizationalStructure::Disorganized {
            // if disorganized, we don't sell anything. Consider sending our plan needs for tomorrow to the
            // pop for them to buy for us.
            return;
        }
        // First, put our products up for sale on the market at our selected price
        // then, when we have enough AMV, go out and try to purchase the inputs we need
        // once all inputs are purchased to the best of our ability, send our done message to the market
        // then hold until we either sell out
        // or we recieve the message that the market day has ended.
        todo!()
    }
}

impl Seller for Firm {
    fn actor_type(&self) -> ActorType {
        ActorType::Firm
    }

    fn get_id(&self) -> usize {
        self.id
    }

    fn actor_info(&self) -> ActorInfo {
        ActorInfo::Firm(self.id)
    }
}

impl Buyer for Firm {}

impl Actor for Firm {
    /// Run the market day of the firm.
    /// 
    /// First waits to get the all clear on startup to enusre all other actors
    /// in it's market are running.
    /// 
    /// Second, it puts up what it offers today up for sale.
    /// 
    /// Third, rotate between Buying what we need to work,
    /// doing processes for the day, and completing transactions to (don't 
    /// block on checking for selling, just go to the end of the current 
    /// queue) sell what we're offering. Continue until all our processes 
    /// are done, and we've bought (or failed to) buy everything we wanted.
    /// 
    /// Fourth, continue checking and completing selling, while also 
    /// cogitating with remaining time and labor. This means doing any
    /// market research, corporate espionage, advanced calculation,  
    /// business organization/messaging or the like. Complete this until 
    /// all cogitating time is out.
    /// 
    /// Lastly, send out an ActorMessage::Finished to the market and enter 
    /// a holding pattern, completing sales remaining and observing for 
    /// messages.
    /// 
    /// Once we get the AllFinished message, complete any remaining cleanup, 
    /// and close out.
    fn run_market_day(&mut self, 
        tx: &mut Sender<ActorMessage>,
        rx: &mut Receiver<ActorMessage>,
        data: &DataManager,
        demos: &Demographics,
        history: &MarketHistory) {
        // TODO idea, Firms hire retailers who handle the details of sales and then report their results back to here. They are on separate threads. THis is a bad, crazy idea, but fuckit it may just work.
        
        // Prep for the day. Firms not much, if anything, should be needed.

        // Started up, so wait for the start signal.
        match rx.recv().expect("Channel Broke.") {
            ActorMessage::StartDay => (),
            _ => panic!("Recived something before DayStart. Panic.")
        }

        // go to work day processing, buy work from employees and do any transfers there
        // Work day also includes doing any processes and work and putting out sell orders if the firm is selling.
        self.work_time_processing(rx, tx, data, demos, history);

        //* Note: Disorganized Firms skip a lot of what follows, after they do their local work, they send all 
        // their stuff back, possibly minus time to plan things out further.

        // After the core work time is done, do any shopping needed for the next day's processes,
        // getting a bit extra to ensure decay doesn't hit too hard.
        // during this time we also regularly ensure we handle deals.
        // Send Finish Message after we're done here.
        self.buy_and_sell_processing(rx, tx, data, demos, history);

        // during the end of day wrap-up, don't consume anything

        // Decay our goods.

        // Then adapt today's plan for tomorrow.

        tx.send(ActorMessage::Finished { sender: self.actor_info() })
            .expect("Channel Closed Unexpectedly!");

        // Enter holding pattern for the end of the day.
        self.active_wait(rx, tx, data, history, &vec![
            ActorMessage::AllFinished
        ]);
    }
}

#[derive(Debug)]
pub enum FirmRank {
    /// Firms are the smallest kind of business.
    /// Capable of only a few jobs, a primary job, a management job, and
    /// an owner job. Little else.
    /// 
    /// Can only exist in a single market, and lacks the ability to form
    /// proper subdivisions, and finds it difficult to research new things
    /// actively.
    Firm,
    /// The Second Rank, capable of internal specialization through subfirms
    /// as well as expansion into other markets. It also has access to more 
    /// jobs, and more levels of management, allowing for greater 
    /// organizational gains.
    Company,
    /// Third Rank of a firm. More Options, More Expansion, more self
    /// contained specialization, management, and ownership options.
    /// 
    /// Can become an institution under the right circumstances.
    Corporation,
    /// Fourth rank, Even more of the above, but also capable of becoming a
    /// full state under the right circumstances, and easily becomes an
    /// institution.
    Megacorporation
}

/// What kind of firm this is.
/// 
/// This defines the overarching logic for how the firm will function, as well
/// as enable or disable certain features.
#[derive(Debug)]
pub enum FirmKind {
    /// The default or generic option, for those with little or no special
    /// logic.
    Workshop,
    /// A shop which buys local goods in order to store and resell them
    /// at a profit.
    Retailer,
    /// A Local merchant, IE, a merchant who travels between his home market
    /// and it's neigbors. They buy goods, transport them across the market 
    /// border, and sells them on the other side.
    Merchant,
    /// A Long Distance Merchant. The firm travels between various markets,
    /// buying and selling goods for a profit, only to travel on and repeat.
    Trader,
    /// A Firm specialized in advancing the destruction of products, like
    /// turning swords into iron for other uses.
    Scrapper,
    /// A Subsistence Firm, focuses it's logic on supporting itself alone.
    /// Does not normally offer goods to the market, instead creating offers
    /// for others to find and sells directly from the pop to the market.
    Subsistence,
    /// This firm, and the people in it are nomadic, capable of travelling
    /// between various markets, seeking the highest return for their work.
    Nomadic,
    /// A Combination of Subsistence firm and Nomadic firm. They travel 
    /// around, but instead of seeking higher profits, they optimize for 
    /// raw output. Herding is is a good example.
    SubsistenceNomad,
    /// The firm is a cultural firm, producing something that is cultural
    /// in nature, like a church or similar institution. As such, it typically
    /// offers services for cheap or free, and asks for charity in return.
    Cultural,
    //Political,
    /// The firm is not actually a firm, but instead it represents a military
    /// force. It does not produce regular goods, but instead creates violence.
    /// Depending on it's use, it can be used for security, opression, or theft.
    Military,
}

/// How the profits of the firm are distributed.
#[derive(Debug)]
pub enum ProfitStructure {
    /// The profits are distributed equally to all who own, work, or otherwise
    /// are attached to the firm. This is often for LossSharing, Disorganized,
    /// or other self-owned firms.
    Distributed,
    /// The firm has shares, and uses those to distribute profits.
    Shares,
    /// There is one share, and it belongs to the owner pop of a firm.
    PrivatelyOwned,
    /// The profits are shared out, but the owners get the lions share.
    ProfitSharing,
    /// Profits are not distributed, but are used to either invest in the
    /// company or to reduce prices.
    NonProfit
}

/// How the firm's ownership is structured.
/// 
/// Defines some features available to the firm.
#[derive(Debug)]
pub enum OwnershipStructure {
    /// It is not a structly organized firm, but instead a collection of
    /// small firms that are not working together. Think family farms.
    /// 
    /// Can only have a single job, no others.
    /// 
    /// Lacks any efficiency gain methods via organization.
    /// 
    /// Must use 
    /// - OrganizationalStructure::DisorganizedStructure
    /// - WageType::LossSharing
    /// - ProfitStructure::Distributed
    /// 
    /// Not strictly limited, but should avoid
    /// - FirmKind::Retailer
    /// - FirmKind::Merchant
    /// - FirmKind::Trader
    SelfEmployed,
    /// A Loose organization of individual or small business owner/operators.
    /// 
    /// Can be used at any level above Firm.
    /// 
    /// The children firms are self-owning, and the owners of those firms
    /// each have an equal share in their Association Parent.
    Association,
    /// A more formal and tightly organized collection of individual or
    /// small business owner/operators.
    /// 
    /// Technically available above any level of firm, but most common at
    /// Company level.
    /// 
    /// Tends to be limited to a singular Market.
    /// 
    /// Child Firms are self-directing in the day-to-day, but are limited in
    /// what they offer, and have restrictions on how they set prices.
    /// 
    /// Guilds also are ruled primarily by the most skilled members.
    Guild,
    /// The Firm is a private business. It is owned by one or a small group of
    /// people, the owners.
    /// 
    /// They are capable of anything, and have no restrictions in what they
    /// may do, but all choices are made by the owner, and for their profit.
    /// 
    /// While owning a Private firm gives the owner total control and access
    /// to all profits, he is also liable for any debts or liabilities.
    Private,
    /// The Firm is a Worker Cooperative, with the workers being given
    /// partial ownership of the firm. 
    /// 
    /// All decisions are available for influence from the workers, and there
    /// is no strict 'owner'. Managers are given more day-to-day running power.
    /// 
    /// May be of any scale.
    /// 
    /// There is no 'owner' job in these firms.
    /// 
    /// Profit must be destributed among the owners (workers) and these shares
    /// cannot be bought, sold, or otherwise exchanged. They are created upon
    /// joining and lost upon leaving.
    /// 
    /// If a Firm is a WorkerCooperative, then all of that firm's children must
    /// also be a WorkerCooperative.
    WorkerCooperative,
    /// The firm is a Cooperative for producers, for their benefit. As such
    /// grain farmers collectively owning a mill, or warehouse for storing
    /// their grain.
    /// 
    /// The Child Firms of this organization may be of any kind.
    /// 
    /// Producer Cooperatives are controlled by their children. The ownership
    /// of the parent is split among the owners of the children. They are
    /// typically non-profit organizations, with the purpose of passing on 
    /// savings and profits to their member-owners.
    /// 
    /// This should be always of Company Level or Higher.
    ProducerCooperative,
    /// The firm is a Consumer Cooperative.
    /// 
    /// The child firms of this firm may be in any form.
    /// 
    /// Consumer Cooperatives are controlled by their children. And Ownership
    /// is split among it's members equally. They focus on non-profit and 
    /// try to pass along any savings to the consumers who own them.
    /// 
    /// This should always be of company level or higher.
    ConsumerCooperative,
    /// The Firm is publicly traded. It has a number of shares which are
    /// freely bought, traded, and sold. These shares can include or exclude
    /// - voting rights (ownership controls)
    /// - Dividends (Share of Profit)
    /// - other rights or liabilities
    /// 
    /// Shares and their mechanics are explained elsewhere.
    /// 
    /// The Shareholders are the ultimate owners. The Owner position is instead
    /// replaced by the Board of Directors, who are selected by the 
    /// Shareholders. The highest level Managers become the Chief Officers
    /// (CEO, COO, CFO, etc).
    /// 
    /// While not explicitly restricted to any level, this tends to be
    /// more common the larger and higher ranked the company.
    PubliclyTraded,
    /// The firm is controlled by various stakeholders, often this comes
    /// through a mixture of shareholders, worker representation,
    /// consumer or producer representatives, and the like. The exact
    /// mixture is defined by the firm in question.
    Stakeholders,
    /// The firm is explicitly a state owned and run organization. There is no
    /// explicit owner, though there may be a Board to fulfill the role.
    /// 
    /// Managers are Bureaucrats, selected by the state, and answerable to
    /// them.
    /// 
    /// Employees are considered state employeees, and likewise answerable to
    /// the state.
    StateOwned,
    /// The firm is owned by a specific institution. Typically, this is used
    /// to distinguish between a state made entity and a firm or similar actor
    /// which is integral to a specific Institution.
    /// 
    /// For Example, a state with an integrated Church may have it's churches
    /// considered institutional and belong to the Church rather than being
    /// being under the direct purview of the State as a whole.
    /// 
    /// State Firms can become Institutional as management is delegated and
    /// ownership is concentrated.
    /// 
    /// Institutional Firms can also become state firms if the process
    /// reverses, but is less likely as institutions fight to hold on
    /// more strongly than the state.
    Institutional,
}

/// An enum which defines how a firm organizes itself and it's children,
/// as well as how tightly it and it's children are bound together.
#[derive(Debug, PartialEq, Eq)]
pub enum OrganizationalStructure {
    /// The firm is not organized at all, it is a collection of small
    /// business in a market.
    Disorganized,
    /// The firm is a small, solo busines, typically a small business with a
    /// few employees. It cannot have children. Think a local workshop or
    /// mom and pop diner style.
    SmallBusiness,
    /// A local firm capable of having children. Those children are owned
    /// and operated separately, but can take orders from the guild to 
    /// change, their operations, typically adjusting prices and wages.
    /// 
    /// Tends to stay at Firm Rank 0 or 1 (Firm or Company).
    /// 
    /// The guild also offers minor services, giving them a small efficiency
    /// boost.
    Guild,
    /// The firm intentionally reduces it's hight, minimizing middle 
    /// management. Upper management are closer to employees and employees
    /// can me shifted to middle management roles trivially.
    /// 
    /// Predominantly used in Companies and Firms. (1 and 0)
    Flat,
    /// Divides the company into subcomponents focused on particular jobs
    /// or duties, like marketing, R&D, Sales, Operations, etc.
    /// 
    /// Authority is centralized into the higher levels, with few local
    /// manageres having much authority.
    /// 
    /// Tends to be used for Companies and Corporations.
    Functional,
    /// Organizes itself around products, projects, and subsidiaries rather
    /// than specifically specialized sections. Divisions are given much more
    /// independence.
    /// Very Common.
    Divisional,
    /// The Firm is organized around a Franchise model, where the children are 
    /// typically copy pasted firms, with high managerial independence, but 
    /// low product indepence. The Parent Firm decides what is offered, at what
    /// price, and also typically sets up supply deals for material inputs and
    /// capital.
    /// 
    /// Very Common.
    Franchise,
    /// The Firm is Decentralized. Lower Firm Ranks are effectively 
    /// independent from the higher ranks, capable of doing almost anything
    /// on their own. Higher Ranks are instead organizational placeholders,
    /// or firms which produce something used among the lower levels.
    /// Higher levels have little to no authority.
    /// 
    /// Uncommon, typically unstable also.
    Decentralized,
}

/// # Plan Results
/// 
/// A helper 
pub struct PlanResults {
    /// All expended products
    pub expenses: HashMap<usize, f64>,
    /// All products created by today's plans.
    pub production: HashMap<usize, f64>,
    /// All capital used up (not destroyed).
    pub used: HashMap<usize, f64>,
    /// The results of the iterations.
    pub plan_results: HashMap<usize, HashMap<usize, f64>>
}