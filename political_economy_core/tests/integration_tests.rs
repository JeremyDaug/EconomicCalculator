#[cfg(test)]
mod integration_tests {
    use std::collections::{HashMap, HashSet, VecDeque};

    use political_economy_core::{actor_manager::ActorManager, data_manager::DataManager, demographics::Demographics, objects::{actor_objects::{actor, desire::Desire, firm::{Firm, FirmKind, FirmRank, OrganizationalStructure, OwnershipStructure, ProfitStructure}, firm_job::{AssignmentInfo, FirmJob, WageType}, job::Job, pop::Pop, property::{Property, TieredValue}}, data_objects::{item::Item, process::{Process, ProcessPart, ProcessSectionTag, ProcessTag}, product::Product, want::Want}, demographic_objects::{pop_breakdown_table::{PBRow, PopBreakdownTable}, species::Species}, environmental_objects::market::{Market, MarketHistory}}, runner::Runner};

    #[test]
    pub fn two_disorg_firms_with_market() {
        // 2 pops, 2 firms, 2 processes, 1 market, simple desires for them to trade for.
        let mut data = DataManager::new();
        let mut actor_manager = ActorManager::new();
        let mut demos = Demographics {
            species: HashMap::new(),
            cultures: HashMap::new(),
            ideology: HashMap::new(),
        };

        // bring in default stuff.
        data.required_items();

        // Add in some food want for needs.
        // Food want 100
        let food = Want::new(100, 
        String::from("Food"), 
        String::from("The calories and vitamins needed to satisfy hunger."), 
        0.1).expect("Want Failed To make.");

        data.wants.insert(100, food);

        // add in products for food and wealth production
        // Ambrosia 100
        let ambrosia = Product {
            id: 100,
            name: String::from("Ambrosia"),
            variant_name: String::from(""),
            description: String::from("Ambrosia, a fruit full of everything a man needs to live."),
            unit_name: String::from("Fruit(s)"),
            quality: 0,
            mass: 1.0,
            bulk: 0.0001,
            mean_time_to_failure: Some(9),
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
        // Furs 101
        let furs = Product {
            id: 101,
            name: String::from("Furs"),
            variant_name: String::from(""),
            description: String::from("Furs, used to keep one warm, but grow moldy fast."),
            unit_name: String::from("Fur(s)"),
            quality: 0,
            mass: 1.0,
            bulk: 0.0001,
            mean_time_to_failure: Some(9),
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

        data.products.insert(100, ambrosia);
        data.products.insert(101, furs);

        // ambrosia from land
        let ambrosia_farming = Process {
            id: 100,
            name: String::from("Ambrosia Farming"),
            variant_name: String::from(""),
            description: String::from(""),
            minimum_time: 1.0,
            process_parts: vec![
                ProcessPart { 
                    item: Item::Product(0), // time
                    amount: 1.0, 
                    part_tags: vec![], 
                    part: ProcessSectionTag::Input 
                },
                ProcessPart { 
                    item: Item::Product(3), // land
                    amount: 1.0, 
                    part_tags: vec![], 
                    part: ProcessSectionTag::Capital 
                },
                ProcessPart { 
                    item: Item::Product(100), // Ambrosia
                    amount: 1.0, 
                    part_tags: vec![], 
                    part: ProcessSectionTag::Output 
                }
            ],
            process_tags: vec![],
            technology_requirement: None,
            tertiary_tech: None,
        };
        let ambrosia_consumption = Process {
            id: 101,
            name: String::from("Ambrosia Farming"),
            variant_name: String::from(""),
            description: String::from(""),
            minimum_time: 1.0,
            process_parts: vec![
                ProcessPart { 
                    item: Item::Product(0), // time
                    amount: 0.1, 
                    part_tags: vec![], 
                    part: ProcessSectionTag::Input 
                },
                ProcessPart { 
                    item: Item::Product(100), // Ambrosia
                    amount: 1.0, 
                    part_tags: vec![], 
                    part: ProcessSectionTag::Capital 
                },
                ProcessPart { 
                    item: Item::Want(100), // Food
                    amount: 1.0, 
                    part_tags: vec![], 
                    part: ProcessSectionTag::Output 
                }
            ],
            process_tags: vec![
                ProcessTag::Consumption(100)
            ],
            technology_requirement: None,
            tertiary_tech: None,
        };
        let fur_collecting = Process {
            id: 102,
            name: String::from("Ambrosia Farming"),
            variant_name: String::from(""),
            description: String::from(""),
            minimum_time: 1.0,
            process_parts: vec![
                ProcessPart { 
                    item: Item::Product(0), // time
                    amount: 1.0, 
                    part_tags: vec![], 
                    part: ProcessSectionTag::Input 
                },
                ProcessPart { 
                    item: Item::Product(3), // Land
                    amount: 1.0, 
                    part_tags: vec![], 
                    part: ProcessSectionTag::Capital 
                },
                ProcessPart { 
                    item: Item::Product(101), // Fur
                    amount: 1.0, 
                    part_tags: vec![], 
                    part: ProcessSectionTag::Output 
                }
            ],
            process_tags: vec![],
            technology_requirement: None,
            tertiary_tech: None,
        };
        let fur_use = Process {
            id: 103,
            name: String::from("Ambrosia Farming"),
            variant_name: String::from(""),
            description: String::from(""),
            minimum_time: 1.0,
            process_parts: vec![
                ProcessPart { 
                    item: Item::Product(0), // time
                    amount: 0.1, 
                    part_tags: vec![], 
                    part: ProcessSectionTag::Input 
                },
                ProcessPart { 
                    item: Item::Product(101), // Fur
                    amount: 1.0, 
                    part_tags: vec![], 
                    part: ProcessSectionTag::Capital 
                },
                ProcessPart { 
                    item: Item::Want(1), // Wealth
                    amount: 1.0, 
                    part_tags: vec![], 
                    part: ProcessSectionTag::Output 
                }
            ],
            process_tags: vec![
                ProcessTag::Use(5)
            ],
            technology_requirement: None,
            tertiary_tech: None,
        };

        data.processes.insert(100, ambrosia_farming);
        data.processes.insert(101, ambrosia_consumption);
        data.processes.insert(102, fur_collecting);
        data.processes.insert(103, fur_use);

        data.connect_processes_to_products_and_wants().expect("Failed to connect processes");

        // setup species with it's desires
        let species = Species {
            id: 0,
            name: String::from("Test Species"),
            variant_name: String::from(""),
            desires: vec![
                Desire::new(Item::Want(0), 0, Some(8), 2.0, 0.0, 2, vec![]).unwrap(),
                Desire::new(Item::Want(1), 0, None, 0.2, 0.0, 1, vec![]).unwrap(),
                Desire::new(Item::Want(100), 0, Some(8), 0.2, 0.0, 1, vec![]).unwrap(),
            ],
            tags: vec![],
            relations: vec![],
            base_productivity: 1.0,
            birth_rate: 0.0,
            mortality_rate: 0.0,
        };
        demos.species.insert(species.id, species);

        // Set up pops and firms
        let breakdown_table = PopBreakdownTable { 
            table: vec![
                PBRow::new(0, 10)
            ], 
            total: 10 
        };
        let mut pop1 = Pop::new_pop(0, 0, 0, 0, breakdown_table, &demos);
        pop1.update_desires(&demos);
        pop1.property.add_property(3, 100.0, &data);
        let job1 = Job {
            id: 0,
            name: String::from("Ambrosia Farmer"),
            variant_name: String::from(""),
            skill: 0,
            processes: vec![
                100
            ],
            consistency_modifier: 1.0,
        };
        let mut firm1 = Firm {
            id: 0,
            name: String::from(""),
            sub_name: String::from(""),
            firm_kind: FirmKind::Subsistence,
            firm_rank: FirmRank::Firm,
            ownership_type: OwnershipStructure::SelfEmployed,
            profit_structure: ProfitStructure::Distributed,
            organization_structure: OrganizationalStructure::Disorganized,
            children: vec![],
            parent: None,
            jobs: vec![
                FirmJob { 
                    pop: 0, 
                    pop_size: 1,
                    job: 0, 
                    wage_type: WageType::LossSharing, 
                    wage: HashMap::new(), 
                    accepted_conversions: vec![], 
                    assignments: HashMap::new()
                }
            ],
            prices: HashMap::new(),
            property: HashMap::new(),
            wants: HashMap::new(),
            backlog: VecDeque::new(),
            todays_results: None,
        };
        firm1.jobs.get_mut(0).unwrap().assignments.insert(100, AssignmentInfo {
            iterations: 20.0,
            _progress: 0.0,
        });

        let breakdown_table = PopBreakdownTable { 
            table: vec![
                PBRow::new(0, 10)
            ], 
            total: 10 
        };
        let mut pop2 = Pop::new_pop(1, 1, 1, 0, breakdown_table, &demos);
        pop2.property.add_property(3, 100.0, &data);
        let job2 = Job {
            id: 1,
            name: String::from("Ambrosia Farmer"),
            variant_name: String::from(""),
            skill: 0,
            processes: vec![
                101
            ],
            consistency_modifier: 1.0,
        };
        let mut firm2 = Firm {
            id: 1,
            name: String::from("Furriers"),
            sub_name: String::from(""),
            firm_kind: FirmKind::Subsistence,
            firm_rank: FirmRank::Firm,
            ownership_type: OwnershipStructure::SelfEmployed,
            profit_structure: ProfitStructure::Distributed,
            organization_structure: OrganizationalStructure::Disorganized,
            children: vec![],
            parent: None,
            jobs: vec![
                FirmJob { 
                    pop: 1, 
                    pop_size: 1,
                    job: 1, 
                    wage_type: WageType::LossSharing, 
                    wage: HashMap::new(), 
                    accepted_conversions: vec![], 
                    assignments: HashMap::new()
                }
            ],
            prices: HashMap::new(),
            property: HashMap::new(),
            wants: HashMap::new(),
            backlog: VecDeque::new(),
            todays_results: None,
        };
        firm2.jobs.get_mut(0).unwrap().assignments.insert(101, AssignmentInfo {
            iterations: 20.0,
            _progress: 0.0,
        });

        data.jobs.insert(0, job1);
        data.jobs.insert(1, job2);


        // make market
        let mut market = Market {
            id: 0,
            name: String::from("Test Market"),
            firms: vec![0, 1],
            pops: vec![0, 1],
            institutions: vec![],
            states: vec![],
            territories: vec![],
            neighbors: HashMap::new(),
            resources: HashMap::new(),
            prices: HashMap::new(),
            products_for_sale: HashMap::new(),
            product_demanded: HashMap::new(),
            product_sold: HashMap::new(),
            product_output: HashMap::new(),
            product_exchanged_total: HashMap::new(),
            salability: HashMap::new(),
            want_prices: HashMap::new(),
            want_requests: HashMap::new(),
            want_sources: HashMap::new(),
            state_currencies: vec![],
            previous_day: MarketHistory {
                product_info: HashMap::new(),
                class_info: HashMap::new(),
                want_info: HashMap::new(),
                sale_priority: vec![],
                currencies: vec![],
            },
            seller_weights: HashMap::new(),
            pop_wealth_weight: vec![],
            ongoing_deals: vec![],
        };
        market.prices.insert(0, 1.0);
        market.prices.insert(1, 1.0);
        market.prices.insert(2, 1.0);
        market.prices.insert(3, 1.0);
        market.prices.insert(100, 1.0);
        market.prices.insert(101, 1.0);
        market.salability.insert(0, 0.5);
        market.salability.insert(1, 0.5);
        market.salability.insert(2, 0.5);
        market.salability.insert(3, 0.5);
        market.salability.insert(100, 0.5);
        market.salability.insert(101, 0.5);

        //data.markets.insert(market.id, market);

        let mut actor_manager = ActorManager {
            markets: HashMap::new(),
            pops: HashMap::new(),
            firms: HashMap::new(),
            institutions: HashMap::new(),
            states: HashMap::new(),
        };
        actor_manager.pops.insert(pop1.id, pop1);
        actor_manager.pops.insert(pop2.id, pop2);
        actor_manager.firms.insert(firm1.id, firm1);
        actor_manager.firms.insert(firm2.id, firm2);
        actor_manager.markets.insert(market.id, market);

        let mut runner = Runner {
            data_manager: data,
            demographics: demos,
            map: (),
            actors: actor_manager,
        };

        runner.market_day();
    }
}