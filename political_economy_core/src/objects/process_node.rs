#[derive(Debug)]
pub struct ProcessNode {
    /// The Process we represent
    process: usize,
    /// the processes which output things we take in
    pub inputs: Vec<usize>,
    /// the processes which output things we take as capital
    pub capitals: Vec<usize>,
    /// the processes which take what we output
    pub outputs: Vec<usize>,
    /// whether we output our own inputs
    pub can_feed_self: bool
}

impl ProcessNode {
    pub fn new(process: usize) -> Self {
         Self { process: process, 
            inputs: vec![], 
            capitals: vec![], 
            outputs: vec![], 
            can_feed_self: false
        } 
    }


    pub fn process(&self) -> usize {
        self.process
    }
}