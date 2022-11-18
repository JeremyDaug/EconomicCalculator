use core::panic;
use std::collections::{HashMap, HashSet};

use crate::objects::{want::Want, skill_group::SkillGroup, skill::Skill, technology::Technology, technology_family::TechnologyFamily, product::Product, process_node::ProcessNode, process::Process, job::Job, species::Species, culture::Culture, pop::Pop, market::Market, firm::Firm};


#[derive(Debug)]
pub struct DataManager {
    // Stand Alone items

    pub wants: HashMap<u64, Want>,

    pub technology: HashMap<u64, Technology>,
    pub technology_families: HashMap<u64, TechnologyFamily>,

    pub products: HashMap<u64, Product>,

    pub skill_groups: HashMap<u64, SkillGroup>,
    pub skills: HashMap<u64, Skill>,

    pub processes: HashMap<u64, Process>,
    pub process_nodes: HashMap<u64, ProcessNode>,

    pub jobs: HashMap<u64, Job>,

    pub species: HashMap<u64, Species>,
    pub cultures: HashMap<u64, Culture>,

    pub pops: HashMap<u64, Pop>,

    pub territories: HashMap<u64, Species>,
    pub markets: HashMap<u64, Market>,
    pub firms: HashMap<u64, Firm>,
    pub sets: Vec<String>
}

impl DataManager {
    fn new() -> Self {
        Self { 
            wants: HashMap::new(),
            technology: HashMap::new(), 
            technology_families: HashMap::new(), 
            products: HashMap::new(), 
            skill_groups: HashMap::new(),
            skills: HashMap::new(),
            processes: HashMap::new(),
            process_nodes: HashMap::new(),
            jobs: HashMap::new(),
            species: HashMap::new(),
            cultures: HashMap::new(),
            pops: HashMap::new(),
            territories: HashMap::new(),
            markets: HashMap::new(),
            firms: HashMap::new(),
            sets: Vec::new()
        }
    }

    /// Loads wants from a file into the data manager,
    /// Currently, this just loads pre-existing data.
    pub fn load_wants(&mut self, file_name: String) {
        let rest =  match Want::new(0, 
            String::from("Rest"), 
            String::from("Rest is the joy of Idle time."), 
            0.1) {
                Option::None => panic!(),
                Option::Some(want) => want
            };

        let food = match Want::new(
            1,
            String::from("Food"),
            String::from("Food is the desire for sustenance, necissary for all living things."),
            0.2) {
                Option::None => panic!(),
                Option::Some(want) => want
            };

        let shelter = match Want::new(
            2,
            String::from("Shelter"),
            String::from("Shelter is the protection from the elements, a space where the difficulties of the outside world are lessened and made tolerable."),
            0.2) {
            Option::None => panic!(),
            Option::Some(want) => want
        };

        let clothing = match Want::new(
            3,
            String::from("Clothing"),
            String::from("Clothing is the personal protection from the elements, while it does not separate one from the wider world wholly, it does lessen it's toll."),
            0.2) {
            Option::None => panic!(),
            Option::Some(want) => want
        };

        let clothing = match Want::new(
            4,
            String::from("Clothing"),
            String::from("Clothing is the personal protection from the elements, while it does not separate one from the wider world wholly, it does lessen it's toll."),
            0.2) {
            Option::None => panic!(),
            Option::Some(want) => want
        };

        let fashion = match Want::new(
            5,
            String::from("Fashion"),
            String::from("Fashion is about presentation, showing your wealth through jewelry, and higher quality clothing."),
            0.2) {
            Option::None => panic!(),
            Option::Some(want) => want
        };

        let wealth = match Want::new(
            6,
            String::from("Wealth"),
            String::from("Wealth is the amount of things you have built up. Not just money, but things. This is a required item."),
            0.2) {
            Option::None => panic!(),
            Option::Some(want) => want
        };

        self.wants.insert(rest.id(), rest);
        self.wants.insert(food.id(), food);
        self.wants.insert(shelter.id(), shelter);
        self.wants.insert(clothing.id(), clothing);
        self.wants.insert(fashion.id(), fashion);
        self.wants.insert(wealth.id(), wealth);

    }

    pub fn load_technologies(&mut self, _file_name: String) {
        todo!("Not doing right now. Have better things to do than test out technology rules.")
    }

