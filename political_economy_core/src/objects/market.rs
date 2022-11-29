use std::collections::HashMap;

use super::{territory::Territory, product::Product, firm::Firm, pop::Pop};

#[derive(Debug)]
pub struct Market {
    pub id: u64,
    pub name: String,
    pub firms: Vec<Firm>,
    pub pops: Vec<Pop>,
    pub territories: Vec<Territory>,
    pub neighbors: HashMap<Market, f64>,
    pub resources: HashMap<Product, f64>,
    pub market_prices: HashMap<Product, f64>,
    pub products_for_sale: HashMap<Product, f64>,
    pub product_sold: HashMap<Product, f64>,
    pub product_output: HashMap<Product, f64>,
    pub product_exchanged_total: HashMap<Product, f64>
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