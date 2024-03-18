
mod data_manager_tests {
    use std::collections::{HashMap, HashSet};

    use itertools::Itertools;
    use political_economy_core::data_manager::DataManager;
    use political_economy_core::objects::data_objects::product::Product;

    #[test]
    pub fn update_product_classes_correctly() {
        let mut test = DataManager::new();
        // class 0
        test.products.insert(0, Product{ id: 0,
            name: "T0".to_string(),
            variant_name: "".to_string(),
            description: "".to_string(),
            unit_name: "".to_string(),
            quality: 0,
            mass: 0.0,
            bulk: 0.0,
            mean_time_to_failure: None,
            fractional: false,
            tags: vec![],
            wants: HashMap::new(),
            processes: HashSet::new(),
            failure_process: None,
            use_processes: HashSet::new(),
            consumption_processes: HashSet::new(),
            maintenance_processes: HashSet::new(),
            tech_required: None,
            product_class: Some(0),
        });
        test.products.insert(1, Product{ id: 1,
            name: "T1".to_string(),
            variant_name: "".to_string(),
            description: "".to_string(),
            unit_name: "".to_string(),
            quality: 0,
            mass: 0.0,
            bulk: 0.0,
            mean_time_to_failure: None,
            fractional: false,
            tags: vec![],
            wants: HashMap::new(),
            processes: HashSet::new(),
            failure_process: None,
            use_processes: HashSet::new(),
            consumption_processes: HashSet::new(),
            maintenance_processes: HashSet::new(),
            tech_required: None,
            product_class: Some(0),
        });
        test.products.insert(2, Product{ id: 2,
            name: "T2".to_string(),
            variant_name: "".to_string(),
            description: "".to_string(),
            unit_name: "".to_string(),
            quality: 0,
            mass: 0.0,
            bulk: 0.0,
            mean_time_to_failure: None,
            fractional: false,
            tags: vec![],
            wants: HashMap::new(),
            processes: HashSet::new(),
            failure_process: None,
            use_processes: HashSet::new(),
            consumption_processes: HashSet::new(),
            maintenance_processes: HashSet::new(),
            tech_required: None,
            product_class: Some(0),
        });
        // class 4
        test.products.insert(3, Product{ id: 3,
            name: "T3".to_string(),
            variant_name: "".to_string(),
            description: "".to_string(),
            unit_name: "".to_string(),
            quality: 0,
            mass: 0.0,
            bulk: 0.0,
            mean_time_to_failure: None,
            fractional: false,
            tags: vec![],
            wants: HashMap::new(),
            processes: HashSet::new(),
            failure_process: None,
            use_processes: HashSet::new(),
            consumption_processes: HashSet::new(),
            maintenance_processes: HashSet::new(),
            tech_required: None,
            product_class: Some(4),
        });
        test.products.insert(4, Product{ id: 4,
            name: "T4".to_string(),
            variant_name: "".to_string(),
            description: "".to_string(),
            unit_name: "".to_string(),
            quality: 0,
            mass: 0.0,
            bulk: 0.0,
            mean_time_to_failure: None,
            fractional: false,
            tags: vec![],
            wants: HashMap::new(),
            processes: HashSet::new(),
            failure_process: None,
            use_processes: HashSet::new(),
            consumption_processes: HashSet::new(),
            maintenance_processes: HashSet::new(),
            tech_required: None,
            product_class: Some(4),
        });
        // No class
        test.products.insert(5, Product{ id: 5,
            name: "T5".to_string(),
            variant_name: "".to_string(),
            description: "".to_string(),
            unit_name: "".to_string(),
            quality: 0,
            mass: 0.0,
            bulk: 0.0,
            mean_time_to_failure: None,
            fractional: false,
            tags: vec![],
            wants: HashMap::new(),
            processes: HashSet::new(),
            failure_process: None,
            use_processes: HashSet::new(),
            consumption_processes: HashSet::new(),
            maintenance_processes: HashSet::new(),
            tech_required: None,
            product_class: None,
        });
        // test the results as a success
        test.update_product_classes().expect("Should not panic!");
        assert_eq!(test.product_classes.len(), 2);
        assert!(test.product_classes.contains_key(&0));
        assert!(test.product_classes.get(&0).unwrap().contains(&0));
        assert!(test.product_classes.get(&0).unwrap().contains(&1));
        assert!(test.product_classes.get(&0).unwrap().contains(&2));
        assert!(test.product_classes.contains_key(&4));
        assert!(test.product_classes.get(&4).unwrap().contains(&3));
        assert!(test.product_classes.get(&4).unwrap().contains(&4));
        // test for the error
        test.products.insert(6, Product{ id: 6,
            name: "T6".to_string(),
            variant_name: "".to_string(),
            description: "".to_string(),
            unit_name: "".to_string(),
            quality: 0,
            mass: 0.0,
            bulk: 0.0,
            mean_time_to_failure: None,
            fractional: false,
            tags: vec![],
            wants: HashMap::new(),
            processes: HashSet::new(),
            failure_process: None,
            use_processes: HashSet::new(),
            consumption_processes: HashSet::new(),
            maintenance_processes: HashSet::new(),
            tech_required: None,
            product_class: Some(7),
        });
        test.products.insert(7, Product{ id: 7,
            name: "T7".to_string(),
            variant_name: "".to_string(),
            description: "".to_string(),
            unit_name: "".to_string(),
            quality: 0,
            mass: 0.0,
            bulk: 0.0,
            mean_time_to_failure: None,
            fractional: false,
            tags: vec![],
            wants: HashMap::new(),
            processes: HashSet::new(),
            failure_process: None,
            use_processes: HashSet::new(),
            consumption_processes: HashSet::new(),
            maintenance_processes: HashSet::new(),
            tech_required: None,
            product_class: Some(6),
        });
        assert!(test.update_product_classes().is_err());
    }

