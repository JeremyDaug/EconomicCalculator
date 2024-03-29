use core::panic;
use std::collections::{HashMap, HashSet};

use itertools::Itertools;

use crate::objects::{
    demographic_objects::culture::Culture, 
    demographic_objects::species::Species, 
    actor_objects::{
        firm::Firm,
        job::Job, 
        pop::Pop, 
    },
    data_objects::{
        item::Item, 
        process::{Process, 
            ProcessPart, 
            ProcessPartTag, 
            ProcessSectionTag, 
            ProcessTag}, 
        product::{Product, ProductTag}, 
        process_node::ProcessNode, 
        technology::Technology, 
        technology_family::TechnologyFamily, 
        want::Want
    },
    environmental_objects::market::Market, 
};
use crate::constants::{
    BRAINSTORMING_TECH_ID,
    DISCERNMENT_PRODUCT_ID, 
    LAND_PRODUCT_ID, 
    RESTING_PROC_ID, 
    REST_WANT_ID, 
    SHOPPING_TIME_PROC_ID, 
    SHOPPING_TIME_PRODUCT_ID, 
    TIME_PRODUCT_ID, 
    WEALTH_WANT_ID
};

/// The DataManager is the main manager for our simulation
/// It contains all of the data needed for the simulation in active memory, available for
/// use as needed.
/// 
/// Items are stored and retrieved via their IDs.
/// IDs for items should be fixed in place from loading onwards.
/// 
/// # Required Items
/// 
/// Currently, there are a handful of items which are considered 'Required' by the system.
/// These are items which should always be there and will effectively always have some use.
/// 
/// - Required Wants
///   - ID 0: Rest - Passive Leisure time like sleep.
///   - ID 1: Wealth - A generic want representing "stuff".
///   - TODO Items
///     - Space - How much space is available absolutely.
///     - Free Space - How much unused space they have available, not used in storage.
/// - Required Products:
///   - ID 0: Time (hr) (Produces 1 rest for owning it, made by pops at day start, 
///                         refreshed every day)
///   - ID 1: Shopping Time (Used to shop)
///   - TODO Items
///     - Land (abstract)
///     - Land (Wasteland)
///     - Land (Marignal Land)
///     - Land (Scrub Land)
///     - Land (Quality Land)
///     - Land (Fertile Land)
///     - Land (Very Fertile Land)
///     - Nothing (void item, may not be needed)
/// - Required Processes:
///   - ID 0: Shopping (Time -> Shopping Time)
///   - ID 1: Sleep (Time -> Rest)
/// - Required Tech:
///   - TODO Items
///     - Brainstorming (origin tech)
#[derive(Debug)]
pub struct DataManager {
    // Sets are an organizational and loading tool, once loaded, only the sets are needed.
    pub sets: Vec<String>,
    // These should be fixed during common running, and should be immutable passed around
    // the threads.
    pub wants: HashMap<usize, Want>,
    pub technology: HashMap<usize, Technology>,
    pub technology_families: HashMap<usize, TechnologyFamily>,
    pub products: HashMap<usize, Product>,
    /// The various products, organized by the key, the abstract or generic product,
    /// and it's variants, the values in the attached vector.
    pub product_classes: HashMap<usize, Vec<usize>>,
    // TODO Add in abstract to real product connections here.
    //pub skill_groups: HashMap<usize, SkillGroup>,
    //pub skills: HashMap<usize, Skill>,
    pub processes: HashMap<usize, Process>,
    // TODO Consider combining this with Processes (would still need to be set after loading)
    pub process_nodes: HashMap<usize, ProcessNode>,
    pub jobs: HashMap<usize, Job>,

    // These are mutable, but only record changes as noted, typically demographic data.
    // These should be their own thread (or more accurately grouped together in their
    // own thread.) These are updated only when pops change, and merely record the changes
    // the don't act or send messages. These should have a RWLock on them (only their thread writes).
    pub species: HashMap<usize, Species>,
    pub cultures: HashMap<usize, Culture>,

    // These structs are semi-mutable, they can be updated while the rest of the
    // system is running, race conditions are expected, but they are light on actions.
    pub territories: HashMap<usize, Species>,
    pub markets: HashMap<usize, Market>,

    // These structs are totally mutable, and should expect lots of messages passing 
    // between them.
    pub pops: HashMap<usize, Pop>,
    pub firms: HashMap<usize, Firm>,
    // institutions
    // states

    // id creation data
    want_id: usize,
    tech_id: usize,
    tech_fam_id: usize,
    product_id: usize,
    process_id: usize,
    job_id: usize,
    species_id: usize,
    culture_id: usize,
    pop_id: usize,
    territory_id: usize,
    market_id: usize,
    firm_id: usize,
    _institution_id: usize,
    _state_id: usize,
}

impl DataManager {
    pub fn new() -> Self {
        Self { 
            wants: HashMap::new(),
            technology: HashMap::new(), 
            technology_families: HashMap::new(), 
            products: HashMap::new(), 
            product_classes: HashMap::new(),
            processes: HashMap::new(),
            process_nodes: HashMap::new(),
            jobs: HashMap::new(),
            species: HashMap::new(),
            cultures: HashMap::new(),
            pops: HashMap::new(),
            territories: HashMap::new(),
            markets: HashMap::new(),
            firms: HashMap::new(),
            sets: Vec::new(),
            want_id: 0,
            tech_id: 0,
            tech_fam_id: 0,
            product_id: 0,
            process_id: 0,
            job_id: 0,
            species_id: 0,
            culture_id: 0,
            pop_id: 0,
            territory_id: 0,
            market_id: 0,
            firm_id: 0, 
            _institution_id: 0,
            _state_id: 0
        }
    }

