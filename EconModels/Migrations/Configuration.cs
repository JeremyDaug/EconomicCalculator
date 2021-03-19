namespace EconModels.Migrations
{
    using EconModels.Enums;
    using EconModels.JobModels;
    using EconModels.MarketModel;
    using EconModels.ModelEnums;
    using EconModels.PopulationModel;
    using EconModels.ProcessModel;
    using EconModels.ProductModel;
    using EconModels.SkillsModel;
    using EconModels.TerritoryModel;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Validation;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<EconModels.EconSimContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(EconModels.EconSimContext context)
        {
            if (!System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Launch();

            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
            // To Clean out the database 
            //  run Update-Database -TargetMigration:0 | Update-Database -Force

            /* Sanity Notes:
             * When adding something that has a foreign key add the Id not the 
             * connection itself. IE, FailsIntoPairs.SourceId should be set, not
             * FailsIntoPairs.Source, that will create duplicate data.
             * For more info on how to use AddOrUpdate() Properly see
             * https://stackoverflow.com/questions/8550756/how-to-seed-data-with-many-to-may-relations-in-entity-framework-migrations
            */
            /* Better Sanity Notes:
             * As part of good setup of data classes, ensure that names are unique
             * indices via either [Index(IsUnique = true)] or fluentAPI via
             * modelBuilder.Entity<>()
             *     .HasIndex(x => new { x.Name, x.VariantName })
             *     .IsUnique();
             */
            #region Product
            // Types of labor
            var MenialLabor = new Product
            {
                Name = "Menial Labor",
                UnitName = "Work Day",
                Quality = 0, // For labor quality is the minimum required skill level
                DefaultPrice = 1.00M,
                Bulk = 0,
                Mass = 0,
                ProductType = ProductTypes.Service, // all labors are also services
                Fractional = true,
                Maintainable = false,
                MeanTimeToFailure = 0 // you cannot store labor directly.
            };
            var FarmWork = new Product
            {
                Name = "Farm Labor",
                UnitName = "Work Day",
                Quality = 1,
                DefaultPrice = 1.50M,
                Bulk = 0,
                Mass = 0,
                ProductType = ProductTypes.Service,
                Fractional = true,
                Maintainable = false,
                MeanTimeToFailure = 0
            };
            var MillWork = new Product
            {
                Name = "Mill Work",
                UnitName = "Work Day",
                Quality = 1,
                DefaultPrice = 1.50M,
                Bulk = 0,
                Mass = 0,
                ProductType = ProductTypes.Service,
                Fractional = true,
                Maintainable = false,
                MeanTimeToFailure = 0
            };
            var BakeWork = new Product
            {
                Name = "Baker Work",
                UnitName = "Work Day",
                Quality = 1,
                DefaultPrice = 1.50M,
                Bulk = 0,
                Mass = 0,
                ProductType = ProductTypes.Service,
                Fractional = true,
                Maintainable = false,
                MeanTimeToFailure = 0
            };
            var MineEngineer = new Product
            { // Mines are mostly menial labor.
                Name = "Mine Engineer",
                UnitName = "Work Day",
                Quality = 3,
                DefaultPrice = 1.50M,
                Bulk = 0,
                Mass = 0,
                ProductType = ProductTypes.Service,
                Fractional = true,
                Maintainable = false,
                MeanTimeToFailure = 0
            };
            var SmeltingWork = new Product
            {
                Name = "Forge Worker",
                UnitName = "Work Day",
                Quality = 1,
                DefaultPrice = 1.50M,
                Bulk = 0,
                Mass = 0,
                ProductType = ProductTypes.Service,
                Fractional = true,
                Maintainable = false,
                MeanTimeToFailure = 0
            };
            var BlackSmither = new Product
            {
                Name = "Black Smithing",
                UnitName = "Work Day",
                Quality = 1,
                DefaultPrice = 1.50M,
                Bulk = 0,
                Mass = 0,
                ProductType = ProductTypes.Service,
                Fractional = true,
                Maintainable = false,
                MeanTimeToFailure = 0
            };
            // Land, like Menial Labor should be fixed as a requirement. Though it
            // may be ignored for reasons.
            var Land = new Product
            {
                Name = "Plot", // plots are equal to 1/8 an acre.
                Bulk = double.PositiveInfinity, // land cannot be moved.
                Mass = 0, // it is without mass as artificial land has it's weight tied to the craft.
                DefaultPrice = 10.00M,
                Fractional = false, // Plots are the smallest unit.
                Maintainable = false, // Land needs no care to exist.
                ProductType = ProductTypes.Land,
                Quality = 0, // land quality is tied to territory.
                MeanTimeToFailure = -1, // land doesn't stop existing.
            };
            // Biowaste
            var BioWaste = new Product
            {
                Name = "Bio Waste",
                UnitName = "kg",
                Quality = 0,
                DefaultPrice = 1.00M,
                Bulk = 1,
                Mass = 1,
                ProductType = ProductTypes.Good,
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
                ProductType = ProductTypes.ColdConsumable,
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
                ProductType = ProductTypes.ColdConsumable,
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
                Mass = 1,
                ProductType = ProductTypes.ColdConsumable,
                Maintainable = false,
                Fractional = false,
                MeanTimeToFailure = 5,
            };
            // Simple currency
            var GoldOre = new Product
            {
                Name = "Gold Ore",
                UnitName = "nugget",
                Quality = 1,
                DefaultPrice = 1.0M,
                Bulk = 1,
                Mass = 1,
                ProductType = ProductTypes.Currency,
                Maintainable = false,
                Fractional = true,
                MeanTimeToFailure = -1,
            };
            // Simple Iron
            var IronOre = new Product
            {
                Name = "Iron Ore",
                UnitName = "Kg",
                Quality = 0,
                DefaultPrice = 3.0M,
                Bulk = 1,
                Mass = 1,
                ProductType = ProductTypes.Consumable,
                Maintainable = false,
                Fractional = true,
                MeanTimeToFailure = -1
            };
            var IronIngot = new Product
            {
                Name = "Iron Ingot",
                UnitName = "Kg",
                Quality = 0,
                DefaultPrice = 6.0M,
                Bulk = 1,
                Mass = 1,
                ProductType = ProductTypes.Consumable,
                Maintainable = false,
                Fractional = true,
                MeanTimeToFailure = 10000
            };
            // Captial For Processes
            // Currently 100% Iron
            var FarmTools = new Product
            {
                Name = "Farming Tools",
                UnitName = "Set",
                Quality = 1,
                DefaultPrice = 50.0M,
                Bulk = 2,
                Mass = 20,
                ProductType = ProductTypes.CapitalGood,
                Maintainable = true,
                Fractional = false,
                MeanTimeToFailure = 500,
            };
            // Currently 100% Iron
            var MineTools = new Product
            {
                Name = "Mining Tools",
                UnitName = "Set",
                Quality = 1,
                DefaultPrice = 50.0M,
                Bulk = 2,
                Mass = 20,
                ProductType = ProductTypes.CapitalGood,
                Maintainable = true,
                Fractional = false,
                MeanTimeToFailure = 500,
            };
            // Made by magic and random stones.
            var MillStone = new Product
            {
                Name = "Mill Stone",
                UnitName = "Set",
                Quality = 1,
                DefaultPrice = 100.0M,
                Bulk = 4,
                Mass = 30,
                ProductType = ProductTypes.CapitalGood,
                Maintainable = false,
                Fractional = false,
                MeanTimeToFailure = 400
            };
            var Oven = new Product
            {
                Name = "Oven",
                UnitName = "Oven",
                Quality = 1,
                DefaultPrice = 200.0M,
                Bulk = 4,
                Mass = 20,
                ProductType = ProductTypes.CapitalGood,
                Maintainable = false,
                Fractional = false,
                MeanTimeToFailure = 1800,
            };

            var products = new List<Product>
            {
                MenialLabor,
                FarmWork,
                MillWork,
                BakeWork,
                MineEngineer,
                SmeltingWork,
                BlackSmither,
                WheatGrain,
                BioWaste,
                Flour,
                Bread,
                GoldOre,
                IronOre,
                IronIngot,
                FarmTools,
                MineTools,
                MillStone,
                Oven
            };

            products.ForEach( 
                product => context
                    .Products
                    .AddOrUpdate(x => new { x.Name, x.VariantName }, product));

            context.SaveChanges();

            // Failure and Maintenance pairs.
            var WheatGrainFailsInto = new FailsIntoPair
            {
                SourceId = context.Products.Single(x => x.Name == WheatGrain.Name).Id,
                ResultId = context.Products.Single(x => x.Name == BioWaste.Name).Id,
                Amount = 1
            };
            var FlourFailsInto = new FailsIntoPair
            {
                SourceId = context.Products.Single(x => x.Name == Flour.Name).Id,
                ResultId = context.Products.Single(x => x.Name == BioWaste.Name).Id,
                Amount = 1
            };
            var BreadFailsInto = new FailsIntoPair
            {
                SourceId = context.Products.Single(x => x.Name == Bread.Name).Id,
                ResultId = context.Products.Single(x => x.Name == BioWaste.Name).Id,
                Amount = 0.5
            };
            var IronScrap = new FailsIntoPair
            {
                SourceId = context.Products.Single(x => x.Name == IronIngot.Name).Id,
                ResultId = context.Products.Single(x => x.Name == IronOre.Name).Id,
                Amount = 2
            };
            var FarmingScrap = new FailsIntoPair
            {
                SourceId = context.Products.Single(x => x.Name == FarmTools.Name).Id,
                ResultId = context.Products.Single(x => x.Name == IronOre.Name).Id,
                Amount = 40
            };
            var MiningToolScrap = new FailsIntoPair
            {
                SourceId = context.Products.Single(x => x.Name == MineTools.Name).Id,
                ResultId = context.Products.Single(x => x.Name == IronOre.Name).Id,
                Amount = 40
            };
            // Milling Stones break into nothing just as they come from nothing.
            var BrokenOven = new FailsIntoPair
            {
                SourceId = context.Products.Single(x => x.Name == Oven.Name).Id,
                ResultId = context.Products.Single(x => x.Name == IronOre.Name).Id,
                Amount = 30
            };
            var failurePairs = new List<FailsIntoPair>
            {
                WheatGrainFailsInto,
                FlourFailsInto,
                BreadFailsInto,
                IronScrap,
                FarmingScrap,
                MiningToolScrap,
                BrokenOven
            };
            // Clear old pairs and start from scratch, easier to ensure no
            // extras that way.
            foreach (var pair in context.FailurePairs)
            {
                context.FailurePairs.Remove(pair);
            }

            // Add the new stuff in.
            foreach (var pair in failurePairs)
            {
                context.FailurePairs.Add(pair);
            }

            #endregion Product

            // Commented out below here to focus on products.

            #region Processes
            
            // Processes
            // Farming TODO recalculate for acreage yield standards
            // currently no input required for farming.
            // TODO Will require land input and allow land to modify result.
            // TODO how to handle capital as throughput requirement.
            // Suggestion: Only allow capital to be used 1/ day, but
            //     allow fractions even if capital good is not fractional.
            // TODO figure out how to handle Processes below 1 input/output/capital needed.
            // Suggestion: Maybe allow for a process to happen in fractions so long as
            //     inputs and outputs fractionality is maintained.

            /* Farming
             * Input: (Seed) Grain (1 bushel, 27 Kg/Acre) 
             *        Menial Labor 1 day / acre to harvest
             *        Management 1 overseer / 10 acres
             *        Biowast 25 kg / day Optional
             * Capital: Farming Tools (1 set / 50 Acres) (land not included)
             *          Land Not Included
             * Output: Wheat Grain (8 bushels, 216 Kg/Acre)
             */
            #region FarmingProc
            // Land is not taken taken yet.
            var Farming = new Process
            {
                Name = "Wheat Farming"
            };
            // Seed Grain (labor not included)
            var FarmingInput = new ProcessInput
            {
                Process = Farming,
                InputId = WheatGrain.Id,
                Amount = 27, // equal to 1 bushel.
                Tag = ProcessGoodTag.Investment
            };
            var FarmingLaborInput = new ProcessInput
            {
                Process = Farming,
                InputId = MenialLabor.Id,
                Amount = 1, // 1 day manual labor per acre
                Tag = ProcessGoodTag.Fixed
            };
            var FarmingManagementInput = new ProcessInput
            {
                Process = Farming,
                InputId = FarmWork.Id,
                Amount = 0.1, // 1 overseer per 10 acres
                Tag = ProcessGoodTag.FixedOptional
            };
            var FarmingFertilizer = new ProcessInput
            {
                Process = Farming,
                InputId = BioWaste.Id,
                Amount = 25,
                Tag = ProcessGoodTag.Optional
            };
            // Farming Implements (land not included yet)
            var FarmingCapital = new ProcessCapital
            {
                Process = Farming,
                CapitalId = FarmTools.Id,
                Tag = ProcessGoodTag.Required,
                Amount = 0.02 // 1 set can cover 50 acres max
            };
            // Wheat Grain
            var FarmingOutput = new ProcessOutput
            {
                Process = Farming,
                OutputId = WheatGrain.Id,
                Amount = 27 * 8 // 8 fold growth as base.
            };
            Farming.Inputs.Add(FarmingInput);
            Farming.Inputs.Add(FarmingLaborInput);
            Farming.Inputs.Add(FarmingManagementInput);
            Farming.Inputs.Add(FarmingFertilizer);
            Farming.Capital.Add(FarmingCapital);
            Farming.Outputs.Add(FarmingOutput);

            #endregion FarmingProc
            /* Milling
             * Input: Grain (1 bushel, 27 kg) required
             *        Menial Labor 2 day per 100 bushels fixed
             *        Mill Manager 1 day per 100 bushels fixed
             * Capital: Mill Stone (1 per 100 bushels per day) Required
             *          Land Not Included
             * Output: Flour 18.9 kg (30% weight reduction)
             *      BioWaste 8.1 kg
             */
            #region MillingProc
            var Milling = new Process
            {
                Name = "Wheat Milling",
            };
            // Grain Input
            var millingInput = new ProcessInput
            {
                Process = Milling,
                InputId = WheatGrain.Id,
                Amount = 27, // input of 1 bushel
                Tag = ProcessGoodTag.Required
            };
            var millingLaborInput = new ProcessInput
            {
                Process = Milling,
                InputId = MenialLabor.Id,
                Amount = 0.02, // 2 man can do 270 bushels a day.
                Tag = ProcessGoodTag.Fixed
            };
            var millingManagementLabor = new ProcessInput
            {
                Process = Milling,
                InputId = MillWork.Id,
                Amount = 0.01, // 1 manager can manage 1 Mill a day.
                Tag = ProcessGoodTag.Fixed
            };
            // Capital: Mill Stones
            var millingCapitalStone = new ProcessCapital
            {
                Process = Milling,
                CapitalId = MillStone.Id,
                Tag = ProcessGoodTag.Required,
                Amount = 0.01 // a mill stone can handle 100 bushels a day
            };
            // Outputs: Flour and BioWaste
            var millingFlourOutput = new ProcessOutput
            {
                Process = Milling,
                OutputId = Flour.Id,
                Amount = 18.9 // a reduction of 30 %.
            };
            var millingBioWasteOutput = new ProcessOutput
            {
                Process = Milling,
                OutputId = BioWaste.Id,
                Amount = 8.1 // reducted material becomes waste.
            };
            Milling.Inputs.Add(millingInput);
            Milling.Inputs.Add(millingLaborInput);
            Milling.Inputs.Add(millingManagementLabor);
            Milling.Outputs.Add(millingFlourOutput);
            Milling.Outputs.Add(millingBioWasteOutput);
            Milling.Capital.Add(millingCapitalStone);
            #endregion MillingProc
            /* Millstone Finding (to create a source of mill stones)
             * Inputs: Menial Labor (to find and return) 10 
             * Capital: None
             * Output: Millstone 1 / 10 days of labor
             */
            #region FindMillstone
            var FindMillstone = new Process
            {
                Name = "Find Millstone"
            };
            // Input Labor
            var FindStoneInput = new ProcessInput
            {
                Process = FindMillstone,
                InputId = MenialLabor.Id,
                Amount = 10,
                Tag = ProcessGoodTag.Fixed
            };
            // No Capital
            // Output Millstone
            var MillstoneFound = new ProcessOutput
            {
                Process = FindMillstone,
                OutputId = MillStone.Id,
                Amount = 1
            };
            FindMillstone.Inputs.Add(FindStoneInput);
            FindMillstone.Outputs.Add(MillstoneFound);
            #endregion FindMillstone
            // Baking Bread
            /* Input: Flour 0.5 kg (water, eggs, etc not included)
             *        Baker Labor 0.05 baker per loaf
             * Capital: Oven 0.05 per loaf 
             *          Land Not Included
             * Output: Bread (1kg, 20 loaves / oven*Baker / day)
             */
            #region BakingProc
            var BakeBread = new Process
            {
                Name = "Bake Bread"
            };
            // Flour
            var BakingFlourInput = new ProcessInput
            {
                Process = BakeBread,
                InputId = Flour.Id,
                Tag = ProcessGoodTag.Required,
                Amount = 0.5
            };
            var BakerLabor = new ProcessInput
            {
                Process = BakeBread,
                InputId = BakeWork.Id,
                Amount = 0.05,
                Tag = ProcessGoodTag.Fixed
            };
            // Oven
            var BakingOvenCapital = new ProcessCapital
            {
                Process = BakeBread,
                CapitalId = Oven.Id,
                Amount = 0.05, // 1 oven can make 20 loaves at a time.
                Tag = ProcessGoodTag.Required
            };
            // Bread
            var BakingBreadOutput = new ProcessOutput
            {
                Process = BakeBread,
                OutputId = Bread.Id,
                Amount = 1 // 1/2 KG of flour + magic creates 1 kg of bread
            };
            BakeBread.Inputs.Add(BakingFlourInput);
            BakeBread.Inputs.Add(BakerLabor);
            BakeBread.Outputs.Add(BakingBreadOutput);
            BakeBread.Capital.Add(BakingOvenCapital);
            #endregion BakingProc
            /* Gold Mining
             * Inputs: 1 day Menial Labor
             *         Mine Engineer 0.01 per laborer optional
             * Capital: Mining Tools (1 kg / set / day)
             *      Land not included
             * Output: 1 Kg gold ore nugget (per worker Day)
             */
            #region GoldMining
            var MineGold = new Process
            {
                Name = "Gold Mining",
            };
            // Labor
            var GoldMineLabor = new ProcessInput
            {
                Process = MineGold,
                InputId = MenialLabor.Id,
                Amount = 1,
                Tag = ProcessGoodTag.Fixed
            };
            var GoldMineEngineer = new ProcessInput
            {
                Process = MineGold,
                InputId = MineEngineer.Id,
                Amount = 0.01, // 1 engineer per 100 laborers
                Tag = ProcessGoodTag.FixedOptional
            };
            // Mining Tools (land not included)
            var GoldMiningCapital = new ProcessCapital
            {
                Process = MineGold,
                CapitalId = MineTools.Id,
                Amount = 1
            };
            // Gold Ore
            var GoldMiningOutput = new ProcessOutput
            {
                Process = MineGold,
                OutputId = GoldOre.Id,
                Amount = 1
            };
            MineGold.Inputs.Add(GoldMineLabor);
            MineGold.Inputs.Add(GoldMineEngineer);
            MineGold.Capital.Add(GoldMiningCapital);
            MineGold.Outputs.Add(GoldMiningOutput);
            #endregion GoldMining
            /* Iron Mining
             * Inputs: Menial Labor 1 day 
             *         Mine Engineer 1 per 100 laborers Optional
             * Capital: Mining Tools (100 kg / set / day)
             * Output: Iron Ore 100 / day and set
             */
            #region IronMining
            var IronMining = new Process
            {
                Name = "Iron Mining"
            };
            // Labor
            var IronMineLabor = new ProcessInput
            {
                Process = IronMining,
                InputId = MenialLabor.Id,
                Amount = 1,
                Tag = ProcessGoodTag.Fixed
            };
            var IronMineEngineer = new ProcessInput
            {
                Process = IronMining,
                InputId = MineEngineer.Id,
                Amount = 0.01,
                Tag = ProcessGoodTag.FixedOptional
            };
            // Mining Tools
            var MiningCapitalTools = new ProcessCapital
            {
                Process = IronMining,
                CapitalId = MineTools.Id,
                Amount = 0.01, // 1 per 100 kg mined
                Tag = ProcessGoodTag.Required
            };
            // Iron Ore
            var MiningOutputOre = new ProcessOutput
            {
                Process = IronMining,
                OutputId = IronOre.Id,
                Amount = 1 // 100 kg of ore per set of tools and labor day.
            };
            IronMining.Inputs.Add(IronMineLabor);
            IronMining.Inputs.Add(IronMineEngineer);
            IronMining.Capital.Add(MiningCapitalTools);
            IronMining.Outputs.Add(MiningOutputOre);
            #endregion IronMining
            /* Iron Refining
             * Input: Iron Ore 1 kg
             *        Menial Laborer 0.001 (1 per 1000 kg of ore)
             *        Forge Worker 0.001 (1 per 1000 kg of ore)
             * Capital: (upgrade to smelter) 1 oven to 50 kg of input / day
             * Output: Iron Ingot 0.5
             *      Slag (not included)
             */
            #region IronRefining
            var IronRefining = new Process
            {
                Name = "Iron Refining",
            };
            // Iron Ore
            var IronRefiningInputOre = new ProcessInput
            {
                Process = IronRefining,
                InputId = IronOre.Id,
                Amount = 1,
                Tag = ProcessGoodTag.Required
            };
            // labor
            var IronForgeLabor = new ProcessInput
            {
                Process = IronRefining,
                InputId = MenialLabor.Id,
                Amount = 0.001,
                Tag = ProcessGoodTag.Fixed
            };
            var IronRefiningLabor = new ProcessInput
            {
                Process = IronRefining,
                InputId = SmeltingWork.Id,
                Amount = 0.05,
                Tag = ProcessGoodTag.Fixed
            };
            // smelter (currently just oven)
            var SmeltingOven = new ProcessCapital
            {
                Process = IronRefining,
                CapitalId = Oven.Id,
                Amount = 0.02,
                Tag = ProcessGoodTag.Required
            };
            // Iron Ingots
            var IronRefiningOutputIngot = new ProcessOutput
            {
                Process = IronRefining,
                OutputId = IronIngot.Id,
                Amount = 0.5
            };
            // Slag (not included)
            IronRefining.Inputs.Add(IronRefiningInputOre);
            IronRefining.Inputs.Add(IronForgeLabor);
            IronRefining.Inputs.Add(IronRefiningLabor);
            IronRefining.Capital.Add(SmeltingOven);
            IronRefining.Outputs.Add(IronRefiningOutputIngot);
            #endregion IronRefining
            /* Scrap Recycling
             * Input: Iron Scrap 1 kg
             *        Menial Laborer 0.001 (1 per 1000 kg of scrap)
             *        Forge Worker 0.001 (1 per 1000 kg of ore)
             * Capital: Upgrade to Smelter 1 oven to 50 kg of input / day
             * Output: Iron Ingot 0.5
             *      Slag (not included)
             */
            #region IronRecycling
            var IronRecycling = new Process
            {
                Name = "Iron Recycling",
            };
            // Iron Ore
            var IronScrapInput = new ProcessInput
            {
                Process = IronRecycling,
                InputId = IronOre.Id,
                Amount = 1,
                Tag = ProcessGoodTag.Required
            };
            // labor
            var IronRecyclingLabor = new ProcessInput
            {
                Process = IronRecycling,
                InputId = MenialLabor.Id,
                Amount = 0.001,
                Tag = ProcessGoodTag.Fixed
            };
            var IronRecyclingManagement = new ProcessInput
            {
                Process = IronRecycling,
                InputId = SmeltingWork.Id,
                Amount = 0.05,
                Tag = ProcessGoodTag.Fixed
            };
            // smelter (currently just oven)
            var RecyclingOven = new ProcessCapital
            {
                Process = IronRecycling,
                CapitalId = Oven.Id,
                Amount = 0.02,
                Tag = ProcessGoodTag.Required
            };
            // Iron Ingots
            var IronRecyclingOutputIngot = new ProcessOutput
            {
                Process = IronRecycling,
                OutputId = IronIngot.Id,
                Amount = 0.5
            };
            // Slag (not included)
            IronRecycling.Inputs.Add(IronScrapInput);
            IronRecycling.Inputs.Add(IronRecyclingLabor);
            IronRecycling.Inputs.Add(IronRecyclingManagement);
            IronRecycling.Capital.Add(RecyclingOven);
            IronRecycling.Outputs.Add(IronRecyclingOutputIngot);
            #endregion IronRecycling
            /* Iron Crafting (farm tools)
             * Input: Iron Ingots 25 Kg
             *        Black Smithing 0.25 (1 / 4 per day)
             * Capital: Nothing (we use bare hands to beat iron into shape.)
             * Outputs: 1 set of farming tools
             *      5 kg of scrap iron (iron ingot)
             */
            #region FarmToolCrafting
            var FarmToolCrafting = new Process
            {
                Name = "Farm Tool Crafting"
            };
            // Iron Ingots
            var FarmToolCraftInputIron = new ProcessInput
            {
                Process = FarmToolCrafting,
                InputId = IronIngot.Id,
                Amount = 25,
                Tag = ProcessGoodTag.Required
            };
            // Blacksmith Labor
            var FarmToolSmith = new ProcessInput
            {
                Process = FarmToolCrafting,
                InputId = BlackSmither.Id,
                Amount = 0.25,
                Tag = ProcessGoodTag.Fixed
            };
            // Forge Tools not included
            // farming tool set
            var FarmToolCraftOutputTool = new ProcessOutput
            {
                Process = FarmToolCrafting,
                OutputId = FarmTools.Id,
                Amount = 1
            };
            // scrap iron
            var FarmToolCraftOutputExcess = new ProcessOutput
            {
                Process = FarmToolCrafting,
                OutputId = IronIngot.Id,
                Amount = 5
            };
            FarmToolCrafting.Inputs.Add(FarmToolCraftInputIron);
            FarmToolCrafting.Inputs.Add(FarmToolSmith);
            FarmToolCrafting.Outputs.Add(FarmToolCraftOutputTool);
            FarmToolCrafting.Outputs.Add(FarmToolCraftOutputExcess);
            #endregion FarmToolCrafting
            /* Iron Crafting (mine tools)
             * Input: Iron Ingots 25 Kg
             *        Black Smithing 0.25 (1 / 4 per day)
             * Capital: Nothing (we use bare hands to beat iron into shape.)
             * Outputs: 1 set of Mining tools
             *      5 kg of scrap iron (iron ingot)
             */
            #region MineToolCrafting
            var MineToolCrafting = new Process
            {
                Name = "Mine Tool Crafting"
            };
            // Iron Ingots
            var MineToolCraftInputIron = new ProcessInput
            {
                Process = MineToolCrafting,
                InputId = IronIngot.Id,
                Amount = 25,
                Tag = ProcessGoodTag.Required
            };
            // Blacksmith Labor
            var MineToolSmith = new ProcessInput
            {
                Process = MineToolCrafting,
                InputId = BlackSmither.Id,
                Amount = 0.25,
                Tag = ProcessGoodTag.Fixed
            };
            // Forge Tools not included
            // farming tool set
            var MineToolCraftOutputTool = new ProcessOutput
            {
                Process = MineToolCrafting,
                OutputId = MineTools.Id,
                Amount = 1
            };
            // scrap iron
            var MineToolCraftOutputExcess = new ProcessOutput
            {
                Process = MineToolCrafting,
                OutputId = IronIngot.Id,
                Amount = 5
            };
            MineToolCrafting.Inputs.Add(MineToolCraftInputIron);
            MineToolCrafting.Inputs.Add(MineToolSmith);
            MineToolCrafting.Outputs.Add(MineToolCraftOutputTool);
            MineToolCrafting.Outputs.Add(MineToolCraftOutputExcess);
            #endregion MineToolcrafting
            /* Iron Crafting (Ovens)
             * Input: Iron Ingots 30kg
             *        Blacksmith 1 (1 day per oven)
             * Capital: Nothing (tools not included)
             * Output: 1 Oven
             *      10 kg of scrap iron (iron ingot)
            */
            #region OvenCrafting
            var OvenCrafting = new Process
            {
                Name = "Oven Crafting"
            };
            // iron Ingots
            var OvenCraftInputIron = new ProcessInput
            {
                Process = OvenCrafting,
                InputId = IronIngot.Id,
                Amount = 30,
                Tag = ProcessGoodTag.Required
            };
            // Blacksmith Labor
            var OvenSmithing = new ProcessInput
            {
                Process = OvenCrafting,
                InputId = BlackSmither.Id,
                Amount = 1,
                Tag = ProcessGoodTag.Fixed
            };
            // Forge Tools not included.
            // Oven 
            var OvenCraftOutputOven = new ProcessOutput
            {
                Process = OvenCrafting,
                OutputId = Oven.Id,
                Amount = 1
            };
            // Scrap Iron
            var OvenCraftOutputIron = new ProcessOutput
            {
                Process = OvenCrafting,
                OutputId = IronIngot.Id,
                Amount = 11
            };
            OvenCrafting.Inputs.Add(OvenCraftInputIron);
            OvenCrafting.Inputs.Add(OvenSmithing);
            OvenCrafting.Outputs.Add(OvenCraftOutputOven);
            OvenCrafting.Outputs.Add(OvenCraftOutputIron);
            #endregion OvenCrafting

            var processes = new List<Process>
            {
                Farming,
                Milling,
                FindMillstone,
                BakeBread,
                MineGold,
                IronMining,
                IronRefining,
                IronRecycling,
                FarmToolCrafting,
                MineToolCrafting,
                OvenCrafting
            };

            // Update the processes Themselves.
            processes
                .ForEach(
                    process => context.Processes.AddOrUpdate(
                        x => new { x.Name, x.VariantName }, process));

            context.SaveChanges();

            #endregion Processes

            /* Skills */
            #region Skills

            // General Farming
            var FarmSkill = new Skill
            {
                Name = "Agriculture",
                Max = 5,
                Min = 1,
                Desc = "The General Skill of knowing how to farm effectively."
            };
            FarmSkill.ValidLabors.Add(MenialLabor);
            FarmSkill.ValidLabors.Add(FarmWork);

            // Milling
            var MillSkill = new Skill
            {
                Name = "Milling",
                Max = 5,
                Min = 1,
                Desc = "The skill of running, and managing a flour mill."
            };
            MillSkill.ValidLabors.Add(MenialLabor);
            MillSkill.ValidLabors.Add(MillWork);

            // Baking
            var BakeSkill = new Skill
            {
                Name = "Cooking",
                Max = 5,
                Min = 1,
                Desc = "The ability to cook quality food."
            };
            BakeSkill.ValidLabors.Add(BakeWork);

            // Mine Engineering
            var MineEngineerSkill = new Skill
            {
                Name = "Mine Engineer",
                Max = 5,
                Min = 3,
                Desc = "The Skill of managing and exploiting mines more efficiently."
            };
            MineEngineerSkill.ValidLabors.Add(MineEngineer);

            // Forge work
            var SmeltingSkill = new Skill
            {
                Name = "Smelting",
                Max = 5,
                Min = 1,
                Desc = "The Skill of knowing how to smelt ores and scraps into ingots."
            };
            SmeltingSkill.ValidLabors.Add(MenialLabor);
            SmeltingSkill.ValidLabors.Add(SmeltingWork);

            // Smithing
            var BlacksmithSkill = new Skill
            {
                Name = "Blacksmithing",
                Max = 5,
                Min = 1,
                Desc = "The skill of working dark metals like iron and steel."
            };
            BlacksmithSkill.ValidLabors.Add(BlackSmither);
            BlacksmithSkill.ValidLabors.Add(MenialLabor);

            // Related Skills
            BlacksmithSkill.RelationChild.Add(SmeltingSkill);
            SmeltingSkill.RelationParent.Add(BlacksmithSkill);

            var AllSkills = new List<Skill>
            {
                FarmSkill,
                MillSkill,
                BakeSkill,
                MineEngineerSkill,
                SmeltingSkill,
                BlacksmithSkill
            };

            AllSkills.ForEach(
                skill => context.Skills.AddOrUpdate(
                    x => new { x.Name }, skill));

            context.SaveChanges();

            #endregion Skills

            #region Job

            // Wheat Farmer
            var wheatFarmer = new Job
            {
                Name = "Wheat Farmer",
                JobType = JobTypes.LongTerm,
                JobCategory = JobCategory.Farmer,
                SkillId = FarmSkill.Id,
                SkillLevel = 1
            };
            wheatFarmer.Labor.Add(context.Products.Single(x => x.Name == MenialLabor.Name));
            wheatFarmer.Labor.Add(context.Products.Single(x => x.Name == FarmWork.Name));
            wheatFarmer.Processes.Add(context.Processes.Single(x => x.Name == Farming.Name));

            // Grain Miller
            var grainMiller = new Job
            {
                Name = "Wheat Miller",
                JobType = JobTypes.Processing,
                JobCategory = JobCategory.Craftsman,
                SkillId = MillSkill.Id,
                SkillLevel = 1
            };
            grainMiller.Labor.Add(context.Products.Single(x => x.Name == MenialLabor.Name));
            grainMiller.Labor.Add(context.Products.Single(x => x.Name == MillWork.Name));
            grainMiller.Processes.Add(context.Processes.Single(x => x.Name == Milling.Name));
            grainMiller.Processes.Add(context.Processes.Single(x => x.Name == FindMillstone.Name));

            // Baker
            var baker = new Job
            {
                Name = "Wheat Baker",
                JobType = JobTypes.Processing,
                JobCategory = JobCategory.Craftsman,
                SkillId = BakeSkill.Id,
                SkillLevel = 1
            };
            baker.Labor.Add(context.Products.Single(x => x.Name == BakeWork.Name));
            baker.Processes.Add(context.Processes.Single(x => x.Name == BakeBread.Name));

            // Gold Miner
            var goldMiner = new Job
            {
                Name = "Gold Miner",
                JobType = JobTypes.Extraction,
                JobCategory = JobCategory.Miner,
                SkillId = MineEngineerSkill.Id,
                SkillLevel = 3
            };
            goldMiner.Labor.Add(context.Products.Single(x => x.Name == MenialLabor.Name));
            goldMiner.Labor.Add(context.Products.Single(x => x.Name == MineEngineer.Name));
            goldMiner.Processes.Add(context.Processes.Single(x => x.Name == MineGold.Name));

            // Iron Miner
            var ironMiner = new Job
            {
                Name = "Iron Miner",
                JobType = JobTypes.Extraction,
                JobCategory = JobCategory.Miner,
                SkillId = MineEngineerSkill.Id,
                SkillLevel = 3
            };
            ironMiner.Labor.Add(context.Products.Single(x => x.Name == MenialLabor.Name));
            ironMiner.Labor.Add(context.Products.Single(x => x.Name == MineEngineer.Name));
            ironMiner.Processes.Add(context.Processes.Single(x => x.Name == MineGold.Name));

            // Iron Smelter
            var ironSmelter = new Job
            {
                Name = "Iron Smelter",
                JobType = JobTypes.Processing,
                JobCategory = JobCategory.Craftsman,
                SkillId = SmeltingSkill.Id,
                SkillLevel = 1
            };
            ironSmelter.Labor.Add(context.Products.Single(x => x.Name == MenialLabor.Name));
            ironSmelter.Labor.Add(context.Products.Single(x => x.Name == SmeltingWork.Name));
            ironSmelter.Processes.Add(context.Processes.Single(x => x.Name == IronRefining.Name));
            ironSmelter.Processes.Add(context.Processes.Single(x => x.Name == IronRecycling.Name));

            // Black Smithing
            var BlackSmithing = new Job
            {
                Name = "Black Smithing",
                JobCategory = JobCategory.Craftsman,
                JobType = JobTypes.Crafter,
                SkillId = BlacksmithSkill.Id,
                SkillLevel = 1
            };
            BlackSmithing.Labor.Add(context.Products.Single(x => x.Name == BlackSmither.Name));
            BlackSmithing.Processes.Add(context.Processes.Single(x => x.Name == FarmToolCrafting.Name));
            BlackSmithing.Processes.Add(context.Processes.Single(x => x.Name == MineToolCrafting.Name));
            BlackSmithing.Processes.Add(context.Processes.Single(x => x.Name == OvenCrafting.Name));

            goldMiner.RelatedChild.Add(ironMiner);
            ironMiner.RelatedParent.Add(goldMiner);

            context.Jobs.AddOrUpdate(x => x.Name,
                wheatFarmer,
                grainMiller,
                baker,
                goldMiner,
                ironMiner,
                ironSmelter,
                BlackSmithing);

            context.SaveChanges();

            #endregion Job

            /*
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

            var miners = new PopulationGroup
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

            var minerCultureBreakdown = new PopulationCultureBreakdown
            {
                Parent = miners,
                Culture = HumanCulture,
                Amount = 100
            };

            farmers.CultureBreakdown.Add(farmerCultureBreakdown);
            millers.CultureBreakdown.Add(MillerCultureBreakdown);
            bakers.CultureBreakdown.Add(bakerCultureBreakdown);
            miners.CultureBreakdown.Add(minerCultureBreakdown);

            context.PopCultureBreakdowns.AddOrUpdate(
                farmerCultureBreakdown,
                MillerCultureBreakdown,
                bakerCultureBreakdown,
                minerCultureBreakdown);

            context.PopulationGroups.AddOrUpdate(
                farmers,
                millers,
                bakers,
                miners);

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
                AvailableLand = 664_900 // No land currently owned.
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

            var MineLand = new LandOwner
            {
                Owner = miners,
                Territory = Marketrea,
                Amount = 100
            };

            Marketrea.LandOwners.Add(FarmLand);
            Marketrea.LandOwners.Add(MineLand);

            context.Territories.AddOrUpdate(
                Marketrea,
                Sucktopia);

            context.TerritoryConnections.AddOrUpdate(
                MarkToSuck,
                SuckToMark);

            context.LandOwners.AddOrUpdate(
                FarmLand,
                MineLand);

            #endregion Territory

            #region Market

            var FirstMarket = new Market
            {
                Name = "Market of Marketrea",
                Territory = Marketrea
            };

            // pops in market
            FirstMarket.PopulationGroups.Add(farmers);
            FirstMarket.PopulationGroups.Add(millers);
            FirstMarket.PopulationGroups.Add(bakers);
            FirstMarket.PopulationGroups.Add(miners);

            // product prices
            var bioWastePrice = new ProductPrices
            {
                Market = FirstMarket,
                Product = bioWaste,
                MarketPrice = 0.5M
            };

            var WheatPrice = new ProductPrices
            {
                Market = FirstMarket,
                Product = WheatGrain,
                MarketPrice = 1
            };

            var FlourPrice = new ProductPrices
            {
                Market = FirstMarket,
                Product = Flour,
                MarketPrice = 2
            };

            var BreadPrice = new ProductPrices
            {
                Market = FirstMarket,
                Product = Bread,
                MarketPrice = 4
            };

            var GoldPrice = new ProductPrices
            {
                Market = FirstMarket,
                Product = GoldOre,
                MarketPrice = 1
            };

            // prices to market
            FirstMarket.ProductPrices.Add(bioWastePrice);
            FirstMarket.ProductPrices.Add(WheatPrice);
            FirstMarket.ProductPrices.Add(FlourPrice);
            FirstMarket.ProductPrices.Add(BreadPrice);

            context.Markets.AddOrUpdate(
                FirstMarket);
            context.MarketPrices.AddOrUpdate(
                bioWastePrice,
                WheatPrice,
                FlourPrice,
                BreadPrice,
                GoldPrice);

            #endregion Market
            */
            try
            {
                context.SaveChanges();

                base.Seed(context);
            }
            catch (DbEntityValidationException e)
            {
                Exception raise = e;
                foreach (var errors in e.EntityValidationErrors)
                {
                    foreach (var error in errors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            errors.Entry.Entity.ToString(),
                            error.ErrorMessage);
                        // raise a new exception nesting
                        // the current instance as inner exception
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }
        }
    }
}
