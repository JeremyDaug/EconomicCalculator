use core::panic;
use std::collections::HashMap;

use crate::objects::{want::{Want, self}, skill_group::SkillGroup, skill::Skill, technology::Technology, technology_family::TechnologyFamily, product::Product};


#[derive(Debug)]
pub struct DataManager {
    // Stand Alone items

    pub wants: HashMap<u64, Want>,

    pub technology: HashMap<u64, Technology>,
    pub technology_families: HashMap<u64, TechnologyFamily>,

    pub products: HashMap<u64, Product>,

    pub skills: HashMap<u64, Skill>,
    pub skill_groups: HashMap<u64, SkillGroup>
}

impl DataManager {
    /// Loads wants from a file into the data manager,
    /// Currently, this just loads pre-existing data.
    pub fn load_wants(&self, file_name: String) {
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

    pub fn load_technologies(&self, file_name: String) {

    }

    pub fn load_products(&self, file_name: String) {
        // Generic Time
        let time = Product::new()
        // Shopping Time
        // Ambrosia Fruit (food source)
        // cotton boll
        // cotton thread
        // cotton bolt
        // cotton clothes
        // cotton suit
        // wood logs
        // wood gatherer stick
        // wood spinning wheel
        // wood loom
        // wood-stone axe
        // Stone
        // hut (no resource, low efficiency, un-maintainable)
        // cabin (costs wood, medium efficiency, maintainable)
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