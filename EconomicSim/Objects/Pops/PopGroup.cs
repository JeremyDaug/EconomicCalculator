using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconomicSim.Objects.Firms;
using EconomicSim.Objects.Jobs;
using EconomicSim.Objects.Market;
using EconomicSim.Objects.Pops.Culture;
using EconomicSim.Objects.Pops.Species;
using EconomicSim.Objects.Products;
using EconomicSim.Objects.Skills;
using EconomicSim.Objects.Wants;

namespace EconomicSim.Objects.Pops
{
    /// <summary>
    /// Pop group data class.
    /// </summary>
    internal class PopGroup : IPopGroup
    {
        private readonly IReadOnlyList<(IProduct, decimal)> property;
        private readonly IReadOnlyList<(ISpecies species, int amount)> species;
        private readonly IReadOnlyList<(ICulture culture, int amount)> cultures;

        /// <summary>
        /// Pop Group Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The total number of people in the pop group.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// The job the pop group is attached to.
        /// </summary>
        public IJob Job { get; set; }

        /// <summary>
        /// The firm the pop group is attached to.
        /// </summary>
        public IFirm Firm { get; set; }

        /// <summary>
        /// The market the pop resides in.
        /// </summary>
        public IMarket Market { get; set; }

        /// <summary>
        /// The skill the pop has.
        /// </summary>
        public ISkill Skill => Job.Skill;

        /// <summary>
        /// The Skill Level of the Population Group.
        /// </summary>
        public decimal SkillLevel { get; set; }

        /// <summary>
        /// The property the population Owns.
        /// </summary>
        public IReadOnlyList<(IProduct product, decimal amount)> Property => property;

        /// <summary>
        /// The Species that makes up the pop.
        /// </summary>
        public IReadOnlyList<(ISpecies species, int amount)> Species => species;
        
        /// <summary>
        /// The cultures which make up in the Pop.
        /// </summary>
        public IReadOnlyList<(ICulture culture, int amount)> Cultures => cultures;
        
        /// <summary>
        /// The products desired by the pop every day.
        /// </summary>
        public IReadOnlyList<(IProduct prod, DesireTier tier, decimal amount)> Needs
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
        public IReadOnlyList<(IWant want, DesireTier tier, decimal amount)> Wants
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
    }
}
