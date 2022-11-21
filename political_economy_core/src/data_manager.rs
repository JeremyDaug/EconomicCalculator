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
    pub fn load_wants(&mut self, _file_name: String) {
        let rest =  match Want::new(0, 
            String::from("Rest"), 
            String::from("Rest is the joy of Idle time."), 
            0.1) {
                Result::Err(_) => panic!(),
                Result::Ok(want) => want
            };

        let food = match Want::new(
            1,
            String::from("Food"),
            String::from("Food is the desire for sustenance, necissary for all living things."),
            0.2) {
                Result::Err(_) => panic!(),
                Result::Ok(want) => want
            };

        let shelter = match Want::new(
            2,
            String::from("Shelter"),
            String::from("Shelter is the protection from the elements, a space where the difficulties of the outside world are lessened and made tolerable."),
            0.2) {
                Result::Err(_) => panic!(),
                Result::Ok(want) => want
        };

        let clothing = match Want::new(
            3,
            String::from("Clothing"),
            String::from("Clothing is the personal protection from the elements, while it does not separate one from the wider world wholly, it does lessen it's toll."),
            0.2) {
                Result::Err(_) => panic!(),
                Result::Ok(want) => want
        };

        let fashion = match Want::new(
            4,
            String::from("Fashion"),
            String::from("Fashion is about presentation, showing your wealth through jewelry, and higher quality clothing."),
            0.2) {
                Result::Err(_) => panic!(),
                Result::Ok(want) => want
        };

        let wealth = match Want::new(
            5,
            String::from("Wealth"),
            String::from("Wealth is the amount of things you have built up. Not just money, but things. This is a required item."),
            0.2) {
                Result::Err(_) => panic!(),
                Result::Ok(want) => want
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

    pub fn load_technology_families(&mut self, _file_name: String) {
        todo!("Skipping for same reason as Load Technologies.")
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
            None).unwrap();
        // Shopping Time
        let shopping_time = Product::new(1,
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
            None).unwrap();
        // cotton suit
        let mut cotton_suit = Product::new(7,
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
            None).unwrap();
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
            None).unwrap();
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
            None).unwrap();
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
            None).unwrap();

        let mut shelter = self.wants
            .get_mut(&2).unwrap();
        hut.connect_want(&mut shelter, 1.0).expect("Big Problem");
        cabin.connect_want(&mut shelter, 1.5).expect("Big Problem");

        let clothes = self.wants
            .get_mut(&3).unwrap();
        cotton_clothes.set_want(&clothes, 1.0).expect("Big Problem");
        cotton_suit.set_want(&clothes,1.5).expect("Big Problem");

        let fashion = self.wants
            .get_mut(&4).unwrap();
        cotton_suit.set_want(fashion,1.0).expect("Big Problem");

        let wealth = self.wants
            .get_mut(&4).unwrap();
        cotton_suit.set_want(wealth,1.0).expect("Big Problem");
        cabin.set_want(wealth, 2.0).expect("Big Problem");

        self.products.insert(time.id(), time);
        self.products.insert(shopping_time.id(), shopping_time);
        self.products.insert(ambrosia_fruit.id(), ambrosia_fruit);
        self.products.insert(cotton_boll.id(), cotton_boll);
        self.products.insert(cotton_thread.id(), cotton_thread);
        self.products.insert(cotton_bolt.id(), cotton_bolt);
        self.products.insert(cotton_clothes.id(), cotton_clothes);
        self.products.insert(cotton_suit.id(), cotton_suit);
        self.products.insert(wood_logs.id(), wood_logs);
        self.products.insert(wood_gatherer_sticks.id(), wood_gatherer_sticks);
        self.products.insert(spinning_wheel.id(), spinning_wheel);
        self.products.insert(wood_loom.id(), wood_loom);
        self.products.insert(wood_stone_axe.id(), wood_stone_axe);
        self.products.insert(stone.id(), stone);
        self.products.insert(hut.id(), hut);
        self.products.insert(cabin.id(), cabin);
        self.products.insert(ambrosia_farming.id(), ambrosia_farming);
        self.products.insert(cotton_farming.id(), cotton_farming);
        self.products.insert(thread_spinning.id(), thread_spinning);
        self.products.insert(weaving.id(), weaving);
        self.products.insert(tailoring.id(), tailoring);
        self.products.insert(lumbering.id(), lumbering);
        self.products.insert(tool_making.id(), tool_making);
        self.products.insert(construction.id(), construction);
        self.products.insert(building_repair.id(), building_repair);
        self.products.insert(stone_gathering.id(), stone_gathering);
    }

    pub fn load_skill_groups(&mut self, _file_name: String) {
        // farming 
        let mut farming_skills = HashSet::new();
        farming_skills.insert(0);
        farming_skills.insert(1);
        let mut farming = SkillGroup::new(0,
            String::from("Agriculture"),
            String::from("While the details differ, a farm is a farm."),
            0.4, farming_skills).unwrap();
        let a_farming = self.skills.get_mut(&0).unwrap();
        a_farming.connect_skill_group(&mut farming);
        let c_farming = self.skills.get_mut(&1).unwrap();
        c_farming.connect_skill_group(&mut farming);

        // clothing
        let mut clothing_skills = HashSet::new();
        clothing_skills.insert(1);
        clothing_skills.insert(2);
        clothing_skills.insert(3);
        clothing_skills.insert(4);
        let mut clothier = SkillGroup::new(1,
            String::from("Clothier"),
            String::from("Dealing with thread, no matter it's form, is fairly similar."),
            0.5, clothing_skills).unwrap();
        c_farming.connect_skill_group(&mut clothier);
        let spinning = self.skills.get_mut(&2).unwrap();
        spinning.connect_skill_group(&mut clothier);
        let weaving = self.skills.get_mut(&3).unwrap();
        weaving.connect_skill_group(&mut clothier);
        let tailoring = self.skills.get_mut(&4).unwrap();
        tailoring.connect_skill_group(&mut clothier);

        // carpentry
        let mut carpentry_skills = HashSet::new();
        carpentry_skills.insert(5);
        carpentry_skills.insert(6);
        carpentry_skills.insert(7);
        carpentry_skills.insert(8);
        let mut carpentry = SkillGroup::new(1,
            String::from("Carpentry"),
            String::from("Working with wood, no matter the purpose, has plenty in common."),
            0.5, carpentry_skills).unwrap();
        let skill = self.skills.get_mut(&5).unwrap();
        skill.connect_skill_group(&mut carpentry);
        let skill = self.skills.get_mut(&6).unwrap();
        skill.connect_skill_group(&mut carpentry);
        let skill = self.skills.get_mut(&7).unwrap();
        skill.connect_skill_group(&mut carpentry);
        let skill = self.skills.get_mut(&8).unwrap();
        skill.connect_skill_group(&mut carpentry);

        // architecture
        let mut architecture_skills = HashSet::new();
        architecture_skills.insert(7);
        architecture_skills.insert(8);
        let mut architecture = SkillGroup::new(1,
            String::from("Architecture"),
            String::from("Making and repairing buildings is always similar."),
            0.5, architecture_skills).unwrap();
        let skill = self.skills.get_mut(&7).unwrap();
        skill.connect_skill_group(&mut architecture);
        let skill = self.skills.get_mut(&8).unwrap();
        skill.connect_skill_group(&mut architecture);
    }

    pub fn load_skills(&mut self, _file_name: String) {
        // (labors and Services)
        // Ambrosia Farming
        let mut ambrosia_farming = Skill::new(0,
            String::from("Ambrosia Farming"),
            String::from("Ambrosia Farming"),
            16);
        // Cotton Farming
        let mut cotton_farming = Skill::new(1,
            String::from("Cotton Farming"),
            String::from("Cotton Farming"),
            17);
        // Thread Spinning
        let mut thread_spinning = Skill::new(2,
            String::from("Thread Spinning"),
            String::from("Thread Spinning"),
            18);
        // Weaving
        let mut weaving = Skill::new(3,
            String::from("Weaving"),
            String::from("Weaving"),
            19);
        // Tailoring
        let mut tailoring = Skill::new(4,
            String::from("Tailoring"),
            String::from("Tailoring"),
            20);
        // lumbering
        let mut lumbering = Skill::new(5,
            String::from("Lumbering"),
            String::from("Lumbering"),
            21);
        // tool making
        let mut tool_making = Skill::new(6,
            String::from("Tool Making"),
            String::from("Tool Making"),
            22);
        // construction
        let mut construction = Skill::new(7,
            String::from("Construction"),
            String::from("Construction"),
            23);
        // building repair
        let mut building_repair = Skill::new(8,
            String::from("Building Repair"),
            String::from("Building Repair"),
            24);
        // stone gathering
        let stone_gathering = Skill::new(9,
            String::from("Stone Gathering"),
            String::from("Stone Gathering"),
            25);

        // add skill interconnections
        // ambrosia and cotton farming are connected at 0.75 efficiency
        ambrosia_farming.set_mutual_relation(&mut cotton_farming, 0.75);
        // cotton farming, spinning, weaving, and tailoring relate
        // cotton 
        cotton_farming.set_mutual_relation(&mut thread_spinning, 0.2);
        cotton_farming.set_mutual_relation(&mut weaving, 0.2);
        cotton_farming.set_mutual_relation(&mut tailoring, 0.2);
        // thread
        thread_spinning.set_mutual_relation(&mut weaving, 0.5);
        thread_spinning.set_mutual_relation(&mut tailoring, 0.5);
        // weaving
        weaving.set_mutual_relation(&mut tailoring, 0.75);
        // lumbering
        lumbering.set_mutual_relation(&mut tool_making, 0.3);
        // construction - building repair
        construction.set_mutual_relation(&mut building_repair, 0.8);

        self.skills.insert(ambrosia_farming.id(), ambrosia_farming);
        self.skills.insert(cotton_farming.id(), cotton_farming);
        self.skills.insert(thread_spinning.id(), thread_spinning);
        self.skills.insert(weaving.id(), weaving);
        self.skills.insert(tailoring.id(), tailoring);
        self.skills.insert(lumbering.id(), lumbering);
        self.skills.insert(tool_making.id(), tool_making);
        self.skills.insert(construction.id(), construction);
        self.skills.insert(building_repair.id(), building_repair);
        self.skills.insert(stone_gathering.id(), stone_gathering);
    }


}