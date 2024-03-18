///! Put on hold, I need to push forward. Come back to this later. This will 
///! likely replace the Process_Node stuff as it's more comprehensive.

use std::collections::HashMap;

use crate::{data_manager::DataManager, objects::data_objects::{process::ProcessSectionTag, item::Item}};

/// Product Network storage.
#[derive(Debug)]
pub struct ProductNetwork {
    /// map to indices for wants.
    want_idx: HashMap<usize, usize>,
    /// Map to indices for products.
    product_idx: HashMap<usize, usize>,
    /// The Items in the network, Should be sum of Products and Wants.
    items: Vec<Node>,
    /// All of the connections, should hold all processes and 
    connections: Vec<Connection>,
}

impl ProductNetwork{
    pub fn new() -> Self { 
        Self { 
            want_idx: HashMap::new(),
            product_idx: HashMap::new(),
            items: vec![], 
            connections: vec![] 
        } 
    }

    /// Adds product to our network, returns true if successful, false if it
    /// already exists in our network.
    fn add_product(&mut self, id: usize) -> bool {
        if self.product_idx.contains_key(&id) {
            // if already contained in network
            false
        } else {
            self.product_idx.insert(id, self.items.len());
            self.items.push(Node::new(ItemData::Proudct(id)));
            true
        }
    }

    /// Adds want to our network, returns true if successful, false if it
    /// already exists in our network.
    fn add_want(&mut self, id: usize) -> bool {
        if self.want_idx.contains_key(&id) {
            // if already contained in network
            false
        } else {
            self.want_idx.insert(id, self.items.len());
            self.items.push(Node::new(ItemData::Want(id)));
            true
        }
    }

    /// # Update Product Network
    /// 
    /// Given our current data, it creates a network of
    /// wants, products, and processes to make navigating
    /// what feeds to what easier.
    /// 
    /// Does not record efficiency or conversion rates, that's what
    /// the process itself is for.
    /// 
    /// Clears out old data as well. Only call after loading all 
    /// products, wants, and processes.
    pub fn update_product_network(&mut self, data: &DataManager) {
        self.connections.clear();
        self.items.clear();
        self.product_idx.clear();
        self.want_idx.clear();
        // add wants
        for (id, _) in data.wants.iter() {
            self.add_want(*id);
        }
        // and product nodes
        for (id, _) in data.products.iter() {
            self.add_product(*id);
        }
        // add ownership connections
        for (id, product) in data.products.iter()
        .filter(|(_, prod)| prod.wants.len() > 0) {
            // get the product's idx
            let prod_id = *self.get_prod_idx(id);
            // make a connection for the ownership.
            let mut conn 
                = Connection::new(ConnectionType::Ownership);
            // add our product as the input.
            conn.input_idx.push(prod_id);
            // get that connection's (eventual) index
            let conn_idx = self.connections.len();
            // iterate over the wants it produces
            for (want, _) in product.wants.iter() {
                // get that want's index
                let want_idx = *self.get_want_idx(want);
                // add that want to the outputs of the connection
                conn.output_idx.push(want_idx);
                // get that want's data
                let want_data = self.get_node_mut(want_idx);
                // and add the connection as incoming as well
                want_data.incoming.push(conn_idx);
            }
            // lastly, add connection to our data.
            self.connections.push(conn);
        }
        // go through processes
        for (process_id, process) in data.processes.iter() {
            let mut conn 
                = Connection::new(ConnectionType::Process(*process_id));
            let conn_idx = self.connections.len();
            for part in process.process_parts.iter() {
                match part.part {
                    ProcessSectionTag::Input |
                    ProcessSectionTag::Capital => {
                        // if input or capital, than it's an input 
                        match part.item {
                            Item::Product(id) => {
                                let idx = self.get_prod_idx(&id);
                                conn.input_idx.push(*idx); // add input's idx
                                let node = self.get_node_mut(*idx);
                                node.outgoing.push(conn_idx); // add connection as output
                            },
                            Item::Class(class) => {
                                let class_group = data.product_classes.get(&class).unwrap();
                                for id in class_group {
                                    let idx = self.get_prod_idx(&id);
                                    conn.input_idx.push(*idx); // add input's idx
                                    let node = self.get_node_mut(*idx);
                                    node.outgoing.push(conn_idx); // add connection as output
                                }
                            },
                            Item::Want(id) => {
                                let idx = self.get_want_idx(&id);
                                conn.input_idx.push(*idx); // add input's idx
                                let node = self.get_node_mut(*idx);
                                node.outgoing.push(conn_idx); // add connection as output
                            },
                        }
                    },
                    ProcessSectionTag::Output => {
                        match part.item {
                            Item::Product(id) => {
                                let idx = self.get_prod_idx(&id);
                                conn.output_idx.push(*idx); // add output's idx
                                let node = self.get_node_mut(*idx);
                                node.incoming.push(conn_idx); // add connection as input
                            },
                            Item::Class(class) => {
                                let class_group = data.product_classes.get(&class).unwrap();
                                for id in class_group {
                                    let idx = self.get_prod_idx(&id);
                                    conn.output_idx.push(*idx); // add output's idx
                                    let node = self.get_node_mut(*idx);
                                    node.incoming.push(conn_idx); // add connection as input
                                }
                            },
                            Item::Want(id) => {
                                let idx = self.get_want_idx(&id);
                                conn.output_idx.push(*idx); // add output's idx
                                let node = self.get_node_mut(*idx);
                                node.incoming.push(conn_idx); // add connection as input
                            },
                        }
                    },
                }
            }
        }
    }

    /// get's a product's index in our items.
    fn get_prod_idx(&self, id: &usize) -> &usize {
        self.product_idx.get(id).unwrap()
    }

    /// get's a want's index in our items.
    fn get_want_idx(&self, id: &usize) -> &usize {
        self.want_idx.get(id).unwrap() 
    }

    /// Get's Node, mutable.
    fn get_node_mut(&mut self, want: usize) -> &mut Node {
        self.items.get_mut(want).unwrap()
    }
}

/// # Node Struct
/// 
/// Node for products and wans in our graph.
#[derive(Debug, Clone)]
pub struct Node {
    /// The item in this node.
    _item: ItemData,
    /// Incoming Connections (this is an output)
    incoming: Vec<usize>,
    /// outgoing connections (this is an input)
    outgoing: Vec<usize>,
}

impl Node {
    pub fn new(item: ItemData) -> 
        Self { 
            Self { 
                _item: item, 
                incoming: vec![], 
                outgoing: vec![] 
            } 
        }
}
/// The item infor for a node.
#[derive(Debug, Copy, Clone)]
pub enum ItemData {
    /// The item is a want, with the id attached.
    Want(usize),
    /// The Item is a product, it's id is stored
    Proudct(usize)
}

/// the connection's info
#[derive(Debug, Clone)]
pub struct Connection {
    /// The Input Item(s)' index.
    input_idx: Vec<usize>,
    /// The type of connection, 
    /// 
    /// Process (contains process ID)
    /// or 
    /// Ownership (this is the input)
    _conn_type: ConnectionType,
    /// The index of the output item.
    output_idx: Vec<usize>
}

impl Connection {
    pub fn new(conn_type: ConnectionType) -> Self { 
        Self { input_idx: vec![], 
            _conn_type: conn_type, 
            output_idx: vec![]
        } 
    }
}

/// The type of the connection.
#[derive(Debug, Copy, Clone)]
pub enum ConnectionType {
    /// The process, with process ID
    Process(usize),
    /// An ownership source connection. Solo Input is the ownership product.
    Ownership,
}