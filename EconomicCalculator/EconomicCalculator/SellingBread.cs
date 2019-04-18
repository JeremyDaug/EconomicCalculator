using EconomicCalculator.Common;
using System;
using System.Collections.Generic;

namespace EconomicCalculator
{
    public class SellingBread
    {
        // from chapter 1 - bread

        public void ToMakeBread()
        {
            var wheat = new Product
            {
                Name = "Wheat",
                Id = Guid.NewGuid(),
                PricePerUnit = 0,
                ProductType = ProductType.Crop
            };

            var wheatCrop = new Crop
            {
                Name = "Wheat",
                CropType = CropType.Grain,
                Id = Guid.NewGuid(),
                SowingTime = "Spring",
                HarvestTime = "Autumn",
                GrowthTime = 180,
                Produces = new List<Product> { wheat },
                ProductionAmounts = new List<double> { 480 },
                ReseedProduct = wheat,
                ReseedAmount = 120,
                Spoilage = 0.25
            };

            // 240 lb = 480 * (0.125+0.125)-120
            // var remainder = wheat.SurplusAfterReseed;

            var peas = new Product
            {
                Name = "Peas",
                Id = Guid.NewGuid(),
                PricePerUnit = 0,
                ProductType = ProductType.Crop
            };

            // recovery crop peas
            var peaCrop = new Crop
            {
                Name = "Peas",
                CropType = CropType.Bush,
                Id = Guid.NewGuid(),
                SowingTime = "Spring",
                HarvestTime = "Autumn",
                GrowthTime = 180,
                Produces = new List<Product> { peas },
                ProductionAmounts = new List<double> { 300},
                ReseedProduct = wheat,
                ReseedAmount = 90,
                Spoilage = 0.25
            };

            var WheatFlour = new Product
            {
                Name = "Wheat Flour",
                Id = Guid.NewGuid(),
                PricePerUnit = 0,
                ProductType = ProductType.FoodProduct
            };

            var milling = new Process
            {
                InputsProducts = wheat,
                OutputProducts = WheatFlour,
                ProcessorsCut = 1/8,
                ProductionPerDay = 10_000
            };
        }
    }
}