using EconomicCalculator.DTOs.Planet;
using EconomicCalculator.DTOs.Products;
using EconomicCalculator.DTOs.Technology;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using EconomicCalculator.DTOs.Wants;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using EconomicCalculator.DTOs.Skills;
using EconomicCalculator.DTOs.Processes;
using EconomicCalculator.DTOs.Products.ProductTags;
using EconomicCalculator.DTOs;
using EconomicCalculator.DTOs.Processes.ProcessTags;
using EconomicCalculator.DTOs.Jobs;
using System.Windows.Forms;

namespace EconomicCalculator
{
    /// <summary>
    /// A manager for the worldstate of data currently in existence.
    /// Used to get system data wherever needed.
    /// There should only ever be one of these per active program.
    /// </summary>
    public sealed class Manager
    {
        #region Singleton

        private static Manager instance = null;
        private static readonly object padlock = new object();

        // private Ctor, this is a singleton.
        private Manager()
        {
            UniverseName = "";
            //Planets = new Dictionary<int, Planet>();
            Technologies = new Dictionary<int, ITechnology>();
            Products = new Dictionary<int, IProduct>();
            Wants = new Dictionary<int, IWant>();
            Skills = new Dictionary<int, ISkill>();
            SkillGroups = new Dictionary<int, ISkillGroup>();
            Processes = new Dictionary<int, IProcess>();
            Jobs = new Dictionary<int, IJob>();
        }        

        /// <summary>
        /// The current available instance.
        /// TODO, consider moving to EconomicCalculator project
        /// as this could be used for multiple interfaces.
        /// </summary>
        public static Manager Instance
        {
            get
            {// basic thread locking
                lock (padlock)
                {
                    // if instance has not been built, make it.
                    if (instance == null)
                    {
                        instance = new Manager();
                    }
                }
                // return singleton instance.
                return instance;
            }
        }

        #endregion Singleton

        #region DataStorage

        /// <summary>
        /// The Technologies available to the system, accessible by Id.
        /// </summary>
        public IDictionary<int, ITechnology> Technologies { get; }

        public IDictionary<int, IProduct> Products { get; set; }

        public IDictionary<int, IWant> Wants { get; set; }

        public IDictionary<int, ISkill> Skills { get; set; }

        public IDictionary<int, ISkillGroup> SkillGroups { get; set; }

        public IDictionary<int, IProcess> Processes { get; set; }

        public IDictionary<int, IJob> Jobs { get; set; }

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
        public bool ContainsProduct(IProduct product)
        {
            // if the id is contained then yes, it exists.
            return Products.ContainsKey(product.Id);
        }
        