    #[test]
    pub fn output_existing_data_ids() {

        let mut test = DataManager::new();
        let result = test.load_test_data();

        if let Err(message) = result {
            panic!("\n{}", message);
        }

        println!("----- Wants -----");
        println!("----+------------------");
        println!("| id|name");
        for id in test.wants.keys().sorted() {
            println!("{:>3} | {:<}", id, test.wants[id].name);
        }

        println!("----- Technology -----");
        println!("----+------------------");
        println!("| id|name");
        for id in test.technology.keys().sorted() {
            println!("{:>3} | {:<}", id, test.technology[id].name);
        }

        println!("----- Technology Family -----");
        println!("----+------------------");
        println!("| id|name");
        for id in test.technology_families.keys().sorted() {
            println!("{:>3} | {:<}", id, test.technology_families[id].name());
        }

        println!("----- Product -----");
        println!("----+------------------");
        println!("| id|name");
        for id in test.products.keys().sorted() {
            println!("{:>3} | {:<}", id, test.products[id].get_name());
        }

        println!("----- Processes -----");
        println!("----+------------------");
        println!("| id|name");
        for id in test.processes.keys().sorted() {
            println!("{:>3} | {:<}", id, test.processes[id].get_name());
        }

        println!("----- Jobs -----");
        println!("----+------------------");
        println!("| id|name");
        for id in test.jobs.keys().sorted() {
            println!("{:>3} | {:<}", id, test.jobs[id].get_name());
        }

        println!("----- Species -----");
        println!("----+------------------");
        println!("| id|name");
        for id in test.species.keys().sorted() {
            println!("{:>3} | {:<}", id, test.species[id].get_name());
        }

        println!("----- Cultures -----");
        println!("----+------------------");
        println!("| id|name");
        for id in test.cultures.keys().sorted() {
            println!("{:>3} | {:<}", id, test.cultures[id].get_name());
        }

        println!("----- Pops -----");
        println!("----+------------------");
        println!("| id|name");
        for id in test.pops.keys().sorted() {
            println!("{:>3} | {:<}", id, test.pops[id].id_name());
        }

        println!("----- Territories -----");
        println!("----+------------------");
        println!("| id|name");
        for id in test.territories.keys().sorted() {
            println!("{:>3} | {:<}", id, test.territories[id].name);
        }

        println!("----- Markets -----");
        println!("----+------------------");
        println!("| id|name");
        for id in test.markets.keys().sorted() {
            println!("{:>3} | {:<}", id, test.markets[id].name);
        }

        println!("----- Firms -----");
        println!("----+------------------");
        println!("| id|name");
        for id in test.firms.keys().sorted() {
            println!("{:>3} | {:<}", id, test.firms[id].get_name());
        }
    }
}
