use political_economy_core::objects::actor_objects::property_info::PropertyInfo;
mod property_info_tests {
    use super::*;
    /*
    #[test]
    pub fn safe_remove_should_remove_from_unreserved_and_reserved_only() {
        let mut test = PropertyInfo::new(100.0);
        test.shift_to_class_reserve(25.0);
        test.shift_to_reserved(25.0);
        assert!(test.total_property == 100.0);
        assert!(test.unreserved == 50.0);
        //assert!(test.reserved == 25.0);
        assert!(test.want_reserve == 0.0);
        assert!(test.class_reserve == 25.0);
        assert!(test.product_reserve == 0.0);

        // expend from unreserved
        test.safe_remove(25.0);
        assert!(test.total_property == 75.0);
        assert!(test.unreserved == 25.0);
        //assert!(test.reserved == 25.0);
        assert!(test.want_reserve == 0.0);
        assert!(test.class_reserve == 25.0);
        assert!(test.product_reserve == 0.0);

        // expend from unreserved
        test.safe_remove(25.0);
        assert!(test.total_property == 50.0);
        assert!(test.unreserved == 0.0);
        //assert!(test.reserved == 25.0);
        assert!(test.want_reserve == 0.0);
        assert!(test.class_reserve == 25.0);
        assert!(test.product_reserve == 0.0);

        // expend from reserved
        test.safe_remove(25.0);
        assert!(test.total_property == 25.0);
        assert!(test.unreserved == 0.0);
        //assert!(test.reserved == 0.0);
        assert!(test.want_reserve == 0.0);
        assert!(test.class_reserve == 25.0);
        assert!(test.product_reserve == 0.0);

        // don't expend
        test.safe_remove(25.0);
        assert!(test.total_property == 25.0);
        assert!(test.unreserved == 0.0);
        //assert!(test.reserved == 0.0);
        assert!(test.want_reserve == 0.0);
        assert!(test.class_reserve == 25.0);
        assert!(test.product_reserve == 0.0);
    } */

    #[test]
    pub fn expend_should_remove_from_unreserved_only() {
        let mut test = PropertyInfo::new(100.0);
        test.shift_to_class_reserve(25.0);
        //test.shift_to_reserved(25.0);
        assert!(test.total_property == 100.0);
        assert!(test.unreserved == 75.0);
        //assert!(test.reserved == 25.0);
        assert!(test.want_reserve == 0.0);
        assert!(test.class_reserve == 25.0);
        assert!(test.product_reserve == 0.0);

        // expend from unreserved
        test.expend(25.0);
        assert!(test.total_property == 75.0);
        assert!(test.unreserved == 50.0);
        //assert!(test.reserved == 25.0);
        assert!(test.want_reserve == 0.0);
        assert!(test.class_reserve == 25.0);
        assert!(test.product_reserve == 0.0);

        // expend from unreserved
        test.expend(25.0);
        assert!(test.total_property == 50.0);
        assert!(test.unreserved == 25.0);
        //assert!(test.reserved == 25.0);
        assert!(test.want_reserve == 0.0);
        assert!(test.class_reserve == 25.0);
        assert!(test.product_reserve == 0.0);

        // don't expend
        test.expend(25.0);
        assert!(test.total_property == 25.0);
        assert!(test.unreserved == 0.0);
        //assert!(test.reserved == 25.0);
        assert!(test.want_reserve == 0.0);
        assert!(test.class_reserve == 25.0);
        assert!(test.product_reserve == 0.0);
    }

    mod available_should {
        use super::super::*;

        #[test]
        pub fn return_sum_of_unreserved_and_reserved() {
            // get start to check
            let mut test = PropertyInfo::new(100.0);
            assert!(test.total_property == 100.0);
            assert!(test.unreserved == 100.0);
            //assert!(test.reserved == 0.0);
            assert!(test.want_reserve == 0.0);
            assert!(test.class_reserve == 0.0);
            assert!(test.product_reserve == 0.0);
            // check it's correct
            assert!(test.available() == 100.0);

            // shift to reserve
            // test.shift_to_reserved(50.0);
            // assert!(test.available() == 100.0);

            // shift to Product
            test.shift_to_class_reserve(50.0);
            assert!(test.available() == 50.0);
        }
    }

    mod remove_should {
        use super::super::*;

