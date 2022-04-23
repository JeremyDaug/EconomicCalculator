using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        IReadOnlyList<IFirm> Firms { get; }
        
        // Governors

        IReadOnlyList<IJob> Jobs { get; }

        IReadOnlyList<IMarket> Markets { get; }

        IReadOnlyList<IPopGroup> Pops { get; }

        IReadOnlyList<ICulture> Cultures { get; }
        
        IReadOnlyList<ISpecies> Species { get; }
        
        IReadOnlyList<IProcess> Processes { get; }

        IReadOnlyList<IProduct> Products { get; }

        IReadOnlyList<ISkill> Skills { get; }

        IReadOnlyList<ISkillGroup> SkillGroups { get; }

        IReadOnlyList<ITechFamily> TechFamilies { get; }

        IReadOnlyList<ITechnology> Technologies { get; }

        IReadOnlyList<ITerritory> Territories { get; }

        IReadOnlyList<IWant> Wants { get; }

        void LoadData(IEnumerable<string> sets);
    }
}
