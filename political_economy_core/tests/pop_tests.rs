use std::collections::{HashMap, VecDeque};
use political_economy_core::{
    demographics::Demographics, 
    data_manager::DataManager,
    constants::*,
    objects::{
        data_objects::{
            item::Item,
            want_info::WantInfo,
            want::Want,
            product::Product,
            process::*,

        },
        actor_objects::{
            pop::*,
            property_info::*,
            property::*,
            desire::Desire,
            seller::Seller,
            buy_result::BuyResult,
            actor_message::*,
        },
        demographic_objects::{
            culture::Culture, 
            ideology::Ideology, 
            pop_breakdown_table::{
                PBRow, 
                PopBreakdownTable}, 
            species::Species
        },
        environmental_objects::market::{MarketHistory, ProductInfo, MarketWantInfo}
    }
};

// TODO ALter Pop class and functions to be more invertable, at least with Free_time, Shopping_loop, Try_to_buy, and Standard Buy
mod pop_tests {
    use super::*;

    /// Makes a pop for testing. The pop will have the following info
    /// 
    /// 20 pops total --
    /// 20 pops of the same speciecs
    /// - Desires
    ///   - 20 Food 0/1/2/3/4
    ///   - 20 Shelter 7/9/11/13
    ///   - 20 Clothing 2/4/6/8
    /// 
    /// 10 with a culture
    /// - Desires
    ///   - 10 Ambrosia Fruit 10/15/20/25/30
    ///   - 10 Cotton Clothes 15/25/35 ...
    /// 
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
        };// quickly set all prices to 1.0 for ease going forward.
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

    mod find_class_product_should{
        use std::{thread, time::Duration};
        use super::super::*;

        use super::{make_test_pop, prepare_data_for_market_actions};

        #[test]
        pub fn correctly_respond_to_class_found() {
            let mut test = make_test_pop();
            let pop_info = test.actor_info();
            let (data, market) = prepare_data_for_market_actions(&mut test);
            // don't worry about it buying anything, we'll just pass back a middle finger to get what we want.
            test.is_selling = true;
            // add a bunch of time for shopping.
            test.property.property.insert(TIME_PRODUCT_ID, PropertyInfo::new(test.standard_shop_time_cost() + 100.0));
            // setup messaging
            let (tx, rx) = barrage::bounded(10);
            let mut passed_rx = rx.clone();
            let passed_tx = tx.clone();

            let handle = thread::spawn(move || {
                let result = test.find_class_product(&mut passed_rx, &passed_tx, 0, &data,
                    &market);
                (test, result)
            });
            thread::sleep(Duration::from_millis(100));

            if let ActorMessage::FindClass { class, sender } = rx.recv().unwrap() {
                assert_eq!(class, 0, "Mismatched class.");
                assert_eq!(sender, pop_info, "Mismatched Actor.");
            } else {
                assert!(false, "Incorrect Message Recieved.");
            }

            tx.send(ActorMessage::FoundClass { buyer: pop_info, product: 10 }).expect("Unexpected Break.");
            thread::sleep(Duration::from_millis(100));

            let (_, result) = handle.join().unwrap();
            if let Some(product) = result {
                assert_eq!(product, 10, "Product id Mismatch.");
            } else {
                assert!(false, "No product returned.")
            }
        }

        #[test]
        pub fn correctly_respond_to_class_not_found() {
            let mut test = make_test_pop();
            let pop_info = test.actor_info();
            let (data, market) = prepare_data_for_market_actions(&mut test);
            // don't worry about it buying anything, we'll just pass back a middle finger to get what we want.
            test.is_selling = true;
            // add a bunch of time for shopping.
            test.property.property.insert(TIME_PRODUCT_ID, PropertyInfo::new(test.standard_shop_time_cost() + 100.0));
            // setup messaging
            let (tx, rx) = barrage::bounded(10);
            let mut passed_rx = rx.clone();
            let passed_tx = tx.clone();

            let handle = thread::spawn(move || {
                let result = test.find_class_product(&mut passed_rx, &passed_tx, 0, &data,
                    &market);
                (test, result)
            });
            thread::sleep(Duration::from_millis(100));

            if let ActorMessage::FindClass { class, sender } = rx.recv().unwrap() {
                assert_eq!(class, 0, "Mismatched class.");
                assert_eq!(sender, pop_info, "Mismatched Actor.");
            } else {
                assert!(false, "Incorrect Message Recieved.");
            }

            tx.send(ActorMessage::ClassNotFound { class: 0, buyer: pop_info})
                .expect("Unexpected Break.");
            thread::sleep(Duration::from_millis(100));

            let (_, result) = handle.join().unwrap();
            if let Some(_) = result {
                assert!(false, "Product returned when it shouldn't have.");
            } else {
                assert!(true);
            }
        }
    }

    #[test]
    pub fn update_pop_desires_equivalently() {
        let mut test = Pop{ 
            id: 0, 
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
            item: Item::Product(0), 
            start: 0, 
            end: Some(4), 
            amount: 1.0, 
            satisfaction: 0.0, 
            step: 1, 
            tags: vec![] };
        let species_desire_2 = Desire{ 
            item: Item::Product(1), 
            start: 9, 
            end: None, 
            amount: 1.0, 
            satisfaction: 0.0, 
            step: 1, 
            tags: vec![] };

        let culture_desire_1 = Desire{ 
            item: Item::Product(2), 
            start: 10, 
            end: None, 
            amount: 1.0, 
            satisfaction: 0.0, 
            step: 0, 
            tags: vec![] };
        let culture_desire_2 = Desire{ 
            item: Item::Product(3), 
            start: 15, 
            end: None, 
            amount: 1.0, 
            satisfaction: 0.0, 
            step: 10, 
            tags: vec![] };

        let ideology_desire_1 = Desire{ 
            item: Item::Product(4), 
            start: 30, 
            end: None, 
            amount: 1.0, 
            satisfaction: 0.0, 
            step: 0, 
            tags: vec![] };
        let ideology_desire_2 = Desire{ 
            item: Item::Product(5), 
            start: 31, 
            end: None, 
            amount: 1.0, 
            satisfaction: 0.0, 
            step: 0, 
            tags: vec![] };

        let species = Species::new(0,
            "Species".into(),
            "".into(),
            vec![species_desire_1, species_desire_2],
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

        assert_eq!(test.property.len(), 6);
        // species desire 1 x 20
        let desire_test = test.property.desires.iter()
        .find(|x| x.item == Item::Product(0)).expect("Item Not found");
        assert_eq!(desire_test.amount, 20.0);
        // species desire 1 x 20
        let desire_test = test.property.desires.iter()
        .find(|x| x.item == Item::Product(1)).expect("Item Not found");
        assert_eq!(desire_test.amount, 20.0);
        // culture desire 1 x 10
        let desire_test = test.property.desires.iter()
        .find(|x| x.item == Item::Product(2)).expect("Item Not found");
        assert_eq!(desire_test.amount, 10.0);
        // culture desire 2 x 10
        let desire_test = test.property.desires.iter()
        .find(|x| x.item == Item::Product(3)).expect("Item Not found");
        assert_eq!(desire_test.amount, 10.0);
        // ideology desire 1 x 10
        let desire_test = test.property.desires.iter()
        .find(|x| x.item == Item::Product(4)).expect("Item Not found");
        assert_eq!(desire_test.amount, 10.0);
        // ideology desire 1 x 10
        let desire_test = test.property.desires.iter()
        .find(|x| x.item == Item::Product(5)).expect("Item Not found");
        assert_eq!(desire_test.amount, 10.0);

    }

    mod standard_sell_should {
        use std::{collections::VecDeque, thread, time::Duration};
        use super::super::*;
        use super::prepare_data_for_market_actions;

        #[test]
        pub fn send_out_of_stock_when_item_not_owned() {
            let mut test = Pop {
                id: 0,
                job: 0,
                firm: 0,
                market: 0,
                property: Property::new(vec![]),
                breakdown_table: PopBreakdownTable {
                    table: vec![],
                    total: 1,
                },
                is_selling: true,
                current_sat: TieredValue { tier: 0, value: 0.0 },
                prev_sat: TieredValue { tier: 0, value: 0.0 },
                hypo_change: TieredValue { tier: 0, value: 0.0 },
                backlog: VecDeque::new(),
            };
            let pop_info = test.actor_info();
            let (data, history) = prepare_data_for_market_actions(&mut test);
            // create simple desire to test against.
            test.property.desires.push(Desire::new(Item::Product(1), 0, 
                None, 1.0, 0.0, 
                1, vec![]).expect("whoops"));
            // add product
            test.property.add_property(1, 10.0, &data);
            // setup message queue
            let (tx, rx) = barrage::bounded(10);
            let mut passed_rx = rx.clone();
            let passed_tx = tx.clone();
            // setup pop we're talking with
            let test_buyer = ActorInfo::Pop(1);

            let handle = thread::spawn(move || {
                test.standard_sell(&mut passed_rx, &passed_tx, &data, &history, 
                    10, test_buyer);
                test
            });

            let start = std::time::SystemTime::now();
            while let Ok(msg) = rx.try_recv() {
                if let Some(msg) = msg {
                    if let ActorMessage::NotInStock { buyer, seller,
                    product } = msg {
                        assert_eq!(buyer, test_buyer);
                        assert_eq!(seller, pop_info);
                        assert_eq!(product, 10);
                        break;
                    }
                }
                let now = std::time::SystemTime::now();
                if now.duration_since(start).expect("Bad Time!") 
                    > Duration::from_secs(3) {
                    assert!(false, "Timed Out.");
                }
            }
            // wait a second to let it wrap up.
            thread::sleep(Duration::from_millis(100));
            // check that it's finished
            if !handle.is_finished() { assert!(false); }

            let test = handle.join().unwrap();

            // ensure that the seller hasn't sold anything
            assert_eq!(test.property.property.get(&1).unwrap().total_property, 10.0);
        }

        #[test]
        pub fn send_out_of_stock_when_item_is_insufficient() {
            let mut test = Pop {
                id: 0,
                job: 0,
                firm: 0,
                market: 0,
                property: Property::new(vec![]),
                breakdown_table: PopBreakdownTable {
                    table: vec![],
                    total: 1,
                },
                is_selling: true,
                current_sat: TieredValue { tier: 0, value: 0.0 },
                prev_sat: TieredValue { tier: 0, value: 0.0 },
                hypo_change: TieredValue { tier: 0, value: 0.0 },
                backlog: VecDeque::new(),
            };
            let pop_info = test.actor_info();
            let (data, history) = prepare_data_for_market_actions(&mut test);
            // create simple desire to test against.
            test.property.desires.push(Desire::new(Item::Product(1), 0, 
                None, 1.0, 0.0, 
                1, vec![]).expect("whoops"));
            // add product
            test.property.add_property(1, 10.0, &data);
            test.property.add_property(10, 0.5, &data);
            // setup message queue
            let (tx, rx) = barrage::bounded(10);
            let mut passed_rx = rx.clone();
            let passed_tx = tx.clone();
            // setup pop we're talking with
            let test_buyer = ActorInfo::Pop(1);

            let handle = thread::spawn(move || {
                test.standard_sell(&mut passed_rx, &passed_tx, &data, &history, 
                    10, test_buyer);
                test
            });

            let start = std::time::SystemTime::now();
            while let Ok(msg) = rx.try_recv() {
                if let Some(msg) = msg {
                    if let ActorMessage::NotInStock { buyer, seller,
                    product } = msg {
                        assert_eq!(buyer, test_buyer);
                        assert_eq!(seller, pop_info);
                        assert_eq!(product, 10);
                        break;
                    }
                }
                let now = std::time::SystemTime::now();
                if now.duration_since(start).expect("Bad Time!") 
                    > Duration::from_secs(3) {
                    assert!(false, "Timed Out.");
                }
            }
            // wait a second to let it wrap up.
            thread::sleep(Duration::from_millis(100));
            // check that it's finished
            if !handle.is_finished() { assert!(false); }

            let test = handle.join().unwrap();

            // ensure that the seller hasn't sold anything
            assert_eq!(test.property.property.get(&1).unwrap().total_property, 10.0);
            assert_eq!(test.property.property.get(&10).unwrap().total_property, 0.5);
        }

        #[test]
        pub fn send_out_of_stock_when_item_is_insufficient_and_when_no_satisfaction() {
            let mut test = Pop {
                id: 0,
                job: 0,
                firm: 0,
                market: 0,
                property: Property::new(vec![]),
                breakdown_table: PopBreakdownTable {
                    table: vec![],
                    total: 1,
                },
                is_selling: true,
                current_sat: TieredValue { tier: 0, value: 0.0 },
                prev_sat: TieredValue { tier: 0, value: 0.0 },
                hypo_change: TieredValue { tier: 0, value: 0.0 },
                backlog: VecDeque::new(),
            };
            let pop_info = test.actor_info();
            let (data, history) = prepare_data_for_market_actions(&mut test);
            // create simple desire to test against.
            test.property.desires.push(Desire::new(Item::Product(1), 0, 
                None, 1.0, 0.0, 
                1, vec![]).expect("whoops"));
            // add product
            // test.property.add_property(1, 10.0, &data);
            test.property.add_property(10, 0.5, &data);
            // setup message queue
            let (tx, rx) = barrage::bounded(10);
            let mut passed_rx = rx.clone();
            let passed_tx = tx.clone();
            // setup pop we're talking with
            let test_buyer = ActorInfo::Pop(1);

            let handle = thread::spawn(move || {
                test.standard_sell(&mut passed_rx, &passed_tx, &data, &history, 
                    10, test_buyer);
                test
            });

            let start = std::time::SystemTime::now();
            while let Ok(msg) = rx.try_recv() {
                if let Some(msg) = msg {
                    if let ActorMessage::NotInStock { buyer, seller,
                    product } = msg {
                        assert_eq!(buyer, test_buyer);
                        assert_eq!(seller, pop_info);
                        assert_eq!(product, 10);
                        break;
                    }
                }
                let now = std::time::SystemTime::now();
                if now.duration_since(start).expect("Bad Time!") 
                    > Duration::from_secs(3) {
                    assert!(false, "Timed Out.");
                }
            }
            // wait a second to let it wrap up.
            thread::sleep(Duration::from_millis(100));
            // check that it's finished
            if !handle.is_finished() { assert!(false); }

            let test = handle.join().unwrap();

            // ensure that the seller hasn't sold anything
            assert_eq!(test.property.property.get(&10).unwrap().total_property, 0.5);
        }

        #[test]
        pub fn send_in_stock_and_deal_with_offer_rejected_successfully() {
            let mut test = Pop {
                id: 0,
                job: 0,
                firm: 0,
                market: 0,
                property: Property::new(vec![]),
                breakdown_table: PopBreakdownTable {
                    table: vec![],
                    total: 1,
                },
                is_selling: true,
                current_sat: TieredValue { tier: 0, value: 0.0 },
                prev_sat: TieredValue { tier: 0, value: 0.0 },
                hypo_change: TieredValue { tier: 0, value: 0.0 },
                backlog: VecDeque::new(),
            };
            let pop_info = test.actor_info();
            let (data, history) = prepare_data_for_market_actions(&mut test);
            // create simple desire to test against.
            test.property.desires.push(Desire::new(Item::Product(1), 0, 
                None, 1.0, 0.0, 
                1, vec![]).expect("whoops"));
            // add product
            // test.property.add_property(1, 10.0, &data);
            test.property.add_property(10, 10.0, &data);
            // setup message queue
            let (tx, rx) = barrage::bounded(10);
            let mut passed_rx = rx.clone();
            let passed_tx = tx.clone();
            // setup pop we're talking with
            let test_buyer = ActorInfo::Pop(1);

            let handle = thread::spawn(move || {
                test.standard_sell(&mut passed_rx, &passed_tx, &data, &history, 
                    10, test_buyer);
                test
            });

            let start = std::time::SystemTime::now();
            while let Ok(msg) = rx.try_recv() {
                if let Some(msg) = msg {
                    if let ActorMessage::InStock { buyer, seller,
                    product, price, quantity } = msg {
                        assert_eq!(buyer, test_buyer);
                        assert_eq!(seller, pop_info);
                        assert_eq!(product, 10);
                        assert_eq!(price, 1.0);
                        assert_eq!(quantity, 10.0);
                        break;
                    }
                }
                let now = std::time::SystemTime::now();
                if now.duration_since(start).expect("Bad Time!") 
                    > Duration::from_secs(3) {
                    assert!(false, "Timed Out.");
                }
            }
            // send rejection
            tx.send(ActorMessage::RejectPurchase { buyer: test_buyer, 
                seller: pop_info, 
                product: 10, 
                price_opinion: OfferResult::TooExpensive })
                .expect("Borkde");

            let start = std::time::SystemTime::now();
            while let Ok(msg) = rx.try_recv() {
                if let Some(msg) = msg {
                    if let ActorMessage::RejectPurchase { .. } = msg {
                        break;
                    }
                }
                let now = std::time::SystemTime::now();
                if now.duration_since(start).expect("Bad Time!") 
                    > Duration::from_secs(3) {
                    assert!(false, "Timed Out.");
                }
            }

            // wait a second to let it wrap up.
            thread::sleep(Duration::from_millis(100));
            // check that it's finished
            if !handle.is_finished() { assert!(false); }

            let test = handle.join().unwrap();

            // ensure that the seller hasn't sold anything
            assert_eq!(test.property.property.get(&10).unwrap().total_property, 10.0);
        }

        #[test]
        pub fn send_in_stock_recieve_offer_and_reject_offer_reasonably() {
            let mut test = Pop {
                id: 0,
                job: 0,
                firm: 0,
                market: 0,
                property: Property::new(vec![]),
                breakdown_table: PopBreakdownTable {
                    table: vec![],
                    total: 1,
                },
                is_selling: true,
                current_sat: TieredValue { tier: 0, value: 0.0 },
                prev_sat: TieredValue { tier: 0, value: 0.0 },
                hypo_change: TieredValue { tier: 0, value: 0.0 },
                backlog: VecDeque::new(),
            };
            let pop_info = test.actor_info();
            let (data, mut history) = prepare_data_for_market_actions(&mut test);
            history.product_info.get_mut(&5).unwrap().price = 0.05;
            // create simple desire to test against.
            test.property.desires.push(Desire::new(Item::Product(1), 0, 
                None, 1.0, 0.0, 
                1, vec![]).expect("whoops"));
            test.property.desires.push(Desire::new(Item::Product(10), 0,
                None, 1.0, 0.0, 1, vec![])
                .expect("Bups."));
            // add product
            test.property.add_property(1, 10.0, &data);
            test.property.add_property(10, 0.5, &data);
            // setup message queue
            let (tx, rx) = barrage::bounded(10);
            let mut passed_rx = rx.clone();
            let passed_tx = tx.clone();
            // setup pop we're talking with
            let test_buyer = ActorInfo::Pop(1);

            let handle = thread::spawn(move || {
                test.standard_sell(&mut passed_rx, &passed_tx, &data, &history, 
                    1, test_buyer);
                test
            });

            let start = std::time::SystemTime::now();
            while let Ok(msg) = rx.try_recv() {
                if let Some(msg) = msg {
                    if let ActorMessage::InStock { buyer, seller,
                    product, price, quantity } = msg {
                        assert_eq!(buyer, test_buyer);
                        assert_eq!(seller, pop_info);
                        assert_eq!(product, 1);
                        assert_eq!(price, 1.0);
                        assert_eq!(quantity, 10.0);
                        break;
                    }
                }
                let now = std::time::SystemTime::now();
                if now.duration_since(start).expect("Bad Time!") 
                    > Duration::from_secs(3) {
                    assert!(false, "Timed Out.");
                }
            }
            // send rejection
            tx.send(ActorMessage::BuyOffer { buyer: test_buyer, 
                seller: pop_info, 
                product: 1, 
                price_opinion: OfferResult::Steal, 
                quantity: 6.0, 
                followup: 1 })
                .expect("Borkde");
            tx.send(ActorMessage::BuyOfferFollowup {
                buyer: test_buyer,
                seller: pop_info,
                product: 1,
                offer_product: 5,
                offer_quantity: 1.0,
                followup: 0,
            }).expect("Borkd");

            let start = std::time::SystemTime::now();
            while let Ok(msg) = rx.try_recv() {
                if let Some(msg) = msg {
                    if let ActorMessage::RejectOffer { .. } = msg {
                        break;
                    }
                }
                let now = std::time::SystemTime::now();
                if now.duration_since(start).expect("Bad Time!") 
                    > Duration::from_secs(3) {
                    assert!(false, "Timed Out.");
                }
            }

            // wait a second to let it wrap up.
            thread::sleep(Duration::from_millis(100));
            // check that it's finished
            if !handle.is_finished() { assert!(false); }

            let test = handle.join().unwrap();

            // ensure that the seller hasn't sold anything
            assert_eq!(test.property.property.get(&1).unwrap().total_property, 10.0);
            assert_eq!(test.property.property.get(&10).unwrap().total_property, 0.5);
        }

        #[test]
        pub fn send_in_stock_recieve_offer_and_accept_easy_offer() {
            let mut test = Pop {
                id: 0,
                job: 0,
                firm: 0,
                market: 0,
                property: Property::new(vec![]),
                breakdown_table: PopBreakdownTable {
                    table: vec![],
                    total: 1,
                },
                is_selling: true,
                current_sat: TieredValue { tier: 0, value: 0.0 },
                prev_sat: TieredValue { tier: 0, value: 0.0 },
                hypo_change: TieredValue { tier: 0, value: 0.0 },
                backlog: VecDeque::new(),
            };
            let pop_info = test.actor_info();
            let (data, mut history) = prepare_data_for_market_actions(&mut test);
            history.product_info.get_mut(&5).unwrap().price = 0.05;
            // create simple desire to test against.
            test.property.desires.push(Desire::new(Item::Product(1), 0, 
                None, 1.0, 0.0, 
                1, vec![]).expect("whoops"));
            test.property.desires.push(Desire::new(Item::Product(10), 0,
                None, 1.0, 0.0, 4, vec![])
                .expect("Bups."));
            // add product
            test.property.add_property(1, 10.0, &data);
            test.property.add_property(10, 10.0, &data);
            // setup message queue
            let (tx, rx) = barrage::bounded(10);
            let mut passed_rx = rx.clone();
            let passed_tx = tx.clone();
            // setup pop we're talking with
            let test_buyer = ActorInfo::Pop(1);

            let handle = thread::spawn(move || {
                test.standard_sell(&mut passed_rx, &passed_tx, &data, &history, 
                    10, test_buyer);
                test
            });

            let start = std::time::SystemTime::now();
            while let Ok(msg) = rx.try_recv() {
                if let Some(msg) = msg {
                    if let ActorMessage::InStock { buyer, seller,
                    product, price, quantity } = msg {
                        assert_eq!(buyer, test_buyer);
                        assert_eq!(seller, pop_info);
                        assert_eq!(product, 10);
                        assert_eq!(price, 1.0);
                        assert_eq!(quantity, 7.0);
                        break;
                    }
                }
                let now = std::time::SystemTime::now();
                if now.duration_since(start).expect("Bad Time!") 
                    > Duration::from_secs(3) {
                    assert!(false, "Timed Out.");
                }
            }
            // send rejection
            tx.send(ActorMessage::BuyOffer { buyer: test_buyer, 
                seller: pop_info, 
                product: 10, 
                price_opinion: OfferResult::Steal, 
                quantity: 1.0, 
                followup: 1 })
                .expect("Borkde");
            tx.send(ActorMessage::BuyOfferFollowup {
                buyer: test_buyer,
                seller: pop_info,
                product: 10,
                offer_product: 1,
                offer_quantity: 1.0,
                followup: 0,
            }).expect("Borkd");

            let start = std::time::SystemTime::now();
            while let Ok(msg) = rx.try_recv() {
                if let Some(msg) = msg {
                    if let ActorMessage::SellerAcceptOfferAsIs { .. } = msg {
                        break;
                    }
                }
                let now = std::time::SystemTime::now();
                if now.duration_since(start).expect("Bad Time!") 
                    > Duration::from_secs(3) {
                    assert!(false, "Timed Out.");
                }
            }

            // wait a second to let it wrap up.
            thread::sleep(Duration::from_millis(100));
            // check that it's finished
            if !handle.is_finished() { assert!(false); }

            let test = handle.join().unwrap();

            // ensure that the seller hasn't sold anything
            assert_eq!(test.property.property.get(&1).unwrap().total_property, 11.0);
            assert_eq!(test.property.property.get(&10).unwrap().total_property, 9.0);
        }

        // TODO When returning change is possible, add test here and update previous test.
    }

    mod msg_tests {
        use std::{thread, time::Duration, collections::{HashMap, HashSet}};
        use super::super::*;

        use super::make_test_pop;

        mod specific_wait_should {
            use std::{thread, time::Duration};
            use super::super::*;

            use super::make_test_pop;

            #[test]
            pub fn wait_only_on_product_messages_requested() {
                // do basic setup.
                let mut test = make_test_pop();
                let pop_info = test.actor_info();
                let firm = ActorInfo::Firm(0);
                // setup message queue.
                let (tx, rx) = barrage::bounded(10);
                // push a bunch of stuff to get it blocked.
                let undesired_msg = ActorMessage::CheckItem { buyer: firm, 
                    seller: ActorInfo::Firm(0), proudct: 0 };
                let desired_msg1 = ActorMessage::BuyOffer { buyer: firm, 
                    seller: pop_info, product: 0, price_opinion: OfferResult::Cheap, 
                    quantity: 10.0, followup: 0 };
                let desired_msg2 = ActorMessage::CheckItem { buyer: firm, seller: pop_info, 
                    proudct: 4 };
                let buffer_msg = ActorMessage::BuyOfferFollowup { buyer: firm, 
                    seller: pop_info, product: 0, offer_product: 2, 
                    offer_quantity: 5.0, followup: 0 };

                // get the thread going
                let handler = thread::spawn(move || {
                    let result = test.specific_wait(&rx, &vec![
                        ActorMessage::BuyOffer { buyer: ActorInfo::Firm(0), 
                            seller: ActorInfo::Firm(0), product: 0, price_opinion: OfferResult::Cheap, 
                            quantity: 0.0, followup: 0 },
                        ActorMessage::CheckItem { buyer: firm, seller: pop_info, 
                            proudct: 4 }]);
                    (test, result)
                });

                // add msgs.
                tx.send(undesired_msg).expect("Failed to send."); // ignored
                tx.send(buffer_msg).expect("Failed to send."); // put in backlog
                tx.send(desired_msg1).expect("Failed to send."); // picked up
                tx.send(desired_msg2).expect("Failed to send.");// not recieved.

                thread::sleep(Duration::from_millis(100));
                // get the actor and info back
                let (mut test, result) = handler.join().unwrap();
                // ensure that the result was recieved
                if let ActorMessage::BuyOffer { buyer, seller, 
                product, price_opinion, 
                quantity, followup } = result {
                    assert_eq!(buyer, firm);
                    assert_eq!(seller, pop_info);
                    assert_eq!(product, 0);
                    assert_eq!(price_opinion, OfferResult::Cheap);
                    assert_eq!(quantity, 10.0);
                    assert_eq!(followup, 0);
                } else { assert!(false); }
                assert_eq!(test.backlog.len(), 1);
                let backlogged = test.backlog.pop_front().unwrap();
                if let ActorMessage::BuyOfferFollowup { buyer, seller, 
                product, offer_product, 
                offer_quantity, followup } = backlogged {
                    assert_eq!(buyer, firm);
                    assert_eq!(seller, pop_info);
                    assert_eq!(product, 0);
                    assert_eq!(offer_product, 2);
                    assert_eq!(offer_quantity, 5.0);
                    assert_eq!(followup, 0);
                } else { assert!(false); }
            }

            #[test]
            pub fn pull_specified_msgs_from_backlog_when_available() {
                // do basic setup.
                let mut test = make_test_pop();
                let pop_info = test.actor_info();
                let firm = ActorInfo::Firm(0);
                // setup message queue.
                let (_tx, rx) = barrage::bounded(10);
                // push a bunch of stuff to get it blocked.
                let undesired_msg = ActorMessage::CheckItem { buyer: firm, 
                    seller: ActorInfo::Firm(0), proudct: 0 };
                let desired_msg1 = ActorMessage::BuyOffer { buyer: firm, 
                    seller: pop_info, product: 0, price_opinion: OfferResult::Cheap, 
                    quantity: 10.0, followup: 0 };
                let desired_msg2 = ActorMessage::CheckItem { buyer: firm, seller: pop_info, 
                    proudct: 4 };
                let buffer_msg = ActorMessage::BuyOfferFollowup { buyer: firm, 
                    seller: pop_info, product: 0, offer_product: 2, 
                    offer_quantity: 5.0, followup: 0 };

                // pre-add our msgs
                test.backlog.push_back(undesired_msg);
                test.backlog.push_back(desired_msg1);
                test.backlog.push_back(desired_msg2);
                test.backlog.push_back(buffer_msg);

                // get the thread going
                let handler = thread::spawn(move || {
                    let result = test.specific_wait(&rx, &vec![
                        ActorMessage::BuyOffer { buyer: ActorInfo::Firm(0), 
                            seller: ActorInfo::Firm(0), product: 0, price_opinion: OfferResult::Cheap, 
                            quantity: 0.0, followup: 0 }]);
                    (test, result)
                });

                // don't send anything.

                // get the actor and info back
                let (mut test, result) = handler.join().unwrap();
                // ensure that the result was recieved
                if let ActorMessage::BuyOffer { buyer, seller, 
                product, price_opinion, 
                quantity, followup } = result {
                    assert_eq!(buyer, firm);
                    assert_eq!(seller, pop_info);
                    assert_eq!(product, 0);
                    assert_eq!(price_opinion, OfferResult::Cheap);
                    assert_eq!(quantity, 10.0);
                    assert_eq!(followup, 0);
                } else { assert!(false); }
                assert_eq!(test.backlog.len(), 3);
                let backlogged1 = test.backlog.pop_front().unwrap();
                assert_eq!(backlogged1, undesired_msg);
                let backlogged2 = test.backlog.pop_front().unwrap();
                assert_eq!(backlogged2, desired_msg2);
                let backlogged3 = test.backlog.pop_front().unwrap();
                assert_eq!(backlogged3, buffer_msg);
            }
        }

        #[test]
        pub fn should_consume_msgs_with_msg_catchup_internal_consumes_first() {
            let mut test = make_test_pop();

            // setup message queue.
            let (tx, rx) = barrage::bounded(10);
            let passed_rx = rx.clone();

            // push a bunch of stuff to get it blocked.
            let msg = ActorMessage::WantSplash { sender: ActorInfo::Firm(0), 
                want: 0, amount: 1.0 };
            loop {
                let result = 
                tx.try_send(msg);
                if let Err(_) = result {
                    break;
                }
            }
            
            let handler = thread::spawn(move || {
                test.msg_catchup(&passed_rx);
                test
            });

            // check that our messages are still there in the queue (we didn't read them out.
            let mut in_queue = 0;
            loop {
                let result = rx.try_recv().unwrap();
                if let None = result {
                    break;
                } else {
                    in_queue += 1;
                }
            }
            assert_eq!(in_queue, 10);
            // check that the test has added the msg to the backlog.
            // wait for the handler to get everything.
            let mut test = handler.join().unwrap();
            assert_eq!(test.backlog.len(), 10);
            if let ActorMessage::WantSplash { .. } 
                = test.backlog.pop_front().unwrap() {}
            else { assert!(false); }
        }

        #[test]
        pub fn should_consume_msgs_with_msg_catchup_external_consumes_first() {
            let mut test = make_test_pop();

            // setup message queue.
            let (tx, rx) = barrage::bounded(10);
            let passed_rx = rx.clone();

            // push a bunch of stuff to get it blocked.
            let msg = ActorMessage::WantSplash { sender: ActorInfo::Firm(0), 
                want: 0, amount: 1.0 };
            loop {
                let result = 
                tx.try_send(msg);
                if let Err(_) = result {
                    break;
                }
            }
            
            let handler = thread::spawn(move || {
                test.msg_catchup(&passed_rx);
                test
            });

            // wait for the handler to get everything.
            let mut test = handler.join().unwrap();
            // check that the test has added the msg to the backlog.
            assert_eq!(test.backlog.len(), 10);
            if let ActorMessage::WantSplash { .. } 
                = test.backlog.pop_front().unwrap() {}
            else { assert!(false); }
            // check that our messages are still there in the queue (we didn't read them out.
            let mut in_queue = 0;
            loop {
                let result = rx.try_recv().unwrap();
                if let None = result {
                    break;
                } else {
                    in_queue += 1;
                }
            }
            assert_eq!(in_queue, 10);
        }

        #[test]
        pub fn should_push_msg_safely() {
            let mut test = make_test_pop();
            // setup message queue.
            let (tx, rx) = barrage::bounded(10);
            let passed_rx = rx.clone();
            let passed_tx = tx.clone();
            // push a bunch of stuff to get it blocked.
            let buffered_msg = ActorMessage::CheckItem { buyer: ActorInfo::Firm(0), 
                seller: test.actor_info(), proudct: 0 };
            loop {
                let result = tx.try_send(buffered_msg);
                if let Err(_) = result {
                    break;
                }
            }
            let msg = ActorMessage::BuyOffer { 
                buyer: test.actor_info(), seller: ActorInfo::Firm(0), 
                product: 0, price_opinion: OfferResult::Cheap, 
                quantity: 0.0, followup: 0 };
            let passed_msg = msg.clone();
            // kick off the thread. and pass the buy offer msg
            let handler = thread::spawn(move || {
                test.push_message(&passed_rx, &passed_tx, passed_msg);
                test
            });
            // assert blocked.
            assert!(!handler.is_finished());
            // consume a message and check that our message was sent.
            if let ActorMessage::CheckItem { .. } = rx.recv().unwrap() {
                thread::sleep(Duration::from_millis(100));
                assert!(handler.is_finished());
            } else { // if we didn't get the msg we orignially sent, we have a problem.
                assert!(false);
            }
            
            // wrap up the test thread and check that it's backlog has captured our msgs
            let test = handler.join().unwrap();
            assert_eq!(test.backlog.len(), 10);
            for idx in test.backlog.iter() {
                if let ActorMessage::CheckItem { .. } = idx {}
                else {
                    assert!(false);
                }
            }
            // then check that the reciever got the message sent by the actor
            // get all the messages, in order
            let mut log = vec![];
            while let Ok(msg) = rx.try_recv() {
                if let Some(msg) = msg {
                    log.push(msg);
                } else { break; }
            }
            // assert correct length and last is as expected
            assert_eq!(log.len(), 10);
            if let ActorMessage::BuyOffer { .. } = log.last().unwrap() {
                assert!(true);
            } else { assert!(false); }
        }

        #[test]
        pub fn should_get_next_message_for_pop_and_not_others() {
            let test = make_test_pop();
            // setup message queue.
            let (tx, rx) = barrage::bounded(10);
            let passed_rx = rx.clone();
            // push a bunch of stuff to get it blocked.
            let undesired_msg = ActorMessage::CheckItem { buyer: ActorInfo::Firm(0), 
                seller: ActorInfo::Firm(0), proudct: 0 };
            let desired_msg = ActorMessage::BuyOffer { buyer: ActorInfo::Firm(0), 
                seller: test.actor_info(), product: 0, price_opinion: OfferResult::Cheap, 
                quantity: 10.0, followup: 0 };
            // add msgs.
            tx.send(undesired_msg).expect("Failed to send.");
            tx.send(undesired_msg).expect("Failed to send.");
            tx.send(desired_msg).expect("Failed to send.");
            tx.send(undesired_msg).expect("Failed to send.");

            // kick off the thread. and pass the buy offer msg
            let result = test.get_next_message(&passed_rx);
            if let ActorMessage::BuyOffer { .. } = result {
                assert!(true);
            } else { assert!(false) };
        }
    
        #[test]
        pub fn should_active_wait_successfully() {
            // Test skipped due to use of process_common_message
        }
    
        #[test]
        pub fn should_send_offer_correctly() {
            let mut test = make_test_pop();
            let pop_info = test.actor_info();
            // setup message queue.
            let (tx, rx) = barrage::bounded(10);
            let passed_rx = rx.clone();
            let passed_tx = tx.clone();

            // offer setup
            let mut offer = HashMap::new();
            offer.insert(10, 10.0);
            offer.insert(11, 15.0);
            offer.insert(13, 12.0);
            offer.insert(5, 1.0);
            // offer result
            let offer_result = OfferResult::Cheap;
            // product being purchased
            let product = 2;
            // target to buy
            let target = 10.0;
            // seller
            let firm = ActorInfo::Firm(0);

            test.send_buy_offer(&passed_rx, &passed_tx, product, firm, &offer, offer_result, target);

            // get all the pushed msgs
            let mut msgs = vec![];
            while let Ok(Some(msg)) = rx.try_recv() {
                msgs.push(msg);
            }

            let start = msgs.get(0).unwrap();
            if let ActorMessage::BuyOffer { buyer, seller, 
            product, price_opinion, 
            quantity, followup } = start {
                assert_eq!(*buyer, pop_info);
                assert_eq!(*seller, firm);
                assert_eq!(*product, 2);
                assert_eq!(*price_opinion, offer_result);
                assert_eq!(*quantity, target);
                assert_eq!(*followup, 4);
            } else { assert!(false); }

            let mut idx = 1;
            let mut product_count = HashSet::new();
            while let Some(msg) = msgs.get(idx) {
                if let ActorMessage::BuyOfferFollowup { buyer, seller, 
                product, offer_product, 
                offer_quantity, followup } = msg {
                    product_count.insert(*offer_product);
                    if *offer_product == 10 {
                        assert_eq!(*offer_quantity, 10.0);
                    }  else if *offer_product == 11 {
                        assert_eq!(*offer_quantity, 15.0);
                    }  else if *offer_product == 13 {
                        assert_eq!(*offer_quantity, 12.0);
                    }  else if *offer_product == 5 {
                        assert_eq!(*offer_quantity, 1.0);
                    }  else { assert!(false); }
                    assert_eq!(*product, 2);
                    assert_eq!(*buyer, pop_info);
                    assert_eq!(*seller, firm);
                    assert!(*followup < 4);
                } else { assert!(false); }
                idx += 1;
            }
            assert_eq!(product_count.len(), 4);
        }
    }

    mod process_firm_message {
        use std::collections::HashMap;
        use super::super::*;

        use super::make_test_pop;
        use super::prepare_data_for_market_actions;

        #[test]
        pub fn should_return_true_when_workdayended_recieved() {
            let mut test = make_test_pop();
            let (data, _market) = prepare_data_for_market_actions(&mut test);
            // setup message queue.
            let (tx, rx) = barrage::bounded(10);
            let passed_rx = rx.clone();
            let passed_tx = tx.clone();
            // setup the sender (firm who sent it)
            let sender = ActorInfo::Firm(10);
            let firm_action = FirmEmployeeAction::WorkDayEnded;

            assert!(test.process_firm_message(&passed_rx, &passed_tx, 
                sender, firm_action, &data));
            
            // ensure no messages sent or recieved.
            let rx_msg = rx.try_recv().expect("Unexpected Disconnect?");
            assert!(rx_msg.is_none());
        }

        #[test]
        pub fn should_return_false_and_send_its_time_out() {
            let mut test = make_test_pop();
            let (data, _market) = prepare_data_for_market_actions(&mut test);
            // add the pop's time to work from memory
            let work_time = 10.0;
            test.property.work_time = work_time;
            test.property.property.insert(TIME_PRODUCT_ID, PropertyInfo::new(20.0));
            // setup message queue.
            let (tx, rx) = barrage::bounded(10);
            let passed_rx = rx.clone();
            let passed_tx = tx.clone();
            // setup the sender (firm who sent it)
            let firm = ActorInfo::Firm(10);
            let firm_action = FirmEmployeeAction::RequestTime;

            assert!(!test.process_firm_message(&passed_rx, &passed_tx, 
                firm, firm_action, &data));
            
            // ensure time sent.
            let rx_msg = rx.try_recv().expect("Unexpected Disconnect?");
            if let Some(msg) = rx_msg {
                if let ActorMessage::SendProduct { sender, reciever, 
                product, amount } = msg {
                    assert_eq!(sender, test.actor_info());
                    assert_eq!(reciever, firm);
                    assert_eq!(product, TIME_PRODUCT_ID);
                    assert_eq!(amount, work_time);
                }
                else { assert!(false); }
            } else { assert!(false); }
            // ensure nothing else sent
            let rx_msg = rx.try_recv().expect("Unexpected Disconnect?");
            assert!(rx_msg.is_none());
            // check that the pop has reduced it's time appropriately.
            assert_eq!(test.property.property.get(&TIME_PRODUCT_ID).unwrap().total_property, 10.0);
        }

        #[test]
        pub fn should_send_everything_when_everything_requested() {
            let mut test = make_test_pop();
            let (data, _market) = prepare_data_for_market_actions(&mut test);
            // add the pop's time to work from memory
            let work_time = 10.0;
            test.property.work_time = work_time;
            test.property.property.insert(TIME_PRODUCT_ID, PropertyInfo::new(20.0));
            test.property.property.insert(3, PropertyInfo::new(10.0));
            test.property.property.insert(5, PropertyInfo::new(10.0));
            test.property.want_store.insert(4, WantInfo::new(20.0));
            test.property.want_store.insert(6, WantInfo::new(5.0));
            // setup message queue.
            let (tx, rx) = barrage::bounded(10);
            let passed_rx = rx.clone();
            let passed_tx = tx.clone();
            // setup the sender (firm who sent it)
            let firm = ActorInfo::Firm(10);
            let firm_action = FirmEmployeeAction::RequestEverything;

            assert!(!test.process_firm_message(&passed_rx, &passed_tx, 
                firm, firm_action, &data));
            // Test should've sent everything they had, so check that all of them were added.
            let mut rec_prods = HashMap::new();
            let mut rec_wants = HashMap::new();
            let mut finisher_recieved = false;
            while let Some(msg) = rx.try_recv().unwrap() {
                if let ActorMessage::SendProduct { sender, reciever,
                product, amount } = msg {
                    assert!(reciever == firm);
                    assert!(sender == test.actor_info());
                    rec_prods.insert(product, amount);
                }
                else if let ActorMessage::SendWant { sender, reciever,
                want, amount } = msg {
                    assert!(reciever == firm);
                    assert!(sender == test.actor_info());
                    rec_wants.insert(want, amount);
                } else if let ActorMessage::EmployeeToFirm { employee: sender, firm: reciever, 
                action } = msg {
                    assert!(reciever == firm);
                    assert!(sender == test.actor_info());
                    assert!(action == FirmEmployeeAction::RequestSent);
                    // also assert that nothing comes after this msg.
                    let result = rx.try_recv().unwrap();
                    assert!(result.is_none());
                    finisher_recieved = true;
                }
                else { assert!(false); }
            }
            // assert that the items were sent
            assert_eq!(rec_prods.len(), 3);
            assert_eq!(rec_wants.len(), 2);
            assert_eq!(*rec_prods.get(&TIME_PRODUCT_ID).unwrap(), 20.0);
            assert_eq!(*rec_prods.get(&3).unwrap(), 10.0);
            assert_eq!(*rec_prods.get(&5).unwrap(), 10.0);
            assert_eq!(*rec_wants.get(&4).unwrap(), 20.0);
            assert_eq!(*rec_wants.get(&6).unwrap(), 5.0);
            assert!(finisher_recieved);

            // and assert that those items have been removed from the pop
            assert!(test.property.property.is_empty());
            assert!(test.property.want_store.is_empty());
        }
    
        #[test]
        pub fn should_send_requested_item_when_asked() {
            let mut test = make_test_pop();
            let (data, _market) = prepare_data_for_market_actions(&mut test);
            // add the pop's time to work from memory
            let work_time = 10.0;
            test.property.work_time = work_time;
            test.property.property.insert(TIME_PRODUCT_ID, PropertyInfo::new(20.0));
            test.property.property.insert(3, PropertyInfo::new(10.0));
            test.property.property.insert(5, PropertyInfo::new(10.0));
            test.property.want_store.insert(4, WantInfo::new(20.0));
            test.property.want_store.insert(6, WantInfo::new(5.0));
            // setup message queue.
            let (tx, rx) = barrage::bounded(10);
            let passed_rx = rx.clone();
            let passed_tx = tx.clone();
            // setup the sender (firm who sent it)
            let firm = ActorInfo::Firm(10);
            let firm_action = FirmEmployeeAction::RequestItem { product: 3 };

            assert!(!test.process_firm_message(&passed_rx, &passed_tx, 
                firm, firm_action, &data));
            // Test should've sent everything they had, so check that all of them were added.
            let mut rec_prods = HashMap::new();
            while let Some(msg) = rx.try_recv().unwrap() {
                if let ActorMessage::SendProduct { sender, reciever,
                product, amount } = msg {
                    assert!(reciever == firm);
                    assert!(sender == test.actor_info());
                    rec_prods.insert(product, amount);
                }
                else { assert!(false); }
            }
            // assert that the items were sent
            assert_eq!(rec_prods.len(), 1);
            assert_eq!(*rec_prods.get(&3).unwrap(), 10.0);

            // and assert that those items have been removed from the pop
            assert_eq!(test.property.property.len(), 2);
            assert_eq!(test.property.property.get(&TIME_PRODUCT_ID).unwrap().total_property, 20.0);
            assert_eq!(test.property.property.get(&5).unwrap().total_property, 10.0);
            assert!(!test.property.property.contains_key(&3));
            assert_eq!(test.property.want_store.len(), 2);
            assert_eq!(test.property.want_store.get(&4).unwrap().total_current, 20.0);
            assert_eq!(test.property.want_store.get(&6).unwrap().total_current, 5.0);
        }
    }

    // Complete
    mod work_day_processing {
        use std::{thread, time::Duration};
        use super::super::*;

        use super::*;

        #[test]
        pub fn should_stop_when_work_day_ended_recieved() {
            let mut test = make_test_pop();
            let (data, _market) = prepare_data_for_market_actions(&mut test);
            let pop_info = test.actor_info();
            // setup message queue.
            let (tx, rx) = barrage::bounded(10);
            let mut passed_rx = rx.clone();
            let passed_tx = tx.clone();
            // setup firm which is talknig
            let firm = ActorInfo::Firm(10);
            
            let handler = thread::spawn(move || {
                test.work_day_processing(&mut passed_rx, &passed_tx, &data);
                test
            });

            // assert it's not done.
            assert!(!handler.is_finished());
            // push the FirmToEmployee message with work_day_ended
            tx.send(ActorMessage::FirmToEmployee{ firm, 
                employee: pop_info, action: FirmEmployeeAction::WorkDayEnded })
                .expect("Failed to send?");
            thread::sleep(Duration::from_millis(100));

            assert!(handler.is_finished());
            handler.join().unwrap();
        }

        #[test]
        pub fn should_add_want_splash_recieved() {
            let mut test = make_test_pop();
            let (data, _market) = prepare_data_for_market_actions(&mut test);
            let pop_info = test.actor_info();
            // setup message queue.
            let (tx, rx) = barrage::bounded(10);
            let mut passed_rx = rx.clone();
            let passed_tx = tx.clone();
            // setup firm which is talknig
            let firm = ActorInfo::Firm(10);
            
            let handler = thread::spawn(move || {
                test.work_day_processing(&mut passed_rx, &passed_tx, &data);
                test
            });

            // assert it's not done.
            assert!(!handler.is_finished());

            // send the want splash
            tx.send(ActorMessage::WantSplash { sender: firm, want: 0, amount: 10.0 })
                .expect("Failed to send.");
            thread::sleep(Duration::from_millis(100));

            // end it
            tx.send(ActorMessage::FirmToEmployee{ firm, 
                employee: pop_info, action: FirmEmployeeAction::WorkDayEnded })
                .expect("Failed to send?");
            thread::sleep(Duration::from_millis(100));
            assert!(handler.is_finished());
            let test = handler.join().unwrap();
            
            // check that the want was recieved
            assert_eq!(test.property.want_store.get(&0).unwrap().total_current, 10.0);
        }

        #[test]
        pub fn should_add_sent_products_recieved() {
            let mut test = make_test_pop();
            let (data, _market) = prepare_data_for_market_actions(&mut test);
            let pop_info = test.actor_info();
            // setup message queue.
            let (tx, rx) = barrage::bounded(10);
            let mut passed_rx = rx.clone();
            let passed_tx = tx.clone();
            // setup firm which is talknig
            let firm = ActorInfo::Firm(10);
            
            let handler = thread::spawn(move || {
                test.work_day_processing(&mut passed_rx, &passed_tx, &data);
                test
            });

            // assert it's not done.
            assert!(!handler.is_finished());

            // send the want splash
            tx.send(ActorMessage::SendProduct { sender: firm, reciever: pop_info, 
                product: 10, amount: 10.0 })
                .expect("Failed to send.");
            thread::sleep(Duration::from_millis(100));

            // end it
            tx.send(ActorMessage::FirmToEmployee{ firm, 
                employee: pop_info, action: FirmEmployeeAction::WorkDayEnded })
                .expect("Failed to send?");
            thread::sleep(Duration::from_millis(100));
            assert!(handler.is_finished());
            let test = handler.join().unwrap();
            
            // check that the want was recieved
            assert_eq!(test.property.property.get(&10).unwrap().total_property, 10.0);
        }

        #[test]
        pub fn should_add_all_other_msgs_recieved_to_backlog() {
            let mut test = make_test_pop();
            let (data, _market) = prepare_data_for_market_actions(&mut test);
            let pop_info = test.actor_info();
            // setup message queue.
            let (tx, rx) = barrage::bounded(10);
            let mut passed_rx = rx.clone();
            let passed_tx = tx.clone();
            // setup firm which is talknig
            let firm = ActorInfo::Firm(10);
            
            let handler = thread::spawn(move || {
                test.work_day_processing(&mut passed_rx, &passed_tx, &data);
                test
            });

            // assert it's not done.
            assert!(!handler.is_finished());

            let msg = ActorMessage::BuyOffer { buyer: firm, 
                seller: pop_info, product: 10, 
                price_opinion: OfferResult::Cheap, quantity: 10.0, followup: 0 };

            // send the want splash
            tx.send(msg)
                .expect("Failed to send.");
            thread::sleep(Duration::from_millis(100));

            // end it
            tx.send(ActorMessage::FirmToEmployee{ firm, 
                employee: pop_info, action: FirmEmployeeAction::WorkDayEnded })
                .expect("Failed to send?");
            thread::sleep(Duration::from_millis(100));
            assert!(handler.is_finished());
            let mut test = handler.join().unwrap();
            
            // check that the want was recieved
            assert_eq!(test.backlog.len(), 1);
            if let ActorMessage::BuyOffer { buyer, seller, 
            product, price_opinion, 
            quantity, followup } = test.backlog.pop_front().unwrap() {
                assert_eq!(buyer, firm);
                assert_eq!(seller, pop_info);
                assert_eq!(product, 10);
                if let OfferResult::Cheap = price_opinion {
                    // nothing
                } else { assert!(false); }
                assert_eq!(quantity, 10.0);
                assert_eq!(followup, 0);
            } else { assert!(false); }
        }
    }

    // Completed
    mod create_offer_tests {
        use std::collections::HashMap;
        use super::super::*;

        use super::make_test_pop;

        #[test]
        pub fn should_return_everything_when_short() {
            let pop = make_test_pop();

            let mut data = DataManager::new();

            // TODO when load_all is updated to take a file, relpace this with the 'default' load.
            data.load_test_data().expect("Error loading prefabs");

            // Add items to spend.
            let mut spend = HashMap::new();
            spend.insert(2, PropertyInfo::new(10.0));
            spend.insert(3, PropertyInfo::new(10.0));
            spend.insert(4, PropertyInfo::new(10.0));
            spend.insert(5, PropertyInfo::new(10.0));
            spend.insert(6, PropertyInfo::new(10.0));
            spend.insert(7, PropertyInfo::new(10.0));

            let mut market = MarketHistory {
                product_info: HashMap::new(),
                sale_priority: vec![],
                currencies: vec![],
                class_info: HashMap::new(),
                want_info: HashMap::new(),
            };
            market.product_info.insert(2, ProductInfo {
                available: 0.0,
                price: 1.0,
                offered: 0.0,
                sold: 0.0,
                salability: 100.0,
                is_currency: true,
            });
            market.product_info.insert(3, ProductInfo {
                available: 0.0,
                price: 1.0,
                offered: 0.0,
                sold: 0.0,
                salability: 1.0,
                is_currency: false,
            });
            market.product_info.insert(4, ProductInfo {
                available: 0.0,
                price: 1.0,
                offered: 0.0,
                sold: 0.0,
                salability: 1.0,
                is_currency: false,
            });
            market.product_info.insert(5, ProductInfo {
                available: 0.0,
                price: 1.0,
                offered: 0.0,
                sold: 0.0,
                salability: 1.0,
                is_currency: false,
            });
            market.product_info.insert(6, ProductInfo {
                available: 0.0,
                price: 3.0,
                offered: 0.0,
                sold: 0.0,
                salability: 1.0,
                is_currency: false,
            });
            market.product_info.insert(7, ProductInfo {
                available: 0.0,
                price: 100.0,
                offered: 0.0,
                sold: 0.0,
                salability: 0.0,
                is_currency: false,
            });

            market.currencies.push(2);
            market.sale_priority.push(2);
            market.sale_priority.push(3);
            market.sale_priority.push(4);
            market.sale_priority.push(5);
            market.sale_priority.push(6);
            market.sale_priority.push(7);

            // it is asking for a unit of 1 unit of 7, which has an AMV of 45.0.
            let (offer, total) = pop.create_offer(7, 100.0, 
                &spend, &data, &market);

            assert_eq!(total, 70.0);
            assert_eq!(*offer.get(&2).unwrap(), 10.0);
            assert_eq!(*offer.get(&3).unwrap(), 10.0);
            assert_eq!(*offer.get(&4).unwrap(), 10.0);
            assert_eq!(*offer.get(&5).unwrap(), 10.0);
            assert_eq!(*offer.get(&6).unwrap(), 10.0);

        }

        #[test]
        pub fn should_return_exact_when_possible_no_fractional() {
            let pop = make_test_pop();

            let mut data = DataManager::new();

            // TODO when load_all is updated to take a file, relpace this with the 'default' load.
            data.load_test_data().expect("Error loading prefabs");

            // Add items to spend.
            let mut spend = HashMap::new();
            spend.insert(2, PropertyInfo::new(10.0));
            spend.insert(3, PropertyInfo::new(10.0));
            spend.insert(4, PropertyInfo::new(10.0));
            spend.insert(5, PropertyInfo::new(10.0));
            spend.insert(6, PropertyInfo::new(10.0));
            spend.insert(7, PropertyInfo::new(10.0));

            let mut market = MarketHistory {
                product_info: HashMap::new(),
                sale_priority: vec![],
                currencies: vec![],
                class_info: HashMap::new(),
                want_info: HashMap::new(),
            };
            market.product_info.insert(2, ProductInfo {
                available: 0.0,
                price: 1.0,
                offered: 0.0,
                sold: 0.0,
                salability: 100.0,
                is_currency: true,
            });
            market.product_info.insert(3, ProductInfo {
                available: 0.0,
                price: 1.0,
                offered: 0.0,
                sold: 0.0,
                salability: 1.0,
                is_currency: false,
            });
            market.product_info.insert(4, ProductInfo {
                available: 0.0,
                price: 1.0,
                offered: 0.0,
                sold: 0.0,
                salability: 1.0,
                is_currency: false,
            });
            market.product_info.insert(5, ProductInfo {
                available: 0.0,
                price: 1.0,
                offered: 0.0,
                sold: 0.0,
                salability: 1.0,
                is_currency: false,
            });
            market.product_info.insert(6, ProductInfo {
                available: 0.0,
                price: 3.0,
                offered: 0.0,
                sold: 0.0,
                salability: 1.0,
                is_currency: false,
            });
            market.product_info.insert(7, ProductInfo {
                available: 0.0,
                price: 100.0,
                offered: 0.0,
                sold: 0.0,
                salability: 0.0,
                is_currency: false,
            });

            market.currencies.push(2);
            market.sale_priority.push(2);
            market.sale_priority.push(3);
            market.sale_priority.push(4);
            market.sale_priority.push(5);
            market.sale_priority.push(6);
            market.sale_priority.push(7);

            // it is asking for a unit of 1 unit of 7, which has an AMV of 45.0.
            let (offer, total) = pop.create_offer(7, 55.0, 
                &spend, &data, &market);

            assert_eq!(total, 55.0);
            assert_eq!(*offer.get(&2).unwrap(), 10.0);
            assert_eq!(*offer.get(&3).unwrap(), 10.0);
            assert_eq!(*offer.get(&4).unwrap(), 10.0);
            assert_eq!(*offer.get(&5).unwrap(), 10.0);
            assert_eq!(*offer.get(&6).unwrap(), 5.0);
        }

        #[test]
        pub fn should_return_exact_when_possible_with_fractional() {
            let pop = make_test_pop();

            let mut data = DataManager::new();

            // TODO when load_all is updated to take a file, relpace this with the 'default' load.
            data.load_test_data().expect("Error loading prefabs");
            // make 6 fractional.
            data.products.get_mut(&6).unwrap().fractional = true;

            // Add items to spend.
            let mut spend = HashMap::new();
            spend.insert(2, PropertyInfo::new(10.0));
            spend.insert(3, PropertyInfo::new(10.0));
            spend.insert(4, PropertyInfo::new(10.0));
            spend.insert(5, PropertyInfo::new(10.0));
            spend.insert(6, PropertyInfo::new(10.0));
            spend.insert(7, PropertyInfo::new(10.0));

            let mut market = MarketHistory {
                product_info: HashMap::new(),
                sale_priority: vec![],
                currencies: vec![],
                class_info: HashMap::new(),
                want_info: HashMap::new(),
            };
            market.product_info.insert(2, ProductInfo {
                available: 0.0,
                price: 1.0,
                offered: 0.0,
                sold: 0.0,
                salability: 100.0,
                is_currency: true,
            });
            market.product_info.insert(3, ProductInfo {
                available: 0.0,
                price: 1.0,
                offered: 0.0,
                sold: 0.0,
                salability: 1.0,
                is_currency: false,
            });
            market.product_info.insert(4, ProductInfo {
                available: 0.0,
                price: 1.0,
                offered: 0.0,
                sold: 0.0,
                salability: 1.0,
                is_currency: false,
            });
            market.product_info.insert(5, ProductInfo {
                available: 0.0,
                price: 1.0,
                offered: 0.0,
                sold: 0.0,
                salability: 1.0,
                is_currency: false,
            });
            market.product_info.insert(6, ProductInfo {
                available: 0.0,
                price: 2.0,
                offered: 0.0,
                sold: 0.0,
                salability: 1.0,
                is_currency: false,
            });
            market.product_info.insert(7, ProductInfo {
                available: 0.0,
                price: 5.0,
                offered: 0.0,
                sold: 0.0,
                salability: 0.0,
                is_currency: false,
            });

            market.currencies.push(2);
            market.sale_priority.push(2);
            market.sale_priority.push(3);
            market.sale_priority.push(4);
            market.sale_priority.push(5);
            market.sale_priority.push(6);
            market.sale_priority.push(7);

            // it is asking for a unit of 1 unit of 7, which has an AMV of 45.0.
            let (offer, total) = pop.create_offer(7, 47.0, 
                &spend, &data, &market);

            assert_eq!(total, 47.0);
            assert_eq!(*offer.get(&2).unwrap(), 10.0);
            assert_eq!(*offer.get(&3).unwrap(), 10.0);
            assert_eq!(*offer.get(&4).unwrap(), 10.0);
            assert_eq!(*offer.get(&5).unwrap(), 10.0);
            assert_eq!(*offer.get(&6).unwrap(), 3.5);
        }

        #[test]
        pub fn should_return_within_limit() {
            let pop = make_test_pop();

            let mut data = DataManager::new();

            // TODO when load_all is updated to take a file, relpace this with the 'default' load.
            data.load_test_data().expect("Error loading prefabs");

            // Add items to spend.
            let mut spend = HashMap::new();
            spend.insert(2, PropertyInfo::new(10.0));
            spend.insert(3, PropertyInfo::new(10.0));
            spend.insert(4, PropertyInfo::new(10.0));
            spend.insert(5, PropertyInfo::new(10.0));
            spend.insert(6, PropertyInfo::new(10.0));
            spend.insert(7, PropertyInfo::new(10.0));

            let mut market = MarketHistory {
                product_info: HashMap::new(),
                sale_priority: vec![],
                currencies: vec![],
                class_info: HashMap::new(),
                want_info: HashMap::new(),
            };
            market.product_info.insert(2, ProductInfo {
                available: 0.0,
                price: 2.0,
                offered: 0.0,
                sold: 0.0,
                salability: 100.0,
                is_currency: true,
            });
            market.product_info.insert(3, ProductInfo {
                available: 0.0,
                price: 3.0,
                offered: 0.0,
                sold: 0.0,
                salability: 1.0,
                is_currency: false,
            });
            market.product_info.insert(4, ProductInfo {
                available: 0.0,
                price: 4.0,
                offered: 0.0,
                sold: 0.0,
                salability: 1.0,
                is_currency: false,
            });
            market.product_info.insert(5, ProductInfo {
                available: 0.0,
                price: 15.0,
                offered: 0.0,
                sold: 0.0,
                salability: 1.0,
                is_currency: false,
            });
            market.product_info.insert(6, ProductInfo {
                available: 0.0,
                price: 3.0,
                offered: 0.0,
                sold: 0.0,
                salability: 1.0,
                is_currency: false,
            });
            market.product_info.insert(7, ProductInfo {
                available: 0.0,
                price: 100.0,
                offered: 0.0,
                sold: 0.0,
                salability: 0.0,
                is_currency: false,
            });

            market.currencies.push(2);
            market.sale_priority.push(2);
            market.sale_priority.push(3);
            market.sale_priority.push(4);
            market.sale_priority.push(5);
            market.sale_priority.push(6);
            market.sale_priority.push(7);

            // it is asking for a unit of 1 unit of 7, which has an AMV of 45.0.
            let (offer, total) = pop.create_offer(7, 100.0, 
                &spend, &data, &market);

            assert_eq!(total, 102.0);
            assert_eq!(*offer.get(&2).unwrap(), 10.0);
            assert_eq!(*offer.get(&3).unwrap(), 10.0);
            assert_eq!(*offer.get(&4).unwrap(), 10.0);
            assert!(offer.get(&5).is_none());
            assert_eq!(*offer.get(&6).unwrap(), 4.0);
        }
    }

    // Completed
    mod standard_buy_should {
        use std::{collections::VecDeque, thread, time::Duration};
        use super::super::*;
        use super::{make_test_pop, prepare_data_for_market_actions};

        #[test]
        pub fn return_not_successful_when_not_in_stock() {
            let mut test = make_test_pop();
            let pop_info = test.actor_info();
            let (data, history) = prepare_data_for_market_actions(&mut test);
            // setup basic property for the pop.
            test.property.add_property(6, 10.0, &data);
            // setup message queue.
            let (tx, rx) = barrage::bounded(10);
            let mut passed_rx = rx.clone();
            let passed_tx = tx.clone();
            // setup firm we're talking with
            let seller = ActorInfo::Firm(1);
            // setup property split
            let handle = thread::spawn(move || {
                let result = test.standard_buy(&mut passed_rx, &passed_tx, &data, 
                    &history, 0.0, seller);
                (test, result)
            });
            // check that it's running
            if handle.is_finished() { assert!(false); }

            // send the out of stock message.
            tx.send(ActorMessage::NotInStock { buyer: pop_info, seller, product: 7 })
            .expect("Sudden Disconnect?");
            thread::sleep(Duration::from_millis(100));
            // ensure we closed out
            if !handle.is_finished() { assert!(false); }

            // get our data
            let (test, result) = handle.join().unwrap();
            // check the return is correct.
            if let BuyResult::NotSuccessful { reason } = result {
                assert_eq!(OfferResult::OutOfStock, reason);
            } else { assert!(false); }
            // check that nothing was gained or lost.
            assert!(test.property.property.get(&6).unwrap().total_property == 10.0);
            assert!(test.property.property.get(&7).is_none());
            // check pop memory for the products as well.
            // TODO check info here.
        }

        #[test]
        pub fn correctly_attempt_to_buy_input_value_when_its_higher() {
            let mut test = make_test_pop();
            let pop_info = test.actor_info();
            let (data, mut history) = prepare_data_for_market_actions(&mut test);
            // add in pop's property and sift their desires.
            // we have 20 extra food than we need (20*5=100.0 units)
            // this covers all food and leave excess for trading
            // This covers both species food desire and culture ambrosia fruit desire.
            test.property.add_property(2, 200.0, &data);
            // they have all the shelter they need via huts
            // 4 * 20 units
            // this covers both the shelter desire and the hut desire
            test.property.add_property(14, 80.0, &data);
            // the have all clothing needs (2-8) covered with 80 units
            // clothing culture desire is covered up to tier 85. (30 more than cabin at tier 50)
            // 20 * 5 units
            // they have 20.0 extra units available to trade.
            test.property.add_property(6, 100.0, &data);
            // missing desires are 10 cabins at tier 50, and the infinite
            // desire for 10 units of clothes every 10 tiers.
            // we want to target buying 10 cabins.
            let val = test.property.property.entry(15)
            .or_insert(PropertyInfo::new(0.0));
            val.upper_target = 10.0;
            val.lower_target = 0.0;

            // The pop is trying to buy 10 cabins at tier 50
            // It should always include the 20.0 units of Ambrosia fruit, 
            // which they have in excess.
            // They should also include 20.0 sets of clothes, which are available in excess.
            // Anything else offered would be 
            // set the prices of ambrosia fruit, clothes, and cabins so that the cabin is just purchaseable with
            // 20 ambrosia fruit and 20 cotton clothes.
            history.product_info.get_mut(&2).unwrap().price = 1.0; // 20.0 total
            history.product_info.get_mut(&6).unwrap().price = 2.0; // 40.0 total
            history.product_info.get_mut(&15).unwrap().price = 5.9; // 59.0 total

            // setup message queue.
            let (tx, rx) = barrage::bounded(10);
            let mut passed_rx = rx.clone();
            let passed_tx = tx.clone();
            // setup firm we're talking with
            let selling_firm = ActorInfo::Firm(1);
            // setup property split

            let handle = thread::spawn(move || {
                let result = test.standard_buy(&mut passed_rx, &passed_tx, 
                    &data, &history, 20.0, selling_firm);
                (test, result)
            });
            // check that it's running
            if handle.is_finished() { assert!(false); }

            // send the in stock message.
            tx.send(ActorMessage::InStock { buyer: pop_info, 
                seller: selling_firm, product: 15, price: 5.9, 
                quantity: 100.0 }).expect("Sudden Disconnect?");
            thread::sleep(Duration::from_millis(100));
            // should have the first message we sent
            if let ActorMessage::InStock { .. } = rx.recv().unwrap() {
            } else { assert!(false); }
            // it should have sent a buy order of 20.0 units of Ambrosia Fruit and 
            // 20.0 units of Clothes
            if let ActorMessage::BuyOffer { buyer, 
            seller, product, 
            price_opinion, quantity, 
            followup } = rx.recv().unwrap() {
                assert_eq!(buyer, pop_info);
                assert_eq!(seller, selling_firm);
                assert_eq!(product, 15);
                assert_eq!(price_opinion, OfferResult::Expensive);
                assert_eq!(quantity, 20.0);
                assert_eq!(followup, 2);
            } else { assert!(false); }
            // then check that the sent the expected food
            if let ActorMessage::BuyOfferFollowup { buyer, 
            seller, product, 
            offer_product, offer_quantity, 
            followup } = rx.recv().unwrap() {
                assert_eq!(buyer, pop_info);
                assert_eq!(seller, selling_firm);
                assert_eq!(product, 15);
                assert!(offer_product == 2);
                assert_eq!(offer_quantity, 99.0);
                assert_eq!(followup, 1);
            } else { assert!(false); }
            if let ActorMessage::BuyOfferFollowup { buyer, 
            seller, product, 
            offer_product, offer_quantity, 
            followup } = rx.recv().unwrap() {
                assert_eq!(buyer, pop_info);
                assert_eq!(seller, selling_firm);
                assert_eq!(product, 15);
                assert!(offer_product == 6);
                assert_eq!(offer_quantity, 10.0);
                assert_eq!(followup, 0);
            } else { assert!(false); }
            // with the offer sent correctly, send our acceptance and go forward
            tx.send(ActorMessage::SellerAcceptOfferAsIs { buyer: pop_info, 
                seller: selling_firm, product: 15, offer_result: OfferResult::Cheap })
                .expect("Disconnected?");
            thread::sleep(Duration::from_millis(100));
            // ensure we closed out
            if !handle.is_finished() { assert!(false); }
            // get our data
            let (test, result) = handle.join().unwrap();
            // check the return is correct.
            if let BuyResult::Successful = result {
                assert!(true);
            } else { assert!(false); }
            // check that property was exchanged
            assert!(test.property.property.get(&2).unwrap().total_property == 101.0);
            assert!(test.property.property.get(&6).unwrap().total_property == 90.0);
            assert!(test.property.property.get(&14).unwrap().total_property == 80.0);
            assert!(test.property.property.get(&15).unwrap().total_property == 20.0);
            // check records for the products as well.
            assert_eq!(test.property.property[&2].spent, 99.0);
            assert_eq!(test.property.property[&2].recieved, 0.0);
            assert_eq!(test.property.property[&6].spent, 10.0);
            assert_eq!(test.property.property[&6].recieved, 0.0);
            assert_eq!(test.property.property[&14].spent, 0.0);
            assert_eq!(test.property.property[&14].recieved, 0.0);
            assert_eq!(test.property.property[&15].spent, 0.0);
            assert_eq!(test.property.property[&15].recieved, 20.0);
            assert_eq!(test.property.property[&15].amv_cost, 119.0);
            // assert_eq!(test.property.property[&15].time_cost, test.standard_shop_time_cost());
        }

        #[test]
        pub fn return_success_when_able_to_buy_single_offer_no_change() {
            let mut test = make_test_pop();
            let pop_info = test.actor_info();
            let (data, mut history) = prepare_data_for_market_actions(&mut test);
            // add in pop's property and sift their desires.
            // we have 20 extra food than we need (20*5=100.0 units)
            // this covers all food and leave excess for trading
            // This covers both species food desire and culture ambrosia fruit desire.
            test.property.add_property(2, 120.0, &data);
            // they have all the shelter they need via huts
            // 4 * 20 units
            // this covers both the shelter desire and the hut desire
            test.property.add_property(14, 80.0, &data);
            // the have all clothing needs (2-8) covered with 80 units
            // clothing culture desire is covered up to tier 85. (30 more than cabin at tier 50)
            // 20 * 5 units
            // they have 20.0 extra units available to trade.
            test.property.add_property(6, 100.0, &data);
            // missing desires are 10 cabins at tier 50, and the infinite
            // desire for 10 units of clothes every 10 tiers.
            // we want to target buying 10 cabins.
            let val = test.property.property.entry(15)
            .or_insert(PropertyInfo::new(0.0));
            val.upper_target = 10.0;
            val.lower_target = 0.0;

            // The pop is trying to buy 10 cabins at tier 50
            // It should always include the 20.0 units of Ambrosia fruit, 
            // which they have in excess.
            // They should also include 20.0 sets of clothes, which are available in excess.
            // Anything else offered would be 
            // set the prices of ambrosia fruit, clothes, and cabins so that the cabin is just purchaseable with
            // 20 ambrosia fruit and 20 cotton clothes.
            history.product_info.get_mut(&2).unwrap().price = 1.0; // 20.0 total
            history.product_info.get_mut(&6).unwrap().price = 2.0; // 40.0 total
            history.product_info.get_mut(&15).unwrap().price = 5.9; // 59.0 total

            // setup message queue.
            let (tx, rx) = barrage::bounded(10);
            let mut passed_rx = rx.clone();
            let passed_tx = tx.clone();
            // setup firm we're talking with
            let selling_firm = ActorInfo::Firm(1);
            // setup property split

            let handle = thread::spawn(move || {
                let result = test.standard_buy(&mut passed_rx, &passed_tx, 
                    &data, &history, 0.0, selling_firm);
                (test, result)
            });
            // check that it's running
            if handle.is_finished() { assert!(false); }

            // send the in stock message.
            tx.send(ActorMessage::InStock { buyer: pop_info, 
                seller: selling_firm, product: 15, price: 5.9, 
                quantity: 100.0 }).expect("Sudden Disconnect?");
            thread::sleep(Duration::from_millis(100));
            // should have the first message we sent
            if let ActorMessage::InStock { .. } = rx.recv().unwrap() {
            } else { assert!(false); }
            // it should have sent a buy order of 20.0 units of Ambrosia Fruit and 
            // 20.0 units of Clothes
            if let ActorMessage::BuyOffer { buyer, 
            seller, product, 
            price_opinion, quantity, 
            followup } = rx.recv().unwrap() {
                assert_eq!(buyer, pop_info);
                assert_eq!(seller, selling_firm);
                assert_eq!(product, 15);
                assert_eq!(price_opinion, OfferResult::Cheap);
                assert_eq!(quantity, 10.0);
                assert_eq!(followup, 2);
            } else { assert!(false); }
            // then check that the sent the expected food
            if let ActorMessage::BuyOfferFollowup { buyer, 
            seller, product, 
            offer_product, offer_quantity, 
            followup } = rx.recv().unwrap() {
                assert_eq!(buyer, pop_info);
                assert_eq!(seller, selling_firm);
                assert_eq!(product, 15);
                assert!(offer_product == 2 || offer_product == 6);
                assert_eq!(offer_quantity, 20.0);
                assert_eq!(followup, 1);
            } else { assert!(false); }
            if let ActorMessage::BuyOfferFollowup { buyer, 
            seller, product, 
            offer_product, offer_quantity, 
            followup } = rx.recv().unwrap() {
                assert_eq!(buyer, pop_info);
                assert_eq!(seller, selling_firm);
                assert_eq!(product, 15);
                assert!(offer_product == 2 || offer_product == 6);
                assert_eq!(offer_quantity, 20.0);
                assert_eq!(followup, 0);
            } else { assert!(false); }
            // with the offer sent correctly, send our acceptance and go forward
            tx.send(ActorMessage::SellerAcceptOfferAsIs { buyer: pop_info, 
                seller: selling_firm, product: 15, offer_result: OfferResult::Cheap })
                .expect("Disconnected?");
            thread::sleep(Duration::from_millis(100));
            // ensure we closed out
            if !handle.is_finished() { assert!(false); }
            // get our data
            let (test, result) = handle.join().unwrap();
            // check the return is correct.
            if let BuyResult::Successful = result {
                assert!(true);
            } else { assert!(false); }
            // check that property was exchanged
            assert!(test.property.property.get(&2).unwrap().total_property == 100.0);
            assert!(test.property.property.get(&6).unwrap().total_property == 80.0);
            assert!(test.property.property.get(&14).unwrap().total_property == 80.0);
            assert!(test.property.property.get(&15).unwrap().total_property == 10.0);
            // check records for the products as well.
            assert_eq!(test.property.property[&2].spent, 20.0);
            assert_eq!(test.property.property[&2].recieved, 0.0);
            assert_eq!(test.property.property[&6].spent, 20.0);
            assert_eq!(test.property.property[&6].recieved, 0.0);
            assert_eq!(test.property.property[&14].spent, 0.0);
            assert_eq!(test.property.property[&14].recieved, 0.0);
            assert_eq!(test.property.property[&15].spent, 0.0);
            assert_eq!(test.property.property[&15].recieved, 10.0);
            assert_eq!(test.property.property[&15].amv_cost, 60.0);
            //assert_eq!(test.property.property[&15].time_cost, test.standard_shop_time_cost());
        }

        #[test]
        pub fn correctly_release_class_desire_for_buy_offer() {
            let mut test = make_test_pop();
            let pop_info = test.actor_info();
            let (data, mut history) = prepare_data_for_market_actions(&mut test);
            // swap out infinite clothes for the class desire instead.
            test.property.desires.get_mut(4).unwrap()
                .item = Item::Class(6);
            // add in pop's property and sift their desires.
            // we have 20 extra food than we need (20*5=100.0 units)
            // this covers all food and leave excess for trading
            // This covers both species food desire and culture ambrosia fruit desire.
            test.property.add_property(2, 120.0, &data);
            // they have all the shelter they need via huts
            // 4 * 20 units
            // this covers both the shelter desire and the hut desire
            test.property.add_property(14, 80.0, &data);
            // the have all clothing needs (2-8) covered with 80 units
            // clothing culture desire is covered up to tier 85. (30 more than cabin at tier 50)
            // 20 * 5 units
            // they have 20.0 extra units available to trade.
            test.property.add_property(6, 100.0, &data);
            // Also add in some suits to test out class swapping and ensure
            // class desires are correctly fulfilled and taken from as needed.
            test.property.add_property(7, 10.0, &data);
            // missing desires are 10 cabins at tier 50, and the infinite
            // desire for 10 units of clothes every 10 tiers.
            // we want to target buying 10 cabins.
            let val = test.property.property.entry(15)
            .or_insert(PropertyInfo::new(0.0));
            val.upper_target = 10.0;
            val.lower_target = 0.0;

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

            // setup message queue.
            let (tx, rx) = barrage::bounded(10);
            let mut passed_rx = rx.clone();
            let passed_tx = tx.clone();
            // setup firm we're talking with
            let selling_firm = ActorInfo::Firm(1);
            // setup property split

            let handle = thread::spawn(move || {
                let result = test.standard_buy(&mut passed_rx, &passed_tx, 
                    &data, &history, 0.0, selling_firm);
                (test, result)
            });
            // check that it's running
            if handle.is_finished() { assert!(false); }

            // send the in stock message.
            tx.send(ActorMessage::InStock { buyer: pop_info, 
                seller: selling_firm, product: 15, price: 5.9, 
                quantity: 100.0 }).expect("Sudden Disconnect?");
            thread::sleep(Duration::from_millis(100));
            // should have the first message we sent
            if let ActorMessage::InStock { .. } = rx.recv().unwrap() {
            } else { assert!(false); }
            // it should have sent a buy order of 20.0 units of Ambrosia Fruit and 
            // 20.0 units of Clothes
            if let ActorMessage::BuyOffer { buyer, 
            seller, product, 
            price_opinion, quantity, 
            followup } = rx.recv().unwrap() {
                assert_eq!(buyer, pop_info);
                assert_eq!(seller, selling_firm);
                assert_eq!(product, 15);
                assert_eq!(price_opinion, OfferResult::Cheap);
                assert_eq!(quantity, 10.0);
                assert_eq!(followup, 2);
            } else { assert!(false); }
            // then check that the sent the expected food and clothes.
            let mut msgs = vec![];
            while let Ok(msg) = rx.try_recv() {
                if msg.is_none() {
                    break;
                } else if let Some(val) = msg {
                    msgs.push(val);
                }
            }
            assert_eq!(msgs.len(), 2);
            let mut idx = 0;
            let mut total_amv = 0.0;
            for msg in msgs {
                if let ActorMessage::BuyOfferFollowup { buyer,
                seller, product, offer_product,
                offer_quantity, followup } = msg {
                    assert_eq!(buyer, pop_info);
                    assert_eq!(seller, selling_firm);
                    assert_eq!(product, 15);
                    assert_eq!(followup, 1-idx);
                    if offer_product == 2 {
                        //println!("{} {}s found.", offer_quantity, offer_product);
                        assert_eq!(offer_quantity, 20.0);
                        total_amv += offer_quantity * 1.0;
                    } else if offer_product == 7 { // Six is no longer assumed, as it seems ot be choosing 7 consistently now.
                        //println!("{} {}s found.", offer_quantity, offer_product);
                        assert_eq!(offer_quantity, 4.0);
                        total_amv += offer_quantity * 10.0;
                    } else {
                        assert!(false, "Product of: {} not expected.", offer_product);
                    }
                    idx += 1;
                }
            }
            //println!("Total AMV Recieved {}", total_amv);
            assert!(total_amv > 59.0);
            // with the offer sent correctly, send our acceptance and go forward
            tx.send(ActorMessage::SellerAcceptOfferAsIs { buyer: pop_info, 
                seller: selling_firm, product: 15, offer_result: OfferResult::Cheap })
                .expect("Disconnected?");
            thread::sleep(Duration::from_millis(100));
            // ensure we closed out
            if !handle.is_finished() { assert!(false); }
            // get our data
            let (test, result) = handle.join().unwrap();
            // check the return is correct.
            if let BuyResult::Successful = result {
                assert!(true);
            } else { assert!(false); }
            // check that property was exchanged
            assert!(test.property.property.get(&2).unwrap().total_property == 100.0);
            assert!(test.property.property.get(&6).unwrap().total_property == 100.0);
            assert_eq!(test.property.property[&6].spent, 0.0);
            assert_eq!(test.property.property[&6].recieved, 0.0);
            assert!(test.property.property.get(&7).unwrap().total_property == 6.0);
            assert_eq!(test.property.property[&7].spent, 4.0);
            assert_eq!(test.property.property[&7].recieved, 0.0);
            assert!(test.property.property.get(&14).unwrap().total_property == 80.0);
            assert!(test.property.property.get(&15).unwrap().total_property == 10.0);
            // check records for the products as well.
            assert_eq!(test.property.property[&2].spent, 20.0);
            assert_eq!(test.property.property[&2].recieved, 0.0);

            assert_eq!(test.property.property[&14].spent, 0.0);
            assert_eq!(test.property.property[&14].recieved, 0.0);
            assert_eq!(test.property.property[&15].spent, 0.0);
            assert_eq!(test.property.property[&15].recieved, 10.0);
            assert_eq!(test.property.property[&15].amv_cost, 60.0);
            //assert_eq!(test.property.property[&15].time_cost, test.standard_shop_time_cost());
        }

        #[test]
        pub fn correctly_release_want_desire_for_buy_offer() {
            let mut test = make_test_pop();
            let pop_info = test.actor_info();
            let (data, mut history) = prepare_data_for_market_actions(&mut test);
            // swap out infinite clothes for the want desire instead.
            test.property.desires.get_mut(4).unwrap()
                .item = Item::Want(2);
            // Add in pop's property and sift their desires.
            // We need 100.0 units for species food desire and the overlapping culture desire for ambrosia fruit.
            // We have an additional 10.0 units of food desire at 15+10n tier.
            // This adds an extra 40.0 units of food needed below our goal (cabin at tier 50).
            // we add 20.0 units above this threshold so they can be released and traded.
            test.property.add_property(2, 180.0, &data);
            // they have all the shelter they need via huts
            // 4 * 20 units
            // We add 20.0 units to keep the expense expected the same.
            // this covers both the shelter desire and the hut desire
            test.property.add_property(14, 80.0, &data);
            // the have all clothing needs (2-8) covered with 80 units
            // clothing culture desire is covered up to tier 85. (30 more than cabin at tier 50)
            // 20 * 5 units
            // they have 20.0 extra units available to trade.
            test.property.add_property(6, 100.0, &data);
            // missing desires are 10 cabins at tier 50, and the infinite
            // desire for 10 units of clothes every 10 tiers.
            // we want to target buying 10 cabins.
            let val = test.property.property.entry(15)
                .or_insert(PropertyInfo::new(0.0));
            val.upper_target = 10.0;
            val.lower_target = 0.0;

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

            // setup message queue.
            let (tx, rx) = barrage::bounded(10);
            let mut passed_rx = rx.clone();
            let passed_tx = tx.clone();
            // setup firm we're talking with
            let selling_firm = ActorInfo::Firm(1);
            // setup property split

            let handle = thread::spawn(move || {
                let result = test.standard_buy(&mut passed_rx, &passed_tx, 
                    &data, &history, 0.0, selling_firm);
                (test, result)
            });
            // check that it's running
            if handle.is_finished() { assert!(false); }

            // send the in stock message.
            tx.send(ActorMessage::InStock { buyer: pop_info, 
                seller: selling_firm, product: 15, price: 5.9, 
                quantity: 100.0 }).expect("Sudden Disconnect?");
            thread::sleep(Duration::from_millis(100));
            // should have the first message we sent
            if let ActorMessage::InStock { .. } = rx.recv().unwrap() {
            } else { assert!(false); }
            // it should have sent a buy order of 20.0 units of Ambrosia Fruit and 
            // 20.0 units of Clothes
            if let ActorMessage::BuyOffer { buyer, 
            seller, product, 
            price_opinion, quantity, 
            followup } = rx.recv().unwrap() {
                assert_eq!(buyer, pop_info);
                assert_eq!(seller, selling_firm);
                assert_eq!(product, 15);
                assert_eq!(price_opinion, OfferResult::Expensive);
                assert_eq!(quantity, 10.0);
                assert_eq!(followup, 2);
            } else { assert!(false); }
            // then check that the sent the expected food
            if let ActorMessage::BuyOfferFollowup { buyer, 
            seller, product, 
            offer_product, offer_quantity, 
            followup } = rx.recv().unwrap() {
                assert_eq!(buyer, pop_info);
                assert_eq!(seller, selling_firm);
                assert_eq!(product, 15);
                assert!(offer_product == 2);
                assert_eq!(offer_quantity, 20.0);
                assert_eq!(followup, 1);
            } else { assert!(false); }
            if let ActorMessage::BuyOfferFollowup { buyer, 
            seller, product, 
            offer_product, offer_quantity, 
            followup } = rx.recv().unwrap() {
                assert_eq!(buyer, pop_info);
                assert_eq!(seller, selling_firm);
                assert_eq!(product, 15);
                assert!(offer_product == 6);
                assert_eq!(offer_quantity, 20.0);
                assert_eq!(followup, 0);
            } else { assert!(false); }
            // with the offer sent correctly, send our acceptance and go forward
            tx.send(ActorMessage::SellerAcceptOfferAsIs { buyer: pop_info, 
                seller: selling_firm, product: 15, offer_result: OfferResult::Cheap })
                .expect("Disconnected?");
            thread::sleep(Duration::from_millis(100));
            // ensure we closed out
            if !handle.is_finished() { assert!(false); }
            // get our data
            let (test, result) = handle.join().unwrap();
            // check the return is correct.
            if let BuyResult::Successful = result {
                assert!(true);
            } else { assert!(false); }
            // check that property was exchanged
            assert!(test.property.property.get(&2).unwrap().total_property == 160.0);
            assert!(test.property.property.get(&6).unwrap().total_property == 80.0);
            assert!(test.property.property.get(&14).unwrap().total_property == 80.0);
            assert!(test.property.property.get(&15).unwrap().total_property == 10.0);
            // check records for the products as well.
            assert_eq!(test.property.property[&2].spent, 20.0);
            assert_eq!(test.property.property[&2].recieved, 0.0);
            assert_eq!(test.property.property[&6].spent, 20.0);
            assert_eq!(test.property.property[&6].recieved, 0.0);
            assert_eq!(test.property.property[&14].spent, 0.0);
            assert_eq!(test.property.property[&14].recieved, 0.0);
            assert_eq!(test.property.property[&15].spent, 0.0);
            assert_eq!(test.property.property[&15].recieved, 10.0);
            assert_eq!(test.property.property[&15].amv_cost, 60.0);
            //assert_eq!(test.property.property[&15].time_cost, test.standard_shop_time_cost());
        }

        #[test]
        pub fn cancel_purchase_when_target_reduced_and_sat_lost_is_greater_than_sat_gain() {
            // This results when the seller is a firm and the buyer is unable to get enough
            // AMV and not go over the Satisfaciton Gained. 
            // It takes the current available AMV, reduces the buy target to something more
            // reasonable based on that and updates Satisfaction gained.
            // If this reduction is large enough that they would end up losing satisfaction
            // in the trade, they will cancel the deal, and return NotSuccessful
            let mut test = make_test_pop();
            let pop_info = test.actor_info();
            let (data, mut history) = prepare_data_for_market_actions(&mut test);
            test.property.desires.get_mut(4).unwrap()
                .start = 51;
            test.property.desires.get_mut(4).unwrap()
                .step = 1;
            test.property.desires.get_mut(4).unwrap()
                .amount = 20.0;
            // add in pop's property and sift their desires.
            // we have 20 extra food than we need (20*5=100.0 units)
            // this covers all food and leave excess for trading
            // This covers both species food desire and culture ambrosia fruit desire.
            test.property.add_property(2, 100.0, &data);
            // they have all the shelter they need via huts
            // 4 * 20 units
            // this covers both the shelter desire and the hut desire
            test.property.add_property(14, 120.0, &data);
            // the have all clothing needs (2-8) covered with 80 units
            // clothing culture desire is covered up to tier 85. (30 more than cabin at tier 50)
            // 20 * 5 units
            // they have 20.0 extra units available to trade.
            test.property.add_property(6, 100.0, &data);
            // missing desires are 10 cabins at tier 50, and the infinite
            // desire for 10 units of clothes every 10 tiers.
            // we want to target buying 10 cabins.
            let val = test.property.property.entry(15)
            .or_insert(PropertyInfo::new(0.0));
            val.upper_target = 10.0;
            val.lower_target = 0.0;

            // The pop is trying to buy 10 cabins at tier 50
            // It should always include the 20.0 units of Ambrosia fruit, 
            // which they have in excess.
            // They should also include 20.0 sets of clothes, which are available in excess.
            // Anything else offered would be 
            // set the prices of ambrosia fruit, clothes, and cabins so that the cabin is just purchaseable with
            // 20 ambrosia fruit and 20 cotton clothes.
            history.product_info.get_mut(&2).unwrap().price = 1.0; // 20.0 total
            history.product_info.get_mut(&6).unwrap().price = 2.0; // 40.0 total
            history.product_info.get_mut(&15).unwrap().price = 15.9; // 59.0 total

            // setup message queue.
            let (tx, rx) = barrage::bounded(10);
            let mut passed_rx = rx.clone();
            let passed_tx = tx.clone();
            // setup firm we're talking with
            let selling_firm = ActorInfo::Firm(1);
            // setup property split

            let handle = thread::spawn(move || {
                let result = test.standard_buy(&mut passed_rx, &passed_tx, 
                    &data, &history, 0.0, selling_firm);
                (test, result)
            });
            // check that it's running
            if handle.is_finished() { assert!(false); }

            // send the in stock message.
            tx.send(ActorMessage::InStock { buyer: pop_info, 
                seller: selling_firm, product: 15, price: 15.9, 
                quantity: 100.0 }).expect("Sudden Disconnect?");
            thread::sleep(Duration::from_millis(100));
            // should have the first message we sent
            if let ActorMessage::InStock { .. } = rx.recv().unwrap() {
            } else { assert!(false); }
            // Check that the purchase failed correctly.
            if let ActorMessage::RejectPurchase { 
            price_opinion, .. } = rx.recv().unwrap() {
                assert_eq!(price_opinion, OfferResult::TooExpensive);
            } else { assert!(false, "Standard Buy Did Not Fall Through.") }
            let (test, result) = handle.join().unwrap();
            // ensure result is as expected
            if let BuyResult::NotSuccessful { reason } = result {
                assert_eq!(reason, OfferResult::TooExpensive);
            } else { assert!(false, "Did not return NotSuccessful") }
            // check that the pop did not modify or alter it's data in the process.
            assert_eq!(test.property.property.get(&2).unwrap().total_property, 100.0);
            assert_eq!(test.property.property.get(&2).unwrap().spent, 0.0);
            assert_eq!(test.property.property.get(&2).unwrap().recieved, 0.0);

            assert_eq!(test.property.property.get(&6).unwrap().total_property, 100.0);
            assert_eq!(test.property.property.get(&6).unwrap().spent, 0.0);
            assert_eq!(test.property.property.get(&6).unwrap().recieved, 0.0);

            assert_eq!(test.property.property.get(&14).unwrap().total_property, 120.0);
            assert_eq!(test.property.property.get(&14).unwrap().spent, 0.0);
            assert_eq!(test.property.property.get(&14).unwrap().recieved, 0.0);

            assert_eq!(test.property.property.get(&15).unwrap().total_property, 0.0);
            assert_eq!(test.property.property.get(&15).unwrap().spent, 0.0);
            assert_eq!(test.property.property.get(&15).unwrap().recieved, 0.0);
        }

        /// This should test that when unable to meet the targeted full 
        /// price of a desired set (price * target amount), that it instead
        /// targets a lower quantity. It should send out that buy offer 
        /// instead. We don't care about a further respones one way or another.
        #[test]
        pub fn send_reduced_purchase_when_unable_to_meet_full_pricepoint() {
            let mut test = Pop {
                id: 0,
                job: 0,
                firm: 0,
                market: 0,
                property: Property::new(vec![]),
                breakdown_table: PopBreakdownTable {
                    table: vec![],
                    total: 1,
                },
                is_selling: false,
                current_sat: TieredValue { tier: 0, value: 0.0 },
                prev_sat: TieredValue { tier: 0, value: 0.0 },
                hypo_change: TieredValue { tier: 0, value: 0.0 },
                backlog: VecDeque::new(),
            };
            // to ease getting the sweet spot, remove clothing desire.
            test.property.desires.push(
                Desire { item: Item::Product(15), start: 0, end: None, amount: 1.0, satisfaction: 0.0, step: 1, tags: vec![] }
            );
            let pop_info = test.actor_info();
            let (data, mut history) = prepare_data_for_market_actions(&mut test);
            // add in our property to exchange. 10 shirts, AMV value of 20.0 total
            test.property.add_property(6, 2.0, &data);
            let val = test.property.property.entry(15)
                .or_insert(PropertyInfo::new(0.0));
            val.upper_target = 2.0;
            val.lower_target = 0.0;

            // 2 clothes for targeting 2 cabins. Will only get 1. AMV fin market is arbitrarily high.
            history.product_info.get_mut(&6).unwrap().price = 2.0;
            history.product_info.get_mut(&15).unwrap().price = 1000.0;

            // setup message queue.
            let (tx, rx) = barrage::bounded(10);
            let mut passed_rx = rx.clone();
            let passed_tx = tx.clone();
            // setup firm we're talking with
            let selling_firm = ActorInfo::Firm(1);
            // setup property split

            let handle = thread::spawn(move || {
                let result = test.standard_buy(&mut passed_rx, &passed_tx, 
                    &data, &history, 0.0, selling_firm);
                (test, result)
            });
            // check that it's running
            if handle.is_finished() { assert!(false); }

            // send the in stock message.
            tx.send(ActorMessage::InStock { buyer: pop_info, 
                seller: selling_firm, product: 15, price: 2.5, 
                quantity: 100.0 }).expect("Sudden Disconnect?");
            thread::sleep(Duration::from_millis(100));
            // should have the first message we sent
            if let ActorMessage::InStock { .. } = rx.recv().unwrap() {
            } else { assert!(false); }

            // it should have sent a buy order of 20.0 units of Ambrosia Fruit and 
            // 20.0 units of Clothes
            if let ActorMessage::BuyOffer { buyer, 
            seller, product, 
            price_opinion, quantity, 
            followup } = rx.recv().unwrap() {
                assert_eq!(buyer, pop_info);
                assert_eq!(seller, selling_firm);
                assert_eq!(product, 15);
                assert_eq!(price_opinion, OfferResult::Steal);
                assert_eq!(quantity, 1.0);
                assert_eq!(followup, 1);
            } else { assert!(false); }
            // then check that the sent the expected food
            if let ActorMessage::BuyOfferFollowup { buyer, 
            seller, product, 
            offer_product, offer_quantity, 
            followup } = rx.recv().unwrap() {
                assert_eq!(buyer, pop_info);
                assert_eq!(seller, selling_firm);
                assert_eq!(product, 15);
                assert_eq!(offer_product, 6, "Incorrect Offered Product!");
                assert_eq!(offer_quantity, 2.0);
                assert_eq!(followup, 0);
            } else { assert!(false); }
            // with the offer sent correctly, send our acceptance and go forward
            tx.send(ActorMessage::SellerAcceptOfferAsIs { buyer: pop_info, 
                seller: selling_firm, product: 15, offer_result: OfferResult::Cheap })
                .expect("Disconnected?");
            thread::sleep(Duration::from_millis(100));
            // ensure we closed out
            if !handle.is_finished() { assert!(false); }
            // get our data
            let (test, result) = handle.join().unwrap();
            // check the return is correct.
            if let BuyResult::Successful = result {
                assert!(true);
            } else { assert!(false); }
            // check that property was exchanged
            assert_eq!(test.property.property.get(&6).unwrap().total_property, 0.0);
            assert_eq!(test.property.property.get(&15).unwrap().total_property, 1.0);
            // check records for the products as well.
            assert_eq!(test.property.property[&6].spent, 2.0);
            assert_eq!(test.property.property[&15].spent, 0.0);
            assert_eq!(test.property.property[&15].recieved, 1.0);
            assert_eq!(test.property.property[&15].amv_cost, 4.0);
            //assert_eq!(test.property.property[&15].time_cost, test.standard_shop_time_cost());
        }

        #[test]
        pub fn should_return_success_when_able_to_buy_single_offer_with_positive_change() {
            let mut test = make_test_pop();
            let pop_info = test.actor_info();
            let (data, mut history) = prepare_data_for_market_actions(&mut test);
            // add in pop's property and sift their desires.
            // we have 20 extra food than we need (20*5=100.0 units)
            // this covers all food and leave excess for trading
            // This covers both species food desire and culture ambrosia fruit desire.
            test.property.add_property(2, 120.0, &data);
            // they have all the shelter they need via huts
            // 4 * 20 units
            // this covers both the shelter desire and the hut desire
            test.property.add_property(14, 80.0, &data);
            // the have all clothing needs (2-8) covered with 80 units
            // clothing culture desire is covered up to tier 85. (30 more than cabin at tier 50)
            // 20 * 5 units
            // they have 20.0 extra units available to trade.
            test.property.add_property(6, 100.0, &data);
            // missing desires are 10 cabins at tier 50, and the infinite
            // desire for 10 units of clothes every 10 tiers.
            // we want to target buying 10 cabins.
            let val = test.property.property.entry(15)
            .or_insert(PropertyInfo::new(0.0));
            val.upper_target = 10.0;
            val.lower_target = 0.0;

            // The pop is trying to buy 10 cabins at tier 50
            // It should always include the 20.0 units of Ambrosia fruit, 
            // which they have in excess.
            // They should also include 20.0 sets of clothes, which are available in excess.
            // Anything else offered would be 
            // set the prices of ambrosia fruit, clothes, and cabins so that the cabin is just purchaseable with
            // 20 ambrosia fruit and 20 cotton clothes.
            history.product_info.get_mut(&2).unwrap().price = 1.0; // 20.0 total
            history.product_info.get_mut(&6).unwrap().price = 2.0; // 40.0 total
            history.product_info.get_mut(&15).unwrap().price = 5.9; // 59.0 total

            // setup message queue.
            let (tx, rx) = barrage::bounded(10);
            let mut passed_rx = rx.clone();
            let passed_tx = tx.clone();
            // setup firm we're talking with
            let selling_firm = ActorInfo::Firm(1);
            // setup property split

            let handle = thread::spawn(move || {
                let result = test.standard_buy(&mut passed_rx, &passed_tx, 
                    &data, &history, 0.0, selling_firm);
                (test, result)
            });
            // check that it's running
            if handle.is_finished() { assert!(false); }

            // send the in stock message.
            tx.send(ActorMessage::InStock { buyer: pop_info, 
                seller: selling_firm, product: 15, price: 5.9, 
                quantity: 100.0 }).expect("Sudden Disconnect?");
            thread::sleep(Duration::from_millis(100));
            // should have the first message we sent
            if let ActorMessage::InStock { .. } = rx.recv().unwrap() {
            } else { assert!(false); }
            // it should have sent a buy order of 20.0 units of Ambrosia Fruit and 
            // 20.0 units of Clothes
            let msg = rx.recv().unwrap();
            if let ActorMessage::BuyOffer { buyer, 
            seller, product, 
            price_opinion, quantity, 
            followup } = msg {
                assert_eq!(buyer, pop_info);
                assert_eq!(seller, selling_firm);
                assert_eq!(product, 15);
                assert_eq!(price_opinion, OfferResult::Cheap);
                assert_eq!(quantity, 10.0);
                assert_eq!(followup, 2);
            } else if let ActorMessage::RejectPurchase { .. } = msg {
                    assert!(false, "Reciveed RejectPurchase instead.")
            }
            else { assert!(false, "Did not recieve buy offer."); }
            // then check that the sent the expected food
            if let ActorMessage::BuyOfferFollowup { buyer, 
            seller, product, 
            offer_product, offer_quantity, 
            followup } = rx.recv().unwrap() {
                assert_eq!(buyer, pop_info);
                assert_eq!(seller, selling_firm);
                assert_eq!(product, 15);
                assert!(offer_product == 2 || offer_product == 6);
                assert_eq!(offer_quantity, 20.0);
                assert_eq!(followup, 1);
            } else { assert!(false); }
            if let ActorMessage::BuyOfferFollowup { buyer, 
            seller, product, 
            offer_product, offer_quantity, 
            followup } = rx.recv().unwrap() {
                assert_eq!(buyer, pop_info);
                assert_eq!(seller, selling_firm);
                assert_eq!(product, 15);
                assert!(offer_product == 2 || offer_product == 6);
                assert_eq!(offer_quantity, 20.0);
                assert_eq!(followup, 0);
            } else { assert!(false); }
            // with the offer sent correctly, send our acceptance and go forward
            tx.send(ActorMessage::OfferAcceptedWithChange { buyer: pop_info, 
                seller: selling_firm, product: 15, quantity: 10.0, followups: 1 })
                .expect("Disconnected?");
            tx.send(ActorMessage::ChangeFollowup { buyer: pop_info, seller: selling_firm,
                product: 15, return_product: 2, return_quantity: 5.0, 
                followups: 0 })
                .expect("Disconnected");
            thread::sleep(Duration::from_millis(100));
            // ensure we closed out
            if !handle.is_finished() { assert!(false); }
            // get our data
            let (test, result) = handle.join().unwrap();
            // check the return is correct.
            if let BuyResult::Successful = result {
                assert!(true);
            } else { assert!(false); }
            // check that property was exchanged
            // TODO check out why it's subtracting instead of adding the change.
            assert_eq!(test.property.property.get(&2).unwrap().total_property, 105.0);
            assert_eq!(test.property.property.get(&6).unwrap().total_property, 80.0);
            assert_eq!(test.property.property.get(&14).unwrap().total_property, 80.0);
            assert_eq!(test.property.property.get(&15).unwrap().total_property, 10.0);
            // check records for the products as well.
            assert_eq!(test.property.property[&2].spent, 15.0);
            assert_eq!(test.property.property[&2].recieved, 0.0);
            assert_eq!(test.property.property[&6].spent, 20.0);
            assert_eq!(test.property.property[&6].recieved, 0.0);
            assert_eq!(test.property.property[&14].spent, 0.0);
            assert_eq!(test.property.property[&14].recieved, 0.0);
            assert_eq!(test.property.property[&15].spent, 0.0);
            assert_eq!(test.property.property[&15].recieved, 10.0);
            assert_eq!(test.property.property[&15].amv_cost, 55.0);
            //assert_eq!(test.property.property[&15].time_cost, test.standard_shop_time_cost());
        }

        #[test]
        pub fn should_return_rejection_when_change_returned_with_negative_outcome() {
            let mut test = make_test_pop();
            let pop_info = test.actor_info();
            let (data, mut history) = prepare_data_for_market_actions(&mut test);
            test.property.desires.clear();
            test.property.desires.push(Desire::new(Item::Product(2), 
                0, None, 40.0, 
                0.0, 1, vec![]).unwrap());
            test.property.desires.push(Desire::new(Item::Product(15), 
                0, None, 40.0, 0.0, 1, vec![]).unwrap());
            // enough food for both tier 0, and tier 1.
            test.property.add_property(2, 80.0, &data);
            // a d
            let val = test.property.property.entry(15)
            .or_insert(PropertyInfo::new(0.0));
            val.upper_target = 40.0;
            val.lower_target = 0.0;

            // The pop is trying to buy 10 cabins at tier 50
            // It should always include the 20.0 units of Ambrosia fruit, 
            // which they have in excess.
            // They should also include 20.0 sets of clothes, which are available in excess.
            // Anything else offered would be 
            // set the prices of ambrosia fruit, clothes, and cabins so that the cabin is just purchaseable with
            // 20 ambrosia fruit and 20 cotton clothes.
            history.product_info.get_mut(&2).unwrap().price = 1.025;
            history.product_info.get_mut(&15).unwrap().price = 1.0;

            // setup message queue.
            let (tx, rx) = barrage::bounded(10);
            let mut passed_rx = rx.clone();
            let passed_tx = tx.clone();
            // setup firm we're talking with
            let selling_firm = ActorInfo::Firm(1);
            // setup property split

            let handle = thread::spawn(move || {
                let result = test.standard_buy(&mut passed_rx, &passed_tx, 
                    &data, &history, 0.0, selling_firm);
                (test, result)
            });
            // check that it's running
            if handle.is_finished() { assert!(false); }

            // send the in stock message.
            tx.send(ActorMessage::InStock { buyer: pop_info, 
                seller: selling_firm, product: 15, price: 1.0, 
                quantity: 100.0 }).expect("Sudden Disconnect?");
            thread::sleep(Duration::from_millis(100));
            // should have the first message we sent
            if let ActorMessage::InStock { .. } = rx.recv().unwrap() {
            } else { assert!(false); }
            // it should have sent a buy order of 20.0 units of Ambrosia Fruit and 
            // 20.0 units of Clothes
            if let ActorMessage::BuyOffer { buyer, 
            seller, product, 
            price_opinion, quantity, 
            followup } = rx.recv().unwrap() {
                assert_eq!(buyer, pop_info);
                assert_eq!(seller, selling_firm);
                assert_eq!(product, 15);
                assert_eq!(price_opinion, OfferResult::Expensive);
                assert_eq!(quantity, 40.0);
                assert_eq!(followup, 1);
            } else { assert!(false); }
            // then check that the sent the expected food
            if let ActorMessage::BuyOfferFollowup { buyer, 
            seller, product, 
            offer_product, offer_quantity, 
            followup } = rx.recv().unwrap() {
                assert_eq!(buyer, pop_info);
                assert_eq!(seller, selling_firm);
                assert_eq!(product, 15);
                assert!(offer_product == 2);
                assert_eq!(offer_quantity, 40.0);
                assert_eq!(followup, 0);
            } else { assert!(false); }
            // with the offer sent correctly, send our changed offer.
            tx.send(ActorMessage::OfferAcceptedWithChange { buyer: pop_info, 
                seller: selling_firm, product: 15, quantity: 20.0, followups: 0 })
                .expect("Disconnected?");
            thread::sleep(Duration::from_millis(100));
            // ensure we closed out
            if !handle.is_finished() { assert!(false); }
            // get our data
            let (test, result) = handle.join().unwrap();
            // pop out our change msg
            let _ = rx.recv().expect("Message not recieved!");
            // Get the reject purchase message
            if let ActorMessage::RejectPurchase { price_opinion,
                .. } = rx.recv().unwrap() {
                    // ignore other stuff as it should be the same.
                    assert_eq!(price_opinion, OfferResult::Rejected);
                } else { 
                    assert!(false, "Did not recieve Reject Purchase message") 
                }
            // check the return is correct.
            if let BuyResult::CancelBuy = result {
                assert!(true);
            } else { assert!(false); }
            // check that property was exchanged
            assert_eq!(test.property.property.get(&2).unwrap().total_property, 80.0);
            assert_eq!(test.property.property.get(&15).unwrap().total_property, 0.0);
            // check records for the products as well.
            assert_eq!(test.property.property[&2].spent, 0.0);
            assert_eq!(test.property.property[&2].recieved, 0.0);
            assert_eq!(test.property.property[&15].spent, 0.0);
            assert_eq!(test.property.property[&15].recieved, 00.0);
            assert_eq!(test.property.property[&15].amv_cost, 00.0);
            //assert_eq!(test.property.property[&15].time_cost, test.standard_shop_time_cost());
        }

        #[test]
        pub fn should_return_not_successful_when_reject_offer_recieved_after_offer() {
            // TODO add this test once this capability has been added.
        }

        #[test]
        pub fn should_return_not_successful_when_close_deal_recieved_after_offer() {
            let mut test = make_test_pop();
            let pop_info = test.actor_info();
            let (data, mut history) = prepare_data_for_market_actions(&mut test);
            // add in pop's property and sift their desires.
            // we have 20 extra food than we need (20*5=100.0 units)
            // this covers all food and leave excess for trading
            // This covers both species food desire and culture ambrosia fruit desire.
            test.property.add_property(2, 120.0, &data);
            // they have all the shelter they need via huts
            // 4 * 20 units
            // this covers both the shelter desire and the hut desire
            test.property.add_property(14, 80.0, &data);
            // the have all clothing needs (2-8) covered with 80 units
            // clothing culture desire is covered up to tier 85. (30 more than cabin at tier 50)
            // 20 * 5 units
            // they have 20.0 extra units available to trade.
            test.property.add_property(6, 100.0, &data);
            // missing desires are 10 cabins at tier 50, and the infinite
            // desire for 10 units of clothes every 10 tiers.
            // we want to target buying 10 cabins.
            let val = test.property.property.entry(15)
            .or_insert(PropertyInfo::new(0.0));
            val.upper_target = 10.0;
            val.lower_target = 0.0;

            // The pop is trying to buy 10 cabins at tier 50
            // It should always include the 20.0 units of Ambrosia fruit, 
            // which they have in excess.
            // They should also include 20.0 sets of clothes, which are available in excess.
            // Anything else offered would be 
            // set the prices of ambrosia fruit, clothes, and cabins so that the cabin is just purchaseable with
            // 20 ambrosia fruit and 20 cotton clothes.
            history.product_info.get_mut(&2).unwrap().price = 1.0; // 20.0 total
            history.product_info.get_mut(&6).unwrap().price = 2.0; // 40.0 total
            history.product_info.get_mut(&15).unwrap().price = 5.9; // 59.0 total

            // setup message queue.
            let (tx, rx) = barrage::bounded(10);
            let mut passed_rx = rx.clone();
            let passed_tx = tx.clone();
            // setup firm we're talking with
            let selling_firm = ActorInfo::Firm(1);
            // setup property split

            let handle = thread::spawn(move || {
                let result = test.standard_buy(&mut passed_rx, &passed_tx, 
                    &data, &history, 0.0, selling_firm);
                (test, result)
            });
            // check that it's running
            if handle.is_finished() { assert!(false); }

            // send the in stock message.
            tx.send(ActorMessage::InStock { buyer: pop_info, 
                seller: selling_firm, product: 15, price: 5.9, 
                quantity: 100.0 }).expect("Sudden Disconnect?");
            thread::sleep(Duration::from_millis(100));
            // should have the first message we sent
            if let ActorMessage::InStock { .. } = rx.recv().unwrap() {
            } else { assert!(false); }
            // it should have sent a buy order of 20.0 units of Ambrosia Fruit and 
            // 20.0 units of Clothes
            if let ActorMessage::BuyOffer { buyer, 
            seller, product, 
            price_opinion, quantity, 
            followup } = rx.recv().unwrap() {
                assert_eq!(buyer, pop_info);
                assert_eq!(seller, selling_firm);
                assert_eq!(product, 15);
                assert_eq!(price_opinion, OfferResult::Cheap);
                assert_eq!(quantity, 10.0);
                assert_eq!(followup, 2);
            } else { assert!(false); }
            // then check that the sent the expected food
            if let ActorMessage::BuyOfferFollowup { buyer, 
            seller, product, 
            offer_product, offer_quantity, 
            followup } = rx.recv().unwrap() {
                assert_eq!(buyer, pop_info);
                assert_eq!(seller, selling_firm);
                assert_eq!(product, 15);
                assert!(offer_product == 2 || offer_product == 6);
                assert_eq!(offer_quantity, 20.0);
                assert_eq!(followup, 1);
            } else { assert!(false); }
            if let ActorMessage::BuyOfferFollowup { buyer, 
            seller, product, 
            offer_product, offer_quantity, 
            followup } = rx.recv().unwrap() {
                assert_eq!(buyer, pop_info);
                assert_eq!(seller, selling_firm);
                assert_eq!(product, 15);
                assert!(offer_product == 2 || offer_product == 6);
                assert_eq!(offer_quantity, 20.0);
                assert_eq!(followup, 0);
            } else { assert!(false); }

            // with the offer sent correctly, send our acceptance and go forward
            tx.send(ActorMessage::RejectOffer { buyer: pop_info, 
            seller: selling_firm, product: 15 })
                .expect("Disconnected?");
            thread::sleep(Duration::from_millis(100));
            // ensure we closed out
            if !handle.is_finished() { assert!(false); }
            // get our data
            let (test, result) = handle.join().unwrap();
            // check the return is correct.
            if let BuyResult::NotSuccessful { reason } = result {
                assert_eq!(reason, OfferResult::Rejected);
                assert!(true);
            } else { assert!(false); }
            // check that property was exchanged
            assert_eq!(test.property.property.get(&2).unwrap().total_property, 120.0);
            assert_eq!(test.property.property.get(&6).unwrap().total_property, 100.0);
            assert_eq!(test.property.property.get(&14).unwrap().total_property, 80.0);
            assert_eq!(test.property.property.get(&15).unwrap().total_property, 0.0);
            // check records for the products as well.
            assert_eq!(test.property.property[&2].spent, 0.0);
            assert_eq!(test.property.property[&2].recieved, 0.0);
            assert_eq!(test.property.property[&6].spent, 0.0);
            assert_eq!(test.property.property[&6].recieved, 0.0);
            assert_eq!(test.property.property[&14].spent, 0.0);
            assert_eq!(test.property.property[&14].recieved, 0.0);
            assert_eq!(test.property.property[&15].spent, 0.0);
            assert_eq!(test.property.property[&15].recieved, 0.0);
            assert_eq!(test.property.property[&15].amv_cost, 0.0);
            //assert_eq!(test.property.property[&15].time_cost, test.standard_shop_time_cost());
        }

        // TODO test retry when reject offer recieved here!

        #[test]
        pub fn close_gracefully_when_close_deal_recieved() {
            let mut test = make_test_pop();
            let pop_info = test.actor_info();
            let (data, mut history) = prepare_data_for_market_actions(&mut test);
            // add in pop's property and sift their desires.
            // we have 20 extra food than we need (20*5=100.0 units)
            // this covers all food and leave excess for trading
            // This covers both species food desire and culture ambrosia fruit desire.
            test.property.add_property(2, 120.0, &data);
            // they have all the shelter they need via huts
            // 4 * 20 units
            // this covers both the shelter desire and the hut desire
            test.property.add_property(14, 80.0, &data);
            // the have all clothing needs (2-8) covered with 80 units
            // clothing culture desire is covered up to tier 85. (30 more than cabin at tier 50)
            // 20 * 5 units
            // they have 20.0 extra units available to trade.
            test.property.add_property(6, 100.0, &data);
            // missing desires are 10 cabins at tier 50, and the infinite
            // desire for 10 units of clothes every 10 tiers.
            // we want to target buying 10 cabins.
            let val = test.property.property.entry(15)
            .or_insert(PropertyInfo::new(0.0));
            val.upper_target = 10.0;
            val.lower_target = 0.0;

            // The pop is trying to buy 10 cabins at tier 50
            // It should always include the 20.0 units of Ambrosia fruit, 
            // which they have in excess.
            // They should also include 20.0 sets of clothes, which are available in excess.
            // Anything else offered would be 
            // set the prices of ambrosia fruit, clothes, and cabins so that the cabin is just purchaseable with
            // 20 ambrosia fruit and 20 cotton clothes.
            history.product_info.get_mut(&2).unwrap().price = 1.0; // 20.0 total
            history.product_info.get_mut(&6).unwrap().price = 2.0; // 40.0 total
            history.product_info.get_mut(&15).unwrap().price = 5.9; // 59.0 total

            // setup message queue.
            let (tx, rx) = barrage::bounded(10);
            let mut passed_rx = rx.clone();
            let passed_tx = tx.clone();
            // setup firm we're talking with
            let selling_firm = ActorInfo::Firm(1);
            // setup property split

            let handle = thread::spawn(move || {
                let result = test.standard_buy(&mut passed_rx, &passed_tx, 
                    &data, &history, 0.0, selling_firm);
                (test, result)
            });
            // check that it's running
            if handle.is_finished() { assert!(false); }

            // send the in stock message.
            tx.send(ActorMessage::InStock { buyer: pop_info, 
                seller: selling_firm, product: 15, price: 5.9, 
                quantity: 100.0 }).expect("Sudden Disconnect?");
            thread::sleep(Duration::from_millis(100));
            // should have the first message we sent
            if let ActorMessage::InStock { .. } = rx.recv().unwrap() {
            } else { assert!(false); }
            // it should have sent a buy order of 20.0 units of Ambrosia Fruit and 
            // 20.0 units of Clothes
            if let ActorMessage::BuyOffer { buyer, 
            seller, product, 
            price_opinion, quantity, 
            followup } = rx.recv().unwrap() {
                assert_eq!(buyer, pop_info);
                assert_eq!(seller, selling_firm);
                assert_eq!(product, 15);
                assert_eq!(price_opinion, OfferResult::Cheap);
                assert_eq!(quantity, 10.0);
                assert_eq!(followup, 2);
            } else { assert!(false); }
            // then check that the sent the expected food
            if let ActorMessage::BuyOfferFollowup { buyer, 
            seller, product, 
            offer_product, offer_quantity, 
            followup } = rx.recv().unwrap() {
                assert_eq!(buyer, pop_info);
                assert_eq!(seller, selling_firm);
                assert_eq!(product, 15);
                assert!(offer_product == 2 || offer_product == 6);
                assert_eq!(offer_quantity, 20.0);
                assert_eq!(followup, 1);
            } else { assert!(false); }
            if let ActorMessage::BuyOfferFollowup { buyer, 
            seller, product, 
            offer_product, offer_quantity, 
            followup } = rx.recv().unwrap() {
                assert_eq!(buyer, pop_info);
                assert_eq!(seller, selling_firm);
                assert_eq!(product, 15);
                assert!(offer_product == 2 || offer_product == 6);
                assert_eq!(offer_quantity, 20.0);
                assert_eq!(followup, 0);
            } else { assert!(false); }

            // with the offer sent correctly, send our acceptance and go forward
            tx.send(ActorMessage::CloseDeal { buyer: pop_info, 
            seller: selling_firm, product: 15 })
                .expect("Disconnected?");
            thread::sleep(Duration::from_millis(100));
            // ensure we closed out
            if !handle.is_finished() { assert!(false); }
            // get our data
            let (test, result) = handle.join().unwrap();
            // check the return is correct.
            if let BuyResult::SellerClosed = result {}
            else { assert!(false); }
            // check that property was exchanged
            assert_eq!(test.property.property.get(&2).unwrap().total_property, 120.0);
            assert_eq!(test.property.property.get(&6).unwrap().total_property, 100.0);
            assert_eq!(test.property.property.get(&14).unwrap().total_property, 80.0);
            assert_eq!(test.property.property.get(&15).unwrap().total_property, 0.0);
            // check records for the products as well.
            assert_eq!(test.property.property[&2].spent, 0.0);
            assert_eq!(test.property.property[&2].recieved, 0.0);
            assert_eq!(test.property.property[&6].spent, 0.0);
            assert_eq!(test.property.property[&6].recieved, 0.0);
            assert_eq!(test.property.property[&14].spent, 0.0);
            assert_eq!(test.property.property[&14].recieved, 0.0);
            assert_eq!(test.property.property[&15].spent, 0.0);
            assert_eq!(test.property.property[&15].recieved, 0.0);
            assert_eq!(test.property.property[&15].amv_cost, 0.0);
            //assert_eq!(test.property.property[&15].time_cost, test.standard_shop_time_cost());
        }
    }

    // Completed
    mod try_to_buy_should {
        use std::{thread, time::Duration};
        use  super::super::*;

        use super::{make_test_pop, prepare_data_for_market_actions};

        #[test]
        pub fn return_cancel_buy_when_market_price_is_too_high() {
            let mut test = make_test_pop();
            //let pop_info = test.actor_info();
            let (data, mut history) = prepare_data_for_market_actions(&mut test);
            // add in pop's property and sift their desires.
            // we have 20 extra food than we need (20*5=100.0 units)
            // this covers all food and leave excess for trading
            // This covers both species food desire and culture ambrosia fruit desire.
            test.property.add_property(2, 120.0, &data);
            // they have all the shelter they need via huts
            // 4 * 20 units
            // this covers both the shelter desire and the hut desire
            test.property.add_property(14, 80.0, &data);
            // the have all clothing needs (2-8) covered with 80 units
            // clothing culture desire is covered up to tier 85. (30 more than cabin at tier 50)
            // 20 * 5 units
            // they have 20.0 extra units available to trade.
            test.property.add_property(6, 100.0, &data);
            // missing desires are 10 cabins at tier 50, and the infinite
            // desire for 10 units of clothes every 10 tiers.
            // we want to target buying 10 cabins.
            let val = test.property.property.entry(15)
            .or_insert(PropertyInfo::new(0.0));
            val.amv_unit_estimate = 1.0;
            val.upper_target = 10.0;
            val.lower_target = 0.0;

            // The pop is trying to buy 10 cabins at tier 50
            // It should always include the 20.0 units of Ambrosia fruit, 
            // which they have in excess.
            // They should also include 20.0 sets of clothes, which are available in excess.
            // Anything else offered would be 
            // set the prices of ambrosia fruit, clothes, and cabins so that the cabin is just purchaseable with
            // 20 ambrosia fruit and 20 cotton clothes.
            history.product_info.get_mut(&2).unwrap().price = 1.0; // 20.0 total
            history.product_info.get_mut(&6).unwrap().price = 2.0; // 40.0 total
            history.product_info.get_mut(&15).unwrap().price = 5.9; // 59.0 total

            // setup message queue.
            let (tx, rx) = barrage::bounded(10);
            let mut passed_rx = rx.clone();
            let passed_tx = tx.clone();
            // setup firm we're talking with
            // let selling_firm = ActorInfo::Firm(1);

            // setup property split
            let handle = thread::spawn(move || {
                let result = test.try_to_buy(&mut passed_rx, &passed_tx, &data, 
                    &history, 15, 0.0);
                (test, result)
            });
            thread::sleep(Duration::from_millis(100));
            // since it's too expnesive, it should 
            assert!(handle.is_finished(), "Did not finish yet.");

            let (test, result) = handle.join().unwrap();
            if let BuyResult::CancelBuy = result { }
            else { assert!(false, "Did not recieve CancelBuy as expected!")}
            
            // ensure no change to property.
            assert_eq!(test.property.property.get(&2).unwrap().total_property, 120.0);
            assert_eq!(test.property.property.get(&2).unwrap().spent, 0.0);
            assert_eq!(test.property.property.get(&2).unwrap().recieved, 0.0);

            assert_eq!(test.property.property.get(&6).unwrap().total_property, 100.0);
            assert_eq!(test.property.property.get(&6).unwrap().spent, 0.0);
            assert_eq!(test.property.property.get(&6).unwrap().recieved, 0.0);

            assert_eq!(test.property.property.get(&14).unwrap().total_property, 80.0);
            assert_eq!(test.property.property.get(&14).unwrap().spent, 0.0);
            assert_eq!(test.property.property.get(&14).unwrap().recieved, 0.0);

            assert_eq!(test.property.property.get(&15).unwrap().total_property, 0.0);
            assert_eq!(test.property.property.get(&15).unwrap().spent, 0.0);
            assert_eq!(test.property.property.get(&15).unwrap().recieved, 0.0);
        }

        #[test]
        pub fn return_product_not_found_correctly() {
            let mut test = make_test_pop();
            let pop_info = test.actor_info();
            let (data, mut history) = prepare_data_for_market_actions(&mut test);
            // add in pop's property and sift their desires.
            // we have 20 extra food than we need (20*5=100.0 units)
            // this covers all food and leave excess for trading
            // This covers both species food desire and culture ambrosia fruit desire.
            test.property.add_property(2, 120.0, &data);
            // they have all the shelter they need via huts
            // 4 * 20 units
            // this covers both the shelter desire and the hut desire
            test.property.add_property(14, 80.0, &data);
            // the have all clothing needs (2-8) covered with 80 units
            // clothing culture desire is covered up to tier 85. (30 more than cabin at tier 50)
            // 20 * 5 units
            // they have 20.0 extra units available to trade.
            test.property.add_property(6, 100.0, &data);
            // missing desires are 10 cabins at tier 50, and the infinite
            // desire for 10 units of clothes every 10 tiers.
            // we want to target buying 10 cabins.
            let val = test.property.property.entry(15)
            .or_insert(PropertyInfo::new(0.0));
            val.amv_unit_estimate = 10.0;
            val.upper_target = 10.0;
            val.lower_target = 0.0;

            // The pop is trying to buy 10 cabins at tier 50
            // It should always include the 20.0 units of Ambrosia fruit, 
            // which they have in excess.
            // They should also include 20.0 sets of clothes, which are available in excess.
            // Anything else offered would be 
            // set the prices of ambrosia fruit, clothes, and cabins so that the cabin is just purchaseable with
            // 20 ambrosia fruit and 20 cotton clothes.
            history.product_info.get_mut(&2).unwrap().price = 1.0; // 20.0 total
            history.product_info.get_mut(&6).unwrap().price = 2.0; // 40.0 total
            history.product_info.get_mut(&15).unwrap().price = 5.9; // 59.0 total

            // setup message queue.
            let (tx, rx) = barrage::bounded(10);
            let mut passed_rx = rx.clone();
            let passed_tx = tx.clone();
            // setup firm we're talking with
            // let selling_firm = ActorInfo::Firm(1);

            // setup property split
            let handle = thread::spawn(move || {
                let result = test.try_to_buy(&mut passed_rx, &passed_tx, &data, 
                    &history, 15, 0.0);
                (test, result)
            });

            if handle.is_finished() {assert!(false, "Ended Prematurely!"); }

            tx.send(ActorMessage::ProductNotFound { product: 15, buyer: pop_info })
                .expect("Unexpected Disconnected.");

            thread::sleep(Duration::from_millis(100));
            if !handle.is_finished() { assert!(false, "Didn't end yet?!"); }

            let (test, result) = handle.join().unwrap();
            // check that everything is as expected.
            if let BuyResult::NotSuccessful { reason } = result {
                assert_eq!(reason, OfferResult::NotInMarket);
            } else { assert!(false, "Did not recieve NotSuccessful result!")}
            assert_eq!(test.property.property.get(&2).unwrap().total_property, 120.0);
            assert_eq!(test.property.property.get(&2).unwrap().spent, 0.0);
            assert_eq!(test.property.property.get(&2).unwrap().recieved, 0.0);

            assert_eq!(test.property.property.get(&6).unwrap().total_property, 100.0);
            assert_eq!(test.property.property.get(&6).unwrap().spent, 0.0);
            assert_eq!(test.property.property.get(&6).unwrap().recieved, 0.0);

            assert_eq!(test.property.property.get(&14).unwrap().total_property, 80.0);
            assert_eq!(test.property.property.get(&14).unwrap().spent, 0.0);
            assert_eq!(test.property.property.get(&14).unwrap().recieved, 0.0);

            assert_eq!(test.property.property.get(&15).unwrap().total_property, 0.0);
            assert_eq!(test.property.property.get(&15).unwrap().spent, 0.0);
            assert_eq!(test.property.property.get(&15).unwrap().recieved, 0.0);
        }

        #[test]
        pub fn successfully_reach_buy_and_return_as_normal() {
            let mut test = make_test_pop();
            let pop_info = test.actor_info();
            let (data, mut history) = prepare_data_for_market_actions(&mut test);
            // add in pop's property and sift their desires.
            // we have 20 extra food than we need (20*5=100.0 units)
            // this covers all food and leave excess for trading
            // This covers both species food desire and culture ambrosia fruit desire.
            test.property.add_property(2, 120.0, &data);
            // they have all the shelter they need via huts
            // 4 * 20 units
            // this covers both the shelter desire and the hut desire
            test.property.add_property(14, 80.0, &data);
            // the have all clothing needs (2-8) covered with 80 units
            // clothing culture desire is covered up to tier 85. (30 more than cabin at tier 50)
            // 20 * 5 units
            // they have 20.0 extra units available to trade.
            test.property.add_property(6, 100.0, &data);
            // missing desires are 10 cabins at tier 50, and the infinite
            // desire for 10 units of clothes every 10 tiers.
            // we want to target buying 10 cabins.
            let val = test.property.property.entry(15)
            .or_insert(PropertyInfo::new(0.0));
            val.amv_unit_estimate = 10.0;
            val.upper_target = 10.0;
            val.lower_target = 0.0;

            // The pop is trying to buy 10 cabins at tier 50
            // It should always include the 20.0 units of Ambrosia fruit, 
            // which they have in excess.
            // They should also include 20.0 sets of clothes, which are available in excess.
            // Anything else offered would be 
            // set the prices of ambrosia fruit, clothes, and cabins so that the cabin is just purchaseable with
            // 20 ambrosia fruit and 20 cotton clothes.
            history.product_info.get_mut(&2).unwrap().price = 1.0; // 20.0 total
            history.product_info.get_mut(&6).unwrap().price = 2.0; // 40.0 total
            history.product_info.get_mut(&15).unwrap().price = 5.9; // 59.0 total

            // setup message queue.
            let (tx, rx) = barrage::bounded(10);
            let mut passed_rx = rx.clone();
            let passed_tx = tx.clone();
            // setup firm we're talking with
            let selling_firm = ActorInfo::Firm(1);

            // setup property split
            let handle = thread::spawn(move || {
                let result = test.try_to_buy(&mut passed_rx, &passed_tx, &data, 
                    &history, 15, 0.0);
                (test, result)
            });

            if handle.is_finished() {assert!(false, "Ended Prematurely!"); }
            // check for find product
            if let ActorMessage::FindProduct { product, sender } = rx.recv().unwrap() {
                assert_eq!(product, 15);
                assert_eq!(sender, pop_info);
            } else { assert!(false, "Find Product message was not recived."); }
            // respond to the find message.
            tx.send(ActorMessage::FoundProduct { seller: selling_firm, buyer: pop_info, product: 15 })
                .expect("Unexpected Disconnected.");
            thread::sleep(Duration::from_millis(100));
            // should have the first message we sent
            if let ActorMessage::FoundProduct { .. } = rx.recv().unwrap() {
            } else { assert!(false, "Found Proudct message not recieved."); }

            tx.send(ActorMessage::InStock { buyer: pop_info, 
                seller: selling_firm, product: 15, price: 5.9, 
                quantity: 100.0 }).expect("Sudden Disconnect?");
            thread::sleep(Duration::from_millis(100));
            // should have the first message we sent
            if let ActorMessage::InStock { .. } = rx.recv().unwrap() {
            } else { assert!(false, "In stock message not recieved."); }
            // it should have sent a buy order of 20.0 units of Ambrosia Fruit and 
            // 20.0 units of Clothes
            if let ActorMessage::BuyOffer { buyer, 
            seller, product, 
            price_opinion, quantity, 
            followup } = rx.recv().unwrap() {
                assert_eq!(buyer, pop_info);
                assert_eq!(seller, selling_firm);
                assert_eq!(product, 15);
                assert_eq!(price_opinion, OfferResult::Cheap);
                assert_eq!(quantity, 10.0);
                assert_eq!(followup, 2);
            } else { assert!(false); }
            // then check that the sent the expected food
            if let ActorMessage::BuyOfferFollowup { buyer, 
            seller, product, 
            offer_product, offer_quantity, 
            followup } = rx.recv().unwrap() {
                assert_eq!(buyer, pop_info);
                assert_eq!(seller, selling_firm);
                assert_eq!(product, 15);
                assert!(offer_product == 2 || offer_product == 6);
                assert_eq!(offer_quantity, 20.0);
                assert_eq!(followup, 1);
            } else { assert!(false); }
            if let ActorMessage::BuyOfferFollowup { buyer, 
            seller, product, 
            offer_product, offer_quantity, 
            followup } = rx.recv().unwrap() {
                assert_eq!(buyer, pop_info);
                assert_eq!(seller, selling_firm);
                assert_eq!(product, 15);
                assert!(offer_product == 2 || offer_product == 6);
                assert_eq!(offer_quantity, 20.0);
                assert_eq!(followup, 0);
            } else { assert!(false); }
            // with the offer sent correctly, send our acceptance and go forward
            tx.send(ActorMessage::SellerAcceptOfferAsIs { buyer: pop_info, 
                seller: selling_firm, product: 15, offer_result: OfferResult::Cheap })
                .expect("Disconnected?");
            thread::sleep(Duration::from_millis(100));
            // ensure we closed out
            if !handle.is_finished() { assert!(false); }
            // get our data
            let (test, result) = handle.join().unwrap();
            // check the return is correct.
            if let BuyResult::Successful = result {
                assert!(true);
            } else { assert!(false, "Did not recieve Successful return."); }

            assert_eq!(test.property.property.get(&2).unwrap().total_property, 100.0);
            assert_eq!(test.property.property.get(&2).unwrap().spent, 20.0);
            assert_eq!(test.property.property.get(&2).unwrap().recieved, 0.0);

            assert_eq!(test.property.property.get(&6).unwrap().total_property, 80.0);
            assert_eq!(test.property.property.get(&6).unwrap().spent, 20.0);
            assert_eq!(test.property.property.get(&6).unwrap().recieved, 0.0);

            assert_eq!(test.property.property.get(&14).unwrap().total_property, 80.0);
            assert_eq!(test.property.property.get(&14).unwrap().spent, 0.0);
            assert_eq!(test.property.property.get(&14).unwrap().recieved, 0.0);

            assert_eq!(test.property.property.get(&15).unwrap().total_property, 10.0);
            assert_eq!(test.property.property.get(&15).unwrap().spent, 0.0);
            assert_eq!(test.property.property.get(&15).unwrap().recieved, 10.0);
        }
    }

    // Completed
    mod shopping_loop_should {
        use std::{collections::{HashMap, VecDeque}, thread, time::Duration};
        use super::super::*;

        /// Intentionally super simple pop generation
        /// 
        /// sets desires to 
        /// - 10 food want (0-9)
        /// - 5 clothes want (5-9)
        /// - 5 shelter want (5-9)
        /// - inf ambrosia fruit 10+
        /// - inf clothes class 10+
        /// - inf shelter class 15+
        pub fn default_pop() -> Pop {
            let mut result = Pop {
                id: 0,
                job: 0,
                firm: 0,
                market: 0,
                property: Property::new(vec![]),
                breakdown_table: PopBreakdownTable { table: vec![], total: 0 },
                is_selling: true,
                current_sat: TieredValue { tier: 0, value: 0.0 },
                prev_sat: TieredValue { tier: 0, value: 0.0 },
                hypo_change: TieredValue { tier: 0, value: 0.0 },
                backlog: VecDeque::new(),
            };

            // don't care about pop breakdown. it's not actually being used.
            result.breakdown_table.insert_pops(
                PBRow::new(0, None, None, 
                    None, None, None, 
                    None, None, None, 
                    1));

            // Food 0
            result.property.desires.push(
                Desire{
                    item: Item::Want(2),
                    start: 0,
                    end: Some(9),
                    amount: 1.0,
                    satisfaction: 0.0,
                    step: 1,
                    tags: vec![],
                }
            );

            // Shelter 1
            result.property.desires.push(
                Desire{
                    item: Item::Want(3),
                    start: 5,
                    end: Some(9),
                    amount: 1.0,
                    satisfaction: 0.0,
                    step: 1,
                    tags: vec![],
                }
            );

            // Clothing 2
            result.property.desires.push(
                Desire{
                    item: Item::Want(4),
                    start: 5,
                    end: Some(9),
                    amount: 1.0,
                    satisfaction: 0.0,
                    step: 1,
                    tags: vec![],
                }
            );

            // ambrosia fruit 3
            result.property.desires.push(
                Desire{
                    item: Item::Product(2),
                    start: 10,
                    end: None,
                    amount: 1.0,
                    satisfaction: 0.0,
                    step: 1,
                    tags: vec![],
                }
            );

            // clothes 4
            result.property.desires.push(
                Desire{
                    item: Item::Class(6),
                    start: 15,
                    end: None,
                    amount: 1.0,
                    satisfaction: 0.0,
                    step: 1,
                    tags: vec![],
                }
            );

            // Hut/Cabin 5
            result.property.desires.push(
                Desire{
                    item: Item::Class(14),
                    start: 15,
                    end: None,
                    amount: 1.0,
                    satisfaction: 0.0,
                    step: 1,
                    tags: vec![],
                }
            );

            result
        }

        /// preps a pop's property, the property's data, and market prices of those items.
        /// 
        /// Sets all values to 1.0 amv and salability of 0.5 by default.
        /// 
        /// Exceptions are:
        /// - Ambrosia Fruit are set as a currency (Sal 1.0, currency=true)
        /// - Cotton Boll is set to currency with salability 1.0, and price of 5.0
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

            market.product_info.get_mut(&3).expect("Brok").salability = 1.0;
            market.product_info.get_mut(&3).expect("Brok").is_currency = true;
            market.product_info.get_mut(&3).expect("Brok").price = 5.0;

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

        // TODO add test for upgrading buy_quantity below max up to max.

        #[test]
        pub fn stop_when_out_of_time() {
            let mut test = default_pop();
            let pop_info = test.actor_info();
            let (data, history) = prepare_data_for_market_actions(&mut test);
            let seller = ActorInfo::Firm(1);
            
            // add the initial property of the pop we'll be using\
            // 20 ambrosia fruit, cotton clothes, huts, and cotton bolls
            test.property.add_property(2, 4.0, &data);
            test.property.add_property(3, 20.0, &data);
            test.property.add_property(6, 3.0, &data);
            test.property.add_property(14, 3.0, &data);
            // add in 1.1 shopping trip worth of time
            test.property.add_property(TIME_PRODUCT_ID, 
                1.1 * test.standard_shop_time_cost(), &data);

            // setup message queue.
            let (tx, rx) = barrage::bounded(10);
            let mut passed_rx = rx.clone();
            let mut passed_tx = tx.clone();
            // setup firm we're talking with
            // let selling_firm = ActorInfo::Firm(1);
            let mut unsat_desires = vec![];
            unsat_desires.push(test.property.get_first_unsatisfied_desire().unwrap());
            for _ in 0..5 {
                unsat_desires.push(test.property.walk_up_tiers(
                    Some(*unsat_desires.last().unwrap())).unwrap());
            }

            // setup property split
            let handle = thread::spawn(move || {
                Pop::shopping_loop(&mut test, &mut passed_rx, &mut passed_tx, &data, 
                    &history);
                test
            });
            thread::sleep(Duration::from_millis(100));

            // first want recieved. tier 1, idx 0, food want.
            if let ActorMessage::FindWant { want, sender } = rx
            .recv().expect("Unexpected Disconnect.") {
                //println!("Find Want Recieved.");
                assert_eq!(want, 2, "Want incorrect.");
                assert_eq!(sender, pop_info, "Incorrect sender?");
            } else {
                assert!(false, "FindWant not recieved.")
            }

            // send want found message with ambrosia fruit consumption (13)
            tx.send(ActorMessage::FoundWant { buyer: pop_info, want: 2, process: 13 })
                .expect("Sudden Disconnect!");
            // clear out the message just sent.
            rx.recv().expect("Broke.");

            thread::sleep(Duration::from_millis(100));

            if let ActorMessage::FindProduct { product, sender } = 
            rx.recv().expect("Broke.") {
                //println!("Find Product Recieved.");
                assert_eq!(product, 2, "Incorrect Product.");
                assert_eq!(sender, pop_info, "Incorrect sender");
            } else {
                assert!(false, "FindProduct not recieved.");
            }

            // send product found response
            tx.send(ActorMessage::FoundProduct { seller, 
                buyer: pop_info, product: 2 }).expect("Broke.");
            rx.recv().expect("Broke");

            // send in stock message, since seller has customer.
            tx.send(ActorMessage::InStock { buyer: pop_info, seller, 
                product: 2, price: 1.0, quantity: 1000.0 }).expect("Broke.");
            rx.recv().expect("Broke");

            if let ActorMessage::BuyOffer { buyer, seller, product, 
            price_opinion, quantity, followup } = rx.recv().expect("Broke") {
                //println!("Buy Offer Recieved.");
                assert_eq!(buyer, pop_info, "wrong buyer.");
                assert_eq!(seller, seller, "wrong seller.");
                assert_eq!(product, 2, "wrong product.");
                assert_eq!(price_opinion, OfferResult::Reasonable, "wrong oppinion.");
                assert_eq!(quantity, 1.0, "wrong quantity.");
                assert_eq!(followup, 1, "wrong followups.");
            } else {
                assert!(false, "buy offer not recieved.")
            }

            if let ActorMessage::BuyOfferFollowup { buyer, seller,
            product, offer_product, offer_quantity, followup }
            = rx.recv().expect("Broke.") {
                //println!("Buy Offer Followup Recieved.");
                assert_eq!(buyer, pop_info);
                assert_eq!(seller, seller);
                assert_eq!(product, 2);
                assert_eq!(offer_product, 3);
                assert_eq!(offer_quantity, 1.0);
                assert_eq!(followup, 0);
            } else {
                assert!(false, "Wrong Message.");
            }

            // send back accept message.
            tx.send(ActorMessage::SellerAcceptOfferAsIs { buyer: pop_info, 
                seller, product: 2, offer_result: OfferResult::Reasonable })
                .expect("borkd");
            rx.recv().expect("borkd");

            // Deal completed, should also finish shopping, check for shop as predicted.
            // it should've bought 1.0 units of 2 for 1.0 units of 3 and 0.2 units of time/shopping time.
            let test = handle.join().unwrap();
            let food_info = test.property.property[&2];
            let cotton_info = test.property.property[&3];
            let hut_info = test.property.property[&14];
            let time_info = test.property.property[&TIME_PRODUCT_ID];
            assert!(test.property.property.get(&SHOPPING_TIME_PRODUCT_ID).is_none(), "Shopping Time Found.");
            // check that we recorded our expenditure in time and AMV
            assert_eq!(food_info.total_property, 5.0);
            assert_eq!(food_info.time_cost, test.standard_shop_time_cost());
            assert_eq!(food_info.amv_cost, 5.0);
            assert_eq!(food_info.recieved, 1.0);
            // cotton was expended for food
            assert_eq!(cotton_info.total_property, 19.0);
            assert_eq!(cotton_info.spent, 1.0);
            // huts were untouched
            assert_eq!(hut_info.total_property, 3.0);
            // time was spent for shopping
            assert_eq!(time_info.total_property, (1.1 * test.standard_shop_time_cost())-test.standard_shop_time_cost());
        }

        #[test]
        pub fn stop_when_no_desires_remain() {
            let mut test = default_pop();
            let pop_info = test.actor_info();
            let (data, history) = prepare_data_for_market_actions(&mut test);
            let seller = ActorInfo::Firm(1);
            // alter desires to run out of desires.
            // food 
            test.property.clear_desires();
            test.property.add_desire(&Desire::new(Item::Want(2), 0, 
                Some(10), 1.0, 0.0, 1, vec![]).unwrap());
            
            // add the initial property of the pop we'll be using\
            // 20 ambrosia fruit, cotton clothes, huts, and cotton bolls
            test.property.add_property(2, 10.0, &data);
            test.property.add_property(3, 100.0, &data);
            test.property.add_property(6, 10.0, &data);
            test.property.add_property(14, 10.0, &data);
            // add in way to much shopping time.
            test.property.add_property(TIME_PRODUCT_ID, 
                100.0 * test.standard_shop_time_cost(), &data);

            // setup message queue.
            let (tx, rx) = barrage::bounded(10);
            let mut passed_rx = rx.clone();
            let mut passed_tx = tx.clone();

            // get loop running
            let handle = thread::spawn(move || {
                Pop::shopping_loop(&mut test, &mut passed_rx, &mut passed_tx, &data, 
                    &history);
                test
            });
            thread::sleep(Duration::from_millis(100));

            // first want recieved. tier 1, idx 0, food want.
            if let ActorMessage::FindWant { want, sender } = rx
            .recv().expect("Unexpected Disconnect.") {
                //println!("Find Want Recieved.");
                assert_eq!(want, 2, "Want incorrect.");
                assert_eq!(sender, pop_info, "Incorrect sender?");
            } else {
                assert!(false, "FindWant not recieved.")
            }

            // send want found message with ambrosia fruit consumption (13)
            tx.send(ActorMessage::FoundWant { buyer: pop_info, want: 2, process: 13 })
                .expect("Sudden Disconnect!");
            // clear out the message just sent.
            rx.recv().expect("Broke.");

            thread::sleep(Duration::from_millis(100));

            if let ActorMessage::FindProduct { product, sender } = 
            rx.recv().expect("Broke.") {
                //println!("Find Product Recieved.");
                assert_eq!(product, 2, "Incorrect Product.");
                assert_eq!(sender, pop_info, "Incorrect sender");
            } else {
                assert!(false, "FindProduct not recieved.");
            }

            // send product found response
            tx.send(ActorMessage::FoundProduct { seller, 
                buyer: pop_info, product: 2 }).expect("Broke.");
            rx.recv().expect("Broke");

            // send in stock message, since seller has customer.
            tx.send(ActorMessage::InStock { buyer: pop_info, seller, 
                product: 2, price: 1.0, quantity: 1000.0 }).expect("Broke.");
            rx.recv().expect("Broke");

            if let ActorMessage::BuyOffer { buyer, seller, product, 
            price_opinion, quantity, followup } = rx.recv().expect("Broke") {
                //println!("Buy Offer Recieved.");
                assert_eq!(buyer, pop_info, "wrong buyer.");
                assert_eq!(seller, seller, "wrong seller.");
                assert_eq!(product, 2, "wrong product.");
                assert_eq!(price_opinion, OfferResult::TooExpensive, "wrong oppinion.");
                assert_eq!(quantity, 1.0, "wrong quantity.");
                assert_eq!(followup, 1, "wrong followups.");
            } else {
                assert!(false, "buy offer not recieved.")
            }

            if let ActorMessage::BuyOfferFollowup { buyer, seller,
            product, offer_product, offer_quantity, followup }
            = rx.recv().expect("Broke.") {
                println!("Buy Offer Followup Recieved.");
                assert_eq!(buyer, pop_info);
                assert_eq!(seller, seller);
                assert_eq!(product, 2);
                assert_eq!(offer_product, 6);
                assert_eq!(offer_quantity, 1.0);
                assert_eq!(followup, 0);
            } else {
                assert!(false, "Wrong Message.");
            }

            // send back accept message.
            tx.send(ActorMessage::SellerAcceptOfferAsIs { buyer: pop_info, 
                seller, product: 2, offer_result: OfferResult::Reasonable })
                .expect("borkd");
            rx.recv().expect("borkd");

            // Deal completed, should also finish shopping, check for shop as predicted.
            // it should've bought 1.0 units of 2 for 1.0 units of 3 and 0.2 units of time/shopping time.
            let test = handle.join().unwrap();
            let food_info = test.property.property[&2];
            let cotton_info = test.property.property[&3];
            let hut_info = test.property.property[&14];
            let time_info = test.property.property[&TIME_PRODUCT_ID];
            assert!(test.property.property.get(&SHOPPING_TIME_PRODUCT_ID).is_none(), "Shopping Time Found.");
            // check that we recorded our expenditure in time and AMV
            assert_eq!(food_info.total_property, 11.0);
            assert_eq!(food_info.time_cost, test.standard_shop_time_cost());
            assert_eq!(food_info.amv_cost, 10.0);
            assert_eq!(food_info.recieved, 1.0);
            // cotton was expended for food
            assert_eq!(cotton_info.total_property, 100.0);
            assert_eq!(cotton_info.spent, 0.0);
            // huts were untouched
            assert_eq!(hut_info.total_property, 10.0);
            // time was spent for shopping
            assert_eq!(time_info.total_property, (100.0 * test.standard_shop_time_cost())-test.standard_shop_time_cost());
        }

        #[test]
        pub fn reattempt_purchase_once_before_cancelling_and_moving_on() {
            let mut test = default_pop();
            let pop_info = test.actor_info();
            let (data, history) = prepare_data_for_market_actions(&mut test);
            let seller = ActorInfo::Firm(1);
            // alter desires to run out of desires.
            // food 
            test.property.clear_desires();
            test.property.add_desire(&Desire::new(Item::Want(2), 0, 
                None, 1.0, 0.0, 1, vec![]).unwrap());
            
            // add the initial property of the pop we'll be using\
            // 20 ambrosia fruit, cotton clothes, huts, and cotton bolls
            test.property.add_property(2, 10.0, &data);
            test.property.add_property(3, 100.0, &data);
            test.property.add_property(6, 10.0, &data);
            test.property.add_property(14, 10.0, &data);
            // add in way to much shopping time.
            test.property.add_property(TIME_PRODUCT_ID, 
                100.0 * test.standard_shop_time_cost(), &data);

            // setup message queue.
            let (tx, rx) = barrage::bounded(10);
            let mut passed_rx = rx.clone();
            let mut passed_tx = tx.clone();

            // get loop running
            let handle = thread::spawn(move || {
                Pop::shopping_loop(&mut test, &mut passed_rx, &mut passed_tx, &data, 
                    &history);
                test
            });
            thread::sleep(Duration::from_millis(100));

            // first want recieved. tier 1, idx 0, food want.
            if let ActorMessage::FindWant { want, sender } = rx
            .recv().expect("Unexpected Disconnect.") {
                //println!("Find Want Recieved.");
                assert_eq!(want, 2, "Want incorrect.");
                assert_eq!(sender, pop_info, "Incorrect sender?");
            } else {
                assert!(false, "FindWant not recieved.")
            }

            // send want found message with ambrosia fruit consumption (13)
            tx.send(ActorMessage::FoundWant { buyer: pop_info, want: 2, process: 13 })
                .expect("Sudden Disconnect!");
            // clear out the message just sent.
            rx.recv().expect("Broke.");

            thread::sleep(Duration::from_millis(100));

            if let ActorMessage::FindProduct { product, sender } = 
            rx.recv().expect("Broke.") {
                //println!("Find Product Recieved.");
                assert_eq!(product, 2, "Incorrect Product.");
                assert_eq!(sender, pop_info, "Incorrect sender");
            } else {
                assert!(false, "FindProduct not recieved.");
            }

            // send product found response
            tx.send(ActorMessage::FoundProduct { seller, 
                buyer: pop_info, product: 2 }).expect("Broke.");
            rx.recv().expect("Broke");

            // send in stock message, since seller has customer.
            tx.send(ActorMessage::NotInStock { buyer: pop_info, seller, product: 2 })
                .expect("Broke.");
            rx.recv().expect("Broke");

            // pop tried to buy once and failed, they'll try again, back to the start.

            // first want recieved. tier 1, idx 0, food want.
            if let ActorMessage::FindWant { want, sender } = rx
            .recv().expect("Unexpected Disconnect.") {
                //println!("Find Want Recieved.");
                assert_eq!(want, 2, "Want incorrect.");
                assert_eq!(sender, pop_info, "Incorrect sender?");
            } else {
                assert!(false, "FindWant not recieved.")
            }
            
            // send want found message with ambrosia fruit consumption (13)
            tx.send(ActorMessage::FoundWant { buyer: pop_info, want: 2, process: 13 })
                .expect("Sudden Disconnect!");
            // clear out the message just sent.
            rx.recv().expect("Broke.");

            thread::sleep(Duration::from_millis(100));

            if let ActorMessage::FindProduct { product, sender } = 
            rx.recv().expect("Broke.") {
                //println!("Find Product Recieved.");
                assert_eq!(product, 2, "Incorrect Product.");
                assert_eq!(sender, pop_info, "Incorrect sender");
            } else {
                assert!(false, "FindProduct not recieved.");
            }

            // send product found response
            tx.send(ActorMessage::FoundProduct { seller, 
                buyer: pop_info, product: 2 }).expect("Broke.");
            rx.recv().expect("Broke");

            // send in stock message, since seller has customer.
            tx.send(ActorMessage::NotInStock { buyer: pop_info, seller, product: 2 })
                .expect("Broke.");
            rx.recv().expect("Broke");

            // Failed twice, it should marke it as complete and exit out.
            thread::sleep(Duration::from_millis(1000));
            if !handle.is_finished() {
                assert!(false, "Did not finish yet?")
            }

            // Deal completed, should also finish shopping, check for shop as predicted.
            // it should've bought 1.0 units of 2 for 1.0 units of 3 and 0.2 units of time/shopping time.
            let test = handle.join().unwrap();
            let food_info = test.property.property[&2];
            let cotton_info = test.property.property[&3];
            let hut_info = test.property.property[&14];
            let time_info = test.property.property[&TIME_PRODUCT_ID];
            assert!(test.property.property.get(&SHOPPING_TIME_PRODUCT_ID).is_none(), "Shopping Time Found.");
            // check that we recorded our expenditure in time and AMV
            assert_eq!(food_info.total_property, 10.0);
            assert_eq!(food_info.time_cost, 2.0 * test.standard_shop_time_cost());
            assert_eq!(food_info.amv_cost, 0.0);
            assert_eq!(food_info.recieved, 0.0);
            // cotton was expended for food
            assert_eq!(cotton_info.total_property, 100.0);
            assert_eq!(cotton_info.spent, 0.0);
            // huts were untouched
            assert_eq!(hut_info.total_property, 10.0);
            // time was spent for shopping
            assert_eq!(time_info.total_property, (100.0 * test.standard_shop_time_cost())-test.standard_shop_time_cost());
        }
        
        // additional tests to consider adding
        // TODO Shopping_Loop buy_result tests
        // TODO Shopping_loop emergency buy routing when Emergency buy is made
        // TODO Shopping_loop full_spread Barter when that option is possible.
    }

    mod free_time_should {
        use std::{collections::{HashMap, VecDeque}, thread, time::Duration};

        use barrage::{Sender, Receiver};
        use super::super::*;

        /// Intentionally super simple pop generation
        /// 
        /// sets desires to 
        /// - 10 food want (0-9)
        /// - 5 clothes want (5-9)
        /// - 5 shelter want (5-9)
        /// - inf ambrosia fruit 10+
        /// - inf clothes class 10+
        /// - inf shelter class 15+
        pub fn default_pop() -> Pop {
            let mut result = Pop {
                id: 0,
                job: 0,
                firm: 0,
                market: 0,
                property: Property::new(vec![]),
                breakdown_table: PopBreakdownTable { table: vec![], total: 0 },
                is_selling: true,
                current_sat: TieredValue { tier: 0, value: 0.0 },
                prev_sat: TieredValue { tier: 0, value: 0.0 },
                hypo_change: TieredValue { tier: 0, value: 0.0 },
                backlog: VecDeque::new(),
            };

            // don't care about pop breakdown. it's not actually being used.
            result.breakdown_table.insert_pops(
                PBRow::new(0, None, None, 
                    None, None, None, 
                    None, None, None, 
                    1));

            // Food 0
            result.property.desires.push(
                Desire{
                    item: Item::Want(2),
                    start: 0,
                    end: Some(9),
                    amount: 1.0,
                    satisfaction: 0.0,
                    step: 1,
                    tags: vec![],
                }
            );

            // Shelter 1
            result.property.desires.push(
                Desire{
                    item: Item::Want(3),
                    start: 5,
                    end: Some(9),
                    amount: 1.0,
                    satisfaction: 0.0,
                    step: 1,
                    tags: vec![],
                }
            );

            // Clothing 2
            result.property.desires.push(
                Desire{
                    item: Item::Want(4),
                    start: 5,
                    end: Some(9),
                    amount: 1.0,
                    satisfaction: 0.0,
                    step: 1,
                    tags: vec![],
                }
            );

            // ambrosia fruit 3
            result.property.desires.push(
                Desire{
                    item: Item::Product(2),
                    start: 10,
                    end: None,
                    amount: 1.0,
                    satisfaction: 0.0,
                    step: 1,
                    tags: vec![],
                }
            );

            // clothes 4
            result.property.desires.push(
                Desire{
                    item: Item::Class(6),
                    start: 15,
                    end: None,
                    amount: 1.0,
                    satisfaction: 0.0,
                    step: 1,
                    tags: vec![],
                }
            );

            // Hut/Cabin 5
            result.property.desires.push(
                Desire{
                    item: Item::Class(14),
                    start: 15,
                    end: None,
                    amount: 1.0,
                    satisfaction: 0.0,
                    step: 1,
                    tags: vec![],
                }
            );

            result
        }

        /// preps a pop's property, the property's data, and market prices of those items.
        /// 
        /// Sets all values to 1.0 amv and salability of 0.5 by default.
        /// 
        /// Exceptions are:
        /// - Ambrosia Fruit are set as a currency (Sal 1.0, currency=true)
        /// - Cotton Boll is set to currency with salability 1.0, and price of 5.0
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

            market.product_info.get_mut(&3).expect("Brok").salability = 1.0;
            market.product_info.get_mut(&3).expect("Brok").is_currency = true;
            market.product_info.get_mut(&3).expect("Brok").price = 5.0;

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

        fn dont_shop_loop(_pop: &mut Pop, _rx: &mut Receiver<ActorMessage>,
            _tx: &mut Sender<ActorMessage>,
            _data: &DataManager, _market: &MarketHistory) {}

        /// Shopping loop adds and removes based on the amount of time available to the pop.
        /// Each unit does the following
        /// 
        /// 1. Pure Positive, add food (prod 2) remove only time.
        /// 2. Neutral, add food but remove some cloth.
        /// 3. Negative, remove some cloth.
        fn shop_loop(pop: &mut Pop, _rx: &mut Receiver<ActorMessage>,
        _tx: &mut Sender<ActorMessage>,
        data: &DataManager, _market: &MarketHistory) {
            let time = pop.property.property.get(&TIME_PRODUCT_ID).unwrap().clone();
            if time.available() >= 3.0 {
                pop.property.remove_property(TIME_PRODUCT_ID, 1.0, data);
                pop.property.remove_property(6, 1.0, data);
            }
            if time.available() >= 2.0 {
                pop.property.remove_property(TIME_PRODUCT_ID, 1.0, data);
                pop.property.remove_property(6, 1.0, data);
                pop.property.add_property(2, 1.0, data);
            }
            if time.available() >= 1.0 {
                pop.property.remove_property(TIME_PRODUCT_ID, 1.0, data);
                pop.property.add_property(2, 1.0, data);
            }
        }

        #[test]
        pub fn set_current_and_previous_sat_correctly_after_change() {
            // setup pop, market, and history.
            let mut test = default_pop();
            let pop_info = test.actor_info();
            let (data, history) = prepare_data_for_market_actions(&mut test);
            // alter desires to run out of desires.
            // food 
            test.property.clear_desires();
            test.property.add_desire(&Desire::new(Item::Want(2), 0, 
                None, 1.0, 0.0, 1, vec![]).unwrap());
            
            // add the initial property of the pop we'll be using\
            // 20 ambrosia fruit, cotton clothes, huts, and cotton bolls
            test.property.add_property(2, 10.0, &data);
            test.property.add_property(3, 100.0, &data);
            test.property.add_property(6, 10.0, &data);
            test.property.add_property(14, 10.0, &data);
            // add in way to much shopping time.
            test.property.add_property(TIME_PRODUCT_ID, 
                1.0, &data);

            // setup message queue.
            let (tx, rx) = barrage::bounded(10);
            let mut passed_rx = rx.clone();
            let mut passed_tx = tx.clone();

            // get loop running
            let handle = thread::spawn(move || {
                test.free_time(&mut passed_rx, &mut passed_tx, &data, 
                    &history, shop_loop);
                test
            });
            thread::sleep(Duration::from_millis(100));

            // start loop, recieved and completed, nothing actually done here.
            thread::sleep(Duration::from_millis(1000));
            // should recieve finished here
            let start = std::time::SystemTime::now();
            while let Ok(msg) = rx.try_recv() {
                if let Some(msg) = msg {
                    if let ActorMessage::Finished { sender } = msg {
                        assert_eq!(sender, pop_info, "Incorrect Sender");
                        break;
                    }
                }
                let now = std::time::SystemTime::now();
                if now.duration_since(start).expect("Bad Time!") 
                    > Duration::from_secs(3) {
                    assert!(false, "Timed Out.");
                }
            }

            // send all finished to wrap up.
            tx.send(ActorMessage::AllFinished).expect("Borkd");

            // let it wrap up
            let test = handle.join().unwrap();

            // check that it correctly recorded the satisfaction success/failure.
            // no change should've occurred.
            assert!(test.prev_sat < test.current_sat);
        }

        #[test]
        pub fn set_current_and_previous_sat_correctly_after_no_change() {
            // setup pop, market, and history.
            let mut test = default_pop();
            let pop_info = test.actor_info();
            let (data, history) = prepare_data_for_market_actions(&mut test);
            // alter desires to run out of desires.
            // food 
            test.property.clear_desires();
            test.property.add_desire(&Desire::new(Item::Want(2), 0, 
                None, 1.0, 0.0, 1, vec![]).unwrap());
            
            // add the initial property of the pop we'll be using\
            // 20 ambrosia fruit, cotton clothes, huts, and cotton bolls
            test.property.add_property(2, 10.0, &data);
            test.property.add_property(3, 100.0, &data);
            test.property.add_property(6, 10.0, &data);
            test.property.add_property(14, 10.0, &data);
            // add in way to much shopping time.
            test.property.add_property(TIME_PRODUCT_ID, 
                0.0, &data);

            // setup message queue.
            let (tx, rx) = barrage::bounded(10);
            let mut passed_rx = rx.clone();
            let mut passed_tx = tx.clone();

            // get loop running
            let handle = thread::spawn(move || {
                test.free_time(&mut passed_rx, &mut passed_tx, &data, 
                    &history, shop_loop);
                test
            });
            thread::sleep(Duration::from_millis(100));

            // start loop, recieved and completed, nothing actually done here.
            thread::sleep(Duration::from_millis(1000));
            // should recieve finished here
            let start = std::time::SystemTime::now();
            while let Ok(msg) = rx.try_recv() {
                if let Some(msg) = msg {
                    if let ActorMessage::Finished { sender } = msg {
                        assert_eq!(sender, pop_info, "Incorrect Sender");
                        break;
                    }
                }
                let now = std::time::SystemTime::now();
                if now.duration_since(start).expect("Bad Time!") 
                    > Duration::from_secs(3) {
                    assert!(false, "Timed Out.");
                }
            }

            // send all finished to wrap up.
            tx.send(ActorMessage::AllFinished).expect("Borkd");

            // let it wrap up
            let test = handle.join().unwrap();

            // check that it correctly recorded the satisfaction success/failure.
            // no change should've occurred.
            assert_eq!(test.prev_sat, test.current_sat);
        }

        #[test]
        pub fn correctly_put_excess_goods_up_for_sale() {
            // setup pop, market, and history.
            let mut test = default_pop();
            let pop_info = test.actor_info();
            let (data, history) = prepare_data_for_market_actions(&mut test);
            // alter desires to run out of desires.
            // food 
            test.property.clear_desires();
            test.property.add_desire(&Desire::new(Item::Want(2), 0, 
                None, 1.0, 0.0, 1, vec![]).unwrap());
            
            // add the initial property of the pop we'll be using\
            // 20 ambrosia fruit, cotton clothes, huts, and cotton bolls
            test.property.add_property(2, 10.0, &data);
            test.property.add_property(3, 100.0, &data);
            test.property.add_property(6, 10.0, &data);
            test.property.add_property(14, 10.0, &data);
            // add in way to much shopping time.
            test.property.add_property(TIME_PRODUCT_ID, 
                100.0 * test.standard_shop_time_cost(), &data);

            // set our selling state to true.
            test.is_selling = true;

            // setup message queue.
            let (tx, rx) = barrage::bounded(10);
            let mut passed_rx = rx.clone();
            let mut passed_tx = tx.clone();

            // get loop running
            let handle = thread::spawn(move || {
                test.free_time(&mut passed_rx, &mut passed_tx, &data, 
                    &history, dont_shop_loop);
                test
            });
            thread::sleep(Duration::from_millis(100));
            
            // check that all non-reserved property is put up for sale.
            let mut sales = vec![];
            for _ in 0..3 {
                if let Ok(msg) = rx.recv() {
                    if let ActorMessage::SellOrder { .. } = msg {
                        //println!("{}", msg);
                        sales.push(msg);
                    } else {
                        break;
                    }
                }
            }
            // check that the sales are as predicted
            let mut found_prods = vec![];
            for msg in sales {
                if let ActorMessage::SellOrder { sender, product, 
                quantity, amv } = msg {
                    found_prods.push(product);
                    if product == 3 {
                        assert_eq!(sender, pop_info);
                        assert_eq!(quantity, 100.0);
                        assert_eq!(amv, 5.0);
                    } else if product == 14 {
                        assert_eq!(sender, pop_info);
                        assert_eq!(quantity, 10.0);
                        assert_eq!(amv, 100.0);
                    } else if product == 6 {
                        assert_eq!(sender, pop_info);
                        assert_eq!(quantity, 10.0);
                        assert_eq!(amv, 10.0);
                    } else {
                        assert!(false, "Product {} not expected.", product);
                    }
                } else {
                    assert!(false, "Bad message recievd during sales.");
                }
            }
            assert!(found_prods.contains(&3));
            assert!(found_prods.contains(&14));
            assert!(found_prods.contains(&6));

            // start loop, recieved and completed, nothing actually done here.
            thread::sleep(Duration::from_millis(1000));
            // should recieve finished here
            let start = std::time::SystemTime::now();
            while let Ok(msg) = rx.try_recv() {
                if let Some(msg) = msg {
                    if let ActorMessage::Finished { sender } = msg {
                        assert_eq!(sender, pop_info, "Incorrect Sender");
                        break;
                    }
                }
                let now = std::time::SystemTime::now();
                if now.duration_since(start).expect("Bad Time!") 
                    > Duration::from_secs(3) {
                    assert!(false, "Timed Out.");
                }
            }

            // send all finished to wrap up.
            tx.send(ActorMessage::AllFinished).expect("Borkd");

            // let it wrap up
            let test = handle.join().unwrap();

            // check that it correctly recorded the satisfaction success/failure.
            // no change should've occurred.
            assert_eq!(test.prev_sat, test.current_sat);
        }
    }

    mod adapt_future_plan_should {
        use std::collections::{HashMap, VecDeque};
        use super::super::*;

        /// preps a pop's property, the property's data, and market prices of those items.
        /// 
        /// Sets all values to 1.0 amv and salability of 0.5 by default.
        /// 
        /// Exceptions are:
        /// - Ambrosia Fruit are set as a currency (Sal 1.0, currency=true)
        /// - Cotton Boll is set to currency with salability 1.0, and price of 5.0
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

            market.product_info.get_mut(&3).expect("Brok").salability = 1.0;
            market.product_info.get_mut(&3).expect("Brok").is_currency = true;
            market.product_info.get_mut(&3).expect("Brok").price = 5.0;

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
        pub fn increase_max_when_peak_equal_or_above_max_target() {
            let mut pop = Pop {
                id: 0,
                job: 0,
                firm: 0,
                market: 0,
                property: Property::new(vec![]),
                breakdown_table: PopBreakdownTable { table: vec![], total: 1 },
                is_selling: false,
                current_sat: TieredValue { tier: 0, value: 0.0},
                prev_sat: TieredValue { tier: 0, value: 0.0},
                hypo_change: TieredValue { tier: 0, value: 0.0},
                backlog: VecDeque::new(),
            };

            let (data, history) = prepare_data_for_market_actions(&mut pop);

            // setup equal to max target
            let mut info = PropertyInfo::new(10.0);
            info.upper_target = 20.0;
            info.lower_target = 10.0;
            info.consumed = 5.0;
            info.lost = 5.0;
            pop.property.property.insert(0, info);
            // and greater than max targets.
            let mut info = PropertyInfo::new(10.0);
            info.upper_target = 20.0;
            info.lower_target = 10.0;
            info.consumed = 15.0;
            info.lost = 5.0;
            pop.property.property.insert(1, info);
            // between upper and lower
            let mut info = PropertyInfo::new(10.0);
            info.upper_target = 30.0;
            info.lower_target = 10.0;
            info.consumed = 5.0;
            info.lost = 5.0;
            pop.property.property.insert(2, info);
            // below/equal lower
            let mut info = PropertyInfo::new(10.0);
            info.upper_target = 30.0;
            info.lower_target = 20.0;
            info.consumed = 5.0;
            info.lost = 5.0;
            pop.property.property.insert(3, info);
            // total_loss above lower target
            let mut info = PropertyInfo::new(10.0);
            info.upper_target = 30.0;
            info.lower_target = 5.0;
            info.consumed = 5.0;
            info.lost = 5.0;
            pop.property.property.insert(4, info);
            // total_loss above lower target
            let mut info = PropertyInfo::new(10.0);
            info.upper_target = 30.0;
            info.lower_target = 20.0;
            info.consumed = 5.0;
            info.lost = 5.0;
            pop.property.property.insert(5, info);

            pop.adapt_future_plan(&data, &history);

            let info0 = pop.property.property.get(&0).unwrap();
            assert_eq!(info0.upper_target, 21.0);
            assert_eq!(info0.lower_target, 10.0);
            let info1 = pop.property.property.get(&1).unwrap();
            assert_eq!(info1.upper_target, 25.0);
            assert_eq!(info1.lower_target, 12.0);
            let info2 = pop.property.property.get(&2).unwrap();
            assert_eq!(info2.upper_target, 29.0);
            assert_eq!(info2.lower_target, 10.0);
            let info3 = pop.property.property.get(&3).unwrap();
            assert_eq!(info3.upper_target, 25.0);
            assert_eq!(info3.lower_target, 18.0);
            let info4 = pop.property.property.get(&4).unwrap();
            assert_eq!(info4.upper_target, 29.0);
            assert_eq!(info4.lower_target, 6.0);
            let info5 = pop.property.property.get(&5).unwrap();
            assert_eq!(info5.upper_target, 25.0);
            assert_eq!(info5.lower_target, 18.0);
        }
    }

    /// These tests 
    mod pop_integration_tests {
        use std::{collections::{HashMap, HashSet}, thread};
        use itertools::Itertools;
        use political_economy_core::objects::actor_objects::{actor::Actor, desire::DesireTag};

        use super::super::*;

        /// Gives
        /// 
        /// Wants: Rest, Wealth, Sustenance
        /// Products: Time, Shopping Time, Discernment, Resources, Wealth, Capital, Skill
        /// Processes: Go Shopping, Rest, Extraction, Make Wealth, Make Capital, Consume Capital
        /// Market Prices (AMV):
        /// - Resources: 1.0 
        /// - Wealth: 5.0
        /// - Capital: 3.0
        fn setup_pop_test_data() -> (MarketHistory, DataManager, Demographics) {
            let mut manager = DataManager::new();
            // get required items.
            // Loads 
            // wants: Rest, Wealth, 
            // products: Time, Shopping Time, Discernment Skill
            // processes: Go Shopping, Rest
            manager.required_items();

            // set up 'food' want in sustenance
            let mut sustenance = Want {
                id: 100,
                name: String::from("Sustenance"),
                description: String::from("The things which keep one alive and satiated."),
                decay: 1.0,
                ownership_sources: HashSet::new(),
                process_sources: HashSet::new(),
                use_sources: HashSet::new(),
                consumption_sources: HashSet::new(),
            };

            sustenance.consumption_sources.insert(102);

            // set up simple products and processes
            // Resources, extracted via time and Skill from nothing (Land not yet in system).
            let resources = Product::new(
                101,
                String::from("Resources"),
                String::from(""),
                String::from("Raw resources, needed to make stuff."),
                String::from("Unit(s)"),
                0,
                0.0,
                0.0,
                None,
                true,
                vec![],
                None,
                None,
            ).unwrap();
            // Wealth, made from resources and time, optional capital input and Skill.
            //      can be consumed into wealth want at 1:1.
            //      20% fails each day if not consumed.
            let mut wealth = Product::new(
                102,
                String::from("Wealth"),
                String::from(""),
                String::from("Physical Wealth, consumed to produ value"),
                String::from("Unit(s)"),
                0,
                0.0,
                0.0,
                Some(4),
                true,
                vec![],
                None,
                None,
            ).unwrap();
            // Capital, made from resources and time, optional capital input and skill.
            //      Used in most other processes
            //      10% fails each day into nothing.
            let capital = Product::new(
                103,
                String::from("Capital"),
                String::from(""),
                String::from("Abstract capital goods. Not particularly useful alone, but capable of doing more."),
                String::from("Unit(s)"),
                0,
                0.0,
                0.0,
                Some(9), 
                true,
                vec![],
                None,
                None,
            ).unwrap();
            // Skill, the generic skill separate from shopping time's skill.
            let skill = Product::new(
                104,
                String::from("Skill"),
                String::from(""),
                String::from("The Abstract skill of doing stuff with stuff, but better."),
                String::from("Unit(s)"),
                0,
                0.0,
                0.0,
                None,
                true,
                vec![],
                None,
                None,
            ).unwrap();

            // wealth is consumed into sustenance.
            wealth.consumption_processes.insert(103);

            // processes
            // extract resources from land
            let extraction = Process {
                id: 100,
                name: String::from("Extract Resources"),
                variant_name: String::from(""),
                description: String::from("Getting things to use is always important."),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart { // 1 hour (fixed)
                        item: Item::Product(TIME_PRODUCT_ID), 
                        amount: 1.0, 
                        part_tags: vec![ProcessPartTag::Fixed], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart { // 1 plot (fixed)
                        item: Item::Product(LAND_PRODUCT_ID), 
                        amount: 1.0, 
                        part_tags: vec![ProcessPartTag::Fixed], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart { // 1 unit of capital (optional 0.0->1.0)
                        item: Item::Product(capital.id), 
                        amount: 1.0, 
                        part_tags: vec![ProcessPartTag::Optional { 
                            missing_penalty: 0.0, 
                            final_bonus: 1.0 
                        }], 
                        part: ProcessSectionTag::Capital 
                    },
                    ProcessPart { // 1 level of skill (optional 0.0->1.0)
                        item: Item::Product(skill.id), 
                        amount: 1.0, 
                        part_tags: vec![ProcessPartTag::Optional { 
                            missing_penalty: 0.0, 
                            final_bonus: 1.0 
                        }], 
                        part: ProcessSectionTag::Capital 
                    },
                    ProcessPart {  // 1 unit of resources output (up to 4.0 per process with max bonii)
                        item: Item::Product(resources.id), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output 
                    },
                    ProcessPart {  // 0.1 unit of skill
                        item: Item::Product(skill.id), 
                        amount: 0.1, 
                        part_tags: vec![ProcessPartTag::Fixed], 
                        part: ProcessSectionTag::Output 
                    },
                ],
                process_tags: vec![],
                technology_requirement: None,
                tertiary_tech: None,
            };
            // make wealth with resources
            let make_wealth = Process {
                id: 101,
                name: String::from("Craft Wealth"),
                variant_name: String::from(""),
                description: String::from("Wealth is made from transforming raw resources into more useful material."),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart { // 1 hour (fixed)
                        item: Item::Product(TIME_PRODUCT_ID), 
                        amount: 1.0, 
                        part_tags: vec![ProcessPartTag::Fixed], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart { // 1/4 plot (fixed)
                        item: Item::Product(LAND_PRODUCT_ID), 
                        amount: 0.25, 
                        part_tags: vec![ProcessPartTag::Fixed], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart { // 1 resources
                        item: Item::Product(resources.id), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart { // 1 unit of capital (optional 0.0->1.0)
                        item: Item::Product(capital.id), 
                        amount: 1.0, 
                        part_tags: vec![ProcessPartTag::Optional { 
                            missing_penalty: 0.0, 
                            final_bonus: 1.0 
                        }], 
                        part: ProcessSectionTag::Capital 
                    },
                    ProcessPart { // 1 level of skill (optional 0.0->1.0)
                        item: Item::Product(skill.id), 
                        amount: 1.0, 
                        part_tags: vec![ProcessPartTag::Optional { 
                            missing_penalty: 0.0, 
                            final_bonus: 1.0 
                        }], 
                        part: ProcessSectionTag::Capital 
                    },
                    ProcessPart {  // 1 unit of wealth output (up to 4.0 per process with max bonii)
                        item: Item::Product(wealth.id), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output 
                    },
                    ProcessPart {  // 0.1 unit of skill
                        item: Item::Product(skill.id), 
                        amount: 0.1, 
                        part_tags: vec![ProcessPartTag::Fixed], 
                        part: ProcessSectionTag::Output 
                    },
                ],
                process_tags: vec![],
                technology_requirement: None,
                tertiary_tech: None,
            };
            // make capital with resources
            let make_capital = Process {
                id: 102,
                name: String::from("Craft Capital"),
                variant_name: String::from(""),
                description: String::from("The task of making capital from raw resources. A little extra work now to make tommorow easier."),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart { // 1 hour (fixed)
                        item: Item::Product(TIME_PRODUCT_ID), 
                        amount: 1.0, 
                        part_tags: vec![ProcessPartTag::Fixed], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart { // 1/4 plot (fixed)
                        item: Item::Product(LAND_PRODUCT_ID), 
                        amount: 0.25, 
                        part_tags: vec![ProcessPartTag::Fixed], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart { // 1 resources
                        item: Item::Product(resources.id), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart { // 1 unit of capital (optional 0.0->1.0)
                        item: Item::Product(capital.id), 
                        amount: 1.0, 
                        part_tags: vec![ProcessPartTag::Optional { 
                            missing_penalty: 0.0, 
                            final_bonus: 1.0 
                        }], 
                        part: ProcessSectionTag::Capital 
                    },
                    ProcessPart { // 1 level of skill (optional 0.0->1.0)
                        item: Item::Product(skill.id), 
                        amount: 1.0, 
                        part_tags: vec![ProcessPartTag::Optional { 
                            missing_penalty: 0.0, 
                            final_bonus: 1.0 
                        }], 
                        part: ProcessSectionTag::Capital 
                    },
                    ProcessPart {  // 1 unit of capital output (up to 4.0 per process with max bonii)
                        item: Item::Product(capital.id), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output 
                    },
                    ProcessPart {  // 0.1 unit of skill
                        item: Item::Product(skill.id), 
                        amount: 0.1, 
                        part_tags: vec![ProcessPartTag::Fixed], 
                        part: ProcessSectionTag::Output 
                    },
                ],
                process_tags: vec![],
                technology_requirement: None,
                tertiary_tech: None,
            };
            // consume wealth for wealth want
            let consume_wealth = Process {
                id: 103,
                name: String::from("Consume Wealth"),
                variant_name: String::from(""),
                description: String::from("Eating wealth to satisfy ones desires is necessary, no matter what one may think."),
                minimum_time: 0.0,
                process_parts: vec![
                    ProcessPart { // 0.1 hour
                        item: Item::Product(TIME_PRODUCT_ID), 
                        amount: 0.1, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart { // 1/4 plot (fixed)
                        item: Item::Product(LAND_PRODUCT_ID), 
                        amount: 0.25, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart { // 1 resources
                        item: Item::Product(wealth.id), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Input 
                    },
                    ProcessPart {  // 1 unit of sustenance output
                        item: Item::Want(sustenance.id), 
                        amount: 1.0, 
                        part_tags: vec![], 
                        part: ProcessSectionTag::Output 
                    },
                ],
                process_tags: vec![ProcessTag::Consumption(wealth.id)],
                technology_requirement: None,
                tertiary_tech: None,
            };

            // sets up history.
            let mut history = MarketHistory {
                product_info: HashMap::new(),
                class_info: HashMap::new(),
                want_info: HashMap::new(),
                sale_priority: vec![],
                currencies: vec![],
            };

            history.product_info.insert(resources.id, ProductInfo {
                available: 100.0,
                price: 1.0,
                offered: 100.0,
                sold: 0.0,
                salability: 0.5,
                is_currency: false,
            });
            history.product_info.insert(wealth.id, ProductInfo {
                available: 100.0,
                price: 5.0,
                offered: 100.0,
                sold: 0.0,
                salability: 1.0,
                is_currency: false,
            });
            history.product_info.insert(capital.id, ProductInfo {
                available: 100.0,
                price: 3.0,
                offered: 100.0,
                sold: 0.0,
                salability: 0.5,
                is_currency: false,
            });
            history.want_info.insert(sustenance.id, MarketWantInfo::new(1.0));
            history.sale_priority.push(wealth.id);
            history.sale_priority.push(capital.id);
            history.sale_priority.push(resources.id);

            manager.wants.insert(sustenance.id, sustenance);
            manager.products.insert(resources.id, resources);
            manager.products.insert(wealth.id, wealth);
            manager.products.insert(capital.id, capital);
            manager.products.insert(skill.id, skill);

            manager.processes.insert(extraction.id, extraction);
            manager.processes.insert(make_wealth.id, make_wealth);
            manager.processes.insert(make_capital.id, make_capital);
            manager.processes.insert(consume_wealth.id, consume_wealth);

            let demos = Demographics {
                species: HashMap::new(),
                cultures: HashMap::new(),
                ideology: HashMap::new(),
            };

            (history, manager, demos)
        }
    
        /// # Pop Barter Test
        /// 
        /// Given a simple economy, we test that two pops connecting to each 
        /// other trade successfully at least once.
        #[test]
        fn pop_barter_test() {
            let (history, data, demos) = setup_pop_test_data();

            // get some data to make values more robust.
            let rest = data.wants.get(&REST_WANT_ID).unwrap();
            let sustenance = data.wants.values().find_or_first(|x| {
                x.name.eq(&String::from("Sustenance"))
            }).unwrap();
            let resources = data.products.values().find(|x| {
                x.name == String::from("Resources")
            }).unwrap();
            let wealth = data.products.values().find(|x| {
                x.name == String::from("Wealth")
            }).unwrap();
            let capital = data.products.values().find(|x| {
                x.name == String::from("Capital")
            }).unwrap();

            let mut pop1 = Pop {
                id: 0,
                job: 0,
                firm: 0,
                market: 0,
                property: Property::new(vec![
                    Desire::new(Item::Want(rest.id), 0, Some(10), 1.0, 0.0, 1, vec![]).unwrap(),
                    Desire::new(Item::Want(sustenance.id), 0, Some(10), 1.0, 0.0, 2, vec![]).unwrap(),
                    Desire::new(Item::Want(resources.id), 0, Some(10), 1.0, 0.0, 2, vec![]).unwrap(),
                ]),
                breakdown_table: PopBreakdownTable {
                    table: vec![],
                    total: 1,
                },
                is_selling: true,
                current_sat: TieredValue { tier: 0, value: 0.0 },
                prev_sat: TieredValue { tier: 0, value: 0.0 },
                hypo_change: TieredValue { tier: 0, value: 0.0 },
                backlog: VecDeque::new(),
            };
            let mut pop2 = Pop {
                id: 1,
                job: 1,
                firm: 1,
                market: 0,
                property: Property::new(vec![
                    Desire::new(Item::Want(rest.id), 0, Some(10), 1.0, 0.0, 1, vec![]).unwrap(),
                    Desire::new(Item::Want(sustenance.id), 0, Some(10), 1.0, 0.0, 2, vec![]).unwrap(),
                    Desire::new(Item::Want(resources.id), 0, Some(10), 1.0, 0.0, 2, vec![]).unwrap(),
                ]),
                breakdown_table: PopBreakdownTable {
                    table: vec![],
                    total: 1,
                },
                is_selling: true,
                current_sat: TieredValue { tier: 0, value: 0.0 },
                prev_sat: TieredValue { tier: 0, value: 0.0 },
                hypo_change: TieredValue { tier: 0, value: 0.0 },
                backlog: VecDeque::new(),
            };

            // add property to exchange between them.
            // same time to both.
            pop1.property.add_property(rest.id, 20.0, &data);
            pop2.property.add_property(rest.id, 20.0, &data);
            // one has a bunch of wealth, the other has a bunch of resources.
            pop1.property.add_property(resources.id, 10.0, &data);
            pop2.property.add_property(wealth.id, 10.0, &data);

            // spin them up into their day stuff, then while acting as the market, set them up to trade.
            let (tx, rx) = barrage::bounded(10);
            let mut passed_rx = rx.clone();
            let mut passed_tx = tx.clone();

            // get loop running
            let handle = thread::spawn(move || {
                pop1.run_market_day(&mut passed_tx, &mut passed_rx, &data, &demos, &history);
                pop2.run_market_day(&mut passed_tx, &mut passed_rx, &data, &demos, &history);
                (pop1, pop2)
            });

            // start the market day.
            tx.send(ActorMessage::StartDay).expect("Brokd.");
            // should recieve finished here
            let start = std::time::SystemTime::now();
            if let Ok(ActorMessage::StartDay) = rx.recv() {
                // do nothing, all's fine
            } else {
                assert!(false, "wrong message recieved.");
            }
        }
    }
}
