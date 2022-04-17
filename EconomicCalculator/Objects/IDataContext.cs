using EconomicCalculator.Objects.Firms;
using EconomicCalculator.Objects.Jobs;
using EconomicCalculator.Objects.Market;
using EconomicCalculator.Objects.Pops;
using EconomicCalculator.Objects.Pops.Culture;
using EconomicCalculator.Objects.Pops.Species;
using EconomicCalculator.Objects.Processes;
using EconomicCalculator.Objects.Products;
using EconomicCalculator.Objects.Skills;
using EconomicCalculator.Objects.Technology;
using EconomicCalculator.Objects.Territory;
using EconomicCalculator.Objects.Wants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Objects
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
