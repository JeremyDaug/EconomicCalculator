use super::process::Process;

#[derive(Debug)]
pub struct ProcessNode {
    pub process: Process,
    pub inputs: Vec<ProcessNode>,
    pub capital: Vec<ProcessNode>,
    pub output: Vec<ProcessNode>,
    pub can_feed_self: bool
}

impl ProcessNode {
    
}