    pub fn load_products(&mut self, _file_name: String) {
        // Generic Time
        let time = Product::new(0,
            String::from("Time"),
            String::from(""),
            String::from("Time. Always is short supply."), 
            String::from("Hour(s)"), 
            0,
            0.0,
            0.0,
            Some(0),
            true,
            Vec::new(),
            HashMap::new(),
            HashSet::new(),
            None,
            HashSet::new(),
            HashSet::new(),
            HashSet::new(),
            None).unwrap();
        // Shopping Time
        let shoppingTime = Product::new(1,
            String::from("Time"),
            String::from(""),
            String::from("Shopping Time, productive, but sometimes frustrating."), 
            String::from("Hour(s)"), 
            0,
            0.0,
            0.0,
            Some(0),
            true,
            Vec::new(),
            HashMap::new(),
            HashSet::new(),
            None,
            HashSet::new(),
            HashSet::new(),
            HashSet::new(),
            None).unwrap();
        // Ambrosia Fruit (food source)
        let ambrosiaFruit = Product::new(2,
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
            HashMap::new(),
            HashSet::new(),
            None,
            HashSet::new(),
            HashSet::new(),
            HashSet::new(),
            None).unwrap();
        // cotton boll
        let cottonBoll = Product::new(3,
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
            HashMap::new(),
            HashSet::new(),
            None,
            HashSet::new(),
            HashSet::new(),
            HashSet::new(),
            None).unwrap();
        // cotton thread
        let cottonThread = Product::new(4,
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
            HashMap::new(),
            HashSet::new(),
            None,
            HashSet::new(),
            HashSet::new(),
            HashSet::new(),
            None).unwrap();
        // cotton bolt
        let cottonBolt = Product::new(5,
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
            HashMap::new(),
            HashSet::new(),
            None,
            HashSet::new(),
            HashSet::new(),
            HashSet::new(),
            None).unwrap();
        // cotton clothes
        let cottonClothes = Product::new(6,
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
            HashMap::new(),
            HashSet::new(),
            None,
            HashSet::new(),
            HashSet::new(),
            HashSet::new(),
            None).unwrap();
        // cotton suit
        let cottonSuit = Product::new(7,
            String::from("Suit"),
            String::from("Cotton"),
            String::from("Cotton Suit, a better set of clothes, looks nice."), 
            String::from("Set(s)"), 
            3,
            2.5,
            0.015,
            Some(50),
            false,
            Vec::new(),
            HashMap::new(),
            HashSet::new(),
            None,
            HashSet::new(),
            HashSet::new(),
            HashSet::new(),
            None).unwrap();
        // wood logs
        let woodLogs = Product::new(8,
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
            HashMap::new(),
            HashSet::new(),
            None,
            HashSet::new(),
            HashSet::new(),
            HashSet::new(),
            None).unwrap();
        // wood gatherer stick
        let woodGathererSticks = Product::new(9,
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
            HashMap::new(),
            HashSet::new(),
            None,
            HashSet::new(),
            HashSet::new(),
            HashSet::new(),
            None).unwrap();
        // wood spinning wheel
        let spinningWheel = Product::new(10,
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
            HashMap::new(),
            HashSet::new(),
            None,
            HashSet::new(),
            HashSet::new(),
            HashSet::new(),
            None).unwrap();
        // wood loom
        let woodLoom = Product::new(11,
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
            HashMap::new(),
            HashSet::new(),
            None,
            HashSet::new(),
            HashSet::new(),
            HashSet::new(),
            None).unwrap();
        // wood-stone axe
        let woodStoneAxe = Product::new(12,
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
            HashMap::new(),
            HashSet::new(),
            None,
            HashSet::new(),
            HashSet::new(),
            HashSet::new(),
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
            HashMap::new(),
            HashSet::new(),
            None,
            HashSet::new(),
            HashSet::new(),
            HashSet::new(),
            None).unwrap();
        // hut (no resource, low efficiency, un-maintainable)
        let hut = Product::new(14,
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
            HashMap::new(),
            HashSet::new(),
            None,
            HashSet::new(),
            HashSet::new(),
            HashSet::new(),
            None).unwrap();
        // cabin (costs wood, medium efficiency, maintainable)
        let cabin = Product::new(15,
            String::from("Cabin"),
            String::from(""),
            String::from("Cabin, warm, sturdy, and homely."), 
            String::from("Cabin(s)"), 
            3,
            250.0,
            30,
            Some(60),
            false,
            Vec::new(),
            HashMap::new(),
            HashSet::new(),
            None,
            HashSet::new(),
            HashSet::new(),
            HashSet::new(),
            None).unwrap();
        // (labors and Services)
        // Ambrosia Farming
        let ambrosiaFarming = Product::new(16,
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
            HashMap::new(),
            HashSet::new(),
            None,
            HashSet::new(),
            HashSet::new(),
            HashSet::new(),
            None).unwrap();
        // Cotton Farming
        let cottonFarming = Product::new(17,
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
            HashMap::new(),
            HashSet::new(),
            None,
            HashSet::new(),
            HashSet::new(),
            HashSet::new(),
            None).unwrap();
        // Thread Spinning
        let threadSpinning = Product::new(18,
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
            HashMap::new(),
            HashSet::new(),
            None,
            HashSet::new(),
            HashSet::new(),
            HashSet::new(),
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
            HashMap::new(),
            HashSet::new(),
            None,
            HashSet::new(),
            HashSet::new(),
            HashSet::new(),
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
            HashMap::new(),
            HashSet::new(),
            None,
            HashSet::new(),
            HashSet::new(),
            HashSet::new(),
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
            HashMap::new(),
            HashSet::new(),
            None,
            HashSet::new(),
            HashSet::new(),
            HashSet::new(),
            None).unwrap();
        // tool making
        let toolMaking = Product::new(22,
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
            HashMap::new(),
            HashSet::new(),
            None,
            HashSet::new(),
            HashSet::new(),
            HashSet::new(),
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
            HashMap::new(),
            HashSet::new(),
            None,
            HashSet::new(),
            HashSet::new(),
            HashSet::new(),
            None).unwrap();
        // building repair
        let buildingRepair = Product::new(24,
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
            HashMap::new(),
            HashSet::new(),
            None,
            HashSet::new(),
            HashSet::new(),
            HashSet::new(),
            None).unwrap();
        // stone gathering
        let stoneGathering = Product::new(25,
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
            HashMap::new(),
            HashSet::new(),
            None,
            HashSet::new(),
            HashSet::new(),
            HashSet::new(),
            None).unwrap();

        self.products.insert(time.id(), time);
        self.products.insert(shoppingTime.id(), shoppingTime);
        self.products.insert(ambrosiaFruit.id(), ambrosiaFruit);
        self.products.insert(cottonBoll.id(), cottonBoll);
        self.products.insert(cottonThread.id(), cottonThread);
        self.products.insert(cottonBolt.id(), cottonBolt);
        self.products.insert(cottonClothes.id(), cottonClothes);
        self.products.insert(cottonSuit.id(), cottonSuit);
        self.products.insert(woodLogs.id(), woodLogs);
        self.products.insert(woodGathererSticks.id(), woodGathererSticks);
        self.products.insert(spinningWheel.id(), spinningWheel);
        self.products.insert(woodLoom.id(), woodLoom);
        self.products.insert(woodStoneAxe.id(), woodStoneAxe);
        self.products.insert(stone.id(), stone);
        self.products.insert(hut.id(), hut);
        self.products.insert(cabin.id(), cabin);
        self.products.insert(ambrosiaFarming.id(), ambrosiaFarming);
        self.products.insert(cottonFarming.id(), cottonFarming);
        self.products.insert(threadSpinning.id(), threadSpinning);
        self.products.insert(weaving.id(), weaving);
        self.products.insert(tailoring.id(), tailoring);
        self.products.insert(lumbering.id(), lumbering);
        self.products.insert(toolMaking.id(), toolMaking);
        self.products.insert(construction.id(), construction);
        self.products.insert(buildingRepair.id(), buildingRepair);
        self.products.insert(stoneGathering.id(), stoneGathering);
    }

    pub fn load_skills(&self, file_name: String) {
        // (labors and Services)
        // Ambrosia Farming
        // Cotton Farming
        // Thread Spinning
        // Weaving
        // Tailoring
        // lumbering
        // tool making
        // construction
        // building repair
        // stone gathering
    }

}