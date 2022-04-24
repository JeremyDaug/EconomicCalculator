using System.Data;
using System.Text.Json;
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
    /// Data Storage for our system.
    /// </summary>
    internal class DataContext : IDataContext
    {
        #region SingletonInstance

        private static bool _hasEverLoaded;

        private static DataContext? _instance;

        // Retrieve the currently active instance of our data.
        public static DataContext Instance
        {
            get { return _instance ??= new DataContext(); }
        }
        #endregion SingleTonInstance

        /// <summary>
        /// Default Constructor
        /// </summary>
        private DataContext()
        {
            var folder = Directory.GetCurrentDirectory();
            // TODO: Make this Crossplatform
            DataFolder = Path.GetFullPath(Path.Combine(folder, @"../../../../EconomicSim/Data/"));

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
        
        #region HelperFuncs

        private List<string> FindDuplicates(IList<string> group)
        {
            var result = new List<string>();
            var dupes = group.Distinct().ToList();
            if (group.Count != dupes.Count)
            {
                foreach (var dupe in dupes)
                    if (group.Count(x => x == dupe) > 1)
                        result.Add(dupe);
            }

            return result;
        }
        
        #endregion HelperFuncs

        #region Saving

        public void Save()
        {

        }

        #endregion Saving

        #region Loading

        private string GetDataFile(string set, string category)
        {
            // Data/set/category/categoryset.json
            //return Path.Combine("EconomicSim/Data", set, category, $"{set}{category}.json");
            return Path.Combine(DataFolder, set, category, $"{set}{category}.json");
        }

        private void LoadRequiredItems()
        {
            if (_hasEverLoaded)
                return;
            _hasEverLoaded = true;

            // load wants
            foreach (var want in RequiredItems.Wants.Values)
                Wants.Add((Want)want);

            // load techs
            foreach (var tech in RequiredItems.Technologies.Values)
                Technologies.Add((Technology.Technology)tech);

            // load products
            foreach (var product in RequiredItems.Products.Values)
                Products.Add((Product)product);
        }

        private void LoadWants(string set)
        {
            var filename = GetDataFile(set, "Wants");
            var json = File.ReadAllText(filename);

            var newWants = JsonSerializer.Deserialize<List<Want>>(json);
            if (newWants == null)
                throw new JsonException($"Wants file \"{filename}\" is empty.");

            // quickly check that the new wants are all unique
            var dups = newWants.Select(x => x.Name).Distinct().ToList();
            if (newWants.Count > dups.Count)
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

            if (newFamilies == null)
                throw new DataException($"Tech Family file \"{filename}\" is empty.");
            
            // check for dupes in set
            var dupes = newFamilies.Select(x => x.Name).Distinct().ToList();
            if (dupes.Count < newFamilies.Count)
            {
                // dupes found, find and throw it
                foreach (string dupe in dupes) if (newFamilies.Count(x => x.Name == dupe) > 1)
                        throw new InvalidDataException($"Duplicate Tech Family \"{dupe}\" found in set \"{set}\".");
            }

            // check for dupes in previously loaded sets and add to set
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

        private void LoadTechs(string set)
        {
            var filename = GetDataFile(set, "Technology");
            var json = File.ReadAllText(filename);

            var newTechs = JsonSerializer.Deserialize<List<Technology.Technology>>(json,
                new JsonSerializerOptions
                {
                    Converters =
                    {
                        new TechnologyJsonConverter()
                    }
                });

            // check for dupes in set
            var dupes = (newTechs ?? throw new DataException($"Technology file \"{filename}\" is empty."))
                .Select(x => x.Name)
                .Distinct().ToList();
            if (dupes.Count < newTechs.Count)
            {
                // dupes found, find it and throw
                foreach (var dupe in dupes) if (newTechs.Count(x => x.Name == dupe) > 1)
                        throw new InvalidDataException($"Duplicate Tech \"{dupe}\" found in set \"{set}\".");
            }

            // check for collision with previous sets
            foreach (var tech in newTechs)
            {
                if (Technologies.Select(x => x.Name)
                        .Contains(tech.Name))
                    throw new InvalidDataException($"Duplicate Tech \"{tech.Name}\" in set \"{set}\" found.");
                // add family connections
                var families = new List<TechFamily>();

                // tech must have at least one family
                if (tech.Families.Count == 0)
                    throw new InvalidDataException($"Tech \"{tech.Name}\" must belong to at least one family.");

                foreach (var fam in tech.Families)
                {
                    // get family target
                    var realfam = TechFamilies.Single(x => x.Name == fam.Name);
                    // add to family list
                    families.Add(TechFamilies.Single(x => x.Name == fam.Name));
                    // add to family
                    realfam.Techs.Add(tech);
                }
                tech.Families = families;
                // add tech to list.
                Technologies.Add(tech);
            }

            // connect to techs and families
            foreach (var tech in newTechs)
            {
                var parents = new List<Technology.Technology>();
                foreach (var parent in tech.Parents)
                {
                    parents.Add(Technologies.Single(x => x.Name == parent.Name));
                }
                tech.Parents = parents;

                var children = new List<Technology.Technology>();
                foreach (var child in tech.Children)
                    parents.Add(Technologies.Single(x => x.Name == child.Name));
                tech.Children = children;
            }

            // completed.
        }

        private void LoadProducts(string set)
        {
            var filename = GetDataFile(set, "Products");
            var json = File.ReadAllText(filename);

            var newProducts = JsonSerializer.Deserialize<List<Product>>(json,
                new JsonSerializerOptions
                {
                    Converters =
                    {
                        new ProductJsonConverter()
                    }
                });

            // check for dupes in set
            var dupes = (newProducts ?? throw new DataException($"Products file \"{filename}\" is empty."))
                .Select(x => x.Name).Distinct().ToList();
            if (dupes.Count() < newProducts.Count())
            {
                foreach (var dupe in dupes) if (newProducts.Count(x => x.Name == dupe) > 1)
                        throw new InvalidDataException($"Duplicate Product \"{dupe}\" found in set \"{set}\".");
            }

            // check for collisions with previous sets
            foreach (var prod in newProducts)
            {
                if (Products.Select(x => x.Name)
                    .Contains(prod.Name))
                    throw new InvalidDataException($"Duplicate Product \"{prod.Name}\" in set \"{set}\" found.");

                Products.Add(prod);
            }

            // Processes connect to products, so no connections here.

            // complete.
        }

        private void LoadSkills(string set)
        {
            var filename = GetDataFile(set, "Skills");
            var json = File.ReadAllText(filename);

            var newSkills = JsonSerializer.Deserialize<List<Skill>>(json,
                new JsonSerializerOptions
                {
                    Converters =
                    {
                        new SkillJsonConverter()
                    }
                });
            if (newSkills == null)
                throw new DataException($"Skill file \"{filename}\" is empty.");

            // check for duplicates
            var dupes = newSkills.Select(x => x.Name).Distinct().ToList();
            if (dupes.Count() < newSkills.Count())
            {
                foreach (var dupe in dupes) if (newSkills.Count(x => x.Name == dupe) > 1)
                        throw new InvalidDataException($"Duplicate skill \"{dupe}\" found in set \"{set}\".");
            }

            // check for collisions with previous sets.
            foreach (var skill in newSkills)
            {
                if (Skills.Select(x => x.Name)
                    .Contains(skill.Name))
                    throw new InvalidDataException($"Duplicate skill \"{skill}\" found in set \"{set}\".");

                Skills.Add(skill);
            }

            // connect relations up.
            foreach (var skill in newSkills)
            {
                var relations = new List<(Skill relation, decimal rate)>();
                foreach (var rel in skill.Relations)
                {
                    relations.Add((Skills.Single(x => x.Name == rel.relation.Name), rel.rate));
                }
                skill.Relations = relations;
            }
            // skill groups connect to skills.
            // finished
        }

        private void LoadSkillGroups(string set)
        {
            var filename = GetDataFile(set, "SkillGroups");
            var json = File.ReadAllText(filename);

            var newGroups = JsonSerializer.Deserialize<List<SkillGroup>>(json,
                new JsonSerializerOptions
                {
                    Converters =
                    {
                        new SkillGroupJsonConverter()
                    }
                });
            if (newGroups == null)
                throw new DataException($"Skill Group file \"{filename}\" is empty.");
            
            // check new set for duplicates
            var dupes = FindDuplicates(newGroups.Select(x => x.Name).ToList());
            if (dupes.Count > 0)
            {
                throw new DataException($"Skill Group(s) \"{string.Join(", ", dupes)}\" within set \"{set}\" of has been found.");
            }
            
            // check for duplicates in old sets
            foreach (var dupe in newGroups.Select(x => x.Name))
            {
                if (SkillGroups.Select(x => x.Name).Any(x => x == dupe))
                    throw new DataException($"Duplicate Skill Group \"{dupe}\" found in set \"{set}\"");
            }
            
            // add new skillGroups
            foreach (var newGroup in newGroups)
                SkillGroups.Add(newGroup);
            // complete
        }
        
        /// <summary>
        /// Loads all of the data from the common folder from the sets given.
        /// </summary>
        /// <param name="sets">Which Sets to load from files.</param>
        /// <exception cref="ArgumentNullException">If sets is null.</exception>
        public void LoadData(IEnumerable<string> sets)
        {
            LoadRequiredItems();
            
            foreach (var set in sets)
            {
                LoadWants(set);

                LoadTechFamilies(set);

                LoadTechs(set);

                LoadProducts(set);

                LoadSkills(set);
                
                LoadSkillGroups(set);
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
