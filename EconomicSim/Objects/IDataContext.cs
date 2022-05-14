using EconomicSim.Objects.Firms;
using EconomicSim.Objects.Jobs;
using EconomicSim.Objects.Market;
using EconomicSim.Objects.Pops;
using EconomicSim.Objects.Pops.Culture;
using EconomicSim.Objects.Pops.Species;
using EconomicSim.Objects.Processes;
using EconomicSim.Objects.Products;
using EconomicSim.Objects.Skills;
using EconomicSim.Objects.Technology;
using EconomicSim.Objects.Territory;
using EconomicSim.Objects.Wants;

namespace EconomicSim.Objects
{
    /// <summary>
    /// Data storage for all general data in the system.
    /// </summary>
    public interface IDataContext
    {
        IReadOnlyDictionary<string, IFirm> Firms { get; }

        // Governors

        IReadOnlyDictionary<string, IJob> Jobs { get; }

        IReadOnlyDictionary<string, IMarket> Markets { get; }

        IReadOnlyDictionary<string, IPopGroup> Pops { get; }

        IReadOnlyDictionary<string, ICulture> Cultures { get; }
        
        IReadOnlyDictionary<string, ISpecies> Species { get; }
        
        IReadOnlyDictionary<string, IProcess> Processes { get; }

        IReadOnlyDictionary<string, IProduct> Products { get; }

        IReadOnlyDictionary<string, ISkill> Skills { get; }

        IReadOnlyDictionary<string, ISkillGroup> SkillGroups { get; }

        IReadOnlyDictionary<string, ITechFamily> TechFamilies { get; }

        IReadOnlyDictionary<string, ITechnology> Technologies { get; }

        IReadOnlyDictionary<string, ITerritory> Territories { get; }

        IReadOnlyDictionary<string, IWant> Wants { get; }

        void LoadData(IEnumerable<string> sets);

        void LoadSave(string save);
    }
}
