namespace EconModels.Migrations
{
    using EconModels.JobModels;
    using EconModels.PopulationModel;
    using EconModels.ProcessModel;
    using EconModels.ProductModel;
    using EconModels.TerritoryModel;
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

            #region Product
            var bioWaste = new Product
            {
                Name = "Bio Waste",
                UnitName = "kg",
                Quality = 0,
                DefaultPrice = 1.00M,
                Bulk = 1,
                Mass = 1,
                ProductTypes = ProductTypes.Good,
                Maintainable = false,
                Fractional = true,
                MeanTimeToFailure = -1,
            };
            var WheatGrain = new Product
            {
                Name = "Wheat Grain",
                UnitName = "kg",
                Quality = 1,
                DefaultPrice = 1.00M,
                Bulk = 1,
                Mass = 1,
                ProductTypes = ProductTypes.ColdConsumable,
                Maintainable = false,
                Fractional = true,
                MeanTimeToFailure = 50,
            };
            var Flour = new Product
            {
                Name = "Wheat Flour",
                UnitName = "kg",
                Quality = 1,
                DefaultPrice = 2.0M,
                Bulk = 1,
                Mass = 1,
                ProductTypes = ProductTypes.ColdConsumable,
                Maintainable = false,
                Fractional = true,
                MeanTimeToFailure = 10,
            };
            var Bread = new Product
            {
                Name = "Simple Bread",
                UnitName = "Loaf",
                Quality = 1,
                DefaultPrice = 4.0M,
                Bulk = 1,
                Mass = 0.5,
                ProductTypes = ProductTypes.ColdConsumable,
                Maintainable = false,
                Fractional = false,
                MeanTimeToFailure = 5,
            };

            var WheatGrainFailsInto = new FailsIntoPair
            {
                Source = WheatGrain,
                Result = bioWaste,
                Amount = 1
            };

            var FlourFailsInto = new FailsIntoPair
            {
                Source = Flour,
                Result = bioWaste,
                Amount = 1
            };

            var BreadFailsInto = new FailsIntoPair
            {
                Source = Bread,
                Result = bioWaste,
                Amount = 0.5
            };

            WheatGrain.FailsInto.Add(WheatGrainFailsInto);
            bioWaste.MadeFromFailure.Add(WheatGrainFailsInto);
            Flour.FailsInto.Add(FlourFailsInto);
            bioWaste.MadeFromFailure.Add(FlourFailsInto);
            Bread.FailsInto.Add(BreadFailsInto);
            bioWaste.MadeFromFailure.Add(BreadFailsInto);

            context.Products.AddOrUpdate(
                bioWaste,
                WheatGrain,
                Flour,
                Bread
            );

            context.FailurePairs.AddOrUpdate(
                WheatGrainFailsInto,
                FlourFailsInto,
                BreadFailsInto);

            #endregion Product

            #region Processes
            // Processes
            // Farming
            var Farming = new Process
            {
                Name = "Wheat Farming",
            };
            var FarmingOutput = new ProcessOutput
            {
                Parent = Farming,
                Product = Flour,
                Amount = 0.25
            };
            Farming.Outputs.Add(FarmingOutput);

            // Milling
            var Milling = new Process
            {
                Name = "Wheat Milling",
            };
            var millingInput = new ProcessInput
            {
                Parent = Milling,
                Product = WheatGrain,
                Amount = 1
            };
            var millingOutput = new ProcessOutput
            {
                Parent = Milling,
                Product = Flour,
                Amount = 0.25
            };
            Milling.Inputs.Add(millingInput);
            Milling.Outputs.Add(millingOutput);

            // Baking Bread
            var BakeBread = new Process
            {
                Name = "Bake Bread"
            };
            var BakingInput = new ProcessInput
            {
                Parent = BakeBread,
                Product = Flour,
                Amount = 0.5
            };
            var BakingOutput = new ProcessOutput
            {
                Parent = BakeBread,
                Product = Bread, 
                Amount = 1
            };
            BakeBread.Inputs.Add(BakingInput);
            BakeBread.Outputs.Add(BakingOutput);

            context.Processes.AddOrUpdate(
                Farming,
                Milling,
                BakeBread);

            context.ProcessInputs.AddOrUpdate(
                millingInput,
                BakingInput);

            context.ProcessOutputs.AddOrUpdate(
                FarmingOutput,
                millingOutput,
                BakingOutput);

            #endregion Processes

            #region Job

            var wheatFarmer = new Job
            {
                Name = "Wheat Farmer",
                JobType = JobTypes.Crop,
                JobCategory = JobCategory.Farmer,
                Process = Farming,
                SkillName = "Farmer",
                SkillLevel = 1,
                LaborRequirements = 0.001,
            };

            var grainMiller = new Job
            {
                Name = "Wheat Miller",
                JobType = JobTypes.Processing,
                JobCategory = JobCategory.Craftsman,
                LaborRequirements = 0.005,
                Process = Milling,
                SkillName = "Milling",
                SkillLevel = 1,
            };