    /// # Required Items
    /// 
    /// Adds items which are required by the system innately. Without these 
    /// items, the system will not work.
    /// Currently, there are a handful of items which are considered 'Required' by the system.
    /// These are items which should always be there and will effectively always have some use.
    /// 
    /// - Required Wants
    ///   - ID 0: Rest - Passive Leisure time like sleep.
    ///   - ID 1: Wealth - A generic want representing "stuff".
    ///   - TODO Items
    ///     - Space - How much space is available absolutely.
    ///     - Free Space - How much unused space they have available, not used in storage.
    /// - Required Products:
    ///   - ID 0: Time (hr) (Produces 1 rest for owning it, made by pops at day start, 
    ///                         refreshed every day)
    ///   - ID 1: Shopping Time (Used to shop)
    ///     ID 2: Discernment (shopping skill)
    ///   - TODO Items
    ///     - Land (abstract)
    ///     - Land (Wasteland)
    ///     - Land (Marignal Land)
    ///     - Land (Scrub Land)
    ///     - Land (Quality Land)
    ///     - Land (Fertile Land)
    ///     - Land (Very Fertile Land)
    ///     - Nothing (void item, may not be needed)
    /// - Required Processes:
    ///   - ID 0: Shopping (Time -> Shopping Time)
    ///   - ID 1: Sleep (Time -> Rest)
    /// - Required Tech:
    ///   - ID 0: Brainstorming (origin tech)
    ///   Required Skills:
    ///   - ID 0: Discernment, The Skill of shopping_time.
    pub fn required_items(&mut self) {
        // Wants, Rest and Wealth
        let rest =  match Want::new(REST_WANT_ID, 
        String::from("Rest"), 
        String::from("Rest is the joy of Idle time."), 
        0.1) {
            Result::Err(_) => panic!(),
            Result::Ok(want) => want
        };
        
        let wealth = match Want::new(
            WEALTH_WANT_ID,
            String::from("Wealth"),
            String::from("Wealth is the amount of things you have built up. Not just money, but things. This is a required item."),
            0.2) {
            Result::Err(_) => panic!(),
            Result::Ok(want) => want
        };

        self.wants.insert(REST_WANT_ID, rest);
        self.wants.insert(WEALTH_WANT_ID, wealth);

        // No Tech Groups
        // Techs, Brainstorming
        let brainstorming = Technology {
            id: BRAINSTORMING_TECH_ID,
            name: "Brainstorming".to_string(),
            description: "Thinking hard can often lead to interesting places.".to_string(),
            base_cost: 1,
            tier: 0,
            families: HashSet::new(),
            children: HashSet::new(),
            parents: HashSet::new(),
        };
        self.technology.insert(BRAINSTORMING_TECH_ID, brainstorming);

        // products, Time and Shopping Time
        // Generic Time
        let time = Product::new(TIME_PRODUCT_ID,
            String::from("Time"),
            String::from(""),
            String::from("Time. Always is short supply."), 
            String::from("Hour(s)"), 
            0,
            0.0,
            0.0,
            Some(0),
            true,
            vec![ProductTag::NonTransferrable],
            None,
            None).unwrap();
        // Shopping Time
        let shopping_time = Product::new(SHOPPING_TIME_PRODUCT_ID,
            String::from("Shopping Time"),
            String::from(""),
            String::from("Shopping Time, productive, but sometimes frustrating."), 
            String::from("Hour(s)"), 
            0,
            0.0,
            0.0,
            Some(0),
            true,
            Vec::new(),
            None,
            None).unwrap();
        let discernment = DataManager::create_skill_product(DISCERNMENT_PRODUCT_ID, 
            String::from("Discernment"), String::from("The skill of telling superior from inferior."),
            Some(99));
        let land = Product::new(LAND_PRODUCT_ID,
            String::from("Land"),
            String::from("Simple"),
            String::from("Land has many uses, and everyone needs some."), 
            String::from("Plot(s)"), 
            0,
            0.0,
            0.0,
            None,
            true,
            vec![ProductTag::Fixed],
            None,
            None).unwrap();
        self.products.insert(TIME_PRODUCT_ID, time);
        self.products.insert(SHOPPING_TIME_PRODUCT_ID, shopping_time);
        self.products.insert(DISCERNMENT_PRODUCT_ID, discernment);
        self.products.insert(LAND_PRODUCT_ID, land);

        // processes
        let shop_input = ProcessPart{
            item: Item::Product(TIME_PRODUCT_ID),
            amount: 1.0,
            part_tags: vec![ProcessPartTag::Fixed],
            part: ProcessSectionTag::Input,
        };
        let shop_skill = ProcessPart {
            item: Item::Product(DISCERNMENT_PRODUCT_ID),
            amount: 1.0,
            part_tags: vec![ProcessPartTag::Optional { missing_penalty: 0.0, final_bonus: 1.0 }],
            part: ProcessSectionTag::Capital
        };
        let shop_output = ProcessPart{
            item: Item::Product(SHOPPING_TIME_PROC_ID),
            amount: 1.0,
            part_tags: Vec::new(),
            part: ProcessSectionTag::Output,
        };
        let shop_output_skill = ProcessPart{
            item: Item::Product(DISCERNMENT_PRODUCT_ID),
            amount: 0.1,
            part_tags: Vec::new(),
            part: ProcessSectionTag::Output,
        };
        let go_shopping = Process{
            id: SHOPPING_TIME_PROC_ID,
            name: String::from("Go Shopping"),
            variant_name: String::new(),
            description: String::from("Shopping takes time."),
            minimum_time: 0.0,
            process_parts: vec![shop_input, shop_skill, shop_output, shop_output_skill],
            process_tags: Vec::new(),
            technology_requirement: None,
            tertiary_tech: None,
        };
        self.processes.insert(go_shopping.id, go_shopping);

        // Next do Resting Process
        let rest_input = ProcessPart {
            item: Item::Product(TIME_PRODUCT_ID),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Input,
        };
        let rest_output = ProcessPart {
            item: Item::Want(REST_WANT_ID),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Output,
        };
        let resting = Process {
            id: RESTING_PROC_ID,
            name: String::from("Resting"),
            variant_name: String::new(),
            description: String::from("Chilling Out."),
            minimum_time: 1.0,
            process_parts: vec![rest_input, rest_output],
            process_tags: vec![ProcessTag::Consumption(REST_WANT_ID)],
            technology_requirement: None,
            tertiary_tech: None,
        };
        self.processes.insert(resting.id, resting);
    }

    /// # Create Skill Product
    /// 
    /// This creates a skill product from the given parameters.
    /// 
    /// This skill has the following shared properties.
    /// - No variant name
    /// - Quality 0
    /// - Mass and Bulk 0
    /// - tags, NonTransferrable.
    /// - Fractional: true
    fn create_skill_product(id: usize, name: String, 
    description: String, decay: Option<u32>) -> Product {
        Product {
            id,
            name,
            variant_name: String::from(""),
            description,
            unit_name: String::from("Rank(s)"),
            quality: 0,
            mass: 0.0,
            bulk: 0.0,
            mean_time_to_failure: decay,
            fractional: true,
            tags: vec![ProductTag::NonTransferrable],
            wants: HashMap::new(),
            processes: HashSet::new(),
            failure_process: None,
            use_processes: HashSet::new(),
            consumption_processes: HashSet::new(),
            maintenance_processes: HashSet::new(),
            tech_required: None,
            product_class: None,
        }
    }

    /// Loads wants from a file into the data manager,
    /// Currently, this just loads pre-existing data.
    pub fn load_test_wants(&mut self) -> Result<(), String> {
        let rest =  match Want::new(REST_WANT_ID, 
            String::from("Rest"), 
            String::from("Rest is the joy of Idle time."), 
            0.1) {
                Result::Err(_) => panic!(),
                Result::Ok(want) => want
            };
        
        let wealth = match Want::new(
            WEALTH_WANT_ID,
            String::from("Wealth"),
            String::from("Wealth is the amount of things you have built up. Not just money, but things. This is a required item."),
            0.2) {
            Result::Err(_) => panic!(),
            Result::Ok(want) => want
        };

        let food = match Want::new(
            2,
            String::from("Food"),
            String::from("Food is the desire for sustenance, necissary for all living things."),
            0.2) {
                Result::Err(_) => panic!(),
                Result::Ok(want) => want
            };

        let shelter = match Want::new(
            3,
            String::from("Shelter"),
            String::from("Shelter is the protection from the elements, a space where the difficulties of the outside world are lessened and made tolerable."),
            0.2) {
                Result::Err(_) => panic!(),
                Result::Ok(want) => want
        };

        let clothing = match Want::new(
            4,
            String::from("Clothing"),
            String::from("Clothing is the personal protection from the elements, while it does not separate one from the wider world wholly, it does lessen it's toll."),
            0.2) {
                Result::Err(_) => panic!(),
                Result::Ok(want) => want
        };

        let fashion = match Want::new(
            5,
            String::from("Fashion"),
            String::from("Fashion is about presentation, showing your wealth through jewelry, and higher quality clothing."),
            0.2) {
                Result::Err(_) => panic!(),
                Result::Ok(want) => want
        };

        self.wants.insert(rest.id, rest);
        self.wants.insert(food.id, food);
        self.wants.insert(shelter.id, shelter);
        self.wants.insert(clothing.id, clothing);
        self.wants.insert(fashion.id, fashion);
        self.wants.insert(wealth.id, wealth);

        Ok(())
    }

