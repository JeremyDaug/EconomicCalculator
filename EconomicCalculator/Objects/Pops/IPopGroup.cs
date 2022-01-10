using EconomicCalculator.Objects.Firms;
using EconomicCalculator.Objects.Jobs;
using EconomicCalculator.Objects.Market;
using EconomicCalculator.Objects.Pops.Culture;
using EconomicCalculator.Objects.Pops.Species;
using EconomicCalculator.Objects.Products;
using EconomicCalculator.Objects.Skills;
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
        /// Their level in the skill.
        /// Skill is contained at <see cref="Job.Skill"/>.
        /// </summary>
        decimal SkillLevel { get; }

        /// <summary>
        /// The Property of the group.
        /// </summary>
        IReadOnlyList<(IProduct, decimal)> Property { get; }

        /// <summary>
        /// The species that make up this pop.
        /// </summary>
        IReadOnlyList<(ISpecies, int)> Species { get; }

        /// <summary>
        /// The cultures that make up this pop.
        /// </summary>
        IReadOnlyList<(ICulture, int)> Cultures { get; }
    }
}