            var baker = new Job
            {
                Name = "Bread Baking",
                JobType = JobTypes.Craft,
                JobCategory = JobCategory.Craftsman,
                LaborRequirements = 0.005,
                Process = BakeBread,
                SkillName = "Cooking",
                SkillLevel = 1,
            };

            context.Jobs.AddOrUpdate(
                wheatFarmer,
                grainMiller,
                baker);

            #endregion Job

            #region Culture

            var HumanCulture = new Culture
            {
                Name = "Human",
                CultureGrowthRate = 0.02,
            };

            var GrainNeed = new CultureNeeds
            {
                Culture = HumanCulture,
                Need = WheatGrain,
                NeedType = NeedType.Life,
                Amount = 1
            };

            var flourNeed = new CultureNeeds
            {
                Culture = HumanCulture,
                Need = Flour,
                NeedType = NeedType.Daily,
                Amount = 1
            };

            var breadNeed = new CultureNeeds
            {
                Culture = HumanCulture,
                Need = Bread,
                NeedType = NeedType.Luxury,
                Amount = 1
            };

            HumanCulture.CultureNeeds.Add(GrainNeed);
            HumanCulture.CultureNeeds.Add(flourNeed);
            HumanCulture.CultureNeeds.Add(breadNeed);

            context.CultureNeeds.AddOrUpdate(
                GrainNeed,
                flourNeed,
                breadNeed);

            context.Cultures.AddOrUpdate(
                HumanCulture);

            #endregion Culture

            #region Populations

            var farmers = new PopulationGroup
            {
                Name = "Farmers",
                Count = 100,
                PrimaryJob = wheatFarmer,
                SkillName = "Farmer",
                SkillLevel = 1,
                Priority = 1
            };

            var millers = new PopulationGroup
            {
                Name = "Millers",
                Count = 100,
                PrimaryJob = grainMiller,
                SkillName = "Milling",
                SkillLevel = 1,
                Priority = 1
            };

            var bakers = new PopulationGroup
            {
                Name = "Bakers",
                Count = 100,
                PrimaryJob = baker,
                SkillName = "Cooking",
                SkillLevel = 1,
                Priority = 1
            };

            var farmerCultureBreakdown = new PopulationCultureBreakdown
            {
                Parent = farmers,
                Culture = HumanCulture,
                Amount = 100
            };

            var MillerCultureBreakdown = new PopulationCultureBreakdown
            {
                Parent = millers,
                Culture = HumanCulture,
                Amount = 100
            };

            var bakerCultureBreakdown = new PopulationCultureBreakdown
            {
                Parent = bakers,
                Culture = HumanCulture,
                Amount = 100
            };

            farmers.CultureBreakdown.Add(farmerCultureBreakdown);
            millers.CultureBreakdown.Add(MillerCultureBreakdown);
            bakers.CultureBreakdown.Add(bakerCultureBreakdown);

            context.PopCultureBreakdowns.AddOrUpdate(
                farmerCultureBreakdown,
                MillerCultureBreakdown,
                bakerCultureBreakdown);

            context.PopulationGroups.AddOrUpdate(
                farmers,
                millers,
                bakers);

            // TODO consider adding storage to PopulationGroups, may not bother.

            #endregion Populations

            #region Territory

            var Marketrea = new TerritoryModel.Territory
            {
                Name = "Marketrea",
                X = 0,
                Y = 0,
                Z = 0,
                Extent = 665_000, // Hexagon 20 mi to a side
                Elevation = 0,
                WaterLevel = 0.1M,
                HasRiver = false,
                Humidity = 25,
                Tempurature = 22,
                Roughness = 1,
                InfrastructureLevel = 0,
                AvailableLand = 665_000 // No land currently owned.
            };

            // An undeveloped, unlived in tract of land
            var Sucktopia = new TerritoryModel.Territory
            {
                Name = "Sucktopia",
                X = 0,
                Y = 0,
                Z = 0,
                Extent = 665_000, // Hexagon 20 mi to a side
                Elevation = 0,
                WaterLevel = 0.1M,
                HasRiver = false,
                Humidity = 25,
                Tempurature = 22,
                Roughness = 1,
                InfrastructureLevel = 0,
                AvailableLand = 665_000 // No land currently owned.
            };

            var MarkToSuck = new TerritoryConnection
            {
                Start = Marketrea,
                End = Sucktopia
            };

            var SuckToMark = new TerritoryConnection
            {
                Start = Sucktopia,
                End = Marketrea
            };

            Marketrea.OutgoingConnections.Add(MarkToSuck);
            Sucktopia.OutgoingConnections.Add(SuckToMark);

            Marketrea.IncomingConnections.Add(SuckToMark);
            Sucktopia.IncomingConnections.Add(MarkToSuck);

            var FarmLand = new LandOwner
            {
                Owner = farmers,
                Territory = Marketrea,
                Amount = 100
            };

            Marketrea.LandOwners.Add(FarmLand);

            context.Territories.AddOrUpdate(
                Marketrea,
                Sucktopia);

            context.TerritoryConnections.AddOrUpdate(
                MarkToSuck,
                SuckToMark);

            context.LandOwners.AddOrUpdate(
                FarmLand);

            #endregion
        }
    }
}
