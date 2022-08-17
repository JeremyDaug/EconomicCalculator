using System.Data;
using System.Text.Json;
using EconomicSim.Enums;
using EconomicSim.Helpers;
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
            AvailableSaves = new List<string>();
            AvailableSets = new List<string>();
            var folder = Directory.GetCurrentDirectory();
            // TODO: Make this Crossplatform
            DataFolder = Path.GetFullPath(Path.Combine(folder, @"../../../../EconomicSim/Data/"));
            SaveFolder = Path.GetFullPath(Path.Combine(folder, @"../../../../EconomicSim/Saves/"));
            
            // get possible sets
            foreach (var set in Directory.GetDirectories(DataFolder))
            {
                if (File.Exists(Path.Combine(set, "SetInfo.txt")))
                    AvailableSets.Add(set);
            }
            // get possible saves
            foreach (var save in Directory.GetDirectories(SaveFolder))
            {
                if (File.Exists(Path.Combine(save, "SaveData.json")))
                    AvailableSaves.Add(Path.GetFileName(save));
            }

            Wants = new SortedList<string, Want>();
            TechFamilies = new SortedList<string, TechFamily>();
            Technologies = new SortedList<string, Technology.Technology>();
            Products = new SortedList<string, Product>();
            SkillGroups = new SortedList<string, SkillGroup>();
            Skills = new SortedList<string, Skill>();
            Processes = new SortedList<string, Process>();
            ProcessNodes = new SortedList<string, IProcessNode>();
            Jobs = new SortedList<string, Job>();
            Species = new SortedList<string, Species>();
            Cultures = new SortedList<string, Culture>();
            Pops = new SortedList<string, PopGroup>();
            Territories = new SortedList<string, Territory.Territory>();
            Markets = new SortedList<string, Market.Market>();
            Firms = new SortedList<string, Firm>();
            Sets = new List<string>();
        }

        #region ActionFunctions

        /// <summary>
        /// Runs one day of our current system.
        /// TODO Improve and make possible to distribute work across multiple systems.
        /// </summary>
        public async Task RunDay()
        { 
            // TODO update this to work on a per market basis
            // doing so would allow markets to be broken off and distributed to
            // other machines.
            
            // Labor phase, pops receive the time they can use.
            // They also do a quick and dirty prioritization on time
            var LaborPhase = new List<Task>();
            foreach (var pop in Pops.Values)
            {
                pop.Property.Add((Product)Time, pop.GetTotalHours());
                LaborPhase.Add(pop.ReserveItems());
            }
            await Task.WhenAll(LaborPhase);

            // Production Phase, firms go about their business
            // producing what they planned for, and consuming their inputs.
            var ProductionPhase = new List<Task>();
            foreach (var firm in Firms.Values)
            {
                ProductionPhase.Add(firm.ProductionPhase());
            }
            await Task.WhenAll(ProductionPhase);

            // Sell Phase, a phase where all firms and pops put up wares for
            // sale on the market.
            // Firms, Pops, Governments, and Institutions can all put stuff up
            // for sale.
            var sellPhase = new List<Task<ICanSell>>();
            foreach (var firm in Firms.Values)
            {
                sellPhase.Add(firm.SellPhase());
            }
            foreach (var pop in Pops.Values)
            {
                sellPhase.Add(pop.SellPhase());
            }
            // go through Gov'ts
            // go through Institutions.
            await Task.WhenAll(sellPhase);
            
            // Add all sellers to their markets.
            foreach (var seller in sellPhase.Select(x => x.Result))
            {
                seller.Market.AddSeller(seller);
            }

            // Merchant Phase, merchants get first dibs to purchase, if they
            // didn't already have a contract setup.

            // Pop Buy Phase, where all pops and firms buy what they desire.

            // Pop Consumption phase, where pops get to consume and enjoy
            // the fruits of their labors.

            // Population Recalculation, pops calculate their satisfaction,
            // adjust their moods, grow or shrink, and  and so on. No movement
            // here.

            // Firm Recalculation Phase, Firms take into account all of their 
            // business for the day, compare it to long term metrics,  
            // adjust their production targets, shift workers around, and if
            // growth or contraction is targeted, set those desires as well.

            // Migration, Pops who are want or are able to begin moving about.
            // Those who are able are added to the migration pool, then will
            // seek out better options. First locally, then abroad.

            // Travelling Merchant Phase, Merchants, who bought and sold in
            // previous phases, move to their target location to repeat the
            // reciprocal of their action.

        }

        #endregion
        
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
            // go through each and save anything which isn't in a set
            // TODO complete this when Sets are introduced and midgame generation is available.
            
            // save game specific data.
            SaveFirms(CurrentSave);
            SaveMarkets(CurrentSave);
            SavePops(CurrentSave);
            SaveTerritories(CurrentSave);
        }


        // TODO, update these for set mechanics.
        public void SaveWants(string set = "")
        {
            var filename = GetDataFile("Common", "Wants");
            var json = JsonSerializer.Serialize(Wants.Values,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            File.WriteAllText(filename, json);
        }

        public void SaveTechnologies(string set = "")
        {
            var filename = GetDataFile("Common", "Technology");
            var json = JsonSerializer.Serialize(Technologies
                    .Where(x => !RequiredItems.Technologies.ContainsKey(x.Key))
                    .Select(x => x.Value),
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            File.WriteAllText(filename, json);
        }

        public void SaveTechFamilies(string set = "")
        {
            var filename = GetDataFile("Common", "TechFamilies");
            var json = JsonSerializer.Serialize(TechFamilies.Values,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            File.WriteAllText(filename, json);
        }
        
        public void SaveSkills(string set = "")
        {
            var filename = GetDataFile("Common", "Skills");
            var json = JsonSerializer.Serialize(Skills.Values,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            File.WriteAllText(filename, json);
        }

        public void SaveSkillGroups(string set = "")
        {
            var filename = GetDataFile("Common", "SkillGroups");
            var json = JsonSerializer.Serialize(SkillGroups.Values,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            File.WriteAllText(filename, json);
        }
        
        public void SaveProducts(string set = "")
        {
            var filename = GetDataFile("Common", "Products");
            var json = JsonSerializer.Serialize(Products
                    .Where(x => !RequiredItems.Products.ContainsKey(x.Key))
                    .Select(x => x.Value),
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            File.WriteAllText(filename, json);
        }
        
        public void SaveProcesses(string set = "")
        {
            var filename = GetDataFile("Common", "Processes");
            var json = JsonSerializer.Serialize(Processes.Values,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            File.WriteAllText(filename, json);
        }
        
        public void SaveSpecies(string set = "")
        {
            var filename = GetDataFile("Common", "Species");
            var json = JsonSerializer.Serialize(Species.Values,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            File.WriteAllText(filename, json);
        }
        
        public void SaveCultures(string set = "")
        {
            var filename = GetDataFile("Common", "Cultures");
            var json = JsonSerializer.Serialize(Cultures.Values,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            File.WriteAllText(filename, json);
        }
        
        public void SaveJobs(string set = "")
        {
            var filename = GetDataFile("Common", "Jobs");
            var json = JsonSerializer.Serialize(Jobs.Values,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            File.WriteAllText(filename, json);
        }

        public void SaveFirms(string save)
        {
            var filename = GetFileFromSave(save, "Firms");
            var json = JsonSerializer.Serialize(Firms.Values,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            File.WriteAllText(filename, json);
        }
        
        public void SaveMarkets(string save)
        {
            var filename = GetFileFromSave(save, "Markets");
            var json = JsonSerializer.Serialize(Markets.Values,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            File.WriteAllText(filename, json);
        }
        
        public void SavePops(string save)
        {
            var filename = GetFileFromSave(save, "Pops");
            var json = JsonSerializer.Serialize(Pops.Values,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            File.WriteAllText(filename, json);
        }
        
        public void SaveTerritories(string save)
        {
            var filename = GetFileFromSave(save, "Territories");
            var json = JsonSerializer.Serialize(Territories.Values,
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
            foreach (var want in RequiredItems.Wants.Values)
                Wants.Add(want.Name, (Want)want); 

            // load techs
            foreach (var tech in RequiredItems.Technologies.Values)
                Technologies.Add(tech.Name, (Technology.Technology)tech);

            // load products
            foreach (var product in RequiredItems.Products.Values)
                Products.Add(product.GetName(), (Product)product);
            
            // Load processes 
            foreach (var process in RequiredItems.Processes.Values)
            {
                Processes.Add(process.GetName(), (Process)process);
                foreach (var product in process.ProcessProducts)
                {
                    var prod = Products[product.Product.GetName()];
                    prod.ProductProcesses.Add(process);
                }
            }

            Time = Products["Time"];
            Rest = Wants["Rest"];
            Shopping = Products["Shopping"];
        }

        /// <summary>
        /// Loads wants from the set selected.
        /// </summary>
        /// <param name="set">The set group to load from.</param>
        /// <returns>A list of duplicate wants found.</returns>
        private void LoadWants(string set = "")
        {
            var errors = new List<string>();

            var filename = GetDataFile(set, "Wants");
            if (!File.Exists(filename))
                return;
            var json = File.ReadAllText(filename);
            var newWants = JsonSerializer.Deserialize<List<Want>>(json);
            if (newWants == null)
                return;

            foreach (var want in newWants)
            {
                if (!Wants.TryAdd(want.Name, want))
                    errors.Add($"Duplicate Want of name \"{want.Name}\" from Set \"{set}\" found.");
            }

            if (errors.Any())
                throw new DataException($"Duplicates found:\n" + string.Join('\n', errors));
        }

        private void LoadTechFamilies(string set = "")
        {
            var errors = new List<string>();
            var filename = GetDataFile(set, "TechFamilies");
            if (!File.Exists(filename))
                return;
            var json = File.ReadAllText(filename);
            var newFamilies = JsonSerializer.Deserialize<List<TechFamily>>(json);
            if (newFamilies == null)
                return;
            
            foreach (var fam in newFamilies)
            {
                if (!TechFamilies.TryAdd(fam.Name, fam))
                    errors.Add($"Duplicate Tech Family \"{fam.Name}\" in set \"{set}\" found.");
            }

            if (errors.Any())
                throw new DataException($"Duplicates found:\n" + string.Join('\n', errors));

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

        private void LoadTechs(string set = "")
        {
            var errors = new List<string>();
            var filename = GetDataFile(set, "Technology");
            if (!File.Exists(filename))
                return;
            var json = File.ReadAllText(filename);
            var newTechs = JsonSerializer.Deserialize<List<Technology.Technology>>(json);
            if (newTechs == null)
                return;

            foreach (var tech in newTechs)
            {
                // add tech to list.
                if (!Technologies.TryAdd(tech.Name, tech))
                    errors.Add($"Duplicate tech \"{tech.Name}\" in Set \"{set}\".");
            }

            if (errors.Any())
                throw new DataException($"Duplicates found:\n" + string.Join('\n', errors));

            // connect to techs
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
                    children.Add(Technologies[child.Name]);
                tech.Children = children;
            }

            // completed.
        }

        private void LoadProducts(string set = "")
        {
            var filename = GetDataFile(set, "Products");
            if (!File.Exists(filename))
                return;
            var json = File.ReadAllText(filename);
            var newProducts = JsonSerializer.Deserialize<List<Product>>(json);
            if (newProducts == null)
                return;

            var errors = new List<string>();
            foreach (var prod in newProducts)
            {
                if (!Products.TryAdd(prod.GetName(), prod))
                    errors.Add($"Duplicate Product \"{prod.GetName()}\" in set \"{set}\" found.");
            }

            if (errors.Any())
                throw new DataException("Product Duplicates Found:\n" + string.Join('\n', errors));

            // Processes connect to products, so no connections here.

            // complete.
        }

        private void LoadSkills(string set = "")
        {
            var filename = GetDataFile(set, "Skills");
            if (!File.Exists(filename))
                return;
            var json = File.ReadAllText(filename);

            var newSkills = JsonSerializer.Deserialize<List<Skill>>(json);
            if (newSkills == null)
                return;

            var errors = new List<string>();
            foreach (var skill in newSkills)
            {
                if (!Skills.TryAdd(skill.Name, skill))
                    errors.Add($"Duplicate Skill \"{skill.Name}\" in set \"{set}\"");
            }

            if (errors.Any())
                throw new DataException("Duplicate Skills:\n" + string.Join('\n', errors));

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

        private void LoadSkillGroups(string set = "")
        {
            var filename = GetDataFile(set, "SkillGroups");
            if (!File.Exists(filename))
                return;
            var json = File.ReadAllText(filename);
            var newGroups = JsonSerializer.Deserialize<List<SkillGroup>>(json);
            if (newGroups == null)
                return;

            var errors = new List<string>();
            // add new skillGroups
            foreach (var newGroup in newGroups)
            {
                if (!SkillGroups.TryAdd(newGroup.Name, newGroup))
                    errors.Add($"Duplicate Skill Group \"{newGroup.Name}\" in set \"{set}\".");
            }

            if (errors.Any())
                throw new DataException("Duplicate Skill Groups Found:\n" + string.Join('\n', errors));

            // complete
        }

        private void LoadProcesses(string set = "")
        {
            var filename = GetDataFile(set, "Processes");
            if (!File.Exists(filename)) // ensure file exists.
                return;
            var json = File.ReadAllText(filename);
            var newProcesses = JsonSerializer.Deserialize<List<Process>>(json);
            if (newProcesses == null)
                return;

            var errors = new List<string>();
            foreach (var process in newProcesses)
            {
                if (!Processes.TryAdd(process.GetName(), process))
                    errors.Add($"Duplicate Process \"{process.Name}\" in set \"{set}\".");
            }
            
            // connect processes to Products
            foreach (var process in newProcesses)
            {
                // if it has that product, add it to their processes
                foreach (var product in process.InputProducts)
                    product.Product.ProductProcesses.Add(process);
                foreach (var product in process.CapitalProducts)
                    if (!product.Product.ProductProcesses.Contains(process))
                        product.Product.ProductProcesses.Add(process);
                foreach (var product in process.OutputProducts)
                    if (!product.Product.ProductProcesses.Contains(process))
                        product.Product.ProductProcesses.Add(process);
            }

            foreach (var product in Products.Values)
            {
                try
                {
                    var test = product.FailureProcess;
                    var test1 = product.ConsumptionProcesses;
                    var test2 = product.UseProcesses;
                    var test3 = product.MaintenanceProcesses;
                }
                catch
                {
                    errors.Add($"Product {product.GetName()}, had a duplicate Failure, Consumption, Use, or Maintenance process.");
                }
            }
            
            // connect up all of the processes in the process node network.
            // or is it a tree?
            foreach (var process in Processes.Values)
            {
                var node = new ProcessNode(process);
                ProcessNodes.Add(node.ToString(), node);
            }
            
            if (errors.Any())
                throw new DataException("Duplicate Processes Found:\n" + string.Join('\n', errors));
        }

        private void LoadJobs(string set = "")
        {
            var filename = GetDataFile(set, "Jobs");
            if (!File.Exists(filename))
                return;
            var json = File.ReadAllText(filename);
            var newJobs = JsonSerializer.Deserialize<List<Job>>(json);
            if (newJobs == null)
                return;

            var errors = new List<string>();
            foreach (var job in newJobs)
            {
                if (!Jobs.TryAdd(job.GetName(), job))
                    errors.Add($"Duplicate Job \"{job.Name}\" in set \"{set}\".");
            }
            // no additional work needed.
            if (errors.Any())
                throw new DataException("Duplicate Jobs Found:\n" + string.Join('\n', errors));
        }

        private void LoadSpecies(string set = "")
        {
            var filename = GetDataFile(set, "Species");
            if (!File.Exists(filename))
                return;
            var json = File.ReadAllText(filename);
            var newSpecies = JsonSerializer.Deserialize<List<Species>>(json);
            if (newSpecies == null)
                return;

            var errors = new List<string>();
            // add species
            foreach (var spec in newSpecies)
            {
                if (!Species.TryAdd(spec.GetName(), spec))
                    errors.Add($"Duplicate Species \"{spec.Name}\" in set \"{set}\".");
            }
            if (errors.Any())
                throw new DataException("Duplicate Species Found:\n" + string.Join('\n', errors));

            foreach (var species in newSpecies)
            {
                // connect species relations
                var relations = new List<Species>();
                foreach (var rel in species.Relations)
                    relations.Add(Species[rel.GetName()]);
                species.Relations = relations;
            }
        }

        private void LoadCultures(string set = "")
        {
            var filename = GetDataFile(set, "Cultures");
            if (!File.Exists(filename))
                return;
            var json = File.ReadAllText(filename);
            var newCultures = JsonSerializer.Deserialize<List<Culture>>(json);
            if (newCultures == null)
                return;

            var errors = new List<string>();
            foreach (var culture in newCultures)
            {
                if (!Cultures.TryAdd(culture.GetName(), culture))
                    errors.Add($"Duplicate Culture \"{culture.GetName()}\" found in set \"{set}\".");
            }
            if (errors.Any())
                throw new DataException("Duplicate Cultures Found:\n" + string.Join('\n', errors));
            
            // no connections needed.
        }
        
        /// <summary>
        /// Clear All data to reload everything.
        /// </summary>
        public void ClearData()
        {
            _hasEverLoaded = false;
            
            Firms.Clear();
            Jobs.Clear();
            Markets.Clear();
            Cultures.Clear();
            Species.Clear();
            Processes.Clear();
            Products.Clear();
            Skills.Clear();
            SkillGroups.Clear();
            TechFamilies.Clear();
            Technologies.Clear();
            Territories.Clear();
            Wants.Clear();
        }

        public event EventHandler? InformationChanged;

        /// <summary>
        /// Loads all of the data from the common folder from the sets given.
        /// </summary>
        /// <param name="sets">Which Sets to load from files.</param>
        /// <exception cref="ArgumentNullException">If sets is null.</exception>
        public async Task LoadData(IEnumerable<string> sets, IProgress<(decimal, string)> progress)
        {
            // predict needed steps
            decimal totalSteps = sets.Count() * 10;
            decimal stepsize = 100 / totalSteps;
            decimal currentStep = 0;
            
            LoadRequiredItems();
            
            foreach (var set in sets)
            {
                progress.Report((currentStep, "Loading Wants"));

                LoadWants(set);
                currentStep += stepsize;
                progress.Report((currentStep, $"{set}: Loading Tech Families"));
                //await Task.Delay(500);

                LoadTechFamilies(set);
                currentStep += stepsize;
                progress.Report((currentStep, $"{set}: Loading Techs"));
                //await Task.Delay(500);

                LoadTechs(set);
                currentStep += stepsize;
                progress.Report((currentStep, $"{set}: Loading Products"));
                //await Task.Delay(500);

                LoadProducts(set);
                currentStep += stepsize;
                progress.Report((currentStep, $"{set}: Loading Skills"));
                //await Task.Delay(500);

                LoadSkills(set);
                currentStep += stepsize;
                progress.Report((currentStep, $"{set}: Loading Skill Groups"));
                //await Task.Delay(500);
                
                LoadSkillGroups(set);
                currentStep += stepsize;
                progress.Report((currentStep, $"{set}: Loading Processes"));
                //await Task.Delay(500);
                
                LoadProcesses(set);
                currentStep += stepsize;
                progress.Report((currentStep, $"{set}: Loading Jobs"));
                //await Task.Delay(500);
                
                LoadJobs(set);
                currentStep += stepsize;
                progress.Report((currentStep, $"{set}: Loading Species"));
                //await Task.Delay(500);

                LoadSpecies(set);
                currentStep += stepsize;
                progress.Report((currentStep, $"{set}: Loading Cultures"));
                //await Task.Delay(500);
                
                LoadCultures(set);
                
                currentStep += stepsize;
                Sets.Add(set);
            }

            var setsLoaded = string.Join('\n', sets);
            progress.Report((100, "Sets Loaded.\n" + setsLoaded));
        }

        #endregion Data
        
        #region Saves

        public string GetSaveFolder(string save)
        {
            return Path.Combine(SaveFolder, save);
        }

        private string GetFileFromSave(string save, string section)
        {
            return Path.Combine(GetSaveFolder(save), $"{section}.json");
        }

        private void LoadTerritories(string save)
        {
            var filename = GetFileFromSave(save, "Territories");
            if (!File.Exists(filename))
                return;
            var json = File.ReadAllText(filename);
            var newTerritories = JsonSerializer.Deserialize<List<Territory.Territory>>(json);
            if (newTerritories == null)
                throw new DataException("Territories do not exist.");
                
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
            if (!File.Exists(filename))
                return;
            var json = File.ReadAllText(filename);
            var newMarkets = JsonSerializer.Deserialize<List<Market.Market>>(json);
            if (newMarkets == null)
                throw new DataException("Markets file is empty.");
            
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
            if (!File.Exists(filename))
                return;
            var json = File.ReadAllText(filename);
            var newFirms = JsonSerializer.Deserialize<List<Firm>>(json);
            if (newFirms == null)
                throw new DataException("Firms file is empty.");
            
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
                var hq = (Market.Market) firm.HeadQuarters;
                hq.Firms.Add(firm);

                foreach (var region in firm.Regions)
                {
                    // add if it's not already there (regions shouldn
                    if (!region.Firms.Contains(firm))
                        region.Firms.Add(firm);
                }
                
                // Set unassigned assignments to a basic set.
                foreach (var job in firm.Jobs)
                {
                    if (!job.Assignments.Any())
                    {
                        foreach (var proc in job.Job.Processes)
                        {
                            job.Assignments.Add(proc, new AssignmentInfo(0,0));
                        }
                    }
                }
            }
            
            // with firms and markets loaded, calculate initial 
            // average market price.
            foreach (var market in Markets.Values)
            {
                foreach (var biz in market.Firms)
                {
                    foreach (var prod in biz.Products)
                    {
                        if (!market.MarketPrices.ContainsKey(prod.Key))
                        {
                            market.MarketPrices[prod.Key] = 0;
                            market.ProductSold[prod.Key] = 0;
                        }
                        // average (old price * previous weight + new price )
                        // / (weights)
                        var oldWeight = market.ProductSold[prod.Key];
                        market.MarketPrices[prod.Key]
                            = (market.MarketPrices[prod.Key] * oldWeight
                               + prod.Value) / (oldWeight + 1);
                        market.ProductSold[prod.Key] += 1;
                    }
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
            if (!File.Exists(filename))
                return;
            var json = File.ReadAllText(filename);
            var newPops =  JsonSerializer.Deserialize<List<PopGroup>>(json);
            if (newPops == null)
                throw new DataException("Pops file is empty.");
            
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
        public List<string> AvailableSets { get; }
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
        public List<string> AvailableSaves { get; }
        // TODO un-default this later, when we have the ability to save/load games more properly.
        public string CurrentSave { get; } = "Default";
        public string CurrentFirmFolder => "Firms";
        public string CurrentMarketFolder => "Markets";
        public string CurrentPopsFolder => "Pops";
        public string CurrentTerritoriesFolder => "Territories";
        public string DefaultIcon => @"ProductImages\DefaultIcon.png";

        public GameMode Mode { get; set; }
        public bool DebugMode { get; set; }
        public DebugFlags DebugFlags { get; set; }

        public bool ChangeGameMode(GameMode mode)
        {
            // debug mode always allows for changing.
            if (DebugMode)
            {
                Mode = mode;
                return true;
            }
            // alternatives and checks go hear
            return false;
        }

        #endregion FileStorage

        #region DataStorage

        public SortedList<string, Want> Wants { get; set; }

        public SortedList<string, TechFamily> TechFamilies { get; set; }

        public SortedList<string, Technology.Technology> Technologies { get; set; }

        public SortedList<string, Product> Products { get; set; }

        public SortedList<string, SkillGroup> SkillGroups { get; set; }

        public SortedList<string, Skill> Skills { get; set; }

        public SortedList<string, Process> Processes { get; set; }
        
        public SortedList<string, IProcessNode> ProcessNodes { get; set; }

        public SortedList<string, Job> Jobs { get; set; }

        public SortedList<string, Species> Species { get; set; }

        public SortedList<string, PopGroup> Pops { get; }
        
        public SortedList<string, Culture> Cultures { get; set; }
        
        public SortedList<string, Territory.Territory> Territories { get; set; }
        
        public SortedList<string, Market.Market> Markets { get; set; }
        
        public SortedList<string, Firm> Firms { get; set; }
        
        public List<string> Sets { get; }

        #region KeyItems

        public IProduct Time { get; private set; }
        public IProduct Shopping { get; private set; }
        public IWant Rest { get; private set; }

        #endregion

        #endregion DataStorage
    }
}
