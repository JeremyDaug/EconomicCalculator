use std::collections::HashSet;

use super::{seller::Seller, buyer::Buyer, firm_job::FirmJob};

/// Firms are the productive actors of our system.
/// 
/// They can represent disorganized groups of solo firms, like family farms,
/// to large companies and corporations. When other groups need to act, they
/// have a firm contained within them to do the work.
/// 
/// They buy inputs for the jobs they have, and sell the goods produced at a
/// price. They are harder to please through barter, making indirect exchange
/// more encouraged.
#[derive(Debug)]
pub struct Firm {
    /// The Unique Id for a firm.
    pub id: usize,
    /// The Firm's name
    pub name: String,
    /// The variant name for a firm, used if the Firm is a child of another 
    /// firm or other organization.
    pub variant_name: String,
    /// What kind of firm it is, alters the logic of the firm 
    pub firm_kind: FirmKind,
    /// The rank of the firm. How high it is in it's firm tree.
    pub firm_rank: FirmRank,
    /// How is the firm owned.
    pub ownership_type: OwnershipStructure,
    /// How the profit of the Firm is distributed.
    pub profit_structure: ProfitStructure,
    /// How the firm is organized, or how it attempts to organize itself.
    pub organization_strucutre: OrganizationalStructure,
    /// The ids for the firm's children (subfirms).
    /// May be empty.
    pub children: Vec<usize>,
    /// The id for a parent firm, Firm may not have a parent.
    pub parent: Option<usize>,
    /// The jobs in the firm. Rank 1 firms can only have 1 here.
    /// Also contains the pops and assignments.
    pub jobs: Vec<FirmJob>,
    /// The management jobs the firm uses. The first in the list is it's 
    /// preferred, the others are those which it either must have or which
    /// it is transitioning away from.
    /// 
    /// Also contains the pops and assignments.
    /// 
    /// Unavailable for Disorganized Firms.
    pub management: Vec<FirmJob>,
    /// The ownership jobs the firm uses. The first is the primary and preferred
    /// option, the others are either required by the structure, or that is being
    /// transitioned away from.
    /// 
    /// Also contains the pops and assignments.
    pub ownership: Vec<FirmJob>,
    /// The prices of the products the firm sells.
    /// Stores the ID of the product and the price in AMV it seeks from
    /// the market.
    pub prices: HashSet<usize, f64>,
    /// The property owned or otherwise managed by the firm.
    /// If the firm is not Disorganized or otherwise a distinct entity from
    /// the pop, this is where all of it's inputs and capital is stored.
    pub property: HashSet<usize, f64>,
    firm_outputs: Vec<usize>,
}

impl Firm {

    pub fn get_name(&self) -> String {
        format!("{}({})", self.name, self.variant_name)
    }
}

impl Seller for Firm {}

impl Buyer for Firm {}

#[derive(Debug)]
pub enum FirmRank {
    /// Firms are the smallest kind of business.
    /// Capable of only a few jobs, a primary job, a management job, and
    /// an owner job. Little else.
    /// 
    /// Can only exist in a single market, and lacks the ability to form
    /// a proper subdivisions, and finds it difficult to research new things
    /// actively.
    Firm,
    /// The Second Rank, capable of internal specialization through subfirms
    /// as well as expansion into other markets. It also has access to more 
    /// jobs, and more levels of management, allowing for greater 
    /// organizational gains.
    Company,
    /// Third Rank of a firm. More Options, More Expansion, more self
    /// contained specialization, management, and ownership options.
    /// 
    /// Can become an institution under the right circumstances.
    Corporation,
    /// Fourth rank, Even more of the above, but also capable of becoming a
    /// full state under the right circumstances, and easily becomes an
    /// institution.
    Megacorporation
}

