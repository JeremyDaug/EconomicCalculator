use political_economy_core::objects::{
    actor_objects::desire::Desire,
    data_objects::item::Item,
};

mod desire_tests {
    use super::*;
    mod missing_satisfaction_should {
        use super::super::*;

        #[test]
        pub fn return_missing_satisfaction_to_reach_tiers() {
            let step_1_no_sat = Desire {
                item: Item::Product(0),
                start: 0,
                end: None,
                amount: 2.0,
                satisfaction: 0.0,
                step: 1,
                tags: vec![],
            };
            let step_5_no_sat = Desire {
                item: Item::Product(0),
                start: 0,
                end: None,
                amount: 2.0,
                satisfaction: 0.0,
                step: 5,
                tags: vec![],
            };
            let step_1_some_sat = Desire {
                item: Item::Product(0),
                start: 0,
                end: None,
                amount: 2.0,
                satisfaction: 15.4,
                step: 1,
                tags: vec![],
            };
            let step_5_some_sat = Desire {
                item: Item::Product(0),
                start: 0,
                end: None,
                amount: 2.0,
                satisfaction: 6.32,
                step: 5,
                tags: vec![],
            };

            let val = step_1_no_sat.missing_satisfaction(5);
            assert_eq!(val, 12.0, "step 1 no satisfaction");
            let val = step_5_no_sat.missing_satisfaction(10);
            assert_eq!(val, 6.0, "Step 5, no Satisfaction");
            let val = step_1_some_sat.missing_satisfaction(10);
            assert_eq!(val, 6.6, "Step 1, some Satisfaction, above sat");
            let val = step_1_some_sat.missing_satisfaction(3);
            assert_eq!(val, 0.0, "Step 1, some Satisfaction, below sat");
            let val = step_5_some_sat.missing_satisfaction(15);
            assert_eq!(val, (8.0 - 6.32), "step 5, some sat, above sat");
            let val = step_5_some_sat.missing_satisfaction(10);
            assert_eq!(val, 0.0, "step 5, some sat, below sat");
        }
    }

    mod steps_in_interval_should {
        use super::super::*;

        #[test]
        pub fn always_return_false_when() {
            let test = Desire{ 
                item: Item::Product(0), 
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
                item: Item::Product(0), 
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
                item: Item::Product(0), 
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
                item: Item::Product(0), 
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
        use super::super::*;

        #[test]
        pub fn return_false_if_after_start() {
            let test = Desire{ 
                item: Item::Product(0), 
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
                item: Item::Product(0), 
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
        use super::super::*;

        #[test]
        pub fn return_false_if_nonstretched_desire_is_before_start() {
            let test = Desire{ 
                item: Item::Product(0), 
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
                item: Item::Product(0), 
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
                item: Item::Product(0), 
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
                item: Item::Product(0), 
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
                item: Item::Product(0), 
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
            item: Item::Product(0), 
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
            item: Item::Product(0), 
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
            item: Item::Product(0), 
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
            item: Item::Product(0), 
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
            item: Item::Product(0), 
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
            item: Item::Product(0), 
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
            item: Item::Product(0), 
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
            item: Item::Product(0), 
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
            item: Item::Product(0), 
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
            item: Item::Product(0), 
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
            item: Item::Product(0), 
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
        use super::super::*;
        
        #[test]
        pub fn calculate_steps_to_tier_correctly_for_infinite_desire() {
            let test = Desire{ 
                item: Item::Product(0), 
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
                item: Item::Product(0), 
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
                item: Item::Product(0), 
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
        use super::super::*;

        #[test]
        pub fn calculate_steps_on_tier_correctly() {
            let mut test = Desire{ 
                item: Item::Product(0), 
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
                item: Item::Product(0), 
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
            item: Item::Product(0), 
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
            item: Item::Product(0), 
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
            item: Item::Product(0), 
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
            item: Item::Product(0), 
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

    pub mod satisfied_at_tier_should {
        use super::super::*;

        #[test]
        pub fn correctly_calculate_if_satisfied_at_particular_tiers() {
            let mut test = Desire{
                item: Item::Want(2),
                start: 0,
                end: Some(10),
                amount: 1.0,
                satisfaction: 3.0,
                step: 2,
                tags: vec![],
            };

            assert!(test.satisfied_at_tier(2));
            assert!(test.satisfied_at_tier(3));
            assert!(test.satisfied_at_tier(4));
            assert!(!test.satisfied_at_tier(5));
            assert!(!test.satisfied_at_tier(6));

            test.satisfaction = 0.0;
            assert!(!test.satisfied_at_tier(0));

            test.satisfaction = 3.5;
            assert!(!test.satisfied_at_tier(6));
        }
    }
}
