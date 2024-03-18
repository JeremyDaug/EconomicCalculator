use political_economy_core::objects::actor_objects::property::TieredValue;
use political_economy_core::constants::TIER_RATIO;

mod tiered_value_tests {
    use super::*;
    mod partial_cmp_should {
        use super::*;

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
        use super::*;

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
        use super::*;

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
        use super::*;

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
        use super::*;

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