/// What kind of firm this is.
/// 
/// This defines the overarching logic for how the firm will function, as well
/// as enable or disable certain features.
#[derive(Debug)]
pub enum FirmKind {
    /// The default or generic option, for those with little or no special
    /// logic.
    Workshop,
    /// A shop which buys local goods in order to store and resell them
    /// at a profit.
    Retailer,
    /// A Local merchant, IE, a merchant who travels between his home market
    /// and it's neigbors. They buy goods, transport them across the market 
    /// border, and sells them on the other side.
    Merchant,
    /// A Long Distance Merchant. The firm travels between various markets,
    /// buying and selling goods for a profit, only to travel on and repeat.
    Trader,
    /// A Firm specialized in advancing the destruction of products, like
    /// turning swords into iron for other uses.
    Scrapper,
    /// A Subsistence Firm, focuses it's logic on supporting itself alone.
    /// Does not normally offer goods to the market, instead creating offers
    /// for others to find and sells directly from the pop to the market.
    Subsistence,
    /// This firm, and the people in it are nomadic, capable of travelling
    /// between various markets, seeking the highest return for their work.
    Nomadic,
    /// A Combination of Subsistence firm and Nomadic firm. They travel 
    /// around, but instead of seeking higher profits, they optimize for 
    /// raw output. Herding is is a good example.
    SubsistenceNomad,
    /// The firm is a cultural firm, producing something that is cultural
    /// in nature, like a church or similar institution. As such, it typically
    /// offers services for cheap or free, and asks for charity in return.
    Cultural,
    //Political,
    /// The firm is not actually a firm, but instead it represents a military
    /// force. It does not produce regular goods, but instead creates violence.
    /// Depending on it's use, it can be used for security, opression, or theft.
    Military,
}

/// How the profits of the firm are distributed.
#[derive(Debug)]
pub enum ProfitStructure {
    /// The profits are distributed equally to all who own, work, or otherwise
    /// are attached to the firm. This is often for LossSharing, Disorganized,
    /// or other self-owned firms.
    Distributed,
    /// The firm has shares, and uses those to distribute profits.
    Shares,
    /// There is one share, and it belongs to the owner pop of a firm.
    PrivatelyOwned,
    /// The profits are shared out, but the owners get the lions share.
    ProfitSharing,
    /// Profits are not distributed, but are used to either invest in the
    /// company or to reduce prices.
    NonProfit
}

/// How the firm's ownership is structured.
/// 
/// Defines some features available to the firm.
#[derive(Debug)]
pub enum OwnershipStructure {
    /// It is not a structly organized firm, but instead a collection of
    /// small firms that are not working together. Think family farms.
    /// 
    /// Can only have a single job, no others.
    /// 
    /// Lacks any efficiency gain methods via organization.
    /// 
    /// Must use 
    /// - OrganizationalStructure::DisorganizedStructure
    /// - WageType::LossSharing
    /// - ProfitStructure::Distributed
    /// 
    /// Not strictly limited, but should avoid
    /// - FirmKind::Retailer
    /// - FirmKind::Merchant
    /// - FirmKind::Trader
    SelfEmployed,
    /// A Loose organization of individual or small business owner/operators.
    /// 
    /// Can be used at any level above Firm.
    /// 
    /// The children firms are self-owning, and the owners of those firms
    /// each have an equal share in their Association Parent.
    Association,
    /// A more formal and tightly organized collection of individual or
    /// small business owner/operators.
    /// 
    /// Technically available above any level of firm, but most common at
    /// Company level.
    /// 
    /// Tends to be limited to a singular Market.
    /// 
    /// Child Firms are self-directing in the day-to-day, but are limited in
    /// what they offer, and have restrictions on how they set prices.
    /// 
    /// Guilds also are ruled primarily by the most skilled members.
    Guild,
    /// The Firm is a private business. It is owned by one or a small group of
    /// people, the owners.
    /// 
    /// They are capable of anything, and have no restrictions in what they
    /// may do, but all choices are made by the owner, and for their profit.
    /// 
    /// While owning a Private firm gives the owner total control and access
    /// to all profits, he is also liable for any debts or liabilities.
    Private,
    /// The Firm is a Worker Cooperative, with the workers being given
    /// partial ownership of the firm. 
    /// 
    /// All decisions are available for influence from the workers, and there
    /// is no strict 'owner'. Managers are given more day-to-day running power.
    /// 
    /// May be of any scale.
    /// 
    /// There is no 'owner' job in these firms.
    /// 
    /// Profit must be destributed among the owners (workers) and these shares
    /// cannot be bought, sold, or otherwise exchanged. They are created upon
    /// joining and lost upon leaving.
    /// 
    /// If a Firm is a WorkerCooperative, then all of that firm's children must
    /// also be a WorkerCooperative.
    WorkerCooperative,
    /// The firm is a Cooperative for producers, for their benefit. As such
    /// grain farmers collectively owning a mill, or warehouse for storing
    /// their grain.
    /// 
    /// The Child Firms of this organization may be of any kind.
    /// 
    /// Producer Cooperatives are controlled by their children. The ownership
    /// of the parent is split among the owners of the children. They are
    /// typically non-profit organizations, with the purpose of passing on 
    /// savings and profits to their member-owners.
    /// 
    /// This should be always of Company Level or Higher.
    ProducerCooperative,
    /// The firm is a Consumer Cooperative.
    /// 
    /// The child firms of this firm may be in any form.
    /// 
    /// Consumer Cooperatives are controlled by their children. And Ownership
    /// is split among it's members equally. They focus on non-profit and 
    /// try to pass along any savings to the consumers who own them.
    /// 
    /// This should always be of company level or higher.
    ConsumerCooperative,
    /// The Firm is publicly traded. It has a number of shares which are
    /// freely bought, traded, and sold. These shares can include or exclude
    /// - voting rights (ownership controls)
    /// - Dividends (Share of Profit)
    /// - other rights or liabilities
    /// 
    /// Shares and their mechanics are explained elsewhere.
    /// 
    /// The Shareholders are the ultimate owners. The Owner position is instead
    /// replaced by the Board of Directors, who are selected by the 
    /// Shareholders. The highest level Managers become the Chief Officers
    /// (CEO, COO, CFO, etc).
    /// 
    /// While not explicitly restricted to any level, this tends to be
    /// more common the larger and higher ranked the company.
    PubliclyTraded,
    /// The firm is controlled by various stakeholders, often this comes
    /// through a mixture of shareholders, worker representation,
    /// consumer or producer representatives, and the like. The exact
    /// mixture is defined by the firm in question.
    Stakeholders,
    /// The firm is explicitly a state owned and run organization. There is no
    /// explicit owner, though there may be a Board to fulfill the role.
    /// 
    /// Managers are Bureaucrats, selected by the state, and answerable to
    /// them.
    /// 
    /// Employees are considered state employeees, and likewise answerable to
    /// the state.
    StateOwned,
    /// The firm is owned by a specific institution. Typically, this is used
    /// to distinguish between a state made entity and a firm or similar actor
    /// which is integral to a specific Institution.
    /// 
    /// For Example, a state with an integrated Church may have it's churches
    /// considered institutional and belong to the Church rather than being
    /// being under the direct purview of the State as a whole.
    /// 
    /// State Firms can become Institutional as management is delegated and
    /// ownership is concentrated.
    /// 
    /// Institutional Firms can also become state firms if the process
    /// reverses, but is less likely as institutions fight to hold on
    /// more strongly than the state.
    Institutional,
}

