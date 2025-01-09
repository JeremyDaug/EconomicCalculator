

mod firm_tests {
    mod do_plan_should {
        
    }

    mod decay_goods_should {
        use std::collections::{HashMap, HashSet, VecDeque};

        use political_economy_core::{data_manager::DataManager, objects::{actor_objects::{firm::{Firm, FirmKind, FirmRank, OrganizationalStructure, OwnershipStructure, ProfitStructure}, firm_property_info::FirmPropertyInfo}, data_objects::{item::Item, process::{Process, ProcessPart, ProcessSectionTag, ProcessTag}, product::Product, want::Want}}};

        #[test]
        pub fn not_decay_fresh_failure_products() {
            let mut test = Firm {
                id: 0,
                name: "test".to_string(),
                sub_name: "test".to_string(),
                firm_kind: FirmKind::Subsistence,
                firm_rank: FirmRank::Firm,
                ownership_type: OwnershipStructure::SelfEmployed,
                profit_structure: ProfitStructure::PrivatelyOwned,
                organization_structure: OrganizationalStructure::Disorganized,
                children: vec![],
                parent: None,
                jobs: vec![],
                prices: HashMap::new(),
                property: HashMap::new(),
                wants: HashMap::new(),
                backlog: VecDeque::new(),
                todays_results: None,
            };
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
            test.property.insert(0, FirmPropertyInfo::new().with_total_property(10.0));
            test.property.insert(1, FirmPropertyInfo::new().with_total_property(10.0));
            test.property.insert(2, FirmPropertyInfo::new().with_total_property(10.0));
            test.property.insert(3, FirmPropertyInfo::new().with_total_property(10.0));
            for (_id, propinfo) in test.property.iter_mut() {
                propinfo.expend_capital(10.0);
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
            let mut test = Firm {
                id: 0,
                name: "test".to_string(),
                sub_name: "test".to_string(),
                firm_kind: FirmKind::Subsistence,
                firm_rank: FirmRank::Firm,
                ownership_type: OwnershipStructure::SelfEmployed,
                profit_structure: ProfitStructure::PrivatelyOwned,
                organization_structure: OrganizationalStructure::Disorganized,
                children: vec![],
                parent: None,
                jobs: vec![],
                prices: HashMap::new(),
                property: HashMap::new(),
                wants: HashMap::new(),
                backlog: VecDeque::new(),
                todays_results: None,
            };
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
            test.property.insert(0, FirmPropertyInfo::new().with_total_property(10.0));
            test.property.insert(1, FirmPropertyInfo::new().with_total_property(10.0));
            test.property.insert(2, FirmPropertyInfo::new().with_total_property(10.0));
            test.property.insert(3, FirmPropertyInfo::new().with_total_property(10.0));
            for (_id, propinfo) in test.property.iter_mut() {
                propinfo.expend_capital(10.0);
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
            assert_eq!(test.property[&0].produced, 0.0);
            assert_eq!(test.property[&1].produced, 0.0);
            assert_eq!(test.property[&2].produced, 0.0);
            assert_eq!(test.property[&3].produced, 10.0);
            
            assert_eq!(test.wants[&0], 10.0);
        }

        #[test]
        pub fn decay_goods_correctly_for_all_failure_types() {
            let mut test = Firm {
                id: 0,
                name: "test".to_string(),
                sub_name: "test".to_string(),
                firm_kind: FirmKind::Subsistence,
                firm_rank: FirmRank::Firm,
                ownership_type: OwnershipStructure::SelfEmployed,
                profit_structure: ProfitStructure::PrivatelyOwned,
                organization_structure: OrganizationalStructure::Disorganized,
                children: vec![],
                parent: None,
                jobs: vec![],
                prices: HashMap::new(),
                property: HashMap::new(),
                wants: HashMap::new(),
                backlog: VecDeque::new(),
                todays_results: None,
            };
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
            test.property.insert(0, FirmPropertyInfo::new().with_total_property(10.0));
            test.property.insert(1, FirmPropertyInfo::new().with_total_property(10.0));
            test.property.insert(2, FirmPropertyInfo::new().with_total_property(10.0));
            test.property.insert(3, FirmPropertyInfo::new().with_total_property(10.0));
            test.wants.insert(0, 10.0);
            test.wants.insert(1, 10.0);
            test.wants.insert(2, 10.0);
            test.wants.insert(3, 10.0);
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
            assert_eq!(test.property[&0].produced, 0.0);
            assert_eq!(test.property[&1].produced, 0.0);
            assert_eq!(test.property[&2].produced, 0.0);
            assert_eq!(test.property[&3].produced, 10.0);
            
            assert_eq!(test.wants[&0], 20.0);
            assert_eq!(test.wants[&1], 5.0);
            assert_eq!(test.wants[&2], 7.5);
            assert_eq!(test.wants[&3], 0.0);
        }
    }

}