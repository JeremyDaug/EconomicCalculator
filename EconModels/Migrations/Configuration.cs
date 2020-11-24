namespace EconModels.Migrations
{
    using EconModels.ProductModel;
    using EconomicCalculator.Enums;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<EconSimContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(EconSimContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.

            var bioWaste = new Product
            {
                Name = "Bio Waste",
                UnitName = "kg",
                Quality = 1,
                DefaultPrice = 1.00M,
                Bulk = 1,
                ProductTypes = ProductTypes.ColdConsumable,
                Maintainable = false,
                Fractional = true,
                MeanTimeToFailure = -1,
                FailsInto = null,
                Maintenance = null,
            };
            var WheatGrain = new Product
            {
                Name = "Wheat Grain",
                UnitName = "kg",
                Quality = 2,
                DefaultPrice = 1.00M,
                Bulk = 1,
                ProductTypes = ProductTypes.ColdConsumable,
                Maintainable = false,
                Fractional = true,
                MeanTimeToFailure = 20,
            };

            context.Products.AddOrUpdate(x => x.Name, 
                bioWaste,
                WheatGrain
            );

            var WheatGrainFailsInto = new FailsIntoPair
            {
                Source = context.Products
                    .Single(x => x.Name == WheatGrain.Name),
                Result = context.Products
                    .Single(x => x.Name == bioWaste.Name),
                Amount = 1
            };

            //context.FailurePairs.Add(WheatGrainFailsInto);
        }
    }
}