        /// <summary>
        /// Checks if a want already exists. Does not check for duplicates.
        /// </summary>
        /// <param name="want">The want to check for.</param>
        /// <returns></returns>
        public bool ContainsWant(IWant want)
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
        public IProduct GetProductByName(string name)
        {
            Tuple<string, string> names = Product.GetProductNames(name);
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
        public IWant GetWantByName(string name)
        {
            return Wants.Values.Single(x => x.Name == name);
        }

        /// <summary>
        /// Retrieve a Skill GRoup based on it's name.
        /// </summary>
        /// <param name="name">The desired Group.</param>
        /// <returns></returns>
        public ISkillGroup GetSkillGroupByName(string name)
        {
            return SkillGroups.Values.Single(x => x.Name == name);
        }

        /// <summary>
        /// Retrieve a Skill based on it's name.
        /// </summary>
        /// <param name="name">The name of the Skill</param>
        /// <returns></returns>
        public ISkill GetSkillByName(string name)
        {
            return Skills.Values.Single(x => x.Name == name);
        }

        /// <summary>
        /// Retrieve a process based on it's name.
        /// </summary>
        /// <param name="name">Name of the process.</param>
        /// <returns></returns>
        public IProcess GetProcessByName(string name)
        {
            Tuple<string, string> names = Process.GetProcessNames(name);
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
        public bool IsDuplicate(IProduct product)
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
        public bool IsDuplicate(IProcess process)
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
        public bool IsDupilcate(IJob job)
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
        public IProduct FindDuplicate(IProduct product)
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
        public IWant FindDuplicate(IWant want)
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
        public IJob FindDuplicate(IJob job)
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
                    var labor = new Product
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
                        preex = new Product
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
            List<Product> prods = JsonSerializer.Deserialize<List<Product>>(json);

            // ensure wants are loaded
            foreach (var prod in prods)
            {
                // if all wants loaded also, skip loading from text.
                if (prod.Wants.Count() == prod.WantStrings.Count())
                    continue;

                foreach (var want in prod.WantStrings)
                {
                    var data = Want.DataFromString(want);

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
            List<Want> wants = JsonSerializer.Deserialize<List<Want>>(json);

            _newWantId = 0;
            foreach (var want in wants)
            {
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
            List<Skill> skills = JsonSerializer.Deserialize<List<Skill>>(json);

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
            List<SkillGroup> groups = JsonSerializer.Deserialize<List<SkillGroup>>(json);

            _newSkillGroupId = 0;
            foreach (var group in groups)
            {
                group.Id = NewSkillGroupId;
                SkillGroups.Add(group.Id, group);
            }

            SkillGroups = groups.ToDictionary(x => x.Id, y => (ISkillGroup)y);
        }

        /// <summary>
        /// Load Processes from file.
        /// </summary>
        /// <param name="filename">The file to load from.</param>
        public void LoadProcesses(string filename)
        {
            var json = File.ReadAllText(filename);
            
            List<Process> processes = JsonSerializer.Deserialize<List<Process>>(json, 
                new JsonSerializerOptions
                {
                    Converters = {
                        new AbstractConverter<ProcessProduct, IProcessProduct>(),
                        new AbstractConverter<ProcessWant, IProcessWant>()
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
                    ((Product)prod).Failure = failure;
                }
            }
        }

        /// <summary>
        /// Load Jobs from file.
        /// </summary>
        /// <param name="filename">The file to load frime.</param>
        public void LoadJobs(string filename)
        {
            var json = File.ReadAllText(filename);

            List<Job> jobs = JsonSerializer.Deserialize<List<Job>>(json);

            _newJobId = 0;
            foreach (var job in jobs)
            {
                job.Id = NewJobId;
                Jobs.Add(job.Id, job);
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
            string json = JsonSerializer.Serialize(Products.Values.ToList(), options);
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
            string json = JsonSerializer.Serialize(Wants.Values.ToList(), options);
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

        #endregion SaveFunctions

        /// <summary>
        /// A Test Funciton, should be removed later.
        /// </summary>
        public void LoadAll()
        {
            // Basic stuff loaded first
            LoadWants(@"D:\Projects\EconomicCalculator\EconomicCalculator\Data\CommonWants.json");

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
        }

        /// <summary>
        /// Old load function, remove/replace this with actual load function.
        /// </summary>
        /// <param name="UniverseName"></param>
        /// <returns></returns>
        [Obsolete]
        public bool LoadData(string UniverseName)
        {
            this.UniverseName = UniverseName;

            // TODO, actually load from files. 
            // Load set equal to earth in effect.
            var earth = new Planet();

            earth.LoadPlanet(UniverseFolder, "TestTerra");

            earth.Name = "TestTerra";
            earth.Id = 0;
            // topography is default to sphere
            // type is not used.
            earth.SetBySurfaceArea(510_064_472);
            // Set arbitrary seed for now.
            earth.Seed = 1;
            earth.Dead = false;
            earth.Mass = 5.972e24;
            earth.AirPressure = 1;
            earth.Tempurature = 14;
            earth.manager = this;

            //earth.SavePlanet(UniverseFolder);

            //Planets.Add(earth.Id, earth);

            return true;
        }
    }
}
