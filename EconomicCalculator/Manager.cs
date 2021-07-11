using EconomicCalculator.Storage.Planet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        // TODO make this less shitty.
        public string SavesFolder => "D:\\Projects\\EconomicCalculator\\EconomicCalculator\\Data";

        public string UniverseName { get; set; }

        public string UniverseFolder => Path.Combine(SavesFolder, UniverseName);

        public Dictionary<int, Planet> Planets { get; }

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