    pub fn load_test_products(&mut self) -> Result<(), String> {
        // Generic Time
        let time = Product::new(TIME_PRODUCT_ID,
            String::from("Time"),
            String::from(""),
            String::from("Time. Always is short supply."), 
            String::from("Hour(s)"), 
            0,
            0.0,
            0.0,
            Some(0),
            true,
            vec![ProductTag::NonTransferrable],
            None,
            None).unwrap();
        // Shopping Time
        let shopping_time = Product::new(SHOPPING_TIME_PRODUCT_ID,
            String::from("Shopping Time"),
            String::from(""),
            String::from("Shopping Time, productive, but sometimes frustrating."), 
            String::from("Hour(s)"), 
            0,
            0.0,
            0.0,
            Some(0),
            true,
            Vec::new(),
            None,
            None).unwrap();
        // Ambrosia Fruit (food source)
        let ambrosia_fruit = Product::new(2,
            String::from("Ambrosia Fruit"),
            String::from(""),
            String::from("Ambrosia fruit are all one needs to sate their hunger."), 
            String::from("Fruit(s)"), 
            0,
            0.5,
            0.001,
            Some(10),
            false,
            Vec::new(),
            None,
            None).unwrap();
        // cotton boll
        let cotton_boll = Product::new(3,
            String::from("Cotton Boll"),
            String::from(""),
            String::from("A bunch of raw cotton. Useful in some ways, but in need of refinement."), 
            String::from("kg(s)"), 
            0,
            0.01,
            0.001,
            Some(4),
            false,
            Vec::new(),
            None,
            None).unwrap();
        // cotton thread
        let cotton_thread = Product::new(4,
            String::from("Thread"),
            String::from("Cotton"),
            String::from("Cotton Thread, needed for various things."), 
            String::from("Spool(s)"), 
            1,
            0.01,
            0.0001,
            Some(8),
            false,
            Vec::new(),
            None,
            None).unwrap();
        // cotton bolt
        let cotton_bolt = Product::new(5,
            String::from("Bolt"),
            String::from("Cotton"),
            String::from("Cotton Bolt, a bundle of cloth, useful as a simple robe, but better used in clothing."), 
            String::from("Bolt"), 
            1,
            1.0,
            0.01,
            None,
            false,
            Vec::new(),
            None,
            None).unwrap();
        // cotton clothes
        let mut cotton_clothes = Product::new(6,
            String::from("Clothes"),
            String::from("Cotton"),
            String::from("Cotton Clothes, keeps you warm, but kind of ugly looking."), 
            String::from("Set(s)"), 
            1,
            2.0,
            0.01,
            Some(30),
            false,
            Vec::new(),
            None,
            Some(6)).unwrap();
        // cotton suit
        let mut cotton_suit = Product::new(7,
            String::from("Suit"),
            String::from("Cotton"),
            String::from("Cotton Suit, a better set of clothes, looks nice."), 
            String::from("Set(s)"), 
            3,
            2.25,
            0.015,
            Some(50),
            false,
            Vec::new(),
            None,
            Some(6)).unwrap();
        // wood logs
        let wood_logs = Product::new(8,
            String::from("Wood Logs"),
            String::from(""),
            String::from("Wooden logs, used for many things."), 
            String::from("Log(s)"), 
            1,
            50.0,
            2.0,
            None,
            false,
            Vec::new(),
            None,
            None).unwrap();
        // wood gatherer stick
        let wood_gatherer_sticks = Product::new(9,
            String::from("Gatherer Stick"),
            String::from("Wood"),
            String::from("Wooden Gathering sticks make farming much easier, less hurt backs."), 
            String::from("Stick(s)"), 
            1,
            2.0,
            0.01,
            Some(15),
            false,
            Vec::new(),
            None,
            None).unwrap();
        // wood spinning wheel
        let spinning_wheel = Product::new(10,
            String::from("Spinning Wheel"),
            String::from("Wood"),
            String::from("Spinning Wheels, makes spinning thread so much easier to do."), 
            String::from("Wheel(s)"), 
            1,
            5.0,
            0.5,
            Some(60),
            false,
            Vec::new(),
            None,
            None).unwrap();
        // wood loom
        let wood_loom = Product::new(11,
            String::from("Loom"),
            String::from("Wood"),
            String::from("Looms, make weaving so much easier. How did we do it before them?"), 
            String::from("Loom(s)"), 
            1,
            5.0,
            1.0,
            Some(30),
            false,
            Vec::new(),
            None,
            None).unwrap();
        // wood-stone axe
        let wood_stone_axe = Product::new(12,
            String::from("Stone Axe"),
            String::from(""),
            String::from("Stone Axe, useful for getting even more wood."), 
            String::from("Axe(s)"), 
            1,
            2.0,
            0.005,
            Some(10),
            false,
            Vec::new(),
            None,
            None).unwrap();
        // Stone
        let stone = Product::new(13,
            String::from("Stone"),
            String::from("Flint"),
            String::from("Flint Stone, a nice and useful stone for various purposes."), 
            String::from("Stone(s)"), 
            1,
            1.0,
            0.005,
            None,
            false,
            Vec::new(),
            None,
            None).unwrap();
        // hut (no resource, low efficiency, un-maintainable)
        let mut hut = Product::new(14,
            String::from("Hut"),
            String::from(""),
            String::from("Hut, simple, made of dried mud and thatch, doesn't live long, but lives long enough."), 
            String::from("Hut(s)"), 
            1,
            100.0,
            20.0,
            Some(15),
            false,
            Vec::new(),
            None,
            None).unwrap();
        hut.product_class = Some(hut.id);
        // cabin (costs wood, medium efficiency, maintainable)
        let mut cabin = Product::new(15,
            String::from("Cabin"),
            String::from(""),
            String::from("Cabin, warm, sturdy, and homely."), 
            String::from("Cabin(s)"), 
            3,
            250.0,
            30.0,
            Some(60),
            false,
            Vec::new(),
            None,
            None).unwrap();
        cabin.product_class = Some(hut.id);
        cabin.add_to_class(&mut hut);

        // (labors and Services)
        // Ambrosia Farming
        let ambrosia_farming = Product::new(16,
            String::from("Ambrosia Farming"),
            String::from(""),
            String::from("Ambrosia Farming, a simple enough job, but it requires pacing yourself."), 
            String::from("Hour(s)"), 
            0,
            0.0,
            0.0,
            Some(0),
            false,
            Vec::new(),
            None,
            None).unwrap();
        // Cotton Farming
        let cotton_farming = Product::new(17,
            String::from("Cotton Farming"),
            String::from(""),
            String::from("Cotton farming, always hard work, but rewarding if successful."), 
            String::from("Hour(s)"), 
            0,
            0.0,
            0.0,
            Some(0),
            false,
            Vec::new(),
            None,
            None).unwrap();
        // Thread Spinning
        let thread_spinning = Product::new(18,
            String::from("Thread Spinning"),
            String::from(""),
            String::from("Thread Spinning, a slow and methodical task, but important."), 
            String::from("Hour(s)"), 
            0,
            0.0,
            0.0,
            Some(0),
            false,
            Vec::new(),
            None,
            None).unwrap();
        // Weaving
        let weaving = Product::new(19,
            String::from("Weaving"),
            String::from(""),
            String::from("Weaving, taking threads and weaving them into cloth.."), 
            String::from("Hour(s)"), 
            0,
            0.0,
            0.0,
            Some(0),
            false,
            Vec::new(),
            None,
            None).unwrap();
        // Tailoring
        let tailoring = Product::new(20,
            String::from("Tailoring"),
            String::from(""),
            String::from("Tailoring, taking cloth and making clothes out of it."), 
            String::from("Hour(s)"), 
            0,
            0.0,
            0.0,
            Some(0),
            false,
            Vec::new(),
            None,
            None).unwrap();
        // lumbering
        let lumbering = Product::new(21,
            String::from("Lumbering"),
            String::from(""),
            String::from("Lumbering, chopping down trees for the use."), 
            String::from("Hour(s)"), 
            0,
            0.0,
            0.0,
            Some(0),
            false,
            Vec::new(),
            None,
            None).unwrap();
        // tool making
        let tool_making = Product::new(22,
            String::from("Tool Making"),
            String::from(""),
            String::from("Tool Making, creating tools requires forethought and effort."), 
            String::from("Hour(s)"), 
            0,
            0.0,
            0.0,
            Some(0),
            false,
            Vec::new(),
            None,
            None).unwrap();
        // construction
        let construction = Product::new(23,
            String::from("Construction"),
            String::from(""),
            String::from("Construction, making buildings is often quite difficult as a wrong pillar can cause a collapse."), 
            String::from("Hour(s)"), 
            0,
            0.0,
            0.0,
            Some(0),
            false,
            Vec::new(),
            None,
            None).unwrap();
        // building repair
        let building_repair = Product::new(24,
            String::from("Building Repair"),
            String::from(""),
            String::from("Building Repair, reinforcing failing buildings is a subtle art."), 
            String::from("Hour(s)"), 
            0,
            0.0,
            0.0,
            Some(0),
            false,
            Vec::new(),
            None,
            None).unwrap();
        // stone gathering
        let stone_gathering = Product::new(25,
            String::from("Stone Gathering"),
            String::from(""),
            String::from("Stone Gathering, requires a sharp eye and a bit of tenacity."), 
            String::from("Hour(s)"), 
            0,
            0.0,
            0.0,
            Some(0),
            false,
            Vec::new(),
            None,
            None).unwrap();

        // Set Wants
        let mut shelter = self.wants
            .get_mut(&3).unwrap();
        hut.connect_want(&mut shelter, 1.0).unwrap();
        cabin.connect_want(&mut shelter, 1.5).unwrap();

        let clothes = self.wants
            .get_mut(&4).unwrap();
        cotton_clothes.connect_want(clothes, 1.0).unwrap();
        cotton_suit.connect_want(clothes,1.5).unwrap();

        let fashion = self.wants
            .get_mut(&4).unwrap();
        cotton_suit.connect_want(fashion,1.0).unwrap();

        let wealth = self.wants
            .get_mut(&4).unwrap();
        cotton_suit.connect_want(wealth,1.0).unwrap();
        cabin.connect_want(wealth, 2.0).unwrap();

        self.products.insert(time.id, time);
        self.products.insert(shopping_time.id, shopping_time);
        self.products.insert(ambrosia_fruit.id, ambrosia_fruit);
        self.products.insert(cotton_boll.id, cotton_boll);
        self.products.insert(cotton_thread.id, cotton_thread);
        self.products.insert(cotton_bolt.id, cotton_bolt);
        self.products.insert(cotton_clothes.id, cotton_clothes);
        self.products.insert(cotton_suit.id, cotton_suit);
        self.products.insert(wood_logs.id, wood_logs);
        self.products.insert(wood_gatherer_sticks.id, wood_gatherer_sticks);
        self.products.insert(spinning_wheel.id, spinning_wheel);
        self.products.insert(wood_loom.id, wood_loom);
        self.products.insert(wood_stone_axe.id, wood_stone_axe);
        self.products.insert(stone.id, stone);
        self.products.insert(hut.id, hut);
        self.products.insert(cabin.id, cabin);
        self.products.insert(ambrosia_farming.id, ambrosia_farming);
        self.products.insert(cotton_farming.id, cotton_farming);
        self.products.insert(thread_spinning.id, thread_spinning);
        self.products.insert(weaving.id, weaving);
        self.products.insert(tailoring.id, tailoring);
        self.products.insert(lumbering.id, lumbering);
        self.products.insert(tool_making.id, tool_making);
        self.products.insert(construction.id, construction);
        self.products.insert(building_repair.id, building_repair);
        self.products.insert(stone_gathering.id, stone_gathering);

        self.update_product_classes()?;

        Ok(())
    }

