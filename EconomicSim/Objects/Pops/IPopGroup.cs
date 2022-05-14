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
    [JsonConverter(typeof(PopJsonConverter))]
    public interface IPopGroup
    {
        /// <summary>
        /// Pop Id
        /// </summary>
        int Id { get; }

        /// <summary>
        /// A Generated name for a Pop Group, should be synthesized from
        /// Market, Firm, and Job.
        /// </summary>
        string Name { get; }
        
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
        /// The lower Skill Level of the Population Group.
        /// </summary>
        decimal LowerSkillLevel { get; set; }
        
        /// <summary>
        /// The Highest Skill Level of the Population Group.
        /// </summary>
        decimal HigherSkillLevel { get; set; }

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
        IReadOnlyList<INeedDesire> Needs { get; }

        /// <summary>
        /// The wants desired by this pop.
        /// </summary>
        IReadOnlyList<IWantDesire> Wants { get; }
    }
}
