using EconomicCalculator.Objects.Firms;
using EconomicCalculator.Objects.Jobs;
using EconomicCalculator.Objects.Market;
using EconomicCalculator.Objects.Pops.Culture;
using EconomicCalculator.Objects.Pops.Species;
using EconomicCalculator.Objects.Products;
using EconomicCalculator.Objects.Skills;
using EconomicCalculator.Objects.Wants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Objects.Pops
{
    public interface IPopGroup
    {
        /// <summary>
        /// Pop Id
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The Total Size of the Population group.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// The job the Pop Group does.
        /// </summary>
        IJob Job { get; }

        /// <summary>
        /// The business they work at.
        /// </summary>
        IFirm Firm { get; }

        /// <summary>
        /// The market the pop is in.
        /// </summary>
        IMarket Market { get; }

        /// <summary>
        /// The skill the pop has.
        /// </summary>
        ISkill Skill { get; }

        /// <summary>
        /// Their level in the skill.
        /// Skill is contained at <see cref="Job.Skill"/>.
        /// </summary>
        decimal SkillLevel { get; }

        /// <summary>
        /// The Property of the group.
        /// </summary>
        IReadOnlyList<(IProduct product, decimal amount)> Property { get; }

        /// <summary>
        /// The species that make up this pop.
        /// </summary>
        IReadOnlyList<(ISpecies species, int amount)> Species { get; }

        /// <summary>
        /// The cultures that make up this pop.
        /// </summary>
        IReadOnlyList<(ICulture culture, int amount)> Cultures { get; }

        /// <summary>
        /// The products desired by this pop.
        /// </summary>
        IReadOnlyList<(IProduct prod, DesireTier tier, decimal amount)> Needs { get; }

        /// <summary>
        /// The wants desired by this pop.
        /// </summary>
        IReadOnlyList<(IWant want, DesireTier tier, decimal amount)> Wants { get; }
    }
}