    pub fn update_product_classes(&mut self) -> Result<(), String> {
        // clear out old data for sanity measures
        self.product_classes.clear();
        // update and sanity check product classes
        for (&product_id, prod_data) in self.products.iter()
        .sorted_by(|a,b| a.0.cmp(b.0)) {
            if let Some(id) = prod_data.product_class {
                // sanity check that the generic points to itself
                let other = self.products.get(&id).unwrap();
                if other.product_class.unwrap() != other.id {
                    return Err(format!("Product: '{}' is a generic that does not point to itself.", id));
                }
                // since it correctly points to itself, add to class group records
                self.product_classes.entry(other.id)
                .and_modify(|x| x.push(product_id))
                .or_insert(vec![product_id]);
            }
        }

        Ok(())
    }

    pub fn load_test_processes(&mut self) -> Result<(), String> {
        let time = 0;
        let shopping_time = 1;
        let ambrosia_fruit = 2;
        let wood = 8;
        let hook = 9;
        let spinning_wheel = 10;
        let loom = 11;
        let axe = 12;
        let stone = 13;
        let hut = 14;
        let cabin = 15;
        let fruit_farming = 16;
        let tooling = 22;
        let building = 23;
        let repair = 24;
        let stoning = 25;

        // We for test cases, we are ignoring closed loop logic for now.
        // shopping time first, this is a required process by the system.
        let shop_input = ProcessPart{
            item: Item::Product(time),
            amount: 1.0,
            part_tags: Vec::new(),
            part: ProcessSectionTag::Input,
        };
        let shop_output = ProcessPart{
            item: Item::Product(shopping_time),
            amount: 1.0,
            part_tags: Vec::new(),
            part: ProcessSectionTag::Output,
        };
        let go_shopping = Process{
            id: SHOPPING_TIME_PROC_ID,
            name: String::from("Go Shopping"),
            variant_name: String::new(),
            description: String::from("Shopping takes time."),
            minimum_time: 1.0,
            process_parts: vec![shop_input, shop_output],
            process_tags: Vec::new(),
            technology_requirement: None,
            tertiary_tech: None,
        };
        self.processes.insert(go_shopping.id, go_shopping);

        // Next do Resting Process
        let rest_input = ProcessPart {
            item: Item::Product(TIME_PRODUCT_ID),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Input,
        };
        let rest_output = ProcessPart {
            item: Item::Want(REST_WANT_ID),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Output,
        };
        let resting = Process {
            id: RESTING_PROC_ID,
            name: String::from("Resting"),
            variant_name: String::new(),
            description: String::from("Chilling Out."),
            minimum_time: 1.0,
            process_parts: vec![rest_input, rest_output],
            process_tags: vec![ProcessTag::Consumption(REST_WANT_ID)],
            technology_requirement: None,
            tertiary_tech: None,
        };
        self.processes.insert(resting.id, resting);

        // next do labors, they'll be easy.
        // ambrosia farming 1
        let new_id = self.new_process_id(); // 2

        let ambrosia_default 
            = self.build_skill_process(16, new_id, 
                String::from("Ambrosia Farming"), String::from(""));
        self.processes.insert(ambrosia_default.id, ambrosia_default);
        // cotton farming 2
        let new_id = self.new_process_id();
        let cotton_farming 
            = self.build_skill_process(17, new_id, 
                String::from("Cotton Farming"), String::from(""));
        self.processes.insert(cotton_farming.id, cotton_farming);
        // thread spinning 3
        let new_id = self.new_process_id();
        let spinning = self.build_skill_process(18, new_id, 
            String::from("Thread Spinning"), String::from(""));
        self.processes.insert(spinning.id, spinning);
        // Weaving 4
        let new_id = self.new_process_id();
        let weaving = self.build_skill_process(19, new_id, 
            String::from("Weaving"), String::from(""));
        self.processes.insert(weaving.id, weaving);
        // Tailoring 5
        let new_id = self.new_process_id();
        let tailoring = self.build_skill_process(20, new_id, 
            String::from("Tailoring"), String::from(""));
        self.processes.insert(tailoring.id, tailoring);
        // Lumbering 6
        let new_id = self.new_process_id();
        let lumbering = self.build_skill_process(21, new_id, 
            String::from("Lumbering"), String::from(""));
        self.processes.insert(lumbering.id, lumbering);
        // Tool Maker 7
        let new_id = self.new_process_id();
        let tool_maker = self.build_skill_process(22, new_id, 
            String::from("Tool Making"), String::from(""));
        self.processes.insert(tool_maker.id, tool_maker);
        // construction 8
        let new_id = self.new_process_id();
        let construction = self.build_skill_process(23, new_id, 
            String::from("Construction"), String::from(""));
        self.processes.insert(construction.id, construction);
        // building repair 9
        let new_id = self.new_process_id();
        let building_repair = self.build_skill_process(24, new_id, 
            String::from("Building Repair"), String::from(""));
        self.processes.insert(building_repair.id, building_repair);
        // stone gathering 10
        let new_id = self.new_process_id();
        let stone_gathering = self.build_skill_process(25, new_id, 
            String::from("Stone Gathering"), String::from(""));
        self.processes.insert(stone_gathering.id, stone_gathering);

        // ambrosia chain 
        // Ambrosia Farming -> Ambrosia fruit -> Food (no waste involved)
        let labor_input = ProcessPart{
            item: Item::Product(fruit_farming),
            amount: 1.0,
            part_tags: vec![ProcessPartTag::Fixed],
            part: ProcessSectionTag::Input,
        };
        let harvester_capital = ProcessPart{
            item: Item::Product(hook),
            amount: 1.0,
            part_tags: vec![ProcessPartTag::Optional { missing_penalty: 0.0, final_bonus: 0.25 }],
            part: ProcessSectionTag::Capital,
        };
        let fruit_output = ProcessPart{
            item: Item::Product(ambrosia_fruit),
            amount: 1.0,
            part_tags: Vec::new(),
            part: ProcessSectionTag::Output,
        };
        let ambrosia_farming = Process{
            id: self.new_process_id(), // 11
            name: String::from("Ambrosia Culture"),
            variant_name: String::new(),
            description: String::from("Ambrosia Farming."),
            minimum_time: 1.0,
            process_parts: vec![labor_input, harvester_capital, fruit_output],
            process_tags: Vec::new(),
            technology_requirement: None,
            tertiary_tech: None,
        };
        self.processes.insert(ambrosia_farming.id, ambrosia_farming);
        // eating food
        let food_input = ProcessPart{
            item: Item::Product(2),
            amount: 1.0,
            part_tags: Vec::new(),
            part: ProcessSectionTag::Input,
        };
        let food_output = ProcessPart{
            item: Item::Want(2),
            amount: 1.0,
            part_tags: Vec::new(),
            part: ProcessSectionTag::Output,
        };
        let ambrosia_consumption = Process{
            id: self.new_process_id(), // 13
            name: String::from("Ambrosia Meal"),
            variant_name: String::new(),
            description: String::from("A meal of Ambrosia, even one fruit is enough to satisfy for a day."),
            minimum_time: 0.0,
            process_parts: vec![food_input, food_output],
            process_tags: vec![ProcessTag::Consumption(2)],
            technology_requirement: None,
            tertiary_tech: None,
        };
        self.processes.insert(ambrosia_consumption.id, ambrosia_consumption);

        // Cotton Chain
        // Cotton Farmer -> Spinner -> weaver -> clothing
        let labor_input = ProcessPart{
            item: Item::Product(17),
            amount: 12.0,
            part_tags: vec![ProcessPartTag::Fixed],
            part: ProcessSectionTag::Input,
        };
        let harvester_capital = ProcessPart{
            item: Item::Product(9),
            amount: 1.0,
            part_tags: vec![ProcessPartTag::Optional{ missing_penalty: 0.0, final_bonus: 1.0}],
            part: ProcessSectionTag::Capital,
        };
        let cotton_output = ProcessPart{
            item: Item::Product(3),
            amount: 0.5,
            part_tags: Vec::new(),
            part: ProcessSectionTag::Output,
        };
        let cotton_farming = Process{
            id: self.new_process_id(), // 13
            name: String::from("Cottonculture"),
            variant_name: String::new(),
            description: String::from("Cotton Farming."),
            minimum_time: 1.0,
            process_parts: vec![labor_input, harvester_capital, cotton_output],
            process_tags: Vec::new(),
            technology_requirement: None,
            tertiary_tech: None,
        };
        self.processes.insert(cotton_farming.id, cotton_farming);
        // Spinning
        let labor_input = ProcessPart{
            item: Item::Product(18), // thread spinning
            amount: 12.0,
            part_tags: vec![ProcessPartTag::Fixed],
            part: ProcessSectionTag::Input,
        };
        let cotton_input = ProcessPart{
            item: Item::Product(3), // thread spinning
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Input,
        };
        let spinner_capital = ProcessPart{
            item: Item::Product(10), // spinning wheel
            amount: 1.0,
            part_tags: vec![ProcessPartTag::Optional { missing_penalty: 0.0, final_bonus: 1.0 }],
            part: ProcessSectionTag::Capital,
        };
        let thread_output = ProcessPart{
            item: Item::Product(4), // thread
            amount: 8.0,
            part_tags: Vec::new(),
            part: ProcessSectionTag::Output,
        };
        let thread_spinning = Process{
            id: self.new_process_id(), // 14
            name: String::from("Spinning"),
            variant_name: String::from("Cotton Thread"),
            description: String::from("Spinning thread."),
            minimum_time: 1.0,
            process_parts: vec![cotton_input, labor_input,
                spinner_capital, thread_output],
            process_tags: Vec::new(),
            technology_requirement: None,
            tertiary_tech: None,
        };
        self.processes.insert(thread_spinning.id, thread_spinning);
        // Weaver
        let labor_input = ProcessPart{
            item: Item::Product(19), // Weaving
            amount: 12.0,
            part_tags: vec![ProcessPartTag::Fixed],
            part: ProcessSectionTag::Input,
        };
        let thread_input = ProcessPart{
            item: Item::Product(4), // thread
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Input,
        };
        let loom_capital = ProcessPart{
            item: Item::Product(11),
            amount: 1.0,
            part_tags: vec![ProcessPartTag::Optional { missing_penalty: 0.0, final_bonus: 3.0 }],
            part: ProcessSectionTag::Capital,
        };
        let cloth_output = ProcessPart{
            item: Item::Product(5),
            amount: 1.0,
            part_tags: Vec::new(),
            part: ProcessSectionTag::Output,
        };
        let weaving = Process{
            id: self.new_process_id(), // 15
            name: String::from("Weaving"),
            variant_name: String::from("Cotton"),
            description: String::from("Weaving Cloth from Thread."),
            minimum_time: 1.0,
            process_parts: vec![thread_input, labor_input,
                loom_capital, cloth_output],
            process_tags: Vec::new(),
            technology_requirement: None,
            tertiary_tech: None,
        };
        self.processes.insert(weaving.id, weaving);
        // Clothing
        let labor_input = ProcessPart{
            item: Item::Product(20), // Tailoring
            amount: 12.0,
            part_tags: vec![ProcessPartTag::Fixed],
            part: ProcessSectionTag::Input,
        };
        let cloth_input = ProcessPart{
            item: Item::Product(5), // cotton bolt
            amount: 2.0,
            part_tags: vec![],
            part: ProcessSectionTag::Input,
        };
        let cloth_output = ProcessPart{
            item: Item::Product(6),
            amount: 1.0,
            part_tags: Vec::new(),
            part: ProcessSectionTag::Output,
        };
        let clothing = Process{
            id: self.new_process_id(), // 16
            name: String::from("Clothes"),
            variant_name: String::from("Normal"),
            description: String::from("Normal Clothes."),
            minimum_time: 1.0,
            process_parts: vec![cloth_input, labor_input,
                cloth_output],
            process_tags: Vec::new(),
            technology_requirement: None,
            tertiary_tech: None,
        };
        self.processes.insert(clothing.id, clothing);
        // Suit of clothing
        let labor_input = ProcessPart{
            item: Item::Product(20), // Tailoring
            amount: 36.0,
            part_tags: vec![ProcessPartTag::Fixed],
            part: ProcessSectionTag::Input,
        };
        let cloth_input = ProcessPart{
            item: Item::Product(5), // cotton bolt
            amount: 2.0,
            part_tags: vec![],
            part: ProcessSectionTag::Input,
        };
        let thread_input = ProcessPart{
            item: Item::Product(4), // cotton thread
            amount: 0.25,
            part_tags: vec![],
            part: ProcessSectionTag::Input,
        };
        let cloth_output = ProcessPart{
            item: Item::Product(7),
            amount: 1.0,
            part_tags: Vec::new(),
            part: ProcessSectionTag::Output,
        };
        let clothing = Process{
            id: self.new_process_id(), // 17
            name: String::from("Clothes"),
            variant_name: String::from("Quality"),
            description: String::from("Making Quality Clothes."),
            minimum_time: 1.0,
            process_parts: vec![cloth_input, labor_input, thread_input,
                cloth_output],
            process_tags: Vec::new(),
            technology_requirement: None,
            tertiary_tech: None,
        };
        self.processes.insert(clothing.id, clothing);
        // 
        // Rock Finding
        let labor_input = ProcessPart{
            item: Item::Product(stoning), // Stone Gathering
            amount: 12.0,
            part_tags: vec![ProcessPartTag::Fixed],
            part: ProcessSectionTag::Input,
        };
        let stone_output = ProcessPart{
            item: Item::Product(stone),
            amount: 1.0,
            part_tags: Vec::new(),
            part: ProcessSectionTag::Output,
        };
        let stone_gathering = Process{
            id: self.new_process_id(),
            name: String::from("Stone Hunt"),
            variant_name: String::new(),
            description: String::from("Hunting for stones"),
            minimum_time: 0.0,
            process_parts: vec![labor_input, stone_output],
            process_tags: Vec::new(),
            technology_requirement: None,
            tertiary_tech: None,
        };
        self.processes.insert(stone_gathering.id, stone_gathering);
        // hut making
        let labor_input = ProcessPart{
            item: Item::Product(building), // construction
            amount: 240.0,
            part_tags: vec![ProcessPartTag::Fixed],
            part: ProcessSectionTag::Input,
        };
        let hut_output = ProcessPart{
            item: Item::Product(hut), // hut
            amount: 1.0,
            part_tags: Vec::new(),
            part: ProcessSectionTag::Output,
        };
        let hut_construction = Process{
            id: self.new_process_id(),
            name: String::from("Hut Construction"),
            variant_name: String::new(),
            description: String::from("Making Huts"),
            minimum_time: 0.0,
            process_parts: vec![labor_input, hut_output],
            process_tags: Vec::new(),
            technology_requirement: None,
            tertiary_tech: None,
        };
        self.processes.insert(hut_construction.id, hut_construction);
        // hut repair
        let labor_input = ProcessPart{
            item: Item::Product(23), // construction
            amount: 6.0,
            part_tags: vec![ProcessPartTag::Fixed],
            part: ProcessSectionTag::Input,
        };
        let hut_input = ProcessPart{
            item: Item::Product(14), // hut
            amount: 1.0,
            part_tags: Vec::new(),
            part: ProcessSectionTag::Input,
        };
        let hut_output = ProcessPart{
            item: Item::Product(14), // hut
            amount: 1.0,
            part_tags: Vec::new(),
            part: ProcessSectionTag::Output,
        };
        let hut_repair = Process{
            id: self.new_process_id(),
            name: String::from("Hut Repair"),
            variant_name: String::new(),
            description: String::from("Repairing Huts"),
            minimum_time: 0.0,
            process_parts: vec![labor_input, hut_input, hut_output],
            process_tags: vec![ProcessTag::Maintenance(14)],
            technology_requirement: None,
            tertiary_tech: None,
        };
        self.processes.insert(hut_repair.id, hut_repair);
        // wood and wood crafts
        // Wood -> (Gathering Sticks, Spinning Wheels, Looms, Cabins)
        // gather wood
        let labor_input = ProcessPart{
            item: Item::Product(21), // lumbering
            amount: 12.0,
            part_tags: vec![ProcessPartTag::Fixed],
            part: ProcessSectionTag::Input,
        };
        let axe_capital = ProcessPart{
            item: Item::Product(12), // axe
            amount: 1.0,
            part_tags: vec![ProcessPartTag::Optional { missing_penalty: 0.0, final_bonus: 5.0}],
            part: ProcessSectionTag::Capital,
        };
        let wood_output = ProcessPart{
            item: Item::Product(14), // wood
            amount: 1.0,
            part_tags: Vec::new(),
            part: ProcessSectionTag::Output,
        };
        let lumbering = Process{
            id: self.new_process_id(),
            name: String::from("Lumberjacking"),
            variant_name: String::new(),
            description: String::from("Chopping down trees."),
            minimum_time: 12.0,
            process_parts: vec![labor_input, axe_capital, wood_output],
            process_tags: vec![ProcessTag::Maintenance(14)],
            technology_requirement: None,
            tertiary_tech: None,
        };
        self.processes.insert(lumbering.id, lumbering);
        // wood -> gathering sticks
        let labor_input = ProcessPart{
            item: Item::Product(22), // tool making
            amount: 12.0,
            part_tags: vec![],
            part: ProcessSectionTag::Input,
        };
        let wood_input = ProcessPart{
            item: Item::Product(8), // Wood
            amount: 0.1,
            part_tags: vec![],
            part: ProcessSectionTag::Input,
        };
        let wood_output = ProcessPart{
            item: Item::Product(9), // gathering stick
            amount: 1.0,
            part_tags: Vec::new(),
            part: ProcessSectionTag::Output,
        };
        let hooker_making = Process{
            id: self.new_process_id(),
            name: String::from("Craft"),
            variant_name: String::from("Gathering Stick"),
            description: String::from("Gathering Stick making."),
            minimum_time: 12.0,
            process_parts: vec![labor_input, wood_input, wood_output],
            process_tags: vec![],
            technology_requirement: None,
            tertiary_tech: None,
        };
        self.processes.insert(hooker_making.id, hooker_making);
        // wood -> spinning wheel
        let labor_input = ProcessPart{
            item: Item::Product(tooling),
            amount: 36.0,
            part_tags: vec![ProcessPartTag::Fixed],
            part: ProcessSectionTag::Input,
        };
        let wood_input = ProcessPart{
            item: Item::Product(wood),
            amount: 0.8,
            part_tags: vec![],
            part: ProcessSectionTag::Input,
        };
        let wood_output = ProcessPart{
            item: Item::Product(spinning_wheel),
            amount: 1.0,
            part_tags: Vec::new(),
            part: ProcessSectionTag::Output,
        };
        let wheel_making = Process{
            id: self.new_process_id(),
            name: String::from("Craft"),
            variant_name: String::from("Spinning Wheel"),
            description: String::from("Craft a spinning wheel!"),
            minimum_time: 12.0,
            process_parts: vec![labor_input, wood_input, wood_output],
            process_tags: vec![],
            technology_requirement: None,
            tertiary_tech: None,
        };
        self.processes.insert(wheel_making.id, wheel_making);
        // wood -> looms
        let labor_input = ProcessPart{
            item: Item::Product(tooling),
            amount: 48.0,
            part_tags: vec![ProcessPartTag::Fixed],
            part: ProcessSectionTag::Input,
        };
        let wood_input = ProcessPart{
            item: Item::Product(wood),
            amount: 0.5,
            part_tags: vec![],
            part: ProcessSectionTag::Input,
        };
        let wood_output = ProcessPart{
            item: Item::Product(loom),
            amount: 1.0,
            part_tags: Vec::new(),
            part: ProcessSectionTag::Output,
        };
        let loom_making = Process{
            id: self.new_process_id(),
            name: String::from("Craft"),
            variant_name: String::from("Loom"),
            description: String::from("Craft a Loom!"),
            minimum_time: 12.0,
            process_parts: vec![labor_input, wood_input, wood_output],
            process_tags: vec![],
            technology_requirement: None,
            tertiary_tech: None,
        };
        self.processes.insert(loom_making.id, loom_making);
        // wood -> cabin
        let labor_input = ProcessPart{
            item: Item::Product(building),
            amount: 120.0,
            part_tags: vec![ProcessPartTag::Fixed],
            part: ProcessSectionTag::Input,
        };
        let wood_input = ProcessPart{
            item: Item::Product(wood),
            amount: 10.0,
            part_tags: vec![],
            part: ProcessSectionTag::Input,
        };
        let wood_output = ProcessPart{
            item: Item::Product(cabin),
            amount: 1.0,
            part_tags: Vec::new(),
            part: ProcessSectionTag::Output,
        };
        let loom_making = Process{
            id: self.new_process_id(),
            name: String::from("Construct"),
            variant_name: String::from("Cabin"),
            description: String::from("Craft a Loom!"),
            minimum_time: 12.0,
            process_parts: vec![labor_input, wood_input, wood_output],
            process_tags: vec![],
            technology_requirement: None,
            tertiary_tech: None,
        };
        self.processes.insert(loom_making.id, loom_making);
        // wood -> cabin repair
        let labor_input = ProcessPart{
            item: Item::Product(repair),
            amount: 6.0,
            part_tags: vec![ProcessPartTag::Fixed],
            part: ProcessSectionTag::Input,
        };
        let wood_input = ProcessPart{
            item: Item::Product(wood),
            amount: 0.1,
            part_tags: vec![],
            part: ProcessSectionTag::Input,
        };
        let cabin_input = ProcessPart{
            item: Item::Product(cabin),
            amount: 1.0,
            part_tags: Vec::new(),
            part: ProcessSectionTag::Input,
        };
        let cabin_output = ProcessPart{
            item: Item::Product(cabin),
            amount: 1.0,
            part_tags: Vec::new(),
            part: ProcessSectionTag::Output,
        };
        let cabin_repair = Process{
            id: self.new_process_id(),
            name: String::from("Repair"),
            variant_name: String::from("cabin"),
            description: String::from("Repair this Cabin!"),
            minimum_time: 1.0,
            process_parts: vec![labor_input, cabin_input, wood_input, cabin_output],
            process_tags: vec![ProcessTag::Maintenance(cabin)],
            technology_requirement: None,
            tertiary_tech: None,
        };
        self.processes.insert(cabin_repair.id, cabin_repair);
        // wood + stone -> axe
        let labor_input = ProcessPart{
            item: Item::Product(tooling),
            amount: 6.0,
            part_tags: vec![ProcessPartTag::Fixed],
            part: ProcessSectionTag::Input,
        };
        let wood_input = ProcessPart{
            item: Item::Product(wood),
            amount: 0.1,
            part_tags: vec![],
            part: ProcessSectionTag::Input,
        };
        let stone_input = ProcessPart{
            item: Item::Product(stone),
            amount: 1.0,
            part_tags: Vec::new(),
            part: ProcessSectionTag::Input,
        };
        let axe_output = ProcessPart{
            item: Item::Product(axe),
            amount: 1.0,
            part_tags: Vec::new(),
            part: ProcessSectionTag::Output,
        };
        let axe_making = Process{
            id: self.new_process_id(),
            name: String::from("Craft"),
            variant_name: String::from("Axe"),
            description: String::from("Make an Axe!"),
            minimum_time: 1.0,
            process_parts: vec![labor_input, stone_input, wood_input, axe_output],
            process_tags: vec![],
            technology_requirement: None,
            tertiary_tech: None,
        };
        self.processes.insert(axe_making.id, axe_making);

        // once all processes are loaded connect the products to the processes
        for process in self.processes.values() {
            for part in process.process_parts.iter() {
                if part.item.is_product() {
                    let id = part.item.unwrap();
                    let product = self.products.get_mut(&id).unwrap();
                    product.add_process(process)
                    .expect(
                        format!("An error occured connecting process '{}' to proudct '{}'",
                        process.get_name(), 
                        product.get_name()).as_str());
                }
                else if part.item.is_want() &&
                    part.part == ProcessSectionTag::Output { 
                        // if it is want and an output, then it must be some
                        // use to a want
                        // add it to the want
                        let id = part.item.unwrap();
                        let want = self.wants.get_mut(&id).unwrap();
                        want.add_process_source(process)
                            .expect("Error Occured in processing to want.");
                }
            }
        }
    
        // connect up process nodes.
        // preemtively create all of them so we can add as we go.
        for id in self.processes.keys() {
            self.process_nodes.insert(*id, ProcessNode::new(*id));
        }
        for (id, process) in self.processes.iter() {
            // processes share an ID with their node for simplicity.
            let mut new_node = ProcessNode::new(*id);

            new_node.can_feed_self = process.can_feed_self(&self);

            for (other_id, other_process) in self.processes.iter() {
                // for our current process, iterate through again
                // skip if same id (we already checked it)
                if id == other_id {
                    continue;
                }
                if  new_node.inputs.contains(other_id) || 
                    new_node.capitals.contains(other_id) ||
                    new_node.outputs.contains(other_id) {
                    // if current node already references other_id, skip
                    continue;
                }
                let other_node = self.process_nodes.get_mut(other_id).unwrap();
                // check connections to other process and add to both if there is one
                if process.takes_input_from(other_process) { // inputs to output
                    new_node.inputs.push(*other_id);
                    other_node.outputs.push(*id);
                }
                if process.takes_capital_from(other_process) { // capital to output (product only)
                    new_node.capitals.push(*other_id);
                    other_node.outputs.push(*id);
                }
                if process.gives_output_to_others_input(other_process) { // output to input
                    new_node.outputs.push(*other_id);
                    other_node.inputs.push(*id);
                }
                if process.gives_output_to_others_capital(other_process) { // output to capital (product only)
                    new_node.outputs.push(*other_id);
                    other_node.capitals.push(*id);
                }
            }

            self.process_nodes.insert(*id, new_node);
        };

        // check for duplicate items (TODO update to only check the new items, not the old)
        let mut dups: HashMap<String, Vec<usize>> = HashMap::new();
        for (id, process) in self.processes.iter() {
            dups.entry(process.get_name()).or_insert(vec![]).push(*id);
        }
        let mut err = String::new();
        for (name, ids) in dups.iter() {
            if ids.len() > 1 { // If there is more than 1 id here, add it to our return.
                let mut dup_error = format!("Duplicate process '{}'\n", name);
                for id in ids {
                    dup_error += format!("{:>5}\n", id).as_str();
                }
                err += dup_error.as_str();
            }
        }
        if err.len() > 0 {
            return Err(err);
        }

        Ok(())
    }

