using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage
{
    internal class Territory : ITerritory
    {
        public IMarket Market { get; }

        public string Name { get; }

        public double Extent { get; }

        public int Elevation { get; }

        public double WaterLevel { get; }

        public bool HasRiver { get; }

        public IDictionary<Guid, double> Ownership { get; }
    }
}
