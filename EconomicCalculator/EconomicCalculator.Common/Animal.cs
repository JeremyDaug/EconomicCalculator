using System;

namespace EconomicCalculator.Common
{
    internal class Animal
    {
        public string Name { get; set; }

        public Guid Id { get; set; }

        public bool Game { get; set; }

        public ProductAmount FoodCrop { get; set; }

        public double LiveWeight { get; set; }

        public double CarcasWeight { get; set; }

        public int Lifespan { get; set; }

        public ProductAmount LiveProductsPerDay { get; set; }
    }
}
