use political_economy_core::objects::data_objects::want_info::WantInfo;

#[cfg(test)]
mod want_info_tests {
    mod new_should {
        use super::super::*;

        #[test]
        pub fn correctly_set_values() {
            let test = WantInfo::new(10.0);
            assert_eq!(test.total_current, 10.0);
            assert_eq!(test.day_start, 10.0);
            assert_eq!(test.gained, 0.0);
            assert_eq!(test.expected, 0.0);
            assert_eq!(test.expended, 0.0);
            assert_eq!(test.consumed, 0.0);
        }
    }

    mod new_day_should {
        use super::super::*;

        #[test]
        pub fn correctly_reset_values() {
            let mut test = WantInfo::new(10.0);
            test.gained = 1.0;
            test.expected = 1.0;
            test.expended = 1.0;
            test.consumed = 1.0;
            test.new_day();
            assert_eq!(test.total_current, 10.0);
            assert_eq!(test.day_start, 10.0);
            assert_eq!(test.gained, 0.0);
            assert_eq!(test.expected, 0.0);
            assert_eq!(test.expended, 0.0);
            assert_eq!(test.consumed, 0.0);
        }
    }

    mod consumable_should {
        use super::super::*;

        #[test]
        pub fn return_sum_of_total_current_and_expectations() {
            let mut test = WantInfo::new(10.0);
            test.gained = 1.0;
            test.expected = 1.0;
            test.expended = 1.0;
            test.consumed = 1.0;
            let result = test.consumable();
            assert_eq!(result, 11.0);
        }
    }

    mod expendable_should {
        use super::super::*;

        #[test]
        pub fn return_sum_of_total_and_expectation_excluding_positive_values() {
            let mut test = WantInfo::new(10.0);
            test.gained = 1.0;
            test.expected = 1.0;
            test.expended = 1.0;
            test.consumed = 1.0;
            let result = test.expendable();
            assert_eq!(result, 10.0);
            test.expected = -1.0;
            let result = test.expendable();
            assert_eq!(result, 9.0);
        }
    }

    mod expend_should {
        use super::super::*;

        #[test]
        pub fn correctly_move_value_from_total_current_to_expended() {
            let mut test = WantInfo::new(10.0);
            test.gained = 1.0;
            test.expected = 1.0;
            test.expended = 1.0;
            test.consumed = 1.0;
            test.expend(5.0);
            assert_eq!(test.total_current, 5.0);
            assert_eq!(test.day_start, 10.0);
            assert_eq!(test.gained, 1.0);
            assert_eq!(test.expected, 1.0);
            assert_eq!(test.expended, 6.0);
            assert_eq!(test.consumed, 1.0);
        }
    }

    mod realize_all_should{
        use super::super::*;

        #[test]
        pub fn correctly_add_positive_expectations() {
            let mut test = WantInfo::new(10.0);
            test.gained = 1.0;
            test.expected = 10.0;
            test.expended = 1.0;
            test.consumed = 1.0;
            test.realize_all();
            assert_eq!(test.total_current, 20.0);
            assert_eq!(test.day_start, 10.0);
            assert_eq!(test.gained, 1.0);
            assert_eq!(test.expected, 0.0);
            assert_eq!(test.expended, 1.0);
            assert_eq!(test.consumed, 1.0);
        }
    }

    mod realize_should {
        use super::super::*;

        #[test]
        pub fn correctly_shift_value_from_expectations_to_total_current() {
            let mut test = WantInfo::new(10.0);
            test.gained = 1.0;
            test.expected = 10.0;
            test.expended = 1.0;
            test.consumed = 1.0;
            test.realize(5.0);
            assert_eq!(test.total_current, 15.0);
            assert_eq!(test.expected, 5.0);
            assert_eq!(test.gained, 1.0);
            assert_eq!(test.expended, 1.0);
            assert_eq!(test.consumed, 1.0);
            assert_eq!(test.day_start, 10.0);
        }

        #[test]
        #[should_panic]
        pub fn debug_check_sign_mismatch(){
            let mut test = WantInfo::new(10.0);
            test.gained = 1.0;
            test.expected = 10.0;
            test.expended = 1.0;
            test.consumed = 1.0;
            test.realize(-5.0);
        }

        #[test]
        #[should_panic]
        pub fn debug_check_value_too_high(){
            let mut test = WantInfo::new(10.0);
            test.gained = 1.0;
            test.expected = 10.0;
            test.expended = 1.0;
            test.consumed = 1.0;
            test.realize(15.0);
        }
    }

    mod consume_should {
        use super::super::*;

        #[test]
        pub fn consume_from_total_current_first() {
            let mut test = WantInfo::new(10.0);
            test.gained = 1.0;
            test.expected = 10.0;
            test.expended = 1.0;
            test.consumed = 1.0;
            test.consume(5.0);
            assert_eq!(test.total_current, 5.0);
            assert_eq!(test.expected, 10.0);
            assert_eq!(test.gained, 1.0);
            assert_eq!(test.expended, 1.0);
            assert_eq!(test.consumed, 6.0);
            assert_eq!(test.day_start, 10.0);
        }

        #[test]
        pub fn take_from_total_current_then_from_expectations() {
            let mut test = WantInfo::new(10.0);
            test.gained = 1.0;
            test.expected = 10.0;
            test.expended = 1.0;
            test.consumed = 1.0;
            test.consume(15.0);
            assert_eq!(test.total_current, 0.0);
            assert_eq!(test.expected, 5.0);
            assert_eq!(test.gained, 1.0);
            assert_eq!(test.expended, 1.0);
            assert_eq!(test.consumed, 16.0);
            assert_eq!(test.day_start, 10.0);
        }

        #[test]
        #[should_panic]
        pub fn debug_check_value_greater_than_consumeable() {
            let mut test = WantInfo::new(10.0);
            test.gained = 1.0;
            test.expected = 10.0;
            test.expended = 1.0;
            test.consumed = 1.0;
            test.consume(25.0);
        }

        #[test]
        #[should_panic]
        pub fn debug_check_value_not_negative() {
            let mut test = WantInfo::new(10.0);
            test.gained = 1.0;
            test.expected = 10.0;
            test.expended = 1.0;
            test.consumed = 1.0;
            test.consume(-1.0);
        }
    }
}
