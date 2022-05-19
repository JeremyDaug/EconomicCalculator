using EconomicSim.Objects.Firms;
using EconomicSim.Objects.Jobs;
using EconomicSim.Objects.Pops;
using EconomicSim.Objects.Pops.Culture;
using EconomicSim.Objects.Pops.Species;
using EconomicSim.Objects.Processes;
using EconomicSim.Objects.Products;
using EconomicSim.Objects.Skills;
using EconomicSim.Objects.Technology;
using EconomicSim.Objects.Wants;

namespace EconomicSim.Objects
{
    /// <summary>
    /// Data storage for all general data in the system.
    /// </summary>
    public interface IDataContext
    {
        SortedList<string, Firm> Firms { get; }

        // Governors

        SortedList<string, Job> Jobs { get; }

        SortedList<string, Market.Market> Markets { get; }

        SortedList<string, PopGroup> Pops { get; }

        SortedList<string, Culture> Cultures { get; }
        
        SortedList<string, Species> Species { get; }
        
        SortedList<string, Process> Processes { get; }

        SortedList<string, Product> Products { get; }

        SortedList<string, Skill> Skills { get; }

        SortedList<string, SkillGroup> SkillGroups { get; }

        SortedList<string, TechFamily> TechFamilies { get; }

        SortedList<string, Technology.Technology> Technologies { get; }

        SortedList<string, Territory.Territory> Territories { get; }

        SortedList<string, Want> Wants { get; }

        void LoadData(IEnumerable<string> sets);

        void LoadSave(string save);
        
        // TODO update these for sets later
        void SaveWants();
        void SaveTechnologies();
        void SaveTechFamilies();
        void SaveSkills();
        void SaveSkillGroups();
        void SaveProducts();
        void SaveProcesses();
        void SaveSpecies();
        void SaveCultures();
        void SaveJobs();
        void SaveGame();
    }
}
