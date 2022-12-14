use std::collections::HashMap;

#[derive(Debug)]
pub struct Market {
    pub id: usize,
    pub name: String,
    pub firms: Vec<usize>,
    pub pops: Vec<usize>,
    pub territories: Vec<usize>,
    pub neighbors: HashMap<usize, f64>,
    pub resources: HashMap<usize, f64>,
    pub market_prices: HashMap<usize, f64>,
    pub products_for_sale: HashMap<usize, f64>,
    pub product_sold: HashMap<usize, f64>,
    pub product_output: HashMap<usize, f64>,
    pub product_exchanged_total: HashMap<usize, f64>
}

impl Market {
    pub fn id(&self) -> usize {
        self.id
    }

    pub fn name(&self) -> &str {
        self.name.as_ref()
    }

    pub fn firms(&self) -> &Vec<usize> {
        &self.firms
    }

    pub fn pops(&self) -> &Vec<usize> {
        &self.pops
    }

    pub fn territories(&self) -> &[usize] {
        self.territories.as_ref()
    }

    pub fn neighbors(&self) -> &HashMap<usize, f64> {
        &self.neighbors
    }

    pub fn resources(&self) -> &HashMap<usize, f64> {
        &self.resources
    }

    pub fn products_for_sale(&self) -> &HashMap<usize, f64> {
        &self.products_for_sale
    }

    pub fn market_prices(&self) -> &HashMap<usize, f64> {
        &self.market_prices
    }

    pub fn product_sold(&self) -> &HashMap<usize, f64> {
        &self.product_sold
    }

    pub fn product_output(&self) -> &HashMap<usize, f64> {
        &self.product_output
    }

    pub fn product_exchanged_total(&self) -> &HashMap<usize, f64> {
        &self.product_exchanged_total
    }
}