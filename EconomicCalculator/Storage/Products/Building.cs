using EconomicCalculator.Randomizer;
using EconomicCalculator.Storage.Organizations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage.Products
{
    internal class Building : Product, IBuilding
    {
        public Building(IRandomizer rand) : base(rand)
        {
        }

        public BuildingType BuildingType => throw new NotImplementedException();

        public double PersonSpace => throw new NotImplementedException();

        public double AvailableStorageSpace => throw new NotImplementedException();

        public IList<string> Specializations => throw new NotImplementedException();

        public ITerritory Location => throw new NotImplementedException();

        public int PlotsRequired => throw new NotImplementedException();
    }
}
