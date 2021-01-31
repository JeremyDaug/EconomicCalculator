namespace EconModels.Migrations
{
    using EconModels.Enums;
    using EconModels.JobModels;
    using EconModels.MarketModel;
    using EconModels.PopulationModel;
    using EconModels.ProcessModel;
    using EconModels.ProductModel;
    using EconModels.TerritoryModel;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
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
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
            // To Clean out the database 
            //  run Update-Database -TargetMigration:0 | Update-Database -Force

            /* Sanity Notes:
             * When adding something that has a foreign key add the Id not the 
             * connection itself. IE, FailsIntoPairs.SourceId should be set, not
             * FailsIntoPairs.Source, that will create duplicate data.
            */
            #region Product
            // Biowaste
            var BioWaste = new Product
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
                Mass = 1,
                ProductTypes = ProductTypes.ColdConsumable,
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
                ProductTypes = ProductTypes.Currency,
                Maintainable = false,
                Fractional = true,
                MeanTimeToFailure = -1,
            };
            
            var IronOre = new Product
            {
                Name = "Iron Ore",
                UnitName = "Kg",
                Quality = 0,
                DefaultPrice = 3.0M,
                Bulk = 1,
                Mass = 1,
                ProductTypes = ProductTypes.Consumable,
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
                ProductTypes = ProductTypes.Consumable,
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
                ProductTypes = ProductTypes.CapitalGood,
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
                ProductTypes = ProductTypes.CapitalGood,
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
                ProductTypes = ProductTypes.CapitalGood,
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
                ProductTypes = ProductTypes.CapitalGood,
                Maintainable = false,
                Fractional = false,
                MeanTimeToFailure = 1800,
            };

            var products = new List<Product>
            {
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

            products
                .ForEach(
                    product => context.Products.AddOrUpdate(
                        x => new { x.Name, x.VariantName }, product));

            context.SaveChanges();
            var WheatGrainFailsInto = new FailsIntoPair
            {
                SourceId = WheatGrain.Id,
                ResultId = BioWaste.Id,
                Amount = 1
            };

            var FlourFailsInto = new FailsIntoPair
            {
                SourceId = Flour.Id,
                ResultId = BioWaste.Id,
                Amount = 1
            };

            var BreadFailsInto = new FailsIntoPair
            {
                SourceId = Bread.Id,
                ResultId = BioWaste.Id,
                Amount = 0.5
            };

            var IronScrap = new FailsIntoPair
            {
                SourceId = IronIngot.Id,
                ResultId = IronOre.Id,
                Amount = 2
            };

            var FarmingScrap = new FailsIntoPair
            {
                SourceId = FarmTools.Id,
                ResultId = IronOre.Id,
                Amount = 40
            };

            var MiningToolScrap = new FailsIntoPair
            {
                SourceId = MineTools.Id,
                ResultId = IronOre.Id,
                Amount = 40
            };

            // Milling Stones break into nothing just as they come from nothing.

            var BrokenOven = new FailsIntoPair
            {
                SourceId = Oven.Id,
                ResultId = IronOre.Id,
                Amount = 40
            };

            context.FailurePairs.AddOrUpdate(
                WheatGrainFailsInto,
                FlourFailsInto,
                BreadFailsInto,
                IronScrap,
                FarmingScrap,
                MiningToolScrap,
                BrokenOven);

            // Sanity check this update

            #endregion Product

            // Commented out below here to focus on products.

            #region Processes
            // Processes
            // Farming TODO recalculate for acreage yield standards
            // currently no input required for farming.
            // TODO Will require land input and allow land to modify result.
            // TODO think of how to add option for adding fertilizer
            // TODO figure out how to apply labor to this process.
            // TODO how to handle capital as throughput requirement.
            // Suggestion: Only allow capital to be used 1/ day, but
            //     allow fractions even if capital good is not fractional.
            // TODO figure out how to handle Processes below 1 input/output/capital needed.
            // Suggestion: Maybe allow for a process to happen in fractions so long as
            //     inputs and outputs fractionality is maintained.

            /* Farming
             * Input: (Seed) Grain (1 bushel, 27 Kg/Acre) (Labor not counted)
             * Capital: Farming Tools (1 set / 50 Acres) (land not included)
             * Output: Wheat Grain (8 bushels, 216 Kg/Acre)
             */
            #region FarmingProc
            var Farming = new Process
            {
                Name = "Wheat Farming",
            };
            // Seed Grain (labor not included)
            var FarmingInput = new ProcessInput
            {
                ProcessId = Farming.Id,
                InputId = WheatGrain.Id,
                Amount = 27 // equal to 1 bushel.
            };
            // Farming Implements (land not included yet)
            var FarmingCapital = new ProcessCapital
            {
                ProcessId = Farming.Id,
                CapitalId = FarmTools.Id,
                Amount = 0.02 // 1 set can cover 50 acres max
            };
            // Wheat Grain
            var FarmingOutput = new ProcessOutput
            {
                ProcessId = Farming.Id,
                OutputId = WheatGrain.Id,
                Amount = 27 * 8 // 8 fold growth as base.
            };
            Farming.Inputs.Add(FarmingInput);
            Farming.Capital.Add(FarmingCapital);
            Farming.Outputs.Add(FarmingOutput);
            #endregion FarmingProc
            /* Milling
             * Input: Grain (1 bushel)
             * Capital: Mill Stone (1 per 100 bushels per day)
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
                ProcessId = Milling.Id,
                InputId = WheatGrain.Id,
                Amount = 27 // input of 1 bushel
            };
            // Capital: Mill Stones
            var millingCapitalStone = new ProcessCapital
            {
                ProcessId = Milling.Id,
                CapitalId = MillStone.Id,
                Amount = 0.01 // a mill stone can handle 100 bushels a day
            };
            // Outputs: Flour and BioWaste
            var millingFlourOutput = new ProcessOutput
            {
                ProcessId = Milling.Id,
                OutputId = Flour.Id,
                Amount = 18.9 // a reduction of 30 %.
            };
            var millingBioWasteOutput = new ProcessOutput
            {
                ProcessId = Milling.Id,
                OutputId = BioWaste.Id,
                Amount = 8.1 // reducted material becomes waste.
            };
            Milling.Inputs.Add(millingInput);
            Milling.Outputs.Add(millingFlourOutput);
            Milling.Outputs.Add(millingBioWasteOutput);
            Milling.Capital.Add(millingCapitalStone);
            #endregion MillingProc
            // Baking Bread
            /* Input: Flour 0.5 kg (water, eggs, etc not included)
             *      Labor not included
             * Capital: Oven (20 Loaves / Oven / Day)
             * Output: Bread (1kg)
             */
            #region BakingProc
            var BakeBread = new Process
            {
                Name = "Bake Bread"
            };
            // Flour
            var BakingFlourInput = new ProcessInput
            {
                ProcessId = BakeBread.Id,
                InputId = Flour.Id,
                Amount = 0.5
            };
            // Oven
            var BakingOvenCapital = new ProcessCapital
            {
                ProcessId = BakeBread.Id,
                CapitalId = Oven.Id,
                Amount = 0.05 // 1 oven can make 20 loaves at a time.
            };
            // Bread
            var BakingBreadOutput = new ProcessOutput
            {
                ProcessId = BakeBread.Id,
                OutputId = Bread.Id,
                Amount = 1 // 1/2 KG of flour + magic creates 1 kg of bread
            };
            BakeBread.Inputs.Add(BakingFlourInput);
            BakeBread.Outputs.Add(BakingBreadOutput);
            BakeBread.Capital.Add(BakingOvenCapital);
            #endregion BakingProc
            /* Gold Mining
             * Inputs: Labor not included
             * Capital: Mining Tools (1 kg / set / day)
             *      Land not included
             * Output: 1 Kg gold ore nugget (per worker Day)
             */
            #region GoldMining
            var MineGold = new Process
            {
                Name = "Gold Mining",
            };
            // Labor not included
            // Mining Tools (land not included)
            var GoldMiningCapital = new ProcessCapital
            {
                ProcessId = MineGold.Id,
                CapitalId = MineTools.Id,
                Amount = 1
            };
            // Gold Ore
            var GoldMiningOutput = new ProcessOutput
            {
                ProcessId = MineGold.Id,
                OutputId = GoldOre.Id,
                Amount = 1
            };
            MineGold.Capital.Add(GoldMiningCapital);
            MineGold.Outputs.Add(GoldMiningOutput);
            #endregion GoldMining
            /* Iron Mining
             * Inputs: Labor not Included
             * Capital: Mining Tools (100 kg / set / day)
             * Output: Iron Ore 100 / day and set
             */
            #region IronMining
            var IronMining = new Process
            {
                Name = "Iron Mining"
            };
            // Labor not included
            // Mining Tools
            var MiningCapitalTools = new ProcessCapital
            {
                ProcessId = IronMining.Id,
                CapitalId = MineTools.Id,
                Amount = 0.01 // 1 per 100 kg mined
            };
            // Iron Ore
            var MiningOutputOre = new ProcessOutput
            {
                ProcessId = IronMining.Id,
                OutputId = IronOre.Id,
                Amount = 1 // 100 kg of ore per set of tools and labor day.
            };
            IronMining.Capital.Add(MiningCapitalTools);
            IronMining.Outputs.Add(MiningOutputOre);
            #endregion IronMining
            /* Iron Refining
             * Input: Iron Ore 1
             *      Labon not included
             * Capital: nothing (upgrade to smelter) 1 oven to 5 kg of input / day
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
                ProcessId = IronRefining.Id,
                InputId = IronOre.Id,
                Amount = 1
            };
            // labor not included
            // smelter not included
            // Iron Ingots
            var IronRefiningOutputIngot = new ProcessOutput
            {
                ProcessId = IronRefining.Id,
                OutputId = IronIngot.Id,
                Amount = 0.5
            };
            // Slag (not included)
            IronRefining.Inputs.Add(IronRefiningInputOre);
            IronRefining.Outputs.Add(IronRefiningOutputIngot);
            #endregion IronRefining
            /* Iron Crafting (farm tools)
             * Input: Iron Ingots 25 Kg
             *      Labor not included
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
                ProcessId = FarmToolCrafting.Id,
                InputId = IronIngot.Id,
                Amount = 25
            };
            //  labor not included
            // Forge Tools not included
            // farming tool set
            var FarmToolCraftOutputTool = new ProcessOutput
            {
                ProcessId = FarmToolCrafting.Id,
                OutputId = FarmTools.Id,
                Amount = 1
            };
            // scrap iron
            var FarmToolCraftOutputExcess = new ProcessOutput
            {
                ProcessId = FarmToolCrafting.Id,
                OutputId = IronIngot.Id,
                Amount = 5
            };
            FarmToolCrafting.Inputs.Add(FarmToolCraftInputIron);
            FarmToolCrafting.Outputs.Add(FarmToolCraftOutputTool);
            FarmToolCrafting.Outputs.Add(FarmToolCraftOutputExcess);
            #endregion FarmToolCrafting
            /* Iron Crafting (mine tools)
             * Input: Iron Ingots 25 Kg
             *      Labor not included
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
                ProcessId = MineToolCrafting.Id,
                InputId = IronIngot.Id,
                Amount = 25
            };
            //  labor not included
            // Forge Tools not included
            // farming tool set
            var MineToolCraftOutputTool = new ProcessOutput
            {
                ProcessId = MineToolCrafting.Id,
                OutputId = MineTools.Id,
                Amount = 1
            };
            // scrap iron
            var MineToolCraftOutputExcess = new ProcessOutput
            {
                ProcessId = MineToolCrafting.Id,
                OutputId = IronIngot.Id,
                Amount = 5
            };
            MineToolCrafting.Inputs.Add(MineToolCraftInputIron);
            MineToolCrafting.Outputs.Add(MineToolCraftOutputTool);
            MineToolCrafting.Outputs.Add(MineToolCraftOutputExcess);
            #endregion MineToolcrafting
            /* Iron Crafting (Ovens)
             * Input: Iron Ingots 30kg
             *      Labor not included
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
                ProcessId = OvenCrafting.Id,
                InputId = IronIngot.Id,
                Amount = 30
            };
            // Labor Not included
            // Forge Tools not included.
            // Oven 
            var OvenCraftOutputOven = new ProcessOutput
            {
                ProcessId = OvenCrafting.Id,
                OutputId = Oven.Id,
                Amount = 1
            };
            // Scrap Iron
            var OvenCraftOutputIron = new ProcessOutput
            {
                ProcessId = OvenCrafting.Id,
                OutputId = IronIngot.Id,
                Amount = 10
            };
            OvenCrafting.Inputs.Add(OvenCraftInputIron);
            OvenCrafting.Outputs.Add(OvenCraftOutputOven);
            OvenCrafting.Outputs.Add(OvenCraftOutputIron);
            #endregion OvenCrafting

            var processes = new List<Process>
            {
                Farming,
                Milling,
                BakeBread,
                MineGold,
                IronMining,
                IronRefining,
                FarmToolCrafting,
                MineToolCrafting,
                OvenCrafting
            };

            processes
                .ForEach(
                    process => context.Processes.AddOrUpdate(
                        x => new { x.Name }, process));
            
            #endregion Processes

            /*
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

            var goldMiner = new Job
            {
                Name = "Gold Miner",
                JobType = JobTypes.Mine,
                JobCategory = JobCategory.Miner,
                LaborRequirements = 1,
                Process = MineGold,
                SkillName = "Miner",
                SkillLevel = 1
            };

            context.Jobs.AddOrUpdate(
                wheatFarmer,
                grainMiller,
                baker,
                goldMiner);

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

            context.SaveChanges();

            base.Seed(context);
        }
    }
}