        /// Creates a default process for the skill and it's labor.
    /// 
    /// # Defalts
    /// 
    /// The process defaults to taking 1 Time(fixed) (always id 0) 
    /// and outputting 1 unit of labor.
    /// 
    /// 
    /// The name is Labor, while vairant_name and description are the skill's name.
    /// 
    /// 
    /// Minimum time = 0, skill min = 0, skill max = 3, and no techs attached.
    /// 
    /// The skill is also connected as the process's skill
    /// 
    /// # Notes
    /// 
    /// Edit to meet your needs later, as needed.
    pub fn build_skill_process(&self, labor: usize, id: usize, name: String, desc: String) 
    -> Process {
        let input = ProcessPart {
            item: Item::Product(TIME_PRODUCT_ID),
            amount: 1.0,
            part_tags: vec![ProcessPartTag::Fixed],
            part: ProcessSectionTag::Input,
        };
        let output = ProcessPart{
            item: Item::Product(labor),
            amount: 1.0,
            part_tags: vec![],
            part: ProcessSectionTag::Output,
        };
        Process {
            id: id,
            name: "Labor".into(),
            variant_name: name,
            description: desc,
            minimum_time: 0.0,
            process_parts: vec![input, output],
            process_tags: vec![],
            technology_requirement: None,
            tertiary_tech: None,
        }
    }

