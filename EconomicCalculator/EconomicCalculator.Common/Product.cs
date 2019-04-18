using System;

namespace EconomicCalculator.Common
{
    public class Product
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public float PricePerUnit { get; set; }
        public ProductType ProductType { get; set; }
    }
}