        #[test]
        pub fn remove_from_pools_correctly() {
            let mut test = PropertyInfo::new(100.0);
            // shift into each reserve
            test.shift_to_class_reserve(25.0);
            test.shift_to_specific_reserve(25.0);
            test.shift_to_want_reserve(25.0);
            //test.shift_to_reserved(25.0);
            assert!(test.total_property == 100.0);
            assert!(test.unreserved == 75.0);
            //assert!(test.reserved == 25.0);
            assert!(test.want_reserve == 25.0);
            assert!(test.class_reserve == 25.0);
            assert!(test.product_reserve == 25.0);
            // remove just from unreserved
            test.remove(40.0);
            assert!(test.total_property == 60.0);
            assert!(test.unreserved == 35.0);
            //assert!(test.reserved == 25.0);
            assert!(test.want_reserve == 25.0);
            assert!(test.class_reserve == 25.0);
            assert!(test.product_reserve == 25.0);
            // remove from unreserved and reserve
            test.remove(30.0);
            assert!(test.total_property == 30.0);
            assert!(test.unreserved == 5.0);
            //assert!(test.reserved == 5.0);
            assert!(test.want_reserve == 25.0);
            assert!(test.class_reserve == 25.0);
            assert!(test.product_reserve == 25.0);
            // remove from reserve and spec_reserves
            test.remove(30.0);
            assert!(test.total_property == 00.0);
            assert!(test.unreserved == 00.0);
            //assert!(test.reserved == 0.0);
            assert!(test.want_reserve == 0.0);
            assert!(test.class_reserve == 0.0);
            assert!(test.product_reserve == 0.0);
            // remove from all pools at once
            test.add_property(100.0);
            test.shift_to_class_reserve(25.0);
            test.shift_to_specific_reserve(25.0);
            test.shift_to_want_reserve(25.0);
            //test.shift_to_reserved(25.0);
            assert!(test.total_property == 100.0);
            assert!(test.unreserved == 75.0);
            //assert!(test.reserved == 25.0);
            assert!(test.want_reserve == 25.0);
            assert!(test.class_reserve == 25.0);
            assert!(test.product_reserve == 25.0);
            test.remove(100.0);
            assert!(test.total_property == 00.0);
            assert!(test.unreserved == 00.0);
            //assert!(test.reserved == 0.0);
            assert!(test.want_reserve == 0.0);
            assert!(test.class_reserve == 0.0);
            assert!(test.product_reserve == 0.0);
        }

        #[test]
        pub fn call_add_when_value_is_negative() {
            let mut test = PropertyInfo::new(10.0);

            assert!(test.total_property == 10.0);
            assert!(test.unreserved == 10.0);
            //assert!(test.reserved == 0.0);
            assert!(test.want_reserve == 0.0);
            assert!(test.class_reserve == 0.0);
            assert!(test.product_reserve == 0.0);

            test.remove(-100.0);
            assert!(test.total_property == 110.0);
            assert!(test.unreserved == 110.0);
            //assert!(test.reserved == 0.0);
            assert!(test.want_reserve == 0.0);
            assert!(test.class_reserve == 0.0);
            assert!(test.product_reserve == 0.0);
        }
    }

    mod add_property_should {
        use super::super::*;

        #[test]
        pub fn add_to_both_total_and_reserve() {
            let mut test = PropertyInfo::new(10.0);

            assert!(test.total_property == 10.0);
            assert!(test.unreserved == 10.0);
            //assert!(test.reserved == 0.0);
            assert!(test.want_reserve == 0.0);
            assert!(test.class_reserve == 0.0);
            assert!(test.product_reserve == 0.0);

            test.add_property(100.0);
            assert!(test.total_property == 110.0);
            assert!(test.unreserved == 110.0);
            //assert!(test.reserved == 0.0);
            assert!(test.want_reserve == 0.0);
            assert!(test.class_reserve == 0.0);
            assert!(test.product_reserve == 0.0);
        }

