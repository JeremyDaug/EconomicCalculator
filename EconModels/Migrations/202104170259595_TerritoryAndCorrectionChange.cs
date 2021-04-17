namespace EconModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TerritoryAndCorrectionChange : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.PopulationCultureBreakdowns", newName: "CultureBreakdowns");
            DropIndex("dbo.PopulationGroups", new[] { "PrimaryJobId" });
            AddColumn("dbo.PopulationGroups", "TerritoryId", c => c.Int(nullable: false));
            AddColumn("dbo.CultureBreakdowns", "Percent", c => c.Double(nullable: false));
            AddColumn("dbo.PoliticalBreakdowns", "Percent", c => c.Double(nullable: false));
            CreateIndex("dbo.PopulationGroups", new[] { "TerritoryId", "PrimaryJobId" }, unique: true);
            AddForeignKey("dbo.PopulationGroups", "TerritoryId", "dbo.Territories", "Id", cascadeDelete: false);
            DropColumn("dbo.CultureBreakdowns", "Amount");
            DropColumn("dbo.PoliticalBreakdowns", "Amount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PoliticalBreakdowns", "Amount", c => c.Double(nullable: false));
            AddColumn("dbo.CultureBreakdowns", "Amount", c => c.Double(nullable: false));
            DropForeignKey("dbo.PopulationGroups", "TerritoryId", "dbo.Territories");
            DropIndex("dbo.PopulationGroups", new[] { "TerritoryId", "PrimaryJobId" });
            DropColumn("dbo.PoliticalBreakdowns", "Percent");
            DropColumn("dbo.CultureBreakdowns", "Percent");
            DropColumn("dbo.PopulationGroups", "TerritoryId");
            CreateIndex("dbo.PopulationGroups", "PrimaryJobId");
            RenameTable(name: "dbo.CultureBreakdowns", newName: "PopulationCultureBreakdowns");
        }
    }
}
