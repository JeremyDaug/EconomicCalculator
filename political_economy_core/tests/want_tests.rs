
mod want_tests {

    use political_economy_core::objects::data_objects::{want::Want, product::Product, process::{Process, ProcessPart, ProcessSectionTag, ProcessTag}, item::Item};

    #[test]
    pub fn add_process_source_returns_err_on() {
        let mut test = Want::new(0, 
            String::from("Test"),
            String::from("Desc"),
            0.5).expect("Nothing");

        let test_process = Process{
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

        let result = test.add_process_source(&test_process);
        assert!(result.is_err());
    }

    #[test]
    pub fn add_process_which_is_not_tagged() {
        let mut test = Want::new(0, 
            String::from("Test"),
            String::from("Desc"),
            0.5).expect("Nothing");

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

        let want_output = ProcessPart{
            item: Item::Want(test.id),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Output,
        };
        test_process.process_parts.push(want_output);
        let result = test.add_process_source(&test_process);
        assert!(result.is_ok());
        assert_eq!(test.process_sources.len(), 1);
        assert!(test.process_sources.iter().any(|x| x == &0));
    }

    #[test]
    pub fn add_process_should_add_to_use_when_use_process() {
        let mut test = Want::new(0, 
            String::from("Test"),
            String::from("Desc"),
            0.5).expect("Nothing");

        let test_product = Product::new(
            0, String::from("Test"),
            String::from(""), 
            String::from("des"),
            String::from("unit(s)"),
            0, 
            0.0,
            0.0,
            None,
            true,
            Vec::new(),
            None,
            None
        ).expect("Error, should've returned a product!");
        let tag = ProcessTag::Use(test_product.id);
        let mut test_process = Process{
            id: 0,
            name: String::from("Test"),
            variant_name: String::new(),
            description: String::from(""),
            minimum_time: 1.0,
            process_parts: vec![],
            process_tags: vec![tag],
            technology_requirement: None,
            tertiary_tech: None,
        };
        let product_input = ProcessPart{
            item: Item::Product(test_product.id),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Output,
        };
        let want_output = ProcessPart{
            item: Item::Want(test.id),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Output,
        };
        test_process.process_parts.push(product_input);
        test_process.process_parts.push(want_output);
        let result = test.add_process_source(&test_process);
        assert!(result.is_ok());
        assert_eq!(test.process_sources.len(), 1);
        assert_eq!(test.use_sources.len(), 1);
        assert_eq!(test.consumption_sources.len(), 0);
        assert!(test.process_sources.iter().any(|x| x == &0));
        assert!(test.use_sources.iter().any(|x| x == &0));
    }

    #[test]
    pub fn add_process_should_add_to_consumption_when_consumption_process() {
        let mut test = Want::new(0, 
            String::from("Test"),
            String::from("Desc"),
            0.5).expect("Nothing");

        let test_product = Product::new(
            0, String::from("Test"),
            String::from(""), 
            String::from("des"),
            String::from("unit(s)"),
            0, 
            0.0,
            0.0,
            None,
            true,
            Vec::new(),
            None,
            None
        ).expect("Error, should've returned a product!");
        let tag = ProcessTag::Consumption(test_product.id);
        let mut test_process = Process{
            id: 0,
            name: String::from("Test"),
            variant_name: String::new(),
            description: String::from(""),
            minimum_time: 1.0,
            process_parts: vec![],
            process_tags: vec![tag],
            technology_requirement: None,
            tertiary_tech: None,
        };
        let product_input = ProcessPart{
            item: Item::Product(test_product.id),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Output,
        };
        let want_output = ProcessPart{
            item: Item::Want(test.id),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Output,
        };
        test_process.process_parts.push(product_input);
        test_process.process_parts.push(want_output);
        let result = test.add_process_source(&test_process);
        assert!(result.is_ok());
        assert_eq!(test.process_sources.len(), 1);
        assert_eq!(test.use_sources.len(), 0);
        assert_eq!(test.consumption_sources.len(), 1);
        assert!(test.process_sources.iter().any(|x| x == &0));
        assert!(test.consumption_sources.iter().any(|x| x == &0));
    }

    #[test]
    fn set_decay_does_not_accept_invalid_values(){
        let mut test = Want::new(0, 
            String::from("Test"),
            String::from("Desc"),
            0.5).expect("Nothing");

        let result = test.set_decay(-0.1);
        assert!(!result);

        let result = test.set_decay(1.1);
        assert!(!result);

        let result = test.set_decay(0.1);
        assert!(result);
    }

    #[test]
    fn want_decays_correctly() {
        let test = Want::new(0, 
            String::from("Test"),
            String::from("Desc"),
            0.5).expect("Nothing");

        let decay_result = Want::decay_wants(1000.0, &test);

        assert_eq!(decay_result, 500.0);
    }

    #[test]
    fn new_want_returns_err_when_decay_out_of_range() {
        let test = Want::new(0, 
            String::from("Test"),
            String::from("Desc"),
            1.1);

        assert!(test.is_err());

        let test = Want::new(0, 
            String::from("Test"),
            String::from("Desc"),
            -0.1);

        assert!(test.is_err());

        let test = Want::new(0, 
            String::from("Test"),
            String::from("Desc"),
            0.1);

        assert!(test.is_ok());
    }

    #[test]
    fn adding_want_ownership_product_does_not_duplicate() {
        let mut test_want = Want::new(
            0,
            String::from(""),
            String::from("Desc"),
            1.0
        ).expect("Preemptive error, something went wrong bro.");

        let test_product = Product::new(
            0, String::from("Test"),
            String::from(""), 
            String::from("des"),
            String::from("unit(s)"),
            0, 
            0.0,
            0.0,
            None,
            true,
            Vec::new(),
            None,
            None
        ).expect("Error, should've returned a product!");

        assert_eq!(test_want.ownership_sources.len(), 0);

        test_want.ownership_sources.insert(test_product.id);

        assert_eq!(test_want.ownership_sources.len(), 1);

        test_want.add_ownership_source(&test_product);

        assert_eq!(test_want.ownership_sources.len(), 1);
    }
}