        #[test]
        pub fn call_remove_when_value_is_negative() {
            let mut test = PropertyInfo::new(100.0);
            // shift into each reserve
            test.shift_to_class_reserve(25.0);
            test.shift_to_specific_reserve(25.0);
            test.shift_to_want_reserve(25.0);
            // test.shift_to_reserved(25.0);
            assert!(test.total_property == 100.0);
            assert!(test.unreserved == 75.0);
            //assert!(test.reserved == 25.0);
            assert!(test.want_reserve == 25.0);
            assert!(test.class_reserve == 25.0);
            assert!(test.product_reserve == 25.0);
            // remove just from unreserved
            test.add_property(-40.0);
            assert!(test.total_property == 60.0);
            assert!(test.unreserved == 35.0);
            //assert!(test.reserved == 25.0);
            assert!(test.want_reserve == 25.0);
            assert!(test.class_reserve == 25.0);
            assert!(test.product_reserve == 25.0);
            // remove from unreserved and reserve
            test.add_property(-30.0);
            assert!(test.total_property == 30.0);
            assert!(test.unreserved == 5.0);
            //assert!(test.reserved == 5.0);
            assert!(test.want_reserve == 25.0);
            assert!(test.class_reserve == 25.0);
            assert!(test.product_reserve == 25.0);
            // remove from reserve and spec_reserves
            test.add_property(-30.0);
            assert!(test.total_property == 00.0);
            assert!(test.unreserved == 00.0);
            //assert!(test.reserved == 0.0);
            assert!(test.want_reserve == 0.0);
            assert!(test.class_reserve == 0.0);
            assert!(test.product_reserve == 0.0);
            // remove from all pools at once
            test.add_property(100.0);
            test.shift_to_class_reserve(25.0);
            test.shift_to_specific_reserve(25.0);
            test.shift_to_want_reserve(25.0);
            //test.shift_to_reserved(25.0);
            assert!(test.total_property == 100.0);
            assert!(test.unreserved == 75.0);
            //assert!(test.reserved == 25.0);
            assert!(test.want_reserve == 25.0);
            assert!(test.class_reserve == 25.0);
            assert!(test.product_reserve == 25.0);
            test.add_property(-100.0);
            assert!(test.total_property == 00.0);
            assert!(test.unreserved == 00.0);
            //assert!(test.reserved == 0.0);
            assert!(test.want_reserve == 0.0);
            assert!(test.class_reserve == 0.0);
            assert!(test.product_reserve == 0.0);
        }
    }

    mod reset_reserves_should {
        use super::super::*;

        #[test]
        pub fn reset_reserves_correctly() {
            let mut test = PropertyInfo::new(100.0);

            test.shift_to_class_reserve(50.0);
            // test.shift_to_reserved(25.0);
            assert!(test.total_property == 100.0);
            assert!(test.unreserved == 50.0);
            //assert!(test.reserved == 25.0);
            assert!(test.want_reserve == 0.0);
            assert!(test.class_reserve == 50.0);
            assert!(test.product_reserve == 0.0);

            test.reset_reserves();
            assert!(test.total_property == 100.0);
            assert!(test.unreserved == 100.0);
            //assert!(test.reserved == 0.0);
            assert!(test.want_reserve == 0.0);
            assert!(test.class_reserve == 0.0);
            assert!(test.product_reserve == 0.0);
        }
    }

    mod shift_tests {
        use super::super::*;

        /*#[test]
        pub fn shift_to_reserved_correctly() {
            let mut test = PropertyInfo::new(10.0);

            // test.shift_to_reserved(5.0);
            assert!(test.total_property == 10.0);
            assert!(test.unreserved == 5.0);
            //assert!(test.reserved == 5.0);
            assert!(test.product_reserve == 0.0);
            assert!(test.class_reserve == 0.0);
            assert!(test.want_reserve == 0.0);

            // test.shift_to_reserved(10.0);
            assert!(test.total_property == 10.0);
            assert!(test.unreserved == 0.0);
            //assert!(test.reserved == 10.0);
            assert!(test.product_reserve == 0.0);
            assert!(test.class_reserve == 0.0);
            assert!(test.want_reserve == 0.0);
        }*/

