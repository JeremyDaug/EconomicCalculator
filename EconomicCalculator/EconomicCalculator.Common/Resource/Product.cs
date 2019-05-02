using System;

namespace EconomicCalculator.Common.Resource
{
    public class Product
    {
        public string Name { get; set; }
        public double PricePerUnit { get; set; }
        public ProductType ProductType { get; set; }
        public string Variant { get; set; }
    }

}
