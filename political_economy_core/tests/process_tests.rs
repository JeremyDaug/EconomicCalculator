use political_economy_core::{data_manager::DataManager, 
    objects::data_objects::{item::Item, process::*, product::Product}};
use political_economy_core::objects::actor_objects::property_info::PropertyInfo;

mod process_tests {
    use super::*;
    
    mod uses_product_should {
        use super::super::*;

        #[test]
        pub fn return_true_when_product_class_is_a_input() {
            let mut data = DataManager::new();
            // create 3 products for our stuff
            data.products.insert(0, Product::new(
                0,
                String::from("Test"),
                String::from(""),
                String::from("Desc"),
                String::from("Unit"),
                0,
                0.0,
                0.0,
                Some(3),
                true,
                Vec::new(),
                None,
                Some(0)).unwrap());
            data.products.insert(1, Product::new(
                1,
                String::from("Test"),
                String::from(""),
                String::from("Desc"),
                String::from("Unit"),
                0,
                0.0,
                0.0,
                Some(3),
                true,
                Vec::new(),
                None,
                Some(0)).unwrap());
            data.products.insert(2, Product::new(
                2,
                String::from("Test"),
                String::from(""),
                String::from("Desc"),
                String::from("Unit"),
                0,
                0.0,
                0.0,
                Some(3),
                true,
                Vec::new(),
                None,
                None).unwrap());
            data.products.insert(3, Product::new(
                3,
                String::from("Test"),
                String::from(""),
                String::from("Desc"),
                String::from("Unit"),
                0,
                0.0,
                0.0,
                Some(3),
                true,
                Vec::new(),
                None,
                None).unwrap());
            let process = Process{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart{ 
                        item: Item::Class(0),
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart{ 
                        item: Item::Product(2),
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Capital 
                    },
                    ProcessPart{ 
                        item: Item::Product(3),
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output 
                    }
                ],
                process_tags: vec![],
                technology_requirement: None,
                tertiary_tech: None,
            };
            assert!(process.uses_product(0, &data));
        }

        #[test]
        pub fn return_true_when_product_class_is_a_capital() {
            let mut data = DataManager::new();
            // create 3 products for our stuff
            data.products.insert(0, Product::new(
                0,
                String::from("Test"),
                String::from(""),
                String::from("Desc"),
                String::from("Unit"),
                0,
                0.0,
                0.0,
                Some(3),
                true,
                Vec::new(),
                None,
                Some(0)).unwrap());
            data.products.insert(1, Product::new(
                1,
                String::from("Test"),
                String::from(""),
                String::from("Desc"),
                String::from("Unit"),
                0,
                0.0,
                0.0,
                Some(3),
                true,
                Vec::new(),
                None,
                Some(0)).unwrap());
            data.products.insert(2, Product::new(
                2,
                String::from("Test"),
                String::from(""),
                String::from("Desc"),
                String::from("Unit"),
                0,
                0.0,
                0.0,
                Some(3),
                true,
                Vec::new(),
                None,
                None).unwrap());
            data.products.insert(3, Product::new(
                3,
                String::from("Test"),
                String::from(""),
                String::from("Desc"),
                String::from("Unit"),
                0,
                0.0,
                0.0,
                Some(3),
                true,
                Vec::new(),
                None,
                None).unwrap());
            let process = Process{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart{ 
                        item: Item::Product(2),
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart{ 
                        item: Item::Class(0),
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Capital 
                    },
                    ProcessPart{ 
                        item: Item::Product(3),
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output 
                    }
                ],
                process_tags: vec![],
                technology_requirement: None,
                tertiary_tech: None,
            };
            assert!(process.uses_product(0, &data));
        }

        #[test]
        pub fn return_true_when_product_is_a_capital() {
            let mut data = DataManager::new();
            // create 3 products for our stuff
            data.products.insert(0, Product::new(
                0,
                String::from("Test"),
                String::from(""),
                String::from("Desc"),
                String::from("Unit"),
                0,
                0.0,
                0.0,
                Some(3),
                true,
                Vec::new(),
                None,
                Some(0)).unwrap());
            data.products.insert(1, Product::new(
                1,
                String::from("Test"),
                String::from(""),
                String::from("Desc"),
                String::from("Unit"),
                0,
                0.0,
                0.0,
                Some(3),
                true,
                Vec::new(),
                None,
                Some(0)).unwrap());
            data.products.insert(2, Product::new(
                2,
                String::from("Test"),
                String::from(""),
                String::from("Desc"),
                String::from("Unit"),
                0,
                0.0,
                0.0,
                Some(3),
                true,
                Vec::new(),
                None,
                None).unwrap());
            data.products.insert(3, Product::new(
                3,
                String::from("Test"),
                String::from(""),
                String::from("Desc"),
                String::from("Unit"),
                0,
                0.0,
                0.0,
                Some(3),
                true,
                Vec::new(),
                None,
                None).unwrap());
            let process = Process{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart{ 
                        item: Item::Product(2),
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart{ 
                        item: Item::Product(0),
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Capital 
                    },
                    ProcessPart{ 
                        item: Item::Product(3),
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output 
                    }
                ],
                process_tags: vec![],
                technology_requirement: None,
                tertiary_tech: None,
            };
            assert!(process.uses_product(0, &data));
        }

        #[test]
        pub fn return_true_when_product_is_an_input() {
            let mut data = DataManager::new();
            // create 3 products for our stuff
            data.products.insert(0, Product::new(
                0,
                String::from("Test"),
                String::from(""),
                String::from("Desc"),
                String::from("Unit"),
                0,
                0.0,
                0.0,
                Some(3),
                true,
                Vec::new(),
                None,
                Some(0)).unwrap());
            data.products.insert(1, Product::new(
                1,
                String::from("Test"),
                String::from(""),
                String::from("Desc"),
                String::from("Unit"),
                0,
                0.0,
                0.0,
                Some(3),
                true,
                Vec::new(),
                None,
                Some(0)).unwrap());
            data.products.insert(2, Product::new(
                2,
                String::from("Test"),
                String::from(""),
                String::from("Desc"),
                String::from("Unit"),
                0,
                0.0,
                0.0,
                Some(3),
                true,
                Vec::new(),
                None,
                None).unwrap());
            data.products.insert(3, Product::new(
                3,
                String::from("Test"),
                String::from(""),
                String::from("Desc"),
                String::from("Unit"),
                0,
                0.0,
                0.0,
                Some(3),
                true,
                Vec::new(),
                None,
                None).unwrap());
            let process = Process{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart{ 
                        item: Item::Product(2),
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart{ 
                        item: Item::Class(0),
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Capital 
                    },
                    ProcessPart{ 
                        item: Item::Product(3),
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output 
                    }
                ],
                process_tags: vec![],
                technology_requirement: None,
                tertiary_tech: None,
            };
            assert!(process.uses_product(2, &data));
        }

        #[test]
        pub fn return_false_when_product_is_not_input_or_capital() {
            let mut data = DataManager::new();
            // create 3 products for our stuff
            data.products.insert(0, Product::new(
                0,
                String::from("Test"),
                String::from(""),
                String::from("Desc"),
                String::from("Unit"),
                0,
                0.0,
                0.0,
                Some(3),
                true,
                Vec::new(),
                None,
                Some(0)).unwrap());
            data.products.insert(1, Product::new(
                1,
                String::from("Test"),
                String::from(""),
                String::from("Desc"),
                String::from("Unit"),
                0,
                0.0,
                0.0,
                Some(3),
                true,
                Vec::new(),
                None,
                Some(0)).unwrap());
            data.products.insert(2, Product::new(
                2,
                String::from("Test"),
                String::from(""),
                String::from("Desc"),
                String::from("Unit"),
                0,
                0.0,
                0.0,
                Some(3),
                true,
                Vec::new(),
                None,
                None).unwrap());
            data.products.insert(3, Product::new(
                3,
                String::from("Test"),
                String::from(""),
                String::from("Desc"),
                String::from("Unit"),
                0,
                0.0,
                0.0,
                Some(3),
                true,
                Vec::new(),
                None,
                None).unwrap());
            let process = Process{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart{ 
                        item: Item::Product(2),
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart{ 
                        item: Item::Class(0),
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Capital 
                    },
                    ProcessPart{ 
                        item: Item::Product(3),
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output 
                    }
                ],
                process_tags: vec![],
                technology_requirement: None,
                tertiary_tech: None,
            };
            assert!(!process.uses_product(3, &data));
        }
    }

    // TODO undertested, code is mostly copy/past from other do_process
    mod do_process_with_property_should {
        use std::{collections::{HashMap, HashSet}, str::FromStr};
        use super::super::*;

