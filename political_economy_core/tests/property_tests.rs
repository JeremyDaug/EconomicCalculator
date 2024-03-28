use political_economy_core::objects::{
    actor_objects::{
        pop::Pop,
        property::Property,
        property::TieredValue,
        property::DesireCoord,
        desire::Desire,
        property_info::PropertyInfo,
    },
    data_objects::{
        item::Item,
        product::Product,
        want::Want,
        want_info::WantInfo,
        process::*,
    },
    demographic_objects::{
        pop_breakdown_table::PopBreakdownTable,
        pop_breakdown_table::PBRow,
        species::Species,
        culture::Culture,
        ideology::Ideology,
    },
    environmental_objects::market::{
        MarketHistory, 
        ProductInfo
    },
};
use political_economy_core::{
    demographics::Demographics, 
    data_manager::DataManager
};

mod property_tests {
    use std::collections::{HashSet, HashMap, VecDeque};
    use super::*;

    /// Makes a pop for testing. The pop will have the following info
    /// 
    /// 20 pops total --
    /// 20 pops of the same speciecs
    /// - Desires
    ///   - 20 Food 0/1/2/3/4
    ///   - 20 Shelter 7/9/11/13
    ///   - 20 Clothing 2/4/6/8
    /// 10 with a culture
    /// - Desires
    ///   - 10 Ambrosia Fruit 10/15/20/25/30
    ///   - 10 Cotton Clothes 15/25/35 ...
    /// 10 with an ideology
    /// - Desires
    ///   - 10 Hut 30
    ///   - 10 Cabin 50
    pub fn make_test_pop() -> Pop {
        let mut test = Pop{ 
            id: 10, 
            job: 0, 
            firm: 0, 
            market: 0, 
            property: Property::new(vec![]), 
            breakdown_table: PopBreakdownTable{ table: vec![], total: 0 }, 
            is_selling: true,
            current_sat: TieredValue { tier: 0, value: 0.0 },
            prev_sat: TieredValue { tier: 0, value: 0.0 },
            hypo_change: TieredValue { tier: 0, value: 0.0 },
            backlog: VecDeque::new()};

        let species_desire_1 = Desire{ 
            item: Item::Want(2), // food
            start: 0, 
            end: Some(4), 
            amount: 1.0, 
            satisfaction: 0.0, 
            step: 1, 
            tags: vec![] };
        let species_desire_2 = Desire{ 
            item: Item::Want(3), // shelter
            start: 7, 
            end: Some(13), 
            amount: 1.0, 
            satisfaction: 0.0, 
            step: 2, 
            tags: vec![] };
        let species_desire_3 = Desire{ 
            item: Item::Want(4), //clothing
            start: 2, 
            end: Some(8), 
            amount: 1.0, 
            satisfaction: 0.0, 
            step: 2, 
            tags: vec![] };

        let culture_desire_1 = Desire{ 
            item: Item::Product(2), // ambrosia fruit
            start: 10, 
            end: Some(30), 
            amount: 1.0, 
            satisfaction: 0.0, 
            step: 5, 
            tags: vec![] };
        let culture_desire_2 = Desire{ 
            item: Item::Product(6), // clothes
            start: 15, 
            end: None, 
            amount: 1.0, 
            satisfaction: 0.0, 
            step: 10, 
            tags: vec![] };

        let ideology_desire_1 = Desire{ 
            item: Item::Product(14), // Hut
            start: 30, 
            end: None, 
            amount: 1.0, 
            satisfaction: 0.0, 
            step: 0, 
            tags: vec![] };
        let ideology_desire_2 = Desire{ 
            item: Item::Product(15), // Cabin
            start: 50, 
            end: None, 
            amount: 1.0, 
            satisfaction: 0.0, 
            step: 0, 
            tags: vec![] };

        let species = Species::new(0,
            "Species".into(),
            "".into(),
            vec![species_desire_1, species_desire_2, species_desire_3],
            vec![], vec![], 
            1.0, 0.03,
            0.02).expect("Messed up new.");

        let culture = Culture::new(0,
            "Culture".into(),
            "".into(),
            1.0, 0.01,
            0.01,
            vec![culture_desire_1, culture_desire_2],
            vec![]).expect("Messed up new.");

        let ideology = Ideology::new(0,
            "Ideology".into(),
            "".into(),
            0.0, 0.0,
            1.0,
            vec![ideology_desire_1, ideology_desire_2],
            vec![]).expect("Messed up new.");

        let mut demos = Demographics{ species: HashMap::new(),
             cultures: HashMap::new(), 
            ideology: HashMap::new() };

        demos.species.insert(species.id, species);
        demos.cultures.insert(culture.id, culture);
        demos.ideology.insert(ideology.id, ideology);

        test.breakdown_table.insert_pops(
            PBRow{ species: 0, 
                species_cohort: None,
                species_subtype: None,
                culture: None,
                culture_generation: None,
                culture_class: None,
                ideology: None, 
                ideology_wave: None, 
                ideology_faction: None, 
                count: 5 }
        );
        test.breakdown_table.insert_pops(
            PBRow{ species: 0, 
                species_cohort: None,
                species_subtype: None,
                culture: Some(0),
                culture_generation: None,
                culture_class: None,
                ideology: None, 
                ideology_wave: None, 
                ideology_faction: None, 
                count: 5 }
        );
        test.breakdown_table.insert_pops(
            PBRow{ species: 0, 
                species_cohort: None,
                species_subtype: None,
                culture: None,
                culture_generation: None,
                culture_class: None,
                ideology: Some(0), 
                ideology_wave: None, 
                ideology_faction: None, 
                count: 5 }
        );
        test.breakdown_table.insert_pops(
            PBRow{ species: 0, 
                species_cohort: None,
                species_subtype: None,
                culture: Some(0),
                culture_generation: None,
                culture_class: None,
                ideology: Some(0), 
                ideology_wave: None, 
                ideology_faction: None, 
                count: 5 }
        );

        test.update_desires(demos);

        test
    }

    /// preps a pop's property, the property's data, and market prices of those items.
    /// 
    /// Sets all values to 1.0 amv and salability of 0.5 by default.
    /// 
    /// Exceptions are:
    /// - Ambrosia Fruit are set as a currency (Sal 1.0, currency=true)
    /// - Cotton Clothes are priced at 10.0 amv.
    /// - Cotton Suit is priced at 20.0 amv.
    /// - Hut has a price of 100.0 amv.
    /// - Cabin has a price of 1000.0 amv.
    /// 
    /// This is for testing buy and sell functions, not offer_calculations.
    pub fn prepare_data_for_market_actions(_pop: &mut Pop) -> (DataManager, MarketHistory) {
        let mut data = DataManager::new();
        // TODO update this when we update Load All
        data.load_test_data().expect("Error on load?");
        let product = data.products.get_mut(&6).unwrap();
        product.fractional = true;

        let mut market = MarketHistory {
            product_info: HashMap::new(),
            sale_priority: vec![],
            currencies: vec![],
            class_info: HashMap::new(),
            want_info: HashMap::new(),
        };
        // quickly set all prices to 1.0 for ease going forward.
        for idx in 0..26 {
            market.product_info.insert(idx, ProductInfo {
                available: 0.0,
                price: 1.0,
                offered: 0.0,
                sold: 0.0,
                salability: 0.5,
                is_currency: false,
            });
        }
        // ambrosia fruit
        market.product_info.get_mut(&2).expect("Brok").salability = 1.0;
        market.product_info.get_mut(&2).expect("Brok").is_currency = true;

        market.product_info.get_mut(&6).expect("Brok").price = 10.0;
        market.product_info.get_mut(&7).expect("Brok").price = 20.0;

        market.product_info.get_mut(&14).expect("Brok").price = 100.0;
        market.product_info.get_mut(&15).expect("Brok").price = 1000.0;

        market.currencies.push(2);
        // sale priority would go here if used.

        // pop.property.property.insert(6, PropertyInfo::new(10.0));
        // TODO fix this info.

        (data, market)
    }

