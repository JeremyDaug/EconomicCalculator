mod product_tests {
    use political_economy_core::objects::data_objects::{product::{Product, ProductTag}, want::Want, process::{Process, ProcessPart, ProcessSectionTag, ProcessTag}, item::Item};

    pub mod add_to_class_should {
        use std::collections::{HashSet, HashMap};
        use super::*;


        #[test]
        pub fn panic_if_class_product_is_not_the_class_leader() {
            let mut prod1 = Product{
                id: 0,
                name: "0".to_string(),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
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
            };
            let mut prod2 = Product {
                id: 1,
                name: "1".to_string(),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
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
                product_class: Some(2),
            };

            let result 
            = std::panic::catch_unwind(move || prod1.add_to_class(&mut prod2));
            assert!(result.is_err());
        }

        #[test]
        pub fn connect_products_when_neither_in_class() {
            let mut prod1 = Product{
                id: 0,
                name: "0".to_string(),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
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
            };
            let mut prod2 = Product {
                id: 1,
                name: "1".to_string(),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
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
            };
            prod1.add_to_class(&mut prod2);
            assert_eq!(prod1.product_class.unwrap(), prod2.id);
            assert_eq!(prod2.product_class.unwrap(), prod2.id);
        }

        #[test]
        pub fn connect_products_when_class_product_is_class_leader() {
            let mut prod1 = Product{
                id: 0,
                name: "0".to_string(),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
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
            };
            let mut prod2 = Product {
                id: 1,
                name: "1".to_string(),
                variant_name: String::new(),
                description: String::new(),
                unit_name: String::new(),
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
                product_class: Some(1),
            };
            prod1.add_to_class(&mut prod2);
            assert_eq!(prod1.product_class.unwrap(), prod2.id);
            assert_eq!(prod2.product_class.unwrap(), prod2.id);
        }
    }

    #[test]
    pub fn product_should_add_process_correctly() {
        let mut test = Product::new(
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
            None).unwrap();

        let mut test_process = Process{
            id: 0,
            name: String::from("Test"),
            variant_name: String::new(),
            description: String::from(""),
            minimum_time: 1.0,
            process_parts: vec![],
            process_tags: Vec::new(),
            technology_requirement: None,
            tertiary_tech: None,
        };

        // err on no relation found
        assert!(test.add_process(&test_process).is_err());

        // ignore the untagged
        let part = ProcessPart { 
            item: Item::Product(0), 
            amount: 1.0, 
            part_tags: vec![], 
            part: ProcessSectionTag::Input
        };
        test_process.process_parts.push(part);
        assert!(test.add_process(&test_process).is_ok());
        assert!(test.processes.contains(&0));
        assert!(test.failure_process.is_none());
        assert!(test.consumption_processes.len() == 0);
        assert!(test.use_processes.len() == 0);
        assert!(test.maintenance_processes.len() == 0);
        // cleanup
        test.processes.clear();
        test.failure_process = None;

        // check failure connects
        test_process.process_tags.push(ProcessTag::Failure(0));
        assert!(test.add_process(&test_process).is_ok());
        assert!(test.processes.contains(&0));
        assert!(test.failure_process.is_some());
        assert!(test.consumption_processes.len() == 0);
        assert!(test.use_processes.len() == 0);
        assert!(test.maintenance_processes.len() == 0);
        // check double dipping failure errors
        assert!(test.add_process(&test_process).is_err());
        assert!(test.processes.contains(&0));
        assert!(test.failure_process.is_some());
        assert!(test.consumption_processes.len() == 0);
        assert!(test.use_processes.len() == 0);
        assert!(test.maintenance_processes.len() == 0);
        // cleanup
        test.processes.clear();
        test.failure_process = None;
        test_process.process_tags.clear();
        // check maintenance
        test_process.process_tags.push(ProcessTag::Maintenance(0));
        assert!(test.add_process(&test_process).is_ok());
        assert!(test.processes.contains(&0));
        assert!(test.failure_process.is_none());
        assert!(test.consumption_processes.len() == 0);
        assert!(test.use_processes.len() == 0);
        assert!(test.maintenance_processes.len() == 1);
        // cleanup
        test.processes.clear();
        test.failure_process = None;
        test_process.process_tags.clear();
        test.maintenance_processes.clear();
        // check use
        test_process.process_tags.push(ProcessTag::Use(0));
        assert!(test.add_process(&test_process).is_ok());
        assert!(test.processes.contains(&0));
        assert!(test.failure_process.is_none());
        assert!(test.consumption_processes.len() == 0);
        assert!(test.use_processes.len() == 1);
        assert!(test.maintenance_processes.len() == 0);
        // cleanup
        test.processes.clear();
        test.failure_process = None;
        test_process.process_tags.clear();
        test.use_processes.clear();
        // check consumption
        test_process.process_tags.push(ProcessTag::Consumption(0));
        assert!(test.add_process(&test_process).is_ok());
        assert!(test.processes.contains(&0));
        assert!(test.failure_process.is_none());
        assert!(test.consumption_processes.len() == 1);
        assert!(test.use_processes.len() == 0);
        assert!(test.maintenance_processes.len() == 0);
    }

    #[test]
    pub fn product_correctly_adds_tags() {
        let mut test = Product::new(
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
            None).unwrap();

        let tag = ProductTag::Atomic { protons: 16, neutrons: 16 };

        test.add_tag(tag);

        assert!(test.tags.iter().any(|x| match x {
            ProductTag::Atomic { protons: _, neutrons: _ } => true,
            _ => false
        }));
        assert_eq!(test.tags.len(), 1);

        let tag2 = ProductTag::Abstract;
        test.add_tag(tag2);

        assert_eq!(test.tags.len(), 2);
    }

    #[test]
    pub fn product_should_add_want_correctly() {
        let mut test = Product::new(
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
            None).unwrap();
        let test_want = Want::new(0, 
            String::from("Test"),
            String::from("Desc"),
            0.5).expect("Nothing");

        let result = test.set_want(&test_want, 1.0);

        assert!(result.is_ok());
        assert!(test.wants.contains_key(&test_want.id));
        assert!(!test_want.ownership_sources.contains(&test.id));

        let mut test = Product::new(
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
            None).unwrap();
        let mut test_want = Want::new(0, 
            String::from("Test"),
            String::from("Desc"),
            0.5).expect("Nothing");

        let result = test.connect_want(&mut test_want, 1.0);

        assert!(result.is_ok());
        assert!(test.wants.contains_key(&test_want.id));
        assert!(test_want.ownership_sources.contains(&test.id));

        let mut test = Product::new(
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
            None).unwrap();
        let mut test_want = Want::new(0, 
            String::from("Test"),
            String::from("Desc"),
            0.5).expect("Nothing");

        let result = test.connect_want(&mut test_want, -0.1);
        
        assert!(result.is_err());
        let result = test.set_want(&test_want, -0.1);
        
        assert!(result.is_err());
    }

    #[test]
    pub fn product_returns_name_correctly() {
        let mut test = Product::new(
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
            None).unwrap();

        assert_eq!(test.get_name(), String::from("Test"));

        test.variant_name = String::from("Variant");

        assert_eq!(test.get_name(), String::from("Test(Variant)"));
    }

    #[test]
    pub fn product_correctly_defines_equal() {
        let test1 = Product::new(
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
            None).unwrap();
        let test2 = Product::new(
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
            None).unwrap();
        assert!(test1.is_equal_to(&test2));
    }

    #[test]
    pub fn new_product_returns_correctly() {
        // no problems
        let test = Product::new(
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
            None);

        assert!(test.is_some());
        // negative mass fine (for now)
        let test = Product::new(
            0,
            String::from("Test"),
            String::from(""),
            String::from("Desc"),
            String::from("Unit"),
            0,
            -1.0,
            0.0,
            Some(3),
            true,
            Vec::new(),
            None,
            None);

        assert!(test.is_some());
        // negative volume fine (for now)
        let test = Product::new(
            0,
            String::from("Test"),
            String::from(""),
            String::from("Desc"),
            String::from("Unit"),
            0,
            0.0,
            -1.0,
            Some(3),
            true,
            Vec::new(),
            None,
            None);

        assert!(test.is_some());
        // negative mass + magic good
        let tag = ProductTag::Magic;
        let mut magic_tag = Vec::new();
        magic_tag.push(tag);
        let test = Product::new(
            0,
            String::from("Test"),
            String::from(""),
            String::from("Desc"),
            String::from("Unit"),
            0,
            -1.0,
            0.0,
            Some(3),
            true,
            magic_tag,
            None,
            None);

        assert!(test.is_some());
        // negative volume + magic good
        let tag = ProductTag::Magic;
        let mut magic_tag = Vec::new();
        magic_tag.push(tag);
        let test = Product::new(
            0,
            String::from("Test"),
            String::from(""),
            String::from("Desc"),
            String::from("Unit"),
            0,
            0.0,
            -1.0,
            Some(3),
            true,
            magic_tag,
            None,
            None);

        assert!(test.is_some());
    }
}
