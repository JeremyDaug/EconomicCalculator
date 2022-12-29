pub mod objects;
pub mod data_manager;
pub mod demographics;
pub mod runner;

extern crate lazy_static;

pub fn add(left: usize, right: usize) -> usize {
    left + right
}

#[cfg(test)]
mod tests {
    mod pop_tests {
        use std::collections::HashMap;

        use crate::{objects::{pop::Pop, 
            pop_breakdown_table::{PopBreakdownTable, PBRow},
             desires::Desires, desire::{Desire, DesireItem},
              species::Species, culture::Culture, ideology::Ideology}, 
              demographics::Demographics};

        #[test]
        pub fn update_pop_desires_equivalently() {
            let test = Pop{ 
                id: 0, 
                job: 0, 
                firm: 0, 
                market: 0, 
                skill: 0, 
                lower_skill_level: 0.0, 
                higher_skill_level: 0.0, 
                desires: Desires::new(vec![]), 
                breakdown_table: PopBreakdownTable{ table: vec![], total: 0 }, 
                is_selling: true};

            let species_desire_1 = Desire{ 
                item: DesireItem::Product(0), 
                start: 0, 
                end: Some(4), 
                amount: 1.0, 
                satisfaction: 0.0, 
                reserved: 0.0, 
                step: 1, 
                tags: vec![] };
            let species_desire_2 = Desire{ 
                item: DesireItem::Product(1), 
                start: 9, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0, 
                reserved: 0.0, 
                step: 1, 
                tags: vec![] };

            let culture_desire_1 = Desire{ 
                item: DesireItem::Product(2), 
                start: 10, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0, 
                reserved: 0.0, 
                step: 0, 
                tags: vec![] };
            let culture_desire_2 = Desire{ 
                item: DesireItem::Product(3), 
                start: 15, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0, 
                reserved: 0.0, 
                step: 10, 
                tags: vec![] };

            let ideology_desire_1 = Desire{ 
                item: DesireItem::Product(4), 
                start: 30, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0, 
                reserved: 0.0, 
                step: 0, 
                tags: vec![] };
            let ideology_desire_2 = Desire{ 
                item: DesireItem::Product(5), 
                start: 31, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0, 
                reserved: 0.0, 
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
                "Species".into(),
                "".into(),
                0.0, 0.0,
                1.0,
                vec![ideology_desire_1, ideology_desire_2],
                vec![]).expect("Messed up new.");

            let mut demos = Demographics{ species: HashMap::new(),
                 cultures: HashMap::new(), 
                ideology: HashMap::new() };

            demos.species[&species.id] = species;
            demos.cultures[&culture.id] = culture;
            demos.ideology[&ideology.id] = ideology;

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

            assert_eq!(test.desires.len(), 6);
            // species desire 1 x 20
            let sd1 = test.desires.desires.iter()
            .find(|x| x.item == DesireItem::Product(0));
            // species desire 2 x 20
            let sd1 = test.desires.desires.iter()
            .find(|x| x.item == DesireItem::Product(0));
            // culture desire 1 x 10
            let sd1 = test.desires.desires.iter()
            .find(|x| x.item == DesireItem::Product(0));
            // culture desire 2 x 10
            let sd1 = test.desires.desires.iter()
            .find(|x| x.item == DesireItem::Product(0));
            // ideology desire 1 x 10
            let sd1 = test.desires.desires.iter()
            .find(|x| x.item == DesireItem::Product(0));
            // ideloogy desire 1 x 10
            let sd1 = test.desires.desires.iter()
            .find(|x| x.item == DesireItem::Product(0));

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

    mod desires_tests {
        use crate::objects::{desires::{Desires, DesireCoord}, desire::{Desire, DesireItem}};

        #[test]
        pub fn calculate_barter_value_differenec_correctly() {
            let mut test_desires = vec![];
            test_desires.push(Desire{ // 0,1
                item: DesireItem::Product(0), 
                start: 0, 
                end: Some(1), 
                amount: 1.0, 
                satisfaction: 0.0,
                reserved: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 2,4,6
                item: DesireItem::Product(1), 
                start: 2, 
                end: Some(6), 
                amount: 1.0, 
                satisfaction: 0.0,
                reserved: 0.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 3,6,9, ...
                item: DesireItem::Product(2), 
                start: 3, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                reserved: 0.0,
                step: 3,
                tags: vec![]});
            test_desires.push(Desire{ // 2, 3, 4, ...
                item: DesireItem::Product(3), 
                start: 2, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                reserved: 0.0,
                step: 1,
                tags: vec![]});
            let mut test = Desires::new(test_desires);

            test.desires[0].satisfaction = 2.0;
            test.desires[2].satisfaction = 1.0;
            test.desires[3].satisfaction = 1.0;

            let mut offer = vec![];
            offer.push((1, 1.0));
            offer.push((2, 1.0));
            let mut ask = vec![];
            ask.push((0, 1.0));
            ask.push((3, 1.0));

            let expected_in = 0.9_f64.powf(2.0) + 0.9_f64.powf(6.0);
            let expected_out = 0.9_f64.powf(1.0) + 0.9_f64.powf(2.0);

            let result = test.barter_value_difference(offer, ask);

            assert_eq!(result.in_value, expected_in);
            assert_eq!(result.out_value, expected_out);
        }

        #[test]
        pub fn get_out_barter_value_correctly() {
            let mut test_desires = vec![];
            test_desires.push(Desire{ // 0,1
                item: DesireItem::Product(0), 
                start: 0, 
                end: Some(1), 
                amount: 1.0, 
                satisfaction: 0.0,
                reserved: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 2,4,6
                item: DesireItem::Product(1), 
                start: 2, 
                end: Some(6), 
                amount: 1.0, 
                satisfaction: 0.0,
                reserved: 0.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 3,6,9, ...
                item: DesireItem::Product(0), 
                start: 3, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                reserved: 0.0,
                step: 3,
                tags: vec![]});
            let mut test = Desires::new(test_desires);

            let result = test.out_barter_value(&0, 2.0);
            assert!(result.is_none());

            test.desires[0].satisfaction = 0.5;
            let result = test
                .out_barter_value(&0, 1.0)
                .expect("Value not found!");
            assert_eq!(result.0, 0);
            assert_eq!(result.1, 0.5);

            test.desires[0].satisfaction = 2.0;
            let result = test
                .out_barter_value(&0, 2.0)
                .expect("Value not found!");
            assert_eq!(result.0, 1);
            assert_eq!(result.1, 1.0 + 1.0*(1.0/0.9));

            test.desires[2].satisfaction = 1.0;
            let result = test
                .out_barter_value(&0, 2.0)
                .expect("Value not found!");
            assert_eq!(result.0, 3);
            assert_eq!(result.1, 1.0 +1.0/(0.9_f64.powf(2.0)));

            let result = test
                .out_barter_value(&0, 3.0)
                .expect("Value not found!");
            assert_eq!(result.0, 3);
            assert_eq!(result.1, 1.0 +1.0/(0.9_f64.powf(2.0)) +1.0/(0.9_f64.powf(3.0)));
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
                reserved: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 2,4,6
                item: DesireItem::Product(1), 
                start: 2, 
                end: Some(6), 
                amount: 1.0, 
                satisfaction: 0.0,
                reserved: 0.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 3,6,9, ...
                item: DesireItem::Want(0), 
                start: 3, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                reserved: 0.0,
                step: 3,
                tags: vec![]});
            let mut test = Desires::new(test_desires);

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
                reserved: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 2,4,6
                item: DesireItem::Product(1), 
                start: 2, 
                end: Some(6), 
                amount: 1.0, 
                satisfaction: 0.0,
                reserved: 0.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 3,6,9, ...
                item: DesireItem::Want(0), 
                start: 3, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                reserved: 0.0,
                step: 3,
                tags: vec![]});
            let mut test = Desires::new(test_desires);

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
                reserved: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 0, 2,4,6
                item: DesireItem::Product(1), 
                start: 0, 
                end: Some(6), 
                amount: 1.0, 
                satisfaction: 0.0,
                reserved: 0.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 0,3,6,9, ...
                item: DesireItem::Product(0), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                reserved: 0.0,
                step: 3,
                tags: vec![]});
            let test = Desires::new(test_desires);

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
                reserved: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 0, 2,4,6
                item: DesireItem::Product(1), 
                start: 0, 
                end: Some(6), 
                amount: 1.0, 
                satisfaction: 0.0,
                reserved: 0.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 0,3,6,9, ...
                item: DesireItem::Want(0), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                reserved: 0.0,
                step: 3,
                tags: vec![]});
            let test = Desires::new(test_desires);

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
        pub fn correctly_calculate_in_barter_value() {
            let mut test_desires = vec![];
            test_desires.push(Desire{ // 0,1
                item: DesireItem::Product(0), 
                start: 0, 
                end: Some(1), 
                amount: 1.0, 
                satisfaction: 0.0,
                reserved: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 0, 2,4,6
                item: DesireItem::Product(1), 
                start: 2, 
                end: Some(6), 
                amount: 1.0, 
                satisfaction: 0.0,
                reserved: 0.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 0,3,6,9, ...
                item: DesireItem::Want(0), 
                start: 3, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                reserved: 0.0,
                step: 3,
                tags: vec![]});
            let mut test = Desires::new(test_desires);
            
            test.desires[0].satisfaction = 0.5;
            let results = test.in_barter_value(&0, 0.5);
            assert!(results.is_some());
            let results = results.unwrap();
            assert_eq!(results.0, 0);
            assert_eq!(results.1, 0.5);

            let results = test.in_barter_value(&0, 1.0);
            assert!(results.is_some());
            let results = results.unwrap();
            assert_eq!(results.0, 0);
            assert_eq!(results.1, 0.5+0.5*0.9);

            let results = test.in_barter_value(&0, 2.0);
            assert!(results.is_some());
            let results = results.unwrap();
            assert_eq!(results.0, 0);
            assert_eq!(results.1, 0.5+1.0*0.9);

            // 0 has 0.5 sat for 0 and 1      seeks 1.0 units / lvl
            // 2 has 0 sat for 0, 3, 6, 9 ... seeks 1.5 units / lvl
            test.desires[2].item = DesireItem::Product(0);
            test.desires[2].amount = 1.5;
            let results = test.in_barter_value(&0, 0.5);
            assert!(results.is_some());
            let results = results.unwrap();
            assert_eq!(results.0, 0);
            assert_eq!(results.1, 0.5);

            // 0.5 in 0,0 and 0.5 in 2,0
            let results = test.in_barter_value(&0, 1.0);
            assert!(results.is_some());
            let results = results.unwrap();
            assert_eq!(results.0, 0);
            assert_eq!(results.1, 0.95);

            let results = test.in_barter_value(&0, 2.0);
            assert!(results.is_some());
            let results = results.unwrap();
            assert_eq!(results.0, 0);
            assert_eq!(results.1, 1.4+0.5*(0.9 as f64).powf(3.0));

            let results = test.in_barter_value(&0, 3.0);
            assert!(results.is_some());
            let results = results.unwrap();
            assert_eq!(results.0, 0);
            assert_eq!(results.1, 1.4+1.5*(0.9 as f64).powf(3.0));

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
                reserved: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 2,4,6
                item: DesireItem::Product(1), 
                start: 2, 
                end: Some(6), 
                amount: 1.0, 
                satisfaction: 0.0,
                reserved: 0.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 3,6,9, ...
                item: DesireItem::Want(0), 
                start: 3, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                reserved: 0.0,
                step: 3,
                tags: vec![]});
            let mut test = Desires::new(test_desires);

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
                reserved: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 2,4,6
                item: DesireItem::Product(0), 
                start: 2, 
                end: Some(6), 
                amount: 1.0, 
                satisfaction: 0.0,
                reserved: 0.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 3,6,9, ...
                item: DesireItem::Product(0), 
                start: 3, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                reserved: 0.0,
                step: 3,
                tags: vec![]});
            let mut test = Desires::new(test_desires);

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
                reserved: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 2,4,6
                item: DesireItem::Product(0), 
                start: 2, 
                end: Some(6), 
                amount: 1.0, 
                satisfaction: 0.0,
                reserved: 0.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 3,6,9, ...
                item: DesireItem::Product(1), 
                start: 3, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                reserved: 0.0,
                step: 3,
                tags: vec![]});
            let mut test = Desires::new(test_desires);

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

        #[test]
        pub fn walk_up_tiers_correctly() {
            let mut test_desires = vec![];
            test_desires.push(Desire{ // 0,1
                item: DesireItem::Product(0), 
                start: 0, 
                end: Some(1), 
                amount: 1.0, 
                satisfaction: 0.0,
                reserved: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,4,6
                item: DesireItem::Product(0), 
                start: 0, 
                end: Some(6), 
                amount: 1.0, 
                satisfaction: 0.0,
                reserved: 0.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 0,3,6,9, ...
                item: DesireItem::Product(0), 
                start: 0, 
                end: None, 
                amount: 1.0, 
                satisfaction: 0.0,
                reserved: 0.0,
                step: 3,
                tags: vec![]});
            let mut test = Desires::new(test_desires);

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
                reserved: 0.0,
                step: 1,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,4,6
                item: DesireItem::Product(0), 
                start: 0, 
                end: Some(6), 
                amount: 1.0, 
                satisfaction: 0.0,
                reserved: 0.0,
                step: 2,
                tags: vec![]});
            let mut test = Desires::new(test_desires);

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
    
        #[test]
        pub fn walk_up_tiers_for_item_correctly() {
            let mut test_desires = vec![];
            test_desires.push(Desire{ // 0,2, 4
                item: DesireItem::Product(0), 
                start: 0, 
                end: Some(4), 
                amount: 1.0, 
                satisfaction: 0.0,
                reserved: 0.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,4,6
                item: DesireItem::Product(1), 
                start: 0, 
                end: Some(6), 
                amount: 1.0, 
                satisfaction: 0.0,
                reserved: 0.0,
                step: 2,
                tags: vec![]});
            test_desires.push(Desire{ // 0,2,4,6
                item: DesireItem::Want(0), 
                start: 0, 
                end: Some(6), 
                amount: 1.0, 
                satisfaction: 0.0,
                reserved: 0.0,
                step: 2,
                tags: vec![]});
            let test = Desires::new(test_desires);

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

    mod desire_tests {
        use crate::objects::desire::{Desire, DesireItem};

        #[test]
        pub fn correctly_multiply_desire() {
            let test = Desire{ 
                item: DesireItem::Product(0), 
                start: 0, 
                end: None, 
                amount: 2.0, 
                satisfaction: 0.0,
                reserved: 0.0,
                step: 1,
                tags: vec![]};

            let result = test.create_multiple(5);

            assert_eq!(result.item, test.item);
            assert_eq!(result.start, test.start);
            assert_eq!(result.end, test.end);
            assert_eq!(result.satisfaction, test.satisfaction);
            assert_eq!(result.reserved, test.reserved);
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
                reserved: 0.0,
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
                reserved: 0.0,
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
                reserved: 0.0,
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
                reserved: 0.0,
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
                reserved: 0.0,
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
                reserved: 0.0,
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
                reserved: 0.0,
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
                reserved: 0.0,
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
                reserved: 0.0,
                step: 2,
                tags: vec![]};
            
            assert_eq!(test.satisfaction_at_tier(2).expect("Invalid!"), 0.75);
            assert_eq!(test.satisfaction_at_tier(0).expect("Invalid!"), 1.0);
            assert_eq!(test.satisfaction_at_tier(4).expect("Invalid!"), 0.0);
            assert!(test.satisfaction_at_tier(1).is_err());
        }
    
        #[test]
        pub fn calculate_steps_to_tier_correctly() {
            let test = Desire{ 
                item: DesireItem::Product(0), 
                start: 0, 
                end: None, 
                amount: 2.0, 
                satisfaction: 3.5,
                reserved: 0.0,
                step: 2,
                tags: vec![]};

            assert_eq!(test.steps_to_tier(2).expect("Error!"), 1);
            assert_eq!(test.steps_to_tier(0).expect("Error!"), 0);
            assert_eq!(test.steps_to_tier(4).expect("Error!"), 2);
            assert!(test.steps_to_tier(1).is_err());
        }

        #[test]
        pub fn calculate_steps_on_tier_correctly() {
            let mut test = Desire{ 
                item: DesireItem::Product(0), 
                start: 0, 
                end: Some(10), 
                amount: 2.0, 
                satisfaction: 3.5,
                reserved: 0.0,
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
        }

        #[test]
        pub fn check_if_fully_satisfied() {
            let mut test = Desire{ 
                item: DesireItem::Product(0), 
                start: 0, 
                end: Some(10), 
                amount: 2.0, 
                satisfaction: 3.5,
                reserved: 0.0,
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
                reserved: 0.0,
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
                reserved: 0.0,
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
                reserved: 0.0,
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
        use crate::data_manager::DataManager;

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
        use crate::objects::{process::{Process, ProcessPart, PartItem, ProcessSectionTag}};

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
                item: PartItem::Product(0),
                amount: 1.0,
                part_tags: vec![],
                part: ProcessSectionTag::Input,
            };
            let output = ProcessPart{
                item: PartItem::Product(0),
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
                item: PartItem::Product(0),
                amount: 1.0,
                part_tags: vec![],
                part: ProcessSectionTag::Input,
            };
            let output = ProcessPart{
                item: PartItem::Product(0),
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
                item: PartItem::Product(0),
                amount: 1.0,
                part_tags: vec![],
                part: ProcessSectionTag::Input,
            };
            let output = ProcessPart{
                item: PartItem::Product(0),
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
                item: PartItem::Product(0),
                amount: 1.0,
                part_tags: vec![],
                part: ProcessSectionTag::Capital,
            };
            let output = ProcessPart{
                item: PartItem::Product(0),
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
                item: PartItem::Product(0),
                amount: 1.0,
                part_tags: vec![],
                part: ProcessSectionTag::Capital,
            };
            let output = ProcessPart{
                item: PartItem::Product(0),
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
                item: PartItem::Product(0),
                amount: 1.0,
                part_tags: vec![],
                part: ProcessSectionTag::Capital,
            };
            let output = ProcessPart{
                item: PartItem::Product(0),
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
                item: PartItem::Product(0),
                amount: 1.0,
                part_tags: vec![],
                part: ProcessSectionTag::Output,
            };
            let output = ProcessPart{
                item: PartItem::Product(0),
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
                item: PartItem::Product(0),
                amount: 1.0,
                part_tags: vec![],
                part: ProcessSectionTag::Output,
            };
            let output = ProcessPart{
                item: PartItem::Product(0),
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
                item: PartItem::Product(0),
                amount: 1.0,
                part_tags: vec![],
                part: ProcessSectionTag::Output,
            };
            let output = ProcessPart{
                item: PartItem::Product(0),
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
            assert!(!test.can_feed_self());
            // match on input-output product
            test.process_parts.push(ProcessPart {
                item: PartItem::Product(0),
                amount: 1.0,
                part_tags: vec![],
                part: ProcessSectionTag::Input,
            });
            test.process_parts.push(ProcessPart {
                item: PartItem::Product(0),
                amount: 1.0,
                part_tags: vec![],
                part: ProcessSectionTag::Output,
            });

            assert!(test.can_feed_self());

            // match on capital-output product
            test.process_parts.clear();
            test.process_parts.push(ProcessPart {
                item: PartItem::Product(0),
                amount: 1.0,
                part_tags: vec![],
                part: ProcessSectionTag::Capital,
            });
            test.process_parts.push(ProcessPart {
                item: PartItem::Product(0),
                amount: 1.0,
                part_tags: vec![],
                part: ProcessSectionTag::Output,
            });

            assert!(test.can_feed_self());
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

            assert!(test.can_feed_self());
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

            assert!(!test.can_feed_self());
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
                    x.item.is_product() && x.item.unwrap() == 0
                }));
            assert!(result.process_parts.iter()
                .any(|x| {
                    x.item.is_product() && x.item.unwrap() == test_skill.labor
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
                item: PartItem::Product(0), 
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
                None).unwrap();
            let test_want = Want::new(0, 
                String::from("Test"),
                String::from("Desc"),
                0.5).expect("Nothing");

            let result = test.set_want(&test_want, 1.0);

            assert!(result.is_ok());
            assert!(test.wants.contains_key(&test_want.id()));
            assert!(!test_want.ownership_sources.contains(&test.id()));

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
                None).unwrap();
            let mut test_want = Want::new(0, 
                String::from("Test"),
                String::from("Desc"),
                0.5).expect("Nothing");

            let result = test.connect_want(&mut test_want, 1.0);

            assert!(result.is_ok());
            assert!(test.wants.contains_key(&test_want.id()));
            assert!(test_want.ownership_sources.contains(&test.id()));

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
                None);

            assert!(test.is_some());
            // negative mass err
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
                None);

            assert!(test.is_none());
            // negative volume err
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
                None);

            assert!(test.is_none());
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
                item: PartItem::Want(test.id()),
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
                None
            ).expect("Error, should've returned a product!");
            let tag = ProcessTag::Use(test_product.id());
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
                item: PartItem::Product(test_product.id()),
                amount: 1.0,
                part_tags: vec![],
                part: ProcessSectionTag::Output,
            };
            let want_output = ProcessPart{
                item: PartItem::Want(test.id()),
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
                None
            ).expect("Error, should've returned a product!");
            let tag = ProcessTag::Consumption(test_product.id());
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
                item: PartItem::Product(test_product.id()),
                amount: 1.0,
                part_tags: vec![],
                part: ProcessSectionTag::Output,
            };
            let want_output = ProcessPart{
                item: PartItem::Want(test.id()),
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

            let decay_result = Want::decay_wants(&1000.0, &test);

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
                None
            ).expect("Error, should've returned a product!");

            assert_eq!(test_want.ownership_sources.len(), 0);

            test_want.ownership_sources.insert(test_product.id());

            assert_eq!(test_want.ownership_sources.len(), 1);

            test_want.add_ownership_source(&test_product);

            assert_eq!(test_want.ownership_sources.len(), 1);
        }
    }
}
