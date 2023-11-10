pub mod objects;
pub mod data_manager;
pub mod demographics;
pub mod runner;
pub mod actor_manager;
pub mod constants;
pub mod helper_types;
pub mod product_network;

extern crate lazy_static;

#[cfg(test)]
mod tests {
    mod property_info_tests {
        use crate::objects::property_info::PropertyInfo;

        #[test]
        pub fn safe_remove_should_remove_from_unreserved_and_reserved_only() {
            let mut test = PropertyInfo::new(100.0);
            test.shift_to_class_reserve(25.0);
            test.shift_to_reserved(25.0);
            assert!(test.total_property == 100.0);
            assert!(test.unreserved == 50.0);
            assert!(test.reserved == 25.0);
            assert!(test.want_reserve == 0.0);
            assert!(test.class_reserve == 25.0);
            assert!(test.specific_reserve == 0.0);

            // expend from unreserved
            test.safe_remove(25.0);
            assert!(test.total_property == 75.0);
            assert!(test.unreserved == 25.0);
            assert!(test.reserved == 25.0);
            assert!(test.want_reserve == 0.0);
            assert!(test.class_reserve == 25.0);
            assert!(test.specific_reserve == 0.0);

            // expend from unreserved
            test.safe_remove(25.0);
            assert!(test.total_property == 50.0);
            assert!(test.unreserved == 0.0);
            assert!(test.reserved == 25.0);
            assert!(test.want_reserve == 0.0);
            assert!(test.class_reserve == 25.0);
            assert!(test.specific_reserve == 0.0);

            // expend from reserved
            test.safe_remove(25.0);
            assert!(test.total_property == 25.0);
            assert!(test.unreserved == 0.0);
            assert!(test.reserved == 0.0);
            assert!(test.want_reserve == 0.0);
            assert!(test.class_reserve == 25.0);
            assert!(test.specific_reserve == 0.0);

            // don't expend
            test.safe_remove(25.0);
            assert!(test.total_property == 25.0);
            assert!(test.unreserved == 0.0);
            assert!(test.reserved == 0.0);
            assert!(test.want_reserve == 0.0);
            assert!(test.class_reserve == 25.0);
            assert!(test.specific_reserve == 0.0);
        }

        #[test]
        pub fn expend_should_remove_from_unreserved_only() {
            let mut test = PropertyInfo::new(100.0);
            test.shift_to_class_reserve(25.0);
            test.shift_to_reserved(25.0);
            assert!(test.total_property == 100.0);
            assert!(test.unreserved == 50.0);
            assert!(test.reserved == 25.0);
            assert!(test.want_reserve == 0.0);
            assert!(test.class_reserve == 25.0);
            assert!(test.specific_reserve == 0.0);

            // expend from unreserved
            test.expend(25.0);
            assert!(test.total_property == 75.0);
            assert!(test.unreserved == 25.0);
            assert!(test.reserved == 25.0);
            assert!(test.want_reserve == 0.0);
            assert!(test.class_reserve == 25.0);
            assert!(test.specific_reserve == 0.0);

            // expend from unreserved
            test.expend(25.0);
            assert!(test.total_property == 50.0);
            assert!(test.unreserved == 0.0);
            assert!(test.reserved == 25.0);
            assert!(test.want_reserve == 0.0);
            assert!(test.class_reserve == 25.0);
            assert!(test.specific_reserve == 0.0);

            // don't expend
            test.expend(25.0);
            assert!(test.total_property == 50.0);
            assert!(test.unreserved == 0.0);
            assert!(test.reserved == 25.0);
            assert!(test.want_reserve == 0.0);
            assert!(test.class_reserve == 25.0);
            assert!(test.specific_reserve == 0.0);
        }

        mod available_should {
            use crate::objects::property_info::PropertyInfo;

            #[test]
            pub fn return_sum_of_unreserved_and_reserved() {
                // get start to check
                let mut test = PropertyInfo::new(100.0);
                assert!(test.total_property == 100.0);
                assert!(test.unreserved == 100.0);
                assert!(test.reserved == 0.0);
                assert!(test.want_reserve == 0.0);
                assert!(test.class_reserve == 0.0);
                assert!(test.specific_reserve == 0.0);
                // check it's correct
                assert!(test.available() == 100.0);

                // shift to reserve
                test.shift_to_reserved(50.0);
                assert!(test.available() == 100.0);

                // shift to specific
                test.shift_to_class_reserve(50.0);
                assert!(test.available() == 50.0);
            }
        }

        mod remove_should {
            use crate::objects::property_info::PropertyInfo;

            #[test]
            pub fn remove_from_pools_correctly() {
                let mut test = PropertyInfo::new(100.0);
                // shift into each reserve
                test.shift_to_class_reserve(25.0);
                test.shift_to_specific_reserve(25.0);
                test.shift_to_want_reserve(25.0);
                test.shift_to_reserved(25.0);
                assert!(test.total_property == 100.0);
                assert!(test.unreserved == 50.0);
                assert!(test.reserved == 25.0);
                assert!(test.want_reserve == 25.0);
                assert!(test.class_reserve == 25.0);
                assert!(test.specific_reserve == 25.0);
                // remove just from unreserved
                test.remove(40.0);
                assert!(test.total_property == 60.0);
                assert!(test.unreserved == 10.0);
                assert!(test.reserved == 25.0);
                assert!(test.want_reserve == 25.0);
                assert!(test.class_reserve == 25.0);
                assert!(test.specific_reserve == 25.0);
                // remove from unreserved and reserve
                test.remove(30.0);
                assert!(test.total_property == 30.0);
                assert!(test.unreserved == 00.0);
                assert!(test.reserved == 5.0);
                assert!(test.want_reserve == 25.0);
                assert!(test.class_reserve == 25.0);
                assert!(test.specific_reserve == 25.0);
                // remove from reserve and spec_reserves
                test.remove(30.0);
                assert!(test.total_property == 00.0);
                assert!(test.unreserved == 00.0);
                assert!(test.reserved == 0.0);
                assert!(test.want_reserve == 0.0);
                assert!(test.class_reserve == 0.0);
                assert!(test.specific_reserve == 0.0);
                // remove from all pools at once
                test.add_property(100.0);
                test.shift_to_class_reserve(25.0);
                test.shift_to_specific_reserve(25.0);
                test.shift_to_want_reserve(25.0);
                test.shift_to_reserved(25.0);
                assert!(test.total_property == 100.0);
                assert!(test.unreserved == 50.0);
                assert!(test.reserved == 25.0);
                assert!(test.want_reserve == 25.0);
                assert!(test.class_reserve == 25.0);
                assert!(test.specific_reserve == 25.0);
                test.remove(100.0);
                assert!(test.total_property == 00.0);
                assert!(test.unreserved == 00.0);
                assert!(test.reserved == 0.0);
                assert!(test.want_reserve == 0.0);
                assert!(test.class_reserve == 0.0);
                assert!(test.specific_reserve == 0.0);
            }

            #[test]
            pub fn call_add_when_value_is_negative() {
                let mut test = PropertyInfo::new(10.0);

                assert!(test.total_property == 10.0);
                assert!(test.unreserved == 10.0);
                assert!(test.reserved == 0.0);
                assert!(test.want_reserve == 0.0);
                assert!(test.class_reserve == 0.0);
                assert!(test.specific_reserve == 0.0);

                test.remove(-100.0);
                assert!(test.total_property == 110.0);
                assert!(test.unreserved == 110.0);
                assert!(test.reserved == 0.0);
                assert!(test.want_reserve == 0.0);
                assert!(test.class_reserve == 0.0);
                assert!(test.specific_reserve == 0.0);
            }
        }

        mod add_property_should {
            use crate::objects::property_info::PropertyInfo;

            #[test]
            pub fn add_to_both_total_and_reserve() {
                let mut test = PropertyInfo::new(10.0);

                assert!(test.total_property == 10.0);
                assert!(test.unreserved == 10.0);
                assert!(test.reserved == 0.0);
                assert!(test.want_reserve == 0.0);
                assert!(test.class_reserve == 0.0);
                assert!(test.specific_reserve == 0.0);

                test.add_property(100.0);
                assert!(test.total_property == 110.0);
                assert!(test.unreserved == 110.0);
                assert!(test.reserved == 0.0);
                assert!(test.want_reserve == 0.0);
                assert!(test.class_reserve == 0.0);
                assert!(test.specific_reserve == 0.0);
            }

            #[test]
            pub fn call_remove_when_value_is_negative() {
                let mut test = PropertyInfo::new(100.0);
                // shift into each reserve
                test.shift_to_class_reserve(25.0);
                test.shift_to_specific_reserve(25.0);
                test.shift_to_want_reserve(25.0);
                test.shift_to_reserved(25.0);
                assert!(test.total_property == 100.0);
                assert!(test.unreserved == 50.0);
                assert!(test.reserved == 25.0);
                assert!(test.want_reserve == 25.0);
                assert!(test.class_reserve == 25.0);
                assert!(test.specific_reserve == 25.0);
                // remove just from unreserved
                test.add_property(-40.0);
                assert!(test.total_property == 60.0);
                assert!(test.unreserved == 10.0);
                assert!(test.reserved == 25.0);
                assert!(test.want_reserve == 25.0);
                assert!(test.class_reserve == 25.0);
                assert!(test.specific_reserve == 25.0);
                // remove from unreserved and reserve
                test.add_property(-30.0);
                assert!(test.total_property == 30.0);
                assert!(test.unreserved == 00.0);
                assert!(test.reserved == 5.0);
                assert!(test.want_reserve == 25.0);
                assert!(test.class_reserve == 25.0);
                assert!(test.specific_reserve == 25.0);
                // remove from reserve and spec_reserves
                test.add_property(-30.0);
                assert!(test.total_property == 00.0);
                assert!(test.unreserved == 00.0);
                assert!(test.reserved == 0.0);
                assert!(test.want_reserve == 0.0);
                assert!(test.class_reserve == 0.0);
                assert!(test.specific_reserve == 0.0);
                // remove from all pools at once
                test.add_property(100.0);
                test.shift_to_class_reserve(25.0);
                test.shift_to_specific_reserve(25.0);
                test.shift_to_want_reserve(25.0);
                test.shift_to_reserved(25.0);
                assert!(test.total_property == 100.0);
                assert!(test.unreserved == 50.0);
                assert!(test.reserved == 25.0);
                assert!(test.want_reserve == 25.0);
                assert!(test.class_reserve == 25.0);
                assert!(test.specific_reserve == 25.0);
                test.add_property(-100.0);
                assert!(test.total_property == 00.0);
                assert!(test.unreserved == 00.0);
                assert!(test.reserved == 0.0);
                assert!(test.want_reserve == 0.0);
                assert!(test.class_reserve == 0.0);
                assert!(test.specific_reserve == 0.0);
            }
        }

        mod reset_reserves_should {
            use crate::objects::property_info::PropertyInfo;

            #[test]
            pub fn reset_reserves_correctly() {
                let mut test = PropertyInfo::new(100.0);

                test.shift_to_class_reserve(50.0);
                test.shift_to_reserved(25.0);
                assert!(test.total_property == 100.0);
                assert!(test.unreserved == 25.0);
                assert!(test.reserved == 25.0);
                assert!(test.want_reserve == 0.0);
                assert!(test.class_reserve == 50.0);
                assert!(test.specific_reserve == 0.0);

                test.reset_reserves();
                assert!(test.total_property == 100.0);
                assert!(test.unreserved == 100.0);
                assert!(test.reserved == 0.0);
                assert!(test.want_reserve == 0.0);
                assert!(test.class_reserve == 0.0);
                assert!(test.specific_reserve == 0.0);
            }
        }

        mod shift_tests {
            use crate::objects::property_info::PropertyInfo;

            #[test]
            pub fn shift_to_reserved_correctly() {
                let mut test = PropertyInfo::new(10.0);

                test.shift_to_reserved(5.0);
                assert!(test.total_property == 10.0);
                assert!(test.unreserved == 5.0);
                assert!(test.reserved == 5.0);
                assert!(test.specific_reserve == 0.0);
                assert!(test.class_reserve == 0.0);
                assert!(test.want_reserve == 0.0);

                test.shift_to_reserved(10.0);
                assert!(test.total_property == 10.0);
                assert!(test.unreserved == 0.0);
                assert!(test.reserved == 10.0);
                assert!(test.specific_reserve == 0.0);
                assert!(test.class_reserve == 0.0);
                assert!(test.want_reserve == 0.0);
            }

            #[test]
            pub fn get_max_special_reserve_correctly() {
                let mut test = PropertyInfo::new(10.0);
                let result = test.max_spec_reserve();
                assert!(result == 0.0);

                test.specific_reserve = 1.0;
                let result = test.max_spec_reserve();
                assert!(result == 1.0);

                test.class_reserve = 2.0;
                let result = test.max_spec_reserve();
                assert!(result == 2.0);

                test.want_reserve = 3.0;
                let result = test.max_spec_reserve();
                assert!(result == 3.0);
            }

            #[test]
            pub fn shift_to_specific_reserve_correctly() {
                let mut test = PropertyInfo::new(10.0);
                test.shift_to_reserved(5.0);
                test.total_property += 5.0;
                test.class_reserve += 5.0;

                // check that it reserves from overlap first.
                test.shift_to_specific_reserve(2.5);
                assert!(test.total_property == 15.0);
                assert!(test.unreserved == 5.0);
                assert!(test.reserved == 5.0);
                assert!(test.specific_reserve == 2.5);
                assert!(test.class_reserve == 5.0);
                assert!(test.want_reserve == 0.0);

                // check that it takes from overlap and reserved
                test.shift_to_specific_reserve(5.0);
                assert!(test.total_property == 15.0);
                assert!(test.unreserved == 5.0);
                assert!(test.reserved == 2.5);
                assert!(test.specific_reserve == 7.5);
                assert!(test.class_reserve == 5.0);
                assert!(test.want_reserve == 0.0);

                // check that it takes from reserve and unreserved
                test.shift_to_specific_reserve(5.0);
                assert!(test.total_property == 15.0);
                assert!(test.unreserved == 2.5);
                assert!(test.reserved == 0.0);
                assert!(test.specific_reserve == 12.5);
                assert!(test.class_reserve == 5.0);
                assert!(test.want_reserve == 0.0);

                // check that it takes from unreserved and returns excess
                test.shift_to_specific_reserve(5.0);
                assert!(test.total_property == 15.0);
                assert!(test.unreserved == 0.0);
                assert!(test.reserved == 0.0);
                assert!(test.specific_reserve == 15.0);
                assert!(test.class_reserve == 5.0);
                assert!(test.want_reserve == 0.0);
            }

            #[test]
            pub fn shift_to_class_reserve_correctly() {
                let mut test = PropertyInfo::new(10.0);
                test.shift_to_reserved(5.0);
                test.total_property += 5.0;
                test.specific_reserve += 5.0;

                // check that it reserves from overlap first.
                test.shift_to_class_reserve(2.5);
                assert!(test.total_property == 15.0);
                assert!(test.unreserved == 5.0);
                assert!(test.reserved == 5.0);
                assert!(test.specific_reserve == 5.0);
                assert!(test.class_reserve == 2.5);
                assert!(test.want_reserve == 0.0);

                // check that it takes from overlap and reserved
                test.shift_to_class_reserve(5.0);
                assert!(test.total_property == 15.0);
                assert!(test.unreserved == 5.0);
                assert!(test.reserved == 2.5);
                assert!(test.specific_reserve == 5.0);
                assert!(test.class_reserve == 7.5);
                assert!(test.want_reserve == 0.0);

                // check that it takes from reserve and unreserved
                test.shift_to_class_reserve(5.0);
                assert!(test.total_property == 15.0);
                assert!(test.unreserved == 2.5);
                assert!(test.reserved == 0.0);
                assert!(test.specific_reserve == 5.0);
                assert!(test.class_reserve == 12.5);
                assert!(test.want_reserve == 0.0);

                // check that it takes from unreserved and returns excess
                test.shift_to_class_reserve(5.0);
                assert!(test.total_property == 15.0);
                assert!(test.unreserved == 0.0);
                assert!(test.reserved == 0.0);
                assert!(test.specific_reserve == 5.0);
                assert!(test.class_reserve == 15.0);
                assert!(test.want_reserve == 0.0);
            }

            #[test]
            pub fn shift_to_want_reserve_correctly() {
                let mut test = PropertyInfo::new(10.0);
                test.shift_to_reserved(5.0);
                test.total_property += 5.0;
                test.specific_reserve += 5.0;

                // check that it reserves from overlap first.
                test.shift_to_want_reserve(2.5);
                assert!(test.total_property == 15.0);
                assert!(test.unreserved == 5.0);
                assert!(test.reserved == 5.0);
                assert!(test.specific_reserve == 5.0);
                assert!(test.class_reserve == 0.0);
                assert!(test.want_reserve == 2.5);

                // check that it takes from overlap and reserved
                test.shift_to_want_reserve(5.0);
                assert!(test.total_property == 15.0);
                assert!(test.unreserved == 5.0);
                assert!(test.reserved == 2.5);
                assert!(test.specific_reserve == 5.0);
                assert!(test.class_reserve == 0.0);
                assert!(test.want_reserve == 7.5);

                // check that it takes from reserve and unreserved
                test.shift_to_want_reserve(5.0);
                assert!(test.total_property == 15.0);
                assert!(test.unreserved == 2.5);
                assert!(test.reserved == 0.0);
                assert!(test.specific_reserve == 5.0);
                assert!(test.class_reserve == 0.0);
                assert!(test.want_reserve == 12.5);

                // check that it takes from unreserved and returns excess
                test.shift_to_want_reserve(5.0);
                assert!(test.total_property == 15.0);
                assert!(test.unreserved == 0.0);
                assert!(test.reserved == 0.0);
                assert!(test.specific_reserve == 5.0);
                assert!(test.class_reserve == 0.0);
                assert!(test.want_reserve == 15.0);
            }
        }
    }

    mod pop_tests {
        // fix
        use std::collections::{HashMap, VecDeque};

        use crate::{objects::{pop::Pop, 
            pop_breakdown_table::{PopBreakdownTable, PBRow},
             property::Property, desire::{Desire, DesireItem},
              species::Species, culture::Culture, ideology::Ideology, market::{MarketHistory, ProductInfo}, property_info::PropertyInfo}, 
              demographics::Demographics, data_manager::DataManager};

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
                skill: 0, 
                lower_skill_level: 0.0, 
                higher_skill_level: 0.0, 
                property: Property::new(vec![]), 
                breakdown_table: PopBreakdownTable{ table: vec![], total: 0 }, 
                is_selling: true,
                backlog: VecDeque::new()};

            let species_desire_1 = Desire { 
                item: DesireItem::Want(2), // food
                start: 0, 
                end: Some(4), 
                amount: 1.0, 
                satisfaction: 0.0, 
                step: 1, 
                tags: vec![] };
            let species_desire_2 = Desire { 
                item: DesireItem::Want(3), // shelter
                start: 7, 
                end: Some(13), 
                amount: 1.0, 
                satisfaction: 0.0, 
                step: 2, 
                tags: vec![] };
            let species_desire_3 = Desire { 
                item: DesireItem::Want(4), //clothing
                start: 2, 
                end: Some(8), 
                amount: 1.0, 
                satisfaction: 0.0, 
                step: 2, 
                tags: vec![] };

            let culture_desire_1 = Desire { 
                item: DesireItem::Product(2), // ambrosia fruit
                start: 10, 
                end: Some(30), 
                amount: 1.0, 
                satisfaction: 0.0, 
                step: 5, 
                tags: vec![] };
            let culture_desire_2 = Desire { 
                item: DesireItem::Product(6), // clothes
                start: 15, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0, 
                step: 10, 
                tags: vec![] };

            let ideology_desire_1 = Desire { 
                item: DesireItem::Product(14), // Hut
                start: 30, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0, 
                step: 0, 
                tags: vec![] };
            let ideology_desire_2 = Desire { 
                item: DesireItem::Product(15), // Cabin
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
            data.load_all(&String::from("")).expect("Error on load?");
            let product = data.products.get_mut(&6).unwrap();
            product.fractional = true;

            let mut market = MarketHistory {
                info: HashMap::new(),
                sale_priority: vec![],
                currencies: vec![],
            };
            // quickly set all prices to 1.0 for ease going forward.
            for idx in 0..26 {
                market.info.insert(idx, ProductInfo {
                    available: 0.0,
                    price: 1.0,
                    offered: 0.0,
                    sold: 0.0,
                    salability: 0.5,
                    is_currency: false,
                });
            }
            // ambrosia fruit
            market.info.get_mut(&2).expect("Brok").salability = 1.0;
            market.info.get_mut(&2).expect("Brok").is_currency = true;

            market.info.get_mut(&6).expect("Brok").price = 10.0;
            market.info.get_mut(&7).expect("Brok").price = 20.0;

            market.info.get_mut(&14).expect("Brok").price = 100.0;
            market.info.get_mut(&15).expect("Brok").price = 1000.0;

            market.currencies.push(2);
            // sale priority would go here if used.

            // pop.property.property.insert(6, PropertyInfo::new(10.0));
            // TODO fix this info.

            (data, market)
        }

        // FIXME
        mod free_time {
            use std::{thread, time::Duration};

            use crate::{objects::{actor_message::ActorMessage, seller::Seller, property_info::PropertyInfo}, constants::TIME_ID};

            use super::{make_test_pop, prepare_data_for_market_actions};


            /// This is a test to touch most of the free time stuff just to 
            /// sanity check it. Doesn't push any messages for 
            /// common_processing to deal with, just forces it to
            /// run out of stuff to buy and checks that it acts appropriately.
            #[test]
            pub fn should_act_as_expected() {
                let mut test = make_test_pop();
                let pop_info = test.actor_info();
                let (data, market) = prepare_data_for_market_actions(&mut test);
                // don't worry about it buying anything, we'll just pass back a middle finger to get what we want.
                test.is_selling = true;
                // add a bunch of time for shopping.
                test.property.property.insert(TIME_ID, PropertyInfo::new(test.standard_shop_time_cost() + 100.0));
                // setup messaging
                let (tx, rx) = barrage::bounded(10);
                let mut passed_rx = rx.clone();
                let passed_tx = tx.clone();

                let handle = thread::spawn(move || {
                    test.free_time(&mut passed_rx, &passed_tx, &data, &market);
                    test
                });
                thread::sleep(Duration::from_millis(100));
                // ensure that free time put up their items for sale
                let msg = rx.recv().expect("Unexpected Close.");
                if handle.is_finished() { assert!(false); } // ensure we're not done.

                if let ActorMessage::SellOrder { sender, product, quantity, 
                amv } = msg {
                    assert_eq!(sender, pop_info);
                    assert_eq!(product, 6);
                    assert!(quantity == 10.0);
                    assert!(amv == 2.0);
                } else { assert!(false); }
                thread::sleep(Duration::from_millis(100));

                // wait for the pop to send out it's Find Product Message
                let msg = rx.recv().expect("Find Product Not recieved.");
                if let ActorMessage::FindProduct { product, sender } = msg {
                    assert_eq!(product, 7);
                    assert_eq!(sender, pop_info);
                } else { assert!(false); }

                tx.send(ActorMessage::ProductNotFound { product: 7, buyer: pop_info })
                .expect("Unexpected Break.");
                rx.recv().expect("Not Closed"); // consume our ProductNotFound message
                thread::sleep(Duration::from_millis(100));
                // with the not found message sent, that should be the only 
                // thing they attempted to buy and should finish up.
                let msg = rx.recv().expect("Unexpected Close");
                if let ActorMessage::Finished { sender } = msg {
                    assert_eq!(sender, pop_info);
                } else { assert!(false); };
                if handle.is_finished() { assert!(false); }
                // Send our Finished msg to wrap it up.
                tx.send(ActorMessage::AllFinished).expect("Closed?");
                thread::sleep(Duration::from_millis(100));
                // with that send, wrap up.
                if !handle.is_finished() { assert!(false); }
                let test = handle.join().unwrap();

                assert!(test.property.property.get(&TIME_ID).unwrap().total_property == 100.0);
            }

            /// This is a test to touch most of the free time stuff just to 
            /// sanity check it. Doesn't push any messages for 
            /// common_processing to deal with, just forces it to
            /// run out of stuff to buy and checks that it acts appropriately.
            #[test]
            pub fn should_leave_when_time_has_run_out() {
                let mut test = make_test_pop();
                let pop_info = test.actor_info();
                let (data, market) = prepare_data_for_market_actions(&mut test);
                // don't worry about it buying anything, we'll just pass back a middle finger to get what we want.
                test.is_selling = true;
                // add a bunch of time for shopping.
                test.property.property.insert(TIME_ID, PropertyInfo::new(test.standard_shop_time_cost() + 1.0));
                // add an additional product to the priority list
                // setup messaging
                let (tx, rx) = barrage::bounded(10);
                let mut passed_rx = rx.clone();
                let passed_tx = tx.clone();

                let handle = thread::spawn(move || {
                    test.free_time(&mut passed_rx, &passed_tx, &data, &market);
                    test
                });
                thread::sleep(Duration::from_millis(100));
                // ensure that free time put up their items for sale
                let msg = rx.recv().expect("Unexpected Close.");
                if handle.is_finished() { assert!(false); } // ensure we're not done.

                if let ActorMessage::SellOrder { sender, product, quantity, 
                amv } = msg {
                    assert_eq!(sender, pop_info);
                    assert_eq!(product, 6);
                    assert!(quantity == 10.0);
                    assert!(amv == 2.0);
                } else { assert!(false); }
                thread::sleep(Duration::from_millis(100));

                // wait for the pop to send out it's Find Product Message
                let msg = rx.recv().expect("Find Product Not recieved.");
                if let ActorMessage::FindProduct { product, sender } = msg {
                    assert_eq!(product, 7);
                    assert_eq!(sender, pop_info);
                } else { assert!(false, "Did not Recieve Find Product msg!"); }

                tx.send(ActorMessage::ProductNotFound { product: 7, buyer: pop_info })
                .expect("Unexpected Break.");
                rx.recv().expect("Not Closed"); // consume our ProductNotFound message
                thread::sleep(Duration::from_millis(100));
                // with the not found message sent, that should be the only 
                // thing they attempted to buy and should finish up.
                let msg = rx.recv().expect("Unexpected Close");
                if let ActorMessage::Finished { sender } = msg {
                    assert_eq!(sender, pop_info);
                } else if let ActorMessage::FindProduct { .. } = msg {
                    assert!(false, "Enough time to send for next product!");
                }
                else { assert!(false, "Did Not Recieve Finished!"); };
                if handle.is_finished() { assert!(false); }
                // Send our Finished msg to wrap it up.
                tx.send(ActorMessage::AllFinished).expect("Closed?");
                thread::sleep(Duration::from_millis(100));
                // with that send, wrap up.
                if !handle.is_finished() { assert!(false); }
                let test = handle.join().unwrap();

                assert!(test.property.property.get(&TIME_ID).unwrap().total_property == 1.0);
            }
        }

