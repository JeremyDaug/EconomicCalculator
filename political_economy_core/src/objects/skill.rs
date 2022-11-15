pub struct Skill {
    Id: u64,
    Name: String,
    Description: String,
    Labor: Product,
    SkillGroups: HashSet<SkillGroup>,
    RelatedSkills: HashMap<Skill, f64>,
}