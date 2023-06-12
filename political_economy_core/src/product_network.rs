/// # Node Struct
/// 
/// Node for products and wans in our graph.
#[derive(Debug, Copy, Clone)]
pub struct Node {
    item: ItemData,
    incoming: Vec<usize>,
    outgoing: Vec<usize>,
}
/// The item infor for a node.
#[derive(Debug, Copy, Clone)]
pub enum ItemData {
    Want(usize),
    Proudct(usize)
}

/// the connection's info
#[derive(Debug, Copy, Clone)]
pub struct Connection {
    input_idx: usize,
    conn_type: ConnectionType,
    output_idx: usize
}

/// The type of the connection.
#[derive(Debug, Copy, Clone)]
pub enum ConnectionType {
    Process(usize),
    Ownership,
}