        #[test]
        pub fn update_pop_desires_equivalently() {
            let mut test = Pop{ 
                id: 0, 
                job: 0, 
                firm: 0, 
                market: 0, 
                skill: 0, 
                lower_skill_level: 0.0, 
                higher_skill_level: 0.0, 
                property: Property::new(vec![]), 
                breakdown_table: PopBreakdownTable{ table: vec![], total: 0 }, 
                is_selling: true,
                backlog: VecDeque::new()};

            let species_desire_1 = Desire{ 
                item: DesireItem::Product(0), 
                start: 0, 
                end: Some(4), 
                amount: 1.0, 
                satisfaction: 0.0, 
                step: 1, 
                tags: vec![] };
            let species_desire_2 = Desire{ 
                item: DesireItem::Product(1), 
                start: 9, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0, 
                step: 1, 
                tags: vec![] };

            let culture_desire_1 = Desire{ 
                item: DesireItem::Product(2), 
                start: 10, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0, 
                step: 0, 
                tags: vec![] };
            let culture_desire_2 = Desire{ 
                item: DesireItem::Product(3), 
                start: 15, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0, 
                step: 10, 
                tags: vec![] };

            let ideology_desire_1 = Desire{ 
                item: DesireItem::Product(4), 
                start: 30, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0, 
                step: 0, 
                tags: vec![] };
            let ideology_desire_2 = Desire{ 
                item: DesireItem::Product(5), 
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
            .find(|x| x.item == DesireItem::Product(0)).expect("Item Not found");
            assert_eq!(desire_test.amount, 20.0);
            // species desire 1 x 20
            let desire_test = test.property.desires.iter()
            .find(|x| x.item == DesireItem::Product(1)).expect("Item Not found");
            assert_eq!(desire_test.amount, 20.0);
            // culture desire 1 x 10
            let desire_test = test.property.desires.iter()
            .find(|x| x.item == DesireItem::Product(2)).expect("Item Not found");
            assert_eq!(desire_test.amount, 10.0);
            // culture desire 2 x 10
            let desire_test = test.property.desires.iter()
            .find(|x| x.item == DesireItem::Product(3)).expect("Item Not found");
            assert_eq!(desire_test.amount, 10.0);
            // ideology desire 1 x 10
            let desire_test = test.property.desires.iter()
            .find(|x| x.item == DesireItem::Product(4)).expect("Item Not found");
            assert_eq!(desire_test.amount, 10.0);
            // ideology desire 1 x 10
            let desire_test = test.property.desires.iter()
            .find(|x| x.item == DesireItem::Product(5)).expect("Item Not found");
            assert_eq!(desire_test.amount, 10.0);

        }

        // FIXME
        mod standard_sell {
            use std::{collections::HashMap, thread, time::Duration};
            use crate::objects::{actor_message::{ActorInfo, ActorMessage, OfferResult}, seller::Seller, property_info::PropertyInfo};
            use super::{make_test_pop, prepare_data_for_market_actions};

            #[test]
            pub fn should_send_out_of_stock_and_return_when_unable_to_sell_item() {
                let mut test = make_test_pop();
                let pop_info = test.actor_info();
                let (data, history) = prepare_data_for_market_actions(&mut test);
                // setup message queu
                let (tx, rx) = barrage::bounded(10);
                let mut passed_rx = rx.clone();
                let passed_tx = tx.clone();
                // setup firm we're talking with
                let buyer = ActorInfo::Firm(1);
                // setup property split
                let mut keep = HashMap::new();

                keep.insert(6, 
                        PropertyInfo::new(test.property.property.get(&6).unwrap().total_property + 10.0));

                let handle = thread::spawn(move || {
                    test.standard_sell(&mut passed_rx, &passed_tx, &data, &history, 
                        10, buyer);
                    test
                }); 
                // wait a second to let it wrap up.
                thread::sleep(Duration::from_millis(100));
                // check that it's finished
                if !handle.is_finished() { assert!(false); }
                // confirm the message was sent
                let result = rx.recv().unwrap();
                if let ActorMessage::NotInStock { buyer, seller, 
                product } = result {
                    assert_eq!(buyer, buyer);
                    assert_eq!(seller, pop_info);
                    assert_eq!(product, 10);
                } else { assert!(false); }
                // get the seller back
                let test = handle.join().unwrap();

                // ensure that the seller hasn't sold anything
                assert!(test.property.property.get(&6).unwrap().total_property == 10.0);
                assert!(test.property.property.get(&7).is_none());
            }

            #[test]
            pub fn should_send_in_stock_and_accept_reject_purchase_and_close_deal() {
                let mut test = make_test_pop();
                let pop_info = test.actor_info();
                let (data, history) = prepare_data_for_market_actions(&mut test);
                // setup message queu
                let (tx, rx) = barrage::bounded(10);
                let mut passed_rx = rx.clone();
                let passed_tx = tx.clone();
                // setup firm we're talking with
                let buyer = ActorInfo::Firm(1);
                // update the property and spend to have product 7.
                test.property.property.clear();
                test.property.property.insert(7, PropertyInfo::new(10.0));
                let mut keep = HashMap::new();
                keep.insert(7, PropertyInfo::new(10.0));

                let handle = thread::spawn(move || {
                    test.standard_sell(&mut passed_rx, &passed_tx, &data, &history, 
                        7, buyer);
                    test
                }); 
                // wait a second to let it wrap up.
                thread::sleep(Duration::from_millis(100));
                // check that it's not finished
                if handle.is_finished() { assert!(false); }
                // confirm the message was sent
                let result = rx.recv().unwrap();
                if let ActorMessage::InStock { buyer, seller, 
                product, price, quantity } = result {
                    assert_eq!(buyer, buyer);
                    assert_eq!(seller, pop_info);
                    assert_eq!(product, 7);
                    assert!(price == 5.0);
                    assert!(quantity == 10.0);
                } else { assert!(false); }

                // send RejectPurchase and Close Deal Messages
                tx.send(ActorMessage::RejectPurchase { buyer, 
                    seller: pop_info, product: 7, 
                    price_opinion: OfferResult::TooExpensive })
                    .expect("Failed to send.");
                tx.send(ActorMessage::CloseDeal { buyer, seller: pop_info, 
                    product: 7 }).expect("Failed to send, here!");
                thread::sleep(Duration::from_millis(100));
                // get the seller back
                let test = handle.join().unwrap();

                // ensure that the seller hasn't sold anything
                assert!(test.property.property.get(&7).unwrap().total_property == 10.0);
                assert!(test.property.property.get(&6).is_none());
            }

            #[test]
            pub fn should_send_in_stock_recieve_buy_offer_and_close_when_offer_underprice() {
                let mut test = make_test_pop();
                let pop_info = test.actor_info();
                let (data, history) = prepare_data_for_market_actions(&mut test);
                // setup message queu
                let (tx, rx) = barrage::bounded(10);
                let mut passed_rx = rx.clone();
                let passed_tx = tx.clone();
                // setup firm we're talking with
                let buyer = ActorInfo::Firm(1);
                // update the property and spend to have product 7.
                test.property.property.clear();
                test.property.property.insert(7, PropertyInfo::new(10.0));
                let mut keep = HashMap::new();
                keep.insert(7, PropertyInfo::new(10.0));

                let handle = thread::spawn(move || {
                    let result = test.standard_sell(&mut passed_rx, &passed_tx, &data, &history, 
                        7, buyer);
                    (test, result)
                }); 
                // wait a second to let it wrap up.
                thread::sleep(Duration::from_millis(100));
                // check that it's not finished
                if handle.is_finished() { assert!(false); }
                // confirm the message was sent
                let result = rx.recv().unwrap();
                if let ActorMessage::InStock { buyer: _, seller, 
                product, price, quantity } = result {
                    assert_eq!(seller, pop_info);
                    assert_eq!(product, 7);
                    assert!(price == 5.0);
                    assert!(quantity == 10.0);
                } else { assert!(false); }

                // send Buy Offer. 3 of item 6.
                tx.send(ActorMessage::BuyOffer { buyer, seller: pop_info, 
                    product: 7, price_opinion: OfferResult::Reasonable, 
                    quantity: 1.0, followup: 1 })
                    .expect("Failed to send.");
                tx.send(ActorMessage::BuyOfferFollowup { buyer, seller: pop_info, 
                    product: 7, offer_product: 6, offer_quantity: 2.0, 
                    followup: 0 })
                    .expect("Failed to send.");
                thread::sleep(Duration::from_millis(100));
                // get the seller back
                let (test, returned) = handle.join().unwrap();

                // check that we get back the close deal rejection
                let mut result = ActorMessage::AllFinished;
                while let Ok(msg) = rx.try_recv() {
                    if let Some(msg) = msg {
                        result = msg; // last message should be the expected close
                    } else { break; }
                }

                if let ActorMessage::CloseDeal { buyer, seller,
                product } = result {
                    assert_eq!(buyer, buyer);
                    assert_eq!(seller, pop_info);
                    assert_eq!(product, 7);
                }

                // ensure that the seller hasn't sold anything
                assert!(test.property.property.get(&7).unwrap().total_property == 10.0);
                assert!(test.property.property.get(&6).is_none());
                assert_eq!(returned.len(), 0);
            }

            #[test]
            pub fn should_send_in_stock_recieve_buy_offer_and_accept_when_properly_paid() {
                let mut test = make_test_pop();
                let pop_info = test.actor_info();
                let (data, history) = prepare_data_for_market_actions(&mut test);
                // setup message queue
                let (tx, rx) = barrage::bounded(10);
                let mut passed_rx = rx.clone();
                let passed_tx = tx.clone();
                // setup firm we're talking with
                let buyer = ActorInfo::Firm(1);
                // update the property and spend to have product 7.
                test.property.property.clear();
                test.property.property.insert(7, PropertyInfo::new(10.0));
                let mut keep = HashMap::new();
                keep.insert(7, PropertyInfo::new(10.0));

                let handle = thread::spawn(move || {
                    let result = test.standard_sell(&mut passed_rx, &passed_tx, &data, &history, 
                        7, buyer);
                    (test, result)
                }); 
                // wait a second to let it wrap up.
                thread::sleep(Duration::from_millis(100));
                // check that it's not finished
                if handle.is_finished() { assert!(false); }
                // confirm the message was sent
                let result = rx.recv().unwrap();
                if let ActorMessage::InStock { buyer: _, seller, 
                product, price, quantity } = result {
                    assert_eq!(seller, pop_info);
                    assert_eq!(product, 7);
                    assert!(price == 5.0);
                    assert!(quantity == 10.0);
                } else { assert!(false); }

                // send Buy Offer. 6 of item 6.
                tx.send(ActorMessage::BuyOffer { buyer, seller: pop_info, 
                    product: 7, price_opinion: OfferResult::Reasonable, 
                    quantity: 1.0, followup: 1 })
                    .expect("Failed to send.");
                tx.send(ActorMessage::BuyOfferFollowup { buyer, seller: pop_info, 
                    product: 7, offer_product: 6, offer_quantity: 6.0, 
                    followup: 0 })
                    .expect("Failed to send.");
                thread::sleep(Duration::from_millis(100));
                // get the seller back
                let (test, returned) = handle.join().unwrap();

                // check that we get back the close deal rejection
                let mut result = ActorMessage::AllFinished;
                while let Ok(msg) = rx.try_recv() {
                    if let Some(msg) = msg {
                        result = msg; // last message should be the expected close
                    } else { break; }
                }
                if let ActorMessage::SellerAcceptOfferAsIs { buyer, seller, 
                product, offer_result: _ } = result {
                    assert_eq!(buyer, buyer);
                    assert_eq!(seller, pop_info);
                    assert_eq!(product, 7);
                } else { assert!(false); }

                // ensure that the seller has sold correctly
                assert!(test.property.property.get(&7).unwrap().total_property == 9.0);
                assert!(test.property.property.get(&6).is_none());
                assert!(*returned.get(&6).unwrap() == 6.0);
            }

            // TODO When returning change is possible, add test here and update previous test.
        }

        mod msg_tests {
            use std::{thread, time::Duration, collections::{HashMap, HashSet}};

            use crate::objects::{actor_message::{ActorMessage, ActorInfo, OfferResult}, seller::Seller};

            use super::make_test_pop;

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
            pub fn should_wait_only_on_specific_messages_requested() {
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

            use crate::{objects::{actor_message::{ActorInfo, FirmEmployeeAction, ActorMessage}, seller::Seller, property_info::PropertyInfo}, constants::TIME_ID};

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
                test.property.property.insert(TIME_ID, PropertyInfo::new(20.0));
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
                        assert_eq!(product, TIME_ID);
                        assert_eq!(amount, work_time);
                    }
                    else { assert!(false); }
                } else { assert!(false); }
                // ensure nothing else sent
                let rx_msg = rx.try_recv().expect("Unexpected Disconnect?");
                assert!(rx_msg.is_none());
                // check that the pop has reduced it's time appropriately.
                assert_eq!(test.property.property.get(&TIME_ID).unwrap().total_property, 10.0);
            }

