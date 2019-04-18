using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Common
{
    public class Crop
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public CropType CropType { get; set; }
        public Product ReseedProduct { get; set; }
        public float ReseedAmount { get; set; }
        public List<Product> Produces { get; set; }
        public List<double> ProductionAmounts { get; set; }
        public string SowingTime { get; set; }
        public int GrowthTime { get; set; }
        public string HarvestTime { get; set; }
        public double Spoilage { get; set; }
    }
}
