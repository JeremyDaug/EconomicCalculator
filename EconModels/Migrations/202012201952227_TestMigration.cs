namespace EconModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TestMigration : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.FailsIntoPairs", "SourceId", "dbo.Products");
            DropForeignKey("dbo.FailsIntoPairs", "ResultId", "dbo.Products");
            DropForeignKey("dbo.MaintenancePairs", "ResultId", "dbo.Products");
            DropForeignKey("dbo.MaintenancePairs", "SourceId", "dbo.Products");
            DropForeignKey("dbo.TerritoryConnections", "EndId", "dbo.Territories");
            DropForeignKey("dbo.TerritoryConnections", "StartId", "dbo.Territories");
            DropIndex("dbo.MaintenancePairs", "UniqueCoupling");
            RenameColumn(table: "dbo.MaintenancePairs", name: "ResultId", newName: "__mig_tmp__0");
            RenameColumn(table: "dbo.MaintenancePairs", name: "SourceId", newName: "ResultId");
            RenameColumn(table: "dbo.MaintenancePairs", name: "__mig_tmp__0", newName: "SourceId");
            CreateIndex("dbo.MaintenancePairs", new[] { "SourceId", "ResultId" }, unique: true, name: "UniqueCoupling");
            AddForeignKey("dbo.FailsIntoPairs", "SourceId", "dbo.Products", "Id", cascadeDelete: true);
            AddForeignKey("dbo.FailsIntoPairs", "ResultId", "dbo.Products", "Id", cascadeDelete: false);
            AddForeignKey("dbo.MaintenancePairs", "SourceId", "dbo.Products", "Id", cascadeDelete: false);
            AddForeignKey("dbo.MaintenancePairs", "ResultId", "dbo.Products", "Id", cascadeDelete: true);
            AddForeignKey("dbo.TerritoryConnections", "EndId", "dbo.Territories", "Id", cascadeDelete: false);
            AddForeignKey("dbo.TerritoryConnections", "StartId", "dbo.Territories", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TerritoryConnections", "StartId", "dbo.Territories");
            DropForeignKey("dbo.TerritoryConnections", "EndId", "dbo.Territories");
            DropForeignKey("dbo.MaintenancePairs", "ResultId", "dbo.Products");
            DropForeignKey("dbo.MaintenancePairs", "SourceId", "dbo.Products");
            DropForeignKey("dbo.FailsIntoPairs", "ResultId", "dbo.Products");
            DropForeignKey("dbo.FailsIntoPairs", "SourceId", "dbo.Products");
            DropIndex("dbo.MaintenancePairs", "UniqueCoupling");
            RenameColumn(table: "dbo.MaintenancePairs", name: "SourceId", newName: "__mig_tmp__0");
            RenameColumn(table: "dbo.MaintenancePairs", name: "ResultId", newName: "SourceId");
            RenameColumn(table: "dbo.MaintenancePairs", name: "__mig_tmp__0", newName: "ResultId");
            CreateIndex("dbo.MaintenancePairs", new[] { "SourceId", "ResultId" }, unique: true, name: "UniqueCoupling");
            AddForeignKey("dbo.TerritoryConnections", "StartId", "dbo.Territories", "Id");
            AddForeignKey("dbo.TerritoryConnections", "EndId", "dbo.Territories", "Id");
            AddForeignKey("dbo.MaintenancePairs", "SourceId", "dbo.Products", "Id");
            AddForeignKey("dbo.MaintenancePairs", "ResultId", "dbo.Products", "Id");
            AddForeignKey("dbo.FailsIntoPairs", "ResultId", "dbo.Products", "Id");
            AddForeignKey("dbo.FailsIntoPairs", "SourceId", "dbo.Products", "Id");
        }
    }
}
