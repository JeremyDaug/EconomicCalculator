
mod pop_breakdown_table_tests {
    use political_economy_core::objects::demographic_objects::pop_breakdown_table::*;

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
