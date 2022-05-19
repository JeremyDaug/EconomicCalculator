using System.Data;
using System.Text.Json;
using EconomicSim.Enums;
using EconomicSim.Objects.Firms;
using EconomicSim.Objects.Jobs;
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

            Wants = new SortedList<string, Want>();
            TechFamilies = new SortedList<string, TechFamily>();
            Technologies = new SortedList<string, Technology.Technology>();
            Products = new SortedList<string, Product>();
            SkillGroups = new SortedList<string, SkillGroup>();
            Skills = new SortedList<string, Skill>();
            Processes = new SortedList<string, Process>();
            Jobs = new SortedList<string, Job>();
            Species = new SortedList<string, Species>();
            Cultures = new SortedList<string, Culture>();
            Pops = new SortedList<string, PopGroup>();
            Territories = new SortedList<string, Territory.Territory>();
            Markets = new SortedList<string, Market.Market>();
            Firms = new SortedList<string, Firm>();
            Sets = new List<string>();
        }
        
        #region HelperFuncs

        private List<string> FindDuplicates(IEnumerable<string> group)
        {
            var result = new List<string>();
            var enumerable = group.ToList();
            var dupes = enumerable.Distinct().ToList();
            if (enumerable.Count() != dupes.Count)
            {
                foreach (var dupe in dupes)
                    if (enumerable.Count(x => x == dupe) > 1)
                        result.Add(dupe);
            }

            return result;
        }
        
        #endregion HelperFuncs

        #region Saving

        public void SaveAllData()
        {
            
        }

        public void SaveGame()
        {
            
        }

        
        // TODO, update these for set mechanics.
        public void SaveWants()
        {
            var filename = GetDataFile("Common", "Wants");
            var json = JsonSerializer.Serialize(Wants.Values,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            File.WriteAllText(filename, json);
        }

        public void SaveTechnologies()
        {
            var filename = GetDataFile("Common", "Technology");
            var json = JsonSerializer.Serialize(Technologies.Values,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            File.WriteAllText(filename, json);
        }

        public void SaveTechFamilies()
        {
            var filename = GetDataFile("Common", "TechFamilies");
            var json = JsonSerializer.Serialize(TechFamilies.Values,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            File.WriteAllText(filename, json);
        }
        
        public void SaveSkills()
        {
            var filename = GetDataFile("Common", "Skills");
            var json = JsonSerializer.Serialize(Skills.Values,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            File.WriteAllText(filename, json);
        }

        public void SaveSkillGroups()
        {
            var filename = GetDataFile("Common", "SkillGroups");
            var json = JsonSerializer.Serialize(SkillGroups.Values,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            File.WriteAllText(filename, json);
        }
        
        public void SaveProducts()
        {
            var filename = GetDataFile("Common", "Products");
            var json = JsonSerializer.Serialize(Products.Values,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            File.WriteAllText(filename, json);
        }
        
        public void SaveProcesses()
        {
            var filename = GetDataFile("Common", "Processes");
            var json = JsonSerializer.Serialize(Processes.Values,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            File.WriteAllText(filename, json);
        }
        
        public void SaveSpecies()
        {
            var filename = GetDataFile("Common", "Species");
            var json = JsonSerializer.Serialize(Species.Values,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            File.WriteAllText(filename, json);
        }
        
        public void SaveCultures()
        {
            var filename = GetDataFile("Common", "Cultures");
            var json = JsonSerializer.Serialize(Cultures.Values,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            File.WriteAllText(filename, json);
        }
        
        public void SaveJobs()
        {
            var filename = GetDataFile("Common", "Jobs");
            var json = JsonSerializer.Serialize(Jobs.Values,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            File.WriteAllText(filename, json);
        }
        
        #endregion Saving

        #region Loading

        #region Data
        
        private string GetDataFile(string set, string category)
        {
            // Data/set/category/categoryset.json
            //return Path.Combine("EconomicSim/Data", set, category, $"{set}{category}.json");
            return Path.Combine(DataFolder, "Common", category, $"{set}{category}.json");
        }

        private void LoadRequiredItems()
        {
            if (_hasEverLoaded)
                return;
            _hasEverLoaded = true;

            // load wants
            // foreach (var want in RequiredItems.Wants.Values)
            //    Wants.Add(want.Name, (Want)want); 

            // load techs
            foreach (var tech in RequiredItems.Technologies.Values)
                Technologies.Add(tech.Name, (Technology.Technology)tech);

            // load products
            foreach (var product in RequiredItems.Products.Values)
                Products.Add(product.GetName(), (Product)product);
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
                // TODO when duplicates found, deal with it.
                try
                { // try to add
                    Wants.Add(want.Name, want);
                }
                catch (ArgumentException e)
                { // if clash, rethrow with more data.
                    throw new InvalidDataException($"Duplicate Want of name \"{want.Name}\" from Set \"{set}\" found.");
                }
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
                if (TechFamilies.ContainsKey(fam.Name))
                    throw new InvalidDataException($"Duplicate Tech Family \"{fam.Name}\" in set \"{set}\" found.");
                TechFamilies.Add(fam.Name, fam);
            }

            // connect tech families to each other.
            foreach (var fam in newFamilies)
            {
                List<TechFamily> relations = new List<TechFamily>();
                foreach (var rel in fam.Relations.Select(x => x.Name))
                    relations.Add(TechFamilies[rel]);
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
                if (Technologies.ContainsKey(tech.Name))
                    throw new InvalidDataException($"Duplicate Tech \"{tech.Name}\" in set \"{set}\" found.");
                // add family connections
                var families = new List<TechFamily>();

                // tech must have at least one family
                if (tech.Families.Count == 0)
                    throw new InvalidDataException($"Tech \"{tech.Name}\" must belong to at least one family.");

                foreach (var fam in tech.Families)
                {
                    // get family target
                    var realfam = TechFamilies[fam.Name];
                    // add to family list
                    families.Add(TechFamilies[fam.Name]);
                    // add to family
                    realfam.Techs.Add(tech);
                }
                tech.Families = families;
                // add tech to list.
                Technologies.Add(tech.Name, tech);
            }

            // connect to techs and families
            foreach (var tech in newTechs)
            {
                var parents = new List<Technology.Technology>();
                foreach (var parent in tech.Parents)
                {
                    parents.Add(Technologies[parent.Name]);
                }
                tech.Parents = parents;

                var children = new List<Technology.Technology>();
                foreach (var child in tech.Children)
                    parents.Add(Technologies[child.Name]);
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
                if (Products.ContainsKey(prod.GetName()))
                    throw new InvalidDataException($"Duplicate Product \"{prod.Name}\" in set \"{set}\" found.");

                Products.Add(prod.GetName(), prod);
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
                if (Skills.ContainsKey(skill.Name))
                    throw new InvalidDataException($"Duplicate skill \"{skill}\" found in set \"{set}\".");

                Skills.Add(skill.Name, skill);
            }

            // connect relations up.
            foreach (var skill in newSkills)
            {
                var relations = new List<(Skill relation, decimal rate)>();
                foreach (var rel in skill.Relations)
                {
                    relations.Add((Skills[rel.relation.Name], rel.rate));
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
                throw new DataException($"Skill Group(s) \"{string.Join(", ", dupes)}\" within set \"{set}\" has been found.");
            }
            
            // check for duplicates in old sets
            foreach (var dupe in newGroups.Select(x => x.Name))
            {
                if (SkillGroups.ContainsKey(dupe))
                    throw new DataException($"Duplicate Skill Group \"{dupe}\" found in set \"{set}\"");
            }
            
            // add new skillGroups
            foreach (var newGroup in newGroups)
                SkillGroups.Add(newGroup.Name, newGroup);
            // complete
        }

        private void LoadProcesses(string set)
        {
            var filename = GetDataFile(set, "Processes");
            if (!File.Exists(filename)) // ensure file exists.
                return;
            var json = File.ReadAllText(filename);
            var newProcesses = JsonSerializer.Deserialize<List<Process>>(json);
            if (newProcesses == null)
                return; // if file is empty, that's fine.
            
            // check new set for duplicates
            var dupes = FindDuplicates(newProcesses.Select(x => x.GetName()).ToList());
            if (dupes.Count > 0)
            {
                throw new DataException($"Process(es) \"{string.Join(", ", dupes)}\" within set \"{set}\" has been found.");
            }
            
            // check for duplicates in old sets
            foreach (var dupe in newProcesses.Select(x => x.GetName()))
            {
                if (Processes.ContainsKey(dupe))
                    throw new DataException($"Duplicate Process Group \"{dupe}\" found in set \"{set}\"");
            }
            
            foreach (var process in newProcesses)
            {
                Processes.Add(process.GetName(), process);
            }
            // no connections need to be made. Let's GO!
        }

        private void LoadJobs(string set)
        {
            var filename = GetDataFile(set, "Jobs");
            var json = File.ReadAllText(filename);

            var newJobs = JsonSerializer.Deserialize<List<Job>>(json);
            
            // check for dupes in set
            var dupes = FindDuplicates(newJobs.Select(x => x.GetName()).ToList());
            if (dupes.Count > 0)
                throw new DataException(
                    $"Duplicate Job(s) \"{string.Join(", ", dupes)}\" within set \"{set}\" has been found.");

            // check for dupes in old set
            foreach (var dupe in newJobs.Select(x => x.GetName()))
            {
                if (Jobs.ContainsKey(dupe))
                    throw new DataException($"Duplicate Job \"{dupe}\" found in set \"{set}\".");
            }
            
            foreach (var job in newJobs)
                Jobs.Add(job.GetName(), job);
            // no additional work needed.
        }

        private void LoadSpecies(string set)
        {
            var filename = GetDataFile(set, "Species");
            var json = File.ReadAllText(filename);
            var newSpecies = JsonSerializer.Deserialize<List<Species>>(json);

            // check for duplicates
            var dupes = FindDuplicates(newSpecies.Select(x => x.GetName()).ToList());
            if (dupes.Count > 0)
            {
                throw new DataException(
                    $"Species \"{string.Join(", ", dupes)}\" within set \"{set}\" have been found.");
            }
            
            // check for duplicates in old sets
            foreach (var dupe in newSpecies.Select(x => x.GetName()))
            {
                if (Species.ContainsKey(dupe))
                    throw new DataException($"Duplicate Species \"{dupe}\" found in set \"{set}\".");
            }
            
            // add species
            foreach (var spec in newSpecies)
                Species.Add(spec.GetName(), spec);

            foreach (var species in newSpecies)
            {
                // connect species relations
                var relations = new List<Species>();
                foreach (var rel in species.Relations)
                    relations.Add(Species[rel.GetName()]);
                species.Relations = relations;
            }
        }

        private void LoadCultures(string set)
        {
            var filename = GetDataFile(set, "Cultures");
            var json = File.ReadAllText(filename);
            var newCultures = JsonSerializer.Deserialize<List<Culture>>(json);
            
            // check for dupes within the set.
            var dupes = FindDuplicates(newCultures.Select(x => x.GetName()).ToList());
            if (dupes.Count > 0)
            {
                throw new DataException(
                    $"Cultures \"{string.Join(", ", dupes)}\" within set \"{set}\" have been found.");
            }
            
            // check for dupes in old sets.
            foreach (var dupe in newCultures.Select(x => x.GetName()))
            {
                if (Cultures.ContainsKey(dupe))
                    throw new DataException($"Duplicate Culture \"{dupe}\" found in set \"{set}\".");
            }
            
            foreach (var culture in newCultures)
                Cultures.Add(culture.GetName(), culture);
            
            // no connections needed.
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
                
                LoadProcesses(set);
                
                LoadJobs(set);

                LoadSpecies(set);
                
                LoadCultures(set);
            }
        }

        #endregion Data
        
        #region Saves

        public string GetSaveFolder(string save)
        {
            return Path.Combine(DataFolder, "Saves", save);
        }

        private string GetFileFromSave(string save, string section)
        {
            return Path.Combine(GetSaveFolder(save), $"{section}.json");
        }

        private void LoadTerritories(string save)
        {
            var filename = GetFileFromSave(save, "Territories");
            var json = File.ReadAllText(filename);

            var newTerritories = JsonSerializer.Deserialize<List<Territory.Territory>>(json);
            
            // check for dupes within territories
            var dupes = FindDuplicates(newTerritories.Select(x => x.Name).ToList());
            if (dupes.Count > 0)
            {
                throw new DataException("Territories Contain Duplicate Names.");
            }

            // Connect neighboring Territories
            foreach (var terr in newTerritories)
            {
                var rep = new List<NeighborConnection>();
                foreach (var neigh in terr.Neighbors)
                {
                    var neighbor = newTerritories.Single(x => x.Name == neigh.Neighbor.Name);
                    rep.Add(new NeighborConnection
                    {
                        Neighbor = neighbor,
                        Distance = neigh.Distance,
                        Type = neigh.Type
                    });
                }

                terr.Neighbors = rep;
            }
            
            // add to list and complete.
            foreach (var terr in newTerritories)
                Territories.Add(terr.Name, terr);
        }

        private void LoadMarkets(string save)
        {
            var filename = GetFileFromSave(save, "Markets");
            var json = File.ReadAllText(filename);

            var newMarkets = JsonSerializer.Deserialize<List<Market.Market>>(json);
            
            // find duplicates in the save
            var dupes = FindDuplicates(newMarkets.Select(x => x.Name));
            if (dupes.Count > 0)
            {
                throw new DataException($"Duplicate Markets \"{string.Join(',', dupes)}\"");
            }
            
            // go through connecting neighbors if possible.
            // TODO improve Territories lots, connections need to be improved. 
            foreach (var market in newMarkets)
            {
                // for each territory's
                foreach (var terr in market.Territories)
                {
                    // connections
                    foreach (var connection in terr.Neighbors)
                    {
                        // check if the connection is already in the market.
                        if (!market.Territories.Select(x => x.Name)
                                .Contains(connection.Neighbor.Name))
                        { // if not in already
                            // check that the connected territory has a market
                            // and it can be walked to 
                            if (connection.Neighbor.Market != null && 
                                (connection.Type == TerritoryConnectionType.Land ||
                                connection.Type == TerritoryConnectionType.Tunnel))
                            { // if it can, add/update the connection
                                if (market.Neighbors.ContainsKey(connection.Neighbor.Market) &&
                                    market.Neighbors[connection.Neighbor.Market] < connection.Distance)
                                    market.Neighbors[connection.Neighbor.Market] = connection.Distance;
                                else // if not already connected just add it
                                    market.Neighbors[connection.Neighbor.Market] = connection.Distance;
                            }
                        }
                    }
                }
            }
            
            // complete, put to memory
            foreach (var market in newMarkets)
                Markets.Add(market.Name, market);
        }

        private void LoadFirms(string save)
        {
            var filename = GetFileFromSave(save, "Firms");
            var json = File.ReadAllText(filename);
            var newFirms = JsonSerializer.Deserialize<List<Firm>>(json);
            
            // connect firm parent and children
            foreach (var firm in newFirms)
            {
                if (firm.Parent != null)
                    firm.Parent = newFirms.Single(x => x.Name == firm.Parent.Name);

                if (firm.Children.Any())
                {
                    var children = new List<Firm>();
                    foreach (var child in firm.Children)
                        children.Add(newFirms.Single(x => x.Name == child.Name));
                    firm.Children = children;
                }
                
                // connect markets back.
                var hq = firm.HeadQuarters;
                hq.Firms.Add(firm);

                foreach (var region in firm.Regions)
                {
                    // add if it's not already there (regions shouldn
                    if (!region.Firms.Contains(firm))
                        region.Firms.Add(firm);
                }
            }
            
            // put into memory
            foreach (var firm in newFirms)
            {
                Firms.Add(firm.Name, firm);
            }
            // finished.
        }
        
        private void LoadPops(string save)
        {
            var filename = GetFileFromSave(save, "Pops");
            var json = File.ReadAllText(filename);

            var newPops =  JsonSerializer.Deserialize<List<PopGroup>>(json);
            
            // No additional Connections needed.
            // add and throw if any duplicates
            foreach (var pop in newPops)
                Pops.Add(pop.Name, pop);
        }

        public void LoadSave(string save)
        {
            // maybe load sets required by save here.
            
            // load territories
            LoadTerritories(save);
            
            // load markets
            LoadMarkets(save);

            // load firms
            LoadFirms(save);
            
            // Load Pops
            LoadPops(save);
        }

        #endregion Saves
        
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

        public SortedList<string, Want> Wants { get; set; }

        public SortedList<string, TechFamily> TechFamilies { get; set; }

        public SortedList<string, Technology.Technology> Technologies { get; set; }

        public SortedList<string, Product> Products { get; set; }

        public SortedList<string, SkillGroup> SkillGroups { get; set; }

        public SortedList<string, Skill> Skills { get; set; }

        public SortedList<string, Process> Processes { get; set; }

        public SortedList<string, Job> Jobs { get; set; }

        public SortedList<string, Species> Species { get; set; }

        public SortedList<string, PopGroup> Pops { get; }
        
        public SortedList<string, Culture> Cultures { get; set; }
        
        public SortedList<string, Territory.Territory> Territories { get; set; }
        
        public SortedList<string, Market.Market> Markets { get; set; }
        
        public SortedList<string, Firm> Firms { get; set; }
        
        public List<string> Sets { get; set; }

        #endregion DataStorage
    }
}