/// An enum which defines how a firm organizes itself and it's children,
/// as well as how tightly it and it's children are bound together.
#[derive(Debug)]
pub enum OrganizationalStructure {
    /// The firm is not organized at all, it is a collection of small
    /// business in a market.
    Disorganized,
    /// The firm is a small, solo busines, typically a small business with a
    /// few employees. It cannot have children. Think a local workshop or
    /// mom and pop diner style.
    SmallBusiness,
    /// A local firm capable of having children. Those children are owned
    /// and operated separately, but can take orders from the guild to 
    /// change, their operations, typically adjusting prices and wages.
    /// 
    /// Tends to stay at Firm Rank 0 or 1 (Firm or Company).
    /// 
    /// The guild also offers minor services, giving them a small efficiency
    /// boost.
    Guild,
    /// The firm intentionally reduces it's hight, minimizing middle 
    /// management. Upper management are closer to employees and employees
    /// can me shifted to middle management roles trivially.
    /// 
    /// Predominantly used in Companies and Firms. (1 and 0)
    Flat,
    /// Divides the company into subcomponents focused on particular jobs
    /// or duties, like marketing, R&D, Sales, Operations, etc.
    /// 
    /// Authority is centralized into the higher levels, with few local
    /// manageres having much authority.
    /// 
    /// Tends to be used for Companies and Corporations.
    Functional,
    /// Organizes itself around products, projects, and subsidiaries rather
    /// than specifically specialized sections. Divisions are given much more
    /// independence.
    /// Very Common.
    Divisional,
    /// The Firm is organized around a Franchise model, where the children are 
    /// typically copy pasted firms, with high managerial independence, but 
    /// low product indepence. The Parent Firm decides what is offered, at what
    /// price, and also typically sets up supply deals for material inputs and
    /// capital.
    /// 
    /// Very Common.
    Franchise,
    /// The Firm is Decentralized. Lower Firm Ranks are effectively 
    /// independent from the higher ranks, capable of doing almost anything
    /// on their own. Higher Ranks are instead organizational placeholders,
    /// or firms which produce something used among the lower levels.
    /// Higher levels have little to no authority.
    /// 
    /// Uncommon, typically unstable also.
    Decentralized,
}