    pub fn load_test_jobs(&mut self) -> Result<(), String> {
        // Food, clothes, and shelter
        let mut subsistence_farmer = Job::new(
            0, 
            "Subsistence Farmer".into(),
            String::new(),
            0);
        subsistence_farmer.processes.push(1); // Labor(Ambrosia Farming)
        subsistence_farmer.processes.push(2); // Labor(Cotton Farming)
        subsistence_farmer.processes.push(3); // Labor(Thread Spinning)
        subsistence_farmer.processes.push(4); // Labor(Weaving)
        subsistence_farmer.processes.push(5); // Labor(Tailoring)
        subsistence_farmer.processes.push(8); // Labor(Construction)
        subsistence_farmer.processes.push(9); // Labor(Building Repair)
        subsistence_farmer.processes.push(11); // Ambrosia Culture
        subsistence_farmer.processes.push(13); // cottonculture
        subsistence_farmer.processes.push(14); // Spinning(Cotton Thread)
        subsistence_farmer.processes.push(15); // Weaving(Cotton)
        subsistence_farmer.processes.push(16); // Clothes(Normal)
        subsistence_farmer.processes.push(19); // Hut Construction
        subsistence_farmer.processes.push(20); // Hut Repair
        self.jobs.insert(0, subsistence_farmer);

        // Ambrosia Farming
        let mut ambrosia_farmer = Job::new(
            1,
            "Ambrosia Farmer".into(),
            String::new(),
            0
        );
        ambrosia_farmer.processes.push(1);
        ambrosia_farmer.processes.push(11);
        self.jobs.insert(1, ambrosia_farmer);

        // all of these share the same processes, but focus on different parts
        // cotton farming
        let mut cotton_farming = Job::new(
            2,
            "Cotton Farmer".into(),
            String::new(),
            1
        );
        cotton_farming.processes.push(2);
        cotton_farming.processes.push(3);
        cotton_farming.processes.push(4);
        cotton_farming.processes.push(5);
        cotton_farming.processes.push(13);
        cotton_farming.processes.push(14);
        cotton_farming.processes.push(15);
        cotton_farming.processes.push(16);
        cotton_farming.processes.push(17);
        self.jobs.insert(2, cotton_farming);
        // spinning
        let mut thread_spinning = Job::new(
            3,
            "Thread Spinner".into(),
            String::new(),
            2
        );
        thread_spinning.processes.push(2);
        thread_spinning.processes.push(3);
        thread_spinning.processes.push(4);
        thread_spinning.processes.push(5);
        thread_spinning.processes.push(13);
        thread_spinning.processes.push(14);
        thread_spinning.processes.push(15);
        thread_spinning.processes.push(16);
        thread_spinning.processes.push(17);
        self.jobs.insert(3, thread_spinning);
        // weaving
        let mut thread_spinning = Job::new(
            4,
            "Weaving".into(),
            String::new(),
            3
        );
        thread_spinning.processes.push(2);
        thread_spinning.processes.push(3);
        thread_spinning.processes.push(4);
        thread_spinning.processes.push(5);
        thread_spinning.processes.push(13);
        thread_spinning.processes.push(14);
        thread_spinning.processes.push(15);
        thread_spinning.processes.push(16);
        thread_spinning.processes.push(17);
        self.jobs.insert(4, thread_spinning);
        // tailoring
        let mut thread_spinning = Job::new(
            5,
            "Tailoring".into(),
            String::new(),
            4
        );
        thread_spinning.processes.push(2);
        thread_spinning.processes.push(3);
        thread_spinning.processes.push(4);
        thread_spinning.processes.push(5);
        thread_spinning.processes.push(13);
        thread_spinning.processes.push(14);
        thread_spinning.processes.push(15);
        thread_spinning.processes.push(16);
        thread_spinning.processes.push(17);
        self.jobs.insert(5, thread_spinning);

        // these two are cousin jobs
        // lumbering
        let mut lumbering = Job::new(
            6,
            "Lumbering".into(),
            String::new(),
            5
        );
        lumbering.processes.push(6);
        lumbering.processes.push(7);
        lumbering.processes.push(21);
        lumbering.processes.push(22);
        lumbering.processes.push(23);
        lumbering.processes.push(24);
        lumbering.processes.push(27);
        self.jobs.insert(6, lumbering);
        // tool making
        let mut tool_making = Job::new(
            7,
            "Tool Making".into(),
            String::new(),
            6
        );
        tool_making.processes.push(6);
        tool_making.processes.push(7);
        tool_making.processes.push(21);
        tool_making.processes.push(22);
        tool_making.processes.push(23);
        tool_making.processes.push(24);
        tool_making.processes.push(27);
        self.jobs.insert(7, tool_making);
        
        // construction / repair
        let mut construction = Job::new(
            8,
            "Constructing".into(),
            String::new(),
            7
        );
        construction.processes.push(6);
        construction.processes.push(8);
        construction.processes.push(9);
        construction.processes.push(19);
        construction.processes.push(20);
        construction.processes.push(21);
        construction.processes.push(25);
        construction.processes.push(26);
        self.jobs.insert(8, construction);
        // repair
        let mut repair = Job::new(
            9,
            "Repairman".into(),
            String::new(),
            8
        );
        repair.processes.push(6);
        repair.processes.push(8);
        repair.processes.push(9);
        repair.processes.push(19);
        repair.processes.push(20);
        repair.processes.push(21);
        repair.processes.push(25);
        repair.processes.push(26);
        self.jobs.insert(9, repair);

        // stone gathering the outlier
        let mut stone_gathering = Job::new(
            10,
            "Stone Gathering".into(),
            String::new(),
            9
        );
        stone_gathering.processes.push(10);
        stone_gathering.processes.push(18);
        self.jobs.insert(10, stone_gathering);

        Ok(())
    }

