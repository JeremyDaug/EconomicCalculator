using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using EconomicCalculator.DTOs.Products;
using EconomicCalculator.DTOs.Wants;
using EconomicCalculator.DTOs.Jobs;
using EconomicCalculator.DTOs.Processes;
using EconomicCalculator.DTOs.Skills;
using EconomicCalculator.DTOs.Processes.ProcessTags;
using EconomicCalculator.DTOs.Products.ProductTags;
using System.Windows.Forms;
using EconomicCalculator.DTOs;
using EconomicCalculator.Objects;
using AutoMapper;
using EconomicCalculator.Objects.Wants;
using EconomicCalculator.DTOs.Technology;
using EconomicCalculator.Enums;
using EconomicCalculator.DTOs.Pops.Species;
using EconomicCalculator.DTOs.Pops.Species.AttachedTagData;

namespace EconomicCalculator
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

        private readonly EconCalcAutomapperProfile mapper;

        // private Ctor, this is a singleton.
        private DTOManager()
        {
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
            mapper = new EconCalcAutomapperProfile();
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

        #endregion DataStorage

        // TODO make this less shitty.
        public string DataFolder => "D:\\Projects\\EconomicCalculator\\EconomicCalculator\\Data\\";

        public string DefaultIcon => @"ProductImages\DefaultIcon.png";

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
        /// Helper to retrieve a new, unused, Tech Id.
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
                    var result = MessageBox.Show("Duplicate non-service found, do you wish to override it? Yes, override, no Skip, Cancel, stop.", "Dupilcate found.", MessageBoxButtons.YesNoCancel);

                    // override
                    if (result == DialogResult.Yes)
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
                    else if (result == DialogResult.No)
                    {
                        continue;
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        return;
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
                });

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
                    ((ProductDTO)prod).Failure = failure;
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
                    MessageBox.Show("Skills on processes should be Either Empty or Null if not given a skill name.", "Invalid Skill Name",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
        /// <param name="filename">The file to load frime.</param>
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
        /// <param name="filename">The file to load frime.</param>
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
        /// <param name="filename">The file to load frime.</param>
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

            // get products for needs
            foreach (var spec in species)
            {
                // add to storage
                Species.Add(NewSpeciesId, spec);

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

        #endregion SaveFunctions

        /// <summary>
        /// A Test Funciton, should be removed later.
        /// </summary>
        public void LoadAll()
        {
            var mapper = InitMapper();

            // Load Required Wants
            foreach (var item in RequiredItems.Wants.Values)
            {
                var newWant = mapper.Map<WantDTO>(item);
                Wants.Add(newWant.Id, newWant);
            }
            // Basic stuff loaded first
            LoadWants(@"D:\Projects\EconomicCalculator\EconomicCalculator\Data\CommonWants.json");

            // Get All Tech Families
            LoadTechFamilies(@"D:\Projects\EconomicCalculator\EconomicCalculator\Data\CommonTechFamilies.json");

            // Get Require Techs
            foreach (var item in RequiredItems.Technologies)
            {
                var newTech = mapper.Map<TechnologyDTO>(item.Value);
                Technologies.Add(newTech.Id, newTech);
            }
            // Load Technologies
            LoadTechs(@"D:\Projects\EconomicCalculator\EconomicCalculator\Data\CommonTechs.json");


            // load required products.
            foreach (var item in RequiredItems.Products.Values)
            {
                var newProd = mapper.Map<ProductDTO>(item);
                Products.Add(newProd.Id, newProd);
            }
            // More advanced stuff next.
            LoadProducts(@"D:\Projects\EconomicCalculator\EconomicCalculator\Data\CommonProducts.json");

            // Get all skill groups
            LoadSkillGroups(@"D:\Projects\EconomicCalculator\EconomicCalculator\Data\CommonSkillGroups.json");

            // Get All Skills
            LoadSkills(@"D:\Projects\EconomicCalculator\EconomicCalculator\Data\CommonSkills.json");

            // Get All Processes
            LoadProcesses(@"D:\Projects\EconomicCalculator\EconomicCalculator\Data\CommonProcesses.json");

            // Get all Jobs.
            LoadJobs(@"D:\Projects\EconomicCalculator\EconomicCalculator\Data\CommonJobs.json");

            // Get all Species
            LoadSpecies(@"D:\Projects\EconomicCalculator\EconomicCalculator\Data\CommonSpecies.json");
        }

        private Mapper InitMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<EconCalcAutomapperProfile>();
            });
            var mapper = new Mapper(config);
            return mapper;
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
