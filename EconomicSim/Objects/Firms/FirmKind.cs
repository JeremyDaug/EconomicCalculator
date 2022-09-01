namespace EconomicSim.Objects.Firms;

public enum FirmKind
{
    // TODO perhaps split this up into more things
    // In particular, splitting off subsistence into it's own thing, would be good.
    // As well as any 'unemployed' jobs.
    /// <summary>
    /// The firm produces something from other things.
    /// </summary>
    Workshop,
    /// <summary>
    /// The firm is a local shop, which buys goods to resell
    /// them on the market.
    /// </summary>
    Shopkeeper,
    /// <summary>
    /// The firm is a Local Merchant who travels to travels between
    /// neighboring Markets to buy and sell goods for a profit.
    /// Buy locally to sell Abroad Buy abroad to sell locally.
    /// </summary>
    Merchant,
    /// <summary>
    /// The firm is a merchant who travels a long distance and over many
    /// days for goods. Often with a large vehicle or vessel.
    /// </summary>
    Trader,
    /// <summary>
    /// This firm is specialised not in making products but destroying them.
    /// As such, it looks for things which can be recycled or otherwise removed
    /// from the market for a profit.
    /// </summary>
    Scrapper,
    /// <summary>
    /// The firm doesn't actually want to function as part of the market.
    /// Instead it simply focuses on meeting it's own desires and maybe
    /// selling the excess (if it isn't taxed away).
    /// </summary>
    Subsistence,
    /// <summary>
    /// This firm, and the people in it, are nomadic, travelling around between
    /// various markets, seeking the highest returns for their work.
    /// </summary>
    Nomadic,
    /// <summary>
    /// These pops are both nomadic and subsistence level, they travel around, but
    /// rather than seeking to profit from working with the markets, they are instead
    /// separate from it. May be dangerous.
    /// </summary>
    SubsistenceNomad,
    /// <summary>
    /// This firm is a cultural touchstone of some kind, like a church or
    /// similar institution. As such, it typically doesn't sell things,
    /// instead surviving off of donations.
    /// </summary>
    Cultural,
    /// <summary>
    /// This firm is part of a political entity, typically an institution,
    /// but also possibly part of a government directly. As such it is
    /// funded not by selling but rather by taxes.
    /// </summary>
    Political,
    /// <summary>
    /// This firm isn't actually a firm, but a military organization.
    /// It's primary export is violence, it's income is expropriation,
    /// and it's inputs are the tools of war.
    /// </summary>
    Military
}