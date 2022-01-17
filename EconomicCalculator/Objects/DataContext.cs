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
    /// Data Context Storage
    /// </summary>
    internal class DataContext : IDataContext
    {

        public DataContext()
        {
            Firms = new List<IFirm>();
        }

        public List<IFirm> Firms { get; set; }
        IReadOnlyList<IFirm> IDataContext.Firms => Firms;

        public List<IJob> Jobs { get; set; }
        IReadOnlyList<IJob> IDataContext.Jobs => Jobs;

        public List<IMarket> Markets { get; set; }
        IReadOnlyList<IMarket> IDataContext.Markets => Markets;

        IReadOnlyList<IPopGroup> IDataContext.Pops => throw new NotImplementedException();

        IReadOnlyList<ICulture> IDataContext.Cultures => throw new NotImplementedException();

        IReadOnlyList<ISpecies> IDataContext.Species => throw new NotImplementedException();

        IReadOnlyList<IProcess> IDataContext.Processes => throw new NotImplementedException();

        IReadOnlyList<IProduct> IDataContext.Products => throw new NotImplementedException();

        IReadOnlyList<ISkill> IDataContext.Skills => throw new NotImplementedException();

        IReadOnlyList<ISkillGroup> IDataContext.SkillGroups => throw new NotImplementedException();

        IReadOnlyList<ITechFamily> IDataContext.TechFamilies => throw new NotImplementedException();

        IReadOnlyList<ITechnology> IDataContext.Technologies => throw new NotImplementedException();

        IReadOnlyList<ITerritory> IDataContext.Territories => throw new NotImplementedException();

        IReadOnlyList<IWant> IDataContext.Wants => throw new NotImplementedException();
    }
}