    /// Placeholder loader for everything Should load by sets later on, rather than all at once.
    /// 
    /// Todo when updating any of these, ensure that the current data is saved somewhere so it can be used for tests.
    pub fn load_test_data(&mut self) -> Result<(), String> {
        self.load_test_wants()?;
        self.load_test_products()?;
        self.load_test_processes()?;
        self.load_test_jobs()?;

        Ok(())
    }

    /// # Get Product Class
    /// 
    /// Gets the product's class.
    /// 
    /// If it has one it returns the class ID. If it does not, it returns None.
    /// 
    /// TODO Test This.
    pub fn get_product_class(&self, product: usize) -> Option<usize> {
        if let Some(class) = self.products.get(&product).expect("Product Not Found").product_class {
            Some(class)
        } else { None }
    }
}

// new ids section
impl DataManager {
    pub fn new_want_id(&mut self) -> usize {
        loop {
            if self.wants.contains_key(&self.want_id) {
                self.want_id += 1;
            }
            else {
                return self.want_id;
            }
        }
    }

    pub fn new_tech_id(&mut self) -> usize {
        loop {
            if self.technology.contains_key(&self.tech_id) {
                self.tech_id += 1;
            }
            else {
                return self.tech_id;
            }
        }
    }

    pub fn new_tech_fam_id(&mut self) -> usize {
        loop {
            if self.technology_families.contains_key(&self.tech_fam_id) {
                self.tech_fam_id += 1;
            }
            else {
                return self.tech_fam_id;
            }
        }
    }

