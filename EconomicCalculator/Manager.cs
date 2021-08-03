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
            Planets = new Dictionary<int, Planet>();
            Technologies = new Dictionary<int, ITechnology>();
            Products = new Dictionary<int, Product>();
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

        public IDictionary<int, Product> Products { get; set; }

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
        public bool ContainsProduct(Product product)
        {
            // if the id is contained then yes, it exists.
            return Products.ContainsKey(product.Id);
        }

        /// <summary>
        /// Checks if a product is a duplicate of another or not.
        /// </summary>
        /// <param name="product">The product to check for.</param>
        /// <returns></returns>
        public bool IsDuplicate(Product product)
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
        /// Finds the duplicate product, if it exists. Returns null if not found.
        /// If it shares an ID with a product, it will not return that product.
        /// Only those who are proper duplicates.
        /// </summary>
        /// <param name="product">The product to find the duplicate of.</param>
        /// <returns></returns>
        public Product FindDuplicate(Product product)
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

        public string UniverseName { get; set; }

        public string UniverseFolder => Path.Combine(DataFolder, UniverseName);

        public Dictionary<int, Planet> Planets { get; }

        public int _newProductId;

        /// <summary>
        /// Helper to retrieve a new, unused, product Id.
        /// </summary>
        public int NewProductId
        {
            get
            {
                while (Products.Keys.Contains(_newProductId))
                {
                    ++_newProductId;
                }

                return _newProductId;
            }
        }

        public void LoadProducts(string fileName)
        {
            var json = File.ReadAllText(fileName);
            List<Product> prods = JsonSerializer.Deserialize<List<Product>>(json);

            Products = prods.ToDictionary(x => x.Id, y => y);
        }

        public void SaveProducts(string fileName)
        {
            var options = new JsonSerializerOptions();
            options.WriteIndented = true;
            string json = JsonSerializer.Serialize(Products.Values.ToList(), options);
            File.WriteAllText(fileName, json);
        }

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

            Planets.Add(earth.Id, earth);

            return true;
        }
    }
}
