namespace EconModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DBV1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CultureNeeds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NeedType = c.Int(nullable: false),
                        Culture_Id = c.Int(nullable: false),
                        Need_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Cultures", t => t.Culture_Id, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.Need_Id, cascadeDelete: true)
                .Index(t => t.Culture_Id)
                .Index(t => t.Need_Id);
            
            CreateTable(
                "dbo.Cultures",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CultureGrowthRate = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 30),
                        VariantName = c.String(maxLength: 30),
                        UnitName = c.String(nullable: false, maxLength: 15),
                        Quality = c.Int(nullable: false),
                        DefaultPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Mass = c.Double(nullable: false),
                        Bulk = c.Double(nullable: false),
                        ProductTypes = c.Int(nullable: false),
                        Maintainable = c.Boolean(nullable: false),
                        Fractional = c.Boolean(nullable: false),
                        MeanTimeToFailure = c.Int(nullable: false),
                        Market_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Markets", t => t.Market_Id)
                .Index(t => t.Market_Id);
            
            CreateTable(
                "dbo.FailsIntoPairs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SourceId = c.Int(nullable: false),
                        ResultId = c.Int(nullable: false),
                        Amount = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ResultId)
                .ForeignKey("dbo.Products", t => t.SourceId)
                .Index(t => new { t.SourceId, t.ResultId }, unique: true, name: "UniqueCoupling");
            
            CreateTable(
                "dbo.MaintenancePairs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SourceId = c.Int(nullable: false),
                        ResultId = c.Int(nullable: false),
                        Amount = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ResultId)
                .ForeignKey("dbo.Products", t => t.SourceId)
                .Index(t => new { t.SourceId, t.ResultId }, unique: true, name: "UniqueCoupling");
            
            CreateTable(
                "dbo.Jobs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 30),
                        JobType = c.Int(nullable: false),
                        JobCategory = c.Int(nullable: false),
                        SkillName = c.String(nullable: false, maxLength: 30),
                        SkillLevel = c.Int(nullable: false),
                        LaborRequirements = c.Double(nullable: false),
                        Process_Id = c.Int(nullable: false),
                        Job_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Processes", t => t.Process_Id, cascadeDelete: true)
                .ForeignKey("dbo.Jobs", t => t.Job_Id)
                .Index(t => t.Process_Id)
                .Index(t => t.Job_Id);
            
            CreateTable(
                "dbo.Processes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 30),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProcessCapitals",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ParentId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Amount = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Processes", t => t.ParentId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => new { t.ParentId, t.ProductId }, unique: true, name: "UniqueCoupling");
            
            CreateTable(
                "dbo.ProcessInputs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ParentId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Amount = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Processes", t => t.ParentId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => new { t.ParentId, t.ProductId }, unique: true, name: "UniqueCoupling");
            
            CreateTable(
                "dbo.ProcessOutputs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ParentId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Amount = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Processes", t => t.ParentId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => new { t.ParentId, t.ProductId }, unique: true, name: "UniqueCoupling");
            
            CreateTable(
                "dbo.LandOwners",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Owner_Id = c.Int(nullable: false),
                        Territory_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PopulationGroups", t => t.Owner_Id, cascadeDelete: true)
                .ForeignKey("dbo.Territories", t => t.Territory_Id, cascadeDelete: true)
                .Index(t => t.Owner_Id)
                .Index(t => t.Territory_Id);
            
            CreateTable(
                "dbo.PopulationGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 30),
                        Count = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SkillName = c.String(nullable: false, maxLength: 30),
                        SkillLevel = c.Int(nullable: false),
                        PrimaryJobId = c.Int(nullable: false),
                        Priority = c.Int(nullable: false),
                        Market_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Jobs", t => t.PrimaryJobId, cascadeDelete: true)
                .ForeignKey("dbo.Markets", t => t.Market_Id)
                .Index(t => t.PrimaryJobId)
                .Index(t => t.Market_Id);
            
            CreateTable(
                "dbo.PopulationCultureBreakdowns",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Amount = c.Double(nullable: false),
                        Culture_Id = c.Int(nullable: false),
                        Parent_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Cultures", t => t.Culture_Id, cascadeDelete: true)
                .ForeignKey("dbo.PopulationGroups", t => t.Parent_Id, cascadeDelete: true)
                .Index(t => t.Culture_Id)
                .Index(t => t.Parent_Id);
            
            CreateTable(
                "dbo.OwnedProperties",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Owner_Id = c.Int(nullable: false),
                        Product_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PopulationGroups", t => t.Owner_Id, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.Product_Id, cascadeDelete: true)
                .Index(t => t.Owner_Id)
                .Index(t => t.Product_Id);
            
            CreateTable(
                "dbo.Territories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 40),
                        X = c.Int(),
                        Y = c.Int(),
                        Z = c.Int(),
                        Extent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Elevation = c.Int(nullable: false),
                        WaterLevel = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HasRiver = c.Boolean(nullable: false),
                        Humidity = c.Int(nullable: false),
                        Tempurature = c.Int(nullable: false),
                        Roughness = c.Int(nullable: false),
                        InfrastructureLevel = c.Int(nullable: false),
                        AvailableLand = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TerritoryConnections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StartId = c.Int(nullable: false),
                        EndId = c.Int(nullable: false),
                        Distance = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Territories", t => t.EndId)
                .ForeignKey("dbo.Territories", t => t.StartId)
                .Index(t => new { t.StartId, t.EndId }, unique: true, name: "UniqueCoupling");
            
            CreateTable(
                "dbo.ProductPrices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MarketId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        MarketPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Markets", t => t.MarketId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => new { t.MarketId, t.ProductId }, unique: true, name: "UniqueCoupling");
            
            CreateTable(
                "dbo.Markets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 40),
                        Territory_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Territories", t => t.Territory_Id)
                .Index(t => t.Territory_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductPrices", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Products", "Market_Id", "dbo.Markets");
            DropForeignKey("dbo.Markets", "Territory_Id", "dbo.Territories");
            DropForeignKey("dbo.ProductPrices", "MarketId", "dbo.Markets");
            DropForeignKey("dbo.PopulationGroups", "Market_Id", "dbo.Markets");
            DropForeignKey("dbo.LandOwners", "Territory_Id", "dbo.Territories");
            DropForeignKey("dbo.TerritoryConnections", "StartId", "dbo.Territories");
            DropForeignKey("dbo.TerritoryConnections", "EndId", "dbo.Territories");
            DropForeignKey("dbo.LandOwners", "Owner_Id", "dbo.PopulationGroups");
            DropForeignKey("dbo.PopulationGroups", "PrimaryJobId", "dbo.Jobs");
            DropForeignKey("dbo.OwnedProperties", "Product_Id", "dbo.Products");
            DropForeignKey("dbo.OwnedProperties", "Owner_Id", "dbo.PopulationGroups");
            DropForeignKey("dbo.PopulationCultureBreakdowns", "Parent_Id", "dbo.PopulationGroups");
            DropForeignKey("dbo.PopulationCultureBreakdowns", "Culture_Id", "dbo.Cultures");
            DropForeignKey("dbo.Jobs", "Job_Id", "dbo.Jobs");
            DropForeignKey("dbo.Jobs", "Process_Id", "dbo.Processes");
            DropForeignKey("dbo.ProcessOutputs", "ProductId", "dbo.Products");
            DropForeignKey("dbo.ProcessOutputs", "ParentId", "dbo.Processes");
            DropForeignKey("dbo.ProcessInputs", "ProductId", "dbo.Products");
            DropForeignKey("dbo.ProcessInputs", "ParentId", "dbo.Processes");
            DropForeignKey("dbo.ProcessCapitals", "ProductId", "dbo.Products");
            DropForeignKey("dbo.ProcessCapitals", "ParentId", "dbo.Processes");
            DropForeignKey("dbo.CultureNeeds", "Need_Id", "dbo.Products");
            DropForeignKey("dbo.MaintenancePairs", "SourceId", "dbo.Products");
            DropForeignKey("dbo.MaintenancePairs", "ResultId", "dbo.Products");
            DropForeignKey("dbo.FailsIntoPairs", "SourceId", "dbo.Products");
            DropForeignKey("dbo.FailsIntoPairs", "ResultId", "dbo.Products");
            DropForeignKey("dbo.CultureNeeds", "Culture_Id", "dbo.Cultures");
            DropIndex("dbo.Markets", new[] { "Territory_Id" });
            DropIndex("dbo.ProductPrices", "UniqueCoupling");
            DropIndex("dbo.TerritoryConnections", "UniqueCoupling");
            DropIndex("dbo.OwnedProperties", new[] { "Product_Id" });
            DropIndex("dbo.OwnedProperties", new[] { "Owner_Id" });
            DropIndex("dbo.PopulationCultureBreakdowns", new[] { "Parent_Id" });
            DropIndex("dbo.PopulationCultureBreakdowns", new[] { "Culture_Id" });
            DropIndex("dbo.PopulationGroups", new[] { "Market_Id" });
            DropIndex("dbo.PopulationGroups", new[] { "PrimaryJobId" });
            DropIndex("dbo.LandOwners", new[] { "Territory_Id" });
            DropIndex("dbo.LandOwners", new[] { "Owner_Id" });
            DropIndex("dbo.ProcessOutputs", "UniqueCoupling");
            DropIndex("dbo.ProcessInputs", "UniqueCoupling");
            DropIndex("dbo.ProcessCapitals", "UniqueCoupling");
            DropIndex("dbo.Jobs", new[] { "Job_Id" });
            DropIndex("dbo.Jobs", new[] { "Process_Id" });
            DropIndex("dbo.MaintenancePairs", "UniqueCoupling");
            DropIndex("dbo.FailsIntoPairs", "UniqueCoupling");
            DropIndex("dbo.Products", new[] { "Market_Id" });
            DropIndex("dbo.CultureNeeds", new[] { "Need_Id" });
            DropIndex("dbo.CultureNeeds", new[] { "Culture_Id" });
            DropTable("dbo.Markets");
            DropTable("dbo.ProductPrices");
            DropTable("dbo.TerritoryConnections");
            DropTable("dbo.Territories");
            DropTable("dbo.OwnedProperties");
            DropTable("dbo.PopulationCultureBreakdowns");
            DropTable("dbo.PopulationGroups");
            DropTable("dbo.LandOwners");
            DropTable("dbo.ProcessOutputs");
            DropTable("dbo.ProcessInputs");
            DropTable("dbo.ProcessCapitals");
            DropTable("dbo.Processes");
            DropTable("dbo.Jobs");
            DropTable("dbo.MaintenancePairs");
            DropTable("dbo.FailsIntoPairs");
            DropTable("dbo.Products");
            DropTable("dbo.Cultures");
            DropTable("dbo.CultureNeeds");
        }
    }
}