        #[test]
        pub fn get_max_special_reserve_correctly() {
            let mut test = PropertyInfo::new(10.0);
            let result = test.max_spec_reserve();
            assert!(result == 0.0);

            test.product_reserve = 1.0;
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
            // test.shift_to_reserved(5.0);
            test.total_property += 5.0;
            test.class_reserve += 5.0;

            // check that it reserves from overlap first.
            test.shift_to_specific_reserve(2.5);
            assert!(test.total_property == 15.0);
            assert!(test.unreserved == 10.0);
            //assert!(test.reserved == 5.0);
            assert!(test.product_reserve == 2.5);
            assert!(test.class_reserve == 5.0);
            assert!(test.want_reserve == 0.0);

            // check that it takes from overlap and reserved
            test.shift_to_specific_reserve(5.0);
            assert!(test.total_property == 15.0);
            assert!(test.unreserved == 7.5);
            //assert!(test.reserved == 2.5);
            assert!(test.product_reserve == 7.5);
            assert!(test.class_reserve == 5.0);
            assert!(test.want_reserve == 0.0);

            // check that it takes from reserve and unreserved
            test.shift_to_specific_reserve(5.0);
            assert!(test.total_property == 15.0);
            assert!(test.unreserved == 2.5);
            //assert!(test.reserved == 0.0);
            assert!(test.product_reserve == 12.5);
            assert!(test.class_reserve == 5.0);
            assert!(test.want_reserve == 0.0);

            // check that it takes from unreserved and returns excess
            test.shift_to_specific_reserve(5.0);
            assert!(test.total_property == 15.0);
            assert!(test.unreserved == 0.0);
            //assert!(test.reserved == 0.0);
            assert!(test.product_reserve == 15.0);
            assert!(test.class_reserve == 5.0);
            assert!(test.want_reserve == 0.0);
        }

        #[test]
        pub fn shift_to_class_reserve_correctly() {
            let mut test = PropertyInfo::new(10.0);
            // test.shift_to_reserved(5.0);
            test.total_property += 5.0;
            test.product_reserve += 5.0;

            // check that it reserves from overlap first.
            test.shift_to_class_reserve(2.5);
            assert!(test.total_property == 15.0);
            assert!(test.unreserved == 10.0);
            //assert!(test.reserved == 5.0);
            assert!(test.product_reserve == 5.0);
            assert!(test.class_reserve == 2.5);
            assert!(test.want_reserve == 0.0);

            // check that it takes from overlap and reserved
            test.shift_to_class_reserve(5.0);
            assert!(test.total_property == 15.0);
            assert!(test.unreserved == 7.5);
            //assert!(test.reserved == 2.5);
            assert!(test.product_reserve == 5.0);
            assert!(test.class_reserve == 7.5);
            assert!(test.want_reserve == 0.0);

            // check that it takes from reserve and unreserved
            test.shift_to_class_reserve(5.0);
            assert!(test.total_property == 15.0);
            assert!(test.unreserved == 2.5);
            //assert!(test.reserved == 0.0);
            assert!(test.product_reserve == 5.0);
            assert!(test.class_reserve == 12.5);
            assert!(test.want_reserve == 0.0);

            // check that it takes from unreserved and returns excess
            test.shift_to_class_reserve(5.0);
            assert!(test.total_property == 15.0);
            assert!(test.unreserved == 0.0);
            //assert!(test.reserved == 0.0);
            assert!(test.product_reserve == 5.0);
            assert!(test.class_reserve == 15.0);
            assert!(test.want_reserve == 0.0);
        }

        #[test]
        pub fn shift_to_want_reserve_correctly() {
            let mut test = PropertyInfo::new(10.0);
            // test.shift_to_reserved(5.0);
            test.total_property += 5.0;
            test.product_reserve += 5.0;

            // check that it reserves from overlap first.
            test.shift_to_want_reserve(2.5);
            assert!(test.total_property == 15.0);
            assert!(test.unreserved == 10.0);
            //assert!(test.reserved == 5.0);
            assert!(test.product_reserve == 5.0);
            assert!(test.class_reserve == 0.0);
            assert!(test.want_reserve == 2.5);

            // check that it takes from overlap and reserved
            test.shift_to_want_reserve(5.0);
            assert!(test.total_property == 15.0);
            assert!(test.unreserved == 7.5);
            //assert!(test.reserved == 2.5);
            assert!(test.product_reserve == 5.0);
            assert!(test.class_reserve == 0.0);
            assert!(test.want_reserve == 7.5);

            // check that it takes from reserve and unreserved
            test.shift_to_want_reserve(5.0);
            assert!(test.total_property == 15.0);
            assert!(test.unreserved == 2.5);
            //assert!(test.reserved == 0.0);
            assert!(test.product_reserve == 5.0);
            assert!(test.class_reserve == 0.0);
            assert!(test.want_reserve == 12.5);

            // check that it takes from unreserved and returns excess
            test.shift_to_want_reserve(5.0);
            assert!(test.total_property == 15.0);
            assert!(test.unreserved == 0.0);
            //assert!(test.reserved == 0.0);
            assert!(test.product_reserve == 5.0);
            assert!(test.class_reserve == 0.0);
            assert!(test.want_reserve == 15.0);
        }
    }
}
