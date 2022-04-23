using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using EconomicSim.Objects.Wants;
using EconomicSim.Enums;
using EconomicSim.DTOs.Pops.Species.AttachedTagData;
using EconomicSim.DTOs.Hexmap;
using EconomicSim.Objects.Firms;
using EconomicSim.DTOs;
using EconomicSim.DTOs.Firms;
using EconomicSim.DTOs.Jobs;
using EconomicSim.DTOs.Market;
using EconomicSim.DTOs.Pops;
using EconomicSim.DTOs.Pops.Culture;
using EconomicSim.DTOs.Pops.Species;
using EconomicSim.DTOs.Processes;
using EconomicSim.DTOs.Processes.ProcessTags;
using EconomicSim.DTOs.Products;
using EconomicSim.DTOs.Products.ProductTags;
using EconomicSim.DTOs.Skills;
using EconomicSim.DTOs.Technology;
using EconomicSim.DTOs.Territory;
using EconomicSim.DTOs.Wants;
using EconomicSim.Objects;

namespace EconomicSim
{
    /// <summary>
    /// A manager for the worldstate of data currently in existence.
    /// Used to get system data wherever needed.
    /// There should only ever be one of these per active program.
    /// </summary>
    public sealed class DTOManager
    {
        #region Singleton

        private static DTOManager instance = null;
        private static readonly object padlock = new object();

