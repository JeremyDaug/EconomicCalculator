namespace EconModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Planets : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PopulationGroups", "TerritoryId", "dbo.Territories");
            DropIndex("dbo.Territories", new[] { "Name" });
            CreateTable(
                "dbo.Planets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 30),
                        Dead = c.Boolean(nullable: false),
                        Mass = c.Double(nullable: false),
                        SurfaceArea = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AirPressure = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Tempurature = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HeadRegionId = c.Int(nullable: false),
                        NorthPoleId = c.Int(),
                        SouthPoleId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Regions", t => t.HeadRegionId, cascadeDelete: true)
                .ForeignKey("dbo.Territories", t => t.NorthPoleId)
                .ForeignKey("dbo.Territories", t => t.SouthPoleId)
                .Index(t => t.Name, unique: true)
                .Index(t => t.HeadRegionId)
                .Index(t => t.NorthPoleId)
                .Index(t => t.SouthPoleId);
            
            CreateTable(
                "dbo.Regions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 30),
                        Rank = c.Int(nullable: false),
                        ParentId = c.Int(),
                        PlanetId = c.Int(nullable: false),
                        TerritoryId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Regions", t => t.ParentId, cascadeDelete: true)
                .ForeignKey("dbo.Planets", t => t.PlanetId)
                .Index(t => new { t.Name, t.PlanetId }, unique: true)
                .Index(t => t.ParentId);
            
            CreateTable(
                "dbo.PlanetResources",
                c => new
                    {
                        PlanetId = c.Int(nullable: false),
                        ResourceId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => new { t.PlanetId, t.ResourceId })
                .ForeignKey("dbo.Products", t => t.ResourceId, cascadeDelete: true)
                .ForeignKey("dbo.Planets", t => t.PlanetId, cascadeDelete: true)
                .Index(t => t.PlanetId)
                .Index(t => t.ResourceId);
            
            CreateTable(
                "dbo.InfrastructureRequirements",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        Density = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Tag = c.String(),
                        IsDiscrete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => new { t.ProductId, t.Tag }, unique: true);
            
            AddColumn("dbo.Territories", "PlanetId", c => c.Int(nullable: false));
            AddColumn("dbo.Territories", "RegionId", c => c.Int(nullable: false));
            AddColumn("dbo.Territories", "Region_Id", c => c.Int());
            CreateIndex("dbo.Territories", new[] { "Name", "PlanetId" }, unique: true);
            CreateIndex("dbo.Territories", "Region_Id");
            AddForeignKey("dbo.Territories", "Region_Id", "dbo.Regions", "Id");
            AddForeignKey("dbo.Territories", "PlanetId", "dbo.Planets", "Id", cascadeDelete: true);
            AddForeignKey("dbo.PopulationGroups", "TerritoryId", "dbo.Territories", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PopulationGroups", "TerritoryId", "dbo.Territories");
            DropForeignKey("dbo.InfrastructureRequirements", "ProductId", "dbo.Products");
            DropForeignKey("dbo.PlanetResources", "PlanetId", "dbo.Planets");
            DropForeignKey("dbo.PlanetResources", "ResourceId", "dbo.Products");
            DropForeignKey("dbo.Territories", "PlanetId", "dbo.Planets");
            DropForeignKey("dbo.Planets", "SouthPoleId", "dbo.Territories");
            DropForeignKey("dbo.Regions", "PlanetId", "dbo.Planets");
            DropForeignKey("dbo.Planets", "NorthPoleId", "dbo.Territories");
            DropForeignKey("dbo.Planets", "HeadRegionId", "dbo.Regions");
            DropForeignKey("dbo.Territories", "Region_Id", "dbo.Regions");
            DropForeignKey("dbo.Regions", "ParentId", "dbo.Regions");
            DropIndex("dbo.InfrastructureRequirements", new[] { "ProductId", "Tag" });
            DropIndex("dbo.PlanetResources", new[] { "ResourceId" });
            DropIndex("dbo.PlanetResources", new[] { "PlanetId" });
            DropIndex("dbo.Regions", new[] { "ParentId" });
            DropIndex("dbo.Regions", new[] { "Name", "PlanetId" });
            DropIndex("dbo.Planets", new[] { "SouthPoleId" });
            DropIndex("dbo.Planets", new[] { "NorthPoleId" });
            DropIndex("dbo.Planets", new[] { "HeadRegionId" });
            DropIndex("dbo.Planets", new[] { "Name" });
            DropIndex("dbo.Territories", new[] { "Region_Id" });
            DropIndex("dbo.Territories", new[] { "Name", "PlanetId" });
            DropColumn("dbo.Territories", "Region_Id");
            DropColumn("dbo.Territories", "RegionId");
            DropColumn("dbo.Territories", "PlanetId");
            DropTable("dbo.InfrastructureRequirements");
            DropTable("dbo.PlanetResources");
            DropTable("dbo.Regions");
            DropTable("dbo.Planets");
            CreateIndex("dbo.Territories", "Name", unique: true);
            AddForeignKey("dbo.PopulationGroups", "TerritoryId", "dbo.Territories", "Id");
        }
    }
}
