namespace EconModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PlanetUpdate : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PopulationGroups", "TerritoryId", "dbo.Territories");
            DropForeignKey("dbo.PopulationGroups", "Market_Id", "dbo.Markets");
            DropForeignKey("dbo.Markets", "Territory_Id", "dbo.Territories");
            DropIndex("dbo.PopulationGroups", new[] { "TerritoryId", "PrimaryJobId" });
            DropIndex("dbo.PopulationGroups", new[] { "Market_Id" });
            DropIndex("dbo.Territories", new[] { "Region_Id" });
            DropIndex("dbo.Markets", new[] { "Territory_Id" });
            DropColumn("dbo.Territories", "RegionId");
            RenameColumn(table: "dbo.Territories", name: "Region_Id", newName: "RegionId");
            RenameColumn(table: "dbo.PopulationGroups", name: "Market_Id", newName: "MarketId");
            RenameColumn(table: "dbo.Markets", name: "Territory_Id", newName: "TerritoryId");
            AddColumn("dbo.Territories", "WaterCoverage", c => c.Single(nullable: false));
            AddColumn("dbo.Territories", "WaterQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Territories", "HasRiver", c => c.Boolean(nullable: false));
            AddColumn("dbo.Territories", "ExploitationLevel", c => c.Int(nullable: false));
            AddColumn("dbo.Planets", "Seed", c => c.Int(nullable: false));
            AddColumn("dbo.Planets", "RowMin", c => c.Int(nullable: false));
            AddColumn("dbo.Planets", "RowMax", c => c.Int(nullable: false));
            AddColumn("dbo.Planets", "ColMin", c => c.Int(nullable: false));
            AddColumn("dbo.Planets", "ColMax", c => c.Int(nullable: false));
            AlterColumn("dbo.PopulationGroups", "MarketId", c => c.Int(nullable: false));
            AlterColumn("dbo.Territories", "RegionId", c => c.Int(nullable: false));
            AlterColumn("dbo.Markets", "Name", c => c.String(maxLength: 40));
            AlterColumn("dbo.Markets", "TerritoryId", c => c.Int(nullable: false));
            CreateIndex("dbo.PopulationGroups", new[] { "MarketId", "PrimaryJobId" }, unique: true);
            CreateIndex("dbo.Markets", "TerritoryId");
            CreateIndex("dbo.Territories", "RegionId");
            AddForeignKey("dbo.PopulationGroups", "MarketId", "dbo.Markets", "Id", cascadeDelete: false);
            AddForeignKey("dbo.Markets", "TerritoryId", "dbo.Territories", "Id", cascadeDelete: false);
            DropColumn("dbo.PopulationGroups", "TerritoryId");
            DropColumn("dbo.Territories", "WaterStorage");
            DropColumn("dbo.Territories", "WaterStorageSpace");
            DropColumn("dbo.Territories", "WaterInFlow");
            DropColumn("dbo.Territories", "WaterOutFlow");
            DropColumn("dbo.Territories", "AvailableLand");
            DropColumn("dbo.Regions", "TerritoryId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Regions", "TerritoryId", c => c.Int());
            AddColumn("dbo.Territories", "AvailableLand", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Territories", "WaterOutFlow", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Territories", "WaterInFlow", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Territories", "WaterStorageSpace", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Territories", "WaterStorage", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.PopulationGroups", "TerritoryId", c => c.Int(nullable: false));
            DropForeignKey("dbo.Markets", "TerritoryId", "dbo.Territories");
            DropForeignKey("dbo.PopulationGroups", "MarketId", "dbo.Markets");
            DropIndex("dbo.Territories", new[] { "RegionId" });
            DropIndex("dbo.Markets", new[] { "TerritoryId" });
            DropIndex("dbo.PopulationGroups", new[] { "MarketId", "PrimaryJobId" });
            AlterColumn("dbo.Markets", "TerritoryId", c => c.Int());
            AlterColumn("dbo.Markets", "Name", c => c.String(nullable: false, maxLength: 40));
            AlterColumn("dbo.Territories", "RegionId", c => c.Int());
            AlterColumn("dbo.PopulationGroups", "MarketId", c => c.Int());
            DropColumn("dbo.Planets", "ColMax");
            DropColumn("dbo.Planets", "ColMin");
            DropColumn("dbo.Planets", "RowMax");
            DropColumn("dbo.Planets", "RowMin");
            DropColumn("dbo.Planets", "Seed");
            DropColumn("dbo.Territories", "ExploitationLevel");
            DropColumn("dbo.Territories", "HasRiver");
            DropColumn("dbo.Territories", "WaterQuantity");
            DropColumn("dbo.Territories", "WaterCoverage");
            RenameColumn(table: "dbo.Markets", name: "TerritoryId", newName: "Territory_Id");
            RenameColumn(table: "dbo.PopulationGroups", name: "MarketId", newName: "Market_Id");
            RenameColumn(table: "dbo.Territories", name: "RegionId", newName: "Region_Id");
            AddColumn("dbo.Territories", "RegionId", c => c.Int(nullable: false));
            CreateIndex("dbo.Markets", "Territory_Id");
            CreateIndex("dbo.Territories", "Region_Id");
            CreateIndex("dbo.PopulationGroups", "Market_Id");
            CreateIndex("dbo.PopulationGroups", new[] { "TerritoryId", "PrimaryJobId" }, unique: true);
            AddForeignKey("dbo.Markets", "Territory_Id", "dbo.Territories", "Id");
            AddForeignKey("dbo.PopulationGroups", "Market_Id", "dbo.Markets", "Id");
            AddForeignKey("dbo.PopulationGroups", "TerritoryId", "dbo.Territories", "Id");
        }
    }
}