        // private Ctor, this is a singleton.
        private DTOManager()
        {
            // get Data Folder
            var folder = Directory.GetCurrentDirectory();
            DataFolder = Path.GetFullPath(Path.Combine(folder, @"..\..\..\EconomicCalculator\Data\"));

            UniverseName = "";
            //Planets = new Dictionary<int, Planet>();
            //Technologies = new Dictionary<int, ITechnology>();
            Products = new Dictionary<int, IProductDTO>();
            Wants = new Dictionary<int, IWantDTO>();
            Skills = new Dictionary<int, ISkillDTO>();
            SkillGroups = new Dictionary<int, ISkillGroupDTO>();
            Processes = new Dictionary<int, IProcessDTO>();
            Jobs = new Dictionary<int, IJobDTO>();
            TechFamilies = new Dictionary<int, ITechFamilyDTO>();
            Technologies = new Dictionary<int, ITechnologyDTO>();
            Species = new Dictionary<int, ISpeciesDTO>();
            Cultures = new Dictionary<int, ICultureDTO>();
            Pops = new Dictionary<int, IPopDTO>();
            SimpleTerritories = new List<ISimpleTerritoryDTO>();
            Markets = new Dictionary<int, IMarketDTO>();
            Firms = new Dictionary<int, IFirmDTO>();
        }        

        /// <summary>
        /// The current available instance.
        /// TODO, consider moving to EconomicCalculator project
        /// as this could be used for multiple interfaces.
        /// </summary>
        public static DTOManager Instance
        {
            get
            {// basic thread locking
                lock (padlock)
                {
                    // if instance has not been built, make it.
                    if (instance == null)
                    {
                        instance = new DTOManager();
                    }
                }
                // return singleton instance.
                return instance;
            }
        }

        #endregion Singleton

        #region DataStorage

        public IDictionary<int, IProductDTO> Products { get; set; }

        public IDictionary<int, IWantDTO> Wants { get; set; }

        public IDictionary<int, ISkillDTO> Skills { get; set; }

        public IDictionary<int, ISkillGroupDTO> SkillGroups { get; set; }

        public IDictionary<int, IProcessDTO> Processes { get; set; }

        public IDictionary<int, IJobDTO> Jobs { get; set; }

        public IDictionary<int, ITechFamilyDTO> TechFamilies { get; set; }

        public IDictionary<int, ITechnologyDTO> Technologies { get; set; }

        public IDictionary<int, ISpeciesDTO> Species { get; set; }

        public IDictionary<int, ICultureDTO> Cultures { get; set; }

        public IDictionary<int, IPopDTO> Pops { get; set; }

        public IList<ISimpleTerritoryDTO> SimpleTerritories { get; set; }

        public IDictionary<int, IMarketDTO> Markets { get; set; }

        public IDictionary<int, IFirmDTO> Firms { get; set; }

        #endregion DataStorage

        // TODO make this less shitty.
        public string DataFolder { get; set; }

        public string DefaultIcon => @"ProductImages\DefaultIcon.png";

        public string GetDataFilePath(string File)
        {
            return Path.Combine(DataFolder, File);
        }

        #region ContainsXFunctions

        /// <summary>
        /// Checks if a product already exists.
        /// Does not check if stored product matches or not.
        /// </summary>
        /// <param name="product">The product to check against.</param>
        /// <returns></returns>
        public bool ContainsProduct(IProductDTO product)
        {
            // if the id is contained then yes, it exists.
            return Products.ContainsKey(product.Id);
        }
        
        /// <summary>
        /// Checks if a want already exists. Does not check for duplicates.
        /// </summary>
        /// <param name="want">The want to check for.</param>
        /// <returns></returns>
        public bool ContainsWant(IWantDTO want)
        {
            return Wants.ContainsKey(want.Id);
        }

        #endregion ContainsXFunctions

        #region GetFunctions

        #region ByName

        /// <summary>
        /// Retrieves a product based on it's name and variant name.
        /// </summary>
        /// <param name="name">The name and variant name combined.</param>
        /// <returns></returns>
        public IProductDTO GetProductByFullName(string name)
        {
            Tuple<string, string> names = ProductDTO.GetProductNames(name);
            try
            {
                return Products.Values.Single(x =>
                {
                    if (!string.Equals(x.Name, names.Item1))
                    {// if primary name doesn't match.
                    return false;
                    }
                    else if (string.IsNullOrEmpty(names.Item2))
                    {// if second name is expected to be null.
                    return string.IsNullOrEmpty(x.VariantName);
                    }
                    else
                    { // if second name is not null.
                    return string.Equals(x.VariantName, names.Item2);
                    }
                });
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException(
                    string.Format("Did not find product {0}", names));
            }
        }

        /// <summary>
        /// Gets a want by it's name.
        /// </summary>
        /// <param name="name"/>
        /// <returns></returns>
        public IWantDTO GetWantByName(string name)
        {
            return Wants.Values.Single(x => x.Name == name);
        }

        /// <summary>
        /// Retrieve a Skill GRoup based on it's name.
        /// </summary>
        /// <param name="name">The desired Group.</param>
        /// <returns></returns>
        public ISkillGroupDTO GetSkillGroupByName(string name)
        {
            return SkillGroups.Values.Single(x => x.Name == name);
        }

        /// <summary>
        /// Retrieve a Skill based on it's name.
        /// </summary>
        /// <param name="name">The name of the Skill</param>
        /// <returns></returns>
        public ISkillDTO GetSkillByName(string name)
        {
            return Skills.Values.Single(x => x.Name == name);
        }

        /// <summary>
        /// Retrieve a process based on it's name.
        /// </summary>
        /// <param name="name">Name of the process.</param>
        /// <returns></returns>
        public IProcessDTO GetProcessByName(string name)
        {
            Tuple<string, string> names = ProcessDTO.GetProcessNames(name);
            return Processes.Values.Single(x => x.Name == names.Item1
                                        && x.VariantName == names.Item2);
        }

        /// <summary>
        /// Retrieve a Species based on it's name.
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public ISpeciesDTO GetSpeciesByName(string culture)
        {

            var names = SpeciesDTO.ProcessName(culture);

            return Species.Values.Single(x => x.Name == names.Name
                                        && x.VariantName == names.VariantName);
        }

        /// <summary>
        /// Retrieve a Culture based on it's name.
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public ICultureDTO GetCultureByName(string species)
        {

            var names = CultureDTO.ProcessName(species);

            return Cultures.Values.Single(x => x.Name == names.Name
                                        && x.VariantName == names.VariantName);
        }

        /// <summary>
        /// Retrieve a job based on it's name.
        /// </summary>
        /// <param name="species"></param>
        /// <returns></returns>
        public IJobDTO GetJobByName(string job)
        {
            var names = JobDTO.GetJobNames(job);

            return Jobs.Values.Single(x => x.Name == names.Name
                                        && x.VariantName == names.VariantName);
        }

        /// <summary>
        /// Retrieves the Territory of the given name.
        /// </summary>
        /// <param name="territory">The name of the territory desired.</param>
        /// <returns>The territory selected.</returns>
        /// <exception cref="InvalidOperationException">
        /// If territory does not exist, or a duplicate is found.
        /// </exception>
        public ISimpleTerritoryDTO GetTerritoryByName(string territory)
        {
            return SimpleTerritories.Single(x => x.Name.Equals(territory));
        }

        #endregion ByName

        #endregion GetFunctions

        /// <summary>
        /// Checks if a product is a duplicate of another or not.
        /// </summary>
        /// <param name="product">The product to check for.</param>
        /// <returns></returns>
        public bool IsDuplicate(IProductDTO product)
        {
            // get all products which share both a name and variant name.
            var matches = Products
                .Where(x => x.Value.Name == product.Name
                       && x.Value.VariantName == product.VariantName)
                .ToDictionary(
                    x => x.Key, x => x.Value);

            // if matches is empty, then there are no duplicates.
            if (matches.Count() == 0)
                return false;

            // if the only match is this product and it shares an ID,
            // then it's not a duplicate either.
            if (matches.Count() == 1 && matches.ContainsKey(product.Id))
                return false;

            // if there was more than 1 match, or the one match
            // contained did not share the id, then the product
            // is a duplicate.
            return true;
        }

        /// <summary>
        /// check if a product is a duplicate of another or not.
        /// </summary>
        /// <param name="process">The process to check for.</param>
        /// <returns></returns>
        public bool IsDuplicate(IProcessDTO process)
        {
            var matches = Processes
                .Where(x => x.Value.Name == process.Name
                        && x.Value.VariantName == process.VariantName)
                .ToDictionary(x => x.Key, x => x.Value);

            // if empty, then no matches exist.
            if (matches.Count() == 0)
                return false;

            // if 1 and it's id matches, we are updating that one then.
            if (matches.Count() == 1 && matches.ContainsKey(process.Id))
                return false;

            // if more than 1 or 1 but id's don't match, then we have a duplicate.
            return true;
        }

        /// <summary>
        /// Checksi if a job is a duplicate of another or not.
        /// </summary>
        /// <param name="job">The job to check for.</param>
        /// <returns></returns>
        public bool IsDuplicate(IJobDTO job)
        {
            var matches = Jobs
                .Where(x => x.Value.Name == job.Name
                            && x.Value.VariantName == job.VariantName)
                .ToDictionary(x => x.Key, x => x.Value);

            // if empty, then no matches.
            if (matches.Count() == 0)
                return false;

            // if 1 and it's id matches, we are updating that one then.
            if (matches.Count() == 1 && matches.ContainsKey(job.Id))
                return false;

            // if more than 1 or 1 but id's don't match, then we have a duplicate.
            return true;
        }

        /// <summary>
        /// Returns the Id of a want of the given name.
        /// </summary>
        /// <param name="name">The name we want the Id of.</param>
        /// <returns>The Id of the want with our name.</returns>
        /// <exception cref="InvalidOperationException">Want was not found.</exception>
        public int GetWantId(string name)
        {
            return Wants.Values.Single(x => x.Name == name).Id;
        }

        #region FindDuplicate

        /// <summary>
        /// Finds the duplicate product, if it exists. Returns null if not found.
        /// If it shares an ID with a product, it will not return that product.
        /// Only those who are proper duplicates.
        /// </summary>
        /// <param name="product">The product to find the duplicate of.</param>
        /// <returns></returns>
        public IProductDTO FindDuplicate(IProductDTO product)
        {
            var matches = Products
                .Where(x => x.Value.Name == product.Name
                       && x.Value.VariantName == product.VariantName)
                .ToDictionary(
                    x => x.Key, x => x.Value);

            // if empty, there are no duplicates.
            if (matches.Count() == 0)
                return null;

            // If only one match and it shares an ID, it's not a duplicate.
            if (matches.Count() == 1 && matches.ContainsKey(product.Id))
                return null;

            // If other conditions not met, then a dup must exist, get the first
            // which does not share the ID and return it.
            return matches.Values.First(x => x.Id != product.Id);
        }

        /// <summary>
        /// Finds whether there is a duplicate or not and returns it if it is.
        /// </summary>
        /// <param name="want">The want to check against.</param>
        /// <returns>The duplicate want, null if there is no duplicate.</returns>
        public IWantDTO FindDuplicate(IWantDTO want)
        {
            // get all wants which share a name
            var matches = Wants
                .Where(x => x.Value.Name == want.Name)
                .ToDictionary(x => x.Key, x => x.Value);

            // if no matches fonud, return null
            if (matches.Count() == 0)
                return null;

            // if one match found and it shares an id, return null
            if (matches.Count() == 1 && matches.ContainsKey(want.Id))
                return null;

            // otherwise it returns the duplicate
            return matches.Values.First(x => x.Id != want.Id);
        }

        /// <summary>
        /// Finds whether a of the given job.
        /// </summary>
        /// <param name="job">The job to find a duplicate of.</param>
        /// <returns>A duplicate job, null if there is no duplicate.</returns>
        public IJobDTO FindDuplicate(IJobDTO job)
        {
            var matches = Jobs
                .Where(x => x.Value.Name == job.Name
                            && x.Value.VariantName == job.VariantName)
                .ToDictionary(x => x.Key, x => x.Value);

            // if empty, then no matches.
            if (matches.Count() == 0)
                return null;

            // if 1 and it's id matches, we are updating that one then.
            if (matches.Count() == 1 && matches.ContainsKey(job.Id))
                return null;

            // if more than 1 or 1 but id's don't match, then we have a duplicate.
            return matches.Values.First(x => x.Id != job.Id);
        }

        #endregion FindDuplicate

        public string UniverseName { get; set; }

        public string UniverseFolder => Path.Combine(DataFolder, UniverseName);

        #region NewIds

        private int _newProductId;

        /// <summary>
        /// Helper to retrieve a new, unused, product Id.
        /// </summary>
        public int NewProductId
        {
            get
            {
                while (Products.ContainsKey(_newProductId))
                    ++_newProductId;

                return _newProductId;
            }
        }

        private int _newWantId;

        /// <summary>
        /// Helper to retrieve a new, unused, want Id.
        /// </summary>
        public int NewWantId
        {
            get
            {
                while (Wants.ContainsKey(_newWantId))
                    ++_newWantId;

                return _newWantId;
            }
        }

        private int _newProductInfoTagId;

        /// <summary>
        /// Helper to retrieve a new, unused, ProductTagInfo Id.
        /// </summary>
        public int NewProductInfoTagId
        {
            get
            {
                while (Wants.ContainsKey(_newProductInfoTagId))
                    ++_newProductInfoTagId;

                return _newProductInfoTagId;
            }
        }

        private int _newSkillId;

        /// <summary>
        /// Helper to retrieve a new, unused, Skill Id.
        /// </summary>
        public int NewSKillId
        {
            get
            {
                while (Skills.ContainsKey(_newSkillId))
                    ++_newSkillId;

                return _newSkillId;
            }
        }

        private int _newSkillGroupId;

        /// <summary>
        /// Helper to retrieve a new, unused, Skill Group Id.
        /// </summary>
        public int NewSkillGroupId
        {
            get
            {
                while (SkillGroups.ContainsKey(_newSkillGroupId))
                    ++_newSkillGroupId;

                return _newSkillGroupId;
            }
        }

        private int _newProcessId;

        /// <summary>
        /// Helper to retrieve a new, unused, Process Id.
        /// </summary>
        public int NewProcessId
        {
            get
            {
                while (Processes.ContainsKey(_newProcessId))
                    ++_newProcessId;

                return _newProcessId;
            }
        }

        private int _newJobId;

        /// <summary>
        /// Helper to retrieve a new, unused, Job Id.
        /// </summary>
        public int NewJobId
        {
            get
            {
                while (Jobs.ContainsKey(_newJobId))
                    ++_newJobId;
                return _newJobId;
            }
        }

        private int _newTechFamilyId;

        /// <summary>
        /// Helper to retrieve a new, unused, TechFamily Id.
        /// </summary>
        public int NewTechFamilyId
        {
            get
            {
                while (TechFamilies.ContainsKey(_newTechFamilyId))
                    ++_newTechFamilyId;
                return _newTechFamilyId;
            }
        }

        private int _newTechId;

        /// <summary>
        /// Helper to retrieve a new, unused, Tech Id.
        /// </summary>
        public int NewTechId
        {
            get
            {
                while (Technologies.ContainsKey(_newTechId)) 
                    ++_newTechId;
                return _newTechId;
            }
        }

        private int _newSpeciesId;
        /// <summary>
        /// Helper to retrieve a new, unused, Species Id.
        /// </summary>
        public int NewSpeciesId
        {
            get
            {
                while (Species.ContainsKey(_newSpeciesId))
                    ++_newSpeciesId;
                return _newSpeciesId;
            }
        }

        private int _newCultureId;
        /// <summary>
        /// Helper to retrieve a new, unused, Culture Id.
        /// </summary>
        public int NewCultureId
        {
            get
            {
                while (Cultures.ContainsKey(_newCultureId))
                    ++_newCultureId;
                return _newCultureId;
            }
        }

        private int _newPopId;
        /// <summary>
        /// Helper to retrieve a new, unused, Pop Id.
        /// </summary>
        public int NewPopId
        {
            get
            {
                while (Pops.ContainsKey(_newPopId))
                    ++_newPopId;
                return _newPopId;
            }
        }

        private int _newMarketId;
        /// <summary>
        /// Helper to retrieve a new, unused, Market Id.
        /// </summary>
        public int NewMarketId
        {
            get
            {
                while (Markets.ContainsKey(_newMarketId))
                    ++_newMarketId;
                return _newMarketId;
            }
        }

        private int _newFirmId;
        /// <summary>
        /// Helper to retrieve a new, unused, Firm Id.
        /// </summary>
        public int NewFirmId
        {
            get
            {
                while (Firms.ContainsKey(_newMarketId))
                    ++_newMarketId;
                return _newMarketId;
            }
        }

        #endregion NewIds

        /// <summary>
        /// A helper function which goes through all of the info in the system
        /// and ensures no data is duplicated and all ids actually exist.
        /// </summary>
        /// <exception cref="Exception">Throws an exception if any inconsistency is found.</exception>
        public void SanityCheck()
        {
            // TODO, this thing. Build as feels needed.
            ProcessesValid();

            TerritoriesValid();
        }

        private bool ProcessesValid()
        {
            // Processes with faliure tags should connect to a
            // product a product should only have 1 failure process.
            var failures = Processes.Values.Where(x => x.Tags.Any(y => y.Tag == ProcessTag.Failure));

            // the first input product is the product it is connected to.
            var productsWithFailures = new HashSet<int>();

            // insert each until either success or failure.
            foreach (var failure in failures)
                if (productsWithFailures.Add(failure.InputProducts.First().ProductId))
                    return false;

            return true;
        }

        private bool TerritoriesValid()
        {
            // for all territories
            foreach (var terr in SimpleTerritories)
            {
                // ensure that size = land + water
                if (terr.Size != (terr.Land + terr.Water))
                {
                    // make note.
                    return false;
                }

                // ensure that all plots add up to the territory size * 8
                if (terr.Land * 8 != (terr.Plots.Aggregate((a, c) => a + c)))
                {
                    // make note
                    return false;
                }

                // nodes must be either -1 or greater than 0
                if (terr.Nodes.Any(x => x.Stockpile != -1 || x.Stockpile < 0))
                {
                    // make note
                    return false;
                }

                // Resources must be either -1 or greater than 0
                if (terr.Resources.Any(x => x.Amount != -1 || x.Amount < 0))
                {
                    // make note
                    return false;
                }

                // Resources in Territory must be physical.

                // nodes must conatin real goods (not services)
            }
            return true;
        }

        /// <summary>
        /// Given a list of Territories find those which are not
        /// connected to the first in the list.
        /// </summary>
        /// <returns>A list of disconnected Territories. If no disconnects are found, the list will be empty.</returns>
        public List<string> FindDisconnectedTerritories(List<SimpleTerritoryDTO> territories) 
        {
            var result = new List<string>();

            // Breadth first search style
            // TODO, do this later.

            return result;
        }

        #region DeleteFunctions

        // TODO delete functions for all objects in our system.

        #endregion DeleteFunctions

        #region GenerationFunctions

        public void GenerateSkillLabors()
        {
            // for all skills
            foreach (var skill in Skills.Values)
            {
                // check that no product of the same name exists.
                var preex = Products.Values
                    .SingleOrDefault(x => x.Name == skill.Name);

                // if it does exist and it's a service, skip it.
                if (preex != null &&
                    preex.ContainsTag(ProductTag.Service))
                    continue;
                // if labor exists duplicate exists, create it
                if (preex == null)
                {
                    var labor = new ProductDTO
                    {
                        Id = NewProductId,
                        Name = skill.Name,
                        Mass = 0,
                        Quality = 1,
                        Fractional = true,
                        UnitName = "man hour",
                        Bulk = 0,
                    };

                    labor.Tags.Add(new AttachedProductTag
                    {
                        Tag = ProductTag.Service
                    });
                    labor.TagStrings
                        .Add(ProductTagInfo
                             .GetProductExample(ProductTag.Service));

                    Products.Add(labor.Id, labor);
                    continue;
                }

                // if a product with the same name exists, but is not 
                // a service, request authority to override.
                if (preex != null && 
                    !preex.ContainsTag(ProductTag.Service))
                {
                    // currently we raise a message box, letting you
                    // either override, skip, or cancel out.
                    var result = true;//MessageBox.Show("Duplicate non-service found, do you wish to override it? Yes, override, no Skip, Cancel, stop.", "Dupilcate found.", MessageBoxButtons.YesNoCancel);

                    // override
                    if (result == true)
                    {
                        preex = new ProductDTO
                        {
                            Id = preex.Id,
                            Name = skill.Name,
                            Mass = 0,
                            Quality = 1,
                            Fractional = true,
                            UnitName = "man hour",
                            Bulk = 0
                        };

                        Products.Add(preex.Id, preex);
                        continue;
                    }
                }
            }
        }

        #endregion GenerationFunctions

        #region LoadFunctions

        /// <summary>
        /// Load Products from File.
        /// </summary>
        /// <param name="fileName">The file to load from.</param>
        public void LoadProducts(string fileName)
        {
            var json = File.ReadAllText(fileName);
            List<ProductDTO> prods = JsonSerializer.Deserialize<List<ProductDTO>>(json);

            // ensure wants are loaded
            foreach (var prod in prods)
            {
                // if all wants loaded also, skip loading from text.
                if (prod.Wants.Count() == prod.WantStrings.Count())
                    continue;

                foreach (var want in prod.WantStrings)
                {
                    var data = WantDTO.DataFromString(want);

                    var wantId = GetWantByName(data.Item1).Id;

                    prod.Wants[wantId] = data.Item2;
                }
            }

            // ensure all tags are loaded
            foreach (var prod in prods)
            {
                foreach (var tag in prod.TagStrings)
                {
                    var data = ProductTagInfo.ProcessTagString(tag);

                    prod.Tags.Add(data);
                }
            }

            // Save all products.
            _newProductId = 0;
            foreach (var prod in prods)
            {
                // Check that there are no dups on load. No dups allowed.
                if (IsDuplicate(prod))
                    throw new InvalidDataException(
                        string.Format("Duplicate product of name '{0}' found. No duplicates allowd.",
                                prod.GetName()));

                prod.Id = NewProductId;
                Products.Add(prod.Id, prod);
            }
        }

        /// <summary>
        /// Loads wants from file.
        /// </summary>
        /// <param name="fileName">The file to load from.</param>
        public void LoadWants(string fileName)
        {
            var json = File.ReadAllText(fileName);
            List<WantDTO> wants = JsonSerializer.Deserialize<List<WantDTO>>(json);

            _newWantId = 0;
            foreach (var want in wants)
            {
                // check for name clashes
                if (Wants.Any(x => x.Value.Name == want.Name))
                    throw new InvalidOperationException(
                        string.Format("Want of the name '{0}' already exists. Cannot load duplicates.", want.Name));

                want.Id = NewWantId;
                Wants.Add(want.Id, want);
            }
        }

        /// <summary>
        /// Load Skills from file.
        /// </summary>
        /// <param name="filename">The file to load from.</param>
        public void LoadSkills(string filename)
        {
            var json = File.ReadAllText(filename);
            List<SkillDTO> skills = JsonSerializer.Deserialize<List<SkillDTO>>(json);

            _newSkillId = 0;
            foreach (var skill in skills)
            {
                skill.Id = NewSKillId;
                Skills.Add(skill.Id, skill);
            }

            foreach (var skill in skills)
                skill.SetDataFromStrings();
        }

        /// <summary>
        /// Load Skill Groups from file.
        /// </summary>
        /// <param name="filename">The file to load from.</param>
        public void LoadSkillGroups(string filename)
        {
            var json = File.ReadAllText(filename);
            List<SkillGroupDTO> groups = JsonSerializer.Deserialize<List<SkillGroupDTO>>(json);

            _newSkillGroupId = 0;
            foreach (var group in groups)
            {
                group.Id = NewSkillGroupId;
                SkillGroups.Add(group.Id, group);
            }

            SkillGroups = groups.ToDictionary(x => x.Id, y => (ISkillGroupDTO)y);
        }

        /// <summary>
        /// Load Processes from file.
        /// Should be loaded after Products, Wants, and Skills to ensure all connections
        /// are made.
        /// </summary>
        /// <remarks>
        /// Processes should be loaded after Products, Wants, and Skills.
        /// </remarks>
        /// <param name="filename">The file to load from.</param>
        public void LoadProcesses(string filename)
        {
            var json = File.ReadAllText(filename);
            
            List<ProcessDTO> processes = JsonSerializer.Deserialize<List<ProcessDTO>>(json, 
                new JsonSerializerOptions
                {
                    Converters = {
                        new AbstractConverter<ProcessProductDTO, IProcessProductDTO>(),
                        new AbstractConverter<ProcessWantDTO, IProcessWantDTO>()
                    }
                })!;

            _newProcessId = 0;
            foreach (var process in processes)
            {
                process.Id = NewProcessId;
                Processes.Add(process.Id, process);
            }

            processes.ForEach(x => x.SetTagsFromStrings());

            // if products are loaded.
            if (Products.Any())
            {
                // update failure processes on products
                var failures = processes.Where(x => x.Tags.Any(y => y.Tag == ProcessTag.Failure));

                // get attach to the first (and only) input product in the process
                foreach (var failure in failures)
                {
                    var prod = Products[failure.InputProducts.First().ProductId];

                    if (prod.Failure != null)
                        throw new InvalidDataException("Product has 2 failure processes.");
                    (((ProductDTO) prod)!).Failure = failure;
                }
            }

            bool warned = false;
            // load skills
            foreach (var process in Processes.Values)
            {
                var skillName = process.SkillName;

                if (string.IsNullOrEmpty(skillName))
                { // if null or empty, just continue, the process has no skill.
                    continue;
                }
                else if (string.IsNullOrWhiteSpace(skillName) && !warned)
                { // if skill name is not empty, but whitespace, then throw a warning message.
                    warned = true;
                    //MessageBox.Show("Skills on processes should be Either Empty or Null if not given a skill name.", "Invalid Skill Name",
                        //MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    ((ProcessDTO)process).SkillName = "";
                    continue;
                }

                // if not null or empty, it must be a skill
                ((ProcessDTO)process).SkillId = GetSkillByName(skillName).Id;
            }
        }

        /// <summary>
        /// Load Jobs from file.
        /// </summary>
        /// <param name="filename">The file to load from.</param>
        public void LoadJobs(string filename)
        {
            var json = File.ReadAllText(filename);

            List<JobDTO> jobs = JsonSerializer.Deserialize<List<JobDTO>>(json);

            _newJobId = 0;
            foreach (var job in jobs)
            {
                job.Id = NewJobId;
                Jobs.Add(job.Id, job);
            }
        }

        /// <summary>
        /// Load Tech Families from file.
        /// </summary>
        /// <param name="filename">The file to load from.</param>
        public void LoadTechFamilies(string filename)
        {
            var json = File.ReadAllText(filename);

            List<TechFamilyDTO> techFamilies = JsonSerializer.Deserialize<List<TechFamilyDTO>>(json);

            _newJobId = 0;
            foreach (var fam in techFamilies)
            {
                // ensure no duplicate tech family names.
                if (TechFamilies.Values.Any(x => x.Name == fam.Name))
                    throw new InvalidDataException(
                        string.Format("Duplicate Tech Family of name '{0}' found. No duplicates allowd.", 
                                 fam.Name));

                fam.Id = NewTechFamilyId;
                TechFamilies.Add(fam.Id, fam);
            }

            foreach (var fam in techFamilies)
                fam.SetRelatedFamiliesFromStrings();
        }

        /// <summary>
        /// Load Technologies from file.
        /// </summary>
        /// <param name="filename">The file to load from.</param>
        public void LoadTechs(string filename)
        {
            var json = File.ReadAllText(filename);

            List<TechnologyDTO> techs = JsonSerializer.Deserialize<List<TechnologyDTO>>(json);

            _newTechId = 0;
            // set families
            foreach (var tech in techs)
            {
                foreach (var fam in tech.Families)
                {
                    tech.FamilyIds.Add(TechFamilies.Values.Single(x => x.Name == fam).Id);
                }
            }

            // add techs to the list
            foreach (var tech in techs)
            {
                tech.Id = NewTechId;

                // check if duplicate name.
                if (Technologies.Values
                    .Select(x => x.Name).Contains(tech.Name))
                    throw new InvalidDataException(
                        string.Format("Duplicate Technology of name '{0}' found. No duplicates allowd.",
                                 tech.Name));

                Technologies.Add(tech.Id, tech);
            }

            // connect techs to parents and children.
            foreach (var tech in techs)
            { 
                foreach (var child in tech.Children)
                    tech.ChildrenIds.Add(Technologies.Values.Single(x => x.Name == child).Id);

                foreach (var parent in tech.Parents)
                    tech.ParentIds.Add(Technologies.Values.Single(x => x.Name == parent).Id);
            }
        }

        /// <summary>
        /// Load Species from file.
        /// </summary>
        /// <param name="filename">The file to load from.</param>
        public void LoadSpecies(string filename) 
        {
            var json = File.ReadAllText(filename);

            List<SpeciesDTO> species = JsonSerializer.Deserialize<List<SpeciesDTO>>(json
                , new JsonSerializerOptions
                {
                    Converters = {
                        new AbstractConverter<SpeciesNeedDTO, ISpeciesNeedDTO>(),
                        new AbstractConverter<SpeciesWantDTO, ISpeciesWantDTO>()
                    }
                });

            // get desires
            foreach (var spec in species)
            {
                spec.Id = NewSpeciesId;
                // add to storage
                Species.Add(spec.Id, spec);

                // get needs.
                foreach (var need in spec.Needs)
                {
                    ((SpeciesNeedDTO)need).ProductId = GetProductByFullName(need.Product).Id;
                }
                // get wants
                foreach (var want in spec.Wants)
                {
                    ((SpeciesWantDTO)want).WantId = Wants.Values.Single(x => x.Name == want.Want).Id;
                }
            }

            // add species relations
            foreach (var spec in species)
            {
                foreach (var rel in spec.RelatedSpecies)
                {
                    spec.RelatedSpeciesIds
                        .Add(Species.Values
                                    .Single(x => x.ToString() == rel).Id);
                }
            }

            // TODO Species Tags
        }

        public void LoadCultures(string fileName)
        {
            var json = File.ReadAllText(fileName);

            List<CultureDTO> cultures = JsonSerializer.Deserialize<List<CultureDTO>>(json,
                new JsonSerializerOptions
                {
                    Converters =
                    {
                        new AbstractConverter<CultureNeedDTO, ICultureNeedDTO>(),
                        new AbstractConverter<CultureWantDTO, ICultureWantDTO>()
                    }
                });

            // get desires
            foreach (var culture in cultures)
            {
                culture.Id = NewCultureId;
                Cultures.Add(culture.Id, culture);

                // get needs
                foreach (var need in culture.Needs)
                {
                    ((CultureNeedDTO)need).ProductId = GetProductByFullName(need.Product).Id;
                }
                // get wants
                foreach (var want in culture.Wants)
                {
                    ((CultureWantDTO)want).WantId = Wants.Values.Single(x => x.Name == want.Want).Id;
                }
            }

            // add Culture relations
            foreach (var culture in cultures)
            {
                foreach (var rel in culture.RelatedCultures)
                {
                    culture.RelatedCulturesIds
                        .Add(Species.Values
                                    .Single(x => x.ToString() == rel).Id);
                }
            }

            // TODO Culutre Tags
        }

        public void LoadPops(string filename)
        {
            var json = File.ReadAllText(filename);

            List<PopDTO> pops = JsonSerializer.Deserialize<List<PopDTO>>(json,
                new JsonSerializerOptions
                {
                    Converters =
                    {
                        new AbstractConverter<PopCulturePortion, IPopCulturePortion>(),
                        new AbstractConverter<PopSpeciesPortion, IPopSpeciesPortion>()
                    }
                });

            // connect Everything else
            foreach (var pop in pops)
            {
                pop.JobId = GetJobByName(pop.Job).Id;

                // firms and markets set pops from their end.

                pop.SkillId = GetSkillByName(pop.Skill).Id;

                // species
                foreach (var spec in pop.SpeciesPortions)
                {
                    ((PopSpeciesPortion)spec).SpeciesId = GetSpeciesByName(spec.Species).Id;
                }

                // Cultures
                foreach (var culture in pop.CulturePortions)
                {
                    ((PopCulturePortion)culture).CultureId = GetCultureByName(culture.Culture).Id;
                }
            }

            foreach (var pop in pops)
            {
                Pops.Add(pop.Id, pop);
            }
        }

        public void LoadSimpleTerritories(string filename)
        {
            var json = File.ReadAllText(filename);

            List<SimpleTerritoryDTO> territories = JsonSerializer.Deserialize<List<SimpleTerritoryDTO>>(json,
                new JsonSerializerOptions
                {
                    Converters =
                    {
                        new AbstractConverter<NeighborConnection, INeighborConnection>(),
                        new AbstractConverter<ResourceNode, IResourceNode>(),
                        new AbstractConverter<TerritoryResource, ITerritoryResource>()
                    }
                });

            // link stuff up.
            foreach (var terr in territories)
            {
                // Node Product Ids
                foreach (var node in terr.Nodes)
                {
                    ((ResourceNode)node).ResourceId 
                        = GetProductByFullName(node.Resource).Id;
                }

                // Resource Product Ids.
                foreach (var resource in terr.Resources)
                {
                    ((TerritoryResource)resource).ResourceId 
                        = GetProductByFullName(resource.Resource).Id;
                }

                // add to territory list.
                SimpleTerritories.Add(terr);
            }
        }

        public void LoadMarkets(string filename)
        {
            var json = File.ReadAllText(filename);

            List<MarketDTO> markets = JsonSerializer.Deserialize<List<MarketDTO>>(json);

            // link stuff up.
            // nothing needs linked currently.
            // Pops connect to markets.
            // Territories don't have Ids.
            // Why bother getting IDS, these are just DTOs, not workable data.

            foreach(var market in markets)
            {
                market.Id = NewMarketId;

                Markets[market.Id] = market;
            }
        }

        public void LoadFirms(string filename)
        {
            var json = File.ReadAllText(filename);

            List<FirmDTO> firms = JsonSerializer.Deserialize<List<FirmDTO>>(json,
                new JsonSerializerOptions
                {
                    Converters =
                    {
                        new AbstractConverter<JobWageData, IJobWageData>()
                    }
                });

            List<string> Errors = new List<string>();

            // go through each firm and make trivial connections.
            foreach (var firm in firms)
            {
                firm.Id = NewFirmId;
                Firms.Add(firm.Id, firm);

                // Connect Jobs
                foreach (var job in firm.JobData)
                {
                    // ensure job exists
                    ((JobWageData)job).JobId = GetJobByName(job.JobName).Id;
                }

                foreach (var proc in firm.Processes)
                {
                    try
                    {
                        GetProcessByName(proc);
                    }
                    catch (InvalidOperationException ex)
                    {
                        Errors.Add(string.Format("Process Not Found: {0}\n", proc));
                    }
                }

                // connect products and resources
                // JKLOL, just throw errors if we can't connect them up.
                // Getting IDs is overrated.
                foreach (var prod in firm.ProductPrices.Keys)
                {
                    try
                    {
                        GetProductByFullName(prod);
                    }
                    catch (InvalidOperationException)
                    {
                        Errors.Add(string.Format("Salable Product Not Found: {0}\n", prod));
                    }
                }

                // resource check.
                foreach (var prod in firm.Resources.Keys)
                {
                    try
                    {
                        GetProductByFullName(prod);
                    }
                    catch (InvalidOperationException)
                    {
                        Errors.Add(string.Format("Resource Product Not Found: {0}\n", prod));
                    }
                }
            }

            // if any errors throw
            if (Errors.Count > 0)
            {
                throw new InvalidDataException("Data Missing: \n" + string.Concat(Errors));
            }

            // go back through and do any additional internal connections needed.
        }

        public void ConnectPopsFirmsAndMarkets()
        {
            // start with pops
            foreach (var pop in Pops.Values)
            {
                // connect pops and market
                var market = Markets.Values.Single(x => x.Name == pop.Market);
                ((PopDTO)pop).MarketId = market.Id;
                market.PopIds.Add(pop.Id);

                // connect pops to firms
                var firm = Firms.Values.Single(x => x.Name == pop.Firm);
                ((PopDTO)pop).FirmId = firm.Id;
                firm.Employees.Add(pop.Id);
            }

            var Errors = new List<string>();

            // connect markets with firms
            foreach (var firm in Firms.Values)
            {
                // check that the market references the firm back
                var market = Markets.Values.SingleOrDefault(x => x.Name == firm.Market);

                // if market not found, mark it and continue.
                if (market == null)
                {
                    Errors.Add("Missing Market: " + firm.Market);
                    continue;
                }
                // if market doesn't contain firm mark it and return.
                if (!market.Firms.Contains(firm.Name))
                {
                    Errors.Add("Missing Firm: " + firm.Name);
                    continue;
                }

                // both exist, connect them together
                ((FirmDTO)firm).MarketId = market.Id;
                market.FirmIds.Add(firm.Id);

                // repeat with other regions
                foreach (var region in firm.Regions)
                {
                    // check that the market references the firm back
                    var otherMarket = Markets.Values.SingleOrDefault(x => x.Name == region);

                    // if market not found, mark it and continue.
                    if (otherMarket == null)
                    {
                        Errors.Add("Missing Market: " + firm.Market);
                        continue;
                    }
                    // if market doesn't contain firm mark it and return.
                    if (!otherMarket.Firms.Contains(firm.Name))
                    {
                        Errors.Add("Missing Firm: " + firm.Name);
                        continue;
                    }

                    // both exist, so add them together.
                    firm.RegionIds.Add(otherMarket.Id);
                    otherMarket.FirmIds.Add(firm.Id);
                }
            }

            if (Errors.Count > 0)
                throw new InvalidDataException("Data Missing: \n" + string.Concat(Errors));
        }

        #endregion LoadFunctions

        #region SaveFunctions

        /// <summary>
        /// Saves all products to file.
        /// </summary>
        /// <param name="fileName"></param>
        public void SaveProducts(string fileName)
        {
            var options = new JsonSerializerOptions();
            options.WriteIndented = true;
            options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            // add all but the required items.
            string json = JsonSerializer.Serialize(Products.Values
                .Where(x => !RequiredItems
                    .Products
                    .Values
                    .Select(y => y.GetName())
                    .Contains(x.GetName()))
                .ToList(), options);
            File.WriteAllText(fileName, json);
        }

        /// <summary>
        /// Saves wants to the given file.
        /// </summary>
        /// <param name="filename">The file to save it to.</param>
        public void SaveWants(string filename)
        {
            var options = new JsonSerializerOptions();
            options.WriteIndented = true;
            string json = JsonSerializer.Serialize(Wants.Values
                .Where(x => !RequiredItems
                    .Wants
                    .Values
                    .Select(y => y.Name)
                    .Contains(x.Name))
                .ToList(), options);
            File.WriteAllText(filename, json);
        }

        /// <summary>
        /// Save Skill Groups to the given file.
        /// </summary>
        /// <param name="filename">The file to save it to.</param>
        public void SaveSkillGroups(string filename)
        {
            var options = new JsonSerializerOptions();
            options.WriteIndented = true;
            string json = JsonSerializer.Serialize(SkillGroups.Values.ToList(), options);
            File.WriteAllText(filename, json);
        }

        /// <summary>
        /// Save skills to file.
        /// </summary>
        /// <param name="filename">The file to save it to.</param>
        public void SaveSkills(string filename)
        {
            var options = new JsonSerializerOptions();
            options.WriteIndented = true;
            string json = JsonSerializer.Serialize(Skills.Values.ToList(), options);
            File.WriteAllText(filename, json);
        }

        /// <summary>
        /// Save Processes to file.
        /// </summary>
        /// <param name="filename">The file to save it to.</param>
        public void SaveProcesses(string filename)
        {
            var options = new JsonSerializerOptions();
            options.WriteIndented = true;
            options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            string json = JsonSerializer.Serialize(Processes.Values.ToList(), options);
            File.WriteAllText(filename, json);
        }

        /// <summary>
        /// Save Jobs to file.
        /// </summary>
        /// <param name="filename">The file to save it to.</param>
        public void SaveJobs(string filename)
        {
            var options = new JsonSerializerOptions();
            options.WriteIndented = true;
            options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            string json = JsonSerializer.Serialize(Jobs.Values.ToList(), options);
            File.WriteAllText(filename, json);
        }

        /// <summary>
        /// Save Tech Families to file.
        /// </summary>
        /// <param name="filename">The file to save it to.</param>
        public void SaveTechFamilies(string filename)
        {
            var options = new JsonSerializerOptions();
            options.WriteIndented = true;
            options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            string json = JsonSerializer.Serialize(TechFamilies.Values
                .ToList(), options);
            File.WriteAllText(filename, json);
        }

        /// <summary>
        /// Save Technologies to file.
        /// </summary>
        /// <param name="filename">The file to save it to.</param>
        public void SaveTechs(string filename)
        {
            var options = new JsonSerializerOptions();
            options.WriteIndented = true;
            options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            string json = JsonSerializer.Serialize(Technologies.Values
                .Where(x => !RequiredItems
                    .Technologies
                    .Values
                    .Select(y => y.Name)
                    .Contains(x.Name))
                .ToList(), options);
            File.WriteAllText(filename, json);
        }

        public void SaveSpecies(string filename)
        {
            var options = new JsonSerializerOptions();
            options.WriteIndented = true;
            options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            string json = JsonSerializer.Serialize(Species.Values, options);

            File.WriteAllText(filename, json);
        }

        public void SaveCultures(string filename)
        {
            var options = new JsonSerializerOptions();
            options.WriteIndented = true;
            options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            string json = JsonSerializer.Serialize(Cultures.Values, options);

            File.WriteAllText (filename, json);
        }

        public void SavePops(string filename)
        {
            var options = new JsonSerializerOptions();
            options.WriteIndented = true;
            options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            string json = JsonSerializer.Serialize(Pops.Values, options);

            File.WriteAllText(filename, json);
        }

        public void SaveSimpleTerritories(string filename)
        {
            var options = new JsonSerializerOptions();
            options.WriteIndented = true;
            options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            string json = JsonSerializer.Serialize(SimpleTerritories, options);

            File.WriteAllText(filename, json);
        }

        public void SaveMarkets(string filename)
        {
            var options = new JsonSerializerOptions();
            options.WriteIndented = true;
            options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            string json = JsonSerializer.Serialize(Markets.Values, options);

            File.WriteAllText(filename, json);
        }

        public void SaveFirms(string filename)
        {
            var options = new JsonSerializerOptions();
            options.WriteIndented = true;
            options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            string json = JsonSerializer.Serialize(Firms.Values, options);

            File.WriteAllText(filename, json);
        }

        #endregion SaveFunctions

        /// <summary>
        /// A Test Funciton, should be removed later.
        /// </summary>
        public void LoadAll()
        {
            // Load Required Wants
            // Basic stuff loaded first
            LoadWants(GetDataFilePath("CommonWants.json"));

            // Get All Tech Families
            LoadTechFamilies(GetDataFilePath("CommonTechFamilies.json"));

            // Get Require Techs
            // Load Technologies
            LoadTechs(GetDataFilePath("CommonTechs.json"));


            // load required products.
            // More advanced stuff next.
            LoadProducts(GetDataFilePath("CommonProducts.json"));

            // Get all skill groups
            LoadSkillGroups(GetDataFilePath("CommonSkillGroups.json"));

            // Get All Skills
            LoadSkills(GetDataFilePath("CommonSkills.json"));

            // Get All Processes
            LoadProcesses(GetDataFilePath("CommonProcesses.json"));

            // Get all Jobs.
            LoadJobs(GetDataFilePath("CommonJobs.json"));

            // Get all Species
            LoadSpecies(GetDataFilePath("CommonSpecies.json"));

            // Get All Cultures
            LoadCultures(GetDataFilePath("CommonCultures.json"));

            // Get All Pops
            LoadPops(GetDataFilePath("Pops.json"));

            // Get All Territories
            LoadSimpleTerritories(GetDataFilePath("Territories.json"));

            // Get All Markets
            LoadMarkets(GetDataFilePath("Markets.json"));

            // Get All Firms
            LoadFirms(GetDataFilePath("Firms.json"));

            // Connect Pops, Firms, and Markets
            ConnectPopsFirmsAndMarkets();
        }

        /// <summary>
        /// Old load function, remove/replace this with actual load function.
        /// </summary>
        /// <param name="UniverseName"></param>
        /// <returns></returns>
        [Obsolete]
        public bool LoadData(string UniverseName)
        {
            return true;
        }
    }
}
