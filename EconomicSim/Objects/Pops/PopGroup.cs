using System.Text.Json.Serialization;
using EconomicSim.Helpers;
using EconomicSim.Objects.Firms;
using EconomicSim.Objects.Jobs;
using EconomicSim.Objects.Market;
using EconomicSim.Objects.Pops.Culture;
using EconomicSim.Objects.Pops.Species;
using EconomicSim.Objects.Products;
using EconomicSim.Objects.Skills;

namespace EconomicSim.Objects.Pops
{
    /// <summary>
    /// Pop group data class.
    /// </summary>
    [JsonConverter(typeof(PopJsonConverter))]
    public class PopGroup : IPopGroup
    {
        public PopGroup()
        {
            Property = new List<(Product product, decimal amount)>();
            Species = new List<(Species.Species species, int amount)>();
            Cultures = new List<(Culture.Culture culture, int amount)>();
        }

        /// <summary>
        /// Pop Group Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// A Generated name for a Pop Group, should be synthesized from
        /// Market, Firm, and Job.
        /// </summary>
        public string Name => $"{Job.Name} of {Firm.Name} in {Market.Name}";

        /// <summary>
        /// The total number of people in the pop group.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// The job the pop group is attached to.
        /// </summary>
        public Job Job { get; set; }
        IJob IPopGroup.Job => Job;

        /// <summary>
        /// The firm the pop group is attached to.
        /// </summary>
        public Firm Firm { get; set; }
        IFirm IPopGroup.Firm => Firm;

        /// <summary>
        /// The market the pop resides in.
        /// </summary>
        public Market.Market Market { get; set; }
        IMarket IPopGroup.Market => Market;

        /// <summary>
        /// The skill the pop has.
        /// </summary>
        public Skill Skill { get; set; }
        ISkill IPopGroup.Skill => Skill;

        /// <summary>
        /// The lower Skill Level of the Population Group.
        /// </summary>
        public decimal LowerSkillLevel { get; set; }
        
        /// <summary>
        /// The Highest Skill Level of the Population Group.
        /// </summary>
        public decimal HigherSkillLevel { get; set; }

        /// <summary>
        /// The property the population Owns.
        /// </summary>
        public List<(Product product, decimal amount)> Property { get; set; }
        IReadOnlyList<(IProduct product, decimal amount)> IPopGroup.Property
            => Property.Select(x => ((IProduct) x.product, x.amount)).ToList();

        /// <summary>
        /// The Species that makes up the pop.
        /// </summary>
        public List<(Species.Species species, int amount)> Species { get; set; }
        IReadOnlyList<(ISpecies species, int amount)> IPopGroup.Species
            => Species.Select(x => ((ISpecies) x.species, x.amount)).ToList();

        /// <summary>
        /// The cultures which make up in the Pop.
        /// </summary>
        public List<(Culture.Culture culture, int amount)> Cultures { get; set; }
        IReadOnlyList<(ICulture culture, int amount)> IPopGroup.Cultures 
            => Cultures.Select(x => ((ICulture)x.culture, x.amount)).ToList();
        
        /// <summary>
        /// The products desired by the pop every day.
        /// </summary>
        public IReadOnlyList<INeedDesire> Needs
        {
            get
            {
                /*var result = new List<(IProduct product, DesireTier tier, decimal amount)>();

                // for each species
                foreach (var spe in Species)
                {
                    // go through their needs
                    foreach (var need in spe.species.Needs)
                    {
                        // if it's already in the list, add to that
                        var existing = result
                            .SingleOrDefault(x => x.product.Id == need.Product.Id &&
                                                  x.tier == need.Tier);

                        if (!existing.Equals(default))
                        {
                            existing.amount += spe.amount * need.Amount;
                        }
                        else
                        { // else add it to the list.
                            result.Add((need.Product, tier: need.Tier, need.Amount * spe.amount));
                        }
                    }
                }

                // for each culture
                foreach (var culture in cultures)
                {
                    // go through their needs
                    foreach (var need in culture.culture.Needs)
                    {
                        var existing = result
                            .SingleOrDefault(x => x.product.Id == need.product.Id &&
                                                  x.tier == need.tier);
                        if (!existing.Equals(default))
                        {
                            existing.amount += culture.amount * need.amount;
                        }
                        else
                        {
                            result.Add((need.product, need.tier, need.amount * culture.amount));
                        }
                    }
                }

                return result;*/
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// The Wants desired by the pop every day.
        /// </summary>
        public IReadOnlyList<IWantDesire> Wants
        {
            get
            {
                /* var result = new List<(IWant want, DesireTier tier, decimal amount)>();

                // for each species
                foreach (var spe in Species)
                {
                    // go through their needs
                    foreach (var want in spe.species.Wants)
                    {
                        // if it's already in the list, add to that
                        var existing = result
                            .SingleOrDefault(x => x.want.Id == want.Want.Id &&
                                                  x.tier == want.Tier);

                        if (!existing.Equals(default))
                        {
                            existing.amount += spe.amount * want.Amount;
                        }
                        else
                        { // else add it to the list.
                            result.Add((want: want.Want, tier: want.Tier, want.Amount * spe.amount));
                        }
                    }
                }

                // for each culture
                foreach (var culture in cultures)
                {
                    // go through their needs
                    foreach (var wants in culture.culture.Wants)
                    {
                        var existing = result
                            .SingleOrDefault(x => x.want.Id == wants.want.Id &&
                                                  x.tier == wants.tier);
                        if (!existing.Equals(default))
                        {
                            existing.amount += culture.amount * wants.amount;
                        }
                        else
                        {
                            result.Add((wants.want, wants.tier, wants.amount * culture.amount));
                        }
                    }
                }

                return result;*/
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the hours of the population group
        /// Currently this is just 16 hours per day, but should be updated to change the
        /// rate based on species/culture/etc
        /// </summary>
        public decimal GetTotalHours()
        {
            // TODO make this more flexible and use available productivity based on the population's details (species, culture, etc).
            return Count * 16;
        }
    }
}
