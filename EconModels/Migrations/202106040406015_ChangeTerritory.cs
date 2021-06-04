namespace EconModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeTerritory : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PopulationGroups", "TerritoryId", "dbo.Territories");
            DropIndex("dbo.Territories", new[] { "Region_Id" });
            DropColumn("dbo.Territories", "RegionId");
            RenameColumn(table: "dbo.Territories", name: "Region_Id", newName: "RegionId");
            AddColumn("dbo.Territories", "WaterCoverage", c => c.Single(nullable: false));
            AddColumn("dbo.Territories", "WaterQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Territories", "HasRiver", c => c.Boolean(nullable: false));
            AddColumn("dbo.Territories", "ExploitationLevel", c => c.Int(nullable: false));
            AlterColumn("dbo.Territories", "RegionId", c => c.Int(nullable: false));
            CreateIndex("dbo.Territories", "RegionId");
            AddForeignKey("dbo.PopulationGroups", "TerritoryId", "dbo.Territories", "Id", cascadeDelete: true);
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
            DropForeignKey("dbo.PopulationGroups", "TerritoryId", "dbo.Territories");
            DropIndex("dbo.Territories", new[] { "RegionId" });
            AlterColumn("dbo.Territories", "RegionId", c => c.Int());
            DropColumn("dbo.Territories", "ExploitationLevel");
            DropColumn("dbo.Territories", "HasRiver");
            DropColumn("dbo.Territories", "WaterQuantity");
            DropColumn("dbo.Territories", "WaterCoverage");
            RenameColumn(table: "dbo.Territories", name: "RegionId", newName: "Region_Id");
            AddColumn("dbo.Territories", "RegionId", c => c.Int(nullable: false));
            CreateIndex("dbo.Territories", "Region_Id");
            AddForeignKey("dbo.PopulationGroups", "TerritoryId", "dbo.Territories", "Id");
        }
    }
}
