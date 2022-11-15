pub struct SkillGroup {
    Id: u64,
    Name: String,
    Description: String,
    Default: f64,
    Skills: HashSet<Skill>
}