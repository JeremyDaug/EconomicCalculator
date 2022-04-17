﻿using EconomicCalculator.Objects.Firms;
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
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EconomicCalculator.Objects
{
    /// <summary>
    /// Data Storage for our system.
    /// </summary>
    internal class DataContext : IDataContext
    {
        #region SingletonInstance

        private static DataContext instance;

        // Retrieve the currently active instance of our data.
        public static DataContext Instance
        {
            get
            {
                if (instance == null)
                    instance = new DataContext();
                return instance;
            }
        }
        #endregion SingleTonInstance

        /// <summary>
        /// Default Constructor
        /// </summary>
        private DataContext()
        {
            var folder = Directory.GetCurrentDirectory();
            DataFolder = Path.GetFullPath(Path.Combine(folder, @"..\..\..\EconomicCalculator\Data\"));

            SaveFolder = Path.Combine(DataFolder, "Saves");

            // TODO remove this later.
            CurrentSaveFolder = "Default";

            Wants = new List<Want>();
            TechFamilies = new List<TechFamily>();
            Technologies = new List<Technology.Technology>();
            Products = new List<Product>();
            SkillGroups = new List<SkillGroup>();
            Skills = new List<Skill>();
            Processes = new List<Process>();
            Jobs = new List<Job>();
            Species = new List<Species>();
            Cultures = new List<Culture>();
            Pops = new List<PopGroup>();
            Territories = new List<Territory.Territory>();
            Markets = new List<Market.Market>();
            Firms = new List<Firm>();
        }

        #region Saving

        public void Save()
        {

        }

        #endregion Saving

        #region Loading

        private string GetDataFile(string set, string category)
        {
            // Data/set/category/categoryset.json
            return Path.Combine(DataFolder, set, category, $"{set}{category}.json");
        }

        private void LoadWants(string set)
        {
            var filename = GetDataFile(set, "Wants");
            var json = File.ReadAllText(filename);

            var newWants = JsonSerializer.Deserialize<List<Want>>(json);

            // quickly check that the new wants are all unique
            var dups = newWants.Select(x => x.Name).Distinct();
            if (newWants.Count() > dups.Count())
            {
                // We have a dupe. find it and throw it.
                foreach (var dup in dups) if (newWants.Count(x => x.Name == dup) > 1)
                    throw new InvalidDataException($"Duplicate want named \"{dup}\" found in set \"{set}\".");
            }

            foreach (var want in newWants)
            {
                // check that there are no duplicates with existing sets
                // TODO when duplicates found, deal with it.
                if (Wants.Select(x => x.Name)
                        .Contains(want.Name))
                {
                    throw new InvalidDataException($"Duplicate Want of name \"{want.Name}\" from Set \"{set}\" found.");
                }
                // if no clash add to our list
                Wants.Add(want);
            }
        }

        private void LoadTechFamilies(string set)
        {
            var filename = GetDataFile(set, "TechFamilies");
            var json = File.ReadAllText(filename);

            var newFamilies = JsonSerializer.Deserialize<List<TechFamily>>(json, 
                new JsonSerializerOptions
                {
                    Converters =
                    {
                        new TechFamilyJsonConverter()
                    }
                });

            // check for dupes in set
            var dupes = newFamilies.Select(x => x.Name).Distinct();
            if (dupes.Count() < newFamilies.Count())
            {
                // dupes found, find and throw it
                foreach (string dupe in dupes) if (newFamilies.Count(x => x.Name == dupe) > 1)
                        throw new InvalidDataException($"Duplicate Tech Family \"{dupe}\" found in set \"{set}\".");
            }

            // check for dups in previously loaded sets and add to set
            foreach (var fam in newFamilies)
            {
                if (TechFamilies.Select(x => x.Name)
                        .Contains(fam.Name))
                    throw new InvalidDataException($"Duplicate Tech Family \"{fam.Name}\" in set \"{set}\" found.");
                TechFamilies.Add(fam);
            }

            // connect tech families to each other.
            foreach (var fam in newFamilies)
            {
                List<TechFamily> relations = new List<TechFamily>();
                foreach (var rel in fam.Relations.Select(x => x.Name))
                    relations.Add(TechFamilies.Single(x => x.Name == rel));
                fam.Relations = relations;
            }

            // Connect techs only after they are loaded.
            // we're done.
        }

        /// <summary>
        /// Loads all of the data from the common folder from the sets given.
        /// </summary>
        /// <param name="sets">Which Sets to load from files.</param>
        /// <exception cref="ArgumentNullException">If sets is null.</exception>
        public void LoadData(IEnumerable<string> sets)
        {
            foreach (var set in sets)
            {
                LoadWants(set);

                LoadTechFamilies(set);
            }
        }

        #endregion Loading

        #region FileStorage

        public string DataFolder { get; }
        public string JobsFolder => "Jobs";
        public string ProcessesFolder => "Processes";
        public string ProductsFolder => "Products";
        public string SkillGroupsFolder => "SkillGroups";
        public string SkillsFolder => "Skills";
        public string TechFamiliesFolder => "TechFamilies";
        public string TechnologyFolder => "Technology";
        public string WantsFolder => "Wants";
        public string CulturesFolder => "Cultures";
        public string SpeciesFolder => "Species";

        public string SaveFolder { get; }
        public string CurrentSaveFolder { get; }
        public string CurrentFirmFolder => "Firms";
        public string CurrentMarketFolder => "Markets";
        public string CurrentPopsFolder => "Pops";
        public string CurrentTerritoriesFolder => "Territories";

        public string DefaultIcon => @"ProductImages\DefaultIcon.png";

        #endregion FileStorage

        #region DataStorage

        public List<Want> Wants { get; set; }
        IReadOnlyList<IWant> IDataContext.Wants => Wants;

        public List<TechFamily> TechFamilies { get; set; }
        IReadOnlyList<ITechFamily> IDataContext.TechFamilies => TechFamilies;

        public List<Technology.Technology> Technologies { get; set; }
        IReadOnlyList<ITechnology> IDataContext.Technologies => Technologies;

        public List<Product> Products { get; set; }
        IReadOnlyList<IProduct> IDataContext.Products => Products;

        public List<SkillGroup> SkillGroups { get; set; }
        IReadOnlyList<ISkillGroup> IDataContext.SkillGroups => SkillGroups;

        public List<Skill> Skills { get; set; }
        IReadOnlyList<ISkill> IDataContext.Skills => Skills;

        public List<Process> Processes { get; set; }
        IReadOnlyList<IProcess> IDataContext.Processes => Processes;

        public List<Job> Jobs { get; set; }
        IReadOnlyList<IJob> IDataContext.Jobs => Jobs;

        public List<Species> Species { get; set; }
        IReadOnlyList<ISpecies> IDataContext.Species => Species;

        public List<Culture> Cultures { get; set; }
        IReadOnlyList<ICulture> IDataContext.Cultures => Cultures;

        public List<PopGroup> Pops { get; set; }
        IReadOnlyList<IPopGroup> IDataContext.Pops => Pops;

        public List<Territory.Territory> Territories { get; set; }
        IReadOnlyList<ITerritory> IDataContext.Territories => Territories;

        public List<Market.Market> Markets { get; set; }
        IReadOnlyList<IMarket> IDataContext.Markets => Markets;

        public List<Firm> Firms { get; set; }
        IReadOnlyList<IFirm> IDataContext.Firms => Firms;

        #endregion DataStorage
    }
}
