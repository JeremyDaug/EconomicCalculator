
/// Actors, these have AI attached to them, make active decisions, and try
/// to satisfy desires either of their own or of others.
/// 
/// It contains
/// - Markets, which don't act, but are a nexus of action for all of
/// these actors.
/// - Pops, the people who work in the markets as well as buy and consume
/// the goods from the market.
/// - Firms, the productive organizations where people work to produce various
/// things.
/// - Institutions, various groups who are not inherently economic in
/// organization.
/// - States, formalized institution(s) who have gained a domineering position
/// in a market, and have gained the right to monopolize legitimate violence,
/// and extract taxes.
/// 
/// Pops and Firms must be contained by the market. Institutions and States
/// do not need to be contained, but should have delegate/representative in
/// the market to represent and communicate with their them in their capital.
#[derive(Debug)]
pub struct Actors {
    /// The markets managed here.
    pub Markets: Hashmap<usize, Market>,
    /// The Pops within the managed markets.
    pub Pops: HashMap<usize, Pop>,
    /// The firms within the Managed Markets
    pub Firms: Hashmap<usize, Firm>,
    // Institutions
    // States
}