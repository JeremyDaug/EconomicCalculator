use std::{collections::HashMap};

use super::{product::Product, market::Market};

#[derive(Debug)]
pub struct Territory {
    name: String,
    coastal: bool,
    lake: bool,
    size: u64,
    land: u64,
    // water is calculated from size - land
    market: Market,
    plots: HashMap<Product, u64>,
    nodes: Vec<Node>,
    resources: HashMap<Product, f64>
}

impl Territory {
    pub fn name(&self) -> &str {
        self.name.as_ref()
    }

    pub fn coastal(&self) -> bool {
        self.coastal
    }

    pub fn lake(&self) -> bool {
        self.lake
    }

    pub fn size(&self) -> u64 {
        self.size
    }

    pub fn land(&self) -> u64 {
        self.land
    }

    pub fn sea(&self) -> u64 {
        self.size - self.land
    }

    pub fn market(&self) -> &Market {
        &self.market
    }

    pub fn plots(&self) -> &HashMap<Product, u64> {
        &self.plots
    }

    pub fn nodes(&self) -> &[Node] {
        self.nodes.as_ref()
    }

    pub fn resources(&self) -> &HashMap<Product, f64> {
        &self.resources
    }
}

#[derive(Debug)]
pub struct Node {
    resource: Product,
    stockpile: f64,
    depth: u64
}

impl Node {
    pub fn depth(&self) -> u64 {
        self.depth
    }

    pub fn stockpile(&self) -> f64 {
        self.stockpile
    }

    pub fn resource(&self) -> &Product {
        &self.resource
    }
}

#[derive(Debug)]
pub struct NeighborConnection{
    neighbor: Territory,
    distance: f64,
    connection_type: ConnectionType
}

impl NeighborConnection {
    pub fn neighbor(&self) -> &Territory {
        &self.neighbor
    }

    pub fn set_neighbor(&mut self, neighbor: Territory) {
        self.neighbor = neighbor;
    }

    pub fn distance(&self) -> f64 {
        self.distance
    }

    pub fn connection_type(&self) -> &ConnectionType {
        &self.connection_type
    }
}


/// The kind of connection between two (non-trivially connected) territories.
#[derive(Debug)]
pub enum ConnectionType {
    /// A land connection, allowing for even foot travel.
    Land,
    /// Sea travel requiring ships or other aquatic locomotion.
    Sea,
    /// Air connection, requiring flight to reach.
    Air,
    /// A space connection, requiring the ability to travel through a vacuum.
    Space,
    /// A tunnel connection (magical or not) which crosses the intervening space
    Tunnel
}