        #[test]
        pub fn correctly_restrict_when_bonus_pushes_over_normal_but_normal_is_not_current_lowest() {
            let mut data = DataManager::new();
            let prod0 = Product {
                id: 0,
                name: String::from("Optional Input"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
                fractional: false,
                tags: vec![],
                wants: HashMap::new(),
                processes: HashSet::from([0]),
                failure_process: Some(0),
                use_processes: HashSet::new(),
                consumption_processes: HashSet::new(),
                maintenance_processes: HashSet::new(),
                tech_required: None,
                product_class: None,
            };
            let prod1 = Product {
                id: 1,
                name: String::from("Normal Input"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod2 = Product {
                id: 2,
                name: String::from("Fixed Input"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod3 = Product {
                id: 3,
                name: String::from("Fixed Output"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod4 = Product {
                id: 4,
                name: String::from("Normal Output"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod5 = Product {
                id: 5,
                name: String::from("Failed Output"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            // setup product with a failure process.
            let fail_proc = Process {
                id: 0,
                name: String::from("prod0 fail"),
                variant_name: String::new(),
                description: String::new(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart { 
                        item: Item::Product(0), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart { 
                        item: Item::Product(5), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output 
                    }
                ],
                process_tags: vec![],
                technology_requirement: None,
                tertiary_tech: None,
            };
            let test_proc = Process {
                id: 1,
                name: String::from("test process"),
                variant_name: String::new(),
                description: String::new(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart { 
                        item: Item::Product(0), 
                        amount: 1.0, 
                        part_tags: vec![
                            ProcessPartTag::Optional { 
                                missing_penalty: 0.0, 
                                final_bonus: 1.0 }
                        ], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart {
                        item: Item::Product(1), 
                        amount: 1.0, 
                        part_tags: vec![
                        ], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart {
                        item: Item::Product(2), 
                        amount: 1.0, 
                        part_tags: vec![
                            ProcessPartTag::Fixed
                        ], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart { 
                        item: Item::Product(3), 
                        amount: 1.0, 
                        part_tags: vec![
                            ProcessPartTag::Fixed
                        ], 
                        part: ProcessSectionTag::Output 
                    },
                    ProcessPart { 
                        item: Item::Product(4), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output 
                    }
                ],
                process_tags: vec![],
                technology_requirement: None,
                tertiary_tech: None,
            };
            data.products.insert(0, prod0);
            data.products.insert(1, prod1);
            data.products.insert(2, prod2);
            data.products.insert(3, prod3);
            data.products.insert(4, prod4);
            data.products.insert(5, prod5);
            data.processes.insert(0, fail_proc);
            data.processes.insert(1, test_proc);
            let test_proc = data.processes.get(&1).unwrap();

            let mut available_products = HashMap::new();
            available_products.insert(0, PropertyInfo::new(4.0)); // optional, should use all
            available_products.insert(1, PropertyInfo::new(4.0)); // normal, should only 1
            available_products.insert(2, PropertyInfo::new(3.0)); // fixed, should use both via bonus.
            let available_wants: HashMap<usize, f64> = HashMap::new();
            let result = test_proc.do_process_with_property(&available_products, 
                &available_wants, 
                0.0, 
                None, 
                false, 
                &data, false);
            
            assert_eq!(result.effective_iterations, 4.0);
            assert_eq!(result.iterations, 3.0);
            assert_eq!(result.efficiency, 4.0/3.0);
            assert_eq!(result.input_output_wants.len(), 0);
            assert_eq!(result.input_output_products.len(), 6);
            // Note, this rounds due to floating point rounding errors around 
            assert_eq!(result.input_output_products[&0].round(), -1.0);
            assert_eq!(result.input_output_products[&1], -4.0);
            assert_eq!(result.input_output_products[&2], -3.0);
            assert_eq!(result.input_output_products[&3], 3.0);
            assert_eq!(result.input_output_products[&4], 4.0);
            // rounded to to floating point errors.
            assert_eq!(result.input_output_products[&5].round(), 1.0);
            assert_eq!(result.capital_products.len(), 0);
        }
        
        #[test]
        pub fn correctly_restrict_to_available_normal_products() {
            let mut data = DataManager::new();
            let prod0 = Product {
                id: 0,
                name: String::from("Optional Input"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
                fractional: false,
                tags: vec![],
                wants: HashMap::new(),
                processes: HashSet::from([0]),
                failure_process: Some(0),
                use_processes: HashSet::new(),
                consumption_processes: HashSet::new(),
                maintenance_processes: HashSet::new(),
                tech_required: None,
                product_class: None,
            };
            let prod1 = Product {
                id: 1,
                name: String::from("Normal Input"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod2 = Product {
                id: 2,
                name: String::from("Fixed Input"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod3 = Product {
                id: 3,
                name: String::from("Fixed Output"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod4 = Product {
                id: 4,
                name: String::from("Normal Output"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod5 = Product {
                id: 5,
                name: String::from("Failed Output"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            // setup product with a failure process.
            let fail_proc = Process {
                id: 0,
                name: String::from("prod0 fail"),
                variant_name: String::new(),
                description: String::new(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart { 
                        item: Item::Product(0), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart { 
                        item: Item::Product(5), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output 
                    }
                ],
                process_tags: vec![],
                technology_requirement: None,
                tertiary_tech: None,
            };
            let test_proc = Process {
                id: 1,
                name: String::from("test process"),
                variant_name: String::new(),
                description: String::new(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart { 
                        item: Item::Product(0), 
                        amount: 1.0, 
                        part_tags: vec![
                            ProcessPartTag::Optional { 
                                missing_penalty: 0.0, 
                                final_bonus: 1.0 }
                        ], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart {
                        item: Item::Product(1), 
                        amount: 1.0, 
                        part_tags: vec![
                        ], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart {
                        item: Item::Product(2), 
                        amount: 1.0, 
                        part_tags: vec![
                            ProcessPartTag::Fixed
                        ], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart { 
                        item: Item::Product(3), 
                        amount: 1.0, 
                        part_tags: vec![
                            ProcessPartTag::Fixed
                        ], 
                        part: ProcessSectionTag::Output 
                    },
                    ProcessPart { 
                        item: Item::Product(4), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output 
                    }
                ],
                process_tags: vec![],
                technology_requirement: None,
                tertiary_tech: None,
            };
            data.products.insert(0, prod0);
            data.products.insert(1, prod1);
            data.products.insert(2, prod2);
            data.products.insert(3, prod3);
            data.products.insert(4, prod4);
            data.products.insert(5, prod5);
            data.processes.insert(0, fail_proc);
            data.processes.insert(1, test_proc);
            let test_proc = data.processes.get(&1).unwrap();

            let mut available_products = HashMap::new();
            available_products.insert(0, PropertyInfo::new(4.0)); // optional, should use all
            available_products.insert(1, PropertyInfo::new(2.0)); // normal, should only 1
            available_products.insert(2, PropertyInfo::new(4.0)); // fixed, should use both via bonus.
            let available_wants: HashMap<usize, f64> = HashMap::new();
            let result = test_proc.do_process_with_property(&available_products, 
                &available_wants, 
                0.0, 
                None, 
                false, 
                &data, false);
            
            assert_eq!(result.effective_iterations, 2.0);
            assert_eq!(result.iterations, 1.0);
            assert_eq!(result.efficiency, 2.0);
            assert_eq!(result.input_output_wants.len(), 0);
            assert_eq!(result.input_output_products.len(), 6);
            assert_eq!(result.input_output_products[&0], -1.0);
            assert_eq!(result.input_output_products[&1], -2.0);
            assert_eq!(result.input_output_products[&2], -1.0);
            assert_eq!(result.input_output_products[&3], 1.0);
            assert_eq!(result.input_output_products[&4], 2.0);
            assert_eq!(result.input_output_products[&5], 1.0);
            assert_eq!(result.capital_products.len(), 0);
        }
        
        #[test]
        pub fn correctly_restrict_to_available_fixed() {
            let mut data = DataManager::new();
            let prod0 = Product {
                id: 0,
                name: String::from("Optional Input"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
                fractional: false,
                tags: vec![],
                wants: HashMap::new(),
                processes: HashSet::from([0]),
                failure_process: Some(0),
                use_processes: HashSet::new(),
                consumption_processes: HashSet::new(),
                maintenance_processes: HashSet::new(),
                tech_required: None,
                product_class: None,
            };
            let prod1 = Product {
                id: 1,
                name: String::from("Normal Input"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod2 = Product {
                id: 2,
                name: String::from("Fixed Input"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod3 = Product {
                id: 3,
                name: String::from("Fixed Output"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod4 = Product {
                id: 4,
                name: String::from("Normal Output"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod5 = Product {
                id: 5,
                name: String::from("Failed Output"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            // setup product with a failure process.
            let fail_proc = Process {
                id: 0,
                name: String::from("prod0 fail"),
                variant_name: String::new(),
                description: String::new(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart { 
                        item: Item::Product(0), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart { 
                        item: Item::Product(5), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output 
                    }
                ],
                process_tags: vec![],
                technology_requirement: None,
                tertiary_tech: None,
            };
            let test_proc = Process {
                id: 1,
                name: String::from("test process"),
                variant_name: String::new(),
                description: String::new(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart { 
                        item: Item::Product(0), 
                        amount: 1.0, 
                        part_tags: vec![
                            ProcessPartTag::Optional { 
                                missing_penalty: 0.0, 
                                final_bonus: 1.0 }
                        ], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart {
                        item: Item::Product(1), 
                        amount: 1.0, 
                        part_tags: vec![
                        ], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart {
                        item: Item::Product(2), 
                        amount: 1.0, 
                        part_tags: vec![
                            ProcessPartTag::Fixed
                        ], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart { 
                        item: Item::Product(3), 
                        amount: 1.0, 
                        part_tags: vec![
                            ProcessPartTag::Fixed
                        ], 
                        part: ProcessSectionTag::Output 
                    },
                    ProcessPart { 
                        item: Item::Product(4), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output 
                    }
                ],
                process_tags: vec![],
                technology_requirement: None,
                tertiary_tech: None,
            };
            data.products.insert(0, prod0);
            data.products.insert(1, prod1);
            data.products.insert(2, prod2);
            data.products.insert(3, prod3);
            data.products.insert(4, prod4);
            data.products.insert(5, prod5);
            data.processes.insert(0, fail_proc);
            data.processes.insert(1, test_proc);
            let test_proc = data.processes.get(&1).unwrap();

            let mut available_products = HashMap::new();
            available_products.insert(0, PropertyInfo::new(4.0)); // optional, should use all
            available_products.insert(1, PropertyInfo::new(4.0)); // normal, should only 1
            available_products.insert(2, PropertyInfo::new(1.0)); // fixed, should use both via bonus.
            let available_wants: HashMap<usize, f64> = HashMap::new();
            let result = test_proc.do_process_with_property(&available_products, 
                &available_wants, 
                0.0, 
                None, 
                false, 
                &data, false);
            
            assert_eq!(result.effective_iterations, 2.0);
            assert_eq!(result.iterations, 1.0);
            assert_eq!(result.efficiency, 2.0);
            assert_eq!(result.input_output_wants.len(), 0);
            assert_eq!(result.input_output_products.len(), 6);
            assert_eq!(result.input_output_products[&0], -1.0);
            assert_eq!(result.input_output_products[&1], -2.0);
            assert_eq!(result.input_output_products[&2], -1.0);
            assert_eq!(result.input_output_products[&3], 1.0);
            assert_eq!(result.input_output_products[&4], 2.0);
            assert_eq!(result.input_output_products[&5], 1.0);
            assert_eq!(result.capital_products.len(), 0);
        }

        #[test]
        pub fn use_and_consume_optional_input_correctly() {
            let mut data = DataManager::new();
            let prod0 = Product {
                id: 0,
                name: String::from("Optional Input"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
                fractional: false,
                tags: vec![],
                wants: HashMap::new(),
                processes: HashSet::from([0]),
                failure_process: Some(0),
                use_processes: HashSet::new(),
                consumption_processes: HashSet::new(),
                maintenance_processes: HashSet::new(),
                tech_required: None,
                product_class: None,
            };
            let prod1 = Product {
                id: 1,
                name: String::from("Normal Input"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod2 = Product {
                id: 2,
                name: String::from("Fixed Input"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod3 = Product {
                id: 3,
                name: String::from("Fixed Output"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod4 = Product {
                id: 4,
                name: String::from("Normal Output"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod5 = Product {
                id: 5,
                name: String::from("Failed Output"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            // setup product with a failure process.
            let fail_proc = Process {
                id: 0,
                name: String::from("prod0 fail"),
                variant_name: String::new(),
                description: String::new(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart { 
                        item: Item::Product(0), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart { 
                        item: Item::Product(5), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output 
                    }
                ],
                process_tags: vec![],
                technology_requirement: None,
                tertiary_tech: None,
            };
            let test_proc = Process {
                id: 1,
                name: String::from("test process"),
                variant_name: String::new(),
                description: String::new(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart { 
                        item: Item::Product(0), 
                        amount: 1.0, 
                        part_tags: vec![
                            ProcessPartTag::Optional { 
                                missing_penalty: 0.0, 
                                final_bonus: 1.0 }
                        ], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart {
                        item: Item::Product(1), 
                        amount: 1.0, 
                        part_tags: vec![
                        ], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart {
                        item: Item::Product(2), 
                        amount: 1.0, 
                        part_tags: vec![
                            ProcessPartTag::Fixed
                        ], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart { 
                        item: Item::Product(3), 
                        amount: 1.0, 
                        part_tags: vec![
                            ProcessPartTag::Fixed
                        ], 
                        part: ProcessSectionTag::Output 
                    },
                    ProcessPart { 
                        item: Item::Product(4), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output 
                    }
                ],
                process_tags: vec![],
                technology_requirement: None,
                tertiary_tech: None,
            };
            data.products.insert(0, prod0);
            data.products.insert(1, prod1);
            data.products.insert(2, prod2);
            data.products.insert(3, prod3);
            data.products.insert(4, prod4);
            data.products.insert(5, prod5);
            data.processes.insert(0, fail_proc);
            data.processes.insert(1, test_proc);
            let test_proc = data.processes.get(&1).unwrap();

            let mut available_products = HashMap::new();
            available_products.insert(0, PropertyInfo::new(1.0)); // optional, should use all
            available_products.insert(1, PropertyInfo::new(2.0)); // normal, should only 1
            available_products.insert(2, PropertyInfo::new(1.0)); // fixed, should use both via bonus.
            let available_wants: HashMap<usize, f64> = HashMap::new();
            let result = test_proc.do_process_with_property(&available_products, 
                &available_wants, 
                0.0, 
                None, 
                false, 
                &data, false);
            
            assert_eq!(result.effective_iterations, 2.0);
            assert_eq!(result.iterations, 1.0);
            assert_eq!(result.efficiency, 2.0);
            assert_eq!(result.input_output_wants.len(), 0);
            assert_eq!(result.input_output_products.len(), 6);
            assert_eq!(result.input_output_products[&0], -1.0);
            assert_eq!(result.input_output_products[&1], -2.0);
            assert_eq!(result.input_output_products[&2], -1.0);
            assert_eq!(result.input_output_products[&3], 1.0);
            assert_eq!(result.input_output_products[&4], 2.0);
            assert_eq!(result.input_output_products[&5], 1.0);
            assert_eq!(result.capital_products.len(), 0);
        }

        #[test]
        pub fn use_optional_capital_correctly() {
            let mut data = DataManager::new();
            let prod0 = Product {
                id: 0,
                name: String::from("Optional Capital"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
                fractional: false,
                tags: vec![],
                wants: HashMap::new(),
                processes: HashSet::from([0]),
                failure_process: Some(0),
                use_processes: HashSet::new(),
                consumption_processes: HashSet::new(),
                maintenance_processes: HashSet::new(),
                tech_required: None,
                product_class: None,
            };
            let prod1 = Product {
                id: 1,
                name: String::from("Normal Input"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod2 = Product {
                id: 2,
                name: String::from("Fixed Input"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod3 = Product {
                id: 3,
                name: String::from("Fixed Output"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod4 = Product {
                id: 4,
                name: String::from("Normal Output"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod5 = Product {
                id: 5,
                name: String::from("Failed Output"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            // setup product with a failure process.
            let fail_proc = Process {
                id: 0,
                name: String::from("prod0 fail"),
                variant_name: String::new(),
                description: String::new(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart { 
                        item: Item::Product(0), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart { 
                        item: Item::Product(3), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output 
                    }
                ],
                process_tags: vec![],
                technology_requirement: None,
                tertiary_tech: None,
            };
            let test_proc = Process {
                id: 1,
                name: String::from("test process"),
                variant_name: String::new(),
                description: String::new(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart { 
                        item: Item::Product(0), 
                        amount: 1.0, 
                        part_tags: vec![
                            ProcessPartTag::Optional { 
                                missing_penalty: 0.0, 
                                final_bonus: 1.0 }
                        ], 
                        part: ProcessSectionTag::Capital 
                    },
                    ProcessPart {
                        item: Item::Product(1), 
                        amount: 1.0, 
                        part_tags: vec![
                        ], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart {
                        item: Item::Product(2), 
                        amount: 1.0, 
                        part_tags: vec![
                            ProcessPartTag::Fixed
                        ], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart { 
                        item: Item::Product(3), 
                        amount: 1.0, 
                        part_tags: vec![
                            ProcessPartTag::Fixed
                        ], 
                        part: ProcessSectionTag::Output 
                    },
                    ProcessPart { 
                        item: Item::Product(4), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output 
                    }
                ],
                process_tags: vec![],
                technology_requirement: None,
                tertiary_tech: None,
            };
            data.products.insert(0, prod0);
            data.products.insert(1, prod1);
            data.products.insert(2, prod2);
            data.products.insert(3, prod3);
            data.products.insert(4, prod4);
            data.products.insert(5, prod5);
            data.processes.insert(0, fail_proc);
            data.processes.insert(1, test_proc);
            let test_proc = data.processes.get(&1).unwrap();

            let mut available_products = HashMap::new();
            available_products.insert(0, PropertyInfo::new(1.0)); // optional, should use all
            available_products.insert(1, PropertyInfo::new(2.0)); // normal, should only 1
            available_products.insert(2, PropertyInfo::new(1.0)); // fixed, should use both via bonus.
            let available_wants: HashMap<usize, f64> = HashMap::new();
            let result = test_proc.do_process_with_property(&available_products, 
                &available_wants, 
                0.0, 
                None, 
                false, 
                &data, false);
            
            assert_eq!(result.effective_iterations, 2.0);
            assert_eq!(result.iterations, 1.0);
            assert_eq!(result.efficiency, 2.0);
            assert_eq!(result.input_output_wants.len(), 0);
            //assert_eq!(result.input_output_products[&0], -1.0);
            assert_eq!(result.input_output_products.len(), 4);
            assert_eq!(result.input_output_products[&1], -2.0);
            assert_eq!(result.input_output_products[&2], -1.0);
            assert_eq!(result.input_output_products[&3], 1.0);
            assert_eq!(result.input_output_products[&4], 2.0);
            assert_eq!(result.capital_products[&0], 1.0);
        }

        #[test]
        pub fn correctly_consume_tagged_consumption_input() {
            let mut data = DataManager::new();
            let prod0 = Product {
                id: 0,
                name: String::from("Consumed Input"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
                fractional: false,
                tags: vec![],
                wants: HashMap::new(),
                processes: HashSet::from([0]),
                failure_process: Some(0),
                use_processes: HashSet::new(),
                consumption_processes: HashSet::new(),
                maintenance_processes: HashSet::new(),
                tech_required: None,
                product_class: None,
            };
            let prod1 = Product {
                id: 1,
                name: String::from("Normal Input"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod2 = Product {
                id: 2,
                name: String::from("Normal Output"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod3 = Product {
                id: 3,
                name: String::from("Failed Output"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            // setup product with a failure process.
            let fail_proc = Process {
                id: 0,
                name: String::from("prod0 fail"),
                variant_name: String::new(),
                description: String::new(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart { 
                        item: Item::Product(0), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart { 
                        item: Item::Product(3), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output 
                    }
                ],
                process_tags: vec![],
                technology_requirement: None,
                tertiary_tech: None,
            };
            let test_proc = Process {
                id: 1,
                name: String::from("test product"),
                variant_name: String::new(),
                description: String::new(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart { 
                        item: Item::Product(0), 
                        amount: 1.0, 
                        part_tags: vec![
                            ProcessPartTag::Consumed
                        ], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart { 
                        item: Item::Product(1), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart { 
                        item: Item::Product(2), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output 
                    }
                ],
                process_tags: vec![],
                technology_requirement: None,
                tertiary_tech: None,
            };
            data.products.insert(0, prod0);
            data.products.insert(1, prod1);
            data.products.insert(2, prod2);
            data.products.insert(3, prod3);
            data.processes.insert(0, fail_proc);
            data.processes.insert(1, test_proc);
            let test_proc = data.processes.get(&1).unwrap();

            let mut available_products = HashMap::new();
            available_products.insert(0, PropertyInfo::new(2.0)); // optional, should use all
            available_products.insert(1, PropertyInfo::new(2.0)); // normal, should only 1
            let available_wants: HashMap<usize, f64> = HashMap::new();
            let result = test_proc.do_process_with_property(&available_products, 
                &available_wants, 
                0.0, 
                None, 
                false, 
                &data, false);
            
            assert_eq!(result.effective_iterations, 2.0);
            assert_eq!(result.iterations, 2.0);
            assert_eq!(result.efficiency, 1.0);
            assert_eq!(result.input_output_wants.len(), 0);
            assert_eq!(result.input_output_products.len(), 4);
            assert_eq!(result.input_output_products[&0], -2.0);
            assert_eq!(result.input_output_products[&1], -2.0);
            assert_eq!(result.input_output_products[&2], 2.0);
            assert_eq!(result.input_output_products[&3], 2.0);
            assert_eq!(result.capital_products.len(), 0);
        }

        #[test]
        pub fn return_process_returns_empty_correctly() {
            let mut data = DataManager::new();
            data.load_test_data().expect("Failed!");
            let test = Process{ id: 0, name: String::from_str("Test").unwrap(), 
                variant_name: String::from_str("").unwrap(), 
                description: String::from_str("test").unwrap(), 
                minimum_time: 0.0, process_parts: vec![
                    ProcessPart{ item: Item::Product(0), // input product
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Want(0), // input want
                        amount: 2.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Product(1), // Capital product
                        amount: 0.5, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Capital },
                    // placeholder for capital want
                    ProcessPart{ item: Item::Product(2), // output product
                        amount: 1.5, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output },
                    ProcessPart{ item: Item::Want(2), // output want
                        amount: 2.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output },
                ], 
                process_tags: vec![], 
                technology_requirement: None, tertiary_tech: None };

            let available_products = HashMap::new();
            let available_wants = HashMap::new();
            let result = test.do_process_with_property(&available_products, 
                &available_wants, 0.0,
                None, true, &data, false);
            // check that it's all empty.
            assert!(result.iterations == 0.0);
            assert!(result.effective_iterations == 0.0);
            assert!(result.efficiency == 1.0);
            assert_eq!(result.input_output_products.len(), 0);
            assert_eq!(result.input_output_wants.len(), 0);
            assert_eq!(result.capital_products.len(), 0);
        }

        #[test]
        pub fn return_process_returns_single_iteration_correctly() {
            let mut data = DataManager::new();
            data.load_test_data().expect("Failed!");
            let test = Process{ id: 0, name: String::from_str("Test").unwrap(), 
                variant_name: String::from_str("").unwrap(), 
                description: String::from_str("test").unwrap(), 
                minimum_time: 0.0, process_parts: vec![
                    ProcessPart{ item: Item::Product(0), // input product
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Want(0), // input want
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Product(1), // Capital product
                        amount: 0.5, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Capital },
                    // placeholder for capital want
                    ProcessPart{ item: Item::Product(2), // output product
                        amount: 1.5, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output },
                    ProcessPart{ item: Item::Want(2), // output want
                        amount: 2.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output },
                ], 
                process_tags: vec![], 
                technology_requirement: None, tertiary_tech: None };

            // 1 of each item, should allow for only 1 iteration to be done.
            let mut available_products: HashMap<usize, PropertyInfo> = HashMap::new();
            available_products.insert(0, PropertyInfo::new(1.0));
            available_products.insert(1, PropertyInfo::new(1.0));
            available_products.insert(2, PropertyInfo::new(1.0));
            let mut available_wants = HashMap::new();
            available_wants.insert(0, 1.0);
            available_wants.insert(1, 1.0);
            available_wants.insert(2, 1.0);
            let result = test.do_process_with_property(&available_products, 
                &available_wants, 0.0, 
                None, true, &data, false);
            // check that it's all empty.
            assert!(result.iterations == 1.0);
            assert!(result.effective_iterations == 1.0);
            assert!(result.efficiency == 1.0);
            assert_eq!(result.input_output_products.len(), 2);
            assert!(*result.input_output_products.get(&0).unwrap() == -1.0);
            assert!(*result.input_output_products.get(&2).unwrap() == 1.5);
            assert_eq!(result.input_output_wants.len(), 2);
            assert!(*result.input_output_wants.get(&0).unwrap() == -1.0);
            assert!(*result.input_output_wants.get(&2).unwrap() == 2.0);
            assert_eq!(result.capital_products.len(), 1);
            assert!(*result.capital_products.get(&1).unwrap() == 0.5);
        }

        #[test]
        pub fn return_process_returns_multiple_iterations_correctly() {
            let mut data = DataManager::new();
            data.load_test_data().expect("Failed!");
            let test = Process{ id: 0, name: String::from_str("Test").unwrap(), 
                variant_name: String::from_str("").unwrap(), 
                description: String::from_str("test").unwrap(), 
                minimum_time: 0.0, process_parts: vec![
                    ProcessPart{ item: Item::Product(0), // input product
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Want(0), // input want
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Product(1), // Capital product
                        amount: 0.5, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Capital },
                    // placeholder for capital want
                    ProcessPart{ item: Item::Product(2), // output product
                        amount: 1.5, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output },
                    ProcessPart{ item: Item::Want(2), // output want
                        amount: 2.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output },
                ], 
                process_tags: vec![], 
                technology_requirement: None, tertiary_tech: None };

            // 1 of each item, should allow for only 1 iteration to be done.
            let mut available_products: HashMap<usize, PropertyInfo> = HashMap::new();
            available_products.insert(0, PropertyInfo::new(1.5));
            available_products.insert(1, PropertyInfo::new(4.0));
            available_products.insert(2, PropertyInfo::new(4.0));
            let mut available_wants = HashMap::new();
            available_wants.insert(0, 4.0);
            available_wants.insert(1, 4.0);
            available_wants.insert(2, 4.0);
            let result = test.do_process_with_property(&available_products, 
                &available_wants, 0.0, 
                None, true, &data, false);
            // check that it's all empty.
            assert!(result.iterations == 1.5);
            assert!(result.effective_iterations == 1.5);
            assert!(result.efficiency == 1.0);
            assert_eq!(result.input_output_products.len(), 2);
            assert!(*result.input_output_products.get(&0).unwrap() == -1.5);
            assert!(*result.input_output_products.get(&2).unwrap() == 2.25);
            assert_eq!(result.input_output_wants.len(), 2);
            assert!(*result.input_output_wants.get(&0).unwrap() == -1.5);
            assert!(*result.input_output_wants.get(&2).unwrap() == 3.0);
            assert_eq!(result.capital_products.len(), 1);
            assert!(*result.capital_products.get(&1).unwrap() == 0.75);
        }

        /// Only checks against reserved wants, not all possible reserves.
        /// Add those later, should we feel like it.
        #[test]
        pub fn return_process_correctly_restricts_based_on_property_reservations() {
            let mut data = DataManager::new();
            data.load_test_data().expect("Failed!");
            let test = Process{ id: 0, name: String::from_str("Test").unwrap(), 
                variant_name: String::from_str("").unwrap(), 
                description: String::from_str("test").unwrap(), 
                minimum_time: 0.0, process_parts: vec![
                    ProcessPart{ item: Item::Product(0), // input product
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Want(0), // input want
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Product(1), // Capital product
                        amount: 0.5, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Capital },
                    // placeholder for capital want
                    ProcessPart{ item: Item::Product(2), // output product
                        amount: 1.5, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output },
                    ProcessPart{ item: Item::Want(2), // output want
                        amount: 2.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output },
                ], 
                process_tags: vec![], 
                technology_requirement: None, tertiary_tech: None };

            // 1 of each item, should allow for only 1 iteration to be done.
            let mut available_products: HashMap<usize, PropertyInfo> = HashMap::new();
            available_products.insert(0, PropertyInfo::new(1.5));
            available_products.entry(0)
            .and_modify(|x| x.shift_to_want_reserve(0.5));
            available_products.insert(1, PropertyInfo::new(4.0));
            available_products.insert(2, PropertyInfo::new(4.0));
            let mut available_wants = HashMap::new();
            available_wants.insert(0, 4.0);
            available_wants.insert(1, 4.0);
            available_wants.insert(2, 4.0);
            let result = test.do_process_with_property(&available_products, 
                &available_wants, 0.0, 
                None, true, &data, false);
            // check that it's all empty.
            assert!(result.iterations == 1.0);
            assert!(result.effective_iterations == 1.0);
            assert!(result.efficiency == 1.0);
            assert_eq!(result.input_output_products.len(), 2);
            assert!(*result.input_output_products.get(&0).unwrap() == -1.0);
            assert!(*result.input_output_products.get(&2).unwrap() == 1.5);
            assert_eq!(result.input_output_wants.len(), 2);
            assert!(*result.input_output_wants.get(&0).unwrap() == -1.0);
            assert!(*result.input_output_wants.get(&2).unwrap() == 2.0);
            assert_eq!(result.capital_products.len(), 1);
            assert!(*result.capital_products.get(&1).unwrap() == 0.5);
        }

        #[test]
        pub fn return_process_returns_no_iteration_when_missing_input() {
            let mut data = DataManager::new();
            data.load_test_data().expect("Failed!");
            let test = Process{ id: 0, name: String::from_str("Test").unwrap(), 
                variant_name: String::from_str("").unwrap(), 
                description: String::from_str("test").unwrap(), 
                minimum_time: 0.0, process_parts: vec![
                    ProcessPart{ item: Item::Product(0), // input product
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Want(0), // input want
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Product(1), // Capital product
                        amount: 0.5, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Capital },
                    // placeholder for capital want
                    ProcessPart{ item: Item::Product(2), // output product
                        amount: 1.5, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output },
                    ProcessPart{ item: Item::Want(2), // output want
                        amount: 2.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output },
                ], 
                process_tags: vec![], 
                technology_requirement: None, tertiary_tech: None };

            // 1 of each item, should allow for only 1 iteration to be done.
            let mut available_products: HashMap<usize, PropertyInfo> = HashMap::new();
            available_products.insert(0, PropertyInfo::new(0.0));
            available_products.insert(1, PropertyInfo::new(1.0));
            available_products.insert(2, PropertyInfo::new(1.0));
            let mut available_wants = HashMap::new();
            available_wants.insert(0, 1.0);
            available_wants.insert(1, 1.0);
            available_wants.insert(2, 1.0);
            let result = test.do_process_with_property(&available_products, 
                &available_wants, 0.0, 
                None, true, &data, false);
            // check that it's all empty.
            assert!(result.iterations == 0.0);
            assert!(result.effective_iterations == 0.0);
            assert!(result.efficiency == 1.0);
            assert_eq!(result.input_output_products.len(), 0);
            assert_eq!(result.input_output_wants.len(), 0);
            assert_eq!(result.capital_products.len(), 0);
        }
        
        #[test]
        pub fn return_process_returns_no_iteration_when_missing_capital() {
            let mut data = DataManager::new();
            data.load_test_data().expect("Failed!");
            let test = Process{ id: 0, name: String::from_str("Test").unwrap(), 
                variant_name: String::from_str("").unwrap(), 
                description: String::from_str("test").unwrap(), 
                minimum_time: 0.0, process_parts: vec![
                    ProcessPart{ item: Item::Product(0), // input product
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Want(0), // input want
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Product(1), // Capital product
                        amount: 0.5, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Capital },
                    // placeholder for capital want
                    ProcessPart{ item: Item::Product(2), // output product
                        amount: 1.5, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output },
                    ProcessPart{ item: Item::Want(2), // output want
                        amount: 2.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output },
                ], 
                process_tags: vec![], 
                technology_requirement: None, tertiary_tech: None };

            // 1 of each item, should allow for only 1 iteration to be done.
            let mut available_products: HashMap<usize, PropertyInfo> = HashMap::new();
            available_products.insert(0, PropertyInfo::new(1.0));
            available_products.insert(1, PropertyInfo::new(0.0));
            available_products.insert(2, PropertyInfo::new(1.0));
            let mut available_wants = HashMap::new();
            available_wants.insert(0, 1.0);
            available_wants.insert(1, 1.0);
            available_wants.insert(2, 1.0);
            let result = test.do_process_with_property(&available_products, 
                &available_wants, 0.0, 
                None, true, &data, false);
            // check that it's all empty.
            assert!(result.iterations == 0.0);
            assert!(result.effective_iterations == 0.0);
            assert!(result.efficiency == 1.0);
            assert_eq!(result.input_output_products.len(), 0);
            assert_eq!(result.input_output_wants.len(), 0);
            assert_eq!(result.capital_products.len(), 0);
        }
    }

    mod effective_output_of_should {
        use std::str::FromStr;
        use super::super::*;

        #[test]
        pub fn return_correctly_for_input_capital_output_and_unrelated_items() {
            let test = Process{ id: 0, name: String::from_str("Test").unwrap(), 
                variant_name: String::from_str("").unwrap(), 
                description: String::from_str("test").unwrap(), 
                minimum_time: 0.0, process_parts: vec![
                    ProcessPart{ item: Item::Product(0), // input product
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Want(0), // input want
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Product(1), // Capital product
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Capital },
                    // placeholder for capital want
                    ProcessPart{ item: Item::Product(2), // output product
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output },
                    ProcessPart{ item: Item::Want(2), // output want
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output },
                ], 
                process_tags: vec![], 
                technology_requirement: None, tertiary_tech: None };

            // not in at all
            assert!(test.effective_output_of(Item::Want(3)) == 0.0);
            assert!(test.effective_output_of(Item::Product(3)) == 0.0);
            // input
            assert!(test.effective_output_of(Item::Want(0)) == 0.0);
            assert!(test.effective_output_of(Item::Product(0)) == 0.0);
            // capital
            // capital want placeholder.
            assert!(test.effective_output_of(Item::Product(1)) == 0.0);
            // output 
            assert!(test.effective_output_of(Item::Want(2)) == 1.0);
            assert!(test.effective_output_of(Item::Product(2)) == 1.0);
        }
    }

    mod do_process_should {
        use std::{collections::{HashMap, HashSet}, str::FromStr};
        use super::super::*;

        #[test]
        pub fn correctly_restrict_when_bonus_pushes_over_normal_but_normal_is_not_current_lowest() {
            let mut data = DataManager::new();
            let prod0 = Product {
                id: 0,
                name: String::from("Optional Input"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
                fractional: false,
                tags: vec![],
                wants: HashMap::new(),
                processes: HashSet::from([0]),
                failure_process: Some(0),
                use_processes: HashSet::new(),
                consumption_processes: HashSet::new(),
                maintenance_processes: HashSet::new(),
                tech_required: None,
                product_class: None,
            };
            let prod1 = Product {
                id: 1,
                name: String::from("Normal Input"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod2 = Product {
                id: 2,
                name: String::from("Fixed Input"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod3 = Product {
                id: 3,
                name: String::from("Fixed Output"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod4 = Product {
                id: 4,
                name: String::from("Normal Output"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod5 = Product {
                id: 5,
                name: String::from("Failed Output"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            // setup product with a failure process.
            let fail_proc = Process {
                id: 0,
                name: String::from("prod0 fail"),
                variant_name: String::new(),
                description: String::new(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart { 
                        item: Item::Product(0), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart { 
                        item: Item::Product(5), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output 
                    }
                ],
                process_tags: vec![],
                technology_requirement: None,
                tertiary_tech: None,
            };
            let test_proc = Process {
                id: 1,
                name: String::from("test process"),
                variant_name: String::new(),
                description: String::new(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart { 
                        item: Item::Product(0), 
                        amount: 1.0, 
                        part_tags: vec![
                            ProcessPartTag::Optional { 
                                missing_penalty: 0.0, 
                                final_bonus: 1.0 }
                        ], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart {
                        item: Item::Product(1), 
                        amount: 1.0, 
                        part_tags: vec![
                        ], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart {
                        item: Item::Product(2), 
                        amount: 1.0, 
                        part_tags: vec![
                            ProcessPartTag::Fixed
                        ], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart { 
                        item: Item::Product(3), 
                        amount: 1.0, 
                        part_tags: vec![
                            ProcessPartTag::Fixed
                        ], 
                        part: ProcessSectionTag::Output 
                    },
                    ProcessPart { 
                        item: Item::Product(4), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output 
                    }
                ],
                process_tags: vec![],
                technology_requirement: None,
                tertiary_tech: None,
            };
            data.products.insert(0, prod0);
            data.products.insert(1, prod1);
            data.products.insert(2, prod2);
            data.products.insert(3, prod3);
            data.products.insert(4, prod4);
            data.products.insert(5, prod5);
            data.processes.insert(0, fail_proc);
            data.processes.insert(1, test_proc);
            let test_proc = data.processes.get(&1).unwrap();

            let mut available_products = HashMap::new();
            available_products.insert(0, 4.0); // optional, should use all
            available_products.insert(1, 4.0); // normal, should only 1
            available_products.insert(2, 3.0); // fixed, should use both via bonus.
            let available_wants: HashMap<usize, f64> = HashMap::new();
            let result = test_proc.do_process(&available_products, 
                &available_wants, 
                0.0, 
                None, 
                false, 
                &data);
            
            assert_eq!(result.effective_iterations, 4.0);
            assert_eq!(result.iterations, 3.0);
            assert_eq!(result.efficiency, 4.0/3.0);
            assert_eq!(result.input_output_wants.len(), 0);
            assert_eq!(result.input_output_products.len(), 6);
            // Note, this rounds due to floating point rounding errors around 
            assert_eq!(result.input_output_products[&0].round(), -1.0);
            assert_eq!(result.input_output_products[&1], -4.0);
            assert_eq!(result.input_output_products[&2], -3.0);
            assert_eq!(result.input_output_products[&3], 3.0);
            assert_eq!(result.input_output_products[&4], 4.0);
            // rounded to to floating point errors.
            assert_eq!(result.input_output_products[&5].round(), 1.0);
            assert_eq!(result.capital_products.len(), 0);
        }
        
        #[test]
        pub fn correctly_restrict_to_available_normal_products() {
            let mut data = DataManager::new();
            let prod0 = Product {
                id: 0,
                name: String::from("Optional Input"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
                fractional: false,
                tags: vec![],
                wants: HashMap::new(),
                processes: HashSet::from([0]),
                failure_process: Some(0),
                use_processes: HashSet::new(),
                consumption_processes: HashSet::new(),
                maintenance_processes: HashSet::new(),
                tech_required: None,
                product_class: None,
            };
            let prod1 = Product {
                id: 1,
                name: String::from("Normal Input"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod2 = Product {
                id: 2,
                name: String::from("Fixed Input"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod3 = Product {
                id: 3,
                name: String::from("Fixed Output"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod4 = Product {
                id: 4,
                name: String::from("Normal Output"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod5 = Product {
                id: 5,
                name: String::from("Failed Output"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            // setup product with a failure process.
            let fail_proc = Process {
                id: 0,
                name: String::from("prod0 fail"),
                variant_name: String::new(),
                description: String::new(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart { 
                        item: Item::Product(0), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart { 
                        item: Item::Product(5), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output 
                    }
                ],
                process_tags: vec![],
                technology_requirement: None,
                tertiary_tech: None,
            };
            let test_proc = Process {
                id: 1,
                name: String::from("test process"),
                variant_name: String::new(),
                description: String::new(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart { 
                        item: Item::Product(0), 
                        amount: 1.0, 
                        part_tags: vec![
                            ProcessPartTag::Optional { 
                                missing_penalty: 0.0, 
                                final_bonus: 1.0 }
                        ], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart {
                        item: Item::Product(1), 
                        amount: 1.0, 
                        part_tags: vec![
                        ], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart {
                        item: Item::Product(2), 
                        amount: 1.0, 
                        part_tags: vec![
                            ProcessPartTag::Fixed
                        ], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart { 
                        item: Item::Product(3), 
                        amount: 1.0, 
                        part_tags: vec![
                            ProcessPartTag::Fixed
                        ], 
                        part: ProcessSectionTag::Output 
                    },
                    ProcessPart { 
                        item: Item::Product(4), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output 
                    }
                ],
                process_tags: vec![],
                technology_requirement: None,
                tertiary_tech: None,
            };
            data.products.insert(0, prod0);
            data.products.insert(1, prod1);
            data.products.insert(2, prod2);
            data.products.insert(3, prod3);
            data.products.insert(4, prod4);
            data.products.insert(5, prod5);
            data.processes.insert(0, fail_proc);
            data.processes.insert(1, test_proc);
            let test_proc = data.processes.get(&1).unwrap();

            let mut available_products = HashMap::new();
            available_products.insert(0, 4.0); // optional, should use all
            available_products.insert(1, 2.0); // normal, should only 1
            available_products.insert(2, 4.0); // fixed, should use both via bonus.
            let available_wants: HashMap<usize, f64> = HashMap::new();
            let result = test_proc.do_process(&available_products, 
                &available_wants, 
                0.0, 
                None, 
                false, 
                &data);
            
            assert_eq!(result.effective_iterations, 2.0);
            assert_eq!(result.iterations, 1.0);
            assert_eq!(result.efficiency, 2.0);
            assert_eq!(result.input_output_wants.len(), 0);
            assert_eq!(result.input_output_products.len(), 6);
            assert_eq!(result.input_output_products[&0], -1.0);
            assert_eq!(result.input_output_products[&1], -2.0);
            assert_eq!(result.input_output_products[&2], -1.0);
            assert_eq!(result.input_output_products[&3], 1.0);
            assert_eq!(result.input_output_products[&4], 2.0);
            assert_eq!(result.input_output_products[&5], 1.0);
            assert_eq!(result.capital_products.len(), 0);
        }
        
        #[test]
        pub fn correctly_restrict_to_available_fixed() {
            let mut data = DataManager::new();
            let prod0 = Product {
                id: 0,
                name: String::from("Optional Input"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
                fractional: false,
                tags: vec![],
                wants: HashMap::new(),
                processes: HashSet::from([0]),
                failure_process: Some(0),
                use_processes: HashSet::new(),
                consumption_processes: HashSet::new(),
                maintenance_processes: HashSet::new(),
                tech_required: None,
                product_class: None,
            };
            let prod1 = Product {
                id: 1,
                name: String::from("Normal Input"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod2 = Product {
                id: 2,
                name: String::from("Fixed Input"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod3 = Product {
                id: 3,
                name: String::from("Fixed Output"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod4 = Product {
                id: 4,
                name: String::from("Normal Output"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod5 = Product {
                id: 5,
                name: String::from("Failed Output"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            // setup product with a failure process.
            let fail_proc = Process {
                id: 0,
                name: String::from("prod0 fail"),
                variant_name: String::new(),
                description: String::new(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart { 
                        item: Item::Product(0), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart { 
                        item: Item::Product(5), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output 
                    }
                ],
                process_tags: vec![],
                technology_requirement: None,
                tertiary_tech: None,
            };
            let test_proc = Process {
                id: 1,
                name: String::from("test process"),
                variant_name: String::new(),
                description: String::new(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart { 
                        item: Item::Product(0), 
                        amount: 1.0, 
                        part_tags: vec![
                            ProcessPartTag::Optional { 
                                missing_penalty: 0.0, 
                                final_bonus: 1.0 }
                        ], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart {
                        item: Item::Product(1), 
                        amount: 1.0, 
                        part_tags: vec![
                        ], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart {
                        item: Item::Product(2), 
                        amount: 1.0, 
                        part_tags: vec![
                            ProcessPartTag::Fixed
                        ], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart { 
                        item: Item::Product(3), 
                        amount: 1.0, 
                        part_tags: vec![
                            ProcessPartTag::Fixed
                        ], 
                        part: ProcessSectionTag::Output 
                    },
                    ProcessPart { 
                        item: Item::Product(4), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output 
                    }
                ],
                process_tags: vec![],
                technology_requirement: None,
                tertiary_tech: None,
            };
            data.products.insert(0, prod0);
            data.products.insert(1, prod1);
            data.products.insert(2, prod2);
            data.products.insert(3, prod3);
            data.products.insert(4, prod4);
            data.products.insert(5, prod5);
            data.processes.insert(0, fail_proc);
            data.processes.insert(1, test_proc);
            let test_proc = data.processes.get(&1).unwrap();

            let mut available_products = HashMap::new();
            available_products.insert(0, 4.0); // optional, should use all
            available_products.insert(1, 4.0); // normal, should only 1
            available_products.insert(2, 1.0); // fixed, should use both via bonus.
            let available_wants: HashMap<usize, f64> = HashMap::new();
            let result = test_proc.do_process(&available_products, 
                &available_wants, 
                0.0, 
                None, 
                false, 
                &data);
            
            assert_eq!(result.effective_iterations, 2.0);
            assert_eq!(result.iterations, 1.0);
            assert_eq!(result.efficiency, 2.0);
            assert_eq!(result.input_output_wants.len(), 0);
            assert_eq!(result.input_output_products.len(), 6);
            assert_eq!(result.input_output_products[&0], -1.0);
            assert_eq!(result.input_output_products[&1], -2.0);
            assert_eq!(result.input_output_products[&2], -1.0);
            assert_eq!(result.input_output_products[&3], 1.0);
            assert_eq!(result.input_output_products[&4], 2.0);
            assert_eq!(result.input_output_products[&5], 1.0);
            assert_eq!(result.capital_products.len(), 0);
        }

        #[test]
        pub fn use_and_consume_optional_input_correctly() {
            let mut data = DataManager::new();
            let prod0 = Product {
                id: 0,
                name: String::from("Optional Input"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
                fractional: false,
                tags: vec![],
                wants: HashMap::new(),
                processes: HashSet::from([0]),
                failure_process: Some(0),
                use_processes: HashSet::new(),
                consumption_processes: HashSet::new(),
                maintenance_processes: HashSet::new(),
                tech_required: None,
                product_class: None,
            };
            let prod1 = Product {
                id: 1,
                name: String::from("Normal Input"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod2 = Product {
                id: 2,
                name: String::from("Fixed Input"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod3 = Product {
                id: 3,
                name: String::from("Fixed Output"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod4 = Product {
                id: 4,
                name: String::from("Normal Output"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod5 = Product {
                id: 5,
                name: String::from("Failed Output"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            // setup product with a failure process.
            let fail_proc = Process {
                id: 0,
                name: String::from("prod0 fail"),
                variant_name: String::new(),
                description: String::new(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart { 
                        item: Item::Product(0), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart { 
                        item: Item::Product(5), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output 
                    }
                ],
                process_tags: vec![],
                technology_requirement: None,
                tertiary_tech: None,
            };
            let test_proc = Process {
                id: 1,
                name: String::from("test process"),
                variant_name: String::new(),
                description: String::new(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart { 
                        item: Item::Product(0), 
                        amount: 1.0, 
                        part_tags: vec![
                            ProcessPartTag::Optional { 
                                missing_penalty: 0.0, 
                                final_bonus: 1.0 }
                        ], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart {
                        item: Item::Product(1), 
                        amount: 1.0, 
                        part_tags: vec![
                        ], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart {
                        item: Item::Product(2), 
                        amount: 1.0, 
                        part_tags: vec![
                            ProcessPartTag::Fixed
                        ], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart { 
                        item: Item::Product(3), 
                        amount: 1.0, 
                        part_tags: vec![
                            ProcessPartTag::Fixed
                        ], 
                        part: ProcessSectionTag::Output 
                    },
                    ProcessPart { 
                        item: Item::Product(4), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output 
                    }
                ],
                process_tags: vec![],
                technology_requirement: None,
                tertiary_tech: None,
            };
            data.products.insert(0, prod0);
            data.products.insert(1, prod1);
            data.products.insert(2, prod2);
            data.products.insert(3, prod3);
            data.products.insert(4, prod4);
            data.products.insert(5, prod5);
            data.processes.insert(0, fail_proc);
            data.processes.insert(1, test_proc);
            let test_proc = data.processes.get(&1).unwrap();

            let mut available_products = HashMap::new();
            available_products.insert(0, 1.0); // optional, should use all
            available_products.insert(1, 2.0); // fixed, should only 1
            available_products.insert(2, 1.0); // nolmal, should use both via bonus.
            let available_wants: HashMap<usize, f64> = HashMap::new();
            let result = test_proc.do_process(&available_products, 
                &available_wants, 
                0.0, 
                None, 
                false, 
                &data);
            
            assert_eq!(result.effective_iterations, 2.0);
            assert_eq!(result.iterations, 1.0);
            assert_eq!(result.efficiency, 2.0);
            assert_eq!(result.input_output_wants.len(), 0);
            assert_eq!(result.input_output_products.len(), 6);
            assert_eq!(result.input_output_products[&0], -1.0);
            assert_eq!(result.input_output_products[&1], -2.0);
            assert_eq!(result.input_output_products[&2], -1.0);
            assert_eq!(result.input_output_products[&3], 1.0);
            assert_eq!(result.input_output_products[&4], 2.0);
            assert_eq!(result.input_output_products[&5], 1.0);
            assert_eq!(result.capital_products.len(), 0);
        }

        #[test]
        pub fn use_optional_capital_correctly() {
            let mut data = DataManager::new();
            let prod0 = Product {
                id: 0,
                name: String::from("Optional Capital"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
                fractional: false,
                tags: vec![],
                wants: HashMap::new(),
                processes: HashSet::from([0]),
                failure_process: Some(0),
                use_processes: HashSet::new(),
                consumption_processes: HashSet::new(),
                maintenance_processes: HashSet::new(),
                tech_required: None,
                product_class: None,
            };
            let prod1 = Product {
                id: 1,
                name: String::from("Normal Input"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod2 = Product {
                id: 2,
                name: String::from("Fixed Input"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod3 = Product {
                id: 3,
                name: String::from("Fixed Output"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod4 = Product {
                id: 4,
                name: String::from("Normal Output"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod5 = Product {
                id: 5,
                name: String::from("Failed Output"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            // setup product with a failure process.
            let fail_proc = Process {
                id: 0,
                name: String::from("prod0 fail"),
                variant_name: String::new(),
                description: String::new(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart { 
                        item: Item::Product(0), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart { 
                        item: Item::Product(3), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output 
                    }
                ],
                process_tags: vec![],
                technology_requirement: None,
                tertiary_tech: None,
            };
            let test_proc = Process {
                id: 1,
                name: String::from("test process"),
                variant_name: String::new(),
                description: String::new(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart { 
                        item: Item::Product(0), 
                        amount: 1.0, 
                        part_tags: vec![
                            ProcessPartTag::Optional { 
                                missing_penalty: 0.0, 
                                final_bonus: 1.0 }
                        ], 
                        part: ProcessSectionTag::Capital 
                    },
                    ProcessPart {
                        item: Item::Product(1), 
                        amount: 1.0, 
                        part_tags: vec![
                        ], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart {
                        item: Item::Product(2), 
                        amount: 1.0, 
                        part_tags: vec![
                            ProcessPartTag::Fixed
                        ], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart { 
                        item: Item::Product(3), 
                        amount: 1.0, 
                        part_tags: vec![
                            ProcessPartTag::Fixed
                        ], 
                        part: ProcessSectionTag::Output 
                    },
                    ProcessPart { 
                        item: Item::Product(4), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output 
                    }
                ],
                process_tags: vec![],
                technology_requirement: None,
                tertiary_tech: None,
            };
            data.products.insert(0, prod0);
            data.products.insert(1, prod1);
            data.products.insert(2, prod2);
            data.products.insert(3, prod3);
            data.products.insert(4, prod4);
            data.products.insert(5, prod5);
            data.processes.insert(0, fail_proc);
            data.processes.insert(1, test_proc);
            let test_proc = data.processes.get(&1).unwrap();

            let mut available_products = HashMap::new();
            available_products.insert(0, 1.0); // optional, should use all
            available_products.insert(1, 2.0); // fixed, should only 1
            available_products.insert(2, 1.0); // nolmal, should use both via bonus.
            let available_wants: HashMap<usize, f64> = HashMap::new();
            let result = test_proc.do_process(&available_products, 
                &available_wants, 
                0.0, 
                None, 
                false, 
                &data);
            
            assert_eq!(result.effective_iterations, 2.0);
            assert_eq!(result.iterations, 1.0);
            assert_eq!(result.efficiency, 2.0);
            assert_eq!(result.input_output_wants.len(), 0);
            //assert_eq!(result.input_output_products[&0], -1.0);
            assert_eq!(result.input_output_products.len(), 4);
            assert_eq!(result.input_output_products[&1], -2.0);
            assert_eq!(result.input_output_products[&2], -1.0);
            assert_eq!(result.input_output_products[&3], 1.0);
            assert_eq!(result.input_output_products[&4], 2.0);
            assert_eq!(result.capital_products[&0], 1.0);
        }

        #[test]
        pub fn correctly_consume_tagged_consumption_input() {
            let mut data = DataManager::new();
            let prod0 = Product {
                id: 0,
                name: String::from("Consumed Input"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
                fractional: false,
                tags: vec![],
                wants: HashMap::new(),
                processes: HashSet::from([0]),
                failure_process: Some(0),
                use_processes: HashSet::new(),
                consumption_processes: HashSet::new(),
                maintenance_processes: HashSet::new(),
                tech_required: None,
                product_class: None,
            };
            let prod1 = Product {
                id: 1,
                name: String::from("Normal Input"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod2 = Product {
                id: 2,
                name: String::from("Normal Output"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            let prod3 = Product {
                id: 3,
                name: String::from("Failed Output"),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1000),
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
            };
            // setup product with a failure process.
            let fail_proc = Process {
                id: 0,
                name: String::from("prod0 fail"),
                variant_name: String::new(),
                description: String::new(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart { 
                        item: Item::Product(0), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart { 
                        item: Item::Product(3), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output 
                    }
                ],
                process_tags: vec![],
                technology_requirement: None,
                tertiary_tech: None,
            };
            let test_proc = Process {
                id: 1,
                name: String::from("test product"),
                variant_name: String::new(),
                description: String::new(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart { 
                        item: Item::Product(0), 
                        amount: 1.0, 
                        part_tags: vec![
                            ProcessPartTag::Consumed
                        ], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart { 
                        item: Item::Product(1), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart { 
                        item: Item::Product(2), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output 
                    }
                ],
                process_tags: vec![],
                technology_requirement: None,
                tertiary_tech: None,
            };
            data.products.insert(0, prod0);
            data.products.insert(1, prod1);
            data.products.insert(2, prod2);
            data.products.insert(3, prod3);
            data.processes.insert(0, fail_proc);
            data.processes.insert(1, test_proc);
            let test_proc = data.processes.get(&1).unwrap();

            let mut available_products = HashMap::new();
            available_products.insert(0, 2.0);
            available_products.insert(1, 2.0);
            let available_wants: HashMap<usize, f64> = HashMap::new();
            let result = test_proc.do_process(&available_products, 
                &available_wants, 
                0.0, 
                None, 
                false, 
                &data);
            
            assert_eq!(result.effective_iterations, 2.0);
            assert_eq!(result.iterations, 2.0);
            assert_eq!(result.efficiency, 1.0);
            assert_eq!(result.input_output_wants.len(), 0);
            assert_eq!(result.input_output_products.len(), 4);
            assert_eq!(result.input_output_products[&0], -2.0);
            assert_eq!(result.input_output_products[&1], -2.0);
            assert_eq!(result.input_output_products[&2], 2.0);
            assert_eq!(result.input_output_products[&3], 2.0);
            assert_eq!(result.capital_products.len(), 0);
        }

        #[test]
        pub fn return_process_returns_empty_correctly() {
            let mut data = DataManager::new();
            data.load_test_data().expect("Failed!");
            let test = Process{ id: 0, name: String::from_str("Test").unwrap(), 
                variant_name: String::from_str("").unwrap(), 
                description: String::from_str("test").unwrap(), 
                minimum_time: 0.0, process_parts: vec![
                    ProcessPart{ item: Item::Product(0), // input product
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Want(0), // input want
                        amount: 2.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Product(1), // Capital product
                        amount: 0.5, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Capital },
                    // placeholder for capital want
                    ProcessPart{ item: Item::Product(2), // output product
                        amount: 1.5, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output },
                    ProcessPart{ item: Item::Want(2), // output want
                        amount: 2.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output },
                ], 
                process_tags: vec![], 
                technology_requirement: None, tertiary_tech: None };

            let available_products = HashMap::new();
            let available_wants = HashMap::new();
            let result = test.do_process(&available_products, 
                &available_wants, 0.0, 
                None, true, &data);
            // check that it's all empty.
            assert!(result.iterations == 0.0);
            assert!(result.effective_iterations == 0.0);
            assert!(result.efficiency == 1.0);
            assert_eq!(result.input_output_products.len(), 0);
            assert_eq!(result.input_output_wants.len(), 0);
            assert_eq!(result.capital_products.len(), 0);
        }

        #[test]
        pub fn return_process_returns_single_iteration_correctly() {
            let mut data = DataManager::new();
            data.load_test_data().expect("Failed!");
            let test = Process{ id: 0, name: String::from_str("Test").unwrap(), 
                variant_name: String::from_str("").unwrap(), 
                description: String::from_str("test").unwrap(), 
                minimum_time: 0.0, process_parts: vec![
                    ProcessPart{ item: Item::Product(0), // input product
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Want(0), // input want
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Product(1), // Capital product
                        amount: 0.5, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Capital },
                    // placeholder for capital want
                    ProcessPart{ item: Item::Product(2), // output product
                        amount: 1.5, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output },
                    ProcessPart{ item: Item::Want(2), // output want
                        amount: 2.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output },
                ], 
                process_tags: vec![], 
                technology_requirement: None, tertiary_tech: None };

            // 1 of each item, should allow for only 1 iteration to be done.
            let mut available_products = HashMap::new();
            available_products.insert(0, 1.0);
            available_products.insert(1, 1.0);
            available_products.insert(2, 1.0);
            let mut available_wants = HashMap::new();
            available_wants.insert(0, 1.0);
            available_wants.insert(1, 1.0);
            available_wants.insert(2, 1.0);
            let result = test.do_process(&available_products, 
                &available_wants, 0.0, 
                None, true, &data);
            // check that it's all empty.
            assert!(result.iterations == 1.0);
            assert!(result.effective_iterations == 1.0);
            assert!(result.efficiency == 1.0);
            assert_eq!(result.input_output_products.len(), 2);
            assert!(*result.input_output_products.get(&0).unwrap() == -1.0);
            assert!(*result.input_output_products.get(&2).unwrap() == 1.5);
            assert_eq!(result.input_output_wants.len(), 2);
            assert!(*result.input_output_wants.get(&0).unwrap() == -1.0);
            assert!(*result.input_output_wants.get(&2).unwrap() == 2.0);
            assert_eq!(result.capital_products.len(), 1);
            assert!(*result.capital_products.get(&1).unwrap() == 0.5);
        }

        #[test]
        pub fn return_process_returns_multiple_iterations_correctly() {
            let mut data = DataManager::new();
            data.load_test_data().expect("Failed!");
            let test = Process{ id: 0, name: String::from_str("Test").unwrap(), 
                variant_name: String::from_str("").unwrap(), 
                description: String::from_str("test").unwrap(), 
                minimum_time: 0.0, process_parts: vec![
                    ProcessPart{ item: Item::Product(0), // input product
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Want(0), // input want
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Product(1), // Capital product
                        amount: 0.5, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Capital },
                    // placeholder for capital want
                    ProcessPart{ item: Item::Product(2), // output product
                        amount: 1.5, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output },
                    ProcessPart{ item: Item::Want(2), // output want
                        amount: 2.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output },
                ], 
                process_tags: vec![], 
                technology_requirement: None, tertiary_tech: None };

            // 1 of each item, should allow for only 1 iteration to be done.
            let mut available_products = HashMap::new();
            available_products.insert(0, 1.5);
            available_products.insert(1, 4.0);
            available_products.insert(2, 4.0);
            let mut available_wants = HashMap::new();
            available_wants.insert(0, 4.0);
            available_wants.insert(1, 4.0);
            available_wants.insert(2, 4.0);
            let result = test.do_process(&available_products, 
                &available_wants, 0.0, 
                None, true, &data);
            // check that it's all empty.
            assert!(result.iterations == 1.5);
            assert!(result.effective_iterations == 1.5);
            assert!(result.efficiency == 1.0);
            assert_eq!(result.input_output_products.len(), 2);
            assert!(*result.input_output_products.get(&0).unwrap() == -1.5);
            assert!(*result.input_output_products.get(&2).unwrap() == 2.25);
            assert_eq!(result.input_output_wants.len(), 2);
            assert!(*result.input_output_wants.get(&0).unwrap() == -1.5);
            assert!(*result.input_output_wants.get(&2).unwrap() == 3.0);
            assert_eq!(result.capital_products.len(), 1);
            assert!(*result.capital_products.get(&1).unwrap() == 0.75);
        }

        #[test]
        pub fn return_process_returns_no_iteration_when_missing_input() {
            let mut data = DataManager::new();
            data.load_test_data().expect("Failed!");
            let test = Process{ id: 0, name: String::from_str("Test").unwrap(), 
                variant_name: String::from_str("").unwrap(), 
                description: String::from_str("test").unwrap(), 
                minimum_time: 0.0, process_parts: vec![
                    ProcessPart{ item: Item::Product(0), // input product
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Want(0), // input want
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Product(1), // Capital product
                        amount: 0.5, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Capital },
                    // placeholder for capital want
                    ProcessPart{ item: Item::Product(2), // output product
                        amount: 1.5, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output },
                    ProcessPart{ item: Item::Want(2), // output want
                        amount: 2.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output },
                ], 
                process_tags: vec![], 
                technology_requirement: None, tertiary_tech: None };

            // 1 of each item, should allow for only 1 iteration to be done.
            let mut available_products = HashMap::new();
            available_products.insert(0, 0.0);
            available_products.insert(1, 1.0);
            available_products.insert(2, 1.0);
            let mut available_wants = HashMap::new();
            available_wants.insert(0, 1.0);
            available_wants.insert(1, 1.0);
            available_wants.insert(2, 1.0);
            let result = test.do_process(&available_products, 
                &available_wants, 0.0, 
                None, true, &data);
            // check that it's all empty.
            assert!(result.iterations == 0.0);
            assert!(result.effective_iterations == 0.0);
            assert!(result.efficiency == 1.0);
            assert_eq!(result.input_output_products.len(), 0);
            assert_eq!(result.input_output_wants.len(), 0);
            assert_eq!(result.capital_products.len(), 0);
        }
        
        #[test]
        pub fn return_process_returns_no_iteration_when_missing_capital() {
            let mut data = DataManager::new();
            data.load_test_data().expect("Failed!");
            let test = Process{ id: 0, name: String::from_str("Test").unwrap(), 
                variant_name: String::from_str("").unwrap(), 
                description: String::from_str("test").unwrap(), 
                minimum_time: 0.0, process_parts: vec![
                    ProcessPart{ item: Item::Product(0), // input product
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Want(0), // input want
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Product(1), // Capital product
                        amount: 0.5, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Capital },
                    // placeholder for capital want
                    ProcessPart{ item: Item::Product(2), // output product
                        amount: 1.5, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output },
                    ProcessPart{ item: Item::Want(2), // output want
                        amount: 2.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output },
                ], 
                process_tags: vec![], 
                technology_requirement: None, tertiary_tech: None };

            // 1 of each item, should allow for only 1 iteration to be done.
            let mut available_products = HashMap::new();
            available_products.insert(0, 1.0);
            available_products.insert(1, 0.0);
            available_products.insert(2, 1.0);
            let mut available_wants = HashMap::new();
            available_wants.insert(0, 1.0);
            available_wants.insert(1, 1.0);
            available_wants.insert(2, 1.0);
            let result = test.do_process(&available_products, 
                &available_wants, 0.0, 
                None, true, &data);
            // check that it's all empty.
            assert!(result.iterations == 0.0);
            assert!(result.effective_iterations == 0.0);
            assert!(result.efficiency == 1.0);
            assert_eq!(result.input_output_products.len(), 0);
            assert_eq!(result.input_output_wants.len(), 0);
            assert_eq!(result.capital_products.len(), 0);
        }

        #[test]
        pub fn should_return_correctly_when_everything_needed_is_given() {
            let mut data = DataManager::new();
            data.load_test_data().expect("Failed!");
            let test = Process{ id: 0, name: String::from_str("Test").unwrap(), 
                variant_name: String::from_str("").unwrap(), 
                description: String::from_str("test").unwrap(), 
                minimum_time: 0.0, process_parts: vec![
                    ProcessPart{ item: Item::Product(0), // input product
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Want(0), // input want
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Product(1), // Capital product
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Capital },
                    // placeholder for capital want
                    ProcessPart{ item: Item::Product(2), // output product
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output },
                    ProcessPart{ item: Item::Want(2), // output want
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output },
                ], 
                process_tags: vec![], 
                technology_requirement: None, tertiary_tech: None };
            
            // build available items
            let mut avail_products = HashMap::new();
            avail_products.insert(0, 2.0);
            avail_products.insert(1, 2.0);
            let mut avail_wants = HashMap::new();
            avail_wants.insert(0, 2.0);
            
            let results = test.do_process(&avail_products, &avail_wants, 
                0.0, None, false, &data);
            
            assert_eq!(results.capital_products.len(), 1);
            assert!(*results.capital_products.get(&1).unwrap() == 2.0);
            assert_eq!(results.input_output_products.len(), 2);
            assert!(*results.input_output_products.get(&0).unwrap() == -2.0);
            assert!(*results.input_output_products.get(&2).unwrap() == 2.0);
            assert_eq!(results.input_output_wants.len(), 2);
            assert!(*results.input_output_products.get(&0).unwrap() == -2.0);
            assert!(*results.input_output_products.get(&2).unwrap() == 2.0);
            assert!(results.effective_iterations == 2.0);
            assert_eq!(results.efficiency, 1.0);
            assert!(results.iterations == 2.0);
        }

        #[test]
        pub fn should_return_empty_when_input_product_is_missing() {
            let mut data = DataManager::new();
            data.load_test_data().expect("Failed!");
            let test = Process{ id: 0, name: String::from_str("Test").unwrap(), 
                variant_name: String::from_str("").unwrap(), 
                description: String::from_str("test").unwrap(), 
                minimum_time: 0.0, process_parts: vec![
                    ProcessPart{ item: Item::Product(0), // input product
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Want(0), // input want
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Product(1), // Capital product
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Capital },
                    // placeholder for capital want
                    ProcessPart{ item: Item::Product(2), // output product
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output },
                    ProcessPart{ item: Item::Want(2), // output want
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output },
                ], 
                process_tags: vec![], 
                technology_requirement: None, tertiary_tech: None };
            
            // build available items
            let mut avail_products = HashMap::new();
            //avail_products.insert(0, 2.0);
            avail_products.insert(1, 2.0);
            let mut avail_wants = HashMap::new();
            avail_wants.insert(0, 2.0);
            
            let results = test.do_process(&avail_products, &avail_wants, 
                0.0, None, false, &data);
            
            assert_eq!(results.capital_products.len(), 0);
            assert_eq!(results.input_output_products.len(), 0);
            assert_eq!(results.input_output_wants.len(), 0);
            assert!(results.effective_iterations == 0.0);
            assert!(results.efficiency == 1.0);
            assert!(results.iterations == 0.0);
        }

        #[test]
        pub fn should_return_empty_when_capital_product_is_missing() {
            let mut data = DataManager::new();
            data.load_test_data().expect("Failed!");
            let test = Process{ id: 0, name: String::from_str("Test").unwrap(), 
                variant_name: String::from_str("").unwrap(), 
                description: String::from_str("test").unwrap(), 
                minimum_time: 0.0, process_parts: vec![
                    ProcessPart{ item: Item::Product(0), // input product
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Want(0), // input want
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Product(1), // Capital product
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Capital },
                    // placeholder for capital want
                    ProcessPart{ item: Item::Product(2), // output product
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output },
                    ProcessPart{ item: Item::Want(2), // output want
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output },
                ], 
                process_tags: vec![], 
                technology_requirement: None, tertiary_tech: None };
            
            // build available items
            let mut avail_products = HashMap::new();
            avail_products.insert(0, 2.0);
            //avail_products.insert(1, 2.0);
            let mut avail_wants = HashMap::new();
            avail_wants.insert(0, 2.0);
            
            let results = test.do_process(&avail_products, &avail_wants, 
                0.0, None, false, &data);
            
            assert_eq!(results.capital_products.len(), 0);
            assert_eq!(results.input_output_products.len(), 0);
            assert_eq!(results.input_output_wants.len(), 0);
            assert!(results.effective_iterations == 0.0);
            assert!(results.efficiency == 1.0);
            assert!(results.iterations == 0.0);
        }

        #[test]
        pub fn should_return_empty_when_input_want_is_missing() {
            let mut data = DataManager::new();
            data.load_test_data().expect("Failed!");
            let test = Process{ id: 0, name: String::from_str("Test").unwrap(), 
                variant_name: String::from_str("").unwrap(), 
                description: String::from_str("test").unwrap(), 
                minimum_time: 0.0, process_parts: vec![
                    ProcessPart{ item: Item::Product(0), // input product
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Want(0), // input want
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Product(1), // Capital product
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Capital },
                    // placeholder for capital want
                    ProcessPart{ item: Item::Product(2), // output product
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output },
                    ProcessPart{ item: Item::Want(2), // output want
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output },
                ], 
                process_tags: vec![], 
                technology_requirement: None, tertiary_tech: None };
            
            // build available items
            let mut avail_products = HashMap::new();
            avail_products.insert(0, 2.0);
            avail_products.insert(1, 2.0);
            let avail_wants = HashMap::new();
            //avail_wants.insert(0, 2.0);
            
            let results = test.do_process(&avail_products, &avail_wants, 
                0.0, None, false, &data);
            
            assert_eq!(results.capital_products.len(), 0);
            assert_eq!(results.input_output_products.len(), 0);
            assert_eq!(results.input_output_wants.len(), 0);
            assert!(results.effective_iterations == 0.0);
            assert!(results.efficiency == 1.0);
            assert!(results.iterations == 0.0);
        }
    }

    /// TODO, rework this to be more space efficinet (loop over options rather than write it all out)
    #[test]
    pub fn should_return_correctly_on_input_or_capital_to_output_checking(){
        let mut test = Process {
            id: 0,
            name: String::from("Test"),
            variant_name: String::from("Variant"),
            description: String::new(),
            minimum_time: 0.0,
            process_parts: vec![],
            process_tags: vec![],
            technology_requirement: None,
            tertiary_tech: None,
        };

        let mut test_other = Process {
            id: 1,
            name: String::from("Test Other"),
            variant_name: String::from("Variant"),
            description: String::new(),
            minimum_time: 0.0,
            process_parts: vec![],
            process_tags: vec![],
            technology_requirement: None,
            tertiary_tech: None,
        };

        // product never matches want ever, don't bother checking those mismatches
        // test input product to test_other input correct (never match)
        let input = ProcessPart{
            item: Item::Product(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Input,
        };
        let output = ProcessPart{
            item: Item::Product(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Input,
        };
        test.process_parts.push(input);
        test_other.process_parts.push(output);
        assert!(!test.takes_input_from(&test_other));
        assert!(!test.takes_capital_from(&test_other));
        assert!(!test.gives_output_to_others_input(&test_other));
        assert!(!test.gives_output_to_others_capital(&test_other));
        test.process_parts.clear();
        test_other.process_parts.clear();
            // want check
        let input = ProcessPart{
            item: Item::Want(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Input,
        };
        let output = ProcessPart{
            item: Item::Want(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Input,
        };
        test.process_parts.push(input);
        test_other.process_parts.push(output);
        assert!(!test.takes_input_from(&test_other));
        assert!(!test.takes_capital_from(&test_other));
        assert!(!test.gives_output_to_others_input(&test_other));
        assert!(!test.gives_output_to_others_capital(&test_other));
        test.process_parts.clear();
        test_other.process_parts.clear();
        // input to capital (never match)
        let input = ProcessPart{
            item: Item::Product(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Input,
        };
        let output = ProcessPart{
            item: Item::Product(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Capital,
        };
        test.process_parts.push(input);
        test_other.process_parts.push(output);
        assert!(!test.takes_input_from(&test_other));
        assert!(!test.takes_capital_from(&test_other));
        assert!(!test.gives_output_to_others_input(&test_other));
        assert!(!test.gives_output_to_others_capital(&test_other));
        test.process_parts.clear();
        test_other.process_parts.clear();
            // want check
        let input = ProcessPart{
            item: Item::Want(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Input,
        };
        let output = ProcessPart{
            item: Item::Want(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Capital,
        };
        test.process_parts.push(input);
        test_other.process_parts.push(output);
        assert!(!test.takes_input_from(&test_other));
        assert!(!test.takes_capital_from(&test_other));
        assert!(!test.gives_output_to_others_input(&test_other));
        assert!(!test.gives_output_to_others_capital(&test_other));
        test.process_parts.clear();
        test_other.process_parts.clear();
        // input to output
        let input = ProcessPart{
            item: Item::Product(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Input,
        };
        let output = ProcessPart{
            item: Item::Product(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Output,
        };
        test.process_parts.push(input);
        test_other.process_parts.push(output);
        assert!(test.takes_input_from(&test_other));
        assert!(!test.takes_capital_from(&test_other));
        assert!(!test.gives_output_to_others_input(&test_other));
        assert!(!test.gives_output_to_others_capital(&test_other));
        test.process_parts.clear();
        test_other.process_parts.clear();
            // want check
        let input = ProcessPart{
            item: Item::Want(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Input,
        };
        let output = ProcessPart{
            item: Item::Want(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Output,
        };
        test.process_parts.push(input);
        test_other.process_parts.push(output);
        assert!(test.takes_input_from(&test_other));
        assert!(!test.takes_capital_from(&test_other));
        assert!(!test.gives_output_to_others_input(&test_other));
        assert!(!test.gives_output_to_others_capital(&test_other));
        test.process_parts.clear();
        test_other.process_parts.clear();
        // capital to input (never match)
        let input = ProcessPart{
            item: Item::Product(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Capital,
        };
        let output = ProcessPart{
            item: Item::Product(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Input,
        };
        test.process_parts.push(input);
        test_other.process_parts.push(output);
        assert!(!test.takes_input_from(&test_other));
        assert!(!test.takes_capital_from(&test_other));
        assert!(!test.gives_output_to_others_input(&test_other));
        assert!(!test.gives_output_to_others_capital(&test_other));
        test.process_parts.clear();
        test_other.process_parts.clear();
            // want check
        let input = ProcessPart{
            item: Item::Want(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Capital,
        };
        let output = ProcessPart{
            item: Item::Want(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Input,
        };
        test.process_parts.push(input);
        test_other.process_parts.push(output);
        assert!(!test.takes_input_from(&test_other));
        assert!(!test.takes_capital_from(&test_other));
        assert!(!test.gives_output_to_others_input(&test_other));
        assert!(!test.gives_output_to_others_capital(&test_other));
        test.process_parts.clear();
        test_other.process_parts.clear();
        // capital to capital (never match)
        let input = ProcessPart{
            item: Item::Product(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Capital,
        };
        let output = ProcessPart{
            item: Item::Product(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Capital,
        };
        test.process_parts.push(input);
        test_other.process_parts.push(output);
        assert!(!test.takes_input_from(&test_other));
        assert!(!test.takes_capital_from(&test_other));
        assert!(!test.gives_output_to_others_input(&test_other));
        assert!(!test.gives_output_to_others_capital(&test_other));
        test.process_parts.clear();
        test_other.process_parts.clear();
            // want check
        let input = ProcessPart{
            item: Item::Want(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Capital,
        };
        let output = ProcessPart{
            item: Item::Want(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Capital,
        };
        test.process_parts.push(input);
        test_other.process_parts.push(output);
        assert!(!test.takes_input_from(&test_other));
        assert!(!test.takes_capital_from(&test_other));
        assert!(!test.gives_output_to_others_input(&test_other));
        assert!(!test.gives_output_to_others_capital(&test_other));
        test.process_parts.clear();
        test_other.process_parts.clear();
        // capital to output (match only products)
        let input = ProcessPart{
            item: Item::Product(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Capital,
        };
        let output = ProcessPart{
            item: Item::Product(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Output,
        };
        test.process_parts.push(input);
        test_other.process_parts.push(output);
        assert!(!test.takes_input_from(&test_other));
        assert!(test.takes_capital_from(&test_other));
        assert!(!test.gives_output_to_others_input(&test_other));
        assert!(!test.gives_output_to_others_capital(&test_other));
        test.process_parts.clear();
        test_other.process_parts.clear();
            // want check
        let input = ProcessPart{
            item: Item::Want(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Capital,
        };
        let output = ProcessPart{
            item: Item::Want(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Output,
        };
        test.process_parts.push(input);
        test_other.process_parts.push(output);
        assert!(!test.takes_input_from(&test_other));
        assert!(!test.takes_capital_from(&test_other));
        assert!(!test.gives_output_to_others_input(&test_other));
        assert!(!test.gives_output_to_others_capital(&test_other));
        test.process_parts.clear();
        test_other.process_parts.clear();
        // output to input
        let input = ProcessPart{
            item: Item::Product(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Output,
        };
        let output = ProcessPart{
            item: Item::Product(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Input,
        };
        test.process_parts.push(input);
        test_other.process_parts.push(output);
        assert!(!test.takes_input_from(&test_other));
        assert!(!test.takes_capital_from(&test_other));
        assert!(test.gives_output_to_others_input(&test_other));
        assert!(!test.gives_output_to_others_capital(&test_other));
        test.process_parts.clear();
        test_other.process_parts.clear();
            // want check
        let input = ProcessPart{
            item: Item::Want(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Output,
        };
        let output = ProcessPart{
            item: Item::Want(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Input,
        };
        test.process_parts.push(input);
        test_other.process_parts.push(output);
        assert!(!test.takes_input_from(&test_other));
        assert!(!test.takes_capital_from(&test_other));
        assert!(test.gives_output_to_others_input(&test_other));
        assert!(!test.gives_output_to_others_capital(&test_other));
        test.process_parts.clear();
        test_other.process_parts.clear();
        // output to capital (don't match on wants)
        let input = ProcessPart{
            item: Item::Product(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Output,
        };
        let output = ProcessPart{
            item: Item::Product(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Capital,
        };
        test.process_parts.push(input);
        test_other.process_parts.push(output);
        assert!(!test.takes_input_from(&test_other));
        assert!(!test.takes_capital_from(&test_other));
        assert!(!test.gives_output_to_others_input(&test_other));
        assert!(test.gives_output_to_others_capital(&test_other));
        test.process_parts.clear();
        test_other.process_parts.clear();
            // want check
        let input = ProcessPart{
            item: Item::Want(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Output,
        };
        let output = ProcessPart{
            item: Item::Want(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Capital,
        };
        test.process_parts.push(input);
        test_other.process_parts.push(output);
        assert!(!test.takes_input_from(&test_other));
        assert!(!test.takes_capital_from(&test_other));
        assert!(!test.gives_output_to_others_input(&test_other));
        assert!(!test.gives_output_to_others_capital(&test_other));
        test.process_parts.clear();
        test_other.process_parts.clear();
        // output to output (never match)
        let input = ProcessPart{
            item: Item::Product(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Output,
        };
        let output = ProcessPart{
            item: Item::Product(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Output,
        };
        test.process_parts.push(input);
        test_other.process_parts.push(output);
        assert!(!test.takes_input_from(&test_other));
        assert!(!test.takes_capital_from(&test_other));
        assert!(!test.gives_output_to_others_input(&test_other));
        assert!(!test.gives_output_to_others_capital(&test_other));
        test.process_parts.clear();
        test_other.process_parts.clear();
            // want check
        let input = ProcessPart{
            item: Item::Want(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Output,
        };
        let output = ProcessPart{
            item: Item::Want(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Output,
        };
        test.process_parts.push(input);
        test_other.process_parts.push(output);
        assert!(!test.takes_input_from(&test_other));
        assert!(!test.takes_capital_from(&test_other));
        assert!(!test.gives_output_to_others_input(&test_other));
        assert!(!test.gives_output_to_others_capital(&test_other));
        test.process_parts.clear();
        test_other.process_parts.clear();
    }

    #[test]
    pub fn should_return_correctly_can_feed_self(){
        let mut data = DataManager::new();
        data.load_test_data().expect("Failed!");
        let mut test = Process {
            id: 0,
            name: String::from("Test"),
            variant_name: String::from("Variant"),
            description: String::new(),
            minimum_time: 0.0,
            process_parts: vec![],
            process_tags: vec![],
            technology_requirement: None,
            tertiary_tech: None,
        };

        // no match (empty set)
        assert!(!test.can_feed_self(&data));
        // match on input-output product
        test.process_parts.push(ProcessPart {
            item: Item::Product(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Input,
        });
        test.process_parts.push(ProcessPart {
            item: Item::Product(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Output,
        });

        assert!(test.can_feed_self(&data));

        // match on capital-output product
        test.process_parts.clear();
        test.process_parts.push(ProcessPart {
            item: Item::Product(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Capital,
        });
        test.process_parts.push(ProcessPart {
            item: Item::Product(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Output,
        });

        assert!(test.can_feed_self(&data));
        // match on input-output want
        test.process_parts.clear();
        test.process_parts.push(ProcessPart {
            item: Item::Want(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Input,
        });
        test.process_parts.push(ProcessPart {
            item: Item::Want(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Output,
        });

        assert!(test.can_feed_self(&data));
        // don't match on capital-output want
        test.process_parts.clear();
        test.process_parts.push(ProcessPart {
            item: Item::Want(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Capital,
        });
        test.process_parts.push(ProcessPart {
            item: Item::Want(0),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Output,
        });

        assert!(!test.can_feed_self(&data));
    }
    
    #[test]
    pub fn should_correctly_get_name() {
        let test = Process {
            id: 0,
            name: String::from("Test"),
            variant_name: String::from("Variant"),
            description: String::new(),
            minimum_time: 0.0,
            process_parts: vec![],
            process_tags: vec![],
            technology_requirement: None,
            tertiary_tech: None,
        };

        let expectation = format!("{}({})", test.name, test.variant_name);

        assert_eq!(test.get_name(), expectation);
    }
}
