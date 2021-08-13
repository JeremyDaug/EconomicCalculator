using EconomicCalculator.Storage.Planet;
using EconomicCalculator.Storage.Products;
using EconomicCalculator.Storage.Technology;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using EconomicCalculator.Storage.Wants;
using System.Text.Encodings.Web;
using EconomicCalculator.Storage.ProductTags;

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
            ProductTagInfo = new Dictionary<int, IProductTagInfo>();
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

        public IDictionary<int, IProductTagInfo> ProductTagInfo { get; set; }

        #endregion DataStorage

        // TODO make this less shitty.
        public string DataFolder => "D:\\Projects\\EconomicCalculator\\EconomicCalculator\\Data\\";

        public string DefaultIcon => @"ProductImages\DefaultIcon.png";

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
        /// Returns the Id of a want of the given name.
        /// </summary>
        /// <param name="name">The name we want the Id of.</param>
        /// <returns>The Id of the want with our name.</returns>
        /// <exception cref="InvalidOperationException">Want was not found.</exception>
        public int GetWantId(string name)
        {
            return Wants.Values.Single(x => x.Name == name).Id;
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

        public string UniverseName { get; set; }

        public string UniverseFolder => Path.Combine(DataFolder, UniverseName);

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

        /// <summary>
        /// A helper function which goes through all of the info in the system
        /// and ensures no data is duplicated and all ids actually exist.
        /// </summary>
        /// <exception cref="Exception">Throws an exception if any inconsistency is found.</exception>
        public void SanityCheck()
        {
            // TODO, this thing. Build as feels needed.
        }

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

            Products = prods.ToDictionary(x => x.Id, y => (IProduct)y);
        }

        public void LoadWants(string fileName)
        {
            var json = File.ReadAllText(fileName);
            List<Want> wants = JsonSerializer.Deserialize<List<Want>>(json);

            Wants = wants.ToDictionary(x => x.Id, y => (IWant)y);
        }

        public void LoadProductTagInfo(string filename)
        {
            var json = File.ReadAllText(filename);
            List<ProductTagInfo> tags = JsonSerializer.Deserialize<List<ProductTagInfo>>(json);

            ProductTagInfo = tags.ToDictionary(x => x.Id, y => (IProductTagInfo)y);
        }

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

        public void SaveWants(string filename)
        {
            var options = new JsonSerializerOptions();
            options.WriteIndented = true;
            string json = JsonSerializer.Serialize(Wants.Values.ToList(), options);
            File.WriteAllText(filename, json);
        }

        public void SaveProductTagInfo(string filename)
        {
            var options = new JsonSerializerOptions();
            options.WriteIndented = true;
            string json = JsonSerializer.Serialize(ProductTagInfo.Values.ToList(), options);
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