    pub fn new_product_id(&mut self) -> usize {
        loop {
            if self.products.contains_key(&self.product_id) {
                self.product_id += 1;
            }
            else {
                return self.product_id;
            }
        }
    }

    pub fn new_process_id(&mut self) -> usize {
        loop {
            if self.processes.contains_key(&self.process_id) {
                self.process_id += 1;
            }
            else {
                return self.process_id;
            }
        }
    }

    pub fn new_job_id(&mut self) -> usize {
        loop {
            if self.jobs.contains_key(&self.job_id) {
                self.job_id += 1;
            }
            else {
                return self.job_id;
            }
        }
    }

    pub fn new_species_id(&mut self) -> usize {
        loop {
            if self.species.contains_key(&self.species_id) {
                self.species_id += 1;
            }
            else {
                return self.species_id;
            }
        }
    }

    pub fn new_culture_id(&mut self) -> usize {
        loop {
            if self.cultures.contains_key(&self.culture_id) {
                self.culture_id += 1;
            }
            else {
                return self.culture_id;
            }
        }
    }

    pub fn new_pop_id(&mut self) -> usize {
        loop {
            if self.pops.contains_key(&self.pop_id) {
                self.pop_id += 1;
            }
            else {
                return self.pop_id;
            }
        }
    }

    pub fn new_territory_id(&mut self) -> usize {
        loop {
            if self.territories.contains_key(&self.territory_id) {
                self.territory_id += 1;
            }
            else {
                return self.territory_id;
            }
        }
    }

    pub fn new_market_id(&mut self) -> usize {
        loop {
            if self.markets.contains_key(&self.market_id) {
                self.market_id += 1;
            }
            else {
                return self.market_id;
            }
        }
    }

    pub fn new_firm_i(&mut self) -> usize {
        loop {
            if self.firms.contains_key(&self.firm_id) {
                self.firm_id += 1;
            }
            else {
                return self.firm_id;
            }
        }
    }
}

/// Sanity check functions, ensures no duplicate names and that
///  the items loaded equals the items in memory.
impl DataManager {
   
}