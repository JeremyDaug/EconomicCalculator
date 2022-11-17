use std::collections::HashMap;

use super::{territory::Territory, product::Product, firm::Firm, pop::Pop};

#[derive(Debug)]
pub struct Market {
    id: u64,
    name: String,
    firms: Vec<Firm>,
    pops: Vec<Pop>,
    territories: Vec<Territory>,
    neighbors: HashMap<Market, f64>,
    resources: HashMap<Product, f64>,
    market_prices: HashMap<Product, f64>,
    products_for_sale: HashMap<Product, f64>,
    product_sold: HashMap<Product, f64>,
    product_output: HashMap<Product, f64>,
    product_exchanged_total: HashMap<Product, f64>
}

impl Market {
    pub fn id(&self) -> u64 {
        self.id
    }

    pub fn name(&self) -> &str {
        self.name.as_ref()
    }

    pub fn firms(&self) -> &Vec<Firm> {
        &self.firms
    }

    pub fn pops(&self) -> &Vec<Pop> {
        &self.pops
    }

    pub fn territories(&self) -> &[Territory] {
        self.territories.as_ref()
    }

    pub fn neighbors(&self) -> &HashMap<Market, f64> {
        &self.neighbors
    }

    pub fn resources(&self) -> &HashMap<Product, f64> {
        &self.resources
    }

    pub fn products_for_sale(&self) -> &HashMap<Product, f64> {
        &self.products_for_sale
    }

    pub fn market_prices(&self) -> &HashMap<Product, f64> {
        &self.market_prices
    }

    pub fn product_sold(&self) -> &HashMap<Product, f64> {
        &self.product_sold
    }

    pub fn product_output(&self) -> &HashMap<Product, f64> {
        &self.product_output
    }

    pub fn product_exchanged_total(&self) -> &HashMap<Product, f64> {
        &self.product_exchanged_total
    }
}