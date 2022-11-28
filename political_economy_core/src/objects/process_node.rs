#[derive(Debug)]
pub struct ProcessNode {
    /// The Process we represent
    process: u64,
    /// the processes which output things we take in
    pub inputs: Vec<u64>,
    /// the processes which output things we take as capital
    pub capitals: Vec<u64>,
    /// the processes which take what we output
    pub outputs: Vec<u64>,
    /// whether we output our own inputs
    pub can_feed_self: bool
}

impl ProcessNode {
    pub fn new(process: u64) -> Self {
         Self { process: process, 
            inputs: vec![], 
            capitals: vec![], 
            outputs: vec![], 
            can_feed_self: false
        } 
    }


    pub fn process(&self) -> u64 {
        self.process
    }
}