            #[test]
            pub fn should_send_everything_when_everything_requested() {
                let mut test = make_test_pop();
                let (data, _market) = prepare_data_for_market_actions(&mut test);
                // add the pop's time to work from memory
                let work_time = 10.0;
                test.property.work_time = work_time;
                test.property.property.insert(TIME_ID, PropertyInfo::new(20.0));
                test.property.property.insert(3, PropertyInfo::new(10.0));
                test.property.property.insert(5, PropertyInfo::new(10.0));
                test.property.want_store.insert(4, 20.0);
                test.property.want_store.insert(6, 5.0);
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
                assert_eq!(*rec_prods.get(&TIME_ID).unwrap(), 20.0);
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
                test.property.property.insert(TIME_ID, PropertyInfo::new(20.0));
                test.property.property.insert(3, PropertyInfo::new(10.0));
                test.property.property.insert(5, PropertyInfo::new(10.0));
                test.property.want_store.insert(4, 20.0);
                test.property.want_store.insert(6, 5.0);
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
                assert_eq!(test.property.property.get(&TIME_ID).unwrap().total_property, 20.0);
                assert_eq!(test.property.property.get(&5).unwrap().total_property, 10.0);
                assert!(!test.property.property.contains_key(&3));
                assert_eq!(test.property.want_store.len(), 2);
                assert_eq!(*test.property.want_store.get(&4).unwrap(), 20.0);
                assert_eq!(*test.property.want_store.get(&6).unwrap(), 5.0);
            }
        }

        // Working
        mod work_day_processing {
            use std::{thread, time::Duration};

            use crate::{objects::{actor_message::{FirmEmployeeAction, ActorMessage, ActorInfo, OfferResult::Cheap}, seller::Seller}, tests::pop_tests::prepare_data_for_market_actions};

            use super::make_test_pop;

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
                assert_eq!(*test.property.want_store.get(&0).unwrap(), 10.0);
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
                    price_opinion: Cheap, quantity: 10.0, followup: 0 };

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
                    if let Cheap = price_opinion {
                        // nothing
                    } else { assert!(false); }
                    assert_eq!(quantity, 10.0);
                    assert_eq!(followup, 0);
                } else { assert!(false); }
            }
        }

        // Working
        mod create_offer_tests {
            use std::collections::HashMap;

            use crate::{data_manager::DataManager, objects::{market::{MarketHistory, ProductInfo}, property_info::PropertyInfo}};

            use super::make_test_pop;

            #[test]
            pub fn should_return_everything_when_short() {
                let pop = make_test_pop();

                let mut data = DataManager::new();

                // TODO when load_all is updated to take a file, relpace this with the 'default' load.
                data.load_all(&"X".to_string()).expect("Error loading prefabs");

                // Add items to spend.
                let mut spend = HashMap::new();
                spend.insert(2, PropertyInfo::new(10.0));
                spend.insert(3, PropertyInfo::new(10.0));
                spend.insert(4, PropertyInfo::new(10.0));
                spend.insert(5, PropertyInfo::new(10.0));
                spend.insert(6, PropertyInfo::new(10.0));
                spend.insert(7, PropertyInfo::new(10.0));

                let mut market = MarketHistory {
                    info: HashMap::new(),
                    sale_priority: vec![],
                    currencies: vec![],
                };
                market.info.insert(2, ProductInfo {
                    available: 0.0,
                    price: 1.0,
                    offered: 0.0,
                    sold: 0.0,
                    salability: 100.0,
                    is_currency: true,
                });
                market.info.insert(3, ProductInfo {
                    available: 0.0,
                    price: 1.0,
                    offered: 0.0,
                    sold: 0.0,
                    salability: 1.0,
                    is_currency: false,
                });
                market.info.insert(4, ProductInfo {
                    available: 0.0,
                    price: 1.0,
                    offered: 0.0,
                    sold: 0.0,
                    salability: 1.0,
                    is_currency: false,
                });
                market.info.insert(5, ProductInfo {
                    available: 0.0,
                    price: 1.0,
                    offered: 0.0,
                    sold: 0.0,
                    salability: 1.0,
                    is_currency: false,
                });
                market.info.insert(6, ProductInfo {
                    available: 0.0,
                    price: 3.0,
                    offered: 0.0,
                    sold: 0.0,
                    salability: 1.0,
                    is_currency: false,
                });
                market.info.insert(7, ProductInfo {
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
                data.load_all(&"X".to_string()).expect("Error loading prefabs");

                // Add items to spend.
                let mut spend = HashMap::new();
                spend.insert(2, PropertyInfo::new(10.0));
                spend.insert(3, PropertyInfo::new(10.0));
                spend.insert(4, PropertyInfo::new(10.0));
                spend.insert(5, PropertyInfo::new(10.0));
                spend.insert(6, PropertyInfo::new(10.0));
                spend.insert(7, PropertyInfo::new(10.0));

                let mut market = MarketHistory {
                    info: HashMap::new(),
                    sale_priority: vec![],
                    currencies: vec![],
                };
                market.info.insert(2, ProductInfo {
                    available: 0.0,
                    price: 1.0,
                    offered: 0.0,
                    sold: 0.0,
                    salability: 100.0,
                    is_currency: true,
                });
                market.info.insert(3, ProductInfo {
                    available: 0.0,
                    price: 1.0,
                    offered: 0.0,
                    sold: 0.0,
                    salability: 1.0,
                    is_currency: false,
                });
                market.info.insert(4, ProductInfo {
                    available: 0.0,
                    price: 1.0,
                    offered: 0.0,
                    sold: 0.0,
                    salability: 1.0,
                    is_currency: false,
                });
                market.info.insert(5, ProductInfo {
                    available: 0.0,
                    price: 1.0,
                    offered: 0.0,
                    sold: 0.0,
                    salability: 1.0,
                    is_currency: false,
                });
                market.info.insert(6, ProductInfo {
                    available: 0.0,
                    price: 3.0,
                    offered: 0.0,
                    sold: 0.0,
                    salability: 1.0,
                    is_currency: false,
                });
                market.info.insert(7, ProductInfo {
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
                data.load_all(&"X".to_string()).expect("Error loading prefabs");
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
                    info: HashMap::new(),
                    sale_priority: vec![],
                    currencies: vec![],
                };
                market.info.insert(2, ProductInfo {
                    available: 0.0,
                    price: 1.0,
                    offered: 0.0,
                    sold: 0.0,
                    salability: 100.0,
                    is_currency: true,
                });
                market.info.insert(3, ProductInfo {
                    available: 0.0,
                    price: 1.0,
                    offered: 0.0,
                    sold: 0.0,
                    salability: 1.0,
                    is_currency: false,
                });
                market.info.insert(4, ProductInfo {
                    available: 0.0,
                    price: 1.0,
                    offered: 0.0,
                    sold: 0.0,
                    salability: 1.0,
                    is_currency: false,
                });
                market.info.insert(5, ProductInfo {
                    available: 0.0,
                    price: 1.0,
                    offered: 0.0,
                    sold: 0.0,
                    salability: 1.0,
                    is_currency: false,
                });
                market.info.insert(6, ProductInfo {
                    available: 0.0,
                    price: 2.0,
                    offered: 0.0,
                    sold: 0.0,
                    salability: 1.0,
                    is_currency: false,
                });
                market.info.insert(7, ProductInfo {
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
                data.load_all(&"X".to_string()).expect("Error loading prefabs");

                // Add items to spend.
                let mut spend = HashMap::new();
                spend.insert(2, PropertyInfo::new(10.0));
                spend.insert(3, PropertyInfo::new(10.0));
                spend.insert(4, PropertyInfo::new(10.0));
                spend.insert(5, PropertyInfo::new(10.0));
                spend.insert(6, PropertyInfo::new(10.0));
                spend.insert(7, PropertyInfo::new(10.0));

                let mut market = MarketHistory {
                    info: HashMap::new(),
                    sale_priority: vec![],
                    currencies: vec![],
                };
                market.info.insert(2, ProductInfo {
                    available: 0.0,
                    price: 2.0,
                    offered: 0.0,
                    sold: 0.0,
                    salability: 100.0,
                    is_currency: true,
                });
                market.info.insert(3, ProductInfo {
                    available: 0.0,
                    price: 3.0,
                    offered: 0.0,
                    sold: 0.0,
                    salability: 1.0,
                    is_currency: false,
                });
                market.info.insert(4, ProductInfo {
                    available: 0.0,
                    price: 4.0,
                    offered: 0.0,
                    sold: 0.0,
                    salability: 1.0,
                    is_currency: false,
                });
                market.info.insert(5, ProductInfo {
                    available: 0.0,
                    price: 15.0,
                    offered: 0.0,
                    sold: 0.0,
                    salability: 1.0,
                    is_currency: false,
                });
                market.info.insert(6, ProductInfo {
                    available: 0.0,
                    price: 3.0,
                    offered: 0.0,
                    sold: 0.0,
                    salability: 1.0,
                    is_currency: false,
                });
                market.info.insert(7, ProductInfo {
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
    
        mod  standard_buy_should {
            use std::{collections::HashMap, thread, time::Duration};
            use crate::objects::{actor_message::{ActorInfo, ActorMessage, OfferResult}, seller::Seller, buy_result::BuyResult, property_info::PropertyInfo, desire::{Desire, DesireItem}};
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
                        &history, seller);
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
                val.max_target = 10.0;
                val.min_target = 0.0;

                // The pop is trying to buy 10 cabins at tier 50
                // It should always include the 20.0 units of Ambrosia fruit, 
                // which they have in excess.
                // They should also include 20.0 sets of clothes, which are available in excess.
                // Anything else offered would be 
                // set the prices of ambrosia fruit, clothes, and cabins so that the cabin is just purchaseable with
                // 20 ambrosia fruit and 20 cotton clothes.
                history.info.get_mut(&2).unwrap().price = 1.0; // 20.0 total
                history.info.get_mut(&6).unwrap().price = 2.0; // 40.0 total
                history.info.get_mut(&15).unwrap().price = 5.9; // 59.0 total

                // setup message queue.
                let (tx, rx) = barrage::bounded(10);
                let mut passed_rx = rx.clone();
                let passed_tx = tx.clone();
                // setup firm we're talking with
                let selling_firm = ActorInfo::Firm(1);
                // setup property split

                let handle = thread::spawn(move || {
                    let result = test.standard_buy(&mut passed_rx, &passed_tx, 
                        &data, &history, selling_firm);
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
                assert_eq!(test.property.property[&15].time_cost, test.standard_shop_time_cost());
            }

            #[test]
            pub fn correctly_release_class_desire_for_buy_offer() {
                let mut test = make_test_pop();
                let pop_info = test.actor_info();
                let (data, mut history) = prepare_data_for_market_actions(&mut test);
                // swap out infinite clothes for the class desire instead.
                test.property.desires.get_mut(4).unwrap()
                    .item = DesireItem::Class(6);
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
                val.max_target = 10.0;
                val.min_target = 0.0;

                // The pop is trying to buy 10 cabins at tier 50
                // It should always include the 20.0 units of Ambrosia fruit, 
                // which they have in excess.
                // They should also include 20.0 sets of clothes, which are available in excess.
                // Anything else offered would be 
                // set the prices of ambrosia fruit, clothes, and cabins so that the cabin is just purchaseable with
                // 20 ambrosia fruit and 20 cotton clothes.
                history.info.get_mut(&2).unwrap().price = 1.0; // 20.0 total
                history.info.get_mut(&6).unwrap().price = 2.0; // 40.0 total
                history.info.get_mut(&7).unwrap().price = 10.0; // 0.0 likely to exchange.
                history.info.get_mut(&15).unwrap().price = 5.9; // 59.0 total

                // setup message queue.
                let (tx, rx) = barrage::bounded(10);
                let mut passed_rx = rx.clone();
                let passed_tx = tx.clone();
                // setup firm we're talking with
                let selling_firm = ActorInfo::Firm(1);
                // setup property split

                let handle = thread::spawn(move || {
                    let result = test.standard_buy(&mut passed_rx, &passed_tx, 
                        &data, &history, selling_firm);
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
                assert_eq!(test.property.property[&15].time_cost, test.standard_shop_time_cost());
            }

            #[test]
            pub fn correctly_release_want_desire_for_buy_offer() {
                let mut test = make_test_pop();
                let pop_info = test.actor_info();
                let (data, mut history) = prepare_data_for_market_actions(&mut test);
                // swap out infinite clothes for the want desire instead.
                test.property.desires.get_mut(4).unwrap()
                    .item = DesireItem::Want(2);
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
                val.max_target = 10.0;
                val.min_target = 0.0;

                // The pop is trying to buy 10 cabins at tier 50
                // It should always include the 20.0 units of Ambrosia fruit, 
                // which they have in excess.
                // They should also include 20.0 sets of clothes, which are available in excess.
                // Anything else offered would be 
                // set the prices of ambrosia fruit, clothes, and cabins so that the cabin is just purchaseable with
                // 20 ambrosia fruit and 20 cotton clothes.
                history.info.get_mut(&2).unwrap().price = 1.0; // 20.0 total
                history.info.get_mut(&6).unwrap().price = 2.0; // 40.0 total
                history.info.get_mut(&7).unwrap().price = 10.0; // 0.0 likely to exchange.
                history.info.get_mut(&15).unwrap().price = 5.9; // 59.0 total

                // setup message queue.
                let (tx, rx) = barrage::bounded(10);
                let mut passed_rx = rx.clone();
                let passed_tx = tx.clone();
                // setup firm we're talking with
                let selling_firm = ActorInfo::Firm(1);
                // setup property split

                let handle = thread::spawn(move || {
                    let result = test.standard_buy(&mut passed_rx, &passed_tx, 
                        &data, &history, selling_firm);
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
                assert_eq!(test.property.property[&15].time_cost, test.standard_shop_time_cost());
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
                val.max_target = 10.0;
                val.min_target = 0.0;

                // The pop is trying to buy 10 cabins at tier 50
                // It should always include the 20.0 units of Ambrosia fruit, 
                // which they have in excess.
                // They should also include 20.0 sets of clothes, which are available in excess.
                // Anything else offered would be 
                // set the prices of ambrosia fruit, clothes, and cabins so that the cabin is just purchaseable with
                // 20 ambrosia fruit and 20 cotton clothes.
                history.info.get_mut(&2).unwrap().price = 1.0; // 20.0 total
                history.info.get_mut(&6).unwrap().price = 2.0; // 40.0 total
                history.info.get_mut(&15).unwrap().price = 5.9; // 59.0 total

                // setup message queue.
                let (tx, rx) = barrage::bounded(10);
                let mut passed_rx = rx.clone();
                let passed_tx = tx.clone();
                // setup firm we're talking with
                let selling_firm = ActorInfo::Firm(1);
                // setup property split

                let handle = thread::spawn(move || {
                    let result = test.standard_buy(&mut passed_rx, &passed_tx, 
                        &data, &history, selling_firm);
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
            }

            #[test]
            pub fn should_return_success_when_able_to_buy_single_offer_with_positive_change() {
                let mut test = make_test_pop();
                let pop_info = test.actor_info();
                let (data, history) = prepare_data_for_market_actions(&mut test);
                // setup message queue.
                let (tx, rx) = barrage::bounded(10);
                let mut passed_rx = rx.clone();
                let passed_tx = tx.clone();
                // setup firm we're talking with
                let selling_firm = ActorInfo::Firm(1);
                // setup property split

                let handle = thread::spawn(move || {
                    let result = test.standard_buy(&mut passed_rx, &passed_tx, &data, &history, 
                        selling_firm);
                    (test, result)
                });
                // check that it's running
                if handle.is_finished() { assert!(false); }

                // send the out of stock message.
                tx.send(ActorMessage::InStock { buyer: pop_info, 
                    seller: selling_firm, product: 7, price: 5.0, 
                    quantity: 100.0 }).expect("Sudden Disconnect?");
                thread::sleep(Duration::from_millis(100));
                // should have the first message we sent
                if let ActorMessage::InStock { .. } = rx.recv().unwrap() {
                } else { assert!(false); }
                // it should have sent a buy order of 10.0 units of DI6
                if let ActorMessage::BuyOffer { buyer, 
                seller, product, 
                price_opinion, quantity, 
                followup } = rx.recv().unwrap() {
                    assert_eq!(buyer, pop_info);
                    assert_eq!(seller, selling_firm);
                    assert_eq!(product, 7);
                    assert_eq!(price_opinion, OfferResult::Reasonable);
                    assert_eq!(quantity, 2.0);
                    assert_eq!(followup, 1);
                } else { assert!(false); }
                // then check that the sent the expected amount
                if let ActorMessage::BuyOfferFollowup { buyer, 
                seller, product, 
                offer_product, offer_quantity, 
                followup } = rx.recv().unwrap() {
                    assert_eq!(buyer, pop_info);
                    assert_eq!(seller, selling_firm);
                    assert_eq!(product, 7);
                    assert_eq!(offer_product, 6);
                    assert_eq!(offer_quantity, 5.0);
                    assert_eq!(followup, 0);
                } else { assert!(false); }

                // with the offer sent correctly, send our acceptance and go forward
                // the acceptance should include change, it will give back 1 unit of the items offered
                // and 1 unit of another item to give them more to test.
                tx.send(ActorMessage::OfferAcceptedWithChange { buyer: pop_info, 
                    seller: selling_firm, product: 7, quantity: 2.0, followups: 2 })
                    .expect("Disconnected?");
                tx.send(ActorMessage::ChangeFollowup { buyer: pop_info, 
                    seller: selling_firm, product: 7, return_product: 6, 
                    return_quantity: 1.0, followups: 1 })
                    .expect("Disconnected?");
                tx.send(ActorMessage::ChangeFollowup { buyer: pop_info, 
                    seller: selling_firm, product: 7, return_product: 5, 
                    return_quantity: 1.0, followups: 0 })
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
                assert!(test.property.property.get(&5).unwrap().total_property == 1.0);
                assert!(test.property.property.get(&6).unwrap().total_property == 6.0);
                assert!(test.property.property.get(&7).unwrap().total_property == 2.0);
                // check returned correctly includes items.
                assert!(test.property.property.get(&5).unwrap().unreserved == 1.0);
                // check pop memory for the products as well.
                // TODO check this stuff again.
            }

            #[test]
            pub fn should_return_not_successful_when_reject_offer_recieved_after_offer() {
                let mut test = make_test_pop();
                let pop_info = test.actor_info();
                let (data, history) = prepare_data_for_market_actions(&mut test);
                // setup message queue.
                let (tx, rx) = barrage::bounded(10);
                let mut passed_rx = rx.clone();
                let passed_tx = tx.clone();
                // setup firm we're talking with
                let selling_firm = ActorInfo::Firm(1);
                // setup property split

                let handle = thread::spawn(move || {
                    let result = test.standard_buy(&mut passed_rx, &passed_tx, &data, &history, 
                        selling_firm);
                    (test, result)
                });
                // check that it's running
                if handle.is_finished() { assert!(false); }

                // send the out of stock message.
                tx.send(ActorMessage::InStock { buyer: pop_info, 
                    seller: selling_firm, product: 7, price: 5.0, 
                    quantity: 100.0 }).expect("Sudden Disconnect?");
                thread::sleep(Duration::from_millis(100));
                // should have the first message we sent
                if let ActorMessage::InStock { .. } = rx.recv().unwrap() {
                } else { assert!(false); }
                // it should have sent a buy order of 10.0 units of DI6
                if let ActorMessage::BuyOffer { buyer, 
                seller, product, 
                price_opinion, quantity, 
                followup } = rx.recv().unwrap() {
                    assert_eq!(buyer, pop_info);
                    assert_eq!(seller, selling_firm);
                    assert_eq!(product, 7);
                    assert_eq!(price_opinion, OfferResult::Reasonable);
                    assert_eq!(quantity, 2.0);
                    assert_eq!(followup, 1);
                } else { assert!(false); }
                // then check that the sent the expected amount
                if let ActorMessage::BuyOfferFollowup { buyer, 
                seller, product, 
                offer_product, offer_quantity, 
                followup } = rx.recv().unwrap() {
                    assert_eq!(buyer, pop_info);
                    assert_eq!(seller, selling_firm);
                    assert_eq!(product, 7);
                    assert_eq!(offer_product, 6);
                    assert_eq!(offer_quantity, 5.0);
                    assert_eq!(followup, 0);
                } else { assert!(false); }

                // with the offer sent correctly, send our acceptance and go forward
                // the acceptance should include change, it will give back 1 unit of the items offered
                // and 1 unit of another item to give them more to test.
                tx.send(ActorMessage::RejectOffer { buyer: pop_info, seller: selling_firm, 
                product: 7 })
                    .expect("Disconnected?");
                thread::sleep(Duration::from_millis(100));
                // ensure we closed out
                if !handle.is_finished() { assert!(false); }
                // get our data
                let (test, result) = handle.join().unwrap();
                // check the return is correct.
                if let BuyResult::NotSuccessful { reason } = result {
                    assert!(reason == OfferResult::Rejected);
                } else { assert!(false); }
                // check that property was exchanged
                assert!(test.property.property.get(&6).unwrap().total_property == 10.0);
                assert!(test.property.property.get(&7).is_none());
                // check pop memory for the products as well.
                // TODO update checks
            }

            #[test]
            pub fn should_return_not_successful_when_close_deal_recieved_after_offer() {
                let mut test = make_test_pop();
                let pop_info = test.actor_info();
                let (data, history) = prepare_data_for_market_actions(&mut test);
                // setup message queue.
                let (tx, rx) = barrage::bounded(10);
                let mut passed_rx = rx.clone();
                let passed_tx = tx.clone();
                // setup firm we're talking with
                let selling_firm = ActorInfo::Firm(1);
                // setup property split

                let handle = thread::spawn(move || {
                    let result = test.standard_buy(&mut passed_rx, &passed_tx, &data, &history, 
                        selling_firm);
                    (test, result)
                });
                // check that it's running
                if handle.is_finished() { assert!(false); }

                // send the out of stock message.
                tx.send(ActorMessage::InStock { buyer: pop_info, 
                    seller: selling_firm, product: 7, price: 5.0, 
                    quantity: 100.0 }).expect("Sudden Disconnect?");
                thread::sleep(Duration::from_millis(100));
                // should have the first message we sent
                if let ActorMessage::InStock { .. } = rx.recv().unwrap() {
                } else { assert!(false); }
                // it should have sent a buy order of 10.0 units of DI6
                if let ActorMessage::BuyOffer { buyer, 
                seller, product, 
                price_opinion, quantity, 
                followup } = rx.recv().unwrap() {
                    assert_eq!(buyer, pop_info);
                    assert_eq!(seller, selling_firm);
                    assert_eq!(product, 7);
                    assert_eq!(price_opinion, OfferResult::Reasonable);
                    assert_eq!(quantity, 2.0);
                    assert_eq!(followup, 1);
                } else { assert!(false); }
                // then check that the sent the expected amount
                if let ActorMessage::BuyOfferFollowup { buyer, 
                seller, product, 
                offer_product, offer_quantity, 
                followup } = rx.recv().unwrap() {
                    assert_eq!(buyer, pop_info);
                    assert_eq!(seller, selling_firm);
                    assert_eq!(product, 7);
                    assert_eq!(offer_product, 6);
                    assert_eq!(offer_quantity, 5.0);
                    assert_eq!(followup, 0);
                } else { assert!(false); }

                // with the offer sent correctly, send our acceptance and go forward
                // the acceptance should include change, it will give back 1 unit of the items offered
                // and 1 unit of another item to give them more to test.
                tx.send(ActorMessage::CloseDeal { buyer: pop_info, seller: selling_firm, 
                product: 7 })
                    .expect("Disconnected?");
                thread::sleep(Duration::from_millis(100));
                // ensure we closed out
                if !handle.is_finished() { assert!(false); }
                // get our data
                let (test, result) = handle.join().unwrap();
                // check the return is correct.
                if let BuyResult::SellerClosed = result {
                    assert!(true);
                } else { assert!(false); }
                // check that property was exchanged
                assert!(test.property.property.get(&6).unwrap().total_property == 10.0);
                assert!(test.property.property.get(&7).is_none());
                // check pop memory for the products as well.
                // TODO recheck here
            }
        }
    }

    mod pop_breakdown_table_tests {
        use crate::objects::{pop_breakdown_table::PopBreakdownTable, pop_breakdown_table::PBRow};

        #[test]
        pub fn should_return_species_makeup_correctly() {
            let mut test = PopBreakdownTable{table: vec![], total: 0};

            let first_row = PBRow::new(1,
                Some(0),Some(0),Some(0),
                Some(0),Some(0),
                Some(0),Some(0),Some(0),
                10);

            let second_row = PBRow::new(2,
                Some(0),Some(0),Some(0),
                Some(0),Some(0),
                Some(1),Some(0),Some(0),
                10);

            let third_row = PBRow::new(2,
                Some(0),Some(0),Some(0),
                Some(0),Some(0),
                None,Some(0),Some(0),
                20);

            test.insert_pops(first_row);
            test.insert_pops(second_row);
            test.insert_pops(third_row);

            let result = test.species_makeup();

            assert_eq!(result.len(), 2);
            assert_eq!(result[&1], 10);
            assert_eq!(result[&2], 30);
        }

        #[test]
        pub fn should_return_culture_makeup_correctly() {
            let mut test = PopBreakdownTable{table: vec![], total: 0};

            let first_row = PBRow::new(1,
                Some(0),Some(0),None,
                Some(0),Some(0),
                Some(0),Some(0),Some(0),
                10);

            let second_row = PBRow::new(2,
                Some(0),Some(0),Some(0),
                Some(0),Some(0),
                Some(0),Some(0),Some(0),
                10);

            let third_row = PBRow::new(2,
                Some(0),Some(0),Some(0),
                Some(0),Some(0),
                None,Some(0),Some(0),
                20);

            test.insert_pops(first_row);
            test.insert_pops(second_row);
            test.insert_pops(third_row);

            let result = test.culture_makeup();

            assert_eq!(result.len(), 2);
            assert_eq!(result[&None], 10);
            assert_eq!(result[&Some(0)], 30);
        }

        #[test]
        pub fn should_return_ideology_makeup_correctly() {
            let mut test = PopBreakdownTable{table: vec![], total: 0};

            let first_row = PBRow::new(1,
                Some(0),Some(0),None,
                Some(0),Some(0),
                None,Some(0),Some(0),
                10);

            let second_row = PBRow::new(2,
                Some(0),Some(0),Some(0),
                Some(0),Some(0),
                Some(0),Some(0),Some(0),
                10);

            let third_row = PBRow::new(2,
                Some(0),Some(0),Some(0),
                Some(0),Some(0),
                None,Some(0),Some(0),
                20);

            test.insert_pops(first_row);
            test.insert_pops(second_row);
            test.insert_pops(third_row);

            let result = test.ideology_makeup();

            assert_eq!(result.len(), 2);
            assert_eq!(result[&None], 30);
            assert_eq!(result[&Some(0)], 10);
        }

        #[test]
        pub fn should_divide_on_ideologies_correctly() {
            let mut test = PopBreakdownTable{table: vec![], total: 0};

            let first_row = PBRow::new(1,
                Some(0),Some(0),Some(0),
                Some(0),Some(0),
                Some(0),Some(0),Some(0),
                10);

            let second_row = PBRow::new(1,
                Some(0),Some(0),Some(0),
                Some(0),Some(0),
                Some(1),Some(0),Some(0),
                10);

            let third_row = PBRow::new(1,
                Some(0),Some(0),Some(0),
                Some(0),Some(0),
                None,Some(0),Some(0),
                20);

            test.insert_pops(first_row);
            test.insert_pops(second_row);
            test.insert_pops(third_row);

            let division = test.ideology_division();

            assert_eq!(division.len(), 3);
            assert_eq!(division[&Some(0)], 0.25);
            assert_eq!(division[&Some(1)], 0.25);
            assert_eq!(division[&None], 0.5);
        }

        #[test]
        pub fn should_divide_on_cultures_correctly() {
            let mut test = PopBreakdownTable{table: vec![], total: 0};

            let first_row = PBRow::new(1,
                Some(0),Some(0),Some(0),
                Some(0),Some(0),
                Some(0),Some(0),Some(0),
                10);

            let second_row = PBRow::new(1,
                Some(0),Some(0),Some(1),
                Some(0),Some(0),
                Some(0),Some(0),Some(0),
                10);

            let third_row = PBRow::new(1,
                Some(0),Some(0),None,
                Some(0),Some(0),
                Some(0),Some(0),Some(0),
                20);

            test.insert_pops(first_row);
            test.insert_pops(second_row);
            test.insert_pops(third_row);

            let division = test.culture_division();

            assert_eq!(division.len(), 3);
            assert_eq!(division[&Some(0)], 0.25);
            assert_eq!(division[&Some(1)], 0.25);
            assert_eq!(division[&None], 0.5);
        }

        #[test]
        pub fn should_divide_on_species_correctly() {
            let mut test = PopBreakdownTable{table: vec![], total: 0};

            let first_row = PBRow::new(0,
                Some(0),Some(0),Some(0),
                Some(0),Some(0),
                Some(0),Some(0),Some(0),
                10);

            let second_row = PBRow::new(1,
                Some(0),Some(0),Some(0),
                Some(0),Some(0),
                Some(0),Some(0),Some(0),
                10);

            let third_row = PBRow::new(2,
                Some(0),Some(0),Some(0),
                Some(0),Some(0),
                Some(0),Some(0),Some(0),
                20);

            test.insert_pops(first_row);
            test.insert_pops(second_row);
            test.insert_pops(third_row);

            let division = test.species_division();

            assert_eq!(division.len(), 3);
            assert_eq!(division[&0], 0.25);
            assert_eq!(division[&1], 0.25);
            assert_eq!(division[&2], 0.5);
        }

        #[test]
        pub fn should_remove_pops_correctly() {
            let mut test = PopBreakdownTable{table: vec![], total: 0};

            let first_row = PBRow::new(0,
                Some(0),Some(0),Some(0),
                Some(0),Some(0),
                Some(0),Some(0),Some(0),
                20);

            test.insert_pops(first_row);

            let second_row = PBRow::new(0,
                Some(0),Some(0),Some(0),
                Some(0),Some(0),
                Some(0),Some(0),Some(0),
                15);

            test.remove_pops(&second_row);

            assert_eq!(test.table.len(), 1);
            assert_eq!(test.total, 5);
            assert_eq!(test.table[0].count, 5);
            assert_eq!(test.table[0].species, 0);
            assert_eq!(test.table[0].species_cohort, Some(0));
            assert_eq!(test.table[0].species_cohort, Some(0));
            assert_eq!(test.table[0].culture, Some(0));
            assert_eq!(test.table[0].culture_class, Some(0));
            assert_eq!(test.table[0].culture_generation, Some(0));
            assert_eq!(test.table[0].ideology, Some(0));
            assert_eq!(test.table[0].ideology_faction, Some(0));
            assert_eq!(test.table[0].ideology_wave, Some(0));

            test.remove_pops(&second_row);

            assert_eq!(test.table.len(), 0);
            assert_eq!(test.total, 0);
        }

        #[test]
        pub fn should_insert_pops_correctly() {
            let mut test = PopBreakdownTable{table: vec![], total: 0};

            let first_row = PBRow::new(0,
                Some(0),Some(0),Some(0),
                Some(0),Some(0),
                Some(0),Some(0),Some(0),
                10);

            test.insert_pops(first_row);

            assert_eq!(test.table.len(), 1);
            assert_eq!(test.total, 10);
            assert_eq!(test.table[0].count, 10);
            assert_eq!(test.table[0].species, 0);
            assert_eq!(test.table[0].species_cohort, Some(0));
            assert_eq!(test.table[0].species_cohort, Some(0));
            assert_eq!(test.table[0].culture, Some(0));
            assert_eq!(test.table[0].culture_class, Some(0));
            assert_eq!(test.table[0].culture_generation, Some(0));
            assert_eq!(test.table[0].ideology, Some(0));
            assert_eq!(test.table[0].ideology_faction, Some(0));
            assert_eq!(test.table[0].ideology_wave, Some(0));

            let second_row = PBRow::new(1,
                Some(0),Some(0),Some(0),
                Some(0),Some(0),
                Some(0),Some(0),Some(0),
                10);

            test.insert_pops(second_row);

            assert_eq!(test.table.len(), 2);
            assert_eq!(test.total, 20);
            assert_eq!(test.table[1].count, 10);
            assert_eq!(test.table[1].species, 1);
            assert_eq!(test.table[1].species_cohort, Some(0));
            assert_eq!(test.table[1].species_cohort, Some(0));
            assert_eq!(test.table[1].culture, Some(0));
            assert_eq!(test.table[1].culture_class, Some(0));
            assert_eq!(test.table[1].culture_generation, Some(0));
            assert_eq!(test.table[1].ideology, Some(0));
            assert_eq!(test.table[1].ideology_faction, Some(0));
            assert_eq!(test.table[1].ideology_wave, Some(0));

            let second_row_again = PBRow::new(1,
                Some(0),Some(0),Some(0),
                Some(0),Some(0),
                Some(0),Some(0),Some(0),
                10);

            test.insert_pops(second_row_again);

            assert_eq!(test.table.len(), 2);
            assert_eq!(test.total, 30);
            assert_eq!(test.table[1].count, 20);
            assert_eq!(test.table[1].species, 1);
            assert_eq!(test.table[1].species_cohort, Some(0));
            assert_eq!(test.table[1].species_cohort, Some(0));
            assert_eq!(test.table[1].culture, Some(0));
            assert_eq!(test.table[1].culture_class, Some(0));
            assert_eq!(test.table[1].culture_generation, Some(0));
            assert_eq!(test.table[1].ideology, Some(0));
            assert_eq!(test.table[1].ideology_faction, Some(0));
            assert_eq!(test.table[1].ideology_wave, Some(0));
        }
    }

    mod property_tests {
        use std::collections::{HashSet, HashMap, VecDeque};

        use crate::{objects::{property::{Property, DesireCoord}, desire::{Desire, DesireItem}, want::Want, product::Product, process::{Process, ProcessPart, PartItem, ProcessSectionTag}, market::{MarketHistory, ProductInfo}, pop::Pop, pop_breakdown_table::{PBRow, PopBreakdownTable}, ideology::Ideology, culture::Culture, species::Species}, data_manager::DataManager, demographics::Demographics};

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
                skill: 0, 
                lower_skill_level: 0.0, 
                higher_skill_level: 0.0, 
                property: Property::new(vec![]), 
                breakdown_table: PopBreakdownTable{ table: vec![], total: 0 }, 
                is_selling: true,
                backlog: VecDeque::new()};

            let species_desire_1 = Desire{ 
                item: DesireItem::Want(2), // food
                start: 0, 
                end: Some(4), 
                amount: 1.0, 
                satisfaction: 0.0, 
                step: 1, 
                tags: vec![] };
            let species_desire_2 = Desire{ 
                item: DesireItem::Want(3), // shelter
                start: 7, 
                end: Some(13), 
                amount: 1.0, 
                satisfaction: 0.0, 
                step: 2, 
                tags: vec![] };
            let species_desire_3 = Desire{ 
                item: DesireItem::Want(4), //clothing
                start: 2, 
                end: Some(8), 
                amount: 1.0, 
                satisfaction: 0.0, 
                step: 2, 
                tags: vec![] };

            let culture_desire_1 = Desire{ 
                item: DesireItem::Product(2), // ambrosia fruit
                start: 10, 
                end: Some(30), 
                amount: 1.0, 
                satisfaction: 0.0, 
                step: 5, 
                tags: vec![] };
            let culture_desire_2 = Desire{ 
                item: DesireItem::Product(6), // clothes
                start: 15, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0, 
                step: 10, 
                tags: vec![] };

            let ideology_desire_1 = Desire{ 
                item: DesireItem::Product(14), // Hut
                start: 30, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0, 
                step: 0, 
                tags: vec![] };
            let ideology_desire_2 = Desire{ 
                item: DesireItem::Product(15), // Cabin
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
            data.load_all(&String::from("")).expect("Error on load?");
            let product = data.products.get_mut(&6).unwrap();
            product.fractional = true;

            let mut market = MarketHistory {
                info: HashMap::new(),
                sale_priority: vec![],
                currencies: vec![],
            };
            // quickly set all prices to 1.0 for ease going forward.
            for idx in 0..26 {
                market.info.insert(idx, ProductInfo {
                    available: 0.0,
                    price: 1.0,
                    offered: 0.0,
                    sold: 0.0,
                    salability: 0.5,
                    is_currency: false,
                });
            }
            // ambrosia fruit
            market.info.get_mut(&2).expect("Brok").salability = 1.0;
            market.info.get_mut(&2).expect("Brok").is_currency = true;

            market.info.get_mut(&6).expect("Brok").price = 10.0;
            market.info.get_mut(&7).expect("Brok").price = 20.0;

            market.info.get_mut(&14).expect("Brok").price = 100.0;
            market.info.get_mut(&15).expect("Brok").price = 1000.0;

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
                        item: PartItem::Specific(0), 
                        amount: 1.0, 
                        part_tags: vec![],
                        part: ProcessSectionTag::Input
                    },
                    ProcessPart{ 
                        item: PartItem::Want(1), 
                        amount: 1.0, 
                        part_tags: vec![],
                        part: ProcessSectionTag::Output
                    }
                ],
                process_tags: vec![],
                skill: None,
                skill_minimum: 0.0,
                skill_maximum: 0.0,
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
                        item: PartItem::Class(1), 
                        amount: 1.0, 
                        part_tags: vec![],
                        part: ProcessSectionTag::Input
                    },
                    ProcessPart{ // time
                        item: PartItem::Specific(0), 
                        amount: 0.1, 
                        part_tags: vec![],
                        part: ProcessSectionTag::Input
                    },
                    ProcessPart{ 
                        item: PartItem::Want(0), 
                        amount: 1.0, 
                        part_tags: vec![],
                        part: ProcessSectionTag::Output
                    }
                ],
                process_tags: vec![],
                skill: None,
                skill_minimum: 0.0,
                skill_maximum: 0.0,
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
                        item: PartItem::Specific(0), 
                        amount: 1.0, 
                        part_tags: vec![],
                        part: ProcessSectionTag::Input
                    },
                    ProcessPart{ 
                        item: PartItem::Want(1), 
                        amount: 1.0, 
                        part_tags: vec![],
                        part: ProcessSectionTag::Output
                    }
                ],
                process_tags: vec![],
                skill: None,
                skill_minimum: 0.0,
                skill_maximum: 0.0,
                technology_requirement: None,
                tertiary_tech: None,
            };

            result
        }

        mod record_exchange_should {
            use std::collections::HashMap;

            use crate::objects::property::Property;

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

            use crate::objects::{property::Property, desire::{Desire, DesireItem}, market::{MarketHistory, ProductInfo}};

            // TODO when class and want price estimates are added, add tests for them also
            // TODO when improving the function to predict things more accurately add a unified test to ensure overlap is taken into account.
            #[test]
            pub fn predict_satisfaction_gained_from_products() {
                let mut test_desires = vec![];
                test_desires.push(Desire::new(DesireItem::Product(0), 
                    0, 
                    None, 
                    1.0, 
                    0.0, 
                    1, 
                    vec![]).unwrap());
                test_desires.push(Desire::new(DesireItem::Product(1), 
                    0, 
                    None, 
                    1.0, 
                    0.0, 
                    1, 
                    vec![]).unwrap());
                test_desires.push(Desire::new(DesireItem::Product(2), 
                    0, 
                    None, 
                    1.0, 
                    0.0, 
                    1, 
                    vec![]).unwrap());
                test_desires.push(Desire::new(DesireItem::Product(3), 
                    0, 
                    None, 
                    1.0, 
                    0.0, 
                    1, 
                    vec![]).unwrap());
                test_desires.push(Desire::new(DesireItem::Product(4), 
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
                let market = MarketHistory { info, 
                    sale_priority: vec![], 
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

            use crate::{objects::{property::Property, product::Product, want::Want, process::{ProcessTag, Process, ProcessPart, PartItem, ProcessSectionTag}, property_info::PropertyInfo}, data_manager::DataManager};

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
                        ProcessPart{ item: PartItem::Specific(2), 
                            amount: 1.0, part_tags: vec![], 
                            part: ProcessSectionTag::Input },
                        ProcessPart{ item: PartItem::Want(0), 
                            amount: 1.0, part_tags: vec![], 
                            part: ProcessSectionTag::Output },
                        ProcessPart{ item: PartItem::Specific(3), 
                            amount: 1.0, part_tags: vec![], 
                            part: ProcessSectionTag::Output }
                    ],
                    process_tags: vec![
                        ProcessTag::Failure(1)
                    ],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
                    technology_requirement: None,
                    tertiary_tech: None,
                });
                test.property.insert(0, PropertyInfo::new(10.0));
                test.property.insert(1, PropertyInfo::new(10.0));
                test.property.insert(2, PropertyInfo::new(10.0));
                test.property.insert(3, PropertyInfo::new(10.0));
                test.want_store.insert(0, 10.0);
                test.want_store.insert(1, 10.0);
                test.want_store.insert(2, 10.0);
                test.want_store.insert(3, 10.0);
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
                assert_eq!(test.want_store[&0], 20.0);
                assert_eq!(test.want_store[&1], 5.0);
                assert_eq!(test.want_store[&2], 7.5);
                assert_eq!(test.want_store[&3], 0.0);
            }
        }

        mod first_desire_should {
            use crate::objects::{desire::{Desire, DesireItem}, property::Property};
            
            #[test]
            pub fn find_the_lowest_and_first_desire() {
                let mut test_desires = vec![];
                test_desires.push(Desire{ // 0,2,...
                    item: DesireItem::Want(0), 
                    start: 4, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 1,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2,...
                    item: DesireItem::Want(1), 
                    start: 7, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 1,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2,...
                    item: DesireItem::Want(1), 
                    start: 2, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 1,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2,...
                    item: DesireItem::Want(1), 
                    start: 1, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 1,
                    tags: vec![]});
                let mut test = Property::new(test_desires);
                let result = test.first_desire();
                assert_eq!(result.idx, 3);
                assert_eq!(result.tier, 1);
            }
        }

        mod get_first_unsatisfied_desire_should {
            use crate::objects::{desire::{Desire, DesireItem}, property::Property};
            
            #[test]
            pub fn find_the_lowest_and_first_unsatisfied_desire() {
                let mut test_desires = vec![];
                test_desires.push(Desire{ // 0,2,...
                    item: DesireItem::Want(0), 
                    start: 4, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 1.0,
                    step: 0,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2,...
                    item: DesireItem::Want(1), 
                    start: 7, 
                    end: Some(13), 
                    amount: 1.0, 
                    satisfaction: 3.5,
                    step: 1,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2,...
                    item: DesireItem::Want(1), 
                    start: 2, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 10.0,
                    step: 1,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2,...
                    item: DesireItem::Want(1), 
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
                    item: DesireItem::Want(0), 
                    start: 4, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 1.0,
                    step: 0,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2,...
                    item: DesireItem::Want(1), 
                    start: 7, 
                    end: Some(13), 
                    amount: 1.0, 
                    satisfaction: 1.0,
                    step: 0,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2,...
                    item: DesireItem::Want(1), 
                    start: 2, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 1.0,
                    step: 0,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2,...
                    item: DesireItem::Want(1), 
                    start: 1, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 1.0,
                    step: 0,
                    tags: vec![]});
                let mut test = Property::new(test_desires);
                let result = test.get_first_unsatisfied_desire();
                assert!(result.is_none());
            }
        }

        mod add_products_should {
            use std::collections::{HashSet, HashMap};

            use crate::{objects::{process::{Process, ProcessTag, ProcessPart, PartItem, ProcessSectionTag}, want::Want, product::Product, property::Property, desire::{DesireItem, Desire}}, data_manager::DataManager};


            #[test]
            pub fn add_multiple_products_to_property_then_sift() {
                let mut test_desires = vec![];
                test_desires.push(Desire{ // 0,2,...
                    item: DesireItem::Want(0), 
                    start: 0, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 1,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2,...
                    item: DesireItem::Want(1), 
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
                // 0 for specific
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
                        ProcessPart{ item: PartItem::Specific(3), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Capital },
                        ProcessPart{ item: PartItem::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                    ],
                    process_tags: vec![
                        ProcessTag::Use(3)
                    ],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
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
                        ProcessPart{ item: PartItem::Specific(4), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Input },
                        ProcessPart{ item: PartItem::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                    ],
                    process_tags: vec![
                        ProcessTag::Consumption(4)
                    ],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
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

            use crate::{objects::{desire::{Desire, DesireItem}, property::Property, product::Product, want::Want, process::{Process, ProcessPart, ProcessSectionTag, PartItem, ProcessTag}, seller::Seller}, data_manager::DataManager};

            use super::{make_test_pop, prepare_data_for_market_actions};

            #[test]
            pub fn correctly_sift_test_data() {
                let mut test = make_test_pop();
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
                assert_eq!(test.property.property[&2].specific_reserve, 50.0);
                assert_eq!(test.property.property[&2].class_reserve, 0.0);
                assert_eq!(test.property.property[&2].want_reserve, 100.0);

                assert_eq!(test.property.property[&14].total_property, 80.0);
                assert_eq!(test.property.property[&14].unreserved, 0.0);
                assert_eq!(test.property.property[&14].specific_reserve, 10.0);
                assert_eq!(test.property.property[&14].class_reserve, 0.0);
                assert_eq!(test.property.property[&14].want_reserve, 80.0);

                assert_eq!(test.property.property[&6].total_property, 100.0);
                assert_eq!(test.property.property[&6].unreserved, 0.0);
                assert_eq!(test.property.property[&6].specific_reserve, 100.0);
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
                    item: DesireItem::Want(0), 
                    start: 0, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 1,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2,...
                    item: DesireItem::Want(1), 
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
                // 0 for specific
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
                        ProcessPart{ item: PartItem::Specific(3), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Capital },
                        ProcessPart{ item: PartItem::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                    ],
                    process_tags: vec![
                        ProcessTag::Use(3)
                    ],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
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
                        ProcessPart{ item: PartItem::Specific(4), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Input },
                        ProcessPart{ item: PartItem::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                    ],
                    process_tags: vec![
                        ProcessTag::Consumption(4)
                    ],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
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
                assert!(test.desires[0].satisfaction == 1.0);
                assert!(test.desires[1].satisfaction == 2.0);
                assert!(test.property.get(&2).unwrap().total_property == 1.0);
                assert!(test.property.get(&2).unwrap().want_reserve == 1.0);
            }

            #[test]
            pub fn add_and_sift_correctly_without_exchanges() {
                let mut test_desires = vec![];
                test_desires.push(Desire{ // 0,2, ...
                    item: DesireItem::Product(0), 
                    start: 0, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 1,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2,...
                    item: DesireItem::Class(1), 
                    start: 0, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 1,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2,...
                    item: DesireItem::Want(0), 
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
                // 0 for specific
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
                        ProcessPart{ item: PartItem::Specific(3), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Capital },
                        ProcessPart{ item: PartItem::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                    ],
                    process_tags: vec![
                        ProcessTag::Use(3)
                    ],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
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
                        ProcessPart{ item: PartItem::Specific(4), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Input },
                        ProcessPart{ item: PartItem::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                    ],
                    process_tags: vec![
                        ProcessTag::Consumption(4)
                    ],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
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
                assert!(test.property.get(&0).unwrap().specific_reserve == 5.0);
                assert!(test.property.get(&1).is_none());
                assert!(test.property.get(&2).is_none());
                assert!(test.property.get(&3).is_none());
                assert!(test.property.get(&4).is_none());
                test.add_property(1, 5.0, &data);
                assert!(test.desires[0].satisfaction == 5.0);
                assert!(test.desires[1].satisfaction == 5.0);
                assert!(test.desires[2].satisfaction == 0.0);
                assert!(test.property.get(&0).unwrap().total_property == 5.0);
                assert!(test.property.get(&0).unwrap().specific_reserve == 5.0);
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
                assert!(test.property.get(&0).unwrap().specific_reserve == 5.0);
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
                assert!(test.property.get(&0).unwrap().specific_reserve == 5.0);
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
                assert!(test.property.get(&0).unwrap().specific_reserve == 5.0);
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
                assert!(test.property.get(&0).unwrap().specific_reserve == 5.0);
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
                    item: DesireItem::Product(0), 
                    start: 0, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 1,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2,...
                    item: DesireItem::Class(1), 
                    start: 0, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 1,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2,...
                    item: DesireItem::Want(0), 
                    start: 0, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 1,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2,...
                    item: DesireItem::Product(2), 
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
                // 0 for specific and class
                // 1 for class and ownership
                // 2 for specific and ownership
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
                        ProcessPart{ item: PartItem::Specific(3), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Capital },
                        ProcessPart{ item: PartItem::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                    ],
                    process_tags: vec![
                        ProcessTag::Use(3)
                    ],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
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
                        ProcessPart{ item: PartItem::Specific(4), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Input },
                        ProcessPart{ item: PartItem::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                    ],
                    process_tags: vec![
                        ProcessTag::Consumption(4)
                    ],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
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
                assert!(test.property.get(&0).unwrap().specific_reserve == 1.0);
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
                assert!(test.property.get(&0).unwrap().specific_reserve == 1.0);
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
                assert!(test.property.get(&0).unwrap().specific_reserve == 1.0);
                assert!(test.property.get(&0).unwrap().class_reserve == 1.0);
                assert!(test.property.get(&1).unwrap().total_property == 1.0);
                assert!(test.property.get(&1).unwrap().class_reserve == 1.0);
                assert!(test.property.get(&1).unwrap().want_reserve == 1.0);
                assert!(test.property.get(&2).unwrap().total_property == 1.0);
                assert!(test.property.get(&2).unwrap().specific_reserve == 1.0);
                assert!(test.property.get(&2).unwrap().want_reserve == 1.0);
                assert!(test.property.get(&3).is_none());
                assert!(test.property.get(&4).is_none());
                test.add_property(3, 1.0, &data);
                assert!(test.desires[0].satisfaction == 1.0);
                assert!(test.desires[1].satisfaction == 2.0);
                assert!(test.desires[2].satisfaction == 3.0);
                assert!(test.desires[3].satisfaction == 1.0);
                assert!(test.property.get(&0).unwrap().total_property == 1.0);
                assert!(test.property.get(&0).unwrap().specific_reserve == 1.0);
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
            use crate::objects::{desire::{Desire, DesireItem}, property::Property};

            #[test]
            pub fn return_tiered_value_correctly() {
                let mut test_desires = vec![];
                test_desires.push(Desire{ // 0,1, ...
                    item: DesireItem::Product(0), 
                    start: 0, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 1,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,1,...
                    item: DesireItem::Class(1), 
                    start: 0, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 1,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,1,...
                    item: DesireItem::Want(0), 
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

            use crate::{objects::{desire::{Desire, DesireItem}, property::Property, product::Product, want::Want, process::{Process, ProcessPart, ProcessSectionTag, PartItem, ProcessTag}}, data_manager::DataManager};

            #[test]
            pub fn sift_products_down_from_higher_wants() {
                // Currently Not Implemented, Function is Placeholder
                assert!(true);
            }

            #[test]
            pub fn return_value_gained_correctly() {
                let mut test_desires = vec![];
                test_desires.push(Desire{ // 0,2,...
                    item: DesireItem::Want(0), 
                    start: 0, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 1,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2,...
                    item: DesireItem::Want(1), 
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
                // 0 for specific
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
                        ProcessPart{ item: PartItem::Specific(3), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Capital },
                        ProcessPart{ item: PartItem::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                    ],
                    process_tags: vec![
                        ProcessTag::Use(3)
                    ],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
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
                        ProcessPart{ item: PartItem::Specific(4), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Input },
                        ProcessPart{ item: PartItem::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                    ],
                    process_tags: vec![
                        ProcessTag::Consumption(4)
                    ],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
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
                    item: DesireItem::Product(0), 
                    start: 0, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 1,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2,...
                    item: DesireItem::Class(1), 
                    start: 0, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 1,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2,...
                    item: DesireItem::Want(0), 
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
                // 0 for specific
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
                        ProcessPart{ item: PartItem::Specific(3), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Capital },
                        ProcessPart{ item: PartItem::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                    ],
                    process_tags: vec![
                        ProcessTag::Use(3)
                    ],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
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
                        ProcessPart{ item: PartItem::Specific(4), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Input },
                        ProcessPart{ item: PartItem::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                    ],
                    process_tags: vec![
                        ProcessTag::Consumption(4)
                    ],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
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
                    item: DesireItem::Product(0), 
                    start: 0, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 1,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2,...
                    item: DesireItem::Class(1), 
                    start: 0, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 1,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2,...
                    item: DesireItem::Want(0), 
                    start: 0, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 1,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2,...
                    item: DesireItem::Product(2), 
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
                // 0 for specific and class
                // 1 for class and ownership
                // 2 for specific and ownership
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
                        ProcessPart{ item: PartItem::Specific(3), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Capital },
                        ProcessPart{ item: PartItem::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                    ],
                    process_tags: vec![
                        ProcessTag::Use(3)
                    ],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
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
                        ProcessPart{ item: PartItem::Specific(4), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Input },
                        ProcessPart{ item: PartItem::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                    ],
                    process_tags: vec![
                        ProcessTag::Consumption(4)
                    ],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
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

            use crate::{objects::{process::{ProcessTag, PartItem, ProcessPart, ProcessSectionTag, Process}, want::Want, product::Product, property::{Property, TieredValue}, desire::{DesireItem, Desire}}, data_manager::DataManager};

            #[test]
            pub fn return_changed_value_correctly() {
                let mut test_desires = vec![];
                test_desires.push(Desire{ // 0,2, ...
                    item: DesireItem::Product(0), 
                    start: 0, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 1,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2,...
                    item: DesireItem::Class(1), 
                    start: 0, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 1,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2,...
                    item: DesireItem::Want(0), 
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
                // 0 for specific
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
                        ProcessPart{ item: PartItem::Specific(3), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Capital },
                        ProcessPart{ item: PartItem::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                    ],
                    process_tags: vec![
                        ProcessTag::Use(3)
                    ],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
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
                        ProcessPart{ item: PartItem::Specific(4), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Input },
                        ProcessPart{ item: PartItem::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                    ],
                    process_tags: vec![
                        ProcessTag::Consumption(4)
                    ],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
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

            use crate::{objects::{process::{ProcessTag, PartItem, ProcessPart, ProcessSectionTag, Process}, want::Want, product::Product, property::Property, desire::{DesireItem, Desire}}, data_manager::DataManager};

            // TODO add additional stress tests.

            #[test]
            pub fn return_value_lost_correctly() {
                let mut test_desires = vec![];
                test_desires.push(Desire{ // 0,2, ...
                    item: DesireItem::Product(0), 
                    start: 0, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 1,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2,...
                    item: DesireItem::Class(1), 
                    start: 0, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 1,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2,...
                    item: DesireItem::Want(0), 
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
                // 0 for specific
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
                        ProcessPart{ item: PartItem::Specific(3), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Capital },
                        ProcessPart{ item: PartItem::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                    ],
                    process_tags: vec![
                        ProcessTag::Use(3)
                    ],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
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
                        ProcessPart{ item: PartItem::Specific(4), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Input },
                        ProcessPart{ item: PartItem::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                    ],
                    process_tags: vec![
                        ProcessTag::Consumption(4)
                    ],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
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
                item: DesireItem::Product(0), 
                start: 0, 
                end: Some(2), 
                amount: 1.0, 
                satisfaction: 2.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,4,6,8,10
                item: DesireItem::Product(1), 
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
            assert!(val.reserved == 0.0);
            assert!(val.specific_reserve == 0.0);
            assert!(val.class_reserve == 0.0);
            assert!(val.want_reserve == 0.0);
        }

        mod market_wealth_should {
            use std::collections::HashMap;

            use crate::objects::{desire::{Desire, DesireItem}, property::Property, market::{MarketHistory, ProductInfo}};

            #[test]
            pub fn return_the_total_amv_of_property() {
                let mut test_desires = vec![];
                test_desires.push(Desire{ // 0,2
                    item: DesireItem::Product(0), 
                    start: 0, 
                    end: Some(2), 
                    amount: 1.0, 
                    satisfaction: 2.0,
                    step: 2,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2,4,6,8,10
                    item: DesireItem::Product(1), 
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
                    info: product_info,
                    sale_priority: vec![],
                    currencies: vec![],
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

            use crate::objects::{desire::{Desire, DesireItem}, property::Property, market::{MarketHistory, ProductInfo}};

            #[test]
            pub fn return_correct_market_satisfaction() {
                let mut test_desires = vec![];
                test_desires.push(Desire{ // 0,2
                    item: DesireItem::Product(0), 
                    start: 0, 
                    end: Some(2), 
                    amount: 1.0, 
                    satisfaction: 2.0,
                    step: 2,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2,4,6,8,10
                    item: DesireItem::Product(1), 
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
                    info: product_info,
                    sale_priority: vec![],
                    currencies: vec![],
                };
                let result = test.market_satisfaction(&test_market);
                assert!(result == 17.0);
            }
        }

        mod update_satisfactions_should {
            use crate::objects::{desire::{Desire, DesireItem}, property::Property};

            #[test]
            pub fn correctly_update_satisfaction() {
                let mut test_desires = vec![];
                test_desires.push(Desire{ // 0,2
                    item: DesireItem::Product(0), 
                    start: 0, 
                    end: Some(2), 
                    amount: 1.0, 
                    satisfaction: 2.0,
                    step: 2,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2,4,6,8,10
                    item: DesireItem::Product(1), 
                    start: 0, 
                    end: Some(10), 
                    amount: 1.0, 
                    satisfaction: 6.0,
                    step: 2,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2,4,6,8,10
                    item: DesireItem::Product(2), 
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
            pub fn set_full_tier_and_hard_sat_to_none_when_at_least_one_desire_unsatisfied() {
                let mut test_desires = vec![];
                test_desires.push(Desire{ // 0,2
                    item: DesireItem::Product(0), 
                    start: 0, 
                    end: Some(2), 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 2,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2,4,6,8,10
                    item: DesireItem::Product(1), 
                    start: 0, 
                    end: Some(10), 
                    amount: 1.0, 
                    satisfaction: 6.0,
                    step: 2,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2,4,6,8,10
                    item: DesireItem::Product(2), 
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

            use crate::{objects::{property::Property, desire::{Desire, DesireItem}, property_info::PropertyInfo, product::Product, want::Want, process::{Process, ProcessPart, PartItem, ProcessTag, ProcessSectionTag}}, data_manager::DataManager};

            #[test]
            pub fn correctly_remove_item_from_property_and_satisfaction() {
                // make some default data for tests
                let mut data = DataManager::new();
                // 5 products, 
                // 0 for specific
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
                        ProcessPart{ item: PartItem::Specific(3), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Capital },
                        ProcessPart{ item: PartItem::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                    ],
                    process_tags: vec![
                        ProcessTag::Use(3)
                    ],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
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
                        ProcessPart{ item: PartItem::Specific(4), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Input },
                        ProcessPart{ item: PartItem::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                    ],
                    process_tags: vec![
                        ProcessTag::Consumption(4)
                    ],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
                    technology_requirement: None,
                    tertiary_tech: None,
                });
                data.update_product_classes().expect("Failed to setup product classes.");
                // setup desires
                let mut test_desires = vec![];
                test_desires.push(Desire{ // 0,2
                    item: DesireItem::Product(0), 
                    start: 0, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 2,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2,4,6,8,10
                    item: DesireItem::Product(1), 
                    start: 0, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 2,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2
                    item: DesireItem::Product(2), 
                    start: 0, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 2,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2
                    item: DesireItem::Product(3), 
                    start: 0, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 2,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2
                    item: DesireItem::Product(4), 
                    start: 0, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 2,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2
                    item: DesireItem::Class(1), 
                    start: 0, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 2,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2
                    item: DesireItem::Product(4), 
                    start: 0, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 2,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2
                    item: DesireItem::Want(0), 
                    start: 0, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 2,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2
                    item: DesireItem::Want(1), 
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

            use crate::{data_manager::DataManager, objects::{product::Product, want::Want, process::{Process, ProcessPart, PartItem, ProcessSectionTag, ProcessTag}, desire::{Desire, DesireItem}, property::Property, property_info::PropertyInfo}};

            #[test]
            pub fn print_correctly() {
                // make some default data for tests
                let mut data = DataManager::new();
                // 5 products, 
                // 0 for specific
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
                        ProcessPart{ item: PartItem::Specific(3), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Capital },
                        ProcessPart{ item: PartItem::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                    ],
                    process_tags: vec![
                        ProcessTag::Use(3)
                    ],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
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
                        ProcessPart{ item: PartItem::Specific(4), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Input },
                        ProcessPart{ item: PartItem::Want(0), amount: 1.0, part_tags: vec![], part: ProcessSectionTag::Output }
                    ],
                    process_tags: vec![
                        ProcessTag::Consumption(4)
                    ],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
                    technology_requirement: None,
                    tertiary_tech: None,
                });
                data.update_product_classes().expect("Failed to setup product classes.");
                // setup desires
                let mut test_desires = vec![];
                test_desires.push(Desire{ // 0,2
                    item: DesireItem::Product(0), 
                    start: 0, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 2,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2,4,6,8,10
                    item: DesireItem::Product(1), 
                    start: 0, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 2,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2
                    item: DesireItem::Product(2), 
                    start: 0, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 2,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2
                    item: DesireItem::Product(3), 
                    start: 0, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 2,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2
                    item: DesireItem::Product(4), 
                    start: 0, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 2,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2
                    item: DesireItem::Class(1), 
                    start: 0, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 2,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2
                    item: DesireItem::Product(4), 
                    start: 0, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 2,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2
                    item: DesireItem::Want(0), 
                    start: 0, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 2,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2
                    item: DesireItem::Want(1), 
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
                item: DesireItem::Product(0), 
                start: 0, 
                end: Some(2), 
                amount: 1.0, 
                satisfaction: 1.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,4,6,8,10
                item: DesireItem::Product(1), 
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
                item: DesireItem::Product(0), 
                start: 0, 
                end: Some(2), 
                amount: 1.0, 
                satisfaction: 1.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,4,6,8,10
                item: DesireItem::Product(1), 
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

        #[test]
        pub fn satisfied_at_tier_correctly() {
            let mut test_desires = vec![];
            test_desires.push(Desire{ // 0,2
                item: DesireItem::Product(0), 
                start: 0, 
                end: Some(2), 
                amount: 1.0, 
                satisfaction: 1.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,4,6,8,10
                item: DesireItem::Product(1), 
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

        mod sift_up_to_should {
            // TODO update test for lookahead when added.
            use std::collections::{HashMap, HashSet, VecDeque};

            use crate::{objects::{property::{Property, DesireCoord}, desire::{Desire, DesireItem}, property_info::PropertyInfo, product::Product, process::{Process, ProcessPart, PartItem, ProcessSectionTag, ProcessTag}, want::Want, market::{ProductInfo, MarketHistory}, pop::Pop, pop_breakdown_table::{PBRow, PopBreakdownTable}, ideology::Ideology, culture::Culture, species::Species}, data_manager::DataManager, demographics::Demographics};

            #[test]
            pub fn shift_want_class_and_specific_desires_correctly() {
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
                            item: PartItem::Specific(1), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input
                        },
                        ProcessPart { 
                            item: PartItem::Specific(2), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Capital
                        },
                        ProcessPart { 
                            item: PartItem::Want(0), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output
                        }
                    ],
                    process_tags: vec![
                        ProcessTag::Use(2)
                    ],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
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
                            item: PartItem::Specific(2), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input
                        },
                        ProcessPart { 
                            item: PartItem::Specific(3), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input
                        },
                        ProcessPart { 
                            item: PartItem::Want(0), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output
                        }
                    ],
                    process_tags: vec![
                        ProcessTag::Consumption(2)
                    ],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
                    technology_requirement: None,
                    tertiary_tech: None,
                });

                data.update_product_classes().expect("Could not function");
                let test_desires = vec![
                    Desire::new(DesireItem::Want(0),
                        1,
                        None,
                        1.0,
                        0.0,
                        1,
                        vec![]).unwrap(),
                    Desire::new(DesireItem::Class(0),
                        3,
                        Some(30),
                        1.0,
                        0.0,
                        3,
                        vec![]).unwrap(),
                    Desire::new(DesireItem::Product(0),
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
                assert!(prop0.reserved == 0.0);
                assert!(prop0.want_reserve == 15.0);
                // assert!(prop0.class_reserve == 0.0); both are in the same class, so either is valid, selection order cannot be guaranteed (yet).
                assert!(prop0.specific_reserve == 2.0);
                let prop1 = test.property.get(&1).unwrap();
                assert!(prop1.total_property == 10.0);
                assert!(prop1.unreserved == 0.0);
                assert!(prop1.reserved == 0.0);
                assert!(prop1.want_reserve == 10.0);
                assert!((prop1.class_reserve + prop0.class_reserve) == 8.0);
                assert!(prop1.specific_reserve == 0.0);
                let prop2 = test.property.get(&2).unwrap();
                assert!(prop2.total_property == 20.0);
                assert!(prop2.unreserved == 10.0);
                assert!(prop2.reserved == 0.0);
                assert!(prop2.want_reserve == 10.0);
                assert!(prop2.class_reserve == 0.0);
                assert!(prop2.specific_reserve == 0.0);
                let prop3 = test.property.get(&3).unwrap();
                assert!(prop3.total_property == 15.0);
                assert!(prop3.unreserved == 15.0);
                assert!(prop3.reserved == 0.0);
                assert!(prop3.want_reserve == 0.0);
                assert!(prop3.class_reserve == 0.0);
                assert!(prop3.specific_reserve == 0.0);
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
                            item: PartItem::Specific(1), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input
                        },
                        ProcessPart { 
                            item: PartItem::Specific(2), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Capital
                        },
                        ProcessPart { 
                            item: PartItem::Want(0), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output
                        }
                    ],
                    process_tags: vec![
                        ProcessTag::Use(2)
                    ],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
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
                            item: PartItem::Specific(2), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input
                        },
                        ProcessPart { 
                            item: PartItem::Specific(3), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input
                        },
                        ProcessPart { 
                            item: PartItem::Want(0), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output
                        }
                    ],
                    process_tags: vec![
                        ProcessTag::Consumption(2)
                    ],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
                    technology_requirement: None,
                    tertiary_tech: None,
                });

                data.update_product_classes().expect("Could not function");
                let test_desires = vec![
                    Desire::new(DesireItem::Want(0),
                        1,
                        None,
                        1.0,
                        0.0,
                        1,
                        vec![]).unwrap(),
                    Desire::new(DesireItem::Class(0),
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
                assert!(prop0.reserved == 0.0);
                assert!(prop0.want_reserve == 15.0);
                // assert!(prop0.class_reserve == 0.0); both are in the same class, so either is valid, selection order cannot be guaranteed (yet).
                assert!(prop0.specific_reserve == 0.0);
                let prop1 = test.property.get(&1).unwrap();
                assert!(prop1.total_property == 10.0);
                assert!(prop1.unreserved == 0.0);
                assert!(prop1.reserved == 0.0);
                assert!(prop1.want_reserve == 10.0);
                assert!((prop1.class_reserve + prop0.class_reserve) == 8.0);
                assert!(prop1.specific_reserve == 0.0);
                let prop2 = test.property.get(&2).unwrap();
                assert!(prop2.total_property == 20.0);
                assert!(prop2.unreserved == 10.0);
                assert!(prop2.reserved == 0.0);
                assert!(prop2.want_reserve == 10.0);
                assert!(prop2.class_reserve == 0.0);
                assert!(prop2.specific_reserve == 0.0);
                let prop3 = test.property.get(&3).unwrap();
                assert!(prop3.total_property == 15.0);
                assert!(prop3.unreserved == 15.0);
                assert!(prop3.reserved == 0.0);
                assert!(prop3.want_reserve == 0.0);
                assert!(prop3.class_reserve == 0.0);
                assert!(prop3.specific_reserve == 0.0);
                assert_eq!(result.tier, 24);
                assert!(135.0 < result.value);
                assert!(result.value < 136.0);
            }

            #[test]
            pub fn shift_want_and_specific_desires_correctly() {
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
                            item: PartItem::Specific(1), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input
                        },
                        ProcessPart { 
                            item: PartItem::Specific(2), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Capital
                        },
                        ProcessPart { 
                            item: PartItem::Want(0), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output
                        }
                    ],
                    process_tags: vec![
                        ProcessTag::Use(2)
                    ],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
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
                            item: PartItem::Specific(2), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input
                        },
                        ProcessPart { 
                            item: PartItem::Specific(3), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input
                        },
                        ProcessPart { 
                            item: PartItem::Want(0), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output
                        }
                    ],
                    process_tags: vec![
                        ProcessTag::Consumption(2)
                    ],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
                    technology_requirement: None,
                    tertiary_tech: None,
                });

                data.update_product_classes().expect("Could not function");
                let test_desires = vec![
                    Desire::new(DesireItem::Want(0),
                        1,
                        None,
                        1.0,
                        0.0,
                        1,
                        vec![]).unwrap(),
                    Desire::new(DesireItem::Product(2),
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
                assert!(prop0.reserved == 0.0);
                assert!(prop0.want_reserve == 15.0);
                assert!(prop0.class_reserve == 0.0);
                assert!(prop0.specific_reserve == 0.0);
                let prop1 = test.property.get(&1).unwrap();
                assert!(prop1.total_property == 10.0);
                assert!(prop1.unreserved == 0.0);
                assert!(prop1.reserved == 0.0);
                assert!(prop1.want_reserve == 10.0);
                assert!(prop1.class_reserve == 0.0);
                assert!(prop1.specific_reserve == 0.0);
                let prop2 = test.property.get(&2).unwrap();
                assert!(prop2.total_property == 20.0);
                assert!(prop2.unreserved == 10.0);
                assert!(prop2.reserved == 0.0);
                assert!(prop2.want_reserve == 10.0);
                assert!(prop2.class_reserve == 0.0);
                assert!(prop2.specific_reserve == 5.0);
                let prop3 = test.property.get(&3).unwrap();
                assert!(prop3.total_property == 15.0);
                assert!(prop3.unreserved == 15.0);
                assert!(prop3.reserved == 0.0);
                assert!(prop3.want_reserve == 0.0);
                assert!(prop3.class_reserve == 0.0);
                assert!(prop3.specific_reserve == 0.0);
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
                            item: PartItem::Specific(1), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input
                        },
                        ProcessPart { 
                            item: PartItem::Specific(2), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Capital
                        },
                        ProcessPart { 
                            item: PartItem::Want(0), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output
                        }
                    ],
                    process_tags: vec![
                        ProcessTag::Use(2)
                    ],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
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
                            item: PartItem::Specific(2), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input
                        },
                        ProcessPart { 
                            item: PartItem::Specific(3), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input
                        },
                        ProcessPart { 
                            item: PartItem::Want(0), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output
                        }
                    ],
                    process_tags: vec![
                        ProcessTag::Consumption(2)
                    ],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
                    technology_requirement: None,
                    tertiary_tech: None,
                });

                data.update_product_classes().expect("Could not function");
                let test_desires = vec![
                    Desire::new(DesireItem::Want(0),
                        1,
                        None,
                        1.0,
                        0.0,
                        1,
                        vec![]).unwrap(),
                    Desire::new(DesireItem::Want(0),
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
                assert!(prop0.reserved == 0.0);
                assert!(prop0.want_reserve == 15.0);
                assert!(prop0.class_reserve == 0.0);
                assert!(prop0.specific_reserve == 0.0);
                let prop1 = test.property.get(&1).unwrap();
                assert!(prop1.total_property == 10.0);
                assert!(prop1.unreserved == 0.0);
                assert!(prop1.reserved == 0.0);
                assert!(prop1.want_reserve == 10.0);
                assert!(prop1.class_reserve == 0.0);
                assert!(prop1.specific_reserve == 0.0);
                let prop2 = test.property.get(&2).unwrap();
                assert!(prop2.total_property == 20.0);
                assert!(prop2.unreserved == 6.0);
                assert!(prop2.reserved == 0.0);
                assert!(prop2.want_reserve == 14.0);
                assert!(prop2.class_reserve == 0.0);
                assert!(prop2.specific_reserve == 0.0);
                let prop3 = test.property.get(&3).unwrap();
                assert!(prop3.total_property == 15.0);
                assert!(prop3.unreserved == 11.0);
                assert!(prop3.reserved == 0.0);
                assert!(prop3.want_reserve == 4.0);
                assert!(prop3.class_reserve == 0.0);
                assert!(prop3.specific_reserve == 0.0);
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
                    Desire::new(DesireItem::Class(0),
                        0,
                        None,
                        1.0,
                        0.0,
                        4,
                        vec![]).unwrap(),
                    Desire::new(DesireItem::Class(0),
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
                assert_eq!(prop0.reserved, 0.0);
                assert_eq!(prop0.want_reserve, 0.0);
                assert_eq!(prop0.class_reserve, 11.0);
                assert_eq!(prop0.specific_reserve, 0.0);
                // touched 2nd product
                let prop1 = test.property.get(&1).unwrap();
                assert_eq!(prop1.total_property, 15.0);
                assert_eq!(prop1.unreserved, 15.0);
                assert_eq!(prop1.reserved, 0.0);
                assert_eq!(prop1.want_reserve, 0.0);
                assert_eq!(prop1.class_reserve, 0.0);
                assert_eq!(prop1.specific_reserve, 0.0);
                // untouched 3rd product
                let prop2 = test.property.get(&2).unwrap();
                assert_eq!(prop2.total_property, 10.0);
                assert_eq!(prop2.unreserved, 10.0);
                assert_eq!(prop2.reserved, 0.0);
                assert_eq!(prop2.want_reserve, 0.0);
                assert_eq!(prop2.class_reserve, 0.0);
                assert_eq!(prop2.specific_reserve, 0.0);
                assert_eq!(result.tier, 24);
                assert!(58.0 < result.value);
                assert!(result.value < 59.0);
            }

            /// Sifts 1 specific desire only, don't set up wants
            /// or class desires.
            #[test]
            pub fn sift_specific_desire_correctly() {
                // data needed, but not set up for this test.
                let data = &DataManager::new();
                let test_desires = vec![
                    Desire::new(DesireItem::Product(0),
                        0,
                        Some(100),
                        1.0,
                        0.0,
                        5,
                        vec![]).unwrap(),
                    Desire::new(DesireItem::Product(0),
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
                assert!(prop0.reserved == 0.0);
                assert!(prop0.want_reserve == 0.0);
                assert!(prop0.class_reserve == 0.0);
                assert!(prop0.specific_reserve == 10.0);
                let prop1 = test.property.get(&1).unwrap();
                assert!(prop1.total_property == 10.0);
                assert!(prop1.unreserved == 10.0);
                assert!(prop1.reserved == 0.0);
                assert!(prop1.want_reserve == 0.0);
                assert!(prop1.class_reserve == 0.0);
                assert!(prop1.specific_reserve == 0.0);
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
                            item: PartItem::Specific(1), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input
                        },
                        ProcessPart { 
                            item: PartItem::Specific(2), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Capital
                        },
                        ProcessPart { 
                            item: PartItem::Want(0), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output
                        }
                    ],
                    process_tags: vec![
                        ProcessTag::Use(2)
                    ],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
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
                            item: PartItem::Specific(2), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input
                        },
                        ProcessPart { 
                            item: PartItem::Specific(3), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input
                        },
                        ProcessPart { 
                            item: PartItem::Want(0), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output
                        }
                    ],
                    process_tags: vec![
                        ProcessTag::Consumption(2)
                    ],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
                    technology_requirement: None,
                    tertiary_tech: None,
                });

                data.update_product_classes().expect("Could not function");
                let test_desires = vec![
                    Desire::new(DesireItem::Want(0),
                        1,
                        None,
                        1.0,
                        0.0,
                        1,
                        vec![]).unwrap(),
                    Desire::new(DesireItem::Class(0),
                        3,
                        Some(30),
                        1.0,
                        0.0,
                        3,
                        vec![]).unwrap(),
                    Desire::new(DesireItem::Product(0),
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
                assert!(prop0.reserved == 0.0);
                assert!(prop0.want_reserve == 15.0);
                // assert!(prop0.class_reserve == 0.0); both are in the same class, so either is valid, selection order cannot be guaranteed (yet).
                assert!(prop0.specific_reserve == 2.0);
                let prop1 = test.property.get(&1).unwrap();
                assert!(prop1.total_property == 10.0);
                assert!(prop1.unreserved == 1.0);
                assert!(prop1.reserved == 0.0);
                assert!(prop1.want_reserve == 9.0);
                assert!((prop1.class_reserve + prop0.class_reserve) == 8.0);
                assert!(prop1.specific_reserve == 0.0);
                let prop2 = test.property.get(&2).unwrap();
                assert!(prop2.total_property == 20.0);
                assert!(prop2.unreserved == 11.0);
                assert!(prop2.reserved == 0.0);
                assert!(prop2.want_reserve == 9.0);
                assert!(prop2.class_reserve == 0.0);
                assert!(prop2.specific_reserve == 0.0);
                let prop3 = test.property.get(&3).unwrap();
                assert!(prop3.total_property == 15.0);
                assert!(prop3.unreserved == 15.0);
                assert!(prop3.reserved == 0.0);
                assert!(prop3.want_reserve == 0.0);
                assert!(prop3.class_reserve == 0.0);
                assert!(prop3.specific_reserve == 0.0);
                // ensure the tiered value is correct to match
                assert_eq!(result.tier, 24);
                assert!(146.0 < result.value);
                assert!(result.value < 147.0);
            }
        }

        mod release_desire_at_should {
            use std::collections::{VecDeque, HashMap};

            use crate::{objects::{pop::Pop, property::{Property, DesireCoord}, pop_breakdown_table::{PopBreakdownTable, PBRow}, desire::{Desire, DesireItem}, species::Species, culture::Culture, ideology::Ideology, market::{MarketHistory, ProductInfo}}, demographics::Demographics, data_manager::DataManager};

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
                    skill: 0, 
                    lower_skill_level: 0.0, 
                    higher_skill_level: 0.0, 
                    property: Property::new(vec![]), 
                    breakdown_table: PopBreakdownTable{ table: vec![], total: 0 }, 
                    is_selling: true,
                    backlog: VecDeque::new()};

                let species_desire_1 = Desire { 
                    item: DesireItem::Want(2), // food
                    start: 0, 
                    end: Some(4), 
                    amount: 1.0, 
                    satisfaction: 0.0, 
                    step: 1, 
                    tags: vec![] };
                let species_desire_2 = Desire { 
                    item: DesireItem::Want(3), // shelter
                    start: 7, 
                    end: Some(13), 
                    amount: 1.0, 
                    satisfaction: 0.0, 
                    step: 2, 
                    tags: vec![] };
                let species_desire_3 = Desire { 
                    item: DesireItem::Want(4), //clothing
                    start: 2, 
                    end: Some(8), 
                    amount: 1.0, 
                    satisfaction: 0.0, 
                    step: 2, 
                    tags: vec![] };

                let culture_desire_1 = Desire { 
                    item: DesireItem::Product(2), // ambrosia fruit
                    start: 10, 
                    end: Some(30), 
                    amount: 1.0, 
                    satisfaction: 0.0, 
                    step: 5, 
                    tags: vec![] };
                let culture_desire_2 = Desire { 
                    item: DesireItem::Product(6), // clothes
                    start: 15, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 0.0, 
                    step: 10, 
                    tags: vec![] };

                let ideology_desire_1 = Desire { 
                    item: DesireItem::Product(14), // Hut
                    start: 30, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 0.0, 
                    step: 0, 
                    tags: vec![] };
                let ideology_desire_2 = Desire { 
                    item: DesireItem::Product(15), // Cabin
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
                data.load_all(&String::from("")).expect("Error on load?");
                let product = data.products.get_mut(&6).unwrap();
                product.fractional = true;

                let mut market = MarketHistory {
                    info: HashMap::new(),
                    sale_priority: vec![],
                    currencies: vec![],
                };
                // quickly set all prices to 1.0 for ease going forward.
                for idx in 0..26 {
                    market.info.insert(idx, ProductInfo {
                        available: 0.0,
                        price: 1.0,
                        offered: 0.0,
                        sold: 0.0,
                        salability: 0.5,
                        is_currency: false,
                    });
                }
                // ambrosia fruit
                market.info.get_mut(&2).expect("Brok").salability = 1.0;
                market.info.get_mut(&2).expect("Brok").is_currency = true;

                market.info.get_mut(&6).expect("Brok").price = 10.0;
                market.info.get_mut(&7).expect("Brok").price = 20.0;

                market.info.get_mut(&14).expect("Brok").price = 100.0;
                market.info.get_mut(&15).expect("Brok").price = 1000.0;

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
                    .item = DesireItem::Class(6);
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
                history.info.get_mut(&2).unwrap().price = 1.0; // 20.0 total
                history.info.get_mut(&6).unwrap().price = 2.0; // 40.0 total
                history.info.get_mut(&7).unwrap().price = 10.0; // 0.0 likely to exchange.
                history.info.get_mut(&15).unwrap().price = 5.9; // 59.0 total

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

            use crate::{objects::{property::{Property, DesireCoord}, desire::{Desire, DesireItem}, property_info::PropertyInfo, product::Product, process::{Process, ProcessPart, PartItem, ProcessSectionTag, ProcessTag}, want::Want, market::MarketHistory}, data_manager::DataManager};

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
                            item: PartItem::Specific(0), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input
                        },
                        ProcessPart { 
                            item: PartItem::Specific(1), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input
                        },
                        ProcessPart { 
                            item: PartItem::Want(0), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output
                        }
                    ],
                    process_tags: vec![
                        ProcessTag::Use(1)
                    ],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
                    technology_requirement: None,
                    tertiary_tech: None,
                });
                let market = MarketHistory {
                    info: HashMap::new(),
                    sale_priority: vec!(),
                    currencies: vec!(),
                };

                data.update_product_classes().expect("Could not function");
                let test_desires = vec![
                    Desire::new(DesireItem::Want(0),
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
                    &market, 0.0, 0, None);
                assert_eq!(result1, 2.5); // should get back our full target
                assert_eq!(test.property.get(&1)
                    .unwrap().total_property, 2.5); // should still have 2.5 remaining
                // and sanity check that the other is untouched
                assert_eq!(test.property.get(&0)
                    .unwrap().total_property, 3.0);
                
                // if it was remove, do it again, testing that it only returns what is
                // available.
                let capped_result = test.get_shopping_time(5.0, &data, &market, 0.0, 0, None);
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
                            item: PartItem::Specific(0), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input
                        },
                        ProcessPart { 
                            item: PartItem::Specific(1), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output
                        }
                    ],
                    process_tags: vec![
                        ProcessTag::Use(0)
                    ],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
                    technology_requirement: None,
                    tertiary_tech: None,
                });

                data.products.get_mut(&0).unwrap()
                    .processes.insert(0);
                data.products.get_mut(&1).unwrap()
                    .processes.insert(0);
                let market = MarketHistory {
                    info: HashMap::new(),
                    sale_priority: vec!(),
                    currencies: vec!(),
                };

                data.update_product_classes().expect("Could not function");
                let test_desires = vec![
                    Desire::new(DesireItem::Product(0),
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
                    &market, 0.0, 0, None);
                assert_eq!(result1, 2.0); // should get back our full target
                // of the 5 started, 2 are sifted, 2 is consumed for shopping time.
                assert_eq!(test.property[&0].total_property, 3.0);
                
                // do it again, overdrawing and being limited correctly.
                let capped_result = test.get_shopping_time(5.0, &data, &market, 0.0, 0, None);
                assert_eq!(capped_result, 1.0); // should get back our what is available, nothing more.
                // 2 were reserved, so 2 should remain.
                assert_eq!(test.property[&0].total_property, 2.0);
            }
        }

        mod sift_all_should {
            // TODO update test for lookahead when added.
            use std::collections::{HashMap, HashSet};

            use crate::{objects::{property::Property, desire::{Desire, DesireItem}, property_info::PropertyInfo, product::Product, process::{Process, ProcessPart, PartItem, ProcessSectionTag, ProcessTag}, want::Want}, data_manager::DataManager};

            #[test]
            pub fn shift_want_class_and_specific_desires_correctly() {
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
                            item: PartItem::Specific(1), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input
                        },
                        ProcessPart { 
                            item: PartItem::Specific(2), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Capital
                        },
                        ProcessPart { 
                            item: PartItem::Want(0), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output
                        }
                    ],
                    process_tags: vec![
                        ProcessTag::Use(2)
                    ],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
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
                            item: PartItem::Specific(2), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input
                        },
                        ProcessPart { 
                            item: PartItem::Specific(3), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input
                        },
                        ProcessPart { 
                            item: PartItem::Want(0), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output
                        }
                    ],
                    process_tags: vec![
                        ProcessTag::Consumption(2)
                    ],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
                    technology_requirement: None,
                    tertiary_tech: None,
                });

                data.update_product_classes().expect("Could not function");
                let test_desires = vec![
                    Desire::new(DesireItem::Want(0),
                        1,
                        None,
                        1.0,
                        0.0,
                        1,
                        vec![]).unwrap(),
                    Desire::new(DesireItem::Class(0),
                        3,
                        Some(30),
                        1.0,
                        0.0,
                        3,
                        vec![]).unwrap(),
                    Desire::new(DesireItem::Product(0),
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
                assert!(prop0.reserved == 0.0);
                assert!(prop0.want_reserve == 15.0);
                // assert!(prop0.class_reserve == 0.0); both are in the same class, so either is valid, selection order cannot be guaranteed (yet).
                assert!(prop0.specific_reserve == 2.0);
                let prop1 = test.property.get(&1).unwrap();
                assert!(prop1.total_property == 10.0);
                assert!(prop1.unreserved == 0.0);
                assert!(prop1.reserved == 0.0);
                assert!(prop1.want_reserve == 10.0);
                assert!((prop1.class_reserve + prop0.class_reserve) == 10.0);
                assert!(prop1.specific_reserve == 0.0);
                let prop2 = test.property.get(&2).unwrap();
                assert!(prop2.total_property == 20.0);
                assert!(prop2.unreserved == 0.0);
                assert!(prop2.reserved == 0.0);
                assert!(prop2.want_reserve == 20.0);
                assert!(prop2.class_reserve == 0.0);
                assert!(prop2.specific_reserve == 0.0);
                let prop3 = test.property.get(&3).unwrap();
                assert!(prop3.total_property == 15.0);
                assert!(prop3.unreserved == 5.0);
                assert!(prop3.reserved == 0.0);
                assert!(prop3.want_reserve == 10.0);
                assert!(prop3.class_reserve == 0.0);
                assert!(prop3.specific_reserve == 0.0);
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
                            item: PartItem::Specific(1), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input
                        },
                        ProcessPart { 
                            item: PartItem::Specific(2), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Capital
                        },
                        ProcessPart { 
                            item: PartItem::Want(0), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output
                        }
                    ],
                    process_tags: vec![
                        ProcessTag::Use(2)
                    ],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
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
                            item: PartItem::Specific(2), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input
                        },
                        ProcessPart { 
                            item: PartItem::Specific(3), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input
                        },
                        ProcessPart { 
                            item: PartItem::Want(0), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output
                        }
                    ],
                    process_tags: vec![
                        ProcessTag::Consumption(2)
                    ],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
                    technology_requirement: None,
                    tertiary_tech: None,
                });

                data.update_product_classes().expect("Could not function");
                let test_desires = vec![
                    Desire::new(DesireItem::Want(0),
                        1,
                        None,
                        1.0,
                        0.0,
                        1,
                        vec![]).unwrap(),
                    Desire::new(DesireItem::Class(0),
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
                assert!(prop0.reserved == 0.0);
                assert!(prop0.want_reserve == 15.0);
                // assert!(prop0.class_reserve == 0.0); both are in the same class, so either is valid, selection order cannot be guaranteed (yet).
                assert!(prop0.specific_reserve == 0.0);
                let prop1 = test.property.get(&1).unwrap();
                assert!(prop1.total_property == 10.0);
                assert!(prop1.unreserved == 0.0);
                assert!(prop1.reserved == 0.0);
                assert!(prop1.want_reserve == 10.0);
                assert!((prop1.class_reserve + prop0.class_reserve) == 10.0);
                assert!(prop1.specific_reserve == 0.0);
                let prop2 = test.property.get(&2).unwrap();
                assert!(prop2.total_property == 20.0);
                assert!(prop2.unreserved == 0.0);
                assert!(prop2.reserved == 0.0);
                assert!(prop2.want_reserve == 20.0);
                assert!(prop2.class_reserve == 0.0);
                assert!(prop2.specific_reserve == 0.0);
                let prop3 = test.property.get(&3).unwrap();
                assert!(prop3.total_property == 15.0);
                assert!(prop3.unreserved == 5.0);
                assert!(prop3.reserved == 0.0);
                assert!(prop3.want_reserve == 10.0);
                assert!(prop3.class_reserve == 0.0);
                assert!(prop3.specific_reserve == 0.0);
                // ensure want process plan is recorded correctly.
                assert_eq!(test.process_plan[&0], 10.0);
                assert_eq!(test.process_plan[&1], 10.0);
                assert_eq!(result.tier, 35);
                assert!(453.0 < result.value);
                assert!(result.value < 454.0);
            }

            #[test]
            pub fn shift_want_and_specific_desires_correctly() {
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
                            item: PartItem::Specific(1), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input
                        },
                        ProcessPart { 
                            item: PartItem::Specific(2), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Capital
                        },
                        ProcessPart { 
                            item: PartItem::Want(0), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output
                        }
                    ],
                    process_tags: vec![
                        ProcessTag::Use(2)
                    ],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
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
                            item: PartItem::Specific(2), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input
                        },
                        ProcessPart { 
                            item: PartItem::Specific(3), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input
                        },
                        ProcessPart { 
                            item: PartItem::Want(0), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output
                        }
                    ],
                    process_tags: vec![
                        ProcessTag::Consumption(2)
                    ],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
                    technology_requirement: None,
                    tertiary_tech: None,
                });

                data.update_product_classes().expect("Could not function");
                let test_desires = vec![
                    Desire::new(DesireItem::Want(0),
                        1,
                        None,
                        1.0,
                        0.0,
                        1,
                        vec![]).unwrap(),
                    Desire::new(DesireItem::Product(2),
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
                assert!(prop0.reserved == 0.0);
                assert!(prop0.want_reserve == 15.0);
                assert!(prop0.class_reserve == 0.0);
                assert!(prop0.specific_reserve == 0.0);
                let prop1 = test.property.get(&1).unwrap();
                assert!(prop1.total_property == 10.0);
                assert!(prop1.unreserved == 0.0);
                assert!(prop1.reserved == 0.0);
                assert!(prop1.want_reserve == 10.0);
                assert!(prop1.class_reserve == 0.0);
                assert!(prop1.specific_reserve == 0.0);
                let prop2 = test.property.get(&2).unwrap();
                assert!(prop2.total_property == 20.0);
                assert!(prop2.unreserved == 0.0);
                assert!(prop2.reserved == 0.0);
                assert!(prop2.want_reserve == 20.0);
                assert!(prop2.class_reserve == 0.0);
                assert!(prop2.specific_reserve == 5.0);
                let prop3 = test.property.get(&3).unwrap();
                assert!(prop3.total_property == 15.0);
                assert!(prop3.unreserved == 5.0);
                assert!(prop3.reserved == 0.0);
                assert!(prop3.want_reserve == 10.0);
                assert!(prop3.class_reserve == 0.0);
                assert!(prop3.specific_reserve == 0.0);
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
                            item: PartItem::Specific(1), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input
                        },
                        ProcessPart { 
                            item: PartItem::Specific(2), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Capital
                        },
                        ProcessPart { 
                            item: PartItem::Want(0), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output
                        }
                    ],
                    process_tags: vec![
                        ProcessTag::Use(2)
                    ],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
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
                            item: PartItem::Specific(2), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input
                        },
                        ProcessPart { 
                            item: PartItem::Specific(3), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input
                        },
                        ProcessPart { 
                            item: PartItem::Want(0), 
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output
                        }
                    ],
                    process_tags: vec![
                        ProcessTag::Consumption(2)
                    ],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
                    technology_requirement: None,
                    tertiary_tech: None,
                });

                data.update_product_classes().expect("Could not function");
                let test_desires = vec![
                    Desire::new(DesireItem::Want(0),
                        1,
                        None,
                        1.0,
                        0.0,
                        1,
                        vec![]).unwrap(),
                    Desire::new(DesireItem::Want(0),
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
                assert!(prop0.reserved == 0.0);
                assert!(prop0.want_reserve == 15.0);
                assert!(prop0.class_reserve == 0.0);
                assert!(prop0.specific_reserve == 0.0);
                let prop1 = test.property.get(&1).unwrap();
                assert!(prop1.total_property == 10.0);
                assert!(prop1.unreserved == 0.0);
                assert!(prop1.reserved == 0.0);
                assert!(prop1.want_reserve == 10.0);
                assert!(prop1.class_reserve == 0.0);
                assert!(prop1.specific_reserve == 0.0);
                let prop2 = test.property.get(&2).unwrap();
                assert!(prop2.total_property == 20.0);
                assert!(prop2.unreserved == 0.0);
                assert!(prop2.reserved == 0.0);
                assert!(prop2.want_reserve == 20.0);
                assert!(prop2.class_reserve == 0.0);
                assert!(prop2.specific_reserve == 0.0);
                let prop3 = test.property.get(&3).unwrap();
                assert!(prop3.total_property == 15.0);
                assert!(prop3.unreserved == 5.0);
                assert!(prop3.reserved == 0.0);
                assert!(prop3.want_reserve == 10.0);
                assert!(prop3.class_reserve == 0.0);
                assert!(prop3.specific_reserve == 0.0);
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
                    Desire::new(DesireItem::Class(0),
                        0,
                        None,
                        1.0,
                        0.0,
                        4,
                        vec![]).unwrap(),
                    Desire::new(DesireItem::Class(0),
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
                assert!(prop0.reserved == 0.0);
                assert!(prop0.want_reserve == 0.0);
                assert!(prop0.class_reserve == 15.0);
                assert!(prop0.specific_reserve == 0.0);
                let prop1 = test.property.get(&1).unwrap();
                assert!(prop1.total_property == 15.0);
                assert!(prop1.unreserved == 0.0);
                assert!(prop1.reserved == 0.0);
                assert!(prop1.want_reserve == 0.0);
                assert!(prop1.class_reserve == 15.0);
                assert!(prop1.specific_reserve == 0.0);
                let prop2 = test.property.get(&2).unwrap();
                assert!(prop2.total_property == 10.0);
                assert!(prop2.unreserved == 10.0);
                assert!(prop2.reserved == 0.0);
                assert!(prop2.want_reserve == 0.0);
                assert!(prop2.class_reserve == 0.0);
                assert!(prop2.specific_reserve == 0.0);
                // ensure want process plan is recorded correctly.
                assert_eq!(test.process_plan.len(), 0);
                assert_eq!(result.tier, 100);
                assert!(182146.0 < result.value);
                assert!(result.value < 182147.0);
            }

            /// Sifts 1 specific desire only, don't set up wants
            /// or class desires.
            #[test]
            pub fn sift_specific_desire_correctly() {
                // data needed, but not set up for this test.
                let data = &DataManager::new();
                let test_desires = vec![
                    Desire::new(DesireItem::Product(0),
                        0,
                        Some(100),
                        1.0,
                        0.0,
                        5,
                        vec![]).unwrap(),
                    Desire::new(DesireItem::Product(0),
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
                assert!(prop0.reserved == 0.0);
                assert!(prop0.want_reserve == 0.0);
                assert!(prop0.class_reserve == 0.0);
                assert!(prop0.specific_reserve == 15.0);
                let prop1 = test.property.get(&1).unwrap();
                assert!(prop1.total_property == 10.0);
                assert!(prop1.unreserved == 10.0);
                assert!(prop1.reserved == 0.0);
                assert!(prop1.want_reserve == 0.0);
                assert!(prop1.class_reserve == 0.0);
                assert!(prop1.specific_reserve == 0.0);
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
                item: DesireItem::Product(0), 
                start: 0, 
                end: Some(1), 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 2,4,6
                item: DesireItem::Product(1), 
                start: 2, 
                end: Some(6), 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 3,6,9, ...
                item: DesireItem::Want(0), 
                start: 3, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 3,
                tags: vec![]});
            let mut test = Property::new(test_desires);

            let result1 = test.get_highest_satisfied_tier_for_item(DesireItem::Product(0));
            assert_eq!(result1, None);

            test.desires[0].add_satisfaction(2.0);
            let result2 = test.get_highest_satisfied_tier_for_item(DesireItem::Product(0))
                .expect("Couldn't find.");
            assert_eq!(result2, 1);

            test.desires[1].add_satisfaction(2.0);
            let result3 = test.get_highest_satisfied_tier_for_item(DesireItem::Product(0))
                .expect("Couldn't find.");
            assert_eq!(result3, 1);

            test.desires[2].add_satisfaction(2.0);
            let result4 = test.get_highest_satisfied_tier_for_item(DesireItem::Product(0))
                .expect("Couldn't find.");
            assert_eq!(result4, 1);
        }

        #[test]
        pub fn get_highest_satisfied_tier_correctly() {
            let mut test_desires = vec![];
            test_desires.push(Desire{ // 0,1
                item: DesireItem::Product(0), 
                start: 0, 
                end: Some(1), 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 2,4,6
                item: DesireItem::Product(1), 
                start: 2, 
                end: Some(6), 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 3,6,9, ...
                item: DesireItem::Want(0), 
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
                item: DesireItem::Product(0), 
                start: 0, 
                end: Some(1), 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 0, 2,4,6
                item: DesireItem::Product(1), 
                start: 0, 
                end: Some(6), 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 0,3,6,9, ...
                item: DesireItem::Product(0), 
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
                item: DesireItem::Product(0), 
                start: 0, 
                end: Some(1), 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 0, 2,4,6
                item: DesireItem::Product(1), 
                start: 0, 
                end: Some(6), 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 0,3,6,9, ...
                item: DesireItem::Want(0), 
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
                item: DesireItem::Product(0), 
                start: 0, 
                end: Some(1), 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 2,4,6
                item: DesireItem::Product(1), 
                start: 2, 
                end: Some(6), 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 3,6,9, ...
                item: DesireItem::Want(0), 
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
                item: DesireItem::Product(0), 
                start: 0, 
                end: Some(1), 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 2,4,6
                item: DesireItem::Product(0), 
                start: 2, 
                end: Some(6), 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 3,6,9, ...
                item: DesireItem::Product(0), 
                start: 3, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 3,
                tags: vec![]});
            let mut test = Property::new(test_desires);

            let result1 = test.get_lowest_unsatisfied_tier_of_item(DesireItem::Product(0))
                .expect("Error Found on empty thing.");
            assert_eq!(result1, 0);

            test.desires[0].add_satisfaction(2.0);
            let result2 = test.get_lowest_unsatisfied_tier_of_item(DesireItem::Product(0))
                .expect("Couldn't find.");
            assert_eq!(result2, 2);

            test.desires[1].add_satisfaction(2.0);
            let result3 = test.get_lowest_unsatisfied_tier_of_item(DesireItem::Product(0))
                .expect("Couldn't find.");
            assert_eq!(result3, 3);

            test.desires[2].add_satisfaction(2.0);
            let result4 = test.get_lowest_unsatisfied_tier_of_item(DesireItem::Product(0))
                .expect("Couldn't find.");
            assert_eq!(result4, 6);
        }

        #[test]
        pub fn get_lowest_unsatisfied_tier_for_item_and_exclude_other_items_correctly() {
            let mut test_desires = vec![];
            test_desires.push(Desire{ // 0,1
                item: DesireItem::Product(0), 
                start: 0, 
                end: Some(1), 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 2,4,6
                item: DesireItem::Product(0), 
                start: 2, 
                end: Some(6), 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 3,6,9, ...
                item: DesireItem::Product(1), 
                start: 3, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 3,
                tags: vec![]});
            let mut test = Property::new(test_desires);

            let result1 = test.get_lowest_unsatisfied_tier_of_item(DesireItem::Product(0))
                .expect("Error Found on empty thing.");
            assert_eq!(result1, 0);

            test.desires[0].add_satisfaction(2.0);
            let result2 = test.get_lowest_unsatisfied_tier_of_item(DesireItem::Product(0))
                .expect("Couldn't find.");
            assert_eq!(result2, 2);

            test.desires[1].add_satisfaction(2.0);
            let result3 = test.get_lowest_unsatisfied_tier_of_item(DesireItem::Product(0))
                .expect("Couldn't find.");
            assert_eq!(result3, 6);

            test.desires[2].add_satisfaction(2.0);
            let result4 = test.get_lowest_unsatisfied_tier_of_item(DesireItem::Product(0))
                .expect("Couldn't find.");
            assert_eq!(result4, 6);
        }

        mod walk_up_tiers_should {
            use crate::objects::{desire::{Desire, DesireItem}, property::{Property, DesireCoord}};

            /// Tests that if the function is given a desirecoord with
            /// an index > desires.len(), that it simply wraps back around
            /// to 0.
            #[test]
            pub fn safely_accept_overlapped_idxs_from_prev() {
                let mut test_desires = vec![];
                test_desires.push(Desire{ // 0,1
                    item: DesireItem::Product(0), 
                    start: 0, 
                    end: Some(1), 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 1,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2,4,6
                    item: DesireItem::Product(0), 
                    start: 0, 
                    end: Some(6), 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 2,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,3,6,9, ...
                    item: DesireItem::Product(0), 
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
                    item: DesireItem::Product(0), 
                    start: 0, 
                    end: Some(1), 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 1,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2,4,6
                    item: DesireItem::Product(0), 
                    start: 0, 
                    end: Some(6), 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 2,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,3,6,9, ...
                    item: DesireItem::Product(0), 
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
                    item: DesireItem::Product(0), 
                    start: 0, 
                    end: Some(1), 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 1,
                    tags: vec![]});
                test_desires.push(Desire{ // 0,2,4,6
                    item: DesireItem::Product(0), 
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
                item: DesireItem::Product(0), 
                start: 0, 
                end: Some(4), 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,4,6
                item: DesireItem::Product(1), 
                start: 0, 
                end: Some(6), 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,4,6
                item: DesireItem::Want(0), 
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
                let val = test.walk_up_tiers_for_item(&curr, &DesireItem::Product(0));
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
    }

    mod tiered_value_tests {

        mod partial_cmp_should {
            use crate::objects::property::TieredValue;

            #[test]
            pub fn correctly_check_for_ordering() {
                let a = TieredValue { tier: 5, value: 1.0 }; // baseline
                let b = TieredValue { tier: 5, value: 2.0 }; // same tier greater than
                let c = TieredValue { tier: 5, value: 0.5 }; // same tier less than

                let d = TieredValue { tier: 6, value: 1.5 }; // higher tier GT
                let e = TieredValue { tier: 6, value: 1.0 / 0.9 }; // HT EQ
                let f = TieredValue { tier: 6, value: 1.0 }; // HT LT

                let g = TieredValue { tier: 4, value: 1.0 }; // LT GT
                let h = TieredValue { tier: 4, value: 0.9 };// LT EQ
                let i = TieredValue { tier: 4, value: 0.5 };// LT LT

                assert!(a == a);
                assert!(a < b);
                assert!(a > c);

                assert!(a < d);
                assert!(a == e);
                assert!(a > f);

                assert!(a < g);
                assert!(a == h);
                assert!(a > i);
            }
        }

        mod normalize_should {
            use crate::objects::property::TieredValue;

            #[test]
            pub fn push_into_bound_when_greater_and_positive() {
                let test = TieredValue {tier: 100, value: 10.0};
                let result = test.normalize(9.0);
                assert_eq!(result.tier, 101);
                assert!(result.value < 9.1);
                assert!(result.value > 8.9);
            }

            #[test]
            pub fn push_into_bound_when_lesser_and_positive() {
                let test = TieredValue {tier: 100, value: 0.91};
                let result = test.normalize(2.0);
                assert_eq!(result.tier, 99);
                assert!(result.value < 1.1);
                assert!(result.value > 1.0);
            }

            #[test]
            pub fn push_into_bound_when_lesser_and_negative() {
                let test = TieredValue {tier: 100, value: -10.0};
                let result = test.normalize(9.0);
                assert_eq!(result.tier, 101);
                assert!(result.value > -9.1);
                assert!(result.value < -8.9);
            }

            #[test]
            pub fn push_into_bound_when_greater_and_negative() {
                let test = TieredValue {tier: 100, value: -0.91};
                let result = test.normalize(2.0);
                assert_eq!(result.tier, 99);
                assert!(result.value > -1.1);
                assert!(result.value < -1.0);
            }
        }

        mod add_value_should {
            use crate::{objects::property::TieredValue, constants::TIER_RATIO};

            #[test]
            pub fn set_tier_value_when_zero() {
                let mut test = TieredValue{tier: 0, value: 0.0};
                test.add_value(10, 10.0);
                assert!(test.tier == 10);
                assert!(test.value == 10.0);
            }

            #[test]
            pub fn add_correctly_when_same_tier() {
                let mut test = TieredValue{tier: 10, value: 10.0};
                test.add_value(10, 10.0);
                assert!(test.tier == 10);
                assert!(test.value == 20.0);
            }

            #[test]
            pub fn add_correctly_when_lower_tier() {
                let mut test = TieredValue{tier: 10, value: 10.0};
                test.add_value(9, 10.0);
                assert!(test.tier == 10);
                assert!(test.value == (10.0+10.0/TIER_RATIO));
            }

            #[test]
            pub fn add_correctly_when_higher_tier() {
                let mut test = TieredValue{tier: 10, value: 10.0};
                test.add_value(11, 10.0);
                assert!(test.tier == 10);
                assert!(test.value == (10.0+10.0*TIER_RATIO));
            }
        }

        mod tier_equivalence_should {
            use crate::{objects::property::TieredValue, constants::TIER_RATIO};

            #[test]
            pub fn return_unit_when_tiers_equal() {
                let equiv = TieredValue::tier_equivalence(10, 10);
                assert!{equiv == 1.0};
            }

            #[test]
            pub fn return_tier_ratio_when_1_above() {
                let equiv = TieredValue::tier_equivalence(10, 11);
                assert!{equiv == TIER_RATIO};
            }

            #[test]
            pub fn return_tier_ratio_when_1_below() {
                let equiv = TieredValue::tier_equivalence(10, 9);
                assert!{equiv == (1.0/TIER_RATIO)};
            }

            #[test]
            pub fn return_tier_ratio_greater_difference() {
                let start = 10;
                let diff = 4;
                let equiv = TieredValue::tier_equivalence(start, start+diff);
                assert!{equiv == TIER_RATIO.powf(diff as f64)};
                let equiv = TieredValue::tier_equivalence(start, start-diff);
                assert!{equiv == TIER_RATIO.powf(-(diff as f64))};
            }
        }
    
        mod shift_tier_should {
            use crate::{objects::property::TieredValue, constants::TIER_RATIO};

            /// Tests
            #[test]
            pub fn correctly_shift_value_down_on_shifting_down() {
                let test = TieredValue { tier: 1, value: 1.0 };
                let result = test.shift_tier(0);
                assert_eq!(result.tier, 0);
                assert_eq!(result.value, 1.0 / (1.0 / TIER_RATIO));
            }

            #[test]
            pub fn correctly_shift_value_down_on_shifting_up() {
                let test = TieredValue { tier: 1, value: 1.0 };
                let result = test.shift_tier(2);
                assert_eq!(result.tier, 2);
                assert_eq!(result.value, 1.0 / TIER_RATIO);
            }
        }
    }

    mod desire_tests {
        use crate::objects::desire::{Desire, DesireItem};

        mod steps_in_interval_should {
            use crate::objects::desire::{DesireItem, Desire};

            #[test]
            pub fn always_return_false_when() {
                let test = Desire{ 
                    item: DesireItem::Product(0), 
                    start: 10, 
                    end: None, 
                    amount: 2.0, 
                    satisfaction: 0.0,
                    step: 0,
                    tags: vec![]};
                
                // start is after end.
                assert!(!test.steps_in_interval(11, 9));
            }

            #[test]
            pub fn act_correctly_with_single_tier_desire() {
                let test = Desire{ 
                    item: DesireItem::Product(0), 
                    start: 10, 
                    end: None, 
                    amount: 2.0, 
                    satisfaction: 0.0,
                    step: 0,
                    tags: vec![]};
                
                // single tier start steps in interval and on end points.
                assert!(test.steps_in_interval(9, 11));
                assert!(test.steps_in_interval(10, 11));
                assert!(test.steps_in_interval(9, 10));
                assert!(test.steps_in_interval(10, 10));
                // single tier, steps before/after interval
                assert!(!test.steps_in_interval(11, 12));
                assert!(!test.steps_in_interval(8, 9));
            }

            #[test]
            pub fn act_correctly_with_stretched_desire() {
                let test = Desire{ 
                    item: DesireItem::Product(0), 
                    start: 10, 
                    end: Some(20), 
                    amount: 2.0, 
                    satisfaction: 0.0,
                    step: 2,
                    tags: vec![]};
                
                // single tier start steps in interval and on end points.
                assert!(test.steps_in_interval(9, 11)); // start
                assert!(test.steps_in_interval(19, 21)); // end 
                assert!(test.steps_in_interval(11, 13)); // middle
                // single tier, steps before/after interval
                assert!(!test.steps_in_interval(21, 22)); // after end
                assert!(!test.steps_in_interval(8, 9)); // before start
                assert!(!test.steps_in_interval(11, 11)); // in between steps
            }

            #[test]
            pub fn act_correctly_with_infinite_desire() {
                let test = Desire{ 
                    item: DesireItem::Product(0), 
                    start: 10, 
                    end: None, 
                    amount: 2.0, 
                    satisfaction: 0.0,
                    step: 2,
                    tags: vec![]};
                
                // single tier start steps in interval and on end points.
                assert!(test.steps_in_interval(9, 11)); // start
                assert!(test.steps_in_interval(11, 13)); // middle
                // single tier, steps before/after interval
                assert!(!test.steps_in_interval(8, 9)); // before start
                assert!(!test.steps_in_interval(11, 11)); // in between steps
            }
        }

        mod before_start_should {
            use crate::objects::desire::{DesireItem, Desire};

            #[test]
            pub fn return_false_if_after_start() {
                let test = Desire{ 
                    item: DesireItem::Product(0), 
                    start: 10, 
                    end: None, 
                    amount: 2.0, 
                    satisfaction: 0.0,
                    step: 0,
                    tags: vec![]};

                assert!(!test.before_start(11));
            }

            #[test]
            pub fn return_true_if_before_start() {
                let test = Desire{ 
                    item: DesireItem::Product(0), 
                    start: 10, 
                    end: None, 
                    amount: 2.0, 
                    satisfaction: 0.0,
                    step: 0,
                    tags: vec![]};

                assert!(test.before_start(4));
            }
        }

        mod past_end_should {
            use crate::objects::desire::{DesireItem, Desire};

            #[test]
            pub fn return_false_if_nonstretched_desire_is_before_start() {
                let test = Desire{ 
                    item: DesireItem::Product(0), 
                    start: 10, 
                    end: None, 
                    amount: 2.0, 
                    satisfaction: 0.0,
                    step: 0,
                    tags: vec![]};

                assert!(!test.past_end(4));
            }

            #[test]
            pub fn return_true_if_tier_is_after_start_for_non_stretched_desire() {
                let test = Desire{ 
                    item: DesireItem::Product(0), 
                    start: 0, 
                    end: None, 
                    amount: 2.0, 
                    satisfaction: 0.0,
                    step: 0,
                    tags: vec![]};

                assert!(test.past_end(100));
            }

            #[test]
            pub fn return_false_if_before_last() {
                let test = Desire{ 
                    item: DesireItem::Product(0), 
                    start: 0, 
                    end: Some(10), 
                    amount: 2.0, 
                    satisfaction: 0.0,
                    step: 1,
                    tags: vec![]};

                assert!(!test.past_end(9));
            }

            #[test]
            pub fn return_true_if_tier_after_last() {
                let test = Desire{ 
                    item: DesireItem::Product(0), 
                    start: 0, 
                    end: Some(10), 
                    amount: 2.0, 
                    satisfaction: 0.0,
                    step: 1,
                    tags: vec![]};

                assert!(test.past_end(100));
            }

            #[test]
            pub fn return_false_when_desire_is_infinite() {
                let test = Desire{ 
                    item: DesireItem::Product(0), 
                    start: 0, 
                    end: None, 
                    amount: 2.0, 
                    satisfaction: 0.0,
                    step: 1,
                    tags: vec![]};

                assert!(!test.past_end(100));
            }
        }

        #[test]
        pub fn correctly_add_satisfaction_at_tier() {
            let mut test = Desire{ 
                item: DesireItem::Product(0), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]};

            // only add 1 tier, return remainder.
            let result = test.add_satisfaction_at_tier(100.0, 0);
            assert_eq!(result, 99.0);
            // try again, to ensure we get everything back safely.
            let result = test.add_satisfaction_at_tier(100.0, 0);
            assert_eq!(result, 100.0);
            // force misstep.
            test.change_end(None, 5).expect("Error Found!");
            assert_eq!(test.add_satisfaction_at_tier(100.0, 2), 100.0);
        }

        #[test]
        pub fn correctly_multiply_desire() {
            let test = Desire{ 
                item: DesireItem::Product(0), 
                start: 0, 
                end: None, 
                amount: 2.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]};

            let result = test.create_multiple(5);

            assert_eq!(result.item, test.item);
            assert_eq!(result.start, test.start);
            assert_eq!(result.end, test.end);
            assert_eq!(result.satisfaction, test.satisfaction);
            assert_eq!(result.step, test.step);
            assert_eq!(result.amount, test.amount * 5.0);
        }

        #[test]
        pub fn correctly_add_satisfaction() {
            let mut test = Desire{ 
                item: DesireItem::Product(0), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 1,
                tags: vec![]};

            // infinite add, take everything.
            let result = test.add_satisfaction(100.0);
            assert_eq!(result, 0.0);
            // finite add, return partial
            test.satisfaction = 0.0;
            test.change_end(Some(9), 1).expect("Bad Stuff!");
            let result = test.add_satisfaction(100.0);
            assert_eq!(result, 90.0);
        }

        #[test]
        pub fn get_unsatisfied_to_tier() {
            let mut test = Desire{ 
                item: DesireItem::Product(0), 
                start: 1, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 0,
                tags: vec![]};

                assert_eq!(test.unsatisfied_to_tier().unwrap(), 1);
                test.satisfaction = 0.5;
                assert_eq!(test.unsatisfied_to_tier().unwrap(), 1);
                test.satisfaction = 1.0;
                assert_eq!(test.unsatisfied_to_tier(), Option::None);
                test.satisfaction = 1.5;
                assert_eq!(test.unsatisfied_to_tier(), Option::None);
                // stretched
                test.change_end(Some(10), 1).expect("Error!");
                test.satisfaction = 0.0;
                assert_eq!(test.unsatisfied_to_tier().unwrap(), 1);
                test.satisfaction = 0.5;
                assert_eq!(test.unsatisfied_to_tier().unwrap(), 1);
                test.satisfaction = 1.0;
                assert_eq!(test.unsatisfied_to_tier().unwrap(), 2);
                test.satisfaction = 2.5;
                assert_eq!(test.unsatisfied_to_tier().unwrap(), 3);
                test.satisfaction = 10.0;
                assert_eq!(test.unsatisfied_to_tier(), None);
                test.satisfaction = 11.0;
                assert_eq!(test.unsatisfied_to_tier(), None);

                // infinite
                test.change_end(None, 1).expect("Error!");
                test.satisfaction = 0.0;
                assert_eq!(test.unsatisfied_to_tier().unwrap(), 1);
                test.satisfaction = 5.0;
                assert_eq!(test.unsatisfied_to_tier().unwrap(), 6);
                test.satisfaction = 5.5;
                assert_eq!(test.unsatisfied_to_tier().unwrap(), 6);
                test.satisfaction = 6.0;
                assert_eq!(test.unsatisfied_to_tier().unwrap(), 7);
                test.satisfaction = 100.6;
                assert_eq!(test.unsatisfied_to_tier().unwrap(), 101);
        }

        #[test]
        pub fn get_satisfaction_up_to_tier() {
            let mut test = Desire{ 
                item: DesireItem::Product(0), 
                start: 1, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 0,
                tags: vec![]};

            assert_eq!(test.satisfaction_up_to_tier(), None);
            test.satisfaction = 0.5;
            assert_eq!(test.satisfaction_up_to_tier().unwrap(), 1);
            test.satisfaction = 1.0;
            assert_eq!(test.satisfaction_up_to_tier().unwrap(), 1);
            test.satisfaction = 1.5;
            assert_eq!(test.satisfaction_up_to_tier().unwrap(), 1);
            // stretched
            test.change_end(Some(10), 1).expect("Error!");
            test.satisfaction = 0.0;
            assert_eq!(test.satisfaction_up_to_tier(), None);
            test.satisfaction = 0.5;
            assert_eq!(test.satisfaction_up_to_tier().unwrap(), 1);
            test.satisfaction = 2.5;
            assert_eq!(test.satisfaction_up_to_tier().unwrap(), 3);
            test.satisfaction = 10.0;
            assert_eq!(test.satisfaction_up_to_tier().unwrap(), 10);
            test.satisfaction = 11.0;
            assert_eq!(test.satisfaction_up_to_tier().unwrap(), 10);

            // infinite
            test.change_end(None, 1).expect("Error!");
            test.satisfaction = 0.0;
            assert_eq!(test.satisfaction_up_to_tier(), None);
            test.satisfaction = 5.0;
            assert_eq!(test.satisfaction_up_to_tier().unwrap(), 5);
            test.satisfaction = 5.5;
            assert_eq!(test.satisfaction_up_to_tier().unwrap(), 6);
            test.satisfaction = 6.0;
            assert_eq!(test.satisfaction_up_to_tier().unwrap(), 6);
            test.satisfaction = 100.6;
            assert_eq!(test.satisfaction_up_to_tier().unwrap(), 101);

            test.change_end(None, 1).expect("Uh Oh!");
            test.start = 0;
            test.satisfaction = 0.0;
            assert_eq!(test.satisfaction_up_to_tier(), None);
        }

        #[test]
        pub fn get_next_tier_up_correctly() {
            let mut test = Desire{ 
                item: DesireItem::Product(0), 
                start: 10, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 0,
                tags: vec![]};
            
            // single tier
            assert_eq!(test.get_next_tier_up(9).expect("Error!"), 10);
            assert!(test.get_next_tier_up(10).is_none());
            assert!(test.get_next_tier_up(11).is_none());

            // stretched with end
            test.change_end(Some(100), 10).expect("Bad!");
            assert_eq!(test.get_next_tier_up(9).expect("Error!"), 10); // before start.
            assert_eq!(test.get_next_tier_up(10).expect("Error"), 20); // at start
            assert_eq!(test.get_next_tier_up(11).expect("Error"), 20); // between
            assert!(test.get_next_tier_up(100).is_none()); // end
            assert!(test.get_next_tier_up(101).is_none()); // end ++

            // infinite
            test.change_end(None, 10).expect("Bad!");
            assert_eq!(test.get_next_tier_up(9).expect("Error!"), 10); // before start.
            assert_eq!(test.get_next_tier_up(10).expect("Error"), 20); // at start
            assert_eq!(test.get_next_tier_up(11).expect("Error"), 20); // between
        }

        #[test]
        pub fn change_end_correctly() {
            let mut test = Desire{ 
                item: DesireItem::Product(0), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 0,
                tags: vec![]};
            // normal change
            assert!(test.change_end(Some(10), 2).is_ok());
            // bad change (end and step don't coincide.)
            assert!(test.change_end(Some(10), 3).is_err());
            // good change (infinite desire)
            assert!(test.change_end(None, 3).is_ok());
        }

        #[test]
        pub fn correctly_return_steps() {
            let mut test = Desire{ 
                item: DesireItem::Product(0), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 0,
                tags: vec![]};

            assert_eq!(test.steps(), 1);

            test.change_end(Some(10), 2).expect("Do it right dumbass!");
            assert_eq!(test.steps(), 6);

            test.change_end(None, 2).expect("Do it right dumbass!");
            assert_eq!(test.steps(), 0);
        }

        #[test]
        pub fn show_is_stretched() {
            let mut test = Desire{ 
                item: DesireItem::Product(0), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 0,
                tags: vec![]};
            
            assert!(!test.is_stretched());
            test.change_end(Some(10), 2).expect("Stop being dumb!");
            assert!(test.is_stretched());
            test.end = None;
            assert!(test.is_stretched());
        }

        #[test]
        pub fn show_is_infinite() {
            let mut test = Desire{ 
                item: DesireItem::Product(0), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                step: 0,
                tags: vec![]};
            
            assert!(!test.is_infinite());
            test.change_end(Some(10), 2).expect("Stop being dumb!");
            assert!(!test.is_infinite());
            test.end = None;
            assert!(test.is_infinite());
        }

        #[test]
        pub fn calculate_satisfaction_at_tier() {
            let test = Desire{ 
                item: DesireItem::Product(0), 
                start: 0, 
                end: None, 
                amount: 2.0, 
                satisfaction: 3.5,
                step: 2,
                tags: vec![]};
            
            assert_eq!(test.satisfaction_at_tier(2), 1.5);
            assert_eq!(test.satisfaction_at_tier(0), 2.0);
            assert_eq!(test.satisfaction_at_tier(4), 0.0);
            assert!(test.satisfaction_at_tier(1) == 0.0);
        }

        mod steps_to_tier_should {
            use crate::objects::desire::{Desire, DesireItem};

            
            #[test]
            pub fn calculate_steps_to_tier_correctly_for_infinite_desire() {
                let test = Desire{ 
                    item: DesireItem::Product(0), 
                    start: 0, 
                    end: None, 
                    amount: 2.0, 
                    satisfaction: 3.5,
                    step: 2,
                    tags: vec![]};

                assert_eq!(test.steps_to_tier(2).expect("Error!"), 1);
                assert_eq!(test.steps_to_tier(0).expect("Error!"), 0);
                assert_eq!(test.steps_to_tier(4).expect("Error!"), 2);
                assert!(test.steps_to_tier(1).is_err());
            }

            #[test]
            pub fn calculate_steps_to_tier_correctly_for_stretched_desire() {
                let test = Desire{ 
                    item: DesireItem::Product(0), 
                    start: 0, 
                    end: Some(2), 
                    amount: 2.0, 
                    satisfaction: 3.5,
                    step: 2,
                    tags: vec![]};

                assert_eq!(test.steps_to_tier(2).expect("Error!"), 1);
                assert_eq!(test.steps_to_tier(0).expect("Error!"), 0);
                assert!(test.steps_to_tier(4).is_err());
                assert!(test.steps_to_tier(1).is_err());
            }

            #[test]
            pub fn calculate_steps_to_tier_correctly_for_singular_tier_desire() {
                let test = Desire{ 
                    item: DesireItem::Product(0), 
                    start: 1, 
                    end: None, 
                    amount: 2.0, 
                    satisfaction: 3.5,
                    step: 0,
                    tags: vec![]};

                assert!(test.steps_to_tier(0).is_err());
                assert_eq!(test.steps_to_tier(1).expect("Error!"), 0);
                assert!(test.steps_to_tier(2).is_err());
            }

        }

        mod steps_on_tier_should {
            use crate::objects::desire::{Desire, DesireItem};

            #[test]
            pub fn calculate_steps_on_tier_correctly() {
                let mut test = Desire{ 
                    item: DesireItem::Product(0), 
                    start: 0, 
                    end: Some(10), 
                    amount: 2.0, 
                    satisfaction: 3.5,
                    step: 2,
                    tags: vec![]};
                
                assert!(test.steps_on_tier(0));
                assert!(!test.steps_on_tier(1));
                assert!(test.steps_on_tier(2));
                assert!(!test.steps_on_tier(12));

                test.change_end(None, 2).expect("Error!");
                assert!(test.steps_on_tier(0));
                assert!(!test.steps_on_tier(1));
                assert!(test.steps_on_tier(2));
                assert!(test.steps_on_tier(12));

                let test = Desire{ 
                    item: DesireItem::Product(0), 
                    start: 0, 
                    end: None, 
                    amount: 1.0, 
                    satisfaction: 0.0,
                    step: 1,
                    tags: vec![]};

                assert!(test.steps_on_tier(0));
                assert!(test.steps_on_tier(1));
                assert!(test.steps_on_tier(10));
            }
        }

        #[test]
        pub fn check_if_fully_satisfied() {
            let mut test = Desire{ 
                item: DesireItem::Product(0), 
                start: 0, 
                end: Some(10), 
                amount: 2.0, 
                satisfaction: 3.5,
                step: 2,
                tags: vec![]};
            
            assert!(!test.is_fully_satisfied());
            test.satisfaction = 12.0;
            assert!(test.is_fully_satisfied());
        }
    
        #[test]
        pub fn calculate_total_desire() {
            let mut test = Desire{ 
                item: DesireItem::Product(0), 
                start: 0, 
                end: Some(10), 
                amount: 2.0, 
                satisfaction: 3.5,
                step: 2,
                tags: vec![]};
            
            assert_eq!(test.total_desire(), Some(12.0));
            test.change_end(Some(18), 2).expect("Error!");
            assert_eq!(test.total_desire(), Some(20.0));
            test.change_end(None, 2).expect("ERROR!");
            assert_eq!(test.total_desire(), None);
        }

        #[test]
        pub fn calculate_total_satisfaction() {
            let mut test = Desire{ 
                item: DesireItem::Product(0), 
                start: 0, 
                end: Some(10), 
                amount: 2.0, 
                satisfaction: 3.5,
                step: 2,
                tags: vec![]};

            assert_eq!(test.total_satisfaction(), 1.75);
            test.satisfaction = 9.0;
            assert_eq!(test.total_satisfaction(), 4.5);
            test.amount = 3.0;
            assert_eq!(test.total_satisfaction(), 3.0);
        }

        #[test]
        pub fn calculate_total_desire_at_tier() {
            let mut test = Desire{ 
                item: DesireItem::Product(0), 
                start: 0, 
                end: Some(10), 
                amount: 2.0, 
                satisfaction: 3.5,
                step: 2,
                tags: vec![]};

            assert_eq!(test.total_desire_at_tier(2).expect("Error"), 4.0);
            assert_eq!(test.total_desire_at_tier(0).expect("Error"), 2.0);
            assert!(test.total_desire_at_tier(1).is_err());
            test.end = None;
            test.step = 0;
            assert_eq!(test.total_desire_at_tier(0).expect("Error"), 2.0);
        }
    }

    mod data_manager_tests {
        use std::collections::{HashMap, HashSet};

        use crate::{data_manager::DataManager, objects::product::Product};

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
            use itertools::Itertools;

            let mut test = DataManager::new();
            let result = test.load_all(&String::new());

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
                println!("{:>3} | {:<}", id, test.technology[id].name());
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

            println!("----- Skill Group -----");
            println!("----+------------------");
            println!("| id|name");
            for id in test.skill_groups.keys().sorted() {
                println!("{:>3} | {:<}", id, test.skill_groups[id].name);
            }

            println!("----- Skill -----");
            println!("----+------------------");
            println!("| id|name");
            for id in test.skills.keys().sorted() {
                println!("{:>3} | {:<}", id, test.skills[id].name);
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

    mod process_tests {
        use crate::{objects::process::{Process, ProcessPart, PartItem, ProcessSectionTag}, data_manager::DataManager};

        mod uses_product_should {
            use crate::{data_manager::DataManager, objects::{product::Product, process::{Process, ProcessPart, ProcessSectionTag, PartItem}}};

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
                            item: PartItem::Class(0),
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input 
                        },
                        ProcessPart{ 
                            item: PartItem::Specific(2),
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Capital 
                        },
                        ProcessPart{ 
                            item: PartItem::Specific(3),
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output 
                        }
                    ],
                    process_tags: vec![],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
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
                            item: PartItem::Specific(2),
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input 
                        },
                        ProcessPart{ 
                            item: PartItem::Class(0),
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Capital 
                        },
                        ProcessPart{ 
                            item: PartItem::Specific(3),
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output 
                        }
                    ],
                    process_tags: vec![],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
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
                            item: PartItem::Specific(2),
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input 
                        },
                        ProcessPart{ 
                            item: PartItem::Specific(0),
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Capital 
                        },
                        ProcessPart{ 
                            item: PartItem::Specific(3),
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output 
                        }
                    ],
                    process_tags: vec![],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
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
                            item: PartItem::Specific(2),
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input 
                        },
                        ProcessPart{ 
                            item: PartItem::Class(0),
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Capital 
                        },
                        ProcessPart{ 
                            item: PartItem::Specific(3),
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output 
                        }
                    ],
                    process_tags: vec![],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
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
                            item: PartItem::Specific(2),
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input 
                        },
                        ProcessPart{ 
                            item: PartItem::Class(0),
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Capital 
                        },
                        ProcessPart{ 
                            item: PartItem::Specific(3),
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output 
                        }
                    ],
                    process_tags: vec![],
                    skill: None,
                    skill_minimum: 0.0,
                    skill_maximum: 0.0,
                    technology_requirement: None,
                    tertiary_tech: None,
                };
                assert!(!process.uses_product(3, &data));
            }
        }

        mod do_process_with_property_should {
            use std::{str::FromStr, collections::HashMap};

            use crate::{objects::{process::{Process, ProcessPart, ProcessSectionTag, PartItem}, property_info::PropertyInfo}, data_manager::DataManager};

            #[test]
            pub fn return_process_returns_empty_correctly() {
                let mut data = DataManager::new();
                data.load_all(&"file_name".to_string()).expect("Failed!");
                let test = Process{ id: 0, name: String::from_str("Test").unwrap(), 
                    variant_name: String::from_str("").unwrap(), 
                    description: String::from_str("test").unwrap(), 
                    minimum_time: 0.0, process_parts: vec![
                        ProcessPart{ item: PartItem::Specific(0), // input product
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input },
                        ProcessPart{ item: PartItem::Want(0), // input want
                            amount: 2.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input },
                        ProcessPart{ item: PartItem::Specific(1), // Capital product
                            amount: 0.5, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Capital },
                        // placeholder for capital want
                        ProcessPart{ item: PartItem::Specific(2), // output product
                            amount: 1.5, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output },
                        ProcessPart{ item: PartItem::Want(2), // output want
                            amount: 2.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output },
                    ], 
                    process_tags: vec![], 
                    skill: Some(0), skill_minimum: 0.0, skill_maximum: 100.0, 
                    technology_requirement: None, tertiary_tech: None };

                let available_products = HashMap::new();
                let available_wants = HashMap::new();
                let result = test.do_process_with_property(&available_products, 
                    &available_wants, 0.0, 0.0, 
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
                data.load_all(&"file_name".to_string()).expect("Failed!");
                let test = Process{ id: 0, name: String::from_str("Test").unwrap(), 
                    variant_name: String::from_str("").unwrap(), 
                    description: String::from_str("test").unwrap(), 
                    minimum_time: 0.0, process_parts: vec![
                        ProcessPart{ item: PartItem::Specific(0), // input product
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input },
                        ProcessPart{ item: PartItem::Want(0), // input want
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input },
                        ProcessPart{ item: PartItem::Specific(1), // Capital product
                            amount: 0.5, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Capital },
                        // placeholder for capital want
                        ProcessPart{ item: PartItem::Specific(2), // output product
                            amount: 1.5, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output },
                        ProcessPart{ item: PartItem::Want(2), // output want
                            amount: 2.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output },
                    ], 
                    process_tags: vec![], 
                    skill: Some(0), skill_minimum: 0.0, skill_maximum: 100.0, 
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
                    &available_wants, 0.0, 0.0, 
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
                data.load_all(&"file_name".to_string()).expect("Failed!");
                let test = Process{ id: 0, name: String::from_str("Test").unwrap(), 
                    variant_name: String::from_str("").unwrap(), 
                    description: String::from_str("test").unwrap(), 
                    minimum_time: 0.0, process_parts: vec![
                        ProcessPart{ item: PartItem::Specific(0), // input product
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input },
                        ProcessPart{ item: PartItem::Want(0), // input want
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input },
                        ProcessPart{ item: PartItem::Specific(1), // Capital product
                            amount: 0.5, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Capital },
                        // placeholder for capital want
                        ProcessPart{ item: PartItem::Specific(2), // output product
                            amount: 1.5, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output },
                        ProcessPart{ item: PartItem::Want(2), // output want
                            amount: 2.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output },
                    ], 
                    process_tags: vec![], 
                    skill: Some(0), skill_minimum: 0.0, skill_maximum: 100.0, 
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
                    &available_wants, 0.0, 0.0, 
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

            /// Only checks against reserved wants, not all possible reserves.
            /// Add those later, should we feel like it.
            #[test]
            pub fn return_process_correctly_restricts_based_on_property_reservations() {
                let mut data = DataManager::new();
                data.load_all(&"file_name".to_string()).expect("Failed!");
                let test = Process{ id: 0, name: String::from_str("Test").unwrap(), 
                    variant_name: String::from_str("").unwrap(), 
                    description: String::from_str("test").unwrap(), 
                    minimum_time: 0.0, process_parts: vec![
                        ProcessPart{ item: PartItem::Specific(0), // input product
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input },
                        ProcessPart{ item: PartItem::Want(0), // input want
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input },
                        ProcessPart{ item: PartItem::Specific(1), // Capital product
                            amount: 0.5, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Capital },
                        // placeholder for capital want
                        ProcessPart{ item: PartItem::Specific(2), // output product
                            amount: 1.5, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output },
                        ProcessPart{ item: PartItem::Want(2), // output want
                            amount: 2.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output },
                    ], 
                    process_tags: vec![], 
                    skill: Some(0), skill_minimum: 0.0, skill_maximum: 100.0, 
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
                    &available_wants, 0.0, 0.0, 
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
            pub fn return_process_returns_no_iteration_when_missing_input() {
                let mut data = DataManager::new();
                data.load_all(&"file_name".to_string()).expect("Failed!");
                let test = Process{ id: 0, name: String::from_str("Test").unwrap(), 
                    variant_name: String::from_str("").unwrap(), 
                    description: String::from_str("test").unwrap(), 
                    minimum_time: 0.0, process_parts: vec![
                        ProcessPart{ item: PartItem::Specific(0), // input product
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input },
                        ProcessPart{ item: PartItem::Want(0), // input want
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input },
                        ProcessPart{ item: PartItem::Specific(1), // Capital product
                            amount: 0.5, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Capital },
                        // placeholder for capital want
                        ProcessPart{ item: PartItem::Specific(2), // output product
                            amount: 1.5, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output },
                        ProcessPart{ item: PartItem::Want(2), // output want
                            amount: 2.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output },
                    ], 
                    process_tags: vec![], 
                    skill: Some(0), skill_minimum: 0.0, skill_maximum: 100.0, 
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
                    &available_wants, 0.0, 0.0, 
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
                data.load_all(&"file_name".to_string()).expect("Failed!");
                let test = Process{ id: 0, name: String::from_str("Test").unwrap(), 
                    variant_name: String::from_str("").unwrap(), 
                    description: String::from_str("test").unwrap(), 
                    minimum_time: 0.0, process_parts: vec![
                        ProcessPart{ item: PartItem::Specific(0), // input product
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input },
                        ProcessPart{ item: PartItem::Want(0), // input want
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input },
                        ProcessPart{ item: PartItem::Specific(1), // Capital product
                            amount: 0.5, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Capital },
                        // placeholder for capital want
                        ProcessPart{ item: PartItem::Specific(2), // output product
                            amount: 1.5, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output },
                        ProcessPart{ item: PartItem::Want(2), // output want
                            amount: 2.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output },
                    ], 
                    process_tags: vec![], 
                    skill: Some(0), skill_minimum: 0.0, skill_maximum: 100.0, 
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
                    &available_wants, 0.0, 0.0, 
                    None, true, &data);
                // check that it's all empty.
                assert!(result.iterations == 0.0);
                assert!(result.effective_iterations == 0.0);
                assert!(result.efficiency == 1.0);
                assert_eq!(result.input_output_products.len(), 0);
                assert_eq!(result.input_output_wants.len(), 0);
                assert_eq!(result.capital_products.len(), 0);
            }
        }

        mod do_process {
            use std::{str::FromStr, collections::HashMap};

            use crate::{objects::process::{Process, ProcessPart, ProcessSectionTag, PartItem}, data_manager::DataManager};

            mod do_process_should {
                use std::{str::FromStr, collections::HashMap};

                use crate::{objects::process::{Process, PartItem, ProcessPart, ProcessSectionTag}, data_manager::DataManager};

                #[test]
                pub fn return_process_returns_empty_correctly() {
                    let mut data = DataManager::new();
                    data.load_all(&"file_name".to_string()).expect("Failed!");
                    let test = Process{ id: 0, name: String::from_str("Test").unwrap(), 
                        variant_name: String::from_str("").unwrap(), 
                        description: String::from_str("test").unwrap(), 
                        minimum_time: 0.0, process_parts: vec![
                            ProcessPart{ item: PartItem::Specific(0), // input product
                                amount: 1.0, 
                                part_tags: vec![], 
                                part: ProcessSectionTag::Input },
                            ProcessPart{ item: PartItem::Want(0), // input want
                                amount: 2.0, 
                                part_tags: vec![], 
                                part: ProcessSectionTag::Input },
                            ProcessPart{ item: PartItem::Specific(1), // Capital product
                                amount: 0.5, 
                                part_tags: vec![], 
                                part: ProcessSectionTag::Capital },
                            // placeholder for capital want
                            ProcessPart{ item: PartItem::Specific(2), // output product
                                amount: 1.5, 
                                part_tags: vec![], 
                                part: ProcessSectionTag::Output },
                            ProcessPart{ item: PartItem::Want(2), // output want
                                amount: 2.0, 
                                part_tags: vec![], 
                                part: ProcessSectionTag::Output },
                        ], 
                        process_tags: vec![], 
                        skill: Some(0), skill_minimum: 0.0, skill_maximum: 100.0, 
                        technology_requirement: None, tertiary_tech: None };

                    let available_products = HashMap::new();
                    let available_wants = HashMap::new();
                    let result = test.do_process(&available_products, 
                        &available_wants, 0.0, 0.0, 
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
                    data.load_all(&"file_name".to_string()).expect("Failed!");
                    let test = Process{ id: 0, name: String::from_str("Test").unwrap(), 
                        variant_name: String::from_str("").unwrap(), 
                        description: String::from_str("test").unwrap(), 
                        minimum_time: 0.0, process_parts: vec![
                            ProcessPart{ item: PartItem::Specific(0), // input product
                                amount: 1.0, 
                                part_tags: vec![], 
                                part: ProcessSectionTag::Input },
                            ProcessPart{ item: PartItem::Want(0), // input want
                                amount: 1.0, 
                                part_tags: vec![], 
                                part: ProcessSectionTag::Input },
                            ProcessPart{ item: PartItem::Specific(1), // Capital product
                                amount: 0.5, 
                                part_tags: vec![], 
                                part: ProcessSectionTag::Capital },
                            // placeholder for capital want
                            ProcessPart{ item: PartItem::Specific(2), // output product
                                amount: 1.5, 
                                part_tags: vec![], 
                                part: ProcessSectionTag::Output },
                            ProcessPart{ item: PartItem::Want(2), // output want
                                amount: 2.0, 
                                part_tags: vec![], 
                                part: ProcessSectionTag::Output },
                        ], 
                        process_tags: vec![], 
                        skill: Some(0), skill_minimum: 0.0, skill_maximum: 100.0, 
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
                        &available_wants, 0.0, 0.0, 
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
                    data.load_all(&"file_name".to_string()).expect("Failed!");
                    let test = Process{ id: 0, name: String::from_str("Test").unwrap(), 
                        variant_name: String::from_str("").unwrap(), 
                        description: String::from_str("test").unwrap(), 
                        minimum_time: 0.0, process_parts: vec![
                            ProcessPart{ item: PartItem::Specific(0), // input product
                                amount: 1.0, 
                                part_tags: vec![], 
                                part: ProcessSectionTag::Input },
                            ProcessPart{ item: PartItem::Want(0), // input want
                                amount: 1.0, 
                                part_tags: vec![], 
                                part: ProcessSectionTag::Input },
                            ProcessPart{ item: PartItem::Specific(1), // Capital product
                                amount: 0.5, 
                                part_tags: vec![], 
                                part: ProcessSectionTag::Capital },
                            // placeholder for capital want
                            ProcessPart{ item: PartItem::Specific(2), // output product
                                amount: 1.5, 
                                part_tags: vec![], 
                                part: ProcessSectionTag::Output },
                            ProcessPart{ item: PartItem::Want(2), // output want
                                amount: 2.0, 
                                part_tags: vec![], 
                                part: ProcessSectionTag::Output },
                        ], 
                        process_tags: vec![], 
                        skill: Some(0), skill_minimum: 0.0, skill_maximum: 100.0, 
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
                        &available_wants, 0.0, 0.0, 
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
                    data.load_all(&"file_name".to_string()).expect("Failed!");
                    let test = Process{ id: 0, name: String::from_str("Test").unwrap(), 
                        variant_name: String::from_str("").unwrap(), 
                        description: String::from_str("test").unwrap(), 
                        minimum_time: 0.0, process_parts: vec![
                            ProcessPart{ item: PartItem::Specific(0), // input product
                                amount: 1.0, 
                                part_tags: vec![], 
                                part: ProcessSectionTag::Input },
                            ProcessPart{ item: PartItem::Want(0), // input want
                                amount: 1.0, 
                                part_tags: vec![], 
                                part: ProcessSectionTag::Input },
                            ProcessPart{ item: PartItem::Specific(1), // Capital product
                                amount: 0.5, 
                                part_tags: vec![], 
                                part: ProcessSectionTag::Capital },
                            // placeholder for capital want
                            ProcessPart{ item: PartItem::Specific(2), // output product
                                amount: 1.5, 
                                part_tags: vec![], 
                                part: ProcessSectionTag::Output },
                            ProcessPart{ item: PartItem::Want(2), // output want
                                amount: 2.0, 
                                part_tags: vec![], 
                                part: ProcessSectionTag::Output },
                        ], 
                        process_tags: vec![], 
                        skill: Some(0), skill_minimum: 0.0, skill_maximum: 100.0, 
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
                        &available_wants, 0.0, 0.0, 
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
                    data.load_all(&"file_name".to_string()).expect("Failed!");
                    let test = Process{ id: 0, name: String::from_str("Test").unwrap(), 
                        variant_name: String::from_str("").unwrap(), 
                        description: String::from_str("test").unwrap(), 
                        minimum_time: 0.0, process_parts: vec![
                            ProcessPart{ item: PartItem::Specific(0), // input product
                                amount: 1.0, 
                                part_tags: vec![], 
                                part: ProcessSectionTag::Input },
                            ProcessPart{ item: PartItem::Want(0), // input want
                                amount: 1.0, 
                                part_tags: vec![], 
                                part: ProcessSectionTag::Input },
                            ProcessPart{ item: PartItem::Specific(1), // Capital product
                                amount: 0.5, 
                                part_tags: vec![], 
                                part: ProcessSectionTag::Capital },
                            // placeholder for capital want
                            ProcessPart{ item: PartItem::Specific(2), // output product
                                amount: 1.5, 
                                part_tags: vec![], 
                                part: ProcessSectionTag::Output },
                            ProcessPart{ item: PartItem::Want(2), // output want
                                amount: 2.0, 
                                part_tags: vec![], 
                                part: ProcessSectionTag::Output },
                        ], 
                        process_tags: vec![], 
                        skill: Some(0), skill_minimum: 0.0, skill_maximum: 100.0, 
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
                        &available_wants, 0.0, 0.0, 
                        None, true, &data);
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

                use crate::objects::process::{Process, ProcessPart, PartItem, ProcessSectionTag};

                #[test]
                pub fn return_correctly_for_input_capital_output_and_unrelated_items() {
                    let test = Process{ id: 0, name: String::from_str("Test").unwrap(), 
                        variant_name: String::from_str("").unwrap(), 
                        description: String::from_str("test").unwrap(), 
                        minimum_time: 0.0, process_parts: vec![
                            ProcessPart{ item: PartItem::Specific(0), // input product
                                amount: 1.0, 
                                part_tags: vec![], 
                                part: ProcessSectionTag::Input },
                            ProcessPart{ item: PartItem::Want(0), // input want
                                amount: 1.0, 
                                part_tags: vec![], 
                                part: ProcessSectionTag::Input },
                            ProcessPart{ item: PartItem::Specific(1), // Capital product
                                amount: 1.0, 
                                part_tags: vec![], 
                                part: ProcessSectionTag::Capital },
                            // placeholder for capital want
                            ProcessPart{ item: PartItem::Specific(2), // output product
                                amount: 1.0, 
                                part_tags: vec![], 
                                part: ProcessSectionTag::Output },
                            ProcessPart{ item: PartItem::Want(2), // output want
                                amount: 1.0, 
                                part_tags: vec![], 
                                part: ProcessSectionTag::Output },
                        ], 
                        process_tags: vec![], 
                        skill: Some(0), skill_minimum: 0.0, skill_maximum: 100.0, 
                        technology_requirement: None, tertiary_tech: None };

                    // not in at all
                    assert!(test.effective_output_of(PartItem::Want(3)) == 0.0);
                    assert!(test.effective_output_of(PartItem::Specific(3)) == 0.0);
                    // input
                    assert!(test.effective_output_of(PartItem::Want(0)) == 0.0);
                    assert!(test.effective_output_of(PartItem::Specific(0)) == 0.0);
                    // capital
                    // capital want placeholder.
                    assert!(test.effective_output_of(PartItem::Specific(1)) == 0.0);
                    // output 
                    assert!(test.effective_output_of(PartItem::Want(2)) == 1.0);
                    assert!(test.effective_output_of(PartItem::Specific(2)) == 1.0);
                }
            }

            #[test]
            pub fn should_return_correctly_when_eerything_needed_is_given() {
                let mut data = DataManager::new();
                data.load_all(&"file_name".to_string()).expect("Failed!");
                let test = Process{ id: 0, name: String::from_str("Test").unwrap(), 
                    variant_name: String::from_str("").unwrap(), 
                    description: String::from_str("test").unwrap(), 
                    minimum_time: 0.0, process_parts: vec![
                        ProcessPart{ item: PartItem::Specific(0), // input product
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input },
                        ProcessPart{ item: PartItem::Want(0), // input want
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input },
                        ProcessPart{ item: PartItem::Specific(1), // Capital product
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Capital },
                        // placeholder for capital want
                        ProcessPart{ item: PartItem::Specific(2), // output product
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output },
                        ProcessPart{ item: PartItem::Want(2), // output want
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output },
                    ], 
                    process_tags: vec![], 
                    skill: Some(0), skill_minimum: 0.0, skill_maximum: 100.0, 
                    technology_requirement: None, tertiary_tech: None };
                
                // build available items
                let mut avail_products = HashMap::new();
                avail_products.insert(0, 2.0);
                avail_products.insert(1, 2.0);
                let mut avail_wants = HashMap::new();
                avail_wants.insert(0, 2.0);
                
                let results = test.do_process(&avail_products, &avail_wants, 
                    0.0, 0.0, None, false, &data);
                
                assert_eq!(results.capital_products.len(), 1);
                assert!(*results.capital_products.get(&1).unwrap() == 2.0);
                assert_eq!(results.input_output_products.len(), 2);
                assert!(*results.input_output_products.get(&0).unwrap() == -2.0);
                assert!(*results.input_output_products.get(&2).unwrap() == 2.0);
                assert_eq!(results.input_output_wants.len(), 2);
                assert!(*results.input_output_products.get(&0).unwrap() == -2.0);
                assert!(*results.input_output_products.get(&2).unwrap() == 2.0);
                assert!(results.effective_iterations == 2.0);
                assert!(results.efficiency == 1.0);
                assert!(results.iterations == 2.0);
            }

            #[test]
            pub fn should_return_empty_when_input_product_is_missing() {
                let mut data = DataManager::new();
                data.load_all(&"file_name".to_string()).expect("Failed!");
                let test = Process{ id: 0, name: String::from_str("Test").unwrap(), 
                    variant_name: String::from_str("").unwrap(), 
                    description: String::from_str("test").unwrap(), 
                    minimum_time: 0.0, process_parts: vec![
                        ProcessPart{ item: PartItem::Specific(0), // input product
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input },
                        ProcessPart{ item: PartItem::Want(0), // input want
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input },
                        ProcessPart{ item: PartItem::Specific(1), // Capital product
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Capital },
                        // placeholder for capital want
                        ProcessPart{ item: PartItem::Specific(2), // output product
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output },
                        ProcessPart{ item: PartItem::Want(2), // output want
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output },
                    ], 
                    process_tags: vec![], 
                    skill: Some(0), skill_minimum: 0.0, skill_maximum: 100.0, 
                    technology_requirement: None, tertiary_tech: None };
                
                // build available items
                let mut avail_products = HashMap::new();
                //avail_products.insert(0, 2.0);
                avail_products.insert(1, 2.0);
                let mut avail_wants = HashMap::new();
                avail_wants.insert(0, 2.0);
                
                let results = test.do_process(&avail_products, &avail_wants, 
                    0.0, 0.0, None, false, &data);
                
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
                data.load_all(&"file_name".to_string()).expect("Failed!");
                let test = Process{ id: 0, name: String::from_str("Test").unwrap(), 
                    variant_name: String::from_str("").unwrap(), 
                    description: String::from_str("test").unwrap(), 
                    minimum_time: 0.0, process_parts: vec![
                        ProcessPart{ item: PartItem::Specific(0), // input product
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input },
                        ProcessPart{ item: PartItem::Want(0), // input want
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input },
                        ProcessPart{ item: PartItem::Specific(1), // Capital product
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Capital },
                        // placeholder for capital want
                        ProcessPart{ item: PartItem::Specific(2), // output product
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output },
                        ProcessPart{ item: PartItem::Want(2), // output want
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output },
                    ], 
                    process_tags: vec![], 
                    skill: Some(0), skill_minimum: 0.0, skill_maximum: 100.0, 
                    technology_requirement: None, tertiary_tech: None };
                
                // build available items
                let mut avail_products = HashMap::new();
                avail_products.insert(0, 2.0);
                //avail_products.insert(1, 2.0);
                let mut avail_wants = HashMap::new();
                avail_wants.insert(0, 2.0);
                
                let results = test.do_process(&avail_products, &avail_wants, 
                    0.0, 0.0, None, false, &data);
                
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
                data.load_all(&"file_name".to_string()).expect("Failed!");
                let test = Process{ id: 0, name: String::from_str("Test").unwrap(), 
                    variant_name: String::from_str("").unwrap(), 
                    description: String::from_str("test").unwrap(), 
                    minimum_time: 0.0, process_parts: vec![
                        ProcessPart{ item: PartItem::Specific(0), // input product
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input },
                        ProcessPart{ item: PartItem::Want(0), // input want
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Input },
                        ProcessPart{ item: PartItem::Specific(1), // Capital product
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Capital },
                        // placeholder for capital want
                        ProcessPart{ item: PartItem::Specific(2), // output product
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output },
                        ProcessPart{ item: PartItem::Want(2), // output want
                            amount: 1.0, 
                            part_tags: vec![], 
                            part: ProcessSectionTag::Output },
                    ], 
                    process_tags: vec![], 
                    skill: Some(0), skill_minimum: 0.0, skill_maximum: 100.0, 
                    technology_requirement: None, tertiary_tech: None };
                
                // build available items
                let mut avail_products = HashMap::new();
                avail_products.insert(0, 2.0);
                avail_products.insert(1, 2.0);
                let avail_wants = HashMap::new();
                //avail_wants.insert(0, 2.0);
                
                let results = test.do_process(&avail_products, &avail_wants, 
                    0.0, 0.0, None, false, &data);
                
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
                skill: None,
                skill_minimum: 0.0,
                skill_maximum: 0.0,
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
                skill: None,
                skill_minimum: 0.0,
                skill_maximum: 0.0,
                technology_requirement: None,
                tertiary_tech: None,
            };

            // product never matches want ever, don't bother checking those mismatches
            // test input product to test_other input correct (never match)
            let input = ProcessPart{
                item: PartItem::Specific(0),
                amount: 1.0,
                part_tags: vec![],
                part: ProcessSectionTag::Input,
            };
            let output = ProcessPart{
                item: PartItem::Specific(0),
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
                item: PartItem::Want(0),
                amount: 1.0,
                part_tags: vec![],
                part: ProcessSectionTag::Input,
            };
            let output = ProcessPart{
                item: PartItem::Want(0),
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
                item: PartItem::Specific(0),
                amount: 1.0,
                part_tags: vec![],
                part: ProcessSectionTag::Input,
            };
            let output = ProcessPart{
                item: PartItem::Specific(0),
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
                item: PartItem::Want(0),
                amount: 1.0,
                part_tags: vec![],
                part: ProcessSectionTag::Input,
            };
            let output = ProcessPart{
                item: PartItem::Want(0),
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
                item: PartItem::Specific(0),
                amount: 1.0,
                part_tags: vec![],
                part: ProcessSectionTag::Input,
            };
            let output = ProcessPart{
                item: PartItem::Specific(0),
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
                item: PartItem::Want(0),
                amount: 1.0,
                part_tags: vec![],
                part: ProcessSectionTag::Input,
            };
            let output = ProcessPart{
                item: PartItem::Want(0),
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
                item: PartItem::Specific(0),
                amount: 1.0,
                part_tags: vec![],
                part: ProcessSectionTag::Capital,
            };
            let output = ProcessPart{
                item: PartItem::Specific(0),
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
                item: PartItem::Want(0),
                amount: 1.0,
                part_tags: vec![],
                part: ProcessSectionTag::Capital,
            };
            let output = ProcessPart{
                item: PartItem::Want(0),
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
                item: PartItem::Specific(0),
                amount: 1.0,
                part_tags: vec![],
                part: ProcessSectionTag::Capital,
            };
            let output = ProcessPart{
                item: PartItem::Specific(0),
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
                item: PartItem::Want(0),
                amount: 1.0,
                part_tags: vec![],
                part: ProcessSectionTag::Capital,
            };
            let output = ProcessPart{
                item: PartItem::Want(0),
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
                item: PartItem::Specific(0),
                amount: 1.0,
                part_tags: vec![],
                part: ProcessSectionTag::Capital,
            };
            let output = ProcessPart{
                item: PartItem::Specific(0),
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
                item: PartItem::Want(0),
                amount: 1.0,
                part_tags: vec![],
                part: ProcessSectionTag::Capital,
            };
            let output = ProcessPart{
                item: PartItem::Want(0),
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
                item: PartItem::Specific(0),
                amount: 1.0,
                part_tags: vec![],
                part: ProcessSectionTag::Output,
            };
            let output = ProcessPart{
                item: PartItem::Specific(0),
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
                item: PartItem::Want(0),
                amount: 1.0,
                part_tags: vec![],
                part: ProcessSectionTag::Output,
            };
            let output = ProcessPart{
                item: PartItem::Want(0),
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
                item: PartItem::Specific(0),
                amount: 1.0,
                part_tags: vec![],
                part: ProcessSectionTag::Output,
            };
            let output = ProcessPart{
                item: PartItem::Specific(0),
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
                item: PartItem::Want(0),
                amount: 1.0,
                part_tags: vec![],
                part: ProcessSectionTag::Output,
            };
            let output = ProcessPart{
                item: PartItem::Want(0),
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
                item: PartItem::Specific(0),
                amount: 1.0,
                part_tags: vec![],
                part: ProcessSectionTag::Output,
            };
            let output = ProcessPart{
                item: PartItem::Specific(0),
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
                item: PartItem::Want(0),
                amount: 1.0,
                part_tags: vec![],
                part: ProcessSectionTag::Output,
            };
            let output = ProcessPart{
                item: PartItem::Want(0),
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
            data.load_all(&"file_name".to_string()).expect("Failed!");
            let mut test = Process {
                id: 0,
                name: String::from("Test"),
                variant_name: String::from("Variant"),
                description: String::new(),
                minimum_time: 0.0,
                process_parts: vec![],
                process_tags: vec![],
                skill: None,
                skill_minimum: 0.0,
                skill_maximum: 0.0,
                technology_requirement: None,
                tertiary_tech: None,
            };

            // no match (empty set)
            assert!(!test.can_feed_self(&data));
            // match on input-output product
            test.process_parts.push(ProcessPart {
                item: PartItem::Specific(0),
                amount: 1.0,
                part_tags: vec![],
                part: ProcessSectionTag::Input,
            });
            test.process_parts.push(ProcessPart {
                item: PartItem::Specific(0),
                amount: 1.0,
                part_tags: vec![],
                part: ProcessSectionTag::Output,
            });

            assert!(test.can_feed_self(&data));

            // match on capital-output product
            test.process_parts.clear();
            test.process_parts.push(ProcessPart {
                item: PartItem::Specific(0),
                amount: 1.0,
                part_tags: vec![],
                part: ProcessSectionTag::Capital,
            });
            test.process_parts.push(ProcessPart {
                item: PartItem::Specific(0),
                amount: 1.0,
                part_tags: vec![],
                part: ProcessSectionTag::Output,
            });

            assert!(test.can_feed_self(&data));
            // match on input-output want
            test.process_parts.clear();
            test.process_parts.push(ProcessPart {
                item: PartItem::Want(0),
                amount: 1.0,
                part_tags: vec![],
                part: ProcessSectionTag::Input,
            });
            test.process_parts.push(ProcessPart {
                item: PartItem::Want(0),
                amount: 1.0,
                part_tags: vec![],
                part: ProcessSectionTag::Output,
            });

            assert!(test.can_feed_self(&data));
            // don't match on capital-output want
            test.process_parts.clear();
            test.process_parts.push(ProcessPart {
                item: PartItem::Want(0),
                amount: 1.0,
                part_tags: vec![],
                part: ProcessSectionTag::Capital,
            });
            test.process_parts.push(ProcessPart {
                item: PartItem::Want(0),
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
                skill: None,
                skill_minimum: 0.0,
                skill_maximum: 0.0,
                technology_requirement: None,
                tertiary_tech: None,
            };

            let expectation = format!("{}({})", test.name, test.variant_name);

            assert_eq!(test.get_name(), expectation);
        }
    }

    mod skill_group_tests {
        // skipped, shouldn't have much need right now.
    }

    mod skill_tests {
        use crate::objects::{skill::Skill, product::ProductTag};
        // Tests here are kept to a minimum, they should 'just work'.

        #[test]
        pub fn should_return_process_from_process_build_if_labor(){
            let test_skill = Skill::new(0, 
                format!("Name"),
                format!("Desc"),
                1);

            let result = test_skill.build_skill_process(1);

            assert!(result.is_ok());

            let result = result.unwrap();

            assert_eq!(result.id, 1);
            assert_eq!(result.name, String::from("Labor"));
            assert_eq!(result.variant_name, test_skill.name);
            assert_eq!(result.description, test_skill.description);
            assert_eq!(result.minimum_time, 0.0);
            assert!(result.process_parts.iter()
                .any(|x| {
                    x.item.is_specific() && x.item.unwrap() == 0
                }));
            assert!(result.process_parts.iter()
                .any(|x| {
                    x.item.is_specific() && x.item.unwrap() == test_skill.labor
                }));
            assert!(result.process_tags.is_empty());
            assert_eq!(result.skill_minimum, 0.0);
            assert_eq!(result.skill_maximum, 3.0);
            assert!(result.technology_requirement.is_none());
            assert!(result.tertiary_tech.is_none());
        }

        #[test]
        pub fn should_return_err_from_process_build_if_no_labor(){
            let test_skill = Skill::new(0, 
                format!("Name"),
                format!("Desc"),
                0);

        let result = test_skill.build_skill_process(1);

        assert!(result.is_err());
        }

        #[test]
        pub fn should_return_product_if_skill_has_no_labor(){
            let test_skill = Skill::new(0, 
                format!("Name"),
                format!("Desc"),
                0);

            let result = test_skill.build_skill_labor(1);

            assert!(result.is_some());

            let result = result.unwrap();

            assert_eq!(result.name, test_skill.name);
            assert_eq!(result.variant_name, String::new());
            assert_eq!(result.description, test_skill.description);
            assert_eq!(result.unit_name, String::from("Hour(s)"));
            assert!(result.mean_time_to_failure.is_some());
            assert_eq!(result.mean_time_to_failure.unwrap(), 0);
            assert!(result.tags.contains(&ProductTag::Service));
            assert!(result.tech_required.is_none());
        }

        #[test]
        pub fn should_return_none_if_skill_has_labor(){
            let test_skill = Skill::new(0, 
                format!("Name"),
                format!("Desc"),
                2);

            let result = test_skill.build_skill_labor(1);

            assert!(result.is_none());
        }

        #[test]
        pub fn set_relation_correctly() {
            let mut test1 = Skill::new(0,
                String::from("Test"),
                String::from("Test"), 
                0);
            
            let test2 = Skill::new(1,
                String::from("Test1"),
                String::from("Test1"), 
                1);

            assert_eq!(test1.related_skills.len(), 0);

            test1.set_relation(&test2, 0.1);

            assert_eq!(test1.related_skills.len(), 1);
            assert_eq!(match test1.related_skills.get(&1) {
                None => &-1.0,
                Some(val) => val
            },
             &0.1);
            assert_eq!(test2.related_skills.len(), 0);

            let mut test1 = Skill::new(0,
                String::from("Test"),
                String::from("Test"), 
                0);
            
            let mut test2 = Skill::new(1,
                String::from("Test1"),
                String::from("Test1"), 
                1);

            assert_eq!(test1.related_skills.len(), 0);

            test1.set_mutual_relation(&mut test2, 0.1);

            assert_eq!(test1.related_skills.len(), 1);
            assert_eq!(match test1.related_skills.get(&1){
                None => &-1.0,
                Some(val) => val
            }
            , &0.1);
            assert_eq!(test2.related_skills.len(), 1);
            assert_eq!(match test2.related_skills.get(&0) {
                None => &-1.0,
                Some(val) => val
            }, &0.1);
        }
    }

    mod product_tests {
        use crate::objects::{product::{Product, ProductTag}, want::Want, process::{Process, ProcessPart, ProcessSectionTag, PartItem, ProcessTag}};

        pub mod add_to_class_should {
            use std::collections::{HashSet, HashMap};

            use crate::objects::product::Product;


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
                skill: None,
                skill_minimum: 0.0,
                skill_maximum: 3.0,
                technology_requirement: None,
                tertiary_tech: None,
            };

            // err on no relation found
            assert!(test.add_process(&test_process).is_err());

            // ignore the untagged
            let part = ProcessPart { 
                item: PartItem::Specific(0), 
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

    mod want_tests {

        use crate::objects::{want::Want, product::Product, process::{Process, ProcessPart, PartItem, ProcessSectionTag, ProcessTag}};

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
                skill: None,
                skill_minimum: 0.0,
                skill_maximum: 3.0,
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
                skill: None,
                skill_minimum: 0.0,
                skill_maximum: 3.0,
                technology_requirement: None,
                tertiary_tech: None,
            };

            let want_output = ProcessPart{
                item: PartItem::Want(test.id),
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
                skill: None,
                skill_minimum: 0.0,
                skill_maximum: 3.0,
                technology_requirement: None,
                tertiary_tech: None,
            };
            let product_input = ProcessPart{
                item: PartItem::Specific(test_product.id),
                amount: 1.0,
                part_tags: vec![],
                part: ProcessSectionTag::Output,
            };
            let want_output = ProcessPart{
                item: PartItem::Want(test.id),
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
                skill: None,
                skill_minimum: 0.0,
                skill_maximum: 3.0,
                technology_requirement: None,
                tertiary_tech: None,
            };
            let product_input = ProcessPart{
                item: PartItem::Specific(test_product.id),
                amount: 1.0,
                part_tags: vec![],
                part: ProcessSectionTag::Output,
            };
            let want_output = ProcessPart{
                item: PartItem::Want(test.id),
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
}