    pub fn _setup_data_manager() -> DataManager {
        let result = DataManager::new();

        // 4 wants, 
        // food, consumption focused (consuming bread)
        // rest, use focused (using widgets, consume time)
        // shinies, ownership (Having Gadgets)
        // mysticism, consumption, use, and ownership focused 
        //      (using bread, consuming gadgets, having widgets)
        let _want0 = Want{ 
            id: 0, 
            name: "Food".to_string(), 
            description: "".to_string(), 
            decay: 0.0, 
            ownership_sources: HashSet::new(), 
            process_sources: HashSet::new(), 
            use_sources: HashSet::new(), 
            consumption_sources: HashSet::new() };
        let _want1 = Want{ 
            id: 1, 
            name: "Rest".to_string(), 
            description: "".to_string(), 
            decay: 0.0, 
            ownership_sources: HashSet::new(), 
            process_sources: HashSet::new(), 
            use_sources: HashSet::new(), 
            consumption_sources: HashSet::new() };
        let want2 = Want{ 
            id: 2, 
            name: "Shinies".to_string(), 
            description: "".to_string(), 
            decay: 0.0, 
            ownership_sources: HashSet::new(), 
            process_sources: HashSet::new(), 
            use_sources: HashSet::new(), 
            consumption_sources: HashSet::new() };
        let _want3 = Want{ 
            id: 3, 
            name: "Mysticism".to_string(), 
            description: "".to_string(), 
            decay: 0.0, 
            ownership_sources: HashSet::new(), 
            process_sources: HashSet::new(), 
            use_sources: HashSet::new(), 
            consumption_sources: HashSet::new() };
        // 5 products for general testing
        // time for consumption/use and to satisfy rest
        // bread to feed oneself
        // widgets used to make rest more restful
        // gadgets used to show your shinies
        // incense to make things mystical
        let mut _time = Product{ id: 0,
            name: "Time".to_string(),
            variant_name: "".to_string(),
            description: "".to_string(),
            unit_name: "Hour(s)".to_string(),
            quality: 1,
            mass: 0.0,
            bulk: 0.0,
            mean_time_to_failure: Some(0),
            fractional: true,
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
        let mut _bread = Product{ id: 1,
            name: "Bread".to_string(),
            variant_name: "".to_string(),
            description: "".to_string(),
            unit_name: "Loaf(s)".to_string(),
            quality: 1,
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
        let mut _bun = Product{ id: 2,
            name: "Bun".to_string(),
            variant_name: "".to_string(),
            description: "".to_string(),
            unit_name: "Loaf(s)".to_string(),
            quality: 1,
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
        let mut _widget = Product{ id: 3,
            name: "Widget".to_string(),
            variant_name: "".to_string(),
            description: "".to_string(),
            unit_name: "Unit(s)".to_string(),
            quality: 1,
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
        let mut gadget = Product{ id: 4,
            name: "Gadget".to_string(),
            variant_name: "".to_string(),
            description: "".to_string(),
            unit_name: "Unit(s)".to_string(),
            quality: 1,
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
        gadget.wants.insert(want2.id, 1.0);
        let mut _incense = Product{ id: 5,
            name: "Incense".to_string(),
            variant_name: "".to_string(),
            description: "".to_string(),
            unit_name: "Tuft(s)".to_string(),
            quality: 1,
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

        // make processes for each
        // rest 1 hour -> 1 rest want
        let _rest = Process{
            id: 0,
            name: "resting".to_string(),
            variant_name: "".to_string(),
            description: "".to_string(),
            minimum_time: 0.0,
            process_parts: vec![
                ProcessPart{ 
                    item: Item::Product(0), 
                    amount: 1.0, 
                    part_tags: vec![],
                    part: ProcessSectionTag::Input
                },
                ProcessPart{ 
                    item: Item::Want(1), 
                    amount: 1.0, 
                    part_tags: vec![],
                    part: ProcessSectionTag::Output
                }
            ],
            process_tags: vec![],
            technology_requirement: None,
            tertiary_tech: None,
        };
        // eating 1 bread -> 1 food want
        let _eating = Process{ id: 1,
            name: "eating".to_string(),
            variant_name: "".to_string(),
            description: "".to_string(),
            minimum_time: 0.0,
            process_parts: vec![
                ProcessPart{ // bread
                    item: Item::Class(1), 
                    amount: 1.0, 
                    part_tags: vec![],
                    part: ProcessSectionTag::Input
                },
                ProcessPart{ // time
                    item: Item::Product(0), 
                    amount: 0.1, 
                    part_tags: vec![],
                    part: ProcessSectionTag::Input
                },
                ProcessPart{ 
                    item: Item::Want(0), 
                    amount: 1.0, 
                    part_tags: vec![],
                    part: ProcessSectionTag::Output
                }
            ],
            process_tags: vec![],
            technology_requirement: None,
            tertiary_tech: None,
        };
        let _proc3 = Process{ id: 2,
            name: "resting".to_string(),
            variant_name: "".to_string(),
            description: "".to_string(),
            minimum_time: 0.0,
            process_parts: vec![
                ProcessPart{ 
                    item: Item::Product(0), 
                    amount: 1.0, 
                    part_tags: vec![],
                    part: ProcessSectionTag::Input
                },
                ProcessPart{ 
                    item: Item::Want(1), 
                    amount: 1.0, 
                    part_tags: vec![],
                    part: ProcessSectionTag::Output
                }
            ],
            process_tags: vec![],
            technology_requirement: None,
            tertiary_tech: None,
        };

        result
    }

    mod record_exchange_should {
        use std::collections::HashMap;
        use super::*;

        #[test]
        pub fn panic_when_any_product_not_in_property() {
            let mut test = Property::new(vec![]);

            let mut exchange = HashMap::new();
            exchange.insert(0, 1.0);

            let mut test_clone = test.clone();
            let exch1 = exchange.clone();

            let result 
                = std::panic::catch_unwind(move || test_clone.record_exchange(exch1));
            assert!(result.is_err());

            test.unsafe_add_property(0, 1.0);
            let result 
                = std::panic::catch_unwind(move || test.record_exchange(exchange));
            assert!(result.is_ok());
        }

        #[test]
        pub fn correctly_record_spend_and_recieved() {
            let mut test = Property::new(vec![]);
            test.unsafe_add_property(0, 10.0);
            test.unsafe_add_property(1, 1.0);

            let mut exchange = HashMap::new();
            exchange.insert(0, -1.0);
            exchange.insert(1, 5.0);

            test.record_exchange(exchange);

            assert_eq!(test.property[&0].spent, 1.0);
            assert_eq!(test.property[&1].recieved, 5.0);
        }
    }

    mod satisfaction_from_amv_should {
        use std::collections::HashMap;
        use super::*;

        // TODO when class and want price estimates are added, add tests for them also
        // TODO when improving the function to predict things more accurately add a unified test to ensure overlap is taken into account.
        #[test]
        pub fn predict_satisfaction_gained_from_products() {
            let mut test_desires = vec![];
            test_desires.push(Desire::new(Item::Product(0), 
                0, 
                None, 
                1.0, 
                0.0, 
                1, 
                vec![]).unwrap());
            test_desires.push(Desire::new(Item::Product(1), 
                0, 
                None, 
                1.0, 
                0.0, 
                1, 
                vec![]).unwrap());
            test_desires.push(Desire::new(Item::Product(2), 
                0, 
                None, 
                1.0, 
                0.0, 
                1, 
                vec![]).unwrap());
            test_desires.push(Desire::new(Item::Product(3), 
                0, 
                None, 
                1.0, 
                0.0, 
                1, 
                vec![]).unwrap());
            test_desires.push(Desire::new(Item::Product(4), 
                0, 
                None, 
                1.0, 
                0.0, 
                1, 
                vec![]).unwrap());
            let mut test = Property::new(test_desires);
            let mut info = HashMap::new();
            info.insert(0, ProductInfo::new(1.0));
            info.insert(1, ProductInfo::new(5.0));
            info.insert(2, ProductInfo::new(10.0));
            info.insert(3, ProductInfo::new(20.0));
            info.insert(4, ProductInfo::new(50.0));
            let market = MarketHistory { product_info: info, 
                sale_priority: vec![], 
                want_info: HashMap::new(),
                class_info: HashMap::new(),
                currencies: vec![] };
            let result = test.satisfaction_from_amv(1.0, &market); // first
            assert_eq!(result.tier, 0);
            assert!(0.999 < result.value);
            assert!(result.value < 1.001);
            let result = test.satisfaction_from_amv(6.0, &market); // second
            assert_eq!(result.tier, 0);
            assert!(1.999 < result.value);
            assert!(result.value < 2.001);
            let result = test.satisfaction_from_amv(11.0, &market); // second + half
            assert_eq!(result.tier, 0);
            assert!(2.499 < result.value);
            assert!(result.value < 2.501);
            let result = test.satisfaction_from_amv(16.0, &market); // third
            assert_eq!(result.tier, 0);
            assert!(2.999 < result.value);
            assert!(result.value < 3.001);
            let result = test.satisfaction_from_amv(36.0, &market); // fourth
            assert_eq!(result.tier, 0);
            assert!(3.999 < result.value);
            assert!(result.value < 4.001);
            let result = test.satisfaction_from_amv(86.0, &market); // 5th
            assert_eq!(result.tier, 0);
            assert!(4.999 < result.value);
            assert!(result.value < 5.001);
            let result = test.satisfaction_from_amv(87.0, &market); // 6th just for proof
            assert_eq!(result.tier, 0);
            assert!(5.899 < result.value);
            assert!(result.value < 5.901);
            // add some existing satisfaction and try again
            test.desires.get_mut(2).unwrap().satisfaction = 1.0;
            let result = test.satisfaction_from_amv(1.0, &market); // first
            assert_eq!(result.tier, 0);
            assert!(0.999 < result.value);
            assert!(result.value < 1.001);
            let result = test.satisfaction_from_amv(6.0, &market); // second
            assert_eq!(result.tier, 0);
            assert!(1.999 < result.value);
            assert!(result.value < 2.001);
            let result = test.satisfaction_from_amv(26.0, &market); // third Skipped, 4th
            assert_eq!(result.tier, 0);
            assert!(2.999 < result.value);
            assert!(result.value < 3.001);
            let result = test.satisfaction_from_amv(76.0, &market); // 5th
            assert_eq!(result.tier, 0);
            assert!(3.999 < result.value);
            assert!(result.value < 4.001);
            let result = test.satisfaction_from_amv(77.0, &market); // 6th for extra proof
            assert_eq!(result.tier, 0);
            assert!(4.899 < result.value);
            assert!(result.value < 4.901);
        }
    }

    mod decay_goods_should {
        use std::collections::{HashMap, HashSet};

        use super::*;

        #[test]
        pub fn not_decay_fresh_failure_products() {
            let test_desires = vec![];
            let mut test = Property::new(test_desires);
            // make some default data for tests
            let mut data = DataManager::new();
            // 4 products, 
            // 0 for never fails
            // 1 for fails 50% into nothing
            // 2 for fails 100% through process into P3 1:1
            // 3 fails 100% into nothing.
            data.products.insert(0, Product{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            data.products.insert(1, Product{
                id: 1,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1),
                fractional: true,
                tags: vec![],
                wants: HashMap::new(),
                processes: HashSet::new(),
                failure_process: None,
                use_processes: HashSet::new(),
                consumption_processes: HashSet::new(),
                maintenance_processes: HashSet::new(),
                tech_required: None,
                product_class: Some(1),
            });
            data.products.insert(2, Product{
                id: 2,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(0),
                fractional: true,
                tags: vec![],
                wants: HashMap::new(),
                processes: HashSet::new(),
                failure_process: Some(0),
                use_processes: HashSet::new(),
                consumption_processes: HashSet::new(),
                maintenance_processes: HashSet::new(),
                tech_required: None,
                product_class: None,
            });
            data.products.insert(3, Product{
                id: 3,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(0),
                fractional: true,
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
            // add processes
            // 1 P2 -> 1 P3
            data.processes.insert(0, Process{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart{ item: Item::Product(2), 
                        amount: 1.0, part_tags: vec![], 
                        part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Product(3), 
                        amount: 1.0, part_tags: vec![], 
                        part: ProcessSectionTag::Output }
                ],
                process_tags: vec![
                    ProcessTag::Failure(1)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });
            test.property.insert(0, PropertyInfo::new(10.0));
            test.property.insert(1, PropertyInfo::new(10.0));
            test.property.insert(2, PropertyInfo::new(10.0));
            test.property.insert(3, PropertyInfo::new(10.0));
            for (_id, propinfo) in test.property.iter_mut() {
                propinfo.shift_to_used(10.0);
            }
            test.decay_goods(&data);
            // test
            assert_eq!(test.property[&0].total_property, 10.0);
            assert_eq!(test.property[&1].total_property, 5.0);
            assert_eq!(test.property[&2].total_property, 0.0);
            assert_eq!(test.property[&3].total_property, 10.0);
            assert_eq!(test.property[&0].lost, 0.0);
            assert_eq!(test.property[&1].lost, 5.0);
            assert_eq!(test.property[&2].lost, 10.0);
            assert_eq!(test.property[&3].lost, 10.0);
        }

        #[test]
        pub fn decay_capital_goods() {
            let test_desires = vec![];
            let mut test = Property::new(test_desires);
            // make some default data for tests
            let mut data = DataManager::new();
            // 4 products, 
            // 0 for never fails
            // 1 for fails 50% into nothing
            // 2 for fails 100% through process into P3 and W0
            // 3 for what 2 fails into
            data.products.insert(0, Product{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            data.products.insert(1, Product{
                id: 1,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1),
                fractional: true,
                tags: vec![],
                wants: HashMap::new(),
                processes: HashSet::new(),
                failure_process: None,
                use_processes: HashSet::new(),
                consumption_processes: HashSet::new(),
                maintenance_processes: HashSet::new(),
                tech_required: None,
                product_class: Some(1),
            });
            data.products.insert(2, Product{
                id: 2,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(0),
                fractional: true,
                tags: vec![],
                wants: HashMap::new(),
                processes: HashSet::new(),
                failure_process: Some(0),
                use_processes: HashSet::new(),
                consumption_processes: HashSet::new(),
                maintenance_processes: HashSet::new(),
                tech_required: None,
                product_class: None,
            });
            data.products.insert(3, Product{
                id: 3,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            // wants
            // 0 doesn't decay (is produced by P2)
            let want0 = Want{
                id: 0,
                name: "".to_string(),
                description: "".to_string(),
                decay: 0.0,
                ownership_sources: HashSet::new(),
                process_sources: HashSet::new(),
                use_sources: HashSet::new(),
                consumption_sources: HashSet::new(),
            };
            data.wants.insert(want0.id, want0);
            // add processes
            data.processes.insert(0, Process{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart{ item: Item::Product(2), 
                        amount: 1.0, part_tags: vec![], 
                        part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Want(0), 
                        amount: 1.0, part_tags: vec![], 
                        part: ProcessSectionTag::Output },
                    ProcessPart{ item: Item::Product(3), 
                        amount: 1.0, part_tags: vec![], 
                        part: ProcessSectionTag::Output }
                ],
                process_tags: vec![
                    ProcessTag::Failure(1)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });
            test.property.insert(0, PropertyInfo::new(10.0));
            test.property.insert(1, PropertyInfo::new(10.0));
            test.property.insert(2, PropertyInfo::new(10.0));
            test.property.insert(3, PropertyInfo::new(10.0));
            for (_id, propinfo) in test.property.iter_mut() {
                propinfo.shift_to_used(10.0);
            }
            test.decay_goods(&data);
            // test
            assert_eq!(test.property[&0].total_property, 10.0);
            assert_eq!(test.property[&1].total_property, 5.0);
            assert_eq!(test.property[&2].total_property, 0.0);
            assert_eq!(test.property[&3].total_property, 20.0);
            assert_eq!(test.property[&0].lost, 0.0);
            assert_eq!(test.property[&1].lost, 5.0);
            assert_eq!(test.property[&2].lost, 10.0);
            assert_eq!(test.property[&3].lost, 0.0);
            assert_eq!(test.property[&0].recieved, 0.0);
            assert_eq!(test.property[&1].recieved, 0.0);
            assert_eq!(test.property[&2].recieved, 0.0);
            assert_eq!(test.property[&3].recieved, 10.0);
            
            assert_eq!(test.want_store[&0].total_current, 10.0);
        }

        #[test]
        pub fn decay_goods_correctly_for_all_failure_types() {
            let test_desires = vec![];
            let mut test = Property::new(test_desires);
            // make some default data for tests
            let mut data = DataManager::new();
            // 4 products, 
            // 0 for never fails
            // 1 for fails 50% into nothing
            // 2 for fails 100% through process into P3 and W0
            // 3 for what 2 fails into
            data.products.insert(0, Product{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            data.products.insert(1, Product{
                id: 1,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(1),
                fractional: true,
                tags: vec![],
                wants: HashMap::new(),
                processes: HashSet::new(),
                failure_process: None,
                use_processes: HashSet::new(),
                consumption_processes: HashSet::new(),
                maintenance_processes: HashSet::new(),
                tech_required: None,
                product_class: Some(1),
            });
            data.products.insert(2, Product{
                id: 2,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: Some(0),
                fractional: true,
                tags: vec![],
                wants: HashMap::new(),
                processes: HashSet::new(),
                failure_process: Some(0),
                use_processes: HashSet::new(),
                consumption_processes: HashSet::new(),
                maintenance_processes: HashSet::new(),
                tech_required: None,
                product_class: None,
            });
            data.products.insert(3, Product{
                id: 3,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            // wants
            // 0 doesn't decay (is produced by P2)
            // 1 decays by 50%
            // 2 decays by 25%
            // 3 decays by 100%
            let want0 = Want{
                id: 0,
                name: "".to_string(),
                description: "".to_string(),
                decay: 0.0,
                ownership_sources: HashSet::new(),
                process_sources: HashSet::new(),
                use_sources: HashSet::new(),
                consumption_sources: HashSet::new(),
            };
            let want1 = Want{
                id: 1,
                name: "".to_string(),
                description: "".to_string(),
                decay: 0.5,
                ownership_sources: HashSet::new(),
                process_sources: HashSet::new(),
                use_sources: HashSet::new(),
                consumption_sources: HashSet::new(),
            };
            let want2 = Want{
                id: 2,
                name: "".to_string(),
                description: "".to_string(),
                decay: 0.25,
                ownership_sources: HashSet::new(),
                process_sources: HashSet::new(),
                use_sources: HashSet::new(),
                consumption_sources: HashSet::new(),
            };
            let want3 = Want{
                id: 3,
                name: "".to_string(),
                description: "".to_string(),
                decay: 1.0,
                ownership_sources: HashSet::new(),
                process_sources: HashSet::new(),
                use_sources: HashSet::new(),
                consumption_sources: HashSet::new(),
            };
            data.wants.insert(want0.id, want0);
            data.wants.insert(want1.id, want1);
            data.wants.insert(want2.id, want2);
            data.wants.insert(want3.id, want3);
            // add processes
            data.processes.insert(0, Process{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart{ item: Item::Product(2), 
                        amount: 1.0, part_tags: vec![], 
                        part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Want(0), 
                        amount: 1.0, part_tags: vec![], 
                        part: ProcessSectionTag::Output },
                    ProcessPart{ item: Item::Product(3), 
                        amount: 1.0, part_tags: vec![], 
                        part: ProcessSectionTag::Output }
                ],
                process_tags: vec![
                    ProcessTag::Failure(1)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });
            test.property.insert(0, PropertyInfo::new(10.0));
            test.property.insert(1, PropertyInfo::new(10.0));
            test.property.insert(2, PropertyInfo::new(10.0));
            test.property.insert(3, PropertyInfo::new(10.0));
            test.want_store.insert(0, WantInfo::new(10.0));
            test.want_store.insert(1, WantInfo::new(10.0));
            test.want_store.insert(2, WantInfo::new(10.0));
            test.want_store.insert(3, WantInfo::new(10.0));
            test.decay_goods(&data);
            // check that everything decayed correctly.
            assert_eq!(test.property[&0].total_property, 10.0);
            assert_eq!(test.property[&1].total_property, 5.0);
            assert_eq!(test.property[&2].total_property, 0.0);
            assert_eq!(test.property[&3].total_property, 20.0);
            assert_eq!(test.property[&0].lost, 0.0);
            assert_eq!(test.property[&1].lost, 5.0);
            assert_eq!(test.property[&2].lost, 10.0);
            assert_eq!(test.property[&3].lost, 0.0);
            assert_eq!(test.property[&0].recieved, 0.0);
            assert_eq!(test.property[&1].recieved, 0.0);
            assert_eq!(test.property[&2].recieved, 0.0);
            assert_eq!(test.property[&3].recieved, 10.0);
            
            assert_eq!(test.want_store[&0].total_current, 20.0);
            assert_eq!(test.want_store[&1].total_current, 5.0);
            assert_eq!(test.want_store[&2].total_current, 7.5);
            assert_eq!(test.want_store[&3].total_current, 0.0);
            assert_eq!(test.want_store[&0].lost, 0.0);
            assert_eq!(test.want_store[&1].lost, 5.0);
            assert_eq!(test.want_store[&2].lost, 2.5);
            assert_eq!(test.want_store[&3].lost, 10.0);
            assert_eq!(test.want_store[&0].gained, 10.0);
            assert_eq!(test.want_store[&1].gained, 0.0);
            assert_eq!(test.want_store[&2].gained, 0.0);
            assert_eq!(test.want_store[&3].gained, 0.0);
        }
    }

    mod first_desire_should {
        use super::*;
        
        #[test]
        pub fn find_the_lowest_and_first_desire() {
            let mut test_desires = vec![];
            test_desires.push(Desire{ // 0,2,...
                item: Item::Want(0), 
                start: 4, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,...
                item: Item::Want(1), 
                start: 7, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,...
                item: Item::Want(1), 
                start: 2, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,...
                item: Item::Want(1), 
                start: 1, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            let test = Property::new(test_desires);
            let result = test.first_desire();
            assert_eq!(result.idx, 3);
            assert_eq!(result.tier, 1);
        }
    }

    mod get_first_unsatisfied_desire_should {
        use super::*;

        #[test]
        pub fn find_lowest_when_one_has_no_sat_but_is_not_lowest() {
            let mut test_desires = vec![];
            test_desires.push(Desire{ // 0,2,...
                item: Item::Want(0), 
                start: 4, 
                end: None, 
                amount: 1.0, 
                satisfaction: 1.0,
                step: 0,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,...
                item: Item::Want(1), 
                start: 7, 
                end: Some(13), 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,...
                item: Item::Want(1), 
                start: 2, 
                end: None, 
                amount: 1.0, 
                satisfaction: 1.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,...
                item: Item::Want(1), 
                start: 1, 
                end: None, 
                amount: 1.0, 
                satisfaction: 10.0,
                step: 1,
                tags: vec![]});
            let mut test = Property::new(test_desires);
            let result = test.get_first_unsatisfied_desire().unwrap();
            assert_eq!(result.idx, 2);
            assert_eq!(result.tier, 3);
        }

        #[test]
        pub fn find_lowest_when_it_has_no_sat() {
            let mut test_desires = vec![];
            test_desires.push(Desire{ // 0,2,...
                item: Item::Want(0), 
                start: 4, 
                end: None, 
                amount: 1.0, 
                satisfaction: 1.0,
                step: 0,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,...
                item: Item::Want(1), 
                start: 7, 
                end: Some(13), 
                amount: 1.0, 
                satisfaction: 3.5,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,...
                item: Item::Want(1), 
                start: 2, 
                end: None, 
                amount: 1.0, 
                satisfaction: 10.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,...
                item: Item::Want(1), 
                start: 1, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            let mut test = Property::new(test_desires);
            let result = test.get_first_unsatisfied_desire().unwrap();
            assert_eq!(result.idx, 3);
            assert_eq!(result.tier, 1);
        }
        
        #[test]
        pub fn find_the_lowest_and_first_unsatisfied_desire() {
            let mut test_desires = vec![];
            test_desires.push(Desire{ // 0,2,...
                item: Item::Want(0), 
                start: 4, 
                end: None, 
                amount: 1.0, 
                satisfaction: 1.0,
                step: 0,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,...
                item: Item::Want(1), 
                start: 7, 
                end: Some(13), 
                amount: 1.0, 
                satisfaction: 3.5,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,...
                item: Item::Want(1), 
                start: 2, 
                end: None, 
                amount: 1.0, 
                satisfaction: 10.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,...
                item: Item::Want(1), 
                start: 1, 
                end: None, 
                amount: 1.0, 
                satisfaction: 21.0,
                step: 1,
                tags: vec![]});
            let mut test = Property::new(test_desires);
            let result = test.get_first_unsatisfied_desire().unwrap();
            assert_eq!(result.idx, 1);
            assert_eq!(result.tier, 10);
            test.desires[1].satisfaction += 4.0;
            let result = test.get_first_unsatisfied_desire().unwrap();
            assert_eq!(result.idx, 2);
            assert_eq!(result.tier, 12);
        }

        #[test]
        pub fn return_none_when_all_desires_are_fully_satisfied() {
            let mut test_desires = vec![];
            test_desires.push(Desire{ // 0,2,...
                item: Item::Want(0), 
                start: 4, 
                end: None, 
                amount: 1.0, 
                satisfaction: 1.0,
                step: 0,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,...
                item: Item::Want(1), 
                start: 7, 
                end: Some(13), 
                amount: 1.0, 
                satisfaction: 1.0,
                step: 0,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,...
                item: Item::Want(1), 
                start: 2, 
                end: None, 
                amount: 1.0, 
                satisfaction: 1.0,
                step: 0,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,...
                item: Item::Want(1), 
                start: 1, 
                end: None, 
                amount: 1.0, 
                satisfaction: 1.0,
                step: 0,
                tags: vec![]});
            let test = Property::new(test_desires);
            let result = test.get_first_unsatisfied_desire();
            assert!(result.is_none());
        }
    }

    mod add_products_should {
        use std::collections::{HashSet, HashMap};
        use super::*;

        #[test]
        pub fn add_multiple_products_to_property_then_sift() {
            let mut test_desires = vec![];
            test_desires.push(Desire{ // 0,2,...
                item: Item::Want(0), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,...
                item: Item::Want(1), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            let mut test = Property::new(test_desires);
            // make some default data for tests
            let mut data = DataManager::new();
            // 5 products, 
            // 0 for Product
            // 1 for class
            // 2 for ownership
            // 3 for use
            // 4 for consumption
            data.products.insert(0, Product{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            data.products.insert(1, Product{
                id: 1,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
                tags: vec![],
                wants: HashMap::new(),
                processes: HashSet::new(),
                failure_process: None,
                use_processes: HashSet::new(),
                consumption_processes: HashSet::new(),
                maintenance_processes: HashSet::new(),
                tech_required: None,
                product_class: Some(1),
            });
            data.products.insert(2, Product{
                id: 2,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            data.products.entry(2)
                .and_modify(|x| {
                    x.wants.insert(0, 1.0);
                    x.wants.insert(1, 2.0);
            });
            data.products.insert(3, Product{
                id: 3,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            data.products.get_mut(&3).unwrap()
            .use_processes.insert(0);
            data.products.insert(4, Product{
                id: 4,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            let mut want0 = Want{
                id: 0,
                name: "".to_string(),
                description: "".to_string(),
                decay: 0.0,
                ownership_sources: HashSet::new(),
                process_sources: HashSet::new(),
                use_sources: HashSet::new(),
                consumption_sources: HashSet::new(),
            };
            let mut want1 = Want{
                id: 1,
                name: "".to_string(),
                description: "".to_string(),
                decay: 0.0,
                ownership_sources: HashSet::new(),
                process_sources: HashSet::new(),
                use_sources: HashSet::new(),
                consumption_sources: HashSet::new(),
            };
            want0.ownership_sources.insert(2);
            want0.process_sources.insert(0);
            want0.process_sources.insert(1);
            want0.use_sources.insert(0);
            want0.consumption_sources.insert(1);
            data.wants.insert(want0.id, want0);
            want1.ownership_sources.insert(2);
            data.wants.insert(want1.id, want1);
            // add processes
            data.processes.insert(0, Process{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart{ item: Item::Product(3), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Capital },
                    ProcessPart{ item: Item::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                ],
                process_tags: vec![
                    ProcessTag::Use(3)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });
            data.processes.insert(1, Process{
                id: 1,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart{ item: Item::Product(4), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                ],
                process_tags: vec![
                    ProcessTag::Consumption(4)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });
            data.update_product_classes().expect("Failed to setup product classes.");
            test.is_sifted = true;
            // double check that everything is set up.
            assert!(test.is_sifted);
            assert!(test.desires[0].satisfaction == 0.0);
            assert!(test.desires[1].satisfaction == 0.0);
            let mut additions = HashMap::new();
            additions.insert(0, 1.0);
            additions.insert(1, 1.0);
            additions.insert(2, 1.0);
            additions.insert(3, 1.0);
            additions.insert(4, 1.0);
            test.add_products(&additions, &data);
            // should satisfy 1 of want 0, and 2 of want 1
            assert!(test.desires[0].satisfaction == 3.0);
            assert!(test.desires[1].satisfaction == 2.0);
            assert!(test.property.get(&0).unwrap().total_property == 1.0);
            assert!(test.property.get(&1).unwrap().total_property == 1.0);
            assert!(test.property.get(&2).unwrap().total_property == 1.0);
            assert!(test.property.get(&3).unwrap().total_property == 1.0);
            assert!(test.property.get(&4).unwrap().total_property == 1.0);
        }
    }

    mod add_property_should {
        use std::collections::{HashSet, HashMap};
        use super::*;

        use super::{make_test_pop, prepare_data_for_market_actions};

        #[test]
        pub fn correctly_sift_test_data() {
            let mut test = make_test_pop();
            let (data, _history) = prepare_data_for_market_actions(&mut test);
            // add in pop's property and sift their desires.
            // we have 20 extra food than we need (20*5=100.0 units)
            // this covers all food and leave excess for trading
            // This covers both species food desire and culture ambrosia fruit desire.
            test.property.add_property(2, 120.0, &data);
            // they have all the shelter they need via huts
            // 4 * 20 units
            // this covers both the shelter desire and the hut desire
            test.property.add_property(14, 80.0, &data);
            // the have all clothing desires covered, 
            // clothing culture desire is covered up to tier 85.
            // 20 * 5 units
            // they have 20.0 extra units available to trade.
            test.property.add_property(6, 100.0, &data);
            // missing desires are 10 cabins at tier 50, and the infinite
            // desire for 10 units of clothes every 10 tiers.
            // we want to target buying 10 cabins.

            // everything should automatically be sifted
            assert_eq!(test.property.property[&2].total_property, 120.0);
            assert_eq!(test.property.property[&2].unreserved, 20.0);
            assert_eq!(test.property.property[&2].product_reserve, 50.0);
            assert_eq!(test.property.property[&2].class_reserve, 0.0);
            assert_eq!(test.property.property[&2].want_reserve, 100.0);

            assert_eq!(test.property.property[&14].total_property, 80.0);
            assert_eq!(test.property.property[&14].unreserved, 0.0);
            assert_eq!(test.property.property[&14].product_reserve, 10.0);
            assert_eq!(test.property.property[&14].class_reserve, 0.0);
            assert_eq!(test.property.property[&14].want_reserve, 80.0);

            assert_eq!(test.property.property[&6].total_property, 100.0);
            assert_eq!(test.property.property[&6].unreserved, 0.0);
            assert_eq!(test.property.property[&6].product_reserve, 100.0);
            assert_eq!(test.property.property[&6].class_reserve, 0.0);
            assert_eq!(test.property.property[&6].want_reserve, 80.0);

            // process is planned correctly.
            assert_eq!(test.property.process_plan[&13], 100.0);
        }

        #[test]
        pub fn sift_products_down_from_higher_wants() {
            // Currently Not Implemented, Function is Placeholder
            assert!(true);
        }

        #[test]
        pub fn sift_from_expected_wants_correctly() {
            let mut test_desires = vec![];
            test_desires.push(Desire{ // 0,2,...
                item: Item::Want(0), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,...
                item: Item::Want(1), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            let mut test = Property::new(test_desires);
            // make some default data for tests
            let mut data = DataManager::new();
            // 5 products, 
            // 0 for Product
            // 1 for class
            // 2 for ownership
            // 3 for use
            // 4 for consumption
            data.products.insert(0, Product{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            data.products.insert(1, Product{
                id: 1,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
                tags: vec![],
                wants: HashMap::new(),
                processes: HashSet::new(),
                failure_process: None,
                use_processes: HashSet::new(),
                consumption_processes: HashSet::new(),
                maintenance_processes: HashSet::new(),
                tech_required: None,
                product_class: Some(1),
            });
            data.products.insert(2, Product{
                id: 2,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            data.products.entry(2)
                .and_modify(|x| {
                    x.wants.insert(0, 1.0);
                    x.wants.insert(1, 2.0);
            });
            data.products.insert(3, Product{
                id: 3,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            data.products.get_mut(&3).unwrap()
            .use_processes.insert(0);
            data.products.insert(4, Product{
                id: 4,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            let mut want0 = Want{
                id: 0,
                name: "".to_string(),
                description: "".to_string(),
                decay: 0.0,
                ownership_sources: HashSet::new(),
                process_sources: HashSet::new(),
                use_sources: HashSet::new(),
                consumption_sources: HashSet::new(),
            };
            let mut want1 = Want{
                id: 1,
                name: "".to_string(),
                description: "".to_string(),
                decay: 0.0,
                ownership_sources: HashSet::new(),
                process_sources: HashSet::new(),
                use_sources: HashSet::new(),
                consumption_sources: HashSet::new(),
            };
            want0.ownership_sources.insert(2);
            want0.process_sources.insert(0);
            want0.process_sources.insert(1);
            want0.use_sources.insert(0);
            want0.consumption_sources.insert(1);
            data.wants.insert(want0.id, want0);
            want1.ownership_sources.insert(2);
            data.wants.insert(want1.id, want1);
            // add processes
            data.processes.insert(0, Process{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart{ item: Item::Product(3), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Capital },
                    ProcessPart{ item: Item::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                ],
                process_tags: vec![
                    ProcessTag::Use(3)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });
            data.processes.insert(1, Process{
                id: 1,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart{ item: Item::Product(4), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                ],
                process_tags: vec![
                    ProcessTag::Consumption(4)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });
            data.update_product_classes().expect("Failed to setup product classes.");
            test.is_sifted = true;
            // double check that everything is set up.
            assert!(test.is_sifted);
            assert!(test.desires[0].satisfaction == 0.0);
            assert!(test.desires[1].satisfaction == 0.0);
            test.add_property(2, 1.0, &data);
            // should satisfy 1 of want 0, and 2 of want 1
            assert_eq!(test.desires[0].satisfaction, 1.0);
            assert_eq!(test.desires[1].satisfaction, 2.0);
            assert!(test.property.get(&2).unwrap().total_property == 1.0);
            assert!(test.property.get(&2).unwrap().want_reserve == 1.0);
        }

        #[test]
        pub fn add_and_sift_correctly_without_exchanges() {
            let mut test_desires = vec![];
            test_desires.push(Desire{ // 0,2, ...
                item: Item::Product(0), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,...
                item: Item::Class(1), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,...
                item: Item::Want(0), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            let mut test = Property::new(test_desires);
            // make some default data for tests
            let mut data = DataManager::new();
            // 5 products, 
            // 0 for Product
            // 1 for class
            // 2 for ownership
            // 3 for use
            // 4 for consumption
            data.products.insert(0, Product{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            data.products.insert(1, Product{
                id: 1,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
                tags: vec![],
                wants: HashMap::new(),
                processes: HashSet::new(),
                failure_process: None,
                use_processes: HashSet::new(),
                consumption_processes: HashSet::new(),
                maintenance_processes: HashSet::new(),
                tech_required: None,
                product_class: Some(1),
            });
            data.products.insert(2, Product{
                id: 2,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            data.products.entry(2)
                .and_modify(|x| {
                    x.wants.insert(0, 1.0);
            });
            data.products.insert(3, Product{
                id: 3,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            data.products.get_mut(&3).unwrap()
            .use_processes.insert(0);
            data.products.insert(4, Product{
                id: 4,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            let mut want0 = Want{
                id: 0,
                name: "".to_string(),
                description: "".to_string(),
                decay: 0.0,
                ownership_sources: HashSet::new(),
                process_sources: HashSet::new(),
                use_sources: HashSet::new(),
                consumption_sources: HashSet::new(),
            };
            want0.ownership_sources.insert(2);
            want0.process_sources.insert(0);
            want0.process_sources.insert(1);
            want0.use_sources.insert(0);
            want0.consumption_sources.insert(1);
            data.wants.insert(want0.id, want0);
            // add processes
            data.processes.insert(0, Process{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart{ item: Item::Product(3), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Capital },
                    ProcessPart{ item: Item::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                ],
                process_tags: vec![
                    ProcessTag::Use(3)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });
            data.processes.insert(1, Process{
                id: 1,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart{ item: Item::Product(4), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                ],
                process_tags: vec![
                    ProcessTag::Consumption(4)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });
            data.update_product_classes().expect("Failed to setup product classes.");
            test.is_sifted = true;
            // double check that everything is set up.
            assert!(test.is_sifted);
            assert!(test.desires[0].satisfaction == 0.0);
            assert!(test.desires[1].satisfaction == 0.0);
            assert!(test.desires[2].satisfaction == 0.0);
            // test satisfaction and reserves for each
            test.add_property(0, 5.0, &data);
            assert!(test.desires[0].satisfaction == 5.0);
            assert!(test.desires[1].satisfaction == 0.0);
            assert!(test.desires[2].satisfaction == 0.0);
            assert!(test.property.get(&0).unwrap().total_property == 5.0);
            assert!(test.property.get(&0).unwrap().product_reserve == 5.0);
            assert!(test.property.get(&1).is_none());
            assert!(test.property.get(&2).is_none());
            assert!(test.property.get(&3).is_none());
            assert!(test.property.get(&4).is_none());
            test.add_property(1, 5.0, &data);
            assert!(test.desires[0].satisfaction == 5.0);
            assert!(test.desires[1].satisfaction == 5.0);
            assert!(test.desires[2].satisfaction == 0.0);
            assert!(test.property.get(&0).unwrap().total_property == 5.0);
            assert!(test.property.get(&0).unwrap().product_reserve == 5.0);
            assert!(test.property.get(&1).unwrap().total_property == 5.0);
            assert!(test.property.get(&1).unwrap().class_reserve == 5.0);
            assert!(test.property.get(&2).is_none());
            assert!(test.property.get(&3).is_none());
            assert!(test.property.get(&4).is_none());
            test.add_property(2, 5.0, &data);
            assert!(test.desires[0].satisfaction == 5.0);
            assert!(test.desires[1].satisfaction == 5.0);
            assert!(test.desires[2].satisfaction == 5.0);
            assert!(test.property.get(&0).unwrap().total_property == 5.0);
            assert!(test.property.get(&0).unwrap().product_reserve == 5.0);
            assert!(test.property.get(&1).unwrap().total_property == 5.0);
            assert!(test.property.get(&1).unwrap().class_reserve == 5.0);
            assert!(test.property.get(&2).unwrap().total_property == 5.0);
            assert!(test.property.get(&2).unwrap().want_reserve == 5.0);
            assert!(test.property.get(&3).is_none());
            assert!(test.property.get(&4).is_none());
            test.add_property(3, 5.0, &data);
            assert!(test.desires[0].satisfaction == 5.0);
            assert!(test.desires[1].satisfaction == 5.0);
            assert!(test.desires[2].satisfaction == 10.0);
            assert!(test.property.get(&0).unwrap().total_property == 5.0);
            assert!(test.property.get(&0).unwrap().product_reserve == 5.0);
            assert!(test.property.get(&1).unwrap().total_property == 5.0);
            assert!(test.property.get(&1).unwrap().class_reserve == 5.0);
            assert!(test.property.get(&2).unwrap().total_property == 5.0);
            assert!(test.property.get(&2).unwrap().want_reserve == 5.0);
            assert!(test.property.get(&3).unwrap().total_property == 5.0);
            assert!(test.property.get(&3).unwrap().want_reserve == 5.0);
            assert!(test.property.get(&4).is_none());
            test.add_property(4, 5.0, &data);
            assert!(test.desires[0].satisfaction == 5.0);
            assert!(test.desires[1].satisfaction == 5.0);
            assert!(test.desires[2].satisfaction == 15.0);
            assert!(test.property.get(&0).unwrap().total_property == 5.0);
            assert!(test.property.get(&0).unwrap().product_reserve == 5.0);
            assert!(test.property.get(&1).unwrap().total_property == 5.0);
            assert!(test.property.get(&1).unwrap().class_reserve == 5.0);
            assert!(test.property.get(&2).unwrap().total_property == 5.0);
            assert!(test.property.get(&2).unwrap().want_reserve == 5.0);
            assert!(test.property.get(&3).unwrap().total_property == 5.0);
            assert!(test.property.get(&3).unwrap().want_reserve == 5.0);
            assert!(test.property.get(&4).unwrap().total_property == 5.0);
            assert!(test.property.get(&4).unwrap().want_reserve == 5.0);
            test.add_property(4, 5.0, &data);
            assert!(test.desires[0].satisfaction == 5.0);
            assert!(test.desires[1].satisfaction == 5.0);
            assert!(test.desires[2].satisfaction == 20.0);
            assert!(test.property.get(&0).unwrap().total_property == 5.0);
            assert!(test.property.get(&0).unwrap().product_reserve == 5.0);
            assert!(test.property.get(&1).unwrap().total_property == 5.0);
            assert!(test.property.get(&1).unwrap().class_reserve == 5.0);
            assert!(test.property.get(&2).unwrap().total_property == 5.0);
            assert!(test.property.get(&2).unwrap().want_reserve == 5.0);
            assert!(test.property.get(&3).unwrap().total_property == 5.0);
            assert!(test.property.get(&3).unwrap().want_reserve == 5.0);
            assert!(test.property.get(&4).unwrap().total_property == 10.0);
            assert!(test.property.get(&4).unwrap().want_reserve == 10.0);
        }

        #[test]
        pub fn add_and_sift_correctly_with_overlap_between_sections() {
            let mut test_desires = vec![];
            test_desires.push(Desire{ // 0,2, ...
                item: Item::Product(0), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,...
                item: Item::Class(1), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,...
                item: Item::Want(0), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,...
                item: Item::Product(2), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            let mut test = Property::new(test_desires);
            // make some default data for tests
            let mut data = DataManager::new();
            // 5 products, 
            // 0 for Product and class
            // 1 for class and ownership
            // 2 for Product and ownership
            // 3 for use
            // 4 for consumption
            data.products.insert(0, Product{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
                tags: vec![],
                wants: HashMap::new(),
                processes: HashSet::new(),
                failure_process: None,
                use_processes: HashSet::new(),
                consumption_processes: HashSet::new(),
                maintenance_processes: HashSet::new(),
                tech_required: None,
                product_class: Some(1),
            });
            data.products.insert(1, Product{
                id: 1,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
                tags: vec![],
                wants: HashMap::new(),
                processes: HashSet::new(),
                failure_process: None,
                use_processes: HashSet::new(),
                consumption_processes: HashSet::new(),
                maintenance_processes: HashSet::new(),
                tech_required: None,
                product_class: Some(1),
            });
            data.products.get_mut(&1).unwrap()
            .wants.insert(0, 1.0);
            data.products.insert(2, Product{
                id: 2,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            data.products.entry(2)
                .and_modify(|x| {
                    x.wants.insert(0, 1.0);
            });
            data.products.insert(3, Product{
                id: 3,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            data.products.get_mut(&3).unwrap()
            .use_processes.insert(0);
            data.products.insert(4, Product{
                id: 4,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            let mut want0 = Want{
                id: 0,
                name: "".to_string(),
                description: "".to_string(),
                decay: 0.0,
                ownership_sources: HashSet::new(),
                process_sources: HashSet::new(),
                use_sources: HashSet::new(),
                consumption_sources: HashSet::new(),
            };
            want0.ownership_sources.insert(1);
            want0.ownership_sources.insert(2);
            want0.process_sources.insert(0);
            want0.process_sources.insert(1);
            want0.use_sources.insert(0);
            want0.consumption_sources.insert(1);
            data.wants.insert(want0.id, want0);
            // add processes
            data.processes.insert(0, Process{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart{ item: Item::Product(3), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Capital },
                    ProcessPart{ item: Item::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                ],
                process_tags: vec![
                    ProcessTag::Use(3)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });
            data.processes.insert(1, Process{
                id: 1,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart{ item: Item::Product(4), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                ],
                process_tags: vec![
                    ProcessTag::Consumption(4)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });
            data.update_product_classes().expect("Failed to setup product classes.");
            test.is_sifted = true;
            // double check that everything is set up.
            assert!(test.is_sifted);
            assert!(test.desires[0].satisfaction == 0.0);
            assert!(test.desires[1].satisfaction == 0.0);
            assert!(test.desires[2].satisfaction == 0.0);
            assert!(test.desires[3].satisfaction == 0.0);
            // test satisfaction and reserves for each
            let val = test.add_property(0, 1.0, &data); // spec + class
            assert_eq!(val.tier, 0);
            assert!(val.value == 2.0);
            assert!(test.desires[0].satisfaction == 1.0);
            assert!(test.desires[1].satisfaction == 1.0);
            assert!(test.desires[2].satisfaction == 0.0);
            assert!(test.desires[3].satisfaction == 0.0);
            assert!(test.property.get(&0).unwrap().total_property == 1.0);
            assert!(test.property.get(&0).unwrap().product_reserve == 1.0);
            assert!(test.property.get(&0).unwrap().class_reserve == 1.0);
            assert!(test.property.get(&1).is_none());
            assert!(test.property.get(&2).is_none());
            assert!(test.property.get(&3).is_none());
            assert!(test.property.get(&4).is_none());
            let val = test.add_property(1, 1.0, &data); // class + want
            assert_eq!(val.tier, 0);
            assert!(val.value == 1.9);
            assert!(test.desires[0].satisfaction == 1.0);
            assert!(test.desires[1].satisfaction == 2.0);
            assert!(test.desires[2].satisfaction == 1.0);
            assert!(test.desires[3].satisfaction == 0.0);
            assert!(test.property.get(&0).unwrap().total_property == 1.0);
            assert!(test.property.get(&0).unwrap().product_reserve == 1.0);
            assert!(test.property.get(&0).unwrap().class_reserve == 1.0);
            assert!(test.property.get(&1).unwrap().total_property == 1.0);
            assert!(test.property.get(&1).unwrap().class_reserve == 1.0);
            assert!(test.property.get(&1).unwrap().want_reserve == 1.0);
            assert!(test.property.get(&2).is_none());
            assert!(test.property.get(&3).is_none());
            assert!(test.property.get(&4).is_none());
            let val = test.add_property(2, 1.0, &data); // spec + want
            assert_eq!(val.tier, 0);
            assert!(val.value == 1.9);
            assert!(test.desires[0].satisfaction == 1.0);
            assert!(test.desires[1].satisfaction == 2.0);
            assert!(test.desires[2].satisfaction == 2.0);
            assert!(test.desires[3].satisfaction == 1.0);
            assert!(test.property.get(&0).unwrap().total_property == 1.0);
            assert!(test.property.get(&0).unwrap().product_reserve == 1.0);
            assert!(test.property.get(&0).unwrap().class_reserve == 1.0);
            assert!(test.property.get(&1).unwrap().total_property == 1.0);
            assert!(test.property.get(&1).unwrap().class_reserve == 1.0);
            assert!(test.property.get(&1).unwrap().want_reserve == 1.0);
            assert!(test.property.get(&2).unwrap().total_property == 1.0);
            assert!(test.property.get(&2).unwrap().product_reserve == 1.0);
            assert!(test.property.get(&2).unwrap().want_reserve == 1.0);
            assert!(test.property.get(&3).is_none());
            assert!(test.property.get(&4).is_none());
            test.add_property(3, 1.0, &data);
            assert!(test.desires[0].satisfaction == 1.0);
            assert!(test.desires[1].satisfaction == 2.0);
            assert!(test.desires[2].satisfaction == 3.0);
            assert!(test.desires[3].satisfaction == 1.0);
            assert!(test.property.get(&0).unwrap().total_property == 1.0);
            assert!(test.property.get(&0).unwrap().product_reserve == 1.0);
            assert!(test.property.get(&1).unwrap().total_property == 1.0);
            assert!(test.property.get(&1).unwrap().class_reserve == 1.0);
            assert!(test.property.get(&2).unwrap().total_property == 1.0);
            assert!(test.property.get(&2).unwrap().want_reserve == 1.0);
            assert!(test.property.get(&3).unwrap().total_property == 1.0);
            assert!(test.property.get(&3).unwrap().want_reserve == 1.0);
            assert!(test.property.get(&4).is_none());
        }
    }

    mod total_estimated_value_should {
        use super::*;

        #[test]
        pub fn return_tiered_value_correctly() {
            let mut test_desires = vec![];
            test_desires.push(Desire{ // 0,1, ...
                item: Item::Product(0), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 0,1,...
                item: Item::Class(1), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 0,1,...
                item: Item::Want(0), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            let mut test = Property::new(test_desires);
            test.is_sifted = true;
            // 1 point in tier 0
            test.desires[0].satisfaction += 1.0;
            test.full_tier_satisfaction = None;
            let result = test.total_estimated_value();
            assert_eq!(result.tier, 0);
            assert_eq!(result.value, 1.0);

            // 2 points it tier 0
            test.desires[1].satisfaction += 1.0;
            test.full_tier_satisfaction = None;
            let result = test.total_estimated_value();
            assert_eq!(result.tier, 0);
            assert_eq!(result.value, 2.0);

            // 3 points it tier 0
            test.desires[2].satisfaction += 1.0;
            test.full_tier_satisfaction = None;
            let result = test.total_estimated_value();
            assert_eq!(result.tier, 0);
            assert_eq!(result.value, 3.0);

            // 1 points it tier 1
            test.desires[0].satisfaction += 1.0;
            test.highest_tier = 1;
            test.full_tier_satisfaction = Some(0);
            let result = test.total_estimated_value();
            assert_eq!(result.tier, 0);
            assert_eq!(result.value, 3.0 + 0.9);
            
            // 1 points it tier 2
            test.desires[0].satisfaction += 1.0;
            test.highest_tier = 2;
            test.full_tier_satisfaction = Some(0);
            let result = test.total_estimated_value();
            assert_eq!(result.tier, 0);
            assert_eq!(result.value, 3.0 + 0.9 + 0.81);

            // fill tier 1
            test.desires[1].satisfaction += 1.0;
            test.desires[2].satisfaction += 1.0;
            test.full_tier_satisfaction = Some(1);
            let result = test.total_estimated_value();
            assert_eq!(result.tier, 1);
            assert_eq!(result.value, (3.0 + 2.7 + 0.81)/0.9);
        }
    }

    mod preduct_value_gained_should {
        use std::collections::{HashSet, HashMap};
        use super::*;

        #[test]
        pub fn sift_products_down_from_higher_wants() {
            // Currently Not Implemented, Function is Placeholder
            assert!(true);
        }

        #[test]
        pub fn return_value_gained_correctly() {
            let mut test_desires = vec![];
            test_desires.push(Desire{ // 0,2,...
                item: Item::Want(0), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,...
                item: Item::Want(1), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            let mut test = Property::new(test_desires);
            // make some default data for tests
            let mut data = DataManager::new();
            // 5 products, 
            // 0 for Product
            // 1 for class
            // 2 for ownership
            // 3 for use
            // 4 for consumption
            data.products.insert(0, Product{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            data.products.insert(1, Product{
                id: 1,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
                tags: vec![],
                wants: HashMap::new(),
                processes: HashSet::new(),
                failure_process: None,
                use_processes: HashSet::new(),
                consumption_processes: HashSet::new(),
                maintenance_processes: HashSet::new(),
                tech_required: None,
                product_class: Some(1),
            });
            data.products.insert(2, Product{
                id: 2,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            data.products.entry(2)
                .and_modify(|x| {
                    x.wants.insert(0, 1.0);
                    x.wants.insert(1, 2.0);
            });
            data.products.insert(3, Product{
                id: 3,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            data.products.get_mut(&3).unwrap()
            .use_processes.insert(0);
            data.products.insert(4, Product{
                id: 4,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            let mut want0 = Want{
                id: 0,
                name: "".to_string(),
                description: "".to_string(),
                decay: 0.0,
                ownership_sources: HashSet::new(),
                process_sources: HashSet::new(),
                use_sources: HashSet::new(),
                consumption_sources: HashSet::new(),
            };
            let mut want1 = Want{
                id: 1,
                name: "".to_string(),
                description: "".to_string(),
                decay: 0.0,
                ownership_sources: HashSet::new(),
                process_sources: HashSet::new(),
                use_sources: HashSet::new(),
                consumption_sources: HashSet::new(),
            };
            want0.ownership_sources.insert(2);
            want0.process_sources.insert(0);
            want0.process_sources.insert(1);
            want0.use_sources.insert(0);
            want0.consumption_sources.insert(1);
            data.wants.insert(want0.id, want0);
            want1.ownership_sources.insert(2);
            data.wants.insert(want1.id, want1);
            // add processes
            data.processes.insert(0, Process{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart{ item: Item::Product(3), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Capital },
                    ProcessPart{ item: Item::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                ],
                process_tags: vec![
                    ProcessTag::Use(3)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });
            data.processes.insert(1, Process{
                id: 1,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart{ item: Item::Product(4), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                ],
                process_tags: vec![
                    ProcessTag::Consumption(4)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });
            data.update_product_classes().expect("Failed to setup product classes.");
            // add property to test with
            let result = test.predict_value_gained(2, 1.0, &data);
            // check that nothing was added.
            assert_eq!(test.property.len(), 0);
            let actual = test.add_property(2, 1.0, &data);
            assert_eq!(result.tier, actual.tier);
            assert!(result.value == actual.value);
        }

        #[test]
        pub fn calculate_correctly_without_exchanges() {
            let mut test_desires = vec![];
            test_desires.push(Desire{ // 0,2, ...
                item: Item::Product(0), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,...
                item: Item::Class(1), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,...
                item: Item::Want(0), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            let mut test = Property::new(test_desires);
            // make some default data for tests
            let mut data = DataManager::new();
            // 5 products, 
            // 0 for Product
            // 1 for class
            // 2 for ownership
            // 3 for use
            // 4 for consumption
            data.products.insert(0, Product{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            data.products.insert(1, Product{
                id: 1,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
                tags: vec![],
                wants: HashMap::new(),
                processes: HashSet::new(),
                failure_process: None,
                use_processes: HashSet::new(),
                consumption_processes: HashSet::new(),
                maintenance_processes: HashSet::new(),
                tech_required: None,
                product_class: Some(1),
            });
            data.products.insert(2, Product{
                id: 2,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            data.products.entry(2)
                .and_modify(|x| {
                    x.wants.insert(0, 1.0);
            });
            data.products.insert(3, Product{
                id: 3,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            data.products.get_mut(&3).unwrap()
            .use_processes.insert(0);
            data.products.insert(4, Product{
                id: 4,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            let mut want0 = Want{
                id: 0,
                name: "".to_string(),
                description: "".to_string(),
                decay: 0.0,
                ownership_sources: HashSet::new(),
                process_sources: HashSet::new(),
                use_sources: HashSet::new(),
                consumption_sources: HashSet::new(),
            };
            want0.ownership_sources.insert(2);
            want0.process_sources.insert(0);
            want0.process_sources.insert(1);
            want0.use_sources.insert(0);
            want0.consumption_sources.insert(1);
            data.wants.insert(want0.id, want0);
            // add processes
            data.processes.insert(0, Process{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart{ item: Item::Product(3), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Capital },
                    ProcessPart{ item: Item::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                ],
                process_tags: vec![
                    ProcessTag::Use(3)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });
            data.processes.insert(1, Process{
                id: 1,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart{ item: Item::Product(4), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                ],
                process_tags: vec![
                    ProcessTag::Consumption(4)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });
            data.update_product_classes().expect("Failed to setup product classes.");
            test.is_sifted = true;
            // double check that everything is set up.
            assert!(test.is_sifted);
            assert!(test.desires[0].satisfaction == 0.0);
            assert!(test.desires[1].satisfaction == 0.0);
            assert!(test.desires[2].satisfaction == 0.0);
            // test satisfaction and reserves for each
            let result = test.predict_value_gained(0, 2.0, &data);
            assert_eq!(result.tier, 0);
            assert_eq!(result.value, 1.9);
            assert!(test.desires[0].satisfaction == 0.0);
            assert!(test.desires[1].satisfaction == 0.0);
            assert!(test.desires[2].satisfaction == 0.0);
            assert!(test.property.get(&0).is_none());
            assert!(test.property.get(&1).is_none());
            assert!(test.property.get(&2).is_none());
            assert!(test.property.get(&3).is_none());
            assert!(test.property.get(&4).is_none());
            let result = test.predict_value_gained(1, 1.0, &data);
            assert_eq!(result.tier, 0);
            assert_eq!(result.value, 1.0);
            assert!(test.desires[0].satisfaction == 0.0);
            assert!(test.desires[1].satisfaction == 0.0);
            assert!(test.desires[2].satisfaction == 0.0);
            assert!(test.property.get(&0).is_none());
            assert!(test.property.get(&1).is_none());
            assert!(test.property.get(&2).is_none());
            assert!(test.property.get(&3).is_none());
            assert!(test.property.get(&4).is_none());
            let result = test.predict_value_gained(2, 1.0, &data);
            assert_eq!(result.tier, 0);
            assert_eq!(result.value, 1.0);
            assert!(test.desires[0].satisfaction == 0.0);
            assert!(test.desires[1].satisfaction == 0.0);
            assert!(test.desires[2].satisfaction == 0.0);
            assert!(test.property.get(&0).is_none());
            assert!(test.property.get(&1).is_none());
            assert!(test.property.get(&2).is_none());
            assert!(test.property.get(&3).is_none());
            assert!(test.property.get(&4).is_none());
            let result = test.predict_value_gained(3, 1.0, &data);
            assert_eq!(result.tier, 0);
            assert_eq!(result.value, 1.0);
            assert!(test.desires[0].satisfaction == 0.0);
            assert!(test.desires[1].satisfaction == 0.0);
            assert!(test.desires[2].satisfaction == 0.0);
            assert!(test.property.get(&0).is_none());
            assert!(test.property.get(&1).is_none());
            assert!(test.property.get(&2).is_none());
            assert!(test.property.get(&3).is_none());
            assert!(test.property.get(&4).is_none());
            let result = test.predict_value_gained(4, 1.0, &data);
            assert_eq!(result.tier, 0);
            assert_eq!(result.value, 1.0);
            assert!(test.desires[0].satisfaction == 0.0);
            assert!(test.desires[1].satisfaction == 0.0);
            assert!(test.desires[2].satisfaction == 0.0);
            assert!(test.property.get(&0).is_none());
            assert!(test.property.get(&1).is_none());
            assert!(test.property.get(&2).is_none());
            assert!(test.property.get(&3).is_none());
            assert!(test.property.get(&4).is_none());
            let result = test.predict_value_gained(4, 1.0, &data);
            assert_eq!(result.tier, 0);
            assert_eq!(result.value, 1.0);
            assert!(test.desires[0].satisfaction == 0.0);
            assert!(test.desires[1].satisfaction == 0.0);
            assert!(test.desires[2].satisfaction == 0.0);
            assert!(test.property.get(&0).is_none());
            assert!(test.property.get(&1).is_none());
            assert!(test.property.get(&2).is_none());
            assert!(test.property.get(&3).is_none());
            assert!(test.property.get(&4).is_none());
        }

        #[test]
        pub fn correctly_calculate_with_overlap_between_sections() {
            let mut test_desires = vec![];
            test_desires.push(Desire{ // 0,2, ...
                item: Item::Product(0), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,...
                item: Item::Class(1), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,...
                item: Item::Want(0), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,...
                item: Item::Product(2), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            let mut test = Property::new(test_desires);
            // make some default data for tests
            let mut data = DataManager::new();
            // 5 products, 
            // 0 for Product and class
            // 1 for class and ownership
            // 2 for Product and ownership
            // 3 for use
            // 4 for consumption
            data.products.insert(0, Product{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
                tags: vec![],
                wants: HashMap::new(),
                processes: HashSet::new(),
                failure_process: None,
                use_processes: HashSet::new(),
                consumption_processes: HashSet::new(),
                maintenance_processes: HashSet::new(),
                tech_required: None,
                product_class: Some(1),
            });
            data.products.insert(1, Product{
                id: 1,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
                tags: vec![],
                wants: HashMap::new(),
                processes: HashSet::new(),
                failure_process: None,
                use_processes: HashSet::new(),
                consumption_processes: HashSet::new(),
                maintenance_processes: HashSet::new(),
                tech_required: None,
                product_class: Some(1),
            });
            data.products.get_mut(&1).unwrap()
            .wants.insert(0, 1.0);
            data.products.insert(2, Product{
                id: 2,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            data.products.entry(2)
                .and_modify(|x| {
                    x.wants.insert(0, 1.0);
            });
            data.products.insert(3, Product{
                id: 3,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            data.products.get_mut(&3).unwrap()
            .use_processes.insert(0);
            data.products.insert(4, Product{
                id: 4,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            let mut want0 = Want{
                id: 0,
                name: "".to_string(),
                description: "".to_string(),
                decay: 0.0,
                ownership_sources: HashSet::new(),
                process_sources: HashSet::new(),
                use_sources: HashSet::new(),
                consumption_sources: HashSet::new(),
            };
            want0.ownership_sources.insert(1);
            want0.ownership_sources.insert(2);
            want0.process_sources.insert(0);
            want0.process_sources.insert(1);
            want0.use_sources.insert(0);
            want0.consumption_sources.insert(1);
            data.wants.insert(want0.id, want0);
            // add processes
            data.processes.insert(0, Process{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart{ item: Item::Product(3), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Capital },
                    ProcessPart{ item: Item::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                ],
                process_tags: vec![
                    ProcessTag::Use(3)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });
            data.processes.insert(1, Process{
                id: 1,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart{ item: Item::Product(4), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                ],
                process_tags: vec![
                    ProcessTag::Consumption(4)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });
            data.update_product_classes().expect("Failed to setup product classes.");
            test.is_sifted = true;
            // double check that everything is set up.
            assert!(test.is_sifted);
            assert!(test.desires[0].satisfaction == 0.0);
            assert!(test.desires[1].satisfaction == 0.0);
            assert!(test.desires[2].satisfaction == 0.0);
            assert!(test.desires[3].satisfaction == 0.0);
            // test satisfaction and reserves for each
            let val = test.predict_value_gained(0, 2.0, &data); // spec + class
            assert_eq!(val.tier, 0);
            assert!(val.value == 3.8);
            assert!(test.desires[0].satisfaction == 0.0);
            assert!(test.desires[1].satisfaction == 0.0);
            assert!(test.desires[2].satisfaction == 0.0);
            assert!(test.desires[3].satisfaction == 0.0);
            assert!(test.property.get(&0).is_none());
            assert!(test.property.get(&1).is_none());
            assert!(test.property.get(&2).is_none());
            assert!(test.property.get(&3).is_none());
            assert!(test.property.get(&4).is_none());
            let val = test.predict_value_gained(1, 1.0, &data); // class + want
            assert_eq!(val.tier, 0);
            assert!(val.value == 2.0);
            assert!(test.desires[0].satisfaction == 0.0);
            assert!(test.desires[1].satisfaction == 0.0);
            assert!(test.desires[2].satisfaction == 0.0);
            assert!(test.desires[3].satisfaction == 0.0);
            assert!(test.property.get(&0).is_none());
            assert!(test.property.get(&1).is_none());
            assert!(test.property.get(&2).is_none());
            assert!(test.property.get(&3).is_none());
            assert!(test.property.get(&4).is_none());
            let val = test.predict_value_gained(2, 1.0, &data); // spec + want
            assert_eq!(val.tier, 0);
            assert!(val.value == 2.0);
            assert!(test.desires[0].satisfaction == 0.0);
            assert!(test.desires[1].satisfaction == 0.0);
            assert!(test.desires[2].satisfaction == 0.0);
            assert!(test.desires[3].satisfaction == 0.0);
            assert!(test.property.get(&0).is_none());
            assert!(test.property.get(&1).is_none());
            assert!(test.property.get(&2).is_none());
            assert!(test.property.get(&3).is_none());
            assert!(test.property.get(&4).is_none());
            let val = test.predict_value_gained(3, 1.0, &data);
            assert_eq!(val.tier, 0);
            assert!(val.value == 1.0);
            assert!(test.desires[0].satisfaction == 0.0);
            assert!(test.desires[1].satisfaction == 0.0);
            assert!(test.desires[2].satisfaction == 0.0);
            assert!(test.desires[3].satisfaction == 0.0);
            assert!(test.property.get(&0).is_none());
            assert!(test.property.get(&1).is_none());
            assert!(test.property.get(&2).is_none());
            assert!(test.property.get(&3).is_none());
            assert!(test.property.get(&4).is_none());
        }
    }

    mod predict_value_changed_should {
        use std::collections::{HashSet, HashMap};
        use super::*;

        #[test]
        pub fn return_changed_value_correctly() {
            let mut test_desires = vec![];
            test_desires.push(Desire{ // 0,2, ...
                item: Item::Product(0), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,...
                item: Item::Class(1), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,...
                item: Item::Want(0), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            let mut test = Property::new(test_desires);
            // make some default data for tests
            let mut data = DataManager::new();
            // 5 products, 
            // 0 for Product
            // 1 for class
            // 2 for ownership
            // 3 for use
            // 4 for consumption
            data.products.insert(0, Product{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            data.products.insert(1, Product{
                id: 1,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
                tags: vec![],
                wants: HashMap::new(),
                processes: HashSet::new(),
                failure_process: None,
                use_processes: HashSet::new(),
                consumption_processes: HashSet::new(),
                maintenance_processes: HashSet::new(),
                tech_required: None,
                product_class: Some(1),
            });
            data.products.insert(2, Product{
                id: 2,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            data.products.entry(2)
                .and_modify(|x| {
                    x.wants.insert(0, 1.0);
            });
            data.products.insert(3, Product{
                id: 3,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            data.products.get_mut(&3).unwrap()
            .use_processes.insert(0);
            data.products.insert(4, Product{
                id: 4,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            let mut want0 = Want{
                id: 0,
                name: "".to_string(),
                description: "".to_string(),
                decay: 0.0,
                ownership_sources: HashSet::new(),
                process_sources: HashSet::new(),
                use_sources: HashSet::new(),
                consumption_sources: HashSet::new(),
            };
            want0.ownership_sources.insert(2);
            want0.process_sources.insert(0);
            want0.process_sources.insert(1);
            want0.use_sources.insert(0);
            want0.consumption_sources.insert(1);
            data.wants.insert(want0.id, want0);
            // add processes
            data.processes.insert(0, Process{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart{ item: Item::Product(3), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Capital },
                    ProcessPart{ item: Item::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                ],
                process_tags: vec![
                    ProcessTag::Use(3)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });
            data.processes.insert(1, Process{
                id: 1,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart{ item: Item::Product(4), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                ],
                process_tags: vec![
                    ProcessTag::Consumption(4)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });
            data.update_product_classes().expect("Failed to setup product classes.");
            test.is_sifted = true;
            test.add_property(0, 5.0, &data);
            test.add_property(1, 5.0, &data);
            test.add_property(2, 5.0, &data);
            test.add_property(3, 5.0, &data);
            test.add_property(4, 5.0, &data);
            // make a bunch of changes
            let mut changes = HashMap::new();
            changes.insert(0, 1.0);
            changes.insert(1, -1.0);
            changes.insert(2, 1.0);
            changes.insert(3, -1.0);
            changes.insert(4, 1.0);
            let prediction = test.predict_value_changed(&changes, &data);
            assert_eq!(test.property.len(), 5);
            assert!(test.property[&0].total_property == 5.0);
            assert!(test.property[&1].total_property == 5.0);
            assert!(test.property[&2].total_property == 5.0);
            assert!(test.property[&3].total_property == 5.0);
            assert!(test.property[&4].total_property == 5.0);
            // implement changes
            let mut actual_sum = TieredValue { tier: 0, value: 0.0 };
            for (&prod, &amount) in changes.iter() {
                actual_sum += test.add_property(prod, amount, &data);
            }
            // and check against the prediction
            assert!(prediction.near_eq(&actual_sum, 0.001));
        }
    }

    mod predict_value_lost_should {
        use std::collections::{HashSet, HashMap};
        use super::*;

        // TODO add additional stress tests.

        #[test]
        pub fn return_value_lost_correctly() {
            let mut test_desires = vec![];
            test_desires.push(Desire{ // 0,2, ...
                item: Item::Product(0), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,...
                item: Item::Class(1), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,...
                item: Item::Want(0), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            let mut test = Property::new(test_desires);
            // make some default data for tests
            let mut data = DataManager::new();
            // 5 products, 
            // 0 for Product
            // 1 for class
            // 2 for ownership
            // 3 for use
            // 4 for consumption
            data.products.insert(0, Product{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            data.products.insert(1, Product{
                id: 1,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
                tags: vec![],
                wants: HashMap::new(),
                processes: HashSet::new(),
                failure_process: None,
                use_processes: HashSet::new(),
                consumption_processes: HashSet::new(),
                maintenance_processes: HashSet::new(),
                tech_required: None,
                product_class: Some(1),
            });
            data.products.insert(2, Product{
                id: 2,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            data.products.entry(2)
                .and_modify(|x| {
                    x.wants.insert(0, 1.0);
            });
            data.products.insert(3, Product{
                id: 3,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            data.products.get_mut(&3).unwrap()
            .use_processes.insert(0);
            data.products.insert(4, Product{
                id: 4,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            let mut want0 = Want{
                id: 0,
                name: "".to_string(),
                description: "".to_string(),
                decay: 0.0,
                ownership_sources: HashSet::new(),
                process_sources: HashSet::new(),
                use_sources: HashSet::new(),
                consumption_sources: HashSet::new(),
            };
            want0.ownership_sources.insert(2);
            want0.process_sources.insert(0);
            want0.process_sources.insert(1);
            want0.use_sources.insert(0);
            want0.consumption_sources.insert(1);
            data.wants.insert(want0.id, want0);
            // add processes
            data.processes.insert(0, Process{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart{ item: Item::Product(3), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Capital },
                    ProcessPart{ item: Item::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                ],
                process_tags: vec![
                    ProcessTag::Use(3)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });
            data.processes.insert(1, Process{
                id: 1,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart{ item: Item::Product(4), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                ],
                process_tags: vec![
                    ProcessTag::Consumption(4)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });
            data.update_product_classes().expect("Failed to setup product classes.");
            test.is_sifted = true;
            test.add_property(0, 10.0, &data);
            test.add_property(1, 10.0, &data);
            test.add_property(2, 10.0, &data);
            test.add_property(3, 10.0, &data);
            test.add_property(4, 10.0, &data);
            // do the remove test
            let prediction = test.predict_value_lost(0, 5.0, &data);
            // check that nothing in test has changed
            assert_eq!(test.property.len(), 5);
            assert!(test.property[&0].total_property == 10.0);
            assert!(test.property[&1].total_property == 10.0);
            assert!(test.property[&2].total_property == 10.0);
            assert!(test.property[&3].total_property == 10.0);
            assert!(test.property[&4].total_property == 10.0);
            // nothing's changed. Check by removing, they should be equal.
            let removed = test.remove_property(0, 5.0, &data);
            assert_eq!(prediction.tier, removed.tier);
            assert!(prediction.value == removed.value);
        }
    }

    #[test]
    pub fn unsafe_add_property_should_add_and_mark_as_unsifted() {
        let mut test_desires = vec![];
        test_desires.push(Desire{ // 0,2
            item: Item::Product(0), 
            start: 0, 
            end: Some(2), 
            amount: 1.0, 
            satisfaction: 2.0,
            step: 2,
            tags: vec![]});
        test_desires.push(Desire{ // 0,2,4,6,8,10
            item: Item::Product(1), 
            start: 0, 
            end: Some(10), 
            amount: 1.0, 
            satisfaction: 3.0,
            step: 2,
            tags: vec![]});
        let mut test = Property::new(test_desires);

        // sanity check that it's sifted, and has no property
        assert!(test.is_sifted);
        assert_eq!(test.property.len(), 0);
        // unsafe add the property
        test.unsafe_add_property(0, 10.0);
        assert!(!test.is_sifted);
        assert_eq!(test.property.len(), 1);
        let val = test.property.get(&0).unwrap();
        assert!(val.total_property == 10.0);
        assert!(val.unreserved == 10.0);
        //assert!(val.reserved == 0.0);
        assert!(val.product_reserve == 0.0);
        assert!(val.class_reserve == 0.0);
        assert!(val.want_reserve == 0.0);
    }

    mod market_wealth_should {
        use std::collections::HashMap;
        use super::*;

        #[test]
        pub fn return_the_total_amv_of_property() {
            let mut test_desires = vec![];
            test_desires.push(Desire{ // 0,2
                item: Item::Product(0), 
                start: 0, 
                end: Some(2), 
                amount: 1.0, 
                satisfaction: 2.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,4,6,8,10
                item: Item::Product(1), 
                start: 0, 
                end: Some(10), 
                amount: 1.0, 
                satisfaction: 3.0,
                step: 2,
                tags: vec![]});
            let mut test = Property::new(test_desires);
            let mut product_info = HashMap::new();
            product_info.insert(0, ProductInfo{
                available: 0.0,
                price: 1.0,
                offered: 0.0,
                sold: 0.0,
                salability: 0.5,
                is_currency: false,
            });
            product_info.insert(1, ProductInfo{
                available: 0.0,
                price: 5.0,
                offered: 0.0,
                sold: 0.0,
                salability: 0.5,
                is_currency: false,
            });
            let test_market = MarketHistory{
                product_info,
                sale_priority: vec![],
                currencies: vec![],
                want_info: HashMap::new(),
                class_info: HashMap::new(),
            };
            // TODO fix this
            test.unsafe_add_property(0, 4.0);
            test.unsafe_add_property(1, 5.0);
            let result = test.market_wealth(&test_market);
            assert!(result == 29.0);
        }
    }

    mod market_satisfaction_should {
        use std::collections::HashMap;
        use super::*;

        #[test]
        pub fn return_correct_market_satisfaction() {
            let mut test_desires = vec![];
            test_desires.push(Desire{ // 0,2
                item: Item::Product(0), 
                start: 0, 
                end: Some(2), 
                amount: 1.0, 
                satisfaction: 2.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,4,6,8,10
                item: Item::Product(1), 
                start: 0, 
                end: Some(10), 
                amount: 1.0, 
                satisfaction: 3.0,
                step: 2,
                tags: vec![]});
            let mut test = Property::new(test_desires);
            let mut product_info = HashMap::new();
            product_info.insert(0, ProductInfo{
                available: 0.0,
                price: 1.0,
                offered: 0.0,
                sold: 0.0,
                salability: 0.5,
                is_currency: false,
            });
            product_info.insert(1, ProductInfo{
                available: 0.0,
                price: 5.0,
                offered: 0.0,
                sold: 0.0,
                salability: 0.5,
                is_currency: false,
            });
            let test_market = MarketHistory{
                product_info,
                sale_priority: vec![],
                currencies: vec![],
                want_info: HashMap::new(),
                class_info: HashMap::new(),
            };
            let result = test.market_satisfaction(&test_market);
            assert!(result == 17.0);
        }
    }

    mod update_satisfactions_should {
        use super::*;

        #[test]
        pub fn correctly_update_satisfaction() {
            let mut test_desires = vec![];
            test_desires.push(Desire{ // 0,2
                item: Item::Product(0), 
                start: 0, 
                end: Some(2), 
                amount: 1.0, 
                satisfaction: 2.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,4,6,8,10
                item: Item::Product(1), 
                start: 0, 
                end: Some(10), 
                amount: 1.0, 
                satisfaction: 6.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,4,6,8,10
                item: Item::Product(2), 
                start: 0, 
                end: Some(10), 
                amount: 1.0, 
                satisfaction: 4.0,
                step: 2,
                tags: vec![]});
            let mut test = Property::new(test_desires);
            test.update_satisfactions();

            assert_eq!(test.full_tier_satisfaction.unwrap(), 6);
            assert_eq!(test.highest_tier, 10);
            assert!(test.quantity_satisfied == 12.0);
            assert!(test.partial_satisfaction > 3.0 && test.partial_satisfaction < 4.0);
            assert_eq!(test.hard_satisfaction.unwrap(), 4);
        }

        #[test]
        pub fn set_satisfaction_for_fully_satiated_desires_correctly() {
            let mut test = Property::new(vec![
                Desire::new(Item::Product(2), 0, Some(10), 1.0, 11.0, 1, vec![]).unwrap()
            ]);

            test.update_satisfactions();

            assert_eq!(test.full_tier_satisfaction.unwrap(), 10);
            assert_eq!(test.hard_satisfaction.unwrap(), 11);
            assert_eq!(test.quantity_satisfied, 11.0);
            assert_eq!(test.partial_satisfaction, 1.0);
            assert_eq!(test.highest_tier, 10);
        }

        #[test]
        pub fn set_full_tier_and_hard_sat_to_none_when_at_least_one_desire_unsatisfied() {
            let mut test_desires = vec![];
            test_desires.push(Desire{ // 0,2
                item: Item::Product(0), 
                start: 0, 
                end: Some(2), 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,4,6,8,10
                item: Item::Product(1), 
                start: 0, 
                end: Some(10), 
                amount: 1.0, 
                satisfaction: 6.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,4,6,8,10
                item: Item::Product(2), 
                start: 0, 
                end: Some(10), 
                amount: 1.0, 
                satisfaction: 4.0,
                step: 2,
                tags: vec![]});
            let mut test = Property::new(test_desires);
            test.update_satisfactions();

            assert!(test.full_tier_satisfaction.is_none());
            assert_eq!(test.highest_tier, 10);
            assert!(test.quantity_satisfied == 10.0);
            assert!(test.partial_satisfaction > 6.0 && test.partial_satisfaction < 7.0);
            assert!(test.hard_satisfaction.is_none());
        }
    }

    mod remove_property_should {
        use std::collections::{HashMap, HashSet};
        use super::*;

        #[test]
        pub fn correctly_remove_item_from_property_and_satisfaction() {
            // make some default data for tests
            let mut data = DataManager::new();
            // 5 products, 
            // 0 for Product
            // 1 for class
            // 2 for ownership
            // 3 for use
            // 4 for consumption
            data.products.insert(0, Product{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            data.products.insert(1, Product{
                id: 1,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
                tags: vec![],
                wants: HashMap::new(),
                processes: HashSet::new(),
                failure_process: None,
                use_processes: HashSet::new(),
                consumption_processes: HashSet::new(),
                maintenance_processes: HashSet::new(),
                tech_required: None,
                product_class: Some(1),
            });
            data.products.insert(2, Product{
                id: 2,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            data.products.entry(2)
                .and_modify(|x| {
                    x.wants.insert(0, 1.0);
                    x.wants.insert(1, 2.0);
            });
            data.products.insert(3, Product{
                id: 3,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            data.products.get_mut(&3).unwrap()
            .use_processes.insert(0);
            data.products.insert(4, Product{
                id: 4,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            let mut want0 = Want{
                id: 0,
                name: "".to_string(),
                description: "".to_string(),
                decay: 0.0,
                ownership_sources: HashSet::new(),
                process_sources: HashSet::new(),
                use_sources: HashSet::new(),
                consumption_sources: HashSet::new(),
            };
            let mut want1 = Want{
                id: 1,
                name: "".to_string(),
                description: "".to_string(),
                decay: 0.0,
                ownership_sources: HashSet::new(),
                process_sources: HashSet::new(),
                use_sources: HashSet::new(),
                consumption_sources: HashSet::new(),
            };
            want0.ownership_sources.insert(2);
            want0.process_sources.insert(0);
            want0.process_sources.insert(1);
            want0.use_sources.insert(0);
            want0.consumption_sources.insert(1);
            data.wants.insert(want0.id, want0);
            want1.ownership_sources.insert(2);
            data.wants.insert(want1.id, want1);
            // add processes
            data.processes.insert(0, Process{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart{ item: Item::Product(3), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Capital },
                    ProcessPart{ item: Item::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                ],
                process_tags: vec![
                    ProcessTag::Use(3)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });
            data.processes.insert(1, Process{
                id: 1,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart{ item: Item::Product(4), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                ],
                process_tags: vec![
                    ProcessTag::Consumption(4)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });
            data.update_product_classes().expect("Failed to setup product classes.");
            // setup desires
            let mut test_desires = vec![];
            test_desires.push(Desire{ // 0,2
                item: Item::Product(0), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,4,6,8,10
                item: Item::Product(1), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2
                item: Item::Product(2), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2
                item: Item::Product(3), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2
                item: Item::Product(4), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2
                item: Item::Class(1), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2
                item: Item::Product(4), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2
                item: Item::Want(0), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2
                item: Item::Want(1), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 2,
                tags: vec![]});
            let mut test = Property::new(test_desires);
            // add property
            test.property.insert(0, PropertyInfo::new(10.0));
            test.property.insert(1, PropertyInfo::new(10.0));
            test.property.insert(2, PropertyInfo::new(10.0));
            test.property.insert(3, PropertyInfo::new(10.0));
            test.property.insert(4, PropertyInfo::new(10.0));
            // get initial value
            let initial = test.sift_all(&data);
            //print!("{}", test.print_satisfactions(None, None));
            assert_eq!(initial.tier, 8);
            assert!(92.0 < initial.value);
            assert!(initial.value < 93.0);
            // print!("{}", test.print_satisfactions(None, None));
            // removes
            // subtract 1.0 from 0
            let first_remove = test.remove_property(0, 1.0, &data);
            // print!("{}", test.print_satisfactions(None, None));
            assert_eq!(first_remove.tier, 8);
            assert!(-1.0 < first_remove.value);
            assert!(first_remove.value < 0.0);
            // subtract 1.0 from 1
            let second_remove = test.remove_property(1, 1.0, &data);
            assert_eq!(second_remove.tier, 8);
            assert!(-1.0 < second_remove.value);
            assert!(second_remove.value < 0.0);
            // print!("{}", test.print_satisfactions(None, None));
            // subtract 1.0 from 2
            let second_remove = test.remove_property(2, 1.0, &data);
            assert_eq!(second_remove.tier, 8);
            assert!(-1.0 < second_remove.value);
            assert!(second_remove.value < 0.0);
            // print!("{}", test.print_satisfactions(None, None));
            // subtract 1.0 from 3
            let second_remove = test.remove_property(3, 1.0, &data);
            assert_eq!(second_remove.tier, 8);
            assert!(-1.0 < second_remove.value);
            assert!(second_remove.value < 0.0);
            // print!("{}", test.print_satisfactions(None, None));
            // subtract 1.0 from 4
            let second_remove = test.remove_property(4, 1.0, &data);
            assert_eq!(second_remove.tier, 6);
            assert!(-1.0 < second_remove.value);
            assert!(second_remove.value < 0.0);
            // print!("{}", test.print_satisfactions(None, None));
        }
    }

    mod print_satisfactions_should {
        use std::collections::{HashMap, HashSet};
        use super::*;

        #[test]
        pub fn print_correctly() {
            // make some default data for tests
            let mut data = DataManager::new();
            // 5 products, 
            // 0 for Product
            // 1 for class
            // 2 for ownership
            // 3 for use
            // 4 for consumption
            data.products.insert(0, Product{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            data.products.insert(1, Product{
                id: 1,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
                tags: vec![],
                wants: HashMap::new(),
                processes: HashSet::new(),
                failure_process: None,
                use_processes: HashSet::new(),
                consumption_processes: HashSet::new(),
                maintenance_processes: HashSet::new(),
                tech_required: None,
                product_class: Some(1),
            });
            data.products.insert(2, Product{
                id: 2,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            data.products.entry(2)
                .and_modify(|x| {
                    x.wants.insert(0, 1.0);
                    x.wants.insert(1, 2.0);
            });
            data.products.insert(3, Product{
                id: 3,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            data.products.get_mut(&3).unwrap()
            .use_processes.insert(0);
            data.products.insert(4, Product{
                id: 4,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                unit_name: "".to_string(),
                quality: 0,
                mass: 0.0,
                bulk: 0.0,
                mean_time_to_failure: None,
                fractional: true,
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
            let mut want0 = Want{
                id: 0,
                name: "".to_string(),
                description: "".to_string(),
                decay: 0.0,
                ownership_sources: HashSet::new(),
                process_sources: HashSet::new(),
                use_sources: HashSet::new(),
                consumption_sources: HashSet::new(),
            };
            let mut want1 = Want{
                id: 1,
                name: "".to_string(),
                description: "".to_string(),
                decay: 0.0,
                ownership_sources: HashSet::new(),
                process_sources: HashSet::new(),
                use_sources: HashSet::new(),
                consumption_sources: HashSet::new(),
            };
            want0.ownership_sources.insert(2);
            want0.process_sources.insert(0);
            want0.process_sources.insert(1);
            want0.use_sources.insert(0);
            want0.consumption_sources.insert(1);
            data.wants.insert(want0.id, want0);
            want1.ownership_sources.insert(2);
            data.wants.insert(want1.id, want1);
            // add processes
            data.processes.insert(0, Process{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart{ item: Item::Product(3), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Capital },
                    ProcessPart{ item: Item::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                ],
                process_tags: vec![
                    ProcessTag::Use(3)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });
            data.processes.insert(1, Process{
                id: 1,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart{ item: Item::Product(4), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Input },
                    ProcessPart{ item: Item::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                ],
                process_tags: vec![
                    ProcessTag::Consumption(4)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });
            data.update_product_classes().expect("Failed to setup product classes.");
            // setup desires
            let mut test_desires = vec![];
            test_desires.push(Desire{ // 0,2
                item: Item::Product(0), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,4,6,8,10
                item: Item::Product(1), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2
                item: Item::Product(2), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2
                item: Item::Product(3), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2
                item: Item::Product(4), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2
                item: Item::Class(1), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2
                item: Item::Product(4), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2
                item: Item::Want(0), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2
                item: Item::Want(1), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 2,
                tags: vec![]});
            let mut test = Property::new(test_desires);
            // add property
            test.property.insert(0, PropertyInfo::new(10.0));
            test.property.insert(1, PropertyInfo::new(10.0));
            test.property.insert(2, PropertyInfo::new(10.0));
            test.property.insert(3, PropertyInfo::new(10.0));
            test.property.insert(4, PropertyInfo::new(10.0));
            // get initial value
            let _initial = test.sift_all(&data);
            //print!("{}", test.print_satisfactions(None, None));
        }
    }

    #[test]
    pub fn total_desire_at_tier() {
        let mut test_desires = vec![];
        test_desires.push(Desire{ // 0,2
            item: Item::Product(0), 
            start: 0, 
            end: Some(2), 
            amount: 1.0, 
            satisfaction: 1.0,
            step: 2,
            tags: vec![]});
        test_desires.push(Desire{ // 0,2,4,6,8,10
            item: Item::Product(1), 
            start: 0, 
            end: Some(10), 
            amount: 1.0, 
            satisfaction: 2.0,
            step: 2,
            tags: vec![]});
        let test = Property::new(test_desires);

        assert_eq!(test.total_desire_at_tier(0), 2.0);
        assert_eq!(test.total_desire_at_tier(1), 0.0);
        assert_eq!(test.total_desire_at_tier(2), 2.0);
        assert_eq!(test.total_desire_at_tier(4), 1.0);
    }

    #[test]
    pub fn total_satisfaction_at_tier() {
        let mut test_desires = vec![];
        test_desires.push(Desire{ // 0,2
            item: Item::Product(0), 
            start: 0, 
            end: Some(2), 
            amount: 1.0, 
            satisfaction: 1.0,
            step: 2,
            tags: vec![]});
        test_desires.push(Desire{ // 0,2,4,6,8,10
            item: Item::Product(1), 
            start: 0, 
            end: Some(10), 
            amount: 1.0, 
            satisfaction: 2.0,
            step: 2,
            tags: vec![]});
        let test = Property::new(test_desires);

        assert_eq!(test.total_satisfaction_at_tier(0), 2.0);
        assert_eq!(test.total_satisfaction_at_tier(2), 1.0);
        assert_eq!(test.total_satisfaction_at_tier(4), 0.0);
    }

    pub mod satisfied_at_tier_should {
        use super::*;

        #[test]
        pub fn calculate_correctly() {
            let mut test_desires = vec![];
            test_desires.push(Desire{ // 0,2
                item: Item::Product(0), 
                start: 0, 
                end: Some(2), 
                amount: 1.0, 
                satisfaction: 1.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,4,6,8,10
                item: Item::Product(1), 
                start: 0, 
                end: Some(10), 
                amount: 1.0, 
                satisfaction: 2.0,
                step: 2,
                tags: vec![]});
            let test = Property::new(test_desires);

            assert!(test.satisfied_at_tier(0));
            assert!(!test.satisfied_at_tier(2));
            assert!(!test.satisfied_at_tier(4));
        }

    }

    mod sift_up_to_should {
        // TODO update test for lookahead when added.
        use std::collections::{HashMap, HashSet};
        use super::*;

        #[test]
        pub fn shift_want_class_and_product_desires_correctly() {
            // data needed, but not set up for this test.
            let mut data = DataManager::new();
            // wants 0
            let mut want0 = Want{
                id: 0,
                name: "".to_string(),
                description: "".to_string(),
                decay: 0.0,
                ownership_sources: HashSet::new(),
                process_sources: HashSet::new(),
                use_sources: HashSet::new(),
                consumption_sources: HashSet::new(),
            };
            want0.ownership_sources.insert(0);
            want0.process_sources.insert(0);
            want0.process_sources.insert(1);
            want0.use_sources.insert(0);
            want0.consumption_sources.insert(1);
            data.wants.insert(0, want0);
            // products
            data.products.insert(0, Product{
                id: 0,
                name: "".to_string(),
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
            data.products.get_mut(&0).unwrap()
            .wants.insert(0, 1.0);
            data.products.insert(1, Product{
                id: 1,
                name: "".to_string(),
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
            let mut product2 = Product{
                id: 2,
                name: "".to_string(),
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
            };
            product2.use_processes.insert(0);
            product2.consumption_processes.insert(1);
            data.products.insert(2, product2);
            data.products.insert(3, Product{
                id: 3,
                name: "".to_string(),
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
            // products use 1 + 2 = want 0
            data.processes.insert(0, Process{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
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
                        part: ProcessSectionTag::Capital
                    },
                    ProcessPart { 
                        item: Item::Want(0), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output
                    }
                ],
                process_tags: vec![
                    ProcessTag::Use(2)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });
            // products consume 2 + 3 = want 0
            data.processes.insert(1, Process{
                id: 1,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart { 
                        item: Item::Product(2), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input
                    },
                    ProcessPart { 
                        item: Item::Product(3), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input
                    },
                    ProcessPart { 
                        item: Item::Want(0), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output
                    }
                ],
                process_tags: vec![
                    ProcessTag::Consumption(2)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });

            data.update_product_classes().expect("Could not function");
            let test_desires = vec![
                Desire::new(Item::Want(0),
                    1,
                    None,
                    1.0,
                    0.0,
                    1,
                    vec![]).unwrap(),
                Desire::new(Item::Class(0),
                    3,
                    Some(30),
                    1.0,
                    0.0,
                    3,
                    vec![]).unwrap(),
                Desire::new(Item::Product(0),
                    5,
                    Some(10),
                    1.0,
                    0.0,
                    5,
                    vec![]).unwrap()
            ];
            let mut test = Property::new(test_desires);
            test.property.insert(0, PropertyInfo::new(15.0));
            test.property.insert(1, PropertyInfo::new(10.0));
            test.property.insert(2, PropertyInfo::new(20.0));
            test.property.insert(3, PropertyInfo::new(15.0));
            let coord = DesireCoord { tier: 25, idx: 10};
            let result = test.sift_up_to(&coord, &data);
            // check that the sitfing was done correctly.
            // 26.0 into desire 0, (tier 100)
            let desire0 = test.desires.get(0).unwrap();
            assert_eq!(desire0.satisfaction_up_to_tier().unwrap(), 25);
            assert!(desire0.satisfaction == 25.0);
            // 4.0 into desire 1 (tier 12, totally satisfied)
            let desire1 = test.desires.get(1).unwrap();
            assert_eq!(desire1.satisfaction_up_to_tier().unwrap(), 24);
            assert!(desire1.satisfaction == 8.0);
            // 4.0 into desire 1 (tier 12, totally satisfied)
            let desire2 = test.desires.get(2).unwrap();
            assert!(desire2.is_fully_satisfied());
            assert_eq!(desire2.satisfaction_up_to_tier().unwrap(), 10);
            assert!(desire2.satisfaction == 2.0);
            // and check that items were reserved correctly.
            let prop0 = test.property.get(&0).unwrap();
            assert!(prop0.total_property == 15.0);
            assert!(prop0.unreserved == 0.0);
            //assert!(prop0.reserved == 0.0);
            assert!(prop0.want_reserve == 15.0);
            // assert!(prop0.class_reserve == 0.0); both are in the same class, so either is valid, selection order cannot be guaranteed (yet).
            assert!(prop0.product_reserve == 2.0);
            let prop1 = test.property.get(&1).unwrap();
            assert!(prop1.total_property == 10.0);
            assert!(prop1.unreserved == 0.0);
            //assert!(prop1.reserved == 0.0);
            assert!(prop1.want_reserve == 10.0);
            assert!((prop1.class_reserve + prop0.class_reserve) == 8.0);
            assert!(prop1.product_reserve == 0.0);
            let prop2 = test.property.get(&2).unwrap();
            assert!(prop2.total_property == 20.0);
            assert!(prop2.unreserved == 10.0);
            //assert!(prop2.reserved == 0.0);
            assert!(prop2.want_reserve == 10.0);
            assert!(prop2.class_reserve == 0.0);
            assert!(prop2.product_reserve == 0.0);
            let prop3 = test.property.get(&3).unwrap();
            assert!(prop3.total_property == 15.0);
            assert!(prop3.unreserved == 15.0);
            //assert!(prop3.reserved == 0.0);
            assert!(prop3.want_reserve == 0.0);
            assert!(prop3.class_reserve == 0.0);
            assert!(prop3.product_reserve == 0.0);
            // ensure want process plan is recorded correctly.
            assert_eq!(test.process_plan[&0], 10.0);
            assert!(test.process_plan.get(&2).is_none());
            // ensure the tiered value is correct to match
            assert_eq!(result.tier, 24);
            assert!(147.0 < result.value);
            assert!(result.value < 148.0);
        }

        #[test]
        pub fn shift_want_and_class_desires_correctly() {
            // data needed, but not set up for this test.
            let mut data = DataManager::new();
            // wants 0
            let mut want0 = Want{
                id: 0,
                name: "".to_string(),
                description: "".to_string(),
                decay: 0.0,
                ownership_sources: HashSet::new(),
                process_sources: HashSet::new(),
                use_sources: HashSet::new(),
                consumption_sources: HashSet::new(),
            };
            want0.ownership_sources.insert(0);
            want0.process_sources.insert(0);
            want0.process_sources.insert(1);
            want0.use_sources.insert(0);
            want0.consumption_sources.insert(1);
            data.wants.insert(0, want0);
            // products
            data.products.insert(0, Product{
                id: 0,
                name: "".to_string(),
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
            data.products.get_mut(&0).unwrap()
            .wants.insert(0, 1.0);
            data.products.insert(1, Product{
                id: 1,
                name: "".to_string(),
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
            let mut product2 = Product{
                id: 2,
                name: "".to_string(),
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
            };
            product2.use_processes.insert(0);
            product2.consumption_processes.insert(1);
            data.products.insert(2, product2);
            data.products.insert(3, Product{
                id: 3,
                name: "".to_string(),
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
            // products use 1 + 2 = want 0
            data.processes.insert(0, Process{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
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
                        part: ProcessSectionTag::Capital
                    },
                    ProcessPart { 
                        item: Item::Want(0), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output
                    }
                ],
                process_tags: vec![
                    ProcessTag::Use(2)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });
            // products consume 2 + 3 = want 0
            data.processes.insert(1, Process{
                id: 1,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart { 
                        item: Item::Product(2), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input
                    },
                    ProcessPart { 
                        item: Item::Product(3), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input
                    },
                    ProcessPart { 
                        item: Item::Want(0), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output
                    }
                ],
                process_tags: vec![
                    ProcessTag::Consumption(2)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });

            data.update_product_classes().expect("Could not function");
            let test_desires = vec![
                Desire::new(Item::Want(0),
                    1,
                    None,
                    1.0,
                    0.0,
                    1,
                    vec![]).unwrap(),
                Desire::new(Item::Class(0),
                    3,
                    Some(30),
                    1.0,
                    0.0,
                    3,
                    vec![]).unwrap(),
            ];
            let mut test = Property::new(test_desires);
            test.property.insert(0, PropertyInfo::new(15.0));
            test.property.insert(1, PropertyInfo::new(10.0));
            test.property.insert(2, PropertyInfo::new(20.0));
            test.property.insert(3, PropertyInfo::new(15.0));
            let coord = DesireCoord { tier: 25, idx: 10};
            let result = test.sift_up_to(&coord, &data);
            // check that the sitfing was done correctly.
            // 26.0 into desire 0, (tier 100)
            let desire0 = test.desires.get(0).unwrap();
            assert_eq!(desire0.satisfaction_up_to_tier().unwrap(), 25);
            assert!(desire0.satisfaction == 25.0);
            // 4.0 into desire 1 (tier 12, totally satisfied)
            let desire1 = test.desires.get(1).unwrap();
            assert_eq!(desire1.satisfaction_up_to_tier().unwrap(), 24);
            assert!(desire1.satisfaction == 8.0);
            // and check that items were reserved correctly.
            let prop0 = test.property.get(&0).unwrap();
            assert!(prop0.total_property == 15.0);
            assert!(prop0.unreserved == 0.0);
            //assert!(prop0.reserved == 0.0);
            assert!(prop0.want_reserve == 15.0);
            // assert!(prop0.class_reserve == 0.0); both are in the same class, so either is valid, selection order cannot be guaranteed (yet).
            assert!(prop0.product_reserve == 0.0);
            let prop1 = test.property.get(&1).unwrap();
            assert!(prop1.total_property == 10.0);
            assert!(prop1.unreserved == 0.0);
            //assert!(prop1.reserved == 0.0);
            assert!(prop1.want_reserve == 10.0);
            assert!((prop1.class_reserve + prop0.class_reserve) == 8.0);
            assert!(prop1.product_reserve == 0.0);
            let prop2 = test.property.get(&2).unwrap();
            assert!(prop2.total_property == 20.0);
            assert!(prop2.unreserved == 10.0);
            //assert!(prop2.reserved == 0.0);
            assert!(prop2.want_reserve == 10.0);
            assert!(prop2.class_reserve == 0.0);
            assert!(prop2.product_reserve == 0.0);
            let prop3 = test.property.get(&3).unwrap();
            assert!(prop3.total_property == 15.0);
            assert!(prop3.unreserved == 15.0);
            //assert!(prop3.reserved == 0.0);
            assert!(prop3.want_reserve == 0.0);
            assert!(prop3.class_reserve == 0.0);
            assert!(prop3.product_reserve == 0.0);
            assert_eq!(result.tier, 24);
            assert!(135.0 < result.value);
            assert!(result.value < 136.0);
        }

        #[test]
        pub fn shift_want_and_product_desires_correctly() {
            // data needed, but not set up for this test.
            let mut data = DataManager::new();
            // wants 0
            let mut want0 = Want{
                id: 0,
                name: "".to_string(),
                description: "".to_string(),
                decay: 0.0,
                ownership_sources: HashSet::new(),
                process_sources: HashSet::new(),
                use_sources: HashSet::new(),
                consumption_sources: HashSet::new(),
            };
            want0.ownership_sources.insert(0);
            want0.process_sources.insert(0);
            want0.process_sources.insert(1);
            want0.use_sources.insert(0);
            want0.consumption_sources.insert(1);
            data.wants.insert(0, want0);
            // products
            data.products.insert(0, Product{
                id: 0,
                name: "".to_string(),
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
            data.products.get_mut(&0).unwrap()
            .wants.insert(0, 1.0);
            data.products.insert(1, Product{
                id: 1,
                name: "".to_string(),
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
            let mut product2 = Product{
                id: 2,
                name: "".to_string(),
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
            };
            product2.use_processes.insert(0);
            product2.consumption_processes.insert(1);
            data.products.insert(2, product2);
            data.products.insert(3, Product{
                id: 3,
                name: "".to_string(),
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
            // products use 1 + 2 = want 0
            data.processes.insert(0, Process{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
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
                        part: ProcessSectionTag::Capital
                    },
                    ProcessPart { 
                        item: Item::Want(0), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output
                    }
                ],
                process_tags: vec![
                    ProcessTag::Use(2)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });
            // products consume 2 + 3 = want 0
            data.processes.insert(1, Process{
                id: 1,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart { 
                        item: Item::Product(2), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input
                    },
                    ProcessPart { 
                        item: Item::Product(3), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input
                    },
                    ProcessPart { 
                        item: Item::Want(0), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output
                    }
                ],
                process_tags: vec![
                    ProcessTag::Consumption(2)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });

            data.update_product_classes().expect("Could not function");
            let test_desires = vec![
                Desire::new(Item::Want(0),
                    1,
                    None,
                    1.0,
                    0.0,
                    1,
                    vec![]).unwrap(),
                Desire::new(Item::Product(2),
                    3,
                    Some(15),
                    1.0,
                    0.0,
                    3,
                    vec![]).unwrap(),
            ];
            let mut test = Property::new(test_desires);
            test.property.insert(0, PropertyInfo::new(15.0));
            test.property.insert(1, PropertyInfo::new(10.0));
            test.property.insert(2, PropertyInfo::new(20.0));
            test.property.insert(3, PropertyInfo::new(15.0));
            let coord = DesireCoord { tier: 25, idx: 10};
            let result = test.sift_up_to(&coord, &data);
            // check that the sitfing was done correctly.
            // 26.0 into desire 0, (tier 100)
            let desire0 = test.desires.get(0).unwrap();
            assert_eq!(desire0.satisfaction_up_to_tier().unwrap(), 25);
            assert!(desire0.satisfaction == 25.0);
            // 4.0 into desire 1 (tier 12, totally satisfied)
            let desire1 = test.desires.get(1).unwrap();
            assert!(desire1.is_fully_satisfied());
            assert_eq!(desire1.satisfaction_up_to_tier().unwrap(), 15);
            assert!(desire1.satisfaction == 5.0);
            // and check that items were reserved correctly.
            let prop0 = test.property.get(&0).unwrap();
            assert!(prop0.total_property == 15.0);
            assert!(prop0.unreserved == 0.0);
            //assert!(prop0.reserved == 0.0);
            assert!(prop0.want_reserve == 15.0);
            assert!(prop0.class_reserve == 0.0);
            assert!(prop0.product_reserve == 0.0);
            let prop1 = test.property.get(&1).unwrap();
            assert!(prop1.total_property == 10.0);
            assert!(prop1.unreserved == 0.0);
            //assert!(prop1.reserved == 0.0);
            assert!(prop1.want_reserve == 10.0);
            assert!(prop1.class_reserve == 0.0);
            assert!(prop1.product_reserve == 0.0);
            let prop2 = test.property.get(&2).unwrap();
            assert!(prop2.total_property == 20.0);
            assert!(prop2.unreserved == 10.0);
            //assert!(prop2.reserved == 0.0);
            assert!(prop2.want_reserve == 10.0);
            assert!(prop2.class_reserve == 0.0);
            assert!(prop2.product_reserve == 5.0);
            let prop3 = test.property.get(&3).unwrap();
            assert!(prop3.total_property == 15.0);
            assert!(prop3.unreserved == 15.0);
            //assert!(prop3.reserved == 0.0);
            assert!(prop3.want_reserve == 0.0);
            assert!(prop3.class_reserve == 0.0);
            assert!(prop3.product_reserve == 0.0);
            assert_eq!(result.tier, 25);
            assert!(146.0 < result.value);
            assert!(result.value < 147.0);
        }

        #[test]
        pub fn shift_want_desires_correctly() {
            // data needed, but not set up for this test.
            let mut data = DataManager::new();
            // wants 0
            let mut want0 = Want{
                id: 0,
                name: "".to_string(),
                description: "".to_string(),
                decay: 0.0,
                ownership_sources: HashSet::new(),
                process_sources: HashSet::new(),
                use_sources: HashSet::new(),
                consumption_sources: HashSet::new(),
            };
            want0.ownership_sources.insert(0);
            want0.process_sources.insert(0);
            want0.process_sources.insert(1);
            want0.use_sources.insert(0);
            want0.consumption_sources.insert(1);
            data.wants.insert(0, want0);
            // products
            data.products.insert(0, Product{
                id: 0,
                name: "".to_string(),
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
            data.products.get_mut(&0).unwrap()
            .wants.insert(0, 1.0);
            data.products.insert(1, Product{
                id: 1,
                name: "".to_string(),
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
            let mut product2 = Product{
                id: 2,
                name: "".to_string(),
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
            };
            product2.use_processes.insert(0);
            product2.consumption_processes.insert(1);
            data.products.insert(2, product2);
            data.products.insert(3, Product{
                id: 3,
                name: "".to_string(),
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
            // products use 1 + 2 = want 0
            data.processes.insert(0, Process{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
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
                        part: ProcessSectionTag::Capital
                    },
                    ProcessPart { 
                        item: Item::Want(0), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output
                    }
                ],
                process_tags: vec![
                    ProcessTag::Use(2)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });
            // products consume 2 + 3 = want 0
            data.processes.insert(1, Process{
                id: 1,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart { 
                        item: Item::Product(2), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input
                    },
                    ProcessPart { 
                        item: Item::Product(3), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input
                    },
                    ProcessPart { 
                        item: Item::Want(0), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output
                    }
                ],
                process_tags: vec![
                    ProcessTag::Consumption(2)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });

            data.update_product_classes().expect("Could not function");
            let test_desires = vec![
                Desire::new(Item::Want(0),
                    1,
                    None,
                    1.0,
                    0.0,
                    1,
                    vec![]).unwrap(),
                Desire::new(Item::Want(0),
                    3,
                    Some(12),
                    1.0,
                    0.0,
                    3,
                    vec![]).unwrap(),
            ];
            let mut test = Property::new(test_desires);
            test.property.insert(0, PropertyInfo::new(15.0));
            test.property.insert(1, PropertyInfo::new(10.0));
            test.property.insert(2, PropertyInfo::new(20.0));
            test.property.insert(3, PropertyInfo::new(15.0));
            let coord = DesireCoord { tier: 25, idx: 10};
            let result = test.sift_up_to(&coord, &data);
            // check that the sitfing was done correctly.
            // 26.0 into desire 0, (tier 100)
            let desire0 = test.desires.get(0).unwrap();
            assert_eq!(desire0.satisfaction_up_to_tier().unwrap(), 25);
            assert!(desire0.satisfaction == 25.0);
            // 4.0 into desire 1 (tier 12, totally satisfied)
            let desire1 = test.desires.get(1).unwrap();
            assert!(desire1.is_fully_satisfied());
            assert_eq!(desire1.satisfaction_up_to_tier().unwrap(), 12);
            assert!(desire1.satisfaction == 4.0);
            // and check that items were reserved correctly.
            let prop0 = test.property.get(&0).unwrap();
            assert!(prop0.total_property == 15.0);
            assert!(prop0.unreserved == 0.0);
            //assert!(prop0.reserved == 0.0);
            assert!(prop0.want_reserve == 15.0);
            assert!(prop0.class_reserve == 0.0);
            assert!(prop0.product_reserve == 0.0);
            let prop1 = test.property.get(&1).unwrap();
            assert!(prop1.total_property == 10.0);
            assert!(prop1.unreserved == 0.0);
            //assert!(prop1.reserved == 0.0);
            assert!(prop1.want_reserve == 10.0);
            assert!(prop1.class_reserve == 0.0);
            assert!(prop1.product_reserve == 0.0);
            let prop2 = test.property.get(&2).unwrap();
            assert!(prop2.total_property == 20.0);
            assert!(prop2.unreserved == 6.0);
            //assert!(prop2.reserved == 0.0);
            assert!(prop2.want_reserve == 14.0);
            assert!(prop2.class_reserve == 0.0);
            assert!(prop2.product_reserve == 0.0);
            let prop3 = test.property.get(&3).unwrap();
            assert!(prop3.total_property == 15.0);
            assert!(prop3.unreserved == 11.0);
            //assert!(prop3.reserved == 0.0);
            assert!(prop3.want_reserve == 4.0);
            assert!(prop3.class_reserve == 0.0);
            assert!(prop3.product_reserve == 0.0);
            assert_eq!(result.tier, 25);
            assert!(143.0 < result.value);
            assert!(result.value < 144.0);
        }

        /// sift class desires only, don't even set up specfic
        /// or want desires.
        #[test]
        pub fn sift_class_desires_correctly() {
            // data needed, but not set up for this test.
            let mut data = DataManager::new();
            data.products.insert(0, Product{
                id: 0,
                name: "".to_string(),
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
            data.products.insert(1, Product{
                id: 1,
                name: "".to_string(),
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
            data.products.insert(2, Product{
                id: 2,
                name: "".to_string(),
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
            data.update_product_classes().expect("Could not function");
            let test_desires = vec![
                Desire::new(Item::Class(0),
                    0,
                    None,
                    1.0,
                    0.0,
                    4,
                    vec![]).unwrap(),
                Desire::new(Item::Class(0),
                    3,
                    Some(12),
                    1.0,
                    0.0,
                    3,
                    vec![]).unwrap(),
            ];
            let mut test = Property::new(test_desires);
            test.property.insert(0, PropertyInfo::new(15.0));
            test.property.insert(1, PropertyInfo::new(15.0));
            test.property.insert(2, PropertyInfo::new(10.0));
            let coord = DesireCoord { tier: 25, idx: 10};
            let result = test.sift_up_to(&coord, &data);
            // check that the sitfing was done correctly.
            // 26.0 into desire 0, (tier 100)
            let desire0 = test.desires.get(0).unwrap();
            assert_eq!(desire0.satisfaction_up_to_tier().unwrap(), 24);
            assert_eq!(desire0.satisfaction, 7.0);
            // 4.0 into desire 1 (tier 12, totally satisfied)
            let desire1 = test.desires.get(1).unwrap();
            assert!(desire1.is_fully_satisfied());
            assert_eq!(desire1.satisfaction_up_to_tier().unwrap(), 12);
            assert_eq!(desire1.satisfaction, 4.0);
            // and check that items were reserved correctly.
            // touched 1st product
            let prop0 = test.property.get(&0).unwrap();
            assert_eq!(prop0.total_property, 15.0);
            assert_eq!(prop0.unreserved, 4.0);
            //assert_eq!(prop0.reserved, 0.0);
            assert_eq!(prop0.want_reserve, 0.0);
            assert_eq!(prop0.class_reserve, 11.0);
            assert_eq!(prop0.product_reserve, 0.0);
            // touched 2nd product
            let prop1 = test.property.get(&1).unwrap();
            assert_eq!(prop1.total_property, 15.0);
            assert_eq!(prop1.unreserved, 15.0);
            //assert_eq!(prop1.reserved, 0.0);
            assert_eq!(prop1.want_reserve, 0.0);
            assert_eq!(prop1.class_reserve, 0.0);
            assert_eq!(prop1.product_reserve, 0.0);
            // untouched 3rd product
            let prop2 = test.property.get(&2).unwrap();
            assert_eq!(prop2.total_property, 10.0);
            assert_eq!(prop2.unreserved, 10.0);
            //assert_eq!(prop2.reserved, 0.0);
            assert_eq!(prop2.want_reserve, 0.0);
            assert_eq!(prop2.class_reserve, 0.0);
            assert_eq!(prop2.product_reserve, 0.0);
            assert_eq!(result.tier, 24);
            assert!(58.0 < result.value);
            assert!(result.value < 59.0);
        }

        /// Sifts 1 Product desire only, don't set up wants
        /// or class desires.
        #[test]
        pub fn sift_product_desire_correctly() {
            // data needed, but not set up for this test.
            let data = &DataManager::new();
            let test_desires = vec![
                Desire::new(Item::Product(0),
                    0,
                    Some(100),
                    1.0,
                    0.0,
                    5,
                    vec![]).unwrap(),
                Desire::new(Item::Product(0),
                    3,
                    Some(12),
                    1.0,
                    0.0,
                    3,
                    vec![]).unwrap(),
            ];
            let mut test = Property::new(test_desires);
            test.property.insert(0, PropertyInfo::new(15.0));
            test.property.insert(1, PropertyInfo::new(10.0));
            let coord = DesireCoord { tier: 25, idx: 10};
            let result = test.sift_up_to(&coord, &data);
            // check that the sitfing was done correctly.
            // 11.0 into desire 0, (tier 50)
            let desire0 = test.desires.get(0).unwrap();
            assert_eq!(desire0.satisfaction_up_to_tier().unwrap(), 25);
            assert!(desire0.satisfaction == 6.0);
            // 4.0 into desire 1 (tier 12, totally satisfied)
            let desire1 = test.desires.get(1).unwrap();
            assert!(desire1.is_fully_satisfied());
            assert_eq!(desire1.satisfaction_up_to_tier().unwrap(), 12);
            assert!(desire1.satisfaction == 4.0);
            // and check that items were reserved correctly.
            let prop0 = test.property.get(&0).unwrap();
            assert!(prop0.total_property == 15.0);
            assert!(prop0.unreserved == 5.0);
            //assert!(prop0.reserved == 0.0);
            assert!(prop0.want_reserve == 0.0);
            assert!(prop0.class_reserve == 0.0);
            assert!(prop0.product_reserve == 10.0);
            let prop1 = test.property.get(&1).unwrap();
            assert!(prop1.total_property == 10.0);
            assert!(prop1.unreserved == 10.0);
            //assert!(prop1.reserved == 0.0);
            assert!(prop1.want_reserve == 0.0);
            assert!(prop1.class_reserve == 0.0);
            assert!(prop1.product_reserve == 0.0);
            assert_eq!(result.tier, 25);
            assert!(59.0 < result.value);
            assert!(result.value < 60.0);
        }
    
        #[test]
        pub fn sift_desires_upto_mid_tier_selection() {
            // data needed, but not set up for this test.
            let mut data = DataManager::new();
            // wants 0
            let mut want0 = Want{
                id: 0,
                name: "".to_string(),
                description: "".to_string(),
                decay: 0.0,
                ownership_sources: HashSet::new(),
                process_sources: HashSet::new(),
                use_sources: HashSet::new(),
                consumption_sources: HashSet::new(),
            };
            want0.ownership_sources.insert(0);
            want0.process_sources.insert(0);
            want0.process_sources.insert(1);
            want0.use_sources.insert(0);
            want0.consumption_sources.insert(1);
            data.wants.insert(0, want0);
            // products
            data.products.insert(0, Product{
                id: 0,
                name: "".to_string(),
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
            data.products.get_mut(&0).unwrap()
            .wants.insert(0, 1.0);
            data.products.insert(1, Product{
                id: 1,
                name: "".to_string(),
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
            let mut product2 = Product{
                id: 2,
                name: "".to_string(),
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
            };
            product2.use_processes.insert(0);
            product2.consumption_processes.insert(1);
            data.products.insert(2, product2);
            data.products.insert(3, Product{
                id: 3,
                name: "".to_string(),
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
            // products use 1 + 2 = want 0
            data.processes.insert(0, Process{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
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
                        part: ProcessSectionTag::Capital
                    },
                    ProcessPart { 
                        item: Item::Want(0), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output
                    }
                ],
                process_tags: vec![
                    ProcessTag::Use(2)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });
            // products consume 2 + 3 = want 0
            data.processes.insert(1, Process{
                id: 1,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart { 
                        item: Item::Product(2), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input
                    },
                    ProcessPart { 
                        item: Item::Product(3), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input
                    },
                    ProcessPart { 
                        item: Item::Want(0), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output
                    }
                ],
                process_tags: vec![
                    ProcessTag::Consumption(2)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });

            data.update_product_classes().expect("Could not function");
            let test_desires = vec![
                Desire::new(Item::Want(0),
                    1,
                    None,
                    1.0,
                    0.0,
                    1,
                    vec![]).unwrap(),
                Desire::new(Item::Class(0),
                    3,
                    Some(30),
                    1.0,
                    0.0,
                    3,
                    vec![]).unwrap(),
                Desire::new(Item::Product(0),
                    5,
                    Some(10),
                    1.0,
                    0.0,
                    5,
                    vec![]).unwrap()
            ];
            let mut test = Property::new(test_desires);
            test.property.insert(0, PropertyInfo::new(15.0));
            test.property.insert(1, PropertyInfo::new(10.0));
            test.property.insert(2, PropertyInfo::new(20.0));
            test.property.insert(3, PropertyInfo::new(15.0));
            let coord = DesireCoord { tier: 25, idx: 0};
            let result = test.sift_up_to(&coord, &data);
            // check that the sitfing was done correctly.
            // 26.0 into desire 0, (tier 100)
            let desire0 = test.desires.get(0).unwrap();
            assert_eq!(desire0.satisfaction_up_to_tier().unwrap(), 24);
            assert!(desire0.satisfaction == 24.0);
            // 4.0 into desire 1 (tier 12, totally satisfied)
            let desire1 = test.desires.get(1).unwrap();
            assert_eq!(desire1.satisfaction_up_to_tier().unwrap(), 24);
            assert!(desire1.satisfaction == 8.0);
            // 4.0 into desire 1 (tier 12, totally satisfied)
            let desire2 = test.desires.get(2).unwrap();
            assert!(desire2.is_fully_satisfied());
            assert_eq!(desire2.satisfaction_up_to_tier().unwrap(), 10);
            assert!(desire2.satisfaction == 2.0);
            // and check that items were reserved correctly.
            let prop0 = test.property.get(&0).unwrap();
            assert!(prop0.total_property == 15.0);
            assert!(prop0.unreserved == 0.0);
            //assert!(prop0.reserved == 0.0);
            assert!(prop0.want_reserve == 15.0);
            // assert!(prop0.class_reserve == 0.0); both are in the same class, so either is valid, selection order cannot be guaranteed (yet).
            assert!(prop0.product_reserve == 2.0);
            let prop1 = test.property.get(&1).unwrap();
            assert!(prop1.total_property == 10.0);
            assert!(prop1.unreserved == 1.0);
            //assert!(prop1.reserved == 0.0);
            assert!(prop1.want_reserve == 9.0);
            assert!((prop1.class_reserve + prop0.class_reserve) == 8.0);
            assert!(prop1.product_reserve == 0.0);
            let prop2 = test.property.get(&2).unwrap();
            assert!(prop2.total_property == 20.0);
            assert!(prop2.unreserved == 11.0);
            //assert!(prop2.reserved == 0.0);
            assert!(prop2.want_reserve == 9.0);
            assert!(prop2.class_reserve == 0.0);
            assert!(prop2.product_reserve == 0.0);
            let prop3 = test.property.get(&3).unwrap();
            assert!(prop3.total_property == 15.0);
            assert!(prop3.unreserved == 15.0);
            //assert!(prop3.reserved == 0.0);
            assert!(prop3.want_reserve == 0.0);
            assert!(prop3.class_reserve == 0.0);
            assert!(prop3.product_reserve == 0.0);
            // ensure the tiered value is correct to match
            assert_eq!(result.tier, 24);
            assert!(146.0 < result.value);
            assert!(result.value < 147.0);
        }
    }

    mod release_desire_at_should {
        use std::collections::{VecDeque, HashMap};
        use super::*;

        /// Makes a pop for testing. The pop will have the following info
        /// 
        /// 20 pops total --
        /// 20 pops of the same speciecs
        /// - Desires
        ///   - 20 Food 0/1/2/3/4
        ///   - 20 Shelter 7/9/11/13
        ///   - 20 Clothing 2/4/6/8
        /// 10 with a culture
        /// - Desires
        ///   - 10 Ambrosia Fruit 10/15/20/25/30
        ///   - 10 Cotton Clothes 15/25/35 ...
        /// 10 with an ideology
        /// - Desires
        ///   - 10 Hut 30
        ///   - 10 Cabin 50
        pub fn make_test_pop() -> Pop {
            let mut test = Pop { 
                id: 10, 
                job: 0, 
                firm: 0, 
                market: 0, 
                property: Property::new(vec![]), 
                breakdown_table: PopBreakdownTable{ table: vec![], total: 0 }, 
                is_selling: true,
                current_sat: TieredValue { tier: 0, value: 0.0 },
                prev_sat: TieredValue { tier: 0, value: 0.0 },
                hypo_change: TieredValue { tier: 0, value: 0.0 },
                backlog: VecDeque::new()};

            let species_desire_1 = Desire { 
                item: Item::Want(2), // food
                start: 0, 
                end: Some(4), 
                amount: 1.0, 
                satisfaction: 0.0, 
                step: 1, 
                tags: vec![] };
            let species_desire_2 = Desire { 
                item: Item::Want(3), // shelter
                start: 7, 
                end: Some(13), 
                amount: 1.0, 
                satisfaction: 0.0, 
                step: 2, 
                tags: vec![] };
            let species_desire_3 = Desire { 
                item: Item::Want(4), //clothing
                start: 2, 
                end: Some(8), 
                amount: 1.0, 
                satisfaction: 0.0, 
                step: 2, 
                tags: vec![] };

            let culture_desire_1 = Desire { 
                item: Item::Product(2), // ambrosia fruit
                start: 10, 
                end: Some(30), 
                amount: 1.0, 
                satisfaction: 0.0, 
                step: 5, 
                tags: vec![] };
            let culture_desire_2 = Desire { 
                item: Item::Product(6), // clothes
                start: 15, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0, 
                step: 10, 
                tags: vec![] };

            let ideology_desire_1 = Desire { 
                item: Item::Product(14), // Hut
                start: 30, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0, 
                step: 0, 
                tags: vec![] };
            let ideology_desire_2 = Desire { 
                item: Item::Product(15), // Cabin
                start: 50, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0, 
                step: 0, 
                tags: vec![] };

            let species = Species::new(0,
                "Species".into(),
                "".into(),
                vec![species_desire_1, species_desire_2, species_desire_3],
                vec![], vec![], 
                1.0, 0.03,
                0.02).expect("Messed up new.");

            let culture = Culture::new(0,
                "Culture".into(),
                "".into(),
                1.0, 0.01,
                0.01,
                vec![culture_desire_1, culture_desire_2],
                vec![]).expect("Messed up new.");

            let ideology = Ideology::new(0,
                "Ideology".into(),
                "".into(),
                0.0, 0.0,
                1.0,
                vec![ideology_desire_1, ideology_desire_2],
                vec![]).expect("Messed up new.");

            let mut demos = Demographics{ species: HashMap::new(),
                cultures: HashMap::new(), 
                ideology: HashMap::new() };

            demos.species.insert(species.id, species);
            demos.cultures.insert(culture.id, culture);
            demos.ideology.insert(ideology.id, ideology);

            test.breakdown_table.insert_pops(
                PBRow{ species: 0, 
                    species_cohort: None,
                    species_subtype: None,
                    culture: None,
                    culture_generation: None,
                    culture_class: None,
                    ideology: None, 
                    ideology_wave: None, 
                    ideology_faction: None, 
                    count: 5 }
            );
            test.breakdown_table.insert_pops(
                PBRow{ species: 0, 
                    species_cohort: None,
                    species_subtype: None,
                    culture: Some(0),
                    culture_generation: None,
                    culture_class: None,
                    ideology: None, 
                    ideology_wave: None, 
                    ideology_faction: None, 
                    count: 5 }
            );
            test.breakdown_table.insert_pops(
                PBRow{ species: 0, 
                    species_cohort: None,
                    species_subtype: None,
                    culture: None,
                    culture_generation: None,
                    culture_class: None,
                    ideology: Some(0), 
                    ideology_wave: None, 
                    ideology_faction: None, 
                    count: 5 }
            );
            test.breakdown_table.insert_pops(
                PBRow{ species: 0, 
                    species_cohort: None,
                    species_subtype: None,
                    culture: Some(0),
                    culture_generation: None,
                    culture_class: None,
                    ideology: Some(0), 
                    ideology_wave: None, 
                    ideology_faction: None, 
                    count: 5 }
            );

            test.update_desires(demos);

            test
        }

        /// preps a pop's property, the property's data, and market prices of those items.
        /// 
        /// Sets all values to 1.0 amv and salability of 0.5 by default.
        /// 
        /// Exceptions are:
        /// - Ambrosia Fruit are set as a currency (Sal 1.0, currency=true)
        /// - Cotton Clothes are priced at 10.0 amv.
        /// - Cotton Suit is priced at 20.0 amv.
        /// - Hut has a price of 100.0 amv.
        /// - Cabin has a price of 1000.0 amv.
        /// 
        /// This is for testing buy and sell functions, not offer_calculations.
        pub fn prepare_data_for_market_actions(_pop: &mut Pop) -> (DataManager, MarketHistory) {
            let mut data = DataManager::new();
            // TODO update this when we update Load All
            data.load_test_data().expect("Error on load?");
            let product = data.products.get_mut(&6).unwrap();
            product.fractional = true;

            let mut market = MarketHistory {
                product_info: HashMap::new(),
                sale_priority: vec![],
                currencies: vec![],
                class_info: HashMap::new(),
                want_info: HashMap::new(),
            };
            // quickly set all prices to 1.0 for ease going forward.
            for idx in 0..26 {
                market.product_info.insert(idx, ProductInfo {
                    available: 0.0,
                    price: 1.0,
                    offered: 0.0,
                    sold: 0.0,
                    salability: 0.5,
                    is_currency: false,
                });
            }
            // ambrosia fruit
            market.product_info.get_mut(&2).expect("Brok").salability = 1.0;
            market.product_info.get_mut(&2).expect("Brok").is_currency = true;

            market.product_info.get_mut(&6).expect("Brok").price = 10.0;
            market.product_info.get_mut(&7).expect("Brok").price = 20.0;

            market.product_info.get_mut(&14).expect("Brok").price = 100.0;
            market.product_info.get_mut(&15).expect("Brok").price = 1000.0;

            market.currencies.push(2);
            // sale priority would go here if used.

            // pop.property.property.insert(6, PropertyInfo::new(10.0));
            // TODO fix this info.

            (data, market)
        }

        #[test]
        pub fn release_selected_desire_and_return_deterministically() {
            let mut test = make_test_pop();
            let (data, mut history) = prepare_data_for_market_actions(&mut test);
            let mut test = test.property;
            // swap out infinite clothes for the class desire instead.
            test.desires.get_mut(4).unwrap()
                .item = Item::Class(6);
            // add in pop's property and sift their desires.
            // we have 20 extra food than we need (20*5=100.0 units)
            // this covers all food and leave excess for trading
            // This covers both species food desire and culture ambrosia fruit desire.
            test.add_property(2, 120.0, &data);
            // they have all the shelter they need via huts
            // 4 * 20 units
            // this covers both the shelter desire and the hut desire
            test.add_property(14, 80.0, &data);
            // the have all clothing needs (2-8) covered with 80 units
            // clothing culture desire is covered up to tier 85. (30 more than cabin at tier 50)
            // 20 * 5 units
            // they have 20.0 extra units available to trade.
            test.add_property(6, 100.0, &data);
            // Also add in some suits to test out class swapping and ensure
            // class desires are correctly fulfilled and taken from as needed.
            test.add_property(7, 10.0, &data);
            // The pop is trying to buy 10 cabins at tier 50
            // It should always include the 20.0 units of Ambrosia fruit, 
            // which they have in excess.
            // They should also include 20.0 sets of clothes, which are available in excess.
            // Anything else offered would be 
            // set the prices of ambrosia fruit, clothes, and cabins so that the cabin is just purchaseable with
            // 20 ambrosia fruit and 20 cotton clothes.
            history.product_info.get_mut(&2).unwrap().price = 1.0; // 20.0 total
            history.product_info.get_mut(&6).unwrap().price = 2.0; // 40.0 total
            history.product_info.get_mut(&7).unwrap().price = 10.0; // 0.0 likely to exchange.
            history.product_info.get_mut(&15).unwrap().price = 5.9; // 59.0 total

            // Try to release the highest desire tier, at tier 105, 
            // the class desire for clothes.
            // TODO Pick up here for testing!!!!!!
            let result = test
                .release_desire_at(&DesireCoord { tier: 105, idx: 4 }, 
                    &history, &data);
            assert_eq!(result[&6], 10.0);
        }

    }

    // TODO Get Shopping Time will likely be reworked into a generic get X product, which will get and produce a generic item. This will likely also have a get X want partner.
    mod get_shopping_time_should {
        use std::collections::{HashMap, HashSet};
        use super::*;

        #[test]
        pub fn exit_early_when_product_already_exists() {
            let mut data = DataManager::new();
            // wants 0
            let mut want0 = Want {
                id: 0,
                name: "Leisure".to_string(),
                description: "".to_string(),
                decay: 0.0,
                ownership_sources: HashSet::new(),
                process_sources: HashSet::new(),
                use_sources: HashSet::new(),
                consumption_sources: HashSet::new(),
            };
            want0.process_sources.insert(0);
            want0.use_sources.insert(0);
            data.wants.insert(0, want0);
            // products
            // time
            data.products.insert(0, Product{
                id: 0,
                name: "Time".to_string(),
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
            data.products.insert(1, Product{
                id: 1,
                name: "Shopping Time".to_string(),
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
            // products use 0 + 1 = want 0
            data.processes.insert(0, Process{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart { 
                        item: Item::Product(0), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input
                    },
                    ProcessPart { 
                        item: Item::Product(1), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input
                    },
                    ProcessPart { 
                        item: Item::Want(0), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output
                    }
                ],
                process_tags: vec![
                    ProcessTag::Use(1)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });
            let market = MarketHistory {
                product_info: HashMap::new(),
                sale_priority: vec!(),
                currencies: vec!(),
                class_info: HashMap::new(),
                want_info: HashMap::new(),
            };

            data.update_product_classes().expect("Could not function");
            let test_desires = vec![
                Desire::new(Item::Want(0),
                    1,
                    None,
                    1.0,
                    0.0,
                    1,
                    vec![]).unwrap(),
            ];
            let mut test = Property::new(test_desires);
            // add the products to our property.
            test.property.insert(0, PropertyInfo::new(3.0));
            test.property.insert(1, PropertyInfo::new(5.0));
            // get total result and see if it gets out with existing property.
            let result1 = test.get_shopping_time(2.5, &data, 
                &market, None);
            assert_eq!(result1, 2.5); // should get back our full target
            assert_eq!(test.property.get(&1)
                .unwrap().total_property, 2.5); // should still have 2.5 remaining
            // and sanity check that the other is untouched
            assert_eq!(test.property.get(&0)
                .unwrap().total_property, 3.0);
            
            // if it was remove, do it again, testing that it only returns what is
            // available.
            let capped_result = test.get_shopping_time(5.0, &data, &market, None);
            assert_eq!(capped_result, 2.5); // should get back our what is available, nothing more.
            assert_eq!(test.property.get(&1)
                .unwrap().total_property, 0.0); // should still have 2.5 remaining
            // and sanity check that the other is untouched
            assert_eq!(test.property.get(&0)
                .unwrap().total_property, 3.0);
        }

        #[test]
        pub fn do_processes_to_generate_more_product() {
            let mut data = DataManager::new();
            // wants 0
            let mut want0 = Want{
                id: 0,
                name: "Leisure".to_string(),
                description: "".to_string(),
                decay: 0.0,
                ownership_sources: HashSet::new(),
                process_sources: HashSet::new(),
                use_sources: HashSet::new(),
                consumption_sources: HashSet::new(),
            };
            want0.process_sources.insert(0);
            want0.use_sources.insert(0);
            data.wants.insert(0, want0);
            // products
            // time
            data.products.insert(0, Product{
                id: 0,
                name: "Time".to_string(),
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
            // shopping time.
            data.products.insert(1, Product{
                id: 1,
                name: "Shopping Time".to_string(),
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
            // products use 0 + 1 = product 1
            data.processes.insert(0, Process{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart { 
                        item: Item::Product(0), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input
                    },
                    ProcessPart { 
                        item: Item::Product(1), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output
                    }
                ],
                process_tags: vec![
                    ProcessTag::Use(0)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });

            data.products.get_mut(&0).unwrap()
                .processes.insert(0);
            data.products.get_mut(&1).unwrap()
                .processes.insert(0);
            let market = MarketHistory {
                product_info: HashMap::new(),
                sale_priority: vec!(),
                currencies: vec!(),
                class_info: HashMap::new(),
                want_info: HashMap::new(),
            };

            data.update_product_classes().expect("Could not function");
            let test_desires = vec![
                Desire::new(Item::Product(0),
                    1,
                    None,
                    1.0,
                    0.0,
                    1,
                    vec![]).unwrap(),
            ];
            let mut test = Property::new(test_desires);
            // add the products to our property.
            test.property.insert(0, PropertyInfo::new(5.0));
            // lock up 3 P0 into desires.
            test.sift_up_to(&DesireCoord { tier: 2, idx: 1 }, &data);
            //test.property.insert(1, PropertyInfo::new(5.0));
            // get total result and see if it gets out with existing property.
            let result1 = test.get_shopping_time(2.0, &data, 
                &market, None);
            assert_eq!(result1, 2.0); // should get back our full target
            // of the 5 started, 2 are sifted, 2 is consumed for shopping time.
            assert_eq!(test.property[&0].total_property, 3.0);
            
            // do it again, overdrawing and being limited correctly.
            let capped_result = test.get_shopping_time(5.0, &data, &market, None);
            assert_eq!(capped_result, 1.0); // should get back our what is available, nothing more.
            // 2 were reserved, so 2 should remain.
            assert_eq!(test.property[&0].total_property, 2.0);
        }
    }

    mod sift_all_should {
        // TODO update test for lookahead when added.
        use std::collections::{HashMap, HashSet};
        use super::*;

        #[test]
        pub fn shift_want_class_and_product_desires_correctly() {
            // data needed, but not set up for this test.
            let mut data = DataManager::new();
            // wants 0
            let mut want0 = Want{
                id: 0,
                name: "".to_string(),
                description: "".to_string(),
                decay: 0.0,
                ownership_sources: HashSet::new(),
                process_sources: HashSet::new(),
                use_sources: HashSet::new(),
                consumption_sources: HashSet::new(),
            };
            want0.ownership_sources.insert(0);
            want0.process_sources.insert(0);
            want0.process_sources.insert(1);
            want0.use_sources.insert(0);
            want0.consumption_sources.insert(1);
            data.wants.insert(0, want0);
            // products
            data.products.insert(0, Product{
                id: 0,
                name: "".to_string(),
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
            data.products.get_mut(&0).unwrap()
            .wants.insert(0, 1.0);
            data.products.insert(1, Product{
                id: 1,
                name: "".to_string(),
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
            let mut product2 = Product{
                id: 2,
                name: "".to_string(),
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
            };
            product2.use_processes.insert(0);
            product2.consumption_processes.insert(1);
            data.products.insert(2, product2);
            data.products.insert(3, Product{
                id: 3,
                name: "".to_string(),
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
            // products use 1 + 2 = want 0
            data.processes.insert(0, Process{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
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
                        part: ProcessSectionTag::Capital
                    },
                    ProcessPart { 
                        item: Item::Want(0), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output
                    }
                ],
                process_tags: vec![
                    ProcessTag::Use(2)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });
            // products consume 2 + 3 = want 0
            data.processes.insert(1, Process{
                id: 1,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart { 
                        item: Item::Product(2), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input
                    },
                    ProcessPart { 
                        item: Item::Product(3), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input
                    },
                    ProcessPart { 
                        item: Item::Want(0), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output
                    }
                ],
                process_tags: vec![
                    ProcessTag::Consumption(2)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });

            data.update_product_classes().expect("Could not function");
            let test_desires = vec![
                Desire::new(Item::Want(0),
                    1,
                    None,
                    1.0,
                    0.0,
                    1,
                    vec![]).unwrap(),
                Desire::new(Item::Class(0),
                    3,
                    Some(30),
                    1.0,
                    0.0,
                    3,
                    vec![]).unwrap(),
                Desire::new(Item::Product(0),
                    5,
                    Some(10),
                    1.0,
                    0.0,
                    5,
                    vec![]).unwrap()
            ];
            let mut test = Property::new(test_desires);
            test.property.insert(0, PropertyInfo::new(15.0));
            test.property.insert(1, PropertyInfo::new(10.0));
            test.property.insert(2, PropertyInfo::new(20.0));
            test.property.insert(3, PropertyInfo::new(15.0));
            let result = test.sift_all(&data);
            // check that the sitfing was done correctly.
            // 26.0 into desire 0, (tier 100)
            let desire0 = test.desires.get(0).unwrap();
            assert_eq!(desire0.satisfaction_up_to_tier().unwrap(), 35);
            assert!(desire0.satisfaction == 35.0);
            // 4.0 into desire 1 (tier 12, totally satisfied)
            let desire1 = test.desires.get(1).unwrap();
            assert!(desire1.is_fully_satisfied());
            assert_eq!(desire1.satisfaction_up_to_tier().unwrap(), 30);
            assert!(desire1.satisfaction == 10.0);
            // 4.0 into desire 1 (tier 12, totally satisfied)
            let desire2 = test.desires.get(2).unwrap();
            assert!(desire2.is_fully_satisfied());
            assert_eq!(desire2.satisfaction_up_to_tier().unwrap(), 10);
            assert!(desire2.satisfaction == 2.0);
            // and check that items were reserved correctly.
            let prop0 = test.property.get(&0).unwrap();
            assert!(prop0.total_property == 15.0);
            assert!(prop0.unreserved == 0.0);
            //assert!(prop0.reserved == 0.0);
            assert!(prop0.want_reserve == 15.0);
            // assert!(prop0.class_reserve == 0.0); both are in the same class, so either is valid, selection order cannot be guaranteed (yet).
            assert!(prop0.product_reserve == 2.0);
            let prop1 = test.property.get(&1).unwrap();
            assert!(prop1.total_property == 10.0);
            assert!(prop1.unreserved == 0.0);
            //assert!(prop1.reserved == 0.0);
            assert!(prop1.want_reserve == 10.0);
            assert!((prop1.class_reserve + prop0.class_reserve) == 10.0);
            assert!(prop1.product_reserve == 0.0);
            let prop2 = test.property.get(&2).unwrap();
            assert!(prop2.total_property == 20.0);
            assert!(prop2.unreserved == 0.0);
            //assert!(prop2.reserved == 0.0);
            assert!(prop2.want_reserve == 20.0);
            assert!(prop2.class_reserve == 0.0);
            assert!(prop2.product_reserve == 0.0);
            let prop3 = test.property.get(&3).unwrap();
            assert!(prop3.total_property == 15.0);
            assert!(prop3.unreserved == 5.0);
            //assert!(prop3.reserved == 0.0);
            assert!(prop3.want_reserve == 10.0);
            assert!(prop3.class_reserve == 0.0);
            assert!(prop3.product_reserve == 0.0);
            // ensure want process plan is recorded correctly.
            assert_eq!(test.process_plan[&0], 10.0);
            assert_eq!(test.process_plan[&1], 10.0);
            // ensure the tiered value is correct to match
            assert_eq!(result.tier, 35);
            assert!(490.0 < result.value);
            assert!(result.value < 491.0);
        }

        #[test]
        pub fn shift_want_and_class_desires_correctly() {
            // data needed, but not set up for this test.
            let mut data = DataManager::new();
            // wants 0
            let mut want0 = Want{
                id: 0,
                name: "".to_string(),
                description: "".to_string(),
                decay: 0.0,
                ownership_sources: HashSet::new(),
                process_sources: HashSet::new(),
                use_sources: HashSet::new(),
                consumption_sources: HashSet::new(),
            };
            want0.ownership_sources.insert(0);
            want0.process_sources.insert(0);
            want0.process_sources.insert(1);
            want0.use_sources.insert(0);
            want0.consumption_sources.insert(1);
            data.wants.insert(0, want0);
            // products
            data.products.insert(0, Product{
                id: 0,
                name: "".to_string(),
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
            data.products.get_mut(&0).unwrap()
            .wants.insert(0, 1.0);
            data.products.insert(1, Product{
                id: 1,
                name: "".to_string(),
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
            let mut product2 = Product{
                id: 2,
                name: "".to_string(),
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
            };
            product2.use_processes.insert(0);
            product2.consumption_processes.insert(1);
            data.products.insert(2, product2);
            data.products.insert(3, Product{
                id: 3,
                name: "".to_string(),
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
            // products use 1 + 2 = want 0
            data.processes.insert(0, Process{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
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
                        part: ProcessSectionTag::Capital
                    },
                    ProcessPart { 
                        item: Item::Want(0), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output
                    }
                ],
                process_tags: vec![
                    ProcessTag::Use(2)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });
            // products consume 2 + 3 = want 0
            data.processes.insert(1, Process{
                id: 1,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart { 
                        item: Item::Product(2), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input
                    },
                    ProcessPart { 
                        item: Item::Product(3), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input
                    },
                    ProcessPart { 
                        item: Item::Want(0), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output
                    }
                ],
                process_tags: vec![
                    ProcessTag::Consumption(2)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });

            data.update_product_classes().expect("Could not function");
            let test_desires = vec![
                Desire::new(Item::Want(0),
                    1,
                    None,
                    1.0,
                    0.0,
                    1,
                    vec![]).unwrap(),
                Desire::new(Item::Class(0),
                    3,
                    Some(30),
                    1.0,
                    0.0,
                    3,
                    vec![]).unwrap(),
            ];
            let mut test = Property::new(test_desires);
            test.property.insert(0, PropertyInfo::new(15.0));
            test.property.insert(1, PropertyInfo::new(10.0));
            test.property.insert(2, PropertyInfo::new(20.0));
            test.property.insert(3, PropertyInfo::new(15.0));
            let result = test.sift_all(&data);
            // check that the sitfing was done correctly.
            // 26.0 into desire 0, (tier 100)
            let desire0 = test.desires.get(0).unwrap();
            assert_eq!(desire0.satisfaction_up_to_tier().unwrap(), 35);
            assert!(desire0.satisfaction == 35.0);
            // 4.0 into desire 1 (tier 12, totally satisfied)
            let desire1 = test.desires.get(1).unwrap();
            assert!(desire1.is_fully_satisfied());
            assert_eq!(desire1.satisfaction_up_to_tier().unwrap(), 30);
            assert!(desire1.satisfaction == 10.0);
            // and check that items were reserved correctly.
            let prop0 = test.property.get(&0).unwrap();
            assert!(prop0.total_property == 15.0);
            assert!(prop0.unreserved == 0.0);
            //assert!(prop0.reserved == 0.0);
            assert!(prop0.want_reserve == 15.0);
            // assert!(prop0.class_reserve == 0.0); both are in the same class, so either is valid, selection order cannot be guaranteed (yet).
            assert!(prop0.product_reserve == 0.0);
            let prop1 = test.property.get(&1).unwrap();
            assert!(prop1.total_property == 10.0);
            assert!(prop1.unreserved == 0.0);
            //assert!(prop1.reserved == 0.0);
            assert!(prop1.want_reserve == 10.0);
            assert!((prop1.class_reserve + prop0.class_reserve) == 10.0);
            assert!(prop1.product_reserve == 0.0);
            let prop2 = test.property.get(&2).unwrap();
            assert!(prop2.total_property == 20.0);
            assert!(prop2.unreserved == 0.0);
            //assert!(prop2.reserved == 0.0);
            assert!(prop2.want_reserve == 20.0);
            assert!(prop2.class_reserve == 0.0);
            assert!(prop2.product_reserve == 0.0);
            let prop3 = test.property.get(&3).unwrap();
            assert!(prop3.total_property == 15.0);
            assert!(prop3.unreserved == 5.0);
            //assert!(prop3.reserved == 0.0);
            assert!(prop3.want_reserve == 10.0);
            assert!(prop3.class_reserve == 0.0);
            assert!(prop3.product_reserve == 0.0);
            // ensure want process plan is recorded correctly.
            assert_eq!(test.process_plan[&0], 10.0);
            assert_eq!(test.process_plan[&1], 10.0);
            assert_eq!(result.tier, 35);
            assert!(453.0 < result.value);
            assert!(result.value < 454.0);
        }

        #[test]
        pub fn shift_want_and_product_desires_correctly() {
            // data needed, but not set up for this test.
            let mut data = DataManager::new();
            // wants 0
            let mut want0 = Want{
                id: 0,
                name: "".to_string(),
                description: "".to_string(),
                decay: 0.0,
                ownership_sources: HashSet::new(),
                process_sources: HashSet::new(),
                use_sources: HashSet::new(),
                consumption_sources: HashSet::new(),
            };
            want0.ownership_sources.insert(0);
            want0.process_sources.insert(0);
            want0.process_sources.insert(1);
            want0.use_sources.insert(0);
            want0.consumption_sources.insert(1);
            data.wants.insert(0, want0);
            // products
            data.products.insert(0, Product{
                id: 0,
                name: "".to_string(),
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
            data.products.get_mut(&0).unwrap()
            .wants.insert(0, 1.0);
            data.products.insert(1, Product{
                id: 1,
                name: "".to_string(),
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
            let mut product2 = Product{
                id: 2,
                name: "".to_string(),
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
            };
            product2.use_processes.insert(0);
            product2.consumption_processes.insert(1);
            data.products.insert(2, product2);
            data.products.insert(3, Product{
                id: 3,
                name: "".to_string(),
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
            // products use 1 + 2 = want 0
            data.processes.insert(0, Process{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
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
                        part: ProcessSectionTag::Capital
                    },
                    ProcessPart { 
                        item: Item::Want(0), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output
                    }
                ],
                process_tags: vec![
                    ProcessTag::Use(2)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });
            // products consume 2 + 3 = want 0
            data.processes.insert(1, Process{
                id: 1,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart { 
                        item: Item::Product(2), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input
                    },
                    ProcessPart { 
                        item: Item::Product(3), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input
                    },
                    ProcessPart { 
                        item: Item::Want(0), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output
                    }
                ],
                process_tags: vec![
                    ProcessTag::Consumption(2)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });

            data.update_product_classes().expect("Could not function");
            let test_desires = vec![
                Desire::new(Item::Want(0),
                    1,
                    None,
                    1.0,
                    0.0,
                    1,
                    vec![]).unwrap(),
                Desire::new(Item::Product(2),
                    3,
                    Some(15),
                    1.0,
                    0.0,
                    3,
                    vec![]).unwrap(),
            ];
            let mut test = Property::new(test_desires);
            test.property.insert(0, PropertyInfo::new(15.0));
            test.property.insert(1, PropertyInfo::new(10.0));
            test.property.insert(2, PropertyInfo::new(20.0));
            test.property.insert(3, PropertyInfo::new(15.0));
            let result = test.sift_all(&data);
            // check that the sitfing was done correctly.
            // 26.0 into desire 0, (tier 100)
            let desire0 = test.desires.get(0).unwrap();
            assert_eq!(desire0.satisfaction_up_to_tier().unwrap(), 35);
            assert!(desire0.satisfaction == 35.0);
            // 4.0 into desire 1 (tier 12, totally satisfied)
            let desire1 = test.desires.get(1).unwrap();
            assert!(desire1.is_fully_satisfied());
            assert_eq!(desire1.satisfaction_up_to_tier().unwrap(), 15);
            assert!(desire1.satisfaction == 5.0);
            // and check that items were reserved correctly.
            let prop0 = test.property.get(&0).unwrap();
            assert!(prop0.total_property == 15.0);
            assert!(prop0.unreserved == 0.0);
            //assert!(prop0.reserved == 0.0);
            assert!(prop0.want_reserve == 15.0);
            assert!(prop0.class_reserve == 0.0);
            assert!(prop0.product_reserve == 0.0);
            let prop1 = test.property.get(&1).unwrap();
            assert!(prop1.total_property == 10.0);
            assert!(prop1.unreserved == 0.0);
            //assert!(prop1.reserved == 0.0);
            assert!(prop1.want_reserve == 10.0);
            assert!(prop1.class_reserve == 0.0);
            assert!(prop1.product_reserve == 0.0);
            let prop2 = test.property.get(&2).unwrap();
            assert!(prop2.total_property == 20.0);
            assert!(prop2.unreserved == 0.0);
            //assert!(prop2.reserved == 0.0);
            assert!(prop2.want_reserve == 20.0);
            assert!(prop2.class_reserve == 0.0);
            assert!(prop2.product_reserve == 5.0);
            let prop3 = test.property.get(&3).unwrap();
            assert!(prop3.total_property == 15.0);
            assert!(prop3.unreserved == 5.0);
            //assert!(prop3.reserved == 0.0);
            assert!(prop3.want_reserve == 10.0);
            assert!(prop3.class_reserve == 0.0);
            assert!(prop3.product_reserve == 0.0);
            // ensure want process plan is recorded correctly.
            assert_eq!(test.process_plan[&0], 10.0);
            assert_eq!(test.process_plan[&1], 10.0);
            assert_eq!(result.tier, 35);
            assert!(435.0 < result.value);
            assert!(result.value < 436.0);
        }

        #[test]
        pub fn shift_want_desires_correctly() {
            // data needed, but not set up for this test.
            let mut data = DataManager::new();
            // wants 0
            let mut want0 = Want{
                id: 0,
                name: "".to_string(),
                description: "".to_string(),
                decay: 0.0,
                ownership_sources: HashSet::new(),
                process_sources: HashSet::new(),
                use_sources: HashSet::new(),
                consumption_sources: HashSet::new(),
            };
            want0.ownership_sources.insert(0);
            want0.process_sources.insert(0);
            want0.process_sources.insert(1);
            want0.use_sources.insert(0);
            want0.consumption_sources.insert(1);
            data.wants.insert(0, want0);
            // products
            data.products.insert(0, Product{
                id: 0,
                name: "".to_string(),
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
            data.products.get_mut(&0).unwrap()
            .wants.insert(0, 1.0);
            data.products.insert(1, Product{
                id: 1,
                name: "".to_string(),
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
            let mut product2 = Product{
                id: 2,
                name: "".to_string(),
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
            };
            product2.use_processes.insert(0);
            product2.consumption_processes.insert(1);
            data.products.insert(2, product2);
            data.products.insert(3, Product{
                id: 3,
                name: "".to_string(),
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
            // products use 1 + 2 = want 0
            data.processes.insert(0, Process{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
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
                        part: ProcessSectionTag::Capital
                    },
                    ProcessPart { 
                        item: Item::Want(0), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output
                    }
                ],
                process_tags: vec![
                    ProcessTag::Use(2)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });
            // products consume 2 + 3 = want 0
            data.processes.insert(1, Process{
                id: 1,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart { 
                        item: Item::Product(2), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input
                    },
                    ProcessPart { 
                        item: Item::Product(3), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input
                    },
                    ProcessPart { 
                        item: Item::Want(0), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output
                    }
                ],
                process_tags: vec![
                    ProcessTag::Consumption(2)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });

            data.update_product_classes().expect("Could not function");
            let test_desires = vec![
                Desire::new(Item::Want(0),
                    1,
                    None,
                    1.0,
                    0.0,
                    1,
                    vec![]).unwrap(),
                Desire::new(Item::Want(0),
                    3,
                    Some(12),
                    1.0,
                    0.0,
                    3,
                    vec![]).unwrap(),
            ];
            let mut test = Property::new(test_desires);
            test.property.insert(0, PropertyInfo::new(15.0));
            test.property.insert(1, PropertyInfo::new(10.0));
            test.property.insert(2, PropertyInfo::new(20.0));
            test.property.insert(3, PropertyInfo::new(15.0));
            let result = test.sift_all(&data);
            // check that the sitfing was done correctly.
            // 26.0 into desire 0, (tier 100)
            let desire0 = test.desires.get(0).unwrap();
            assert_eq!(desire0.satisfaction_up_to_tier().unwrap(), 31);
            assert!(desire0.satisfaction == 31.0);
            // 4.0 into desire 1 (tier 12, totally satisfied)
            let desire1 = test.desires.get(1).unwrap();
            assert!(desire1.is_fully_satisfied());
            assert_eq!(desire1.satisfaction_up_to_tier().unwrap(), 12);
            assert!(desire1.satisfaction == 4.0);
            // and check that items were reserved correctly.
            let prop0 = test.property.get(&0).unwrap();
            assert!(prop0.total_property == 15.0);
            assert!(prop0.unreserved == 0.0);
            //assert!(prop0.reserved == 0.0);
            assert!(prop0.want_reserve == 15.0);
            assert!(prop0.class_reserve == 0.0);
            assert!(prop0.product_reserve == 0.0);
            let prop1 = test.property.get(&1).unwrap();
            assert!(prop1.total_property == 10.0);
            assert!(prop1.unreserved == 0.0);
            // assert!(prop1.reserved == 0.0);
            assert!(prop1.want_reserve == 10.0);
            assert!(prop1.class_reserve == 0.0);
            assert!(prop1.product_reserve == 0.0);
            let prop2 = test.property.get(&2).unwrap();
            assert!(prop2.total_property == 20.0);
            assert!(prop2.unreserved == 0.0);
            // assert!(prop2.reserved == 0.0);
            assert!(prop2.want_reserve == 20.0);
            assert!(prop2.class_reserve == 0.0);
            assert!(prop2.product_reserve == 0.0);
            let prop3 = test.property.get(&3).unwrap();
            assert!(prop3.total_property == 15.0);
            assert!(prop3.unreserved == 5.0);
            // assert!(prop3.reserved == 0.0);
            assert!(prop3.want_reserve == 10.0);
            assert!(prop3.class_reserve == 0.0);
            assert!(prop3.product_reserve == 0.0);
            // ensure want process plan is recorded correctly.
            assert_eq!(test.process_plan[&0], 10.0);
            assert_eq!(test.process_plan[&1], 10.0);
            assert_eq!(result.tier, 31);
            assert!(277.0 < result.value);
            assert!(result.value < 278.0);
        }

        /// sift class desires only, don't even set up specfic
        /// or want desires.
        #[test]
        pub fn sift_class_desires_correctly() {
            // data needed, but not set up for this test.
            let mut data = DataManager::new();
            data.products.insert(0, Product{
                id: 0,
                name: "".to_string(),
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
            data.products.insert(1, Product{
                id: 1,
                name: "".to_string(),
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
            data.products.insert(2, Product{
                id: 2,
                name: "".to_string(),
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
            data.update_product_classes().expect("Could not function");
            let test_desires = vec![
                Desire::new(Item::Class(0),
                    0,
                    None,
                    1.0,
                    0.0,
                    4,
                    vec![]).unwrap(),
                Desire::new(Item::Class(0),
                    3,
                    Some(12),
                    1.0,
                    0.0,
                    3,
                    vec![]).unwrap(),
            ];
            let mut test = Property::new(test_desires);
            test.property.insert(0, PropertyInfo::new(15.0));
            test.property.insert(1, PropertyInfo::new(15.0));
            test.property.insert(2, PropertyInfo::new(10.0));
            let result = test.sift_all(&data);
            // check that the sitfing was done correctly.
            // 26.0 into desire 0, (tier 100)
            let desire0 = test.desires.get(0).unwrap();
            assert_eq!(desire0.satisfaction_up_to_tier().unwrap(), 100);
            assert!(desire0.satisfaction == 26.0);
            // 4.0 into desire 1 (tier 12, totally satisfied)
            let desire1 = test.desires.get(1).unwrap();
            assert!(desire1.is_fully_satisfied());
            assert_eq!(desire1.satisfaction_up_to_tier().unwrap(), 12);
            assert!(desire1.satisfaction == 4.0);
            // and check that items were reserved correctly.
            let prop0 = test.property.get(&0).unwrap();
            assert!(prop0.total_property == 15.0);
            assert!(prop0.unreserved == 0.0);
            // assert!(prop0.reserved == 0.0);
            assert!(prop0.want_reserve == 0.0);
            assert!(prop0.class_reserve == 15.0);
            assert!(prop0.product_reserve == 0.0);
            let prop1 = test.property.get(&1).unwrap();
            assert!(prop1.total_property == 15.0);
            assert!(prop1.unreserved == 0.0);
            // assert!(prop1.reserved == 0.0);
            assert!(prop1.want_reserve == 0.0);
            assert!(prop1.class_reserve == 15.0);
            assert!(prop1.product_reserve == 0.0);
            let prop2 = test.property.get(&2).unwrap();
            assert!(prop2.total_property == 10.0);
            assert!(prop2.unreserved == 10.0);
            // assert!(prop2.reserved == 0.0);
            assert!(prop2.want_reserve == 0.0);
            assert!(prop2.class_reserve == 0.0);
            assert!(prop2.product_reserve == 0.0);
            // ensure want process plan is recorded correctly.
            assert_eq!(test.process_plan.len(), 0);
            assert_eq!(result.tier, 100);
            assert!(182146.0 < result.value);
            assert!(result.value < 182147.0);
        }

        /// Sifts 1 Product desire only, don't set up wants
        /// or class desires.
        #[test]
        pub fn sift_product_desire_correctly() {
            // data needed, but not set up for this test.
            let data = &DataManager::new();
            let test_desires = vec![
                Desire::new(Item::Product(0),
                    0,
                    Some(100),
                    1.0,
                    0.0,
                    5,
                    vec![]).unwrap(),
                Desire::new(Item::Product(0),
                    3,
                    Some(12),
                    1.0,
                    0.0,
                    3,
                    vec![]).unwrap(),
            ];
            let mut test = Property::new(test_desires);
            test.property.insert(0, PropertyInfo::new(15.0));
            test.property.insert(1, PropertyInfo::new(10.0));
            let result = test.sift_all(data);
            // check that the sitfing was done correctly.
            // 11.0 into desire 0, (tier 50)
            let desire0 = test.desires.get(0).unwrap();
            assert_eq!(desire0.satisfaction_up_to_tier().unwrap(), 50);
            assert!(desire0.satisfaction == 11.0);
            // 4.0 into desire 1 (tier 12, totally satisfied)
            let desire1 = test.desires.get(1).unwrap();
            assert!(desire1.is_fully_satisfied());
            assert_eq!(desire1.satisfaction_up_to_tier().unwrap(), 12);
            assert!(desire1.satisfaction == 4.0);
            // and check that items were reserved correctly.
            let prop0 = test.property.get(&0).unwrap();
            assert!(prop0.total_property == 15.0);
            assert!(prop0.unreserved == 0.0);
            // assert!(prop0.reserved == 0.0);
            assert!(prop0.want_reserve == 0.0);
            assert!(prop0.class_reserve == 0.0);
            assert!(prop0.product_reserve == 15.0);
            let prop1 = test.property.get(&1).unwrap();
            assert!(prop1.total_property == 10.0);
            assert!(prop1.unreserved == 10.0);
            // assert!(prop1.reserved == 0.0);
            assert!(prop1.want_reserve == 0.0);
            assert!(prop1.class_reserve == 0.0);
            assert!(prop1.product_reserve == 0.0);
            // ensure want process plan is recorded correctly.
            assert_eq!(test.process_plan.len(), 0);
            assert_eq!(result.tier, 50);
            assert!(846.0 < result.value);
            assert!(result.value < 847.0);
        }
    }

    #[test]
    pub fn get_highest_satisfied_tier_for_item_correctly() {
        let mut test_desires = vec![];
        test_desires.push(Desire{ // 0,1
            item: Item::Product(0), 
            start: 0, 
            end: Some(1), 
            amount: 1.0, 
            satisfaction: 0.0,
            step: 1,
            tags: vec![]});
        test_desires.push(Desire{ // 2,4,6
            item: Item::Product(1), 
            start: 2, 
            end: Some(6), 
            amount: 1.0, 
            satisfaction: 0.0,
            step: 2,
            tags: vec![]});
        test_desires.push(Desire{ // 3,6,9, ...
            item: Item::Want(0), 
            start: 3, 
            end: None, 
            amount: 1.0, 
            satisfaction: 0.0,
            step: 3,
            tags: vec![]});
        let mut test = Property::new(test_desires);

        let result1 = test.get_highest_satisfied_tier_for_item(Item::Product(0));
        assert_eq!(result1, None);

        test.desires[0].add_satisfaction(2.0);
        let result2 = test.get_highest_satisfied_tier_for_item(Item::Product(0))
            .expect("Couldn't find.");
        assert_eq!(result2, 1);

        test.desires[1].add_satisfaction(2.0);
        let result3 = test.get_highest_satisfied_tier_for_item(Item::Product(0))
            .expect("Couldn't find.");
        assert_eq!(result3, 1);

        test.desires[2].add_satisfaction(2.0);
        let result4 = test.get_highest_satisfied_tier_for_item(Item::Product(0))
            .expect("Couldn't find.");
        assert_eq!(result4, 1);
    }

    #[test]
    pub fn get_highest_satisfied_tier_correctly() {
        let mut test_desires = vec![];
        test_desires.push(Desire{ // 0,1
            item: Item::Product(0), 
            start: 0, 
            end: Some(1), 
            amount: 1.0, 
            satisfaction: 0.0,
            step: 1,
            tags: vec![]});
        test_desires.push(Desire{ // 2,4,6
            item: Item::Product(1), 
            start: 2, 
            end: Some(6), 
            amount: 1.0, 
            satisfaction: 0.0,
            step: 2,
            tags: vec![]});
        test_desires.push(Desire{ // 3,6,9, ...
            item: Item::Want(0), 
            start: 3, 
            end: None, 
            amount: 1.0, 
            satisfaction: 0.0,
            step: 3,
            tags: vec![]});
        let mut test = Property::new(test_desires);

        let result1 = test.get_highest_satisfied_tier();
        assert_eq!(result1, None);

        test.desires[0].add_satisfaction(2.0);
        let result2 = test.get_highest_satisfied_tier()
            .expect("Couldn't find.");
        assert_eq!(result2, 1);

        test.desires[1].add_satisfaction(2.0);
        let result3 = test.get_highest_satisfied_tier()
            .expect("Couldn't find.");
        assert_eq!(result3, 4);

        test.desires[2].add_satisfaction(2.0);
        let result4 = test.get_highest_satisfied_tier()
            .expect("Couldn't find.");
        assert_eq!(result4, 6);
    }

    #[test]
    pub fn correctly_walk_down_tiers_for_item() {
        let mut test_desires = vec![];
        test_desires.push(Desire{ // 0,1
            item: Item::Product(0), 
            start: 0, 
            end: Some(1), 
            amount: 1.0, 
            satisfaction: 0.0,
            step: 1,
            tags: vec![]});
        test_desires.push(Desire{ // 0, 2,4,6
            item: Item::Product(1), 
            start: 0, 
            end: Some(6), 
            amount: 1.0, 
            satisfaction: 0.0,
            step: 2,
            tags: vec![]});
        test_desires.push(Desire{ // 0,3,6,9, ...
            item: Item::Product(0), 
            start: 0, 
            end: None, 
            amount: 1.0, 
            satisfaction: 0.0,
            step: 3,
            tags: vec![]});
        let test = Property::new(test_desires);

        let mut current = DesireCoord{ tier: 3,
            idx: 3 };
        let mut results = vec![];
        loop {
            let result = test.walk_down_tiers(&current);
            if result.is_none() {
                break;
            }
            current = result.unwrap();
            results.push(current.clone());
        }

        assert_eq!(results[0].idx, 2);
        assert_eq!(results[0].tier, 3);

        assert_eq!(results[2].idx, 0);
        assert_eq!(results[2].tier, 1);

        assert_eq!(results[3].idx, 2);
        assert_eq!(results[3].tier, 0);

        assert_eq!(results[5].idx, 0);
        assert_eq!(results[5].tier, 0);
    }

    #[test]
    pub fn correctly_walk_down_tiers() {
        let mut test_desires = vec![];
        test_desires.push(Desire{ // 0,1
            item: Item::Product(0), 
            start: 0, 
            end: Some(1), 
            amount: 1.0, 
            satisfaction: 0.0,
            step: 1,
            tags: vec![]});
        test_desires.push(Desire{ // 0, 2,4,6
            item: Item::Product(1), 
            start: 0, 
            end: Some(6), 
            amount: 1.0, 
            satisfaction: 0.0,
            step: 2,
            tags: vec![]});
        test_desires.push(Desire{ // 0,3,6,9, ...
            item: Item::Want(0), 
            start: 0, 
            end: None, 
            amount: 1.0, 
            satisfaction: 0.0,
            step: 3,
            tags: vec![]});
        let test = Property::new(test_desires);

        let mut current = DesireCoord{ tier: 3,
            idx: 3 };
        let mut results = vec![];
        loop {
            let result = test.walk_down_tiers(&current);
            if result.is_none() {
                break;
            }
            current = result.unwrap();
            results.push(current.clone());
        }

        assert_eq!(results[0].idx, 2);
        assert_eq!(results[0].tier, 3);

        assert_eq!(results[1].idx, 1);
        assert_eq!(results[1].tier, 2);

        assert_eq!(results[2].idx, 0);
        assert_eq!(results[2].tier, 1);

        assert_eq!(results[3].idx, 2);
        assert_eq!(results[3].tier, 0);

        assert_eq!(results[4].idx, 1);
        assert_eq!(results[4].tier, 0);

        assert_eq!(results[5].idx, 0);
        assert_eq!(results[5].tier, 0);
    }

    #[test]
    pub fn get_lowest_unsatisfied_tier_correctly() {
        let mut test_desires = vec![];
        test_desires.push(Desire{ // 0,1
            item: Item::Product(0), 
            start: 0, 
            end: Some(1), 
            amount: 1.0, 
            satisfaction: 0.0,
            step: 1,
            tags: vec![]});
        test_desires.push(Desire{ // 2,4,6
            item: Item::Product(1), 
            start: 2, 
            end: Some(6), 
            amount: 1.0, 
            satisfaction: 0.0,
            step: 2,
            tags: vec![]});
        test_desires.push(Desire{ // 3,6,9, ...
            item: Item::Want(0), 
            start: 3, 
            end: None, 
            amount: 1.0, 
            satisfaction: 0.0,
            step: 3,
            tags: vec![]});
        let mut test = Property::new(test_desires);

        let result1 = test.get_lowest_unsatisfied_tier()
            .expect("Error Found on empty thing.");
        assert_eq!(result1, 0);

        test.desires[0].add_satisfaction(2.0);
        let result2 = test.get_lowest_unsatisfied_tier()
            .expect("Couldn't find.");
        assert_eq!(result2, 2);

        test.desires[1].add_satisfaction(2.0);
        let result3 = test.get_lowest_unsatisfied_tier()
            .expect("Couldn't find.");
        assert_eq!(result3, 3);

        test.desires[2].add_satisfaction(2.0);
        let result4 = test.get_lowest_unsatisfied_tier()
            .expect("Couldn't find.");
        assert_eq!(result4, 6);
    }

    #[test]
    pub fn get_lowest_unsatisfied_tier_for_item_correctly() {
        let mut test_desires = vec![];
        test_desires.push(Desire{ // 0,1
            item: Item::Product(0), 
            start: 0, 
            end: Some(1), 
            amount: 1.0, 
            satisfaction: 0.0,
            step: 1,
            tags: vec![]});
        test_desires.push(Desire{ // 2,4,6
            item: Item::Product(0), 
            start: 2, 
            end: Some(6), 
            amount: 1.0, 
            satisfaction: 0.0,
            step: 2,
            tags: vec![]});
        test_desires.push(Desire{ // 3,6,9, ...
            item: Item::Product(0), 
            start: 3, 
            end: None, 
            amount: 1.0, 
            satisfaction: 0.0,
            step: 3,
            tags: vec![]});
        let mut test = Property::new(test_desires);

        let result1 = test.get_lowest_unsatisfied_tier_of_item(Item::Product(0))
            .expect("Error Found on empty thing.");
        assert_eq!(result1, 0);

        test.desires[0].add_satisfaction(2.0);
        let result2 = test.get_lowest_unsatisfied_tier_of_item(Item::Product(0))
            .expect("Couldn't find.");
        assert_eq!(result2, 2);

        test.desires[1].add_satisfaction(2.0);
        let result3 = test.get_lowest_unsatisfied_tier_of_item(Item::Product(0))
            .expect("Couldn't find.");
        assert_eq!(result3, 3);

        test.desires[2].add_satisfaction(2.0);
        let result4 = test.get_lowest_unsatisfied_tier_of_item(Item::Product(0))
            .expect("Couldn't find.");
        assert_eq!(result4, 6);
    }

    #[test]
    pub fn get_lowest_unsatisfied_tier_for_item_and_exclude_other_items_correctly() {
        let mut test_desires = vec![];
        test_desires.push(Desire{ // 0,1
            item: Item::Product(0), 
            start: 0, 
            end: Some(1), 
            amount: 1.0, 
            satisfaction: 0.0,
            step: 1,
            tags: vec![]});
        test_desires.push(Desire{ // 2,4,6
            item: Item::Product(0), 
            start: 2, 
            end: Some(6), 
            amount: 1.0, 
            satisfaction: 0.0,
            step: 2,
            tags: vec![]});
        test_desires.push(Desire{ // 3,6,9, ...
            item: Item::Product(1), 
            start: 3, 
            end: None, 
            amount: 1.0, 
            satisfaction: 0.0,
            step: 3,
            tags: vec![]});
        let mut test = Property::new(test_desires);

        let result1 = test.get_lowest_unsatisfied_tier_of_item(Item::Product(0))
            .expect("Error Found on empty thing.");
        assert_eq!(result1, 0);

        test.desires[0].add_satisfaction(2.0);
        let result2 = test.get_lowest_unsatisfied_tier_of_item(Item::Product(0))
            .expect("Couldn't find.");
        assert_eq!(result2, 2);

        test.desires[1].add_satisfaction(2.0);
        let result3 = test.get_lowest_unsatisfied_tier_of_item(Item::Product(0))
            .expect("Couldn't find.");
        assert_eq!(result3, 6);

        test.desires[2].add_satisfaction(2.0);
        let result4 = test.get_lowest_unsatisfied_tier_of_item(Item::Product(0))
            .expect("Couldn't find.");
        assert_eq!(result4, 6);
    }

    mod walk_up_tiers_should {
        use super::*;

        /// Tests that if the function is given a desirecoord with
        /// an index > desires.len(), that it simply wraps back around
        /// to 0.
        #[test]
        pub fn safely_accept_overlapped_idxs_from_prev() {
            let mut test_desires = vec![];
            test_desires.push(Desire{ // 0,1
                item: Item::Product(0), 
                start: 0, 
                end: Some(1), 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,4,6
                item: Item::Product(0), 
                start: 0, 
                end: Some(6), 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 0,3,6,9, ...
                item: Item::Product(0), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 3,
                tags: vec![]});
            let test = Property::new(test_desires);

            let curr = Some(DesireCoord{
                tier: 0,
                idx: 5,
            });
            let result = test.walk_up_tiers(curr).expect("Err");
            
            assert_eq!(result.tier, 1);
            assert_eq!(result.idx, 0);
        }

        #[test]
        pub fn walk_up_tiers_correctly() {
            let mut test_desires = vec![];
            test_desires.push(Desire{ // 0,1
                item: Item::Product(0), 
                start: 0, 
                end: Some(1), 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,4,6
                item: Item::Product(0), 
                start: 0, 
                end: Some(6), 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 0,3,6,9, ...
                item: Item::Product(0), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 3,
                tags: vec![]});
            let test = Property::new(test_desires);

            let mut curr = None;
            let mut results = vec![];
            for _ in 0..11 {
                let val = test.walk_up_tiers(curr).expect("Err.");
                results.push((val.tier, val.idx));
                curr = Some(val);
            }

            assert_eq!(results[0], (0,0));
            assert_eq!(results[1], (0,1));
            assert_eq!(results[2], (0,2));
            assert_eq!(results[3], (1,0));
            assert_eq!(results[4], (2,1));
            assert_eq!(results[5], (3,2));
            assert_eq!(results[6], (4,1));
            assert_eq!(results[7], (6,1));
            assert_eq!(results[8], (6,2));
            assert_eq!(results[9], (9,2));
            assert_eq!(results[10], (12,2));
        }

        #[test]
        pub fn walk_up_tiers_correctly_and_return_none_when_out() {
            let mut test_desires = vec![];
            test_desires.push(Desire{ // 0,1
                item: Item::Product(0), 
                start: 0, 
                end: Some(1), 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,4,6
                item: Item::Product(0), 
                start: 0, 
                end: Some(6), 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 2,
                tags: vec![]});
            let test = Property::new(test_desires);

            let mut curr = None;
            let mut results = vec![];
            for _ in 0..7 {
                let val = test.walk_up_tiers(curr);
                if let Some(value) = val {
                    results.push(Some((value.tier, value.idx)));
                    curr = Some(value);
                } 
                else { results.push(None); curr = None;}
            }

            assert_eq!(results[0].expect("err!"), (0,0));
            assert_eq!(results[1].expect("err!"), (0,1));
            assert_eq!(results[2].expect("err!"), (1,0));
            assert_eq!(results[3].expect("err!"), (2,1));
            assert_eq!(results[4].expect("err!"), (4,1));
            assert_eq!(results[5].expect("err!"), (6,1));
            assert_eq!(results[6], None);
        }
    }

    #[test]
    pub fn walk_up_tiers_for_item_correctly() {
        let mut test_desires = vec![];
        test_desires.push(Desire{ // 0,2, 4
            item: Item::Product(0), 
            start: 0, 
            end: Some(4), 
            amount: 1.0, 
            satisfaction: 0.0,
            step: 2,
            tags: vec![]});
        test_desires.push(Desire{ // 0,2,4,6
            item: Item::Product(1), 
            start: 0, 
            end: Some(6), 
            amount: 1.0, 
            satisfaction: 0.0,
            step: 2,
            tags: vec![]});
        test_desires.push(Desire{ // 0,2,4,6
            item: Item::Want(0), 
            start: 0, 
            end: Some(6), 
            amount: 1.0, 
            satisfaction: 0.0,
            step: 2,
            tags: vec![]});
        let test = Property::new(test_desires);

        let mut curr = None;
        let mut results = vec![];
        loop {
            let val = test.walk_up_tiers_for_item(&curr, &Item::Product(0));
            if let Some(value) = val {
                results.push(Some((value.tier, value.idx)));
                curr = Some(value);
            } 
            else { results.push(None); break;}
        }

        assert_eq!(results[0].expect("err!"), (0,0));
        assert_eq!(results[1].expect("err!"), (2,0));
        assert_eq!(results[2].expect("err!"), (4,0));
        assert_eq!(results[3], None);
    }

    mod consume_goods_should {
        use std::collections::{HashMap, HashSet};
        use super::*;

        #[test]
        pub fn satisfy_and_consume_products_for_desires_as_planned() {
            // Start by setting up the data and sifting.
            // data needed, but not set up for this test.
            let mut data = DataManager::new();
            // wants 0
            let mut want0 = Want{
                id: 0,
                name: "".to_string(),
                description: "".to_string(),
                decay: 0.0,
                ownership_sources: HashSet::new(),
                process_sources: HashSet::new(),
                use_sources: HashSet::new(),
                consumption_sources: HashSet::new(),
            };
            want0.ownership_sources.insert(0);
            want0.process_sources.insert(0);
            want0.process_sources.insert(1);
            want0.use_sources.insert(0);
            want0.consumption_sources.insert(1);
            data.wants.insert(0, want0);
            // products
            data.products.insert(0, Product{
                id: 0,
                name: "".to_string(),
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
            data.products.get_mut(&0).unwrap()
            .wants.insert(0, 1.0);
            data.products.insert(1, Product{
                id: 1,
                name: "".to_string(),
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
            let mut product2 = Product{
                id: 2,
                name: "".to_string(),
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
            };
            product2.use_processes.insert(0);
            product2.consumption_processes.insert(1);
            data.products.insert(2, product2);
            data.products.insert(3, Product{
                id: 3,
                name: "".to_string(),
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
            // products use 1 + 2 = want 0
            data.processes.insert(0, Process{
                id: 0,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
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
                        part: ProcessSectionTag::Capital
                    },
                    ProcessPart { 
                        item: Item::Want(0), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output
                    }
                ],
                process_tags: vec![
                    ProcessTag::Use(2)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });
            // products consume 2 + 3 = want 0
            data.processes.insert(1, Process{
                id: 1,
                name: "".to_string(),
                variant_name: "".to_string(),
                description: "".to_string(),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart { 
                        item: Item::Product(2), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input
                    },
                    ProcessPart { 
                        item: Item::Product(3), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input
                    },
                    ProcessPart { 
                        item: Item::Want(0), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output
                    }
                ],
                process_tags: vec![
                    ProcessTag::Consumption(2)
                ],
                technology_requirement: None,
                tertiary_tech: None,
            });

            data.update_product_classes().expect("Could not function");
            let test_desires = vec![
                Desire::new(Item::Want(0),
                    1,
                    None,
                    1.0,
                    0.0,
                    1,
                    vec![]).unwrap(),
                Desire::new(Item::Class(0),
                    3,
                    None,
                    1.0,
                    0.0,
                    1,
                    vec![]).unwrap(),
                Desire::new(Item::Product(0),
                    5,
                    None,
                    1.0,
                    0.0,
                    1,
                    vec![]).unwrap()
            ];
            let mut test = Property::new(test_desires);
            test.property.insert(0, PropertyInfo::new(15.0)); // 0 consumed
            test.property.insert(1, PropertyInfo::new(10.0)); // 10 consumed in 0
            test.property.insert(2, PropertyInfo::new(20.0)); // 10 consumed in 1, 10 used in 0
            test.property.insert(3, PropertyInfo::new(15.0)); // 10 consumed in 1

            // With sifting and sanity checking that done, do and check consume goods.
            let history = MarketHistory {
                product_info: HashMap::new(),
                class_info: HashMap::new(),
                want_info: HashMap::new(),
                sale_priority: vec![],
                currencies: vec![],
            };
            
            test.consume_goods(&data, &history);
            // with consume goods run
            // check desires again.
            let desire0 = test.desires.get(0).unwrap();
            assert_eq!(desire0.satisfaction, 35.0);
            // 4.0 into desire 1 (tier 12, totally satisfied)
            let desire1 = test.desires.get(1).unwrap();
            assert_eq!(desire1.satisfaction, 25.0);
            // 4.0 into desire 1 (tier 12, totally satisfied)
            let desire2 = test.desires.get(2).unwrap();
            assert_eq!(desire2.satisfaction, 15.0);

            // check all products unreserved and consumed as expected.
            let prop0 = test.property.get(&0).unwrap();
            assert_eq!(prop0.total_property, 0.0);
            assert_eq!(prop0.unreserved, 0.0);
            assert_eq!(prop0.used, 15.0);

            let prop1 = test.property.get(&1).unwrap();
            assert_eq!(prop1.total_property, 0.0);
            assert_eq!(prop1.unreserved, 0.0);

            let prop2 = test.property.get(&2).unwrap();
            assert_eq!(prop2.total_property, 0.0);
            assert_eq!(prop2.unreserved, 0.0);
            assert_eq!(prop2.used, 10.0);

            let prop3 = test.property.get(&3).unwrap();
            assert_eq!(prop3.total_property, 5.0);
            assert_eq!(prop3.unreserved, 5.0);
            // check wants consumed/produced as expected and with no expected remaining.
            let want0 = test.want_store.get(&0).unwrap();
            assert_eq!(want0.consumed, 35.0);
            assert_eq!(want0.expected, 0.0);
            assert_eq!(want0.total_current, 0.0);

            // and check that processes planned has successfully zeroed out.
            assert!(!test.process_plan.contains_key(&0));
            assert!(!test.process_plan.contains_key(&1));
        }
    }
}
